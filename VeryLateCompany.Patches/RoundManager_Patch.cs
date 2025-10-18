using DunGen;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace McBowie.VeryLateCompany.VeryLateCompany.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManager_Patch
    {
        public static FieldInfo __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);
        public static Type __RpcExecStage = typeof(NetworkBehaviour).GetNestedType("__RpcExecStage", BindingFlags.Instance | BindingFlags.NonPublic);
        public static System.Random levelRandom;


        private static readonly MethodInfo __beginSendClientRpc = typeof(NetworkBehaviour).GetMethod("__beginSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo __endSendClientRpc = typeof(NetworkBehaviour).GetMethod("__endSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);
        [HarmonyPatch("SetLockedDoors")]

        [HarmonyPrefix]
        private static bool SetLockedDoorsPrefix(RoundManager __instance, Vector3 mainEntrancePosition) {
            levelRandom = new System.Random(StartOfRound.Instance.randomMapSeed);
            Debug.Log("Setting locked doors for round.");
            if (__instance.LevelRandom == null)
            {
                Debug.LogError($"LevelRandom is null. Trying to InitalizeLevelRandom .");
                __instance.InitializeRandomNumberGenerators();
            }
            if (__instance.LevelRandom == null) {
                Debug.LogError($"LevelRandom is still null. Cannot lock doors.");
                return false;
            }
            return true;
            
        }
        [HarmonyPatch("SetLockedDoors")]
        [HarmonyPostfix]
        private static void SetLockedDoors_Postfix(RoundManager __instance, Vector3 mainEntrancePosition) {
            Debug.Log("Setting end locked doors for round.");
        }
        [HarmonyPatch("InitializeRandomNumberGenerators")]
        [HarmonyPrefix]
        public static bool InitializeRandomNumberGenerators(RoundManager __instance)
        {
            var roundSeed = StartOfRound.Instance.randomMapSeed;
            Debug.Log("Initializing random number generators.");
            SoundManager.Instance.InitializeRandom();
            __instance.LevelRandom = new System.Random(roundSeed);
            Debug.Log("LevelRandom seed : " + roundSeed);
            __instance.AnomalyRandom = new System.Random(roundSeed + 5);
            Debug.Log("AnomalyRandom seed : " + (roundSeed + 5));
            __instance.EnemySpawnRandom = new System.Random(roundSeed + 40);
            Debug.Log("EnemySpawnRandom seed : " + (roundSeed + 40));
            __instance.OutsideEnemySpawnRandom = new System.Random(roundSeed + 41);
            Debug.Log("OutsideEnemySpawnRandom seed : " + (roundSeed + 41));
            __instance.BreakerBoxRandom = new System.Random(roundSeed + 20);
            Debug.Log("BreakerBoxRandom seed : " + (roundSeed + 20));
            return false;
        }

        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPrefix]
        public static bool GenerateNewLevelClientRpc(RoundManager __instance,int randomSeed, int levelID, int moldIterations = 0, int moldStartPosition = 0, int[] syncDestroyedMold = null)
        {

            object executeStage = Enum.Parse(__RpcExecStage, "Execute");
            object sendStage = Enum.Parse(__RpcExecStage, "Send");
            NetworkManager networkManager = __instance.NetworkManager;
            if ((object)networkManager == null || !networkManager.IsListening)
            {
                Debug.LogWarning("NetworkManager is not ready. Cannot generate new level.");
                return false;
            }

            if (!__rpc_exec_stage.GetValue(__instance).Equals(executeStage) && (networkManager.IsServer || networkManager.IsHost))
            {
                ClientRpcParams clientRpcParams = default(ClientRpcParams);
                FastBufferWriter bufferWriter =(FastBufferWriter)__beginSendClientRpc.Invoke(__instance,new object[] { 3073943002u, clientRpcParams, RpcDelivery.Reliable });
                BytePacker.WriteValueBitPacked(bufferWriter, randomSeed);
                BytePacker.WriteValueBitPacked(bufferWriter, levelID);
                BytePacker.WriteValueBitPacked(bufferWriter, moldIterations);
                BytePacker.WriteValueBitPacked(bufferWriter, moldStartPosition);
                bool value = syncDestroyedMold != null;
                bufferWriter.WriteValueSafe(value, default(FastBufferWriter.ForPrimitives));
                if (value)
                {
                    bufferWriter.WriteValueSafe(syncDestroyedMold, default(FastBufferWriter.ForPrimitives));
                }

                __endSendClientRpc.Invoke(__instance,new object[] { bufferWriter, 3073943002u, clientRpcParams, RpcDelivery.Reliable });
            }
            /*
            if (!__rpc_exec_stage.GetValue(__instance).Equals(executeStage)|| (!networkManager.IsClient && !networkManager.IsHost))
            {
                Debug.LogWarning($"Cannot generate new level. Not on client or server.rpc exec stage : {__rpc_exec_stage.GetValue(__instance).ToString()}\nis client : {networkManager.IsClient}\nis host : {networkManager.IsHost}");
                return false;
            }
            */
            __rpc_exec_stage.SetValue(__instance, sendStage);
            __instance.outsideAINodes = (from x in GameObject.FindGameObjectsWithTag("OutsideAINode")
                              orderby Vector3.Distance(x.transform.position, StartOfRound.Instance.elevatorTransform.position)
                              select x).ToArray();
            __instance.currentLevel.moldSpreadIterations = moldIterations;
            __instance.currentLevel.moldStartPosition = moldStartPosition;
            if (moldIterations > 0)
            {
                Vector3 position = __instance.outsideAINodes[Mathf.Min(moldStartPosition, __instance.outsideAINodes.Length - 1)].transform.position;
                if (syncDestroyedMold != null)
                {
                    UnityEngine.Object.FindObjectOfType<MoldSpreadManager>().SyncDestroyedMoldPositions(syncDestroyedMold);
                }

                UnityEngine.Object.FindObjectOfType<MoldSpreadManager>().GenerateMold(position, moldIterations);
            }

            __instance.playersManager.randomMapSeed = randomSeed;
            __instance.currentLevel = __instance.playersManager.levels[levelID];
            InitializeRandomNumberGenerators(__instance);
            __instance.Invoke("SetChallengeFileRandomModifiers",0);
            HUDManager.Instance.loadingText.text = $"Random seed: {randomSeed}";
            HUDManager.Instance.loadingDarkenScreen.enabled = true;
            __instance.dungeonCompletedGenerating = false;
            __instance.mapPropsContainer = GameObject.FindGameObjectWithTag("MapPropsContainer");
            if (!__instance.currentLevel.spawnEnemiesAndScrap)
            {
                Debug.Log("Not spawning enemies and scrap.");
                return  false;
            }

            __instance.dungeonGenerator = UnityEngine.Object.FindObjectOfType<RuntimeDungeon>(includeInactive: false);
            if (__instance.dungeonGenerator != null)
            {
                Debug.Log("Dungeon generator found.");
                __instance.GenerateNewFloor();
                Debug.Log("Dungeon generator generated new floor.");
                if (__instance.dungeonGenerator.Generator.Status == GenerationStatus.Complete)
                {
                    Debug.Log("Dungeon finished generating.");
                    __instance.Invoke("FinishGeneratingLevel", 0);
                    Debug.Log("Dungeon finished generating in one frame.");
                }
                else
                {
                    Debug.Log("Dungeon generator is not complete. Listening to status changes.");
                    var Generator_OnGenerationStatusChanged = __instance.GetType().GetMethod("Generator_OnGenerationStatusChanged", BindingFlags.Instance | BindingFlags.NonPublic);
                    var eventhandler = (GenerationStatusDelegate)Delegate.CreateDelegate(
                        typeof(GenerationStatusDelegate),
                        __instance,
                        Generator_OnGenerationStatusChanged
                        );
                    __instance.dungeonGenerator.Generator.OnGenerationStatusChanged += eventhandler;
                    Debug.Log("Now listening to dungeon generator status.");
                }
                Debug.Log("Dungeon generation complete.");
                GameObject.Find("Environment/SpaceProps/Planets").SetActive(false);
            }
            else
            {
                Debug.LogError($"This client could not find dungeon generator! scene count: {SceneManager.sceneCount}");
            }
            return false;
        }
    }
}
