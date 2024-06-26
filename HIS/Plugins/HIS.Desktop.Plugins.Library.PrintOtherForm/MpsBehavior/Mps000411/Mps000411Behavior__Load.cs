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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000411
{
    public partial class Mps000411Behavior : MpsDataBase, ILoad
    {
        private void LoadData()
        {
            try
            {
                if (this.inputAdo != null)
                {
                    List<Action> methods = new List<Action>();
                    methods.Add(LoadServiceReq);
                    methods.Add(LoadSereServPttt);
                    methods.Add(LoadSereServExt);
                    methods.Add(LoadDhst);
                    methods.Add(LoadEkipPlanUser);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
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

        private void LoadSereServPttt()
        {
            try
            {
                if (this.inputAdo.SereServId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisSereServPtttViewFilter filter = new HisSereServPtttViewFilter();
                    filter.SERE_SERV_ID = this.inputAdo.SereServId.Value;
                    var listSsPttt = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listSsPttt != null && listSsPttt.Count > 0)
                    {
                        this.sereServPTTT = listSsPttt.FirstOrDefault();
                    }
                    LogSystem.Debug(LogUtil.TraceData("sereServExtPTTT", sereServPTTT));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServExt()
        {
            try
            {
                if (this.inputAdo.SereServId.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisSereServExtFilter filter = new HisSereServExtFilter();
                    filter.SERE_SERV_ID = this.inputAdo.SereServId.Value;
                    var listSsExt = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listSsExt != null && listSsExt.Count > 0)
                    {
                        this.sereServExt = listSsExt.FirstOrDefault();
                    }
                    LogSystem.Debug(LogUtil.TraceData("sereServExt", sereServExt));
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
                if (this.inputAdo.TreatmentId > 0)
                {
                    filter.TREATMENT_ID = this.inputAdo.TreatmentId;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadEkipPlanUser()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipUserViewFilter filter = new HisEkipUserViewFilter();
                if (this.inputAdo.EkipId.HasValue && this.inputAdo.EkipId.Value > 0)
                {
                    filter.EKIP_ID = this.inputAdo.EkipId.Value;
                    var listPlanUser = new BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, filter, param);
                    this.ekipUsers = listPlanUser;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
