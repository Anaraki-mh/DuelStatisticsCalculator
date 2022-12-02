using DuelStatsCalculator.Enums;
using DuelStatsCalculator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DuelStatsCalculator.Helpers;
using Convert = DuelStatsCalculator.Helpers.Convert;

namespace ConsoleApp1
{
    public class Program
    {
        static int numberOfTestsRun;
        public static void Main(string[] args)
        {

            int rangeMin = 2;
            int rangeMax = 40 + 1;
            int numberOfDuels = 10000;
            int numberOfResults = 10;
            int maxApiCalls = 25;
            bool beep = false;

            Console.WriteLine("Hellooo! \n" +
                "If you want to change the values, type an integer value in fron of the ':' and press enter; " +
                "Otherwise just press enter and the program will use the hard coded initial values which are: \n\n" +
                $"Range min: {rangeMin}\n" +
                $"Range max: {rangeMax - 1}\n" +
                $"Number of duels: {numberOfDuels}\n" +
                $"Number of resutls to display: {numberOfResults}\n" +
                $"beep: {beep}\n" +
                $"\n");

            Console.Write("Range min: ");
            rangeMin = Convert.ToInt(2, Console.ReadLine());

            Console.Write("Range max: ");
            rangeMax = Convert.ToInt(rangeMax, Console.ReadLine(), 1);

            Console.Write("Number of Duels (in each range): ");
            numberOfDuels = Convert.ToInt(numberOfDuels, Console.ReadLine());

            Console.Write("Number of results to display: ");
            numberOfResults = Convert.ToInt(numberOfResults, Console.ReadLine());

            Console.Write("Max number of API calls: ");
            maxApiCalls = Convert.ToInt(maxApiCalls, Console.ReadLine());

            Console.Write("Beep when done? (y/n) ");
            if (Console.ReadLine() == "y")
                beep = true;

            AverageDuelStats averageStats = new AverageDuelStats();
            List<AverageDuelStats> averageStatsList = new List<AverageDuelStats>();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int right = rangeMax - 1; right > rangeMin; right--)
            {
                for (int left = rangeMin; left < right; left++)
                {
                    Console.WriteLine($"Range: {left:00} - {right:00}, Calculating...");

                    averageStats = DuelAverage(numberOfDuels, left, right);
                    averageStatsList.Add(averageStats);
                    Console.WriteLine("Done!");
                }
            }

            List<AverageDuelStats> topResults = averageStatsList
                .Where(x => x.ApiCalls <= maxApiCalls)
                .OrderBy(x => Math.Abs(x.BlueWon - x.RedWon))
                .Take(numberOfResults)
                .ToList();

            stopwatch.Stop();

            Console.WriteLine("\n\n" + $"Top {numberOfResults} results: \n");

            foreach (var result in topResults)
            {
                Console.WriteLine($"Range: {result.RangeMin:00} to {result.RangeMax:00}, " +
                    $"Blue won: {result.BlueWon:0.000}%, Red won: {result.RedWon:0.000}%, " +
                    $"Api calls: {result.ApiCalls:0.0}");
            }

            Console.WriteLine($"\nTotal number of tests run: {numberOfTestsRun.ToString("N0")}");
            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds}.{stopwatch.Elapsed.Milliseconds} s");

            if (beep)
                Console.Beep(900, 2000);


            Console.Write("\nOrder and display the results by number of API calls? (y/n) ");
            if (Console.ReadLine() == "y")
            {
                topResults = topResults.OrderBy(x => x.ApiCalls).ToList();

                Console.WriteLine("\n\n" + $"Top {numberOfResults} results: \n");

                foreach (var result in topResults)
                {
                    Console.WriteLine($"Range: {result.RangeMin:00} to {result.RangeMax:00}, " +
                        $"Blue won: {result.BlueWon:0.000}%, Red won: {result.RedWon:0.000}%, " +
                        $"Api calls: {result.ApiCalls:0.0}");
                }
            }

            Console.Write("\nRestart the program? (y/n) ");
            if (Console.ReadLine() == "y")
            {
                Console.Clear();
                Main(args);
            }

            Console.ReadLine();
        }

        static AverageDuelStats DuelAverage(float numberOfDuels, int min, int max)
        {
            AverageDuelStats averageStats = new AverageDuelStats();

            averageStats.RangeMin = min;
            averageStats.RangeMax = max;

            Random random = new Random();
            DuelStats stats = new DuelStats();

            for (int i = 0; i < numberOfDuels; i++)
            {
                stats = Duel(stats, random, min, max);

                if (stats.Winner == Winner.Blue)
                    averageStats.BlueWon++;
                else
                    averageStats.RedWon++;

                averageStats.ApiCalls += stats.NumberOfApiCalls;
            }

            averageStats.BlueWon = averageStats.BlueWon / numberOfDuels * 100;
            averageStats.RedWon = averageStats.RedWon / numberOfDuels * 100;
            averageStats.ApiCalls = averageStats.ApiCalls / numberOfDuels;

            return averageStats;
        }

        static DuelStats Duel(DuelStats stats, Random random, int min, int max)
        {
            numberOfTestsRun++;

            int blue = 100;
            int red = 100;

            stats.Winner = Winner.none;
            stats.NumberOfApiCalls = 2;

            while (true)
            {
                red = red - random.Next(min, max);

                if (red <= 0)
                {
                    stats.Winner = Winner.Blue;
                    break;
                }

                stats.NumberOfApiCalls++;

                blue = blue - random.Next(min + 1, max);

                if (blue <= 0)
                {
                    stats.Winner = Winner.Red;
                    break;
                }

                stats.NumberOfApiCalls++;
            }

            return stats;
        }

    }




}
