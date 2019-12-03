using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    /// <summary>
    /// Generic definition of a ship
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// Size in units
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Display orientation
        /// </summary>
        public EnumShipOrientation Orientation { get; private set; }

        /// <summary>
        /// Create a new instance of Ship
        /// </summary>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        public Ship(int size, EnumShipOrientation orientation)
        {
            if (size <= 0)
            {
                throw new ArgumentException($"invalid ship size: {size}");
            }

            Size = size;
            Orientation = orientation;
        }        
    }
}
