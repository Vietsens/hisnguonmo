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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    /// <summary>
    /// Định danh (ModuleLink) các plugin xem kết quả được tái sử dụng khi bấm "Xem kết quả".
    /// Trùng logic định tuyến của HIS.Desktop.Plugins.ServiceReqList.repositoryItemButtonView_ButtonClick.
    /// </summary>
    internal class ModuleLinkString
    {
        /// <summary>Xem kết quả khám bệnh.</summary>
        internal const string ExamServiceReqResult = "HIS.Desktop.Plugins.ExamServiceReqResult";

        /// <summary>Xem kết quả xét nghiệm (chỉ số).</summary>
        internal const string SereServTein = "HIS.Desktop.Plugins.SereServTein";

        /// <summary>Xem kết quả xét nghiệm kháng sinh đồ.</summary>
        internal const string SereServTeinBacterium = "HIS.Desktop.Plugins.SereServTeinBacterium";

        /// <summary>Xem kết quả dịch vụ (CĐHA, TDCN, PTTT gửi ngoài, PHCN...).</summary>
        internal const string ServiceReqResultView = "HIS.Desktop.Plugins.ServiceReqResultView";
    }

    /// <summary>
    /// Wrapper mở plugin động qua ModuleExt (tái sử dụng module có sẵn) — copy pattern từ ServiceReqList.
    /// </summary>
    internal class CallModule
    {
        public CallModule(string moduleLink, long roomId, long roomTypeId, List<object> listObj)
        {
            CallModuleProcess(moduleLink, roomId, roomTypeId, listObj);
        }

        private void CallModuleProcess(string moduleLink, long roomId, long roomTypeId, List<object> listObj)
        {
            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(moduleLink, roomId, roomTypeId, listObj);
        }
    }
}
