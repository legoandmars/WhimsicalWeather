﻿using HarmonyLib;
using LethalVision.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace LethalVision.Patches
{
    [HarmonyPatch]
    internal class VoicePitchPatches
    {
        private const float _maxVoicePitchAddAmount = 0.3f;

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
                    Debug.Log("patch");
                    codes[i].opcode = OpCodes.Call;
                    codes[i].operand = _modifiedVoiceTargetsMethod;
                }
            }
            return codes.Where(x => x.opcode != OpCodes.Nop).AsEnumerable();
        }

        public static float[] ModifiedPlayerVoicePitches(SoundManager soundManager)
        {
            if (LethalVisuals.LethalVisualsEnabled)
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
            _voicePitchAddAmount = percent * _maxVoicePitchAddAmount;
        }
    }
}