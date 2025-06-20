using System.Collections.Generic;
using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Provides utility methods for managing thin roof designations in a map.
    /// </summary>
    /// <remarks>
    /// This class contains methods to assist with the removal of thin roofs in specified areas of a
    /// map. It is intended for internal use and operates on the provided map and cell data.
    /// </remarks>
    internal static class ThinRoofUtility
    {
        /// <summary>
        /// Designates thin roofs for removal in the specified cells within the given map.
        /// </summary>
        /// <param name="cells">The collection of cells to check for thin roofs.</param>
        /// <param name="map">The map containing the roof grid and area management data.</param>
        /// <remarks>
        /// This method iterates through the provided cells and designates any thin roofs for
        /// removal by updating the <see cref="AreaManager.NoRoof"/> and <see cref="AreaManager.BuildRoof"/> areas.
        /// Cells that are out of bounds or fogged are skipped.
        /// </remarks>
        internal static void DesignateThinRoofsForRemovalIfPresent(IEnumerable<IntVec3> cells, Map map)
        {
            foreach (IntVec3 cell in cells)
            {
                // Skip cells that are out of bounds or fogged
                if (!cell.InBounds(map) || cell.Fogged(map))
                {
                    continue;
                }

                // Check if the cell has a thin roof and update the area manager accordingly, specifically not
                // using the Designator because of flickering issues
                RoofDef roof = map.roofGrid.RoofAt(cell);
                if (roof != null && !roof.isThickRoof && !map.areaManager.NoRoof[cell])
                {
                    map.areaManager.NoRoof[cell] = true;
                    map.areaManager.BuildRoof[cell] = false;
                }
            }
        }
    }
}
