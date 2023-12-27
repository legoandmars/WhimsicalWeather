using BepInEx;
using HarmonyLib;
using LethalVision.Visuals;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LethalVision
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Material PassMaterial;
        public static GameObject VisualsObject;

        private LethalVisuals _visuals;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            PassMaterial = GetShader();
            VisualsObject = GetVisuals();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var visualsObject = new GameObject("LethalVisuals");
            visualsObject.hideFlags = HideFlags.HideAndDontSave;
            _visuals = visualsObject.AddComponent<LethalVisuals>();
            _visuals.SetAudio(VisualsObject.transform.Find("SparkleAudio").GetComponent<AudioSource>().clip);
        }

        private Material GetShader()
        {
            var bundlePath = Path.Join(Path.GetDirectoryName(this.Info.Location), "fullscreen_okhsl");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var asset = bundle.LoadAsset<Material>("assets/shaders/fullscreen_okhsl.mat");

            return asset;
        }

        private GameObject GetVisuals()
        {
            var bundlePath = Path.Join(Path.GetDirectoryName(this.Info.Location), "visualsobject");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var asset = bundle.LoadAsset<GameObject>("assets/visualsobject.prefab");

            return asset;
        }

    }
}