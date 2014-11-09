using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FivePlay;

namespace FivePlayTest
{
    [TestClass]
    public class BoardTest
    {
        private Board board;

        [TestInitialize]
        public void Initialize()
        {
            board = new Board();
        }

        [TestMethod]
        public void Test_GetChesses()
        {
            board.CleanBoard();
            board.Add(1, 1, PlayerType.FirstOne);
            board.Add(2, 2, PlayerType.FirstOne);
            board.Add(3, 3, PlayerType.SecondOne);
            board.Add(4, 4, PlayerType.FirstOne);

            var indexes = board.GetChesses(ChessType.BLACK);
            Assert.AreEqual(indexes.Length, 3);
            Assert.AreEqual(indexes[0], 0);
            Assert.AreEqual(indexes[1], 16);
            Assert.AreEqual(indexes[2], 48);
        }

        [TestMethod]
        public void Test_GetBoardIndex()
        {
            var index = board.GetBoardIndex(2, 3);
            Assert.AreEqual(index, 17);

            index = board.GetBoardIndex(1, 1);
            Assert.AreEqual(index, 0);

            index = board.GetBoardIndex(Board.BoardSize, Board.BoardSize);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfBoardException))]
        public void Test_GetBoardIndex_OutOfBoardException()
        {
            board.GetBoardIndex(0, 0);
        }

        [TestMethod]
        public void Test_GetLoaction()
        {
            var location = board.GetLoaction(17);
            Assert.AreEqual(location[0], 2);
            Assert.AreEqual(location[1], 3);
            Assert.AreEqual(location[2], (int)ChessType.EMPTY);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfBoardException))]
        public void Test_GetLoaction_OutOfBoardException()
        {
            board.GetLoaction(-1);
            board.GetLoaction(Board.BoardSize + 1);
        }

        [TestMethod]
        public void Test_Add()
        {
            var index = board.Add(2, 3, PlayerType.FirstOne);

            var location = board.GetLoaction(index);
            Assert.AreEqual(location[0], 2);
            Assert.AreEqual(location[1], 3);
            Assert.AreEqual(location[2], (int)ChessType.BLACK);

            board.CleanBoard();
        }

        [TestMethod]
        public void Test_CleanBoard()
        {
            var index = board.Add(1, 1, PlayerType.FirstOne);
            board.CleanBoard();

            var location = board.GetLoaction(index);
            Assert.AreEqual(location[2], (int)ChessType.EMPTY);
        }

        [TestMethod]
        public void Test_Horizontal_Win()
        {
            board.CleanBoard();
            board.Add(1, 2, PlayerType.FirstOne);
            board.Add(1, 3, PlayerType.FirstOne);
            board.Add(1, 4, PlayerType.FirstOne);
            board.Add(1, 5, PlayerType.FirstOne);
            board.Add(1, 6, PlayerType.FirstOne);

            var res = board.IsHorizontalWin(1, 2, ChessType.BLACK);
            Assert.AreEqual(res, true);
            res = board.IsHorizontalWin(1, 5, ChessType.BLACK);
            Assert.AreEqual(res, true);

            board.CleanBoard();
            board.Add(1, 1, PlayerType.FirstOne);
            board.Add(1, 2, PlayerType.FirstOne);
            board.Add(1, 3, PlayerType.FirstOne);
            board.Add(1, 4, PlayerType.FirstOne);
            board.Add(1, 6, PlayerType.FirstOne);
            board.Add(1, 7, PlayerType.FirstOne);
            board.Add(1, 8, PlayerType.FirstOne);
            board.Add(1, 9, PlayerType.FirstOne);
            board.Add(1, 10, PlayerType.FirstOne);
            res = board.IsHorizontalWin(1, 4, ChessType.BLACK);
            Assert.AreEqual(res, false);

            board.CleanBoard();
            board.Add(1, 12, PlayerType.FirstOne);
            board.Add(1, 13, PlayerType.FirstOne);
            board.Add(1, 14, PlayerType.FirstOne);
            board.Add(1, 15, PlayerType.FirstOne);
            board.Add(2, 1, PlayerType.FirstOne);
            res = board.IsHorizontalWin(1, 15, ChessType.BLACK);
            Assert.AreEqual(res, false);
        }

        [TestMethod]
        public void Test_Vertical_Win()
        {
            board.CleanBoard();
            board.Add(2, 1, PlayerType.FirstOne);
            board.Add(3, 1, PlayerType.FirstOne);
            board.Add(4, 1, PlayerType.FirstOne);
            board.Add(5, 1, PlayerType.FirstOne);
            board.Add(6, 1, PlayerType.FirstOne);
            var res = board.IsVerticalWin(2, 1, ChessType.BLACK);
            Assert.AreEqual(res, true);

            res = board.IsVerticalWin(4, 1, ChessType.BLACK);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_Diagonal_Front_Win()
        {
            board.CleanBoard();
            board.Add(2, 2, PlayerType.FirstOne);
            board.Add(3, 3, PlayerType.FirstOne);
            board.Add(4, 4, PlayerType.FirstOne);
            board.Add(5, 5, PlayerType.FirstOne);
            board.Add(6, 6, PlayerType.FirstOne);
            var res = board.IsDiagonalFrontWin(2, 2, ChessType.BLACK);
            Assert.AreEqual(res, true);

            res = board.IsDiagonalFrontWin(4, 4, ChessType.BLACK);
            Assert.AreEqual(res, true);

            board.CleanBoard();
            board.Add(4, 4, PlayerType.FirstOne);
            board.Add(5, 5, PlayerType.FirstOne);
            board.Add(6, 6, PlayerType.FirstOne);
            board.Add(8, 8, PlayerType.FirstOne);
            board.Add(9, 9, PlayerType.FirstOne);
            board.Add(10, 10, PlayerType.FirstOne);
            board.Add(11, 11, PlayerType.FirstOne);
            board.Add(12, 12, PlayerType.FirstOne);
            res = board.IsDiagonalFrontWin(8, 8, ChessType.BLACK);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void Test_Diagonal_Back_Win()
        {
            board.CleanBoard();
            board.Add(6, 2, PlayerType.FirstOne);
            board.Add(5, 3, PlayerType.FirstOne);
            board.Add(4, 4, PlayerType.FirstOne);
            board.Add(3, 5, PlayerType.FirstOne);
            board.Add(2, 6, PlayerType.FirstOne);
            var res = board.IsDiagonalBackWin(6, 2, ChessType.BLACK);
            Assert.AreEqual(res, true);
            res = board.IsDiagonalBackWin(2, 6, ChessType.BLACK);
            Assert.AreEqual(res, true);

            board.CleanBoard();
            board.Add(9, 3, PlayerType.FirstOne);
            board.Add(8, 4, PlayerType.FirstOne);
            board.Add(7, 5, PlayerType.FirstOne);
            board.Add(5, 7, PlayerType.FirstOne);
            board.Add(4, 8, PlayerType.FirstOne);
            board.Add(3, 9, PlayerType.FirstOne);
            board.Add(2, 10, PlayerType.FirstOne);
            board.Add(1, 11, PlayerType.FirstOne);
            res = board.IsDiagonalBackWin(5, 7, ChessType.BLACK);
            Assert.AreEqual(res, true);
        }
    }
}
