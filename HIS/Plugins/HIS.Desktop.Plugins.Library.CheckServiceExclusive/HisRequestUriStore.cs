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

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive
{
    /// <summary>Kho URI cua danh muc dich vu khong duoc chi dinh dong thoi (viec 57452)</summary>
    public class HisRequestUriStore
    {
        public const string MOSHIS_SERVICE_EXCLUSIVE_GET = "api/HisServiceExclusive/Get";
        public const string MOSHIS_SERVICE_EXCLUSIVE_GET_VIEW = "api/HisServiceExclusive/GetView";
        public const string MOSHIS_SERVICE_EXCLUSIVE_CREATE_LIST = "api/HisServiceExclusive/CreateList";
        public const string MOSHIS_SERVICE_EXCLUSIVE_UPDATE_LIST = "api/HisServiceExclusive/UpdateList";
        public const string MOSHIS_SERVICE_EXCLUSIVE_DELETE_LIST = "api/HisServiceExclusive/DeleteList";

        /// <summary>Dung o man danh muc de nap 2 luoi dich vu</summary>
        public const string MOSHIS_SERVICE_GET_VIEW = "api/HisService/GetView";

        /// <summary>Lay dich vu da chi dinh cua ca lan dieu tri (loc theo TREATMENT_ID)</summary>
        public const string MOSHIS_SERE_SERV_GET = "api/HisSereServ/Get";
    }
}
