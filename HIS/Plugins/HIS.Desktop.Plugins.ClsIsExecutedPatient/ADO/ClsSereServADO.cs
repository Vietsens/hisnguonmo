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

namespace HIS.Desktop.Plugins.ClsIsExecutedPatient.ADO
{
    /// <summary>
    /// Dong tren cay "Thuc hien y lenh CLS" (PTTK_XXXXX_Thuc_Hien_Y_Lenh_CLS_Man_Hinh_Buong_Benh).
    /// Dong cha = y lenh (IS_PARENT = true), dong con = dich vu can lam sang.
    /// </summary>
    class ClsSereServADO
    {
        // Khoa cay
        public string CONCRETE_ID__IN_SETY { get; set; }
        public string PARENT_ID__IN_SETY { get; set; }
        public bool IS_PARENT { get; set; }

        // Y lenh
        public long? SERVICE_REQ_ID { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public long INTRUCTION_TIME { get; set; }

        // Dich vu
        public long SERE_SERV_ID { get; set; }
        public long? TDL_SERVICE_TYPE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }

        // Dau dieu duong da thuc hien
        public bool? IS_EXECUTED { get; set; }
        public long? EXECUTE_TIME { get; set; }
        public DateTime? EXECUTE_TIME_DT { get; set; }
        public string EXECUTE_USERNAME { get; set; }

        /// <summary>Thoi gian nhap khong hop le (truoc gio y lenh / sau hien tai) -> to do canh bao, chua ghi</summary>
        public bool IS_IN_VALID { get; set; }

        public ClsSereServADO()
        {
        }
    }
}
