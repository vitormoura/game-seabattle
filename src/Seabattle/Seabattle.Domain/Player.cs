using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seabattle.Domain
{
    public class Player
    {
        /// <summary>
        /// Player unique ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Ship fleet
        /// </summary>
        public List<Ship> Fleet { get; set; }

        /// <summary>
        /// Player board
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Player is ready to play
        /// </summary>
        public bool Ready { get; set; }

        /// <summary>
        /// How many points the player has
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Change ship position identified by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public void Set(string id, Coordinates pos)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("invalid id");
            }
                        
            Board.Set(Fleet.Find(x => x.ID == id), pos);
        }
    }
}
