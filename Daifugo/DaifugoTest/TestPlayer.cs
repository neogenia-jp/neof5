using Daifugo.Bases;
using Daifugo.Cards;
using Daifugo.Players;
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

        public void Connect(GameEvents evt)
        {
            throw new NotImplementedException();
        }
    }
}
