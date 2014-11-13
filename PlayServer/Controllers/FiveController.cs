using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

using PlayServer.Models;
using FivePlay;

namespace PlayServer.Controllers
{
    public class FiveController : Controller
    {
        private Board BOARD = new Board();
        private PlayerManager playerManager = new PlayerManager();
        private PlayServerContext db = new PlayServerContext();

        public ActionResult Index(string PID1, string PID2)
        {
            ResultData data = new ResultData();
            data.winner = "";
            data.Data = new List<MoveData>();
            
            var players = db.PlayerDBs.ToArray();
            ViewData["players"] = new SelectList(players, "ID", "Name");

            if (Request.IsAjaxRequest())
            {
                if (!string.IsNullOrEmpty(PID1) && !string.IsNullOrEmpty(PID2))
                {
                    var p1 = players.First(p => p.ID == int.Parse(PID1));
                    var p2 = players.First(p => p.ID == int.Parse(PID2));
                    data = Run(p1, p2);
                }
                return PartialView("_Moves", data);
            }

            return View(data);
        }

        private ResultData Run(PlayerDB p1, PlayerDB p2)
        {
            ResultData result = new ResultData();

            // register first player
            //PlayerManager.RegisterPlayer("http://10.148.204.235/Test1/api/test");
            playerManager.RegisterPlayer(p1.Name, p1.URL);
            // register second player
            playerManager.RegisterPlayer(p2.Name, p2.URL);

            GoData nextMove = playerManager.CurrentPlayer.Play("PLAY_INVITE");
            if (nextMove == null ||
                nextMove.b_x != 0 ||
                nextMove.b_y != 0)
            {
                result.Data = BOARD.MoveDatas;
                result.winner = playerManager.SwitchPlayer().Name;
                // first player lose
                return result;//Json(result, JsonRequestBehavior.AllowGet);
            }

            // first move
            var points = ParsePoints(nextMove);
            BOARD.Add(points.Item1, points.Item2, playerManager.CurrentPlayer.PlayerType);

            GameStatus status = GameStatus.Progressing;
            while (true)
            {
                nextMove = playerManager.SwitchPlayer().Play("PLAY_NORMAL", nextMove);

                points = ParsePoints(nextMove);
                status = BOARD.Add(points.Item1, points.Item2, playerManager.CurrentPlayer.PlayerType);
                if (status == GameStatus.Break_Rules ||
                    status == GameStatus.Win ||
                    status == GameStatus.Tie)
                {
                    break;
                }
            }

            // current player lose the game
            //if (status == GameStatus.Break_Rules)
            //{
            //    PlayerManager.CurrentPlayer.End("PLAY_END_LOSE");
            //    PlayerManager.SwitchPlayer().End("PLAY_END_WIN");
            //}
            //else if (status == GameStatus.Win)
            //{
            //    PlayerManager.CurrentPlayer.End("PLAY_END_WIN");
            //    PlayerManager.SwitchPlayer().End("PLAY_END_LOSE");
            //}
            //else if (status == GameStatus.Tie)
            //{
            //    PlayerManager.CurrentPlayer.End("PLAY_END_TIE");
            //    PlayerManager.SwitchPlayer().End("PLAY_END_TIE");
            //}

            result.Data = BOARD.MoveDatas;
            result.winner = playerManager.SwitchPlayer().Name;
            return result;//Json(result, JsonRequestBehavior.AllowGet);
        }

        private Tuple<Point, Point?> ParsePoints(GoData data)
        {
            Point p1;
            p1.X = data.a_x;
            p1.Y = data.a_y;

            Point? p2 = null;
            if (data.b_x != 0 && data.b_y != 0)
            {
                Point _p2;
                _p2.X = data.b_x;
                _p2.Y = data.b_y;
                p2 = _p2;
            }
            return new Tuple<Point, Point?>(p1, p2);
        }

    }
}
