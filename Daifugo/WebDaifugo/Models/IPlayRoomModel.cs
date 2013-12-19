using Daifugo.GameImples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Models
{
    public interface IPlayRoomModel
    {
        GameMaster Master { get; }
        string RoomID { get; }
		IEnumerable<IStandingsModel> Standings { get; }
    }
}