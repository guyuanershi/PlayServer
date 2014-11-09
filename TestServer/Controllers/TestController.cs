using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestServer.Models;

namespace TestServer.Controllers
{
    public class TestController : ApiController
    {
        public GoData Play([FromBody] GoData data)
        {
            GoData newGo = new GoData();
            if (Request.Headers.Contains("x-yuan-status"))
            {
                var status = Request.Headers.GetValues("x-yuan-status").FirstOrDefault();
                switch (status)
                {
                    case "Play-Invite":
                        newGo.a_x = 7;
                        newGo.a_y = 7;
                        break;
                    default:
                        break;
                }
            }

            return newGo;
        }
    }
}
