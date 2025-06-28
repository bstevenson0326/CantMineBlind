using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Represents the configuration settings for the "Can't Mine Blind" mod, allowing customization of various
    /// overlays and terrain categories.
    /// </summary>
    /// <remarks>
    /// This class provides options to enable or disable specific overlays and terrain categories 
    /// used in the mod. The settings are serialized and deserialized automatically during game save/load operations to
    /// persist user preferences.
    /// </remarks>
    public class CantMineBlindSettings : ModSettings
    {
        public bool enableAutoRoofOverlay = true;
        public bool autoDesignateThinRoof = false;
        public bool enableAutoFertilityOverlay = true;
        public bool enableTerrainAffordanceOverlay = false;
        public bool enablePlanningTerrainOverlay = false;
        public bool enableBuildingTerrainOverlay = false;
        public bool terrainCategoryWalls = true;
        public bool terrainCategoryFloors = true;
        public bool terrainCategoryFurniture = true;

        /// <summary>
        /// Saves and loads the state of the object's data during serialization.
        /// </summary>
        /// <remarks>This method is typically used to persist the values of the object's fields during
        /// game save/load operations. It ensures that the values of <see cref="enableAutoRoofOverlay"/> 
        /// and <see cref="autoDesignateThinRoof"/> are correctly serialized and deserialized.
        /// </remarks>
        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableAutoRoofOverlay, "enableAutoRoofOverlay", true);
            Scribe_Values.Look(ref autoDesignateThinRoof, "autoDesignateThinRoof", false);
            Scribe_Values.Look(ref enableAutoFertilityOverlay, "enableAutoFertilityOverlay", true);
            Scribe_Values.Look(ref enableTerrainAffordanceOverlay, "enableTerrainAffordanceOverlay", false);
            Scribe_Values.Look(ref enablePlanningTerrainOverlay, "enablePlanningTerrainOverlay", false);
            Scribe_Values.Look(ref enableBuildingTerrainOverlay, "enableBuildingTerrainOverlay", false);
            Scribe_Values.Look(ref terrainCategoryWalls, "terrainCategoryWalls", true);
            Scribe_Values.Look(ref terrainCategoryFloors, "terrainCategoryFloors", true);
            Scribe_Values.Look(ref terrainCategoryFurniture, "terrainCategoryFurniture", true);
        }
    }
}
