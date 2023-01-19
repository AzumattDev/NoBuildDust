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
        internal const string ModVersion = "1.0.2";
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
            foreach (GameObject instanceMPrefab in __instance.m_prefabs)
            {
                instanceMPrefab.TryGetComponent(out Piece? piece);
                instanceMPrefab.TryGetComponent(out WearNTear? wearNTear);
                if (piece != null)
                {
                    try
                    {
                        if (piece.m_placeEffect.m_effectPrefabs.Length > 0)
                        {
                            piece.m_placeEffect.m_effectPrefabs = piece.m_placeEffect.m_effectPrefabs
                                .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
                        }
                    }
                    catch
                    {
                        NoBuildDustPlugin.NoBuildDustLogger.LogWarning(
                            $"Couldn't replace the placement effects for: {Localization.instance.Localize(piece.m_name)} [{piece.name}]");
                    }
                }

                if (wearNTear != null)
                {
                    try
                    {
                        if (wearNTear.m_destroyedEffect.m_effectPrefabs.Length > 0)
                        {
                            wearNTear.m_destroyedEffect.m_effectPrefabs = wearNTear.m_destroyedEffect.m_effectPrefabs
                                .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
                        }
                    }
                    catch
                    {
                        NoBuildDustPlugin.NoBuildDustLogger.LogWarning(
                            $"Couldn't replace the destruction effects for: {Utils.GetPrefabName(wearNTear.transform.root.gameObject)}");
                    }
                }
            }

        }
    }
}