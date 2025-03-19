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
    ///  FIM encoding
    ///  Written by: Brad Barnhill
    /// </summary>
    class FIM: BarcodeCommon, IBarcode
    {
        private string[] FIM_Codes = { "110010011", "101101101", "110101011", "111010111" };
        public enum FIMTypes {FIM_A = 0, FIM_B, FIM_C, FIM_D};

        public FIM(string input)
        {
            input = input.Trim();

            switch (input)
            {
                case "A":
                case "a": Raw_Data = FIM_Codes[(int)FIMTypes.FIM_A];
                    break;
                case "B":
                case "b": Raw_Data = FIM_Codes[(int)FIMTypes.FIM_B];
                    break;
                case "C":
                case "c": Raw_Data = FIM_Codes[(int)FIMTypes.FIM_C];
                    break;
                case "D":
                case "d": Raw_Data = FIM_Codes[(int)FIMTypes.FIM_D];
                    break;
                default: Error("EFIM-1: Could not determine encoding type. (Only pass in A, B, C, or D)");
                    break;
            }//switch
        }

        public string Encode_FIM()
        {
            string Encoded = "";
            foreach (char c in RawData)
            {
                Encoded += c + "0";
            }//foreach

            Encoded = Encoded.Substring(0, Encoded.Length - 1);

            return Encoded;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_FIM(); }
        }

        #endregion
    }
}
