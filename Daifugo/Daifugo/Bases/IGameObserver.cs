using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
    public interface IGameObserver<T> : IGameEventListener where T: IObserverContext
    {
        /// <summary>
        /// トリック開始通知
        /// </summary>
        /// <param name="ctx"></param>
        void Start(T ctx);

        /// <summary>
        /// 手札カード（の一部）が配布された
        /// </summary>
        /// <param name="ctx"></param>
        void CardDistributed(T ctx);

        /// <summary>
        /// 手札カードが交換された
        /// </summary>
        /// <param name="ctx"></param>
        void CardSwapped(T ctx);

        /// <summary>
        ///  ほかのプレイヤーに手番がまわった
        /// </summary>
        /// <param name="ctx"></param>
        void Thinking(T ctx);

        /// <summary>
        /// 手番の人がカードを捨てた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        void CardsArePut(T ctx);

        /// <summary>
        /// 革命がおこった
        /// </summary>
        /// <param name="ctx"></param>
        void Kakumei(T ctx);

        /// <summary>
        /// 流れた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        void Nagare(T ctx);

        /// <summary>
        /// 手番の人が上がった
        /// </summary>
        /// <param name="ctx"></param>
        void Agari(T ctx);

        /// <summary>
        /// トリック終了通知
        /// </summary>
        /// <param name="ctx"></param>
        void Finish(T ctx);

    }

    public class GameEventBinder<T,Y> where T: class, Daifugo.Bases.IObserverContext where Y: IGameObserver<T>
    {
        protected Y _this;
        protected GameEvents evt;
        private static Dictionary<Y, GameEventBinder<T, Y>> pool = new Dictionary<Y, GameEventBinder<T, Y>>();

        private GameEventBinder(Y t, GameEvents e) { _this = t; evt = e; }
        public static GameEventBinder<T, Y> GetOrCreate(Y obj, GameEvents e)
        {
            if (!pool.ContainsKey(obj))
            {
                pool[obj] = new GameEventBinder<T,Y>(obj, e);
            }
            return pool[obj];
        }

        private void Start(GameEvents.getcontext ctx) { _this.Start(ctx(_this) as T); }

        private void Finish(GameEvents.getcontext ctx) { _this.Finish(ctx(_this) as T); }

        private void CardsArePut(GameEvents.getcontext ctx) { _this.CardsArePut(ctx(_this) as T); }

        private void Nagare(GameEvents.getcontext ctx) { _this.Nagare(ctx(_this) as T); }

        private void Agari(GameEvents.getcontext ctx) { _this.Agari(ctx(_this) as T); }

        private void Kakumei(GameEvents.getcontext ctx) { _this.Kakumei(ctx(_this) as T); }

        private void CardDistributed(GameEvents.getcontext ctx) { _this.CardDistributed(ctx(_this) as T); }

        private void CardSwapped(GameEvents.getcontext ctx) { _this.CardSwapped(ctx(_this) as T); }

        private void Thinking(GameEvents.getcontext ctx) { _this.Thinking(ctx(_this) as T); }

        public virtual void bindEvents()
        {
            evt.agari += Agari;
            evt.cardDistributed += CardDistributed;
            evt.cardsArePut += CardsArePut;
            evt.cardSwapped += CardSwapped;
            evt.finish += Finish;
            evt.kakumei += Kakumei;
            evt.nagare += Nagare;
            evt.start += Start;
            evt.thinking += Thinking;
        }

        public virtual void unbindEvents()
        {
            evt.agari -= Agari;
            evt.cardDistributed -= CardDistributed;
            evt.cardsArePut -= CardsArePut;
            evt.cardSwapped -= CardSwapped;
            evt.finish -= Finish;
            evt.kakumei -= Kakumei;
            evt.nagare -= Nagare;
            evt.start -= Start;
            evt.thinking -= Thinking;
            pool.Remove(_this);
        }
    }

}
