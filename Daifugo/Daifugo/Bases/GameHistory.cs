using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Bases
{
    // ========== base classies ==========

    public abstract class GameHistoryEntity
    {
        public override string ToString() { return _toString(); }
        protected abstract string _toString();
    }

    public abstract class HE_PlayerHand : GameHistoryEntity
    {
        public int PlayerNum { get; protected set; }
    }


    // ========== implements ========== 

    public class HE_PutCards : HE_PlayerHand
    {
        public IEnumerable<Card> Cards { get; protected set; }
        public HE_PutCards(int playerNum, IEnumerable<Card> cards) { PlayerNum = playerNum; Cards = cards; }
        protected override string _toString() { return string.Format("{0}-[{1}]", PlayerNum, Cards.ToCardsetString()); }
    }

    public class HE_Pass : HE_PlayerHand
    {
        public HE_Pass(int playerNum) { PlayerNum = playerNum; }
        protected override string _toString() { return string.Format("{0}-PASS", PlayerNum); }
    }

    public class HE_Nagare : GameHistoryEntity
    {
        protected override string _toString() { return "/"; }
    }

    public class HE_Agari : GameHistoryEntity
    {
        public int PlayerNum { get; protected set; }
        public HE_Agari(int playerNum) { PlayerNum = playerNum; }
        protected override string _toString() { return string.Format("{0}-AGARI", PlayerNum); }
    }
}