using Daifugo.Bases;
using Daifugo.Cards;
using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Players
{
    public class PoorPlayer : BasePlayer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="a"></param>
        public PoorPlayer(string name, Action<IEnumerable<Card>> a ) : base(name, a) {}

        public static PoorPlayer Create(string name, GameMaster gm)
        {
            PoorPlayer p = null;
            Action<IEnumerable<Card>> a = (c) => gm.PutCards(p, c);
            p = new PoorPlayer(name, a);
            return p;
        }

        /// <summary>
        /// 手番がまわってきた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        protected override IEnumerable<Card> _TurnCame(IPlayerContext ctx)
        {
            var tefuda = ctx.Deck.ToList();
            tefuda.Sort();
            if (ctx.GameContext.IsKakumei) tefuda.Reverse();
            int n = 1;
            if (ctx.GameContext.Ba.Any()) n = ctx.GameContext.Ba.Last().Count();

            
            // 弱いカードから順番に出す。
            for (int i = 0; i < tefuda.Count - (n - 1); i++)
            {
                var temp = tefuda.GetRange(i, n);
                ICheckResult result = ctx.GameContext.Rule.CheckPutCards(ctx.GameContext, temp);
                if (result is CheckOK)
                {
                    Debug.WriteLine(string.Format("{0} {1}", Name, temp.ToCardsetString()));
                    return temp;
                }
            }

            Debug.WriteLine(string.Format("{0} PASS", Name));
            return null; // 出せるカードがないのでパス
        }

        public override void BindEvents(GameEvents evt) { }
    }
}
