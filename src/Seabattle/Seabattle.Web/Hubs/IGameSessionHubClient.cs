using Seabattle.Domain;
using Seabattle.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seabattle.Web.Hubs
{
    /// <summary>
    /// Defines the game session hub operations
    /// </summary>
    public interface IGameSessionHubClient
    {
        Task GameSessionFound(GameSessionInfo resp);

        Task OpponentAttack(ShipAttackInfo attack);

        Task GameSessionStateChanged(GameplayStateResponse resp);        
    }
}
