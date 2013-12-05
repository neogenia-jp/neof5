using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    public interface IGamePlayer : IGameObserver
    {
        /// <summary>
        /// プレイヤー名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 手番がまわってきたときの処理
        /// </summary>
        /// <param name="ctx"></param>
        void ProcessTurn(IPlayerContext ctx);
    }
}
