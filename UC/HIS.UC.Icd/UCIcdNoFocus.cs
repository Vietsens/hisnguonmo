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
using HIS.UC.Icd.ADO;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.Icd.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using System.Text.RegularExpressions;
using Inventec.Desktop.CustomControl;
using Inventec.Desktop.CustomControl.NoFocus;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.UC.Icd
{
    public partial class UCIcdNoFocus : UserControl
    {
        private IcdInitADO InitAdo { get; set; }
        private int positionHandle = -1;
        private List<HIS_ICD> dataIcds;

        private DelegateRequiredCause requiredCause;
        private DelegateRefeshIcd refeshIcd;
        private DelegatNextFocus nextFocus;
        DevExpress.XtraGrid.Columns.GridColumn aColumnCode;
        string _TextIcdName = "";
        bool IsObligatoryTranferMediOrg = false;
        long autoCheckIcd;

        public UCIcdNoFocus()
        {
            InitializeComponent();

            this.SetCaptionByLanguageKey();
        }

        public UCIcdNoFocus(IcdInitADO data)
        {
            InitializeComponent();

            this.SetCaptionByLanguageKey();
            this.InitAdo = data;
            if (data.Height > 0 && data.Width > 0)
            {
                this.Size = new Size(data.Width, data.Height);
            }
            if (data.DelegateNextFocus != null)
            {
                nextFocus = data.DelegateNextFocus;
            }
            if (data.DelegateRefeshIcd != null)
            {
                refeshIcd = data.DelegateRefeshIcd;
            }
            if (data.DelegateRequiredCause != null)
            {
                requiredCause = data.DelegateRequiredCause;
            }
            if (data.DataIcds != null && data.DataIcds.Count > 0)
            {
                dataIcds = data.DataIcds.Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            if (data.IsColor)
            {
                ValidationICD(10, 500, true);
            }
            else
            {
                ValidationSingleControlWithMaxLength(txtIcdCode, false, 10);
                ValidationSingleControlWithMaxLength(txtIcdMainText, false, 500);
            }
            this.IsObligatoryTranferMediOrg = data.IsObligatoryTranferMediOrg;
            if (data.LabelTextSize > 0)
            {
                lciIcdText.TextSize = new Size(data.LabelTextSize, lciIcdText.Height);
            }
            if (data.MinSize > 0)
            {
                lciIcdText.MinSize = new Size(data.MinSize, lciIcdText.Height);
            }
            if (data.SizeText > 0)
            {
                this.txtIcdCode.Font = new Font(txtIcdCode.Font.FontFamily, data.SizeText);
                this.txtIcdMainText.Font = new Font(txtIcdCode.Font.FontFamily, data.SizeText);
                this.cboIcds.Font = new Font(txtIcdCode.Font.FontFamily, data.SizeText);
                this.chkEditIcd.Font = new Font(txtIcdCode.Font.FontFamily, data.SizeText);
                this.lciIcdText.AppearanceItemCaption.Font = new Font(this.lciIcdText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciIcdText.TextSize = new Size(200, 20);
                this.layoutControlItem3.AppearanceItemCaption.Font = new Font(this.lciIcdText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciIcdText.Size = new Size(321, 40);
            }

            this.chkEditIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            if (!String.IsNullOrEmpty(data.LblIcdMain))
            {
                this.lciIcdText.Text = data.LblIcdMain;
            }
            else
            {
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_ICD_MAIN", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }

            if (!String.IsNullOrEmpty(data.ToolTipsIcdMain))
            {
                this.lciIcdText.OptionsToolTip.ToolTip = data.ToolTipsIcdMain;
            }
        }

        private void UCIcd_Load(object sender, EventArgs e)
        {
            try
            {
                DataToComboChuanDoanTD(cboIcds);

                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                chkEditIcd.Enabled = (this.autoCheckIcd != 2);

                txtIcdCode.Focus();
                txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(IcdInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (this.InitAdo.IcdInput == null) this.InitAdo.IcdInput = new IcdInputADO();
                    this.InitAdo.IcdInput.ICD_CODE = input.ICD_CODE;
                    this.InitAdo.IcdInput.ICD_NAME = input.ICD_NAME;
                    FillDataToCboIcd();
                }
                else
                {
                    txtIcdCode.Text = null;
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = null;
                    chkEditIcd.Checked = false;
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
                txtIcdCode.Focus();
                txtIcdCode.SelectAll();
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
                if (inPut is IcdInputADO)
                {
                    if (this.InitAdo.IcdInput == null) this.InitAdo.IcdInput = new IcdInputADO();
                    this.InitAdo.IcdInput.ICD_CODE = ((IcdInputADO)inPut).ICD_CODE;
                    //this.InitAdo.IcdInput.ICD_ID = ((IcdInputADO)inPut).ICD_ID ?? 0;
                    this.InitAdo.IcdInput.ICD_NAME = ((IcdInputADO)inPut).ICD_NAME;
                    if (!string.IsNullOrEmpty(this.InitAdo.IcdInput.ICD_CODE))
                    {
                        FillDataToCboIcd();
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
                    txtIcdCode.ReadOnly = true;
                    txtIcdMainText.ReadOnly = true;
                    cboIcds.ReadOnly = true;
                    chkEditIcd.ReadOnly = true;
                    cboIcds.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtIcdCode.ReadOnly = false;
                    txtIcdMainText.ReadOnly = false;
                    cboIcds.ReadOnly = false;
                    chkEditIcd.ReadOnly = false;
                    cboIcds.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidationIcd()
        {
            bool result = true;
            try
            {
                if (InitAdo.IsColor)
                {
                    //chkEditIcd.Focus();
                    this.positionHandle = -1;
                    result = (dxValidationProvider1.Validate());
                }
                else
                {
                    //chkEditIcd.Focus();
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

        public bool ValidationIcdWithMessage(List<string> errorEmpty, List<string> errorOther)
        {
            bool result = true;
            try
            {
                if (InitAdo.IsColor)
                {
                    chkEditIcd.Focus();
                    this.positionHandle = -1;
                    result = (dxValidationProvider1.Validate());
                }
                else
                {
                    chkEditIcd.Focus();
                    this.positionHandle = -1;
                    result = (dxValidationProvider2.Validate());
                }

                if (!result)
                {
                    var invalidControls = dxValidationProvider1.GetInvalidControls();
                    if (invalidControls != null && invalidControls.Count > 0)
                    {
                        foreach (System.Windows.Forms.Control c in invalidControls)
                        {
                            string errorC = this.lciIcdText.Text.Replace(":", "");
                            string errorT = dxValidationProvider1.GetValidationRule(c).ErrorText;
                            if (errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc)
                         || errorT == Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc))
                            {
                                errorEmpty.Add(errorC);
                            }
                            else
                            {
                                errorC = String.Format("{0}: {1}", errorC, errorT);
                                errorOther.Add(errorC);
                            }
                        }

                        if (errorEmpty != null && errorEmpty.Count > 0)
                        {
                            errorEmpty = errorEmpty.Distinct().ToList();
                        }
                        if (errorOther != null && errorOther.Count > 0)
                        {
                            errorOther = errorOther.Distinct().ToList();
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

        public object GetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (txtIcdCode.ErrorText == "")
                {
                    if (chkEditIcd.Checked)
                        outPut.ICD_NAME = txtIcdMainText.Text;
                    else
                        outPut.ICD_NAME = cboIcds.Text;

                    if (!String.IsNullOrEmpty(txtIcdCode.Text))
                    {
                        outPut.ICD_CODE = txtIcdCode.Text;
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

        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdCombo(txtIcdCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMainText.Text = result.First().ICD_NAME;
                        cboIcds.EditValue = listData.First().ID;
                        chkEditIcd.Checked = (chkEditIcd.Enabled ? InitAdo.AutoCheckIcd : false);
                        //chkEditIcd.Focus();

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

                        if (this.refeshIcd != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug("this.refeshIcd.execute");
                            this.refeshIcd(listData.First());
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

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.InitAdo.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
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

        private void ChangecboChanDoanTD()
        {
            try
            {
                cboIcds.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = dataIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMainText.Text = icd.ICD_NAME;
                    chkEditIcd.Checked = (chkEditIcd.Enabled ? InitAdo.AutoCheckIcd : false);
                    if (chkEditIcd.Checked && this.nextFocus != null)
                    {
                        this.nextFocus();
                    }
                    else if (chkEditIcd.Enabled)
                    {
                        chkEditIcd.Focus();
                    }
                    else
                    {
                        if (this.nextFocus != null)
                            this.nextFocus();
                    }

                    if (this.refeshIcd != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("this.refeshIcd.execute");
                        this.refeshIcd(icd);
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
                    if (nextFocus != null) nextFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumnNoFocus cbo)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);
                List<IcdADO> listADO = new List<IcdADO>();
                foreach (var item in dataIcds)
                {
                    IcdADO icd = new IcdADO();
                    icd.ID = item.ID;
                    icd.ICD_CODE = item.ICD_CODE;
                    icd.ICD_NAME = item.ICD_NAME;
                    icd.ICD_NAME_UNSIGN = Inventec.Common.String.Convert.UnSignVNese(item.ICD_NAME);
                    listADO.Add(icd);
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
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
       
        private void FillDataToCboIcd()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.InitAdo.IcdInput.ICD_CODE))
                {
                    var icd = dataIcds.Where(p => p.ICD_CODE == (this.InitAdo.IcdInput.ICD_CODE)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCode.Text = icd.ICD_CODE;
                        cboIcds.EditValue = icd.ID;
                        if (InitAdo.AutoCheckIcd || (!String.IsNullOrEmpty(this.InitAdo.IcdInput.ICD_NAME) && (this.InitAdo.IcdInput.ICD_NAME ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditIcd.Checked = (this.autoCheckIcd != 2);
                            txtIcdMainText.Text = this.InitAdo.IcdInput.ICD_NAME;
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
                else if (!string.IsNullOrEmpty(this.InitAdo.IcdInput.ICD_NAME))
                {
                    chkEditIcd.Checked = (this.autoCheckIcd != 2);
                    txtIcdMainText.Text = this.InitAdo.IcdInput.ICD_NAME;
                }
                else
                {
                    txtIcdCode.Text = null;
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = null;
                    chkEditIcd.Checked = false;
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
                    lciIcdText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    ValidationRuleControl icdMainRule = new ValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cboIcds;
                    icdMainRule.txtMainText = txtIcdMainText;
                    icdMainRule.chkCheck = chkEditIcd;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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

        public void ValidationSingleControlWithMaxLength(Control control, bool isRequired, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule icdMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                icdMainRule.editor = control;
                icdMainRule.maxLength = maxLength;
                icdMainRule.IsRequired = isRequired;
                icdMainRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, icdMainRule);
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
                dxValidationProvider1.SetValidationRule(txtIcdCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataIcds.Where(o => o.ICD_CODE.Equals(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtIcdCode);
                        ValidationICD(10, 500, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text;
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
                    icd = this.dataIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (this.requiredCause == null)
                    return;

                if (!this.InitAdo.IsUCCause && dataIcds != null && icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1)
                    {
                        this.requiredCause(true);
                    }
                    else
                        this.requiredCause(false);
                }
                else
                {
                    this.requiredCause(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCIcdNoFocus
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus = new ResourceManager("HIS.UC.Icd.Resources.Lang", typeof(UCIcdNoFocus).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
                this.gridLookUpEdit1View.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.gridLookUpEdit1View.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.chkEditIcd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
                this.lciIcdText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.lciIcdText.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("UCIcdNoFocus.lciIcdText.Text", Resources.ResourceLanguageManager.LanguageResourceUCIcdNoFocus, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
