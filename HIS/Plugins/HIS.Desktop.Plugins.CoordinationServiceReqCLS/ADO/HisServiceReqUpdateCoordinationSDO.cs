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

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS.ADO
{
    /// <summary>
    /// Body của API api/HisServiceReq/UpdateCoordination (mục 4.2 tài liệu).
    /// TẠM khai báo cục bộ; khi MOS.SDO có HisServiceReqUpdateCoordinationSDO thì đổi using sang MOS.SDO.
    /// </summary>
    public class HisServiceReqUpdateCoordinationSDO
    {
        /// <summary>ID y lệnh cần cập nhật.</summary>
        public long Id { get; set; }

        /// <summary>Nội dung hướng giải quyết.</summary>
        public string SolutionDes { get; set; }
    }
}
