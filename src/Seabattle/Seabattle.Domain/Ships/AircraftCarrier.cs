using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    [Serializable]
    public class AircraftCarrier : Ship
    {
        public AircraftCarrier(string id, EnumShipOrientation orientation) : base(id, 5, orientation)
        {
        }
    }
}
