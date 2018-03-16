using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRSimpleChatServerDotNet46
{
    public class Chat : Hub
    {
        public void Send(string nick, string message)
        {
            Clients.All.Send(nick, $"{message} ({DateTime.Now.ToString()})");
        }
    }
}