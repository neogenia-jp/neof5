using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo;
using Daifugo.Cards;
using Daifugo.Rules;
using Daifugo.Observers;
using Daifugo.Bases;
using Daifugo.GameImples;

namespace DaifugoTest
{

    [TestClass]
    public class SimplestRuleTest
    {
        GameContext _ctx;
        IPlayerContext _pctx1, _pctx2;
        SimplestRule _rule;

        [TestInitialize]
        public void init()
        {
            this._rule = new SimplestRule();

            _ctx = ContextFactory.CreateGameContext() as GameContext;

            var p1 = new TestPlayer("test1");
            _pctx1 = ContextFactory.CreatePlayerContext(p1, _ctx);

            var p2 = new TestPlayer("test2");
            _pctx2 = ContextFactory.CreatePlayerContext(p2, _ctx);
        }

        private void _assertCheckPutCardsIsOK(IEnumerable<Card> cards)
        {
            var ret = _rule.CheckPutCards(_ctx, cards);
            if (!(ret is CheckOK))
            {
                Assert.Fail("チェックエラー cards=" + cards.ToCardsetString());
            }
        }

        private void _assertCheckPutCardsIsNG(IEnumerable<Card> cards, ICheckResult result)
        {
            var ret = _rule.CheckPutCards(_ctx, cards);
            if (!(ret is CheckOK))
            {
                Assert.IsTrue(ret.Message.Contains(result.Message));
                return;
            }
            Assert.Fail("チェック通過 cards=" + cards.ToCardsetString());
        }

        [TestMethod]
        public void Rule_CompareCard_同じ強さ()
        {
            { // 同じカード
                Card c1 = new Card(Suit.CLB, 1);
                Card c2 = new Card(Suit.CLB, 1);
                Assert.AreEqual(0, _rule.CompareCards(c1, c2));
            }
            { // ジョーカーどうし
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.JKR, 0);
                Assert.AreEqual(0, _rule.CompareCards(c1, c2));
            }
            { // スートが違うが同じ数字
                Card c1 = new Card(Suit.SPD, 13);
                Card c2 = new Card(Suit.HRT, 13);
                Assert.AreEqual(0, _rule.CompareCards(c1, c2));
            }
        }

        [TestMethod]
        public void Rule_CompareCard_違う強さ()
        {
            { // 1 <=> 2
                Card c1 = new Card(Suit.CLB, 1);
                Card c2 = new Card(Suit.CLB, 2);
                Assert.AreEqual(-1, _rule.CompareCards(c1, c2));
            }
            { // 1 <=> 13
                Card c1 = new Card(Suit.CLB, 1);
                Card c2 = new Card(Suit.HRT, 13);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2));
            }
            { // 2 <=> 3
                Card c1 = new Card(Suit.SPD, 2);
                Card c2 = new Card(Suit.HRT, 3);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2));
            }
            { // JK <=> 2
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.HRT, 2);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2));
            }
            { // JK <=> 3
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.HRT, 3);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2));
            }
        }

        [TestMethod]
        public void Rule_CompareCard_違う強さ_革命時()
        {
            {  // 1 <=> 2
                Card c1 = new Card(Suit.CLB, 1);
                Card c2 = new Card(Suit.CLB, 2);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2, true));
            }
            { // 1 <=> 13
                Card c1 = new Card(Suit.CLB, 1);
                Card c2 = new Card(Suit.HRT, 13);
                Assert.AreEqual(-1, _rule.CompareCards(c1, c2, true));
            }
            { // 2 <=> 3
                Card c1 = new Card(Suit.SPD, 2);
                Card c2 = new Card(Suit.HRT, 3);
                Assert.AreEqual(-1, _rule.CompareCards(c1, c2, true));
            }
            { // JK <=> 2
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.HRT, 2);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2, true));
            }
            { // JK <=> 3
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.HRT, 3);
                Assert.AreEqual(1, _rule.CompareCards(c1, c2, true));
            }
        }

        [TestMethod]
        public void Rule_TryGetFirstCard_パス()
        {
            var cards = new List<Card>();
            Card firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(null, firstCard);
        }

        [TestMethod]
        public void Rule_TryGetFirstCard_1枚()
        {
            var cards = DeckGenerator.FromCardsetString("H1");
            Card firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.HRT, firstCard.suit);
            Assert.AreEqual(1, firstCard.num);

            cards = DeckGenerator.FromCardsetString("JK");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.JKR, firstCard.suit);
        }

        [TestMethod]
        public void Rule_TryGetFirstCard_2枚()
        {
            var cards = DeckGenerator.FromCardsetString("H1H2");
            Card firstCard = null;
            Assert.AreEqual(false, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(null, firstCard);

            cards = DeckGenerator.FromCardsetString("H1D1");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.HRT, firstCard.suit);
            Assert.AreEqual(1, firstCard.num);

            cards = DeckGenerator.FromCardsetString("JKC3");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.CLB, firstCard.suit);
            Assert.AreEqual(3, firstCard.num);
        }

        [TestMethod]
        public void Rule_TryGetFirstCard_3枚()
        {
            var cards = DeckGenerator.FromCardsetString("H1H2S2");
            Card firstCard = null;
            Assert.AreEqual(false, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(null, firstCard);

            cards = DeckGenerator.FromCardsetString("HKSKDK");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.HRT, firstCard.suit);
            Assert.AreEqual(13, firstCard.num);

            cards = DeckGenerator.FromCardsetString("S3JKC3");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.SPD, firstCard.suit);
            Assert.AreEqual(3, firstCard.num);
        }


        [TestMethod]
        public void Rule_TryGetFirstCard_4枚()
        {
            var cards = DeckGenerator.FromCardsetString("HQD2S2C2");
            Card firstCard = null;
            Assert.AreEqual(false, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(null, firstCard);

            cards = DeckGenerator.FromCardsetString("HJSJDJCJ");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.HRT, firstCard.suit);
            Assert.AreEqual(11, firstCard.num);

            cards = DeckGenerator.FromCardsetString("JKC0S0D0");
            firstCard = null;
            Assert.AreEqual(true, _rule.TryGetFirstCard(cards, ref firstCard));
            Assert.AreEqual(Suit.CLB, firstCard.suit);
            Assert.AreEqual(10, firstCard.num);
        }

        [TestMethod]
        public void Rule_CheckPut_場なし_パス()
        {
            _assertCheckPutCardsIsOK(new Card[] { });
        }

        [TestMethod]
        public void Rule_CheckPut_場なし_1枚(){
            _ctx._history.Clear();
            _assertCheckPutCardsIsOK( new[] { new Card(Suit.CLB, 1) });
        }

        [TestMethod]
        public void Rule_CheckPut_場なし_2枚()
        {
            _ctx._history.Clear();
            _assertCheckPutCardsIsOK(new[] { new Card(Suit.CLB, 1), new Card(Suit.SPD, 1) });
            _assertCheckPutCardsIsNG(new[] { new Card(Suit.CLB, 1), new Card(Suit.SPD, 2) }, SimplestRule.ERR_DIFFERENT_NUM);
        }

        [TestMethod]
        public void Rule_CheckPut_場なし_3枚()
        {
            _ctx._history.Clear();
            _assertCheckPutCardsIsOK(new[] { new Card(Suit.CLB, 13), new Card(Suit.SPD, 13), new Card(Suit.HRT, 13) });
            _assertCheckPutCardsIsNG(new[] { new Card(Suit.CLB, 13), new Card(Suit.SPD, 13), new Card(Suit.HRT, 12) },SimplestRule.ERR_DIFFERENT_NUM);
        }

        [TestMethod]
        public void Rule_CheckPut_場なし_4枚()
        {
            _ctx._history.Clear();
            _assertCheckPutCardsIsOK(new[] { new Card(Suit.CLB, 3), new Card(Suit.SPD, 3), new Card(Suit.HRT, 3), new Card(Suit.DIA, 3) });
            _assertCheckPutCardsIsNG(new[] { new Card(Suit.CLB, 3), new Card(Suit.SPD, 3), new Card(Suit.HRT, 4), new Card(Suit.DIA, 3) }, SimplestRule.ERR_DIFFERENT_NUM);
        }


        [TestMethod]
        public void Rule_CheckPut_場あり_パス()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.JKR,03).ToArray()));
            _assertCheckPutCardsIsOK(new Card[] { });
        }

        [TestMethod]
        public void Rule_CheckPut_場あり_1枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.HRT, 3).ToArray()));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 4));
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 3), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.JKR, 0));
        }

        [TestMethod]
        public void Rule_CheckPut_場あり_2枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.DIA, 4, Suit.SPD, 4).ToArray()));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 5, Suit.SPD, 5));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.JKR, 0, Suit.SPD, 5));  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 4, Suit.SPD, 4), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 5), SimplestRule.ERR_COUNT_DOESNT_MATCH);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.JKR, 0), SimplestRule.ERR_COUNT_DOESNT_MATCH);
        }

        [TestMethod]
        public void Rule_CheckPut_場あり_3枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.JKR, 0, Suit.SPD, 13, Suit.HRT, 13).ToArray()));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 1, Suit.SPD, 1, Suit.HRT, 1));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 1, Suit.JKR, 0, Suit.HRT, 1));  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 12, Suit.SPD, 12, Suit.HRT, 12), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 12, Suit.JKR, 0, Suit.HRT, 12), SimplestRule.ERR_NOT_STRONG);  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 1, Suit.SPD, 1), SimplestRule.ERR_COUNT_DOESNT_MATCH);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.JKR, 0), SimplestRule.ERR_COUNT_DOESNT_MATCH);
        }

        [TestMethod]
        public void Rule_CheckPut_場あり_4枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.DIA, 1, Suit.SPD, 1, Suit.JKR, 0, Suit.DIA, 1).ToArray()));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 2, Suit.SPD, 2, Suit.HRT, 2, Suit.DIA, 2));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 2, Suit.SPD, 2, Suit.HRT, 2, Suit.JKR, 0));  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 12, Suit.SPD, 12, Suit.HRT, 12, Suit.DIA, 12), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 12, Suit.JKR, 0, Suit.HRT, 12, Suit.DIA, 12), SimplestRule.ERR_NOT_STRONG);  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 2, Suit.SPD, 2), SimplestRule.ERR_COUNT_DOESNT_MATCH);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.JKR, 0), SimplestRule.ERR_COUNT_DOESNT_MATCH);
        }

        [TestMethod]
        public void Rule_CheckPut_場あり革命中_1枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.CLB, 5).ToArray()));
            _ctx.IsKakumei = true;
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 4));
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 5), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 6), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 2), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.JKR, 0));
        }

        [TestMethod]
        public void Rule_CheckPut_場あり革命中_2枚()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0,DeckGenerator.Generate(Suit.DIA, 2, Suit.SPD, 2).ToArray()));
            _ctx.IsKakumei = true;
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 1, Suit.SPD, 1));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 3, Suit.SPD, 3));
            _assertCheckPutCardsIsOK(DeckGenerator.Generate(Suit.CLB, 3, Suit.JKR, 0));  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 2, Suit.DIA, 2), SimplestRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 2, Suit.JKR, 2), SimplestRule.ERR_NOT_STRONG);  // ジョーカーまじり
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.CLB, 5), SimplestRule.ERR_COUNT_DOESNT_MATCH);
            _assertCheckPutCardsIsNG(DeckGenerator.Generate(Suit.JKR, 0), SimplestRule.ERR_COUNT_DOESNT_MATCH);
        }
    }
}
