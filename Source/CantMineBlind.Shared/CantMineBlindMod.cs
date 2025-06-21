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
        public override string SettingsCategory() => "Can't Mine Blind";

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
        /// Draws the settings window contents for configuring mod-specific options.
        /// </summary>
        /// <param name="inRect">The rectangle within which the settings window contents are drawn.</param>
        /// <remarks>
        /// This method displays checkboxes and labels for user-configurable settings, such as
        /// enabling or disabling specific mod features. If certain settings are enabled, additional warnings or
        /// contextual information may be displayed to guide the user.
        /// </remarks>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

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
            else
            {
                // Display a warning if the auto-designate thin roof setting is enabled
                if (Settings.autoDesignateThinRoof)
                {
                    listing.CheckboxLabeled(
                        "CMB_EnableAutoThinRoof_Setting".Translate(),
                        ref Settings.autoDesignateThinRoof,
                        tooltip: "CMB_EnableAutoThinRoofTooltip_Setting".Translate()
                    );

                    Color oldColor = GUI.color;
                    GUI.color = Color.red;
                    listing.Label("CMB_EnableAutoThinRoofWarning_Setting".Translate());
                    GUI.color = oldColor;
                }
            }

            listing.End();
        }
    }
}
