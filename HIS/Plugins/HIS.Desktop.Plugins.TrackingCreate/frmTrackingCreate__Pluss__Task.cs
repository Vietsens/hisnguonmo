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
using AutoMapper;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using MPS.ADO.TrackingPrint;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class frmTrackingCreateNew : FormBase
    {
        List<long> _ServiceReqIdsTab1 = new List<long>();
        List<long> _ServiceReqIdsTab2 = new List<long>();
        private async Task InitUser()
        {
            try
            {
                Action myaction = () =>
                {
                    ListUsser = BackendDataWorker.Get<ACS_USER>() ?? new List<ACS_USER>();
                    ListUsser = ListUsser.Where(o => !string.IsNullOrEmpty(o.LOGINNAME) && !string.IsNullOrEmpty(o.USERNAME)).ToList();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLogin, ListUsser, controlEditorADO);
                cboLogin.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitTrackingTemp()
        {
            try
            {
                Action myaction = () =>
                {
                    trackingTemps = LoadTrackingTemp();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                InitComboTrackingTemp(trackingTemps);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitTrackingOld()
        {
            try
            {
                Action myaction = () =>
                {
                    HisTrackingADO = LoadTrackingOld();
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                InitComboTrackingOld(HisTrackingADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitIcd()
        {
            try
            {
                List<HIS_TREATMENT> rs = null;
                Action myaction = () =>
                {
                    if (this.treatmentId > 0)
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisTreatmentFilter filter = new MOS.Filter.HisTreatmentFilter();
                        filter.ID = this.treatmentId;
                        rs = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, param);
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                this._Treatment = rs.FirstOrDefault();
                HIS.UC.Icd.ADO.IcdInputADO icd = new HIS.UC.Icd.ADO.IcdInputADO();
                //icd.ICD_ID = rs.FirstOrDefault().ICD_ID;
                icd.ICD_CODE = _Treatment.ICD_CODE;
                icd.ICD_NAME = _Treatment.ICD_NAME;
                if (ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, icd);
                }

                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = _Treatment.ICD_SUB_CODE;
                subIcd.ICD_TEXT = _Treatment.ICD_TEXT;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }

                //dùng UC
                //txtIcdExtraName.Text = _Treatment.ICD_TEXT;
                //txtIcdExtraCode.Text = _Treatment.ICD_SUB_CODE;

                HIS.UC.Icd.ADO.IcdInputADO icdYhct = new HIS.UC.Icd.ADO.IcdInputADO();
                icdYhct.ICD_CODE = _Treatment.TRADITIONAL_ICD_CODE;
                icdYhct.ICD_NAME = _Treatment.TRADITIONAL_ICD_NAME;
                if (ucIcdYhct != null)
                {
                    icdYhctProcessor.Reload(ucIcdYhct, icdYhct);
                }

                if (!String.IsNullOrWhiteSpace(_Treatment.TRADITIONAL_ICD_SUB_CODE) || !String.IsNullOrWhiteSpace(_Treatment.TRADITIONAL_ICD_TEXT))
                {
                    SecondaryIcdDataADO subYhctIcd = new SecondaryIcdDataADO();
                    subYhctIcd.ICD_SUB_CODE = _Treatment.TRADITIONAL_ICD_SUB_CODE;
                    subYhctIcd.ICD_TEXT = _Treatment.TRADITIONAL_ICD_TEXT;
                    if (ucSecondaryIcdYhct != null)
                    {
                        subIcdYhctProcessor.Reload(ucSecondaryIcdYhct, subYhctIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitConfigAcs()
        {
            try
            {
                string key = "";
                Action myaction = () =>
                {
                    CommonParam param = new CommonParam();
                    SDA.Filter.SdaConfigAppFilter configAppFilter = new SDA.Filter.SdaConfigAppFilter();
                    configAppFilter.APP_CODE_ACCEPT = GlobalVariables.APPLICATION_CODE;
                    configAppFilter.KEY = "CONFIG_KEY__HIS_PLUGINS_TRACKING_LIST__IS_SIGN_IS_PRINT_DOCUMENT_SIGNED";

                    _currentConfigApp = new BackendAdapter(param).Get<List<SDA_CONFIG_APP>>("api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param).FirstOrDefault();
                    if (_currentConfigApp != null)
                    {
                        key = _currentConfigApp.DEFAULT_VALUE;
                        SDA.Filter.SdaConfigAppUserFilter appUserFilter = new SDA.Filter.SdaConfigAppUserFilter();
                        appUserFilter.LOGINNAME = this.loginName;
                        appUserFilter.CONFIG_APP_ID = _currentConfigApp.ID;
                        currentConfigAppUser = new BackendAdapter(param).Get<List<SDA_CONFIG_APP_USER>>("api/SdaConfigAppUser/Get", ApiConsumers.SdaConsumer, appUserFilter, param).FirstOrDefault();
                        if (currentConfigAppUser != null)
                        {
                            key = currentConfigAppUser.VALUE;
                        }
                    }
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (!string.IsNullOrEmpty(key))
                {
                    _ConfigADO = (ConfigADO)Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigADO>(key);
                    if (_ConfigADO != null)
                    {
                        if (_ConfigADO.IsSign == "1")
                            chkSign.Checked = true;
                        else
                            chkSign.Checked = false;
                        if (_ConfigADO.IsPrintDocumentSigned == "1")
                            chkPrintDocumentSigned.Checked = true;
                        else
                            chkPrintDocumentSigned.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitDhst()
        {
            try
            {
                List<HIS_DHST> rsDhst = null;
                List<HIS_DHST> rsDhstTracking = null;
                Action myaction = () =>
                {
                    if (currentTracking != null)
                    {

                        MOS.Filter.HisDhstFilter dhstFilter = new MOS.Filter.HisDhstFilter();
                        dhstFilter.TRACKING_ID = currentTracking.ID;
                        rsDhstTracking = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, new CommonParam());
                    }

                    HIS_DHST rs = new HIS_DHST();
                    MOS.Filter.HisDhstFilter dhstFilter1 = new MOS.Filter.HisDhstFilter();
                    dhstFilter1.TREATMENT_ID = treatmentId;
                    rsDhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter1, new CommonParam());
                };
                Task task = new Task(myaction);
                task.Start();

                await task;
                if (Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_SHOWLASTEST_DHST)) == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && treatmentId > 0)
                {
                    if (rsDhst != null && rsDhst.Count > 0)
                    {
                        if (action == GlobalVariables.ActionAdd)
                        {
                            var dhst = rsDhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                            FillDataDhstToControl(dhst);
                            if (chkUpdateTimeDHST.Checked && dtTrackingTime != null && dtTrackingTime.DateTime != DateTime.MinValue)
                                dhstProcessor.SetExecuteTime(ucControlDHST, Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTrackingTime.DateTime));

                        }
                        else
                            FillDataDhstToControl(rsDhstTracking.FirstOrDefault());
                    }
                }
                else
                {
                    if (rsDhstTracking != null && rsDhstTracking.Count > 0)
                    {
                        _Dhst = rsDhstTracking.FirstOrDefault();
                        FillDataDhstToControl(rsDhstTracking.FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();

                serviceReqFilter.TREATMENT_ID = this.treatmentId;

                if (!isSearch && this.currentTracking != null)
                {
                    serviceReqFilter.TRACKING_ID = currentTracking.ID;
                }
                else
                {
                    if (dtTimeFromNew.EditValue != null && dtTimeFromNew.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFromNew.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dtTimeToNew.EditValue != null && dtTimeToNew.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeToNew.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                }

                if (this.currentTracking != null)
                {
                    //TODO
                }
                else
                {
                    var work = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                    //if (work != null)
                    //    serviceReqFilter.REQUEST_DEPARTMENT_ID = work.DepartmentId;
                }

                //if (chkIsMineNew.Checked)
                //{
                //	serviceReqFilter.REQUEST_LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //}

                rsServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                if (rsServiceReq != null && rsServiceReq.Count > 0)
                {

                    if (chkIsMineNew.Checked)
                    {
                        rsServiceReq = rsServiceReq.Where(o => o.REQUEST_LOGINNAME == this.loginName || o.IS_TEMPORARY_PRES == 1).ToList();
                    }
                    if (this.currentTracking != null)
                    {
                        this._ServiceReqByTrackings = new List<HIS_SERVICE_REQ>();
                        this._ServiceReqByTrackings = rsServiceReq.Where(p => p.TRACKING_ID == this.currentTracking.ID).ToList();
                        rsServiceReq = rsServiceReq.Where(p => p.TRACKING_ID == null || p.TRACKING_ID == this.currentTracking.ID).ToList();
                    }
                    else
                    {
                        rsServiceReq = rsServiceReq.Where(p => p.TRACKING_ID == null).ToList();
                    }
                }


                rsSereServ = new List<HisSereServADONumOrder>();
                _ServiceReqIdsTab1 = new List<long>();

                if (rsServiceReq != null && rsServiceReq.Count > 0)
                {


                    _ServiceReqIdsTab1 = rsServiceReq.Select(p => p.ID).Distinct().ToList();
                    MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                    ssFilter.SERVICE_REQ_IDs = _ServiceReqIdsTab1;
                    ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>();
                    ssFilter.TDL_SERVICE_TYPE_IDs.AddRange(new List<long>
                    {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                    });

                    var dataSereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        rsSereServ.AddRange((from r in dataSereServs select new HisSereServADONumOrder(r)).ToList());
                    }

                    if (BloodPresOption)
                    {
                        rsSereServ = rsSereServ.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                        HisExpMestBltyReq = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
                        CommonParam param_ = new CommonParam();
                        //var hisser = BackendDataWorker.Get<HIS_SERVICE>();
                        //HisExpMestBltyReq = HisExpMestBltyReq.Where(p => p.TDL_TREATMENT_ID == this.treatmentId).ToList();
                        MOS.Filter.HisExpMestBltyReqView2Filter filter = new HisExpMestBltyReqView2Filter();
                        filter.TDL_TREATMENT_ID = this.treatmentId;
                        if (!isSearch && this.currentTracking != null)
                        {
                            filter.TRACKING_ID = currentTracking.ID;
                        }
                        else
                        {
                            if (dtTimeFromNew.EditValue != null && dtTimeFromNew.DateTime != DateTime.MinValue)
                            {
                                filter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeFromNew.EditValue).ToString("yyyyMMdd") + "000000");
                            }
                            if (dtTimeToNew.EditValue != null && dtTimeToNew.DateTime != DateTime.MinValue)
                            {
                                filter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTimeToNew.EditValue).ToString("yyyyMMdd") + "235959");
                            }
                        }
                        HisExpMestBltyReq = new BackendAdapter(param_).Get<List<V_HIS_EXP_MEST_BLTY_REQ_2>>("/api/HisExpMestBltyReq/GetView2", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param_);

                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisExpMestBltyReq), HisExpMestBltyReq));
                        if (HisExpMestBltyReq != null && HisExpMestBltyReq.Count > 0)
                        {
                            if (!isSearch && this.currentTracking != null)
                            {
                                HisExpMestBltyReq = HisExpMestBltyReq.Where(x => x.TRACKING_ID != null).ToList();
                            }
                            else
                            {
                                HisExpMestBltyReq = HisExpMestBltyReq.Where(x => x.TRACKING_ID == null || x.TRACKING_ID < 1).ToList();
                            }

                            //Inventec.Common.Logging.LogSystem.Debug("HisExpMestBltyReq____2" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisExpMestBltyReq), HisExpMestBltyReq));
                            foreach (var item in HisExpMestBltyReq)
                            {
                                HisSereServADONumOrder AddItem = new HisSereServADONumOrder();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HisSereServADONumOrder>(AddItem, item);
                                AddItem.TDL_REQUEST_LOGINNAME = item.REQUEST_LOGINNAME;
                                AddItem.IS_NO_EXECUTE = item.IS_NO_EXECUTE;
                                AddItem.TDL_SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                                AddItem.TDL_SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                AddItem.TDL_REQUEST_ROOM_ID = item.REQUEST_ROOM_ID;
                                AddItem.TDL_REQUEST_DEPARTMENT_ID = item.REQUEST_DEPARTMENT_ID;
                                AddItem.AMOUNT = item.AMOUNT;
                                AddItem.TDL_INTRUCTION_DATE = item.INTRUCTION_DATE;
                                AddItem.TDL_INTRUCTION_TIME = item.INTRUCTION_TIME;
                                AddItem.TDL_SERVICE_CODE = item.SERVICE_CODE;
                                AddItem.TDL_SERVICE_NAME = item.SERVICE_NAME;
                                AddItem.TDL_SERVICE_REQ_TYPE_ID = item.SERVICE_REQ_TYPE_ID;
                                AddItem.TDL_TREATMENT_ID = item.TDL_TREATMENT_ID;
                                AddItem.SERVICE_REQ_ID = item.SERVICE_REQ_ID;
                                AddItem.IS_ACTIVE = item.IS_ACTIVE;
                                AddItem.IS_DELETE = item.IS_DELETE;
                                AddItem.SERVICE_ID = item.SERVICE_ID;
                                var svr = rsServiceReq.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                AddItem.USE_TIME = svr != null ? svr.USE_TIME : null;
                                rsSereServ.Add(AddItem);
                            }
                        }
                    }

                    //Thuoc - VT ngoai kho
                    LoadServiceReqMetyMatys(_ServiceReqIdsTab1, rsServiceReq);

                    //Suat an
                    LoadSereServRation(_ServiceReqIdsTab1, rsServiceReq);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (_ServiceReqIdsTab1 != null && _ServiceReqIdsTab1.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    _ExpMests_input = new List<HIS_EXP_MEST>();
                    MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_IDs = _ServiceReqIdsTab1;
                    _ExpMests_input = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (_ExpMests_input != null && _ExpMests_input.Count > 0)
                    {
                        CreateThread(_ExpMests_input.Select(p => p.ID).ToList());
                    }
                    #region ---CreateTree---
                    List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
                    treeListServiceReq.DataSource = null;
                    var listRootSety = (rsSereServ != null && rsSereServ.Count > 0) ? rsSereServ.GroupBy(g => g.TDL_INTRUCTION_DATE).ToList() : null;
                    foreach (var rootSety in listRootSety)
                    {
                        var listBySety = rootSety.ToList<HisSereServADONumOrder>().GroupBy(p => p.TDL_SERVICE_TYPE_ID).ToList();
                        TreeSereServADO ssInTime = new TreeSereServADO();
                        ssInTime.CONCRETE_ID__IN_SETY = rootSety.First().TDL_INTRUCTION_DATE + "";
                        ssInTime.SERVICE_REQ_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().TDL_INTRUCTION_DATE.ToString());
                        ssInTime.TDL_INTRUCTION_DATE = rootSety.First().TDL_INTRUCTION_DATE;
                        ssInTime.LEVER = 1;
                        SereServADOs.Add(ssInTime);
                        foreach (var itemSS in listBySety)
                        {
                            TreeSereServADO ssServiceType = new TreeSereServADO();
                            ssServiceType.CONCRETE_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY + "_" + itemSS.First().TDL_SERVICE_TYPE_ID + "";
                            ssServiceType.PARENT_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY;
                            if (itemSS.First().TDL_SERVICE_TYPE_ID == 998)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Thuốc tự mua";
                                ssServiceType.IS_OUT_MEDI_MATE = true;
                            }
                            else if (itemSS.First().TDL_SERVICE_TYPE_ID == 999)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Vật tư tự mua";
                                ssServiceType.IS_OUT_MEDI_MATE = true;
                            }
                            else if (itemSS.First().TDL_SERVICE_TYPE_ID == 1000)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Suất ăn";
                                ssServiceType.IS_RATION = true;
                            }
                            else
                            {
                                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == itemSS.First().TDL_SERVICE_TYPE_ID);
                                ssServiceType.SERVICE_REQ_CODE = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "");
                            }
                            ssServiceType.TDL_SERVICE_TYPE_ID = itemSS.First().TDL_SERVICE_TYPE_ID;
                            ssServiceType.LEVER = 2;
                            ssServiceType.tabType = TreeSereServADO.TAB_TYPE.TAB_1;
                            SereServADOs.Add(ssServiceType);
                            int d = 0;
                            foreach (var itemSSChild in itemSS)
                            {
                                bool IsNotShowMediAndMate = false;
                                string CONCRETE_ID__IN_SETY = "";
                                var ServiceReq = rsServiceReq.FirstOrDefault(o => o.ID == itemSSChild.SERVICE_REQ_ID);
                                if (ServiceReq != null &&
                                    ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT &&
                                    ServiceReq.USE_TIME != null &&
                                    ServiceReq.USE_TIME > ServiceReq.INTRUCTION_DATE)
                                {
                                    string CONCRETE_ID__IN_SETY_USE_TIME = ssServiceType.CONCRETE_ID__IN_SETY + "_" + itemSSChild.USE_TIME;
                                    if (SereServADOs != null && SereServADOs.Count > 0 && !SereServADOs.Exists(o => o.CONCRETE_ID__IN_SETY == CONCRETE_ID__IN_SETY_USE_TIME))
                                    {
                                        TreeSereServADO ssServiceReqUseTime = new TreeSereServADO();
                                        ssServiceReqUseTime.LEVER = 3;
                                        ssServiceReqUseTime.CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY_USE_TIME;
                                        ssServiceReqUseTime.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;
                                        ssServiceReqUseTime.SERVICE_REQ_CODE = ssServiceType.SERVICE_REQ_CODE + " dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ServiceReq.USE_TIME.Value);
                                        ssServiceReqUseTime.TDL_SERVICE_TYPE_ID = itemSSChild.TDL_SERVICE_TYPE_ID;
                                        ssServiceReqUseTime.IsMedicinePreventive = true;
                                        SereServADOs.Add(ssServiceReqUseTime);

                                    }

                                    string CONCRETE_ID__IN_SETY_USE_TIME_CHILD = CONCRETE_ID__IN_SETY_USE_TIME + "_" + itemSSChild.SERVICE_REQ_ID;
                                    CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY_USE_TIME_CHILD;
                                    if (SereServADOs != null && SereServADOs.Count > 0 && !SereServADOs.Exists(o => o.CONCRETE_ID__IN_SETY == CONCRETE_ID__IN_SETY_USE_TIME_CHILD))
                                    {
                                        TreeSereServADO ssServiceReq = new TreeSereServADO();
                                        ssServiceReq.LEVER = 4;
                                        ssServiceReq.CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY;
                                        ssServiceReq.PARENT_ID__IN_SETY = CONCRETE_ID__IN_SETY_USE_TIME;
                                        if (itemSSChild.TDL_SERVICE_TYPE_ID == 1000)
                                        {
                                            ssServiceReq.SERVICE_REQ_CODE = itemSSChild.TDL_SERVICE_REQ_CODE + " - " + itemSSChild.RATION_TIME_NAME;
                                        }
                                        else
                                        {
                                            ssServiceReq.SERVICE_REQ_CODE = itemSSChild.TDL_SERVICE_REQ_CODE;
                                        }
                                        ssServiceReq.tabType = TreeSereServADO.TAB_TYPE.TAB_1;
                                        ssServiceReq.SERVICE_REQ_ID = itemSSChild.SERVICE_REQ_ID;

                                        if (ServiceReq != null)
                                        {
                                            ssServiceReq.TDL_SERVICE_REQ_TYPE_ID = ServiceReq.SERVICE_REQ_TYPE_ID;
                                            ssServiceReq.IS_EXECUTE_KIDNEY_PRES = ServiceReq.IS_EXECUTE_KIDNEY_PRES;
                                            ssServiceReq.PRESCRIPTION_TYPE_ID = ServiceReq.PRESCRIPTION_TYPE_ID;
                                            ssServiceReq.REQUEST_LOGINNAME = ServiceReq.REQUEST_LOGINNAME;
                                            ssServiceReq.SERVICE_REQ_STT_ID = ServiceReq.SERVICE_REQ_STT_ID;
                                            ssServiceReq.CREATOR = ServiceReq.CREATOR;
                                            ssServiceReq.TDL_REQUEST_DEPARTMENT_ID = ServiceReq.REQUEST_DEPARTMENT_ID;
                                            ssServiceReq.IS_NO_EXECUTE = ServiceReq.IS_NO_EXECUTE;
                                            ssServiceReq.TDL_TREATMENT_ID = ServiceReq.TREATMENT_ID;
                                            ssServiceReq.TDL_PATIENT_DOB = ServiceReq.TDL_PATIENT_DOB;
                                            ssServiceReq.TDL_PATIENT_NAME = ServiceReq.TDL_PATIENT_NAME;
                                            ssServiceReq.TDL_PATIENT_GENDER_NAME = ServiceReq.TDL_PATIENT_GENDER_NAME;
                                            ssServiceReq.TDL_INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                                            ssServiceReq.TDL_USE_TIME = ServiceReq.USE_TIME;
                                            ssServiceReq.TDL_INTRUCTION_DATE = ServiceReq.INTRUCTION_DATE;
                                            ssServiceReq.USED_FOR_TRACKING_ID = ServiceReq.USED_FOR_TRACKING_ID;
                                            ssServiceReq.IsServiceUseForTracking = true;
                                        }

                                        if ((ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                            || ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) && this._ServiceReqByTrackings != null
                                            && this._ServiceReqByTrackings.Count > 0)
                                        {
                                            var dataByServicereq = this._ServiceReqByTrackings.FirstOrDefault(p => p.ID == itemSSChild.SERVICE_REQ_ID);
                                            if (dataByServicereq != null)
                                            {
                                                if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                                    && dataByServicereq.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                                                {
                                                    IsNotShowMediAndMate = true;
                                                    ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                                }
                                                else if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                                    && dataByServicereq.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                                                {
                                                    IsNotShowMediAndMate = true;
                                                    ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                                }
                                            }
                                        }
                                        SereServADOs.Add(ssServiceReq);
                                    }



                                }
                                else
                                {
                                    string CONCRETE_ID__IN_SETY_NOT_USE_TIME = ssServiceType.CONCRETE_ID__IN_SETY + "_" + itemSSChild.SERVICE_REQ_ID;
                                    CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY_NOT_USE_TIME;
                                    if (SereServADOs != null && SereServADOs.Count > 0 && !SereServADOs.Exists(o => o.CONCRETE_ID__IN_SETY == CONCRETE_ID__IN_SETY_NOT_USE_TIME))
                                    {
                                        TreeSereServADO ssServiceReq = new TreeSereServADO();
                                        ssServiceReq.LEVER = 3;
                                        ssServiceReq.CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY;
                                        ssServiceReq.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;
                                        if (itemSSChild.TDL_SERVICE_TYPE_ID == 1000)
                                        {
                                            ssServiceReq.SERVICE_REQ_CODE = itemSSChild.TDL_SERVICE_REQ_CODE + " - " + itemSSChild.RATION_TIME_NAME;
                                        }
                                        else
                                        {
                                            ssServiceReq.SERVICE_REQ_CODE = itemSSChild.TDL_SERVICE_REQ_CODE;
                                        }
                                        ssServiceReq.tabType = TreeSereServADO.TAB_TYPE.TAB_1;

                                        ssServiceReq.SERVICE_REQ_ID = itemSSChild.SERVICE_REQ_ID;

                                        if (ServiceReq != null)
                                        {
                                            ssServiceReq.TDL_SERVICE_REQ_TYPE_ID = ServiceReq.SERVICE_REQ_TYPE_ID;
                                            ssServiceReq.IS_EXECUTE_KIDNEY_PRES = ServiceReq.IS_EXECUTE_KIDNEY_PRES;
                                            ssServiceReq.PRESCRIPTION_TYPE_ID = ServiceReq.PRESCRIPTION_TYPE_ID;
                                            ssServiceReq.REQUEST_LOGINNAME = ServiceReq.REQUEST_LOGINNAME;
                                            ssServiceReq.SERVICE_REQ_STT_ID = ServiceReq.SERVICE_REQ_STT_ID;
                                            ssServiceReq.CREATOR = ServiceReq.CREATOR;
                                            ssServiceReq.TDL_REQUEST_DEPARTMENT_ID = ServiceReq.REQUEST_DEPARTMENT_ID;
                                            ssServiceReq.IS_NO_EXECUTE = ServiceReq.IS_NO_EXECUTE;
                                            ssServiceReq.TDL_TREATMENT_ID = ServiceReq.TREATMENT_ID;
                                            ssServiceReq.TDL_PATIENT_DOB = ServiceReq.TDL_PATIENT_DOB;
                                            ssServiceReq.TDL_PATIENT_NAME = ServiceReq.TDL_PATIENT_NAME;
                                            ssServiceReq.TDL_PATIENT_GENDER_NAME = ServiceReq.TDL_PATIENT_GENDER_NAME;
                                            ssServiceReq.TDL_INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                                            ssServiceReq.TDL_USE_TIME = ServiceReq.USE_TIME;
                                            ssServiceReq.TDL_INTRUCTION_DATE = ServiceReq.INTRUCTION_DATE;
                                        }

                                        if ((ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                            || ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) && this._ServiceReqByTrackings != null
                                            && this._ServiceReqByTrackings.Count > 0)
                                        {
                                            var dataByServicereq = this._ServiceReqByTrackings.FirstOrDefault(p => p.ID == itemSSChild.SERVICE_REQ_ID);
                                            if (dataByServicereq != null)
                                            {
                                                if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                                    && dataByServicereq.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                                                {
                                                    IsNotShowMediAndMate = true;
                                                    ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                                }
                                                else if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                                    && dataByServicereq.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                                                {
                                                    IsNotShowMediAndMate = true;
                                                    ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                                }
                                            }
                                        }
                                        SereServADOs.Add(ssServiceReq);
                                    }
                                }


                                d++;
                                var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == itemSSChild.TDL_SERVICE_UNIT_ID);
                                TreeSereServADO leaf = new TreeSereServADO(itemSSChild);
                                leaf.LEVER = 5;
                                leaf.CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY + "_" + d;
                                leaf.PARENT_ID__IN_SETY = CONCRETE_ID__IN_SETY;

                                leaf.SERVICE_REQ_CODE = itemSSChild.TDL_SERVICE_NAME;
                                leaf.SERVICE_UNIT_NAME = (serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : itemSSChild.TDL_SERVICE_UNIT_NAME);
                                leaf.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                leaf.IsNotShowOutMediAndMate = (itemSSChild.TDL_SERVICE_REQ_TYPE_ID == 999 || itemSSChild.TDL_SERVICE_REQ_TYPE_ID == 998);
                                leaf.NUM_ORDER = itemSSChild.NUM_ORDER;
                                leaf.tabType = TreeSereServADO.TAB_TYPE.TAB_1;
                                SereServADOs.Add(leaf);

                            }

                        }
                    }


                    SereServADOs = SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_DATE).ThenBy(p => p.NUM_ORDER).ToList();
                    foreach (var item in SereServADOs)
                    {
                        item.TDL_REQUEST_ROOM_ID = rsServiceReq.Where(o => o.ID == item.SERVICE_REQ_ID).Select(o => o.REQUEST_ROOM_ID).FirstOrDefault();
                    }
                    SereServADOsFirstForm = SereServADOs;

                    BindingList<TreeSereServADO> records = new BindingList<TreeSereServADO>(SereServADOs);
                    treeListServiceReq.DataSource = records;
                    treeListServiceReq.ExpandAll();
                    if (chkChiLayYLTuBBNew.Checked)
                    {
                        treeSereServ_CheckYLFromBB(treeListServiceReq.Nodes);
                    }
                    else
                    {
                        treeSereServ_CheckAllNode(treeListServiceReq.Nodes);
                    }
                    #endregion
                }
                else
                {
                    treeListServiceReq.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();

                serviceReqFilter.TREATMENT_ID = this.treatmentId;

                if (!isSearch && this.currentTracking != null)
                {
                    serviceReqFilter.TRACKING_ID__OR__USED_FOR_TRACKING_ID = currentTracking.ID;
                }
                else
                {
                    if (dteFromPreventive.EditValue != null && dteFromPreventive.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.USE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteFromPreventive.EditValue).ToString("yyyyMMdd") + "000000");
                    }
                    if (dteToPreventive.EditValue != null && dteToPreventive.DateTime != DateTime.MinValue)
                    {
                        serviceReqFilter.USE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteToPreventive.EditValue).ToString("yyyyMMdd") + "235959");
                    }
                }

                if (this.currentTracking != null)
                {
                    //TODO
                }
                else
                {
                    var work = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                    //if (work != null)
                    //    serviceReqFilter.REQUEST_DEPARTMENT_ID = work.DepartmentId;
                }

                if (cboLogin.EditValue != null)
                {
                    serviceReqFilter.REQUEST_LOGINNAME__EXACT = cboLogin.EditValue.ToString();
                }

                rsServiceReqTab2 = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);


                if (rsServiceReqTab2 != null && rsServiceReqTab2.Count > 0)
                {
                    rsServiceReqTab2 = rsServiceReqTab2.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && o.USE_TIME > o.INTRUCTION_DATE).ToList();
                    if (rsServiceReqTab2 != null && rsServiceReqTab2.Count > 0)
                    {
                        if (this.currentTracking != null)
                        {
                            this._ServiceReqByTrackingsTab2 = new List<HIS_SERVICE_REQ>();
                            this._ServiceReqByTrackingsTab2 = rsServiceReqTab2.Where(p => p.USED_FOR_TRACKING_ID == this.currentTracking.ID).ToList();
                            rsServiceReqTab2 = rsServiceReqTab2.Where(p => p.USED_FOR_TRACKING_ID == null || p.USED_FOR_TRACKING_ID == this.currentTracking.ID).ToList();
                        }
                        else
                        {
                            rsServiceReqTab2 = rsServiceReqTab2.Where(p => p.USED_FOR_TRACKING_ID == null).ToList();
                        }
                    }
                }
                rsSereServTab2 = new List<HisSereServADONumOrder>();
                _ServiceReqIdsTab2 = new List<long>();
                if (rsServiceReqTab2 != null && rsServiceReqTab2.Count > 0)
                {
                    _ServiceReqIdsTab2 = rsServiceReqTab2.Select(p => p.ID).Distinct().ToList();
                    MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                    ssFilter.SERVICE_REQ_IDs = _ServiceReqIdsTab2;
                    ssFilter.TDL_SERVICE_TYPE_IDs = new List<long>();
                    ssFilter.TDL_SERVICE_TYPE_IDs.AddRange(new List<long>
                    {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                    });
                    var dataSereServs = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        rsSereServTab2.AddRange((from r in dataSereServs
                                                 join a in rsServiceReqTab2 on r.SERVICE_REQ_ID equals a.ID
                                                 select new HisSereServADONumOrder(r, a)).ToList());
                    }

                    _ExpMests_inputTab2 = new List<HIS_EXP_MEST>();

                    LoadServiceReqMetyMatysTab2(_ServiceReqIdsTab2, rsServiceReqTab2);
                    LoadSereServRationTab2(_ServiceReqIdsTab2, rsServiceReqTab2);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                CommonParam param = new CommonParam();
                if (_ServiceReqIdsTab2 != null && _ServiceReqIdsTab2.Count > 0)
                {
                    MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                    expMestFilter.SERVICE_REQ_IDs = _ServiceReqIdsTab2;
                    _ExpMests_inputTab2 = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (_ExpMests_inputTab2 != null && _ExpMests_inputTab2.Count > 0)
                    {
                        CreateThreadTab2(_ExpMests_inputTab2.Select(p => p.ID).ToList());
                    }
                    List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
                    treeListPreventive.DataSource = null;
                    var listRootSety = (rsSereServTab2 != null && rsSereServTab2.Count > 0) ? rsSereServTab2.GroupBy(g => g.USE_TIME).ToList() : null;
                    foreach (var rootSety in listRootSety)
                    {
                        var listBySety = rootSety.ToList<HisSereServADONumOrder>().GroupBy(p => p.TDL_SERVICE_TYPE_ID).ToList();
                        TreeSereServADO ssInTime = new TreeSereServADO();
                        ssInTime.CONCRETE_ID__IN_SETY = rootSety.First().USE_TIME + "";
                        ssInTime.SERVICE_REQ_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().USE_TIME.ToString());
                        ssInTime.TDL_INTRUCTION_DATE = rootSety.First().TDL_INTRUCTION_DATE;
                        ssInTime.LEVER = 1;
                        SereServADOs.Add(ssInTime);
                        foreach (var itemSS in listBySety)
                        {
                            var listBySety234 = itemSS.ToList<HisSereServADONumOrder>().GroupBy(p => p.SERVICE_REQ_ID).ToList();
                            TreeSereServADO ssServiceType = new TreeSereServADO();
                            ssServiceType.CONCRETE_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY + "_" + itemSS.First().TDL_SERVICE_TYPE_ID + "";
                            ssServiceType.PARENT_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY;
                            if (itemSS.First().TDL_SERVICE_TYPE_ID == 998)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Thuốc tự mua";
                                ssServiceType.IS_OUT_MEDI_MATE = true;
                            }
                            else if (itemSS.First().TDL_SERVICE_TYPE_ID == 999)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Vật tư tự mua";
                                ssServiceType.IS_OUT_MEDI_MATE = true;
                            }
                            else if (itemSS.First().TDL_SERVICE_TYPE_ID == 1000)
                            {
                                ssServiceType.SERVICE_REQ_CODE = "Suất ăn";
                                ssServiceType.IS_RATION = true;
                            }
                            else
                            {
                                var serviceType = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == itemSS.First().TDL_SERVICE_TYPE_ID);
                                ssServiceType.SERVICE_REQ_CODE = (serviceType != null ? serviceType.SERVICE_TYPE_NAME : "");
                            }
                            ssServiceType.tabType = TreeSereServADO.TAB_TYPE.TAB_2;
                            ssServiceType.TDL_SERVICE_TYPE_ID = itemSS.First().TDL_SERVICE_TYPE_ID;
                            ssServiceType.LEVER = 2;
                            SereServADOs.Add(ssServiceType);
                            foreach (var itemSR in listBySety234)
                            {
                                bool IsNotShowMediAndMate = false;

                                TreeSereServADO ssServiceReq = new TreeSereServADO();
                                ssServiceReq.LEVER = 3;
                                ssServiceReq.CONCRETE_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY + "_" + itemSR.First().SERVICE_REQ_ID;
                                ssServiceReq.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;
                                if (itemSR.First().TDL_SERVICE_TYPE_ID == 1000)
                                {
                                    ssServiceReq.SERVICE_REQ_CODE = itemSR.First().TDL_SERVICE_REQ_CODE + " - " + itemSR.First().RATION_TIME_NAME;
                                }
                                else
                                {
                                    ssServiceReq.SERVICE_REQ_CODE = itemSR.First().TDL_SERVICE_REQ_CODE;
                                }
                                ssServiceReq.tabType = TreeSereServADO.TAB_TYPE.TAB_2;
                                var ServiceReq = rsServiceReqTab2.FirstOrDefault(o => o.ID == itemSR.First().SERVICE_REQ_ID);
                                ssServiceReq.SERVICE_REQ_ID = itemSR.First().SERVICE_REQ_ID;
                                if (ServiceReq != null)
                                {
                                    ssServiceReq.TDL_SERVICE_REQ_TYPE_ID = ServiceReq.SERVICE_REQ_TYPE_ID;
                                    ssServiceReq.IS_EXECUTE_KIDNEY_PRES = ServiceReq.IS_EXECUTE_KIDNEY_PRES;
                                    ssServiceReq.PRESCRIPTION_TYPE_ID = ServiceReq.PRESCRIPTION_TYPE_ID;
                                    ssServiceReq.REQUEST_LOGINNAME = ServiceReq.REQUEST_LOGINNAME;
                                    ssServiceReq.SERVICE_REQ_STT_ID = ServiceReq.SERVICE_REQ_STT_ID;
                                    ssServiceReq.CREATOR = ServiceReq.CREATOR;
                                    ssServiceReq.TDL_REQUEST_DEPARTMENT_ID = ServiceReq.REQUEST_DEPARTMENT_ID;
                                    ssServiceReq.IS_NO_EXECUTE = ServiceReq.IS_NO_EXECUTE;
                                    ssServiceReq.TDL_TREATMENT_ID = ServiceReq.TREATMENT_ID;
                                    ssServiceReq.TDL_PATIENT_DOB = ServiceReq.TDL_PATIENT_DOB;
                                    ssServiceReq.TDL_PATIENT_NAME = ServiceReq.TDL_PATIENT_NAME;
                                    ssServiceReq.TDL_PATIENT_GENDER_NAME = ServiceReq.TDL_PATIENT_GENDER_NAME;
                                    ssServiceReq.TDL_INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                                    ssServiceReq.TDL_USE_TIME = ServiceReq.USE_TIME;
                                    ssServiceReq.TDL_INTRUCTION_DATE = ServiceReq.INTRUCTION_DATE;
                                    ssServiceReq.TDL_TRACKING_ID = ServiceReq.TRACKING_ID;
                                    ssServiceReq.USED_FOR_TRACKING_ID = ServiceReq.USED_FOR_TRACKING_ID;
                                    ssServiceReq.IsServiceUseForTracking = true;
                                }
                                if ((ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                    || ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC) && this._ServiceReqByTrackingsTab2 != null
                                    && this._ServiceReqByTrackingsTab2.Count > 0)
                                {
                                    var dataByServicereq = this._ServiceReqByTrackingsTab2.FirstOrDefault(p => p.ID == itemSR.First().SERVICE_REQ_ID);
                                    if (dataByServicereq != null)
                                    {
                                        if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                            && dataByServicereq.IS_NOT_SHOW_MATERIAL_TRACKING == 1)
                                        {
                                            IsNotShowMediAndMate = true;
                                            ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                        }
                                        else if (ssServiceType.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                            && dataByServicereq.IS_NOT_SHOW_MEDICINE_TRACKING == 1)
                                        {
                                            IsNotShowMediAndMate = true;
                                            ssServiceReq.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                        }
                                    }
                                }

                                SereServADOs.Add(ssServiceReq);
                                int d = 0;

                                foreach (var item in itemSR)
                                {
                                    d++;
                                    var serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(p => p.ID == item.TDL_SERVICE_UNIT_ID);
                                    TreeSereServADO leaf = new TreeSereServADO(item);
                                    leaf.LEVER = 4;
                                    leaf.CONCRETE_ID__IN_SETY = ssServiceReq.CONCRETE_ID__IN_SETY + "_" + d;
                                    leaf.PARENT_ID__IN_SETY = ssServiceReq.CONCRETE_ID__IN_SETY;

                                    leaf.SERVICE_REQ_CODE = item.TDL_SERVICE_NAME;
                                    leaf.SERVICE_UNIT_NAME = (serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : item.TDL_SERVICE_UNIT_NAME);
                                    leaf.IsNotShowMediAndMate = IsNotShowMediAndMate;
                                    leaf.IsNotShowOutMediAndMate = (item.TDL_SERVICE_REQ_TYPE_ID == 999 || item.TDL_SERVICE_REQ_TYPE_ID == 998);
                                    leaf.NUM_ORDER = item.NUM_ORDER;
                                    leaf.tabType = TreeSereServADO.TAB_TYPE.TAB_2;

                                    SereServADOs.Add(leaf);

                                }

                            }
                        }
                    }
                    SereServADOs = SereServADOs.OrderByDescending(o => o.TDL_USE_TIME).ThenBy(p => p.NUM_ORDER).ToList();
                    foreach (var item in SereServADOs)
                    {
                        item.TDL_REQUEST_ROOM_ID = rsServiceReqTab2.Where(o => o.ID == item.SERVICE_REQ_ID).Select(o => o.REQUEST_ROOM_ID).FirstOrDefault();
                    }
                    SereServADOsFirstFormTab2 = SereServADOs;
                    BindingList<TreeSereServADO> records = new BindingList<TreeSereServADO>(SereServADOs);
                    treeListPreventive.DataSource = records;
                    treeListPreventive.ExpandAll();
                    treeSereServ_CheckAllNodeTab2(treeListPreventive.Nodes);
                }
                else
                {
                    treeListPreventive.DataSource = null;
                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
