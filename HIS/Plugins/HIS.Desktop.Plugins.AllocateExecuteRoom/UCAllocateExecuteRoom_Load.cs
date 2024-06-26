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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.AllocateExecuteRoom.Base;
using HIS.Desktop.Plugins.AllocateExecuteRoom.ADO;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.AllocateExecuteRoom
{
    public partial class UCAllocateExecuteRoom : UserControlBase
    {
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkCungKhoa.Name)
                        {
                            chkCungKhoa.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkCungKhuVuc.Name)
                        {
                            chkCungKhuVuc.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkInSTT.Name)
                        {
                            chkInSTT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkInPhieuChiDinh.Name)
                        {
                            chkInPhieuChiDinh.Checked = item.VALUE == "1";
                        }
                        
                    }
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(btnCodeFind_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(btnCodeFind_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                DXMenuItem itemRangeDate = new DXMenuItem(typeCodeFind_RangeDate, new EventHandler(btnCodeFind_Click));
                itemRangeDate.Tag = "RangeDate";
                menu.Items.Add(itemRangeDate);

                btnCodeFind.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadActionButtonRefesh(bool isBtnRefesh)
        {
            try
            {
                if (isBtnRefesh)
                {
                    btnRefesh.Image = imageList1.Images[1];
                    btnRefesh.Tag = EnumUtil.REFESH_ENUM.ON;
                }
                else
                {
                    btnRefesh.Image = imageList1.Images[0];
                    btnRefesh.Tag = EnumUtil.REFESH_ENUM.OFF;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultData()
        {
            try
            {
                btnPrint.Enabled = false;
                btnCodeFind.Text = typeCodeFind__KeyWork_InDate;
                FormatDtIntructionDate();
                dtIntructionDate.DateTime = DateTime.Now;
                dtIntructionDateTo.DateTime = DateTime.Now;
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatDtIntructionDate()
        {
            try
            {

                dtIntructionDate.DateTime = DateTime.Now;
                dtIntructionDateTo.DateTime = DateTime.Now;
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate )
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "dd/MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "dd/MM/yyyy";
                    dtIntructionDate.Properties.ShowClear = false;
 //                       this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate;
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind__InMonth)
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtIntructionDate.Properties.EditMask = "MM/yyyy";
                    dtIntructionDate.Properties.Mask.EditMask = "MM/yyyy";
                    dtIntructionDate.Properties.ShowClear = false;
                }
                else if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate)
                {
                    dtIntructionDate.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtIntructionDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtIntructionDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDate.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtIntructionDate.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtIntructionDate.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtIntructionDate.Properties.ShowClear = false;

                    dtIntructionDateTo.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtIntructionDateTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDateTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtIntructionDateTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtIntructionDateTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
                    dtIntructionDateTo.Properties.EditMask = "dd/MM/yyyy HH:mm";
                    dtIntructionDateTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
                    dtIntructionDateTo.Properties.ShowClear = false;
                }

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_RangeDate)
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciNext.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPreview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dtIntructionDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartDay() ?? 0));
                    dtIntructionDateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                }
                else
                {
                    lciIntructionDateTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciNext.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPreview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridControl()
        {
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            FillDataToGridServiceReq(new CommonParam(0, (int)numPageSize));
            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridServiceReq, param, numPageSize, gridControl1);
        }

        internal void FillDataToGridServiceReq(object param)
        {
            try
            {
               
                WaitingManager.Show();
                int start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                numPageSize = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<L_HIS_SERVICE_REQ_1>> apiResult = new ApiResultObject<List<L_HIS_SERVICE_REQ_1>>();
                MOS.Filter.HisServiceReqLView1Filter hisServiceReqFilter = new HisServiceReqLView1Filter();

                if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    hisServiceReqFilter.SERVICE_REQ_CODE__EXACT = code;
                }

                hisServiceReqFilter.KEYWORD__SERVICE_REQ_CODE__TREATMENT_CODE__PATIENT_NAME__PATIENT_CODE = txtSearchKey.Text;

                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                   && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                    && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.VIR_INTRUCTION_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMM") + "00000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind_RangeDate
                    && dtIntructionDate.EditValue != null && dtIntructionDate.DateTime != DateTime.MinValue
                    && dtIntructionDateTo.EditValue != null && dtIntructionDateTo.DateTime != DateTime.MinValue)
                {
                    hisServiceReqFilter.INTRUCTION_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDate.EditValue).ToString("yyyyMMddHHmm00") );

                    hisServiceReqFilter.INTRUCTION_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtIntructionDateTo.EditValue).ToString("yyyyMMddHHmm59"));
                }

                hisServiceReqFilter.EXECUTE_ROOM_ID = this.roomId;
             
                     hisServiceReqFilter.SERVICE_REQ_STT_IDs= new List<long>();
                     hisServiceReqFilter.SERVICE_REQ_STT_IDs.AddRange(new List<long>
                    {
                    
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL,
                    });

                hisServiceReqFilter.ORDER_FIELD = "INTRUCTION_DATE";
                hisServiceReqFilter.ORDER_FIELD1 = "SERVICE_REQ_STT_ID";
                hisServiceReqFilter.ORDER_FIELD2 = "PRIORITY";
                hisServiceReqFilter.ORDER_FIELD3 = "NUM_ORDER";

                hisServiceReqFilter.ORDER_DIRECTION = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION1 = "ASC";
                hisServiceReqFilter.ORDER_DIRECTION2 = "DESC";
                hisServiceReqFilter.ORDER_DIRECTION3 = "ASC";



                Inventec.Common.Logging.LogSystem.Debug("HIS.Desktop.Plugins.AllocateExecuteRoom FillDataToGridServiceReq hisServiceReqFilter" + Inventec.Common.Logging.LogUtil.TraceData("", hisServiceReqFilter));
                apiResult = new BackendAdapter(paramCommon)
                    .GetRO<List<L_HIS_SERVICE_REQ_1>>("api/HisServiceReq/GetLView1", ApiConsumers.MosConsumer, hisServiceReqFilter, paramCommon);
                bool IsAutoFocus = false;
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                gridControl1.BeginUpdate();
                if (apiResult != null && apiResult.Data != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult.Data), apiResult.Data));
                    serviceReqs1 = (from r in apiResult.Data select new ServiceReq1ADO(r)).ToList();
                    rowCount = (serviceReqs1 == null ? 0 : serviceReqs1.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    if (rowCount > 0)
                    {

                        if (serviceReqs1.Count == 1)
                        {
                            foreach (var item in serviceReqs1)
                            
                            {
                                item.isCheckRow = true;
                            }
                            IsAutoFocus = true;
                            
                        }

                    }
                    else
                    {
                        serviceReqs1 = null;
                    }
                }
                else
                {
                    serviceReqs1 = null;
                }
                gridControl1.DataSource = serviceReqs1;
                if (serviceReqs1 != null)
                {
                  
                    gridView1.FocusedRowHandle = 0;
                    gridView1.FocusedColumn = gridColumn2;
                }

                if (IsAutoFocus)
                {
                   
                    LoadDataToPanelRight((ServiceReq1ADO)gridView1.GetRow(0));
                    
                }
                gridControl1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost((CommonParam)param);
                #endregion

                gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;

                maxTimeReload = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>("HIS.EXECUTE_ROOM.MAX_TIME_AUTO_RELOAD"));
                lblAutoReload.Text = "";
                if ((EnumUtil.REFESH_ENUM)btnRefesh.Tag == EnumUtil.REFESH_ENUM.OFF 
                    && maxTimeReload > 0
                    )
                {
                    timeCount = 0;
                    timerAutoReload.Start();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToPanelRight(ServiceReq1ADO serviceReq)
        {
            try
            {
                WaitingManager.Show();
                LoadRoom(serviceReq);
                WaitingManager.Hide();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRoom(ServiceReq1ADO serviceReq)
        {
            try
            {
               
                InitRestoreLayoutGridViewFromXml(gridView2);
                gridControl2.DataSource = null;
                roomCounter = new List<RoomADO>();
                if (serviceReq != null)
                {
               
                    var serviceReqAlls = this.gridView1.DataSource as List<ServiceReq1ADO>;
                    gridControl1.BeginUpdate();
                    if (serviceReqAlls != null)
                    {
                        foreach (var ro in serviceReqAlls)
                        {
                            ro.isCheckRow = false;
                            if (ro.ID == serviceReq.ID)
                            {
                                ro.isCheckRow = !ro.isCheckRow;
                            }
                        }

                        var ServiceInList = this.serviceReqs1.FirstOrDefault(o => o.ID == serviceReq.ID);
                        if (ServiceInList != null)
                        {
                            ServiceInList.isCheckRow = serviceReq.isCheckRow;
                        }

                        this.gridControl1.DataSource = serviceReqAlls;
                    }
                    this.gridControl1.EndUpdate();
                 
                    
                    CommonParam param = new CommonParam();
                    List<long> lstServiceIds = new List<long>();
                    string strIds=serviceReq.TDL_SERVICE_IDS;
                    if(!string.IsNullOrEmpty(strIds)){
                        if (strIds.Contains(";"))
                        {
                            string[] arrIds = strIds.Split(';');
                            for (int i = 0; i < arrIds.Length; i++)
                            {
                                lstServiceIds.Add(Int64.Parse(arrIds[i]));
                            }
                        }
                        else
                        {
                            lstServiceIds.Add(Int64.Parse(strIds));
                        }
                    }
                    HisRoomCounterLViewFilter filter = new HisRoomCounterLViewFilter();
                    filter.IS_ACTIVE = 1;
                    filter.IS_PAUSE_ENCLITIC =  false;
                    filter.ID__NOT_EQUAL = roomId;
                    filter.SERVICE_IDs = lstServiceIds;

                    HisRoomViewFilter roomfilter = new HisRoomViewFilter();
                    roomfilter.IS_ACTIVE = 1;
                    roomfilter.ID = this.roomId;
                    var roomArea =  new BackendAdapter(param).Get<List<V_HIS_ROOM>>("api/HisRoom/GetView", ApiConsumers.MosConsumer, roomfilter, param);

                    if (roomArea != null && roomArea.Count > 0)
                    {
                        if (chkCungKhoa.Checked) filter.DEPARTMENT_ID = roomArea.FirstOrDefault().DEPARTMENT_ID;
                        if (chkCungKhuVuc.Checked) filter.AREA_ID = roomArea.FirstOrDefault().AREA_ID;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("api/HisRoom/GetCounterLView___"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    var dataRoom = new BackendAdapter(param)
                    .Get<List<L_HIS_ROOM_COUNTER>>("api/HisRoom/GetCounterLView", ApiConsumers.MosConsumer, filter, param);
                    if (dataRoom != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataRoom), dataRoom));
                        roomCounter = (from r in dataRoom select new RoomADO(r)).ToList();
                        string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtSearchKeyRoom.Text.ToLower().Trim());
                        var query = this.roomCounter.AsQueryable();
                        query = query.Where(o => 
                                Inventec.Common.String.Convert.UnSignVNese(o.EXECUTE_ROOM_NAME.ToLower()).Contains(keyword)
                                || Inventec.Common.String.Convert.UnSignVNese(o.EXECUTE_ROOM_CODE.ToLower()).Contains(keyword)
                                
                                );
                   
                        var userRooms = query
                            .OrderBy(o => o.DEPARTMENT_NAME).Distinct().ToList();

                        bool isAutoFocus = false;
                        if (userRooms.Count == 1)
                        {
                            foreach (var ur in userRooms)
                            {
                                ur.IsChecked = true;
                            }
                            isAutoFocus = true;
                        }
                        this.gridControl2.Focus();
                        this.gridView2.Focus();
                        this.gridControl2.DataSource = userRooms;
                        
                        gridView2.FocusedColumn = gridColumn9;
                        this.gridView2.FocusedRowHandle = 0;
                        if (isAutoFocus)
                        {
                            btnSave.Focus();
                        }
                        
                    }                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

    }
}
