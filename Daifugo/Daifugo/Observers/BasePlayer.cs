using Daifugo.Cards;
using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.Observers
{
    using ContextType = IPlayerContext;

    public abstract class BasePlayer : IGamePlayer
    {
        public string Name { get; protected set; }
        
        private Action<IEnumerable<Card>> _putCardCallback;

        public BasePlayer(string name, Action<IEnumerable<Card>> putCardCallback) { 
            Name = name;
            _putCardCallback = putCardCallback;
        }

        /// <summary>
        /// 手番がまわってきた
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cards"></param>
        public virtual void ProcessTurn(ContextType ctx) { _putCardCallback(_TurnCame(ctx)); }

        protected abstract IEnumerable<Card> _TurnCame(ContextType ctx);

        public virtual void Start(ContextType ctx){}

        public virtual void CardDistributed(ContextType ctx) { }

        public virtual void CardSwapped(ContextType ctx) { }

        public virtual void Thinking(ContextType ctx) { }

        public virtual void CardsArePut(ContextType ctx) { }

        public virtual void Kakumei(ContextType ctx) { }

        public virtual void Nagare(ContextType ctx) { }

        public virtual void Agari(ContextType ctx) { }

        public virtual void Finish(ContextType ctx) { }

        public virtual void bindEvents(GameEvents evt) { ((IGamePlayer)this).BindEvents(evt); }
        public virtual void unbindEvents(GameEvents evt) { ((IGamePlayer)this).UnbindEvents(evt); }
    }
}
