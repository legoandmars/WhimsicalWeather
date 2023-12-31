using BepInEx;
using HarmonyLib;
using LethalVision.Behaviours;
using LethalVision.Visuals;
using System.IO;
using System.Reflection;
using UnityEngine;
using static LethalLib.Modules.Levels;

namespace LethalVision
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Material PassMaterial;
        public static Shader RainbowTextShader;
        public static Shader RainbowUIShader;
        public static Shader RainbowLerpShader; // intended for lightning; currently unused
        public static Shader RainbowParticleShader; // uses vertex IDs to get ~4 random colors for particles without having to get complex
        public static GameObject VisualsObject;

        private LethalVisuals _visuals;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            VisualsObject = GetVisuals();
            PassMaterial = GetPrefabMaterialFromName("PassShader");
            RainbowTextShader = GetPrefabMaterialFromName("TextShader").shader;
            RainbowUIShader = GetPrefabMaterialFromName("UIShader").shader;
            RainbowLerpShader = GetPrefabMaterialFromName("SmoothShader").shader;
            RainbowParticleShader = GetPrefabMaterialFromName("ParticleShader").shader;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var visualsObject = new GameObject("LethalVisuals");
            visualsObject.hideFlags = HideFlags.HideAndDontSave;
            _visuals = visualsObject.AddComponent<LethalVisuals>();
            _visuals.SetAudio(VisualsObject.transform.Find("SparkleAudio").GetComponent<AudioSource>().clip);

            RegisterWeather(visualsObject);
        }

        private void RegisterWeather(GameObject visualsObject)
        {
            var dummyObject = new GameObject("LethalVisualsDummy");
            dummyObject.hideFlags = HideFlags.HideAndDontSave;

            var permanentObject = new GameObject("LethalVisualsPermanentObject");
            permanentObject.hideFlags = HideFlags.HideAndDontSave;
            var activityBehaviour = permanentObject.AddComponent<GameobjectActivityBehaviour>();
            permanentObject.SetActive(false);
            activityBehaviour.EventsEnabled = true;


            WeatherEffect weatherEffect = new()
            {
                name = "<color=#FF00FF>Whimsical</color>",
                effectObject = dummyObject,
                effectPermanentObject = permanentObject,
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
    }
}