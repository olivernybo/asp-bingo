using asp_bingo.Web.Services;
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
        private static readonly Random random = new Random();
        public static string CallerKey { get; private set; }

        static BingoHub()
        {
            Console.WriteLine("BingoHub: Generating CalleyKey");
            char[] key = new char[32];
            for (int i = 0; i < key.Length; i++)
                key[i] = (char)random.Next(97, 123);
            CallerKey = new string(key);
            Console.WriteLine($"BingoHub: CallerKey: {CallerKey}");
        }

        public async Task BingoCaller(string key, string message)
        {
            if (key == CallerKey)
                await Clients.All.SendAsync("BingoCallerRecieve", message);
        }

        public void GetSheet()
        {
            string id = Context.GetHttpContext().Session.Id;
            Console.WriteLine($"BingoHub: Requesting sheet for {id}");
            int[] sheet = BingoService.GetBingoSheet(id);
            Console.WriteLine("BingoHub: Sending sheet");
            Clients.Caller.SendAsync("Sheet", sheet);
        }
    }
}