using Daifugo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Daifugo.Cards;
using Daifugo.Bases;
using Daifugo.Utils;

namespace WebDaifugo.Neof5Protocols
{
    internal class ProtocolData
    {
        private object _data;

        public ProtocolData(int playerNum, string kind, IPlayerContext ctx)
        {
            _data = new
            {
                YourNum = playerNum,
                Kind = kind,
                Teban = ctx.GameContext.Teban,
                IsKakumei = ctx.GameContext.IsKakumei,
                PlayerInfo = ctx.GameContext.PlayerInfo,
                Deck = ctx.Deck.ToCardsetString(),
                Ba = ctx.GameContext.Ba.Select(cards => cards.ToCardsetString()),
                Yama = ctx.GameContext.Yama.JoinString(" "),
                History = ctx.GameContext.History.Select(gh=>gh.ToString()),
            };
        }

        public ProtocolData(ICheckResult result)
        {
            _data = new
            {
                Kind = "Exception",
                Message = result.Message
            };
        }

        public ProtocolData(Exception e)
        {
            _data = new
            {
                Kind = "Exception",
                Message = e.Message
            };
        }
        
        public string ToJson() {
            return JsonConvert.SerializeObject(_data);
        }
    }

}