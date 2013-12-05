using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    public interface IObserberContext
    {
        /// <summary>
        /// ゲーム全体のコンテキスト
        /// </summary>
        IGameContext GameContext { get; }
    }

    /// <summary>
    /// あるプレイヤーから見たゲームの情況
    /// </summary>
    public interface IPlayerContext : IObserberContext
    {

        /// <summary>
        /// 自分の手札
        /// </summary>
        IEnumerable<Card> Deck { get; }
    }

    /// <summary>
    /// モニターから見たゲームの情況
    /// </summary>
    public interface IMonitorContext : IObserberContext
    {

        // 全員の手札
        IEnumerable<IEnumerable<Card>> AllDeck { get; }
    }
}
