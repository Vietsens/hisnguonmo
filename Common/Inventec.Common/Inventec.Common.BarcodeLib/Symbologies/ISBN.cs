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
    ///  ISBN encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            Raw_Data = input;
        }
        /// <summary>
        /// Encode the raw data using the Bookland/ISBN algorithm.
        /// </summary>
        private string Encode_ISBN_Bookland()
        {
            if (!CheckNumericOnly(Raw_Data))
                Error("EBOOKLANDISBN-1: Numeric Data Only");

            string type = "UNKNOWN";
            if (Raw_Data.Length == 10 || Raw_Data.Length == 9)
            {
                if (Raw_Data.Length == 10) Raw_Data = Raw_Data.Remove(9, 1);
                Raw_Data = "978" + Raw_Data;
                type = "ISBN";
            }//if
            else if (Raw_Data.Length == 12 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-NOCHECKDIGIT";
            }//else if
            else if (Raw_Data.Length == 13 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-CHECKDIGIT";
                Raw_Data = Raw_Data.Remove(12, 1);
            }//else if

            //check to see if its an unknown type
            if (type == "UNKNOWN") Error("EBOOKLANDISBN-2: Invalid input.  Must start with 978 and be length must be 9, 10, 12, 13 characters.");

            EAN13 ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }//Encode_ISBN_Bookland

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_ISBN_Bookland(); }
        }

        #endregion
    }
}
