using Daifugo.Cards;
using System;
using System.Collections.Generic;

namespace Daifugo.Bases
{
	using ContextType = IMonitorContext;

    public interface IGameMonitor : IGameObserver<ContextType>
    {
    }

    public static class IGameMonitorExtention
    {
        public static void BindEvents(this IGameMonitor _this, GameEvents evt)
        {
            var a = GameEventBinder<ContextType,IGameMonitor>.GetOrCreate(_this,evt);
            a.bindEvents();
        }
        public static void UnbindEvents(this IGameMonitor _this, GameEvents evt)
        {
            var a = GameEventBinder<ContextType,IGameMonitor>.GetOrCreate(_this,evt);
            a.unbindEvents();
        }
    }
}
