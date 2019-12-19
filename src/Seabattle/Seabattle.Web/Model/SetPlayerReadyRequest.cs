using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Model
{
    public class SetPlayerReadyRequest
    {
        public string SessionID { get; set; }

        public string PlayerID { get; set; }
    }
}
