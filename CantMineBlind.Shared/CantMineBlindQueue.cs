using System.Collections.Generic;
using Verse;

namespace CantMineBlind
{
    /// <summary>
    /// Manages a queue of designations for removing thin roofs, ensuring a delay before processing each designation.
    /// </summary>
    /// <remarks>
    /// This component is responsible for queuing and processing roof removal designations in a controlled manner. 
    /// Designations are delayed for a short period to avoid input overlap or conflicts. Use the 
    /// <see cref="Enqueue"/> method to add new designations to the queue.
    /// </remarks>
    internal class CantMineBlindQueue : GameComponent
    {
        private readonly List<QueuedDesignation> pendingDesignations = new List<QueuedDesignation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CantMineBlindQueue"/> class.
        /// </summary>
        /// <param name="game">The game instance associated with this queue. Cannot be null.</param>
        public CantMineBlindQueue(Game game)
        {
        }

        /// <summary>
        /// Gets the current instance of the <see cref="CantMineBlindQueue"/> component associated with the game.
        /// </summary>
        public static CantMineBlindQueue Get => Current.Game.GetComponent<CantMineBlindQueue>();

        /// <summary>
        /// Processes and applies pending roof removal designations during each game tick.
        /// </summary>
        /// <remarks>
        /// This method iterates through the list of pending designations, decrements their delay
        /// counters, and removes thin roofs from the specified cells on the associated map when the delay reaches
        /// zero. Designations are removed from the queue once processed.
        /// </remarks>
        public override void GameComponentTick()
        {
            for (int i = pendingDesignations.Count - 1; i >= 0; i--)
            {
                QueuedDesignation item = pendingDesignations[i];
                item.DelayTicks--;

                if (item.DelayTicks <= 0)
                {
                    ThinRoofUtility.DesignateThinRoofsForRemovalIfPresent(item.Cells, item.Map);
                    pendingDesignations.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds a new designation to the queue for processing.
        /// </summary>
        /// <param name="map">The map where the designation will be applied. Cannot be null.</param>
        /// <param name="cells">The collection of cells to be designated. Cannot be null or empty.</param>
        /// <remarks>
        /// The designation is queued with a delay to prevent input overlap. Ensure that
        /// <paramref name="map"/> and <paramref name="cells"/> are valid and properly initialized before calling this
        /// method.
        /// </remarks>
        internal void Enqueue(Map map, IEnumerable<IntVec3> cells)
        {
            pendingDesignations.Add(new QueuedDesignation
            {
                Map = map,
                Cells = new List<IntVec3>(cells),
                DelayTicks = 10 // Enough to avoid input overlap
            });
        }

        /// <summary>
        /// Represents a designation that is queued for processing, including its associated map, target cells, and
        /// delay.
        /// </summary>
        /// <remarks>
        /// This class encapsulates the details of a queued designation, including the map where the designation applies, 
        /// the specific cells affected, and the delay in ticks before the designation is processed.
        /// </remarks>
        private class QueuedDesignation
        {
            public Map Map;
            public List<IntVec3> Cells;
            public int DelayTicks;
        }
    }
}
