using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Bases
{

    /// <summary>
    /// プレイヤー情報を表すクラス
    /// </summary>
    internal class BasePlayerInfo : IPlayerInfo
    {
        internal Func<string> GetName;
        internal Func<IEnumerable<Card>> GetHavingCard;
        internal Func<PlayerRank> GetRanking;
        internal Func<int> GetOrderOfFinish;

        public string Name { get { return GetName(); } }
        public int HavingCardCount { get { return GetHavingCard().Count(); } }
        public PlayerRank Ranking { get { return GetRanking(); } }
        public int OrderOfFinish { get { return GetOrderOfFinish(); } }
        //public bool LastIsPass { get { return GetLastIsPass(); } }
    }

    internal class BasePlayerInfoForMonitor : BasePlayerInfo, IPlayerInfoForMonitor
    {
        public IEnumerable<Card> HavingCards { get { return GetHavingCard(); } }
    }

    /// <summary>
    /// IGameContextの基本実装
    /// </summary>
    public abstract class BaseGameContext : IGameContext
    {
        protected readonly Dictionary<IGamePlayer, Func<IPlayerInfo>> _getPInfoFuncs = new Dictionary<IGamePlayer, Func<IPlayerInfo>>();

        // プレイヤーを追加する
        public virtual void AddPlayer(IGamePlayer p, Func<IPlayerInfo> f) { _getPInfoFuncs.Add(p, f); }

        // プレイヤー情報
        public abstract IEnumerable<IPlayerInfo> PlayerInfo { get; }

        //// 現在の親
        //public abstract int LastPutPlayerNum { get; set; }

        // 現在の手番
        public abstract int Teban { get; set; }

        // 場に出ているカード
        public abstract IEnumerable<IEnumerable<Card>> Ba { get; }

        // 流れたカード
        public abstract IEnumerable<Card> Yama { get; }

        // カードヒストリ
        public abstract IEnumerable<GameHistoryEntity> History {get; }

        // 革命中かどうか
        public abstract bool IsKakumei { get; set; }

        // ルール
        public abstract IRule Rule { get; protected set; }

    }
    
}
