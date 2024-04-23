using GladioMoriMorePlayers;
using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace GladioMorePlayers {
	[HarmonyPatch]
	public static class HarmonyPatches {
		[HarmonyPrefix, HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Awake")]
		static bool AddNewSpawns() {
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

			MorePlayersMod Mod = MorePlayersMod.instance!;
			if (Mod.randomizeSpawns == null) {
				MorePlayersMod.log!.LogError(
				    "Could not get random spawns pref! Defaulting to false.");
				return true;
			}
			if (Mod.randomizeSpawns.Value) {
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

		[HarmonyPrefix, HarmonyPatch(typeof(SteamManager), "HostLobby")]
		private static bool ChangeLobbyMaxConnections() {
			MorePlayersMod Mod = MorePlayersMod.instance!;
			if (Mod.maxPlayers == null) {
				MorePlayersMod.log!.LogError(
				    "Could not get max players pref! Defaulting to 4 players.");
				return true;
			}
			NetworkManager.singleton.maxConnections = Mod.maxPlayers.Value;
			if (SteamClient.IsValid) {
				SteamMatchmaking.CreateLobbyAsync(Mod.maxPlayers.Value);
			}
			return false;
		}

		[HarmonyPostfix, HarmonyPatch(typeof(PlayerMultiplayerInputManager), "Start")]
		private static void SpectateOnPlayerSpawn(PlayerMultiplayerInputManager __instance,
		                                          GameObject ___playerCharacter) {
			if (!MorePlayersMod.instance!.currentSpectators.ContainsKey(
			        __instance.multiplayerRoomPlayer.netId)) {
				return;
			}
			if (MorePlayersMod.instance!
			        .currentSpectators[__instance.multiplayerRoomPlayer.netId]) {
				__instance.HandlePlayerDeath();
				Object.Destroy(___playerCharacter.transform.Find("PlayerModelPhysics").gameObject);
				Object.Destroy(
				    ___playerCharacter.transform.Find("PlayerModelAnimation").gameObject);
			}
		}
	}
}
