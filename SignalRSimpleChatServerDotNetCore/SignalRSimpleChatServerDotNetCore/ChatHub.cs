﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRSimpleChatServerDotNetCore
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, $"[{DateTime.Now}] {message}");
        }
    }

}
