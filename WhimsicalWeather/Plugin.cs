﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using WhimsicalWeather.Behaviours;
using WhimsicalWeather.Visuals;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using static LethalLib.Modules.Levels;

namespace WhimsicalWeather
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static Material PassMaterial;
        public static Shader RainbowTextShader;
        public static Shader RainbowUIShader;
        public static Shader RainbowLerpShader; // intended for lightning; currently unused
        public static Shader RainbowParticleShader; // uses vertex IDs to get ~4 random colors for particles without having to get complex
        public static Shader RainbowDropshipLightsShader; // used for itemdropship tree christmas lights/star

        public static GameObject SparkParticles;
        public static GameObject Rainbow;
        public static GameObject GooglyEyes;
        public static GameObject JesterHat;

        public static GameObject RedLollypop;
        public static GameObject GrayLollypop;
        public static GameObject YellowLollypop;

        public static GameObject VisualsObject;
        public static GameObjectActivityBehaviour WeatherEvents;

        private static Shader? _litShader;

        private WhimsicalVisuals _visuals;

        private void Awake()
        {
            Logger = base.Logger;
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            new Config(Config);

            _litShader = Shader.Find("HDRP/Lit"); // idk if this will ever return the wrong lit shader?
            VisualsObject = GetVisuals();

            PassMaterial = GetPrefabMaterialFromName("PassShader");
            RainbowTextShader = GetPrefabMaterialFromName("TextShader").shader;
            RainbowUIShader = GetPrefabMaterialFromName("UIShader").shader;
            RainbowLerpShader = GetPrefabMaterialFromName("SmoothShader").shader;
            RainbowParticleShader = GetPrefabMaterialFromName("ParticleShader").shader;
            RainbowDropshipLightsShader = GetPrefabMaterialFromName("DropshipLightsShader").shader;

            SparkParticles = GetModelPrefabFromName("Sparticles");
            Rainbow = GetModelPrefabFromName("rainbow");
            GooglyEyes = GetModelPrefabFromName("GooglyEyeHolder");
            JesterHat = GetModelPrefabFromName("JesterHat");
            RedLollypop = GetModelPrefabFromName("Lollypop");
            GrayLollypop = GetModelPrefabFromName("GrayLollypop");
            YellowLollypop = GetModelPrefabFromName("YellowLollypop");

            ReplaceShadersWithLit(GooglyEyes);
            ReplaceShadersWithLit(JesterHat);
            ReplaceShadersWithLit(RedLollypop);
            ReplaceShadersWithLit(GrayLollypop);
            ReplaceShadersWithLit(YellowLollypop);
            InitializeTransparentMaterial(RedLollypop, "LollypopTransparent");
            InitializeTransparentMaterial(GrayLollypop, "GrayLollypopTransparent");
            InitializeTransparentMaterial(YellowLollypop, "YellowLollypopTransparent");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var visualsObject = new GameObject("WhimsicalVisuals");
            visualsObject.hideFlags = HideFlags.HideAndDontSave;
            _visuals = visualsObject.AddComponent<WhimsicalVisuals>();
            _visuals.SetAudio(VisualsObject.transform.Find("SparkleAudio").GetComponent<AudioSource>().clip);

            RegisterWeather(visualsObject);
        }

        private void RegisterWeather(GameObject visualsObject)
        {
            var dummyObject = new GameObject("WhimsicalVisualsVisualsDummy");
            dummyObject.hideFlags = HideFlags.HideAndDontSave;

            var weatherEventsObject = new GameObject("WhimsicalVisualsPermanentObject");
            weatherEventsObject.hideFlags = HideFlags.HideAndDontSave;
            WeatherEvents = weatherEventsObject.AddComponent<GameObjectActivityBehaviour>();
            weatherEventsObject.SetActive(false);
            WeatherEvents.EventsEnabled = true;


            WeatherEffect weatherEffect = new()
            {
                name = "<color=#FF00FF>Whimsical</color>",
                effectObject = dummyObject,
                effectPermanentObject = weatherEventsObject,
                lerpPosition = false,
                sunAnimatorBool = "",
                transitioning = false
            };

            LethalLib.Modules.Weathers.RegisterWeather("<color=#FF00FF>Whimsical</color>", weatherEffect, LevelTypes.All, 0, 0);

        }

        private GameObject GetVisuals()
        {
            var bundlePath = Path.Join(Path.GetDirectoryName(this.Info.Location), "visualsobject");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var asset = bundle.LoadAsset<GameObject>("assets/visualsobject.prefab");

            return asset;
        }

        private Material GetPrefabMaterialFromName(string name)
        {
            return VisualsObject.transform.Find(name).GetComponent<MeshRenderer>().sharedMaterial;
        }

        private GameObject GetModelPrefabFromName(string name)
        {
            return VisualsObject.transform.Find("Models/"+name).gameObject;
        }

        public static void ReplaceShadersWithLit(GameObject gameObject)
        {
            if (_litShader == null)
            {
                Plugin.Logger.LogWarning("Cannot find HDRP/Lit");
                return;
            }

            var materials = gameObject.GetComponentsInChildren<Renderer>(true).SelectMany(x => x.sharedMaterials).ToList();
            foreach (var material in materials)
            {
                material.shader = _litShader;
            }

        }
        public static void InitializeTransparentMaterial(GameObject gameObject, string materialName)
        {
            var materials = gameObject.GetComponentsInChildren<Renderer>(true).SelectMany(x => x.sharedMaterials).ToList();
            foreach (var material in materials)
            {
                if (material.name != materialName) continue;
                material.renderQueue = (int)RenderQueue.Transparent;
            }
        }
    }
}