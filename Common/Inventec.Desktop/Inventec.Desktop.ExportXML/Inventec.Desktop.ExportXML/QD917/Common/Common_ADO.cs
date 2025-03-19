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
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.ExportXML.QD917.Common
{
    public class Common_ADO
    {
    }

    public class InputData
    {
        //public string PathSaveXml { get; set; }//Đường dẫn lưu file XML(bắt buộc ng dùng chọn đường dẫn lưu file XML)
        //public string ChuKyDonVi { get; set; }//Chữ ký đơn vị(k bắt buộc)
        //public string MA_CSKCB { get; set; }//Mã cơ sở khám chữa bệnh xuất XML
        //public List<string> L_ICD_CODE_NGOAIDS { get; set; }//danh sách mã bệnh ngoài định suất
        //public List<string> L_ICD_CODE_NGOAIDS_TE { get; set; }//danh sách mã bệnh ngoài định suất trẻ em
        //public Config CONFIG { get; set; }//cấu hình các thông số theo DB của từng viện

        public V_HIS_TREATMENT V_HIS_TREATMENT { get; set; }//Mỗi lần truyền vào 1 hồ sơ điều trị
        public V_HIS_HEIN_APPROVAL_BHYT V_HIS_HEIN_APPROVAL_BHYT { get; set; }//Thông tin 1 thẻ BHYT của 1 bệnh nhân
        public V_HIS_TRAN_PATI V_HIS_TRAN_PATI { get; set; }//lấy thông tin bệnh nhân chuyển viện(chuyển đi, chuyển đến)
        public V_HIS_ACCIDENT_HURT V_HIS_ACCIDENT_HURT { get; set; }//thông tin tai nạn thương tích
        public List<V_HIS_SERE_SERV> L_V_HIS_SERE_SERV { get; set; }//các dịch vụ bệnh nhân đã sử dụng
        public HIS_DHST HIS_DHST { get; set; }//lấy cân nặng với trường hợp là trẻ em dưới 1 tuổi
        //public List<HIS_ICD> L_HIS_ICD { get; set; }//toàn bộ bảng HIS_ICD hoặc truyền vào những mã bệnh thuộc dịch vụ thủ thuật, phẫu thuật trong bảng sere_serv(vì cần lấy mã bệnh theo chuẩn của bộ y tế (trong bảng sere_serv không có mã bệnh chỉ có ID))
        //public List<V_HIS_MEDICINE_TYPE> L_V_HIS_MEDICINE_TYPE { get; set; }//lấy mã thuốc (HIS_SERE_SERV không có mã vật thuốc)
        //public List<V_HIS_MATERIAL_TYPE> L_V_HIS_MATERIAL_TYPE { get; set; }//lấy mã vật tư (HIS_SERE_SERV không có mã vật tư)
        //public List<V_HIS_HEIN_SERVICE_BHYT> L_V_HIS_HEIN_SERVICE_BHYT { get; set; }//lấy mã hoạt chất của thuốc
    }

    public class ResultReturn
    {
        public bool Succeeded { get; set; }
        public object Obj { get; set; }
        public string Result { get; set; }
    }
}
