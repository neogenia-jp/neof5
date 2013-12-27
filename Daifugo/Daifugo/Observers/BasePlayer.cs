using Daifugo.Cards;
using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Players
{
    public abstract class BasePlayer : IGamePlayer
    {
        public string Name { get; protected set; }
        
        private Action<IEnumerable<Card>> _putCardCallback;

        public BasePlayer(string name, Action<IEnumerable<Card>> putCardCallback) { 
            Name = name;
            _putCardCallback = putCardCallback;
        }

        ///// <summary>
        ///// トリック開始通知
        ///// </summary>
        ///// <param name="ctx"></param>
        //public virtual void Start(IPlayerContext ctx) { }

        //public virtual void CardDistributed(IPlayerContext ctx){}

        //public virtual void CardSwapped(IPlayerContext ctx){}

        //public virtual void Thinking(IPlayerContext ctx){}

        /// <summary>
        /// 手番がまわってきた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        public virtual void ProcessTurn(IPlayerContext ctx) { _putCardCallback(_TurnCame(ctx)); }

        protected abstract IEnumerable<Card> _TurnCame(IPlayerContext ctx);

        ///// <summary>
        ///// 手番の人がカードを捨てた
        ///// </summary>
        ///// <param name="ctx"></param>
        ///// <param name="cards"></param>
        //public virtual void CardsArePut(IPlayerContext ctx) { }

        ///// <summary>
        ///// 革命がおこった
        ///// </summary>
        ///// <param name="ctx"></param>
        //public virtual void Kakumei(IPlayerContext ctx) { }

        ///// <summary>
        ///// 流れた
        ///// </summary>
        ///// <param name="ctx"></param>
        //public virtual void Nagare(IPlayerContext ctx) { }
        
        ///// <summary>
        ///// 手番の人が上がった
        ///// </summary>
        ///// <param name="ctx"></param>
        //public virtual void Agari(IPlayerContext ctx) { }

        ///// <summary>
        ///// トリック終了通知
        ///// </summary>
        ///// <param name="ctx"></param>
        //public virtual void Finish(IPlayerContext ctx) { }


        public abstract void BindEvents(GameEvents evt);
    }
}
