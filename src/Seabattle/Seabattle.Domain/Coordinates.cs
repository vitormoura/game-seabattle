using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Coordinates
    /// </summary>
    public class Coordinates
    {
        /// <summary>
        /// Position X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position Y
        /// </summary>
        public int Y { get; set; }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
