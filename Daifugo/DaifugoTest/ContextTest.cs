using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo;
using Daifugo.Cards;
using Daifugo.Observers;
using System.Diagnostics;
using Daifugo.Bases;
using Daifugo.Rules;
using Daifugo.GameImples;

namespace DaifugoTest
{
    [TestClass]
    public class ContextTest
    {
        TestPlayer p1 = new TestPlayer("test1");
        TestPlayer p2 = new TestPlayer("test2");

        [TestMethod]
        public void Context_Init()
        {
            var ctx = ContextFactory.CreateGameContext();

            var pctx1 = ContextFactory.CreatePlayerContext(p1, ctx);

            var pctx2 = ContextFactory.CreatePlayerContext(p2, ctx);

            // ゲームコンテキストの内容チェック
            Assert.AreEqual(0, ctx.Ba.Count());
            Assert.AreEqual(false, ctx.IsKakumei);
            Assert.AreEqual(0, ctx.Teban);
            Assert.AreEqual(2, ctx.PlayerInfo.Count());
            // プレイヤー情報1
            var pi = ctx.PlayerInfo.First();
            Assert.AreEqual(p1.Name, pi.Name);
            Assert.AreEqual(0, pi.HavingCardCount);
            Assert.AreEqual(PlayerRank.HEIMIN, pi.Ranking);
            // プレイヤー情報2
            pi = ctx.PlayerInfo.Last();
            Assert.AreEqual(p2.Name, pi.Name);
            Assert.AreEqual(0, pi.HavingCardCount);
            Assert.AreEqual(PlayerRank.HEIMIN, pi.Ranking);
            
        }

        [TestMethod]
        public void Context_Nagere()
        {
            var ctx = new GameContext(RuleFactory.GetDefaultRule());

            // 最初は空
            Assert.IsFalse(ctx.Yama.Any());
            Assert.IsFalse(ctx.Ba.Any());

            // 場にカードを置く
            ctx._history.Add(new HE_PutCards(ctx.Teban, DeckGenerator.FromCardsetString("HK").ToArray()));
            Assert.IsTrue(ctx.Ba.Any());
            Assert.IsFalse(ctx.Yama.Any());

            // 流す
            //ctx.DoNagare();
            ctx._history.Add(new HE_Nagare());
            Assert.AreEqual(1, ctx.Yama.Count());
            Assert.AreEqual(0, ctx.Ba.Count());

            // 場にカードを置く（複数枚、複数回）
            ctx._history.Add(new HE_PutCards(ctx.Teban,DeckGenerator.FromCardsetString("D3 H3").ToArray()));
            ctx._history.Add(new HE_PutCards(ctx.Teban,DeckGenerator.FromCardsetString("S5 C5").ToArray()));
            Assert.AreEqual(1, ctx.Yama.Count());
            Assert.AreEqual(2, ctx.Ba.Count());

            // 流す
            //ctx.DoNagare();
            ctx._history.Add(new HE_Nagare());
            Assert.AreEqual(5, ctx.Yama.Count());
            Assert.AreEqual(0, ctx.Ba.Count());

            // リセット
            ctx.Reset();
            Assert.AreEqual(0, ctx.Yama.Count());
            Assert.AreEqual(0, ctx.Ba.Count());
        }

        [TestMethod]
        public void PContext_PlayerInfo()
        {
            var ctx = (BaseGameContext)ContextFactory.CreateGameContext();

            var pctx1 = new PlayerContext(p1, ctx);

            // プレイヤー情報1
            var pi = ctx.PlayerInfo.First();
            Assert.AreEqual(p1.Name, pi.Name);
            Assert.AreEqual(0, pi.HavingCardCount);
            Assert.AreEqual(PlayerRank.HEIMIN, pi.Ranking);
            //Assert.AreEqual(false, pi.LastIsPass);
            Assert.AreEqual(0, pi.OrderOfFinish);
            
            // プレイヤーコンテキストをいじる
            //pctx1.LastIsPass = true;
            pctx1._deck.Add(new Card(Suit.SPD, 1));
            //Assert.AreEqual(true, pi.LastIsPass);
            Assert.AreEqual(1, pi.HavingCardCount);
            Assert.AreEqual("SA", pctx1.Deck.First().ToString());

            // さらにいじる
            //pctx1.LastIsPass = false;
            pctx1.OrderOfFinish = 1;
            pctx1.Ranking = PlayerRank.DAIFUGO;
            pctx1._deck.Clear();
            //Assert.AreEqual(false, pi.LastIsPass);
            Assert.AreEqual(0, pi.HavingCardCount);
            Assert.AreEqual(1, pi.OrderOfFinish);
            Assert.AreEqual(PlayerRank.DAIFUGO, pi.Ranking);

            // リセット
            pctx1.Reset();
            Assert.AreEqual(p1.Name, pi.Name);
            Assert.AreEqual(0, pi.HavingCardCount);
            Assert.AreEqual(PlayerRank.DAIFUGO, pi.Ranking);
            //Assert.AreEqual(false, pi.LastIsPass);
            Assert.AreEqual(0, pi.OrderOfFinish);
        }
    }
}
