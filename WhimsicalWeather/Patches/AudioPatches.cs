using HarmonyLib;
using WhimsicalWeather.Controllers;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Patches
{
    [HarmonyPatch(typeof(AudioSource))]
    internal class AudioPatches
    {
        private static ReplacementController? _replacementController;
        public static void SetReplacementController(ReplacementController replacementController)
        {
            _replacementController = replacementController;
        }

        [HarmonyPatch("PlayOneShot", new Type[] { typeof(AudioClip) })]
        [HarmonyPatch("PlayOneShotHelper")]
        [HarmonyPrefix]
        private static void PlayOneShotPrefix(ref AudioClip clip)
        {
            if (!WhimsicalVisuals.WhimsicalVisualsEnabled) return;
            if (_replacementController == null) return;
            if (clip == null || clip.name == null) return;
            if (clip.name == "ShovelHitDefault" || clip.name == "ShovelHitDefault2")
            {
                if (!Config.Instance.LollypopMeleeWeapons.Value) return;
            }
            _replacementController.ReplaceSoundIfNeeded(ref clip);
        }
    }
}
