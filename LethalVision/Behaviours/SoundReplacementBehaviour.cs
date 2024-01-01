using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Behaviours
{
    [Serializable]
    public class SoundReplacementBehaviour : MonoBehaviour
    {
        public AudioClip ReplacementClip;
        public string SoundName;
    }
}
