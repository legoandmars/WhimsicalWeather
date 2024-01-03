using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhimsicalWeather
{
    public class Config
    {
        public static Config Instance;
        public ConfigEntry<bool> RainbowUI { get; set; }
        public ConfigEntry<float> RainbowUIHueShiftSpeed { get; set; }
        public ConfigEntry<bool> ChangePlayerPitch { get; set; }
        public ConfigEntry<float> PlayerPitchMultiplier { get; set; }
        public ConfigEntry<bool> RemovePlayerBlood { get; set; }
        public ConfigEntry<bool> FearLaughing { get; set; }
        public ConfigEntry<bool> ChangeShipHornPitch { get; set; }
        public ConfigEntry<float> ShipHornPitch { get; set; }
        public ConfigEntry<float> ShipHornStartPitch { get; set; }
        public ConfigEntry<bool> LollypopMeleeWeapons { get; set; }
        public ConfigEntry<bool> RainbowZapGun { get; set; }
        public ConfigEntry<bool> RainbowApparatus { get; set; }
        public ConfigEntry<bool> ClearMagnifyingGlass { get; set; }
        public ConfigEntry<bool> GooglyEyeDogs { get; set; }
        public ConfigEntry<bool> JesterHat { get; set; }
        public ConfigEntry<bool> RainbowBlob { get; set; }
        public ConfigEntry<bool> RainbowItemDropship { get; set; }
        public ConfigEntry<bool> DeveloperToggle { get; set; }

        public Config(ConfigFile cfg)
        {
            RainbowUI = cfg.Bind(
                "Modifications",
                "RainbowUI",
                true,
                "Make the UI cycle through rainbow colors when using whimsical vision"
            );
            RainbowUIHueShiftSpeed = cfg.Bind(
                "Modifications",
                "RainbowUISpeed",
                1f,
                "How fast to cycle the UI's colors when using whimsical vision. Does nothing if Rainbow UI is disabled"
            );
            ChangePlayerPitch = cfg.Bind(
                "Modifications.Player",
                "ChangePlayerPitch",
                true,
                "Pitch up the voice chat of other players when using whimsical vision"
            );
            PlayerPitchMultiplier = cfg.Bind(
                "Modifications.Player",
                "PlayerPitchMultiplier",
                1.3f,
                "The amount player voice chat pitch is multiplied when using whimsical vision. Does nothing if Change Player Pitch is disabled"
            );
            RemovePlayerBlood = cfg.Bind(
                "Modifications.Player",
                "RemovePlayerBlood",
                true,
                "Remove all player blood when using whimsical vision"
            );
            FearLaughing = cfg.Bind(
                "Modifications.Player",
                "FearLaughing",
                true,
                "Have the player laugh to themselves when fear is high using whimsical vision"
            );
            ChangeShipHornPitch = cfg.Bind(
                "Modifications.Ship",
                "ChangeShipHornPitch",
                true,
                "Pitch up the ship's loud horn when using whimsical vision"
            );
            ShipHornPitch = cfg.Bind(
                "Modifications.Ship",
                "ShipHornMaximumPitch",
                1.3f,
                "The maximum pitch of the ship's loud horn when using whimsical vision. Does nothing if Change Ship Horn Pitch is disabled"
            );
            ShipHornStartPitch = cfg.Bind(
                "Modifications.Ship",
                "ShipHornMinimumPitch",
                1.25f,
                "The minimum/starting pitch of the ship's loud horn when using whimsical vision. Does nothing if Change Ship Horn Pitch is disabled"
            );
            LollypopMeleeWeapons = cfg.Bind(
                "Modifications.Items",
                "LollypopMeleeWeapons",
                true,
                "Replace melee weapons (Shovel, Stop Sign, Yield Sign) with lollypops when using whimsical vision"
            );
            RainbowZapGun = cfg.Bind(
                "Modifications.Items",
                "RainbowZapGun",
                true,
                "Make the zap gun's color rainbow over time when using whimsical vision"
            );
            RainbowApparatus = cfg.Bind(
                "Modifications.Items",
                "RainbowApparatus",
                true,
                "Make the apparatus's color rainbow over time when using whimsical vision"
            );
            ClearMagnifyingGlass = cfg.Bind(
                "Modifications.Items",
                "MagnifyingGlassShowsReality",
                true,
                "Make the magnifying glass show a clear picture of reality when using whimsical vision"
            );
            GooglyEyeDogs = cfg.Bind(
                "Modifications.Enemies",
                "GooglyEyeDogs",
                true,
                "Adds googly eyes to eyeless dogs when using whimsical vision"
            );
            JesterHat = cfg.Bind(
                "Modifications.Enemies",
                "JesterHat",
                true,
                "Adds a jester hat to the jester's final form when using whimsical vision"
            );
            RainbowBlob = cfg.Bind(
                "Modifications.Enemies",
                "RainbowBlob",
                true,
                "Make the Hygrodere's color rainbow over time when using whimsical vision"
            );
            RainbowItemDropship = cfg.Bind(
                "Modifications.World",
                "RainbowItemDropship",
                true,
                "Adds rainbow christmas lights to the item dropship when using whimsical vision"
            );
            DeveloperToggle = cfg.Bind(
                "Other.Development",
                "ManualWhimsicalVisionToggle",
                false,
                "Allows you to press F10 to manually enable/disable whimsical vision. Do NOT enable this unless you know what you're doing"
            );

            Instance = this;
        }
    }
}
