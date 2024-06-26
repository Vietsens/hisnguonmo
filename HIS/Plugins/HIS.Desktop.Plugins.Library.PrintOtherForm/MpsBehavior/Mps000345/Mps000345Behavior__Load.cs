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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000345
{
    public partial class Mps000345Behavior : MpsDataBase, ILoad
    {
        private void LoadData()
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(LoadPatient);
                methods.Add(LoadServiceReq);
                methods.Add(LoadTreatment);
                methods.Add(LoadSereServPTTT);
                methods.Add(LoadDHST);
                methods.Add(LoadSereServ);
                methods.Add(LoadExpMestMedicines);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                //LoadExpMestMedicines();
                //LoadDHST();
                LoadEkipUser();
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
                CommonParam param = new CommonParam();
                HisPatientViewFilter filter = new HisPatientViewFilter();
                filter.ID = this.PatientId;
                var listPatient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param);
                if (listPatient != null && listPatient.Count > 0)
                {
                    this.patient = listPatient.FirstOrDefault();
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
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = this.ServiceReqId;
                var listServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    this.serviceReq = listServiceReq.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDHST()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDhstViewFilter filter = new HisDhstViewFilter();
                filter.TREATMENT_ID = this.TreatmentId;
                var dhsts = new BackendAdapter(param).Get<List<V_HIS_DHST>>("api/HisDhst/GetView", ApiConsumers.MosConsumer, filter, param);
                if (dhsts != null && dhsts.Count() > 0)
                {
                    dhst = dhsts.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                }

                Inventec.Common.Logging.LogSystem.Debug("LoadDHST this.dhst" + Inventec.Common.Logging.LogUtil.TraceData("", this.dhst));
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
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = this.TreatmentId;
                var listTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    this.treatment = listTreatment.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServPTTT()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServPtttViewFilter filter = new HisSereServPtttViewFilter();
                filter.SERE_SERV_ID = this.SereServId;
                List<V_HIS_SERE_SERV_PTTT> sereServPTTTs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, filter, param);
                if (sereServPTTTs != null && sereServPTTTs.Count > 0)
                {
                    this.sereServPTTT = sereServPTTTs.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
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
                CommonParam param = new CommonParam();
                HisSereServExtFilter filter = new HisSereServExtFilter();
                filter.SERE_SERV_ID = this.SereServId;
                List<HIS_SERE_SERV_EXT> sereServExts = new BackendAdapter(param)
                .Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter, param);
                if (sereServExts != null && sereServExts.Count > 0)
                {
                    this.sereServExt = sereServExts.OrderByDescending(o => o.CREATE_TIME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSereServ()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServFilter filter = new HisSereServFilter();
                filter.ID = this.SereServId;
                var listSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                if (listSereServ != null && listSereServ.Count > 0)
                {
                    this.sereServ = listSereServ.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadExpMestMedicines()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = this.TreatmentId;
                this.expMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Debug("LoadExpMestMedicines this.expMestMedicines" + Inventec.Common.Logging.LogUtil.TraceData("", this.expMestMedicines));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadEkipUser()
        {
            try
            {
                if (sereServ != null && sereServ.EKIP_ID.HasValue)
                {
                    CommonParam param = new CommonParam();
                    HisEkipUserViewFilter filter = new HisEkipUserViewFilter();
                    filter.EKIP_ID = this.sereServ.EKIP_ID.Value;
                    this.ekipUsers = new BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>("api/HisEkipUser/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTreatmentBedRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentBedRoomFilter filter = new HisTreatmentBedRoomFilter();
                filter.TREATMENT_ID = this.TreatmentId;
                this.treatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
