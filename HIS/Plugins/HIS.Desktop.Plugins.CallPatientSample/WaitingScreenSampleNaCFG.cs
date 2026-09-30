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
    /// Cau hinh man hinh cho lay mau ban Nghe An (frmWaitingScreenSample_NA).
    /// Chi dung 2 key: mot key chon man hinh, mot key gom toan bo phan hien thi.
    /// </summary>
    public class WaitingScreenSampleNaCFG
    {
        /// <summary>
        /// = 1 thi mo man hinh cho ban Nghe An (_NA).
        /// Khac 1 hoac chua co key thi mo man hinh cho hien tai.
        /// </summary>
        public const string WAITING_SCREEN_OPTION = "SAMPLE.WAITING_SCREEN.OPTION";

        /// <summary>
        /// Toan bo cau hinh hien thi cua man hinh cho, gom trong MOT key.
        /// 13 doan ngan cach bang dau |, dung thu tu duoi day. Doan de trong thi dung mac dinh cua giao dien.
        /// Doan mau la 3 so RGB ngan cach bang dau phay. Doan co chu va so la so nguyen.
        ///
        ///  1. Ten to chuc
        ///  2. Mau chu ten phong lay mau
        ///  3. Co chu ten phong lay mau
        ///  4. Mau chu ten nguoi dang nhap
        ///  5. Co chu ten nguoi dang nhap
        ///  6. Mau nen man hinh
        ///  7. Mau nen cac dong trong luoi
        ///  8. Mau nen dong tieu de luoi
        ///  9. Mau chu dong tieu de luoi
        /// 10. Mau chu cac dong trong luoi
        /// 11. Co chu cac dong trong luoi
        /// 12. Chu ky tai lai danh sach, tinh bang GIAY (cung la nhip nhap nhay cua dong dang goi)
        /// 13. So benh nhan toi da hien tren danh sach, bo trong thi lay 10
        ///
        /// Vi du day du:
        /// BENH VIEN HNDK NGHE AN|255,255,0|36|255,255,255|20|0,51,102|0,51,102|0,41,82|255,255,0|255,255,255|20|5|10
        ///
        /// Vi du chi doi co chu va chu ky, con lai de mac dinh:
        /// ||36||20||||||20|5|
        ///
        /// Luu y: dong dang duoc goi luon bi de thanh Arial 29 dam, nen xanh chu vang,
        /// khong chinh duoc bang cau hinh (xem gridViewWaiting_RowStyle).
        /// </summary>
        public const string WAITING_SCREEN_DISPLAY = "SAMPLE.WAITING_SCREEN.DISPLAY";

        private const int IDX_ORGANIZATION_NAME = 0;
        private const int IDX_ROOM_NAME_COLOR = 1;
        private const int IDX_ROOM_NAME_SIZE = 2;
        private const int IDX_USER_NAME_COLOR = 3;
        private const int IDX_USER_NAME_SIZE = 4;
        private const int IDX_PARENT_BACK_COLOR = 5;
        private const int IDX_GRID_BACK_COLOR = 6;
        private const int IDX_GRID_HEADER_BACK_COLOR = 7;
        private const int IDX_GRID_HEADER_FORE_COLOR = 8;
        private const int IDX_GRID_BODY_FORE_COLOR = 9;
        private const int IDX_GRID_BODY_SIZE = 10;
        private const int IDX_TIMER_AUTO_LOAD = 11;
        private const int IDX_SO_BENH_NHAN = 12;

        public static bool IS_USE_WAITING_SCREEN_NA
        {
            get
            {
                string value = GetConfig(WAITING_SCREEN_OPTION);
                return !String.IsNullOrWhiteSpace(value) && value.Trim() == "1";
            }
        }

        public static string ORGANIZATION_NAME
        {
            get { return GetText(IDX_ORGANIZATION_NAME); }
        }

        public static List<int> ROOM_NAME_FORCE_COLOR_CODES
        {
            get { return GetColor(IDX_ROOM_NAME_COLOR); }
        }

        public static int ROOM_NAME_SIZE
        {
            get { return GetNumber(IDX_ROOM_NAME_SIZE); }
        }

        public static List<int> USER_NAME_FORCE_COLOR_CODES
        {
            get { return GetColor(IDX_USER_NAME_COLOR); }
        }

        public static int USER_NAME_SIZE
        {
            get { return GetNumber(IDX_USER_NAME_SIZE); }
        }

        public static List<int> PARENT_BACK_COLOR_CODES
        {
            get { return GetColor(IDX_PARENT_BACK_COLOR); }
        }

        public static List<int> GRID_PATIENTS_BACK_COLOR_CODES
        {
            get { return GetColor(IDX_GRID_BACK_COLOR); }
        }

        public static List<int> GRID_PATIENTS_HEADER_BACK_COLOR_CODES
        {
            get { return GetColor(IDX_GRID_HEADER_BACK_COLOR); }
        }

        public static List<int> GRID_PATIENTS_HEADER_FORCE_COLOR_CODES
        {
            get { return GetColor(IDX_GRID_HEADER_FORE_COLOR); }
        }

        public static List<int> GRID_PATIENTS_BODY_FORCE_COLOR_CODES
        {
            get { return GetColor(IDX_GRID_BODY_FORE_COLOR); }
        }

        public static int PATIENT_BODY_SIZE
        {
            get { return GetNumber(IDX_GRID_BODY_SIZE); }
        }

        public static int TIMER_FOR_AUTO_LOAD_WAITING_SCREENS
        {
            get { return GetNumber(IDX_TIMER_AUTO_LOAD); }
        }

        /// <summary>So benh nhan hien tren danh sach cho, mac dinh 10.</summary>
        public static int SO_BENH_NHAN_HIEN_THI
        {
            get
            {
                int value = GetNumber(IDX_SO_BENH_NHAN);
                return value > 0 ? value : 10;
            }
        }

        /// <summary>Cat gia tri cau hinh hien thi thanh cac doan theo dau |.</summary>
        private static string[] GetSegments()
        {
            try
            {
                string value = GetConfig(WAITING_SCREEN_DISPLAY);
                if (String.IsNullOrWhiteSpace(value))
                {
                    return new string[0];
                }
                return value.Split('|');
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return new string[0];
            }
        }

        private static string GetText(int index)
        {
            string result = "";
            try
            {
                string[] segments = GetSegments();
                if (index < segments.Length)
                {
                    result = segments[index].Trim();
                }
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Doan mau: 3 so RGB ngan cach bang dau phay. Sai dinh dang thi tra ve rong de giu mau thiet ke.</summary>
        private static List<int> GetColor(int index)
        {
            List<int> result = new List<int>();
            try
            {
                string value = GetText(index);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    result = value.Split(',')
                        .Where(o => !String.IsNullOrWhiteSpace(o))
                        .Select(o => Inventec.Common.TypeConvert.Parse.ToInt32(o.Trim()))
                        .ToList();
                }
                if (result.Count != 3)
                {
                    result = new List<int>();
                }
            }
            catch (Exception ex)
            {
                result = new List<int>();
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static int GetNumber(int index)
        {
            int result = 0;
            try
            {
                result = Inventec.Common.TypeConvert.Parse.ToInt32(GetText(index));
            }
            catch (Exception ex)
            {
                result = 0;
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static string GetConfig(string code)
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
    }
}
