using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seabattle.Domain
{
    public class Board
    {
        private readonly List<Ship> fleet;

        private readonly Ship[,] grid;

        public int Width
        {
            get { return grid.GetLength(0); }
        }

        public IEnumerable<Ship> Fleet
        {
            get
            {
                return fleet;
            }
        }

        public Board(int width)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"invalid grid width: {width}");
            }

            fleet = new List<Ship>();
            grid = new Ship[width, width];
        }

        public void Position(Ship ship, Coordinates pos)
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
                var currentShip = orientation == EnumShipOrientation.Horizontal ? grid[pos.Y, pos.X + i] : grid[pos.Y + i, pos.X];

                if (currentShip != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
