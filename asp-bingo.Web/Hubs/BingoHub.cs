using asp_bingo.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace asp_bingo.Web.Hubs
{
    public class BingoHub : Hub
    {
        public static string CallerKey { get; private set; }

        private static readonly Random random = new Random();

		private new HttpContext Context => base.Context.GetHttpContext();
        private string Id => Context.Session.Id;

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
                Clients.All.SendAsync("StartingNewGame");
            }
        }

		public async void NextRound(string key)
		{
			Console.WriteLine("BingoHub: Requesting next round");
			await Clients.All.SendAsync("StartingNewRound");
			await Task.Delay(10000);
			BingoService.Continue();
		}

        public void GetSheet()
        {
            Console.WriteLine($"BingoHub: Requesting sheet for {Id}");
            int[] sheet = BingoService.GetBingoSheet(Context);
            Console.WriteLine("BingoHub: Sending sheet");
            Clients.Caller.SendAsync("Sheet", sheet);
        }

		#nullable enable
        public void CallBingo()
        {
			int rowsNeeded = BingoService.RowsNeeded;
            (bool hasBingo, string? name, string? className) = BingoService.CallBingo(Id);
            if (hasBingo)
            {
				BingoService.Pause();
				ConnectionFactory factory = new ConnectionFactory { HostName = "rabbit" };
				using (IConnection connection = factory.CreateConnection())
				using (IModel channel = connection.CreateModel())
				{
					channel.QueueDeclare(queue: "winners",
										durable: false,
										exclusive: false,
										autoDelete: false,
										arguments: null);

					string message = $"{name}, {className} - {rowsNeeded} row(s)";
					byte[] body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "",
										routingKey: "winners",
										basicProperties: null,
										body: body);
					Console.WriteLine($"BingoHub: Winner {message}");
				}
                if (BingoService.RowsNeeded <= 3)
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