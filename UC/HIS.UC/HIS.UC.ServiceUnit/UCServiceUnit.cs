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
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.ServiceUnit.ADO;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.UC.ServiceUnit
{
    public partial class UCServiceUnit : UserControl
    {
        private ServiceUnitInitADO InitAdo { get; set; }
        private int positionHandle = -1;
        private List<HIS_SERVICE_UNIT> dataServiceUnits;

        private DelegateRequiredCause requiredCause;
        private DelegateRefeshServiceUnit refeshServiceUnit;
        private DelegatNextFocus nextFocus;
        DevExpress.XtraGrid.Columns.GridColumn aColumnCode;
        string _TextServiceUnitName = "";
        bool IsObligatoryTranferMediOrg = false;
        long autoCheckServiceUnit;

        public UCServiceUnit()
        {
            InitializeComponent();
        }

        public UCServiceUnit(ServiceUnitInitADO data)
        {
            InitializeComponent();
            this.InitAdo = data;
            if (data.Height > 0 && data.Width > 0)
            {
                this.Size = new Size(data.Width, data.Height);
            }
            if (data.DelegateNextFocus != null)
            {
                nextFocus = data.DelegateNextFocus;
            }
            if (data.DelegateRefeshServiceUnit != null)
            {
                refeshServiceUnit = data.DelegateRefeshServiceUnit;
            }
            if (data.DelegateRequiredCause != null)
            {
                requiredCause = data.DelegateRequiredCause;
            }
            if (data.DataServiceUnits != null && data.DataServiceUnits.Count > 0)
            {
                dataServiceUnits = data.DataServiceUnits.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            if (data.IsColor = false)
            {
                ValidationSingleControlWithMaxLength(txtServiceUnitMainText, false, 100);
            }
            this.IsObligatoryTranferMediOrg = data.IsObligatoryTranferMediOrg;



            this.chkEditServiceUnit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ServiceUnit__LCI_CHECK_EDIT_ServiceUnit", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ServiceUnit__LCI_ServiceUnit_MAIN", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            if (data.WidthCustomLayout > 0)
            {
                this.layoutControlItem3.TextSize = new Size(data.WidthCustomLayout, 20);
            }

        }

        private void UCServiceUnit_Load(object sender, EventArgs e)
        {
            try
            {
                DataToComboChuanDoanTD(cboServiceUnits);
                //....
                //this.autoCheckServiceUnit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckServiceUnit");
                chkEditServiceUnit.Enabled = (this.autoCheckServiceUnit != 2);

                cboServiceUnits.Focus();
                cboServiceUnits.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(ServiceUnitInputADO input)
        {
            try
            {
                if (chkEditServiceUnit.Checked)
                    dxValidationProvider2.SetValidationRule(txtServiceUnitMainText, null);
                else
                    dxValidationProvider2.SetValidationRule(cboServiceUnits, null);

                if (input != null)
                {
                    if (input.SERVICE_UNIT_ID != null && input.SERVICE_UNIT_ID > 0)
                    {
                        txtServiceUnitMainText.Visible = false;
                        cboServiceUnits.Visible = true;
                        txtServiceUnitMainText.Text = cboServiceUnits.Text;
                        if (this.InitAdo.ServiceUnitInput == null)
                            this.InitAdo.ServiceUnitInput = new ServiceUnitInputADO();
                        this.InitAdo.ServiceUnitInput.SERVICE_UNIT_ID = input.SERVICE_UNIT_ID;
                        this.InitAdo.ServiceUnitInput.SERVICE_UNIT_NAME = input.SERVICE_UNIT_NAME;
                        FillDataToCboServiceUnit();
                    }
                    else
                    {
                        chkEditServiceUnit.Checked = true;
                        cboServiceUnits.Visible = false;
                        txtServiceUnitMainText.Visible = true;
                        if (this.InitAdo.ServiceUnitInput == null)
                            this.InitAdo.ServiceUnitInput = new ServiceUnitInputADO();
                        this.InitAdo.ServiceUnitInput.SERVICE_UNIT_NAME = input.SERVICE_UNIT_NAME;
                        txtServiceUnitMainText.Text = this.InitAdo.ServiceUnitInput.SERVICE_UNIT_NAME;
                        txtServiceUnitMainText.Focus();
                        txtServiceUnitMainText.SelectAll();

                    }
                }
                else
                {
                    cboServiceUnits.EditValue = null;
                    txtServiceUnitMainText.Text = null;
                    chkEditServiceUnit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusControl()
        {
            try
            {
                cboServiceUnits.Focus();
                cboServiceUnits.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(object inPut)
        {
            try
            {
                if (inPut is ServiceUnitInputADO)
                {
                    if (this.InitAdo.ServiceUnitInput == null) this.InitAdo.ServiceUnitInput = new ServiceUnitInputADO();
                    this.InitAdo.ServiceUnitInput.SERVICE_UNIT_ID = ((ServiceUnitInputADO)inPut).SERVICE_UNIT_ID;
                    this.InitAdo.ServiceUnitInput.SERVICE_UNIT_NAME = ((ServiceUnitInputADO)inPut).SERVICE_UNIT_NAME;
                    FillDataToCboServiceUnit();
                }
                else
                {
                    cboServiceUnits.EditValue = null;
                    txtServiceUnitMainText.Text = null;
                    chkEditServiceUnit.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReadOnly(bool isReadOnly)
        {
            try
            {
                if (isReadOnly)
                {
                    txtServiceUnitMainText.ReadOnly = true;
                    cboServiceUnits.ReadOnly = true;
                    chkEditServiceUnit.ReadOnly = true;
                    cboServiceUnits.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtServiceUnitMainText.ReadOnly = false;
                    cboServiceUnits.ReadOnly = false;
                    chkEditServiceUnit.ReadOnly = false;
                    cboServiceUnits.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidationServiceUnit()
        {
            ResetValidationServiceUnit();
            bool result = true;
            try
            {

                if (chkEditServiceUnit.Checked)
                    ValidationSingleControlWithMaxLength(txtServiceUnitMainText, true, 100);
                else
                    ValidationSingleControlWithMaxLength(cboServiceUnits, true, 100);
                chkEditServiceUnit.Focus();
                this.positionHandle = -1;
                result = (dxValidationProvider2.Validate());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public void ResetValidationServiceUnit()
        {
            try
            {
                if (chkEditServiceUnit.Checked)
                    dxValidationProvider2.SetValidationRule(txtServiceUnitMainText, null);
                else
                    dxValidationProvider2.SetValidationRule(cboServiceUnits, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtServiceUnitMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditServiceUnit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnits_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboServiceUnits.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.InitAdo.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextServiceUnitName))
                        this.ChangecboChanDoanTD_V2_GanServiceUnitNAME(this._TextServiceUnitName);
                    else
                        SendKeys.Send("{TAB}");
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD()
        {
            try
            {
                cboServiceUnits.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT ServiceUnit = dataServiceUnits.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceUnits.EditValue ?? 0).ToString()));
                if (ServiceUnit != null)
                {
                    txtServiceUnitMainText.Text = ServiceUnit.SERVICE_UNIT_NAME;
                    chkEditServiceUnit.Checked = (chkEditServiceUnit.Enabled ? InitAdo.AutoCheckServiceUnit : false);
                    if (chkEditServiceUnit.Checked && this.nextFocus != null)
                    {
                        this.nextFocus();
                    }
                    else if (chkEditServiceUnit.Enabled)
                    {
                        chkEditServiceUnit.Focus();
                    }
                    else
                    {
                        if (this.nextFocus != null)
                            this.nextFocus();
                    }

                    if (this.refeshServiceUnit != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("this.refeshServiceUnit.execute");
                        this.refeshServiceUnit(ServiceUnit);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanServiceUnitNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (this.autoCheckServiceUnit != 2)
                {
                    this.chkEditServiceUnit.Enabled = true;
                    this.chkEditServiceUnit.Checked = true;
                }
                this.txtServiceUnitMainText.Text = text;
                this.txtServiceUnitMainText.Focus();
                this.txtServiceUnitMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceUnits_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboServiceUnits.ClosePopup();
                    cboServiceUnits.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboServiceUnits.ClosePopup();
                    if (cboServiceUnits.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                //else
                //    cboServiceUnits.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditServiceUnit_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditServiceUnit.Checked == true)
                {
                    cboServiceUnits.Visible = false;
                    txtServiceUnitMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtServiceUnitMainText.Text = this._TextServiceUnitName;
                    else
                        txtServiceUnitMainText.Text = cboServiceUnits.Text;
                    txtServiceUnitMainText.Focus();
                    txtServiceUnitMainText.SelectAll();
                }
                else if (chkEditServiceUnit.Checked == false)
                {
                    txtServiceUnitMainText.Visible = false;
                    cboServiceUnits.Visible = true;
                    txtServiceUnitMainText.Text = cboServiceUnits.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditServiceUnit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtServiceUnitMainText.Text != null)
                    {
                        //this.data.DelegateRefeshServiceUnitMainText(txtServiceUnitMainText.Text);
                    }
                    if (cboServiceUnits.EditValue != null)
                    {
                        //var hisServiceUnit = BackendDataWorker.Get<HIS_ServiceUnit>().Where(p => p.ID == (long)cboServiceUnits.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshServiceUnit(hisServiceUnit);
                    }
                    if (nextFocus != null) nextFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ServiceUnit_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ServiceUnit_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ServiceUnit_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataServiceUnits, controlEditorADO);
                List<ServiceUnitADO> listADO = new List<ServiceUnitADO>();
                foreach (var item in dataServiceUnits)
                {
                    ServiceUnitADO ServiceUnit = new ServiceUnitADO();
                    ServiceUnit.ID = item.ID;
                    ServiceUnit.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                    ServiceUnit.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                    ServiceUnit.ServiceUnit_NAME_UNSIGN = convertToUnSign3(item.SERVICE_UNIT_NAME);
                    listADO.Add(ServiceUnit);
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "SERVICE_UNIT_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                aColumnCode = cbo.Properties.View.Columns.AddField("SERVICE_UNIT_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("SERVICE_UNIT_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ServiceUnit_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ServiceUnit_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void FillDataToCboServiceUnit()
        {
            try
            {
                var serviceUnit = dataServiceUnits.Where(p => p.ID == (this.InitAdo.ServiceUnitInput.SERVICE_UNIT_ID));
                chkEditServiceUnit.Checked = false;
                if (serviceUnit != null)
                {
                    cboServiceUnits.EditValue = serviceUnit.First().ID;
                    txtServiceUnitMainText.Text = serviceUnit.First().SERVICE_UNIT_NAME;
                }
                else
                {
                    cboServiceUnits.EditValue = null;
                    txtServiceUnitMainText.Text = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        public void ValidationSingleControlWithMaxLength(Control control, bool isRequired, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule ServiceUnitMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                ServiceUnitMainRule.editor = control;
                ServiceUnitMainRule.maxLength = maxLength;
                ServiceUnitMainRule.IsRequired = isRequired;
                ServiceUnitMainRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, ServiceUnitMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboServiceUnits_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (!cboServiceUnits.Properties.Buttons[1].Visible)
                    return;
                this._TextServiceUnitName = "";
                cboServiceUnits.EditValue = null;
                txtServiceUnitMainText.Text = "";
                cboServiceUnits.Properties.Buttons[1].Visible = false;
            }
        }

        private void cboServiceUnits_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboServiceUnits.Text))
                {
                    cboServiceUnits.EditValue = null;
                    txtServiceUnitMainText.Text = "";
                    chkEditServiceUnit.Checked = false;
                }
                else
                {
                    this._TextServiceUnitName = cboServiceUnits.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public object GetValue()
        {
            object result = null;
            try
            {
                if (chkEditServiceUnit.Checked)
                    dxValidationProvider2.SetValidationRule(txtServiceUnitMainText, null);
                else
                    dxValidationProvider2.SetValidationRule(cboServiceUnits, null);
                ServiceUnitInputADO outPut = new ServiceUnitInputADO();
                //if (cboServiceUnits.ErrorText == "")
                //{
                    if (chkEditServiceUnit.Checked)
                        outPut.SERVICE_UNIT_NAME = txtServiceUnitMainText.Text;
                    else
                    {
                        outPut.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnits.EditValue.ToString());
                        outPut.SERVICE_UNIT_NAME = cboServiceUnits.Text;

                    }
                //}
                //else

                //    if (chkEditServiceUnit.Checked)
                //    {
                //        if (txtServiceUnitMainText.Text != null)
                //        {
                //            ResetValidationServiceUnit();
                //            outPut.SERVICE_UNIT_NAME = txtServiceUnitMainText.Text;
                //        }
                //        else
                //            outPut = null;
                //    }
                //    else
                //    {
                //        if (chkEditServiceUnit.Checked == false)
                //        {
                //            if (cboServiceUnits.EditValue == null)
                //            {
                //                outPut.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnits.EditValue.ToString());
                //                outPut.SERVICE_UNIT_NAME = cboServiceUnits.Text;
                //                ResetValidationServiceUnit();
                //            }
                //            else
                //                outPut = null;
                //        }

                //    }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboServiceUnits_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboServiceUnits_EditValueChanged.1");
                HIS_SERVICE_UNIT ServiceUnit = null;
                if (this.cboServiceUnits.EditValue != null)
                    ServiceUnit = this.dataServiceUnits.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceUnits.EditValue.ToString()));

                if (this.requiredCause == null)
                    return;



                Inventec.Common.Logging.LogSystem.Debug("cboServiceUnits_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
