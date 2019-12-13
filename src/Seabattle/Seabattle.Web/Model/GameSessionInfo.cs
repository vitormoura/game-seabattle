using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class GameSessionInfo
    {
        public string ID { get; set; }

        public string PlayerID { get; set; }

        public int BoardSize { get; set; }

        public IEnumerable<Ship> PlayerFleet { get; set; }
    }
}
