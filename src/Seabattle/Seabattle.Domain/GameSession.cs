using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seabattle.Domain
{
    /// <summary>
    /// Represents a game session
    /// </summary>
    public class GameSession
    {
        private IPlayerFactory playerFactory;

        /// <summary>
        /// Game session unique ID
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Current player in turn
        /// </summary>
        public Player Current { get; private set; }

        /// <summary>
        /// Winner player
        /// </summary>
        public Player Winner { get; private set; }

        /// <summary>
        /// Player 1
        /// </summary>
        public Player P1 { get; set; }

        /// <summary>
        /// Player 2
        /// </summary>
        public Player P2 { get; set; }

        /// <summary>
        /// Collection of session players
        /// </summary>
        public IEnumerable<Player> Players
        {
            get
            {
                return new List<Player> { P1, P2 };
            }
        }

        /// <summary>
        /// State of current game session
        /// </summary>
        public EnumGameSessionState State { get; private set; }

        /// <summary>
        /// Create a new instance of GameSession
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pf"></param>
        public GameSession(string id, IPlayerFactory pf)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("invalid game session id");
            }

            ID = id;
            State = EnumGameSessionState.Created;
            playerFactory = pf ?? throw new ArgumentNullException(nameof(pf));
        }

        /// <summary>
        /// Initializes a game session
        /// </summary>
        public void Init()
        {
            State = EnumGameSessionState.WaitingForPlayers;
        }

        /// <summary>
        /// A new player join the game session
        /// </summary>
        /// <param name="id"></param>
        public void Join(string id)
        {
            CheckCurrentState(EnumGameSessionState.WaitingForPlayers, "join is only possible during WaitingForPlayers state");

            const int BOARD_SIZE = 10;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("invalid player id");
            }
                        
            if (P1 == null)
            {
                P1 = playerFactory.New(id, BOARD_SIZE);
            }
            else
            {
                if (id.ToLower() == P1.ID.ToLower())
                {
                    throw new ArgumentException("A player cannot play with him/her self");
                }

                P2 = playerFactory.New(id, BOARD_SIZE);
                State = EnumGameSessionState.WaitingPlayerConfirmation;
            }
        }

        /// <summary>
        /// Signal when player is ready
        /// </summary>
        /// <param name="playerId"></param>
        public void Ready(string playerId)
        {
            CheckCurrentState(EnumGameSessionState.WaitingPlayerConfirmation, "game Session is not waiting for player confirmation");
                        
            var player = GetRequiredPlayer(playerId);
            GetReady(player);

            //When both ready, start the game
            if (P1.Ready && P2.Ready)
            {
                State = EnumGameSessionState.Playing;
                Current = P1;
            }
        }

        /// <summary>
        /// Position player ship in his board
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="shipId"></param>
        /// <param name="pos"></param>
        public void PositionPlayerShip(string playerId, string shipId, Coordinates pos)
        {
            CheckCurrentState(EnumGameSessionState.WaitingPlayerConfirmation, "invalid game state");
            GetRequiredPlayer(playerId).Set(shipId, pos);
        }

        /// <summary>
        /// Position all player ships
        /// </summary>
        /// <param name="playerId"></param>
        public void PositionAllPlayerShips(string playerId)
        {
            CheckCurrentState(EnumGameSessionState.WaitingPlayerConfirmation, "invalid game state");

            var player = GetRequiredPlayer(playerId);
            player.Board.Set(player.Fleet);
        }

        /// <summary>
        /// Register player shoot
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="pos"></param>
        public bool Shoot(string playerId, Coordinates pos)
        {
            CheckCurrentState(EnumGameSessionState.Playing, "invalid game state");
            
            if (pos == null)
            {
                throw new ArgumentNullException(nameof(pos));
            }

            if (string.IsNullOrEmpty(playerId))
            {
                throw new ArgumentException("invalid player id");
            }

            if (Current.ID != playerId)
            {
                throw new InvalidOperationException("invalid player turn");
            }

            var opponent = Current.ID == P1.ID ? P2 : P1;
            var target = opponent.Board.Get(pos);

            if (target != null)
            {
                Current.Points++;
                opponent.Board.Remove(target);
            }

            //Is it a game over?
            if (opponent.Board.Fleet.Count() == 0)
            {
                State = EnumGameSessionState.Finished;
                Winner = Current;
            }

            //Turn changes
            Current = opponent;

            return target != null;
        }

        /// <summary>
        /// Get a reference of the player identified by playerId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public Player GetPlayer(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("invalid player id");
            }

            return P1.ID == playerId ? P1 : (P2.ID == playerId ? P2 : null);
        }

        /// <summary>
        /// Get a reference of the opponent player
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public Player GetPlayerOpponent(string playerId)
        {
            var player = GetPlayer(playerId);
            return player == P1 ? P2 : P1;
        }

        /// /////////////////////////////////////////////////////////

        private bool GetReady(Player p)
        {
            var positionedShips = p.Board.Fleet.Count();
            var fleetQtd = p.Fleet.Count;

            p.Ready = positionedShips == fleetQtd;

            return p.Ready;
        }

        private void CheckCurrentState(EnumGameSessionState state, string message)
        {
            if (State != state)
            {
                throw new ArgumentException(message);
            }
        }

        private Player GetRequiredPlayer(string playerId)
        {
            var p = GetPlayer(playerId);

            if (p == null)
            {
                throw new ArgumentException("this player is not in this game session");
            }

            return p;
        }

        
    }
}
