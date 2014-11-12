using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

using PlayServer.Models;

namespace PlayServer.Controllers
{
    public class DataController : ApiController
    {
        private PlayServerContext db = new PlayServerContext();

        public JsonResult<PlayerDB[]> GetPlayerDB()
        {
            return Json(db.PlayerDBs.ToArray());
        }
    }
}
