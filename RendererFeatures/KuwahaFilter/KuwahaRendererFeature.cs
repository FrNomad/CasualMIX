using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwahaRendererFeature : ScriptableRendererFeature
{
    public Shader m_Shader;
    public int m_WindowSize;
    public int m_NumAreas;

    Material m_Material;

    [SerializeField] private CustomPassSettings settings;
    private KuwahaRenderPass m_RenderPass = null;

    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    class KuwahaRenderPass : ScriptableRenderPass
    {
        private KuwahaRendererFeature.CustomPassSettings settings;

        private RenderTargetIdentifier colorBuffer, filterBuffer;
        private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");

        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("KuwahaFilter");
        private Material m_Material;
        private RTHandle m_CameraColorTarget;

        private int m_WindowSize;
        private int m_NumAreas;

        public KuwahaRenderPass(KuwahaRendererFeature.CustomPassSettings settings, Material material)
        {
            m_Material = material;
            this.settings = settings;
            this.renderPassEvent = settings.renderPassEvent;
        }

        public void SetTarget(RTHandle colorHandle, int windowsize, int numarea)
        {
            m_CameraColorTarget = colorHandle;
            m_WindowSize = windowsize;
            m_NumAreas = numarea;
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

                m_Material.SetInt("_WindowSize", m_WindowSize);
                m_Material.SetInt("_NumAreas", m_NumAreas);

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

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(m_RenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_RenderPass.SetTarget(renderer.cameraColorTargetHandle, m_WindowSize, m_NumAreas);
        }
    }

    public override void Create() // Create material from given shader and pass it to the RenderPass
    {
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_RenderPass = new KuwahaRenderPass(settings, m_Material);
    }

    protected override void Dispose(bool disposing) // When RendererFeature is disposed, then destory the material
    {
        CoreUtils.Destroy(m_Material);
    }
}