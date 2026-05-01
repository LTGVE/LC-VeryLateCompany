using GameNetcodeStuff;
using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;

namespace VeryLateCompany.Patches
{
    [HarmonyDebug]

    [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
    internal class OnPlayerDC_patch

    {

        [HarmonyPrefix]
        private static bool Prefix(StartOfRound __instance, int playerObjectNumber, ulong clientId)
        {/*
            if (clientId == OnPlayerConnectedClientRpc_patch.currentClientId && !NetworkManager.Singleton.IsServer)
            {
                Debug.Log($"OnPlayerDC: Local client is disconnecting currentClientId: {OnPlayerConnectedClientRpc_patch.currentClientId} and clientId: {clientId}");
                OnPlayerConnectedClientRpc_patch.isClient = false;
                RoundManager_Patch.isMidSessionJoiningRound = false;

            }
            Debug.Log("Calling OnPlayerDC!");
            if (!__instance.ClientPlayerList.ContainsKey(clientId))
            {
                Debug.Log("disconnect: clientId key already removed!");
                return false;
            }
            if (GameNetworkManager.Instance.localPlayerController != null && clientId == GameNetworkManager.Instance.localPlayerController.actualClientId)
            {
                Debug.Log("OnPlayerDC: Local client is disconnecting so return.");
                return false;
            }
            if (((NetworkBehaviour)__instance).NetworkManager.ShutdownInProgress || NetworkManager.Singleton == null)
            {
                Debug.Log("Shutdown is in progress, returning");
                return false;
            }
            Debug.Log("Player DC'ing 2");
            if (((NetworkBehaviour)__instance).IsServer && __instance.ClientPlayerList.TryGetValue(clientId, out var value))
            {
                HUDManager.Instance.AddTextToChatOnServer($"[playerNum{__instance.allPlayerScripts[value].playerClientId}] disconnected.");
            }
            if (!__instance.allPlayerScripts[playerObjectNumber].isPlayerDead)
            {
                __instance.livingPlayers--;
            }
            __instance.ClientPlayerList.Remove(clientId);
            __instance.connectedPlayersAmount--;
            Debug.Log("Player DC'ing 3");
            PlayerControllerB component = __instance.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            component.sentPlayerValues = false;
            component.isPlayerControlled = false;
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
            {
                HUDManager.Instance.UpdateBoxesSpectateUI();
            }
            if (!NetworkManager.Singleton.ShutdownInProgress && ((NetworkBehaviour)__instance).IsServer)
            {
                ((Component)component).gameObject.GetComponent<NetworkObject>().RemoveOwnership();
            }
            Object.FindObjectOfType<QuickMenuManager>()?.RemoveUserFromPlayerList(playerObjectNumber);
            component.DropAllHeldItems(itemsFall: true, disconnecting: true);
            Plugin.SetLobbyJoinable(joinable: true);
            component.DisablePlayerModel(OnPlayerConnectedClientRpc_patch.StartOfRoundInstance.allPlayerObjects[playerObjectNumber]);
            */

            
            Debug.Log($"Calling OnPlayerDC! playerObjectNumber: {playerObjectNumber}; clientId: {clientId}");
            if (!__instance.ClientPlayerList.ContainsKey(clientId))
            {
                Debug.Log("disconnect: clientId key already removed!");
                return false;
            }

            if (GameNetworkManager.Instance.localPlayerController != null && clientId == GameNetworkManager.Instance.localPlayerController.actualClientId)
            {
                Debug.Log("OnPlayerDC: Local client is disconnecting so return.");
                return false;
            }

            if (__instance.NetworkManager.ShutdownInProgress || NetworkManager.Singleton == null)
            {
                Debug.Log("Shutdown is in progress, returning");
                return false;
            }

            Debug.Log("Player DC'ing 2");
            if (__instance.IsServer && __instance.ClientPlayerList.TryGetValue(clientId, out var value))
            {
                HUDManager.Instance.AddTextToChatOnServer($"[playerNum{__instance.allPlayerScripts[value].playerClientId}] disconnected.");
            }

            if (!__instance.allPlayerScripts[playerObjectNumber].isPlayerDead)
            {
                __instance.livingPlayers--;
            }

            __instance.ClientPlayerList.Remove(clientId);
            __instance.connectedPlayersAmount--;
            Debug.Log("Player DC'ing 3");
            PlayerControllerB component = __instance.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            try
            {
                bool flag = !component.isPlayerDead;
                component.sentPlayerValues = false;
                component.isPlayerControlled = false;
                component.isPlayerDead = false;
                if (!__instance.inShipPhase)
                {
                    component.disconnectedMidGame = true;
                    if (__instance.livingPlayers == 0)
                    {
                        __instance.allPlayersDead = true;
                        __instance.ShipLeaveAutomatically();
                    }
                }

                component.DropAllHeldItems(itemsFall: true, disconnecting: true);
                Debug.Log("Teleporting disconnected player out");
                if (__instance.IsServer && flag)
                {
                    __instance.LocalPlayerDieEvent.Invoke(component, 200);
                }

                component.TeleportPlayer(__instance.notSpawnedPosition.position);
                UnlockableSuit.SwitchSuitForPlayer(component, 0, playAudio: false);
                if (GameNetworkManager.Instance.localPlayerController.isPlayerDead)
                {
                    HUDManager.Instance.UpdateBoxesSpectateUI();
                }

                Debug.Log($"Is networkmanager in shutdown?: {NetworkManager.Singleton.ShutdownInProgress}");
                if (NetworkManager.Singleton != null && !NetworkManager.Singleton.ShutdownInProgress && __instance.IsServer)
                {
                    component.gameObject.GetComponent<NetworkObject>().RemoveOwnership();
                }

                QuickMenuManager quickMenuManager = UnityEngine.Object.FindObjectOfType<QuickMenuManager>();
                if (quickMenuManager != null)
                {
                    quickMenuManager.RemoveUserFromPlayerList(playerObjectNumber);
                }
                Plugin.SetLobbyJoinable(joinable: true);
                Debug.Log($"Current players after dc: {__instance.connectedPlayersAmount}");
            }
            catch (Exception arg)
            {
                Debug.LogError($"Error while handling player disconnect!: {arg}");
                Plugin.LogException(arg);
            }
            return false;
        }
            //return true;
        //}

        [HarmonyPostfix]
        public static void Postfix(StartOfRound __instance, int playerObjectNumber, ulong clientId) {
            Debug.Log("Calling OnPlayerDC Postfix!");
            Plugin.SetLobbyJoinable(joinable: true);
        }
    }
}
