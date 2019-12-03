using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public enum EnumGameSessionState
    {
        Created,
        WaitingForPlayers,
        WaitingPlayerConfirmation,
        Playing,
        Cancelled,
        Finished
    }
}
