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

namespace HIS.Desktop.Plugins.PaanExecuteList
{
    public class PaanRequestUriStore
    {
        /// <summary>
        /// Lay danh sach benh nhan cho xu ly Giai phau benh.
        /// Tra ve List&lt;V_HIS_SERE_SERV_GPBL&gt;.
        /// </summary>
        internal const string HIS_SERE_SERV_GET_VIEW_GPBL = "/api/HisSereServ/GetViewGpbl";

        /// <summary>Lay danh muc khoa, do combo loc Khoa chi dinh.</summary>
        internal const string HIS_DEPARTMENT_GET = "/api/HisDepartment/Get";

        /// <summary>Lay danh muc phong, do combo loc Phong chi dinh.</summary>
        internal const string HIS_ROOM_GET_VIEW = "/api/HisRoom/GetView";

        /// <summary>Lay danh muc doi tuong thanh toan, do combo loc Doi tuong.</summary>
        internal const string HIS_PATIENT_TYPE_GET = "/api/HisPatientType/Get";

        /// <summary>Lay y lenh day du truoc khi mo man Tra ket qua.</summary>
        internal const string HIS_SERVICE_REQ_GET_VIEW = "/api/HisServiceReq/GetView";

        /// <summary>
        /// Lay y lenh dang L_HIS_SERVICE_REQ (ban "Light", 81 cot).
        /// Day la kieu du lieu ma toan bo cac nut va menu chuot phai chep tu man
        /// hinh cu dang dung. Xem UCPaanExecuteList___Bridge.cs.
        /// </summary>
        internal const string HIS_SERVICE_REQ_GET_LVIEW = "api/HisServiceReq/GetLView";

        /// <summary>Bat dau xu ly y lenh (sinh thoi gian bat dau).</summary>
        internal const string HIS_SERVICE_REQ_REQUEST_ORDER = "/api/HisServiceReq/RequestOrder";
    }
}
