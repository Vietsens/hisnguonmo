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
using HIS.Desktop.Plugins.ExamServiceReqExecute.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    class ExamServiceReqExecuteControlProcess
    {
       
        static void CreateDefaultExamSereDireSDO(ref DiseaseRelationADO sdo)
        {
            try
            {
                sdo.ID = 0;
                sdo = sdo ?? new DiseaseRelationADO();
                sdo.CheckState = false;
                sdo.LblMonth = "tháng";
                sdo.DESCRIPTION = "Mô tả";
                sdo.MONTH_COUNT = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static bool CheckTreatmentStatus(long treatmentId)
        {
            bool value = true;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;

                List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> hisTreatment = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);

                if (hisTreatment != null && hisTreatment.Count == 1)
                {
                    if (hisTreatment[0].IS_PAUSE == 1)
                    {
                        value = false;
                    }
                    //else
                    //{
                    //    if (!CheckDepartmentTranInOut(treatmentId))
                    //    {
                    //        value = false;
                    //    }
                    //}
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return value;
            }
        }
    }
}
