using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace VeryLateCompany.VeryLateCompany.Patches
{
   // [HarmonyDebug]
    //[HarmonyPatch(typeof(NetworkConnectionManager))]
    public class NetcodeConnectionManager_patch
    {
        public static FieldInfo f_LocalClient = typeof(NetworkConnectionManager).GetField("LocalClient", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo m_SetRole = typeof(NetworkClient).GetMethod("SetRole", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo f_NetworkManager = typeof(NetworkConnectionManager).GetField("NetworkManager", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo f_ConnectedClients = typeof(NetworkConnectionManager).GetField("ConnectedClients", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo f_ConnectedClientIds = typeof(NetworkConnectionManager).GetField("ConnectedClientIds", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo f_ConnectedClientsList = typeof(NetworkConnectionManager).GetField("ConnectedClientsList", BindingFlags.NonPublic | BindingFlags.Instance);
        public static Type t_clientConnectedMessage = AccessTools.TypeByName("Unity.Netcode.ClientConnectedMessage");

        public static FieldInfo f_MessageManager = typeof(NetworkManager).GetField("MessageManager", BindingFlags.NonPublic | BindingFlags.Instance);
        [HarmonyPatch("AddClient")]
        [HarmonyPrefix]
        public static bool AddClient(NetworkConnectionManager __instance, ulong clientId,ref NetworkClient __result) {
            /*
            Debug.LogError($"Trying to add a client.This is a debug log.Don't worry about it.");
            try
            {
                var connectedClients = (Dictionary<ulong, NetworkClient>)f_ConnectedClients.GetValue(__instance);
                var connectedClientsList = (List<NetworkClient>)f_ConnectedClientsList.GetValue(__instance);
                var connectedClientIds = (List<ulong>)f_ConnectedClientIds.GetValue(__instance);
                NetworkClient localClient = (NetworkClient)f_LocalClient.GetValue(__instance);
                localClient = new NetworkClient();
                if (connectedClients.ContainsKey(clientId) || connectedClientIds.Contains(clientId) || connectedClientsList.Contains(localClient))
                {
                    Debug.LogError($"Client with id {clientId} already exists in the list of connected clients. This is a critical error.Try to use already existing client instead. In connectedDic? {connectedClients.ContainsKey(clientId)} In connectedClientIds? {connectedClientIds.Contains(clientId)} In connectedClientsList? {connectedClientsList.Contains(localClient)}");
                    localClient = connectedClients[clientId];
                    __result = localClient;
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Plugin.LogException(e);
            }
            /*
            Debug.Log($"Creating new local client with id {clientId}");
            var networkManager = f_NetworkManager.GetValue(__instance) as NetworkManager;
            m_SetRole.Invoke(localClient, new object[] { clientId == 0, networkManager.IsClient, f_NetworkManager.GetValue(__instance) as NetworkManager });
            localClient.ClientId = clientId;
            connectedClients.Add(clientId, localClient);
            f_ConnectedClients.SetValue(__instance, connectedClients);
            connectedClientsList.Add(localClient);
            f_ConnectedClientsList.SetValue(__instance, connectedClientsList);
            connectedClientIds.Add(clientId);
            f_ConnectedClientIds.SetValue(__instance, connectedClientIds);
            /*
            if (clientId != 0L)
            {
                Debug.Log($"Client {clientId} connected. Starting to send ClientConnectedMessage to other clients");
                var messageType = t_clientConnectedMessage.GetType();
                Debug.Log($"Created new ClientConnectedMessage of type {messageType.ToString()}");
                var clientConnectedMessage = Activator.CreateInstance(t_clientConnectedMessage);
                Debug.Log($"Created new ClientConnectedMessage instance is Null? {clientConnectedMessage == null}");
                FieldInfo clientConnectedMessageClientId = t_clientConnectedMessage.GetField("ClientId", BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.Public| BindingFlags.GetProperty);
                Debug.Log($"Got ClientConnectedMessage ClientId property. is Null? {clientConnectedMessageClientId == null}");
                clientConnectedMessageClientId.SetValue(clientConnectedMessage, clientId);
                Debug.Log($"Set Client Connected Message ClientId to {clientId}");
                object message = clientConnectedMessage;
                var connectedclientIds = (List<ulong>)f_ConnectedClientIds.GetValue(__instance);
                Debug.Log($"Sending ClientConnectedMessage to {connectedclientIds.Count} clients");
                f_MessageManager.GetType().GetMethod("SendMessage")?.Invoke(f_MessageManager.GetValue(__instance), new object[] { message, NetworkDelivery.ReliableFragmentedSequenced, connectedclientIds } );
            }
            Debug.Log($"Successfully added client {clientId} to the list of connected clients");
            __result = localClient;
            */
            return true;
        }
        /*
        public static void RefreshClientList(NetworkConnectionManager __instance) {
            var connectedClients = (Dictionary<ulong, NetworkClient>)f_ConnectedClients.GetValue(__instance);
            var connectedClientsList = (List<NetworkClient>)f_ConnectedClientsList.GetValue(__instance);
            var connectedClientIds = (List<ulong>)f_ConnectedClientIds.GetValue(__instance);
            foreach (var key in connectedClients.Keys) {
                var client = connectedClients[key];
            }

        }*/
    }
}
