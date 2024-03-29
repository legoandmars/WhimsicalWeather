﻿using WhimsicalWeather.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Behaviours
{
    [Serializable]
    public class SoundReplacementBehaviour : MonoBehaviour
    {
        public AudioClip ReplacementClip;
        public string SoundName;
        public SoundReplacementType SoundReplacementType = SoundReplacementType.OneShotAudio;
    }
}
