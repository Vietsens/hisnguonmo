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

using MPS.ProcessorBase;
namespace MPS.Processor.Mps000461
{
    class Mps000461ExtendSingleKey : CommonKey
    {
        internal const string ADDRESS = "ADDRESS";
        internal const string GENDER = "GENDER";
        internal const string AGE = "AGE";
        internal const string BARCODE_PATIENT_CODE_STR = "PATIENT_CODE_BAR";
        internal const string BARCODE_TREATMENT_CODE_STR = "TREATMENT_CODE_BAR";

        internal const string STR_YEAR = "STR_YEAR";
        internal const string STR_DOB = "STR_DOB";

        internal const string TREATMENT_CODE_BAR_5 = "TREATMENT_CODE_BAR_5";
        internal const string IMG_AVATAR = "IMG_AVATAR";
        internal const string AVT_AND_BHYT_NULL = "AVT_AND_BHYT_NULL";
    }
}
