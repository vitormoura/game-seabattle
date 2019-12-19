using Seabattle.Web.Model;
using System.Threading.Tasks;

namespace Seabattle.Web.Hubs
{
    /// <summary>
    /// Defines the game session hub operations
    /// </summary>
    public interface IGameSessionHubClient
    {
        /// <summary>
        /// Indicates that a game session is found
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        Task GameSessionFound(GameSessionInfo resp);

        /// <summary>
        /// Informs that the opponent attacked the player
        /// </summary>
        /// <param name="attack"></param>
        /// <returns></returns>
        Task OpponentAttack(ShipAttackInfo attack);

        /// <summary>
        /// Game session state changed
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        Task GameSessionStateChanged(GameplayStateResponse resp);

        /// <summary>
        /// Informs the end of game session
        /// </summary>
        /// <param name="resp"></param>
        /// <returns></returns>
        Task GameOver(GameplayStateResponse resp);
    }
}
