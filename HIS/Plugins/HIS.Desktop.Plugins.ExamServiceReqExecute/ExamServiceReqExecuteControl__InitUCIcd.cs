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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using HIS.Desktop.Utility;
using HIS.UC.Icd.ADO;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void UcIcdNextForcusOut()
        {
            try
            {
                btnAssignService.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UcIcdFocusComtrol()
        {
            try
            {
                txtIcdCode.Focus();
                txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (chkEditIcd.Checked)
                    outPut.ICD_NAME = txtIcdMainText.Text.Trim();
                else
                    outPut.ICD_NAME = cboIcds.Text.Trim();

                if (!String.IsNullOrEmpty(txtIcdCode.Text.Trim()))
                {
                    outPut.ICD_CODE = txtIcdCode.Text.Trim();
                }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
                    icdMainRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProviderForLeftPanel.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCode.ErrorText = "";
                    dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCode);
                    dxValidationProviderForLeftPanel.SetValidationRule(txtIcdCode, null);
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
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule icdMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                icdMainRule.editor = control;
                icdMainRule.maxLength = maxLength;
                icdMainRule.IsRequired = isRequired;
                icdMainRule.ErrorType = ErrorType.Warning;
                dxValidationProviderForLeftPanel.SetValidationRule(control, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetValidationICD()
        {
            try
            {
                lciIcdText.AppearanceItemCaption.ForeColor = Color.Black;
                dxValidationProviderForLeftPanel.SetValidationRule(txtIcdCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCIcdInit()
        {
            try
            {
                DataToComboChuanDoanTD(cboIcds, this.currentIcds);
                chkEditIcd.Enabled = (this.autoCheckIcd != 2);

                gridViewIcdCode.BeginUpdate();
                gridViewIcdCode.GridControl.DataSource = this.currentIcds;
                gridViewIcdCode.EndUpdate();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        isShowContainerMediMatyForChoose = true;
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

    }
}
