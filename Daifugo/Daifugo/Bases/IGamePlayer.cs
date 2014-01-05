using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    using ContextType = IPlayerContext;

    public interface IGamePlayer : IGameObserver<ContextType>
    {
        /// <summary>
        /// プレイヤー名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 手番がまわってきたときの処理
        /// </summary>
        /// <param name="ctx"></param>
        void ProcessTurn(ContextType ctx);
    }


    public static class IGamePlayerExtention
    {
        public static void BindEvents(this IGamePlayer _this, GameEvents evt)
        {
            var a = GameEventBinder<ContextType,IGamePlayer>.GetOrCreate(_this,evt);
            a.bindEvents();
        }
        public static void UnbindEvents(this IGamePlayer _this, GameEvents evt)
        {
            var a = GameEventBinder<ContextType,IGamePlayer>.GetOrCreate(_this,evt);
            a.unbindEvents();
        }
    }
}


