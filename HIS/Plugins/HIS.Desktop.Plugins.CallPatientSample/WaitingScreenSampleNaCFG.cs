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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.CallPatientSample
{
    /// <summary>
    /// Cau hinh rieng cho man hinh cho _NA (tien to SAMPLE.WAITING_SCREEN.*).
    /// Tach khoi WaitingScreenCFG (tien to EXE.WAITING_SCREEN.*) de khong anh huong man hinh cho dang chay.
    /// </summary>
    public class WaitingScreenSampleNaCFG
    {
        /// <summary>
        /// Chua co key (null/rong) hoac = 1 thi mo man hinh cho _NA, gia tri khac thi mo man hinh cho hien tai.
        /// Dang de trong giai doan test nen chua co key cung ra man hinh moi.
        /// </summary>
        public const string WAITING_SCREEN_OPTION = "SAMPLE.WAITING_SCREEN.OPTION";

        //Ten to chuc hien tren man hinh cho. Gia tri: chuoi tu do. Bo trong thi khong hien.
        private const string ORGANIZATION_NAME_CODE = "SAMPLE.WAITING_SCREEN.ORGANIZATION_NAME";

        //Mau chu ten phong lay mau, dong chu "MOI NGUOI BENH" va nhan phu. Gia tri: 3 so RGB cach nhau dau phay, vi du 255,255,0
        private const string ROOM_NAME_COLOR_CODES = "SAMPLE.WAITING_SCREEN.ROOM_NAME.COLOR_CODES";

        //Mau chu ten nguoi dang dang nhap goc tren. Gia tri: 3 so RGB, vi du 255,255,255
        private const string USER_NAME_COLOR_CODES = "SAMPLE.WAITING_SCREEN.USER_NAME.COLOR_CODES";

        //Co chu ten phong lay mau. Gia tri: 1 so nguyen (point), vi du 36. <= 0 hoac bo trong thi giu co chu thiet ke san
        private const string ROOM_NAME_SIZE_CODES = "SAMPLE.WAITING_SCREEN.ROOM_NAME.SIZE_CODES";

        //Co chu ten nguoi dang nhap. Gia tri: 1 so nguyen, vi du 36
        private const string USER_NAME_SIZE_CODES = "SAMPLE.WAITING_SCREEN.USER_NAME.SIZE_CODES";

        //Co chu cac dong trong luoi danh sach cho. Gia tri: 1 so nguyen, vi du 20.
        //Rieng dong dang duoc goi luon bi de len co chu 29 dam (xem gridViewWaiting_RowStyle)
        private const string GRID_PATIENT_BODY_SIZE_CODES = "SAMPLE.WAITING_SCREEN.FONT_SIZE_GRID_PATIENT_BODY.SIZE_CODES";

        //Mau nen toan man hinh. Gia tri: 3 so RGB, vi du 0,51,102
        private const string BACKGROUND_PARENT_COLOR_CODES = "SAMPLE.WAITING_SCREEN.BACKGROUND_PARENT.COLOR_CODES";

        //Mau nen cac dong trong luoi. Gia tri: 3 so RGB
        private const string GRID_PATIENT_BACK_COLOR_CODES = "SAMPLE.WAITING_SCREEN.BACK_COLOR_GRID_PATIENT.COLOR_CODES";

        //Mau nen dong tieu de luoi. Gia tri: 3 so RGB
        private const string GRID_PATIENT_HEADER_BACK_COLOR_CODES = "SAMPLE.WAITING_SCREEN.BACK_COLOR_GRID_PATIENT_HEADER.COLOR_CODES";

        //Mau chu dong tieu de luoi. Gia tri: 3 so RGB
        private const string GRID_PATIENT_HEADER_FORCE_COLOR_CODES = "SAMPLE.WAITING_SCREEN.FORCE_COLOR_GRID_PATIENT_HEADER.COLOR_CODES";

        //Mau chu cac dong trong luoi. Gia tri: 3 so RGB
        private const string GRID_PATIENT_BODY_FORCE_COLOR_CODES = "SAMPLE.WAITING_SCREEN.FORCE_COLOR_GRID_PATIENT_BODY.COLOR_CODES";

        //Mau chu trang thai moi. Gia tri: 3 so RGB. Hien doc ra bien nhung man hinh _NA chua dung den
        private const string NEW_STATUS_FORCE_COLOR_CODES = "SAMPLE.WAITING_SCREEN.FORCE_COLOR_NEW_STATUS.COLOR_CODES";

        //Chu ky tai lai danh sach, tinh bang GIAY. Gia tri: 1 so nguyen, vi du 5.
        //<= 0 hoac bo trong thi giu Interval cua timer trong designer.
        //Luu y: day cung la nhip nhap nhay cua dong dang goi
        private const string TIMER_FOR_AUTO_LOAD_WAITING_SCREEN = "SAMPLE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS";

        //So benh nhan toi da hien tren danh sach cho. Gia tri: 1 so nguyen. Bo trong hoac <= 0 thi lay 10
        private const string SO_BENH_NHAN_TREN_DANH_SACH = "SAMPLE.CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_CLS";

        public static bool IS_USE_WAITING_SCREEN_NA
        {
            get
            {
                string value = GetName(WAITING_SCREEN_OPTION);
                return String.IsNullOrWhiteSpace(value) || value.Trim() == "1";
            }
        }

        public static string ORGANIZATION_NAME
        {
            get { return GetName(ORGANIZATION_NAME_CODE); }
        }

        public static List<int> ROOM_NAME_FORCE_COLOR_CODES
        {
            get { return GetIds(ROOM_NAME_COLOR_CODES); }
        }

        public static List<int> USER_NAME_FORCE_COLOR_CODES
        {
            get { return GetIds(USER_NAME_COLOR_CODES); }
        }

        public static List<int> PARENT_BACK_COLOR_CODES
        {
            get { return GetIds(BACKGROUND_PARENT_COLOR_CODES); }
        }

        public static List<int> GRID_PATIENTS_BACK_COLOR_CODES
        {
            get { return GetIds(GRID_PATIENT_BACK_COLOR_CODES); }
        }

        public static List<int> GRID_PATIENTS_HEADER_BACK_COLOR_CODES
        {
            get { return GetIds(GRID_PATIENT_HEADER_BACK_COLOR_CODES); }
        }

        public static List<int> GRID_PATIENTS_HEADER_FORCE_COLOR_CODES
        {
            get { return GetIds(GRID_PATIENT_HEADER_FORCE_COLOR_CODES); }
        }

        public static List<int> GRID_PATIENTS_BODY_FORCE_COLOR_CODES
        {
            get { return GetIds(GRID_PATIENT_BODY_FORCE_COLOR_CODES); }
        }

        public static List<int> NEW_STATUS_REQUEST_FORCE_COLOR_CODES
        {
            get { return GetIds(NEW_STATUS_FORCE_COLOR_CODES); }
        }

        public static int ROOM_NAME_SIZE
        {
            get { return GetId(ROOM_NAME_SIZE_CODES); }
        }

        public static int USER_NAME_SIZE
        {
            get { return GetId(USER_NAME_SIZE_CODES); }
        }

        public static int PATIENT_BODY_SIZE
        {
            get { return GetId(GRID_PATIENT_BODY_SIZE_CODES); }
        }

        public static int TIMER_FOR_AUTO_LOAD_WAITING_SCREENS
        {
            get { return GetId(TIMER_FOR_AUTO_LOAD_WAITING_SCREEN); }
        }

        /// <summary>So benh nhan hien tren danh sach cho, mac dinh 10.</summary>
        public static int SO_BENH_NHAN_HIEN_THI
        {
            get
            {
                int value = GetId(SO_BENH_NHAN_TREN_DANH_SACH);
                return value > 0 ? value : 10;
            }
        }

        private static string GetName(string code)
        {
            string result = "";
            try
            {
                result = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static List<int> GetIds(string code)
        {
            List<int> result = new List<int>();
            try
            {
                string value = GetName(code);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    result = value.Split(',')
                        .Where(o => !String.IsNullOrWhiteSpace(o))
                        .Select(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.Trim()))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                result = new List<int>();
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static int GetId(string code)
        {
            int result = 0;
            try
            {
                result = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(code);
            }
            catch (Exception ex)
            {
                result = 0;
                LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
