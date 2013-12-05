using Daifugo.Bases;
using Daifugo.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daifugo.GameImples
{
    public class ContextFactory
    {
        public static GameMaster CreateGameMaster(IGameContext ctx)
        {
            return new GameMaster(ctx as GameContext);
        }

        public static IGameContext CreateGameContext() {
            return new GameContext(RuleFactory.GetDefaultRule());
        }

        public static IPlayerContext CreatePlayerContext(IGamePlayer p, IGameContext ctx) {
            if (!(ctx is BaseGameContext)) throw new NotSupportedException("ctx is unknoun type");
            return new PlayerContext(p, ctx as BaseGameContext);
        }
    }
}
