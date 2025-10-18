using System;
using System.Reflection;
using HarmonyLib;
using McBowie.VeryLateCompany.VeryLateCompany.Patches;
using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.FastBufferWriter;

[HarmonyPatch(typeof(RoundManager), "__rpc_handler_3073943002")]
[HarmonyWrapSafe]
internal static class __rpc_handler_3073943002_patch
{
	public static FieldInfo RPCExecStage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);
	public static long lastTime = 0;

	[HarmonyPrefix]
	private static bool Prefix(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if (networkManager != null && networkManager.IsListening && !networkManager.IsHost)
		{
			try
            {
                Debug.Log($"Reading level info...");
				if (DateTime.Now.Ticks - lastTime > 10000000)
				{
					lastTime = DateTime.Now.Ticks;
				}
				else {
					Debug.LogWarning("Too frequent level info request, ignoring.");
					return false;
				
				}
					int randomSeed = default(int);
				ByteUnpacker.ReadValueBitPacked(reader, out randomSeed);
				int levelID = default(int);
				ByteUnpacker.ReadValueBitPacked(reader, out levelID);
				int moldIterations = default(int);
				ByteUnpacker.ReadValueBitPacked(reader, out moldIterations);
				int moldStartPosition = default(int);
				ByteUnpacker.ReadValueBitPacked(reader, out moldStartPosition);
				bool value5 = default(bool);
				 reader.ReadValueSafe<bool>(out value5);
                Debug.Log($"Read end! Level info: Seed: {randomSeed},ID: {levelID}, MoldIterations: {moldIterations}, ModStartPosition: {moldStartPosition}, Value5(Unknown value): {value5}");

                int[] syncDestroyedMold = null;
				if (value5)
				{
					reader.ReadValueSafe<int>(out syncDestroyedMold, default(ForPrimitives));
				}
				if (reader.Position < reader.Length)
				{
					int currentWeather = default(int);
					ByteUnpacker.ReadValueBitPacked(reader, out currentWeather);
					Debug.Log($"Current weather: {currentWeather}");
					currentWeather -= 255;
					if (currentWeather < 0)
					{
						currentWeather = -1;
					}
					WeatherSync.CurrentWeather = (LevelWeatherType)currentWeather;
					WeatherSync.DoOverride = true;
					
				}
				RoundManager.Instance.currentLevel.currentWeather = WeatherSync.CurrentWeather;
				RPCExecStage.SetValue(target, RpcEnum.Client);
				(target as RoundManager).GenerateNewLevelClientRpc(randomSeed, levelID, moldIterations, moldStartPosition, syncDestroyedMold);
				RPCExecStage.SetValue(target, RpcEnum.None);
				return false;
			}
			catch(Exception e)
			{ 
				Debug.LogError(e+"\n"+e.StackTrace);
				WeatherSync.DoOverride = false;
				reader.Seek(0);
				return true;
			}
		}
		return true;
	}
}
