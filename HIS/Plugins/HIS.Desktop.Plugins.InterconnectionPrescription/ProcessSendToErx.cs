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
using HIS.ERXConnect;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InterconnectionPrescription
{
    class ProcessSendToErx
    {
        const int MaxReq = 100;
        const string HinhThuc_NoiTru = "noitru";
        const string HinhThuc_NgoaiTru = "ngoaitru";
        const string Loai_co_ban = "c";
        const string Loai_huong_than = "h";
        const string Loai_gay_nghien = "n";
        const string TUTORIAL_DEFAULT = ".";

        List<HIS_SERVICE_REQ> ListServiceReq;
        List<HIS_EMPLOYEE> ListEmplyee;
        string Config;
        string MediOrgCode;

        List<V_HIS_SERE_SERV_2> ListSs2 = new List<V_HIS_SERE_SERV_2>();
        List<HIS_SERVICE_REQ_METY> ListReqMety = new List<HIS_SERVICE_REQ_METY>();
        List<HIS_DHST> ListDhst = new List<HIS_DHST>();
        List<HIS_TREATMENT> ListTreatment = new List<HIS_TREATMENT>();

        public ProcessSendToErx(List<HIS_SERVICE_REQ> listServiceReq,List<HIS_EMPLOYEE> listEmplyee, string config, string mediOrgCode)
        {
            this.ListServiceReq = listServiceReq;
            this.ListEmplyee = listEmplyee;
            this.Config = config;
            this.MediOrgCode = mediOrgCode;
        }

        public DataResult Send()
        {
            DataResult result = null;
            try
            {
                if (ListServiceReq != null && ListServiceReq.Count > 0 && !String.IsNullOrWhiteSpace(Config))
                {
                    var cfg = Config.Split('|');
                    if (cfg.Count() < 3)
                    {
                        throw new Exception("Loi cau hinh he thong: " + Config);
                    }

                    if (String.IsNullOrWhiteSpace(MediOrgCode) || MediOrgCode.Length != 5)
                    {
                        throw new Exception("Sai thong tin chi nhanh lam viec: " + MediOrgCode);
                    }

                    ThreadLoadDataMediMate();

                    DataInput sendData = new DataInput();
                    sendData.Url = cfg[0];
                    sendData.HospitalLoginname = cfg[1];
                    sendData.HospitalPassword = cfg[2];
                    sendData.MediOrgCode = MediOrgCode;
                    sendData.ListDhst = ListDhst;
                    sendData.ListEmplyee = ListEmplyee;
                    sendData.ListServiceReq = ListServiceReq;
                    sendData.ListSereServs = ListSs2;
                    sendData.ListMedicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    sendData.ListReqMety = ListReqMety;
                    sendData.ListServiceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                    sendData.ListTreatment = ListTreatment;

                    result = new HIS.ERXConnect.ERXConnectProcessor().SendPrescription(sendData);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ThreadLoadDataMediMate()
        {
            Thread ss = new Thread(GetSereServ);
            Thread dhst = new Thread(GetDhst);
            Thread treatment = new Thread(GetTreatment);
            try
            {
                ss.Start();
                dhst.Start();
                treatment.Start();

                ss.Join();
                dhst.Join();
                treatment.Join();
            }
            catch (Exception ex)
            {
                ss.Abort();
                dhst.Abort();
                treatment.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTreatment()
        {
            if (ListServiceReq != null && ListServiceReq.Count > 0)
            {
                List<long> listTreatmentId = ListServiceReq.Select(s => s.TREATMENT_ID).Distinct().ToList();

                int skip = 0;
                while (listTreatmentId.Count - skip > 0)
                {
                    var listIds = listTreatmentId.Skip(skip).Take(MaxReq).ToList();
                    skip += MaxReq;

                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.IDs = listIds;
                    var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (treatment != null && treatment.Count > 0)
                    {
                        ListTreatment.AddRange(treatment);
                    }
                }
            }
        }

        private void GetDhst()
        {
            try
            {
                if (ListServiceReq != null && ListServiceReq.Count > 0)
                {
                    List<long> listTreatmentId = ListServiceReq.Select(s => s.TREATMENT_ID).Distinct().ToList();

                    int skip = 0;
                    while (listTreatmentId.Count - skip > 0)
                    {
                        var listIds = listTreatmentId.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;

                        CommonParam param = new CommonParam();
                        HisDhstFilter filter = new HisDhstFilter();
                        filter.TREATMENT_IDs = listIds;
                        var dhst = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, filter, param);
                        if (dhst != null && dhst.Count > 0)
                        {
                            dhst = dhst.Where(o => o.WEIGHT.HasValue).ToList();
                            if (dhst != null && dhst.Count > 0)
                            {
                                ListDhst.AddRange(dhst);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetSereServ()
        {
            try
            {
                if (ListServiceReq != null && ListServiceReq.Count > 0)
                {
                    int skip = 0;
                    while (ListServiceReq.Count - skip > 0)
                    {
                        var listIds = ListServiceReq.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;

                        CommonParam param = new CommonParam();
                        HisSereServView2Filter filter = new HisSereServView2Filter();
                        filter.SERVICE_REQ_IDs = listIds.Select(s => s.ID).ToList();
                        var ss2 = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>("api/HisSereServ/GetView2", ApiConsumers.MosConsumer, filter, param);
                        if (ss2 != null && ss2.Count > 0)
                        {
                            ListSs2.AddRange(ss2);
                        }
                    }

                    List<long> serviceReqIds = ListServiceReq.Select(s => s.ID).Distinct().ToList();
                    skip = 0;
                    while (serviceReqIds.Count - skip > 0)
                    {
                        var listIds = serviceReqIds.Skip(skip).Take(MaxReq).ToList();
                        skip += MaxReq;

                        CommonParam param = new CommonParam();
                        HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                        metyFilter.SERVICE_REQ_IDs = listIds;
                        var metys = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumer.ApiConsumers.MosConsumer, metyFilter, param);
                        if (metys != null && metys.Count > 0)
                        {
                            ListReqMety.AddRange(metys);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
