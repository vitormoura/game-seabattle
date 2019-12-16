using Seabattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class ShipAttackInfo
    {
        public Coordinates Position { get; set; }
        
        public bool Hit { get; set; }
    }
}
