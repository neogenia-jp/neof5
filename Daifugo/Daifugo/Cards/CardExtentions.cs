using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Utils;

namespace Daifugo.Cards
{

    public static class CardExtentions
    {

        /// <summary>
        /// カードの組み合わせを文字列化する
        /// </summary>
        /// <param name="cardset"></param>
        /// <returns></returns>
        public static string ToCardsetString(this IEnumerable<Card> cardset)
        {
            return cardset.JoinString();
        }

        /// <summary>
        /// ジョーカーを別のコレクションに取り分ける
        /// </summary>
        /// <param name="listcards"></param>
        /// <returns></returns>
        public static List<Card> ExtractJokers(this List<Card> listcards)
        {
            var jokers = listcards.Where(c => c.IsJoker()).ToList();
            listcards.RemoveAll(c => c.IsJoker());
            return jokers;
        }
    }
}
