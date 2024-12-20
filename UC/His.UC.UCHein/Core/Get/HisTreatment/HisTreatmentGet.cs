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

namespace His.UC.UCHein.HisTreatment
{
    class HisTreatmentGet
    {
        internal static MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4 GetById(long treatmentId)
        {
            CommonParam param = new CommonParam();
            HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
            filter.ID = treatmentId;
            return new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_4>>(RequestUriStore.HIS_TREATMENT__GETVIEW_4, ApiConsumerStore.MosConsumer, filter, param).SingleOrDefault();
        }
    }
}
