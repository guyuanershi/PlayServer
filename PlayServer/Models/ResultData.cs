using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FivePlay;

namespace PlayServer.Models
{
    public class ResultData
    {
        public string winner { get; set; }
        public List<MoveData> Data { get; set; }
    }
}