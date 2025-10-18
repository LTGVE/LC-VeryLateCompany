using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace VeryLateCompany.Patches
{
	[HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
	internal class OnPlayerDC_patch
	{
		[HarmonyPrefix]
		private static bool Prefix(StartOfRound __instance, int playerObjectNumber, ulong clientId)
		{
			Debug.Log((object)"Calling OnPlayerDC!");
			if (!__instance.ClientPlayerList.ContainsKey(clientId))
			{
				Debug.Log((object)"disconnect: clientId key already removed!");
				return false;
			}
			if ((Object)(object)GameNetworkManager.Instance.localPlayerController != (Object)null && clientId == GameNetworkManager.Instance.localPlayerController.actualClientId)
			{
				Debug.Log((object)"OnPlayerDC: Local client is disconnecting so return.");
				return false;
			}
			if (((NetworkBehaviour)__instance).NetworkManager.ShutdownInProgress || (Object)(object)NetworkManager.Singleton == (Object)null)
			{
				Debug.Log((object)"Shutdown is in progress, returning");
				return false;
			}
			Debug.Log((object)"Player DC'ing 2");
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
			Debug.Log((object)"Player DC'ing 3");
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
