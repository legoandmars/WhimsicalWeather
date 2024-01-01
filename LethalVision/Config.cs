﻿using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalVision
{
    public class Config
    {
        public static Config Instance;
        public ConfigEntry<bool> RainbowUI { get; set; } // TODO: Hue Shift Speed. already supported in shader so should be easy to sync
        public ConfigEntry<bool> ChangePlayerPitch { get; set; }
        public ConfigEntry<float> PlayerPitchMultiplier { get; set; }
        public ConfigEntry<bool> RemovePlayerBlood { get; set; }
        public ConfigEntry<bool> ChangeShipHornPitch { get; set; }
        public ConfigEntry<float> ShipHornPitch { get; set; }
        public ConfigEntry<float> ShipHornStartPitch { get; set; }
        public ConfigEntry<bool> LollypopMeleeWeapons { get; set; }
        public ConfigEntry<bool> RainbowZapGun { get; set; }
        public ConfigEntry<bool> GooglyEyeDogs { get; set; }
        public ConfigEntry<bool> JesterHat { get; set; }

        public Config(ConfigFile cfg)
        {
            RainbowUI = cfg.Bind(
                "Modifications",
                "Rainbow UI",
                true,
                "Make the UI slowly cycle through rainbow colors when using whimsical vision"
            );
            ChangePlayerPitch = cfg.Bind(
                "Modifications.Player",
                "Change Player Pitch",
                true,
                "Pitch up the voice chat of other players when using whimsical vision"
            );
            PlayerPitchMultiplier = cfg.Bind(
                "Modifications.Player",
                "Player Pitch Multiplier",
                1.3f,
                "The amount player voice chat pitch is multplied when using whimsical vision. Does nothing if Change Player Pitch is disabled"
            );
            RemovePlayerBlood = cfg.Bind(
                "Modifications.Player",
                "Remove Player Blood",
                true,
                "Remove all player blood when using whimsical vision"
            );
            ChangeShipHornPitch = cfg.Bind(
                "Modifications.Ship",
                "Change Ship Horn Pitch",
                true,
                "Pitch up the ship's loud horn when using whimsical vision"
            );
            ShipHornPitch = cfg.Bind(
                "Modifications.Ship",
                "Ship Horn Maximum Pitch",
                1.3f,
                "The maximum pitch of the ship's loud horn when using whimsical vision. Does nothing if Change Ship Horn Pitch is disabled"
            );
            ShipHornStartPitch = cfg.Bind(
                "Modifications.Ship",
                "Ship Horn Minimum Pitch",
                1.25f,
                "The minimum/starting pitch of the ship's loud horn when using whimsical vision. Does nothing if Change Ship Horn Pitch is disabled"
            );
            LollypopMeleeWeapons = cfg.Bind(
                "Modifications.Items",
                "Lollypop Melee Weapons",
                true,
                "Replace melee weapons (Shovel, Stop Sign, Yield Sign) with lollypops when using whimsical vision"
            );
            RainbowZapGun = cfg.Bind(
                "Modifications.Items",
                "Rainbow Zap Gun",
                true,
                "Make the zap gun's color rainbow over time when using whimsical vision"
            );
            GooglyEyeDogs = cfg.Bind(
                "Modifications.Enemies",
                "Googly Eye Dogs",
                true,
                "Adds googly eyes to eyeless dogs when using whimsical vision"
            );
            JesterHat = cfg.Bind(
                "Modifications.Enemies",
                "Jester Hat",
                true,
                "Adds a jester hat to the jester's final form when using whimsical vision"
            );

            Instance = this;
        }
    }
}
