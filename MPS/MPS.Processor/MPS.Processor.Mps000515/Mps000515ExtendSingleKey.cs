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

namespace MPS.Processor.Mps000515
{
    class Mps000515ExtendSingleKey : CommonKey
    {
        internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string QRCODE_PATIENT = "QRCODE_PATIENT";

        internal const string HEIN_CARD_NUMBER_SEPERATOR = "HEIN_CARD_NUMBER_SEPERATOR";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string FROM_DATE_STR = "FROM_DATE_STR";
        internal const string TO_DATE_STR = "TO_DATE_STR";

        internal const string LOGIN_USER_NAME = "LOGIN_USER_NAME";
        internal const string LOGIN_LOGIN_NAME = "LOGIN_LOGIN_NAME";

        internal const string GATE = "GATE";
        internal const string PRINT_TIME_FULL_STR = "PRINT_TIME_FULL_STR";
    }
}
