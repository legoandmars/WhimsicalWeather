using HarmonyLib;
using LethalVision.Controllers;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
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
            if (!LethalVisuals.LethalVisualsEnabled) return;
            if (_replacementController == null) return;
            if (clip.name == "ShovelHitDefault" || clip.name == "ShovelHitDefault2")
            {
                if (!Config.Instance.LollypopMeleeWeapons.Value) return;
            }
            _replacementController.ReplaceSoundIfNeeded(ref clip);
        }
    }
}
