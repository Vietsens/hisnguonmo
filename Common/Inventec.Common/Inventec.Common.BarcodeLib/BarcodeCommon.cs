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
using System.Text;

namespace Inventec.Common.BarcodeLib
{
    abstract class BarcodeCommon
    {
        protected string Raw_Data = "";
        protected List<string> _Errors = new List<string>();

        public string RawData
        {
            get { return this.Raw_Data; }
        }

        public List<string> Errors
        {
            get { return this._Errors; }
        }

        public void Error(string ErrorMessage)
        {
            this._Errors.Add(ErrorMessage);
            throw new Exception(ErrorMessage);
        }

        internal static bool CheckNumericOnly(string Data)
        {
            //This function takes a string of data and breaks it into parts and trys to do Int64.TryParse
            //This will verify that only numeric data is contained in the string passed in.  The complexity below
            //was done to ensure that the minimum number of interations and checks could be performed.

            //early check to see if the whole number can be parsed to improve efficency of this method
            long value = 0;
            if (Data != null)
            {
                if (Int64.TryParse(Data, out value))
                    return true;
            }
            else
            {
                return false;
            }

            //9223372036854775808 is the largest number a 64bit number(signed) can hold so ... make sure its less than that by one place
            int STRING_LENGTHS = 18;

            string temp = Data;
            string[] strings = new string[(Data.Length / STRING_LENGTHS) + ((Data.Length % STRING_LENGTHS == 0) ? 0 : 1)];

            int i = 0;
            while (i < strings.Length)
            {
                if (temp.Length >= STRING_LENGTHS)
                {
                    strings[i++] = temp.Substring(0, STRING_LENGTHS);
                    temp = temp.Substring(STRING_LENGTHS);
                }//if
                else
                    strings[i++] = temp.Substring(0);
            }

            foreach (string s in strings)
            {
                if (!Int64.TryParse(s, out value))
                    return false;
            }//foreach

            return true;
        }//CheckNumericOnly
    }//BarcodeVariables abstract class
}//namespace
