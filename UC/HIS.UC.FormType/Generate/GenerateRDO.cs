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
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    public class GenerateRDO
    {
        public GenerateRDO() { }

        public object DetailData { get; set; }
        public SAR.EFMODEL.DataModels.V_SAR_REPORT Report { get; set; }
        public DynamicFilterRDO DynamicFilter { get; set; }
    }
    public class DynamicFilterRDO
    {
        /// <summary>
        /// ID của dòng, xác định để truyền gửi dữ liệu
        /// </summary>
        public long? ID { get; set; }
        /// <summary>
        /// Dữ liệu lấy trong cache, ghi theo cấu trúc [tên_bảng]{"ValueMember":"ID", "DisplayCodeMember":"DEPARTMENT_CODE","DisplayNameMember":"DEPARTMENT_NAME","ParentID":"P_ID","ParentCode":"P_CODE"}
        /// </summary>
        public string DATA_CACHE { get; set; }
        /// <summary>
        /// Dữ liệu truy vấn SQL do backend trả về, câu truy vấn phải đặt các as ValueMember, DisplayCodeMember, DisplayNameMember, ParentID, ParentCode
        /// </summary>
        public List<DataTable> DATA_TABLE { get; set; }
        /// <summary>
        /// Thuộc tính của các bộ lọc
        /// </summary>
        public PropetiesRDO Propeties { get; set; }
        /// <summary>
        /// Điều kiện của bộ lọc
        /// </summary>
        public V_SAR_RETY_FOFI Fofi { get; set; }
    }
    public class PropetiesRDO
    {
        /// <summary>
        /// Định dạng: DateEdit
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// Giá trị mặc định: 
        /// Radio, CheckEdit: true/false
        /// TextEdit: string
        /// GridLookUpEdit, GridControl: CODE
        /// DateEdit: Thời gian
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// Loại dữ liệu:
        /// DateTime: Thời gian trong tháng, thời gian hiện tại, ...
        /// </summary>
        public short? Type { get; set; }
        /// <summary>
        /// Giới hạn giá trị:
        /// GridLookUpEdit, GridControl: CODE
        /// </summary>
        public string LimitCode { get; set; }
        /// <summary>
        /// Delegate liên kết control:
        /// ID của Control
        /// </summary>
        public short? IdReference { get; set; }
        /// <summary>
        /// Dữ liệu liên kết truyền:
        /// Đặt là ValueMember, DisplayCodeMember, DisplayNameMember
        /// </summary>
        public string ValueTransfer { get; set; }
        /// <summary>
        /// Dữ liệu liên kết nhận:
        /// Trường dữ liệu này so với ValueTransfer sẽ nhận giá trị cha - con từ ParentId, ParentCode, DisplayCodeMember, DisplayNameMember
        /// </summary>
        public string ValueReceive { get; set; }
    }
}
