using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LethalVision.Models
{
    // used to keep track of changes we make to the game so we can undo them
    // meant to instantiate material for now, maybe eventually it could have some high-effort deduplication
    internal abstract class ImageShaderReplacement
    {
        // https://www.youtube.com/watch?v=x0H_WPzoTb0
        protected Dictionary<Image, Material?> _originalMaterialsByImage = new();

        public bool ImageShadersReplaced => _originalMaterialsByImage.Count > 0;

        public void ReplaceImageShaderIfMatch(Image image)
        {
            if (!ImageShaderIsMatch(image)) return;
            ReplaceImageShader(image);
        }

        public abstract bool ImageShaderIsMatch(Image image);
        public abstract void ReplaceImageShader(Image image);
        public abstract void RestoreImageShaders();
    }
}
