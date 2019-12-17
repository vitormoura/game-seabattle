using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public class BoardShip
    {
        public Ship Ship { get; set; }
        
        public ICollection<Coordinates> Cells { get; set; }
    }
}
