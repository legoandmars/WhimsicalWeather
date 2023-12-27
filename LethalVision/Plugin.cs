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
        public static Shader RainbowTextShader;
        public static Shader RainbowUIShader;
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

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var visualsObject = new GameObject("LethalVisuals");
            visualsObject.hideFlags = HideFlags.HideAndDontSave;
            _visuals = visualsObject.AddComponent<LethalVisuals>();
            _visuals.SetAudio(VisualsObject.transform.Find("SparkleAudio").GetComponent<AudioSource>().clip);
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