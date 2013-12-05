using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daifugo.Cards;

namespace DaifugoTest
{
    [TestClass]
    public class CardTest
    {
        [TestMethod]
        public void Card_FullSet()
        {
            var r = DeckGenerator.DeckWithJoker();

            // 全体枚数
            Assert.AreEqual(53, r.Count());

            // スート毎の枚数
            var r_clv = from t1 in r where t1.suit == Suit.CLB select 1;
            Assert.AreEqual(13, r_clv.Count());
            var r_dia = from t1 in r where t1.suit == Suit.DIA select 1;
            Assert.AreEqual(13, r_dia.Count());
            var r_hrt = from t1 in r where t1.suit == Suit.HRT select 1;
            Assert.AreEqual(13, r_hrt.Count());
            var r_spd = from t1 in r where t1.suit == Suit.SPD select 1;
            Assert.AreEqual(13, r_spd.Count());
            // ジョーカー
            var r_jkr = from t1 in r where t1.suit == Suit.JKR select 1;
            Assert.AreEqual(1, r_jkr.Count());
        }

        [TestMethod]
        public void Card_ToString()
        {
            var r = DeckGenerator.DeckWithJoker();

            object[] tests = {
                                 Suit.CLB, 1, "CA",
                                 Suit.CLB, 5, "C5",
                                 Suit.DIA, 7, "D7",
                                 Suit.HRT, 10, "H0",
                                 Suit.HRT, 11, "HJ",
                                 Suit.SPD, 12, "SQ",
                                 Suit.SPD, 13, "SK",
                                 Suit.JKR, 0 , "JK",
                             };
            for (int i = 0; i < tests.Length; i += 3)
            {
                var x = from t1 in r where t1.suit.Equals(tests[i]) && ((int)t1.num).Equals(tests[i + 1]) select t1;

                Assert.AreEqual(1, x.Count(), "対象カードがみつからない：" + tests[i] + "," + tests[i + 1]);
                var x0 = x.First();
                Assert.AreEqual(tests[i + 2], x0.ToString());
            }
        }

        [TestMethod]
        public void Card_FromCardString()
        {
            var test = DeckGenerator.Generate(
                Suit.CLB, 5,
                Suit.DIA, 7,
                Suit.JKR, 0,
                Suit.SPD, 1,
                Suit.SPD, 13,
                Suit.HRT, 11,
                Suit.HRT, 12
                );
            string[] str = new[]{"C5","D7","JK","SA","SK","HJ","HQ"};
            
            for (int i = 0; i < str.Length; i++)
            {
                Assert.AreEqual(test.ElementAt(i),Card.FromString(str[i]));
            }
        }

        [TestMethod]
        public void Card_FromCardsetString()
        {
            var test = DeckGenerator.Generate(
                Suit.CLB, 5,
                Suit.DIA, 7,
                Suit.JKR, 0,
                Suit.SPD, 1,
                Suit.SPD, 13,
                Suit.HRT, 11,
                Suit.HRT, 12
                );
            string str = "CA C5 D7 JK SA SK HJ HQ";

            var ret = DeckGenerator.FromCardsetString(str, 2);

            Assert.AreEqual(test.Count(), ret.Count());

            for (int i = 0; i < test.Count(); i++)
            {
                Assert.AreEqual(test.ElementAt(i), ret.ElementAt(i));
            }

        }

        [TestMethod]
        public void Card_Equals()
        {
            {
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.JKR, 0);
                Assert.AreEqual(c1, c2);
            }
            {
                Card c1 = new Card(Suit.HRT, 1);
                Card c2 = new Card(Suit.DIA, 1);
                Assert.AreNotEqual(c1, c2);
            }
            {
                Card c1 = new Card(Suit.SPD, 12);
                Card c2 = new Card(Suit.SPD, 12);
                Assert.IsTrue(c1 == c2);
            }
            {
                Card c1 = new Card(Suit.CLB, 11);
                Card c2 = new Card(Suit.CLB, 10);
                Assert.IsTrue(c1 != c2);
            }
        }

        [TestMethod]
        public void Card_operator()
        {
            // 数字比較
            {
                Card c1 = new Card(Suit.CLB, 3);
                Card c2 = new Card(Suit.CLB, 4);
                Assert.IsTrue(c1 < c2);
                Assert.IsTrue(c1 <= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.CLB, 2);
                Card c2 = new Card(Suit.CLB, 3);
                Assert.IsTrue(c1 > c2);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.HRT, 13);
                Card c2 = new Card(Suit.HRT, 1);
                Assert.IsTrue(c1 < c2);
                Assert.IsTrue(c1 <= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.SPD, 11);
                Card c2 = new Card(Suit.SPD, 11);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 <= c2);
            }
            // ジョーカー最強
            {
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.CLB, 4);
                Assert.IsTrue(c1 > c2);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.JKR, 0);
                Card c2 = new Card(Suit.JKR, 0);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 <= c2);
            }
            // スート比較
            {
                Card c1 = new Card(Suit.HRT, 1);
                Card c2 = new Card(Suit.SPD, 1);
                Assert.IsTrue(c1 < c2);
                Assert.IsTrue(c1 <= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.DIA, 2);
                Card c2 = new Card(Suit.CLB, 2);
                Assert.IsTrue(c1 > c2);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 != c2);
            }
            // 混合
            {
                Card c1 = new Card(Suit.CLB, 2);
                Card c2 = new Card(Suit.SPD, 1);
                Assert.IsTrue(c1 > c2);
                Assert.IsTrue(c1 >= c2);
                Assert.IsTrue(c1 != c2);
            }
            {
                Card c1 = new Card(Suit.DIA, 3);
                Card c2 = new Card(Suit.HRT, 12);
                Assert.IsTrue(c1 < c2);
                Assert.IsTrue(c1 <= c2);
                Assert.IsTrue(c1 != c2);
            }
        }
    }
}
