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

namespace HIS.Desktop.Plugins.PublicMedicineGeneral.Base
{
    public class GlobaStore
    {
        internal const string HisPatientGetview = "api/HisPatient/GetView";
        internal const string HisPrescriptionGetview1 = "api/HisPrescription/GetView1";
        internal const string HisTreatmentGetView = "api/HisTreatment/GetView";
        internal const string HisTreatmentBedRoomGetview = "api/HisTreatmentBedRoom/GetView";
        internal const string HisAggrExpMestGetview = "api/HisAggrExpMest/GetView";
        internal const string HisExpMestGet = "api/HisExpMest/Get";
        internal const string HisBedRoomGetView = "api/HisBedRoom/GetView";
        internal const string HisSereServGetView = "api/HisSereServ/GetView";

        internal const string HisMedicineGet = "api/HisMedicine/Get";
        internal const string HisMaterialGet = "api/HisMaterial/Get";
        internal const int MAX_REQUEST_LENGTH_PARAM = 100;
    }
}
