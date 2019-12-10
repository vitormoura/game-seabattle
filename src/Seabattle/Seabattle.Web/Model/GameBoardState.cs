using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class GameBoardState
    {
        public int Size { get; set; }

        public IEnumerable<Ship> Fleet { get; set; }
    }
}
