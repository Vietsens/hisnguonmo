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

namespace HIS.Desktop.Plugins.HisImportService.ADO
{
    /// <summary>
    /// Dong thuoc hao phi (thuoc phong xa) cua dich vu - sheet "Thuoc PX" cua file IMPORT_SERVICE.
    /// Phuc vu khoi DS_THUOCPX cua XML TT12 MAU_05.
    /// </summary>
    public class ServiceMetyImportADO
    {
        // Cot doc truc tiep tu Excel
        public string SERVICE_CODE { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string EXPEND_PRICE_STR { get; set; }
        public string DM_NSX_CDD_STR { get; set; }
        public string DM_THUCTE_CDD_STR { get; set; }
        public string LIEU_BQ_PX_STR { get; set; }
        public string TL_THUCTE_BQ_PX_STR { get; set; }

        // Gia tri sau khi kiem tra, dung khi luu
        public long? SERVICE_ID { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long? SERVICE_UNIT_ID { get; set; }
        public decimal? EXPEND_PRICE { get; set; }
        public decimal? DM_NSX_CDD { get; set; }
        public decimal? DM_THUCTE_CDD { get; set; }
        public decimal? LIEU_BQ_PX { get; set; }
        public decimal? TL_THUCTE_BQ_PX { get; set; }

        public string SERVICE_NAME { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string ERROR { get; set; }

        /// <summary>
        /// Dong trong hoan toan thi bo qua, khong tinh la du lieu import.
        /// </summary>
        public bool IsEmptyRow
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.SERVICE_CODE)
                    && string.IsNullOrWhiteSpace(this.MEDICINE_TYPE_CODE)
                    && string.IsNullOrWhiteSpace(this.EXPEND_PRICE_STR)
                    && string.IsNullOrWhiteSpace(this.DM_NSX_CDD_STR)
                    && string.IsNullOrWhiteSpace(this.DM_THUCTE_CDD_STR)
                    && string.IsNullOrWhiteSpace(this.LIEU_BQ_PX_STR)
                    && string.IsNullOrWhiteSpace(this.TL_THUCTE_BQ_PX_STR);
            }
        }
    }
}
