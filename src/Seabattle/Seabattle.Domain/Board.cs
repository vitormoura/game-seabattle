using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Sea battle board
    /// </summary>
    public class Board
    {
        private readonly List<Ship> fleet;

        private readonly Ship[,] grid;

        /// <summary>
        /// Board width
        /// </summary>
        public int Width
        {
            get { return grid.GetLength(0); }
        }

        /// <summary>
        /// Fleet of ships
        /// </summary>
        public IEnumerable<Ship> Fleet
        {
            get
            {
                return fleet;
            }
        }

        /// <summary>
        /// Create a new instance of Board with a specified width
        /// </summary>
        /// <param name="width"></param>
        public Board(int width)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"invalid grid width: {width}");
            }

            fleet = new List<Ship>();
            grid = new Ship[width, width];
        }

        /// <summary>
        /// Try to position a new ship at coordinates pos
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="pos"></param>
        public void Set(Ship ship, Coordinates pos)
        {
            if (pos == null)
            {
                throw new ArgumentNullException(nameof(pos));
            }

            if (ship == null)
            {
                throw new ArgumentNullException(nameof(ship));
            }

            if (!CheckValidPosition(pos, ship.Size, ship.Orientation))
            {
                throw new ArgumentException($"invalid coordinates or ship position: {pos}");
            }

            for (int i = 0; i < ship.Size; i++)
            {
                //TODO: Refactor, this code is duplicated in line #128 
                if (ship.Orientation == EnumShipOrientation.Horizontal)
                {
                    grid[pos.Y, pos.X + i] = ship;
                }
                else
                {
                    grid[pos.Y + i, pos.X] = ship;
                }
            }

            fleet.Add(ship);
        }

        /// <summary>
        /// Get current ship at position pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Ship Get(Coordinates pos)
        {
            if (pos == null)
            {
                throw new ArgumentNullException(nameof(pos));
            }

            return grid[pos.X, pos.Y];
        }

        /// <summary>
        /// Get current ship at position x, y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Ship Get(int x, int y)
        {
            return Get(new Coordinates { X = x, Y = y });
        }

        /// <summary>
        /// Returns a string representation of this board
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            var ids = new Dictionary<object, char>();
            var currentId = 'A';

            for (int y = 0; y < Width; y++)
            {
                sb.Append("[ ");

                for (int x = 0; x < Width; x++)
                {
                    var ship = grid[y, x];

                    if (ship != null)
                    {
                        if (!ids.ContainsKey(ship))
                        {
                            ids.Add(ship, currentId);
                            currentId = (char)(currentId + 1);
                        }

                        sb.Append(ids[ship]);
                    }
                    else
                        sb.Append("-");
                }

                sb.AppendFormat(" ]{0}", Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Verifies if the informed position is valid for a new ship
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="shipSize"></param>
        /// <param name="orientation"></param>
        /// <returns></returns>
        private bool CheckValidPosition(Coordinates pos, int shipSize, EnumShipOrientation orientation)
        {
            var gridWidth = Width;

            if (shipSize <= 0)
            {
                return false;
            }

            if (pos.X > gridWidth || pos.Y > gridWidth)
            {
                return false;
            }

            int shipAnchorPos = orientation == EnumShipOrientation.Horizontal ? pos.X : pos.Y;

            if (shipAnchorPos + shipSize > gridWidth)
            {
                return false;
            }

            for (int i = 0; i < shipSize; i++)
            {
                var currentShip = orientation == EnumShipOrientation.Horizontal ? Get(pos.Y, pos.X + i) : Get(pos.Y + i, pos.X);

                if (currentShip != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
