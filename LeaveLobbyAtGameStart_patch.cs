using HarmonyLib;

[HarmonyPatch(typeof(GameNetworkManager), "LeaveLobbyAtGameStart")]
[HarmonyWrapSafe]
internal static class LeaveLobbyAtGameStart_patch
{
	[HarmonyPrefix]
	private static bool Prefix()
	{
		return false;
	}
}
