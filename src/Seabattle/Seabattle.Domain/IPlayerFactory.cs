using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Component responsable for create new player instances
    /// </summary>
    public interface IPlayerFactory
    {
        /// <summary>
        /// Create new player with a board and fleet
        /// </summary>
        /// <returns></returns>
        Player New(string id, int boardSize);
    }
}
