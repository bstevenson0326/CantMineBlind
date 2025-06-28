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
        /// A Harmony patch for the <see cref="DesignatorManager.Select"/> method that automatically adjusts overlay
        /// settings based on the selected designator and the mod's configuration.
        /// </summary>
        /// <remarks>
        /// This patch modifies the behavior of the <see cref="DesignatorManager.Select"/> method
        /// to enable or disable specific overlays (roof, fertility, and terrain affordance) based on the selected
        /// designator and the mod's settings. The overlays are automatically toggled to assist the user in planning and
        /// building tasks, such as mining, growing zones, or placing structures.  The patch respects the user's
        /// previous overlay settings and restores them when the relevant designator is no longer selected. This ensures
        /// that the mod's behavior is non-intrusive and does not permanently alter the user's preferences.
        /// </remarks>
        [HarmonyPatch(typeof(DesignatorManager), nameof(DesignatorManager.Select))]
        internal static class DesignatorManager_Select_Patch
        {
            internal static void Postfix(Designator des)
            {
                // 1) Make sure we run if *any* feature is on:
                if (!CantMineBlindMod.Settings.enableAutoRoofOverlay
                    && !CantMineBlindMod.Settings.enableAutoFertilityOverlay
                    && !CantMineBlindMod.Settings.enableTerrainAffordanceOverlay)
                {
                    return;
                }

                if (Find.PlaySettings == null)
                {
                    return;
                }

                // Roof overlay
                if (CantMineBlindMod.Settings.enableAutoRoofOverlay)
                {
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

                // Fertility overlay
                if (CantMineBlindMod.Settings.enableAutoFertilityOverlay)
                {
                    if (des is Designator_ZoneAdd_Growing || des is Designator_ZoneAdd_Growing_Expand)
                    {
                        if (CantMineBlindState.PreviousFertilityOverlaySetting == null)
                        {
                            CantMineBlindState.PreviousFertilityOverlaySetting = Find.PlaySettings.showFertilityOverlay;
                        }

                        Find.PlaySettings.showFertilityOverlay = true;
                    }
                    else if (CantMineBlindState.PreviousFertilityOverlaySetting.HasValue)
                    {
                        Find.PlaySettings.showFertilityOverlay = CantMineBlindState.PreviousFertilityOverlaySetting.Value;
                        CantMineBlindState.PreviousFertilityOverlaySetting = null;
                    }
                }

                // Terrain-affordance overlay (planning + building)
                if (CantMineBlindMod.Settings.enableTerrainAffordanceOverlay)
                {
                    bool shouldEnableTerrainAffordanceOverlay = false;

                    // Planning
                    if (CantMineBlindMod.Settings.enablePlanningTerrainOverlay && des is Designator_Plan)
                    {
                        shouldEnableTerrainAffordanceOverlay = true;
                    }

                    // Building
                    if (!shouldEnableTerrainAffordanceOverlay
                        && CantMineBlindMod.Settings.enableBuildingTerrainOverlay
                        && des is Designator_Build buildDes)
                    {
                        BuildableDef buildDef = buildDes.PlacingDef;

                        // Floors are TerrainDefs
                        if (buildDef is TerrainDef && CantMineBlindMod.Settings.terrainCategoryFloors)
                        {
                            shouldEnableTerrainAffordanceOverlay = true;
                        }
                        // All other BuildableDefs are ThingDefs
                        else if (buildDef is ThingDef td && td.building != null)
                        {
                            // Walls
#if RW_1_6
                           bool isStructure = td.IsWall || td.IsDoor || td.IsFence; 
#else
                            bool isStructure = buildDef?.ToString().ToUpperInvariant() == "Wall".ToUpperInvariant() || td.IsDoor || td.IsFence;
#endif
                            if (isStructure && CantMineBlindMod.Settings.terrainCategoryWalls)
                            {
                                shouldEnableTerrainAffordanceOverlay = true;
                            }
                            // Furniture & everything else
                            else if (!isStructure && CantMineBlindMod.Settings.terrainCategoryFurniture)
                            {
                                shouldEnableTerrainAffordanceOverlay = true;
                            }
                        }
                    }

                    // Flip or revert
                    if (shouldEnableTerrainAffordanceOverlay)
                    {
                        if (CantMineBlindState.PreviousTerrainAffordanceOverlaySetting == null)
                        {
                            CantMineBlindState.PreviousTerrainAffordanceOverlaySetting = Find.PlaySettings.showTerrainAffordanceOverlay;
                        }

                        Find.PlaySettings.showTerrainAffordanceOverlay = true;
                    }
                    else if (CantMineBlindState.PreviousTerrainAffordanceOverlaySetting.HasValue)
                    {
                        Find.PlaySettings.showTerrainAffordanceOverlay = CantMineBlindState.PreviousTerrainAffordanceOverlaySetting.Value;
                        CantMineBlindState.PreviousTerrainAffordanceOverlaySetting = null;
                    }
                }
            }
        }

        /// <summary>
        /// A Harmony patch for the <see cref="DesignatorManager.Deselect"/> method that restores overlay settings to
        /// their previous states after a designator is deselected.
        /// </summary>
        /// <remarks>
        /// This patch reverts the roof, fertility, and terrain-affordance overlay settings to
        /// their prior values if the corresponding auto-overlay features are enabled in the mod settings. This ensures
        /// that any temporary changes to these overlays made during designator usage are undone when the designator is
        /// deselected.
        /// </remarks>
        [HarmonyPatch(typeof(DesignatorManager), nameof(DesignatorManager.Deselect))]
        internal static class DesignatorManager_Deselect_Patch
        {
            internal static void Postfix()
            {
                // Roof overlay revert
                if (CantMineBlindMod.Settings.enableAutoRoofOverlay
                    && CantMineBlindState.PreviousRoofOverlaySetting.HasValue)
                {
                    Find.PlaySettings.showRoofOverlay = CantMineBlindState.PreviousRoofOverlaySetting.Value;
                    CantMineBlindState.PreviousRoofOverlaySetting = null;
                }

                // Fertility overlay revert
                if (CantMineBlindMod.Settings.enableAutoFertilityOverlay
                    && CantMineBlindState.PreviousFertilityOverlaySetting.HasValue)
                {
                    Find.PlaySettings.showFertilityOverlay = CantMineBlindState.PreviousFertilityOverlaySetting.Value;
                    CantMineBlindState.PreviousFertilityOverlaySetting = null;
                }

                // Terrain-affordance overlay revert
                if (CantMineBlindMod.Settings.enableTerrainAffordanceOverlay
                    && CantMineBlindState.PreviousTerrainAffordanceOverlaySetting.HasValue)
                {
                    Find.PlaySettings.showTerrainAffordanceOverlay = CantMineBlindState.PreviousTerrainAffordanceOverlaySetting.Value;
                    CantMineBlindState.PreviousTerrainAffordanceOverlaySetting = null;
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
        /// Represents the state of the system when mining is not allowed due to blind conditions.
        /// </summary>
        /// <remarks>
        /// This class stores the previous settings for various overlays, which may be used to
        /// restore the system's state after the blind mining condition is resolved. It is intended for internal use
        /// only.
        /// </remarks>
        internal static class CantMineBlindState
        {
            internal static bool? PreviousRoofOverlaySetting;
            internal static bool? PreviousFertilityOverlaySetting;
            internal static bool? PreviousTerrainAffordanceOverlaySetting;
        }
    }
}
