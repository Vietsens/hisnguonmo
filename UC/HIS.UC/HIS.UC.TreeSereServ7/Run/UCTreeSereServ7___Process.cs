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
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;

namespace HIS.UC.TreeSereServ7.Run
{
    public partial class UCTreeSereServ7 : UserControl
    {
        public void LoadTreeListByFilter(bool indexFisrtDefault, List<V_HIS_SERE_SERV_7> sereServInputs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (sereServInputs != null)
                {
                    List<HIS_SERVICE_REQ> listServiceReq = new List<HIS_SERVICE_REQ>();
                    CommonParam param_ = new CommonParam();
                    HisServiceReqFilter filter_ = new HisServiceReqFilter();
                    filter_.IDs = sereServInputs.Where(o => o.SERVICE_REQ_ID != null).Select(o => (long)o.SERVICE_REQ_ID).Distinct().ToList();
                    filter_.ColumnParams = new List<string>()
                    {
                        "ID",
                        "SAMPLE_TIME",
                        "RECEIVE_SAMPLE_TIME",
                    };

                    var data_ = new BackendAdapter(param_).GetRO<List<HIS_SERVICE_REQ>>("api/HisServiceReq/GetDynamic", ApiConsumers.MosConsumer, filter_, param_);
                    if (data_ != null && data_.Data.Count() > 0)
                    {
                        listServiceReq = data_.Data;
                    }
                    var sereServs = (from r in sereServInputs select new SereServADO(r, listServiceReq)).ToList();
                    List<SereServADO> sereServResults = new List<SereServADO>();

                    if (this._DepartmentInput > 0)
                    {
                        List<SereServADO> sereServOtherDepartments = sereServs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == this._DepartmentInput).ToList();

                        //if (indexFisrtDefault)
                        //    cboFilterByDepartment.SelectedIndex = 0;

                        if (cboFilterByDepartment.SelectedIndex == 0) //Theo khoa
                        {
                            sereServResults = sereServOtherDepartments;
                        }
                        else if (cboFilterByDepartment.SelectedIndex == 1) //Tat ca
                        {
                            sereServResults = sereServs;
                        }
                    }
                    else
                    {
                        sereServResults = sereServs;
                    }

                    var listRootSety = sereServResults.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                    foreach (var rootSety in listRootSety)
                    {
                        var listBySety = rootSety.ToList<SereServADO>();
                        SereServADO ssRootSety = new SereServADO();
                        ssRootSety.CONCRETE_ID__IN_SETY = listBySety.First().TDL_SERVICE_TYPE_ID + "";
                        ssRootSety.TDL_SERVICE_CODE = listBySety.First().SERVICE_TYPE_NAME;
                        SereServADOs.Add(ssRootSety);
                        int d = 0;
                        foreach (var item in listBySety)
                        {
                            d++;
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
                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.TDL_SERVICE_CODE).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                records = new BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadTreeListByFilter(bool indexFisrtDefault, List<MOS.SDO.DHisSereServ2> sereServInputs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (sereServInputs != null)
                {
                    var sereServs = (from r in sereServInputs select new SereServADO(r)).ToList();

                    List<SereServADO> sereServResults = new List<SereServADO>();

                    if (this._DepartmentInput > 0)
                    {
                        List<SereServADO> sereServOtherDepartments = sereServs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == this._DepartmentInput).ToList();

                        //if (indexFisrtDefault)
                        //    cboFilterByDepartment.SelectedIndex = 0;

                        if (cboFilterByDepartment.SelectedIndex == 0) //Theo khoa
                        {
                            sereServResults = sereServOtherDepartments;
                        }
                        else if (cboFilterByDepartment.SelectedIndex == 1) //Tat ca
                        {
                            sereServResults = sereServs;
                        }
                    }
                    else
                    {
                        sereServResults = sereServs;
                    }

                    var listRootByType = sereServResults.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                    foreach (var types in listRootByType)
                    {
                        SereServADO ssRootType = new SereServADO();
                        ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
                        ssRootType.TDL_SERVICE_CODE = types.First().SERVICE_TYPE_NAME;
                        SereServADOs.Add(ssRootType);

                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();

                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_EXECUTE_DEPARTMENT_ID = listBySety.First().TDL_EXECUTE_DEPARTMENT_ID;
                            ssRootSety.SERVICE_REQ_ID = listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.TDL_SERVICE_REQ_TYPE_ID = listBySety.First().TDL_SERVICE_REQ_TYPE_ID;
                            ssRootSety.TDL_SERVICE_TYPE_ID = listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.TDL_TREATMENT_CODE = listBySety.First().TDL_TREATMENT_CODE;
                            ssRootSety.SERVICE_REQ_STT_ID = listBySety.First().SERVICE_REQ_STT_ID;
                            ssRootSety.CREATOR = listBySety.First().CREATOR;
                            ssRootSety.TDL_SERVICE_REQ_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;

                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;

                            V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == listBySety.First().TDL_REQUEST_ROOM_ID) ?? new V_HIS_ROOM();
                            HIS_DEPARTMENT executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == listBySety.First().TDL_REQUEST_DEPARTMENT_ID) ?? new HIS_DEPARTMENT();

                            ssRootSety.TDL_SERVICE_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                            ssRootSety.TDL_SERVICE_NAME = String.Format("- {0} - {1}", executeRoom.ROOM_NAME, executeDepartment.DEPARTMENT_NAME);
                            SereServADOs.Add(ssRootSety);
                            int d = 0;
                            foreach (var item in listBySety)
                            {
                                d++;
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

                }
                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.TDL_SERVICE_CODE).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                records = new BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadTreeListByFilter(bool indexFisrtDefault, List<SereServADO> sereServInputs)
        {
            try
            {
                SereServADOs = new List<SereServADO>();
                if (sereServInputs != null)
                {
                    List<HIS_SERVICE_REQ> listServiceReq = new List<HIS_SERVICE_REQ>();
                    CommonParam param_ = new CommonParam();
                    HisServiceReqFilter filter_ = new HisServiceReqFilter();
                    filter_.IDs = sereServInputs.Where(o => o.SERVICE_REQ_ID != null).Select(o => (long)o.SERVICE_REQ_ID).Distinct().ToList();
                    filter_.ColumnParams = new List<string>()
                    {
                        "ID",
                        "SAMPLE_TIME",
                        "RECEIVE_SAMPLE_TIME",
                    };

                    var data_ = new BackendAdapter(param_).GetRO<List<HIS_SERVICE_REQ>>("api/HisServiceReq/GetDynamic", ApiConsumers.MosConsumer, filter_, param_);
                    if (data_ != null && data_.Data.Count() > 0)
                    {
                        listServiceReq = data_.Data;
                    }
                    var sereServs = (from r in sereServInputs select new SereServADO(r, listServiceReq)).ToList();

                    List<SereServADO> sereServResults = new List<SereServADO>();

                    if (this._DepartmentInput > 0)
                    {
                        List<SereServADO> sereServOtherDepartments = sereServs.Where(o => o.TDL_REQUEST_DEPARTMENT_ID == this._DepartmentInput).ToList();

                        //if (indexFisrtDefault)
                        //    cboFilterByDepartment.SelectedIndex = 0;

                        if (cboFilterByDepartment.SelectedIndex == 0) //Theo khoa
                        {
                            sereServResults = sereServOtherDepartments;
                        }
                        else if (cboFilterByDepartment.SelectedIndex == 1) //Tat ca
                        {
                            sereServResults = sereServs;
                        }
                    }
                    else
                    {
                        sereServResults = sereServs;
                    }

                    var listRootByType = sereServResults.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();

                    foreach (var types in listRootByType)
                    {
                        SereServADO ssRootType = new SereServADO();
                        ssRootType.CONCRETE_ID__IN_SETY = types.First().TDL_SERVICE_TYPE_ID + "";
                        ssRootType.TDL_SERVICE_CODE = types.First().SERVICE_TYPE_NAME;
                        SereServADOs.Add(ssRootType);

                        var listRootSety = types.GroupBy(g => g.SERVICE_REQ_ID).ToList();

                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_EXECUTE_DEPARTMENT_ID = listBySety.First().TDL_EXECUTE_DEPARTMENT_ID;
                            ssRootSety.SERVICE_REQ_ID = listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.TDL_SERVICE_REQ_TYPE_ID = listBySety.First().TDL_SERVICE_REQ_TYPE_ID;
                            ssRootSety.TDL_SERVICE_TYPE_ID = listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.TDL_TREATMENT_CODE = listBySety.First().TDL_TREATMENT_CODE;
                            ssRootSety.SERVICE_REQ_STT_ID = listBySety.First().SERVICE_REQ_STT_ID;
                            ssRootSety.CREATOR = listBySety.First().CREATOR;
                            ssRootSety.TDL_SERVICE_REQ_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;

                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY + "_" + listBySety.First().SERVICE_REQ_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootType.CONCRETE_ID__IN_SETY;

                            V_HIS_ROOM executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == listBySety.First().TDL_REQUEST_ROOM_ID) ?? new V_HIS_ROOM();
                            HIS_DEPARTMENT executeDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == listBySety.First().TDL_REQUEST_DEPARTMENT_ID) ?? new HIS_DEPARTMENT();

                            ssRootSety.TDL_SERVICE_CODE = listBySety.First().TDL_SERVICE_REQ_CODE;
                            ssRootSety.TDL_SERVICE_NAME = String.Format("- {0} - {1}", executeRoom.ROOM_NAME, executeDepartment.DEPARTMENT_NAME); //listBySety.First().SERVICE_TYPE_NAME;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssRootSety), ssRootSety));
                            SereServADOs.Add(ssRootSety);
                            int d = 0;
                            foreach (var item in listBySety)
                            {
                                d++;
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

                }
                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.TDL_SERVICE_CODE).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                records = new BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
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
