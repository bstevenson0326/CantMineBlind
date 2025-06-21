using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Provides Harmony patches for modifying the behavior of designators in the game.
    /// </summary>
    /// <remarks>
    /// This class applies patches to various methods in the game's designator system to enable
    /// custom mod functionality. The patches include automatic roof overlay adjustments and thin roof designation
    /// behavior based on mod settings.
    /// </remarks>
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        /// <summary>
        /// Initializes and applies all Harmony patches for the mod.
        /// </summary>
        /// <remarks>
        /// This static constructor sets up Harmony patches using the provided mod identifier. It
        /// ensures that all necessary patches are applied at runtime.
        /// </remarks>
        static HarmonyPatches()
        {
            var harmony = new Harmony("com.hawqeye19.cantmineblind");
            harmony.PatchAll();
            Log.Message("[CantMineBlind] Initialized.");
        }

        /// <summary>
        /// A Harmony patch for the <see cref="DesignatorManager.Select"/> method that modifies the behavior of roof
        /// overlay settings based on the selected designator.
        /// </summary>
        /// <remarks>
        /// This patch ensures that the roof overlay is automatically enabled when a <see cref="Designator_Mine"/> 
        /// is selected, and restores the previous roof overlay setting when a different
        /// designator is selected. The behavior is controlled by the <c>enableAutoRoofOverlay</c> setting in
        /// <c>CantMineBlindMod.Settings</c>.
        /// </remarks>
        [HarmonyPatch(typeof(DesignatorManager), nameof(DesignatorManager.Select))]
        internal static class DesignatorManager_Select_Patch
        {
            internal static void Postfix(Designator des)
            {
                if (!CantMineBlindMod.Settings.enableAutoRoofOverlay)
                {
                    return;
                }

                if (Find.PlaySettings == null)
                {
                    return;
                }

                if (des is Designator_Mine)
                {
                    if (CantMineBlindState.PreviousRoofOverlaySetting == null)
                    {
                        CantMineBlindState.PreviousRoofOverlaySetting = Find.PlaySettings.showRoofOverlay;
                    }

                    Find.PlaySettings.showRoofOverlay = true;
                }
                else if (CantMineBlindState.PreviousRoofOverlaySetting.HasValue)
                {
                    Find.PlaySettings.showRoofOverlay = CantMineBlindState.PreviousRoofOverlaySetting.Value;
                    CantMineBlindState.PreviousRoofOverlaySetting = null;
                }
            }
        }

        /// <summary>
        /// A Harmony patch for the <see cref="DesignatorManager.Deselect"/> method that restores the roof overlay
        /// setting  after a designator is deselected, if the auto roof overlay feature is enabled.
        /// </summary>
        /// <remarks>
        /// This patch ensures that the roof overlay setting is reverted to its previous state
        /// when the auto roof overlay feature is enabled. If the feature is disabled, the patch does not modify the
        /// roof overlay setting.
        /// </remarks>
        [HarmonyPatch(typeof(DesignatorManager), nameof(DesignatorManager.Deselect))]
        internal static class DesignatorManager_Deselect_Patch
        {
            internal static void Postfix()
            {
                if (!CantMineBlindMod.Settings.enableAutoRoofOverlay)
                {
                    return;
                }

                if (CantMineBlindState.PreviousRoofOverlaySetting.HasValue)
                {
                    Find.PlaySettings.showRoofOverlay = CantMineBlindState.PreviousRoofOverlaySetting.Value;
                    CantMineBlindState.PreviousRoofOverlaySetting = null;
                }
            }
        }

        /// <summary>
        /// A Harmony patch for the <see cref="Designator.DesignateMultiCell"/> method. Automatically enqueues cells for
        /// processing if the current map is available and the <see cref="Designator"/> instance is of type <see cref="Designator_Mine"/>.
        /// </summary>
        /// <remarks>
        /// This patch is applied to enhance the behavior of the <see cref="Designator.DesignateMultiCell"/> method. 
        /// If the <c>autoDesignateThinRoof</c> setting is enabled, and the <see cref="Designator"/> instance is
        /// specifically a <see cref="Designator_Mine"/>, the designated cells are added to the processing queue for further handling.
        /// </remarks>
        [HarmonyPatch(typeof(Designator), nameof(Designator.DesignateMultiCell))]
        internal static class Patch_DesignatorMine_DesignateMultiCell
        {
            internal static void Postfix(Designator __instance, IEnumerable<IntVec3> cells)
            {
                // Skip when De-roof mining is active
                if (ModLister.GetActiveModWithIdentifier("Darkelvar.DeroofMining") != null)
                {
                    return;
                }

                // Skip if auto-designate thin roof is not enabled
                if (!CantMineBlindMod.Settings.autoDesignateThinRoof)
                {
                    return;
                }

                if (__instance is Designator_Mine && Find.CurrentMap != null)
                {
                    CantMineBlindQueue.Get.Enqueue(Find.CurrentMap, cells);
                }
            }
        }

        /// <summary>
        /// Represents the state of the system when mining is disabled due to blindness.
        /// </summary>
        /// <remarks>
        /// This class is used internally to track specific settings related to the mining
        /// state.
        /// </remarks>
        internal static class CantMineBlindState
        {
            internal static bool? PreviousRoofOverlaySetting;
        }
    }
}
