using HarmonyLib;
using VeryLateCompany;

[HarmonyPatch(typeof(StartOfRound), "StartGame")]
internal static class StartGame_patch
{
	[HarmonyPrefix]
	private static void Prefix()
	{
		Plugin.SetLobbyJoinable(joinable: false);
	}

	[HarmonyPostfix]
	private static void Postfix()
	{
		Plugin.SetLobbyJoinable(joinable: true);
	}
}
