using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Game session states
    /// </summary>
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
