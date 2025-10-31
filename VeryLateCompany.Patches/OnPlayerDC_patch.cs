using GameNetcodeStuff;
using HarmonyLib;
using McBowie.VeryLateCompany.VeryLateCompany.Patches;
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
		{
			if (clientId==OnPlayerConnectedClientRpc_patch.currentClientId&&!NetworkManager.Singleton.IsServer) {
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
			return false;
		}
	}
}
