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

namespace HIS.Desktop.Plugins.MedicineTypeCreateParent
{
    class CallModule
    {
        internal const string HisMedicineLine = "HIS.Desktop.Plugins.HisMedicineLine";
        internal const string HisManufacturer = "HIS.Desktop.Plugins.HisManufacturer";
        internal const string HisMedicineUseForm = "HIS.Desktop.Plugins.HisMedicineUseForm";
        internal const string HisPackingType = "HIS.Desktop.Plugins.HisPackingType";
        internal const string HisMedicineTypeAcin = "HIS.Desktop.Plugins.HisMedicineTypeAcin";
        internal const string HisServicePatyList = "HIS.Desktop.Plugins.HisServicePatyList";
        internal const string HisServiceHein = "HIS.Desktop.Plugins.HisServiceHein";
        internal const string MedicineTypeCreateParent = "HIS.Desktop.Plugins.MedicineTypeCreateParent";

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
