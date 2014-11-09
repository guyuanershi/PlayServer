using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FivePlay;

namespace FivePlayTest
{
    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void Test_Not_Empty()
        {
            Board board = new Board();
            Point p1, p2;
            p1.X = 5;
            p1.Y = 5;
            p2.X = 4;
            p2.Y = 5;
            
            Rules.Clear();
            Rules.RegisterRule(new LocationNotEmptyRule());
            var res = Rules.CheckRules(board, p1, p2);

            Assert.AreEqual(res, false);

            res = Rules.CheckRules(board, p1, null);
            Assert.AreEqual(res, false);

            board.Add(5, 5, PlayerType.FirstOne);
            res = Rules.CheckRules(board, p1, null);
            Assert.AreEqual(res, true);
        }


        [TestMethod]
        public void Test_Arround_First_Point()
        {
            Board board = new Board();
            Point p1, p2;
            p1.X = 5;
            p1.Y = 5;
            p2.X = 4;
            p2.Y = 5;

            Rules.Clear();
            Rules.RegisterRule(new LocationArroundFirstPlaceRule());
            var res = Rules.CheckRules(board, p1, p2);
            Assert.AreEqual(res, true);

            p2.X = 10;
            p2.Y = 10;
            res = Rules.CheckRules(board, p1, p2);
            Assert.AreEqual(res, false);

            p2.X = 4;
            p2.Y = 4;
            res = Rules.CheckRules(board, p1, p2);
            Assert.AreEqual(res, true);

            p2.X = 6;
            p2.Y = 6;
            res = Rules.CheckRules(board, p1, p2);
            Assert.AreEqual(res, true);

            p2.X = 7;
            p2.Y = 7;
            res = Rules.CheckRules(board, p1, p2);
            Assert.AreEqual(res, false);
        }
    }
}
