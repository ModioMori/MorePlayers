using GladioMoriMorePlayers;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib.Tools;

[assembly:MelonInfo(typeof(MorePlayersMod), "Gladio More Players", "1.1.0", "checkraisefold")]
[assembly:MelonGame("Plebeian Studio", "Gladio Mori")]

namespace GladioMoriMorePlayers {
	public class MorePlayersMod : MelonMod {
		public static MorePlayersMod? instance;

		private MelonPreferences_Category? modPrefs;
		public MelonPreferences_Entry<int>? maxPlayers;
		private MelonPreferences_Entry<string>? openMenuBind;
		public MelonPreferences_Entry<bool>? randomizeSpawns;

		private List<MultiplayerRoomPlayer>? currentPlayers;
		public Dictionary<uint, bool> currentSpectators = new Dictionary<uint, bool>();
		private float nextPlayerFetchTime = 0;
		private bool uiOpen = false;

		public override void OnEarlyInitializeMelon() {
			instance = this;
		}

		public override void OnInitializeMelon() {
			modPrefs = MelonPreferences.CreateCategory("MorePlayersPrefs", "Main Preferences");
			maxPlayers =
			    modPrefs.CreateEntry<int>("maxPlayers", 16, "Maximum Players",
			                              "Maximum amount of players that can join your server.");
			openMenuBind = modPrefs.CreateEntry<string>(
			    "openMenuBind", "f6", "Open Manager Bind",
			    "Keybind to open the manager UI. See 'Mapping virtual axes to controls' on https://docs.unity3d.com/Manual/class-InputManager.html for key names.");
			randomizeSpawns = modPrefs.CreateEntry<bool>(
			    "randomizeSpawns", false, "Randomize Spawns",
			    "Whether player's spawns should be randomized every round. true or false.");

			LoggerInstance.Msg("More Players has initialized!");
		}

		public override void OnGUI() {
			if (!uiOpen) {
				return;
			}
			if (maxPlayers == null || randomizeSpawns == null) {
				return;
			}
			GUILayout.BeginArea(new Rect(Screen.width / 2 - Screen.width / 8, 0, Screen.width / 4,
			                             Screen.height / 2));
			GUILayout.BeginVertical();
			GUILayout.Box("More Players Manager");

			GUILayout.BeginHorizontal();
			GUILayout.Box($"Maximum Players: {maxPlayers.Value}\n(restart lobby after change)");
			maxPlayers.Value = (int)GUILayout.HorizontalSlider(maxPlayers.Value, 1.0f, 16.0f);
			GUILayout.EndHorizontal();

			randomizeSpawns.Value = GUILayout.Toggle(randomizeSpawns.Value, "Randomize Spawns");

			if (currentPlayers == null || currentPlayers.Count == 0) {
				GUILayout.EndVertical();
				GUILayout.EndArea();
				return;
			}

			foreach (MultiplayerRoomPlayer player in currentPlayers) {
				if (player == null || player.disconnecting) {
					continue;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Box(
				    $"Player: {player.playerName} is {(player.playerReadyState ? "Ready" : "Not Ready")}");
				if (GUILayout.Button("Kick")) {
					player.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
				}
				if (GUILayout.Button("Toggle Ready")) {
					player.SetReadyToBegin(!player.playerReadyState);
					MultiplayerRoomManager RoomManager =
					    (MultiplayerRoomManager)NetworkManager.singleton;
					RoomManager.ReadyStatusChanged();
				}
				currentSpectators[player.netId] =
				    GUILayout.Toggle(currentSpectators[player.netId], "Spectator");
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		public override void OnUpdate() {
			if (Time.time >= nextPlayerFetchTime && uiOpen) {
				nextPlayerFetchTime = Time.time + 2;
				currentPlayers = Object.FindObjectsOfType<MultiplayerRoomPlayer>().ToList();
				foreach (MultiplayerRoomPlayer player in currentPlayers) {
					if (!currentSpectators.ContainsKey(player.netId)) {
						currentSpectators[player.netId] = false;
					}
				}
			}
			if (openMenuBind == null) {
				return;
			}
			if (Input.GetKeyDown(openMenuBind.Value)) {
				uiOpen = !uiOpen;
			}
		}
	}
}
