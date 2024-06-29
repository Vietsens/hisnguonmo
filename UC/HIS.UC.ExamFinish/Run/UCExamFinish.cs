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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using MOS.EFMODEL.DataModels;
using HIS.UC.ExamFinish.ADO;
using HIS.UC.ExamFinish.Config;
using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;

namespace HIS.UC.ExamFinish.Run
{
    public partial class UCExamFinish : UserControl
    {
        int positionHandleControl = -1;
        ExamFinishInitADO examFinishInitADO { get; set; }

        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.UC.ExamFinish";

        public UCExamFinish()
        {
            InitializeComponent();
        }
        public UCExamFinish(ExamFinishInitADO _examFinishInitADO)
        {
            InitializeComponent();
            try
            {
                this.examFinishInitADO = _examFinishInitADO;
            }
            catch (Exception ex)
            {
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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void UCHospitalize_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfig.LoadConfig();
                if (examFinishInitADO.FinishTime.HasValue)
                {
                    dtFinishTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examFinishInitADO.FinishTime.Value) ?? DateTime.Now;
                }
                else
                {
                    dtFinishTime.DateTime = DateTime.Now;
                }

                if (examFinishInitADO.AppointmentTime != null)
                {
                    chkAppointment.Checked = true;
                    chkAppointment.Enabled = true;
                    chkPrintAppointment.Enabled = true;
                    dtAppointment.Enabled = true;
                    txtAdvise.Enabled = true;
                    dtAppointment.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(examFinishInitADO.AppointmentTime ?? 0) ?? DateTime.Now;
                    txtAdvise.Text = examFinishInitADO.AppointmentDesc;
                    layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                else if (!examFinishInitADO.IsMainExam)
                {
                    chkAppointment.Enabled = true;
                    chkPrintAppointment.Enabled = false;
                    dtAppointment.Enabled = false;
                    txtAdvise.Enabled = false;
                }
                else
                {
                    //chkAppointment.Enabled = false;
                    chkPrintAppointment.Enabled = false;
                    dtAppointment.Enabled = false;
                    txtAdvise.Enabled = false;
                }
                memNote.Text = examFinishInitADO.Note;
                InitCombo();

                ValidateForm();
                InitControlState();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                InitComboRoomAppointment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoomAppointment()
        {
            try
            {
                long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                if (branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                    throw new Exception("branchId null");
                List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(
                    o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1
                        && o.BRANCH_ID == branchId)
                        .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboRoomAppointment, rooms, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintAppointment.Name)
                        {
                            chkPrintAppointment.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAppointment.Checked)
                {
                    chkPrintAppointment.Enabled = true;
                    dtAppointment.Enabled = true;
                    txtAdvise.Enabled = true;

                    var appointmentTimeDefault = Inventec.Common.DateTime.Calculation.Add(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFinishTime.DateTime) ?? 0, HisConfig.AppointmentTime__DEFAULT, Inventec.Common.DateTime.Calculation.UnitDifferenceTime.DAY) ?? 0;

                    dtAppointment.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTimeDefault);

                    layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    txtServiceAppointment.Enabled = txtRoomAppointment.Enabled = cboServiceAppointment.Enabled = cboRoomAppointment.Enabled = true; ;
                }
                else
                {
                    chkPrintAppointment.Enabled = false;
                    dtAppointment.Enabled = false;
                    txtAdvise.Enabled = false;
                    layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Empty;
                    txtServiceAppointment.Enabled = txtRoomAppointment.Enabled = cboServiceAppointment.Enabled = cboRoomAppointment.Enabled = false; ;
                    cboServiceAppointment.EditValue = cboRoomAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPrintAppointment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintAppointment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintAppointment.Name;
                    csAddOrUpdate.VALUE = (chkPrintAppointment.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRoomAppointment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadRoomAppointment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRoomAppointment(string searchCode, bool v)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboRoomAppointment.Focus();
                    cboRoomAppointment.ShowPopup();
                    gridLookUpEdit1View.FocusedRowHandle = 0;
                }
                else
                {

                    long branchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                    if (branchId <= 0)// || !examServiceAddInitADO.roomId.HasValue)
                        throw new Exception("branchId null");
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().Where(
                        o => o.IS_EXAM == 1 && o.IS_ACTIVE == 1
                            && o.BRANCH_ID == branchId && o.ROOM_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboRoomAppointment.EditValue = data[0].ID;
                            cboServiceAppointment.Focus();
                            cboServiceAppointment.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboRoomAppointment.EditValue = search.ID;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                            }
                            else
                            {
                                cboRoomAppointment.EditValue = null;
                                cboRoomAppointment.Focus();
                                cboRoomAppointment.ShowPopup();
                                gridLookUpEdit1View.FocusedRowHandle = 0;
                            }
                        }
                    }
                    else
                    {
                        cboRoomAppointment.EditValue = null;
                        cboRoomAppointment.Focus();
                        cboRoomAppointment.ShowPopup();
                        gridLookUpEdit1View.FocusedRowHandle = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoomAppointment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtRoomAppointment.Text = null;
                    cboRoomAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceAppointment(long? roomId)
        {
            try
            {
                List<V_HIS_SERVICE_ROOM> serviceRooms = new List<V_HIS_SERVICE_ROOM>();
                if (roomId != null)
                {
                    serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == roomId && o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == 1).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", columnInfos, false, 250);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboServiceAppointment, serviceRooms, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceAppointment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadServiceAppointment(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceAppointment(string searchCode, bool v)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboServiceAppointment.Focus();
                    cboServiceAppointment.ShowPopup();
                    gridView1.FocusedRowHandle = 0;
                }
                else
                {
                    long? roomId = cboRoomAppointment.EditValue != null ? (long?)Convert.ToInt64(cboRoomAppointment.EditValue.ToString()) : null;
                    if (roomId == null) return;
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == roomId && o.IS_ACTIVE == 1 && o.SERVICE_TYPE_ID == 1 && o.SERVICE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboServiceAppointment.EditValue = data[0].ID;
                            cboServiceAppointment.Focus();
                            cboServiceAppointment.ShowPopup();
                        }
                        else
                        {
                            var search = data.FirstOrDefault(m => m.ROOM_CODE == searchCode);
                            if (search != null)
                            {
                                cboServiceAppointment.EditValue = search.ID;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                            }
                            else
                            {
                                cboServiceAppointment.EditValue = null;
                                cboServiceAppointment.Focus();
                                cboServiceAppointment.ShowPopup();
                                gridView1.FocusedRowHandle = 0;
                            }
                        }
                    }
                    else
                    {
                        cboServiceAppointment.EditValue = null;
                        cboServiceAppointment.Focus();
                        cboServiceAppointment.ShowPopup();
                        gridView1.FocusedRowHandle = 0;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceAppointment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtServiceAppointment.Text = null;
                    cboServiceAppointment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoomAppointment_Closed(object sender, ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                InitComboServiceAppointment(null);
                txtRoomAppointment.Text = null;
                txtServiceAppointment.Text = null;
                cboServiceAppointment.EditValue = null;
                if (cboRoomAppointment.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoomAppointment.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        InitComboServiceAppointment(data.ID);
                        txtRoomAppointment.Text = data.ROOM_CODE;
                    }
                }
            }
        }

        private void cboServiceAppointment_CloseUp(object sender, CloseUpEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                txtServiceAppointment.Text = "";
                if (cboServiceAppointment.EditValue != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().FirstOrDefault(o => o.SERVICE_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceAppointment.EditValue ?? 0).ToString()));
                    if (data != null)
                    {
                        txtServiceAppointment.Text = data.SERVICE_CODE;
                    }
                }
            }
        }
    }
}
