using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class OutlinePass : ScriptableRenderPass
{
    private OutlineRendererFeature.CustomPassSettings settings;

    private RenderTargetIdentifier colorBuffer, filterBuffer;
    private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Outline");
    Material m_Material;
    RTHandle m_CameraColorTarget;

    // user-defined attributes
    float m_Intensity;
    float m_Thickness;
    float m_Threshold_Depth;
    float m_Threshold_Normal;
    Color m_OutlineColor;

    public OutlinePass(OutlineRendererFeature.CustomPassSettings settings, Material material)
    {
        this.renderPassEvent = settings.renderPassEvent;
        this.settings = settings;
        m_Material = material;
    }

    public void SetTarget(RTHandle colorHandle, float intensity, float thickness, Color outline_color, float threshold_depth, float threshold_normal)
    {
        m_CameraColorTarget = colorHandle;
        m_Intensity = intensity;
        m_Thickness = thickness;
        m_Threshold_Depth = threshold_depth;
        m_Threshold_Normal = threshold_normal;
        m_OutlineColor = outline_color;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureTarget(m_CameraColorTarget);

        colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;

        cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
        filterBuffer = new RenderTargetIdentifier(pixelBufferID);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cameraData = renderingData.cameraData;
        if (cameraData.camera.cameraType != CameraType.Game)
            return;

        if (m_Material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);

            m_Material.SetFloat("_Intensity", m_Intensity);
            m_Material.SetFloat("_Thickness", m_Thickness);
            m_Material.SetColor("_OutlineColor", m_OutlineColor);
            m_Material.SetFloat("_Threshold_Depth", m_Threshold_Depth);
            m_Material.SetFloat("_Threshold_Normal", m_Threshold_Normal);

            Blit(cmd, colorBuffer, filterBuffer, m_Material);
            Blit(cmd, filterBuffer, colorBuffer);
        }
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new System.ArgumentNullException("cmd");
        cmd.ReleaseTemporaryRT(pixelBufferID);
    }
}
