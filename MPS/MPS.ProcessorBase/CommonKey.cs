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

namespace MPS.ProcessorBase
{
    /// <summary>
    /// Danh sach cac the du lieu thuong gap va khong phu thuoc vao nghiep vu dac thu cua tung bieu mau.
    /// Luu y tat ca cac the common phai set vao day de tien theo doi & quan ly, tuyet doi nghiem cam hard code string trong khoi xu ly (processor).
    /// Ten hang so quy dinh dat trung voi ten key.
    /// Cac lop ...SingleKey trong Core deu phai ke thua CommonKey, luu y tranh trung key.
    /// </summary>
    public class CommonKey
    {
        public const string _MEDI_ORG_CODE = "CURRENT_MEDI_ORG_CODE";
        public const string _ORGANIZATION_NAME = "ORGANIZATION_NAME";
        public const string _ORGANIZATION_ADDRESS = "ORGANIZATION_ADDRESS";
        public const string _PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        public const string _CURRENT_TIME_STR = "CURRENT_TIME_STR";
        public const string _CURRENT_DATE_STR = "CURRENT_DATE_STR";
        public const string _CURRENT_MONTH_STR = "CURRENT_MONTH_STR";
        public const string _CURRENT_DATE_SEPARATE_STR = "CURRENT_DATE_SEPARATE_STR";
        public const string _CURRENT_TIME_SEPARATE_STR = "CURRENT_TIME_SEPARATE_STR";
        public const string _CURRENT_TIME_SEPARATE_BEGIN_TIME_STR = "CURRENT_TIME_SEPARATE_BEGIN_TIME_STR";
        public const string _CURRENT_MONTH_SEPARATE_STR = "CURRENT_MONTH_SEPARATE_STR";
        public const string _CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR = "CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR";
        public const string _TOTAL_DAY_TREATMENT = "TOTAL_DAY_TREATMENT"; // Thời gian điều trị
        public const string _CURRENT_LOGINNAME = "CURRENT_LOGINNAME";
        public const string _CURRENT_USERNAME = "CURRENT_USERNAME";
        public const string _CURRENT_LOGO = "CURRENT_LOGO";
        public const string _CURRENT_LOGO_URI = "CURRENT_LOGO_URI";
        public const string _CURRENT_NUM_ORDER_PRINT = "CURRENT_NUM_ORDER_PRINT";
        public const string _CURRENT_COMPUTER_NAME = "CURRENT_COMPUTER_NAME";

        public const string _QRCODE_TREATMENT_CODE_COMMON_KEY = "TREATMENT_QRCODE_COMMON";
        public const string _QRCODE_PATIENT_CODE_COMMON_KEY = "PATIENT_QRCODE_COMMON";
    }
}
