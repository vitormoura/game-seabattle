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
        private readonly Dictionary<string, Ship> fleet;

        private BoardCell[,] grid;

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
                return fleet.Values;
            }
        }

        public IEnumerator<BoardCell> GetCells()
        {
            for (int y = 0; y < Width; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return grid[y, x];
                }
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

            fleet = new Dictionary<string, Ship>();

            PrepareGrid(width);
        }

        /// <summary>
        /// Position automatically a fleet of ships
        /// </summary>
        /// <param name="fleet"></param>
        public void Set(IEnumerable<Ship> fleet)
        {
            if (fleet == null)
            {
                throw new ArgumentNullException(nameof(fleet));
            }

            var qtdShips = fleet.Count();
            var maxWidth = 0;
            var index = 0;

            //TODO: Review implementation

            foreach (var s in fleet.Where(x => x.Orientation == EnumShipOrientation.Horizontal).ToList())
            {
                Set(s, new Coordinates { X = 0, Y = index++ });

                if(maxWidth < s.Size)
                {
                    maxWidth = s.Size;
                }
            }

            foreach(var s in fleet.Where(x => x.Orientation == EnumShipOrientation.Vertical).ToList())
            {
                Set(s, new Coordinates { X = maxWidth++, Y = 0 });
            }
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

            if (string.IsNullOrEmpty(ship.ID))
            {
                throw new ArgumentException("every ship on board must have an id");
            }

            if (!CheckValidPosition(pos, ship.Size, ship.Orientation))
            {
                throw new ArgumentException($"invalid coordinates or ship position: {pos}");
            }

            Remove(ship);

            for (int i = 0; i < ship.Size; i++)
            {
                //TODO: Refactor, this code is duplicated in line #128 
                if (ship.Orientation == EnumShipOrientation.Horizontal)
                {
                    grid[pos.Y, pos.X + i].Ship = ship;
                }
                else
                {
                    grid[pos.Y + i, pos.X].Ship = ship;
                }
            }

            fleet.Add(ship.ID, ship);
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

            return grid[pos.Y, pos.X].Ship;
        }

        /// <summary>
        /// Remove ship from board
        /// </summary>
        /// <param name="ship"></param>
        public void Remove(Ship ship)
        {
            if (ship == null)
            {
                throw new ArgumentNullException(nameof(ship));
            }

            if (!fleet.ContainsKey(ship.ID))
            {
                return;
            }

            for (int y = 0; y < Width; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (grid[y, x].Ship == ship)
                    {
                        grid[y, x].Ship = null;
                    }
                }
            }

            fleet.Remove(ship.ID);
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
            var ids = new Dictionary<string, char>();
            var currentId = 'A';

            for (int y = 0; y < Width; y++)
            {
                sb.Append("[ ");

                for (int x = 0; x < Width; x++)
                {
                    var cell = grid[y, x];

                    if (cell.Ship != null)
                    {
                        if (!ids.ContainsKey(cell.Ship.ID))
                        {
                            ids.Add(cell.Ship.ID, currentId);
                            currentId = (char)(currentId + 1);
                        }

                        sb.Append(ids[cell.Ship.ID]);
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
                var currentShip = orientation == EnumShipOrientation.Horizontal ? Get(pos.X + i, pos.Y) : Get(pos.X, pos.Y + i);

                if (currentShip != null)
                {
                    return false;
                }
            }

            return true;
        }

        private void PrepareGrid(int width)
        {
            grid = new BoardCell[width, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grid[j, i] = new BoardCell
                    {
                        Position = new Coordinates { X = j, Y = i }
                    };
                }
            }
        }
    }
}
