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
using HIS.UC.National.ADO;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.UC.National.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using System.Text.RegularExpressions;
using Inventec.Desktop.CustomControl;

namespace HIS.UC.National
{
    public partial class UCNational : UserControl
    {
        private NationalInitADO InitAdo { get; set; }
        //internal TextEdit txtEditFocus;
        private int positionHandle = -1;
        private List<SDA_NATIONAL> dataNationals;

        private DelegatNextFocus nextFocus;
        DevExpress.XtraGrid.Columns.GridColumn aColumnCode;

        string _TextNationalName = "";

        public UCNational()
        {
            InitializeComponent();
        }

        public UCNational(NationalInitADO data)
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
            if (data.DataNationals != null && data.DataNationals.Count > 0)
            {
                dataNationals = data.DataNationals.Where(p => p.IS_ACTIVE == 1).ToList();
            }

            ValidationNATIONAL();

            if (data.SizeText > 0)
            {
                this.txtNationalCode.Font = new Font(txtNationalCode.Font.FontFamily, data.SizeText);
                this.txtNationalMainText.Font = new Font(txtNationalCode.Font.FontFamily, data.SizeText);
                this.cboNationals.Font = new Font(txtNationalCode.Font.FontFamily, data.SizeText);
                this.chkEditNational.Font = new Font(txtNationalCode.Font.FontFamily, data.SizeText);
                this.lciNationalText.AppearanceItemCaption.Font = new Font(this.lciNationalText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciNationalText.TextSize = new Size(200, 20);
                this.layoutControlItem3.AppearanceItemCaption.Font = new Font(this.lciNationalText.AppearanceItemCaption.Font.FontFamily, data.SizeText);
                this.lciNationalText.Size = new Size(321, 40);
            }

            this.chkEditNational.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_NATIONAL__LCI_CHECK_EDIT_NATIONAL", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            if (!String.IsNullOrEmpty(data.LblNationalMain))
            {
                this.lciNationalText.Text = data.LblNationalMain;
            }
            else
            {
                this.lciNationalText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_NATIONAL__LCI_NATIONAL_MAIN", Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }

        }

        private void UCNational_Load(object sender, EventArgs e)
        {
            try
            {
                DataToComboChuanDoanTD(cboNationals);
                txtNationalCode.Focus();
                txtNationalCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(NationalInputADO input)
        {
            try
            {
                if (input != null)
                {
                    if (this.InitAdo.NationalInput == null) this.InitAdo.NationalInput = new NationalInputADO();
                    this.InitAdo.NationalInput.NATIONAL_CODE = input.NATIONAL_CODE;
                    this.InitAdo.NationalInput.NATIONAL_NAME = input.NATIONAL_NAME;
                    FillDataToCboNational();
                }
                else
                {
                    txtNationalCode.Text = null;
                    cboNationals.EditValue = null;
                    txtNationalMainText.Text = null;
                    chkEditNational.Checked = false;
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
                txtNationalCode.Focus();
                txtNationalCode.SelectAll();
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
                if (inPut is NationalInputADO)
                {
                    if (this.InitAdo.NationalInput == null) this.InitAdo.NationalInput = new NationalInputADO();
                    this.InitAdo.NationalInput.NATIONAL_CODE = ((NationalInputADO)inPut).NATIONAL_CODE;
                    //this.InitAdo.NationalInput.NATIONAL_ID = ((NationalInputADO)inPut).NATIONAL_ID ?? 0;
                    this.InitAdo.NationalInput.NATIONAL_NAME = ((NationalInputADO)inPut).NATIONAL_NAME;
                    if (!string.IsNullOrEmpty(this.InitAdo.NationalInput.NATIONAL_CODE))
                    {
                        FillDataToCboNational();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(((NationalInputADO)inPut).NATIONAL_NAME))
                        {
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = this.InitAdo.NationalInput.NATIONAL_NAME = ((NationalInputADO)inPut).NATIONAL_NAME;
                        }
                        else
                        {
                            chkEditNational.Checked = false;
                        }
                    }
                }
                else
                {
                    txtNationalCode.Text = null;
                    cboNationals.EditValue = null;
                    txtNationalMainText.Text = null;
                    chkEditNational.Checked = false;
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
                    txtNationalCode.ReadOnly = true;
                    txtNationalMainText.ReadOnly = true;
                    cboNationals.ReadOnly = true;
                    chkEditNational.ReadOnly = true;
                    cboNationals.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    txtNationalCode.ReadOnly = false;
                    txtNationalMainText.ReadOnly = false;
                    cboNationals.ReadOnly = false;
                    chkEditNational.ReadOnly = false;
                    cboNationals.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidationNational()
        {
            bool result = true;
            try
            {
                this.positionHandle = -1;
                result = (dxValidationProvider1.Validate());
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
                NationalInputADO outPut = new NationalInputADO();
                if (txtNationalCode.ErrorText == "")
                {
                    if (chkEditNational.Checked)
                        outPut.NATIONAL_NAME = txtNationalMainText.Text;
                    else
                        outPut.NATIONAL_NAME = cboNationals.Text;

                    if (!String.IsNullOrEmpty(txtNationalCode.Text))
                    {
                        outPut.NATIONAL_CODE = txtNationalCode.Text;
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

        private void txtNationalCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadNationalCombo(txtNationalCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadNationalCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = dataNationals.Where(o => o.NATIONAL_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NATIONAL_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtNationalCode.Text = result.First().NATIONAL_CODE;
                        txtNationalMainText.Text = result.First().NATIONAL_NAME;
                        cboNationals.EditValue = listData.First().ID;
                        chkEditNational.Checked = InitAdo.AutoCheckNational;
                        //chkEditNational.Focus();

                        if (chkEditNational.Checked)
                        {
                            txtNationalMainText.Focus();
                            txtNationalMainText.SelectAll();
                        }
                        else
                        {
                            cboNationals.Focus();
                            cboNationals.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboNationals.Focus();
                    cboNationals.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNationalMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNational.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationals_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboNationals.EditValue != null)
                        this.ChangecboChanDoanTD();
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
                cboNationals.Properties.Buttons[1].Visible = true;
                SDA.EFMODEL.DataModels.SDA_NATIONAL national = dataNationals.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNationals.EditValue ?? 0).ToString()));
                if (national != null)
                {
                    txtNationalCode.Text = national.NATIONAL_CODE;
                    txtNationalMainText.Text = national.NATIONAL_NAME;
                    chkEditNational.Checked = InitAdo.AutoCheckNational;
                    if (chkEditNational.Checked && this.nextFocus != null)
                    {
                        this.nextFocus();
                    }
                    else
                    {
                        chkEditNational.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanNATIONALNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                this.chkEditNational.Enabled = true;
                this.chkEditNational.Checked = true;
                this.txtNationalMainText.Text = text;
                this.txtNationalMainText.Focus();
                this.txtNationalMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationals_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboNationals.ClosePopup();
                    cboNationals.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboNationals.ClosePopup();
                    if (cboNationals.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                else
                    cboNationals.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNational.Checked == true)
                {
                    cboNationals.Visible = false;
                    txtNationalMainText.Visible = true;
                    txtNationalMainText.Text = cboNationals.Text;
                    txtNationalMainText.Focus();
                    txtNationalMainText.SelectAll();
                }
                else if (chkEditNational.Checked == false)
                {
                    txtNationalMainText.Visible = false;
                    cboNationals.Visible = true;
                    txtNationalMainText.Text = cboNationals.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNational_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNationalMainText.Text != null)
                    {
                        //this.data.DelegateRefeshNationalMainText(txtNationalMainText.Text);
                    }
                    if (cboNationals.EditValue != null)
                    {
                        //var hisNational = BackendDataWorker.Get<SDA_NATIONAL>().Where(p => p.ID == (long)cboNationals.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshNational(hisNational);
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
                //columnInfos.Add(new ColumnInfo("NATIONAL_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("NATIONAL_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("NATIONAL_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataNationals, controlEditorADO);
                List<NationalADO> listADO = new List<NationalADO>();
                foreach (var item in dataNationals)
                {
                    NationalADO national = new NationalADO();
                    national.ID = item.ID;
                    national.NATIONAL_CODE = item.NATIONAL_CODE;
                    national.NATIONAL_NAME = item.NATIONAL_NAME;
                    national.NATIONAL_NAME_UNSIGN = convertToUnSign3(item.NATIONAL_NAME);
                    listADO.Add(national);
                }

                cbo.Properties.DataSource = listADO;
                cbo.Properties.DisplayMember = "NATIONAL_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 250);

                aColumnCode = cbo.Properties.View.Columns.AddField("NATIONAL_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("NATIONAL_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("NATIONAL_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["NATIONAL_NAME_UNSIGN"].Width = 0;
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

        private void FillDataToCboNational()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.InitAdo.NationalInput.NATIONAL_CODE))
                {
                    var national = dataNationals.Where(p => p.NATIONAL_CODE == (this.InitAdo.NationalInput.NATIONAL_CODE)).FirstOrDefault();
                    if (national != null)
                    {
                        txtNationalCode.Text = national.NATIONAL_CODE;
                        cboNationals.EditValue = national.ID;
                        if (InitAdo.AutoCheckNational || (!String.IsNullOrEmpty(this.InitAdo.NationalInput.NATIONAL_NAME) && (this.InitAdo.NationalInput.NATIONAL_NAME ?? "").Trim().ToLower() != (national.NATIONAL_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditNational.Checked = true;
                            txtNationalMainText.Text = this.InitAdo.NationalInput.NATIONAL_NAME;
                        }
                        else
                        {
                            chkEditNational.Checked = false;
                            txtNationalMainText.Text = national.NATIONAL_NAME;
                        }
                    }
                    else
                    {
                        txtNationalCode.Text = null;
                        cboNationals.EditValue = null;
                        txtNationalMainText.Text = null;
                        chkEditNational.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(this.InitAdo.NationalInput.NATIONAL_NAME))
                {
                    chkEditNational.Checked = true;
                    txtNationalMainText.Text = this.InitAdo.NationalInput.NATIONAL_NAME;
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

        public void ValidationNATIONAL()
        {
            try
            {
                //lciNationalText.AppearanceItemCaption.ForeColor = Color.Maroon;

                ValidationRuleControl nationalMainRule = new ValidationRuleControl();
                nationalMainRule.txtMainText = txtNationalMainText;
                nationalMainRule.chkCheck = chkEditNational;
                nationalMainRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtNationalCode, nationalMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetValidationNATIONAL()
        {
            try
            {
                lciNationalText.AppearanceItemCaption.ForeColor = Color.Black;
                dxValidationProvider1.SetValidationRule(txtNationalCode, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNationalCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.NationalKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNationalCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text;
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataNationals.Where(o => o.NATIONAL_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NATIONAL_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtNationalCode.ErrorText = "";
                        dxValidationProvider1.RemoveControlError(txtNationalCode);
                        //ValidationNATIONAL();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNationals_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (!cboNationals.Properties.Buttons[1].Visible)
                    return;
                this._TextNationalName = "";
                cboNationals.EditValue = null;
                txtNationalCode.Text = "";
                txtNationalMainText.Text = "";
                cboNationals.Properties.Buttons[1].Visible = false;
            }
        }

        private void cboNationals_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboNationals.Text))
                {
                    cboNationals.EditValue = null;
                    txtNationalMainText.Text = "";
                    chkEditNational.Checked = false;
                }
                else
                {
                    this._TextNationalName = cboNationals.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNationals_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (cboNationals.EditValue != null)
                //    this.ChangecboChanDoanTD();
                //else if (this.IsObligatoryTranferMediOrg)
                //{
                //    if (!string.IsNullOrEmpty(cboNationals.Text))
                //        this._TextNationalName = cboNationals.Text;
                //    this.ChangecboChanDoanTD_V2_GanNATIONALNAME(this._TextNationalName);
                //}


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
