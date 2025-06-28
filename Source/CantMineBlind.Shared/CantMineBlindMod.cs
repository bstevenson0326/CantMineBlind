using RimWorld;
using UnityEngine;
using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Represents the "Can't Mine Blind" mod, which provides custom settings and functionality for the game.
    /// </summary>
    /// <remarks
    /// >This mod includes configurable options such as enabling or disabling specific features
    /// related to roof overlays  and automatic thin roof designation. It also provides a settings category for user
    /// customization and integrates  with the game's modding framework.
    /// </remarks>
    public class CantMineBlindMod : Mod
    {
        public static CantMineBlindSettings Settings { get; private set; }

        /// <summary>
        /// Override this method to provide a custom settings category name for the mod.
        /// </summary>
        /// <returns>Returns the settings category</returns>
        public override string SettingsCategory() => "Can't Mine (or Work) Blind";

        /// <summary>
        /// Initializes a new instance of the <see cref="CantMineBlindMod"/> class, which represents a mod for the game.
        /// </summary>
        /// <param name="content">The content pack associated with the mod. This provides access to mod-specific resources and definitions.</param>
        /// <remarks>This constructor sets up the mod by loading its settings using the <see cref="GetSettings{T}"/> method.</remarks>
        public CantMineBlindMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<CantMineBlindSettings>();
        }

        /// <summary>
        /// Draws the settings window contents for configuring the mod's options.
        /// </summary>
        /// <param name="inRect">The rectangle area within which the settings UI should be drawn.</param>
        /// <remarks>
        /// This method provides a user interface for configuring various mod settings, such as
        /// enabling or disabling overlays  for roof, fertility, and terrain affordance. It dynamically adjusts the
        /// available options based on the current  settings and the presence of other mods. Warnings and tooltips are
        /// displayed where applicable to guide the user.
        /// </remarks>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            // Roof overlay
            listing.CheckboxLabeled(
                "CMB_EnableRoofOverlay_Setting".Translate(),
                ref Settings.enableAutoRoofOverlay,
                tooltip: "CMB_EnableRoofOverlayTooltip_Setting".Translate()
            );

            if (ModLister.GetActiveModWithIdentifier("Darkelvar.DeroofMining") != null)
            {
                Color oldColor = GUI.color;
                GUI.color = Color.yellow;
                listing.Label("CMB_DRMAutoThinRoofWarning_Setting".Translate());
                GUI.color = oldColor;
            }
            else if (Settings.autoDesignateThinRoof)
            {
                listing.CheckboxLabeled(
                    "CMB_EnableAutoThinRoof_Setting".Translate(),
                    ref Settings.autoDesignateThinRoof,
                    tooltip: "CMB_EnableAutoThinRoofTooltip_Setting".Translate()
                );

                listing.Indent();
                Color oldColor = GUI.color;
                GUI.color = Color.red;
                listing.Label("CMB_EnableAutoThinRoofWarning_Setting".Translate());
                GUI.color = oldColor;
                listing.Outdent();
            }

            listing.Gap(6f);
            listing.GapLine(6f);
            listing.Gap(6f);

            // Fertility overlay
            listing.CheckboxLabeled(
                "CMB_EnableFertilityOverlay_Setting".Translate(),
                ref Settings.enableAutoFertilityOverlay,
                tooltip: "CMB_EnableFertilityOverlayTooltip_Setting".Translate()
            );

            listing.Gap(6f);
            listing.GapLine(6f);
            listing.Gap(6f);

            // Terrain-affordance overlay
            listing.CheckboxLabeled(
                "CMB_EnableTerrainAffordanceOverlay_Setting".Translate(),
                ref Settings.enableTerrainAffordanceOverlay,
                tooltip: "CMB_EnableTerrainAffordanceOverlayTooltip_Setting".Translate()
            );

            if (Settings.enableTerrainAffordanceOverlay)
            {
                listing.Gap(4f);

                // Planning sub-option
                listing.CheckboxLabeled(
                    "CMB_EnablePlanningTerrainOverlay_Setting".Translate(),
                    ref Settings.enablePlanningTerrainOverlay,
                    tooltip: "CMB_EnablePlanningTerrainOverlayTooltip_Setting".Translate()
                );

                // Building sub-option
                listing.CheckboxLabeled(
                    "CMB_EnableBuildingTerrainOverlay_Setting".Translate(),
                    ref Settings.enableBuildingTerrainOverlay,
                    tooltip: "CMB_EnableBuildingTerrainOverlayTooltip_Setting".Translate()
                );

                if (Settings.enableTerrainAffordanceOverlay && Settings.enableBuildingTerrainOverlay)
                {
                    listing.Gap(4f);
                    listing.Label("CMB_SelectTerrainCategories".Translate());

                    listing.CheckboxLabeled(
                        "CMB_TerrainCatWalls".Translate(),
                        ref Settings.terrainCategoryWalls,
                        tooltip: "CMB_TerrainCatWallsTooltip".Translate()
                    );
                    listing.CheckboxLabeled(
                        "CMB_TerrainCatFloors".Translate(),
                        ref Settings.terrainCategoryFloors,
                        tooltip: "CMB_TerrainCatFloorsTooltip".Translate()
                    );
                    listing.CheckboxLabeled(
                        "CMB_TerrainCatFurniture".Translate(),
                        ref Settings.terrainCategoryFurniture,
                        tooltip: "CMB_TerrainCatFurnitureTooltip".Translate()
                    );
                }
            }

            listing.End();
        }
    }
}
