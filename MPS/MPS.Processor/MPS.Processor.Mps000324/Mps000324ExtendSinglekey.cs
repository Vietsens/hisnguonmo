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

namespace MPS.Processor.Mps000324
{
    class Mps000324ExtendSingleKey : CommonKey
    {
        public const string PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        public const string ORGANIZATION_NAME = "ORGANIZATION_NAME";
        public const string EXECUTE_ROOM_NAME = "EXECUTE_ROOM_NAME";
        public const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        public const string GENDER_MALE = "GENDER_MALE";
        public const string GENDER_FEMALE = "GENDER_FEMALE";
        public const string DOB = "DOB_YEAR";
        public const string AGE = "AGE";
        public const string PROVINCE_NAME = "PROVINCE_NAME";
        public const string WORK_PLACE = "WORK_PLACE";
        public const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        public const string START_TIME_STR = "START_TIME_STR";
        public const string FINISH_TIME_STR = "FINISH_TIME_STR";
        public const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        public const string LOGIN_NAME_EXECUTE_ROLE_MAIN = "LOGIN_NAME_EXECUTE_ROLE_MAIN";
        public const string USERNAME_EXECUTE_ROLE_MAIN = "USERNAME_NAME_EXECUTE_ROLE_MAIN";
        public const string LOGIN_NAME_EXECUTE_ROLE_TT = "LOGIN_NAME_EXECUTE_ROLE_TT";
        public const string USERNAME_EXECUTE_ROLE_TT = "USERNAME_NAME_EXECUTE_ROLE_TT";
        public const string LOGIN_NAME_EXECUTE_ROLE_PM1 = "LOGIN_NAME_EXECUTE_ROLE_PM1";
        public const string USERNAME_EXECUTE_ROLE_PM1 = "USERNAME_EXECUTE_ROLE_TT1";
        public const string LOGIN_NAME_EXECUTE_ROLE_PM2 = "LOGIN_NAME_EXECUTE_ROLE_PM2";
        public const string USERNAME_EXECUTE_ROLE_PM2 = "USERNAME_EXECUTE_ROLE_PM2";
        public const string LOGIN_NAME_EXECUTE_ROLE_PME1 = "LOGIN_NAME_EXECUTE_ROLE_PME1";
        public const string USERNAME_EXECUTE_ROLE_PME1 = "USERNAME_EXECUTE_ROLE_PME1";
        public const string LOGIN_NAME_EXECUTE_ROLE_PME2 = "LOGIN_NAME_EXECUTE_ROLE_PME2";
        public const string USERNAME_EXECUTE_ROLE_PME2 = "USERNAME_EXECUTE_ROLE_PME2";
        public const string LOGIN_NAME_EXECUTE_ROLE_GMHS = "LOGIN_NAME_EXECUTE_ROLE_GMHS";
        public const string USERNAME_EXECUTE_ROLE_GMHS = "USERNAME_EXECUTE_ROLE_GMHS";
        public const string LOGIN_NAME_EXECUTE_ROLE_GV = "LOGIN_NAME_EXECUTE_ROLE_GV";
        public const string USERNAME_EXECUTE_ROLE_GV = "USERNAME_EXECUTE_ROLE_GV";

        #region Key bo sung — mau cu khong dung, chi them vao singleValueDictionary

        /// <summary>Prefix key gop TAT CA ten nguoi cua mot vai kip mo, ngan cach bang ", ".
        /// Khac voi USERNAME_EXECUTE_ROLE_ (chi giu 1 nguoi) nen khong lam doi mau cu.</summary>
        public const string PREFIX_USERNAMES_EXECUTE_ROLE = "USERNAMES_EXECUTE_ROLE_";

        /// <summary>Prefix key gop TAT CA tai khoan cua mot vai kip mo, ngan cach bang ", "</summary>
        public const string PREFIX_LOGIN_NAMES_EXECUTE_ROLE = "LOGIN_NAMES_EXECUTE_ROLE_";

        /// <summary>Prefix key ten vai tro lay tu danh muc HIS_EXECUTE_ROLE — mau in khong viet cung nhan</summary>
        public const string PREFIX_EXECUTE_ROLE_NAME = "EXECUTE_ROLE_NAME_";

        /// <summary>Ten dataset TAT CA vai tro kip mo dua vao FlexCel</summary>
        public const string OBJECT_TAG_EKIP_ROLES = "EkipRoles";

        /// <summary>Ten dataset chi cac vai tro CO nguoi dua vao FlexCel</summary>
        public const string OBJECT_TAG_EKIP_ROLES_USED = "EkipRolesUsed";

        /// <summary>Ten phau thuat vien chinh — xac dinh qua co HIS_EXECUTE_ROLE.IS_SURG_MAIN,
        /// khong phu thuoc ma vai tro nen mau in khong can hardcode</summary>
        public const string SURG_MAIN_USERNAME_STR = "SURG_MAIN_USERNAME_STR";

        /// <summary>Barcode Code128 cua so vao vien (HIS_TREATMENT.IN_CODE)</summary>
        public const string BARCODE_IN_CODE_STR = "BARCODE_IN_CODE_STR";

        /// <summary>Barcode Code128 cua ma dieu tri (HIS_TREATMENT.TREATMENT_CODE)</summary>
        public const string BARCODE_TREATMENT_CODE_STR = "BARCODE_TREATMENT_CODE_STR";

        /// <summary>Ma giuong benh</summary>
        public const string BED_CODE_STR = "BED_CODE_STR";

        /// <summary>Ten giuong benh</summary>
        public const string BED_NAME_STR = "BED_NAME_STR";

        /// <summary>Ten buong benh</summary>
        public const string BED_ROOM_NAME_STR = "BED_ROOM_NAME_STR";

        /// <summary>Ghep "Buong - Giuong" de in o dong Phong/Giuong</summary>
        public const string BED_ROOM_BED_STR = "BED_ROOM_BED_STR";

        /// <summary>Gio bat dau PT/TT dang "08 gio 00 phut, Ngay 27 thang 05 nam 2026"</summary>
        public const string START_TIME_SEPARATE_STR = "START_TIME_SEPARATE_STR";

        /// <summary>Gio ket thuc PT/TT dang "08 gio 00 phut, Ngay 27 thang 05 nam 2026"</summary>
        public const string FINISH_TIME_SEPARATE_STR = "FINISH_TIME_SEPARATE_STR";

        /// <summary>So phieu = SERVICE_REQ_CODE + " - " + NUM_ORDER</summary>
        public const string TICKET_NUMBER_STR = "TICKET_NUMBER_STR";

        /// <summary>Ghi chu PT/TT — lay tu V_HIS_SERE_SERV_PTTT.OTHER</summary>
        public const string PTTT_NOTE_STR = "PTTT_NOTE_STR";

        /// <summary>Ten dich vu PT/TT chinh (sere serv cha)</summary>
        public const string MAIN_SERVICE_NAME_STR = "MAIN_SERVICE_NAME_STR";

        /// <summary>Phuong phap PT/TT thuc te = REAL_PTTT_METHOD_CODE + " " + REAL_PTTT_METHOD_NAME</summary>
        public const string REAL_PTTT_METHOD_STR = "REAL_PTTT_METHOD_STR";

        /// <summary>Tong chi phi cac khoan (cong tat ca cac nhom)</summary>
        public const string GRAND_TOTAL_AMOUNT = "GRAND_TOTAL_AMOUNT";

        /// <summary>Ten dataset nhom dich vu dua vao FlexCel</summary>
        public const string OBJECT_TAG_GROUPS = "Groups";

        /// <summary>Ten dataset dong chi tiet dua vao FlexCel</summary>
        public const string OBJECT_TAG_ITEMS = "Items";

        #endregion
    }
}
