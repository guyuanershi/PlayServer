using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;

using FivePlay;
using System.Net;

namespace PlayServer.Models
{
    public abstract class Player
    {
        protected static JavaScriptSerializer JS = new JavaScriptSerializer();

        public PlayerType PlayerType { get; set; }

        public String URL { get; set; }

        public String Name { get; set; }

        public abstract GoData Play(string status, GoData preMove = null);

        public virtual void End(string status)
        {
            var req = GetRequest(status);
            req.Method = "GET";
            req.GetResponse();
        }

        private HttpWebRequest _request;
        protected HttpWebRequest GetRequest(string status)
        {
            if (_request == null)
            {
                _request = (HttpWebRequest)WebRequest.Create(this.URL);
                _request.Method = "POST";
                _request.ContentLength = 0;
                _request.ContentType = "application/json";
            }
            var values = _request.Headers.GetValues("x-yuan-status");
            if (values.Count() == 0)
                _request.Headers.Add("x-yuan-status", status);
            else
                _request.Headers.Set("x-yuan-status", status);
            return _request;
        }
    }


    public class RealPlayer : Player
    {
        public RealPlayer(string name, string url, PlayerType playerType)
        {
            this.URL = url;
            this.Name = name;
            this.PlayerType = playerType;
        }

        public override GoData Play(string status, GoData preMove)
        {
            var req = GetRequest(status);
            var nextData = GetNextMove(req);
            return nextData;
        }

        private GoData GetNextMove(HttpWebRequest req)
        {
            GoData nextData = new GoData();
            var resp = req.GetResponse() as HttpWebResponse;
            if (resp != null)
            {
                using (var stream = resp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var d1 = reader.ReadToEnd();
                    nextData = JS.Deserialize(d1, typeof(GoData)) as GoData;
                }
            }

            return nextData;
        }
    }

    public class RobotPlayer : Player
    {
        public enum ConnectionType : uint
        {
            Horizontal = 0,
            Vertical,
            DiagonalFront,
            DiagonalBack
        }

        public RobotPlayer(string name, string url, PlayerType playerType)
        {
            this.URL = url;
            this.Name = name;
            this.PlayerType = playerType;
        }

        public override GoData Play(string status, GoData preMove)
        {
            GoData nextMove = new GoData();
            var headerStatus = Utils.GetHeaderStatus(status);
            if (headerStatus == HeaderStatus.PLAY_INVITE)
            {
                nextMove = new GoData();
                nextMove.a_x = Board.BoardSize / 2;
                nextMove.a_y = nextMove.a_x;
            }
            else if (headerStatus == HeaderStatus.PLAY_NORMAL)
            {
                nextMove = GetNextMove(preMove);
            }
            return nextMove;
        }

        private GoData GetNextMove(GoData preMove)
        {
            // just test codes
            var nextmove = new GoData();
            //nextmove.a_x = preMove.a_x - 1;
            //nextmove.a_y = preMove.a_y - 1;
            //nextmove.b_x = preMove.b_x + 1;
            //nextmove.b_y = preMove.b_y + 1;

            // get my moves
            var myChessType = GetChessType(PlayerType);
            var oppChessType = GetOpponentChessType(PlayerType);
            var myMoves = Board.GetChesses(myChessType);

            // get the opponent's moves
            var oppMoves = Board.GetChesses(oppChessType);

            // get empty places
            var emptyPlcs = Board.GetChesses(ChessType.EMPTY);

            // find opponent's move whether have 3 chesses connect together
            List<int> indexes;
            Dictionary<ConnectionType, List<int>> result = new Dictionary<ConnectionType, List<int>>();
            foreach (var index in oppMoves)
            {
                var loc = Board.GetLoaction(index);
                var x = loc[0];
                var y = loc[1];
                if (Board.IsHorizontalWin(x, y, oppChessType, out indexes, 3))
                {
                    if (!DuplicatedIndex(index, result))
                    {
                        result.Add(ConnectionType.Horizontal, indexes);
                    }
                }
                else if (Board.IsVerticalWin(x, y, oppChessType, out indexes, 3))
                {
                    if (!DuplicatedIndex(index, result))
                    {
                        result.Add(ConnectionType.Vertical, indexes);
                    }
                }
                else if (Board.IsDiagonalFrontWin(x, y, oppChessType, out indexes, 3))
                {
                    if (!DuplicatedIndex(index, result))
                    {
                        result.Add(ConnectionType.DiagonalFront, indexes);
                    }
                }
                else if (Board.IsDiagonalBackWin(x, y, oppChessType, out indexes, 3))
                {
                    if (!DuplicatedIndex(index, result))
                    {
                        result.Add(ConnectionType.DiagonalBack, indexes);
                    }
                }

                // only need 2 connections enough
                if (result.Count >= 2)
                    break;
            }

            var allPossibleIndexes = new List<int>();
            result.ToList().ForEach(c => TryPlaceAChess(c.Key, c.Value.ToArray(), allPossibleIndexes));
            
            // if the result have two, then one move to block each other
            if (allPossibleIndexes.Count() == 4)
            {
                allPossibleIndexes = allPossibleIndexes.Intersect(emptyPlcs).ToList();
            }
            // if the result have one, then one move to block it, the other try to find a way to win
            else if (result.Count == 2)
            {
                allPossibleIndexes = allPossibleIndexes.Intersect(emptyPlcs).ToList();
            }
            // if the result is empty, then try to find a way make win
            else
            {

            }

            if (allPossibleIndexes.Count < 2)
            {
                for (int i = 0; i <= 2 - allPossibleIndexes.Count; i++)
                {
                    var l = emptyPlcs.Except(allPossibleIndexes).ToList();
                    allPossibleIndexes.Add(emptyPlcs[(new Random()).Next(l.Count)]);
                }
            }

            var r1 = Board.GetLoaction(allPossibleIndexes[0]);
            var r2 = Board.GetLoaction(allPossibleIndexes[1]);
            nextmove.a_x = r1[0];
            nextmove.a_y = r1[1];
            nextmove.b_x = r2[0];
            nextmove.b_y = r2[1];

            return nextmove;
        }

        private void TryPlaceAChess(ConnectionType conntype, int[] indexes, List<int> allPossibleIndexes)
        {
            var first_loc = Board.GetLoaction(indexes.First());
            var last_loc = Board.GetLoaction(indexes.Last());

            if (conntype == ConnectionType.Horizontal)
            {
                // o x x x x
                GatherPossibleIndex(first_loc[0], first_loc[1] - 1, allPossibleIndexes);
                // x x x x o
                GatherPossibleIndex(last_loc[0], last_loc[1] + 1, allPossibleIndexes);
            }
            else if (conntype == ConnectionType.Vertical)
            {
                GatherPossibleIndex(first_loc[0] - 1, first_loc[1], allPossibleIndexes);
                GatherPossibleIndex(last_loc[0] + 1, last_loc[1], allPossibleIndexes);
            }
            else if (conntype == ConnectionType.DiagonalFront)
            {
                GatherPossibleIndex(first_loc[0] - 1, first_loc[1] - 1, allPossibleIndexes);
                GatherPossibleIndex(last_loc[0] + 1, last_loc[1] + 1, allPossibleIndexes);
            }
            else if (conntype == ConnectionType.DiagonalBack)
            {
                GatherPossibleIndex(first_loc[0] + 1, first_loc[1] - 1, allPossibleIndexes);
                GatherPossibleIndex(last_loc[0] - 1, last_loc[1] + 1, allPossibleIndexes);
            }
        }

        private void GatherPossibleIndex(int x, int y, List<int> ls)
        {
            if (!Board.IsOutOfBoard(x, y))
                ls.Add(Board.GetBoardIndex(x, y));
        }


        private bool DuplicatedIndex(int index, Dictionary<ConnectionType, List<int>> result)
        {
            foreach (var ls in result.Values)
            {
                var pt = Board.GetLoaction(index);
                if (ls.Contains(index))
                    return true;
               
            }
            return false;
        }

        private ChessType GetChessType(PlayerType pt)
        {
            return (pt == FivePlay.PlayerType.FirstOne) ? ChessType.BLACK : ChessType.WHITE;
        }

        private ChessType GetOpponentChessType(PlayerType pt)
        {
            return (pt == FivePlay.PlayerType.FirstOne) ? ChessType.WHITE : ChessType.BLACK;
        }

        public Board Board { get;  set; }

    }

    public class PlayerManager
    {
        private Player _firstPlayer;
        private Player _secondPlayer;

        public void RegisterPlayer(string name, string url)
        {
            if (_firstPlayer == null)
            {
                _firstPlayer = CreatePlayer(name, url, PlayerType.FirstOne);
                // initial current player
                CurrentPlayer = _firstPlayer;
            }
            else if (_secondPlayer == null)
            {
                _secondPlayer = CreatePlayer(name, url, PlayerType.SecondOne);
            }
        }

        private Player CreatePlayer(string name, string url, PlayerType pt)
        {
            if (url.Contains("http://"))
                return new RealPlayer(name, url, pt);
            else
                return new RobotPlayer(name, url, pt);
        }

        public Player CurrentPlayer { get; set; }

        public Player SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer.PlayerType == PlayerType.FirstOne) ? _secondPlayer : _firstPlayer;
            return CurrentPlayer;
        }

        public void SetBoardToRobotPlayer(Board board)
        {
            var robotPlayer = _firstPlayer as RobotPlayer;
            if (robotPlayer != null)
            {
                robotPlayer.Board = board;
            }

            robotPlayer = _secondPlayer as RobotPlayer;
            if (robotPlayer != null)
            {
                robotPlayer.Board = board;
            }

        }
    }
}