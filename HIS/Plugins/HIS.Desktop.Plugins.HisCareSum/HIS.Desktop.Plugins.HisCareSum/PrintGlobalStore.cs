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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCareSum
{
    public class PrintGlobalStore
    {
        public static V_HIS_TREATMENT getTreatment(long treatmentId)
        {
            Inventec.Common.Logging.LogSystem.Info("Begin get MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO");
            CommonParam param = new CommonParam();
            //MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO treatmentADO = new MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO();
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();
                    //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO>();
                    //treatmentADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO>(currentHisTreatment);
                    //treatmentADO.LOCK_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(treatmentADO.LOCK_TIME ?? 0);
                }
                Inventec.Common.Logging.LogSystem.Info("End get MPS.Processor.Mps000151.PDO.Mps000151PDO.TreatmentADO");
            }
            catch (Exception ex)
            {
                currentHisTreatment = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentHisTreatment;
        }
    }
}
