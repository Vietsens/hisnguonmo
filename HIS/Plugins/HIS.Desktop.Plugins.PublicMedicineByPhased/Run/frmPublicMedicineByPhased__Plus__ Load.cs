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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicMedicineByPhased.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineByPhased
{
    public partial class frmPublicMedicineByPhased : HIS.Desktop.Utility.FormBase
    {
        List<HIS_SERVICE_REQ> _ServiceReqs { get;  set; }
        List<HIS_EXP_MEST> _ExpMests { get; set; }
        public List<V_HIS_SERE_SERV> lstsereServ = new List<V_HIS_SERE_SERV>();
        List<long> _serviceReqIds = new List<long>();
        List<V_HIS_SERE_SERV> sereServ = new List<V_HIS_SERE_SERV>();
        Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();
        Dictionary<long, HIS_EXP_MEST> dicExpMest = new Dictionary<long, HIS_EXP_MEST>();

        List<SereServADO> _SereServADOs = new List<SereServADO>();

        private void LoadDataSereServ()
        {
            try
            {
                if (this._treatmentId > 0)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    gridControlSereServ.DataSource = null;
                    MOS.Filter.HisSereServFilter filter = new HisSereServFilter();
                    filter.TREATMENT_ID = this._treatmentId;
                    List<long> _serviceTypeIds = new List<long>();
                    if (chkMedicine.Checked)
                        _serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC);
                    if (chkBlood.Checked)
                        _serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU);
                    if (chkHoaChat.Checked || chkMaterial.Checked)
                        _serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                    filter.TDL_SERVICE_TYPE_IDs = _serviceTypeIds;//Review TDL_SERVICE_TYPE_IDs

                    if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                    {
                        filter.TDL_INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                    {
                        filter.TDL_INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                    }


                    var datas = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, param);
                    if (datas != null && datas.Count > 0)
                    {
                        datas = datas.Where(p => p.IS_NO_EXECUTE != 1).ToList();
                        if (chkHoaChat.Checked || chkMaterial.Checked)
                            _SereServADOs.AddRange((from r in datas select new SereServADO(r, BackendDataWorker.Get<HIS_SERVICE_UNIT>(), BackendDataWorker.Get<HIS_MATERIAL_TYPE>())).ToList());
                        else
                            _SereServADOs.AddRange((from r in datas select new SereServADO(r, BackendDataWorker.Get<HIS_SERVICE_UNIT>())).ToList());
                    }
                    this._SereServADOs = this._SereServADOs.OrderBy(p => p.TDL_SERVICE_NAME).ToList();
                    gridControlSereServ.DataSource = this._SereServADOs;
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void GetAllData()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                gridControlSereServ.DataSource = null;
                _Datas = new List<ADO.ExpMestMediAndMateADO>();
                this._ServiceReqs = new List<HIS_SERVICE_REQ>();
                long timeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFromTime.DateTime) ?? 0;
                long timeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtToTime.DateTime) ?? 0;
                //Inventec.Common.Logging.LogSystem.Error("Load HIS_SERVICE_REQ");
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this._treatmentId;
                //serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONNT;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long>();
                serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_FROM = timeFrom;
                serviceReqFilter.USE_TIME_OR_INTRUCTION_TIME_TO = timeTo;
                serviceReqFilter.IS_NO_EXECUTE = false;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT);
                serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM);
                // serviceReqFilter.SERVICE_REQ_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONNGT);
                //if (this.RoomTypeBed)
                //{
                //    //this.cboDepartment.Enabled = true;
                //}
                //else
                //{
                if (cboDepartment.EditValue != null && cboDepartment.EditValue.ToString() != "")
                {
                    serviceReqFilter.REQUEST_DEPARTMENT_ID = (long)cboDepartment.EditValue;
                }
                else
                {
                    serviceReqFilter.REQUEST_DEPARTMENT_ID = null;
                }
                //WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId; // filter theo khoa yeu cau(khoa cua nguoi dung dang dang nhap)
                //this.cboDepartment.Enabled = false;

                //}
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqFilter), serviceReqFilter));
                this._ServiceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                // Inventec.Common.Logging.LogSystem.Error("End");

                List<long> _expMestIds = new List<long>();
                List<long> _departmentIds = new List<long>();
                List<long> _serviceReqIdsOne = new List<long>();

                _serviceReqIdsOne = null;
                this._ExpMests = new List<HIS_EXP_MEST>();
                if (_ServiceReqs != null && _ServiceReqs.Count > 0)
                {
                    _departmentIds = _ServiceReqs.Select(p => p.REQUEST_DEPARTMENT_ID).ToList();
                    //LoadDataCboDepartment(_departmentIds);
                    _ServiceReqs = _ServiceReqs.Where(p => p.IS_NO_EXECUTE != 1).ToList();
                    _serviceReqIds = _ServiceReqs.Select(p => p.ID).ToList();
                    HisSereServViewFilter sereServVFilter = new HisSereServViewFilter();
                    sereServVFilter.SERVICE_REQ_IDs = _serviceReqIds;
                    lstsereServ = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServVFilter, param).ToList();

                    CreateThreadProcessor(_serviceReqIds);
                }
                else
                {
                    WaitingManager.Hide();
                    return;
                }
                if (this._ExpMests != null && this._ExpMests.Count > 0)
                {
                    _expMestIds = this._ExpMests.Select(p => p.ID).Distinct().ToList();
                }

                CreateThreadLoadData(_expMestIds);

                this._Datas = this._Datas.OrderBy(p => p.TYPE).ThenBy(p => p.MEDICINE_TYPE_NAME).ToList();
                gridControlSereServ.DataSource = this._Datas;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCboDepartment(List<long> _departmentIds)
        {
            try
            {
                List<HIS_DEPARTMENT> Departmnets = new List<HIS_DEPARTMENT>();
                //if (!this.RoomTypeBed)
                //{
                //    Departmnets = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.IS_ACTIVE == 1).ToList();
                //}
                //else if (_departmentIds != null && _departmentIds.Count > 0)
                //{
                //    Departmnets = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => _departmentIds.Contains(p.ID) && p.IS_ACTIVE == 1).ToList();
                //}
                //else {
                Departmnets = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(p => p.IS_ACTIVE == 1).ToList();
                //}

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.cboDepartment, Departmnets, controlEditorADO);
                //this.cboDepartment.EditValue = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        /// <summary>
        /// Trong Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));
            Thread threadBlood = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataBloodNewThread));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);
                threadBlood.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
                threadBlood.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
                threadBlood.Abort();
            }
        }

        private void LoadDataMedicineNewThread(object param)
        {
            try
            {
                LoadDataMedicine((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> _expMestIds)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("Load V_HIS_EXP_MEST_MEDICINE start");
                CommonParam param = new CommonParam();
                List<V_HIS_SERE_SERV> listSereServ = lstsereServ;
                bool checkIsNoExcute = false;
                MOS.Filter.HisExpMestMedicineViewFilter expMestMediFilter = new HisExpMestMedicineViewFilter();
                expMestMediFilter.EXP_MEST_IDs = _expMestIds;
                if (lstHisPatientTypeSelecteds != null && lstHisPatientTypeSelecteds.Count > 0)
                {
                    expMestMediFilter.PATIENT_TYPE_IDs = (from m in lstHisPatientTypeSelecteds select m.ID).ToList();
                }

                var _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMediFilter, param);

                if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0 && this.dicServiceReq != null && this.dicServiceReq.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    List<List<V_HIS_EXP_MEST_MEDICINE>> expMestMedicineGroups = new List<List<V_HIS_EXP_MEST_MEDICINE>>();
                    if (chkTachHDSD.Checked)
                    {
                        expMestMedicineGroups = _ExpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.TUTORIAL, p.MEDICINE_ID, p.EXP_MEST_ID, p.IS_EXPEND, p.REQ_DEPARTMENT_ID, p.PATIENT_TYPE_ID }).Select(p => p.ToList()).ToList();
                    }
                    else 
                    {
                        expMestMedicineGroups = _ExpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.MEDICINE_ID, p.EXP_MEST_ID, p.IS_EXPEND, p.REQ_DEPARTMENT_ID, p.PATIENT_TYPE_ID }).Select(p => p.ToList()).ToList();
                    }

                    if (expMestMedicineGroups != null && expMestMedicineGroups.Count > 0)
                    {
                        foreach (var itemGroups in expMestMedicineGroups)
                        {
                            checkIsNoExcute = false;
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                            {
                                var serviceReq = this.dicServiceReq[this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0].SERVICE_REQ_ID ?? 0];

                                if (serviceReq != null)
                                {
                                    decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                    //if (_AMOUNT > 0)
                                    //{
                                    ExpMestMediAndMateADO expMedi = new ExpMestMediAndMateADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(expMedi, itemGroups[0]);
                                    foreach (var item in (List<V_HIS_SERE_SERV>)listSereServ)
                                    {
                                        if (item.EXP_MEST_MEDICINE_ID == expMedi.ID && item.IS_NO_EXECUTE == 1)
                                        {
                                            checkIsNoExcute = true;
                                            break;
                                        }
                                    }
                                    if (checkIsNoExcute) break;
                                    //AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, ExpMestMediAndMateADO>();
                                    //expMedi = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);
                                    if(serviceReq.USE_TIME.HasValue)
                                    {
                                        expMedi.INTRUCTION_DATE = Int64.Parse(serviceReq.USE_TIME.Value.ToString().Substring(0,8) + "000000");
                                    }
                                    else
                                    {
                                        expMedi.INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                                    }  
                                    expMedi.INTRUCTION_TIME = serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME;
                                    expMedi.REQ_DEPARTMENT_ID = serviceReq.REQUEST_DEPARTMENT_ID;
                                    expMedi.TYPE = 1;
                                    expMedi.IS_EXPEND = itemGroups[0].IS_EXPEND;
                                    expMedi.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                                    expMedi.AMOUNT = _AMOUNT;

                                    _Datas.Add(expMedi);
                                    // }
                                }
                            }
                        }
                    }

                }

                // Inventec.Common.Logging.LogSystem.Error("Loaded V_HIS_EXP_MEST_MEDICINE end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialNewThread(object param)
        {
            try
            {
                LoadDataMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> _expMestIds)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("Load V_HIS_EXP_MEST_MATERIAL start");
                bool checkIsNoExcute = false;
                List<V_HIS_SERE_SERV> listSereServ = lstsereServ;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMaterialViewFilter expMestMateFilter = new HisExpMestMaterialViewFilter();
                expMestMateFilter.EXP_MEST_IDs = _expMestIds;

                if (lstHisPatientTypeSelecteds != null && lstHisPatientTypeSelecteds.Count > 0)
                {
                    expMestMateFilter.PATIENT_TYPE_IDs = (from m in lstHisPatientTypeSelecteds select m.ID).ToList();
                }
                var _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMateFilter, param);
                //_ExpMestMaterials V_HIS_EXP_MEST_MATERIAL.ID = HIS_SERE_SERV.EXP_MEST_MATERIAL_ID;

                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0 && this.dicServiceReq != null && this.dicServiceReq.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestMaterialGroups = _ExpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.MATERIAL_ID, p.EXP_MEST_ID, p.IS_CHEMICAL_SUBSTANCE, p.IS_EXPEND, p.REQ_DEPARTMENT_ID, p.PATIENT_TYPE_ID }).Select(p => p.ToList()).ToList();
                    if (expMestMaterialGroups != null && expMestMaterialGroups.Count > 0)
                    {
                        foreach (var itemGroups in expMestMaterialGroups)
                        {
                            checkIsNoExcute = false;
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID ?? 0))
                            {
                                var serviceReq = this.dicServiceReq[this.dicExpMest[itemGroups[0].EXP_MEST_ID ?? 0].SERVICE_REQ_ID ?? 0];
                                if (serviceReq != null)
                                {

                                    decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT - (p.TH_AMOUNT ?? 0));
                                    // if (_AMOUNT > 0)
                                    //{
                                    ExpMestMediAndMateADO expMate = new ExpMestMediAndMateADO();
                                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(expMate, itemGroups[0]);

                                    foreach (var item in listSereServ)
                                    {
                                        if (item.EXP_MEST_MATERIAL_ID == expMate.ID && item.IS_NO_EXECUTE == 1)
                                        {
                                            checkIsNoExcute = true;
                                            break;
                                        }
                                    }
                                    if (checkIsNoExcute) break;
                                    //AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, ExpMestMediAndMateADO>();
                                    //expMate = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);
                                    expMate.MEDICINE_TYPE_NAME = itemGroups[0].MATERIAL_TYPE_NAME;
                                    expMate.MEDICINE_TYPE_CODE = itemGroups[0].MATERIAL_TYPE_CODE;
                                    expMate.MEDICINE_TYPE_ID = itemGroups[0].MATERIAL_TYPE_ID;
                                    expMate.MEDICINE_ID = itemGroups[0].MATERIAL_ID; 
                                    if (serviceReq.USE_TIME.HasValue)
                                    {
                                        expMate.INTRUCTION_DATE = Int64.Parse(serviceReq.USE_TIME.Value.ToString().Substring(0, 8) + "000000");
                                    }
                                    else
                                    {
                                        expMate.INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                                    }
                                    expMate.INTRUCTION_TIME = serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME;
                                    expMate.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                                    expMate.AMOUNT = _AMOUNT;
                                    expMate.REQ_DEPARTMENT_ID = serviceReq.REQUEST_DEPARTMENT_ID;
                                    expMate.TYPE = 2;
                                    expMate.IS_EXPEND = itemGroups[0].IS_EXPEND;

                                    if (itemGroups[0].IS_CHEMICAL_SUBSTANCE == 1)
                                    {
                                        expMate.TYPE = 3;
                                    }
                                    expMate.IS_CHEMICAL_SUBSTANCE = itemGroups[0].IS_CHEMICAL_SUBSTANCE;
                                    _Datas.Add(expMate);
                                    //}
                                }
                            }
                        }
                    }
                }

                // Inventec.Common.Logging.LogSystem.Error("Loaded V_HIS_EXP_MEST_MATERIAL end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBloodNewThread(object param)
        {
            try
            {
                LoadDataBlood((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBlood(List<long> _expMestIds)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("Load V_HIS_EXP_MEST_BLOOD start");
                bool checkIsNoExcute = false;
                List<V_HIS_SERE_SERV> listSereServ = lstsereServ;
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_IDs = _expMestIds;
                if (lstHisPatientTypeSelecteds != null && lstHisPatientTypeSelecteds.Count > 0)
                {
                    bloodFilter.PATIENT_TYPE_IDs = (from m in lstHisPatientTypeSelecteds select m.ID).ToList();
                }
                var _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);

                //_ExpMestBloods V_HIS_EXP_MEST_BLOOD.BLOOD_ID = HIS_SERE_SERV.BLOOD_ID;

                if (_ExpMestBloods != null && _ExpMestBloods.Count > 0 && this.dicServiceReq != null && this.dicServiceReq.Count > 0 && this.dicExpMest != null && this.dicExpMest.Count > 0)
                {
                    var expMestBllodGroups = _ExpMestBloods.GroupBy(p => new { p.BLOOD_ID, p.EXP_MEST_ID, p.REQ_DEPARTMENT_ID, p.PATIENT_TYPE_ID }).Select(p => p.ToList()).ToList();
                    if (expMestBllodGroups != null && expMestBllodGroups.Count > 0)
                    {
                        foreach (var itemGroups in expMestBllodGroups)
                        {
                            checkIsNoExcute = false;
                            if (this.dicExpMest.ContainsKey(itemGroups[0].EXP_MEST_ID))
                            {
                                var serviceReq = this.dicServiceReq[this.dicExpMest[itemGroups[0].EXP_MEST_ID].SERVICE_REQ_ID ?? 0];
                                if (serviceReq != null)
                                {
                                    decimal _AMOUNT = itemGroups.Count;
                                    if (_AMOUNT > 0)
                                    {
                                        ExpMestMediAndMateADO expMestBlood = new ExpMestMediAndMateADO();
                                        Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMediAndMateADO>(expMestBlood, itemGroups[0]);
                                        //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD, ExpMestMediAndMateADO>();
                                        //expMestBlood = AutoMapper.Mapper.Map<ExpMestMediAndMateADO>(itemGroups[0]);

                                        foreach (var item in listSereServ)
                                        {
                                            if (item.BLOOD_ID == expMestBlood.ID && item.IS_NO_EXECUTE == 1)
                                            {
                                                checkIsNoExcute = true;
                                                break;
                                            }
                                        }
                                        if (checkIsNoExcute) break;
                                        expMestBlood.AMOUNT = _AMOUNT;
                                        expMestBlood.PRICE = itemGroups[0].PRICE;
                                        expMestBlood.MEDICINE_TYPE_ID = itemGroups[0].BLOOD_TYPE_ID;
                                        expMestBlood.MEDICINE_ID = itemGroups[0].BLOOD_ID;
                                        if (serviceReq.USE_TIME.HasValue)
                                        {
                                            expMestBlood.INTRUCTION_DATE = Int64.Parse(serviceReq.USE_TIME.Value.ToString().Substring(0, 8) + "000000");
                                        }
                                        else
                                        {
                                            expMestBlood.INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                                        }
                                        expMestBlood.INTRUCTION_TIME = serviceReq.USE_TIME ?? serviceReq.INTRUCTION_TIME;
                                        expMestBlood.MEDICINE_TYPE_CODE = itemGroups[0].BLOOD_TYPE_CODE;
                                        expMestBlood.MEDICINE_TYPE_NAME = itemGroups[0].BLOOD_TYPE_NAME;
                                        expMestBlood.PRICE = itemGroups[0].IMP_PRICE;
                                        expMestBlood.Service_Type_Id = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU;
                                        expMestBlood.REQ_DEPARTMENT_ID = serviceReq.REQUEST_DEPARTMENT_ID;
                                        expMestBlood.TYPE = 4;
                                        _Datas.Add(expMestBlood);
                                    }
                                }
                            }
                        }
                    }

                }
                // Inventec.Common.Logging.LogSystem.Error("Loaded V_HIS_EXP_MEST_BLOOD end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xu Ly Dic
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadProcessor(object param)
        {
            Thread threadServiceReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessorServiceReqNewThread));
            Thread threadSereServ = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessorSereServNewThread));
            Thread threadExpMest = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessorExpMestNewThread));
            try
            {
                threadServiceReq.Start(param);
                threadSereServ.Start(param);
                threadExpMest.Start(param);

                threadServiceReq.Join();
                threadSereServ.Join();
                threadExpMest.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadServiceReq.Abort();
                threadExpMest.Abort();
            }
        }

        private void ProcessorSereServNewThread(object param)
        {
            try
            {
                ProcessorSereServ((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorSereServ(List<long> param)
        {
            try
            {
                //foreach (var item in this._ServiceReqs)
                //{
                //    if (!dicServiceReq.ContainsKey(item.ID))
                //    {
                //        dicServiceReq[item.ID] = new HIS_SERVICE_REQ();
                //        dicServiceReq[item.ID] = item;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorServiceReqNewThread(object param)
        {
            try
            {
                ProcessorServiceReq((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorServiceReq(List<long> _serviceReqIds)
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Error("Load Dic HIS_SERVICE_REQ start");
                foreach (var item in this._ServiceReqs)
                {
                    if (!dicServiceReq.ContainsKey(item.ID))
                    {
                        dicServiceReq[item.ID] = new HIS_SERVICE_REQ();
                        dicServiceReq[item.ID] = item;
                    }
                }

                // Inventec.Common.Logging.LogSystem.Error("Loaded Dic HIS_SERVICE_REQ end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorExpMestNewThread(object param)
        {
            try
            {
                ProcessorExpMest((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorExpMest(List<long> _serviceReqIds)
        {
            try
            {
                // Inventec.Common.Logging.LogSystem.Error("Load HIS_EXP_MEST start");
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestFilter filter = new HisExpMestFilter();
                HisServiceReqFilter serReqFilter = new HisServiceReqFilter();
                serReqFilter.IDs = _serviceReqIds;
                serReqFilter.IS_NO_EXECUTE = false;

                var serReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serReqFilter, param);
                if (serReqs != null && serReqs.Count > 0)
                {
                    //serReqs = serReqs.Where(p => p.IS_NO_EXECUTE != 1).ToList();
                    List<long> serReqIds = new List<long>();
                    serReqIds = serReqs.Select(p => p.ID).ToList();
                    filter.SERVICE_REQ_IDs = serReqIds;
                    this._ExpMests = new List<HIS_EXP_MEST>();
                    this._ExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, filter, param);

                    if (this._ExpMests != null && this._ExpMests.Count > 0)
                    {
                        foreach (var item in this._ExpMests)
                        {
                            if (!dicExpMest.ContainsKey(item.ID))
                            {
                                dicExpMest[item.ID] = new HIS_EXP_MEST();
                                dicExpMest[item.ID] = item;
                            }
                        }
                    }
                }


                //  Inventec.Common.Logging.LogSystem.Error("Loaded HIS_EXP_MEST end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khoi tao chinh sach gia
        /// </summary>
        private void CreatePatyAll()
        {
            try
            {
                MOS.Filter.HisServicePatyFilter filter = new HisServicePatyFilter();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SereServADO : HIS_SERE_SERV
    {
        public short? IS_CHEMICAL_SUBSTANCE { set; get; }
        public string SERVICE_UNIT_NAME { set; get; }

        public SereServADO() { }

        public SereServADO(HIS_SERE_SERV data, List<HIS_SERVICE_UNIT> _serviceUnits)
        {
            try
            {
                if (data != null && _serviceUnits != null && _serviceUnits.Count > 0)
                {

                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);
                    var unitName = _serviceUnits.FirstOrDefault(p => p.ID == data.TDL_SERVICE_UNIT_ID);
                    this.SERVICE_UNIT_NAME = unitName != null ? unitName.SERVICE_UNIT_NAME : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public SereServADO(HIS_SERE_SERV data, List<HIS_SERVICE_UNIT> _serviceUnits, List<HIS_MATERIAL_TYPE> _materialTypes)
        {
            try
            {
                if (data != null && _serviceUnits != null && _serviceUnits.Count > 0)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);
                    var unitName = _serviceUnits.FirstOrDefault(p => p.ID == data.TDL_SERVICE_UNIT_ID);
                    this.SERVICE_UNIT_NAME = unitName != null ? unitName.SERVICE_UNIT_NAME : null;

                    if (data.MATERIAL_ID > 0 && _materialTypes != null && _materialTypes.Count > 0)
                    {
                        var ser = _materialTypes.FirstOrDefault(p => p.SERVICE_ID == data.SERVICE_ID);
                        this.IS_CHEMICAL_SUBSTANCE = ser != null ? ser.IS_CHEMICAL_SUBSTANCE : null;
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
