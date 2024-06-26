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

namespace MPS.Processor.Mps000382
{
    class Mps000011ExtendSingleKey : CommonKey
    {
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string HEIN_CARD_FROM_TIME = "HEIN_CARD_FROM_TIME";
        internal const string HEIN_CARD_TO_TIME = "HEIN_CARD_TO_TIME";
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        internal const string CLOSE_TIME_SEPARATE_STR = "CLOSE_TIME_SEPARATE_STR";
        internal const string IS_NOT_HEIN = "IS_NOT_HEIN";
        internal const string IS_HEIN = "IS_HEIN";
        internal const string RIGHT_ROUTE_TYPE_NAME_CC = "RIGHT_ROUTE_TYPE_NAME_CC";
        internal const string RIGHT_ROUTE_TYPE_NAME = "RIGHT_ROUTE_TYPE_NAME";
        internal const string NOT_RIGHT_ROUTE_TYPE_NAME = "NOT_RIGHT_ROUTE_TYPE_NAME";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string METHOD = "METHOD";
        internal const string ADVISE = "ADVISE";
        internal const string ICD_MAIN_CODE = "ICD_MAIN_CODE";
        internal const string ICD_MAIN_TEXT = "ICD_MAIN_TEXT";
        internal const string AGE = "AGE";
        internal const string END_ORDER = "END_ORDER";
        internal const string DATE_TIME_APPOINT = "DATE_TIME_APPOINT";
        internal const string MEDI_ORG_TO_NAME = "MEDI_ORG_TO_NAME";
        internal const string HEIN_MEDI_ORG_CODE = "HEIN_MEDI_ORG_CODE";
        internal const string DAU_HIEU_LAM_SANG = "DAUHIEULAMSANG";
        internal const string XET_NGHIEM = "XETNGHIEM";
        internal const string THUOC_DA_DUNG = "THUOCDADUNG";
        internal const string HUONG_DIEU_TRI = "HUONGDIEUTRI";
        internal const string TINH_TRANG = "TINHTRANG";
        internal const string PHUONG_TIEN_CHUYEN = "PHUONGTIENCHUYEN";
        internal const string NGUOI_HO_TONG = "NGUOIHOTONG";
        internal const string HINH_THUC_CHUYEN = "HINHTHUCCHUYEN";
        internal const string OUT_CODE = "OUT_CODE";
        internal const string OUT_ORDER = "OUT_ORDER";
        internal const string BARCODE_PATIENT_CODE_STR = "BARCODE_PATIENT_CODE_STR";
        internal const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE_STR";

        internal const string ADDRESS = "ADDRESS";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string DOB = "DOB";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string STR_DOB = "STR_DOB";
        internal const string STR_YEAR = "STR_YEAR";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";
        internal const string TREATMENT_RESULT_CODE = "TREATMENT_RESULT_CODE";
        internal const string TREATMENT_RESULT_NAME = "TREATMENT_RESULT_NAME";
    }
}
