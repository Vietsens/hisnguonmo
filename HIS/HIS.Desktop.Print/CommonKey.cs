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

namespace HIS.Desktop.Print
{
    /// <summary>
    /// Danh sach cac the du lieu thuong gap va khong phu thuoc vao nghiep vu dac thu cua tung bieu mau.
    /// Luu y tat ca cac the common phai set vao day de tien theo doi & quan ly, tuyet doi nghiem cam hard code string trong khoi xu ly (processor).
    /// Ten hang so quy dinh dat trung voi ten key.
    /// Cac lop ...SingleKey trong Core deu phai ke thua CommonKey, luu y tranh trung key.
    /// </summary>
    class CommonKey
    {
        internal const string _ORGANIZATION_NAME = "ORGANIZATION_NAME";
        internal const string _PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        internal const string _CURRENT_TIME_STR = "CURRENT_TIME_STR";
        internal const string _CURRENT_DATE_STR = "CURRENT_DATE_STR";
        internal const string _CURRENT_MONTH_STR = "CURRENT_MONTH_STR";
        internal const string _CURRENT_DATE_SEPARATE_STR = "CURRENT_DATE_SEPARATE_STR";
        internal const string _CURRENT_TIME_SEPARATE_STR = "CURRENT_TIME_SEPARATE_STR";
        internal const string _CURRENT_TIME_SEPARATE_BEGIN_TIME_STR = "CURRENT_TIME_SEPARATE_BEGIN_TIME_STR";
        internal const string _CURRENT_MONTH_SEPARATE_STR = "CURRENT_MONTH_SEPARATE_STR";
        internal const string _CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR = "CURRENT_TIME_SEPARATE_WITHOUT_SECOND_STR";
    }
}
