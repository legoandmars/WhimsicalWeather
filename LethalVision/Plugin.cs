using BepInEx;
using HarmonyLib;
using LethalVision.Behaviours;
using LethalVision.Visuals;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
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
        public static Shader RainbowDropshipLightsShader; // used for itemdropship tree christmas lights/star

        public static GameObject SparkParticles;
        public static GameObject Rainbow;
        public static GameObject GooglyEyes;
        public static GameObject JesterHat;
        public static GameObject RedLollypop;

        public static GameObject VisualsObject;
        public static GameObjectActivityBehaviour WeatherEvents;

        private static Shader? _litShader;

        private LethalVisuals _visuals;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
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

            ReplaceShadersWithLit(GooglyEyes);
            ReplaceShadersWithLit(JesterHat);
            ReplaceShadersWithLit(RedLollypop);
            InitializeTransparentMaterial(RedLollypop, "LollypopTransparent");

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

            var weatherEventsObject = new GameObject("LethalVisualsPermanentObject");
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
                Debug.LogWarning("Cannot find HDRP/Lit");
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