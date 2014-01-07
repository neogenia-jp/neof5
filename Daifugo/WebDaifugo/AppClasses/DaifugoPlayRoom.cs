using Daifugo.Bases;
using Daifugo.GameImples;
using Daifugo.Observers;
using System.Collections.Generic;
using System.Linq;
using WebDaifugo.Basis;
using WebDaifugo.Models;

namespace WebDaifugo.AppClasses
{
    public class DaifugoPlayRoom : IGameEventListener, IPlayRoomModel 
    {
        public GameMaster Master { get; private set; }
        public string RoomID { get; private set;  }
		public string Rule {get;private set;}

        private readonly Dictionary<string, IGamePlayer> _players = new Dictionary<string, IGamePlayer>();
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

        private void OnGameFinish(GameEvents.getcontext ctx)
        {
            var _ctx = ctx(this).GameContext;
			int i=0;
            foreach (var pi in _ctx.PlayerInfo)
            {
                _standings.Values.ElementAt(i++).AddGameResult(pi.Ranking);
            }
        }

        public void Dispose()
        {
            Master.finish -= OnGameFinish;
            Master.Dispose();
            _standings.Clear();
        }

        public void AddPlayer(string key, IGamePlayer p)
        {
            Master.AddPlayer(p);
            _players.Add(key, p);
            _standings.Add(p, new StandingsModel(p.Name));
        }

        public IRemoteGamePlayer FindRemotePlayer(string key)
        {
            IGamePlayer p = null;
            _players.TryGetValue(key, out p);
            return p as IRemoteGamePlayer;
        }

        public void Tweet(string message)
        {
            foreach (ITweetListener pl in _players.Values.Where((p) => p is ITweetListener))
            {
                pl.Tweet(message);
            }
            foreach (ITweetListener mn in Master.Observers)
            {
                mn.Tweet(message);
            }
        }

        public void AddObserver(IGameEventListener m)
        {
            Master.AddObserver(m);
        }
        
        public void RemoveObserver(IGameEventListener m)
        {
            Master.RemoveObsrver(m);
        }

        public void bindEvents(GameEvents evt)
        {
            evt.finish += OnGameFinish;
        }

        public void unbindEvents(GameEvents evt)
        {
            evt.finish -= OnGameFinish;
        }


        public void DoComplementPlayers(int num)
        {
            var limit = num - Master.NumOfPlayers;
            for (int i = 1; i <= limit; i++)
            {
                var name = "COM" + i;
                AddPlayer(name, PoorPlayer.Create(name, Master));
            }
        }
    }
}