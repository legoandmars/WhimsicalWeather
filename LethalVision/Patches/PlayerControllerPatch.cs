using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using TMPro;

namespace LethalVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        private static void StartPrefix(PlayerControllerB __instance)
        {
            Debug.Log("HDLethalCompany - Applying configs");

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
                Debug.Log("Setting additional camera data...");
                additionalCameraData.customRenderingSettings = true;
                ToggleCustomPass(additionalCameraData, true);
            }

            TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI)) as TextMeshProUGUI[];
            foreach(var text in texts)
            {
                Debug.Log(text.fontMaterial.shader.name);
            }

        }

        public static void ToggleCustomPass(HDAdditionalCameraData cameraData, bool enable)
        {
            cameraData.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.CustomPass] = true;
            cameraData.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass, enable);
        }
    }
}
