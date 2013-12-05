using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Cards;
using System.Collections.ObjectModel;
using Daifugo.Bases;

namespace Daifugo.GameImples
{
    internal class PlayerContext : IPlayerContext
    {
        // ゲーム全体のコンテキスト
        internal readonly BaseGameContext _gameContext;
        public IGameContext GameContext { get { return _gameContext; } }

        //// プレイヤー情報
        //private Func<BaseGameContext, IEnumerable<IPlayerInfo>> func;
        //public IEnumerable<IPlayerInfo> PlayerInfo { get { return func(_gameContext); } }

        // 手札
        internal readonly List<Card> _deck = new List<Card>(11);
        public IEnumerable<Card> Deck { get { return _deck.ToArray(); } }

        // ランク
        public PlayerRank Ranking { get; internal set; }

        // あがった順番（順位）
        public int OrderOfFinish { get; internal set; }
        
        //[Obsolete]
        //public bool LastIsPass { get; internal set; }


        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        /// <param name="gameContext"></param>
        internal PlayerContext(IGamePlayer player, BaseGameContext gameContext) {
            Ranking = PlayerRank.HEIMIN;
            _gameContext = gameContext;
            //this.func = func;
            _gameContext.AddPlayer(player, () => new BasePlayerInfo()
            {
                GetName = () => player.Name,
                GetHavingCard = () => _deck,
                GetRanking = () => Ranking,
                GetOrderOfFinish = () => OrderOfFinish,
                //GetLastIsPass = () => LastIsPass,
            });            
        }

        ///// <summary>
        ///// プレイヤー情報を表すクラス
        ///// </summary>
        //internal class _PlayerInfo : IPlayerInfo
        //{
        //    internal Func<string> GetName;
        //    internal Func<int> GetHavingCardCount;
        //    internal Func<PlayerRank> GetRanking;
        //    internal Func<int> GetOrderOfFinish;
        //    //internal Func<bool> GetLastIsPass;

        //    public string Name { get { return GetName(); } }
        //    public int HavingCardCount { get { return GetHavingCardCount(); } }
        //    public PlayerRank Ranking { get { return GetRanking(); } }
        //    public int OrderOfFinish { get { return GetOrderOfFinish(); } }
        //    //public bool LastIsPass { get { return GetLastIsPass(); } }
        //}

        public void Reset()
        {
            _deck.Clear();
            OrderOfFinish = 0;
        }
    }

    public class BaseMonitorContext : IMonitorContext
    {
        // ゲーム全体のコンテキスト
        internal readonly BaseGameContext _gameContext;
        public IGameContext GameContext { get { return _gameContext; } }

        //// プレイヤー情報
        //private Func<BaseGameContext, IEnumerable<IPlayerInfo>> func;
        //public IEnumerable<IPlayerInfo> PlayerInfo { get { return func(_gameContext); } }

        // 全員の手札
        internal readonly Func<IEnumerable<IEnumerable<Card>>> _func;
        public IEnumerable<IEnumerable<Card>> AllDeck { get { return _func(); } }

        /// <summary>
        /// コンストラクタ（非公開）
        /// </summary>
        /// <param name="gameContext"></param>
        internal BaseMonitorContext(BaseGameContext gameContext, Func<IEnumerable<IEnumerable<Card>>> getAllDeckFunc) {
            _gameContext = gameContext;
            _func = getAllDeckFunc;
        }
    }

}
