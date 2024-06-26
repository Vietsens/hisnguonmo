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
using System.Globalization;
using System.Text;
using System.Linq;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    class ConvertNumber
    {
        public static string ConvertDecToFracByConfig(double dec)
        {
            string result = "";
            try
            {
                if (dec == 0)
                {
                    result = "";
                }
                else if (HisConfigCFG.IsTutorialNumberIsFrac)
                {
                    result = ConvertNumber.Dec2frac(dec);
                }
                else
                {
                    result = dec.ToString();
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.IsTutorialNumberIsFrac", HisConfigCFG.IsTutorialNumberIsFrac) + Inventec.Common.Logging.LogUtil.TraceData("result", result));
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static string ConvertDecToFracByConfig(double dec, int numberDisplaySeperateFormatAmount)
        {
            string result = "";
            try
            {
                if (dec == 0)
                {
                    result = "";
                }
                else if (HisConfigCFG.IsTutorialNumberIsFrac)
                {
                    result = ConvertNumber.Dec2frac(dec);
                }
                else
                {
                    result = Inventec.Common.Number.Convert.NumberToStringRoundAuto((decimal)(dec), numberDisplaySeperateFormatAmount);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("HisConfigCFG.IsTutorialNumberIsFrac", HisConfigCFG.IsTutorialNumberIsFrac) + Inventec.Common.Logging.LogUtil.TraceData("numberDisplaySeperateFormatAmount", numberDisplaySeperateFormatAmount) + Inventec.Common.Logging.LogUtil.TraceData("result", result));
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static double? ConvertStringToNumber(string vl)//strNumber
        {
            double? amountInput = null;
            try
            {
                if (String.IsNullOrEmpty(vl))
                    return amountInput;

                if (vl.Contains(".") || vl.Contains(","))
                {
                    vl = vl.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    vl = vl.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    amountInput = Convert.ToDouble(vl);
                }
                else if (vl.Contains("/"))
                {
                    var arrNumber = vl.Split('/');
                    if (arrNumber != null && arrNumber.Count() > 1)
                    {
                        amountInput = Convert.ToDouble(arrNumber[0]) / Convert.ToDouble(arrNumber[1]);
                    }
                }
                else
                {
                    amountInput = Convert.ToDouble(vl);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return amountInput;
        }

        /// <summary>
        /// Sửa cơ chế lưu dữ liệu vào các trường "sáng", "trưa", "chiều", "tối" trong chi tiết đơn thuốc (morning, noon, afternoon, evening trong his_exp_mest_medicine) để lưu theo định dạng như trên. Cụ thể:
        //- Nếu số nguyên thì ko hiển thị ,00 phía sau. Số nguyên nhỏ hơn 10 thì hiển thị 0 phía trước.
        //vd:
        //+ SL ghi 1,00 --> sửa thành 01.
        //+ SL 12,00 --> sửa thành 12
        //- Số lượng bằng 0 thì để trống ko ghi 0,00 như hiện tại
        //- Số lượng có phần thập phân thì ghi đủ phần thập phân phía sau
        /// </summary>
        /// <param name="dbl"></param>
        /// <returns></returns>
        public static string Dec2frac(double dbl)
        {
            char neg = ' ';
            double dblDecimal = dbl;
            if (dblDecimal == (int)dblDecimal) return ProcessNumberInterger((decimal)dblDecimal); //return no if it's not a decimal
            if (dblDecimal < 0)
            {
                dblDecimal = Math.Abs(dblDecimal);
                neg = '-';
            }
            var whole = (int)Math.Truncate(dblDecimal);
            NumberFormatInfo nfi = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            string decpart = dblDecimal.ToString().Replace(Math.Truncate(dblDecimal) + nfi.NumberDecimalSeparator, "");
            double rN = System.Convert.ToDouble(decpart);
            double rD = Math.Pow(10, decpart.Length);

            string rd = Recur(decpart);
            int rel = System.Convert.ToInt32(rd);
            if (rel != 0)
            {
                rN = rel;
                rD = (int)Math.Pow(10, rd.Length) - 1;
            }
            //just a few prime factors for testing purposes
            var primes = new[] { 47, 43, 37, 31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2 };
            foreach (int i in primes) ReduceNo(i, ref rD, ref rN);

            rN = rN + (whole * rD);
            return string.Format("{0}{1}/{2}", neg, rN, rD);
        }

        public static string Dec2frac(decimal dbl)
        {
            char neg = ' ';
            decimal dblDecimal = dbl;
            if (dblDecimal == (int)dblDecimal) return dblDecimal.ToString(); //return no if it's not a decimal
            if (dblDecimal < 0)
            {
                dblDecimal = Math.Abs(dblDecimal);
                neg = '-';
            }
            var whole = (int)Math.Truncate(dblDecimal);
            NumberFormatInfo nfi = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
            string decpart = dblDecimal.ToString().Replace(Math.Truncate(dblDecimal) + nfi.NumberDecimalSeparator, "");
            double rN = System.Convert.ToDouble(decpart);
            double rD = Math.Pow(10, decpart.Length);

            string rd = Recur(decpart);
            int rel = System.Convert.ToInt32(rd);
            if (rel != 0)
            {
                rN = rel;
                rD = (int)Math.Pow(10, rd.Length) - 1;
            }
            //just a few prime factors for testing purposes
            var primes = new[] { 47, 43, 37, 31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2 };
            foreach (int i in primes) ReduceNo(i, ref rD, ref rN);

            rN = rN + (whole * rD);
            string rs = string.Format("{0}{1}/{2}", neg, rN, rD);

            return rs.Trim();
        }

        internal static string ProcessNumberInterger(decimal dblDecimal)
        {
            string rs = "";
            if (dblDecimal == 0)
            {
                rs = "";
            }
            else if (dblDecimal < 10)
            {
                rs = String.Format("{0:00}", dblDecimal);
            }
            else
            {
                rs = Inventec.Common.Number.Convert.NumberToString((decimal)dblDecimal, 0);
            }
            return rs;
        }

        internal static string ProcessNumberInterger(string sDecimal)
        {
            string rs = "";
            try
            {

                if (String.IsNullOrEmpty(sDecimal) || String.IsNullOrWhiteSpace(sDecimal) || sDecimal.Contains("/"))
                {
                    return sDecimal;
                }

                sDecimal = sDecimal.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                decimal dValue = Inventec.Common.TypeConvert.Parse.ToDecimal(sDecimal);
                if (dValue == (int)dValue)
                {
                    rs = String.Format("{0}", (int)dValue);
                }
                else
                {
                    rs = sDecimal;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sDecimal), sDecimal)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dValue), dValue)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return rs;
        }

        /// <summary>
        /// Finds out the recurring decimal in a specified number
        /// </summary>
        /// <param name="db">Number to check</param>
        /// <returns></returns>
        private static string Recur(string db)
        {
            if (db.Length < 13) return "0";
            var sb = new StringBuilder();
            for (int i = 0; i < 7; i++)
            {
                sb.Append(db[i]);
                int dlength = (db.Length / sb.ToString().Length);
                int occur = Occurence(sb.ToString(), db);
                if (dlength == occur || dlength == occur - sb.ToString().Length)
                {
                    return sb.ToString();
                }
            }
            return "0";
        }

        /// <summary>
        /// Checks for number of occurence of specified no in a number
        /// </summary>
        /// <param name="s">The no to check occurence times</param>
        /// <param name="check">The number where to check this</param>
        /// <returns></returns>
        private static int Occurence(string s, string check)
        {
            int i = 0;
            int d = s.Length;
            string ds = check;
            for (int n = (ds.Length / d); n > 0; n--)
            {
                if (ds.Contains(s))
                {
                    i++;
                    ds = ds.Remove(ds.IndexOf(s), d);
                }
            }
            return i;
        }

        /// <summary>
        /// Reduces a fraction given the numerator and denominator
        /// </summary>
        /// <param name="i">Number to use in an attempt to reduce fraction</param>
        /// <param name="rD">the Denominator</param>
        /// <param name="rN">the Numerator</param>
        private static void ReduceNo(int i, ref double rD, ref double rN)
        {
            //keep reducing until divisibility ends
            while ((rD % i) == 0 && (rN % i) == 0)
            {
                rN = rN / i;
                rD = rD / i;
            }
        }
    }
}
