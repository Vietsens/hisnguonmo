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

namespace MPS.Processor.Mps000015
{
    class Mps000015ExtendSingleKey : CommonKey
    {
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string HEIN_CARD_FROM_TIME = "HEIN_CARD_FROM_TIME";
        internal const string HEIN_CARD_TO_TIME = "HEIN_CARD_TO_TIME";
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        internal const string CLOSE_TIME_SEPARATE_STR = "CLOSE_TIME_SEPARATE_STR";
        internal const string HEIN_MEDI_ORG_CODE = "HEIN_MEDI_ORG_CODE";
        internal const string AGE = "AGE";
        internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
        internal const string SERVICE_TYPE_NAME = "SERVICE_TYPE_NAME";
        internal const string NOTE = "NOTE";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string INTRUCTION_TIME_FULL_STR = "INTRUCTION_TIME_FULL_STR";
        internal const string INTRUCTION_DATE_FULL_STR = "INTRUCTION_DATE_FULL_STR";
        internal const string FINISH_TIME_FULL_STR = "FINISH_TIME_FULL_STR";
        internal const string FINISH_DATE_FULL_STR = "FINISH_DATE_FULL_STR";
        internal const string RATIO = "RATIO ";
        internal const string RATIO_STR = "RATIO_STR";
    }
}
