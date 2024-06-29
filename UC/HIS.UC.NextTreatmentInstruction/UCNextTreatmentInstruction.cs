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
using HIS.UC.NextTreatmentInstruction.ADO;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.NextTreatmentInstruction.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using System.Text.RegularExpressions;
using Inventec.Desktop.CustomControl;

namespace HIS.UC.NextTreatmentInstruction
{
    public partial class UCNextTreatmentInstruction : UserControl
    {
        private NextTreatmentInstructionInitADO InitAdo { get; set; }
        private int positionHandle = -1;
        private List<HIS_NEXT_TREA_INTR> dataNextTreatmentInstructions;

        private DelegatNextFocus nextFocus;
        DevExpress.XtraGrid.Columns.GridColumn aColumnCode;

        string _TextNextTreatmentInstructionName = "";
        bool IsObligatoryTranferMediOrg = false;

        public UCNextTreatmentInstruction()
        {
            InitializeComponent();
        }

        public UCNextTreatmentInstruction(NextTreatmentInstructionInitADO data)
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
            if (data.DataNextTreatmentInstructions != null && data.DataNextTreatmentInstructions.Count > 0)
            {
                dataNextTreatmentInstructions = data.DataNextTreatmentInstructions.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            if (data.IsColor)
            {
                ValidationICD(2, 100, true);
            }
            else
            {
                ValidationSingleControlWithMaxLength(txtNextTreatmentInstructionCode, false, 2);
                ValidationSingleControlWithMaxLength(txtNextTreatmentInstructionMainText, false, 100);
            }
            this.IsObligatoryTranferMediOrg = data.IsObligatoryTranferMediOrg;

            if (data.SizeText > 0)
            {
                this.txtNextTreatmentInstructionCode.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                this.txtNextTreatmentInstructionMainText.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                this.cboNextTreatmentInstructions.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                this.chkEditNextTreatmentInstruction.Font = new Font(txtNextTreatmentInstructionCode.Font.FontFamily, data.SizeText);
                this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font = new Font(this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciNextTreatmentInstructionText.TextSize = new Size(200, 20);
                this.layoutControlItem3.AppearanceItemCaption.Font = new Font(this.lciNextTreatmentInstructionText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciNextTreatmentInstructionText.Size = new Size(321, 40);
            }

            if (!String.IsNullOrEmpty(data.ToolTipsNextTreatmentInstructionMain))
            {
                this.lciNextTreatmentInstructionText.OptionsToolTip.ToolTip = data.ToolTipsNextTreatmentInstructionMain;
            }
        }

        private void UCNextTreatmentInstruction_Load(object sender, EventArgs e)
        {
            try
            {
                DataToComboChuanDoanTD(cboNextTreatmentInstructions);
                txtNextTreatmentInstructionCode.Focus();
                txtNextTreatmentInstructionCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(NextTreatmentInstructionInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (this.InitAdo.NextTreatmentInstructionInput == null) this.InitAdo.NextTreatmentInstructionInput = new NextTreatmentInstructionInputADO();
                    this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_CODE = input.NEXT_TREA_INTR_CODE;
                    this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME = input.NEXT_TREA_INTR_NAME;
                    FillDataToCboNextTreatmentInstruction();
                }
                else
                {
                    txtNextTreatmentInstructionCode.Text = null;
                    cboNextTreatmentInstructions.EditValue = null;
                    txtNextTreatmentInstructionMainText.Text = null;
                    chkEditNextTreatmentInstruction.Checked = false;
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
                txtNextTreatmentInstructionCode.Focus();
                txtNextTreatmentInstructionCode.SelectAll();
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
                if (inPut is NextTreatmentInstructionInputADO)
                {
                    if (this.InitAdo.NextTreatmentInstructionInput == null) this.InitAdo.NextTreatmentInstructionInput = new NextTreatmentInstructionInputADO();
                    this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_CODE = ((NextTreatmentInstructionInputADO)inPut).NEXT_TREA_INTR_CODE;
                    //this.InitAdo.NextTreatmentInstructionInput.ICD_ID = ((NextTreatmentInstructionInputADO)inPut).ICD_ID ?? 0;
                    this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME = ((NextTreatmentInstructionInputADO)inPut).NEXT_TREA_INTR_NAME;
                    if (!string.IsNullOrEmpty(this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_CODE))
                    {
                        FillDataToCboNextTreatmentInstruction();
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
                    txtNextTreatmentInstructionCode.ReadOnly = true;
                    txtNextTreatmentInstructionMainText.ReadOnly = true;
                    cboNextTreatmentInstructions.ReadOnly = true;
                    chkEditNextTreatmentInstruction.ReadOnly = true;
                    cboNextTreatmentInstructions.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtNextTreatmentInstructionCode.ReadOnly = false;
                    txtNextTreatmentInstructionMainText.ReadOnly = false;
                    cboNextTreatmentInstructions.ReadOnly = false;
                    chkEditNextTreatmentInstruction.ReadOnly = false;
                    cboNextTreatmentInstructions.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidationNextTreatmentInstruction()
        {
            bool result = true;
            try
            {
                if (InitAdo.IsColor)
                {
                    chkEditNextTreatmentInstruction.Focus();
                    this.positionHandle = -1;
                    result = (dxValidationProvider1.Validate());
                }
                else
                {
                    chkEditNextTreatmentInstruction.Focus();
                    this.positionHandle = -1;
                    result = (dxValidationProvider2.Validate());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public object GetValue()
        {
            object result = null;
            try
            {
                NextTreatmentInstructionInputADO outPut = new NextTreatmentInstructionInputADO();
                if (txtNextTreatmentInstructionCode.ErrorText == "")
                {
                    if (chkEditNextTreatmentInstruction.Checked)
                        outPut.NEXT_TREA_INTR_NAME = txtNextTreatmentInstructionMainText.Text;
                    else
                        outPut.NEXT_TREA_INTR_NAME = cboNextTreatmentInstructions.Text;

                    if (!String.IsNullOrEmpty(txtNextTreatmentInstructionCode.Text))
                    {
                        outPut.NEXT_TREA_INTR_CODE = txtNextTreatmentInstructionCode.Text;
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

        private void txtNextTreatmentInstructionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadNextTreatmentInstructionCombo(txtNextTreatmentInstructionCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadNextTreatmentInstructionCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = dataNextTreatmentInstructions.Where(o => o.NEXT_TREA_INTR_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NEXT_TREA_INTR_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtNextTreatmentInstructionCode.Text = result.First().NEXT_TREA_INTR_CODE;
                        txtNextTreatmentInstructionMainText.Text = result.First().NEXT_TREA_INTR_NAME;
                        cboNextTreatmentInstructions.EditValue = listData.First().ID;
                        chkEditNextTreatmentInstruction.Checked = InitAdo.AutoCheckNextTreatmentInstruction;
                        //chkEditNextTreatmentInstruction.Focus();

                        if (chkEditNextTreatmentInstruction.Checked)
                        {
                            txtNextTreatmentInstructionMainText.Focus();
                            txtNextTreatmentInstructionMainText.SelectAll();
                        }
                        else
                        {
                            cboNextTreatmentInstructions.Focus();
                            cboNextTreatmentInstructions.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboNextTreatmentInstructions.Focus();
                    cboNextTreatmentInstructions.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNextTreatmentInstructionMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNextTreatmentInstruction.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboNextTreatmentInstructions.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.InitAdo.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextNextTreatmentInstructionName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextNextTreatmentInstructionName);
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
                cboNextTreatmentInstructions.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_NEXT_TREA_INTR nextTreatmentIntruction = dataNextTreatmentInstructions.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNextTreatmentInstructions.EditValue ?? 0).ToString()));
                if (nextTreatmentIntruction != null)
                {
                    txtNextTreatmentInstructionCode.Text = nextTreatmentIntruction.NEXT_TREA_INTR_CODE;
                    txtNextTreatmentInstructionMainText.Text = nextTreatmentIntruction.NEXT_TREA_INTR_NAME;
                    chkEditNextTreatmentInstruction.Checked = InitAdo.AutoCheckNextTreatmentInstruction;
                    if (chkEditNextTreatmentInstruction.Checked && this.nextFocus != null)
                    {
                        this.nextFocus();
                    }
                    else
                    {
                        chkEditNextTreatmentInstruction.Focus();
                    }
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
                this.chkEditNextTreatmentInstruction.Enabled = true;
                this.chkEditNextTreatmentInstruction.Checked = true;
                this.txtNextTreatmentInstructionMainText.Text = text;
                this.txtNextTreatmentInstructionMainText.Focus();
                this.txtNextTreatmentInstructionMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboNextTreatmentInstructions.ClosePopup();
                    cboNextTreatmentInstructions.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboNextTreatmentInstructions.ClosePopup();
                    if (cboNextTreatmentInstructions.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboNextTreatmentInstructions.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNextTreatmentInstruction_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNextTreatmentInstruction.Checked == true)
                {
                    cboNextTreatmentInstructions.Visible = false;
                    txtNextTreatmentInstructionMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtNextTreatmentInstructionMainText.Text = this._TextNextTreatmentInstructionName;
                    else
                        txtNextTreatmentInstructionMainText.Text = cboNextTreatmentInstructions.Text;
                    txtNextTreatmentInstructionMainText.Focus();
                    txtNextTreatmentInstructionMainText.SelectAll();
                }
                else if (chkEditNextTreatmentInstruction.Checked == false)
                {
                    txtNextTreatmentInstructionMainText.Visible = false;
                    cboNextTreatmentInstructions.Visible = true;
                    txtNextTreatmentInstructionMainText.Text = cboNextTreatmentInstructions.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNextTreatmentInstruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNextTreatmentInstructionMainText.Text != null)
                    {
                        //this.data.DelegateRefeshNextTreatmentInstructionMainText(txtNextTreatmentInstructionMainText.Text);
                    }
                    if (cboNextTreatmentInstructions.EditValue != null)
                    {
                        //var hisNextTreatmentInstruction = BackendDataWorker.Get<HIS_NEXT_TREA_INTR>().Where(p => p.ID == (long)cboNextTreatmentInstructions.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshNextTreatmentInstruction(hisNextTreatmentInstruction);
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
                //columnInfos.Add(new ColumnInfo("NEXT_TREA_INTR_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("NEXT_TREA_INTR_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("NEXT_TREA_INTR_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataNextTreatmentInstructions, controlEditorADO);
                List<NextTreatmentInstructionADO> listADO = new List<NextTreatmentInstructionADO>();
                foreach (var item in dataNextTreatmentInstructions)
                {
                    NextTreatmentInstructionADO nextTreatmentIntruction = new NextTreatmentInstructionADO();
                    nextTreatmentIntruction.ID = item.ID;
                    nextTreatmentIntruction.NEXT_TREA_INTR_CODE = item.NEXT_TREA_INTR_CODE;
                    nextTreatmentIntruction.NEXT_TREA_INTR_NAME = item.NEXT_TREA_INTR_NAME;
                    nextTreatmentIntruction.NEXT_TREA_INTR_NAME_UNSIGN = convertToUnSign3(item.NEXT_TREA_INTR_NAME);
                    listADO.Add(nextTreatmentIntruction);
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "NEXT_TREA_INTR_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                aColumnCode = cbo.Properties.View.Columns.AddField("NEXT_TREA_INTR_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("NEXT_TREA_INTR_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("NEXT_TREA_INTR_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["NEXT_TREA_INTR_NAME_UNSIGN"].Width = 0;
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

        private void FillDataToCboNextTreatmentInstruction()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_CODE))
                {
                    var nextTreatmentIntruction = dataNextTreatmentInstructions.Where(p => p.NEXT_TREA_INTR_CODE == (this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_CODE)).FirstOrDefault();
                    if (nextTreatmentIntruction != null)
                    {
                        txtNextTreatmentInstructionCode.Text = nextTreatmentIntruction.NEXT_TREA_INTR_CODE;
                        cboNextTreatmentInstructions.EditValue = nextTreatmentIntruction.ID;
                        if (InitAdo.AutoCheckNextTreatmentInstruction || (!String.IsNullOrEmpty(this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME) && (this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME ?? "").Trim().ToLower() != (nextTreatmentIntruction.NEXT_TREA_INTR_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditNextTreatmentInstruction.Checked = true;
                            txtNextTreatmentInstructionMainText.Text = this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME;
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
                else if (!string.IsNullOrEmpty(this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME))
                {
                    chkEditNextTreatmentInstruction.Checked = true;
                    txtNextTreatmentInstructionMainText.Text = this.InitAdo.NextTreatmentInstructionInput.NEXT_TREA_INTR_NAME;
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

        public void ValidationICD(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciNextTreatmentInstructionText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ValidationRuleControl nextTreatmentIntructionMainRule = new ValidationRuleControl();
                    nextTreatmentIntructionMainRule.txtNextTreatmentInstructionCode = txtNextTreatmentInstructionCode;
                    nextTreatmentIntructionMainRule.btnBenhChinh = cboNextTreatmentInstructions;
                    nextTreatmentIntructionMainRule.txtMainText = txtNextTreatmentInstructionMainText;
                    nextTreatmentIntructionMainRule.chkCheck = chkEditNextTreatmentInstruction;
                    nextTreatmentIntructionMainRule.maxLengthCode = maxLengthCode;
                    nextTreatmentIntructionMainRule.maxLengthText = maxLengthText;
                    nextTreatmentIntructionMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    nextTreatmentIntructionMainRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                    nextTreatmentIntructionMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtNextTreatmentInstructionCode, nextTreatmentIntructionMainRule);
                }
                else
                {
                    lciNextTreatmentInstructionText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtNextTreatmentInstructionCode.ErrorText = "";
                    dxValidationProvider1.RemoveControlError(txtNextTreatmentInstructionCode);
                    dxValidationProvider1.SetValidationRule(txtNextTreatmentInstructionCode, null);
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
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule nextTreatmentIntructionMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                nextTreatmentIntructionMainRule.editor = control;
                nextTreatmentIntructionMainRule.maxLength = maxLength;
                nextTreatmentIntructionMainRule.IsRequired = isRequired;
                nextTreatmentIntructionMainRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, nextTreatmentIntructionMainRule);
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
                lciNextTreatmentInstructionText.AppearanceItemCaption.ForeColor = Color.Black;
                dxValidationProvider1.SetValidationRule(txtNextTreatmentInstructionCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNextTreatmentInstructionCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.NextTreatmentInstructionKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNextTreatmentInstructionCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataNextTreatmentInstructions.Where(o => o.NEXT_TREA_INTR_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NEXT_TREA_INTR_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtNextTreatmentInstructionCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtNextTreatmentInstructionCode);
                        ValidationICD(10, 500,true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNextTreatmentInstructions_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (!cboNextTreatmentInstructions.Properties.Buttons[1].Visible)
                    return;
                this._TextNextTreatmentInstructionName = "";
                cboNextTreatmentInstructions.EditValue = null;
                txtNextTreatmentInstructionCode.Text = "";
                txtNextTreatmentInstructionMainText.Text = "";
                cboNextTreatmentInstructions.Properties.Buttons[1].Visible = false;
            }
        }

        private void cboNextTreatmentInstructions_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboNextTreatmentInstructions.Text))
                {
                    cboNextTreatmentInstructions.EditValue = null;
                    txtNextTreatmentInstructionMainText.Text = "";
                    chkEditNextTreatmentInstruction.Checked = false;
                }
                else
                {
                    this._TextNextTreatmentInstructionName = cboNextTreatmentInstructions.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboNextTreatmentInstructions.EditValue != null)
                //    this.ChangecboChanDoanTD();
                //else if (this.IsObligatoryTranferMediOrg)
                //{
                //    if (!string.IsNullOrEmpty(cboNextTreatmentInstructions.Text))
                //        this._TextNextTreatmentInstructionName = cboNextTreatmentInstructions.Text;
                //    this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextNextTreatmentInstructionName);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
