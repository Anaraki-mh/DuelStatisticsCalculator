using DuelStatsCalculator.Enums;
using DuelStatsCalculator.Structs;
using System;
using System.Collections.Generic;
using System.Data;
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
        public ulong NumberOfDuels { get; set; }
        public int NumberOfResults { get; set; }
        public ulong NumberOfTestsRun = 0;
        public int AddToRedRandomDamageTakenMin { get; set; }
        public int AddToRedRandomDamageTakenMax { get; set; }
        public int AddToBlueRandomDamageTakenMin { get; set; }
        public int AddToBlueRandomDamageTakenMax { get; set; }
        public bool Beep { get; set; }
        public bool Log { get; set; }

        private Random Random { get; set; }
        private DuelStats DuelStats { get; set; }
        private Stopwatch stopwatch;



        #endregion


        #region Constructor

        public StatsCalculator()
        {
            DamageRangeMin = 4;
            DamageRangeMax = 30 + 1;

            NumberOfDuels = 100_000_000;
            NumberOfResults = 10;
            MaxApiCalls = 20;

            AddToRedRandomDamageTakenMin = 3;
            AddToRedRandomDamageTakenMax = 0;
            AddToBlueRandomDamageTakenMin = 0;
            AddToBlueRandomDamageTakenMax = 0;

            Beep = false;
            Log = false;

            Random = new Random();
            DuelStats = new DuelStats();
            stopwatch = new Stopwatch();
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
            $"Number of duels: {NumberOfDuels.ToString("N0")}\n" +
            $"Number of resutls to display: {NumberOfResults}\n" +
            $"Add to Red's min random damage taken: {AddToRedRandomDamageTakenMin}\n" +
            $"Add to Red's max random damage taken: {AddToRedRandomDamageTakenMax}\n" +
            $"Add to Blue's min random damage taken: {AddToBlueRandomDamageTakenMin}\n" +
            $"Add to Blue's max random damage taken: {AddToBlueRandomDamageTakenMax}\n" +
            $"Beep: {Beep}\n" +
            $"Log: {Log}\n" +
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
            NumberOfDuels = Convert.ToUlong(NumberOfDuels, Console.ReadLine());

            Console.Write("Number of results to display: ");
            NumberOfResults = Convert.ToInt(NumberOfResults, Console.ReadLine());

            Console.Write("Add to Red's min random damage taken: ");
            AddToRedRandomDamageTakenMin = Convert.ToInt(AddToRedRandomDamageTakenMin, Console.ReadLine());

            Console.Write("Add to Red's max random damage taken: ");
            AddToRedRandomDamageTakenMax = Convert.ToInt(AddToRedRandomDamageTakenMax, Console.ReadLine());

            Console.Write("Add to Blue's min random damage taken: ");
            AddToBlueRandomDamageTakenMin = Convert.ToInt(AddToBlueRandomDamageTakenMin, Console.ReadLine());

            Console.Write("Add to Blue's max random damage taken: ");
            AddToBlueRandomDamageTakenMax = Convert.ToInt(AddToBlueRandomDamageTakenMax, Console.ReadLine());

            Console.Write("Beep when done? (y/n) ");
            if (Console.ReadLine() == "y")
                Beep = true;

            Console.Write("Log the progress? (can slow the program down) (y/n) ");
            if (Console.ReadLine() == "y")
                Log = true;
        }

        public async Task DuelAverageInAllDamageRanges()
        {
            NumberOfTestsRun = 0;

            List<AverageDuelStats> averageStatsList = new List<AverageDuelStats>();
            List<Task<AverageDuelStats>> tasksList = new List<Task<AverageDuelStats>>();

            stopwatch.Start();

            if (Log == true)
            {
                for (int right = DamageRangeMax - 1; right > DamageRangeMin; right--)
                {
                    for (int left = DamageRangeMin;
                        left + AddToRedRandomDamageTakenMin < right + AddToRedRandomDamageTakenMax &&
                        left + AddToBlueRandomDamageTakenMin < right + AddToBlueRandomDamageTakenMax
                        ; left++)
                    {
                        Console.WriteLine($"Damage Range: {left:00} - {right:00}, Calculating...");
                        tasksList.Add(DuelAverage(NumberOfDuels, left, right));
                    }
                }
            }
            else
            {
                Console.Write("Calculating...");
                Console.CursorVisible = false;
                double numberOfRanges = NumberOfRanges();
                double completedRanges = 0;

                for (int right = DamageRangeMax - 1; right > DamageRangeMin; right--)
                {
                    for (int left = DamageRangeMin;
                        left + AddToRedRandomDamageTakenMin < right + AddToRedRandomDamageTakenMax &&
                        left + AddToBlueRandomDamageTakenMin < right + AddToBlueRandomDamageTakenMax
                        ; left++)
                    {
                        Console.CursorLeft = 15;
                        Console.Write($"(%{completedRanges / numberOfRanges * 100:0})");

                        tasksList.Add(DuelAverage(NumberOfDuels, left, right));
                        completedRanges++;
                    }
                }
                Console.CursorVisible = true;

            }

            await Task.WhenAll(tasksList);

            foreach (var task in tasksList)
            {
                averageStatsList.Add(task.Result);
                //Console.WriteLine($"DamageRange: {task.Result.RangeMin:00} - {task.Result.RangeMax:00}, Done!");
            }

            List<AverageDuelStats> topResults = averageStatsList
                .Where(x => x.ApiCalls <= MaxApiCalls)
                .OrderBy(x => Math.Abs(x.RedWon - x.BlueWon))
                .ThenBy(x => x.ApiCalls)
                .Take(NumberOfResults)
                .ToList();

            stopwatch.Stop();

            Console.WriteLine("\n\n" + $"Top {NumberOfResults} results: \n");

            foreach (var result in topResults)
            {
                Console.WriteLine($"DamageRange: {result.RangeMin:00} to {result.RangeMax:00}, " +
                    $"Red won: {result.RedWon:0.000}%, Blue won: {result.BlueWon:0.000}%, " +
                    $"Api calls: {result.ApiCalls:0.0}");
            }

            Console.WriteLine($"\nTotal number of tests run: {NumberOfTestsRun.ToString("N0")}");
            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds:0.00} s");

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
                        $"Red won: {result.RedWon:0.000}%, Blue won: {result.BlueWon:0.000}%, " +
                        $"Api calls: {result.ApiCalls:0.0}");
                }
            }
        }

        public async Task DuelAverageWithLog(float numberOfDuels, int min, int max)
        {
            AverageDuelStats averageStats;

            averageStats.RangeMin = min;
            averageStats.RangeMax = max;
            averageStats.RedWon = 0;
            averageStats.BlueWon = 0;
            averageStats.ApiCalls = 0;

            if (Log)
            {
                stopwatch.Restart();

                for (ulong i = 0UL; i < (ulong)numberOfDuels; i++)
                {
                    DuelStats = Duel(min, max);

                    if (DuelStats.Winner == Winner.Red)
                        averageStats.RedWon++;
                    else
                        averageStats.BlueWon++;

                    averageStats.ApiCalls += DuelStats.NumberOfApiCalls;

                    Console.WriteLine($"{i + 1}_ {DuelStats.Winner} won!");
                }
                stopwatch.Stop();

                averageStats.RedWon = averageStats.RedWon / numberOfDuels * 100;
                averageStats.BlueWon = averageStats.BlueWon / numberOfDuels * 100;
                averageStats.ApiCalls = averageStats.ApiCalls / numberOfDuels;
            }
            else
            {
                stopwatch.Restart();
                averageStats = await DuelAverage(numberOfDuels, min, max);
                stopwatch.Stop();
            }

            Console.WriteLine($"\nDamageRange: {averageStats.RangeMin:00} to {averageStats.RangeMax - 1:00}, " +
            $"Red won: {averageStats.RedWon:0.000}%, " +
            $"Blue won: {averageStats.BlueWon:0.000}%, " +
            $"Api calls: {averageStats.ApiCalls:0.0}");

            Console.WriteLine($"\nTotal number of tests run: {NumberOfTestsRun.ToString("N0")}");
            Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds} s");
        }

        private async Task<AverageDuelStats> DuelAverage(float numberOfDuels, int min, int max)
        {
            AverageDuelStats averageStats;

            averageStats.RangeMin = min;
            averageStats.RangeMax = max;
            averageStats.RedWon = 0;
            averageStats.BlueWon = 0;
            averageStats.ApiCalls = 0;

            for (ulong i = 0UL; i < (ulong)numberOfDuels; i++)
            {
                DuelStats = Duel(min, max);

                if (DuelStats.Winner == Winner.Red)
                    averageStats.RedWon++;
                else
                    averageStats.BlueWon++;

                averageStats.ApiCalls += DuelStats.NumberOfApiCalls;
            }

            averageStats.RedWon = averageStats.RedWon / numberOfDuels * 100;
            averageStats.BlueWon = averageStats.BlueWon / numberOfDuels * 100;
            averageStats.ApiCalls = averageStats.ApiCalls / numberOfDuels;

            return averageStats;
        }

        private static readonly Random _random = new Random();
        private DuelStats Duel(int min, int max)
        {
            DuelStats duelStats = new DuelStats();

            Interlocked.Increment(ref NumberOfTestsRun);

            int red = 100;
            int blue = 100;
            int addToBlueRandomDamageTakenMin = AddToBlueRandomDamageTakenMin;
            int addToBlueRandomDamageTakenMax = AddToBlueRandomDamageTakenMax;
            int addToRedRandomDamageTakenMin = AddToRedRandomDamageTakenMin;
            int addToRedRandomDamageTakenMax = AddToRedRandomDamageTakenMax;

            duelStats.Winner = Winner.none;
            duelStats.NumberOfApiCalls = 2;

            while (true)
            {
                blue -= _random.Next(min + addToBlueRandomDamageTakenMin, max + addToBlueRandomDamageTakenMax);
                duelStats.NumberOfApiCalls++;

                if (blue <= 0)
                {
                    duelStats.Winner = Winner.Red;
                    break;
                }

                red -= _random.Next(min + addToRedRandomDamageTakenMin, max + addToRedRandomDamageTakenMax);
                duelStats.NumberOfApiCalls++;

                if (red <= 0)
                {
                    duelStats.Winner = Winner.Blue;
                    break;
                }
            }

            return duelStats;
        }

        private int NumberOfRanges()
        {
            int result = 0;
            for (int right = DamageRangeMax - 1; right > DamageRangeMin; right--)
            {
                for (int left = DamageRangeMin;
                    left + AddToRedRandomDamageTakenMin < right + AddToRedRandomDamageTakenMax &&
                    left + AddToBlueRandomDamageTakenMin < right + AddToBlueRandomDamageTakenMax
                    ; left++)
                {
                    result++;
                }
            }
            return result;
        }

        #endregion
    }
}
