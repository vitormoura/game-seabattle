using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public class Player
    {
        public string ID { get; set; }

        public List<Ship> Fleet { get; set; }
                
    }
}
