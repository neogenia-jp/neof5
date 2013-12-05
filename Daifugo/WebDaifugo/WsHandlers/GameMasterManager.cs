using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.WsHandlers
{
    public class GameMasterManager
    {
        private static Dictionary<string, GameMaster> pool = new Dictionary<string, GameMaster>();

        public static GameMaster GetOrCreate(string sessionId, string rule="A")
        {
            lock (pool)
            {
                sessionId = sessionId ?? "0";
                if (!pool.ContainsKey(sessionId))
                {
                    var ctx = ContextFactory.CreateGameContext(rule=="A"?0:1);
                    var gm = ContextFactory.CreateGameMaster(ctx);
                    gm.wait_msec = 800;
                    pool[sessionId] = gm;
                    return gm;
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

        public static string MakeTempSession()
        {
            int sessionId = 10000;
            while (pool.Keys.Contains("" + sessionId)) sessionId++;
            return "" + sessionId;
        }

    }
}