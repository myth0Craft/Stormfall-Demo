using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CustomMaskRenderFeature : ScriptableRendererFeature
{
    [SerializeField] CustomMaskRenderFeatureSettings settings;
    [SerializeField] RenderPassEvent pass;
    CustomMaskRenderFeaturePass foregroundPass;
    private RTHandle handle;
    private RenderTexture tex;
    private Camera hiddenCam;
    private GameObject hiddenCamGO;
    private RenderTexture hiddenCamTex;

    private CameraDriver _driver;



    void SyncCamera(Camera hidden, Camera main)
    {
        hidden.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
        hidden.orthographic = main.orthographic;
        hidden.orthographicSize = main.orthographicSize;
        hidden.fieldOfView = main.fieldOfView;
        hidden.aspect = main.aspect;
        hidden.nearClipPlane = main.nearClipPlane;
        hidden.farClipPlane = main.farClipPlane;
        hidden.projectionMatrix = main.projectionMatrix;

        // Resize hidden cam RT if screen size changed
        if (hidden.targetTexture != null &&
            (hidden.targetTexture.width != main.pixelWidth ||
             hidden.targetTexture.height != main.pixelHeight))
        {
            hidden.targetTexture.Release();
            hidden.targetTexture.width = main.pixelWidth / settings.downscaling;
            hidden.targetTexture.height = main.pixelHeight / settings.downscaling;
            hidden.targetTexture.Create();
        }
    }
    /// <inheritdoc/>
    public override void Create()
    {

        var w = Mathf.Max(1, Screen.width / settings.downscaling);
        var h = Mathf.Max(1, Screen.height / settings.downscaling);
        // Only reallocate if truly necessary
        if (tex != null && tex.IsCreated() &&
            tex.width == w && tex.height == h)
        {
            // RT is fine, just recreate the pass
            if (foregroundPass != null)
            {
                foregroundPass.renderPassEvent = settings.renderBehindScene
                    ? RenderPassEvent.BeforeRenderingOpaques
                    : RenderPassEvent.AfterRenderingTransparents;
            }

            return;
        }

        // Full realloc
        handle?.Release();
        handle = null;
        if (tex != null) { tex.Release(); tex = null; }
        if (hiddenCamGO != null) { SafeDestroy(hiddenCamGO); hiddenCamGO = null; hiddenCam = null; }
        if (_driver != null) { SafeDestroy(_driver.gameObject); _driver = null; }


        

        var desc = new RenderTextureDescriptor(w, h, RenderTextureFormat.ARGB32, 0);

        desc.depthBufferBits = 0;
        desc.depthStencilFormat = GraphicsFormat.None;
        desc.msaaSamples = 1;
        desc.sRGB = true;
        tex = new RenderTexture(desc);
        tex.name = "__BlurRT";
        tex.Create();

        handle = RTHandles.Alloc(tex);



        foregroundPass = new CustomMaskRenderFeaturePass(settings, handle);
        foregroundPass.renderPassEvent = settings.renderBehindScene
            ? RenderPassEvent.BeforeRenderingOpaques
            : RenderPassEvent.AfterRenderingTransparents;

        foregroundPass.InvalidateRTInfo();

        hiddenCamGO = new GameObject("HiddenCamGO") { hideFlags = HideFlags.HideAndDontSave };
        hiddenCam = hiddenCamGO.AddComponent<Camera>();
        hiddenCam.cullingMask = settings.foregroundLayers;
        hiddenCam.depthTextureMode = DepthTextureMode.None;
        hiddenCam.targetTexture = tex;
        hiddenCam.forceIntoRenderTexture = true;
        hiddenCam.enabled = false;
        hiddenCam.clearFlags = CameraClearFlags.SolidColor;
        hiddenCam.backgroundColor = Color.clear;
        hiddenCam.allowHDR = false;
        hiddenCam.allowMSAA = false;
        hiddenCam.useOcclusionCulling = false;
        hiddenCam.allowDynamicResolution = false;

        var urpData = hiddenCam.gameObject.AddComponent<UniversalAdditionalCameraData>();
        urpData.SetRenderer(0);
        urpData.renderPostProcessing = false;
        urpData.renderShadows = false;
        urpData.requiresDepthTexture = false;
        urpData.requiresColorTexture = false;

        var driverGO = new GameObject("__CaptureDriver") { hideFlags = HideFlags.HideAndDontSave };
        _driver = driverGO.AddComponent<CameraDriver>();
        _driver.Setup(hiddenCam, this);

    }


    protected override void Dispose(bool disposing)
    {
        handle?.Release();
        handle = null;

        if (tex != null)
        {
            tex.Release();
            tex = null;
        }

        if (hiddenCamGO != null)
        {
            SafeDestroy(hiddenCamGO);

            hiddenCam = null;
        }
        if (_driver != null)
        {
            SafeDestroy(_driver?.gameObject);
            _driver = null;
        }

        if (hiddenCamTex != null)
        {
            hiddenCamTex.Release();
            hiddenCamTex = null;
        }
    }

    void SafeDestroy(UnityEngine.Object obj)
    {
        if (!obj) return;

        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    class CameraDriver : MonoBehaviour
    {
        Camera hidden;
        CustomMaskRenderFeature feature;
        private bool _isRendering = false;
        private int _lastRenderedFrame = -1;
        private CommandBuffer _clearCmd;

        public void Setup(Camera cam, CustomMaskRenderFeature f)
        {
            hidden = cam;
            feature = f;
            _clearCmd = new CommandBuffer { name = "ClearHiddenRT" };
            RenderPipelineManager.beginContextRendering -= OnBeginCameraRendering;
            RenderPipelineManager.beginContextRendering += OnBeginCameraRendering;
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginContextRendering -= OnBeginCameraRendering;
            _clearCmd?.Release();
            _clearCmd = null;
        }

        void OnBeginCameraRendering(ScriptableRenderContext ctx, List<Camera> cams)
        {
            if (_isRendering) return;
            if (hidden == null) return;
            if (cams.Contains(hidden)) return;
            if (feature.handle == null) return;
            if (hidden.targetTexture == null) return;
            //if (Time.frameCount == _lastRenderedFrame) return;
            _lastRenderedFrame = Time.frameCount;

            Camera main = null;
            foreach (var cam in cams)
            {
                if (cam.cameraType == CameraType.Game) { main = cam; break; }
            }
            if (main == null) return;

            hidden.cullingMask = feature.settings.foregroundLayers;
            feature.SyncCamera(hidden, main);

            // Reuse cached command buffer
            _clearCmd.Clear();
            _clearCmd.SetRenderTarget(hidden.targetTexture);
            _clearCmd.ClearRenderTarget(true, true, Color.clear);
            Graphics.ExecuteCommandBuffer(_clearCmd);

            _isRendering = true;
            try
            {
                var request = new UniversalRenderPipeline.SingleCameraRequest();
                RenderPipeline.SubmitRenderRequest(hidden, request);
            }
            finally
            {
                _isRendering = false;
            }
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(foregroundPass);


    }

    [Serializable]
    public class CustomMaskRenderFeatureSettings
    {
        public Material material;
        public Material mat2;
        public float blurAmount;
        public LayerMask foregroundLayers;
        [Range(1, 8)]
        public int downscaling = 1;
        public bool renderBehindScene = false;


    }

    class CustomMaskRenderFeaturePass : ScriptableRenderPass
    {
        private readonly RTHandle rt;
        readonly CustomMaskRenderFeatureSettings settings;
        static readonly int BlurAmountID = Shader.PropertyToID("_BlurAmount");
        static readonly int DirectionID = Shader.PropertyToID("_Direction");
        static readonly int TexelSizeID = Shader.PropertyToID("_TexelSize");

        private RenderTargetInfo _rtInfo;
        private ImportResourceParams _importParams;

        private bool _rtInfoDirty = true;

        public CustomMaskRenderFeaturePass(
            CustomMaskRenderFeatureSettings settings,
            RTHandle handle)
        {
            this.settings = settings;
            this.rt = handle;
        }


        class PassData
        {
            public TextureHandle source;
            public Material blitMaterial;
            public Material mat2;
            public float blurAmount;
        }



        public void InvalidateRTInfo() => _rtInfoDirty = true;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            if (rt == null || rt.rt == null || !rt.rt.IsCreated()) return;

            if (_rtInfoDirty)
            {
                _importParams = new ImportResourceParams
                {
                    clearOnFirstUse = false,
                    discardOnLastUse = false
                };
                _rtInfo = new RenderTargetInfo
                {
                    width = rt.rt.width,
                    height = rt.rt.height,
                    volumeDepth = 1,
                    msaaSamples = 1,
                    format = rt.rt.graphicsFormat
                };
                _rtInfoDirty = false;
            }

            settings.downscaling = Mathf.Clamp(settings.downscaling, 1, 8);

            var camData = frameData.Get<UniversalCameraData>();
            if (camData.camera.cameraType != CameraType.Game)
                return;
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;
            var depth = resourceData.activeDepthTexture;
            //TextureHandle sourceHandle = renderGraph.ImportTexture(rt);
            /*var importParams = new ImportResourceParams();
            importParams.clearOnFirstUse = false;
            importParams.discardOnLastUse = false;*/


            /*var rtInfo = new RenderTargetInfo();

            rtInfo.width = rt.rt.width;
            rtInfo.height = rt.rt.height;
            rtInfo.volumeDepth = 1;
            rtInfo.msaaSamples = 1;
            rtInfo.format = rt.rt.graphicsFormat;*/

            TextureHandle sourceHandle = renderGraph.ImportTexture(rt, _rtInfo, _importParams);

            // Build tempA desc manually from camera pixel dimensions
            var tempDesc = new TextureDesc(rt.rt.width, rt.rt.height);
            tempDesc.name = "BlurTempA";
            tempDesc.colorFormat = GraphicsFormat.R8G8B8A8_SRGB;
            tempDesc.clearBuffer = false;
            tempDesc.clearColor = Color.clear;
            tempDesc.depthBufferBits = DepthBits.None;
            tempDesc.dimension = TextureDimension.Tex2D;
            tempDesc.useMipMap = false;
            tempDesc.enableRandomWrite = false;
            tempDesc.msaaSamples = MSAASamples.None;
            TextureHandle tempA = renderGraph.CreateTexture(tempDesc);




            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass1", out var passData))
            {
                builder.AllowPassCulling(false);

                builder.SetRenderAttachment(tempA, 0, AccessFlags.Write);

                passData.source = sourceHandle;
                passData.blitMaterial = settings.material;
                passData.mat2 = settings.mat2;
                passData.blurAmount = settings.blurAmount;

                builder.UseTexture(passData.source, AccessFlags.Read);


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    //data.blitMaterial.SetTexture("_MainTex", data.source);
                    data.blitMaterial.SetFloat(BlurAmountID, data.blurAmount);
                    data.blitMaterial.SetVector(DirectionID, new Vector2(1, 0));
                    //data.blitMaterial.SetVector("_CustomTexelSize", new Vector4(texelSizeX, texelSizeY, 0, 0));
                    //Blitter.BlitTexture2D(context.cmd, data.source, viewportScale, 0, true);
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
                });
            }


            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass2", out var passData))
            {

                builder.AllowPassCulling(false);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                passData.source = tempA;
                passData.blitMaterial = settings.material;
                passData.mat2 = settings.mat2;
                passData.blurAmount = settings.blurAmount;

                builder.UseTexture(passData.source, AccessFlags.Read);


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    //data.blitMaterial.SetTexture("_MainTex", data.source);
                    data.mat2.SetFloat(BlurAmountID, data.blurAmount);
                    data.mat2.SetVector(DirectionID, new Vector2(0, 1));
                    //data.mat2.SetVector("_CustomTexelSize", new Vector4(texelSizeX, texelSizeY, 0, 0));
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.mat2, 0);
                });

            }
        }
    }
}