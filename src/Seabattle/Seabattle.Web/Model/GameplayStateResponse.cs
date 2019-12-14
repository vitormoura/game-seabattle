using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class GameplayStateResponse
    {
        public string CurrentPlayerTurn { get; set; }

        public Dictionary<string,int> PlayerScore { get; set; }
    }
}
