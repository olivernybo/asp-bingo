using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace asp_bingo.Web.Hubs
{
    public class BingoHub : Hub
    {
        public async Task BingoCaller(string message) => await Clients.All.SendAsync("BingoCallerRecieve", message);
    }
}