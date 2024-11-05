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
using AutoMapper;
using DevExpress.XtraTreeList.Nodes;
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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using MPS.ADO.TrackingPrint;
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
        internal List<HisSereServADONumOrder> rsSereServ { get; set; }
        internal List<HisSereServADONumOrder> rsSereServTab2 { get; set; }
        internal List<HIS_EXP_MEST> _ExpMests_input { get; set; }
        internal List<HIS_EXP_MEST> _ExpMests_inputTab2 { get; set; }
        List<V_HIS_EXP_MEST_BLTY_REQ_2> HisExpMestBltyReq = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
        List<V_HIS_EXP_MEST_BLTY_REQ_2> HisExpMestBltyReqTab2 = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
        List<TreeSereServADO> SereServADOsFirstForm = new List<TreeSereServADO>();
        List<TreeSereServADO> SereServADOsFirstFormTab2 = new List<TreeSereServADO>();
        List<HIS_SERVICE_REQ> rsServiceReq { get; set; }
        List<HIS_SERVICE_REQ> rsServiceReqTab2 { get; set; }

        bool isSearch = false;
        void LoadDataSS(bool isSearch)
        {
            try
            {
                WaitingManager.Show();
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

                var work = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                if (cboDepartment.SelectedIndex == 0)
                    serviceReqFilter.REQUEST_DEPARTMENT_ID = work.DepartmentId;

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
                List<long> _ServiceReqIds = new List<long>();

                if (rsServiceReq != null && rsServiceReq.Count > 0)
                {

                    _ServiceReqIds = rsServiceReq.Select(p => p.ID).Distinct().ToList();

                    int startSereServ = 0;
                    int countServiceReqId_SereServ = _ServiceReqIds.Count;
                    List<HIS_SERE_SERV> dataSereServs = new List<HIS_SERE_SERV>();
                    while (countServiceReqId_SereServ > 0)
                    {
                        int limit = (countServiceReqId_SereServ <= 100) ? countServiceReqId_SereServ : 100;
                        var listSub = _ServiceReqIds.Skip(startSereServ).Take(limit).ToList();

                        MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                        ssFilter.SERVICE_REQ_IDs = listSub;
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

                        var ssSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        dataSereServs.AddRange(ssSereServ);
                        startSereServ += 100;
                        countServiceReqId_SereServ -= 100;
                    }


                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        rsSereServ.AddRange((from r in dataSereServs select new HisSereServADONumOrder(r)).ToList());
                    }

                    if (BloodPresOption)
                    {
                        rsSereServ = rsSereServ.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                        HisExpMestBltyReq = new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
                        CommonParam param_ = new CommonParam();
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
                        if (HisExpMestBltyReq != null && HisExpMestBltyReq.Count > 0 && chkIsMineNew.Checked)
                        {
                            HisExpMestBltyReq = HisExpMestBltyReq.Where(p => p.REQUEST_LOGINNAME == this.loginName).ToList();
                        }
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
                                AddItem.TDL_SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                                var svr = rsServiceReq.FirstOrDefault(o => o.ID == item.SERVICE_REQ_ID);
                                AddItem.USE_TIME = svr != null ? svr.USE_TIME : null;

                                rsSereServ.Add(AddItem);
                            }
                        }
                    }

                    //Lay Thuoc,Vt
                    int startExpMest = 0;
                    int countServiceReqId_ExpMest = _ServiceReqIds.Count;
                    _ExpMests_input = new List<HIS_EXP_MEST>();
                    while (countServiceReqId_ExpMest > 0)
                    {
                        int limit = (countServiceReqId_ExpMest <= 100) ? countServiceReqId_ExpMest : 100;
                        var listSub = _ServiceReqIds.Skip(startExpMest).Take(limit).ToList();

                        MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                        expMestFilter.SERVICE_REQ_IDs = listSub;
                        var ssExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        _ExpMests_input.AddRange(ssExpMest);
                        startExpMest += 100;
                        countServiceReqId_ExpMest -= 100;
                    }

                    if (_ExpMests_input != null && _ExpMests_input.Count > 0)
                    {
                        CreateThread(_ExpMests_input.Select(p => p.ID).ToList());
                    }

                    //Thuoc - VT ngoai kho
                    LoadServiceReqMetyMatys(_ServiceReqIds, rsServiceReq);

                    //Suat an
                    LoadSereServRation(_ServiceReqIds, rsServiceReq);


                    #region ---CreateTree---
                    List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
                    treeListServiceReq.DataSource = null;
                    var listRootSety = (rsSereServ != null && rsSereServ.Count > 0) ? rsSereServ.GroupBy(g => g.TDL_INTRUCTION_DATE).ToList() : null;
                    if (listRootSety != null && listRootSety.Count > 0)
                    {
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<HisSereServADONumOrder>().GroupBy(p => p.TDL_SERVICE_TYPE_ID).ToList();
                            TreeSereServADO ssInTime = new TreeSereServADO();
                            ssInTime.CONCRETE_ID__IN_SETY = rootSety.First().TDL_INTRUCTION_DATE + "";
                            ssInTime.SERVICE_REQ_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rootSety.First().TDL_INTRUCTION_DATE.ToString());
                            ssInTime.TDL_INTRUCTION_DATE = rootSety.First().TDL_INTRUCTION_DATE;
                            ssInTime.LEVER = 1;
                            SereServADOs.Add(ssInTime);
                            int demm = 0;
                            List<TreeSereServADO> listSSTemp = new List<TreeSereServADO>();
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
                                ssServiceType.tabType = TreeSereServADO.TAB_TYPE.TAB_1;
                                ssServiceType.TDL_SERVICE_TYPE_ID = itemSS.First().TDL_SERVICE_TYPE_ID;
                                ssServiceType.LEVER = 2;
                                SereServADOs.Add(ssServiceType);
                                int d = 0;
                                int dem = 0;
                                List<TreeSereServADO> listTemp = new List<TreeSereServADO>();
                                foreach (var itemSSChild in itemSS)
                                {
                                    bool IsNotShowMediAndMate = false;
                                    string CONCRETE_ID__IN_SETY = "";
                                    var ServiceReq = rsServiceReq.FirstOrDefault(o => o.ID == itemSSChild.SERVICE_REQ_ID);
                                    if (ServiceReq != null &&
                                        (ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN 
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA 
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT 
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL
                                        ) &&
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
                                            ssServiceReqUseTime.SERVICE_REQ_CODE = (ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                        || ServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL) ? "Dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ServiceReq.USE_TIME.Value) : ssServiceType.SERVICE_REQ_CODE + " dự trù ngày " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(ServiceReq.USE_TIME.Value);
                                            ssServiceReqUseTime.TDL_SERVICE_TYPE_ID = itemSSChild.TDL_SERVICE_TYPE_ID;
                                            ssServiceReqUseTime.IsMedicinePreventive = true;
                                            SereServADOs.Add(ssServiceReqUseTime);
                                            listTemp.Add(ssServiceReqUseTime);
                                            if (ServiceReq.IS_TEMPORARY_PRES == 1)
                                                dem++;
                                        }

                                        string CONCRETE_ID__IN_SETY_USE_TIME_CHILD = CONCRETE_ID__IN_SETY_USE_TIME + "_" + itemSSChild.SERVICE_REQ_ID;
                                        CONCRETE_ID__IN_SETY = CONCRETE_ID__IN_SETY_USE_TIME_CHILD;
                                        if (SereServADOs != null && SereServADOs.Count > 0 && !SereServADOs.Exists(o => o.CONCRETE_ID__IN_SETY == CONCRETE_ID__IN_SETY_USE_TIME_CHILD))
                                        {
                                            TreeSereServADO ssServiceReq = new TreeSereServADO();
                                            ssServiceReq.LEVER = 4;
                                            ssServiceReq.tabType = TreeSereServADO.TAB_TYPE.TAB_1;
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
                                            listTemp.Add(ssServiceReq);
                                            if (ServiceReq.IS_TEMPORARY_PRES == 1)
                                                dem++;
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
                                            listTemp.Add(ssServiceReq);
                                            if (ServiceReq.IS_TEMPORARY_PRES == 1)
                                                dem++;
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
                                if (listTemp.Count > 0 && listTemp.Count == dem)
                                {
                                    ssServiceType.IS_DISABLE = 1;
                                }
                                else
                                    ssServiceType.IS_DISABLE = 0;
                                if (ssServiceType.IS_DISABLE == 1)
                                    demm++;
                                listSSTemp.Add(ssServiceType);

                            }
                            if(listSSTemp.Count > 0 && listSSTemp.Count == demm)
                            {
                                ssInTime.IS_DISABLE = 1;
                            }
                            else
                                ssInTime.IS_DISABLE = 0;
                        }
                    }

                    #region ----- ThuHoi -----
                    //if (_ImpMestMedis != null && _ImpMestMedis.Count > 0)
                    //{
                    //    TreeSereServADO ssInTime = new TreeSereServADO();
                    //    ssInTime.CONCRETE_ID__IN_SETY = "_ImpMestMedis999999";
                    //    ssInTime.SERVICE_REQ_CODE = "THU HỒI";
                    //    ssInTime.IS_THUHOI = true;
                    //    SereServADOs.Add(ssInTime);


                    //    TreeSereServADO ssServiceType = new TreeSereServADO();
                    //    ssServiceType.CONCRETE_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY + "_" + "_ImpMestMedis999999";
                    //    ssServiceType.PARENT_ID__IN_SETY = ssInTime.CONCRETE_ID__IN_SETY;
                    //    ssServiceType.SERVICE_REQ_CODE = "Thuốc";
                    //    ssServiceType.IS_THUHOI = true;
                    //    SereServADOs.Add(ssServiceType);

                    //    int d = 0;
                    //    foreach (var item in _ImpMestMedis)
                    //    {
                    //        d++;
                    //        TreeSereServADO leaf = new TreeSereServADO();
                    //        leaf.CONCRETE_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY + "_" + d;
                    //        leaf.PARENT_ID__IN_SETY = ssServiceType.CONCRETE_ID__IN_SETY;
                    //        leaf.SERVICE_REQ_CODE = item.MEDICINE_TYPE_NAME;
                    //        leaf.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    //        leaf.AMOUNT = item.AMOUNT;
                    //        leaf.IS_THUHOI = true;
                    //        SereServADOs.Add(leaf);
                    //    }
                    //}
                    #endregion


                    SereServADOs = SereServADOs.OrderByDescending(o => o.TDL_INTRUCTION_DATE).ThenBy(p => p.NUM_ORDER).ToList();
                    foreach (var item in SereServADOs)
                    {
                        item.TDL_REQUEST_ROOM_ID = rsServiceReq.Where(o => o.ID == item.SERVICE_REQ_ID).Select(o => o.REQUEST_ROOM_ID).FirstOrDefault();
                        item.IS_TEMPORARY_PRES = rsServiceReq.Where(o => o.ID == item.SERVICE_REQ_ID).Select(o => o.IS_TEMPORARY_PRES).FirstOrDefault();
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
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDataSSTab2(bool isSearch)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new MOS.Filter.HisServiceReqFilter();

                serviceReqFilter.TREATMENT_ID = this.treatmentId;

                if (!isSearch && this.currentTracking != null)
                {
                    serviceReqFilter.TRACKING_ID__OR__USED_FOR_TRACKING_ID = currentTracking.ID;
                }

                if (dteFromPreventive.EditValue != null && dteFromPreventive.DateTime != DateTime.MinValue)
                {
                    serviceReqFilter.USE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteFromPreventive.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dteToPreventive.EditValue != null && dteToPreventive.DateTime != DateTime.MinValue)
                {
                    serviceReqFilter.USE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dteToPreventive.EditValue).ToString("yyyyMMdd") + "235959");
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
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqFilter), serviceReqFilter));
                rsServiceReqTab2 = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsServiceReqTab2.Count), rsServiceReqTab2.Count));
                if (rsServiceReqTab2 != null && rsServiceReqTab2.Count > 0)
                {
                    rsServiceReqTab2 = rsServiceReqTab2.Where(o => (o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                        || o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL) && o.USE_TIME > o.INTRUCTION_DATE).ToList();
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
                List<long> _ServiceReqIds = new List<long>();

                List<TreeSereServADO> SereServADOs = new List<TreeSereServADO>();
                if (rsServiceReqTab2 != null && rsServiceReqTab2.Count > 0)
                {
                    _ServiceReqIds = rsServiceReqTab2.Select(p => p.ID).Distinct().ToList();

                    int startSereServ = 0;
                    int countServiceReqId_SS = _ServiceReqIds.Count;
                    List<HIS_SERE_SERV> dataSereServs = new List<HIS_SERE_SERV>();
                    while (countServiceReqId_SS > 0)
                    {
                        int limit = (countServiceReqId_SS <= 100) ? countServiceReqId_SS : 100;
                        var listSub = _ServiceReqIds.Skip(startSereServ).Take(limit).ToList();
                        MOS.Filter.HisSereServFilter ssFilter = new MOS.Filter.HisSereServFilter();
                        ssFilter.SERVICE_REQ_IDs = listSub;
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
                        var ssSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        dataSereServs.AddRange(ssSereServ);
                        startSereServ += 100;
                        countServiceReqId_SS -= 100;
                    }


                    if (dataSereServs != null && dataSereServs.Count > 0)
                    {
                        rsSereServTab2.AddRange((from r in dataSereServs
                                                 join a in rsServiceReqTab2 on r.SERVICE_REQ_ID equals a.ID
                                                 select new HisSereServADONumOrder(r, a)).ToList());
                    }

                    int startExpMestTab2 = 0;
                    int countServiceReqId_ExpMestTab2 = _ServiceReqIds.Count;
                    _ExpMests_inputTab2 = new List<HIS_EXP_MEST>();
                    while (countServiceReqId_ExpMestTab2 > 0)
                    {
                        int limit = (countServiceReqId_ExpMestTab2 <= 100) ? countServiceReqId_ExpMestTab2 : 100;
                        var listSub = _ServiceReqIds.Skip(startExpMestTab2).Take(limit).ToList();

                        MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                        expMestFilter.SERVICE_REQ_IDs = listSub;
                        var ssExpMests_inputTab2 = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        _ExpMests_inputTab2.AddRange(ssExpMests_inputTab2);
                        startExpMestTab2 += 100;
                        countServiceReqId_ExpMestTab2 -= 100;
                    }

                    if (_ExpMests_inputTab2 != null && _ExpMests_inputTab2.Count > 0)
                    {
                        CreateThreadTab2(_ExpMests_inputTab2.Select(p => p.ID).ToList());
                    }
                    LoadServiceReqMetyMatysTab2(_ServiceReqIds, rsServiceReqTab2);
                    LoadSereServRationTab2(_ServiceReqIds, rsServiceReqTab2);

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
                    treeListPreventive.BeginUpdate();
                    BindingList<TreeSereServADO> records = new BindingList<TreeSereServADO>(SereServADOs);
                    treeListPreventive.DataSource = records;
                    treeListPreventive.EndUpdate();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CheckAllNodeTab2(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.CheckAll();
                        CheckNode(node);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
            }
        }

        private void CreateThread(object param)
        {
            Thread threadMetyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadExpMestMetyReqNewThread));
            Thread threadMatyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadExpMestMatyReqNewThread));

            try
            {
                threadMatyReq.Start(param);
                threadMetyReq.Start(param);
                // threadImpMest.Start(param);

                threadMetyReq.Join();
                threadMatyReq.Join();
                // threadImpMest.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
                //threadImpMest.Abort();
            }
        }

        private void CreateThreadTab2(object param)
        {
            Thread threadMetyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadExpMestMetyReqNewThreadTab2));
            Thread threadMatyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadExpMestMatyReqNewThreadTab2));

            try
            {
                threadMatyReq.Start(param);
                threadMetyReq.Start(param);

                threadMetyReq.Join();
                threadMatyReq.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
            }
        }

        private void LoadExpMestMetyReqNewThread(object param)
        {
            try
            {
                LoadExpMestMetyReq((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMetyReqNewThreadTab2(object param)
        {
            try
            {
                LoadExpMestMetyReqTab2((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMetyReqTab2(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HIS_EXP_MEST_METY_REQ> datas = new List<HIS_EXP_MEST_METY_REQ>();
                int startMetyReq = 0;
                int countExpMestIds = _expMestIds.Count;
                while (countExpMestIds > 0)
                {
                    int limit = (countExpMestIds <= 100) ? countExpMestIds : 100;
                    var listSub = _expMestIds.Skip(startMetyReq).Take(limit).ToList();

                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new MOS.Filter.HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_IDs = listSub;
                    var ssdata = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    datas.AddRange(ssdata);
                    startMetyReq += 100;
                    countExpMestIds -= 100;
                }

                if (datas != null && datas.Count > 0)
                {
                    var _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in datas)
                    {
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_inputTab2.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            ado.USE_TIME = rsServiceReqTab2.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID).USE_TIME;
                        }
                        var mediType = _medicineTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServTab2.Add(ado);
                    }
                }

                int startMedicine = 0;
                int countExpMestId_Medicine = _expMestIds.Count;
                List<HIS_EXP_MEST_MEDICINE> dataMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                while (countExpMestId_Medicine > 0)
                {
                    int limit = (countExpMestId_Medicine <= 100) ? countExpMestId_Medicine : 100;
                    var listSub = _expMestIds.Skip(startMedicine).Take(limit).ToList();

                    MOS.Filter.HisExpMestMedicineFilter medicineFilter = new MOS.Filter.HisExpMestMedicineFilter();
                    medicineFilter.EXP_MEST_IDs = listSub;
                    var ssdataMedicines = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    dataMedicines.AddRange(ssdataMedicines);
                    startMedicine += 100;
                    countExpMestId_Medicine -= 100;
                }

                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    var _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in dataMedicines)
                    {
                        if (item.IS_NOT_PRES == 1)
                            continue;
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_inputTab2.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            ado.USE_TIME = rsServiceReqTab2.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID).USE_TIME;
                        }
                        var mediType = _medicineTypes.FirstOrDefault(p => p.ID == item.TDL_MEDICINE_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServTab2.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMatyReqNewThreadTab2(object param)
        {
            try
            {
                LoadExpMestMatyReqTab2((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMatyReqTab2(List<long> _expMestIds)
        {
            try
            {
                if (this._IsMaterial)
                    return;

                CommonParam param = new CommonParam();
                int startMatyReq = 0;
                int countExpMestIds_MatyReq = _expMestIds.Count;
                List<HIS_EXP_MEST_MATY_REQ> datas = new List<HIS_EXP_MEST_MATY_REQ>();
                while (countExpMestIds_MatyReq > 0)
                {
                    int limit = (countExpMestIds_MatyReq <= 100) ? countExpMestIds_MatyReq : 100;
                    var listSub = _expMestIds.Skip(startMatyReq).Take(limit).ToList();

                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new MOS.Filter.HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_IDs = listSub;
                    var ssdata = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    datas.AddRange(ssdata);
                    startMatyReq += 100;
                    countExpMestIds_MatyReq -= 100;
                }

                if (datas != null && datas.Count > 0)
                {
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in datas)
                    {
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_inputTab2.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            ado.USE_TIME = rsServiceReqTab2.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID).USE_TIME;
                        }
                        var mediType = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MATERIAL_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServTab2.Add(ado);
                    }
                }

                int startMaterial = 0;
                int countExpMestIds_Material = _expMestIds.Count;
                List<HIS_EXP_MEST_MATERIAL> dataMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                while (countExpMestIds_Material > 0)
                {
                    int limit = (countExpMestIds_Material <= 100) ? countExpMestIds_Material : 100;
                    var listSub = _expMestIds.Skip(startMaterial).Take(limit).ToList();

                    MOS.Filter.HisExpMestMaterialFilter materialReqFilter = new MOS.Filter.HisExpMestMaterialFilter();
                    materialReqFilter.EXP_MEST_IDs = _expMestIds;
                    var ssdataMaterial = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, materialReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    dataMaterials.AddRange(ssdataMaterial);
                    startMaterial += 100;
                    countExpMestIds_Material -= 100;
                }

                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in dataMaterials)
                    {
                        if (item.IS_NOT_PRES == 1)//#19933 Thuoc K Phai Bs Ke
                            continue;
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_inputTab2.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            ado.USE_TIME = rsServiceReqTab2.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID).USE_TIME;
                        }
                        var mediType = _materialTypes.FirstOrDefault(p => p.ID == item.TDL_MATERIAL_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MATERIAL_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServTab2.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMetyReq(List<long> _expMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();

                int startMetyReq = 0;
                int countServiceReqId_MetyReq = _expMestIds.Count;
                List<HIS_EXP_MEST_METY_REQ> datas = new List<HIS_EXP_MEST_METY_REQ>();
                while (countServiceReqId_MetyReq > 0)
                {
                    int limit = (countServiceReqId_MetyReq <= 100) ? countServiceReqId_MetyReq : 100;
                    var listSub = _expMestIds.Skip(startMetyReq).Take(limit).ToList();

                    MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new MOS.Filter.HisExpMestMetyReqFilter();
                    metyReqFilter.EXP_MEST_IDs = listSub;
                    var ssdata = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    datas.AddRange(ssdata);
                    startMetyReq += 100;
                    countServiceReqId_MetyReq -= 100;
                }


                if (datas != null && datas.Count > 0)
                {
                    var _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in datas)
                    {
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_input.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            var svr = rsServiceReq.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID);
                            ado.USE_TIME = svr != null ? svr.USE_TIME : null;
                        }
                        var mediType = _medicineTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServ.Add(ado);
                    }
                }

                int startMedicine = 0;
                int countServiceReqId_Medicine = _expMestIds.Count;
                List<HIS_EXP_MEST_MEDICINE> dataMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                while (countServiceReqId_Medicine > 0)
                {
                    int limit = (countServiceReqId_Medicine <= 100) ? countServiceReqId_Medicine : 100;
                    var listSub = _expMestIds.Skip(startMedicine).Take(limit).ToList();

                    MOS.Filter.HisExpMestMedicineFilter medicineFilter = new MOS.Filter.HisExpMestMedicineFilter();
                    medicineFilter.EXP_MEST_IDs = listSub;
                    var ssdataMedicine = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    dataMedicines.AddRange(ssdataMedicine);
                    startMedicine += 100;
                    countServiceReqId_Medicine -= 100;
                }


                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    var _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in dataMedicines)
                    {
                        if (item.IS_NOT_PRES == 1)
                            continue;
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_input.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            var svr = rsServiceReq.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID);
                            ado.USE_TIME = svr != null ? svr.USE_TIME : null;
                        }
                        var mediType = _medicineTypes.FirstOrDefault(p => p.ID == item.TDL_MEDICINE_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServ.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMatyReqNewThread(object param)
        {
            try
            {
                LoadExpMestMatyReq((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestMatyReq(List<long> _expMestIds)
        {
            try
            {
                if (this._IsMaterial)
                    return;
                CommonParam param = new CommonParam();

                int startMatyReq = 0;
                int countServiceReqId_MatyReq = _expMestIds.Count;
                List<HIS_EXP_MEST_MATY_REQ> datas = new List<HIS_EXP_MEST_MATY_REQ>();
                while (countServiceReqId_MatyReq > 0)
                {
                    int limit = (countServiceReqId_MatyReq <= 100) ? countServiceReqId_MatyReq : 100;
                    var listSub = _expMestIds.Skip(startMatyReq).Take(limit).ToList();

                    MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new MOS.Filter.HisExpMestMatyReqFilter();
                    matyReqFilter.EXP_MEST_IDs = listSub;
                    var ssdata = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    datas.AddRange(ssdata);
                    startMatyReq += 100;
                    countServiceReqId_MatyReq -= 100;
                }


                if (datas != null && datas.Count > 0)
                {
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in datas)
                    {
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_input.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            var svr = rsServiceReq.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID);
                            ado.USE_TIME = svr != null ? svr.USE_TIME : null;
                        }
                        var mediType = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MATERIAL_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServ.Add(ado);
                    }
                }

                int startMaterial = 0;
                int countServiceReqId_Material = _expMestIds.Count;
                List<HIS_EXP_MEST_MATERIAL> dataMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                while (countServiceReqId_Material > 0)
                {
                    int limit = (countServiceReqId_Material <= 100) ? countServiceReqId_Material : 100;
                    var listSub = _expMestIds.Skip(startMaterial).Take(limit).ToList();

                    MOS.Filter.HisExpMestMaterialFilter materialReqFilter = new MOS.Filter.HisExpMestMaterialFilter();
                    materialReqFilter.EXP_MEST_IDs = _expMestIds;
                    var ssMaterial = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, materialReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    dataMaterials.AddRange(ssMaterial);
                    startMaterial += 100;
                    countServiceReqId_Material -= 100;
                }


                if (dataMaterials != null && dataMaterials.Count > 0)
                {
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in dataMaterials)
                    {
                        if (item.IS_NOT_PRES == 1)//#19933 Thuoc K Phai Bs Ke
                            continue;
                        HisSereServADONumOrder ado = new HisSereServADONumOrder();
                        ado.TDL_SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        ado.AMOUNT = item.AMOUNT;
                        var expM = _ExpMests_input.FirstOrDefault(p => p.ID == item.EXP_MEST_ID);
                        if (expM != null)
                        {
                            ado.TDL_INTRUCTION_DATE = expM.TDL_INTRUCTION_DATE ?? 0;
                            ado.TDL_SERVICE_REQ_CODE = expM.TDL_SERVICE_REQ_CODE;
                            ado.SERVICE_REQ_ID = expM.SERVICE_REQ_ID;
                            var svr = rsServiceReq.FirstOrDefault(o => o.ID == expM.SERVICE_REQ_ID);
                            ado.USE_TIME = svr != null ? svr.USE_TIME : null;
                        }
                        var mediType = _materialTypes.FirstOrDefault(p => p.ID == item.TDL_MATERIAL_TYPE_ID);
                        if (mediType != null)
                        {
                            ado.TDL_SERVICE_NAME = mediType.MATERIAL_TYPE_NAME;
                            ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                        }
                        ado.NUM_ORDER = item.NUM_ORDER;
                        rsSereServ.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //THUOC - VT ngoài kho
        List<HIS_SERVICE_REQ_METY> _ServiceReqMetys { get; set; }
        List<HIS_SERVICE_REQ_METY> _ServiceReqMetysTab2 { get; set; }
        List<HIS_SERVICE_REQ_MATY> _ServiceReqMatys { get; set; }
        private void LoadServiceReqMetyMatys(List<long> _serviceReqIds, List<HIS_SERVICE_REQ> _HisServiceReqs)
        {
            try
            {
                _ServiceReqMetys = new List<HIS_SERVICE_REQ_METY>();
                _ServiceReqMatys = new List<HIS_SERVICE_REQ_MATY>();
                CommonParam param = new CommonParam();

                int startReqMety = 0;
                int countServiceReqId_ReqMety = _serviceReqIds.Count;
                List<HIS_SERVICE_REQ_METY> dataMetys = new List<HIS_SERVICE_REQ_METY>();
                while (countServiceReqId_ReqMety > 0)
                {
                    int limit = (countServiceReqId_ReqMety <= 100) ? countServiceReqId_ReqMety : 100;
                    var listSub = _serviceReqIds.Skip(startReqMety).Take(limit).ToList();

                    MOS.Filter.HisServiceReqMetyFilter metyFIlter = new HisServiceReqMetyFilter();
                    metyFIlter.SERVICE_REQ_IDs = listSub;
                    var ssdataMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFIlter, param);

                    dataMetys.AddRange(ssdataMety);
                    startReqMety += 100;
                    countServiceReqId_ReqMety -= 100;
                }

                dataMetys = dataMetys.Where(o => o.IS_SUB_PRES != 1).ToList();
                foreach (var item in dataMetys)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 998;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    var mediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (mediType != null)
                    {
                        ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                    }
                    else if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME))
                    {
                        ado.TDL_SERVICE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_NAME = item.UNIT_NAME;
                    }
                    else
                        continue;
                    ado.NUM_ORDER = item.NUM_ORDER;
                    rsSereServ.Add(ado);
                }

                int startReqMaty = 0;
                int countServiceReqId_ReqMaty = _serviceReqIds.Count;
                List<HIS_SERVICE_REQ_MATY> dataMatys = new List<HIS_SERVICE_REQ_MATY>();
                while (countServiceReqId_ReqMaty > 0)
                {
                    int limit = (countServiceReqId_ReqMaty <= 100) ? countServiceReqId_ReqMaty : 100;
                    var listSub = _serviceReqIds.Skip(startReqMaty).Take(limit).ToList();

                    MOS.Filter.HisServiceReqMatyFilter matyFIlter = new HisServiceReqMatyFilter();
                    matyFIlter.SERVICE_REQ_IDs = listSub;
                    var ssMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFIlter, param);

                    dataMatys.AddRange(ssMaty);
                    startReqMaty += 100;
                    countServiceReqId_ReqMaty -= 100;
                }

                dataMatys = dataMatys.Where(o => o.IS_SUB_PRES != 1).ToList();
                foreach (var item in dataMatys)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 999;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    var mateType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (mateType != null)
                    {
                        ado.TDL_SERVICE_NAME = mateType.MATERIAL_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_ID = mateType.SERVICE_UNIT_ID;
                    }
                    ado.NUM_ORDER = item.NUM_ORDER;
                    rsSereServ.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadServiceReqMetyMatysTab2(List<long> _serviceReqIds, List<HIS_SERVICE_REQ> _HisServiceReqs)
        {
            try
            {
                _ServiceReqMetysTab2 = new List<HIS_SERVICE_REQ_METY>();
                List<HIS_SERVICE_REQ_METY> dataMetys = new List<HIS_SERVICE_REQ_METY>();
                int startMety = 0;
                int countServiceReqId_Mety = _serviceReqIds.Count;
                _ExpMests_inputTab2 = new List<HIS_EXP_MEST>();
                while (countServiceReqId_Mety > 0)
                {
                    CommonParam param = new CommonParam();
                    int limit = (countServiceReqId_Mety <= 100) ? countServiceReqId_Mety : 100;
                    var listSub = _serviceReqIds.Skip(startMety).Take(limit).ToList();

                    MOS.Filter.HisServiceReqMetyFilter metyFIlter = new HisServiceReqMetyFilter();
                    metyFIlter.SERVICE_REQ_IDs = listSub;
                    var ssMetys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, metyFIlter, param);
                    dataMetys.AddRange(ssMetys);
                    startMety += 100;
                    countServiceReqId_Mety -= 100;
                }

                dataMetys = dataMetys.Where(o => o.IS_SUB_PRES != 1).ToList();
                foreach (var item in dataMetys)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 998;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    var mediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                    if (mediType != null)
                    {
                        ado.TDL_SERVICE_NAME = mediType.MEDICINE_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_ID = mediType.SERVICE_UNIT_ID;
                    }
                    else if (!string.IsNullOrEmpty(item.MEDICINE_TYPE_NAME))
                    {
                        ado.TDL_SERVICE_NAME = item.MEDICINE_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_NAME = item.UNIT_NAME;
                    }
                    else
                        continue;
                    ado.NUM_ORDER = item.NUM_ORDER;
                    rsSereServ.Add(ado);
                }

                List<HIS_SERVICE_REQ_MATY> dataMatys = new List<HIS_SERVICE_REQ_MATY>();
                int startMaty = 0;
                int countServiceReqId_Maty = _serviceReqIds.Count;
                _ExpMests_inputTab2 = new List<HIS_EXP_MEST>();
                while (countServiceReqId_Maty > 0)
                {
                    CommonParam param = new CommonParam();
                    int limit = (countServiceReqId_Maty <= 100) ? countServiceReqId_Maty : 100;
                    var listSub = _serviceReqIds.Skip(startMaty).Take(limit).ToList();

                    MOS.Filter.HisServiceReqMatyFilter matyFIlter = new HisServiceReqMatyFilter();
                    matyFIlter.SERVICE_REQ_IDs = listSub;
                    var ssMatys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, matyFIlter, param);
                    dataMatys.AddRange(ssMatys);
                    startMaty += 100;
                    countServiceReqId_Maty -= 100;
                }


                dataMatys = dataMatys.Where(o => o.IS_SUB_PRES != 1).ToList();
                foreach (var item in dataMatys)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 999;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    var mateType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                    if (mateType != null)
                    {
                        ado.TDL_SERVICE_NAME = mateType.MATERIAL_TYPE_NAME;
                        ado.TDL_SERVICE_UNIT_ID = mateType.SERVICE_UNIT_ID;
                    }
                    ado.NUM_ORDER = item.NUM_ORDER;
                    rsSereServTab2.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadSereServRation(List<long> _serviceReqIds, List<HIS_SERVICE_REQ> _HisServiceReqs)
        {
            try
            {
                int startTracking = 0;
                int countServiceReqId = _serviceReqIds.Count;
                List<V_HIS_SERE_SERV_RATION> dataRations = new List<V_HIS_SERE_SERV_RATION>();
                while (countServiceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    int limit = (countServiceReqId <= 100) ? countServiceReqId : 100;
                    var listSub = _serviceReqIds.Skip(startTracking).Take(limit).ToList();
                    HisSereServRationViewFilter filterRation = new HisSereServRationViewFilter();
                    filterRation.SERVICE_REQ_IDs = listSub;
                    var ssRation = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filterRation, param);
                    dataRations.AddRange(ssRation);
                    startTracking += 100;
                    countServiceReqId -= 100;
                }

                foreach (var item in dataRations)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 1000;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    ado.TDL_SERVICE_NAME = item.SERVICE_NAME;
                    ado.TDL_SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    ado.RATION_TIME_NAME = item.RATION_TIME_NAME;
                    ado.NUM_ORDER = 999999;
                    rsSereServ.Add(ado);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadSereServRationTab2(List<long> _serviceReqIds, List<HIS_SERVICE_REQ> _HisServiceReqs)
        {
            try
            {
                int startTracking = 0;
                int countServiceReqId = _serviceReqIds.Count;
                List<V_HIS_SERE_SERV_RATION> dataRations = new List<V_HIS_SERE_SERV_RATION>();
                while (countServiceReqId > 0)
                {
                    CommonParam param = new CommonParam();
                    int limit = (countServiceReqId <= 100) ? countServiceReqId : 100;
                    var listSub = _serviceReqIds.Skip(startTracking).Take(limit).ToList();
                    HisSereServRationViewFilter filterRation = new HisSereServRationViewFilter();
                    filterRation.SERVICE_REQ_IDs = listSub;
                    var ssRation = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_RATION>>("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filterRation, param);
                    dataRations.AddRange(ssRation);
                    startTracking += 100;
                    countServiceReqId -= 100;
                }

                foreach (var item in dataRations)
                {
                    HisSereServADONumOrder ado = new HisSereServADONumOrder();
                    ado.TDL_SERVICE_TYPE_ID = 1000;
                    ado.AMOUNT = item.AMOUNT;
                    var serviceReq = _HisServiceReqs.FirstOrDefault(p => p.ID == item.SERVICE_REQ_ID);
                    if (serviceReq != null)
                    {
                        ado.TDL_INTRUCTION_DATE = serviceReq.INTRUCTION_DATE;
                        ado.TDL_SERVICE_REQ_CODE = serviceReq.SERVICE_REQ_CODE;
                        ado.SERVICE_REQ_ID = serviceReq.ID;
                        ado.USE_TIME = serviceReq.USE_TIME;
                    }
                    ado.TDL_SERVICE_NAME = item.SERVICE_NAME;
                    ado.TDL_SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                    ado.RATION_TIME_NAME = item.RATION_TIME_NAME;
                    ado.NUM_ORDER = 999999;
                    rsSereServTab2.Add(ado);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}
