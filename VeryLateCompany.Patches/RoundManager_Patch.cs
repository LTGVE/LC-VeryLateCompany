using DunGen;
using HarmonyLib;
using System;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VeryLateCompany.Patches
{
    [HarmonyDebug]

    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManager_Patch
    {
        public static bool isMidSessionJoiningRound = false;


        public static FieldInfo __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);


        public static readonly MethodInfo __beginSendClientRpc = typeof(NetworkBehaviour).GetMethod("__beginSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);

        public static readonly MethodInfo __endSendClientRpc = typeof(NetworkBehaviour).GetMethod("__endSendClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);
        /*
        [HarmonyPatch("SetLockedDoors")]

        [HarmonyPrefix]
        private static bool SetLockedDoorsPrefix(RoundManager __instance, Vector3 mainEntrancePosition) {
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
        */
        
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPrefix]
        public static bool GenerateNewLevelClientRpc(RoundManager __instance, int randomSeed, int levelID, int moldIterations = 0, int moldStartPosition = 0, int[] syncDestroyedMold = null)
        {
            try
            {
                Debug.Log($"localPlayerController is Null ? {StartOfRound.Instance.localPlayerController == null}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("localPlayerController is Null!!!");
            }
            return true;//has been Fixed
            /*
            if (isMidSessionJoiningRound&&!__instance.IsServer) {
                lastRpcExecStage = (object)__rpc_exec_stage.GetValue(__instance);
                __rpc_exec_stage.SetValue(__instance, RpcEnum.Execute);
                changedRpcExecStage = true;
            }
            */
            #region lastChanged
            /*
            Debug.Log($"Generating new level. RPC Execute Stage: {__rpc_exec_stage.GetValue(__instance).ToString()} {(int)__rpc_exec_stage.GetValue(__instance)}");

            NetworkManager networkManager = __instance.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                Debug.LogWarning("NetworkManager is not ready. Cannot generate new level.");
                return false;
            }

            if ((int)__rpc_exec_stage.GetValue(__instance) != (int)(RpcEnum.Execute) && (networkManager.IsServer || networkManager.IsHost))
            {
                Debug.Log("Sending level data to clients.");
                ClientRpcParams clientRpcParams = default(ClientRpcParams);
                FastBufferWriter bufferWriter = (FastBufferWriter)__beginSendClientRpc.Invoke(__instance, new object[] { 3073943002u, clientRpcParams, RpcDelivery.Reliable });
                BytePacker.WriteValueBitPacked(bufferWriter, randomSeed);
                BytePacker.WriteValueBitPacked(bufferWriter, levelID);
                BytePacker.WriteValueBitPacked(bufferWriter, moldIterations);
                BytePacker.WriteValueBitPacked(bufferWriter, moldStartPosition);
                bool value = syncDestroyedMold != null;
                bufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
                if (value)
                {
                    bufferWriter.WriteValueSafe(syncDestroyedMold, default(FastBufferWriter.ForPrimitives));
                }
                Debug.Log("Write data in buffer.");
                var Params = new object[] { bufferWriter, 3073943002u, clientRpcParams, RpcDelivery.Reliable };
                try
                {
                    __endSendClientRpc.Invoke(__instance, Params);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                Debug.Log("Sent level data to clients.");
            }

            if ((int)__rpc_exec_stage.GetValue(__instance) != (int)RpcEnum.Execute || (!networkManager.IsClient && !networkManager.IsHost))
            {
                Debug.LogWarning($"Cannot generate new level. Not on client or server.rpc exec stage : {(int)__rpc_exec_stage.GetValue(__instance) != (int)RpcEnum.Execute}+{__rpc_exec_stage.GetValue(__instance).ToString()}\nis client : {networkManager.IsClient}\nis host : {networkManager.IsHost}");
                return false;
            }*/
            #endregion

            /*
            NetworkManager networkManager = __instance.NetworkManager;
            if ((object)networkManager == null || !networkManager.IsListening)
            {
                Debug.LogWarning("NetworkManager is not ready. Cannot generate new level.");
                return false;
            }



            if (!__rpc_exec_stage.GetValue(__instance).Equals(RpcEnum.Execute) && (networkManager.IsServer || networkManager.IsHost))
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
            }*/
            #region lastChanged
            /*
            Debug.Log("Generating Others.");
            __rpc_exec_stage.SetValue(__instance, (int)RpcEnum.Send);
            __instance.GetOutsideAINodes(getUnderwaterNodes: false);
            __instance.currentLevel.moldSpreadIterations = moldIterations;
            __instance.currentLevel.moldStartPosition = moldStartPosition;
            if (moldIterations > 0)
            {
                if (moldStartPosition >= __instance.outsideAINodes.Length)
                {
                    Debug.LogError($"Mold error: Mold start position index {moldStartPosition} is greater than outsideAINodes count: {__instance.outsideAINodes.Length}! Cannot sync mold");
                }

                Vector3 position = __instance.outsideAINodes[Mathf.Min(moldStartPosition, __instance.outsideAINodes.Length - 1)].transform.position;
                if (syncDestroyedMold != null)
                {
                    UnityEngine.Object.FindObjectOfType<MoldSpreadManager>().SyncDestroyedMoldPositions(syncDestroyedMold);
                }

                UnityEngine.Object.FindObjectOfType<MoldSpreadManager>().GenerateMold(position, moldIterations);
            }

            Debug.Log($"SetPlayerManagerRandomSeed: {randomSeed} playerManager is Null ? {__instance.playersManager == null}");
            __instance.playersManager.randomMapSeed = randomSeed;
            __instance.currentLevel = __instance.playersManager.levels[levelID];
            try
            {
                Debug.Log($"localPlayerController is Null ? {StartOfRound.Instance.localPlayerController == null}");
            }
            catch (Exception e) { 
                Debug.LogException(e);
            }
            Debug.Log($"RANDOM MAP SEED - {__instance.playersManager.randomMapSeed}\nMoon: {__instance.currentLevel.PlanetName}");
            Debug.Log("Initializing random number generators.");
            __instance.InitializeRandomNumberGenerators();
            __instance.Invoke("SetChallengeFileRandomModifiers", 0);
            HUDManager.Instance.loadingText.text = $"Random seed: {randomSeed}";
            HUDManager.Instance.LoadingScreen.SetBool("IsLoading", true);
            __instance.dungeonCompletedGenerating = false;
            __instance.mapPropsContainer = GameObject.FindGameObjectWithTag("MapPropsContainer");
            GameObject gameObject = GameObject.FindGameObjectWithTag("SpecialStartRoomBounds");
            if (gameObject != null)
            {
                __instance.startRoomSpecialBounds = gameObject.GetComponent<Collider>();
            }

            if (!__instance.currentLevel.spawnEnemiesAndScrap)
            {
                return false;
            }

            __instance.dungeonGenerator = UnityEngine.Object.FindObjectOfType<RuntimeDungeon>(includeInactive: false);
            if (__instance.dungeonGenerator != null)
            {
                __instance.dungeonGenerator.Generator.GenerateAsynchronously = true;
                __instance.dungeonGenerator.Generator.MaxAsyncFrameMilliseconds = 1f;
                __instance.dungeonGenerator.Generator.PauseBetweenRooms = 0f;
                __instance.GenerateNewFloor();
                if ((int)__instance.dungeonGenerator.Generator.Status == 8)
                {
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
            }
            else
            {
                Debug.LogError($"This client could not find dungeon generator! scene count: {SceneManager.sceneCount}");
            }
            Debug.Log("Generating complete.");*/
            #endregion
            /*
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
            __instance.InitializeRandomNumberGenerators();
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
                    var Generator_OnGenerationStatusChanged =(GenerationStatusDelegate) __instance.GetType().GetMethod("Generator_OnGenerationStatusChanged", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate(typeof(GenerationStatusDelegate));

                    __instance.dungeonGenerator.Generator.OnGenerationStatusChanged += Generator_OnGenerationStatusChanged;
                    Debug.Log("Now listening to dungeon generator status.");
                }
                Debug.Log("Dungeon generation complete.");
            }
            else
            {
                Debug.LogError($"This client could not find dungeon generator! scene count: {SceneManager.sceneCount}");
            }
            */
            //return false;
            return false;
        }
        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        public static void GenerateNewLevelClientRpc_Postfix(RoundManager __instance, int randomSeed, int levelID, int moldIterations = 0, int moldStartPosition = 0, int[] syncDestroyedMold = null) {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(false);
        }

    }
}

