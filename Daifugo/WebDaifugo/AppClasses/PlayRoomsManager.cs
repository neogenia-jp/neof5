using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDaifugo.Models;

namespace WebDaifugo.AppClasses
{
    public class PlayRoomsManager
    {
        private static Dictionary<string, DaifugoPlayRoom> pool = new Dictionary<string, DaifugoPlayRoom>();
        private static Random rand = new Random();

        public static DaifugoPlayRoom GetOrCreate(string sessionId, string rule="A")
        {
            lock (pool)
            {
                sessionId = sessionId ?? "0";
                if (!pool.ContainsKey(sessionId))
                {
					var pr = new DaifugoPlayRoom(sessionId, rule);
                    pool[sessionId] = pr;
                    pr.Master.wait_msec = 800;
                    return pr;
                }
                return pool[sessionId];
            }
        }

        public static void Remove(string sessionId)
        {
            lock (pool)
            {
                sessionId = sessionId ?? "0";
                if (pool.ContainsKey(sessionId))
                {
                    pool.Remove(sessionId);
                }
            }
        }

        public static string MakeTempSession(string prefix="")
        {
            var sessionId = (prefix + rand.Next(10000)).ToString();
            return pool.Keys.Contains(sessionId) ? MakeTempSession() : sessionId;
        } 

        public static DaifugoPlayRoom Get(string sessionid)
        {
            return pool.ContainsKey(sessionid) ? pool[sessionid] : null;
        }
    }
}