using HarmonyLib;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace VeryLateCompany.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRound_patch
    {

        public static FieldInfo __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);
        public static MethodInfo OnPlayerConnectedClientRpc = typeof(StartOfRound).GetMethod("OnPlayerConnectedClientRpc", BindingFlags.Instance | BindingFlags.NonPublic);

        /*
        [HarmonyPatch("EndGameServerRpc")]
        [HarmonyPrefix]
        public static void EndGameServerRpc(StartOfRound __instance) {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(true);

        }
        [HarmonyPatch("EndGameClientRpc")]
        [HarmonyPrefix]
        public static void EndGameClientRpc(StartOfRound __instance)
        {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(true);
        }*/
        [HarmonyPatch("ShipLeave")]
        [HarmonyPrefix]
        public static bool ShipLeave()
        {
            GameObject.Find("Environment/SpaceProps/Planets").SetActive(true);
            return true;
        }

        [HarmonyPatch("__rpc_handler_886676601")]
        [HarmonyPrefix]
        public static bool __rpc_handler_886676601(NetworkBehaviour target,FastBufferReader reader,__RpcParams rpcParams) {
            NetworkManager networkManager = target.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                return false;
            }
            ulong num;
            ByteUnpacker.ReadValueBitPacked(reader, out num);

            Debug.Log($"Player with Client Id {num},{target.OwnerClientId} connected to the round");

            int num2;
            ByteUnpacker.ReadValueBitPacked(reader, out num2);
            bool flag;
            reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
            ulong[] array = null;
            if (flag)
            {
                reader.ReadValueSafe<ulong>(out array, default(FastBufferWriter.ForPrimitives));
            }
            int num3;
            ByteUnpacker.ReadValueBitPacked(reader, out num3);
            int num4;
            ByteUnpacker.ReadValueBitPacked(reader, out num4);
            int num5;
            ByteUnpacker.ReadValueBitPacked(reader, out num5);
            int num6;
            ByteUnpacker.ReadValueBitPacked(reader, out num6);
            int num7;
            ByteUnpacker.ReadValueBitPacked(reader, out num7);
            int num8;
            ByteUnpacker.ReadValueBitPacked(reader, out num8);
            int num9;
            ByteUnpacker.ReadValueBitPacked(reader, out num9);
            bool flag2;
            reader.ReadValueSafe<bool>(out flag2, default(FastBufferWriter.ForPrimitives));
            __rpc_exec_stage.SetValue(target, RpcEnum.Execute);
            OnPlayerConnectedClientRpc.Invoke (target,new object[] { num, num2, array, num3, num4, num5, num6, num7, num8, num9, flag2 });
            __rpc_exec_stage.SetValue(target, RpcEnum.Send);
            return false;
        }

    }
}
