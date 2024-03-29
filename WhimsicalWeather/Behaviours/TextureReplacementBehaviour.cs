﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Behaviours
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
