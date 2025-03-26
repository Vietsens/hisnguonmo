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

namespace Inventec.Common.String
{
    public class Generate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string DigitRandom(short length)
        {
            string result = null;
            try
            {
                return Random(length, CharacterMode.ONLY_NUMERIC, UpperLowerMode.ANY);

                //Random random = new Random();
                //string format = new System.String('0', length).ToString();
                //if (length == 1) result = random.Next(0, 9).ToString(format);
                //else if (length == 2) result = random.Next(0, 99).ToString(format);
                //else if (length == 3) result = random.Next(0, 999).ToString(format);
                //else if (length == 4) result = random.Next(0, 9999).ToString(format);
                //else if (length == 5) result = random.Next(0, 99999).ToString(format);
                //else if (length == 6) result = random.Next(0, 999999).ToString(format);
                //else if (length == 7) result = random.Next(0, 9999999).ToString(format);
                //else if (length == 8) result = random.Next(0, 99999999).ToString(format);
                //else if (length == 9) result = random.Next(0, 999999999).ToString(format);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public static string Random(short length, CharacterMode charMode, UpperLowerMode upperOrLowerMode)
        {
            string result = null;
            try
            {
                RandomStringGenerator rsg = new RandomStringGenerator();
                RsgSetCharMode(charMode, rsg);
                RsgSetUpperLowerMode(upperOrLowerMode, rsg);
                result = rsg.Generate(length);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public static string Random(short from, short to, CharacterMode charMode, UpperLowerMode upperOrLowerMode)
        {
            string result = null;
            try
            {
                RandomStringGenerator rsg = new RandomStringGenerator();
                RsgSetCharMode(charMode, rsg);
                RsgSetUpperLowerMode(upperOrLowerMode, rsg);
                result = rsg.Generate(from, to);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        private static void RsgSetUpperLowerMode(UpperLowerMode upperOrLowerMode, RandomStringGenerator rsg)
        {
            switch (upperOrLowerMode)
            {
                case UpperLowerMode.ANY:
                    break;
                case UpperLowerMode.UPPER:
                    rsg.UseUpperCaseCharacters = true;
                    break;
                case UpperLowerMode.LOWER:
                    rsg.UseLowerCaseCharacters = true;
                    break;
                default:
                    break;
            }
        }

        private static void RsgSetCharMode(CharacterMode charMode, RandomStringGenerator rsg)
        {
            switch (charMode)
            {
                case CharacterMode.ALL:
                    break;
                case CharacterMode.ONLY_NUMERIC:
                    rsg.UseSpecialCharacters = false;
                    rsg.UseUpperCaseCharacters = false;
                    rsg.UseLowerCaseCharacters = false;
                    break;
                case CharacterMode.ONLY_LETTER:
                    rsg.UseSpecialCharacters = false;
                    rsg.UseNumericCharacters = false;
                    break;
                case CharacterMode.ONLY_SPECIAL:
                    rsg.UseUpperCaseCharacters = false;
                    rsg.UseLowerCaseCharacters = false;
                    rsg.UseNumericCharacters = false;
                    break;
                case CharacterMode.ONLY_LETTER_NUMERIC:
                    rsg.UseSpecialCharacters = false;
                    break;
                case CharacterMode.ONLY_LETTER_SPECIAL:
                    rsg.UseNumericCharacters = false;
                    break;
                case CharacterMode.ONLY_NUMERIC_SPECIAL:
                    rsg.UseUpperCaseCharacters = false;
                    rsg.UseLowerCaseCharacters = false;
                    break;
                default:
                    break;
            }
        }

        public enum CharacterMode
        {
            ALL,
            ONLY_NUMERIC,
            ONLY_LETTER,
            ONLY_SPECIAL,
            ONLY_LETTER_NUMERIC,
            ONLY_LETTER_SPECIAL,
            ONLY_NUMERIC_SPECIAL,
        }

        public enum UpperLowerMode
        {
            ANY,
            UPPER,
            LOWER,
        }
    }
}
