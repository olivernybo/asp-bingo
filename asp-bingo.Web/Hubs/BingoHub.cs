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
        public static string CallerKey { get; private set; }

        private static readonly Random random = new Random();

        private string Id => Context.GetHttpContext().Session.Id;

        static BingoHub()
        {
            Console.WriteLine("BingoHub: Generating CalleyKey");
            char[] key = new char[32];
            for (int i = 0; i < key.Length; i++)
                key[i] = (char)random.Next(97, 123);
            CallerKey = new string(key);
            Console.WriteLine($"BingoHub: CallerKey: {CallerKey}");
        }

        public async Task BingoCaller(string key, int number)
        {
            if (key == CallerKey)
                await Clients.All.SendAsync("BingoCallerRecieve", number);
        }

        public void NewGame(string key)
        {
            if (key == CallerKey)
            {
                Console.WriteLine("BingoHub: Requesting new game");
                BingoService.NewGame();
                Clients.All.SendAsync("GameOver");
            }
        }

        public void GetSheet()
        {
            Console.WriteLine($"BingoHub: Requesting sheet for {Id}");
            int[] sheet = BingoService.GetBingoSheet(Id);
            Console.WriteLine("BingoHub: Sending sheet");
            Clients.Caller.SendAsync("Sheet", sheet);
        }

        public void CallBingo()
        {
            bool hasBingo = BingoService.CallBingo(Id);
            if (hasBingo)
            {
                if (BingoService.GameIsRunning)
                {
                    Clients.Others.SendAsync("BingoCalled");
                    Clients.All.SendAsync("RowsNeededForBingo", BingoService.RowsNeeded);
                } else Clients.Others.SendAsync("GameOver");
                Clients.Caller.SendAsync("Victory");
            } else Clients.Caller.SendAsync("NotBingo");
        }

        public void GetHistory() => Clients.Caller.SendAsync("History", BingoService.History);
        public void GetRowsNeededForBingo() => Clients.Caller.SendAsync("RowsNeededForBingo", BingoService.RowsNeeded);
    }
}