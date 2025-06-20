using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Represents settings for controlling automatic roof overlay and thin roof designation behavior.
    /// </summary>
    /// <remarks>
    /// This class provides configuration options for enabling or disabling automatic roof overlay
    /// visualization and automatic designation of thin roofs. These settings can be persisted and loaded using the 
    /// <see cref="ExposeData"/> method.
    /// </remarks>
    public class CantMineBlindSettings : ModSettings
    {
        public bool enableAutoRoofOverlay = true;
        public bool autoDesignateThinRoof = false;

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
        }
    }
}
