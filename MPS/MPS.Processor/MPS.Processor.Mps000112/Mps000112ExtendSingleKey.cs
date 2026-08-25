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
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000112
{
    class Mps000112ExtendSingleKey : CommonKey
    {
        internal const string DOB_STR = "DOB_STR";
        internal const string YEAR_STR = "YEAR_STR";
        internal const string AGE_STR = "AGE_STR";
        internal const string AMOUNT = "AMOUNT";
        internal const string AMOUNT_TEXT = "AMOUNT_TEXT";
        internal const string AMOUNT_TEXT_UPPER_FIRST = "AMOUNT_TEXT_UPPER_FIRST";
        internal const string PRINT_COUNT = "PRINT_COUNT";
        internal const string CREATE_DATE_SEPARATE_STR = "CREATE_DATE_SEPARATE_STR";

        internal const string RATIO = "RATIO";
        internal const string RATIO_STR = "RATIO_STR";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string REQUEST_DEPARTMENT_CODE = "REQUEST_DEPARTMENT_CODE";
        internal const string REQUEST_DEPARTMENT_NAME = "REQUEST_DEPARTMENT_NAME";
        internal const string CURRENT_DEPARTMENT_CODE = "CURRENT_DEPARTMENT_CODE";
        internal const string CURRENT_DEPARTMENT_NAME = "CURRENT_DEPARTMENT_NAME";
        internal const string CURRENT_PREVIOUS_DEPARTMENT_CODE = "CURRENT_PREVIOUS_DEPARTMENT_CODE";
        internal const string CURRENT_PREVIOS_DEPARTMENT_NAME = "CURRENT_PREVIOS_DEPARTMENT_NAME";
        internal const string TREATMENT_CODE_BARCODE = "TREATMENT_CODE_BARCODE";
        internal const string NEXT_DEPARTMENT_CODE = "NEXT_DEPARTMENT_CODE";
        internal const string NEXT_DEPARTMENT_NAME = "NEXT_DEPARTMENT_NAME";

        internal const string IN_TREATMENT_TYPE_NAME = "IN_TREATMENT_TYPE_NAME";
        internal const string PATIENT_CLASSIFY_NAME = "PATIENT_CLASSIFY_NAME";

        //Hinh thuc thanh toan
        internal const string PAY_FORM_CODE = "PAY_FORM_CODE";
        internal const string PAY_FORM_NAME = "PAY_FORM_NAME";

        //Band danh sach dich vu
        internal const string OBJECT_TAG__SERVICE = "Service";

        //Tong hop danh sach dich vu
        internal const string SERVICE_COUNT = "SERVICE_COUNT";
        internal const string SERVICE_TOTAL_PRICE = "SERVICE_TOTAL_PRICE";
        internal const string SERVICE_TOTAL_HEIN_PRICE = "SERVICE_TOTAL_HEIN_PRICE";
        internal const string SERVICE_TOTAL_PATIENT_PRICE_BHYT = "SERVICE_TOTAL_PATIENT_PRICE_BHYT";
        internal const string SERVICE_TOTAL_PATIENT_PRICE = "SERVICE_TOTAL_PATIENT_PRICE";
        internal const string SERVICE_TOTAL_DEPOSIT_AMOUNT = "SERVICE_TOTAL_DEPOSIT_AMOUNT";
        internal const string SERVICE_TOTAL_DISCOUNT = "SERVICE_TOTAL_DISCOUNT";

        //Band bang hinh thuc thanh toan + bang chiet khau
        internal const string OBJECT_TAG__PAYFORM = "Payform";
        internal const string OBJECT_TAG__DISCOUNT = "Discount";

        //Tong hop bang hinh thuc thanh toan
        internal const string PAYFORM_COUNT = "PAYFORM_COUNT";
        internal const string PAYFORM_TOTAL_AMOUNT = "PAYFORM_TOTAL_AMOUNT";
        internal const string PAYFORM_TOTAL_SURCHARGE = "PAYFORM_TOTAL_SURCHARGE";
        internal const string PAYFORM_TOTAL = "PAYFORM_TOTAL";

        //Tong hop bang chiet khau
        internal const string DISCOUNT_COUNT = "DISCOUNT_COUNT";
        internal const string DISCOUNT_TOTAL = "DISCOUNT_TOTAL";
    }
}
