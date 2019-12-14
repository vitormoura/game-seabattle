using Seabattle.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class SetPlayerShipPositionRequest
    {
        public string SessionID { get; set; }

        public string ShipID { get; set; }

        public Coordinates Position { get; set; }
    }
}
