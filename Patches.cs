using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace NoBuildDust;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
static class ZNetSceneAwakePatch
{
    static void Postfix(ZNetScene __instance)
    {
        foreach (GameObject instanceMPrefab in __instance.m_prefabs.Where(instanceMPrefab =>
                     instanceMPrefab.GetComponent<Piece>()))
        {
            Piece? pieceComponent = instanceMPrefab.GetComponent<Piece>();
            pieceComponent.m_placeEffect.m_effectPrefabs = pieceComponent.m_placeEffect.m_effectPrefabs
                .Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
        }
    }
}