using HarmonyLib;

namespace MorePlayers {
	public class Loader : IMod {
		public void Initialize() {
			Harmony.CreateAndPatchAll(typeof(HarmonyPatches));
			GeneralManager.singleton.gameObject.AddComponent<MorePlayersMod>();
		}
	}
}
