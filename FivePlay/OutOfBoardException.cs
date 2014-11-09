using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FivePlay
{
    public class OutOfBoardException: Exception
    {
        public override string Message
        {
            get
            {
                return "Location is out of board!";
            }
        }
    }
}
