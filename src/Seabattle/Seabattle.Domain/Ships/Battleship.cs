using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    [Serializable]
    public class Battleship : Ship
    {
        public Battleship(string id, EnumShipOrientation orientation) : base(id, 4, orientation)
        {
        }
    }
}
