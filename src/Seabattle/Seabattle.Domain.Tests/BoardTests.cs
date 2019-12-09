using Seabattle.Domain.Ships;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Seabattle.Domain.Tests
{
    public class BoardTests
    {
        const int GRID_WIDTH = 10;

        public Board Board { get; set; }

        public BoardTests()
        {
            Board = new Board(GRID_WIDTH);
        }

        [Fact]
        public void With_ShipWithoutID_WhenCreate_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var s1 = new Ship(null, 2, EnumShipOrientation.Vertical);
            });
        }

        [Fact]
        public void With_ValidShipCoordinates_WhenCallPosition_FleetUpdateOK()
        {
            var s1 = new Ship("S1",2, EnumShipOrientation.Vertical);

            Board.Set(s1, new Coordinates { X = 0, Y = 0 });

            Assert.True(Board.Fleet.Count() == 1);
        }

        [Fact]
        public void With_NegativeBoardWidth_WhenCreateBoard_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Board(-1);
            });
        }

        [Fact]
        public void With_ZeroBoardWidth_WhenCreateBoard_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Board(0);
            });
        }

        [Fact]
        public void With_ValidBoardWidth_WhenCheckWidthProp_ReturnsExpectedWidth()
        {
            Assert.Equal(GRID_WIDTH, Board.Width);
        }

        [Fact]
        public void With_ShipCoordOffLimitsY_WhenCallPosition_ThrowsException()
        {
            var s1 = new Ship("s1", Board.Width, EnumShipOrientation.Vertical);

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Set(s1, new Coordinates { X = 0, Y = 1 });
            });
        }

        [Fact]
        public void With_ShipCoordOffLimitsX_WhenCallPosition_ThrowsException()
        {
            var s1 = new Ship("s1", Board.Width, EnumShipOrientation.Horizontal);

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Set(s1, new Coordinates { X = 1, Y = 0 });
            });
        }

        [Fact]
        public void With_ShipCollide_WhenCallPosition_ThrowsException()
        {
            var s1 = new Ship("S1", 5, EnumShipOrientation.Vertical);
            var s2 = new Ship("S2", 5, EnumShipOrientation.Horizontal);

            Board.Set(s1, new Coordinates { X = 5, Y = 0 });

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Set(s2, new Coordinates { X = 4, Y = 2 });
            });
        }

        [Fact]
        public void With_SameShipAlreadyAdded_WhenCallPosition_FleetDoesNotChange()
        {
            var s1 = new Ship("S1", 5, EnumShipOrientation.Vertical);
            
            Board.Set(s1, new Coordinates { X = 5, Y = 0 });

            Assert.True(Board.Fleet.Count() == 1);

            Board.Set(s1, new Coordinates { X = 7, Y = 2 });

            var shipFromBoard = Board.Get(new Coordinates { X = 7, Y = 2 });

            Assert.NotNull(shipFromBoard);
            Assert.Equal(s1, shipFromBoard);
        }

        [Fact]
        public void With_NoShipAdded_WhenCallGet_ReturnsNull()
        {
            var shipFromBoard = Board.Get(0, 0);
            Assert.Null(shipFromBoard);
        }
    }
}
