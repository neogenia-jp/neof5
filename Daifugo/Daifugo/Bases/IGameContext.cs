using Daifugo.Cards;
using Daifugo.Rules;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    /// <summary>
    /// 全体に公開可能なプレイヤー情報
    /// </summary>
    public interface IPlayerInfo
    {
        string Name { get; }
        int HavingCardCount { get; }
        PlayerRank Ranking { get; }
        int OrderOfFinish { get; }
    }

    /// <summary>
    ///  モニターにのみ公開可能なプレイヤー情報
    /// </summary>
    public interface IPlayerInfoForMonitor : IPlayerInfo
    {
        IEnumerable<Card> HavingCards { get; }
    }

    /// <summary>
    /// ゲーム全体の情況
    /// </summary>
    public interface IGameContext
    {
        /// <summary>
        /// プレイヤー情報
        /// </summary>
        IEnumerable<IPlayerInfo> PlayerInfo { get; }

        /// <summary>
        /// 現在の親（0から数える）
        /// </summary>
        //int LastPutPlayerNum { get; }

        /// <summary>
        /// 現在の手番（0から数える）
        /// </summary>
        int Teban { get; }

        /// <summary>
        /// 今、場に出ているカード（古いものが先頭）
        /// </summary>
        IEnumerable<IEnumerable<Card>> Ba { get; }

        /// <summary>
        /// 流れたカード
        /// </summary>
        IEnumerable<Card> Yama { get; }

        /// <summary>
        /// ゲーム進行のすべての履歴
        /// </summary>
        IEnumerable<GameHistoryEntity> History { get; }

        /// <summary>
        /// 革命中かどうか
        /// </summary>
        bool IsKakumei { get; set; }

        /// <summary>
        /// ルール
        /// </summary>
        IRule Rule { get; }
    }

}
