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
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.RegisterV3.RunV3.UC
{
    public partial class RoomVaccine : UserControl
    {
        List<V_HIS_EXECUTE_ROOM> listExecuteRooms;
        List<HIS_PATIENT_TYPE> listPatientTypes;
        DelegateRegister.DelegateFocusNextUserControl delegateFocusNextUserControl;
        int positionHandleControl = -1;

        public RoomVaccine(DelegateRegister.DelegateFocusNextUserControl dlg)
        {
            InitializeComponent();

            try
            {
                this.delegateFocusNextUserControl = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RoomVaccine_Load(object sender, EventArgs e)
        {
            try
            {
                dtRequestTime.DateTime = DateTime.Now;
                ValidateGridLookupWithTextEdit(cboRoom, txtRoom, dxValidationProvider1);
                ValidateGridLookupWithTextEdit(cboPatientType, txtPatientType, dxValidationProvider1);
                ValidateSingleControl(dtRequestTime, dxValidationProvider1);

                listExecuteRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>();
                listPatientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();

                this.InitComboCommon(this.cboRoom, listExecuteRooms.Where(o => o.IS_VACCINE == 1 && o.IS_ACTIVE == 1).ToList(), "ID", "EXECUTE_ROOM_NAME", "EXECUTE_ROOM_CODE");
                this.InitComboCommon(this.cboPatientType, listPatientTypes.Where(o => (o.PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.VACC") || o.PATIENT_TYPE_CODE == HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.VACC_EPI"))).ToList(), "ID", "PATIENT_TYPE_NAME", "PATIENT_TYPE_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControl(Control cbo, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(cbo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtPatientType.Text))
                    {
                        cboPatientType.EditValue = null;
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                    else
                    {
                        List<HIS_PATIENT_TYPE> searchs = null;
                        var listData1 = this.listPatientTypes.Where(o => o.PATIENT_TYPE_CODE.ToUpper().Contains(txtPatientType.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.PATIENT_TYPE_CODE.ToUpper() == txtPatientType.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtPatientType.Text = searchs[0].PATIENT_TYPE_CODE;
                            cboPatientType.EditValue = searchs[0].ID;
                            dtRequestTime.Focus();
                        }
                        else
                        {
                            cboPatientType.Focus();
                            cboPatientType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                try
                {
                    if (e.CloseMode == PopupCloseMode.Normal)
                    {
                        if (cboPatientType.EditValue != null)
                        {
                            var data = this.listPatientTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "").ToString()));
                            if (data != null)
                            {
                                txtPatientType.Text = data.PATIENT_TYPE_CODE;
                                txtPatientType.Focus();
                                txtPatientType.SelectAll();
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

        private void txtRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtRoom.Text))
                    {
                        cboRoom.EditValue = null;
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_EXECUTE_ROOM> searchs = null;
                        var listData1 = this.listExecuteRooms.Where(o => o.EXECUTE_ROOM_CODE.ToUpper().Contains(txtRoom.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == txtRoom.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtRoom.Text = searchs[0].EXECUTE_ROOM_CODE;
                            cboRoom.EditValue = searchs[0].ID;
                            if (this.delegateFocusNextUserControl != null)
                            {
                                this.delegateFocusNextUserControl();
                            }
                        }
                        else
                        {
                            cboRoom.Focus();
                            cboRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var data = this.listExecuteRooms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtRoom.Text = data.EXECUTE_ROOM_CODE;
                            txtRoom.Focus();
                            txtRoom.SelectAll();
                        }
                    }
                }
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

        private void dtRequestTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtRoom.Focus();
                    txtRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtRequestTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRoom.Focus();
                    txtRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
