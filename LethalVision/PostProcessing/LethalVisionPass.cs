using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace LethalVision.PostProcessing
{
    public class LethalVisionPass : CustomPass
    {
        protected override bool executeInSceneView => true;

        private RenderTexture colorTex;
        private int _lastWidth = 0;
        private int _lastHeight = 0;

        void RegenerateRenderTextures(int width, int height)
        {
            colorTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            colorTex.enableRandomWrite = true;
            colorTex.Create();

            _lastWidth = width;
            _lastHeight = height;
        }

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            Debug.Log("we setting up!");
        }

        // not the most efficient way but i was having sorting issues using just a fullscreen custom pass
        // idk if the rescaling actually saves enough performance
        protected override void Execute(CustomPassContext ctx)
        {
            if (Plugin.PassMaterial == null) return;

            int width = ctx.hdCamera.camera.pixelWidth;
            int height = ctx.hdCamera.camera.pixelHeight;

            if (_lastWidth != width || _lastHeight != height || colorTex == null)
            {
                RegenerateRenderTextures(width, height);
            }
            var scale = RTHandles.rtHandleProperties.rtHandleScale;
            ctx.cmd.Blit(ctx.cameraColorBuffer, colorTex, Plugin.PassMaterial, 0, 0);

            ctx.cmd.Blit(colorTex, ctx.cameraColorBuffer, Vector2.one, Vector2.zero, 0, 0);

            Debug.Log("we passin!");
        }
        protected override void Cleanup()
        {
            // todo?
        }
    }
}
