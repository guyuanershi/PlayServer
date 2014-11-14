using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using PlayServer.Models;
using FivePlay;

namespace FivePlayTest
{
    [TestClass]
    public class PlayerTest
    {
        RobotPlayer robot_first;

        [TestInitialize]
        public void Initialize()
        {
            robot_first = new RobotPlayer("robot", "robot", FivePlay.PlayerType.FirstOne);
            robot_first.Board = new Board();
        }

        [TestMethod]
        public void Test_TryPlaceAChess()
        {
            var board = robot_first.Board;
            board.CleanBoard();

            #region Horizontal
            List<int> index = new List<int>();
            index.Add(board.Add(5, 5, robot_first.PlayerType));
            index.Add(board.Add(5, 6, robot_first.PlayerType));
            index.Add(board.Add(5, 7, robot_first.PlayerType));
            index.Add(board.Add(5, 8, robot_first.PlayerType));
            index.Add(board.Add(5, 9, robot_first.PlayerType));

            var ls = robot_first.TryPlaceAChess(RobotPlayer.ConnectionType.Horizontal, index);
            var loc = board.GetLoaction(ls[0]);
            Assert.AreEqual(loc[0], 5);
            Assert.AreEqual(loc[1], 4);
            loc = board.GetLoaction(ls[1]);
            Assert.AreEqual(loc[0], 5);
            Assert.AreEqual(loc[1], 10);

            index.Clear();
            index.Add(board.Add(1, 1, robot_first.PlayerType));
            index.Add(board.Add(1, 2, robot_first.PlayerType));

            ls = robot_first.TryPlaceAChess(RobotPlayer.ConnectionType.Horizontal, index);
            Assert.AreEqual(ls.Count, 1);

            loc = board.GetLoaction(ls[0]);
            Assert.AreEqual(loc[0], 1);
            Assert.AreEqual(loc[1], 3);

            #endregion

            #region Vertical
            index.Clear();
            index.Add(board.Add(5, 5, robot_first.PlayerType));
            index.Add(board.Add(6, 5, robot_first.PlayerType));
            index.Add(board.Add(7, 5, robot_first.PlayerType));
            index.Add(board.Add(8, 5, robot_first.PlayerType));
            index.Add(board.Add(9, 5, robot_first.PlayerType));

            ls = robot_first.TryPlaceAChess(RobotPlayer.ConnectionType.Vertical, index);
            loc = board.GetLoaction(ls[0]);
            Assert.AreEqual(loc[0], 4);
            Assert.AreEqual(loc[1], 5);
            loc = board.GetLoaction(ls[1]);
            Assert.AreEqual(loc[0], 10);
            Assert.AreEqual(loc[1], 5);
            #endregion

            #region Diagonal Front
            index.Clear();
            index.Add(board.Add(5, 5, robot_first.PlayerType));
            index.Add(board.Add(6, 6, robot_first.PlayerType));
            index.Add(board.Add(7, 7, robot_first.PlayerType));
            index.Add(board.Add(8, 8, robot_first.PlayerType));
            index.Add(board.Add(9, 9, robot_first.PlayerType));

            ls = robot_first.TryPlaceAChess(RobotPlayer.ConnectionType.DiagonalFront, index);
            loc = board.GetLoaction(ls[0]);
            Assert.AreEqual(loc[0], 4);
            Assert.AreEqual(loc[1], 4);
            loc = board.GetLoaction(ls[1]);
            Assert.AreEqual(loc[0], 10);
            Assert.AreEqual(loc[1], 10);
            #endregion

            #region Diagonal Back
            index.Clear();
            index.Add(board.Add(3, 7, robot_first.PlayerType));
            index.Add(board.Add(4, 6, robot_first.PlayerType));
            index.Add(board.Add(5, 5, robot_first.PlayerType));
            index.Add(board.Add(6, 4, robot_first.PlayerType));
            index.Add(board.Add(7, 3, robot_first.PlayerType));

            ls = robot_first.TryPlaceAChess(RobotPlayer.ConnectionType.DiagonalBack, index);
            loc = board.GetLoaction(ls[0]);
            Assert.AreEqual(loc[0], 2);
            Assert.AreEqual(loc[1], 8);
            loc = board.GetLoaction(ls[1]);
            Assert.AreEqual(loc[0], 8);
            Assert.AreEqual(loc[1], 2);
            #endregion

        }

        [TestMethod]
        public void Test_FindConnectedChesses()
        {
            var board = robot_first.Board;
            board.CleanBoard();

            List<int> index = new List<int>();
            index.Add(board.Add(5, 3, robot_first.PlayerType));
            index.Add(board.Add(5, 4, robot_first.PlayerType));
            index.Add(board.Add(5, 5, robot_first.PlayerType));
            index.Add(board.Add(5, 6, robot_first.PlayerType));
            index.Add(board.Add(5, 7, robot_first.PlayerType));

            index.Add(board.Add(3, 5, robot_first.PlayerType));
            index.Add(board.Add(4, 5, robot_first.PlayerType));
            index.Add(board.GetBoardIndex(5, 5));
            index.Add(board.Add(6, 5, robot_first.PlayerType));
            index.Add(board.Add(7, 5, robot_first.PlayerType));

            index.Add(board.Add(3, 3, robot_first.PlayerType));
            index.Add(board.Add(4, 4, robot_first.PlayerType));
            index.Add(board.GetBoardIndex(5, 5));
            index.Add(board.Add(6, 6, robot_first.PlayerType));
            index.Add(board.Add(7, 7, robot_first.PlayerType));

            index.Add(board.Add(3, 7, robot_first.PlayerType));
            index.Add(board.Add(4, 6, robot_first.PlayerType));
            index.Add(board.GetBoardIndex(5, 5));
            index.Add(board.Add(6, 4, robot_first.PlayerType));
            index.Add(board.Add(7, 3, robot_first.PlayerType));

            // all
            var res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[]{ board.GetBoardIndex(5, 5)});
            Assert.AreEqual(res.Count, 4);
            CheckValue(res, index);

            // only horizontal
            res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[] { board.GetBoardIndex(5, 3) });
            Assert.AreEqual(res.Count, 3);
            //CheckValue(res, index);

            res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[] { board.GetBoardIndex(3, 5) });
            Assert.AreEqual(res.Count, 3);
            //CheckValue(res, index);

            res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[] { board.GetBoardIndex(3, 3) });
            Assert.AreEqual(res.Count, 1);
            //CheckValue(res, index);

            // only diagonal_back
            res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[] { board.GetBoardIndex(3, 7) });
            Assert.AreEqual(res.Count, 1);
            //CheckValue(res, index);

            // (4,4)
            res = robot_first.FindConnectedChesses(ChessType.BLACK, new int[] { board.GetBoardIndex(4, 4) });
            Assert.AreEqual(res.Count, 4);
            //CheckValue(res, index);
        }

        private void CheckValue(List<KeyValuePair<RobotPlayer.ConnectionType, List<int>>> data, List<int> index)
        {
            foreach (var item in data)
            {
                var startIndex = 0;
                if (item.Key == RobotPlayer.ConnectionType.Horizontal)
                {
                    startIndex = 0;
                }
                else if (item.Key == RobotPlayer.ConnectionType.Vertical)
                {
                    startIndex = 5;
                }
                else if (item.Key == RobotPlayer.ConnectionType.DiagonalFront)
                {
                    startIndex = 10;
                }
                else if (item.Key == RobotPlayer.ConnectionType.DiagonalBack)
                {
                    startIndex = 15;
                }

                //Assert.AreEqual(item.Value.Count, 5);
                for (int j = 0, i = startIndex; j < 5 && i < startIndex + 5; i++, j++)
                {
                    Assert.AreEqual(item.Value[j], index[i]);
                }
            }
        }
    }
}
