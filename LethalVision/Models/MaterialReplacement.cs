using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    // used to keep track of changes we make to the game so we can undo them
    internal abstract class MaterialReplacement
    {
        protected List<Material> _replacedMaterials = new();

        public bool MaterialsReplaced => _replacedMaterials.Count > 0;

        public abstract bool MaterialIsMatch(Material material);

        public abstract void ReplaceMaterialIfMatch(Material material);

        public abstract void ReplaceOnMaterial(Material material);

        public abstract void RestoreMaterials();
    }
}
