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
using HIS.Desktop.Plugins.HisTrackingList.Event;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        internal List<HIS_TRACKING> _TrackingPrints { get; set; }

        V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom { get; set; }

        List<HIS_SERVICE_REQ> _ServiceReqs { get; set; }
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReqs { get; set; }

        List<HIS_SERE_SERV> _SereServs { get; set; }
        Dictionary<long, List<HIS_SERE_SERV>> dicSereServs { get; set; }

        List<HIS_EXP_MEST> _ExpMests { get; set; }
        Dictionary<long, HIS_EXP_MEST> dicExpMests { get; set; }

        List<HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MEDICINE>> dicExpMestMedicines { get; set; }

        List<HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        Dictionary<long, List<HIS_EXP_MEST_MATERIAL>> dicExpMestMaterials { get; set; }

        Dictionary<long, List<HIS_SERVICE_REQ_METY>> dicServiceReqMetys { get; set; }
        Dictionary<long, List<HIS_SERVICE_REQ_MATY>> dicServiceReqMatys { get; set; }

        internal List<HIS_IMP_MEST> _ImpMests_input { get; set; }
        internal List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedis { get; set; }
        internal List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMates { get; set; }

        List<HIS_SERE_SERV_EXT> _SereServExts { get; set; }

        bool IsNotShowOutMediAndMate = false;

        private void CreateThreadLoadData(object param)
        {
            Thread threadTreatment = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataTreatmentNewThread));
            Thread threadServiceReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataServiceReqNewThread));

            try
            {
                threadServiceReq.Start(param);
                threadTreatment.Start(param);

                threadTreatment.Join();
                threadServiceReq.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadTreatment.Abort();
                threadServiceReq.Abort();
            }
        }

        private void LoadDataTreatmentNewThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { LoadDataTreatmentWithTreatment((long)param); }));
                //}
                //else
                //{
                LoadDataTreatment((long)param);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(long treatmentId)
        {
            try
            {
                MOS.Filter.HisTreatmentBedRoomViewFilter bedFilter = new HisTreatmentBedRoomViewFilter();
                bedFilter.TREATMENT_ID = treatmentId;
                bedFilter.ORDER_FIELD = "CREATE_TIME";
                bedFilter.ORDER_DIRECTION = "DESC";
                _TreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, bedFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqNewThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { LoadDataServiceReqWithTreatment((long)param); }));
                //}
                //else
                //{
                LoadDataServiceReq((long)param);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReq(long treatmentId)
        {
            try
            {
                CommonParam param = new CommonParam();
                //danh sach yeu cau
                MOS.Filter.HisServiceReqFilter serviceReqFilterVT = new MOS.Filter.HisServiceReqFilter();
                serviceReqFilterVT.TREATMENT_ID = treatmentId;
                this._ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilterVT, param);
                if (this._ServiceReqs != null && this._ServiceReqs.Count > 0)
                {
                    foreach (var item in this._ServiceReqs)
                    {
                        if (!dicServiceReqs.ContainsKey(item.ID))
                        {
                            dicServiceReqs[item.ID] = new HIS_SERVICE_REQ();
                            dicServiceReqs[item.ID] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        /// Thuoc
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadDataExpMest(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMaterialNewThread));
            Thread threadImpMest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadImpMestNewThread));

            // threadMaterial.Priority = ThreadPriority.Normal;

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadImpMest.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
                threadImpMest.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadImpMest.Abort();
            }
        }

        //Thuoc trong kho
        private void LoadDataExpMestMedicineNewThread(object param)
        {
            try
            {
                LoadDataExpMestMedicine((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMedicine(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineFilter filter = new HisExpMestMedicineFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, filter, param);
                if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    var dataGroups = this._ExpMestMedicines.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        if (!dicExpMestMedicines.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMedicines[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MEDICINE>();
                            dicExpMestMedicines[ado.EXP_MEST_ID ?? 0].Add(ado);
                        }
                        else
                            dicExpMestMedicines[item[0].EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //VT Trong Kho
        private void LoadDataExpMestMaterialNewThread(object param)
        {
            try
            {
                LoadDataExpMestMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMaterial(List<long> _expMestIds)
        {
            try
            {
                long configQY7 = 0;
                configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                if (configQY7 != 1)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                filter.EXP_MEST_IDs = _expMestIds;
                this._ExpMestMaterials = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, param);

                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    var dataGroups = this._ExpMestMaterials.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        if (!dicExpMestMaterials.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        {
                            dicExpMestMaterials[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MATERIAL>();
                            dicExpMestMaterials[ado.EXP_MEST_ID ?? 0].Add(ado);
                        }
                        else
                            dicExpMestMaterials[item[0].EXP_MEST_ID ?? 0].Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Thread EXP_MEST && SERVICE_REQ_METY
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadByServiceReq(object param)
        {
            Thread threadExpMest = new Thread(new ParameterizedThreadStart(LoadDataExpMestNewThread));
            Thread threadServiceReqMety = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMetyNewThread));
            Thread threadServiceReqMaty = new Thread(new ParameterizedThreadStart(LoadDataServiceReqMatyNewThread));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServNewThread));

            try
            {
                threadExpMest.Start(param);
                threadServiceReqMety.Start(param);
                threadServiceReqMaty.Start(param);
                threadSereServ.Start(param);

                threadExpMest.Join();
                threadServiceReqMety.Join();
                threadServiceReqMaty.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadExpMest.Abort();
                threadServiceReqMety.Abort();
                threadServiceReqMaty.Abort();
                threadSereServ.Abort();
            }
        }

        //Exp_mest
        private void LoadDataExpMestNewThread(object param)
        {
            try
            {
                LoadDataExpMest((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMest(List<long> _serviceReqIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var expMestDatas = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                if (expMestDatas != null && expMestDatas.Count > 0)
                {
                    foreach (var item in expMestDatas)
                    {
                        if (!dicExpMests.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = new HIS_EXP_MEST();
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                        }
                        else
                            dicExpMests[item.SERVICE_REQ_ID ?? 0] = (item);
                    }
                }
                this._ExpMests.AddRange(expMestDatas);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Thuoc Ngoai Kho
        private void LoadDataServiceReqMetyNewThread(object param)
        {
            try
            {
                LoadDataServiceReqMety((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMety(List<long> _serviceReqIds)
        {
            try
            {
                if (IsNotShowOutMediAndMate)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMetyFilter metyFIlter = new HisServiceReqMetyFilter();
                    metyFIlter.SERVICE_REQ_IDs = _serviceReqIds;
                    var metyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFIlter, param);
                    if (metyDatas != null && metyDatas.Count > 0)
                    {
                        foreach (var item in metyDatas)
                        {
                            if (!dicServiceReqMetys.ContainsKey(item.SERVICE_REQ_ID))
                            {
                                dicServiceReqMetys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_METY>();
                                dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                            }
                            else
                                dicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //VT Ngoai Kho
        private void LoadDataServiceReqMatyNewThread(object param)
        {
            try
            {
                LoadDataServiceReqMaty((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReqMaty(List<long> _serviceReqIds)
        {
            try
            {
                if (IsNotShowOutMediAndMate)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMatyFilter matyFIlter = new HisServiceReqMatyFilter();
                    matyFIlter.SERVICE_REQ_IDs = _serviceReqIds;
                    var matyDatas = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFIlter, param);
                    if (matyDatas != null && matyDatas.Count > 0)
                    {
                        foreach (var item in matyDatas)
                        {
                            if (!dicServiceReqMatys.ContainsKey(item.SERVICE_REQ_ID))
                            {
                                dicServiceReqMatys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_MATY>();
                                dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                            }
                            else
                                dicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSereServNewThread(object param)
        {
            try
            {
                LoadDataSereServ((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ(List<long> _serviceReqIds)
        {
            try
            {
                if (_serviceReqIds == null || _serviceReqIds.Count <= 0)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter hisSereServFilterVT = new MOS.Filter.HisSereServFilter();
                hisSereServFilterVT.SERVICE_REQ_IDs = _serviceReqIds;
                var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, hisSereServFilterVT, param);
                if (datas != null && datas.Count > 0)
                {
                    this._SereServs.AddRange(datas);
                    foreach (var item in datas)
                    {
                        if (!dicSereServs.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        {
                            dicSereServs[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                            dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                        }
                        else
                            dicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMestNewThread(object param)
        {
            try
            {
                LoadImpMest((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadImpMest(List<long> _expMestIds)
        {
            try
            {
                long keyViewMediMateTH = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MEDI_MATE_TH));
                if (keyViewMediMateTH != 1)
                    return;
                CommonParam param = new CommonParam();
                //Ktra thu hoi
                _ImpMests_input = new List<HIS_IMP_MEST>();
                MOS.Filter.HisImpMestFilter impMestFilter = new MOS.Filter.HisImpMestFilter();
                impMestFilter.MOBA_EXP_MEST_IDs = _expMestIds;
                _ImpMests_input = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (_ImpMests_input != null && _ImpMests_input.Count > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMediFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                    impMestMediFilter.IMP_MEST_IDs = _ImpMests_input.Select(p => p.ID).ToList();
                    _ImpMestMedis = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumers.MosConsumer, impMestMediFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    long configQY7 = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL));
                    if (configQY7 == 1)
                    {
                        MOS.Filter.HisImpMestMaterialViewFilter impMestMateFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                        impMestMateFilter.IMP_MEST_IDs = _ImpMests_input.Select(p => p.ID).ToList();
                        _ImpMestMates = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMestMateFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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
