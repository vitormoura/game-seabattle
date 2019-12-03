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
        public void With_ValidShipCoordinates_WhenCallPosition_FleetUpdateOK()
        {
            var s1 = new Ship
            {
                 Orientation = EnumShipOrientation.Vertical,
                 Size = 2
            };

            Board.Position(s1, new Coordinates { X = 0, Y = 0 });

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
            var s1 = new Ship { Orientation = EnumShipOrientation.Vertical, Size = Board.Width };

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Position(s1, new Coordinates { X = 0, Y = 1 });
            });
        }

        [Fact]
        public void With_ShipCoordOffLimitsX_WhenCallPosition_ThrowsException()
        {
            var s1 = new Ship { Orientation = EnumShipOrientation.Horizontal, Size = Board.Width };

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Position(s1, new Coordinates { X = 1, Y = 0 });
            });
        }

        [Fact]
        public void With_ShipCollide_WhenCallPosition_ThrowsException()
        {
            var s1 = new Ship { Orientation = EnumShipOrientation.Horizontal, Size = 5 };
            var s2 = new Ship { Orientation = EnumShipOrientation.Vertical, Size = 3 };

            Board.Position(s1, new Coordinates { X = 0, Y = 5 });

            Assert.Throws<ArgumentException>(() =>
            {
                Board.Position(s2, new Coordinates { X = 1, Y = 3 });
            });
        }
    }
}
