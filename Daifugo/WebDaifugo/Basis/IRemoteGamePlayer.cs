using Daifugo.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Basis
{
    public interface IRemoteGamePlayer : IGamePlayer
    {
        bool IsConnected { get; }
    }
}