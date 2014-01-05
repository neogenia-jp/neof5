using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Bases
{
	public interface IGameEventListener {
        void bindEvents(GameEvents evt);
        void unbindEvents(GameEvents evt);
    }

    public class GameEvents
    {
        public delegate IObserverContext getcontext(IGameEventListener o);
        public delegate void gameevent(getcontext ctx);

        /// <summary>
        /// トリック開始通知
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent start;

        /// <summary>
        /// 手札カード（の一部）が配布された
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent cardDistributed;

        /// <summary>
        /// 手札カードが交換された
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent cardSwapped;

        /// <summary>
        ///  ほかのプレイヤーに手番がまわった
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent thinking;

        /// <summary>
        /// 手番の人がカードを捨てた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        public event gameevent cardsArePut;

        /// <summary>
        /// 革命がおこった
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent kakumei;

        /// <summary>
        /// 流れた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        public event gameevent nagare;

        /// <summary>
        /// 手番の人が上がった
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent agari;

        /// <summary>
        /// トリック終了通知
        /// </summary>
        /// <param name="ctx"></param>
        public event gameevent finish;


        /// <summary>
        /// トリック開始通知
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void Start(getcontext ctx) { if(start!=null) start(ctx); }

        protected virtual void CardDistributed(getcontext ctx) { if(cardDistributed!=null) cardDistributed(ctx); }

        protected virtual void CardSwapped(getcontext ctx) { if(cardSwapped!=null) cardSwapped(ctx); }

        protected virtual void Thinking(getcontext ctx) { if(thinking!=null) thinking(ctx); }

        /// <summary>
        /// 手番の人がカードを捨てた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        protected virtual void CardsArePut(getcontext ctx) { if(cardsArePut!=null) cardsArePut(ctx); }

        /// <summary>
        /// 革命がおこった
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void Kakumei(getcontext ctx) { if(kakumei!=null) kakumei(ctx); }

        /// <summary>
        /// 流れた
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void Nagare(getcontext ctx) { if(nagare!=null) nagare(ctx); }

        /// <summary>
        /// 手番の人が上がった
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void Agari(getcontext ctx) { if(agari!=null) agari(ctx); }

        /// <summary>
        /// トリック終了通知
        /// </summary>
        /// <param name="ctx"></param>
        protected virtual void Finish(getcontext ctx) { if(finish!=null) finish(ctx); }

    }
}
