using BepInEx;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Bootstrap;

namespace GladioMorePlayers {
	[BepInPlugin("gay.crf.gladiomoreplayers", "Gladio More Players", "2.1.5")]
	public class MorePlayersMod : BaseUnityPlugin {
		public static MorePlayersMod? instance;
		MultiplayerRoomManager roomManagerSingleton;
		public static ManualLogSource? log;

		public ConfigEntry<int>? maxPlayers;
		private ConfigEntry<string>? openMenuBind;
		public ConfigEntry<bool>? randomizeSpawns;

		private List<MultiplayerRoomPlayer>? currentPlayers;
		public Dictionary<uint, bool> currentSpectators = new Dictionary<uint, bool>();
		
		/// <summary>
		/// List of netids and nicknames of banned players 
		/// </summary>
		public List<(string, string)> bannedPlayers = new List<(string, string)>();
		
		private float nextPlayerFetchTime = 0;
		private bool uiOpen = false;

		private void Awake() {
			instance = this;
			log = Logger;
			Chainloader.ManagerObject.hideFlags = HideFlags.HideAndDontSave;
			Harmony.CreateAndPatchAll(typeof(HarmonyPatches));

			maxPlayers = Config.Bind("General", "maxPlayers", 16,
			                         "Maximum amount of players that can join your server.");
			openMenuBind = Config.Bind(
			    "General", "openMenuBind", "f6",
			    "Keybind to open the manager UI. See 'Mapping virtual axes to controls' on https://docs.unity3d.com/Manual/class-InputManager.html for key names.");
			randomizeSpawns = Config.Bind(
			    "General", "randomizeSpawns", false,
			    "Whether player's spawns should be randomized every round. true or false.");

			Logger.LogInfo("More Players has initialized!");
		}

		private void OnGUI() {
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
			
			if (GUILayout.Button("Ready all")) {
				currentPlayers.ForEach(player => SetReadyState(player, true));
			}

			if (GUILayout.Button("Not Ready all")) {
				currentPlayers.ForEach(player => SetReadyState(player, false));
			}

			bool inLobby = MultiplayerLobbyStatusManager.singleton != null;
			
			foreach (MultiplayerRoomPlayer player in currentPlayers) {
				if (player == null || player.disconnecting) {
					continue;
				}
				GUILayout.BeginHorizontal();
				if (inLobby) {
					GUILayout.Box(
					    $"Player: {player.playerName} is {(player.playerReadyState ? "Ready" : "Not Ready")}");
				} else {
					GUILayout.Box($"Player: {player.playerName}");
				}
				if (player.connectionToServer == null) {
					if (GUILayout.Button("Kick"))
						KickPlayer(player);
					if (GUILayout.Button("Ban"))
						BanPlayer(player);
				}
				if (inLobby) {
					if (GUILayout.Button("Toggle Ready")) {
						SetReadyState(player, !player.playerReadyState);
					}
				}
				currentSpectators[player.netId] = GUILayout.Toggle(currentSpectators[player.netId], "Spectator");
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void Update() {
			if (Time.time >= nextPlayerFetchTime && uiOpen) {
				nextPlayerFetchTime = Time.time + 2;
				currentPlayers = Object.FindObjectsOfType<MultiplayerRoomPlayer>().ToList();
				foreach (MultiplayerRoomPlayer player in currentPlayers) {
					if (!currentSpectators.ContainsKey(player.netId)) {
						currentSpectators[player.netId] = false;
					}
					
					if(IsPlayerBanned(player)) {
						KickPlayer(player);
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

		private void SetReadyState(MultiplayerRoomPlayer player, bool readyState) {
			player.SetReadyToBegin(readyState); 
			roomManagerSingleton = (MultiplayerRoomManager)NetworkManager.singleton;
			roomManagerSingleton.ReadyStatusChanged();
		}
		
		private void BanPlayer(MultiplayerRoomPlayer player) {
			string steamId = GetSteamId(player);
			bannedPlayers.Add((steamId, player.playerName));
			KickPlayer(player);
		}

		private static void KickPlayer(MultiplayerRoomPlayer player) {
			player.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
		}

		private bool IsPlayerBanned(MultiplayerRoomPlayer player) {
			string steamId = GetSteamId(player);
			return bannedPlayers.Any(ban => ban.Item1 == steamId) || bannedPlayers.Any(ban => ban.Item2 == player.playerName);
		}

		private string GetSteamId(MultiplayerRoomPlayer player) {
			return roomManagerSingleton.GetTransport().ServerGetClientAddress(player.connectionToClient.connectionId);
		}
	}
}
