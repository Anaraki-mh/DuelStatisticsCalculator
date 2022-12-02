using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuelStatsCalculator.Models
{
    public class AverageDuelStats
    {
        public AverageDuelStats()
        {
        }

        public float RangeMin { get; set; }
        public float RangeMax { get; set; }
        public float BlueWon { get; set; }
        public float RedWon { get; set; }
        public float ApiCalls { get; set; }
    }
}
