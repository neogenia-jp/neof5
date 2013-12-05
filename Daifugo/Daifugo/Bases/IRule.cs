using System;
using System.Collections.Generic;
using Daifugo.Bases;
using Daifugo.Cards;

namespace Daifugo.Bases
{
    public interface IRule
    {
        /// <summary>
        /// ジョーカー枚数
        /// </summary>
        int NumOfJoker { get; }

        /// <summary>
        /// 場にカードが出せるかどうかを判定する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        ICheckResult CheckPutCards(IGameContext context, IEnumerable<Card> cards);

        /// <summary>
        /// 革命を考慮したカードの強弱判定
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="isKakumei"></param>
        /// <returns></returns>
        int CompareCards(Card a, Card b, bool isKakumei = false);

        /// <summary>
        /// あがることができるカードか？
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isKakumei"></param>
        /// <returns></returns>
        ICheckResult CanAgari(IGameContext context, IEnumerable<Card> cards);
    }
}
