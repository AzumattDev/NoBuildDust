using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace NoBuildDust
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NoBuildDustPlugin : BaseUnityPlugin
    {
        internal const string ModName = "NoBuildDust";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;

        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource NoBuildDustLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
        }
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetSceneAwakePatch
    {
        static void Postfix(ZNetScene __instance)
        {
            NoBuildDustPlugin.NoBuildDustLogger.LogDebug("ZNetScene Awake Postfix, turning off build dust");
            foreach (GameObject instanceMPrefab in __instance.m_prefabs.Where(instanceMPrefab =>
                         instanceMPrefab.GetComponent<Piece>()))
            {
                Piece? pieceComponent = instanceMPrefab.GetComponent<Piece>();
                pieceComponent.m_placeEffect.m_effectPrefabs = pieceComponent.m_placeEffect.m_effectPrefabs
                    .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
            }
        }
    }
}