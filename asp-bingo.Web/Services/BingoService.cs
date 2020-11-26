using asp_bingo.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace asp_bingo.Web.Services
{
    public class BingoService
    {
        private readonly static Random random = new Random();
        private readonly static Dictionary<string, int[]> sheets = new Dictionary<string, int[]>();
        private readonly static Thread bingoCaller;
        private static bool gameIsRunning = false;

		static BingoService()
        {
            Console.WriteLine("BingoService: Bingo service starting");
            bingoCaller = new Thread(async () =>
            {
                HubConnection connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost/BingoHub")
                    .Build();
                
                connection.Closed += async (error) =>
                {
                    await Task.Delay(3000);
                    await connection.StartAsync();
                };

                await connection.StartAsync();
                Console.WriteLine("BingoService: Connection established");

                while (gameIsRunning)
                {
                    Console.WriteLine("BingoService: Calling...");
                    await connection.InvokeAsync("BingoCaller", BingoHub.CallerKey, "test");
                    await Task.Delay(1000);
                }
            });
            bingoCaller.Start();
        }

        public static int[] GetBingoSheet(string session)
        {
            if (sheets.ContainsKey(session)) return sheets[session];
            else
            {
                int[] sheet = GenerateSheet();
                sheets.Add(session, sheet);
                return sheet;
            }
        }

        private static int[] GenerateSheet()
        {
            List<int> sheet = new List<int>();

            int c0 = 0;
            int c1 = 0;
            int c2 = 0;
            int c3 = 0;
            int c4 = 0;
            int c5 = 0;
            int c6 = 0;
            int c7 = 0;
            int c8 = 0;
            while (sheet.Count < 15)
            {
                int number = random.Next(1, 91);
                if (!sheet.Contains(number) &&
                    ((number < 10 && c0++ < 3)
                    || (number < 20 && c1++ < 3)
                    || (number < 30 && c2++ < 3)
                    || (number < 40 && c3++ < 3)
                    || (number < 50 && c4++ < 3)
                    || (number < 60 && c5++ < 3)
                    || (number < 70 && c6++ < 3)
                    || (number < 80 && c7++ < 3)
                    || (number <= 90 && c8++ < 3))) sheet.Add(number);
            }

            List<int> row0 = new List<int>(sheet.Take(5));
            List<int> row1 = new List<int>(sheet.Skip(5).Take(5));
            List<int> row2 = new List<int>(sheet.Skip(10).Take(5));

            row0.Sort();
            row1.Sort();
            row2.Sort();

            // todo place 0's

            sheet = row0.Concat(row1).Concat(row2).ToList();

            return sheet.ToArray();
        }
    }
}