using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Prepare new GameSession instances
    /// </summary>
    public class GameSessionFactory
    {
        /// <summary>
        /// Create new GameSession
        /// </summary>
        /// <returns></returns>
        public GameSession Create()
        {
            var p1 = new Player
            {
                ID = string.Empty,
                Fleet = PrepareDefaultFleet()
            };

            var p2 = new Player
            {
                ID = string.Empty,
                Fleet = PrepareDefaultFleet()
            };

            var gs = new GameSession(Guid.NewGuid(), p1, p2);

            return gs;

        }

        private List<Ship> PrepareDefaultFleet()
        {
            return new List<Ship>
            {
                new AircraftCarrier(EnumShipOrientation.Vertical),
                new Battleship(EnumShipOrientation.Vertical),
                new Cruiser( EnumShipOrientation.Vertical),
                new Destroyer( EnumShipOrientation.Horizontal),
                new Destroyer(EnumShipOrientation.Horizontal),
                new Submarine(),
                new Submarine()
            };
        }
    }
}
