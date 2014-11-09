using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlayServer.Models
{
    public enum HeaderStatus
    {
        NONE,
        PLAY_INVITE,
        PLAY_FIRST,
        PLAY_NORMAL,
        PLAY_END_WIN,
        PLAY_END_LOSE,
        PLAY_END_TIE,
        PLAY_TIMEOUT_WIN,
        PLAY_TIMEOUT_LOSE
    }

    public class Utils
    {
        public static HeaderStatus GetHeaderStatus(string stats)
        {
            try
            {
                return (HeaderStatus)Enum.Parse(typeof(HeaderStatus), stats, true);
            }
            catch (Exception)
            {
                //Log it   
                return HeaderStatus.NONE;
            }

        }
    }
}