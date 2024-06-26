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
using HIS.UC.SecondaryIcd;
using MOS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ApprovalSurgery.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using HIS.Desktop.Plugins.ApprovalSurgery.CreateCalendar;
using System.Collections;
using HIS.Desktop.Plugins.ApprovalSurgery.EkipTemp;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using ACS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.ADO;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ApprovalSurgery.Base;
using HIS.Desktop.LocalStorage.BackendData;
using System.Threading;
using DevExpress.XtraBars;
using Inventec.Common.RichEditor.DAL;
using Inventec.Common.RichEditor;
using HIS.Desktop.ModuleExt;
using DevExpress.XtraTab;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.ApprovalSurgery
{
    public partial class UCApprovalSurgery : UserControlBase
    {
        long roomId { get; set; }
        long roomTypeId { get; set; }
        internal HIS.UC.Icd.IcdProcessor IcdProcessor { get; set; }
        int rowCountPtttCalendar = 0;
        int dataTotalPtttCalendar = 0;
        int numPageSizePtttCalendar;
        V_HIS_SERE_SERV_13 sereServ13 { get; set; }
        V_HIS_PTTT_CALENDAR ptttCalendar { get; set; }

        public int positionHandle = -1;

        int rowCountServiceReq = 0;
        int dataTotalServiceReq = 0;
        int numPageSizeServiceReq;

        int lastRowHandle = -1;
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP> ekipTemps { get; set; }

        public List<V_HIS_SERE_SERV_13> sereServ13Choose { get; set; }
        Thread t_;
        Thread t_1;
        Thread t_2;
        long id;
        private RichEditorStore richEditorMain;

        List<ACS_CONTROL> controlAcss { get; set; }

        public UCApprovalSurgery(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            Inventec.Common.Logging.LogSystem.Debug("UCApprovalSurgery moduleData.ModuleLink: " + moduleData.ModuleLink);
            InitializeComponent();
            try
            {
                this.roomId = moduleData.RoomId;
                this.roomTypeId = moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCApprovalSurgery_Load(object sender, EventArgs e)
        {
            try
            {
                LoadControlRule();

                LoadCombo();
                LoadControlDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                //    LoadDepartment(strValue, false);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDepartment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_DEPARTMENT data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? 0).ToString()));
                if (data != null)
                {
                    txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                    cboDepartment.Properties.Buttons[1].Visible = true;
                    btnCreateCalendar.Focus();
                    ptttCalendar = null;

                    FillDataToGridPtttCalendar();
                    FillDataToGridServiceReq();
                    LoadInfoFromSereServ13(null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPtttCalendar_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_PTTT_CALENDAR dataRow = (V_HIS_PTTT_CALENDAR)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPagingPtttCalendar.pagingGrid.CurrentPage - 1) * ucPagingPtttCalendar.pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "TIME_FROM_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TIME_FROM);
                    }
                    else if (e.Column.FieldName == "TIME_TO_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TIME_TO);
                    }
                    else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                    {
                        if (dataRow.APPROVAL_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.APPROVAL_TIME.Value);
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

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV_13 dataRow = (V_HIS_SERE_SERV_13)((System.Collections.IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPagingServiceReq.pagingGrid.CurrentPage - 1) * ucPagingServiceReq.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        if (dataRow.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            e.Value = dataRow.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        else
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.TDL_PATIENT_DOB.ToString());
                    }
                    else if (e.Column.FieldName == "TRANG_THAI_IMG")
                    {
                        long? statusId = dataRow.PTTT_APPROVAL_STT_ID;
                        if (statusId.HasValue)
                        {
                            if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED)

                                e.Value = imageListIcon.Images[3];

                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__REJECTED)

                                e.Value = imageListIcon.Images[4];

                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW)
                                e.Value = imageListIcon.Images[0];
                        }
                        else

                            e.Value = imageListIcon.Images[0];

                    }
                    else if (e.Column.FieldName == "PLAN_TIME_DISPLAY")
                    {
                        if (dataRow.PLAN_TIME_FROM.HasValue && dataRow.PLAN_TIME_TO.HasValue)
                        {
                            string timeFrom = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.PLAN_TIME_FROM.Value);
                            string timeTo = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(dataRow.PLAN_TIME_TO.Value);
                            e.Value = timeFrom + " - " + timeTo;
                        }
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
                sereServ13 = gridViewServiceReq.GetFocusedRow() as V_HIS_SERE_SERV_13;
                if (sereServ13 != null)
                {
                    if (e.Column.FieldName == "ACTION_APPROVAL_PLAN")
                    {
                        ApprovalPlanProcess(sereServ13, APPROVAL_PLAN_ACTION.APPROVAL);
                    }
                    else if (e.Column.FieldName == "ACTION_UNAPPROVAL_PLAN")
                    {
                        ApprovalPlanProcess(sereServ13, APPROVAL_PLAN_ACTION.UNAPPROVAL);
                    }
                    else if (e.Column.FieldName == "ACTION_REJECT_PLAN")
                    {
                        ApprovalPlanProcess(sereServ13, APPROVAL_PLAN_ACTION.REJECT);
                    }
                    else if (e.Column.FieldName == "ACTION_UNREJECT_PLAN")
                    {
                        ApprovalPlanProcess(sereServ13, APPROVAL_PLAN_ACTION.UNREJECT);
                    }
                    else if (e.Column.FieldName == "ServiceReqList")
                    {
                        repServiceReqList_Click(null, null);
                    }
                    else if (e.Column.FieldName == "DebateList")
                    {
                        repDebateList_Click(null, null);
                    }
                    else
                    {
                        LoadInfoFromSereServ13(sereServ13);
                        EnabledControl(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEkipTemp.EditValue != null)
                {
                    cboEkipTemp.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEkipTemp_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.CloseMode == PopupCloseMode.Normal)
                    {
                        if (cboEkipTemp.EditValue != null && this.ekipTemps != null && this.ekipTemps.Count > 0)
                        {
                            var data = this.ekipTemps.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEkipTemp.EditValue ?? 0).ToString()));
                            if (data != null)
                            {
                                cboEkipTemp.Properties.Buttons[1].Visible = true;
                                LoadGridEkipUserFromTemp(data.ID);
                                btnSaveEkipTemp.Focus();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkipPlanUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "ACTION")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridViewEkipPlanUser.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repositoryItemButtonEditPlus;
                    }
                    else if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                    {
                        e.RepositoryItem = repositoryItemButtonEditMinus;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewEkipPlanUser_ShowingEditor(object sender, CancelEventArgs e)
        {

        }

        private void reposityButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridLookUpEdit editor = sender as GridLookUpEdit;
                    if (editor != null)
                    {
                        editor.EditValue = null;
                        editor.Properties.Buttons[1].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemButtonEditPlus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var ekipPlanUsers = gridControlEkipPlanUser.DataSource as List<EkipPlanUserADO>;
                EkipPlanUserADO ekipPlanUserADO = new EkipPlanUserADO();
                ekipPlanUserADO.Action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                ekipPlanUsers.Add(ekipPlanUserADO);
                gridControlEkipPlanUser.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditMinus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var ekipPlanUsers = gridControlEkipPlanUser.DataSource as List<EkipPlanUserADO>;
                var ekipPlanUser = gridViewEkipPlanUser.GetFocusedRow() as EkipPlanUserADO;
                if (ekipPlanUser != null)
                {
                    ekipPlanUsers.Remove(ekipPlanUser);
                    gridControlEkipPlanUser.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreateCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                frmCreateCalendar frm = new frmCreateCalendar(roomId, refeshDataResult);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void refeshDataResult(object data)
        {
            if (data != null)
            {
                FillDataToGridPtttCalendar();
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridServiceReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPtttCalendar_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_PTTT_CALENDAR data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewPtttCalendar.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_PTTT_CALENDAR)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (e.Column.FieldName == "ACTION_APPROVAL")
                        {
                            ACTION_APPROVAL_CALENDAR? action = null;
                            bool checkApproval = CheckApproval(data, ref action);
                            if (action == ACTION_APPROVAL_CALENDAR.APPROVAL)
                            {
                                e.RepositoryItem = checkApproval
                                    ? repositoryItemButtonEditApproval : repositoryItemButtonEditApproval_Disabled;
                            }
                            else if (action == ACTION_APPROVAL_CALENDAR.UNAPPROVAL)
                            {
                                e.RepositoryItem = checkApproval
                                            ? repositoryItemButtonEditUnApproval : repositoryItemButtonEditUnApproval_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "PRINT")
                        {
                            if (data.APPROVAL_TIME.HasValue)
                                e.RepositoryItem = repositoryItemButtonEditPrint;
                            else
                                e.RepositoryItem = repositoryItemButtonEditPrint_Disabled;
                        }
                        else if (e.Column.FieldName == "EDIT")
                        {
                            if (CheckApprovalEdit(data))
                                e.RepositoryItem = repositoryItemButtonEditCalendar;
                            else
                                e.RepositoryItem = repositoryItemButtonEditCalendar_Disabled;
                        }
                        else if (e.Column.FieldName == "DELETE")
                        {
                            if (CheckDelete(data))
                            {
                                e.RepositoryItem = repositoryItemButtonDeleteCalender_Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDeleteCalender_Disable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPtttCalendar_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "ACTION_APPROVAL" && e.Column.FieldName != "EDIT" && e.Column.FieldName != "PRINT")
                {
                    ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                    rdoGroupCalendar.SelectedIndex = 0;
                    FillDataToGridServiceReq();
                    LoadInfoFromSereServ13(null);
                    SetEnableApproval(rdoGroupCalendar.SelectedIndex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                    cboDepartment.Properties.Buttons[1].Visible = false;
                    txtDepartmentCode.Text = null;
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPlanTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                dtPlanTimeTo.Focus();
                dtPlanTimeTo.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPlanTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                // CalculatePlanTime();
                cboEkipTemp.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveEkipTemp_Click(object sender, EventArgs e)
        {
            try
            {
                string mess = "";
                var ekipUsers = gridControlEkipPlanUser.DataSource as List<EkipPlanUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Không có thông tin kip thực hiện");
                    return;
                }
                if (ekipUsers != null && ekipUsers.Count > 0)
                {

                    var lstLoginName = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                    foreach (var Login in lstLoginName)
                    {
                        List<string> lstExecuteRoleName = new List<string>();
                        var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                        var check = ekipUsers.Where(o => o.LOGINNAME == Login).ToList();
                        if (check != null && check.Count > 1)
                        {
                            foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                            {
                                lstExecuteRoleName.Add(executeRole.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                            }
                            string messLogin = String.Join(",", lstExecuteRoleName);
                            List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                            var name = data.Where(o => o.LOGINNAME == check.FirstOrDefault().LOGINNAME).FirstOrDefault();
                            mess += String.Format("Tài khoản {0} được thiết lập với các vai trò {1}.\n", name.USERNAME, messLogin);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => check), check));
                        }
                    }
                    if (!string.IsNullOrEmpty(mess))
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    frmEkipTemp frm = new frmEkipTemp(ekipUsers, RefeshDataEkipTemp);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataEkipTemp()
        {
            try
            {
                LoadComboEkipTemp();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string mess = "";
            bool success = false;
            bool valid = true;
            try
            {
                ValidateControl();
                var ekipUsers = gridControlEkipPlanUser.DataSource as List<EkipPlanUserADO>;
                bool hasInvalid = ekipUsers
                   .Where(o => String.IsNullOrEmpty(o.LOGINNAME)
                       || o.EXECUTE_ROLE_ID <= 0 || o.EXECUTE_ROLE_ID == null).FirstOrDefault() != null ? true : false;
                if (hasInvalid)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin kip thực hiện", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (ekipUsers != null && ekipUsers.Count > 0)
                {
                    var executeRole = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                    var lstLoginName = ekipUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                    var lstRole = ekipUsers.Select(o => o.EXECUTE_ROLE_ID).Distinct().ToList();
                    foreach (var item in lstRole)
                    {
                        var role = executeRole.FirstOrDefault(o => o.ID == item);
                        var users = ekipUsers.Where(o => o.EXECUTE_ROLE_ID == item);
                        if (role.IS_SINGLE_IN_EKIP == 1 && users != null && users.Count() > 1)
                        {
                            mess += String.Format("Không được phép nhập nhiều hơn 1 tài khoản đối với vai trò {0}.\n", role.EXECUTE_ROLE_NAME);
                        }
                    }
                    foreach (var Login in lstLoginName)
                    {
                        List<string> lstExecuteRoleName = new List<string>();
                        var check = ekipUsers.Where(o => o.LOGINNAME == Login).ToList();
                        if (check != null && check.Count > 1)
                        {
                            foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                            {
                                lstExecuteRoleName.Add(executeRole.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                            }
                            string messLogin = String.Join(",", lstExecuteRoleName);
                            List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                            var name = data.Where(o => o.LOGINNAME == check.FirstOrDefault().LOGINNAME).FirstOrDefault();
                            mess += String.Format("Tài khoản {0} được thiết lập với các vai trò {1}.\n", name.USERNAME, messLogin);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => check), check));
                        }
                    }
                    if (!string.IsNullOrEmpty(mess))
                    {
                        WaitingManager.Hide();
                        MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                }


                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    HisServiceReqPlanSDO hisServiceReqPlanSDO = new HisServiceReqPlanSDO();
                    hisServiceReqPlanSDO.PlanTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPlanTimeFrom.DateTime).Value;
                    hisServiceReqPlanSDO.PlanTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPlanTimeTo.DateTime).Value;
                    hisServiceReqPlanSDO.ServiceReqId = sereServ13.SERVICE_REQ_ID ?? 0;
                    hisServiceReqPlanSDO.WorkingRoomId = this.roomId;
                    hisServiceReqPlanSDO.PlanEkip = GetEkipPlanUser();
                    hisServiceReqPlanSDO.ExecuteRoomId = long.Parse(cboRoom.EditValue.ToString());
                    if (cboEmotionless.EditValue != null)
                    {
                        hisServiceReqPlanSDO.EmotionlessMethodId = (long)cboEmotionless.EditValue;
                    }
                    if (cboMethod.EditValue != null)
                    {
                        hisServiceReqPlanSDO.PtttMethodId = (long)cboMethod.EditValue;
                    }
                    hisServiceReqPlanSDO.PlanningRequest = txtPlanningRequest.Text.Trim();
                    hisServiceReqPlanSDO.SurgeryNote = txtSurgeryNote.Text.Trim();
                    hisServiceReqPlanSDO.Manner = txtMANNER.Text;
                    Inventec.Common.Logging.LogSystem.Info("Đầu vào Api api/HisServiceReq/SurgPlan " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqPlanSDO), hisServiceReqPlanSDO));

                    var result = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>("api/HisServiceReq/SurgPlan", ApiConsumers.MosConsumer, hisServiceReqPlanSDO, param);
                    if (result != null)
                    {
                        success = true;
                        FillDataToGridServiceReq();
                        //List<V_HIS_SERE_SERV_13> sereServ13ADOs = gridControlServiceReq.DataSource as List<V_HIS_SERE_SERV_13>;
                        //if (sereServ13ADOs != null && sereServ13ADOs.Count > 0)
                        //{
                        //    foreach (var item in sereServ13ADOs)
                        //    {
                        //        if (item.SERVICE_REQ_ID == result.ID)
                        //        {
                        //            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_13>(item, result);
                        //            gridControlServiceReq.RefreshDataSource();
                        //            break;
                        //        }
                        //    }
                        //}
                        gridControlServiceReq.RefreshDataSource();
                    }
                    //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result),result));
                    //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoGroupCalendar_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void rdoGroupCalendar_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetEnableApproval(rdoGroupCalendar.SelectedIndex);
            if (ptttCalendar != null)
            {
                sereServ13Choose = new List<V_HIS_SERE_SERV_13>();
            }
            RadioGroup rdoGroup = sender as RadioGroup;
            if (rdoGroup.EditorContainsFocus)
            {
                btnFind_Click(null, null);
            }

        }

        private void cboEkipTemp_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                cboEkipTemp.EditValue = null;
                cboEkipTemp.Properties.Buttons[1].Visible = false;
                InitEkipPlanUser();
            }
        }

        private void gridViewServiceReq_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_SERE_SERV_13 data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewServiceReq.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_SERE_SERV_13)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "HAS_CALENDAR")
                        {
                            if (data.PTTT_CALENDAR_ID.HasValue)
                            {
                                e.RepositoryItem = repositoryItemButtonEditHasCalendar;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditNotHasCalendar;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_APPROVAL_PLAN")
                        {
                            if (CheckApprovalPlan(data, APPROVAL_PLAN_ACTION.APPROVAL))
                            {
                                e.RepositoryItem = repositoryItemButtonEditApprovalPlan;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditApprovalPlan_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_UNAPPROVAL_PLAN")
                        {
                            if (CheckApprovalPlan(data, APPROVAL_PLAN_ACTION.UNAPPROVAL))
                            {
                                e.RepositoryItem = repositoryItemButtonEditUnApprovalPlan;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditUnApprovalPlan_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_REJECT_PLAN")
                        {
                            if (CheckApprovalPlan(data, APPROVAL_PLAN_ACTION.REJECT))
                            {
                                e.RepositoryItem = repositoryItemButtonEditRejectPlan;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditRejectPlan_Disabled;
                            }
                        }
                        else if (e.Column.FieldName == "ACTION_UNREJECT_PLAN")
                        {
                            if (CheckApprovalPlan(data, APPROVAL_PLAN_ACTION.UNREJECT))
                            {
                                e.RepositoryItem = repositoryItemButtonEditUnRejectPlan;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditUnRejectPlan_Disabled;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
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
                            if (info.Column.FieldName == "TRANG_THAI_IMG")
                            {

                                long statusId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "PTTT_APPROVAL_STT_ID") ?? "").ToString());
                                if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__APPROVED)
                                {
                                    text = "Đã duyệt";
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__NEW || statusId == 0)
                                {
                                    text = "Chưa duyệt";
                                }
                                else if (statusId == IMSys.DbConfig.HIS_RS.HIS_PTTT_APPROVAL_STT.ID__REJECTED)
                                {
                                    text = "Đã từ chối";
                                }

                                lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                            }
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

        private void gridViewServiceReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == CollectionChangeAction.Refresh && e.ControllerRow < 0)
                {
                    int[] selectRows = gridViewServiceReq.GetSelectedRows();
                    if (selectRows != null && selectRows.Count() > 0)
                    {
                        for (int i = 0; i < selectRows.Count(); i++)
                        {
                            V_HIS_SERE_SERV_13 sereServ13Temp = (V_HIS_SERE_SERV_13)gridViewServiceReq.GetRow(selectRows[i]);
                            if (sereServ13Temp != null)
                            {
                                V_HIS_SERE_SERV_13 temp = sereServ13Choose.FirstOrDefault(o => o.SERVICE_REQ_ID == sereServ13Temp.SERVICE_REQ_ID);
                                if (temp == null)
                                {
                                    sereServ13Choose.Add(sereServ13Temp);
                                }
                            }
                        }
                    }
                }
                else if (e.Action == CollectionChangeAction.Add)
                {
                    V_HIS_SERE_SERV_13 sereServ13Temp = (V_HIS_SERE_SERV_13)gridViewServiceReq.GetRow(e.ControllerRow);
                    if (sereServ13Temp != null)
                    {
                        V_HIS_SERE_SERV_13 temp = sereServ13Choose.FirstOrDefault(o => o.SERVICE_REQ_ID == sereServ13Temp.SERVICE_REQ_ID);
                        if (temp == null)
                        {
                            sereServ13Choose.Add(sereServ13Temp);
                        }
                    }
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    V_HIS_SERE_SERV_13 sereServ13Temp = (V_HIS_SERE_SERV_13)gridViewServiceReq.GetRow(e.ControllerRow);
                    if (sereServ13Temp != null)
                    {
                        V_HIS_SERE_SERV_13 temp = sereServ13Choose.FirstOrDefault(o => o.SERVICE_REQ_ID == sereServ13Temp.SERVICE_REQ_ID);
                        if (temp != null)
                        {
                            sereServ13Choose = sereServ13Choose.Where(o => o.SERVICE_REQ_ID != temp.SERVICE_REQ_ID).ToList();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewPtttCalendar_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.Gainsboro;
            }
            else
            {
                e.Appearance.BackColor = Color.Empty;
            }
        }

        private void btnCalendarAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CalendarAddRemoveProccess(CALENDAR_ACTION.ADD);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCalendarRemove_Click(object sender, EventArgs e)
        {
            try
            {
                CalendarAddRemoveProccess(CALENDAR_ACTION.REMOVE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewEkipPlanUser_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    EkipPlanUserADO data = view.GetFocusedRow() as EkipPlanUserADO;
                    if (view.FocusedColumn.FieldName == "EXECUTE_ROLE_ID" && view.ActiveEditor is GridLookUpEdit)
                    {
                        GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                        if (data != null)
                        {
                            if (data.EXECUTE_ROLE_ID > 0)
                            {
                                editor.EditValue = data.EXECUTE_ROLE_ID;
                                editor.Properties.Buttons[1].Visible = true;
                                editor.ButtonClick += reposityButtonClick;
                            }
                        }
                    }
                    else if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                    {
                        GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                        if (data != null)
                        {
                            if (!String.IsNullOrEmpty(data.LOGINNAME))
                            {
                                editor.EditValue = data.LOGINNAME;
                                editor.Properties.Buttons[1].Visible = true;
                                editor.ButtonClick += reposityButtonClick;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditApproval_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ApprovalProcess(ACTION_APPROVAL_CALENDAR.APPROVAL);
        }

        private void repositoryItemButtonEditUnApproval_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            ApprovalProcess(ACTION_APPROVAL_CALENDAR.UNAPPROVAL);
        }

        private void repositoryItemButtonEditCalendar_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_PTTT_CALENDAR ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                if (ptttCalendar != null)
                {
                    frmCreateCalendar frm = new frmCreateCalendar(roomId, ptttCalendar, refeshDataResult);
                    frm.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnLichSuDieuTri_Click(object sender, EventArgs e)
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
                    treatmentFilter.ID = sereServ13.TDL_TREATMENT_ID;
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

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
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

        private void dtPlanTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPlanTimeTo.Focus();
                    dtPlanTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtPlanTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEkipTemp.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.Gainsboro;
            }
            else
            {
                e.Appearance.BackColor = Color.Empty;
            }
        }

        private void repositoryItemButtonEditPrint_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_LICH_MO__MPS000272, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            return InLichDuyetMo(printCode, fileName);
        }
        private bool DelegateRunPrinter_(string printCode, string fileName)
        {
            return InLichDuyetMo_(printCode, fileName);
        }
        private void repositoryItemButtonDeleteCalender_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_PTTT_CALENDAR ptttCalendar = gridViewPtttCalendar.GetFocusedRow() as V_HIS_PTTT_CALENDAR;
                if (ptttCalendar != null && !ptttCalendar.APPROVAL_TIME.HasValue)
                {
                    var yes = XtraMessageBox.Show("Bạn muốn xóa lịch mổ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                    if (yes != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisPtttCalendarSDO sdo = new HisPtttCalendarSDO();
                    sdo.Id = ptttCalendar.ID;
                    sdo.WorkingRoomId = this.roomId;
                    var result = new BackendAdapter(param).Post<bool>("api/HisPtttCalendar/Delete", ApiConsumers.MosConsumer, sdo, param);
                    if (result)
                    {
                        FillDataToGridPtttCalendar();
                        success = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                dtPlanTimeFrom.Focus();
                dtPlanTimeFrom.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMethod.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PTTT_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMethod.EditValue.ToString()));
                        {
                            txtMethod.Text = data.PTTT_METHOD_CODE;
                            cboMethod.Properties.Buttons[1].Visible = true;
                            txtEmotionless.Focus();
                            txtEmotionless.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmotionless_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEmotionless.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EMOTIONLESS_METHOD data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMOTIONLESS_METHOD>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboEmotionless.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtEmotionless.Text = data.EMOTIONLESS_METHOD_CODE;
                            cboEmotionless.Properties.Buttons[1].Visible = true;
                            cboRoom.Focus();
                            cboRoom.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = false;
                    cboMethod.EditValue = null;
                    txtMethod.Text = "";
                    txtMethod.Focus();
                    txtMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEmotionless_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboEmotionless.Properties.Buttons[1].Visible = false;
                    cboEmotionless.EditValue = null;
                    LoadComboEmotionlessDefault();
                    txtEmotionless.Text = "";
                    txtEmotionless.Focus();
                    txtEmotionless.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {

            try
            {
                FillDataToGridPtttCalendar();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnDichVuDinhKem_Click(object sender, EventArgs e)
        {

            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    WaitingManager.Hide();
                    List<object> listArgs = new List<object>();
                    AssignServiceADO AssignServiceADO = new AssignServiceADO(id, 0, 0);
                    AssignServiceADO.TreatmentId = id;
                    AssignServiceADO.SereServ = new V_HIS_SERE_SERV();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_13, V_HIS_SERE_SERV>();
                    AssignServiceADO.SereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV_13, V_HIS_SERE_SERV>(sereServ13);
                    AssignServiceADO.PatientName = sereServ13.TDL_PATIENT_NAME;
                    AssignServiceADO.GenderName = sereServ13.TDL_PATIENT_GENDER_NAME;
                    AssignServiceADO.SereServ.ID = sereServ13.ID;
                    AssignServiceADO.PatientDob = sereServ13.TDL_PATIENT_DOB;
                    //AssignServiceADO.IsAutoEnableEmergency = true;
                    AssignServiceADO.IsAssignInPttt = true;
                    listArgs.Add(AssignServiceADO);
                    WaitingManager.Hide();
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.roomId, this.roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AssignServiceADO), AssignServiceADO));
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AssignServiceADO.SereServ), AssignServiceADO.SereServ));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }



        }

        private void btnIn_Click(object sender, EventArgs e)
        {

            try
            {

                this.Invoke(new Action(() =>
                {
                    t_1 = new Thread(() =>
                    {
                        this.Invoke(new Action(() =>
                        {
                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_LICH_MO__MPS000272, DelegateRunPrinter_);
                        }));
                    });
                }));
                t_1.IsBackground = true;
                t_1.Start();
                //t_1 = new Thread(() =>
                //{

                //    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                //    richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__IN_LICH_MO__MPS000272, DelegateRunPrinter_);
                //    //id = BackendDataWorker.Get<HIS_TREATMENT>().FirstOrDefault(o => o.TREATMENT_CODE == sereServ13.TDL_TREATMENT_CODE).ID;
                //});
                //t_1.IsBackground = true;
                //t_1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewPtttCalendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<V_HIS_PTTT_CALENDAR> listSelection = new List<V_HIS_PTTT_CALENDAR>();
                var rowHandles = gridViewPtttCalendar.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (V_HIS_PTTT_CALENDAR)gridViewPtttCalendar.GetRow(i);
                        if (row != null)
                        {
                            listSelection.Add(row);
                        }
                    }
                }
                if (listSelection != null && listSelection.Count() > 0)
                {
                    btnIn.Enabled = true;
                }
                else
                {
                    btnIn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPtttCalendar_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var RowData = (V_HIS_PTTT_CALENDAR)view.GetRow(hi.RowHandle);

                        if (RowData != null)
                        {
                            if (hi.Column.FieldName == "ACTION_APPROVAL")
                            {
                                ACTION_APPROVAL_CALENDAR? action = null;
                                bool checkApproval = CheckApproval(RowData, ref action);
                                if (action == ACTION_APPROVAL_CALENDAR.APPROVAL)
                                {
                                    if (checkApproval)
                                    {
                                        repositoryItemButtonEditApproval_ButtonClick(null, null);
                                    }
                                }
                                else if (action == ACTION_APPROVAL_CALENDAR.UNAPPROVAL)
                                {
                                    if (checkApproval)
                                    {
                                        repositoryItemButtonEditUnApproval_ButtonClick(null, null);
                                    }
                                }
                            }
                            else if (hi.Column.FieldName == "PRINT")
                            {
                                if (RowData.APPROVAL_TIME.HasValue)
                                    repositoryItemButtonEditPrint_ButtonClick(null, null);
                            }
                            else if (hi.Column.FieldName == "EDIT")
                            {
                                if (CheckApprovalEdit(RowData))
                                    repositoryItemButtonEditCalendar_ButtonClick(null, null);
                            }
                            else if (hi.Column.FieldName == "DELETE")
                            {
                                if (CheckDelete(RowData))
                                {
                                    repositoryItemButtonDeleteCalender_Enable_ButtonClick(null, null);
                                }
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


        V_HIS_SERE_SERV_13 currentRow = new V_HIS_SERE_SERV_13();
        private void gridViewServiceReq_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewServiceReq.GetVisibleRowHandle(hi.RowHandle);

                    currentRow = (V_HIS_SERE_SERV_13)gridViewServiceReq.GetRow(rowHandle);

                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewServiceReq.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new DevExpress.XtraBars.BarManager();
                        barManager1.Form = this;
                    }

                    PopupMenuProcessor popupMenuProcessor = new PopupMenuProcessor(barManager1, SereServ_MouseRightClick);//(RefeshReference)BtnSearch
                    popupMenuProcessor.InitMenu(this.roomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SereServ_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumerStore.SarConsumer, UriBaseStore.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.HoSoDieuTri:
                            if (currentRow != null)
                            {
                                WaitingManager.Show();
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentList").FirstOrDefault();
                                Inventec.Desktop.Common.Modules.Module moduleShow = new Inventec.Desktop.Common.Modules.Module();
                                moduleShow = ButtonMenuProcessor.CreateModuleData(moduleData, this.roomId, this.roomTypeId);
                                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((moduleShow));
                                int index = IsExistsModuleTabPage(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0"));
                                if (index >= 0)
                                {
                                    if (SessionManager.GetTabControlMain().TabPages[index].PageVisible == false)
                                        SessionManager.GetTabControlMain().TabPages[index].PageVisible = true;

                                    SessionManager.GetTabControlMain().TabPages.Remove(SessionManager.GetTabControlMain().TabPages[index]);
                                }
                                List<object> listArgs = new List<object>();
                                listArgs.Add(currentRow.TDL_TREATMENT_CODE);
                                WaitingManager.Hide();
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentList", this.roomId, this.roomTypeId, listArgs);
                            }
                            break;
                        case PopupMenuProcessor.ItemType.DanhSachToDieuTri:
                            if (currentRow != null)
                            {
                                WaitingManager.Show();
                                List<object> listArgs = new List<object>();
                                listArgs.Add(currentRow.TDL_TREATMENT_ID);
                                WaitingManager.Hide();
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTrackingList", this.roomId, this.roomTypeId, listArgs);
                            }
                            break;
                        case PopupMenuProcessor.ItemType.DanhSachYLenh:
                            if (currentRow != null)
                            {
                                WaitingManager.Show();
                                List<object> listArgs = new List<object>();
                                HIS_TREATMENT thistreatment = new HIS_TREATMENT();
                                thistreatment.ID = currentRow.TDL_TREATMENT_ID ?? 0;
                                listArgs.Add(thistreatment);
                                WaitingManager.Hide();
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.roomId, this.roomTypeId, listArgs);
                            }
                            break;
                        case PopupMenuProcessor.ItemType.BienBanHoiChan:
                            if (currentRow != null)
                            {
                                WaitingManager.Show();
                                List<object> listArgs = new List<object>();
                                listArgs.Add(currentRow.TDL_TREATMENT_ID);
                                WaitingManager.Hide();
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Debate", this.roomId, this.roomTypeId, listArgs);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected static int IsExistsModuleTabPage(XtraTabControl tabControlName, string tabName)
        {
            int result = -1;
            try
            {
                for (int i = 0; i < tabControlName.TabPages.Count; i++)
                {
                    if (tabControlName.TabPages[i].Name == tabName)
                    {
                        result = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        private void repServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gridViewServiceReq.GetFocusedRow() as V_HIS_SERE_SERV_13;
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT thistreatment = new HIS_TREATMENT();
                    thistreatment.ID = row.TDL_TREATMENT_ID ?? 0;
                    listArgs.Add(thistreatment);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceReqList", this.roomId, this.roomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repDebateList_Click(object sender, EventArgs e)
        {
            try
            {
                var row = gridViewServiceReq.GetFocusedRow() as V_HIS_SERE_SERV_13;
                if (row != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.TDL_TREATMENT_ID);
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Debate", this.roomId, this.roomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartmentServiceReq_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartmentServiceReq.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
