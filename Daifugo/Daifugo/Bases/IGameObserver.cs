using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    public interface IGameObserver
    {
        void Connect(GameEvents evt);

        ///// <summary>
        ///// トリック開始通知
        ///// </summary>
        ///// <param name="ctx"></param>
        //void Start(T ctx);

        ///// <summary>
        ///// 手札カード（の一部）が配布された
        ///// </summary>
        ///// <param name="ctx"></param>
        //void CardDistributed(T ctx);

        ///// <summary>
        ///// 手札カードが交換された
        ///// </summary>
        ///// <param name="ctx"></param>
        //void CardSwapped(T ctx);

        ///// <summary>
        /////  ほかのプレイヤーに手番がまわった
        ///// </summary>
        ///// <param name="ctx"></param>
        //void Thinking(T ctx);

        ///// <summary>
        ///// 手番の人がカードを捨てた
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="cards"></param>
        //void CardsArePut(T ctx);

        ///// <summary>
        ///// 革命がおこった
        ///// </summary>
        ///// <param name="ctx"></param>
        //void Kakumei(T ctx);

        ///// <summary>
        ///// 流れた
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="cards"></param>
        //void Nagare(T ctx);

        ///// <summary>
        ///// 手番の人が上がった
        ///// </summary>
        ///// <param name="ctx"></param>
        //void Agari(T ctx);

        ///// <summary>
        ///// トリック終了通知
        ///// </summary>
        ///// <param name="ctx"></param>
        //void Finish(T ctx);

    }
}
