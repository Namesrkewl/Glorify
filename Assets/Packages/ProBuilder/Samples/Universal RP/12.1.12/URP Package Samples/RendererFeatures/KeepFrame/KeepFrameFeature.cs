using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KeepFrameFeature : ScriptableRendererFeature {
    class CopyFramePass : ScriptableRenderPass {
        private RTHandle source;
        private RTHandle destination;

        public void Setup(RTHandle source, RTHandle destination) {
            this.source = source;
            this.destination = destination;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (renderingData.cameraData.camera.cameraType != CameraType.Game)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("CopyFramePass");
            Blitter.BlitCameraTexture(cmd, source, destination);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    class DrawOldFramePass : ScriptableRenderPass {
        private Material m_DrawOldFrameMaterial;
        private RTHandle m_Handle;
        private string m_TextureName;

        public void Setup(Material drawOldFrameMaterial, RTHandle handle, string textureName) {
            m_DrawOldFrameMaterial = drawOldFrameMaterial;
            m_Handle = handle;
            m_TextureName = textureName;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (m_DrawOldFrameMaterial != null) {
                CommandBuffer cmd = CommandBufferPool.Get("DrawOldFramePass");
                cmd.SetGlobalTexture(m_TextureName, m_Handle);
                Blitter.BlitCameraTexture(cmd, m_Handle, m_Handle);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

    [Serializable]
    public class Settings {
        public Material displayMaterial;
        public string textureName = "_FrameCopyTex";
    }

    public Settings settings = new Settings();
    private CopyFramePass m_CopyFrame;
    private DrawOldFramePass m_DrawOldFrame;
    private RTHandle m_OldFrameHandle;

    public override void Create() {
        m_CopyFrame = new CopyFramePass {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };

        m_DrawOldFrame = new DrawOldFramePass {
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques
        };

        m_OldFrameHandle = RTHandles.Alloc(Vector2.one, TextureXR.slices, dimension: TextureXR.dimension, colorFormat: GraphicsFormat.R8G8B8A8_UNorm, useDynamicScale: true, name: "OldFrameRenderTarget");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (settings.displayMaterial == null)
            return;

        m_CopyFrame.Setup(renderer.cameraColorTargetHandle, m_OldFrameHandle);
        renderer.EnqueuePass(m_CopyFrame);

        m_DrawOldFrame.Setup(settings.displayMaterial, m_OldFrameHandle, settings.textureName);
        renderer.EnqueuePass(m_DrawOldFrame);
    }

    protected override void Dispose(bool disposing) {
        base.Dispose(disposing);
        if (disposing) {
            RTHandles.Release(m_OldFrameHandle);
            m_OldFrameHandle = null;
        }
    }
}