using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    public class Battleship : Ship
    {
        public Battleship(EnumShipOrientation orientation) : base(4, orientation)
        {
        }
    }
}
