using System;
using System.Collections.Generic;
using System.Text;
using WhimsicalWeather.Models;
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
