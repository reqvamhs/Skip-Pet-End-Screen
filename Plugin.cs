using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace HsSkipPetEndScreen
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.reqvam.hsskippetendscreen";
        public const string PluginName = "Skip Pet End Screen";
        public const string PluginVersion = "1.0.0";

        private void Awake()
        {
            EndScreenPatches.Log = Logger;

            // Config file name must match the assembly name for Firestone's Mod Manager
            ConfigFile fsConfig = new ConfigFile(
                Path.Combine(Paths.ConfigPath, "HsSkipPetEndScreen.cfg"), true);

            fsConfig.Bind("General", "Name", PluginName);
            fsConfig.Bind("General", "Guid", PluginGuid);
            fsConfig.Bind("General", "Version", PluginVersion);
            fsConfig.Bind("General", "DownloadLink", "https://github.com/reqvamhs/Skip-Pet-End-Screen");
            fsConfig.Bind("General", "Description",
                "Shows the standard end-of-game screen instead of the pet sequence when you have a pet equipped.");

            EndScreenPatches.Enabled = fsConfig.Bind("Features", "SkipPetEndScreen", true,
                "Show the standard end-of-game screen instead of the pet sequence (pet scene, XP and reward popups). The pet is unaffected during games.");

            new Harmony(PluginGuid).PatchAll();
            EndScreenPatches.Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
        }
    }

    /// <summary>
    /// The two questions the pet ending hangs on, GameEntity.PetEndgameSpell and
    /// GameState.HasPetEndgame, both have native no-pet paths these patches select, so the
    /// end screen matches a petless game for win and loss alike. XP and rewards still
    /// accrue; only their popups are skipped.
    /// </summary>
    public static class EndScreenPatches
    {
        internal static ManualLogSource Log;
        internal static ConfigEntry<bool> Enabled;

        internal static bool On() => Enabled != null && Enabled.Value;

        [HarmonyPatch(typeof(GameState), nameof(GameState.HasPetEndgame))]
        public static class NoPetEndgamePatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (On())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GameEntity), "PetEndgameSpell")]
        public static class SuppressPetEndgameSpellPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Spell __result)
            {
                if (On())
                {
                    __result = null;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PetControllerBoard), "get_IsCreationBlocked")]
        public static class BlockEndScreenCutscenePetPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(PetControllerBoard __instance, ref bool __result)
            {
                try
                {
                    // Gameplay only: the collection pet preview uses the same cutscene controller.
                    if (On() && __instance is PetControllerCutscene &&
                        SceneMgr.Get() != null && SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
                    {
                        __result = true;
                        return false;
                    }
                }
                catch (System.Exception e)
                {
                    Log?.LogError($"BlockEndScreenCutscenePet prefix failed: {e}");
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(EndGameScreen), "ShowPetXpGains")]
        public static class SkipPetXpGainsPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (On())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(EndGameScreen), "ShowPetRewards")]
        public static class SkipPetRewardsPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref bool __result)
            {
                if (On())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
