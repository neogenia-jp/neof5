using Daifugo.Bases;
using Daifugo.Cards;
using Daifugo.Observers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DaifugoTest
{

    internal class TestPlayer : IGamePlayer
    {
        public string Name { get; protected set; }

        internal Action<IGamePlayer, string, IPlayerContext> callback;

        public TestPlayer(string name, Action<IGamePlayer,string, IPlayerContext> callback)
        {
            Name = name;
            this.callback = callback;
        }

        public TestPlayer(string name)
        {
            Name = name;
            this.callback = (a, b, c) => { };
        }

        public virtual void Start(IPlayerContext ctx) { callback(this,"Start",ctx); }

        public virtual void CardDistributed(IPlayerContext ctx) { callback(this, "CardDistributed", ctx); }

        public virtual void CardSwapped(IPlayerContext ctx) { callback(this, "CardSwapped", ctx); }

        public virtual void Thinking(IPlayerContext ctx) { callback(this, "Thinking", ctx); }

        public virtual void CardsArePut(IPlayerContext ctx) { callback(this, "CardsArePut", ctx); }

        public virtual void Kakumei(IPlayerContext ctx) { callback(this, "Kakumei", ctx); }
        
        public virtual void Nagare(IPlayerContext ctx) { callback(this, "Nagare", ctx); }

        public virtual void Agari(IPlayerContext ctx) { callback(this, "Agari", ctx); }

        public virtual void Finish(IPlayerContext ctx) { callback(this, "Finish", ctx); }

        public virtual void ProcessTurn(IPlayerContext ctx) { callback(this, "ProcessTurn", ctx); }

        public void unbindEvents(GameEvents evt) { }
        public void bindEvents(GameEvents evt)
        {
            evt.agari += (arg) => this.Agari(arg(this) as IPlayerContext);
            evt.cardDistributed += (arg) => this.CardDistributed(arg(this) as IPlayerContext);
            evt.cardsArePut += (arg) => this.CardsArePut(arg(this) as IPlayerContext);
            evt.cardSwapped += (arg) => this.CardSwapped(arg(this) as IPlayerContext);
            evt.finish += (arg) => this.Finish(arg(this) as IPlayerContext);
            evt.kakumei += (arg) => this.Kakumei(arg(this) as IPlayerContext);
            evt.nagare += (arg) => this.Nagare(arg(this) as IPlayerContext);
            evt.start += (arg) => this.Start(arg(this) as IPlayerContext);
            evt.thinking += (arg) => this.Thinking(arg(this) as IPlayerContext);
        } 
    }
}
