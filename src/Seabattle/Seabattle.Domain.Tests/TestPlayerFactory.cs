using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Seabattle.Domain.Tests
{
    public class TestPlayerFactory : IPlayerFactory
    {
        public List<Ship> SampleFleet { get; private set; }

        public TestPlayerFactory(IEnumerable<Ship> sampleFleet)
        {
            SampleFleet = sampleFleet.ToList();
        }

        public Player New(string id, int boardSize)
        {
            var p = new Player
            {
                ID = id,
                Board = new Board(boardSize),
                Fleet = DeepClone(SampleFleet)
            };

            return p;
        }

        private static T DeepClone<T>(T obj)
        {
            T objResult;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                objResult = (T)bf.Deserialize(ms);
            }
            return objResult;
        }
    }
}
