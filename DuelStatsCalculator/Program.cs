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
using DuelStatsCalculator;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var item in args)
            {
                Console.WriteLine(item);
            }
            StatsCalculator statsCalculator = new StatsCalculator();

            statsCalculator.Intro();

            Console.Write("Do you want to change the value of the parameters? (y/n) ");
            if (Console.ReadLine().ToLower() == "y")
                statsCalculator.ModifyParameters();

            Console.Write("Run tests in all possible ranges between Range min and Range max? (y/n) ");
            if (Console.ReadLine().ToLower() == "y")
                statsCalculator.DuelAverageInAllDamageRanges();
            else
                statsCalculator.DuelAverage(statsCalculator.NumberOfDuels,
                    statsCalculator.DamageRangeMin,
                    statsCalculator.DamageRangeMax,
                    true);

            Console.Write("\nRestart the program? (y/n) ");
            if (Console.ReadLine() == "y")
            {
                Console.Clear();
                Main(args);
            }
            else
                Environment.Exit(0);

            Console.ReadLine();
        }

    }




}
