using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace VeryLateCompany
{
	[BepInPlugin("McBowie.VeryLateCompany", "VeryLateCompany", "0.1.0")]
	internal class Plugin : BaseUnityPlugin
	{
		public static bool AllowJoiningWhileLanded = true;

		public static bool LobbyJoinable = true;

		public static Plugin Instance { get; private set; } = null;

		internal static ManualLogSource Logger { get; private set; } = null;

		internal static Harmony? Harmony { get; set; }

		private void Awake()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			Logger = BepInEx.Logging.Logger.CreateLogSource(MetadataHelper.GetMetadata(this).Name);
            Instance = this;
			Harmony harmony = new Harmony("McBowie.VeryLateCompany");
			harmony.PatchAll(typeof(Plugin).Assembly);
			Logger.LogInfo(" VeryLateCompany v0.1.0 v73 Fixed has loaded! Restorer : LT_GVE");
			Logger.LogWarning(
                "\n！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！" +
                "\nVeryLateCompany v0.1.0 v73 Fixed by LT_GVE" +
                "\nThis mod is Decompiled from  McBowie/VeryLateCompany Mod then fixed and re-built it." +
				"\n"+
				"\nJust for fun"+
				"\nIt maybe has some bugs, but it works fine for me and my friends." +
				"\nIf you have any problems, please create an issue on GitHub to let me know."+
				"\nDon't forget upload logs and your mod list,so that I can solve the problem more better."+
				"\nLink"+
                "\nhttps://github.com/LTGVE/LC-VeryLateCompany/issues" +
                "\n！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
		}

		public static void SetLobbyJoinable(bool joinable)
		{
			LobbyJoinable = joinable;
			GameNetworkManager.Instance.SetLobbyJoinable(joinable);
			QuickMenuManager quickMenuManager = Object.FindObjectOfType<QuickMenuManager>();
			if (quickMenuManager!=null)
			{
				quickMenuManager.inviteFriendsTextAlpha.alpha = (joinable ? 1f : 0.2f);
			}
		}

		internal static void Unpatch()
		{
			Logger.LogDebug((object)"Unpatching...");
			Harmony? harmony = Harmony;
			if (harmony != null)
			{
				harmony.UnpatchSelf();
			}
			Logger.LogDebug((object)"Finished unpatching!");
		}
	}
}
