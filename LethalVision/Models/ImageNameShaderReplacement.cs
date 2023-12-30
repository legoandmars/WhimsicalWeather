using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LethalVision.Models
{
    internal class ImageNameShaderReplacement : ImageShaderReplacement
    {
        private Shader _replacement;
        private string _spriteName;

        public ImageNameShaderReplacement(Shader replacement, string spriteName)
        {
            _spriteName = spriteName;
            _replacement = replacement;
        }

        public override bool ImageShaderIsMatch(Image image)
        {
            // "TextMeshPro/Distance Field"
            return image.sprite.name == _spriteName;
        }

        public override void ReplaceImageShader(Image image)
        {
            if (_originalMaterialsByImage.ContainsKey(image)) return;

            _originalMaterialsByImage.Add(image, image.material);

            image.material = UnityEngine.Object.Instantiate(image.material);
            image.material.shader = _replacement;
        }

        public override void RestoreImageShaders()
        {
            foreach (var (image, material) in _originalMaterialsByImage)
            {
                image.material = material;
            }

            _originalMaterialsByImage.Clear();
        }

        public override string ToString()
        {
            return $"ImageShaderReplacement (Name): {_replacement.name} (replacing sprite: {_spriteName})";
        }
    }
}
