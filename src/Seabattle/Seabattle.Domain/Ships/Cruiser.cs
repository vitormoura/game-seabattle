using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    public class Cruiser : Ship
    {
        public Cruiser(EnumShipOrientation orientation) : base(3, orientation)
        {
        }
    }
}
