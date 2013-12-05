using Daifugo.Bases;
using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Rules
{
    /// <summary>
    /// 階段ありルール
    /// ・ジョーカーは1枚
    /// ・革命、革命返しあり
    /// ・階段あり（同じスートで3枚以上）
    /// ・ジョーカーあがり、2上がり禁止（革命時は3上がり禁止）
    /// </summary>
    public class KaidanRule : SimplestRule
    {
        public static readonly CheckError ERROR_JOKER_AGARI = new CheckError("ジョーカーあがりは禁止です");
        public static readonly CheckError ERROR_2_AGARI = new CheckError("２あがり（革命時は３あがり）は禁止です");
        public static readonly CheckError ERROR_BA_ISNOT_KAIDAN = new CheckError("場が階段でない");
        public static readonly CheckError ERROR_NOT_KAIDAN = new CheckError("カードが階段でない");

        public override int NumOfJoker { get { return 1; } }

        /// <summary>
        /// 場にカードが出せるかどうかを判定する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public override ICheckResult CheckPutCards(IGameContext context, IEnumerable<Card> cards)
        {
            var ba = context.Ba;
            var ret = base.CheckPutCards(context, cards);
            if (ret is CheckOK) return ret;

            if (ba.Any())
            {
                // 場が階段かどうか
                if (IsKaidan(ba.Last()))
                {
                    // 手札も階段か？
                    if (IsKaidan(cards))
                    {
                        // 強さを比較
                        var balast = ba.Last().ToList();
                        balast.ExtractJokers();
                        var bamax = balast.Max();
                        var hand = cards.ToList();
                        hand.ExtractJokers();
                        var handmin = hand.Min();
                        if (handmin > bamax) return CheckResults.Ok;
                        return ERR_NOT_STRONG;
                    }
                    else { return ERROR_NOT_KAIDAN; }
                }
                else
                {
                    if (IsKaidan(cards)) { return ERROR_BA_ISNOT_KAIDAN; }
                }
            }
            else
            {
                if (IsKaidan(cards)) return CheckResults.Ok;
            } 
            return ret;
        }

        public static bool IsKaidan(IEnumerable<Card> cards)
        {
            var listcards = cards.ToList();

            if (listcards.Count < 3) return false; 

            // ジョーカーを別のコレクションに取り分ける
            var jorkers = listcards.ExtractJokers();

            // 同じスートか？
            var first = listcards.First();
            if (!listcards.All(c => c.suit == first.suit)) return false;
            
            // 階段順になっているか？
            int pj = 0;
            listcards.Sort();
            for (int i = 0; i < listcards.Count - 1; i++)
            {
                var cmp = listcards[i+1].CompareTo(listcards[i]);
                cmp--;
                if (cmp==0) continue;
                if (cmp <= (jorkers.Count-pj)) { pj+=cmp; continue; } // ジョーカーを持っている場合はジョーカーを代わりに消費
                return false;
            }
            return true; 
        }

        /// <summary>
        /// あがることができるカードか？
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isKakumei"></param>
        /// <returns></returns>
        public override ICheckResult CanAgari(IGameContext context, IEnumerable<Card> cards)
        {
            var listcards = cards.ToList();
            var jokers = listcards.ExtractJokers();

            // ジョーカーあがり？
            if (listcards.Count == 0) return ERROR_JOKER_AGARI;

            // 2あがり？
            if (listcards.All(c => c.num == (context.IsKakumei ? 3 : 2))) return ERROR_2_AGARI;

            return CheckResults.Ok;
        }
    }
}
