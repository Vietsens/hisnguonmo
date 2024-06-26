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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ReportAll
{
    public class PagingGet
    {
        public static void Get(CommonParam paramCommon, object filter, ref object apiResult)
        {
            try
            {
                if (paramCommon != null)
                {
                    if (paramCommon.Limit == 0 || paramCommon.Limit==null)
                    {
                        paramCommon.Limit=(int)ConfigApplications.NumPageSize;
                    }
                    if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>))
                    {
                        HisPatientFilter patientfilter = filter as HisPatientFilter;
                        
                        patientfilter.ORDER_FIELD = "MODIFY_TIME";
                        patientfilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>
                            (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }

                    else if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>))
                    {
                        HisTreatmentFilter treatmentfilter = filter as HisTreatmentFilter;
                        treatmentfilter.ORDER_FIELD = "MODIFY_TIME";
                        treatmentfilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>
                            (ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }

                    else if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>))
                    {
                        HisServiceReqFilter serviceReqfilter = filter as HisServiceReqFilter;
                        serviceReqfilter.ORDER_FIELD = "MODIFY_TIME";
                        serviceReqfilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>
                            (ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }

                    else if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM_1>>))
                    {
                        HisTreatmentBedRoomView1Filter treatmentBedRoomfilter = filter as HisTreatmentBedRoomView1Filter;
                        treatmentBedRoomfilter.ORDER_FIELD = "MODIFY_TIME";
                        treatmentBedRoomfilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM_1>>
                            ("api/HisTreatmentBedRoom/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }

                    else if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_1>>))
                    {
                        HisMedicineView1Filter medicinefilter = filter as HisMedicineView1Filter;
                        medicinefilter.ORDER_FIELD = "MODIFY_TIME";
                        medicinefilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_1>>
                            ("api/HisMedicine/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }

                    else if (apiResult.GetType() == typeof(ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>>))
                    {
                        HisMaterialView1Filter medicinefilter = filter as HisMaterialView1Filter;
                        medicinefilter.ORDER_FIELD = "MODIFY_TIME";
                        medicinefilter.ORDER_DIRECTION = "DESC";
                        apiResult = new Inventec.Common.Adapter.BackendAdapter
                            (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>>
                            ("api/HisMaterial/GetView1", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static bool checkDigit(string s)
        {
            bool result = true;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) != true)
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
    }
}
