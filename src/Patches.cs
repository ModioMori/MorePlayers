using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Utils;

namespace MorePlayers {
	[HarmonyPatch]
	public static class HarmonyPatches {
		[HarmonyPrefix, HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Awake")]
		private static void AddNewSpawns() {
			// TODO: Redo this to use configuration files or resources instead of hardcoded, so we
			// can support all base game maps. Not needed for custom maps, those should come with
			// more spawns out of the box.
			GameObject spawnPointHolder = GameObject.Find("SpawnPoints");
			Vector3[] positionArray =
			    new[] { new Vector3(-8f, 1.45f, 9f), new Vector3(8f, 1.45f, 9f),
				        new Vector3(8f, 1.45f, -9f), new Vector3(-8f, 1.45f, -9f),
				        new Vector3(0, 1.45f, -12f), new Vector3(0, 1.45f, 12f),
				        new Vector3(-12f, 1.45f, 0), new Vector3(12f, 1.45f, 0),
				        new Vector3(-7f, 6f, 7f),    new Vector3(7f, 6f, 7f),
				        new Vector3(7f, 6f, -7f),    new Vector3(-7f, 6f, -7f) };

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
			if (!mod.randomizeSpawns)
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

		[HarmonyPrefix, HarmonyPatch(typeof(SteamManager), "HostLobby"),
		 HarmonyPatch(typeof(EpicManager), "HostLobby")]
		private static void ChangeLobbyMaxConnections() {
			MorePlayersMod mod = MorePlayersMod.instance!;

			NetworkManager.singleton.maxConnections = System.Convert.ToInt32(mod.maxPlayers);
			NetworkHelpers.maxPlayerCount = mod.maxPlayers;
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
	}
}
