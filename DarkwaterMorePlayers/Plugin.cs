using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Fusion;
using HarmonyLib;

namespace MorePlayers
{
    [BepInPlugin("eu.puszkapotato.darkwater.moreplayers", "More Players", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<int> MaxPlayersConfig;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            MaxPlayersConfig = Config.Bind(
                "General",
                "MaxPlayers",
                8,
                "The maximum number of players allowed in a lobby."
            );

            var harmony = new Harmony("eu.puszkapotato.darkwater.moreplayers");
            harmony.PatchAll();

            Log.LogInfo("More Players mod (Fusion Patcher) has been loaded!");
        }
    }

    /// <summary>
    /// Holds Harmony patches targeting the Photon Fusion library.
    /// </summary>
    [HarmonyPatch]
    public static class FusionPatches
    {
        /// <summary>
        /// This prefix patch targets the core method for starting a game in Photon Fusion.
        /// It modifies the StartGameArgs to change the maximum player count.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkRunner), nameof(NetworkRunner.StartGame))]
        private static void StartGamePrefix(ref StartGameArgs args)
        {
            // We've hit the correct method! Now we can modify the arguments.
            Plugin.Log.LogInfo($"Intercepted NetworkRunner.StartGame. Original PlayerCount: {args.PlayerCount}.");

            // Get the desired player count from our config file.
            int newMaxPlayers = Plugin.MaxPlayersConfig.Value;

            // Overwrite the PlayerCount in the arguments before the game starts.
            args.PlayerCount = newMaxPlayers;

            Plugin.Log.LogInfo($"Overriding PlayerCount with {newMaxPlayers}.");
        }
    }
}
