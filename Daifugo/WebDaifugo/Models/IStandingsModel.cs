using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Models
{
    public interface IStandingsModel
    {
        string PlayerName { get; }

        int NumTimesDaifugo { get; }
        int NumTimesFugo { get; }
        int NumTimesHeimin { get; }
        int NumTimesHinmin { get; }
        int NumTimesDaihinmin { get; }

        int RoundPoint { get; }
        int TransferredPoint { get; }
    }
}
