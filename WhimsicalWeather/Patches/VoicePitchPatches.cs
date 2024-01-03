using HarmonyLib;
using WhimsicalWeather.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace WhimsicalWeather.Patches
{
    [HarmonyPatch]
    internal class VoicePitchPatches
    {
        private static MethodInfo _modifiedVoiceTargetsMethod = AccessTools.Method(typeof(VoicePitchPatches), "ModifiedPlayerVoicePitches");
        private static float _voicePitchAddAmount = 0f;

        [HarmonyPatch(typeof(SoundManager), nameof(SoundManager.SetPlayerVoiceFilters))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SetPlayerVoiceFiltersTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand is FieldInfo fieldInfo && fieldInfo.Name == "playerVoicePitchTargets")
                {
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = _modifiedVoiceTargetsMethod;
                }
            }
            return codes.Where(x => x.opcode != OpCodes.Nop).AsEnumerable();
        }

        public static float[] ModifiedPlayerVoicePitches(SoundManager soundManager)
        {
            if (WhimsicalVisuals.WhimsicalVisualsEnabled && Config.Instance.ChangePlayerPitch.Value)
            {
                return soundManager.playerVoicePitchTargets.Select(x => x + _voicePitchAddAmount).ToArray();
            }
            else
            {
                return soundManager.playerVoicePitchTargets;
            }
        }

        public static void SetVoicePitchOverridePercent(float percent)
        {
            _voicePitchAddAmount = percent * (Config.Instance.PlayerPitchMultiplier.Value - 1);
        }
    }
}
