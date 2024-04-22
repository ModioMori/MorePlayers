using GladioMoriMorePlayers;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;

[assembly:MelonInfo(typeof(MorePlayersMod), "Gladio More Players", "1.1.0", "checkraisefold")]
[assembly:MelonGame("Plebeian Studio", "Gladio Mori")]

namespace GladioMoriMorePlayers {
	public class MorePlayersMod : MelonMod {
		public static MorePlayersMod instance;

		private MelonPreferences_Category modPrefs;
		public MelonPreferences_Entry<int> maxPlayers;
		private MelonPreferences_Entry<string> openMenuBind;
		public MelonPreferences_Entry<bool> randomizeSpawns;

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
			if (Input.GetKeyDown(openMenuBind.Value)) {
				uiOpen = !uiOpen;
			}
		}
	}

	[HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Awake")]
	static class SpawnPointPatch {
		static bool Prefix() {
			GameObject SpawnPointHolder = GameObject.Find("SpawnPoints");
			Vector3[] PositionArray =
			    new[] { new Vector3(-8f, 1.45f, 9f), new Vector3(8f, 1.45f, 9f),
				        new Vector3(8f, 1.45f, -9f), new Vector3(-8f, 1.45f, -9f),
				        new Vector3(0, 1.45f, -12f), new Vector3(0, 1.45f, 12f),
				        new Vector3(-12f, 1.45f, 0), new Vector3(12f, 1.45f, 0),
				        new Vector3(-7f, 6f, 7f),    new Vector3(7f, 6f, 7f),
				        new Vector3(7f, 6f, -7f),    new Vector3(-7f, 6f, -7f) };
			if (SpawnPointHolder.transform.childCount >= PositionArray.Length + 4) {
				return true;
			}

			for (int i = 0; i < PositionArray.Length; i++) {
				GameObject NewSpawn = new GameObject();
				NewSpawn.transform.position = PositionArray[i];
				NewSpawn.transform.SetParent(SpawnPointHolder.transform);
				NewSpawn.name = $"Spawnpoint ({4 + i})";
			}

			if (!MorePlayersMod.instance.randomizeSpawns.Value) {
				return true;
			}

			List<int> Indexes = new List<int>();
			List<Transform> Transforms = new List<Transform>();
			for (int i = 0; i < SpawnPointHolder.transform.childCount; ++i) {
				Indexes.Add(i);
				Transforms.Add(SpawnPointHolder.transform.GetChild(i));
			}
			foreach (Transform T in Transforms) {
				T.SetSiblingIndex(Indexes[Random.Range(0, Indexes.Count)]);
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(SteamManager), "HostLobby")]
	static class RoomConnectionsPatch {
		private static bool Prefix() {
			MorePlayersMod Mod = MorePlayersMod.instance;
			NetworkManager.singleton.maxConnections = Mod.maxPlayers.Value;
			if (SteamClient.IsValid) {
				SteamMatchmaking.CreateLobbyAsync(Mod.maxPlayers.Value);
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Start")]
	static class SpectatorCameraPatch {
		private static void Postfix(PlayerMultiplayerInputManager __instance,
		                            GameObject ___playerCharacter) {
			if (!MorePlayersMod.instance.currentSpectators.ContainsKey(
			        __instance.multiplayerRoomPlayer.netId)) {
			}
			if (MorePlayersMod.instance.currentSpectators[__instance.multiplayerRoomPlayer.netId]) {
				__instance.HandlePlayerDeath();
				Object.Destroy(___playerCharacter.transform.Find("PlayerModelPhysics").gameObject);
				Object.Destroy(
				    ___playerCharacter.transform.Find("PlayerModelAnimation").gameObject);
			}
		}
	}
}
