using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public class Coordinates
    {
        public int X { get; set; }

        public int Y { get; set; }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
