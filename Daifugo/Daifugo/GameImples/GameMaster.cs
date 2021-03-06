﻿using Daifugo.Cards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daifugo.Utils;
using Daifugo.Rules;
using Daifugo.Bases;
using Daifugo.Observers;

namespace Daifugo.GameImples
{
    public class GameMaster : GameEvents, IDisposable
    {
        public int wait_msec = 10;
        private Random rand = new Random();

        GameContext context;
        readonly IDictionary<IGamePlayer, PlayerContext> playerContexts = new Dictionary<IGamePlayer, PlayerContext>();

        readonly List<IGameEventListener> observers = new List<IGameEventListener>();
        public IEnumerable<IGameEventListener> Observers { get { foreach (var o in observers) yield return o; } }

        readonly IMonitorContext monitorCtx = null;
        public bool IsPlaing { get; private set; }

        public ICardDistributer CardDistributer { get; set; }

        //GameEvents eventTable = new GameEvents();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context"></param>
        internal GameMaster(GameContext context, ICardDistributer cardDistributer=null)
        {
            this.context = context;
            monitorCtx = new BaseMonitorContext(context, _getAllDeck);
            IsPlaing = false;
            CardDistributer = cardDistributer ?? new DefaultCardDistributer();
        }

        public void Dispose()
        {
            playerContexts.Clear();
            observers.Clear();
        }

        /// <summary>
        /// 全員の手札を返す（モニター用）
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IEnumerable<Card>> _getAllDeck()
        {
            return playerContexts.Values.Select(pc => pc._deck);
        }

        public int NumOfPlayers { get { return playerContexts.Keys.Count; } }

        public void AddPlayer(IGamePlayer p)
        {
            lock (this)
            {
                playerContexts[p] = new PlayerContext(p, context);
                p.BindEvents(this);
            }
        }

        public void AddObserver(IGameEventListener o) { observers.Add(o); o.bindEvents(this); }
        public void RemoveObsrver(IGameEventListener o) { observers.Remove(o); o.unbindEvents(this); }

        private void _broadCastForPlayers(Action<IGamePlayer, IPlayerContext> action)
        {
            lock (this)
            {
                context._players.ForEach(p => action(p, playerContexts[p]));
            }
        }

        private void _broadCastForMonitors(Action<IGameEventListener, IMonitorContext> action)
        {
            observers.ForEach(o=>action(o, monitorCtx));
        }

        public IObserverContext _getContext(IGameEventListener o)
        {
            if (o==null) throw new ArgumentNullException();
            if (o is IGamePlayer) return playerContexts[(IGamePlayer)o];
            else if (o is IGameEventListener) return monitorCtx;
            throw new ArgumentException("Specified GameListener type is unknonw: " + o.GetType());
        }

        private void _broadcast(Action<GameEvents,GameEvents.getcontext> action)
        {
            action(this, _getContext);
        }

        public void Start()
        {
            IsPlaing = true;
            context.Reset();
            foreach (var c in playerContexts.Values) c.Reset();

            // 開始通知
            Start(_getContext);
            //_broadCastForPlayers((p, pc) => p.Start(pc));

            // カードを生成し、配布する
            {
                var ret = CardDistributer.Distribute(context.PlayerInfo, DeckGenerator.DeckWithJoker());

				// 1枚ずつカードを手札に移し替え、5枚配るごとに通知する。
                while (ret.Any(l => l.Count() > 0))
                {
                    for (int i = 0; i < ret.Length; i++)
                    {
                        var pi = context.PlayerInfo.ElementAt(i);
                        var l = ret[i];
                        if (l.Count <= 0) continue;
                        var pc = playerContexts.ElementAt(i).Value;
                        pc._deck.Add(l[0]);
                        l.RemoveAt(0);
                    }
                    if (playerContexts.ElementAt(0).Value._deck.Count % 5 == 0)
                    {
                    _wait();
                    CardDistributed(_getContext);
                    }
                }
            }

            // 最終的にカード配布通知。
            _wait();
            //_broadcast((evt, f) => evt.CardDistributed(f));
            CardDistributed(_getContext);
            //context._players.ForEach(px => px.CardDistributed(playerContexts[px]));

            {
				var tmp = new Dictionary<IPlayerInfo, IList<Card>>();
                for (int i = 0; i < context._players.Count; i++ )
                {
					var p = context._players[i];
                    var pi = context.PlayerInfo.ElementAt(i);
                    tmp.Add(pi, playerContexts[p]._deck);
                }
			
                // 手番を決める。
				context.Teban = context.Rule.GetFirstDealer(tmp);
				
                // カード交換
                _wait();
                if (CardDistributer.SwapCards(tmp))
                {
                    CardSwapped(_getContext);
                }
            }

            // 手札ソート
            foreach (var pc in playerContexts.Values) pc._deck.Sort();
            _wait();
            //_broadcast((evt, f) => evt.CardDistributed(f));
            CardDistributed(_getContext);
            //context._players.ForEach(px =>
            //{
            //    px.CardDistributed(playerContexts[px]);
            //});

            // 次の手を求める
            RunAsyncLater(() =>
            {
                //_broadcast((evt, f) => evt.Thinking(f));
                Thinking(_getContext);
                var pxc = context.GetCurrentPlayer();
                //context._players.ForEach(px => {if(px!=pxc) px.Thinking(playerContexts[px]);});
                pxc.ProcessTurn(playerContexts[pxc]);
            });
        }

        public bool SetRule(IRule rule)
        {
            if (IsPlaing) return false;
            context.SetRule(rule);
            return true;
        }

	/*
        /// <summary>
        /// 最初の親を決める
        /// </summary>
        /// <returns></returns>
        private int getFirstDealer()
        {
            int sp3 = -1;
            for (int i = 0; i < playerContexts.Count; i++)
            {
                if (playerContexts.ElementAt(i).Value.Ranking == PlayerRank.DAIHINMIN)
                {
                    // 大貧民がいれば、そのプレーヤが親。
                    return i;
                }
                if (playerContexts.ElementAt(i).Value._deck.Contains(new Card(Suit.SPD,3)))
                {
                    // スペードの3を持っている人
                    sp3 = i;
                }
            }
            return sp3;
        }

        /// <summary>
        /// プレイヤータイトル間でカードを交換する
        /// </summary>
        /// <param name="hight">上位タイトル</param>
        /// <param name="low">下位タイトル</param>
        /// <param name="num">枚数</param>
        private void cardSwapProcess(PlayerRank hight, PlayerRank low, int num)
        {
            var fugo = playerContexts.Where((pc) => pc.Value.Ranking == hight).ToArray();
            var hinmin = playerContexts.Where((pc) => pc.Value.Ranking == low).ToArray();
            for (int i = 0; i < fugo.Length; i++)
            {
                var pc_from = fugo.ElementAt(i).Value;
                var pc_to = hinmin.ElementAt(i).Value;
                while(num-- > 0) _swapCard(pc_from, pc_to);
                // 通知
                //_broadcast((evt, f) => evt.CardSwapped(f));
                CardSwapped(_getContext);
                //fugo.ElementAt(i).Key.CardSwapped(pc_from);
                //hinmin.ElementAt(i).Key.CardSwapped(pc_to);
            }
        }

        /// <summary>
        /// 手札の中から最強と最弱のカードを交換する
        /// </summary>
        /// <param name="pc_h"></param>
        /// <param name="pc_l"></param>
        internal static void _swapCard(PlayerContext pc_h, PlayerContext pc_l)
        {
            var card_w = pc_h._deck.Min();  // 最弱カード
            var card_s = pc_l._deck.Max();  // 最強カード
            pc_h._deck.Remove(card_w);
            pc_l._deck.Remove(card_s);
            pc_h._deck.Add(card_s);
            pc_l._deck.Add(card_w);
        }
	*/
        public ICheckResult PutCards(IGamePlayer player, IEnumerable<Card> cards)
        {
            lock (context)
            {
                Debug.WriteLine("GameMaster::PutCards [{0}] cards={1}", player.Name, cards == null ? "PASS" : cards.ToCardsetString());

                // プレイヤーの手番かどうかをチェック
                if (player != context.GetCurrentPlayer())
                {
                    return new CheckError("手番が違います。");
                }

                if (!playerContexts.ContainsKey(player))
                {
                    return new CheckError("コンテキストがありません");
                }

                var pcontext = playerContexts[player];
                var nextTeban = _getNextTeban();

                // パス？
                if (cards == null || !cards.Any())
                {
                    if (context.Ba.Count() == 0)
                    {
                        // 親なのにパスはだめ。
                        return new CheckError("親なのでパスできません。");
                    }

                    // ヒストリー記録
                    context._history.Add(new HE_Pass(context.Teban));

                    // カード配置通知
                    //_broadcast((evt, f) => evt.CardsArePut(f));
                    CardsArePut(_getContext);
                    //context._players.ForEach(p => p.CardsArePut(playerContexts[p]));

                    // 全員パス？
                    if (_isAllPass())
                    {
                        //   context.DoNagare();
                        context._history.Add(new HE_Nagare());
                        //foreach (var pc in playerContexts.Values) pc.LastIsPass = false;

                        //RunAsyncLater(() =>
                        //{
                        // 流れ通知
                        _wait();
                        //_broadcast((evt, f) => evt.Nagare(f));
                        Nagare(_getContext);
                        //context._players.ForEach(p => p.Nagare(playerContexts[p]));

                        //    // 次の人の手をまつ
                        //    context.Teban = context.LastPutPlayerNum;
                        //    var px = context.GetCurrentPlayer();
                        //    px.ProcessTurn(playerContexts[px]);
                        //});
                    }
                }
                else
                {

                    // パスでなければ持っているカードかチェック
                    if (!cards.Any(c => pcontext._deck.Contains(c)))
                    {
                        return new CheckError("手札にないカードを出そうとしています。");
                    }

                    // さらに出せるカードかどうかチェック
                    var ret = context.Rule.CheckPutCards(context, cards);
                    if (!(ret is CheckOK)) return ret;

                    // カード移動
                    foreach (var c in cards) pcontext._deck.Remove(c);
                    //context._ba.Add(cards.ToArray());
                    context._history.Add(new HE_PutCards(context.Teban, cards));
                    //context.LastPutPlayerNum = context.Teban;

                    // カード配置通知
                    //_broadcast((evt, f) => evt.CardsArePut(f));
                    CardsArePut(_getContext);
                    //context._players.ForEach(p => p.CardsArePut(playerContexts[p]));


                    // 革命判定
                    if (cards != null && cards.Count() >= 4)
                    {
                        context.IsKakumei = !context.IsKakumei;
                        _wait();
                        //_broadcast((evt, f) => evt.Kakumei(f));
                        Kakumei(_getContext);
                        //context._players.ForEach(p => p.Kakumei(playerContexts[p]));
                    }
                }

                // 以降は別スレッドで実行
                RunAsyncLater(() =>
                {

                    // 手札がなくなった？
                    if (pcontext._deck.Count == 0)
                    {
                        context._history.Add(new HE_Agari(context.Teban));
                        // あがりカード判定
                        var ret = context.Rule.CanAgari(context, cards);
                        if (ret is CheckOK)
                        {
                            int order = 1;
                            while (order <= playerContexts.Count && playerContexts.Values.Any(pc => pc.OrderOfFinish == order)) order++;
                            pcontext.OrderOfFinish = order;
                        }
                        else
                        {
                            int order = playerContexts.Count;
                            while (order >= 1 && playerContexts.Values.Any(pc => pc.OrderOfFinish == order)) order--;
                            pcontext.OrderOfFinish = order;
                        }
                        // あがり通知
                        //_broadcast((evt, f) => evt.Agari(f));
                        Agari(_getContext);
                        //context._players.ForEach(p => p.Agari(playerContexts[p]));
                    }

                    // 残り1人？
                    var cardHavingPlayers = playerContexts.Values.Where(pc => pc._deck.Any());
                    if (cardHavingPlayers.Count() <= 1)
                    {
                        // 最後の人の順位を設定
                        int order = 1;
                        while (order <= playerContexts.Count && playerContexts.Values.Any(pc => pc.OrderOfFinish == order)) order++;
                        cardHavingPlayers.First().OrderOfFinish = order;

                        // 階級を決める
                        var pcorder = playerContexts.Values.OrderBy(p => p.OrderOfFinish < 1 ? int.MaxValue : p.OrderOfFinish).ToArray();
                        _setRanking(pcorder);

                        // 決着通知
                        //_broadcast((evt, f) => evt.Finish(f));
                        Finish(_getContext);
                        //context._players.ForEach(p => p.Finish(playerContexts[p]));

                        IsPlaing = false;
                    }
                    else
                    {
                        // 手番を進める
                        do
                        {
                            context.Teban++;
                        } while (playerContexts[context._players[context.Teban]]._deck.Count <= 0);
                        //_broadcast((evt, f) => evt.Thinking(f));
                        Thinking(_getContext);
                        var pxc = context.GetCurrentPlayer();
                        //context._players.ForEach(px => {if(px!=pxc) px.Thinking(playerContexts[px]);});
                        pxc.ProcessTurn(playerContexts[pxc]);
                    }
                });
                return CheckResults.Ok;
            }
        } 

        internal int _getNextTeban()
        {
            var t = context.Teban;
            do
            {
                t++;
                if (t >= context._players.Count) t = 0;
            } while (playerContexts[context._players[t]]._deck.Count <= 0);
            return t;
        }

        internal bool _isAllPass()
        {
            var t = _getNextTeban();
            foreach (var he in context.History.Reverse())
            {
                if (he is HE_Nagare) break;
                if (he is HE_PlayerHand)
                {
                    if ((he as HE_PlayerHand).PlayerNum == t) return true;
                    if (he is HE_Pass)
                    {
                        continue;
                    }
                    break;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定された順番に、PlayerRank を付け直す
        /// </summary>
        /// <param name="pcorder">順番</param>
        internal static void _setRanking(PlayerContext[] pcorder)
        {
            int f = 0, l = pcorder.Length-1;
            pcorder[f++].Ranking = PlayerRank.DAIFUGO;  // １位が大富豪
            pcorder[l--].Ranking = PlayerRank.DAIHINMIN; // ビリが大貧民
            if (l-f>=1)
            {
                pcorder[f++].Ranking = PlayerRank.FUGO;  // 富豪
                pcorder[l--].Ranking = PlayerRank.HINMIN; // 貧民
            }
            while (f <= l)
            {
                pcorder[f++].Ranking = PlayerRank.HEIMIN; // 平民
            }
        }

        private void RunAsyncLater(Action a)
        {
            Task.Run(() =>
            {
                _wait();
                a();
            });
        }

        private void _wait()
        {
            System.Threading.Thread.Sleep(wait_msec);
        }
    }
}
