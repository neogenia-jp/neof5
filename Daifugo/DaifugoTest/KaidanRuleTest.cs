using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo;
using Daifugo.Cards;
using Daifugo.Rules;
using Daifugo.Players;
using Daifugo.Bases;
using Daifugo.GameImples;

namespace DaifugoTest
{

    [TestClass]
    public class KaidanRuleTest
    {
        GameContext _ctx;
        IPlayerContext _pctx1, _pctx2;
        KaidanRule _rule;

        [TestInitialize]
        public void init()
        {
            this._rule = new KaidanRule();

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
        public void Rule_IsKaidan_IsOK()
        {
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C3 C4 C5")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C6 C4 C5")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("CK CQ CJ")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C2 CK")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 CQ CK")));
        }

        [TestMethod]
        public void Rule_IsKaidan_枚数不足()
        {
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C3 C4")));
        }

        [TestMethod]
        public void Rule_IsKaidan_スート違い()
        {
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C3 D4 C5")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("D6 C4 C5")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("CK CQ HJ")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("S1 C2 CK")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 CQ DK")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 JK DK")));
        }

        [TestMethod]
        public void Rule_IsKaidan_数字違い()
        {
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C3 C4 C6")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C6 C4 C7")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C2 CQ")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C2 C3")));
        }

        [TestMethod]
        public void Rule_IsKaidan_ジョーカー()
        {
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C3 C4 JK")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("JK C4 C6")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 JK CQ")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C2 JK")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C2 JK JK")));
            Assert.IsTrue(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 CJ JK JK")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C1 C3 JK")));
            Assert.IsFalse(KaidanRule.IsKaidan(DeckGenerator.FromCardsetString("C4 C2 JK")));
        }

        [TestMethod]
        public void Rule_PutCard_Kaidan_場なし()
        {
            _ctx._history.Clear();
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C3 C4 C5"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C6 C4 C5"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("CK CQ CJ"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C1 C2 CK"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C1 CQ CK"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C1 C2 JK CK"));
        }

        [TestMethod]
        public void Rule_PutCard_Kaidan_場が階段でないとき()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0, DeckGenerator.FromCardsetString("C4 D4 H4")));

            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C6 C7 C5"), KaidanRule.ERROR_BA_ISNOT_KAIDAN);
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C6 C7 C5 C8"), KaidanRule.ERROR_BA_ISNOT_KAIDAN);
        }

        [TestMethod]
        public void Rule_IsKaidan_場が階段のとき()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0, DeckGenerator.FromCardsetString("S5 S6 S7")));

            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("S8 S9 S0"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("D8 D9 D0"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("CK CQ CJ"));
            // ジョーカーを含むパターン
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C8 C9 JK"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("JK D9 D0"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("JK S2 SK"));
            // 階段でない場合
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C8 H8 S8"), KaidanRule.ERROR_NOT_KAIDAN);
            // カードの数字が強い数字でない場合
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C6 C7 C8"), KaidanRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C9 C7 C8"), KaidanRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("JK C7 C8"), KaidanRule.ERR_NOT_STRONG);
        }

        [TestMethod]
        public void Rule_IsKaidan_場が4枚でジョーカーを含む階段のとき()
        {
            _ctx._history.Clear();
            _ctx._history.Add(new HE_PutCards(0, DeckGenerator.FromCardsetString("S5 S6 S7 JK")));

            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("S8 S9 S0 SJ"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("D8 D9 D0 DJ"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("CK CQ CJ C1"));
            // ジョーカーを含むパターン
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("C8 C9 JK C0"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("JK D9 D0 D8"));
            _assertCheckPutCardsIsOK(DeckGenerator.FromCardsetString("JK S2 SK SQ"));
            // 階段でない場合
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C8 H8 S8 S8"), KaidanRule.ERROR_NOT_KAIDAN);
            // カードの数字が強い数字でない場合
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C6 C7 C8 C9"), KaidanRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("C9 C7 C8 C0"), KaidanRule.ERR_NOT_STRONG);
            _assertCheckPutCardsIsNG(DeckGenerator.FromCardsetString("JK C7 C8 C0"), KaidanRule.ERR_NOT_STRONG);
        }

        [TestMethod]
        public void Rule_CanAgari()
        {
            _ctx.IsKakumei = false;
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.CLB, 3)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.JKR, 0, Suit.DIA, 4)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.HRT, 1, Suit.JKR, 0, Suit.DIA, 1)));
            // 階段 
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.HRT, 2, Suit.HRT, 13, Suit.HRT, 1)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.DIA, 1, Suit.JKR, 0, Suit.DIA, 12, Suit.DIA, 2)));
            // ジョーカー
            Assert.AreEqual(KaidanRule.ERROR_JOKER_AGARI, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.JKR, 0)));
            Assert.AreEqual(KaidanRule.ERROR_JOKER_AGARI, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.JKR, 0, Suit.JKR, 0)));
            // ２あがり
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.HRT, 2)));
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.DIA, 2, Suit.SPD, 2)));
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.DIA, 2, Suit.JKR, 0, Suit.CLB, 2)));
        }
        
        [TestMethod]
        public void Rule_CanAgari_革命中()
        {
            _ctx.IsKakumei = true;
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.CLB, 2)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.JKR, 0, Suit.DIA, 1)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.HRT, 4, Suit.JKR, 0, Suit.DIA, 4)));
            // 階段 
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.HRT, 3, Suit.HRT, 4, Suit.HRT, 5)));
            Assert.AreEqual(CheckResults.Ok, _rule.CanAgari(_ctx,DeckGenerator.Generate(Suit.DIA, 5, Suit.JKR, 0, Suit.DIA, 4, Suit.DIA, 3)));
            // ジョーカー
            Assert.AreEqual(KaidanRule.ERROR_JOKER_AGARI, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.JKR, 0)));
            Assert.AreEqual(KaidanRule.ERROR_JOKER_AGARI, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.JKR, 0, Suit.JKR, 0)));
            // ２あがり
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.HRT, 3)));
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.DIA, 3, Suit.SPD, 3)));
            Assert.AreEqual(KaidanRule.ERROR_2_AGARI, _rule.CanAgari(_ctx, DeckGenerator.Generate(Suit.DIA, 3, Suit.JKR, 0, Suit.CLB, 3)));
        }
    }
}
