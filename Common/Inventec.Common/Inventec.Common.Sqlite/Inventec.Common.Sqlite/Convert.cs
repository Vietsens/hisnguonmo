/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
