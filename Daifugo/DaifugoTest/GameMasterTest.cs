using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo;
using Daifugo.Bases;
using Daifugo.Cards;
using Daifugo.Rules;
using Daifugo.GameImples;
using Moq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DaifugoTest
{
    [TestClass]
    public class GameMasterTest
    {
        TestPlayer p1 = new TestPlayer("test1");
        TestPlayer p2 = new TestPlayer("test2");
        TestPlayer p3 = new TestPlayer("test3");
        TestPlayer p4 = new TestPlayer("test4");
        TestPlayer p5 = new TestPlayer("test5");
        TestPlayer p6 = new TestPlayer("test6");

        [TestMethod]
        public void GameMaster_SetRanking()
        {
            var ctx = ContextFactory.CreateGameContext();

            // 2人の場合
            var pc1 = (PlayerContext)ContextFactory.CreatePlayerContext(p1, ctx);
            var pc2 = (PlayerContext)ContextFactory.CreatePlayerContext(p2, ctx);
            {
                var pcorder = new[] { pc1, pc2 };
                GameMaster._setRanking(pcorder);
                Assert.AreEqual(PlayerRank.DAIFUGO, pc1.Ranking);
                Assert.AreEqual(PlayerRank.DAIHINMIN, pc2.Ranking);
            }
            // 3人の場合
            var pc3 = (PlayerContext)ContextFactory.CreatePlayerContext(p3, ctx);
            {
                var pcorder = new[] { pc3, pc2, pc1 };
                GameMaster._setRanking(pcorder);
                Assert.AreEqual(PlayerRank.DAIFUGO, pc3.Ranking);
                Assert.AreEqual(PlayerRank.HEIMIN, pc2.Ranking);
                Assert.AreEqual(PlayerRank.DAIHINMIN, pc1.Ranking);
            }
            // 4人の場合
            var pc4 = (PlayerContext)ContextFactory.CreatePlayerContext(p4, ctx);
            {
                var pcorder = new[] { pc2, pc1, pc4, pc3 };
                GameMaster._setRanking(pcorder);
                Assert.AreEqual(PlayerRank.DAIFUGO, pc2.Ranking);
                Assert.AreEqual(PlayerRank.FUGO, pc1.Ranking);
                Assert.AreEqual(PlayerRank.HINMIN, pc4.Ranking);
                Assert.AreEqual(PlayerRank.DAIHINMIN, pc3.Ranking);
            }
            // 5人の場合
            var pc5 = (PlayerContext)ContextFactory.CreatePlayerContext(p5, ctx);
            {
                var pcorder = new[] { pc5, pc2, pc1, pc3, pc4 };
                GameMaster._setRanking(pcorder);
                Assert.AreEqual(PlayerRank.DAIFUGO, pc5.Ranking);
                Assert.AreEqual(PlayerRank.FUGO, pc2.Ranking);
                Assert.AreEqual(PlayerRank.HEIMIN, pc1.Ranking);
                Assert.AreEqual(PlayerRank.HINMIN, pc3.Ranking);
                Assert.AreEqual(PlayerRank.DAIHINMIN, pc4.Ranking);
            }
            // 6人の場合
            var pc6 = (PlayerContext)ContextFactory.CreatePlayerContext(p6, ctx);
            {
                var pcorder = new[] { pc3, pc4, pc5, pc2, pc1,pc6 };
                GameMaster._setRanking(pcorder);
                Assert.AreEqual(PlayerRank.DAIFUGO, pc3.Ranking);
                Assert.AreEqual(PlayerRank.FUGO, pc4.Ranking);
                Assert.AreEqual(PlayerRank.HEIMIN, pc5.Ranking);
                Assert.AreEqual(PlayerRank.HEIMIN, pc2.Ranking);
                Assert.AreEqual(PlayerRank.HINMIN, pc1.Ranking);
                Assert.AreEqual(PlayerRank.DAIHINMIN, pc6.Ranking);
            }
        }

        [TestMethod]
        public void CardDistributer_swapCards()
        {
            // 手札をセット
			var list1 = DeckGenerator.FromCardsetString("H1 D3 DK");
			var list2 = DeckGenerator.FromCardsetString("C0 SQ H4");
            
            // D3 ⇔ SQ
            DefaultCardDistributer._swap(list1, list2, 1);

            // 枚数チェック
            Assert.AreEqual(3, list2.Count());
            Assert.AreEqual(3, list2.Count());

            // スワップされたカードの所在確認
            Assert.IsFalse(list1.Contains(new Card(Suit.DIA, 3)));
            Assert.IsTrue(list1.Contains(new Card(Suit.SPD, 12)));
            Assert.IsFalse(list2.Contains(new Card(Suit.SPD, 12)));
            Assert.IsTrue(list2.Contains(new Card(Suit.DIA, 3)));
        }

        [TestMethod]
        public void CardDistributer_swapCards_Joker()
        {
            // 手札をセット
            var pc1 = DeckGenerator.FromCardsetString("H2 S4 D4 DK C4 H5 S6 S7 H7");
            var pc2 = DeckGenerator.FromCardsetString("C0 H9 S6 S2 SQ SJ JK S1 H3");

            // D4 ⇔ JK
            DefaultCardDistributer._swap(pc1, pc2, 1);

            // 枚数チェック
            Assert.AreEqual(9, pc1.Count());
            Assert.AreEqual(9, pc2.Count());

            // スワップされたカードの所在確認
            Assert.IsFalse(pc1.Contains(new Card(Suit.CLB, 4)));
            Assert.IsTrue(pc1.Contains(new Card(Suit.JKR, 0)));
            Assert.IsFalse(pc2.Contains(new Card(Suit.JKR, 0)));
            Assert.IsTrue(pc2.Contains(new Card(Suit.CLB, 4)));
        }

        [TestMethod]
        public void GameMaster_Start()
        {
            GameMaster gm = new GameMaster((GameContext)ContextFactory.CreateGameContext());
            gm.AddPlayer(p1);
            gm.AddPlayer(p2);

            object[] testCallbacks = {           
                p1, "Start",0,0,
                p2, "Start", 0, 0,
                p1, "CardDistributed", 5, 0,
                p2, "CardDistributed", 5, 0,
                p1, "CardDistributed", 10, 0,
                p2, "CardDistributed", 10, 0,
                p1, "CardDistributed", 15, 0,
                p2, "CardDistributed", 15, 0,
                p1, "CardDistributed", 20, 0,
                p2, "CardDistributed", 20, 0,
                p1, "CardDistributed", 25, 0,
                p2, "CardDistributed", 25, 0,
                p1, "CardDistributed", 27, 0,
                p2, "CardDistributed", 26, 0,
                p1, "CardDistributed", 27, 0,
                p2, "CardDistributed", 26, 0,
            };
            int p = 0;
            
            p1.callback = (player, methodName, pctx) => {
                Debug.WriteLine("[{0}] {1} cards={2}", player.Name, methodName, pctx.Deck.ToCardsetString());
                Assert.AreEqual(testCallbacks[p++], player);
                Assert.AreEqual(testCallbacks[p++], methodName);
                Assert.AreEqual(testCallbacks[p++], pctx.Deck.Count());
                Assert.AreEqual(testCallbacks[p++], pctx.GameContext.Ba.Count());
            };

            p2.callback = p1.callback;
        
            // スタート
            gm.Start();

        }

        [TestMethod]
        public void GameMaster_CardSwap()
        {
            GameContext ctx = (GameContext)ContextFactory.CreateGameContext();
            GameMaster gm = new GameMaster(ctx);
            gm.AddPlayer(p1);
            gm.AddPlayer(p2);

            object[] testCallbacks = {     
                p1, "Start", 0, 0,
                p2, "Start", 0, 0,
                p2, "CardSwapped", 26, 0,
                p1, "CardSwapped", 27, 0,
            };

            int p = 0;
            int count = 0;
            string lastDeck1 = "";
            string lastDeck2 = "";
            p1.callback = p2.callback = (player, methodName, pctx) =>
            {
                count++;
                var n = player.Name;
                Debug.WriteLine("{3}, [{0}] {1} cards={2}", n, methodName, pctx.Deck.ToCardsetString(), count);

                if (count <= 2)
                {
                    if (n == p1.Name)
                    {
                        // プレイヤー1を大貧民にする
                        var pctxx = pctx as PlayerContext;
                        pctxx.Ranking = PlayerRank.DAIHINMIN;
                    }
                    else
                    {
                        // プレイヤー2を大富豪にする
                        var pctxx = pctx as PlayerContext;
                        pctxx.Ranking = PlayerRank.DAIFUGO;
                    }
                }


                if (methodName == "CardDistributed")
                {
                    if (n == p1.Name) lastDeck1 = pctx.Deck.ToCardsetString();
                    else lastDeck2 = pctx.Deck.ToCardsetString();
                    return;
                }

                if (p < testCallbacks.Length)
                {
                    p++;//Assert.AreEqual(testCallbacks[p++], player);
                    Assert.AreEqual(testCallbacks[p++], methodName);
                    p++;// Assert.AreEqual(testCallbacks[p++], pctx.Deck.Count());
                    Assert.AreEqual(testCallbacks[p++], pctx.GameContext.Ba.Count());
                }

                if (methodName == "CardSwapped")
                {
                    Assert.AreNotEqual((n == p1.Name) ? lastDeck1 : lastDeck2, pctx.Deck.ToCardsetString());
                }

            };

            // スタート
            gm.Start();
        }

        [TestMethod]
        public void GameMaster_getFirstDealer()
        {
            GameContext ctx = (GameContext)ContextFactory.CreateGameContext();
            GameMaster gm = new GameMaster(ctx);
            gm.AddPlayer(p1);
            gm.AddPlayer(p2);

            p1.callback = p2.callback = (player, methodName, pctx) =>
            {
                if (methodName == "ProcessTurn")
                {
                    Assert.IsTrue(pctx.Deck.ToCardsetString().Contains("S3"));
                }
            };

            p2.callback = p1.callback;

            // スタート
            gm.Start();
        }

        [TestMethod]
        public void GameMaster_PutCard_Nagare(){
            var ctx = (GameContext)ContextFactory.CreateGameContext();
            GameMaster gm = new GameMaster(ctx);
            gm.AddPlayer(p1);
            gm.AddPlayer(p2);

            string[] testCallbacks = {     
                "ProcessTurn",   // 手番のプレイヤーがカードを出す
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "ProcessTurn",   // 手番のプレイヤーがカードを出す
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "ProcessTurn", 
            };

            int p = 0;
            int count = 0;
            object checkPlayer = null;
            object checkTeban=null;
            p1.callback = p2.callback = (player, mn, pctx) =>
            {
                lock (testCallbacks)
                {
                    count++;
                    var n = player.Name;
                    Debug.WriteLine("{3}, [{0}] {1} cards={2}", n, mn, pctx.Deck.ToCardsetString(), count);

                    if (mn == "CardDistributed" || mn == "CardSwapped" || mn == "Start") return;

                    Assert.AreEqual(testCallbacks[p++], mn);

                    if (mn == "ProcessTurn")
                    {
                        if (p == 1)
                        {
                            gm.PutCards(player, new[] { pctx.Deck.ElementAt(0) });
                            checkTeban = ctx.Teban;
                            checkPlayer = player;
                        }
                        else if (p == 5)
                        {
                            Assert.AreEqual(checkPlayer, player);
                            Assert.AreEqual(checkTeban, ctx.Teban);
                            gm.PutCards(player, new[] { pctx.Deck.ElementAt(0) });
                            checkTeban = ctx.Teban;
                            checkPlayer = player;
                        }
                        else
                        {
                            Assert.AreEqual(1, pctx.GameContext.Yama);
                            gm.PutCards(player, null);
                        }
                    }

                    if (mn == "Nagare")
                    {
                        Assert.AreEqual(0, pctx.GameContext.Yama);
                    }
                }
            };

            p2.callback = p1.callback;

            // スタート
            gm.Start();
        }
        
        [TestMethod]
        public void GameMaster_PutCard_Nagare3(){
            var ctx = (GameContext)ContextFactory.CreateGameContext();
            GameMaster gm = new GameMaster(ctx);
            gm.AddPlayer(p2);
            gm.AddPlayer(p1);
            gm.AddPlayer(p3);

            string[] testCallbacks = { 
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 手番のプレイヤーがカードを出す
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 手番のプレイヤーがカードを出す
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn",   // 次のプレイヤーがパスする
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "Nagare",        // 流れ通知がくる
                "Thinking",
                "Thinking",
                "Thinking",
                "ProcessTurn", 
            };

            bool complete = false;
            int p = 0;
            int count = 0;
            object checkPlayer = null;  // 流れた後、手番がまわることになるプレイヤー
            object checkTeban = null;  // 流れた後の手番
            p1.callback = p2.callback = p3.callback = (player, mn, pctx) =>
            {
                lock (testCallbacks) try
                    {
                        count++;
                        var n = player.Name;
                        Debug.WriteLine("{3}, [{0}] {1} cards={2}", n, mn, pctx.Deck.ToCardsetString(), count);

                        if (mn == "CardDistributed" || mn == "CardSwapped" || mn == "Start" || mn == "CardsArePut") return;

                        Assert.AreEqual(testCallbacks[p++], mn);

                        if (mn == "ProcessTurn")
                        {
                            if (p == 4)
                            {
                                gm.PutCards(player, new[] { pctx.Deck.ElementAt(0) });
                                checkTeban = ctx.Teban;
                                checkPlayer = player;
                            }
                            else if (p == 19 || p==34)
                            {
                                Assert.AreEqual(checkTeban, ctx.Teban);
                                Assert.AreEqual(checkPlayer, player);
                                gm.PutCards(player, new[] { pctx.Deck.ElementAt(0) });
                                checkTeban = ctx.Teban;
                                checkPlayer = player;
                            }
                            else
                            {
                                var cx = pctx.GameContext.Ba;
                                Assert.AreEqual(1, cx.Count());
                                gm.PutCards(player, null);
                            }
                        }

                        if (mn == "Nagare")
                        {
                            var cx = pctx.GameContext.Ba;
                            Assert.AreEqual(0, cx.Count());
                        }
                        if (p == testCallbacks.Count())
                        {
                            // 完走した。
                            Debug.WriteLine("Test Complete. p=" + p);
                            complete = true;
                        }

                    }
                    catch (Exception e) { Debug.WriteLine(e); throw; }

            };

            // スタート
            gm.Start();
            
            for (int i=0;i<10 && !complete;i++) { System.Threading.Thread.Sleep(500); } 
            Assert.IsTrue(complete);
        }

        [TestMethod]
        public void GameMaster_PutCard_p1win()
        {
            var ctx = (GameContext)ContextFactory.CreateGameContext();
            GameMaster gm = new GameMaster(ctx) { wait_msec = 0 };
            gm.AddPlayer(p1);
            gm.AddPlayer(p2);

            int checkTeban = -1;

            // 手札を切る関数
            Action<IGamePlayer, IPlayerContext>[] procs =
            {
                (player, pctx)=>{checkTeban=pctx.GameContext.Teban;gm.PutCards(player, new[] { pctx.Deck.ElementAt(0) });},
                (player, pctx)=>{gm.PutCards(player, null);},
            };

            string[] testCallbacks = {     
                "Agari", 
                "Agari", 
                "Finish",
                "Finish", 
            };

            bool complete = false;

            int p = 0,procp=0;
            int count = 0;
            p1.callback = p2.callback = (player, mn, pctx) =>
            {
                var n = player.Name;
                Debug.WriteLine("{3}, [{0}] {1} cards={2}", n, mn, pctx.Deck.ToCardsetString(), ++count);
                lock (gm) try
                    {

                        if (mn == "CardDistributed" || mn == "CardSwapped" || mn == "Start" 
                            || mn == "Nagare" || mn == "CardsArePut" || mn == "Thinking") return;

                        if (mn == "ProcessTurn")
                        {
                            procs[procp++ % procs.Length](player, pctx);
                            return;
                        }
                        else if (mn == "Nagare")
                        {
                            Assert.AreEqual(0, pctx.GameContext.Yama);
                            Assert.AreEqual(checkTeban, pctx.GameContext.Teban);
                            //Assert.AreEqual(checkTeban, pctx.GameContext.LastPutPlayerNum);
                            return;
                        }

                        Assert.AreEqual(testCallbacks[p++], mn);
                        Assert.AreEqual(checkTeban, pctx.GameContext.Teban);
                        //Assert.AreEqual(checkTeban, pctx.GameContext.LastPutPlayerNum);

                        if (mn == "Finish")
                        {
                            for (int i = 0; i < ctx.PlayerInfo.Count(); i++)
                            {
                                if (i == checkTeban)
                                {
                                    Assert.AreEqual(1, ctx.PlayerInfo.ElementAt(i).OrderOfFinish);
                                    Assert.AreEqual(PlayerRank.DAIFUGO, ctx.PlayerInfo.ElementAt(i).Ranking);
                                }
                                else
                                {
                                    Assert.AreEqual(2, ctx.PlayerInfo.ElementAt(i).OrderOfFinish);
                                    Assert.AreEqual(PlayerRank.DAIHINMIN, ctx.PlayerInfo.ElementAt(i).Ranking);
                                }
                            }
                        }
                        if (p == testCallbacks.Count())
                        {
                            // 完走した。
                            Debug.WriteLine("Test Complete. p=" + p);
                            complete = true;
                        }
                    }
                    catch (Exception e) { Debug.WriteLine(e); throw; }
            };

            // スタート
            gm.Start();

            for (int i=0;i<10 && !complete;i++) { System.Threading.Thread.Sleep(500); } 
            Assert.IsTrue(complete);
            
        }
    }
}
