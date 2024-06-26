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

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    class CallModule
    {
        internal const string TextLibrary = "HIS.Desktop.Plugins.TextLibrary";
        internal const string HisPatientProgram = "HIS.Desktop.Plugins.HisPatientProgram";
        internal const string Bordereau = "HIS.Desktop.Plugins.Bordereau";
        internal const string ListSurgMisuByTreatment = "HIS.Desktop.Plugins.ListSurgMisuByTreatment";
        internal const string OtherForms = "HIS.Desktop.Plugins.OtherForms";
        internal const string AppointmentService = "HIS.Desktop.Plugins.AppointmentService";
        internal const string BedHistory = "HIS.Desktop.Plugins.BedHistory";
        internal const string WorkPlace = "HIS.Desktop.Plugins.HisWorkPlace";
        internal const string EmrDocument = "HIS.Desktop.Plugins.EmrDocument";
        internal const string HisTranPatiTemp = "HIS.Desktop.Plugins.HisTranPatiTemp";
        internal const string InformationAllowGoHome = "HIS.Desktop.Plugins.InformationAllowGoHome";

        internal static void Run(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(_moduleLink, _roomId, _roomTypeId, _listObj);
        }
    }
}
