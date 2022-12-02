using ConsoleApp1;
using DuelStatsCalculator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuelStatsCalculator.Models
{
    public class DuelStats
    {
        public Winner Winner { get; set; }
        public int NumberOfApiCalls { get; set; }

        public void Deconstruct(out Winner winner, out int numberOfApiCalls)
        {
            winner = this.Winner;
            numberOfApiCalls = this.NumberOfApiCalls;
        }
    }
}
