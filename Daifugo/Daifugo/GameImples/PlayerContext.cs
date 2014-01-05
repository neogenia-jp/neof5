using System.Collections.Generic;
using Daifugo.Cards;
using Daifugo.Bases;

namespace Daifugo.GameImples
{
    internal class PlayerContext : IPlayerContext
    {
        // ゲーム全体のコンテキスト
        internal readonly BaseGameContext _gameContext;
        public IGameContext GameContext { get { return _gameContext; } }

        // 手札
        internal readonly List<Card> _deck = new List<Card>(11);
        public IEnumerable<Card> Deck { get { return _deck.ToArray(); } }

        // ランク
        public PlayerRank Ranking { get; internal set; }

        // あがった順番（順位）
        public int OrderOfFinish { get; internal set; }
        

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

        public void Reset()
        {
            _deck.Clear();
            OrderOfFinish = 0;
        }
    }
}
