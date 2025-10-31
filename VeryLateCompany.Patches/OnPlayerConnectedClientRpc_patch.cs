using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using McBowie.VeryLateCompany.VeryLateCompany.Patches;
using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.FastBufferWriter;

namespace VeryLateCompany.Patches
{
    [HarmonyDebug]

    [HarmonyPatch(typeof(StartOfRound), "OnPlayerConnectedClientRpc")]
	[HarmonyWrapSafe]
	internal class OnPlayerConnectedClientRpc_patch
	{
		public static FieldInfo RPCExecStage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo beginSendClientRpcMethod = typeof(NetworkBehaviour).GetMethod("__beginSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo endSendClientRpcMethod = typeof(NetworkBehaviour).GetMethod("__endSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);

		internal static Vector3 PreviousLocation = Vector3.zero;


		private static PlayerControllerB playerControllerB;

		private static GameObject playerObject;

		private static bool isPlayerDead = false;

		private static int playerObjectID = 1;

		private static StartOfRound startOfRound;

		private static bool wasPresentAtGameStart = false;

		public static StartOfRound StartOfRoundInstance = null;
        public static bool isClient = false;
        public static ulong currentClientId= 0;

        internal static void UpdateControlledState()
		{
			for (int i = 0; i < StartOfRound.Instance.connectedPlayersAmount + 1; i++)
			{
				if ((i == 0 || !((NetworkBehaviour)StartOfRound.Instance.allPlayerScripts[i]).IsOwnedByServer) && !StartOfRound.Instance.allPlayerScripts[i].isPlayerDead)
				{
					StartOfRound.Instance.allPlayerScripts[i].isPlayerControlled = true;
				}
			}
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Expected O, but got Unknown
			List<CodeInstruction> list = new List<CodeInstruction>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (!flag3)
				{
					if (!flag && instruction.opcode == OpCodes.Call && instruction.operand != null && instruction.operand.ToString() == "System.Collections.IEnumerator setPlayerToSpawnPosition(UnityEngine.Transform, UnityEngine.Vector3)")
					{
						flag = true;
					}
					else
					{
						if (flag && instruction.opcode == OpCodes.Ldc_I4_0)
						{
							flag2 = true;
							continue;
						}
						if (flag2 && instruction.opcode == OpCodes.Ldloc_0)
						{
							flag2 = false;
							flag3 = true;
							list.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(OnPlayerConnectedClientRpc_patch), "UpdateControlledState", new Type[0], (Type[])null)));
						}
					}
				}
				if (!flag2)
				{
					list.Add(instruction);
				}
			}
			if (!flag3)
			{
				Debug.LogError((object)"Failed to transpile StartOfRound::OnPlayerConnectedClientRpc");
			}
			return list.AsEnumerable();
		}

		[HarmonyPrefix]
		private static void Prefix(StartOfRound __instance, ulong clientId, int connectedPlayers, ulong[] connectedPlayerIdsOrdered, int assignedPlayerObjectId, int serverMoneyAmount, int levelID, int profitQuota, int timeUntilDeadline, int quotaFulfilled, int randomSeed)
		{
            PreviousLocation = __instance.allPlayerObjects[assignedPlayerObjectId].transform.position;
			throw new NullReferenceException("TEST");
		}

        [HarmonyPostfix]
		private static void Postfix(StartOfRound __instance, ulong clientId, int connectedPlayers, ulong[] connectedPlayerIdsOrdered, int assignedPlayerObjectId, int serverMoneyAmount, int levelID, int profitQuota, int timeUntilDeadline, int quotaFulfilled, int randomSeed)
		{
			try
			{
				Debug.Log($"Player is connected with ID: {clientId} has connected players: {connectedPlayers} and assigned player object ID: {assignedPlayerObjectId}");

				StartOfRoundInstance = __instance;
				startOfRound = __instance;
				playerObjectID = assignedPlayerObjectId;
				if (__instance.connectedPlayersAmount + 1 >= __instance.allPlayerScripts.Length)
				{
					Plugin.SetLobbyJoinable(joinable: false);
				}
				playerControllerB = __instance.allPlayerScripts[assignedPlayerObjectId];
				playerObject = __instance.allPlayerObjects[assignedPlayerObjectId];
				playerControllerB.DisablePlayerModel(__instance.allPlayerObjects[assignedPlayerObjectId], enable: true, disableLocalArms: true);
				__instance.livingPlayers = __instance.connectedPlayersAmount + 1;
				for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
				{
					PlayerControllerB playerControllerB2 = __instance.allPlayerScripts[i];
					if (playerControllerB2.isPlayerControlled && playerControllerB2.isPlayerDead)
					{
						__instance.livingPlayers--;
					}
				}
				Debug.Log($"inShipPhase: {__instance.inShipPhase}");


				if (__instance.IsServer && !__instance.inShipPhase)
				{
					RoundManager instance = RoundManager.Instance;
					ClientRpcParams clientRpcParams = default(ClientRpcParams);
					clientRpcParams.Send = new ClientRpcSendParams
					{
						TargetClientIds = new List<ulong> { clientId }
					};
					ClientRpcParams clientRpcParams2 = clientRpcParams;
					uint num = 3073943002u;
					FastBufferWriter fastBufferWriter = (FastBufferWriter)beginSendClientRpcMethod.Invoke(instance, new object[3] { num, clientRpcParams2, 0 });
					BytePacker.WriteValueBitPacked(fastBufferWriter, __instance.randomMapSeed);
					BytePacker.WriteValueBitPacked(fastBufferWriter, __instance.currentLevelID);
					BytePacker.WriteValueBitPacked(fastBufferWriter, __instance.currentLevel.moldSpreadIterations);
					BytePacker.WriteValueBitPacked(fastBufferWriter, __instance.currentLevel.moldStartPosition);
					Debug.Log($"Sending Level Info to client: Seed : {__instance.randomMapSeed} Level ID: {__instance.currentLevelID} Mold Spread Iterations: {__instance.currentLevel.moldSpreadIterations} Mold Start Position: {__instance.currentLevel.moldStartPosition}");


					MoldSpreadManager moldSpreadManager = UnityEngine.Object.FindObjectOfType<MoldSpreadManager>();
					bool value = moldSpreadManager != null;
					fastBufferWriter.WriteValueSafe(value);
					if (value)
					{
						fastBufferWriter.WriteValueSafe<int>(moldSpreadManager.planetMoldStates[StartOfRound.Instance.currentLevelID].destroyedMold.ToArray(), default(ForPrimitives));
					}
					BytePacker.WriteValueBitPacked(fastBufferWriter, (int)(instance.currentLevel.currentWeather + 255));
					endSendClientRpcMethod.Invoke(instance, new object[4] { fastBufferWriter, num, clientRpcParams2, 0 });
					uint num2 = 2729232387u;
					FastBufferWriter fastBufferWriter2 = (FastBufferWriter)beginSendClientRpcMethod.Invoke(instance, new object[3] { num2, clientRpcParams2, 0 });
					Debug.Log((object)("Sending weather to client: " + (int)(instance.currentLevel.currentWeather + 255)));
					endSendClientRpcMethod.Invoke(instance, new object[4] { fastBufferWriter2, num2, clientRpcParams2, 0 });
				}
				if (NetworkManager.Singleton.LocalClientId != clientId && !__instance.inShipPhase)
				{
					__instance.livingPlayers++;
					__instance.allPlayerScripts[0].playersManager.livingPlayers++;
				}
				
                if (__instance.IsClient && NetworkManager.Singleton.LocalClientId == clientId)
                {
                    RoundManager_Patch.isMidSessionJoiningRound = !StartOfRoundInstance.inShipPhase;
                }
            }
			catch (Exception e) { 
			Plugin.LogException(e);
			}
		}

		internal static IEnumerator KillPlayer(PlayerControllerB playerControllerB)
		{
			HUDManager.Instance.DisplayTip("Mid-game join", "You were not alive before reconnecting and will be killed in 5 seconds", isWarning: true);
			yield return (object)new WaitForSeconds(5f);
			playerControllerB.KillPlayer(Vector3.zero, spawnBody: false);
		}

		internal static IEnumerator TeleportToPreviousLocation(PlayerControllerB playerControllerB)
		{
			Debug.Log((object)("Waiting to teleport player: [" + playerControllerB.playerUsername + "]"));
			HUDManager.Instance.DisplayTip("Mid-game join", "You will be teleported back to your disconnect position in 5 seconds", isWarning: true);
			yield return (object)new WaitForSeconds(5f);
			playerControllerB.TeleportPlayer(PreviousLocation);
			Debug.Log((object)("Teleporting player [" + playerControllerB.playerUsername + "]"));
		}

		[HarmonyPatch("OnClientConnect")]
		[HarmonyPostfix]
		private static void OnClientConnectPatch(ulong clientId)
		{
			Debug.Log((object)($"Client {clientId} has connected"));
			isPlayerDead = playerControllerB?.isPlayerDead ?? false;
		}

		[HarmonyPatch(typeof(RoundManager))]
		[HarmonyPatch("GenerateNewFloor")]
		[HarmonyPostfix]
		private static void GenerateNewFloorPostfix(RoundManager __instance)
		{
			wasPresentAtGameStart = UnityEngine.Object.FindObjectOfType<StartMatchLever>().leverHasBeenPulled;
			/*
			if (!wasPresentAtGameStart)
			{
				IEnumerator corout = TeleportToPreviousLocation(startOfRound.allPlayerScripts[playerObjectID]);
				if (!isPlayerDead)
				{
					((MonoBehaviour)startOfRound).StartCoroutine(corout);
					return;
				}
				startOfRound.livingPlayers++;
				corout = KillPlayer(startOfRound.allPlayerScripts[playerObjectID]);
				((MonoBehaviour)startOfRound).StartCoroutine(corout);
				startOfRound.allPlayerScripts[playerObjectID].DisablePlayerModel(playerObject);
			}*/
		}
	}
}
