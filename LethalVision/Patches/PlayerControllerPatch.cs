using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void StartPrefix(PlayerControllerB __instance)
        {
            UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(HDAdditionalCameraData));

            for (int i = 0; i < array.Length; i++)
            {
                HDAdditionalCameraData cameraData = array[i] as HDAdditionalCameraData;

                cameraData.customRenderingSettings = true;
                ToggleCustomPass(cameraData, true);
                
            }
            var additionalCameraData = __instance.gameplayCamera.GetComponent<HDAdditionalCameraData>();
            if (additionalCameraData != null)
            {
                additionalCameraData.customRenderingSettings = true;
                ToggleCustomPass(additionalCameraData, true);
            }
        }

        public static void ToggleCustomPass(HDAdditionalCameraData cameraData, bool enable)
        {
            cameraData.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.CustomPass] = true;
            cameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass, enable);
        }
    }
}
