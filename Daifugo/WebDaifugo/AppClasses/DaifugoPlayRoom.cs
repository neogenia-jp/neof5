using Daifugo.Bases;
using Daifugo.GameImples;
using Daifugo.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
using WebDaifugo.Basis;
using WebDaifugo.Models;
using Daifugo.Utils;

namespace WebDaifugo.AppClasses
{
	/// <summary>
	/// プレイルームの実装クラス
	/// </summary>
    public class DaifugoPlayRoom : IGameEventListener, IPlayRoomModel 
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


		// 未使用となるタイムアウト時間（分）
        private const int UNUSED_TIMEOUT_MINUTES = 60;

		// ゲームマスタ
        public GameMaster Master { get; private set; }
		// ルームID
        public string RoomID { get; private set;  }
		// ルール(インタフェースメンバーにすべき？)
		public string Rule {get;private set;}
		// プレイヤーの成績表取得インタフェース
        public IEnumerable<IStandingsModel> Standings { get { return _standings.Values; } }

		// プレイヤーオブジェクトのコレクション（プレイヤー名=>IGamePlayer）
        private readonly Dictionary<string, IGamePlayer> _players = new Dictionary<string, IGamePlayer>();
		// プレイヤーの成績表のコレクション
        private readonly Dictionary<IGamePlayer, StandingsModel> _standings = new Dictionary<IGamePlayer, StandingsModel>();

		// 現在のラウンド数
        public int NumOfRounds { get; private set; }

        private DateTime _lastUsed;
        private void _updateLastUsed() { _lastUsed = DateTime.Now; }

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
            _updateLastUsed();
        }

		/// <summary>
		/// ゲーム開始時のイベントハンドラ
		/// </summary>
		/// <param name="ctx"></param>
        private void OnGameStart(GameEvents.getcontext ctx)
        {
            _updateLastUsed();
            NumOfRounds++;

            logger.Info("[{0}] Game start. Round={1} Players={2}", RoomID, NumOfRounds, _players.Keys.JoinString());
        }

		/// <summary>
		/// ゲーム終了時のイベントハンドラ
		/// </summary>
		/// <param name="ctx"></param>
        private void OnGameFinish(GameEvents.getcontext ctx)
        {
            _updateLastUsed();
            var _ctx = ctx(this).GameContext;
			int i=0;
            foreach (var pi in _ctx.PlayerInfo)
            {
                _standings.Values.ElementAt(i++).AddGameResult(pi.Ranking);
            }

            logger.Info("[{0}] Game finished. Round={1} History={2}", RoomID, NumOfRounds, _ctx.History.Select(h=>h.ToString()).JoinString());
        }

		/// <summary>
		/// プレイヤーの入室
		/// </summary>
		/// <param name="key"></param>
		/// <param name="p"></param>
        public void AddPlayer(string key, IGamePlayer p)
        {
            _updateLastUsed();
            try
            {
                _players.Add(key, p);
                Master.AddPlayer(p);
                _standings.Add(p, new StandingsModel(p.Name));
                if (_players.Count >= 5) { Master.Start(); }  // 5人そろったら自動的にゲーム開始
            }
            catch (Exception ex)
            {
                logger.ErrorException("Error in Addplayer().", ex);
            }
        }

		/// <summary>
		/// オブザーバーの入室
		/// </summary>
		/// <param name="m"></param>
        public void AddObserver(IGameEventListener m)
        {
            Master.AddObserver(m);
            logger.Info("[{0}] Visit observer. {1}", RoomID, m);
        }
        
		/// <summary>
		/// オブザーバーの退室
		/// </summary>
		/// <param name="m"></param>
        public void RemoveObserver(IGameEventListener m)
        {
            Master.RemoveObsrver(m);
            logger.Info("[{0}] Leave observer. {1}", RoomID, m);
        }

		/// <summary>
		/// キーを指定してリモートプレイヤーを検索
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        public IRemoteGamePlayer FindRemotePlayer(string key)
        {
            IGamePlayer p = null;
            _players.TryGetValue(key, out p);
            return p as IRemoteGamePlayer;
        }

		/// <summary>
		/// Tweet発生時のイベントハンドラ
		/// </summary>
		/// <param name="message"></param>
        public void Tweet(int tweetedPlayerNum, string message)
        {
            logger.Info("[{0}] Tweet. {1}", RoomID, message);
            foreach (ITweetListener pl in _players.Values.Where((p) => p is ITweetListener))
            {
                pl.Tweet(tweetedPlayerNum, message);
            }
            foreach (ITweetListener mn in Master.Observers.Where((o)=>o is ITweetListener))
            {
                mn.Tweet(tweetedPlayerNum, message);
            }
        }

        public void bindEvents(GameEvents evt)
        {
            evt.start += OnGameStart;
            evt.finish += OnGameFinish;
        }

        public void unbindEvents(GameEvents evt)
        {
            evt.start -= OnGameStart;
            evt.finish -= OnGameFinish;
        }

        public void Dispose()
        {
            logger.Info("[{0}] Dispose() called.", RoomID);
            Master.start -= OnGameStart;
            Master.finish -= OnGameFinish;
            Master.Dispose();
            _standings.Clear();
        }

		/// <summary>
		/// 不足人数をCOMプレイヤーで補う
		/// </summary>
		/// <param name="num">希望するプレイヤー人数</param>
        public void DoComplementPlayers(int num)
        {
            var limit = num - Master.NumOfPlayers;
            for (int i = 1; i <= limit; i++)
            {
                var name = "COM" + i;
                AddPlayer(name, PoorPlayer.Create(name, Master));
            }
        }

		/// <summary>
		/// 使用していないかどうかを判定する
		/// </summary>
		/// <returns></returns>
        public bool IsUnused()
        {
			// ある一定時間以上使用していなければ未使用と判断する。
            return _lastUsed <= DateTime.Now.AddMinutes(-UNUSED_TIMEOUT_MINUTES);
        }
    }
}