using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivePlay
{
    public enum ChessType : uint
    {
        EMPTY = 0,
        BLACK,
        WHITE
    }

    public enum PlayerType
    {
        FirstOne = 0,
        SecondOne
    }

    public enum GameStatus
    {
        Progressing = 0,
        Win,
        Tie,
        Break_Rules
    }

    public struct Point
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// board is start from (1,1)
    /// </summary>
    public class Board
    {
        public static int BoardSize = 15;
        // normal board (x,y)
        private List<ChessType> board = new List<ChessType>();

        public Board()
        {
            for (int i = 0; i < BoardSize * BoardSize; i++)
            {
                board.Add(ChessType.EMPTY);
            }

            Rules.Clear();
            Rules.Prepare();
        }

        /// <summary>
        /// get chesses by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int[] GetChesses(ChessType ct)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < board.Count; i++)
            {
                if(board[i] == ct)
                {
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }

        /// <summary>
        /// get input loaction map in board
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetBoardIndex(int x, int y)
        {
            if (x <= 0 || y <= 0 || x > BoardSize || y > BoardSize)
                throw new OutOfBoardException();

            return (y + (x - 1) * BoardSize) - 1;
        }

        /// <summary>
        /// get the location by board index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// 0: x
        /// 1: y
        /// 2: chess type
        /// </returns>
        public int[] GetLoaction(int index)
        {
            if (index > board.Count() || index < 0) throw new OutOfBoardException();
            var x = (index + 1) / BoardSize + 1;
            var y = (index + 1) % BoardSize;
            var c = board[index];
            return new int[] { x, y, (int)c };
        }

        /// <summary>
        /// get current index location status
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ChessType GetLocationStatus(int index)
        {
            if (index > board.Count() || index < 0) throw new OutOfBoardException();
            return board[index];
        }

        /// <summary>
        /// get status by (x, y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public ChessType GetLocationStatus(int x, int y)
        {
            return GetLocationStatus(GetBoardIndex(x, y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public GameStatus Add(Point p1, Point? p2, PlayerType player)
        {
            var status = GameStatus.Progressing;
            if (Rules.CheckRules(this, p1, p2))
                return GameStatus.Break_Rules;

            Add(p1.X, p1.Y, player);

            // check win-lose
            status = CheckWinLoseTie(p1.X, p1.Y, player);
            if (status != GameStatus.Progressing)
                return status;

            if (p2.HasValue)
            {
                Add(p2.Value.X, p2.Value.Y, player);
                
                // check win-lose
                status = CheckWinLoseTie(p2.Value.X,p2.Value.Y, player);
                if (status != GameStatus.Progressing)
                    return status;
            }

            return status;
        }

        public GameStatus CheckWinLoseTie(int x, int y, PlayerType playerType)
        {
            GameStatus status = GameStatus.Progressing;

            var ct = GetCurrentChess(playerType);
            var chesses = GetChesses(ct);

            // the board is full, set the game as tie first
            //
            var emptyPlaces = GetChesses(ChessType.EMPTY);
            if (emptyPlaces.Count() == 0)
                status = GameStatus.Tie;
            
            // find wehter there are five chessed connected together
            // 
            if (IsHorizontalWin(x, y, ct)       ||
                IsVerticalWin(x, y, ct)         ||
                IsDiagonalFrontWin(x, y, ct)    ||
                IsDiagonalBackWin(x, y, ct))
            {
                return GameStatus.Win;
            }

            return status;
        }

        /// <summary>
        /// * * * * *
        /// </summary>
        /// <returns></returns>
        public bool IsHorizontalWin(int x, int y, ChessType ct)
        {
            List<int> indexes = new List<int>();
            for (int i = y - 4; i <= y + 4; i++)
            {
                if (!IsOutOfBoard(x, i))
                {
                    var index = GetBoardIndex(x, i);

                    if (board[index] == ct)
                    {
                        indexes.Add(index);
                    }
                    else
                    {
                        if (indexes.Count < 5)
                        {
                            indexes.Clear();
                        }
                    }
                }
            }
            return indexes.Count() >= 5;
        }

        /// <summary>
        /// *
        /// *
        /// *
        /// *
        /// *
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public bool IsVerticalWin(int x, int y, ChessType ct)
        {
            List<int> indexes = new List<int>();
            for (int i = x - 4; i <= x + 4; i++)
            {
                if (!IsOutOfBoard(i, y))
                {
                    var index = GetBoardIndex(i, y);

                    if (board[index] == ct)
                    {
                        indexes.Add(index);
                    }
                    else
                    {
                        if (indexes.Count < 5)
                            indexes.Clear();
                    }
                }
            }
            return indexes.Count >= 5;
        }

        public bool IsDiagonalFrontWin(int x, int y, ChessType ct)
        {
            List<int> indexes = new List<int>();
            for (int i = x - 4, j = y - 4; i <= x + 4; i++, j++)
            {
                if (!IsOutOfBoard(i, j))
                {
                    var index = GetBoardIndex(i, j);

                    if (board[index] == ct)
                    {
                        indexes.Add(index);
                    }
                    else
                    {
                        if (indexes.Count < 5)
                            indexes.Clear();
                    }
                }
            }
            return indexes.Count >= 5;
        }

        public bool IsDiagonalBackWin(int x, int y, ChessType ct)
        {
            List<int> indexes = new List<int>();
            for (int i = x - 4, j = y + 4; i <= x + 4; i++, j--)
            {
                if (!IsOutOfBoard(i, j))
                {
                    var index = GetBoardIndex(i, j);

                    if (board[index] == ct)
                    {
                        indexes.Add(index);
                    }
                    else
                    {
                        if (indexes.Count < 5)
                            indexes.Clear();
                    }
                }
            }
            return indexes.Count >= 5;
        }

        /// <summary>
        /// add a chess into board
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="player">first or second</param>
        /// <returns>index in board</returns>
        public int Add(int x, int y, PlayerType player)
        {
            var index = GetBoardIndex(x, y);
            board[index] = GetCurrentChess(player);
            return index;
        }

        private ChessType GetCurrentChess(PlayerType playerType)
        {
            return (playerType == PlayerType.FirstOne) ? ChessType.BLACK : ChessType.WHITE;
        }

        /// <summary>
        /// clean board
        /// </summary>
        public void CleanBoard()
        {
            for (int i = 0; i < board.Count(); i++)
            {
                board[i] = ChessType.EMPTY;
            }
        }

        public bool IsOutOfBoard(int index)
        {
            return index < 0 || index > board.Count();
        }

        private bool IsOutOfBoard(int x, int y)
        {
            return (x <= 0 || y <= 0 || x > BoardSize || y > BoardSize);
        }

    }
}
