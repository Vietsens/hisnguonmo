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
namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    class RequestUriStore
    {
        /// <summary>Lấy danh sách dịch vụ CLS (gọi thủ tục PRO_GET_SERVICE_REQ_CLS).</summary>
        internal const string HIS_SERVICE_REQ_GET_SERVICE_REQ_CLS = "api/HisServiceReq/GetServiceReqCLS";

        /// <summary>Cập nhật hướng giải quyết + người xem cho y lệnh.</summary>
        internal const string HIS_SERVICE_REQ_UPDATE_COORDINATION = "api/HisServiceReq/UpdateCoordination";

        /// <summary>Lấy view y lệnh (dùng khi xem kết quả để lấy EXE_SERVICE_MODULE_ID).</summary>
        internal const string HIS_SERVICE_REQ_GETVIEW = "api/HisServiceReq/GetView";

        /// <summary>Lấy danh sách dịch vụ đã thực hiện của y lệnh (dùng khi xem kết quả).</summary>
        internal const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";
    }
}
