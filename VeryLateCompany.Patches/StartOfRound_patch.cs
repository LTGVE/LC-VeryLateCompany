using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VeryLateCompany.VeryLateCompany.Patches
{/*
    [HarmonyPatch(typeof(StartOfRound))]
     public class StartOfRound_patch
    {
        [HarmonyPatch("EndGameServerRpc")]
        [HarmonyPrefix]
        public static void EndGameServerRpc(StartOfRound __instance) {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(true);

        }
        [HarmonyPatch("EndGameClientRpc")]
        [HarmonyPrefix]
        public static void EndGameClientRpc(StartOfRound __instance)
        {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(true);
        }
    }*/
}
