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
using His.UC.UCHein.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.HisPatientTypeAlter
{
    class HisPatientTypeAlterGet
    {
        internal static MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER GetById(long patientTypeAlterId)
        {
            CommonParam param = new CommonParam();
            HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
            filter.ID = patientTypeAlterId;
            return GetView(filter).SingleOrDefault(o => o.ID == patientTypeAlterId);
        }

        internal static List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER> Get(MOS.Filter.HisPatientTypeAlterFilter filter)
        {
            CommonParam param = new CommonParam();
            return new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GETVIEW, ApiConsumerStore.MosConsumer, filter, param);
        }

        internal static List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER> GetView(MOS.Filter.HisPatientTypeAlterViewFilter filter)
        {
            CommonParam param = new CommonParam();
            return new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GETVIEW, ApiConsumerStore.MosConsumer, filter, param);
        }
    }
}
