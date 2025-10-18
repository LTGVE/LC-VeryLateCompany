using HarmonyLib;
using VeryLateCompany;

[HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
internal static class OnShipLandedMiscEvents_patch
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		if (Plugin.AllowJoiningWhileLanded && StartOfRound.Instance.connectedPlayersAmount + 1 < StartOfRound.Instance.allPlayerScripts.Length)
		{
			Plugin.SetLobbyJoinable(joinable: true);
		}
	}
}
