﻿using Microsoft.AspNetCore.SignalR;
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
            if(gs.State == EnumGameSessionState.WaitingPlayerConfirmation)
            {
                await Clients.Group(gs.ID).SendAsync("BeginBoardConfiguration");
            }
        }

        public async Task SetPositionPlayerShip(string sessionId, string shipId, Coordinates pos)
        {
            var playerId = Context.ConnectionId;
            var gs = await SessionManager.Get(sessionId);

            if (gs == null)
            {
                throw new InvalidOperationException("unknown session");
            }

            try
            {
                gs.PositionPlayerShip(playerId, shipId, pos);

                await Clients.Caller.SendAsync("ShipPositionConfirmed", new ShipPositionState
                {
                    Position = pos,
                    ShipID = shipId
                });
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("GameError", ex.Message);
            }
        }
    }
}
