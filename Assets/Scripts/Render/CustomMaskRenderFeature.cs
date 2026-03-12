using System;
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
    private bool _isRendering = false;
    private Action<ScriptableRenderContext, Camera> _renderHandler;
    private int _lastRenderedFrame = -1;

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

        if (hidden.targetTexture != null &&
            (hidden.targetTexture.width != main.pixelWidth || hidden.targetTexture.height != main.pixelHeight))
        {
            hidden.targetTexture.Release();
            hidden.targetTexture.width = main.pixelWidth / settings.downscaling;
            hidden.targetTexture.height = main.pixelHeight / settings.downscaling;
            hidden.targetTexture.Create();
        }
    }

    void OnBeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
    {


        if (cam == hiddenCam) return;
        if (cam.cameraType != CameraType.Game) return;
        if (!cam.CompareTag("MainCamera")) return;
        if (_isRendering) return;
        if (hiddenCam == null) return;
        if (handle == null) return;
        if (hiddenCam.targetTexture == null) return;
        if (Time.frameCount == _lastRenderedFrame) return;
            _lastRenderedFrame = Time.frameCount;


        Debug.Log($"Driver firing - frame:{Time.frameCount} hiddenCam:{hiddenCam.GetInstanceID()}");
        
        hiddenCam.cullingMask = settings.foregroundLayers;

        _isRendering = true;
        try
        {
            SyncCamera(hiddenCam, cam);
            /*RenderTexture prev = RenderTexture.active;
            RenderTexture.active = hiddenCam.targetTexture;
            GL.Clear(false, true, Color.clear);
            RenderTexture.active = prev;*/

            var request = new UniversalRenderPipeline.SingleCameraRequest();
            RenderPipeline.SubmitRenderRequest(hiddenCam, request);
        }
        finally
        {
            _isRendering = false;
        }
    }

    public override void Create()
    {
        var w = Mathf.Max(1, Screen.width / settings.downscaling);
        var h = Mathf.Max(1, Screen.height / settings.downscaling);

        bool needsRealloc = tex == null || !tex.IsCreated() || tex.width != w || tex.height != h;

        // Always unsubscribe first — guarantees exactly one subscription after Create()
        if (_renderHandler != null)
        {
            RenderPipelineManager.beginCameraRendering -= _renderHandler;
            _renderHandler = null;
        }

        // Always destroy old hidden cam
        if (hiddenCamGO != null || hiddenCam != null)
        {
            SafeDestroy(hiddenCamGO);
            hiddenCamGO = null;
            hiddenCam = null;
        }

        if (needsRealloc)
        {
            handle?.Release();
            handle = null;

            if (tex != null) { tex.Release(); tex = null; }

            var desc = new RenderTextureDescriptor(w, h, RenderTextureFormat.ARGB32, 0)
            {
                depthBufferBits = 0,
                depthStencilFormat = GraphicsFormat.None,
                msaaSamples = 1,
                sRGB = true
            };
            tex = new RenderTexture(desc) { name = "__BlurRT" + System.Guid.NewGuid() };
            tex.Create();
            handle = RTHandles.Alloc(tex);

            foregroundPass = new CustomMaskRenderFeaturePass(settings, handle);
            foregroundPass.InvalidateRTInfo();
        }

        if (foregroundPass != null)
            foregroundPass.renderPassEvent = settings.renderBehindScene
                ? RenderPassEvent.BeforeRenderingOpaques
                : RenderPassEvent.AfterRenderingTransparents;

        // Recreate hidden cam
        hiddenCamGO = new GameObject("HiddenCamGO") { hideFlags = HideFlags.HideAndDontSave };
        hiddenCam = hiddenCamGO.AddComponent<Camera>();
        hiddenCam.cullingMask = settings.foregroundLayers;
        hiddenCam.depthTextureMode = DepthTextureMode.None;
        hiddenCam.targetTexture = tex;
        hiddenCam.forceIntoRenderTexture = true;
        hiddenCam.enabled = false;
        hiddenCam.clearFlags = CameraClearFlags.SolidColor;
        hiddenCam.backgroundColor = new Color(0, 0, 0, 0);
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
        urpData.antialiasing = AntialiasingMode.None;

        // Subscribe exactly once
        _renderHandler = OnBeginCameraRendering;
        RenderPipelineManager.beginCameraRendering += _renderHandler;
    }

    protected override void Dispose(bool disposing)
    {
        if (_renderHandler != null)
        {
            RenderPipelineManager.beginCameraRendering -= _renderHandler;
            _renderHandler = null;
        }

        handle?.Release();
        handle = null;

        if (tex != null) { tex.Release(); tex = null; }

        if (hiddenCamGO != null)
        {
            SafeDestroy(hiddenCamGO);
            hiddenCamGO = null;
            hiddenCam = null;
        }
    }

    void SafeDestroy(UnityEngine.Object obj)
    {
        if (!obj) return;
        if (Application.isPlaying) Destroy(obj);
        else DestroyImmediate(obj);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (foregroundPass == null) return;
        if (renderingData.cameraData.camera.cameraType != CameraType.Game) return;
        if (renderingData.cameraData.camera == hiddenCam) return;
        renderer.EnqueuePass(foregroundPass);
    }

    [Serializable]
    public class CustomMaskRenderFeatureSettings
    {
        public Material material;
        public Material mat2;
        public float blurAmount;
        public LayerMask foregroundLayers;
        [Range(1, 8)] public int downscaling = 1;
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

        public CustomMaskRenderFeaturePass(CustomMaskRenderFeatureSettings settings, RTHandle handle)
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
            if (camData.camera.cameraType != CameraType.Game) return;

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;

            TextureHandle sourceHandle = renderGraph.ImportTexture(rt, _rtInfo, _importParams);

            var tempDesc = new TextureDesc(rt.rt.width, rt.rt.height)
            {
                name = "BlurTempA",
                colorFormat = GraphicsFormat.R8G8B8A8_SRGB,
                clearBuffer = false,
                clearColor = Color.clear,
                depthBufferBits = DepthBits.None,
                dimension = TextureDimension.Tex2D,
                useMipMap = false,
                enableRandomWrite = false,
                msaaSamples = MSAASamples.None
            };

            TextureHandle tempA = renderGraph.CreateTexture(tempDesc);

            var tempDescB = tempDesc;
            tempDescB.name = "BlurTempB";
            TextureHandle tempB = renderGraph.CreateTexture(tempDescB);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass1", out var passData))
            {
                builder.AllowPassCulling(false);
                builder.SetRenderAttachment(tempA, 0, AccessFlags.Write);
                passData.source = sourceHandle;
                passData.blitMaterial = settings.material;
                passData.blurAmount = settings.blurAmount;
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    data.blitMaterial.SetFloat(BlurAmountID, data.blurAmount);
                    data.blitMaterial.SetVector(DirectionID, new Vector2(1, 0));
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
                });
            }

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("BlurPass2", out var passData))
            {
                builder.AllowPassCulling(false);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.ReadWrite);
                passData.source = tempA;
                passData.blitMaterial = settings.mat2;
                passData.blurAmount = settings.blurAmount;
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    data.blitMaterial.SetFloat(BlurAmountID, data.blurAmount);
                    data.blitMaterial.SetVector(DirectionID, new Vector2(0, 1));
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.blitMaterial, 0);
                });
            }
        }
    }
}