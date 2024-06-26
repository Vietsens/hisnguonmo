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

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    class CallModule
    {
        internal const string ManuImpMestEdit = "HIS.Desktop.Plugins.ManuImpMestEdit";
        internal const string ManuExpMestCreate = "HIS.Desktop.Plugins.ManuExpMestCreate";
        internal const string EventLog = "Inventec.Desktop.Plugins.EventLog";
        internal const string ImpMestViewDetail = "HIS.Desktop.Plugins.ImpMestViewDetail";
        internal const string ApproveAggrImpMest = "HIS.Desktop.Plugins.ApproveAggrImpMest";
        internal const string ManuImpMestUpdate = "HIS.Desktop.Plugins.ManuImpMestUpdate";
        internal const string CallPatientTypeAlter = "HIS.Desktop.Plugins.CallPatientTypeAlter";
        internal const string BloodImpMestUpdate = "HIS.Desktop.Plugins.ImportBlood";

        public CallModule(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            CallModuleProcess(_moduleLink, _roomId, _roomTypeId, _listObj);
        }

        private void CallModuleProcess(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(_moduleLink, _roomId, _roomTypeId, _listObj);
        }
    }
}
