using Microsoft.AspNetCore.SignalR;
using Seabattle.Domain;
using Seabattle.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Hubs
{
    public class GameSessionHub : Hub
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
            await Clients.Caller.SendAsync("GameSessionFound", new GameSessionInfo
            {
                ID = gs.ID,
                PlayerID = playerId,
                BoardSize = player.Board.Width,
                PlayerFleet = player.Fleet
            });

            //GameSession is ready for action
            if (gs.State == EnumGameSessionState.WaitingPlayerConfirmation)
            {
                await Clients.Group(gs.ID).SendAsync("BeginBoardConfiguration");
            }
        }

        public async Task SetPositionPlayerShip(SetPlayerShipPositionRequest req)
        {
            var gs = await GetGameSession(req.SessionID);
            var playerId = Context.ConnectionId;

            try
            {
                gs.PositionPlayerShip(playerId, req.ShipID, req.Position);

                await Clients.Caller.SendAsync("ShipPositionConfirmed", new ShipPositionState
                {
                    Position = req.Position,
                    ShipID = req.ShipID
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("GameError", ex.Message);
            }
        }

        public async Task SetPlayerReady(SetPlayerReadyRequest req)
        {
            var gs = await GetGameSession(req.SessionID);

            //TODO: review synchronization problems
            gs.Ready(req.PlayerID);

            if (gs.State == EnumGameSessionState.Playing)
            {
                await Clients.Group(gs.ID).SendAsync("StartGameplay", new GameplayStateResponse
                {
                    CurrentPlayerTurn = gs.Current.ID
                });
            }
        }

        public async Task ShootPlayerOpponent(ShootPlayerOpponentRequest req)
        {
            var gs = await GetGameSession(req.SessionID);

            gs.Shoot(req.PlayerID, req.Position);
            
            await Clients.Group(gs.ID).SendAsync("GameplayStateChanged", new GameplayStateResponse
            {
                CurrentPlayerTurn = gs.Current.ID,
                PlayerScore = new Dictionary<string, int>
                {
                    { gs.P1.ID, gs.P1.Points },
                    { gs.P2.ID, gs.P2.Points },
                }
            });
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
    }
}
