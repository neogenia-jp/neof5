using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDaifugo.Basis
{
    public interface ITweetListener
    {
        void Tweet(string message);
    }
}