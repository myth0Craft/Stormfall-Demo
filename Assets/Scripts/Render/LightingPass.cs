using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class Custom2DLightingPass : ScriptableRenderPass
{
    private static readonly ShaderTagId[] k_ShaderTags = {
        new ShaderTagId("UniversalForward"),
        new ShaderTagId("Universal2D"),
        new ShaderTagId("LightweightForward"),
        new ShaderTagId("SRPDefaultUnlit"),
    };

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();
        var lightData = frameData.Get<UniversalLightData>();
        var renderingData = frameData.Get<UniversalRenderingData>();

        // Build the renderer list descriptor
        var sortingSettings = new SortingSettings(cameraData.camera)
        {
            criteria = SortingCriteria.CommonTransparent
        };

        var drawSettings = RenderingUtils.CreateDrawingSettings(
            new System.Collections.Generic.List<ShaderTagId>(k_ShaderTags),
            renderingData,
            cameraData,
            lightData,
            SortingCriteria.CommonTransparent
        );

        var filterSettings = new FilteringSettings(RenderQueueRange.all);

        // Create the RendererListHandle for the render graph
        var listDesc = new RendererListDesc(k_ShaderTags, renderingData.cullResults, cameraData.camera)
        {
            sortingCriteria = SortingCriteria.CommonTransparent,
            renderQueueRange = RenderQueueRange.all,
        };

        RendererListHandle rendererListHandle = renderGraph.CreateRendererList(listDesc);

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("2D Lit Sprites", out var passData))
        {
            passData.rendererList = rendererListHandle;

            builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);
            builder.UseRendererList(rendererListHandle); // Declare dependency

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                ctx.cmd.DrawRendererList(data.rendererList);
            });
        }
    }

    class PassData
    {
        public RendererListHandle rendererList;
    }
}