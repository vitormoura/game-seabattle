using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Seabattle.Domain.Tests
{
    public class GameplayTests
    {
        private const string P1 = "p1";

        private const string P2 = "p2";

        private const string SUB_1 = "SUB-1";

        private const string SUB_2 = "SUB-2";

        private GameSession GS { get; set; }

        public GameplayTests()
        {
            GS = new GameSession("DUMMY_SESSION", new TestPlayerFactory(new List<Ship>
            {
                new Submarine(SUB_1),
                new Submarine(SUB_2),
            }));

            GS.Init();

            //Waiting for players

            GS.Join(P1);
            GS.Join(P2);

            //Setting up board

            GS.P1.Set(SUB_1, new Coordinates { X = 0, Y = 1 });
            GS.P1.Set(SUB_2, new Coordinates { X = 1, Y = 1 });

            GS.P2.Set(SUB_1, new Coordinates { X = 0, Y = 4 });
            GS.P2.Set(SUB_2, new Coordinates { X = 1, Y = 4 });

            //Ready for action

            GS.Ready(P1);
            GS.Ready(P2);

            //PLAY!

        }

        [Fact]
        public void With_InitialGameplay_WhenBegin_InitialPointsShouldBeZero()
        {
            Assert.Equal(0, GS.P1.Points);
            Assert.Equal(0, GS.P2.Points);
        }

        [Fact]
        public void With_GameInCourse_WhenTheWrongPlayersPlays_ExceptionIsThrown()
        {
            GS.Shoot(P1, new Coordinates { X = 0, Y = 6 });

            Assert.Throws<InvalidOperationException>(() =>
           {
               GS.Shoot(P1, new Coordinates { X = 0, Y = 4 });
           });
        }

        [Fact]
        public void With_InitialBoardState_WhenPlayersPlay_StateChangeAccordingly()
        {
            Assert.True(GS.Shoot(P1, new Coordinates { X = 0, Y = 4 }));
            Assert.Equal(1, GS.P1.Points);
            Assert.Equal(EnumGameSessionState.Playing, GS.State);

            Assert.False(GS.Shoot(P2, new Coordinates { X = 5, Y = 5 }));

            Assert.True(GS.Shoot(P1, new Coordinates { X = 1, Y = 4 }));
            Assert.Equal(2, GS.P1.Points);
            Assert.Equal(EnumGameSessionState.Finished, GS.State);
        }
    }
}
