using DuelStatsCalculator.Enums;
using DuelStatsCalculator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convert = DuelStatsCalculator.Helpers.Convert;


namespace DuelStatsCalculator
{
    public class StatsCalculator
    {
        #region Properties

        public int DamageRangeMin { get; set; }
        public int DamageRangeMax { get; set; }
        public int MaxApiCalls { get; set; }
        public int NumberOfDuels { get; set; }
        public int NumberOfResults { get; set; }
        public int NumberOfTestsRun { get; set; }
        public bool Beep { get; set; }

        private Random Random { get; set; }

        #endregion


        #region Constructor

        public StatsCalculator()
        {
            DamageRangeMin = 2;
            DamageRangeMax = 40 + 1;
            NumberOfDuels = 10000;
            NumberOfResults = 10;
            MaxApiCalls = 25;
            Beep = false;

            Random = new Random();
        }

        #endregion


        #region Methods

        public void Intro()
        {
            Console.WriteLine("Hellooo! \n" +
            "If you want to change the values, type an integer value in fron of the ':' and press enter; " +
            "Otherwise just press enter and the program will use the hard coded initial values which are: \n\n" +

            $"Damage range min: {DamageRangeMin}\n" +
            $"Damage range max: {DamageRangeMax - 1}\n" +
            $"Max number of API calls: {MaxApiCalls}\n" +
            $"Number of duels: {NumberOfDuels}\n" +
            $"Number of resutls to display: {NumberOfResults}\n" +
            $"Beep: {Beep}\n" +
            $"\n");
        }

        public void ModifyParameters()
        {
            Console.Write("Range min: ");
            DamageRangeMin = Convert.ToInt(2, Console.ReadLine());

            Console.Write("Range max: ");
            DamageRangeMax = Convert.ToInt(DamageRangeMax, Console.ReadLine(), 1);

            Console.Write("Max number of API calls: ");
            MaxApiCalls = Convert.ToInt(MaxApiCalls, Console.ReadLine());

            Console.Write("Number of Duels (in each range): ");
            NumberOfDuels = Convert.ToInt(NumberOfDuels, Console.ReadLine());

            Console.Write("Number of results to display: ");
            NumberOfResults = Convert.ToInt(NumberOfResults, Console.ReadLine());

            Console.Write("Beep when done? (y/n) ");
            if (Console.ReadLine() == "y")
                Beep = true;
        }

        public void DuelAverageInAllDamageRanges()
        {
            List<AverageDuelStats> averageStatsList = new List<AverageDuelStats>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int right = DamageRangeMax - 1; right > DamageRangeMin; right--)
            {
                for (int left = DamageRangeMin; left < right; left++)
                {
                    Console.WriteLine($"DamageRange: {left:00} - {right:00}, Calculating...");

                    averageStatsList.Add(DuelAverage(NumberOfDuels, left, right, false));
                    Console.WriteLine("Done!");
                }
            }

            List<AverageDuelStats> topResults = averageStatsList
                .Where(x => x.ApiCalls <= MaxApiCalls)
                .OrderBy(x => Math.Abs(x.BlueWon - x.RedWon))
                .ThenBy(x => x.ApiCalls)
                .Take(NumberOfResults)
                .ToList();

            stopwatch.Stop();

            Console.WriteLine("\n\n" + $"Top {NumberOfResults} results: \n");

            foreach (var result in topResults)
            {
                Console.WriteLine($"DamageRange: {result.RangeMin:00} to {result.RangeMax:00}, " +
                    $"Blue won: {result.BlueWon:0.000}%, Red won: {result.RedWon:0.000}%, " +
                    $"Api calls: {result.ApiCalls:0.0}");
            }

            Console.WriteLine($"\nTotal number of tests run: {NumberOfTestsRun.ToString("N0")}");
            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds}.{stopwatch.Elapsed.Milliseconds:0} s");

            if (Beep == true)
                Console.Beep(900, 2000);


            Console.Write("\nOrder and display the results by number of API calls? (y/n) ");
            if (Console.ReadLine() == "y")
            {
                topResults = topResults.OrderBy(x => x.ApiCalls).ToList();

                Console.WriteLine("\n\n" + $"Top {NumberOfResults} results: \n");

                foreach (var result in topResults)
                {
                    Console.WriteLine($"DamageRange: {result.RangeMin:00} to {result.RangeMax:00}, " +
                        $"Blue won: {result.BlueWon:0.000}%, Red won: {result.RedWon:0.000}%, " +
                        $"Api calls: {result.ApiCalls:0.0}");
                }
            }
        }

        public AverageDuelStats DuelAverage(float numberOfDuels, int min, int max, bool? log)
        {
            AverageDuelStats averageStats = new AverageDuelStats();
            DuelStats stats = new DuelStats();

            averageStats.RangeMin = min;
            averageStats.RangeMax = max;
            numberOfDuels++;


            if (log == true)
            {
                for (int i = 1; i < numberOfDuels; i++)
                {
                    Console.WriteLine($"{i}_Calculating...");

                    Duel(ref stats, min, max);

                    if (stats.Winner == Winner.Blue)
                        averageStats.BlueWon++;
                    else
                        averageStats.RedWon++;

                    averageStats.ApiCalls += stats.NumberOfApiCalls;

                    Console.WriteLine("Done!");
                }

                if (Beep == true)
                    Console.Beep(900, 2000);

                Console.WriteLine($"DamageRange: {averageStats.RangeMin:00} to {averageStats.RangeMax:00}, " +
                        $"Blue won: {averageStats.BlueWon:0.000}%, Red won: {averageStats.RedWon:0.000}%, " +
                        $"Api calls: {averageStats.ApiCalls:0.0}");
            }
            else
            {
                for (int i = 1; i < numberOfDuels; i++)
                {
                    Duel(ref stats, min, max);

                    if (stats.Winner == Winner.Blue)
                        averageStats.BlueWon++;
                    else
                        averageStats.RedWon++;

                    averageStats.ApiCalls += stats.NumberOfApiCalls;
                }
            }

            averageStats.BlueWon = averageStats.BlueWon / numberOfDuels * 100;
            averageStats.RedWon = averageStats.RedWon / numberOfDuels * 100;
            averageStats.ApiCalls = averageStats.ApiCalls / numberOfDuels;

            return averageStats;
        }

        private void Duel(ref DuelStats stats, int min, int max)
        {
            NumberOfTestsRun++;

            int blue = 100;
            int red = 100;

            stats.Winner = Winner.none;
            stats.NumberOfApiCalls = 2;

            while (true)
            {
                red = red - Random.Next(min, max);

                if (red <= 0)
                {
                    stats.Winner = Winner.Blue;
                    break;
                }

                stats.NumberOfApiCalls++;

                blue = blue - Random.Next(min + 3, max);

                if (blue <= 0)
                {
                    stats.Winner = Winner.Red;
                    break;
                }

                stats.NumberOfApiCalls++;
            }
        }

        #endregion
    }
}
