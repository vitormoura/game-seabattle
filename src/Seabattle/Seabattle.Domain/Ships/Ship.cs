using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain.Ships
{
    /// <summary>
    /// Generic definition of a ship
    /// </summary>
    /// 
    [Serializable]
    public class Ship
    {
        /// <summary>
        /// Unique ship identifier
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Size in units
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Type description
        /// </summary>
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Display orientation
        /// </summary>
        public EnumShipOrientation Orientation { get; private set; }

        /// <summary>
        /// Create a new instance of Ship
        /// </summary>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        public Ship(string id, int size, EnumShipOrientation orientation)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("invalid ship id");
            }

            if (size <= 0)
            {
                throw new ArgumentException($"invalid ship size: {size}");
            }

            ID = id;
            Size = size;
            Orientation = orientation;
        }        
    }
}
