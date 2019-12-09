using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    [Serializable]
    public class Submarine : Ship
    {
        public Submarine(string id) : base(id, 1, EnumShipOrientation.Horizontal)
        {
        }
    }
}
