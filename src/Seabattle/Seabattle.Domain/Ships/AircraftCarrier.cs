using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    public class AircraftCarrier : Ship
    {
        public AircraftCarrier(EnumShipOrientation orientation) : base(5, orientation)
        {
        }
    }
}
