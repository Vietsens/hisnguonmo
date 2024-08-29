using System;

namespace Inventec.Common.Number
{
    public class Get
    {
        public static decimal RoundCurrency(decimal currency, int numberDigit)
        {
            return Math.Round(currency, numberDigit);
        }
    }
}
