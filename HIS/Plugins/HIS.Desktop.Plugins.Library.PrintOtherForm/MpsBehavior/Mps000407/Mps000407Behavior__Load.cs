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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000407
{
    public partial class Mps000407Behavior : MpsDataBase, ILoad
    {
        private void LoadData()
        {
            try
            {
                if (this.inputAdo != null)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadServiceReq);
                    methods.Add(LoadTreatment);
                    methods.Add(LoadPatient);
                    methods.Add(LoadDhst);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);

                    if (inputAdo.RoomId.HasValue)
                    {
                        this.room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == inputAdo.RoomId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadServiceReq()
        {
            try
            {
                if (this.inputAdo.ServiceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.ID = this.inputAdo.ServiceReqId;
                    var listServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listServiceReq != null && listServiceReq.Count > 0)
                    {
                        this.serviceReq = listServiceReq.FirstOrDefault();
                    }
                    LogSystem.Debug(LogUtil.TraceData("serviceReq", serviceReq));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                if (this.inputAdo.TreatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = this.inputAdo.TreatmentId;
                    var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        this.treatment = listTreatment.FirstOrDefault();
                    }
                    LogSystem.Debug(LogUtil.TraceData("treatment", treatment));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatient()
        {
            try
            {
                if (this.inputAdo.PatientId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientViewFilter filter = new HisPatientViewFilter();
                    filter.ID = this.inputAdo.PatientId;
                    var listPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listPatient != null && listPatient.Count > 0)
                    {
                        this.patient = listPatient.FirstOrDefault();
                    }
                    LogSystem.Debug(LogUtil.TraceData("patient", patient));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDhst()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDhstFilter filter = new HisDhstFilter();
                if (this.inputAdo.DhstId.HasValue)
                {
                    filter.ID = this.inputAdo.DhstId.Value;
                }
                else
                {
                    filter.TREATMENT_ID = this.inputAdo.TreatmentId;
                }
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "EXECUTE_TIME";
                var listDhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                if (listDhst != null && listDhst.Count > 0)
                {
                    if (listDhst.Any(a => a.WEIGHT.HasValue && a.TEMPERATURE.HasValue && a.BLOOD_PRESSURE_MIN.HasValue && a.BLOOD_PRESSURE_MAX.HasValue && a.BREATH_RATE.HasValue))
                        this.dhst = listDhst.FirstOrDefault(a => a.WEIGHT.HasValue && a.TEMPERATURE.HasValue && a.BLOOD_PRESSURE_MIN.HasValue && a.BLOOD_PRESSURE_MAX.HasValue && a.BREATH_RATE.HasValue);
                    else
                        this.dhst = listDhst.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
