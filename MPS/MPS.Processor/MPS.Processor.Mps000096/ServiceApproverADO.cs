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

namespace MPS.Processor.Mps000096
{
    /// <summary>
    /// Key "ServiceApprover": 1 dòng = 1 cặp (nhóm cha dịch vụ, người duyệt KQ).
    /// Nguồn: LIS_SAMPLE.RESULT_APPROVAL_* (duyệt KQ theo mẫu) + nhóm cha (V_HIS_SERVICE.PARENT_ID) của dịch vụ trong mẫu.
    /// </summary>
    public class ServiceApproverADO
    {
        public string APPROVER_LOGINNAME { get; set; }
        public string APPROVER_USERNAME { get; set; }
        public string PARENT_SERVICE_CODE { get; set; }
        public string PARENT_SERVICE_NAME { get; set; }
        /// <summary>Thời gian duyệt muộn nhất của cặp (nhóm cha, người duyệt), yyyyMMddHHmmss.</summary>
        public long APPROVE_TIME { get; set; }
        /// <summary>dd/MM/yyyy HH:mm</summary>
        public string APPROVE_TIME_STR { get; set; }
        /// <summary>Tên các dịch vụ của nhóm do người này duyệt, nối "; ".</summary>
        public string SERVICE_NAMES { get; set; }
    }
}
