using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Daifugo.Cards
{
    /// <summary>
    /// カードの組を生成するためのジェネレータクラス
    /// </summary>
    public static class DeckGenerator {
        private static ReadOnlyCollection<Card> _baseDeck = null;

        /// <summary>
        /// １セットのトランプを生成する。ジョーカーは含まれない。
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<Card> Deck()
        {
            if (_baseDeck == null)
            {
                var deck = new Card[52];
                int i = 0;
                foreach (Suit s in Enum.GetValues(typeof(Suit)))
                {
                    if (s != Suit.JKR)
                    {
                        for (byte n = 1; n <= 13; n++)
                        {
                            deck[i++] = new Card(s, n);
                        }
                    }
                }
                _baseDeck = new ReadOnlyCollection<Card>(deck);
            }
            return _baseDeck;
        }

        /// <summary>
        /// １セットのトランプを生成する。ジョーカーの枚数を指定可能。
        /// </summary>
        /// <returns></returns>
        public static List<Card> DeckWithJoker(int num = 1)
        {
            var l = new List<Card>(Deck());
            while(num-- > 0) l.Add(new Card(Suit.JKR, 0));
            return l;
        }

        /// <summary>
        /// カードの一括生成(内部用)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        internal static List<Card> Generate(Suit s, int num, params object[] a)
        {
            List<Card> l = new List<Card>();
            l.Add(new Card(s,(byte)num));
            for(int i=0; i<a.Length; i+=2){
                l.Add(new Card((Suit)a[i], (byte)Convert.ChangeType(a[i+1], typeof(byte))));
            }
            return l;
        }

        /// <summary>
        /// カードの組み合わせを文字列から復元する
        /// </summary>
        /// <param name="cardsetstr"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static List<Card> FromCardsetString(string cardsetstr, int offset = 0, int length = 0)
        {
            List<Card> l = new List<Card>();

            int i = offset;
            int len = cardsetstr.Length - offset;
            if (length > 0) len = Math.Min(len, length);

            while (i < offset + len - 1)
            {
                if (cardsetstr[i] == ' ') i++;
                else
                {
                    l.Add(Card.FromString(cardsetstr, i));
                    i += 2;
                }
            }
            return l;
        }
    }
}
