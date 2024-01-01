using GameNetcodeStuff;
using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(MouthDogAI))]
    internal class DogPatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPrefix(MouthDogAI __instance)
        {
            if (!LethalVisuals.LethalVisualsEnabled) return;
            if (Plugin.GooglyEyes == null) return;
            if (!Config.Instance.GooglyEyeDogs.Value) return;

            var eyesParent = __instance.transform.Find("MouthDogModel/AnimContainer/Armature/Neck1Container/Neck1/Neck2/JawUpper");
            if (eyesParent == null) return;

            var eyes = UnityEngine.Object.Instantiate(Plugin.GooglyEyes, eyesParent, false);
            eyes.transform.localPosition = Vector3.zero;
            eyes.transform.localRotation = Quaternion.identity;
            eyes.transform.localScale = Vector3.one;
        }
    }
}
