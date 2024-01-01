using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Behaviours
{
    [Serializable]
    public class TextureReplacementBehaviour : MonoBehaviour
    {
        public Texture Texture;
        public string MaterialName;
        public string PropertyName;
        public bool ForceTransparentLayer;
    }
}
