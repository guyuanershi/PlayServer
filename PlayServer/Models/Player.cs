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
        public RealPlayer(string url, PlayerType playerType)
        {
            this.URL = url;
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
        public RobotPlayer(string url, PlayerType playerType)
        {
            this.URL = url;
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
            nextmove.a_x = preMove.a_x - 1;
            nextmove.a_y = preMove.a_y - 1;
            nextmove.b_x = preMove.b_x + 1;
            nextmove.b_y = preMove.b_y + 1;
            return nextmove;
        }
    }

    public static class PlayerManager
    {
        public static Player FirstPlayer { get; set; }
        public static Player SecondPlayer { get; set; }

        public static void RegisterPlayer(string url)
        {
            if (FirstPlayer == null)
            {
                FirstPlayer = CreatePlayer(url, PlayerType.FirstOne);
                // initial current player
                CurrentPlayer = FirstPlayer;
            }
            else if (SecondPlayer == null)
            {
                SecondPlayer = CreatePlayer(url, PlayerType.SecondOne);
            }
        }

        private static Player CreatePlayer(string url, PlayerType pt)
        {
            if (url.Contains("http://"))
                return new RealPlayer(url, pt);
            else
                return new RobotPlayer(url, pt);
        }

        public static Player CurrentPlayer { get; set; }

        public static Player SwitchPlayer()
        {
            CurrentPlayer = (CurrentPlayer.PlayerType == PlayerType.FirstOne) ? SecondPlayer : FirstPlayer;
            return CurrentPlayer;
        }
    }
}