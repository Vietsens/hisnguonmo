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
namespace MPS.Processor.Mps000311
{
    class Mps000311ExtendSingleKey : CommonKey
    {
        /*Mã BN: <#PATIENT_CODE;>
Mã ĐT: <#TREATMENT_CODE;>
<#REQUEST_ROOM_NAME;>Họ tên bệnh nhân: <#VIR_PATIENT_NAME;> Năm sinh: <#DOB_YEAR;> Giới tính: <#GENDER_NAME;> 
         * Địa chỉ: <#VIR_ADDRESS;> Đối tượng: <#PATIENT_TYPE_NAME;> Số thẻ BHYT: <#HEIN_CARD_NUMBER_SEPARATE;> 
         * Chẩn đoán: <#ICD_NAME;> Khoa chuyển đến: Tên PTTT: <#TDL_SERVICE_NAME;>																																							
*/
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string TREATMENT_CODE = "TREATMENT_CODE";
        internal const string REQUEST_ROOM_NAME = "REQUEST_ROOM_NAME";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string DOB_YEAR = "DOB_YEAR";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string PATIENT_TYPE_NAME = "PATIENT_TYPE_NAME";
        internal const string HEIN_CARD_NUMBER_1 = "HEIN_CARD_NUMBER_1";
        internal const string HEIN_CARD_NUMBER_2 = "HEIN_CARD_NUMBER_2";
        internal const string HEIN_CARD_NUMBER_3 = "HEIN_CARD_NUMBER_3";
        internal const string HEIN_CARD_NUMBER_4 = "HEIN_CARD_NUMBER_4";
        internal const string HEIN_CARD_NUMBER_5 = "HEIN_CARD_NUMBER_5";
        internal const string HEIN_CARD_NUMBER_6 = "HEIN_CARD_NUMBER_6";
        internal const string ICD_NAME = "ICD_NAME";
        internal const string REQUEST_DEPARTMENT_NAME = "REQUEST_DEPARTMENT_NAME";
        internal const string TDL_SERVICE_NAME = "TDL_SERVICE_NAME";
    }
}
