using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Default player factory implementation
    /// </summary>
    public class PlayerFactory : IPlayerFactory
    {
        public Player New(string id, int boardSize)
        {
            var p = new Player
            {
                ID = id,
                Board = new Board(boardSize),
                Fleet = new List<Ship>
                {
                    new AircraftCarrier(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Battleship(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Cruiser(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Destroyer(GenerateShipID(), EnumShipOrientation.Horizontal),
                    new Destroyer(GenerateShipID(), EnumShipOrientation.Horizontal),
                    new Submarine(GenerateShipID()),
                    new Submarine(GenerateShipID())
                }
            };

            return p;
        }

        private string GenerateShipID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
