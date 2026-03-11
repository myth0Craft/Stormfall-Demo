using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class RenderMainScene : ScriptableRendererFeature
{
    [SerializeField] CustomMaskRenderFeatureSettings settings;
    [SerializeField] RenderPassEvent pass;
    CustomMaskRenderFeaturePass m_ScriptablePass;
    private RTHandle handle;
    /// <inheritdoc/>
    public override void Create()
    {



        m_ScriptablePass = new CustomMaskRenderFeaturePass(settings);

        if (pass != null) {
            m_ScriptablePass.renderPassEvent = pass;
        } else 
            m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;





    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
        
    }

    [Serializable]
    public class CustomMaskRenderFeatureSettings
    {
        public LayerMask layerMask; 
    }

    class CustomMaskRenderFeaturePass : ScriptableRenderPass
    {
        readonly CustomMaskRenderFeatureSettings settings;
        private FilteringSettings filteringSettings;

        public CustomMaskRenderFeaturePass(CustomMaskRenderFeatureSettings settings)
        {
            this.settings = settings;

            filteringSettings = new FilteringSettings(RenderQueueRange.all, settings.layerMask);

        }

        class PassData
        {
            public TextureHandle source;
        }



        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            var renderData = frameData.Get<UniversalRenderingData>();
            var camData = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.activeColorTexture;
            

            var lightData = frameData.Get<UniversalLightData>();


            TextureDesc desc = cameraColor.GetDescriptor(renderGraph);
            desc.name = "SceneTexture";
            desc.clearBuffer = true;
            desc.clearColor = Color.clear;
            

            

            TextureHandle layerTex = renderGraph.CreateTexture(desc);


            TextureDesc layerDepthDesc = desc;
            layerDepthDesc.name = "SceneDepth";
            layerDepthDesc.depthBufferBits = DepthBits.Depth24;
            TextureHandle layerDepth = renderGraph.CreateTexture(layerDepthDesc);


            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CompositePass", out var passData))
            {

                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);
                builder.SetRenderAttachmentDepth(layerDepth, AccessFlags.ReadWrite);
                passData.source = layerTex;

                builder.UseTexture(passData.source, AccessFlags.Read);

                


                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, Vector2.one, 0, false);
                });
            }
        }
    }
}