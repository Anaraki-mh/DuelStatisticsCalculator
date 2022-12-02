using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuelStatsCalculator.Helpers
{
    public static class Convert
    {
        public static int ToInt(int failValue, string? successValue)
        {
            int convertedValue;
            if (Int32.TryParse(successValue, out convertedValue))
                return convertedValue;
            else
                return failValue;
        }
        public static int ToInt(int failValue, string? successValue, int add)
        {
            int convertedValue;
            if (Int32.TryParse(successValue, out convertedValue))
                return convertedValue + add;
            else
                return failValue;
        }
    }
}
