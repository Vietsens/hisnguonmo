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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientNumOrder.Config
{
    class HisConfigCFG
    {
        private const string CFG_ORGANIZATION_NAME_COLOR_CODES = "EXE.WAITING_SCREEN.ORGANIZATION_NAME.COLOR_CODES";//Mã màu tổ chức
        private const string CFG_ROOM_NAM_COLOR_CODES = "EXE.WAITING_SCREEN.ROOM_NAME.COLOR_CODES";//Mã màu chữ phòng
        private const string CFG_CALL_PATIENT_NUM_ORDER_DO_CAO_TITLE = "HIS.CALL_PATIENT_NUM_ORDER.DO_CAO.TITLE";//Chiều cao dòng chữ
        private const string CFG_CALL_PATIENT_NUM_ORDER_DO_CAO_NUM_ORDER = "HIS.CALL_PATIENT_NUM_ORDER.DO_CAO.NUN_ORDER"; //chiều cao stt
        private const string CFG_CALL_PATIENT_NUM_ORDER_BACKGROUND_IMAGE = "HIS.CALL_PATIENT_NUM_ORDER.BACKGROUND_IMAGE";//Tên ảnh background của màn hình gọi số tiếp đón

        internal static List<int> ORGANIZATION_NAME_COLOR_CODES = null;
        internal static List<int> ROOM_NAM_COLOR_CODES = null;
        internal static int TITLE_SIZE;
        internal static int NUM_ORDER_SIZE;
        internal static string BACKGROUND_IMAGE;

        internal static void LoadConfigs()
        {
            try
            {
                ORGANIZATION_NAME_COLOR_CODES = GetIds(CFG_ORGANIZATION_NAME_COLOR_CODES);
                ROOM_NAM_COLOR_CODES = GetIds(CFG_ROOM_NAM_COLOR_CODES);
                TITLE_SIZE = HisConfigs.Get<int>(CFG_CALL_PATIENT_NUM_ORDER_DO_CAO_TITLE);
                NUM_ORDER_SIZE = HisConfigs.Get<int>(CFG_CALL_PATIENT_NUM_ORDER_DO_CAO_NUM_ORDER);
                BACKGROUND_IMAGE = HisConfigs.Get<string>(CFG_CALL_PATIENT_NUM_ORDER_BACKGROUND_IMAGE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static List<int> GetIds(string code)
        {
            List<int> result = new List<int>();
            try
            {
                string value = HisConfigs.Get<string>(code);
                string pattern = ",";
                Regex myRegex = new Regex(pattern);
                string[] Codes = myRegex.Split(value);
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(int.Parse(item));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

    }
}
