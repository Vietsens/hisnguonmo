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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PayClinicalResult.Base;
using HIS.Desktop.Plugins.PayClinicalResult.Resources;
using HIS.Desktop.Utility;
using HIS.UC.TreeSereServ7;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.ExecuteRoom;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        internal long roomId;
        internal long roomTypeId;

        internal HIS_SERVICE_REQ currentHisServiceReq { get; set; }
        internal HIS_SERVICE_REQ serviceReqRightClick { get; set; }
        internal List<HIS_SERVICE_REQ> serviceReqs { get; set; }
        internal List<V_HIS_SERE_SERV_7> sereServ7s { get; set; }
        internal List<DHisSereServ2> DSereServ2s { get; set; }
        internal List<HIS_DEPARTMENT> departments { get; set; }
        internal List<V_HIS_ROOM> rooms { get; set; }
        HIS_DEPARTMENT currentDepartment = null;
        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;
        int lastRowHandle = -1;
        bool needHandleOnClick = true;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        List<V_HIS_SERE_SERV_6> sereServ6s { get; set; }

        TreeSereServ7Processor ssTreeProcessor;
        UserControl ucTreeSereServ7;
        ExecuteRoomPopupMenuProcessor executeRoomPopupMenuProcessor;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;

        List<HIS_SERVICE_REQ> ServiceReqCurrentTreatment { get; set; }
        List<HIS_SERE_SERV> SereServCurrentTreatment { get; set; }

        long timeCount = 0;
        long maxTimeReload = 0;

        #region IsClick
        bool isEventPopupMenuShowing = false;
        #endregion


        public UCExecuteRoom()
        {
            InitializeComponent();
        }

        public UCExecuteRoom(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UCExecuteRoom_Load(object sender, EventArgs e)
        {
            try
            {
                GetRequestRoom(this.roomId);
                LoadActionButtonRefesh(true);
                InitLanguage();
                InitTreeSereServ();
                InitComboBoxEditStatus();
                LoadDefaultData();
                LoadDepartmentAndRoom();
                FillDataToGridControl();
                gridControlServiceReq.ToolTipController = toolTipController1;
                InitEnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                    this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == result.DEPARTMENT_ID);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void LoadDefaultData()
        {
            try
            {
                dtCreatefrom.Properties.VistaDisplayMode = DefaultBoolean.True;
                dtCreatefrom.Properties.VistaEditTime = DefaultBoolean.True;
                dtCreateTo.Properties.VistaDisplayMode = DefaultBoolean.True;
                dtCreateTo.Properties.VistaEditTime = DefaultBoolean.True;
                dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                cboFind.SelectedIndex = 0;
                cboTreatmentType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                btnFind.Enabled = false;
                currentHisServiceReq = null;
                LoadPatientFromServiceReq(null);
                LoadSereServServiceReq(null);
                LoadTreeListSereServChild(null);
                FillDataToGridControl();
                InitEnableControl();
                btnFind.Enabled = true;
                LoadServiceReqCount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_REQ dataRow = (HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "TRANGTHAI_IMG")
                    {
                        //Chua xu ly: mau trang
                        //dang xu ly: mau vang
                        //Da ket thuc: mau den

                        long statusId = dataRow.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "BUSY_COUNT_DISPLAY")
                    {
                        if (dataRow.IS_WAIT_CHILD.HasValue && dataRow.IS_WAIT_CHILD.Value == 1)
                        {
                            if (dataRow.IS_WAIT_CHILD.Value == 1)
                            {
                                e.Value = imageListIcon.Images[6];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[7];
                            }
                        }
                    }
                    else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    {
                        decimal priority = (dataRow.PRIORITY ?? 0);
                        if ((priority == 1))
                        {
                            e.Value = imageListPriority.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "REQUEST_DATE_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        e.Value = dataRow.TDL_PATIENT_DOB > 0 ? dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4) : null;
                    }
                    else if (e.Column.FieldName == "INSTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "REQUEST_DEPARTMENT_DISPLAY")
                    {
                        HIS_DEPARTMENT department = departments.FirstOrDefault(o => o.ID == dataRow.REQUEST_DEPARTMENT_ID);
                        e.Value = department != null ? department.DEPARTMENT_NAME : null;
                    }
                    else if (e.Column.FieldName == "REQUEST_ROOM_DISPLAY")
                    {
                        V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == dataRow.REQUEST_ROOM_ID);
                        e.Value = room != null ? room.ROOM_NAME : null;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewServiceReq_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.Caption == "Selection")
                    return;

                if (e.Column.FieldName == "CallPatient")
                {
                    this.currentHisServiceReq = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ)gridViewServiceReq.GetFocusedRow();
                    if (this.currentHisServiceReq != null)
                    {
                        List<HIS_SERVICE_REQ> serviceReqTemps = new List<HIS_SERVICE_REQ>();
                        int[] selectRows = gridViewServiceReq.GetSelectedRows();
                        if (selectRows != null && selectRows.Count() > 0)
                        {
                            for (int i = 0; i < selectRows.Count(); i++)
                            {
                                serviceReqTemps.Add((HIS_SERVICE_REQ)gridViewServiceReq.GetRow(selectRows[i]));
                            }
                        }

                        if (serviceReqTemps != null && serviceReqTemps.Count > 0)
                        {
                            if (!serviceReqTemps.Contains(this.currentHisServiceReq))
                                return;
                            CreateThreadCallPatientCPA(serviceReqTemps);
                            gridViewServiceReq.FocusedColumn = gridViewServiceReq.Columns[1];
                        }
                        else
                        {
                            UpdateDicCallPatient(this.currentHisServiceReq);
                            LoadCallPatientByThread(this.currentHisServiceReq);
                        }
                    }

                    gridViewServiceReq.FocusedColumn = gridViewServiceReq.Columns[1];
                }
                else
                {
                    timerDoubleClick.Start();
                    if (e.Clicks == 2)
                    {
                        gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                        gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                        currentHisServiceReq = (HIS_SERVICE_REQ)gridViewServiceReq.GetFocusedRow();
                        needHandleOnClick = false;
                        btnExecuteByDoubleClick(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentHisServiceReq != null)
                {
                    if (currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        CancelFinish(currentHisServiceReq);
                        return;
                    }
                    LoadModuleExecuteService(currentHisServiceReq);
                    InitEnableControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExecuteByDoubleClick(object sender, EventArgs e)
        {
            try
            {
                LoadModuleExecuteService(currentHisServiceReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadServiceReq(object data)
        {
            try
            {
                if (data != null && typeof(HIS_SERVICE_REQ) == data.GetType())
                {
                    HIS_SERVICE_REQ serviceReq = data as HIS_SERVICE_REQ;
                    HIS_SERVICE_REQ serviceReqTemp = null;
                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == serviceReq.ID)
                        {
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(item, serviceReq);
                            serviceReqTemp = item;
                        }
                    }
                    if (cboFind.SelectedIndex != 4 && serviceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        serviceReqs.Remove(serviceReqTemp);
                    }
                    gridControlServiceReq.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        private void btnAssignBlood_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisAssignBlood").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SurgServiceReqExecute");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignBloodADO assignBlood = new AssignBloodADO(this.currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignBlood);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtSearchKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlServiceReq)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long serviceReqSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                                if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.CXL", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.DXL", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.KT", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                }

                            }
                            if (info.Column.FieldName == "PRIORIRY_DISPLAY")
                            {
                                text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.PRIORIRY", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                            }
                            if (info.Column.FieldName == "BUSY_COUNT_DISPLAY")
                            {
                                string busyCount = (view.GetRowCellValue(lastRowHandle, "IS_WAIT_CHILD") ?? "").ToString();
                                if (busyCount == "1")
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.UnFinishCLS", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                                else if (busyCount == "0")
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoom.ToolTipControl.FinishCLS", ResourceLangManager.LanguageUCExecuteRoom, LanguageManager.GetCulture());
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                    AssignServiceADO assignServiceADO = new ADO.AssignServiceADO(currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignServiceADO);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnAssignPrescription_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescription").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescription");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignPrescriptionADO assignPrescription = new ADO.AssignPrescriptionADO(currentHisServiceReq.TREATMENT_ID, intructionTime, this.currentHisServiceReq.ID);
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(assignPrescription);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBordereau_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBedRoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomIn");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "SAR.Desktop.Plugins.SarPrintList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = SAR.Desktop.Plugins.SarPrintList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    SarPrintADO sarPrint = new SarPrintADO();
                    sarPrint.JSON_PRINT_ID = currentHisServiceReq.JSON_PRINT_ID;
                    //sarPrint.JsonPrintResult = JsonPrintResult;
                    listArgs.Add(sarPrint);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void JsonPrintResult(object data)
        {
            throw new NotImplementedException();
        }

        private void btnRoomTran_Click(object sender, EventArgs e)
        {
            try
            {
                //Kiem tra xem dich vu da bat dau hay chua

                if (currentHisServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show(String.Format(ResourceMessage.BanCoMuonHuyBatDauKhong), String.Format(ResourceMessage.XacNhanHuyBatDau), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (myResult == DialogResult.Yes)
                    {
                        if (!this.UnStartEvent())
                            return;
                    }
                    else
                        return;
                }


                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>();
                    V_HIS_SERVICE_REQ serviceReq = AutoMapper.Mapper.Map<HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>(this.currentHisServiceReq);
                    listArgs.Add(serviceReq);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSummaryInforTreatmentRecords_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Transaction").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Transaction");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAggrHospitalFees_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrHospitalFees");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnTreatmentHistory_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = this.currentHisServiceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.patientId = treatment.PATIENT_ID;
                        treatmentHistory.patient_code = treatment.TDL_PATIENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnTreatmentHistory2_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = this.currentHisServiceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.treatmentId = treatment.ID;
                        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositReq_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.RequestDeposit");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridHitInfo hi = e.HitInfo;
            if (hi.InRowCell)
            {
                int rowHandle = gridViewServiceReq.GetVisibleRowHandle(hi.RowHandle);
                serviceReqRightClick = (HIS_SERVICE_REQ)gridViewServiceReq.GetRow(rowHandle);
                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                if (barManager1 == null)
                {
                    barManager1 = new BarManager();
                    barManager1.Form = this;
                }

                executeRoomPopupMenuProcessor = new ExecuteRoomPopupMenuProcessor(serviceReqRightClick, ExecuteRoomMouseRight_Click, barManager1, roomId);
                executeRoomPopupMenuProcessor.InitMenu();
                isEventPopupMenuShowing = true;
            }
        }

        private void btnUnStart_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, currentHisServiceReq.ID, param);
                if (serviceReq != null && serviceReq.ID > 0)
                {
                    LoadServiceReqCount();
                    success = true;
                    btnUnStart.Enabled = false;
                    //Reload data

                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                            currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                        }
                    }

                    gridControlServiceReq.RefreshDataSource();
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private bool UnStartEvent()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var serviceReq = new BackendAdapter(param)
                    .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, currentHisServiceReq.ID, param);
                if (serviceReq != null && serviceReq.ID > 0)
                {
                    result = true;
                    success = true;
                    btnUnStart.Enabled = false;
                    //Reload data

                    foreach (var item in serviceReqs)
                    {
                        if (item.ID == currentHisServiceReq.ID)
                        {
                            item.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                            currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                        }
                    }

                    gridControlServiceReq.RefreshDataSource();
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = currentHisServiceReq.TREATMENT_ID;
                    listArgs.Add(treatment);
                    listArgs.Add(currentHisServiceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                V_HIS_SERE_SERV_6 sereServ = (V_HIS_SERE_SERV_6)gridViewSereServServiceReq.GetRow(e.RowHandle);
                if (sereServ != null && sereServ.IS_NO_EXECUTE != null)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewServiceReq_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                HIS_SERVICE_REQ serviceReq = (HIS_SERVICE_REQ)gridViewServiceReq.GetRow(e.RowHandle);
                if (serviceReq != null)
                {
                    if (serviceReq.PRIORITY != null && serviceReq.PRIORITY == 1)
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    if (!String.IsNullOrEmpty(serviceReq.TDL_HEIN_CARD_NUMBER))
                        e.Appearance.ForeColor = System.Drawing.Color.Blue;
                    //if (serviceReq.IS_CHRONIC.HasValue && serviceReq.IS_CHRONIC == 1)
                    //    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceReqCodeSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGateNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtStepNumber.Focus();
                    txtStepNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCallPatient.Enabled)
                    return;
                if (String.IsNullOrEmpty(txtGateNumber.Text) || String.IsNullOrEmpty(txtStepNumber.Text))
                    return;
                string configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                if (configKeyCallPatientCPA == "1")
                {
                    if (this.clienttManager == null)
                        this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                    this.clienttManager.RecallNumOrder(int.Parse(txtGateNumber.Text), int.Parse(txtStepNumber.Text));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerDoubleClick.Stop();
            if (needHandleOnClick)
            {
                if (!isEventPopupMenuShowing)
                {
                    sereServ7s = null;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                    currentHisServiceReq = (HIS_SERVICE_REQ)gridViewServiceReq.GetFocusedRow();
                    LoadDataToPanelRight(currentHisServiceReq);
                    InitEnableControl();
                    SetTextButtonExecute(currentHisServiceReq);
                }
                isEventPopupMenuShowing = false;
            }
            needHandleOnClick = true;
        }

        private void cboFind_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                V_HIS_SERE_SERV_6 sereServ6 = gridViewSereServServiceReq.GetFocusedRow() as V_HIS_SERE_SERV_6;
                if (sereServ6 != null)
                {
                    if (sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                    || sereServ6.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                        this.PacsCallModuleProccess(sereServ6);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_6 dataRow = (V_HIS_SERE_SERV_6)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow == null) return;
                    if (dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN
                    || dataRow.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                    {
                        if (e.Column.FieldName == "SO_PHIEU")
                        {
                            e.Value = this.GetSoPhieu(dataRow.TDL_SERVICE_REQ_CODE, dataRow.TDL_SERVICE_CODE);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void timerAutoReload_Tick(object sender, EventArgs e)
        {
            try
            {

                TimeRemainDisplay();
                if (timeCount == maxTimeReload)
                {
                    timerAutoReload.Stop();
                    btnFind_Click(null, null);
                    return;
                }
                timeCount = timeCount + 1000;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimeRemainDisplay()
        {
            try
            {
                long timeRemain = maxTimeReload / 1000 - ((timeCount) / 1000);
                if (timeRemain > 0)
                {
                    lblAutoReload.Text = String.Format("{0}", timeRemain);
                    lblAutoReload.ToolTip = String.Format("Tự động tải lại sau {0} giây", timeRemain);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit editor = sender as CheckEdit;
            if (editor != null && editor.EditorContainsFocus)
            {
                btnFind_Click(null, null);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                switch ((EnumUtil.REFESH_ENUM)btnRefesh.Tag)
                {
                    case EnumUtil.REFESH_ENUM.ON:
                        btnRefesh.Image = imageListRefesh.Images[0];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.OFF;
                        btnFind_Click(null, null);
                        break;
                    default:
                        btnRefesh.Image = imageListRefesh.Images[1];
                        btnRefesh.Tag = EnumUtil.REFESH_ENUM.ON;
                        timerAutoReload.Stop();
                        timeCount = 0;
                        lblAutoReload.Text = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
