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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Inventec.Common.String
{
    public class Convert
    {
        public static readonly string specialUnicodeText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
        public static string HexToUTF8(string hexInput)
        {
            string result = "";
            try
            {
                int numberChars = hexInput.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = System.Convert.ToByte(hexInput.Substring(i, 2), 16);
                }
                result = System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public static string HexToUTF8Fix(string hexInput)
        {
            string result = "";
            try
            {
                hexInput = FixUnSignUnicodeError(hexInput);
                int numberChars = hexInput.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = System.Convert.ToByte(hexInput.Substring(i, 2), 16);
                }

                result = GetUnicodeString(bytes);
            }
            catch (Exception ex)
            {
                result = "";
            }
            return result;
        }

        public static string GetUnicodeString(byte[] byteInput)
        {
            string result = "";

            result = System.Text.Encoding.UTF8.GetString(byteInput);

            if (!IsLetterOrDigitOrUnicode(result))
            {
                result = System.Text.Encoding.Unicode.GetString(byteInput);
            }

            return result;
        }

        public static bool IsLetterOrDigitOrUnicode(string text)
        {
            bool valid = true;
            try
            {
                Regex r = new Regex("^[a-zA-Z0-9--\\|/^!~?#@_&';:.,()�\"ŭếÐŏýặìịĭ&*]*$");
                string unUnicodeText = Inventec.Common.String.Convert.UnSignVNese2(text);

                valid = valid && r.IsMatch(unUnicodeText.Replace("   ", "").Replace("  ", "").Replace(" ", ""));
            }
            catch (Exception ex)
            {
                valid = false;
            }
            return valid;
        }

        public static string FixUnSignUnicodeError(string text)
        {
            string result = text;
            try
            {

                List<string> ReplText = new List<string>(new string[]
	            {
	                "as",//á
	                "af",//à
	                "ar",//ả
                    "ax",//ã
	                "aj",//ạ
	                "aa",//â
                    "aas",//ấ
                    "aaf",//ầ
                    "aar",//ẩ
                    "aax",//ẫ
                    "aaj",//ậ
                    "aw",//ă
                    "aws",//ắ
                    "awf",//ằ
                    "awr",//ẳ
                    "awx",//ẵ
                    "awj",//ặ
                    "dd",//đ
                    "es",//é
                    "ef",//è
                    "ar",//ẻ
                    "ex",//ẽ
                    "ej",//ẹ
                    "ee",//ê
                    "ees",//ế
                    "eef",//ề
                    "eer",//ể
                    "eex",//ễ
                    "eej",//ệ
                    "is",//í
                    "if",//ì
                    "ir",//ỉ
                    "ix",//ĩ
                    "ij",//ị
                    "os",//ó
                    "of",//ò
                    "or",//ỏ
                    "ox",//õ
                    "oj",//ọ
                    "oo",//ô
                    "oos",//ố
                    "oof",//ồ
                    "oor",//ổ
                    "oox",//ỗ
                    "ooj",//ộ
                    "ow",//ơ
                    "ows",//ớ
                    "owf",//ờ
                    "owr",//ở
                    "owx",//ỡ
                    "owj",//ợ
                    "us",//ú
                    "uf",//ù
                    "ur",//ủ
                    "ux",//ũ
                    "uj",//ụ
                    "uw",//ư
                    "uws",//ứ
                    "uwf",//ừ
                    "uwr",//ử
                    "uwx",//ữ
                    "uwj",//ự
                    "ys",//ý
                    "yf",//ỳ
                    "yr",//ỷ
                    "yx",//ỹ
                    "yj",//ỵ
                    "AS",//Á
                    "AF",//À
                    "AR",//Ả
                    "AX",//Ã
                    "AJ",//Ạ
                    "AA",//Â
                    "AAS",//Ấ
                    "AAF",//Ầ
                    "AAR",//Ẩ
                    "AAX",//Ẫ
                    "AAJ",//Ậ
                    "AW",//Ă
                    "AWS",//Ắ
                    "AWF",//Ằ
                    "AWR",//Ẳ
                    "AWX",//Ẵ
                    "AWJ",//Ặ
                    "DD",//Đ
                    "ES",//É
                    "EF",//È
                    "ER",//Ẻ
                    "EX",//Ẽ
                    "EJ",//Ẹ
                    "EE",//Ê
                    "EES",//Ế
                    "EEF",//Ề
                    "EER",//Ể
                    "EEX",//Ễ
                    "EEJ",//Ệ
                    "IS",//Í
                    "IF",//Ì
                    "IR",//Ỉ
                    "IX",//Ĩ
                    "IJ",//Ị
                    "OS",//Ó
                    "OF",//Ò
                    "OR",//Ỏ
                    "OX",//Õ
                    "OJ",//Ọ
                    "OO",//Ô
                    "OOS",//Ố
                    "OOF",//Ồ
                    "OOR",//Ổ
                    "OOX",//Ỗ
                    "OOJ",//Ộ
                    "OW",//Ơ
                    "OWS",//Ớ
                    "OWF",//Ờ
                    "OWR",//Ở
                    "OWX",//Ỡ
                    "OWJ",//Ợ
                    "US",//Ú
                    "UF",//Ù
                    "UR",//Ủ
                    "UX",//Ũ
                    "UJ",//Ụ
                    "UW",//Ư
                    "UWS",//Ứ
                    "UWF",//Ừ
                    "UWR",//Ử
                    "UWX",//Ữ
                    "UWJ",//Ự
                    "YS",//Ý
                    "YF",//Ỳ
                    "YR",//Ỷ
                    "YX",//Ỹ
                    "YJ",//Ỵ
	            });

                int index = -1;
                char[] arrChar = specialUnicodeText.ToCharArray();
                while ((index = result.IndexOfAny(arrChar)) != -1)
                {
                    int index2 = specialUnicodeText.IndexOf(result[index]);
                    result = result.Replace(result[index].ToString(), (ReplText[index2]));
                }
            }
            catch (Exception)
            {
                result = text;
            }
            return result;
        }

        public static string HexToUnicode(string hexInput)
        {
            string result;
            try
            {
                int numberChars = hexInput.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = System.Convert.ToByte(hexInput.Substring(i, 2), 16);
                }
                result = System.Text.Encoding.Unicode.GetString(bytes);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Bo dau chuoi ky tu tieng Viet.
        /// Neu co exception thi tra lai chuoi ban dau.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UnSignVNese(string text)
        {
            string result = text;
            try
            {
                const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
                int index = -1;
                char[] arrChar = specialUnicodeText.ToCharArray();
                while ((index = result.IndexOfAny(arrChar)) != -1)
                {
                    int index2 = specialUnicodeText.IndexOf(result[index]);
                    result = result.Replace(result[index], ReplText[index2]);
                }
            }
            catch (Exception)
            {
                result = text;
            }
            return result;
        }

        /// <summary>
        /// Bo dau chuoi ky tu tieng Viet.
        /// Neu co exception thi tra lai chuoi ban dau.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UnSignVNese2(string text)
        {
            string result = text;
            try
            {
                if (!System.String.IsNullOrWhiteSpace(result))
                {
                    for (int i = 33; i < 48; i++)
                    {
                        result = result.Replace(((char)i).ToString(), "");
                    }
                    for (int i = 58; i < 65; i++)
                    {
                        result = result.Replace(((char)i).ToString(), "");
                    }
                    for (int i = 91; i < 97; i++)
                    {
                        result = result.Replace(((char)i).ToString(), "");
                    }
                    for (int i = 123; i < 127; i++)
                    {
                        result = result.Replace(((char)i).ToString(), "");
                    }
                    Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
                    string strFormD = result.Normalize(System.Text.NormalizationForm.FormD);
                    result = regex.Replace(strFormD, System.String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                }
            }
            catch (Exception)
            {
                result = text;
            }
            return result;
        }

        public static string CurrencyToVneseString(string number)
        {
            string result = "";
            try
            {
                result = CurrencyToVneseStringNoUpcaseCustom(number, false);
                result = UppercaseFirst(result);
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        public static string CurrencyToVneseStringNoUpcase(string number)
        {
            string result;
            try
            {
                string[] strTachPhanSauDauPhay;
                if (number.Contains('.') || number.Contains(','))
                {
                    strTachPhanSauDauPhay = number.Split('.', ',');
                    return (CurrencyToVneseStringNoUpcase(strTachPhanSauDauPhay[0]) + "phẩy " + CurrencyToVneseStringNoUpcase(strTachPhanSauDauPhay[1]));
                }

                string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
                string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

                int i, j, k, n, len, found, ddv, rd;

                len = number.Length;
                number += "ss";
                result = "";
                found = 0;
                ddv = 0;
                rd = 0;

                i = 0;
                while (i < len)
                {
                    //So chu so o hang dang duyet
                    n = (len - i + 2) % 3 + 1;

                    //Kiem tra so 0
                    found = 0;
                    for (j = 0; j < n; j++)
                    {
                        if (number[i + j] != '0')
                        {
                            found = 1;
                            break;
                        }
                    }

                    //Duyet n chu so
                    if (found == 1)
                    {
                        rd = 1;
                        for (j = 0; j < n; j++)
                        {
                            ddv = 1;
                            switch (number[i + j])
                            {
                                case '0':
                                    if (n - j == 3) result += cs[0] + " ";
                                    if (n - j == 2)
                                    {
                                        if (number[i + j + 1] != '0')
                                        {
                                            result += "linh ";
                                        }
                                        ddv = 0;
                                    }
                                    break;
                                case '1':
                                    if (n - j == 3) result += cs[1] + " ";
                                    if (n - j == 2)
                                    {
                                        result += "mười ";
                                        ddv = 0;
                                    }
                                    if (n - j == 1)
                                    {
                                        if (i + j == 0) k = 0;
                                        else k = i + j - 1;

                                        if (number[k] != '1' && number[k] != '0')
                                            result += "mốt ";
                                        else
                                            result += cs[1] + " ";
                                    }
                                    break;
                                case '5':
                                    if (result.Trim().EndsWith("mươi"))
                                        result += "lăm ";
                                    else if (result.Trim().EndsWith("linh"))
                                        result += "năm ";
                                    else if ((i + j == len - 1))
                                    {
                                        result += "lăm ";
                                    }
                                    else if ((i + j + 3 == len - 1))
                                        result += "năm ";
                                    else
                                        result += cs[5] + " ";
                                    break;
                                default:
                                    result += cs[(int)number[i + j] - 48] + " ";
                                    break;
                            }

                            //Doc don vi nho
                            if (ddv == 1)
                            {
                                result += ((n - j) != 1) ? dv[n - j - 1] + " " : dv[n - j - 1];
                            }
                        }
                    }


                    //Doc don vi lon
                    if (len - i - n > 0)
                    {
                        if ((len - i - n) % 9 == 0)
                        {
                            if (rd == 1)
                                for (k = 0; k < (len - i - n) / 9; k++)
                                    result += "tỉ ";
                            rd = 0;
                        }
                        else
                            if (found != 0) result += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                    }

                    i += n;
                }

                if (len == 1)
                    if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        static string CurrencyToVneseStringNoUpcaseCustom(string number, bool laPhanThapPhan)
        {
            string result;
            try
            {
                string[] strTachPhanSauDauPhay;
                if (number.Contains('.') || number.Contains(','))
                {
                    strTachPhanSauDauPhay = number.Split('.', ',');
                    return (CurrencyToVneseStringNoUpcaseCustom(strTachPhanSauDauPhay[0], false) + "phẩy " + CurrencyToVneseStringNoUpcaseCustom(strTachPhanSauDauPhay[1], true));
                }

                string[] dv = { "", "mươi", "trăm", "nghìn", "triệu", "tỉ" };
                string[] cs = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

                int i, j, k, n, len, found, ddv, rd;

                len = number.Length;
                number += "ss";
                result = "";
                found = 0;
                ddv = 0;
                rd = 0;

                i = 0;
                while (i < len)
                {
                    //So chu so o hang dang duyet
                    n = (len - i + 2) % 3 + 1;

                    //Kiem tra so 0
                    found = 0;
                    for (j = 0; j < n; j++)
                    {
                        if (number[i + j] != '0')
                        {
                            found = 1;
                            break;
                        }
                    }

                    //Duyet n chu so
                    if (found == 1)
                    {
                        rd = 1;
                        for (j = 0; j < n; j++)
                        {
                            ddv = 1;
                            switch (number[i + j])
                            {
                                case '0':
                                    if (n - j == 3) result += cs[0] + " ";
                                    if (n - j == 2)
                                    {
                                        if (number[i + j + 1] != '0')
                                        {
                                            if (laPhanThapPhan && n == 2)
                                                result += "không ";
                                            else result += "linh ";
                                        }
                                        ddv = 0;
                                    }
                                    break;
                                case '1':
                                    if (n - j == 3) result += cs[1] + " ";
                                    if (n - j == 2)
                                    {
                                        result += "mười ";
                                        ddv = 0;
                                    }
                                    if (n - j == 1)
                                    {
                                        if (i + j == 0) k = 0;
                                        else k = i + j - 1;

                                        if (number[k] != '1' && number[k] != '0')
                                            result += "mốt ";
                                        else
                                            result += cs[1] + " ";
                                    }
                                    break;
                                case '5':
                                    if (result.Trim().ToLower().EndsWith("mười") || result.Trim().ToLower().EndsWith("mươi"))// code cũ result.Trim().EndsWith("mươi")
                                        result += "lăm ";
                                    else if (result.Trim().EndsWith("linh"))
                                        result += "năm ";
                                    else if ((i + j == len - 1))
                                    {
                                        if (laPhanThapPhan)
                                            result += "năm ";
                                        else
                                            result += "lăm ";
                                    }
                                    else if ((i + j + 3 == len - 1))
                                        result += "năm ";
                                    else
                                        result += cs[5] + " ";
                                    break;
                                default:
                                    result += cs[(int)number[i + j] - 48] + " ";
                                    break;
                            }

                            //Doc don vi nho
                            if (ddv == 1)
                            {
                                result += ((n - j) != 1) ? dv[n - j - 1] + " " : dv[n - j - 1];
                            }
                        }
                    }


                    //Doc don vi lon
                    if (len - i - n > 0)
                    {
                        if ((len - i - n) % 9 == 0)
                        {
                            if (rd == 1)
                                for (k = 0; k < (len - i - n) / 9; k++)
                                    result += "tỉ ";
                            rd = 0;
                        }
                        else
                            if (found != 0) result += dv[((len - i - n + 1) % 9) / 3 + 2] + " ";
                    }

                    i += n;
                }

                if (len == 1)
                    if (number[0] == '0' || number[0] == '5') return cs[(int)number[0] - 48];
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        public static string UppercaseFirst(string s)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                {
                    return string.Empty;
                }

                // Return char and concat substring.
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            catch (Exception ex)
            {

            }
            // Check for empty string.
            return "";
        }

        public static string FirstCharToUpper(string input)
        {
            try
            {
                if (!System.String.IsNullOrEmpty(input))
                    return (input.First().ToString().ToUpper() + System.String.Join("", input.Skip(1)));
            }
            catch (Exception)
            {
                input = "";
            }
            return input;
        }

        public string HexToAscii(string hexString)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    string hs = hexString.Substring(i, i + 2);
                    System.Convert.ToChar(System.Convert.ToUInt32(hexString.Substring(0, 2), 16)).ToString();
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
