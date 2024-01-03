using HarmonyLib;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Patches
{
    [HarmonyPatch(typeof(BlobAI))]
    internal class BlobPatches
    {
        private static int _gradientColorProperty = Shader.PropertyToID("_Gradient_Color");
        private static Color _defaultBlobColor = new Color(0.04313725f, 1f, 0.3046348f);
        private static readonly float _hue, _saturation, _value;

        static BlobPatches()
        {
            Color.RGBToHSV(_defaultBlobColor, out float H, out float S, out float V);
            _hue = H;
            _saturation = S;
            _value = V;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void StartPrefix(BlobAI __instance)
        {
            if (!WhimsicalVisuals.WhimsicalVisualsEnabled) return;
            if (!Config.Instance.RainbowBlob.Value) return;

            float newHue = (_hue + (Time.unscaledTime / 5)) % 1; // should maybe use deltatime/persistence instead? idk
            __instance.thisSlimeMaterial.SetColor(_gradientColorProperty, Color.HSVToRGB(newHue, _saturation, _value));
        }
    }
}
