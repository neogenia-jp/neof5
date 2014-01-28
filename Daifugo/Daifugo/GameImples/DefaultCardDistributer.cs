using Daifugo.Bases;
using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.GameImples
{
	using PlayerInfoType = IPlayerInfo;
    public class DefaultCardDistributer : ICardDistributer
    {
        private Random rand = new Random();

		/// <summary>
		/// プレイヤーの中からランダムに一人を選択する。ただし大貧民に近い方が選択確率が高い。
		/// </summary>
		/// <param name="players"></param>
		/// <returns></returns>
        private int _choisePlayerAsBiased(IEnumerable<PlayerInfoType> players, bool reverse=false)
        {
            var ranklist = Enum.GetValues(typeof(PlayerRank));
            var l = ranklist.Length;                          // 5
            var r = (int)Math.Sqrt(rand.Next(l * l));         // 5*5のルートを求める。r<5であるはず。
            if (reverse) r = l - 1 - r;                       // Enum.PlayerRank の逆順で選択する。
            var rank = (PlayerRank)ranklist.GetValue(r);
			
            // そのランクのプレイヤーを探す
			int i=0;
            foreach (var pi in players)
            {
                if (pi.Ranking == rank) return i; 
                i++;
            }
			// ランダム
            return rand.Next(players.Count());
        }

        public virtual IList<Card>[] Distribute(IEnumerable<PlayerInfoType> players, IEnumerable<Card> cardset)
        {
            var n = players.Count();
            var ret = new List<Card>[n];
            for (int i = 0; i < n; i++) ret[i] = new List<Card>();

            var cards = new List<Card>(cardset);

            // カードの偏り
            var forth = rand.Next(13*4)+1;  // 1/4の確率で4枚カードがかたよる。
            if (forth <= 13)
            {
				// 誰に偏るかを決定
                var i = _choisePlayerAsBiased(players, true);
				
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    var c = new Card(suit, forth);
                    cards.Remove(c);
                    ret[i].Add(c);
                }
                var forth2 = rand.Next(13 * 6) + 1;  // さらに1/6の確率で4枚カードがかたよる。
                if (forth2 <= 13 && forth2!=forth)
                {
					i = _choisePlayerAsBiased(players, false);
                    foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                    {
                        var c = new Card(suit, forth2);
                        cards.Remove(c);
						ret[i].Add(c);
                    }
                }
            }
			
			// 残りのカードを均等に割り当てる
            while (cards.Count > 0)
            {
                var r = rand.Next(cards.Count);
                var c = cards[r];
                cards.Remove(c);
                var min = ret.Min(l => l.Count);
                var xx = ret.Where(l => l.Count == min);
                xx.First().Add(c);
            }
            return ret;
        }

        public virtual bool SwapCards(IDictionary<PlayerInfoType, IList<Card>> playerCards)
        {
            bool ret = false;

            // 大富豪⇔大貧民 2枚
            ret |= _swapCards(playerCards, PlayerRank.DAIFUGO, PlayerRank.DAIHINMIN, 2);

            // 富豪⇔貧民 1枚
            ret |= _swapCards(playerCards, PlayerRank.FUGO, PlayerRank.HINMIN, 1);

            return ret;
        }
        
        internal static bool _swapCards(IDictionary<PlayerInfoType, IList<Card>> playerCards, PlayerRank hight, PlayerRank low, int num)
		{
            bool ret = false;
            var fugo = playerCards.Where((pc) => pc.Key.Ranking == hight).ToArray();
            var hinmin = playerCards.Where((pc) => pc.Key.Ranking == low).ToArray();
            for (int i = 0; i < fugo.Length; i++)
            {
                var pc_from = fugo.ElementAt(i).Value;
                var pc_to = hinmin.ElementAt(i).Value;
                _swap(pc_from, pc_to, num);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 手札の中から最強と最弱のカードを交換する
        /// </summary>
        /// <param name="pc_h"></param>
        /// <param name="pc_l"></param>
        internal static void _swap(IList<Card> pc_h, IList<Card> pc_l, int num)
        {
            while (num-- > 0)
            {
                var card_w = pc_h.Min();  // 最弱カード
                var card_s = pc_l.Max();  // 最強カード
                pc_h.Remove(card_w);
                pc_l.Remove(card_s);
                pc_h.Add(card_s);
                pc_l.Add(card_w);
            }
        }

    }
}
