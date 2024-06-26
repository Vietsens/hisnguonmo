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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.UC.ServiceRoomInfo.Run
{
    public partial class UCServiceRoomInfo
    {
        private void ValidateControl()
        {
            try
            {
                //ValidationSingleControl(cboRoom, dxValidationProvider1, ValidRoom, true);
                //ValidationSingleControl(txtExamServiceType, dxValidationProvider1, ValidService, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidService()
        {
            bool valid = true;
            try
            {
                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()) == 0 && Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()) == 0)
                {
                    return valid;
                }

                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()) == 0
                    && Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()) > 0)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ValidRoom()
        {
            bool valid = true;
            try
            {
                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()) == 0 && Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()) == 0)
                {
                    return valid;
                }
                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? 0).ToString()) == 0
                    && Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()) > 0)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, IsValidControl isValidControl, bool isUseOnlyCustomValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                    validRule.isValidControl = isValidControl;
                validRule.isUseOnlyCustomValidControl = isUseOnlyCustomValidControl;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtExamServiceType.Text))
                    {
                        cboRoom.EditValue = null;
                        cboRoom.Properties.Buttons[1].Visible = false;
                        cboRoom.Properties.DataSource = null;
                        cboExamServiceType.EditValue = null;
                        cboExamServiceType.Properties.Buttons[1].Visible = false;
                        cboExamServiceType.ShowPopup();
                    }
                    else
                    {
                        var dataFinds = examServiceTypeByPatys.Where(o => o.EXAM_SERVICE_TYPE_CODE.ToLower().Contains(txtExamServiceType.Text.ToLower())).ToList();
                        if (dataFinds != null && dataFinds.Count > 0)
                        {
                            if (dataFinds.Count == 1)
                            {
                                txtExamServiceType.Text = dataFinds[0].EXAM_SERVICE_TYPE_CODE;
                                cboExamServiceType.EditValue = dataFinds[0].SERVICE_ID;
                                cboExamServiceType.Properties.Buttons[1].Visible = true;
                                var dataRoom = serviceRooms.Where(o => (o.SERVICE_ID == dataFinds[0].SERVICE_ID && o.ROOM_TYPE_ID == HIS_ROOM_TYPE_ID__DV)).ToList();
                                cboRoom.Properties.View.Columns.Clear();
                                InitComboRoom(cboRoom, dataRoom);
                                if (dataRoom != null && dataRoom.Count == 1)
                                {
                                    cboRoom.EditValue = dataRoom[0].ROOM_ID;
                                    cboRoom.Properties.Buttons[1].Visible = true;
                                    if (this.foucusMoveOutServiceRoomInfo != null)
                                        this.foucusMoveOutServiceRoomInfo(this);
                                }
                                else
                                {
                                    cboRoom.Properties.Buttons[1].Visible = false;
                                    cboRoom.EditValue = null;
                                    cboRoom.Focus();
                                    cboRoom.ShowPopup();
                                }
                            }
                            else if (dataFinds.Count > 1)
                            {
                                cboRoom.Properties.Buttons[1].Visible = false;
                                cboRoom.EditValue = null;
                                cboExamServiceType.Properties.Buttons[1].Visible = false;
                                cboRoom.Properties.DataSource = null;
                                cboExamServiceType.EditValue = null;
                                cboExamServiceType.ShowPopup();
                                cboExamServiceType.Focus();
                            }
                            else
                            {
                                cboRoom.Properties.Buttons[1].Visible = false;
                                cboRoom.EditValue = null;
                                cboExamServiceType.Properties.Buttons[1].Visible = false;
                                cboRoom.Properties.DataSource = null;
                                cboExamServiceType.EditValue = null;
                                cboExamServiceType.ShowPopup();
                                cboExamServiceType.Focus();
                            }
                        }
                        else
                        {
                            txtExamServiceType.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadServiceToCombo(TextEdit txt, LookUpEdit cboExamServiceType, GridLookUpEdit cboRoom, GridLookUpEdit cboPaty)
        {
            try
            {
                if (String.IsNullOrEmpty(txt.Text))
                {
                    cboRoom.EditValue = null;
                    cboRoom.Properties.Buttons[1].Visible = false;
                    cboExamServiceType.Properties.Buttons[1].Visible = false;
                    cboRoom.Properties.DataSource = null;
                    cboExamServiceType.EditValue = null;
                    cboExamServiceType.ShowPopup();
                    cboExamServiceType.Focus();
                }
                else
                {
                    var data = examServiceTypeByPatys.Where(o => o.EXAM_SERVICE_TYPE_CODE.Contains(txt.Text)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            txt.Text = data[0].EXAM_SERVICE_TYPE_CODE;
                            cboExamServiceType.EditValue = data[0].SERVICE_ID;
                            cboExamServiceType.Properties.Buttons[1].Visible = true;
                            var dataRoom = serviceRooms.Where(o => (o.SERVICE_ID == data[0].SERVICE_ID && o.ROOM_TYPE_ID == HIS_ROOM_TYPE_ID__DV)).ToList();
                            cboRoom.Properties.View.Columns.Clear();

                            InitComboRoom(cboRoom, dataRoom);
                            if (dataRoom != null && dataRoom.Count == 1)
                            {
                                cboRoom.Properties.Buttons[1].Visible = true;
                                cboRoom.EditValue = dataRoom[0].ROOM_ID;
                                if (this.foucusMoveOutServiceRoomInfo != null)
                                    this.foucusMoveOutServiceRoomInfo(this);
                            }
                            else
                            {
                                cboRoom.Properties.Buttons[1].Visible = false;
                                cboRoom.EditValue = null;
                                cboRoom.Focus();
                                cboRoom.ShowPopup();
                            }
                        }
                        else if (data.Count > 1)
                        {
                            cboRoom.Properties.Buttons[1].Visible = false;
                            cboExamServiceType.Properties.Buttons[1].Visible = false;
                            cboRoom.EditValue = null;
                            cboRoom.Properties.DataSource = null;
                            cboExamServiceType.EditValue = null;
                            cboExamServiceType.ShowPopup();
                            cboExamServiceType.Focus();
                        }
                        else
                        {
                            cboRoom.Properties.Buttons[1].Visible = false;
                            cboExamServiceType.Properties.Buttons[1].Visible = false;
                            cboRoom.EditValue = null;
                            cboRoom.Properties.DataSource = null;
                            cboExamServiceType.EditValue = null;
                            cboExamServiceType.ShowPopup();
                            cboExamServiceType.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExamServiceType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE gt = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE)examServiceTypeByPatys.SingleOrDefault(o => o.SERVICE_ID == (long)cboExamServiceType.EditValue);
                        if (gt != null)
                        {
                            txtExamServiceType.Text = gt.EXAM_SERVICE_TYPE_CODE;
                            cboExamServiceType.Properties.Buttons[1].Visible = true;
                            LoadPhongKhamCombo("", gt.ID, cboRoom);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamServiceType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExamServiceType.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE gt = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE)examServiceTypeByPatys.SingleOrDefault(o => o.SERVICE_ID == (long)cboExamServiceType.EditValue);
                        if (gt != null)
                        {
                            cboExamServiceType.Properties.Buttons[1].Visible = true;
                            txtExamServiceType.Text = gt.EXAM_SERVICE_TYPE_CODE;
                            LoadPhongKhamCombo("", gt.ID, cboRoom);
                        }
                    }
                    else
                    {
                        cboRoom.Properties.DataSource = null;
                        cboRoom.EditValue = null;
                        cboRoom.ShowPopup();
                        cboRoom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPhongKhamCombo(string searchCode, long examServiceTypeId, GridLookUpEdit cboRoom)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode) && examServiceTypeId <= 0)
                {
                    cboRoom.EditValue = null;
                    cboRoom.Properties.Buttons[1].Visible = false;
                    cboRoom.ShowPopup();
                    cboRoom.Focus();
                }
                else
                {
                    var examSVDTO = examServiceTypeByPatys.FirstOrDefault(o => o.ID == examServiceTypeId);
                    if (examSVDTO == null) throw new ArgumentNullException("ExamServiceType is null");

                    var data = serviceRooms.Where(o => o.ROOM_CODE.Contains(searchCode) && (examServiceTypeId == 0 || (examSVDTO != null && o.SERVICE_ID == examSVDTO.SERVICE_ID)) && o.ROOM_TYPE_ID == HIS_ROOM_TYPE_ID__DV).ToList();
                    cboRoom.Properties.View.Columns.Clear();
                    InitComboRoom(cboRoom, data);
                    if (data != null && data.Count == 1)
                    {
                        cboRoom.Properties.Buttons[1].Visible = true;
                        cboRoom.EditValue = data[0].ROOM_ID;
                        if (this.foucusMoveOutServiceRoomInfo != null)
                            this.foucusMoveOutServiceRoomInfo(this);
                    }
                    else
                    {
                        cboRoom.Properties.Buttons[1].Visible = false;
                        cboRoom.EditValue = null;
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (!String.IsNullOrEmpty(cboRoom.Text))
                    {
                        cboRoom.Properties.Buttons[1].Visible = true;
                    }
                    if (this.foucusMoveOutServiceRoomInfo != null)
                        this.foucusMoveOutServiceRoomInfo(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(cboRoom.Text))
                    {
                        cboRoom.Properties.Buttons[1].Visible = true;
                        if (this.foucusMoveOutServiceRoomInfo != null)
                            this.foucusMoveOutServiceRoomInfo(this);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExamServiceType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtExamServiceType.Text = "";
                    cboExamServiceType.Properties.Buttons[1].Visible = false;
                    cboExamServiceType.EditValue = null;
                    cboRoom.Properties.Buttons[1].Visible = false;
                    cboRoom.EditValue = null;
                    cboRoom.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRoom.Properties.Buttons[1].Visible = false;
                    cboRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetServiceRoomInformation()
        {
            try
            {
                cboRoom.EditValue = null;
                txtExamServiceType.Text = "";
                cboExamServiceType.EditValue = null;
                cboRoom.Properties.Buttons[1].Visible = false;
                cboExamServiceType.Properties.Buttons[1].Visible = false;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExamServiceType(object control, List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXAM_SERVICE_TYPE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("EXAM_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXAM_SERVICE_TYPE_NAME", "SERVICE_ID", columnInfos, false, 320);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoom(object control, List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(control, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
