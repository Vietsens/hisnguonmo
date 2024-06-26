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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule;
using HIS.Desktop.Utility;
using HIS.UC.NextTreatmentInstruction.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        private void UcNextTreatmentInstNextForcus()
        {
            try
            {
                txtResultNote.Focus();
                txtResultNote.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UcNextTreatmentInstFocusComtrol()
        {
            try
            {
                txtNextTreatmentInstructionCode.Focus();
                txtNextTreatmentInstructionCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object UcNextTreatmentInstGetValue()
        {
            object result = null;
            try
            {
                NextTreatmentInstructionInputADO outPut = new NextTreatmentInstructionInputADO();
                if (txtNextTreatmentInstructionCode.ErrorText == "")
                {
                    if (chkEditNextTreatmentInstruction.Checked)
                        outPut.NEXT_TREA_INTR_NAME = txtNextTreatmentInstructionMainText.Text.Trim();
                    else
                        outPut.NEXT_TREA_INTR_NAME = cboNextTreatmentInstructions.Text.Trim();

                    if (!String.IsNullOrEmpty(txtNextTreatmentInstructionCode.Text.Trim()))
                    {
                        outPut.NEXT_TREA_INTR_CODE = txtNextTreatmentInstructionCode.Text.Trim();
                    }
                }
                else
                    outPut = null;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UCNextTreatmentInstructionValid()
        {
            try
            {
                ValidationSingleControlWithMaxLength(txtNextTreatmentInstructionCode, false, 2);
                ValidationSingleControlWithMaxLength(txtNextTreatmentInstructionMainText, false, 100);

                //if (data.SizeText > 0)
                //{
                //    this.txtNextTreatmentInstructionCode.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                //    this.txtNextTreatmentInstructionMainText.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                //    this.cboNextTreatmentInstructions.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                //    this.chkEditNextTreatmentInstruction.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                //    this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font = new Font(this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                //    this.lciNextTreatmentInstructionText.TextSize = new Size(200, 20);
                //    this.layoutControlItem3.AppearanceItemCaption.Font = new Font(this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                //    this.lciNextTreatmentInstructionText.Size = new Size(321, 40);
                //}
                //if (!String.IsNullOrEmpty(data.ToolTipsNextTreatmentInstructionMain))
                //{
                //    this.lciNextTreatmentInstructionText.OptionsToolTip.ToolTip = data.ToolTipsNextTreatmentInstructionMain;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationNextTreatmentInst(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciNextTreatmentInstructionText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    NextTreatmentInstValidationRuleControl nextTreatmentIntructionMainRule = new NextTreatmentInstValidationRuleControl();
                    nextTreatmentIntructionMainRule.txtNextTreatmentInstructionCode = txtNextTreatmentInstructionCode;
                    nextTreatmentIntructionMainRule.btnBenhChinh = cboNextTreatmentInstructions;
                    nextTreatmentIntructionMainRule.txtMainText = txtNextTreatmentInstructionMainText;
                    nextTreatmentIntructionMainRule.chkCheck = chkEditNextTreatmentInstruction;
                    nextTreatmentIntructionMainRule.maxLengthCode = maxLengthCode;
                    nextTreatmentIntructionMainRule.maxLengthText = maxLengthText;
                    nextTreatmentIntructionMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    nextTreatmentIntructionMainRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                    nextTreatmentIntructionMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProviderForLeftPanel.SetValidationRule(txtNextTreatmentInstructionCode, nextTreatmentIntructionMainRule);
                }
                else
                {
                    lciNextTreatmentInstructionText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtNextTreatmentInstructionCode.ErrorText = "";
                    dxValidationProviderForLeftPanel.RemoveControlError(txtNextTreatmentInstructionCode);
                    dxValidationProviderForLeftPanel.SetValidationRule(txtNextTreatmentInstructionCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task FillDataToComboNextTreatmentInst()
        {
            try
            {
                this.dataNextTreatmentInstructions = null;
                if (BackendDataWorker.IsExistsKey<HIS_NEXT_TREA_INTR>())
                {
                    this.dataNextTreatmentInstructions = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_NEXT_TREA_INTR>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    filter.IS_ACTIVE = 1;
                    this.dataNextTreatmentInstructions = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_NEXT_TREA_INTR>>("api/HisNextTreaIntr/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (this.dataNextTreatmentInstructions != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_NEXT_TREA_INTR), this.dataNextTreatmentInstructions, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                cboNextTreatmentInstructions.Properties.DataSource = this.dataNextTreatmentInstructions;
                cboNextTreatmentInstructions.Properties.DisplayMember = "NEXT_TREA_INTR_NAME";
                cboNextTreatmentInstructions.Properties.ValueMember = "ID";
                cboNextTreatmentInstructions.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboNextTreatmentInstructions.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboNextTreatmentInstructions.Properties.ImmediatePopup = true;
                cboNextTreatmentInstructions.ForceInitialize();
                cboNextTreatmentInstructions.Properties.View.Columns.Clear();
                cboNextTreatmentInstructions.Properties.PopupFormSize = new System.Drawing.Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboNextTreatmentInstructions.Properties.View.Columns.AddField("NEXT_TREA_INTR_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboNextTreatmentInstructions.Properties.View.Columns.AddField("NEXT_TREA_INTR_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cboNextTreatmentInstructions.Properties.View.Columns.AddField("NEXT_TREA_INTR_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cboNextTreatmentInstructions.Properties.View.Columns["NEXT_TREA_INTR_NAME_UNSIGN"].Width = 0;

                if (!string.IsNullOrEmpty(this.HisServiceReqView.NEXT_TREAT_INTR_CODE))
                {
                    var nextTreatmentIntruction = this.dataNextTreatmentInstructions != null ? this.dataNextTreatmentInstructions.Where(p => p.NEXT_TREA_INTR_CODE == (this.HisServiceReqView.NEXT_TREAT_INTR_CODE)).FirstOrDefault() : null;
                    if (nextTreatmentIntruction != null)
                    {
                        txtNextTreatmentInstructionCode.Text = nextTreatmentIntruction.NEXT_TREA_INTR_CODE;
                        cboNextTreatmentInstructions.EditValue = nextTreatmentIntruction.ID;
                        if (AutoCheckNextTreatmentInstruction || (!String.IsNullOrEmpty(this.HisServiceReqView.NEXT_TREATMENT_INSTRUCTION) && (this.HisServiceReqView.NEXT_TREATMENT_INSTRUCTION ?? "").Trim().ToLower() != (nextTreatmentIntruction.NEXT_TREA_INTR_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditNextTreatmentInstruction.Checked = true;
                            txtNextTreatmentInstructionMainText.Text = this.HisServiceReqView.NEXT_TREATMENT_INSTRUCTION;
                        }
                        else
                        {
                            chkEditNextTreatmentInstruction.Checked = false;
                            txtNextTreatmentInstructionMainText.Text = nextTreatmentIntruction.NEXT_TREA_INTR_NAME;
                        }
                    }
                    else
                    {
                        txtNextTreatmentInstructionCode.Text = null;
                        cboNextTreatmentInstructions.EditValue = null;
                        txtNextTreatmentInstructionMainText.Text = null;
                        chkEditNextTreatmentInstruction.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(this.HisServiceReqView.NEXT_TREATMENT_INSTRUCTION))
                {
                    chkEditNextTreatmentInstruction.Checked = true;
                    txtNextTreatmentInstructionMainText.Text = this.HisServiceReqView.NEXT_TREATMENT_INSTRUCTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
