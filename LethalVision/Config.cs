using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LethalVision
{
    public class Config
    {
        public static Config Instance;
        public ConfigEntry<bool> RemovePlayerBlood { get; set; }
        public ConfigEntry<bool> LollypopMeleeWeapons { get; set; }

        public Config(ConfigFile cfg)
        {
            RemovePlayerBlood = cfg.Bind(
                "Modifications",
                "Remove Player Blood",
                true,
                "Remove all player blood when using whimsical vision"
            );

            LollypopMeleeWeapons = cfg.Bind(
                "Modifications",
                "Lollypop Melee Weapons",
                true,
                "Replace melee weapons (Shovel, Stop Sign, Yield Sign) with lollypops when using whimsical vision"
            );

            Instance = this;
        }
    }
}
