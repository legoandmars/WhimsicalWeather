using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Models
{
    // used to keep track of changes we make to the game so we can undo them
    // this is a bit of a mess right now, there's probably a less-code-reuse heavy way to structure this
    internal abstract class MaterialReplacement
    {
        protected List<Material> _replacedMaterials = new();

        public bool MaterialsReplaced => _replacedMaterials.Count > 0;

        public void ReplaceMaterialIfMatch(Material material)
        {
            if (!MaterialIsMatch(material)) return;
            ReplaceOnMaterial(material);
        }

        public abstract bool MaterialIsMatch(Material material);
        public abstract void ReplaceOnMaterial(Material material);
        public abstract void RestoreMaterials();
    }
}
