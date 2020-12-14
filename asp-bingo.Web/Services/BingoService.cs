using asp_bingo.Web.Hubs;
using asp_bingo.Web.Models;
using Microsoft.AspNetCore.Http;
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
        public static bool GameIsRunning { get; private set; } = false;
        public static int[] History => history.ToArray();
        public static int RowsNeeded { get; private set; } = 1;

        private readonly static Random random = new Random();
        private readonly static Dictionary<string, Player> players = new Dictionary<string, Player>();
        private readonly static Thread bingoCaller;
        private readonly static List<int> history = new List<int>();

		static BingoService()
        {
            Console.WriteLine("BingoService: Bingo service starting");
            bingoCaller = new Thread(async () =>
            {
                HubConnection connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost/BingoHub")
                    .Build();
                
                connection.Closed += async error =>
                {
                    await Task.Delay(3000);
                    await connection.StartAsync();
                };

                await connection.StartAsync();
                Console.WriteLine("BingoService: Connection established");

                Game:
                while (GameIsRunning)
                {
                    Console.WriteLine("BingoService: Calling random number...");
                    await CallRandomNunber(connection);
                    await Task.Delay(5000);
                }
                await Task.Delay(1000);
                goto Game;
            });
            bingoCaller.Start();
        }

        private static async Task CallRandomNunber(HubConnection connection)
        {
			if (history.Count == 90) return;
            int number = 0;
            while (number == 0 || history.Contains(number))
                number = random.Next(1, 91);
            history.Add(number);
            await connection.InvokeAsync("BingoCaller", BingoHub.CallerKey, number);
        }

        public static async void NewGame()
        {
            players.Clear();
            history.Clear();
            RowsNeeded = 1;
			await Task.Delay(10000);
			Continue();
        }

		public static void Continue() => GameIsRunning = true;
		public static void Pause() => GameIsRunning = false;

        public static int[] GetBingoSheet(HttpContext context)
        {
			string session = context.Session.Id;
            if (players.ContainsKey(session)) return players[session].Sheet;
            else
            {
				string name = context.Session.GetString("name");
				string className = context.Session.GetString("class");
				string color = context.Session.GetString("color");
                int[] sheet = GenerateSheet();
				Player player = new Player { Sheet = sheet, Name = name, Class = className, Color = color };
                players.Add(session, player);
                return sheet;
            }
        }

		#nullable enable
        public static (bool, string?, string?) CallBingo(string id)
        {
            if (!players.ContainsKey(id)) return (false, null, null);

            Console.WriteLine($"BingoService: Checking {id}'s sheet for bingo");
            int[] sheet = players[id].Sheet;
            IEnumerable<int>[] rows = new IEnumerable<int>[]
            {
                sheet.Take(9),
                sheet.Skip(9).Take(9),
                sheet.Skip(18).Take(9)
            };

            int validRows = 0;
            foreach (var row in rows)
            {
                bool valid = true;
                foreach (int number in row)
                {
                    if (number == 0) continue;
                    else if (!history.Contains(number))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid) validRows++;
            }

            bool hasBingo = validRows >= RowsNeeded;
            if (hasBingo && ++RowsNeeded == 4) Pause();

            return (hasBingo, players[id].Name, players[id].Class);
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
                
                if (sheet.Contains(number)) continue;
                else if (number < 10)
                {
                    if (c0++ < 3)
                        sheet.Add(number);
                }
                else if (number < 20)
                {
                    if (c1++ < 3)
                        sheet.Add(number);
                }
                else if (number < 30)
                {
                    if (c2++ < 3)
                        sheet.Add(number);
                }
                else if (number < 40)
                {
                    if (c3++ < 3)
                        sheet.Add(number);
                }
                else if (number < 50)
                {
                    if (c4++ < 3)
                        sheet.Add(number);
                }
                else if (number < 60)
                {
                    if (c5++ < 3)
                        sheet.Add(number);
                }
                else if (number < 70)
                {
                    if (c6++ < 3)
                        sheet.Add(number);
                }
                else if (number < 80)
                {
                    if (c7++ < 3)
                        sheet.Add(number);
                }
                else if (number <= 90)
                    if (c8++ < 3)
                        sheet.Add(number);
            }

            int[] row0 = GenerateRow(sheet);
            int[] row1 = GenerateRow(sheet);
            int[] row2 = GenerateRow(sheet);

            // Recursive cause the generation can fail
            int[] sheetArray;
            if (sheet.Count == 0) sheetArray = row0.Concat(row1).Concat(row2).ToArray();
            else sheetArray = GenerateSheet();

            Console.WriteLine("BingoService: Generated sheet");

            return sheetArray;
        }

        private static int[] GenerateRow(List<int> numbers, int rowCount = 5)
        {
            List<int> row = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            List<int> unusedNumbers = new List<int>();
            
            int takenCount = 0;
            while (takenCount < rowCount)
            {
                if (numbers.Count == 0) break;
                int num = numbers[0];
                int numPos = num == 90 ? 8 : int.Parse(num.ToString().PadLeft(2, '0')[0].ToString());
                
                if (row[numPos] == 0)
                {
                    row[numPos] = num;
                    takenCount++;
                } else unusedNumbers.Add(num);
                
                numbers.RemoveAt(0);
            }

            foreach (int num in unusedNumbers)
                numbers.Add(num);

            return row.ToArray();
        }
    }
}