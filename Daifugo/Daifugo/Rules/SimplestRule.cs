using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Cards;
using System.Diagnostics;
using Daifugo.Bases;

namespace Daifugo.Rules
{
    /// <summary>
    /// 最もシンプルなルール
    /// ・ジョーカーは1枚
    /// ・革命あり、革命返しあり
    /// </summary>
    public class SimplestRule : IRule
    {
        public static readonly CheckError ERR_DIFFERENT_NUM = new CheckError("数字が異なるカードが含まれています。");
        public static readonly CheckError ERR_COUNT_DOESNT_MATCH = new CheckError("枚数が一致していません。");
        public static readonly CheckError ERR_NOT_STRONG = new CheckError("強いカードではありません。");
        public static readonly CheckError ERR_INVALID_BA = new CheckError("場のカードが不正です。");

        public virtual int NumOfJoker { get { return 1; } }

        /// <summary>
        /// 場にカードが出せるかどうかを判定する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public virtual ICheckResult CheckPutCards(IGameContext context, IEnumerable<Card> cards)
        {
            // パス？
            if (!cards.Any()) return CheckResults.Ok;

            Card firstCard = cards.First();
            if (!TryGetFirstCard(cards, ref firstCard))
            {
                return ERR_DIFFERENT_NUM;
            }

            // 場にカードが出ていない？
            if (!context.Ba.Any()) return CheckResults.Ok;
            
            // 出せるカードの組み合わせか？
            var lastCards = context.Ba.Last();

            // 場と枚数が一致してるか？
            if (lastCards.Count() != cards.Count())
            {
                return ERR_COUNT_DOESNT_MATCH;
            }
            
            Card baCard = null;
            if (!TryGetFirstCard(lastCards, ref baCard)) return ERR_INVALID_BA;

            // 場より強いカードか？
            if (CompareCards(baCard, firstCard, context.IsKakumei) <= 0)
            {
                return ERR_NOT_STRONG;
            }
            
            return CheckResults.Ok;
        }

        /// <summary>
        /// 革命を考慮したカードの強弱判定
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="isKakumei"></param>
        /// <returns></returns>
        public virtual int CompareCards(Card a, Card b, bool isKakumei=false)
        {
            // 同じ数字かチェック
            if (a.num == b.num) return 0;

            // 革命に関係なくジョーカーが最強
            if (a.suit == Suit.JKR) return -1;
            if (b.suit == Suit.JKR) return 1;

            // 数字で比較
            int ret = a > b ? -1 :1;
            // 革命時は判定逆転
            return isKakumei ? -ret : ret;
        }

        internal bool TryGetFirstCard(IEnumerable<Card> cards, ref Card firstCard) {
            switch(cards.Count()){
                case 0: return true;   // パス
                case 1: firstCard = cards.First(); return true;  // 1枚のとき
            }
            // ジョーカー以外に2種類以上の数字が含まれている場合はダメ
            var x = from t1 in cards where !t1.IsJoker() group t1 by t1.num;
            if (x.Count() > 1) return false;
            firstCard = x.First().First();
            return true;
        }
        public virtual ICheckResult CanAgari(IGameContext context, IEnumerable<Card> cards) { return CheckResults.Ok; }
    }
}
