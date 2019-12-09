using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    /// <summary>
    /// Prepare new GameSession instances
    /// </summary>
    public class GameSessionFactory
    {
        /// <summary>
        /// Create new GameSession
        /// </summary>
        /// <returns></returns>
        public GameSession Create()
        {
            return new GameSession(Guid.NewGuid().ToString());
        }        
    }
}
