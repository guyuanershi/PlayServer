using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivePlay
{
    /// <summary>
    /// return true means break the rule
    /// </summary>
    public interface IRule
    {
        bool Run(Board board, Point p1, Point? p2);
    }

    public class Rule : IRule
    {
        public string Name { get; set; }
        public virtual bool Run(Board board, Point p1, Point? p2)
        {
            throw new NotImplementedException();
        }
    }

    public class LocationNotEmptyRule : Rule
    {
        public override bool Run(Board board, Point p1, Point? p2)
        {
            var status1 = board.GetLocationStatus(p1.X, p1.Y);

            if (!p2.HasValue)
            {
                return (status1 != ChessType.EMPTY);
            }

            if (p1.X == p2.Value.X && p1.Y == p2.Value.Y) return false;

            
            var status2 = board.GetLocationStatus(p2.Value.X, p2.Value.Y);
            return (status1 != ChessType.EMPTY) && (status2 != ChessType.EMPTY);
        }
    }

    public class LocationArroundFirstPlaceRule : Rule
    {
        public override bool Run(Board board, Point p1, Point? p2)
        {
            if (!p2.HasValue)
            {
                return false;
            }

            var indexes = GetImpaceIndexes(board, p1);
            var p2_index = board.GetBoardIndex(p2.Value.X, p2.Value.Y);

            // i - 1: should let start form 1 not 0, i did minus 1 at GetImpaceIndexes
            return indexes.Any(i => i - 1 == p2_index);
        }

        private int[] GetImpaceIndexes(Board board, Point p)
        {
            var m = p.Y + p.Y * Board.BoardSize;
            var s = Board.BoardSize * 2;
            var w = Board.BoardSize;

            List<int> indexes = new List<int>();
            // (x - 1, y - 1)
            if (!board.IsOutOfBoard(m - s - 1))
                indexes.Add(m - s - 1);

            // (x - 1, y)
            if (!board.IsOutOfBoard(m - s))
                indexes.Add(m - s);

            // (x - 1, y + 1)
            if (!board.IsOutOfBoard(m - s + 1))
                indexes.Add(m - s + 1);

            //(x, y - 1)
            if (!board.IsOutOfBoard(m - w - 1))
                indexes.Add(m - w - 1);

            // (x, y)
            if (!board.IsOutOfBoard(m - w))
                indexes.Add(m - w);

            // (x, y + 1)
            if (!board.IsOutOfBoard(m - w + 1))
                indexes.Add(m - w + 1);

            // (x + 1, y - 1)
            if (!board.IsOutOfBoard(m - 1))
                indexes.Add(m - 1);

            // (x + 1, y)
            if (!board.IsOutOfBoard(m))
                indexes.Add(m);

            // (x + 1, y + 1)
            if (!board.IsOutOfBoard(m + 1))
                indexes.Add(m + 1);

            return indexes.ToArray();
        }
    }

    public class Rules
    {
        private static List<Rule> rules = new List<Rule>();

        public static void Prepare()
        {
            rules.Clear();

            RegisterRule(new LocationNotEmptyRule());
            RegisterRule(new LocationArroundFirstPlaceRule());
        }

        public static bool CheckRules(Board board, Point p1, Point? p2)
        {
            return rules.Any(r => r.Run(board, p1, p2));
        }

        public static void RegisterRule(Rule rule)
        {
            if (!rules.Contains(rule, new RuleCompare()))
                rules.Add(rule);
        }

        public static void Clear()
        {
            rules.Clear();
        }

        private class RuleCompare: IEqualityComparer<Rule>
        {

            public bool Equals(Rule x, Rule y)
            {
                if (x == null || y == null) return false;

                return x.Name == y.Name;
            }

            public int GetHashCode(Rule obj)
            {
                return obj.Name.GetHashCode();
            }
        }

    }
}
