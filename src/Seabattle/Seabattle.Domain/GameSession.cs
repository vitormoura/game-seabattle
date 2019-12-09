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
        public string ID { get; private set; }

        public Player Current { get; private set; }

        public Player Winner { get; private set; }

        public Player P1 { get; set; }

        public Player P2 { get; set; }
        
        public EnumGameSessionState State { get; private set; }
                
        public GameSession(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("invalid game session id");
            }

            ID = id;

            State = EnumGameSessionState.Created;
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
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("invalid player id");
            }

            if (State != EnumGameSessionState.WaitingForPlayers)
            {
                throw new InvalidOperationException("Join is only possible during WaitingForPlayers state");
            }
                        
            if (P1 == null)
            {
                P1 = PrepareNewPlayer(id);
            }
            else
            {
                if(id.ToLower() == P1.ID.ToLower())
                {
                    throw new ArgumentException("A player cannot play with him/her self");
                }

                P2 = PrepareNewPlayer(id);
                State = EnumGameSessionState.WaitingPlayerConfirmation;
            }
        }
        
        /// <summary>
        /// Signal when player is ready
        /// </summary>
        /// <param name="playerId"></param>
        public void Ready(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("invalid player id");
            }

            if (State != EnumGameSessionState.WaitingPlayerConfirmation)
            {
                throw new InvalidOperationException("Game Session is not waiting for player confirmation");
            }

            if (P1.ID == playerId)
            {
                GetReady(P1);
            }
            else if (P2.ID == playerId)
            {
                GetReady(P2);
            }
            else
                throw new ArgumentException("unknown player id");

            //When both ready, start the game
            if(P1.Ready && P2.Ready)
            {
                State = EnumGameSessionState.Playing;
                Current = P1;
            }
        }

        /// <summary>
        /// Register player shoot
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="pos"></param>
        public void Shoot(string playerId, Coordinates pos)
        {
            if(State != EnumGameSessionState.Playing)
            {
                throw new ArgumentException("invalid game state");
            }

            if(pos == null)
            {
                throw new ArgumentNullException(nameof(pos));
            }

            if(string.IsNullOrEmpty(playerId))
            {
                throw new ArgumentException("invalid player id");
            }

            if(Current.ID != playerId)
            {
                throw new ArgumentException("invalid player turn");
            }

            var opponent = Current.ID == P1.ID ? P2 : P1;
            var target = opponent.Board.Get(pos);

            if(target != null)
            {
                Current.Points++;
                opponent.Board.Remove(target);
            }

            //Is it a game over?
            if(opponent.Board.Fleet.Count() == 0)
            {
                State = EnumGameSessionState.Finished;
                Winner = Current;

                return;
            }

            //Turn changes
            Current = opponent;
        }

        /// /////////////////////////////////////////////////////////

        private Player PrepareNewPlayer(string id)
        {
            var p = new Player
            {
                ID = id,
                Board = new Board(10),
                Fleet = new List<Ship>
                {
                    new AircraftCarrier(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Battleship(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Cruiser(GenerateShipID(), EnumShipOrientation.Vertical),
                    new Destroyer(GenerateShipID(), EnumShipOrientation.Horizontal),
                    new Destroyer(GenerateShipID(), EnumShipOrientation.Horizontal),
                    new Submarine(GenerateShipID()),
                    new Submarine(GenerateShipID())
                }
            };
                        
            return p;
        }

        private string GenerateShipID()
        {
            return Guid.NewGuid().ToString();
        }

        private bool GetReady(Player p)
        {
            var positionedShips = p.Board.Fleet.Count();
            var fleetQtd = p.Fleet.Count;
                        
            p.Ready = positionedShips == fleetQtd;

            return p.Ready;
        }
    }
}
