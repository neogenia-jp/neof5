using Daifugo.Bases;
using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDaifugo.Models;

namespace WebDaifugo.AppClasses
{
    public class DaifugoPlayRoom : IPlayRoomModel, IDisposable, IGameMonitor
    {
        public GameMaster Master { get; private set; }
        public string RoomID { get; private set;  }
		public string Rule {get;private set;}

        private readonly Dictionary<IGamePlayer, StandingsModel> _standings = new Dictionary<IGamePlayer, StandingsModel>();
        public IEnumerable<IStandingsModel> Standings { get { return _standings.Values; } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="roomID"></param>
        internal DaifugoPlayRoom(string roomID, string rule) { 
            RoomID = roomID;
            Rule = rule;
            var ctx = ContextFactory.CreateGameContext(rule=="A"?0:1);
	        Master = ContextFactory.CreateGameMaster(ctx);
            Master.AddObserver(this);
        }

        internal void OnGameFinish(GameEvents.getcontext ctx)
        {
            var _ctx = ctx(this).GameContext;
			int i=0;
            foreach (var pi in _ctx.PlayerInfo)
            {
                var s = _standings.ElementAt(i);
                s.Value.AddGameResult(pi.Ranking);
            }
        }

        public void Dispose()
        {
            Master.finish -= OnGameFinish;
            Master.Dispose();
            _standings.Clear();
        }

        public void AddPlayer(IGamePlayer p)
        {
            Master.AddPlayer(p);
            _standings.Add(p, new StandingsModel(p.Name));
        }

        public void AddObserver(IGameMonitor m)
        {
            Master.AddObserver(m);
        }
        
        public void RemoveObserver(IGameMonitor m)
        {
            Master.RemoveObsrver(m);
        }

        public void Connect(GameEvents evt)
        {
            evt.finish += OnGameFinish;
        }
    }
}