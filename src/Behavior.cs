using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace MorePlayers {
	public class MorePlayersMod : MonoBehaviour {
		public static MorePlayersMod? instance;

		// TODO: Move these back to being save-able config options.
		public uint maxPlayers = 16u;
		private string openMenuBind = "f6";
		public bool randomizeSpawns = false;

		internal List<MultiplayerRoomPlayer>? currentPlayers;

		internal bool uiOpen = false;

		private void Awake() {
			instance = this;
			Debug.Log("More Players has initialized!");
		}

		private void OnGUI() {
			if (!uiOpen) {
				return;
			}
			GUILayout.BeginArea(new Rect(Screen.width / 2 - Screen.width / 8, 0, Screen.width / 4,
			                             Screen.height / 2));
			GUILayout.BeginVertical();
			GUILayout.Box("More Players Manager");

			GUILayout.BeginHorizontal();
			GUILayout.Box($"Maximum Players: {maxPlayers}\n(restart lobby after change)");
			maxPlayers = (uint)GUILayout.HorizontalSlider(maxPlayers, 1.0f, 16.0f);
			GUILayout.EndHorizontal();

			randomizeSpawns = GUILayout.Toggle(randomizeSpawns, "Randomize Spawns");

			if (currentPlayers == null || currentPlayers.Count == 0) {
				GUILayout.EndVertical();
				GUILayout.EndArea();
				return;
			}

			if (GUILayout.Button("Ready All")) {
				currentPlayers.ForEach(player => SetReadyState(player, true));
			}

			if (GUILayout.Button("Un-Ready All")) {
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
					if (GUILayout.Button("Kick")) {
						player.KickPlayer();
					}
					if (GUILayout.Button("Ban")) {
						player.BanPlayer();
					}
				}
				if (inLobby) {
					if (GUILayout.Button("Toggle Ready")) {
						SetReadyState(player, !player.playerReadyState);
					}
				}

				bool newSpec = GUILayout.Toggle(player.Networkspectator, "Spectator");
				if (player.Networkspectator != newSpec) {
					player.Networkspectator = newSpec;
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void Update() {
			if (openMenuBind == null) {
				return;
			}
			if (Input.GetKeyDown(openMenuBind)) {
				uiOpen = !uiOpen;
			}
		}

		internal void UpdateStoredPlayerList() {
			currentPlayers =
			    Object.FindObjectsByType<MultiplayerRoomPlayer>(FindObjectsSortMode.InstanceID)
			        .ToList();
		}

		private void SetReadyState(MultiplayerRoomPlayer player, bool readyState) {
			player.SetReadyToBegin(readyState);
			MultiplayerRoomManager roomManager = (MultiplayerRoomManager)NetworkManager.singleton;
			roomManager.ReadyStatusChanged();
		}
	}
}
