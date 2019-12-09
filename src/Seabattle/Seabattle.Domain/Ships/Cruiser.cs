using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    [Serializable]
    public class Cruiser : Ship
    {
        public Cruiser(string id, EnumShipOrientation orientation) : base(id, 3, orientation)
        {
        }
    }
}
