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
using HIS.UC.Hospitalize.ValidateRule;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.Hospitalize.Run
{
    public partial class UCHospitalize : UserControl
    {

        List<HIS_ICD> currentIcds;
        List<HIS_ICD> currentTraditionalIcds;
        bool isAutoCheckIcd;
        long autoCheckIcd;
        string _TextIcdName = "";
        string _TextTraditionalIcdName = "";
        bool IsAcceptWordNotInData = false;
        bool IsObligatoryTranferMediOrg = false;
        bool isAllowNoIcd = false;
        string[] icdSeparators = new string[] { ",", ";" };

        private void UCIcdInit()
        {
            try
            {
                DataToComboChuanDoanTD(cboIcds, this.currentIcds);
                DataToComboChuanDoanTD(cboTraditionalIcds, this.currentTraditionalIcds);
                chkEditIcd.Enabled = (this.autoCheckIcd != 2);
                chkTraditionalIcd.Enabled = (this.autoCheckIcd != 2);

                //txtIcdCode.Focus();
                //txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CODE;
                        cboIcds.EditValue = icd.ID;
                        if ((autoCheckIcd == 1) || (!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditIcd.Checked = (this.autoCheckIcd != 2);
                            txtIcdMainText.Text = icdName;
                        }
                        else
                        {
                            chkEditIcd.Checked = false;
                            txtIcdMainText.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtIcdCode.Text = null;
                        cboIcds.EditValue = null;
                        txtIcdMainText.Text = null;
                        chkEditIcd.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(icdName))
                {
                    chkEditIcd.Checked = (this.autoCheckIcd != 2);
                    txtIcdMainText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTraditionalIcdToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentTraditionalIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtTraditionalIcdCode.Text = icd.ICD_CODE;
                        cboTraditionalIcds.EditValue = icd.ID;
                        if ((autoCheckIcd == 1) || (!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkTraditionalIcd.Checked = (this.autoCheckIcd != 2);
                            txtTraditionalIcdMainText.Text = icdName;
                        }
                        else
                        {
                            chkTraditionalIcd.Checked = false;
                            txtTraditionalIcdMainText.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtTraditionalIcdCode.Text = null;
                        cboTraditionalIcds.EditValue = null;
                        txtTraditionalIcdMainText.Text = null;
                        chkTraditionalIcd.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(icdName))
                {
                    chkTraditionalIcd.Checked = (this.autoCheckIcd != 2);
                    txtTraditionalIcdMainText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadTraditionalSubIcdToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtTraditionIcdSubCode.Text = icdCode;
                    txtTraditionIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo, List<HIS_ICD> data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);

                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBenhPhu()
        {
            try
            {
                //BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                //mainRule.maBenhPhuTxt = txtIcdSubCode;
                //mainRule.tenBenhPhuTxt = txtIcdText;
                //mainRule.getIcdMain = this.GetIcdMainCode;
                //mainRule.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderForLeftPanel.SetValidationRule(txtIcdSubCode, mainRule);

                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdSubCode;
                mainRule.tenBenhPhuTxt = txtIcdText;
                mainRule.getIcdMain = this.GetIcdMainCode;
                mainRule.listIcd = currentIcds;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtIcdSubCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                var icdValue = this.UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private string GetTraditionalIcdMainCode()
        {
            string mainCode = "";
            try
            {
                var icdValue = this.UcTraditionalIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (chkEditIcd.Checked)
                    outPut.ICD_NAME = txtIcdMainText.Text;
                else
                    outPut.ICD_NAME = cboIcds.Text;

                if (!String.IsNullOrEmpty(txtIcdCode.Text))
                {
                    outPut.ICD_CODE = txtIcdCode.Text;
                }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public object UcTraditionalIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (chkTraditionalIcd.Checked)
                    outPut.ICD_NAME = txtTraditionalIcdMainText.Text;
                else
                    outPut.ICD_NAME = cboTraditionalIcds.Text;

                if (!String.IsNullOrEmpty(txtTraditionalIcdCode.Text))
                {
                    outPut.ICD_CODE = txtTraditionalIcdCode.Text;
                }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }


        public void ValidationSingleControlWithMaxLength(Control control, bool isRequired, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule icdMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                icdMainRule.editor = control;
                icdMainRule.maxLength = maxLength;
                icdMainRule.IsRequired = isRequired;
                icdMainRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMainText.Text = result.First().ICD_NAME;
                        cboIcds.EditValue = listData.First().ID;
                        chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                        string messErr = null;
                        if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), icdSubCodeScreeen, ref messErr,false,false))
                        {
                            XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                            if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
                            {
                                txtIcdCode.Text = txtIcdMainText.Text = null;
                                cboIcds.EditValue = null;
                            }
                            return;
                        }
                        if (chkEditIcd.Checked)
                        {
                            txtIcdMainText.Focus();
                            txtIcdMainText.SelectAll();
                        }
                        else
                        {
                            cboIcds.Focus();
                            cboIcds.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboIcds.Focus();
                    cboIcds.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTraditionalIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.currentTraditionalIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtTraditionalIcdCode.Text = result.First().ICD_CODE;
                        txtTraditionalIcdMainText.Text = result.First().ICD_NAME;
                        cboTraditionalIcds.EditValue = listData.First().ID;
                        chkTraditionalIcd.Checked = (chkTraditionalIcd.Enabled ? this.isAutoCheckIcd : false);

                        if (chkTraditionalIcd.Checked)
                        {
                            txtTraditionalIcdMainText.Focus();
                            txtTraditionalIcdMainText.SelectAll();
                        }
                        else
                        {
                            cboTraditionalIcds.Focus();
                            cboTraditionalIcds.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboTraditionalIcds.Focus();
                    cboTraditionalIcds.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (this.autoCheckIcd != 2)
                {
                    this.chkEditIcd.Enabled = true;
                    this.chkEditIcd.Checked = true;
                }
                this.txtIcdMainText.Text = text;
                this.txtIcdMainText.Focus();
                this.txtIcdMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME_Traditional(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (this.autoCheckIcd != 2)
                {
                    this.chkTraditionalIcd.Enabled = true;
                    this.chkTraditionalIcd.Checked = true;
                }
                this.txtTraditionalIcdMainText.Text = text;
                this.txtTraditionalIcdMainText.Focus();
                this.txtTraditionalIcdMainText.SelectAll();
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
                txtIcdCode.ErrorText = "";
                dxValidationProvider1.RemoveControlError(txtIcdCode);
                cboIcds.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMainText.Text = icd.ICD_NAME;
                    chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), icdSubCodeScreeen, ref messErr,false,false))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        if (Desktop.Plugins.Library.CheckIcd.CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text))
                        {
                            txtIcdCode.Text = txtIcdMainText.Text = null;
                            cboIcds.EditValue = null;
                        }
                        return;
                    }
                    if (chkEditIcd.Checked)
                    {
                        this.NextForcusSubIcd();
                    }
                    else if (chkEditIcd.Enabled)
                    {
                        chkEditIcd.Focus();
                    }
                    else
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTDTraditional()
        {
            try
            {
                txtTraditionalIcdCode.ErrorText = "";
                dxValidationProvider1.RemoveControlError(txtTraditionalIcdCode);
                cboTraditionalIcds.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = this.currentTraditionalIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTraditionalIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtTraditionalIcdCode.Text = icd.ICD_CODE;
                    txtTraditionalIcdMainText.Text = icd.ICD_NAME;
                    chkTraditionalIcd.Checked = (chkTraditionalIcd.Enabled ? this.isAutoCheckIcd : false);
                    if (chkTraditionalIcd.Checked)
                    {
                        this.NextForcusSubIcd();
                    }
                    else if (chkTraditionalIcd.Enabled)
                    {
                        chkTraditionalIcd.Focus();
                    }
                    else
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                txtIcdSubCode.Focus();
                txtIcdSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcdCause()
        {
            try
            {
                txtIcdSubCode.Focus();
                txtIcdSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void UcSecondaryIcdReadOnly(bool isReadOnly)
        {
            try
            {
                if (isReadOnly)
                {
                    txtIcdSubCode.ReadOnly = true;
                    txtIcdText.ReadOnly = true;
                }
                else
                {
                    txtIcdSubCode.ReadOnly = false;
                    txtIcdText.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToControlIcdSub(string icdSubCode, string icdText)
        {
            try
            {
                this.txtIcdSubCode.Text = icdSubCode;
                this.txtIcdText.Text = icdText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void UcSecondaryIcdFocusComtrol()
        {
            try
            {
                txtIcdSubCode.Focus();
                txtIcdSubCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal object UcSecondaryIcdGetValue()
        {
            object result = null;
            try
            {
                SecondaryIcdDataADO outPut = new SecondaryIcdDataADO();

                if (!String.IsNullOrEmpty(txtIcdSubCode.Text))
                {
                    outPut.ICD_SUB_CODE = txtIcdSubCode.Text;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text))
                {
                    outPut.ICD_TEXT = txtIcdText.Text;
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal object UcTraditionalSecondaryIcdGetValue()
        {
            object result = null;
            try
            {
                SecondaryIcdDataADO outPut = new SecondaryIcdDataADO();

                if (!String.IsNullOrEmpty(txtTraditionIcdSubCode.Text))
                {
                    outPut.ICD_SUB_CODE = txtTraditionIcdSubCode.Text;
                }
                if (!String.IsNullOrEmpty(txtTraditionIcdText.Text))
                {
                    outPut.ICD_TEXT = txtTraditionIcdText.Text;
                }
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboIcds_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    txtIcdMainText.Text = "";
                    cboIcds.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1 && !this.isAllowNoIcd)
                    {
                        this.LoadRequiredCause(true);
                    }
                    else
                        this.LoadRequiredCause(false);
                }
                else
                {
                    this.LoadRequiredCause(false);
                }

                if (cboTreatmentType.EditValue != null)
                {
                    SetDataComboDepartment(Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString()));
                }
                else
                {
                    cboDepartment.Properties.DataSource = null;
                }
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                //ValidationICDCause(10, 500, isRequired);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcds.ClosePopup();
                    cboIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcds.ClosePopup();
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboIcds.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcd.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainText.Text = this._TextIcdName;
                    else
                        txtIcdMainText.Text = cboIcds.Text;
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();
                }
                else if (chkEditIcd.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                    txtIcdMainText.Text = cboIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                    {
                        e.Handled = true;
                        return;
                    }
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ProccessorByTraditionalIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckTraditionalIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                this.SetCheckedTraditionalIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckTraditionalIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtTraditionIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtTraditionIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentTraditionalIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtTraditionIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtTraditionIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtTraditionIcdSubCode.Focus();
                            this.txtTraditionIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            this.txtIcdSubCode.Focus();
                            this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames, this.currentIcds);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetCheckedTraditionalIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtTraditionIcdText.Text == txtTraditionIcdText.Properties.NullValuePrompt ? "" : txtTraditionIcdText.Text);
                txtTraditionIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames, this.currentTraditionalIcds);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtTraditionIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtTraditionIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames, List<HIS_ICD> icdList)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = icdList.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.currentIcds);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ValidationICD(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cboIcds;
                    icdMainRule.txtMainText = txtIcdMainText;
                    icdMainRule.chkCheck = chkEditIcd;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCode.ErrorText = "";
                    dxValidationProvider1.RemoveControlError(txtIcdCode);
                    dxValidationProvider1.SetValidationRule(txtIcdCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, this.currentIcds);
                    WaitingManager.Hide();
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtTraditionalIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadTraditionalIcdCombo(txtTraditionalIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTraditionalIcdCode_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "ICD không đúng";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // icd 
        private void cboTraditionalIcds_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboTraditionalIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextTraditionalIcdName = "";
                    cboTraditionalIcds.EditValue = null;
                    txtTraditionalIcdCode.Text = "";
                    txtTraditionalIcdMainText.Text = "";
                    cboTraditionalIcds.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTraditionalIcds_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboTraditionalIcds.EditValue != null)
                        this.ChangecboChanDoanTDTraditional();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextTraditionalIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME_Traditional(this._TextTraditionalIcdName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTraditionalIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboTraditionalIcds.EditValue != null)
                    icd = this.currentTraditionalIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboTraditionalIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1 && !this.isAllowNoIcd)
                    {
                        this.LoadRequiredCause(true);
                    }
                    else
                        this.LoadRequiredCause(false);
                }
                else
                {
                    this.LoadRequiredCause(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTraditionalIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboTraditionalIcds.ClosePopup();
                    cboTraditionalIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboTraditionalIcds.ClosePopup();
                    if (cboTraditionalIcds.EditValue != null)
                        this.ChangecboChanDoanTDTraditional();
                }
                else
                    cboTraditionalIcds.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTraditionalIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkTraditionalIcd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTraditionalIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTraditionalIcd.Checked == true)
                {
                    cboTraditionalIcds.Visible = false;
                    txtTraditionalIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtTraditionalIcdMainText.Text = this._TextTraditionalIcdName;
                    else
                        txtTraditionalIcdMainText.Text = cboTraditionalIcds.Text;
                    txtTraditionalIcdMainText.Focus();
                    txtTraditionalIcdMainText.SelectAll();
                }
                else if (chkTraditionalIcd.Checked == false)
                {
                    txtTraditionalIcdMainText.Visible = false;
                    cboTraditionalIcds.Visible = true;
                    txtTraditionalIcdMainText.Text = cboTraditionalIcds.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTraditionalIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtTraditionalIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text);
                    }
                    if (cboTraditionalIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
