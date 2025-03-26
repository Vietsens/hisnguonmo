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

namespace Inventec.Common.BarcodeLib.Symbologies
{
    /// <summary>
    ///  Pharmacode encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class Pharmacode: BarcodeCommon, IBarcode
    {
        string _thinBar = "1";
        string _gap = "00";
        string _thickBar = "111";

        /// <summary>
        /// Encodes with Pharmacode.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Pharmacode(string input)
        {
            Raw_Data = input;

            if (!CheckNumericOnly(Raw_Data))
            {
                Error("EPHARM-1: Data contains invalid  characters (non-numeric).");
            }//if
            else if (Raw_Data.Length > 6)
            {
                Error("EPHARM-2: Data too long (invalid data input length).");
            }//if
        }

        /// <summary>
        /// Encode the raw data using the Pharmacode algorithm.
        /// </summary>
        private string Encode_Pharmacode()
        {
            int num;

            if (!Int32.TryParse(Raw_Data, out num))
            {
                Error("EPHARM-3: Input is unparseable.");
            }
            else if (num < 3 || num > 131070)
            {
                Error("EPHARM-4: Data contains invalid  characters (invalid numeric range).");
            }//if

            int startIndex = 0;

            //find start index
            for (int index = 15; index >= 0; index--)
            { 
                if (Math.Pow(2, index) < num/2)
                {
                    startIndex = index;
                    break;
                }
            }

            double sum = Math.Pow(2, startIndex + 1) - 2;
            string [] encoded = new string[startIndex + 1];
            int i = 0;

            for (int index = startIndex; index >= 0; index--)
            {
                double power = Math.Pow(2, index);
                double diff = num - sum;
                if (diff > power)
                {
                    encoded[i++] = _thickBar;
                    sum += power;
                }
                else
                {
                    encoded[i++] = _thinBar;
                }
            }

            string result = String.Empty;
            foreach (string s in encoded)
            {
                if (result != String.Empty)
                {
                    result += _gap;
                }

                result += s;
            }

            return result;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Pharmacode(); }
        }

        #endregion
    }
}
