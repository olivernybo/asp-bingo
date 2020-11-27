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
            //bingoCaller.Start();
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

            int[] row0 = generateRow(sheet);
            int[] row1 = generateRow(sheet);
            int[] row2 = generateRow(sheet);

            int[] sheetArray;
            if (sheet.Count == 0) sheetArray = row0.Concat(row1).Concat(row2).ToArray();
            else sheetArray = GenerateSheet();

            Console.WriteLine("BingoService: Generated sheet");

            return sheetArray;
        }

        private static int[] generateRow(List<int> numbers, int rowCount = 5)
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