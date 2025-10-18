using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace VeryLateCompany.Patches
{
	[HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
	internal class ChatCommands
	{
		private static List<string> commands = new List<string> { "allowJoin", "openLobby", "return", "leave", "entrance", "noclip", "help" };

		private static string msg;

		private static bool noClipState = false;

		private static float noClipSpeed = 0.1f;

		[HarmonyPrefix]
		private static void Prefix()
		{
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			string command = HUDManager.Instance.chatTextField.text.Split(' ')[0];
			if ( !command.StartsWith("!")||!StartOfRound.Instance.IsHost)
			{
				return;
			}
			command = command.Trim('!');
			if (!commands.Contains(command))
			{
				return;
			}
			switch (command)
			{
			case "allowJoin":
			case "openLobby":
				msg = "Opening the lobby for joining";
				Plugin.SetLobbyJoinable(joinable: true);
				break;
			case "return":
			case "leave":
				msg = "Sending the ship out into orbit";
				StartOfRound.Instance.ShipLeaveAutomatically();
				break;
			case "entrance":
			{
				string[] fullCommand = HUDManager.Instance.chatTextField.text.Split(' ');
				if (fullCommand.Length != 2)
				{
					break;
				}
				string name = fullCommand[1];
				PlayerControllerB player = Array.Find(StartOfRound.Instance.allPlayerScripts, (PlayerControllerB x) => x.playerUsername == name);
				if (player == null)
				{
					HUDManager.Instance.AddTextToChatOnServer("Can't find player: " + name);
					break;
				}
				msg = "Teleporting " + name + " to the building's entrace";
				EntranceTeleport[] entrances = UnityEngine.Object.FindObjectsByType<EntranceTeleport>(0);
				EntranceTeleport[] array = entrances;
				foreach (EntranceTeleport entrance in array)
				{
					if (entrance.entranceId == 0 && entrance.isEntranceToBuilding)
					{
						GameNetworkManager.Instance.localPlayerController.TeleportPlayer(entrance.entrancePoint.position);
						GameNetworkManager.Instance.localPlayerController.isInElevator = false;
						GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom = false;
						GameNetworkManager.Instance.localPlayerController.isInsideFactory = false;
						entrance.TeleportPlayerServerRpc((int)player.playerClientId);
						break;
					}
				}
				break;
			}
			case "noclip":
					if (!StartOfRound.Instance.IsHost) { HUDManager.Instance.DisplayTip("Warning", "Only the host can use this command.", true); break; }
				msg = (noClipState ? "Disabling" : "Enabling") + " noclip for " + StartOfRound.Instance.localPlayerController.playerUsername;
				noClipState = !noClipState;
				StartOfRound.Instance.localPlayerController.playerCollider.enabled = !noClipState;
				break;
			case "help":
				msg = StartOfRound.Instance.localPlayerController.playerUsername + " needs a teleport";
				break;
			}
		}

		[HarmonyPostfix]
		private static void Postfix()
		{
			if (!(msg == string.Empty))
			{
				HUDManager.Instance.AddTextToChatOnServer(msg);
				msg = string.Empty;
			}
		}

		[HarmonyPatch(typeof(PlayerControllerB), "Update")]
		[HarmonyPostfix]
		private static void UpdatePostfix(PlayerControllerB __instance)
		{
			if (noClipState)
			{
				NoClipMovement();
			}
		}

		private static void NoClipMovement()
		{

			PlayerControllerB playerController = StartOfRound.Instance.localPlayerController;
			try
			{
				if (((ButtonControl)Keyboard.current.wKey).isPressed)
				{
					Quaternion rotation = playerController.transform.rotation;
					Vector3 val = rotation * Vector3.forward;
					Transform transform = playerController.transform;
					transform.position += val * noClipSpeed;
				}
			}
			catch
			{
			}
			try
			{
				if (((ButtonControl)Keyboard.current.aKey).isPressed)
				{
					Quaternion rotation2 = ((Component)playerController).transform.rotation;
					Quaternion val2 = Quaternion.AngleAxis(-90f, Vector3.up);
					Vector3 val3 = val2 * rotation2 * Vector3.forward;
					Transform transform2 = ((Component)playerController).transform;
					transform2.position += val3 * noClipSpeed;
				}
			}
			catch
			{
			}
			try
			{
				if (((ButtonControl)Keyboard.current.dKey).isPressed)
				{
					Quaternion rotation3 = ((Component)playerController).transform.rotation;
					Quaternion val4 = Quaternion.AngleAxis(90f, Vector3.up);
					Vector3 val5 = val4 * rotation3 * Vector3.forward;
					Transform transform3 = ((Component)playerController).transform;
					transform3.position += val5 * noClipSpeed;
				}
			}
			catch
			{
			}
			try
			{
				if (((ButtonControl)Keyboard.current.sKey).isPressed)
				{
					Quaternion rotation4 = ((Component)playerController).transform.rotation;
					Vector3 val6 = rotation4 * Vector3.back;
					Transform transform4 = ((Component)playerController).transform;
					transform4.position += val6 * noClipSpeed;
				}
			}
			catch
			{
			}
			try
			{
				if (((ButtonControl)Keyboard.current.spaceKey).isPressed)
				{
					Vector3 up = Vector3.up;
					Transform transform5 = ((Component)playerController).transform;
					transform5.position += up * noClipSpeed;
				}
			}
			catch
			{
			}
			try
			{
				if (Keyboard.current.ctrlKey.isPressed)
				{
					Vector3 val7 = -Vector3.up;
					Transform transform6 = ((Component)playerController).transform;
					transform6.position += val7 * noClipSpeed;
				}
			}
			catch
			{
			}
		}
	}
}
