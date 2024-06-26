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

namespace MPS.Processor.Mps000084
{
  class Mps000084ExtendSingleKey : CommonKey
  {
    internal const string DOB_STR = "DOB_STR";
    internal const string INTRUCTION_TIME_STR = "INTRUCTION_TIME_STR";
    internal const string HEIN_CARD_NUMBER_SEPERATOR = "HEIN_CARD_NUMBER_SEPERATOR";
    internal const string FROM_DATE_STR = "FROM_DATE_STR";
    internal const string TO_DATE_STR = "TO_DATE_STR";
    internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE";
    internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE";
    internal const string IMP_TIME_STR = "IMP_TIME_STR";
    internal const string SUM_EXP_PRICE = "SUM_EXP_PRICE";
    internal const string SUM_IMP_PRICE = "SUM_IMP_PRICE";
    internal const string SUM_EXP_PRICE_NUM = "SUM_EXP_PRICE_NUM";
    internal const string SUM_IMP_PRICE_NUM = "SUM_IMP_PRICE_NUM";
    internal const string SUM_EXP_PRICE_TEXT = "SUM_EXP_PRICE_TEXT";
    internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
    internal const string BARCODE_IMP_MEST_CODE = "BARCODE_IMP_MEST_CODE";
  }
}
