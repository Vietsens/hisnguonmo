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
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
{
    class HisConfigCFG
    {
        internal const string HIS_QD_130_BYT__CONNECTION_INFO = "HIS.QD_130_BYT.CONNECTION_INFO";
        internal const string MOS_BHXH__XML_SIGN_OPTION = "MOS.BHXH.XML_SIGN.OPTION";
        internal const string HIS_QD_130_BYT__LAY_CA_DVU_0_DONG = "HIS.QD_130_BYT.LAY_CA_DVU_0_DONG";
        internal const string HIS_QD_130_BYT_XML1_MA_KHOA_OPTION = "HIS.QD_130_BYT.XML1.MA_KHOA_OPTION";
        internal const string MOS_CSDL_4750__IS_AUTO_SYNC = "MOS.CSDL_4750.IS_AUTO_SYNC";
        internal const string HIS_CSDL_4750__CONNECTION_INFO = "HIS.CSDL_4750.CONNECTION_INFO";
        //Danh sách mã đối tượng thanh toán (patient type code) lọc cho luồng đồng bộ KCB 4750, cách nhau bởi dấu ','. Rỗng = lấy tất cả.
        internal const string MOS_CSDL_4750__PATIENT_TYPE_CODES = "MOS.CSDL_4750.PATIENT_TYPE_CODES";
        //Số hồ sơ tối đa đẩy 4750 mỗi chu kỳ (để không chiếm luồng nền quá lâu, ưu tiên gửi BHYT). Rỗng/không hợp lệ = 200.
        internal const string MOS_CSDL_4750__MAX_PER_CYCLE = "MOS.CSDL_4750.MAX_PER_CYCLE";
        internal static string QD_130_BYT__LAY_CA_DVU_0_DONG;
        internal static string QD_130_BVT_XML1_MA_KHOA_OPTION;
        internal static string QD_130_BYT__CONNECTION_INFO;
        internal static string BHXH__XML_SIGN_OPTION;
        //Bật/tắt toàn bộ liên thông CSDL 4750 (dùng để ẩn/hiện checkbox đồng bộ KCB). "1" = bật
        internal static string CSDL_4750__IS_AUTO_SYNC;
        //Thông tin kết nối CSDL 4750: BaseURL | username | password | loginApi | checkinApi [| examApi | importXmlApi]
        internal static string CSDL_4750__CONNECTION_INFO;
        //Mã đối tượng thanh toán lọc cho luồng KCB 4750 (cách nhau bởi ','). Rỗng/null = lấy tất cả đối tượng.
        internal static string CSDL_4750__PATIENT_TYPE_CODES;
        //Số hồ sơ tối đa đẩy 4750 mỗi chu kỳ. Rỗng/không hợp lệ = 200.
        internal static string CSDL_4750__MAX_PER_CYCLE;

        internal static void LoadConfig()
        {
            try
            {
                QD_130_BYT__CONNECTION_INFO = GetValue(HIS_QD_130_BYT__CONNECTION_INFO);
                BHXH__XML_SIGN_OPTION = GetValue(MOS_BHXH__XML_SIGN_OPTION);
                QD_130_BYT__LAY_CA_DVU_0_DONG = GetValue(HIS_QD_130_BYT__LAY_CA_DVU_0_DONG);
                QD_130_BVT_XML1_MA_KHOA_OPTION = GetValue(HIS_QD_130_BYT_XML1_MA_KHOA_OPTION);
                CSDL_4750__IS_AUTO_SYNC = GetValue(MOS_CSDL_4750__IS_AUTO_SYNC);
                CSDL_4750__CONNECTION_INFO = GetValue(HIS_CSDL_4750__CONNECTION_INFO);
                CSDL_4750__PATIENT_TYPE_CODES = GetValue(MOS_CSDL_4750__PATIENT_TYPE_CODES);
                CSDL_4750__MAX_PER_CYCLE = GetValue(MOS_CSDL_4750__MAX_PER_CYCLE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
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
