using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LethalVision.Models
{
    // java name java name
    internal class ImageGameObjectNameShaderReplacement : ImageShaderReplacement
    {
        private Shader _replacement;
        private string _gameObjectName;
        private string? _parentName;

        public ImageGameObjectNameShaderReplacement(Shader replacement, string gameObjectName, string? parentName = null)
        {
            _gameObjectName = gameObjectName;
            _parentName = parentName;
            _replacement = replacement;
        }

        public override bool ImageShaderIsMatch(Image image)
        {
            if (image == null || image.gameObject == null) return false;
            if (_parentName != null && image.transform.parent == null) return false;
            if (_parentName != null && image.transform.parent.name != _parentName) return false;
            if (image.gameObject.name != _gameObjectName) return false;
            return true;
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
            var parentNameString = _parentName == null ? string.Empty : _parentName + "/";
            return $"ImageShaderReplacement (GameObject Name): {_replacement.name} (replacing GameObject: {parentNameString}{_gameObjectName})";
        }
    }
}
