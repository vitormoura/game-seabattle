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

            //Sending to client his new GameSession
            await Clients.Caller.SendAsync("GameSessionFound", new GameSessionInfo
            {
                ID = gs.ID,
                Player = playerId
            });

            //GameSession is ready for action
            if(gs.State == EnumGameSessionState.WaitingPlayerConfirmation)
            {
                var player = gs.GetPlayer(playerId);
                                                                
                await Clients.Group(gs.ID).SendAsync("BeginBoardConfiguration", new GameBoardState
                {
                    Size = player.Board.Width,
                    Fleet = player.Fleet
                });
            }
        }
    }
}
