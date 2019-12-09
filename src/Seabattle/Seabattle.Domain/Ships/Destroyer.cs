using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    [Serializable]
    public class Destroyer : Ship
    {
        public Destroyer(string id, EnumShipOrientation orientation) : base(id, 2, orientation)
        {
        }
    }
}
