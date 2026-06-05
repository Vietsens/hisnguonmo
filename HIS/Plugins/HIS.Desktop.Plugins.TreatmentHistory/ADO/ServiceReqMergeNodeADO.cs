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
using MOS.EFMODEL.DataModels;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.TreatmentHistory.ADO
{
    /// <summary>
    /// Dữ liệu gắn vào node LÁ của cây gộp (Grid 2 chế độ "Gộp kết quả KCB").
    /// Một node lá đại diện cho một mốc y lệnh (INTRUCTION_TIME) trong một nhóm,
    /// chứa tập HIS_SERVICE_REQ để xác định chi tiết dịch vụ hiển thị ở Grid 3.
    /// </summary>
    public class ServiceReqMergeNodeADO
    {
        /// <summary>Các y lệnh thuộc node lá (cùng nhóm cha + cùng INTRUCTION_TIME).</summary>
        public List<HIS_SERVICE_REQ> ServiceReqs { get; set; }

        /// <summary>Tập SERVICE_REQ_CODE dùng để lọc danh sách chi tiết khi nạp Grid 3.</summary>
        public HashSet<string> ServiceReqCodes { get; set; }

        /// <summary>
        /// Khoa thực hiện đại diện truyền cho TreeSereServ7Processor.Reload.
        /// Root A: khoa của nhóm. Root B: 0 (gom xuyên khoa).
        /// </summary>
        public long ExecuteDepartmentId { get; set; }

        public ServiceReqMergeNodeADO()
        {
            this.ServiceReqs = new List<HIS_SERVICE_REQ>();
            this.ServiceReqCodes = new HashSet<string>();
        }
    }
}
