using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    public class Submarine : Ship
    {
        public Submarine() : base(1, EnumShipOrientation.Horizontal)
        {
        }
    }
}
