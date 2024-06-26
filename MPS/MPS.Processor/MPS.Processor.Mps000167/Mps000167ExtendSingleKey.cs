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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000167
{
    class Mps000167ExtendSingleKey : CommonKey
    {
        internal static string IS_NOT_SURGERY = "IS_NOT_SURGERY";
        internal static string IS_SURGERY = "IS_SURGERY";
        internal static string AGE = "AGE";
        internal static string BED_ROOM_NAME = "BED_ROOM_NAME";
        internal static string HEIN_CARD_FROM_TIME_STR = "HEIN_CARD_FROM_TIME_STR";
        internal static string HEIN_CARD_TO_TIME_STR = "HEIN_CARD_TO_TIME_STR";
        internal static string LIQUID_TIME_STR = "LIQUID_TIME_STR";
        internal static string INSTRUCTION_TIME_STR = "INSTRUCTION_TIME_STR";
        internal static string SERVICE_CODE = "SERVICE_CODE";
        internal static string SERVICE_NAME = "SERVICE_NAME";
        internal static string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string DOB = "DOB";
        internal const string ADDRESS = "ADDRESS";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string DISTRICT_CODE = "DISTRICT_CODE";
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string QRCODE_PATIENT = "QRCODE_PATIENT";
        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal const string SERVICE_REQ_CODE_BAR = "SERVICE_REQ_CODE_BAR";
        internal static string SERE_SERV_PATIENT_TYPE_NAME = "SERE_SERV_PATIENT_TYPE_NAME";

        internal static string ASSIGN_TURN_BAR = "ASSIGN_TURN_BAR";

        internal const string CARD_CODE = "CARD_CODE";
        internal const string BANK_CARD_CODE = "BANK_CARD_CODE";
        internal const string PAYMENT_AMOUNT = "PAYMENT_AMOUNT";

    }
}
