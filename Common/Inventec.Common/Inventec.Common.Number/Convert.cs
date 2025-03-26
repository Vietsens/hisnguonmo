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
namespace Inventec.Common.Number
{
    public partial class Convert
    {
        public static bool isShowDecimalOption = false;
        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn không có phần thập phân
        /// Vd: NumberToString(123456.7, 4) = "123,456.7000"; NumberToString(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = NumberToString(number, 0, "");
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến phần thập phân mong muốn
        /// Vd: NumberToString(123456.7, 4) = "123,456.8000"; NumberToString(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToString(decimal number, int numberDigit)
        {
            string result = "";
            try
            {
                result = NumberToString(number, numberDigit, "");
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến phần thập phân mong muốn có truyền vào cultureInfo
        /// Vd: NumberToString(123456.7, 4) = "123,456.8000"; NumberToString(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToString(decimal number, int numberDigit, string cultureInfo)
        {
            string result = "";
            string format = "";
            try
            {
                System.Globalization.CultureInfo globalCultureInfo = (String.IsNullOrWhiteSpace(cultureInfo) ? System.Globalization.CultureInfo.CurrentCulture : System.Globalization.CultureInfo.CreateSpecificCulture(cultureInfo));
                switch (numberDigit)
                {
                    case 0:
                        format = "#,##0";
                        break;
                    case 1:
                        format = "#,##0.0";
                        break;
                    case 2:
                        format = "#,##0.00";
                        break;
                    case 3:
                        format = "#,##0.000";
                        break;
                    case 4:
                        format = "#,##0.0000";
                        break;
                    default:
                        break;
                }
                if (isShowDecimalOption)
                {
                    if (number % 1 == 0)
                        result = number.ToString("#,##0");
                    else
                    {
                        decimal numberRound = NumberToNumberRoundAuto(number, numberDigit);
                        if (numberRound % 1 == 0)
                            result = numberRound.ToString("#,##0");
                        else
                            result = numberRound.ToString(format, globalCultureInfo);
                    }
                }
                else
                    result = NumberToNumberRoundAuto(number, numberDigit).ToString(format, globalCultureInfo);
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến max 4 số sau thập phân
        /// Vd: NumberToStringRoundMax4(123456.81234, "vi") = "123,456.8124"; NumberToStringRoundMax4(123456.81234, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToStringRoundMax4(decimal number)
        {
            string result = "";
            try
            {
                result = NumberToStringRoundAuto(number, 4);
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến phần thập phân mong muốn
        /// Vd: NumberToStringRoundAuto(123456.7, 4) = "123,456.8"; NumberToStringRoundAuto(123456.712, 3) = "123,456.712"; NumberToStringRoundAuto(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToStringRoundAuto(decimal number)
        {
            string result = "";
            try
            {
                result = NumberToStringRoundAuto(number, 0, "");
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến phần thập phân mong muốn
        /// Vd: NumberToStringRoundAuto(123456.7, 4) = "123,456.8"; NumberToStringRoundAuto(123456.712, 3) = "123,456.712"; NumberToStringRoundAuto(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToStringRoundAuto(decimal number, int numberDigit)
        {
            string result = "";
            try
            {
                result = NumberToStringRoundAuto(number, numberDigit, "");
            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm chuyển đổi số sang định dạng chuỗi có tự động làm chòn đến phần thập phân mong muốn có truyền vào cultureInfo
        /// Vd: NumberToStringRoundAuto(123456.7, 4) = "123,456.8"; NumberToStringRoundAuto(123456.712, 3) = "123,456.712"; NumberToStringRoundAuto(123456.7, 0) = "123,457";
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static string NumberToStringRoundAuto(decimal number, int numberDigit, string cultureInfo)
        {
            string result = "";
            string format = "";
            try
            {
                System.Globalization.CultureInfo globalCultureInfo = (String.IsNullOrWhiteSpace(cultureInfo) ? System.Globalization.CultureInfo.CurrentCulture : System.Globalization.CultureInfo.CreateSpecificCulture(cultureInfo));
                switch (numberDigit)
                {
                    case 0:
                        format = "#,##0";
                        break;
                    case 1:
                        format = "#,##0.#";
                        break;
                    case 2:
                        format = "#,##0.##";
                        break;
                    case 3:
                        format = "#,##0.###";
                        break;
                    case 4:
                        format = "#,##0.####";
                        break;
                    default:
                        break;
                }

                if (isShowDecimalOption)
                {
                    if (number % 1 == 0)
                        result = number.ToString("#,##0");
                    else
                    {
                        decimal resultRound = NumberToNumberRoundAuto(number, numberDigit);
                        if (resultRound % 1 == 0)
                            result = resultRound.ToString("#,##0");
                        else
                            result = resultRound.ToString(format, globalCultureInfo);
                    }
                }
                else
                    result = NumberToNumberRoundAuto(number, numberDigit).ToString(format, globalCultureInfo);

            }
            catch (Exception ex)
            {
                result = "";
            }

            return result;
        }

        /// <summary>
        /// Hàm làm chòn số đến tối đa 4 số sau thập phân
        /// Vd: NumberToStringRoundAuto(123456.7, 4) = 123,456.8 ; NumberToStringRoundAuto(123456.712, 3) = 123,456.712 ; NumberToStringRoundAuto(123456.7, 0) = 123,457
        /// </summary>
        /// <param name="number"></param>
        /// <param name="numberDigit"></param>
        /// <returns></returns>
        public static decimal NumberToNumberRoundMax4(decimal number)
        {
            decimal result = 0;
            try
            {
                result = NumberToNumberRoundAuto(number, 4);
            }
            catch (Exception ex)
            {
                result = 0;
            }

            return result;
        }

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

        public static double RoundUpValue(double value, int decimalpoint)
        {
            double result = 0;
            try
            {
                result = Math.Round(value, decimalpoint);
                if (result < value)
                {
                    result += Math.Pow(10, -decimalpoint);
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }
    }
}
