using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class ConvertNumber
    {
        /// <summary>
        /// Hàm làm chòn số đến phần thập phân mong muốn
        /// Vd: NumberToNumberRoundAuto(123456.7, 4) = 123,456.8 ; NumberToNumberRoundAuto(123456.712, 3) = 123,456.712 ; NumberToNumberRoundAuto(123456.7, 0) = 123,457
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static decimal NumberToNumberRoundAuto(decimal number, int numberDigit)
        {
            decimal result = 0;
            try
            {
                result = Math.Round(number, numberDigit, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }
    }
}
