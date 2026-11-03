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
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base
{
    /// <summary>
    /// Loads the disclosure data of many treatments at once.
    /// Id lists are sent in chunks of MAX_ID_PER_REQUEST so that no request exceeds the Oracle IN list limit (1000).
    /// A request that returns null together with an error message marks the loader as failed (HasError).
    /// </summary>
    internal class PublicMedicineDataLoader
    {
        private const int MAX_ID_PER_REQUEST = 500;

        /// <summary>Messages of the failed requests, to show to the user</summary>
        internal CommonParam Param { get; private set; }
        internal bool HasError { get; private set; }

        internal PublicMedicineDataLoader()
        {
            this.Param = new CommonParam();
        }

        internal List<L_HIS_TREATMENT_BED_ROOM> GetTreatmentBedRooms(HisTreatmentBedRoomLViewFilter filter)
        {
            List<L_HIS_TREATMENT_BED_ROOM> result = new List<L_HIS_TREATMENT_BED_ROOM>();
            try
            {
                CommonParam param = new CommonParam();
                List<L_HIS_TREATMENT_BED_ROOM> data = new BackendAdapter(param).Get<List<L_HIS_TREATMENT_BED_ROOM>>(RequestUriStore.HIS_TREATMENT_BED_ROOM_GETLVIEW, ApiConsumers.MosConsumer, filter, param);
                CheckResult(data, param, RequestUriStore.HIS_TREATMENT_BED_ROOM_GETLVIEW);
                if (data != null)
                    result = data;
            }
            catch (Exception ex)
            {
                this.HasError = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal List<HIS_SERVICE_REQ> GetServiceReqs(List<long> treatmentIds, PublicMedicineFilterADO option)
        {
            return GetByChunk<HIS_SERVICE_REQ>(treatmentIds, RequestUriStore.HIS_SERVICE_REQ_GET, (ids) =>
            {
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.TREATMENT_IDs = ids;
                filter.SERVICE_REQ_TYPE_IDs = new List<long>
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM
                };
                filter.USE_TIME_OR_INTRUCTION_TIME_FROM = option.TimeFrom;
                filter.USE_TIME_OR_INTRUCTION_TIME_TO = option.TimeTo;
                filter.IS_NO_EXECUTE = false;
                filter.REQUEST_DEPARTMENT_ID = option.RequestDepartmentId;
                filter.REQUEST_ROOM_ID = option.RequestRoomId;
                return filter;
            });
        }

        internal List<HIS_EXP_MEST> GetExpMests(List<long> serviceReqIds)
        {
            return GetByChunk<HIS_EXP_MEST>(serviceReqIds, RequestUriStore.HIS_EXP_MEST_GET, (ids) =>
            {
                HisExpMestFilter filter = new HisExpMestFilter();
                filter.SERVICE_REQ_IDs = ids;
                return filter;
            });
        }

        internal List<V_HIS_SERE_SERV> GetSereServs(List<long> serviceReqIds)
        {
            return GetByChunk<V_HIS_SERE_SERV>(serviceReqIds, RequestUriStore.HIS_SERE_SERV_GETVIEW, (ids) =>
            {
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.SERVICE_REQ_IDs = ids;
                return filter;
            });
        }

        internal List<V_HIS_EXP_MEST_MEDICINE> GetExpMestMedicines(List<long> expMestIds, List<long> patientTypeIds)
        {
            return GetByChunk<V_HIS_EXP_MEST_MEDICINE>(expMestIds, RequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, (ids) =>
            {
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.EXP_MEST_IDs = ids;
                if (patientTypeIds != null && patientTypeIds.Count > 0)
                    filter.PATIENT_TYPE_IDs = patientTypeIds;
                return filter;
            });
        }

        internal List<V_HIS_EXP_MEST_MATERIAL> GetExpMestMaterials(List<long> expMestIds, List<long> patientTypeIds)
        {
            return GetByChunk<V_HIS_EXP_MEST_MATERIAL>(expMestIds, RequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, (ids) =>
            {
                HisExpMestMaterialViewFilter filter = new HisExpMestMaterialViewFilter();
                filter.EXP_MEST_IDs = ids;
                if (patientTypeIds != null && patientTypeIds.Count > 0)
                    filter.PATIENT_TYPE_IDs = patientTypeIds;
                return filter;
            });
        }

        internal List<V_HIS_EXP_MEST_BLOOD> GetExpMestBloods(List<long> expMestIds, List<long> patientTypeIds)
        {
            return GetByChunk<V_HIS_EXP_MEST_BLOOD>(expMestIds, RequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, (ids) =>
            {
                HisExpMestBloodViewFilter filter = new HisExpMestBloodViewFilter();
                filter.EXP_MEST_IDs = ids;
                if (patientTypeIds != null && patientTypeIds.Count > 0)
                    filter.PATIENT_TYPE_IDs = patientTypeIds;
                return filter;
            });
        }

        internal List<V_HIS_TREATMENT> GetTreatments(List<long> treatmentIds)
        {
            return GetByChunk<V_HIS_TREATMENT>(treatmentIds, RequestUriStore.HIS_TREATMENT_GETVIEW, (ids) =>
            {
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.IDs = ids;
                return filter;
            });
        }

        internal List<V_HIS_BED_LOG> GetBedLogs(List<long> treatmentIds)
        {
            return GetByChunk<V_HIS_BED_LOG>(treatmentIds, RequestUriStore.HIS_BED_LOG_GETVIEW, (ids) =>
            {
                HisBedLogViewFilter filter = new HisBedLogViewFilter();
                filter.TREATMENT_IDs = ids;
                return filter;
            });
        }

        internal List<V_HIS_MEDICINE_TYPE_ACIN> GetMedicineTypeAcins(List<long> medicineTypeIds)
        {
            return GetByChunk<V_HIS_MEDICINE_TYPE_ACIN>(medicineTypeIds, RequestUriStore.HIS_MEDICINE_TYPE_ACIN_GETVIEW, (ids) =>
            {
                HisMedicineTypeAcinViewFilter filter = new HisMedicineTypeAcinViewFilter();
                filter.MEDICINE_TYPE_IDs = ids;
                return filter;
            });
        }

        /// <summary>
        /// Calls the route once per chunk of ids and merges the results
        /// </summary>
        private List<T> GetByChunk<T>(List<long> ids, string uri, Func<List<long>, object> makeFilter)
        {
            List<T> result = new List<T>();
            try
            {
                if (ids == null || ids.Count == 0)
                    return result;

                List<long> distinctIds = ids.Distinct().ToList();
                for (int i = 0; i < distinctIds.Count; i += MAX_ID_PER_REQUEST)
                {
                    List<long> chunk = distinctIds.Skip(i).Take(MAX_ID_PER_REQUEST).ToList();
                    CommonParam param = new CommonParam();
                    List<T> data = new BackendAdapter(param).Get<List<T>>(uri, ApiConsumers.MosConsumer, makeFilter(chunk), param);
                    CheckResult(data, param, uri);
                    if (data != null)
                        result.AddRange(data);
                }
            }
            catch (Exception ex)
            {
                this.HasError = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void CheckResult(object data, CommonParam param, string uri)
        {
            if (data != null || param == null)
                return;

            bool hasMessage = (param.Messages != null && param.Messages.Count > 0) || (param.BugCodes != null && param.BugCodes.Count > 0);
            if (param.HasException || hasMessage)
            {
                this.HasError = true;
                if (param.Messages != null)
                    this.Param.Messages.AddRange(param.Messages);
                if (param.BugCodes != null)
                    this.Param.BugCodes.AddRange(param.BugCodes);
                Inventec.Common.Logging.LogSystem.Warn("PublicMedicineMultiPatient: goi " + uri + " loi. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
        }
    }
}
