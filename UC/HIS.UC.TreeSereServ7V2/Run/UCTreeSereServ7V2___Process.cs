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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Data;
using System.Collections;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.BackendData;
using System.Drawing.Drawing2D;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.TreeSereServ7V2.Run
{
    public partial class UCTreeSereServ7V2 : UserControl
    {
        public void LoadTreeListByFilter(bool indexFisrtDefault, List<SereServADO> sereServInputs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (sereServInputs != null)
                {
                    var sereServs = (from r in sereServInputs select new SereServADO(r)).ToList();
                    List<SereServADO> sereServResults = new List<SereServADO>();

                    if (cboFilterByDepartment.SelectedIndex == 0) //cua chi dinh dang chon
                    {
                        if (this.currentServiceReqList != null && this.currentServiceReqList.Count > 0)
                        {
                            var sereServParentServiceReq = sereServs.Where(o => this.currentServiceReqList.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList();
                            sereServResults = sereServParentServiceReq;
                        }
                    }
                    else if (cboFilterByDepartment.SelectedIndex == 1) //Tat ca
                    {
                        sereServResults = sereServs;
                    }
                    else if (cboFilterByDepartment.SelectedIndex == 2) // Tat ca khong gom noi tru
                    {
                        if (this.currentServiceReqList != null && this.currentServiceReqList.Count > 0)
                        {
                            var serviceReqExam = currentServiceReqList.Where(o => o.TDL_TREATMENT_TYPE_ID == 1).ToList();
                            if (serviceReqExam != null && serviceReqExam.Count() > 0)
                            {
                                var sereServParentServiceReq = sereServs.Where(o => serviceReqExam.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList();
                                sereServResults = sereServParentServiceReq;
                            }
                        }
                    }
                    else
                    {
                        sereServResults = sereServs;
                    }

                    #region GroupSERVICE_TYPE
                    var listRootByType = sereServResults.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                    foreach (var types in listRootByType)
                    {
                        SereServADO ssRootType = new SereServADO();
                        ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
                        ssRootType.TDL_SERVICE_CODE = types.First().SERVICE_TYPE_NAME;
                        ssRootType.NUM_ORDER = types.First().NUM_ORDER;
                        Inventec.Common.Logging.LogSystem.Debug("types.First().NUM_ORDER" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => types.First().NUM_ORDER), types.First().NUM_ORDER));
                        SereServADOs.Add(ssRootType);

                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                        int i = 0;
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_EXECUTE_DEPARTMENT_ID = listBySety.First().TDL_EXECUTE_DEPARTMENT_ID;
                            ssRootSety.SERVICE_REQ_ID = listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.TDL_SERVICE_REQ_TYPE_ID = listBySety.First().TDL_SERVICE_REQ_TYPE_ID;
                            ssRootSety.TDL_SERVICE_TYPE_ID = listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.NUM_ORDER = listBySety.First().NUM_ORDER;

                            Inventec.Common.Logging.LogSystem.Debug(" listBySety.First().NUM_ORDER __2___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBySety.First().NUM_ORDER), listBySety.First().NUM_ORDER));
                            ssRootSety.TDL_TREATMENT_CODE = listBySety.First().TDL_TREATMENT_CODE;
                            ssRootSety.SERVICE_REQ_STT_ID = listBySety.First().SERVICE_REQ_STT_ID;
                            ssRootSety.CREATOR = listBySety.First().CREATOR;
                            ssRootSety.TDL_SERVICE_REQ_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                            ssRootSety.IsServiceParent = true;
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;
                            V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == listBySety.First().TDL_EXECUTE_ROOM_ID) ?? new V_HIS_ROOM();
                            HIS_DEPARTMENT executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == listBySety.First().TDL_EXECUTE_DEPARTMENT_ID) ?? new HIS_DEPARTMENT();
                            ssRootSety.TDL_SERVICE_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                            ssRootSety.TDL_SERVICE_NAME = String.Format("- {0} - {1}", executeRoom.ROOM_NAME, executeDepartment.DEPARTMENT_NAME); //listBySety.First().SERVICE_TYPE_NAME;
                            var ss = sereServs.FirstOrDefault(o => o.ID == listBySety.First().ID);
                            if (ss != null)
                            {
                                ssRootSety.SAMPLE_TIME = ss.SAMPLE_TIME;
                                ssRootSety.RECEIVE_SAMPLE_TIME = ss.RECEIVE_SAMPLE_TIME;
                            }
                            SereServADOs.Add(ssRootSety);
                            int d = 0;
                            foreach (var item in listBySety)
                            {
                                d++;
                                item.NUM_ORDER = null;

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ssRootSety.NUM_ORDER___3___" + Inventec.Common.Logging.LogUtil.GetMemberName(() => ssRootSety.NUM_ORDER), ssRootSety.NUM_ORDER));
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                //Ghi chú
                                //
                                if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                                {
                                    item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_UNIT_NAME;//THuốc
                                }
                                else if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                                {
                                    item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_UNIT_NAME;//Vật tư
                                }
                                else
                                {
                                    item.NOTE_ADO = "";
                                }
                                SereServADOs.Add(item);
                            }
                        }
                    }
                    #endregion

                }
                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.TDL_SERVICE_CODE).ThenBy(o => o.TDL_SERVICE_NAME).ToList();

                records = new BindingList<SereServADO>(SereServADOs);

                // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => records.Where( o => o.NUM_ORDER != null)), records.Where( o => o.NUM_ORDER != null)));
                trvService.DataSource = records;
                trvService.ExpandAll();
                btnThuGon.Visible = true;
                btnChiTiet.Visible = false;
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadTreeListByFilterNotGroupServiceType(List<SereServADO> sereServInputs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (sereServInputs != null)
                {
                    var sereServs = (from r in sereServInputs select new SereServADO(r)).ToList();

                    List<SereServADO> sereServResults = new List<SereServADO>();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentServiceReqList), currentServiceReqList));
                    if (cboFilterByDepartment.SelectedIndex == 0) //cua chi dinh dang chon
                    {
                        if (this.currentServiceReqList != null && this.currentServiceReqList.Count > 0)
                        {
                            var sereServParentServiceReq = sereServs.Where(o => this.currentServiceReqList.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList();
                            sereServResults = sereServParentServiceReq;
                        }
                        else
                        {
                            sereServResults = new List<SereServADO>();
                        }
                    }
                    else if (cboFilterByDepartment.SelectedIndex == 1) //Tat ca
                    {
                        sereServResults = sereServs;
                    }
                    else if (cboFilterByDepartment.SelectedIndex == 2) // Tat ca khong gom noi tru
                    {
                        if (this.currentServiceReqList != null && this.currentServiceReqList.Count > 0)
                        {
                            var serviceReqExam = currentServiceReqList.Where(o => o.TDL_TREATMENT_TYPE_ID == 1).ToList();
                            if (serviceReqExam != null && serviceReqExam.Count() > 0)
                            {
                                var sereServParentServiceReq = sereServs.Where(o => serviceReqExam.Select(p => p.ID).Contains(o.SERVICE_REQ_ID ?? 0)).ToList();
                                sereServResults = sereServParentServiceReq;
                            }
                        }
                    }
                    else
                    {
                        sereServResults = sereServs;
                    }

                    #region GroupSERVICE_REQ_ID

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServResults), sereServResults));
                    var listRootSety = sereServResults.GroupBy(g => g.SERVICE_REQ_ID).ToList();
                    int count = 0;
                    foreach (var rootSety in listRootSety)
                    {
                        count++;
                        var listBySety = rootSety.ToList<SereServADO>();
                        SereServADO ssRootSety = new SereServADO();

                        ssRootSety.TDL_EXECUTE_DEPARTMENT_ID = listBySety.First().TDL_EXECUTE_DEPARTMENT_ID;
                        ssRootSety.SERVICE_REQ_ID = listBySety.First().SERVICE_REQ_ID;
                        ssRootSety.TDL_SERVICE_REQ_TYPE_ID = listBySety.First().TDL_SERVICE_REQ_TYPE_ID;
                        ssRootSety.TDL_SERVICE_TYPE_ID = listBySety.First().TDL_SERVICE_TYPE_ID;
                        ssRootSety.TDL_TREATMENT_CODE = listBySety.First().TDL_TREATMENT_CODE;
                        ssRootSety.SERVICE_REQ_STT_ID = listBySety.First().SERVICE_REQ_STT_ID;
                        ssRootSety.CREATOR = listBySety.First().CREATOR;
                        ssRootSety.NUM_ORDER = listBySety.First().NUM_ORDER;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listBySety.First().NUM_ORDER), listBySety.First().NUM_ORDER));
                        ssRootSety.TDL_SERVICE_REQ_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                        ssRootSety.CONCRETE_ID__IN_SETY = count + "_" + listBySety.First().SERVICE_REQ_ID;
                        ssRootSety.IsServiceParent = true;
                        V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == listBySety.First().TDL_EXECUTE_ROOM_ID) ?? new V_HIS_ROOM();
                        HIS_DEPARTMENT executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == listBySety.First().TDL_EXECUTE_DEPARTMENT_ID) ?? new HIS_DEPARTMENT();

                        ssRootSety.TDL_SERVICE_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                        ssRootSety.TDL_SERVICE_NAME = String.Format("- {0} - {1}", executeRoom.ROOM_NAME, executeDepartment.DEPARTMENT_NAME); //listBySety.First().SERVICE_TYPE_NAME;
                        SereServADOs.Add(ssRootSety);
                        int d = 0;
                        foreach (var item in listBySety)
                        {
                            d++;
                            item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                            item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                            item.NUM_ORDER = null;
                            item.IsLeaf = true;
                            //Ghi chú
                            //
                            if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                            {
                                item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_UNIT_NAME;//THuốc
                            }
                            else if (item.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_UNIT_NAME;//Vật tư
                            }
                            else
                            {
                                item.NOTE_ADO = "";
                            }
                            SereServADOs.Add(item);
                        }
                    }
                    #endregion


                }
                SereServADOs = SereServADOs.OrderBy(o => o.CONCRETE_ID__IN_SETY).ThenBy(p => p.TDL_SERVICE_CODE).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                records = new BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                btnThuGon.Visible = true;
                btnChiTiet.Visible = false;
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
