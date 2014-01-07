using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Cards
{
    public class Card : IComparable, IComparable<Card>
    {
        private byte xx;  // カードの内部表現

        public Suit suit { get { return (Suit)(xx & 0xF0); } } /// スート
        public byte num { get { return (byte)(xx & 0x0F); } }  /// 数字

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="s"></param>
        /// <param name="n"></param>
        internal Card(Suit s, int n)
        {
            if (s == Suit.JKR) n = 0;
            else if (n < 1 || 13 < n) throw new ArgumentOutOfRangeException("Illegual number.");
            xx = (byte)(((byte)s) | n);
        }

        internal Card(byte xx)
        {
            this.xx = xx;
        }

        /// <summary>
        /// ジョーカーかどうかの判定
        /// </summary>
        /// <returns></returns>
        public bool IsJoker() { return suit == Suit.JKR; }

        /// <summary>
        /// 文字列表現。英数２文字でカードを表す。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            char[] s = new char[] { 'J', 'K' };
            if (suit != Suit.JKR)
            {
                switch (suit)
                {
                    case Suit.CLB: s[0] = 'C'; break;
                    case Suit.DIA: s[0] = 'D'; break;
                    case Suit.HRT: s[0] = 'H'; break;
                    case Suit.SPD: s[0] = 'S'; break;
                }
                switch (num)
                {
                    case 1: s[1] = 'A'; break;
                    case 10: s[1] = '0'; break;
                    case 11: s[1] = 'J'; break;
                    case 12: s[1] = 'Q'; break;
                    case 13: s[1] = 'K'; break;
                    default: s[1] = (char)('0' + num); break;
                }
            }
            return new string(s);
        }

        /// <summary>
        /// 文字列からの復元
        /// </summary>
        /// <param name="str"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Card FromString(string str, int offset=0)
        {
            if (str == null || str.Length-offset < 2) throw new ArgumentException("String is too short.");

            Suit s = Suit.JKR;
            int n = 0;
            switch (str[offset])
            {
                case 'C': s = Suit.CLB; break;
                case 'D': s = Suit.DIA; break;
                case 'H': s = Suit.HRT; break;
                case 'S': s = Suit.SPD; break;
                case 'J': s = Suit.JKR; break;
                default: throw new ArgumentException("Unknown Suit.");
            }
            switch (str[offset+1])
            {
                case 'A': n = 1; break;
                case 'K': n = 13; break;
                case 'Q': n = 12; break;
                case 'J': n = 11; break;
                case '0': n = 10; break;
                default: n = str[offset+1] - '0'; break;
            }
            return new Card(s, (byte)n);
        }

        /// <summary>
        /// 同じカードかどうか判定する
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(Object o)
        {
            if (!(o is Card)) return false;
            return this == (Card)o;
        }

        public override int GetHashCode()
        {
            return (int)(suit) + num;
        }

        /// <summary>
        /// 同じカードかどうか判定する
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Card a, Card b)
        {
            if (a.suit != b.suit) return false;
            if (a.num != b.num) return false;
            return true;
        }

        /// <summary>
        /// 同じカードでない
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Card a, Card b) { return !(a == b); }

        /// <summary>
        /// カードの強弱比較。強さは Joker → 2 → 1 → 13 → 12 → ... → 3。
        /// 同じ数字ならスートの順。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator <(Card a, Card b) { return a.CompareTo(b) < 0; }
        public static bool operator >(Card a, Card b) { return a.CompareTo(b) > 0; }
        public static bool operator <=(Card a, Card b) { return a.CompareTo(b) <= 0; }
        public static bool operator >=(Card a, Card b) { return a.CompareTo(b) >= 0; }


        public int CompareTo(object obj)
        {
            var other = obj as Card;
            if (other == null) throw new ArgumentException("Argument is not Card");
            return CompareTo(other);
        }

        public int CompareTo(Card other)
        {
            // 同じなら0
            if (this == other) return 0;
            // ジョーカーなら最強
            if (this.suit == Suit.JKR) return (other.num + 10) % 13;
            if (other.suit == Suit.JKR) return (this.num + 10) % 13 - 13;
            // 数字で比較（3→2）
            var anum = (this.num + 10) % 13;
            var bnum = (other.num + 10) % 13;
            if (anum != bnum) return anum - bnum;
            // スートで比較
            return (int)this.suit - (int)other.suit;
        }
    }
}
