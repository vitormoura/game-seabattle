using Microsoft.AspNetCore.SignalR;
using Seabattle.Domain;
using Seabattle.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Hubs
{
    public class GameSessionHub : Hub<IGameSessionHubClient>
    {
        public GameSessionManager SessionManager { get; }

        public GameSessionHub(GameSessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        public async Task FindNewGameSession()
        {
            var playerId = Context.ConnectionId;
            var gs = await SessionManager.Find(playerId);

            //A game session is a group
            await Groups.AddToGroupAsync(Context.ConnectionId, gs.ID);

            var player = gs.GetPlayer(playerId);

            //Sending to client his new GameSession
            await Clients.Caller.GameSessionFound(new GameSessionInfo
            {
                ID = gs.ID,
                PlayerID = playerId,
                BoardSize = player.Board.Width,
                PlayerFleet = player.Fleet
            });

            await Clients.Group(gs.ID).GameSessionStateChanged(new GameplayStateResponse
            {
                State = gs.State,
                CurrentPlayerTurn = null
            });
        }

        public async Task<ShipPositionState> PositionShip(SetPlayerShipPositionRequest req)
        {
            var gs = await GetGameSession(req.SessionID);
            var playerId = Context.ConnectionId;

            gs.PositionPlayerShip(playerId, req.ShipID, req.Position);

            return new ShipPositionState
            {
                Position = req.Position,
                ShipID = req.ShipID
            };
        }

        public async Task SetPlayerIsReady(SetPlayerReadyRequest req)
        {
            var playerId = Context.ConnectionId;
            var gs = await GetGameSession(req.SessionID);

            //TODO: review synchronization problems
            gs.Ready(playerId);
            
            await Clients.Group(gs.ID).GameSessionStateChanged(GetGameplayState(gs));
        }

        public async Task ShootOpponent(ShootPlayerOpponentRequest req)
        {
            var playerId = Context.ConnectionId;
            var gs = await GetGameSession(req.SessionID);
            var opponent = gs.GetPlayerOpponent(playerId);
            
            var hittedShip = gs.Shoot(playerId, req.Position);

            await Clients.Client(opponent.ID).OpponentAttack(new ShipAttackInfo { 
                Position = req.Position, 
                TargetID = hittedShip?.ID
            });

            await Clients.Group(gs.ID).GameSessionStateChanged(GetGameplayState(gs));
        }

        private async Task<GameSession> GetGameSession(string id)
        {
            var gs = await SessionManager.Get(id);

            if (gs == null)
            {
                throw new InvalidOperationException("unknown session");
            }

            return gs;
        }
        private GameplayStateResponse GetGameplayState(GameSession gs)
        {
            return new GameplayStateResponse
            {
                CurrentPlayerTurn = gs.Current?.ID,
                State = gs.State,
                PlayerScore = new Dictionary<string, int>
                {
                    { gs.P1.ID, gs.P1.Points },
                    { gs.P2.ID, gs.P2.Points },
                }
            };
        }
    }
}
