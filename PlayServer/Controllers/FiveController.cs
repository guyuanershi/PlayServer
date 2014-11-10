﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

using PlayServer.Models;
using FivePlay;

namespace PlayServer.Controllers
{
    public class FiveController : ApiController
    {
        private Board BOARD = new Board();

        public List<MoveData> PlayFirst()
        {
            // register first player
            //PlayerManager.RegisterPlayer("http://10.148.204.235/Test1/api/test");
            PlayerManager.RegisterPlayer("localhost");
            // register second player
            PlayerManager.RegisterPlayer("localhost");

            GoData nextMove = PlayerManager.CurrentPlayer.Play("PLAY_INVITE");
            if (nextMove == null ||
                nextMove.b_x != 0 ||
                nextMove.b_y != 0)
            {
                // first player lose
                return BOARD.MoveDatas;
            }

            // first move
            BOARD.Add(nextMove.a_x, nextMove.a_y, PlayerManager.CurrentPlayer.PlayerType);

            GameStatus status = GameStatus.Progressing;
            while (true)
            {
                nextMove = PlayerManager.SwitchPlayer().Play("PLAY_NORMAL", nextMove);

                var points = ParsePoints(nextMove);
                status = BOARD.Add(points.Item1, points.Item2, PlayerManager.CurrentPlayer.PlayerType);
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

            return BOARD.MoveDatas;
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
