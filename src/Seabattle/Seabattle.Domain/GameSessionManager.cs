using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seabattle.Domain
{
    /// <summary>
    /// Component responsable to manage the game sessions instances availables
    /// </summary>
    public class GameSessionManager
    {
        private readonly List<GameSession> sessions = new List<GameSession>();
        private readonly IPlayerFactory playerFactory;

        public GameSessionManager(IPlayerFactory playerFactory)
        {
            this.playerFactory = playerFactory;
        }

        /// <summary>
        /// Find or creates a new game session for a player
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public Task<GameSession> Find(string playerId)
        {
            lock (sessions)
            {
                var session = null as GameSession;
                var available = sessions.Where(x => x.State == EnumGameSessionState.WaitingForPlayers).ToList();
                
                if (available.Count > 0)
                {
                    session = available.FirstOrDefault();
                }
                else
                {
                    session = new GameSession(Guid.NewGuid().ToString(), playerFactory);
                    session.Init();

                    sessions.Add(session);
                }

                session.Join(playerId);

                return Task.FromResult(session);
            }
        }
    }
}
