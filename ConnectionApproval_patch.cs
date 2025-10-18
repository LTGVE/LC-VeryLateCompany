using HarmonyLib;
using Unity.Netcode;
using static Unity.Netcode.NetworkManager;

[HarmonyPatch(typeof(GameNetworkManager), "ConnectionApproval")]
[HarmonyWrapSafe]
internal static class ConnectionApproval_patch
{
	[HarmonyPostfix]
	private static void Postfix(ref ConnectionApprovalRequest request, ref ConnectionApprovalResponse response)
	{
		if (request.ClientNetworkId != NetworkManager.Singleton.LocalClientId && response.Reason == "Game has already started!")
		{
			response.Reason = "";
			response.Approved = true;
		}
	}
}
