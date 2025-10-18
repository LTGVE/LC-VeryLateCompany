using HarmonyLib;

[HarmonyPatch(typeof(RoundManager), "SetToCurrentLevelWeather")]
internal static class SetToCurrentLevelWeather_patch
{
	[HarmonyPrefix]
	private static void Prefix()
	{
		if (WeatherSync.DoOverride)
		{
			RoundManager.Instance.currentLevel.currentWeather = WeatherSync.CurrentWeather;
			WeatherSync.DoOverride = false;
		}
	}
}
