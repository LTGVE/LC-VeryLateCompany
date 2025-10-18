using HarmonyLib;
using VeryLateCompany;

[HarmonyPatch(typeof(QuickMenuManager), "InviteFriendsButton")]
internal static class InviteFriendsButton_patch
{
	[HarmonyPrefix]
	private static bool Prefix()
	{
		if (Plugin.LobbyJoinable && !GameNetworkManager.Instance.disableSteam)
		{
			GameNetworkManager.Instance.InviteFriendsUI();
		}
		return false;
	}
}
