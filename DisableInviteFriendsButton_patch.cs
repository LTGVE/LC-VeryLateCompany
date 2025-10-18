using HarmonyLib;

[HarmonyPatch(typeof(QuickMenuManager), "DisableInviteFriendsButton")]
internal static class DisableInviteFriendsButton_patch
{
	[HarmonyPrefix]
	private static bool Prefix()
	{
		return false;
	}
}
