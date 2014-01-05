using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebDaifugo.Basis;

namespace WebDaifugo.Models
{
    public class StandingsModel : IStandingsModel
    {
		private readonly Dictionary<PlayerRank, int> _counts = new Dictionary<PlayerRank, int>();
		private int _getScoreByRank(PlayerRank r) {int p=0; _counts.TryGetValue(r, out p); return p;}

        public string PlayerName { get; private set; }

        public int NumTimesDaifugo { get { return _getScoreByRank(PlayerRank.DAIFUGO); } }
        public int NumTimesFugo { get{ return _getScoreByRank(PlayerRank.FUGO); } }
        public int NumTimesHeimin { get{ return _getScoreByRank(PlayerRank.HEIMIN);  }}
        public int NumTimesHinmin { get{ return _getScoreByRank(PlayerRank.HINMIN); }}
        public int NumTimesDaihinmin { get { return _getScoreByRank(PlayerRank.DAIHINMIN); } }

        public int RoundPoint { get { return _counts.Sum(s => s.Key.PointOfRank() * s.Value); } }
        public int TransferredPoint { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="playerName"></param>
        internal StandingsModel(string playerName)
        {
            PlayerName = playerName;
            TransferredPoint = 0;
        }

		/// <summary>
		/// ゲームの結果を追加
		/// </summary>
		/// <param name="rank"></param>
        public void AddGameResult(PlayerRank rank)
        {
            int p = 0;
            _counts.TryGetValue(rank, out p);
            _counts[rank] = ++p;
        }

        public void Clear() { _counts.Clear(); }
    }
}