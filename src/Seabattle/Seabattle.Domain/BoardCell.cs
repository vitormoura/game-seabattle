using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public class BoardCell
    {
        public Coordinates Position { get; set; }

        public Ship Ship { get; set; }
    }
}
