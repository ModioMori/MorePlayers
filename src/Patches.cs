using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace GladioMorePlayers {
	[HarmonyPatch]
	public static class HarmonyPatches {
		[HarmonyPrefix, HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Awake")]
		private static void AddNewSpawns() {
			GameObject spawnPointHolder = GameObject.Find("SpawnPoints");
			Vector3[] positionArray =
			    new[] { new Vector3(-8f, 1.45f, 9f), new Vector3(8f, 1.45f, 9f),
				        new Vector3(8f, 1.45f, -9f), new Vector3(-8f, 1.45f, -9f),
				        new Vector3(0, 1.45f, -12f), new Vector3(0, 1.45f, 12f),
				        new Vector3(-12f, 1.45f, 0), new Vector3(12f, 1.45f, 0),
				        new Vector3(-7f, 6f, 7f),    new Vector3(7f, 6f, 7f),
				        new Vector3(7f, 6f, -7f),    new Vector3(-7f, 6f, -7f) };

			// We should only do this on the default game map.
			if (SceneManager.GetActiveScene().name == "map_ArenaOfBlades" &&
			    spawnPointHolder.transform.childCount < positionArray.Length + 4) {
				for (int i = 0; i < positionArray.Length; i++) {
					GameObject NewSpawn = new GameObject();
					NewSpawn.transform.position = positionArray[i];
					NewSpawn.transform.SetParent(spawnPointHolder.transform);
					NewSpawn.name = $"Spawnpoint ({4 + i})";
				}
			}

			MorePlayersMod mod = MorePlayersMod.instance!;
			if (mod.randomizeSpawns == null) {
				MorePlayersMod.log!.LogError(
				    "Could not get random spawns pref! Defaulting to false.");
				return;
			}
			if (!mod.randomizeSpawns.Value)
				return;

			List<int> indexes = new List<int>();
			List<Transform> transforms = new List<Transform>();
			for (int i = 0; i < spawnPointHolder.transform.childCount; ++i) {
				indexes.Add(i);
				transforms.Add(spawnPointHolder.transform.GetChild(i));
			}
			foreach (Transform T in transforms) {
				T.SetSiblingIndex(indexes[Random.Range(0, indexes.Count)]);
			}
		}

		[HarmonyPrefix, HarmonyPatch(typeof(SteamManager), "HostLobby")]
		private static bool ChangeLobbyMaxConnections() {
			MorePlayersMod mod = MorePlayersMod.instance!;
			if (mod.maxPlayers == null) {
				MorePlayersMod.log!.LogError(
				    "Could not get max players pref! Defaulting to 4 players.");
				return true;
			}
			NetworkManager.singleton.maxConnections = mod.maxPlayers.Value;
			if (SteamClient.IsValid)
				SteamMatchmaking.CreateLobbyAsync(mod.maxPlayers.Value);

			return false;
		}

		[HarmonyPostfix, HarmonyPatch(typeof(MultiplayerRoomPlayer), "OnStartClient")]
		private static void UpdatePlayersOnPlayerStart() {
			MorePlayersMod mod = MorePlayersMod.instance!;
			mod.UpdateStoredPlayerList();
		}

		[HarmonyPostfix, HarmonyPatch(typeof(MultiplayerRoomPlayer), "OnStopClient")]
		private static void UpdatePlayersOnPlayerStop() {
			MorePlayersMod mod = MorePlayersMod.instance!;
			mod.UpdateStoredPlayerList();
		}

		[HarmonyPostfix, HarmonyPatch(typeof(MultiplayerRoomManager), "OnServerConnect")]
		private static void KickBannedPlayers(NetworkConnectionToClient conn) {
			MorePlayersMod mod = MorePlayersMod.instance!;
			MultiplayerRoomManager roomManager = (MultiplayerRoomManager)NetworkManager.singleton;
			if (mod.IsPlayerBanned(conn.connectionId)) {
				roomManager.GetTransport().ServerDisconnect(conn.connectionId);
			}
		}
	}
}
