using Daifugo.Bases;
using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.GameImples
{
    public class BaseMonitorContext : IMonitorContext
    {
        // ゲーム全体のコンテキスト
        internal readonly BaseGameContext _gameContext;
        public IGameContext GameContext { get { return _gameContext; } }


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
