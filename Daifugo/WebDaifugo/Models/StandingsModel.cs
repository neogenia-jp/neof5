using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDaifugo.Basis;

namespace WebDaifugo.Models
{
	/// <summary>
	/// プレイヤーの成績表
	/// </summary>
    public class StandingsModel : IStandingsModel
    {
		private readonly Dictionary<PlayerRank, int> _counts = new Dictionary<PlayerRank, int>();
		private int _getScoreByRank(PlayerRank r) {int p=0; _counts.TryGetValue(r, out p); return p;}

		// プレイヤー名
        public string PlayerName { get; private set; }

		// ランクごとの成った回数の合計
        public int NumTimesDaifugo { get { return _getScoreByRank(PlayerRank.DAIFUGO); } }
        public int NumTimesFugo { get{ return _getScoreByRank(PlayerRank.FUGO); } }
        public int NumTimesHeimin { get{ return _getScoreByRank(PlayerRank.HEIMIN);  }}
        public int NumTimesHinmin { get{ return _getScoreByRank(PlayerRank.HINMIN); }}
        public int NumTimesDaihinmin { get { return _getScoreByRank(PlayerRank.DAIHINMIN); } }

		// 直前ラウンドの点数
        public int RoundPoint { get; private set; }

		// 第1ラウンドからの累計点数
        public int TotalPoint { get { return _counts.Sum(s => s.Key.PointOfRank() * s.Value); } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="playerName"></param>
        internal StandingsModel(string playerName)
        {
            PlayerName = playerName;
            RoundPoint = 0;
        }

		/// <summary>
		/// ゲームの結果を追加
		/// </summary>
		/// <param name="rank"></param>
        public void AddGameResult(PlayerRank rank)
        {
            int tmp = TotalPoint;
            int p = 0;
            _counts.TryGetValue(rank, out p);
            _counts[rank] = ++p;
            RoundPoint = tmp == 0 ? TotalPoint : tmp - TotalPoint;
        }

        public void Clear() { _counts.Clear(); }
    }
}