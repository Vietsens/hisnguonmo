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

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO
{
    class ValidationRequestADO
    {
        /// <summary>
        /// Guid cho từng máy/người dùng
        /// </summary>
        public string sessionID { get; set; }

        /// <summary>
        /// Thông tin chính của từng đơn thuốc
        /// </summary>
        public DrugPatientInfoADO prescriptionInfo { get; set; }

        /// <summary>
        /// Nếu kiểm tra trong thời gian bác sỉ kê toa, thì isTemporary = 1
        /// Nếu lưu đơn chính thức, thì Temporary = 0
        /// </summary>
        public bool isTemporary { get; set; }
    }
}
