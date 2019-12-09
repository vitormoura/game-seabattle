using Seabattle.Domain.Ships;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Seabattle.Domain.Tests
{
    public class GameSessionTests
    {
        private GameSession GS { get; set; }

        public GameSessionTests()
        {
            GS = new GameSession("DUMMY_SESSION", new TestPlayerFactory( new List<Ship>
            {
                new Submarine("SUB-1"),
                new Submarine("SUB-2"),
            }));
        }

        [Fact]
        public void With_ValidCreateParams_WhenCreateSession_StateIs()
        {
            Assert.Null(GS.P1);
            Assert.Null(GS.P2);
            Assert.Equal("DUMMY_SESSION", GS.ID);
            Assert.Equal(EnumGameSessionState.Created, GS.State);
        }

        [Fact]
        public void With_InitialState_WhenInitSession_StateIsWaitingForPlayers()
        {
            GS.Init();
            Assert.Equal(EnumGameSessionState.WaitingForPlayers, GS.State);
        }

        [Fact]
        public void With_NoPlayer_WhenFirstPlayerJoins_StateIsWaitingForPlayers()
        {
            GS.Init();
            GS.Join("player 1");

            Assert.NotNull(GS.P1);
            Assert.Null(GS.P2);
            Assert.Equal(EnumGameSessionState.WaitingForPlayers, GS.State);
        }

        [Fact]
        public void With_OnePlayerJoined_WhenTheSamePlayerJoins_ExceptionIsThrown()
        {
            GS.Init();
            GS.Join("player 1");

            Assert.Throws<ArgumentException>(() =>
            {
                GS.Join("player 1");
            });
        }

        [Fact]
        public void With_BothPlayersJoined_WhenJoinIsCalledAgain_ExceptionIsThrown()
        {
            GS.Init();
            GS.Join("player 1");
            GS.Join("player 2");

            Assert.Throws<InvalidOperationException>(() =>
            {
                GS.Join("player 3");
            });
        }

        [Fact]
        public void With_FirstPlayerAllreadyJoined_WhenSecondPlayerJoins_StateIsWaitingPlayerConfirmation()
        {
            GS.Init();
            GS.Join("player 1");
            GS.Join("player 2");
            
            Assert.NotNull(GS.P1);
            Assert.NotNull(GS.P2);
            Assert.Equal(EnumGameSessionState.WaitingPlayerConfirmation, GS.State);
        }

        [Fact]
        public void With_StateWaitingConfirmation_WhenSetReadyWithoutFleetPositioned_StateDoesntChange()
        {
            GS.Init();
            GS.Join("p1");
            GS.Join("p2");

            Assert.Equal(EnumGameSessionState.WaitingPlayerConfirmation, GS.State);

            GS.Ready("p1");
            GS.Ready("p2");

            Assert.Equal(EnumGameSessionState.WaitingPlayerConfirmation, GS.State);
        }

        [Fact]
        public void With_StateWaitingConfirmation_WhenSetReadyWithAllFleetPositioned_StateChangeToPlaying()
        {
            GS.Init();
            GS.Join("p1");
            GS.Join("p2");

            ///*
            foreach(var p in new Player[] {  GS.P1, GS.P2 })
            {
                p.Board.Set(p.Fleet);
            }
            //*/

            GS.Ready("p1");
            GS.Ready("p2");
                        
            Assert.Equal(EnumGameSessionState.Playing, GS.State);
        }        
    }
}
