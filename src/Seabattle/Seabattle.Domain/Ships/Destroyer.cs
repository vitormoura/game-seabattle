using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    public class Destroyer : Ship
    {
        public Destroyer(EnumShipOrientation orientation) : base(2, orientation)
        {
        }
    }
}
