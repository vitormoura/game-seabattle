using System;

namespace Seabattle.Domain
{
    public class GameSession
    {
        public string ID { get; private set; }

        public Player P1 { get; set; }

        public Player P2 { get; set; }

        public Board Board { get; private set; }

        public EnumGameSessionState State { get; private set; }


        public GameSession(string id, Player p1, Player p2)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("invalid game session id");
            }

            ID = id;
            P1 = p1;
            P2 = p2;

            State = EnumGameSessionState.Created;
        }

        /// <summary>
        /// Initializes a game session
        /// </summary>
        /// <param name="p1"></param>
        public void Init()
        {
            Board = new Board(10);
            State = EnumGameSessionState.WaitingForPlayers;
        }

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

            var player = string.IsNullOrEmpty(P1.ID) ? P1 : P2;
                     
            
            if (string.IsNullOrEmpty(P1.ID) || string.IsNullOrEmpty(P2.ID))
            {
                return;
            }
            else if(P1.ID.ToUpper() == P2.ID.ToUpper())
            {

            }

            

            State = EnumGameSessionState.WaitingPlayerConfirmation;

        }
    }
}
