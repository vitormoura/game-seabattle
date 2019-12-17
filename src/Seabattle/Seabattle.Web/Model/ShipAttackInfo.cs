using Seabattle.Domain;
using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class ShipAttackInfo
    {
        public Coordinates Position { get; set; }

        public bool Success { get; set; }
        
        public BoardShip Target { get; set; }
    }
}
