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
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Core;
using Inventec.Common.Adapter;

namespace HIS.UC.Death
{
    public partial class UCDeath : UserControl
    {
        private List<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE> HisDeathCauses;
        private List<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN> HisDeathWithins;
        private List<MOS.EFMODEL.DataModels.V_HIS_DEATH_CERT_BOOK> HisDeathCertBooks;
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment;
        private HIS_PATIENT hisPatient;
        private DelegateNextFocus NextFocus;
        private int positionHandle = -1;
        private string BranchName = null;
        CheckEdit[] ArrCheckEdit { get; set; }
        public UCDeath()
        {
            ArrCheckEdit = new CheckEdit[] { chkAupopsyY, chkAupopsyN, chkAupopsyYN };
            InitializeComponent();
        }

        public UCDeath(ADO.DeathInitADO data)
            : this()
        {
            try
            {
                InitComboUser();
                ArrCheckEdit = new CheckEdit[] { chkAupopsyY, chkAupopsyN, chkAupopsyYN };
                if (data != null)
                {
                    BranchName = data.BranchName;
                    SetTextHolder(data.IsTextHolder);
                    if (data.DeathDataSource != null)
                    {
                        if(data.DeathDataSource.CurrentHisTreatment != null)
                        {
                            hisPatient = GetPatientByID(data.DeathDataSource.CurrentHisTreatment.PATIENT_ID);
                        }
                        FillCurrentData(data.DeathDataSource);
                    }
                    else
                    {
                        FillDataToForm(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private HIS_PATIENT GetPatientByID(long patientId)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (patients != null && patients.Count() > 0)
                {
                    result = patients.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
        private void LoadComboDeathCertBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEATH_CERT_BOOK_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("DEATH_CERT_BOOK_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEATH_CERT_BOOK_NAME", "ID", columnInfos, true, 350);
                ControlEditorLoader.Load(cboDeathCertBookFirst, BackendDataWorker.Get<HIS_DEATH_CERT_BOOK>().ToList(), controlEditorADO);
                cboDeathCertBookFirst.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTextHolder(bool p)
        {
            try
            {
                lciDeathAnatomize.TextVisible = !p;
                lciDeathCauseDetail.TextVisible = !p;

                txtDeathAnatomize.Properties.NullValuePromptShowForEmptyValue = p;
                txtDeathAnatomize.Properties.ShowNullValuePromptWhenFocused = p;
                txtDeathCauseDetail.Properties.NullValuePromptShowForEmptyValue = p;
                txtDeathCauseDetail.Properties.ShowNullValuePromptWhenFocused = p;

                txtDeathAnatomize.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_ANATOMIZE") : "";
                txtDeathCauseDetail.Properties.NullValuePrompt = p ? GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_CAUSE_DETAIL") : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCDeath_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeysFromlanguage();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                //layout
                this.lciDeathAnatomize.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_ANATOMIZE");
                this.lciDeathCause.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_CAUSE");
                this.lciDeathCauseDetail.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_CAUSE_DETAIL");
                this.lciDeathTime.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_TIME");
                this.lciDeathWithin.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__LCI_DEATH_WITHIN");
                this.lciChkAupopsy.Text = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_DEATH__CHK_AUPOPSY");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.LanguageUCDeath, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void ValidateForm()
        {
            try
            {
                ValidationDeathCause();
                ValidationDeathWithin();
                ValidationDeathTime();
                ValidationDeathIssDate();
                ValidationMaxLength(txtDeathAnatomize, 3000);
                ValidationDeathCauseDetail(txtDeathCauseDetail, 3000);
                ValidationDeathCertBook();
                ValidationSo();
                ValidationNgayCap();
                ValidationRelativePatient();
                ValidationDeathCauseDetail();
                ValidationNoiCap();
                ValidationLoaiGiayTo();
                ValidationLoginName();
                ValidationCheckBox();
                ValidationFormatText();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        

        private async Task InitComboUser()
        {
            try
            {
                List<HIS_EMPLOYEE> datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EMPLOYEE>();

                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TDL_USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationDeathCauseDetail(MemoEdit memoEdit, int? maxLength)
        {
            try
            {
                ValidateMaxLengthAndRequire maxLengthValid = new ValidateMaxLengthAndRequire();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                maxLengthValid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(MemoEdit memoEdit, int? maxLength)
        {
            try
            {
                ValidateMaxLength maxLengthValid = new ValidateMaxLength();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                maxLengthValid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationDeathCause()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.cboDeathCause;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboDeathCause, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationDeathWithin()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.cboDeathWithin;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboDeathWithin, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationDeathTime()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.dtDeathTime;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.dtDeathTime, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationDeathIssDate()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.dteDeathIssDate;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.dteDeathIssDate, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationDeathCertBook()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.cboDeathCertBook;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboDeathCertBook, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationSo()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.txtSo;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtSo, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationNgayCap()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.dtNgayCap;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.dtNgayCap, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationRelativePatient()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.txtRelativePatient;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtRelativePatient, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationDeathCauseDetail()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.txtDeathCauseDetail;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtDeathCauseDetail, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationNoiCap()
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = this.txtNoiCap;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtNoiCap, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationLoaiGiayTo()
        {
            try
            {
                ValidateCombobox validate = new ValidateCombobox();
                validate.edit = this.cboLoaiGiayTo;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.cboLoaiGiayTo, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationLoginName()
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validate = new GridLookupEditWithTextEditValidationRule();
                validate.txtTextEdit = txtLoginName;
                validate.cbo = cboUser;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtLoginName, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationCheckBox()
        {
            try
            {
                ValidateCheckBox validate = new ValidateCheckBox();
                validate.chk1 = chkOne;
                validate.chk2 = chkTwo;
                validate.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.chkOne, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationFormatText()
        {
            try
            {
                ValidationFormatText validate = new ValidationFormatText();
                validate.txtEdit = this.txtDeathCertNumFirst;
                validate.ErrorText = Resources.ResourceMessage.SaiDinhDangChiChoPhepNhapSo;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(this.txtDeathCertNumFirst, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDeathCause_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && !cboDeathCause.ReadOnly)
                {
                    cboDeathCause.EditValue = null;
                    cboDeathCause.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathCause_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDeathCause.EditValue != null && HisDeathCauses != null)
                    {
                        var data = HisDeathCauses.SingleOrDefault(o => o.ID == (long)(cboDeathCause.EditValue ?? 0));
                        if (data != null)
                        {
                            cboDeathCause.Properties.Buttons[1].Visible = true;
                            cboDeathWithin.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDeathCause.EditValue != null && HisDeathCauses != null)
                    {
                        var data = HisDeathCauses.SingleOrDefault(o => o.ID == (long)(cboDeathCause.EditValue ?? 0));
                        if (data != null)
                        {
                            cboDeathCause.Properties.Buttons[1].Visible = true;
                            cboDeathWithin.Focus();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboDeathCause.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void cboDeathWithin_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && !cboDeathWithin.ReadOnly)
                {
                    cboDeathWithin.EditValue = null;
                    cboDeathWithin.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathWithin_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDeathWithin.EditValue != null && HisDeathWithins != null)
                    {
                        var data = HisDeathWithins.SingleOrDefault(o => o.ID == (long)(cboDeathWithin.EditValue ?? 0));
                        if (data != null)
                        {
                            cboDeathWithin.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDeathWithin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDeathWithin.EditValue != null && HisDeathWithins != null)
                    {
                        var data = HisDeathWithins.SingleOrDefault(o => o.ID == (long)(cboDeathWithin.EditValue ?? 0));
                        if (data != null)
                        {
                            cboDeathWithin.Properties.Buttons[1].Visible = true;
                            dtDeathTime.Focus();
                            dtDeathTime.ShowPopup();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboDeathWithin.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDeathTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtDeathTime.DateTime != null)
                    {
                        chkAupopsyY.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDeathTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtDeathTime.DateTime != null)
                    {
                        chkAupopsyY.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAupopsy_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoiTuVong.Focus();
                    txtNoiTuVong.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDeathAnatomize_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (NextFocus != null)
                    {
                        NextFocus();
                    }
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
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void LoadDataToCombo()
        {
            try
            {
                string ma = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA");
                string ten = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN");
                LoadDataGridLookUpEdit(cboDeathCause, "DEATH_CAUSE_CODE", ma, "DEATH_CAUSE_NAME", ten, "ID", HisDeathCauses);
                LoadDataGridLookUpEdit(cboDeathWithin, "DEATH_WITHIN_CODE", ma, "DEATH_WITHIN_NAME", ten, "ID", HisDeathWithins);
                LoadDataGridLookUpEdit(cboDeathCertBook, "DEATH_CERT_BOOK_CODE", ma, "DEATH_CERT_BOOK_NAME", ten, "ID", HisDeathCertBooks);
                LoadComboDeathCertBook();
                if (HisDeathCertBooks != null && HisDeathCertBooks.Count() > 0)
                    cboDeathCertBook.EditValue = HisDeathCertBooks.OrderBy(o => o.ID).FirstOrDefault().ID;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridLookUpEdit(DevExpress.XtraEditors.GridLookUpEdit comboEdit, string code, string captionCode, string name, string captionName, string value, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(code, captionCode, 100, 1));
                columnInfos.Add(new ColumnInfo(name, captionName, 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, value, columnInfos, false, 300);
                ControlEditorLoader.Load(comboEdit, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillCurrentData(ADO.DeathDataSourcesADO input)
        {
            try
            {
                if (input != null)
                {
                    this.NextFocus = input.DelegateNextFocus;
                    this.HisDeathCauses = input.HisDeathCauses;
                    this.HisDeathWithins = input.HisDeathWithins;
                    this.HisDeathCertBooks = input.HisDeathCertBooks;
                    LoadDataToCombo();

                    if (input.CurrentHisTreatment != null)
                    {
                        this.hisTreatment = input.CurrentHisTreatment;
                        FillDataToForm(hisTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToForm(HIS_TREATMENT hisTreatment)
        {
            try
            {
                if (hisTreatment != null)
                {
                    MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE deathCause = HisDeathCauses.FirstOrDefault(o => o.ID == hisTreatment.DEATH_CAUSE_ID);
                    if (deathCause != null)
                    {
                        cboDeathCause.EditValue = deathCause.ID;
                    }
                    else
                    {
                        cboDeathCause.EditValue = null;
                    }
                    txtNoiTuVong.Text = BranchName;
                    MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN deathWithin = HisDeathWithins.FirstOrDefault(o => o.ID == hisTreatment.DEATH_WITHIN_ID);
                    if (deathWithin != null)
                    {
                        cboDeathWithin.EditValue = deathWithin.ID;
                    }
                    else
                    {
                        cboDeathWithin.EditValue = null;
                    }

                    if (hisTreatment.IS_HAS_AUPOPSY != null)
                    {
                        ArrCheckEdit[(hisTreatment.IS_HAS_AUPOPSY ?? 0)- 1].Checked = true;
                    }
                    else
                    {
                        ArrCheckEdit.Last().Checked = true;
                    }

                    txtDeathAnatomize.Text = hisTreatment.SURGERY;
                    txtDeathCauseDetail.Text = hisTreatment.MAIN_CAUSE;

                    if (hisTreatment.DEATH_TIME.HasValue)
                    {
                        dtDeathTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.DEATH_TIME.Value) ?? new DateTime();
                    }
                    else
                    {
                        dtDeathTime.DateTime = DateTime.Now;
                    }
                    if (hisTreatment.DEATH_ISSUED_DATE.HasValue)
                    {
                        dteDeathIssDate.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.DEATH_ISSUED_DATE.Value) ?? new DateTime();
                    }
                    cboDeathCertBook.EditValue = hisTreatment.DEATH_CERT_BOOK_ID;
                    if (cboDeathCertBook.EditValue != null)
                        cboDeathCertBook.ReadOnly = true;
                    else
                        cboDeathCertBook.ReadOnly = false;
                    cboDeathCertBookFirst.EditValue = hisTreatment.DEATH_CERT_BOOK_FIRST_ID;
                    txtDeathCertNumFirst.Text = hisTreatment.DEATH_CERT_NUM_FIRST != null ? hisTreatment.DEATH_CERT_NUM_FIRST.ToString() : "";
                    if (!string.IsNullOrEmpty(hisTreatment.DEATH_CERT_ISSUER_LOGINNAME))
                    {
                        txtLoginName.Text = hisTreatment.DEATH_CERT_ISSUER_LOGINNAME;
                        cboUser.EditValue = hisTreatment.DEATH_CERT_ISSUER_LOGINNAME;
                    }
                    else
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        txtLoginName.Text = loginName;
                        cboUser.EditValue = loginName;
                    }
                    cboLoaiGiayTo.SelectedIndex = -1;
                    txtLoaiGiayTo.Text = null;
                    txtNoiCap.Text = null;
                    txtSo.Text = null;
                    dtNgayCap.EditValue = null;
                    int index = (hisTreatment.DEATH_DOCUMENT_TYPE_CODE ?? 0) - 1;
                    string num = null;
                    string place = null;
                    DateTime? date = null;
                    if (hisTreatment.DEATH_TIME != null)
                    {
                        num = hisTreatment.DEATH_DOCUMENT_NUMBER;
                        place = hisTreatment.DEATH_DOCUMENT_PLACE;
                        date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.hisTreatment.DEATH_DOCUMENT_DATE ?? 0);
                        if (!string.IsNullOrEmpty(hisTreatment.DEATH_DOCUMENT_TYPE) && hisTreatment.DEATH_DOCUMENT_TYPE_CODE == null)
                        {
                            lciTxtLoaiGiayTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            txtLoaiGiayTo.Text = hisTreatment.DEATH_DOCUMENT_TYPE;
                        }
                    }
                    else if(hisPatient != null)
                    {
                        if (!string.IsNullOrEmpty(hisPatient.CCCD_NUMBER))
                        {
                            num = hisPatient.CCCD_NUMBER;
                            place = hisPatient.CCCD_PLACE;
                            date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.hisPatient.CCCD_DATE ?? 0);
                            index = 0;
                        }
                        else if (!string.IsNullOrEmpty(hisPatient.CMND_NUMBER))
                        {
                            num = hisPatient.CMND_NUMBER;
                            place = hisPatient.CMND_PLACE;
                            date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.hisPatient.CMND_DATE ?? 0);
                            index = 1;
                        }
                        else if (!string.IsNullOrEmpty(hisPatient.PASSPORT_NUMBER))
                        {
                            num = hisPatient.PASSPORT_NUMBER;
                            place = hisPatient.PASSPORT_PLACE;
                            date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.hisPatient.PASSPORT_DATE ?? 0);
                            index = 2;
                        }
                    }
                    cboLoaiGiayTo.SelectedIndex = index;
                    txtNoiCap.Text = place;
                    txtSo.Text = num;
                    dtNgayCap.EditValue = date;
                    if (hisTreatment.DEATH_STATUS == 1)
                        chkOne.Checked = true;
                    else if (hisTreatment.DEATH_STATUS == 2)
                        chkTwo.Checked = true;
                    lblPatientCode.Text = hisTreatment.TDL_PATIENT_CODE;
                    lblPatientDob.Text = hisTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? hisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.hisTreatment.TDL_PATIENT_DOB);
                    lblPatientName.Text = hisTreatment.TDL_PATIENT_NAME;
                    lblTreatmentCode.Text = hisTreatment.TREATMENT_CODE;
                    lblHeinCardNum.Text = hisTreatment.TDL_HEIN_CARD_NUMBER;
                    lblGenderName.Text = hisTreatment.TDL_PATIENT_GENDER_NAME;
                    txtRelativePatient.Text = hisTreatment.TDL_PATIENT_RELATIVE_NAME;
                }
                else
                {
                    cboDeathCause.EditValue = null;
                    cboDeathCause.Properties.Buttons[1].Visible = false;
                    cboDeathWithin.EditValue = null;
                    cboDeathWithin.Properties.Buttons[1].Visible = false;
                    dtDeathTime.EditValue = null;
                    dteDeathIssDate.EditValue = null;
                    ArrCheckEdit.ToList().ForEach(o => o.Checked = false);
                    txtDeathAnatomize.Text = "";
                    txtDeathCauseDetail.Text = "";
                    cboDeathCertBookFirst.EditValue = null;
                    txtDeathCertNumFirst.Text = null;
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    txtLoginName.Text = loginName;
                    cboUser.EditValue = loginName;
                    dtNgayCap.EditValue = null;
                    cboDeathCertBook.EditValue = null;
                    txtSo.Text = null;
                    txtNoiCap.Text = null;
                    txtLoaiGiayTo.Text = null;
                    cboLoaiGiayTo.SelectedIndex = -1;
                    txtRelativePatient.Text = null;
                    chkOne.Checked = false;
                    chkTwo.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(ADO.DeathDataSourcesADO input)
        {
            try
            {
                if (input != null)
                {
                    FillCurrentData(input);
                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                cboDeathCause.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReadOnly(bool isReadOnly)
        {
            try
            {
                cboDeathCause.ReadOnly = isReadOnly;
                cboDeathWithin.ReadOnly = isReadOnly;
                dtDeathTime.ReadOnly = isReadOnly;
                chkAupopsyY.ReadOnly = isReadOnly;
                txtDeathAnatomize.ReadOnly = isReadOnly;
                txtDeathCauseDetail.ReadOnly = isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void ReadOnlyAll(bool isReadOnly)
        {
            try
            {
                foreach (DevExpress.XtraLayout.LayoutControlItem item in layoutControlGroup1.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatfrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatfrm.ReadOnly = isReadOnly;
                    }
                }
                txtLoaiGiayTo.ReadOnly = true;
                if (hisTreatment != null && hisTreatment.DEATH_CERT_BOOK_ID != null)
                    cboDeathCertBook.ReadOnly = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(object data)
        {
            try
            {
                if (data is ADO.DeathDataSourcesADO)
                {
                    FillCurrentData((ADO.DeathDataSourcesADO)data);
                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                MOS.SDO.HisTreatmentFinishSDO outPut = new MOS.SDO.HisTreatmentFinishSDO();

                outPut.TreatmentId = hisTreatment.ID;
                if (dtDeathTime.DateTime > DateTime.Now)
                {
                    MessageBox.Show(String.Format(Resources.ResourceMessage.ThoiGianTuVongKhongDuocLonHonThoiGianHienTai, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return null;
                }
                var deathTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtDeathTime.DateTime.ToString("yyyyMMddHHmmss"));
                if (deathTime < hisTreatment.IN_TIME)
                {
                    MessageBox.Show(String.Format(Resources.ResourceMessage.ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(hisTreatment.IN_TIME)));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return null;
                }

                outPut.MainCause = txtDeathCauseDetail.Text;
                outPut.Surgery = txtDeathAnatomize.Text;
                outPut.DeathTime = deathTime;
                outPut.DeathIssuedDate = Inventec.Common.TypeConvert.Parse.ToInt64(dteDeathIssDate.DateTime.ToString("yyyyMMddHHmmss"));
                if (ArrCheckEdit.Last().Checked)
                    outPut.IsHasAupopsy = null;
                else if (ArrCheckEdit.FirstOrDefault(o => o.Checked) != null)
                    outPut.IsHasAupopsy = (short)(ArrCheckEdit.ToList().IndexOf(ArrCheckEdit.FirstOrDefault(o => o.Checked)) + 1);

                if (cboDeathCause.EditValue != null)
                {
                    outPut.DeathCauseId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathCause.EditValue.ToString());
                }
                else
                    outPut.DeathCauseId = null;

                if (cboDeathWithin.EditValue != null)
                {
                    outPut.DeathWithinId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathWithin.EditValue.ToString());
                }
                else
                {
                    outPut.DeathWithinId = null;
                }

                outPut.DeathCertBookId = cboDeathCertBook.EditValue != null ? (long?)cboDeathCertBook.EditValue : null;
                outPut.DeathPlace = txtNoiTuVong.Text;
                if (cboDeathCertBookFirst.EditValue != null)
                    outPut.DeathCertBookFirstId = Int64.Parse(cboDeathCertBookFirst.EditValue.ToString());
                else
                    outPut.DeathCertBookFirstId = null;
                if (!String.IsNullOrEmpty(txtDeathCertNumFirst.Text))
                    outPut.DeathCertNumFirst = Int64.Parse(txtDeathCertNumFirst.Text.ToString());
                else
                    outPut.DeathCertNumFirst = null;
                if(cboUser.EditValue != null)
                {
                    outPut.DeathCertIssuerLoginname = cboUser.EditValue.ToString();
                    outPut.DeathCertIssuerUsername = cboUser.Text.ToString();
                }

                if (cboLoaiGiayTo.SelectedIndex > -1)
                    outPut.DeathDocumentTypeCode = (short?)(cboLoaiGiayTo.SelectedIndex + 1);
                outPut.DeathDocumentType = txtLoaiGiayTo.Text.Trim();

                outPut.PatientRelativeName = txtRelativePatient.Text.Trim();

                long? date = null;
                if (dtNgayCap.EditValue != null)
                {
                    date = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayCap.DateTime);
                }
                string num = txtSo.Text.Trim();
                string place = txtNoiCap.Text.Trim();
                outPut.DeathDocumentNumber = num;
                outPut.DeathDocumentPlace = place;
                outPut.DeathDocumentDate = date;
                if (chkOne.Checked)
                    outPut.DeathStatus = 1;
                else
                    outPut.DeathStatus = 2;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        internal HIS_TREATMENT GetValueHisTreatment()
        {
            HIS_TREATMENT result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                HIS_TREATMENT outPut = hisTreatment;
                if (dtDeathTime.DateTime > DateTime.Now)
                {
                    MessageBox.Show(String.Format(Resources.ResourceMessage.ThoiGianTuVongKhongDuocLonHonThoiGianHienTai, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return null;
                }
                var deathTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtDeathTime.DateTime.ToString("yyyyMMddHHmmss"));
                if (deathTime < hisTreatment.IN_TIME)
                {
                    MessageBox.Show(String.Format(Resources.ResourceMessage.ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(hisTreatment.IN_TIME)));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return null;
                }

                outPut.MAIN_CAUSE = txtDeathCauseDetail.Text.Trim();
                outPut.SURGERY = txtDeathAnatomize.Text.Trim();
                outPut.DEATH_TIME = deathTime;
                outPut.DEATH_ISSUED_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dteDeathIssDate.DateTime.ToString("yyyyMMddHHmmss"));
                if (ArrCheckEdit.Last().Checked)
                    outPut.IS_HAS_AUPOPSY = null;
                else if (ArrCheckEdit.FirstOrDefault(o => o.Checked) != null)
                    outPut.IS_HAS_AUPOPSY = (short)(ArrCheckEdit.ToList().IndexOf(ArrCheckEdit.FirstOrDefault(o => o.Checked)) + 1);

                if (cboDeathCause.EditValue != null)
                {
                    outPut.DEATH_CAUSE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathCause.EditValue.ToString());
                }
                else
                    outPut.DEATH_CAUSE_ID = null;

                if (cboDeathWithin.EditValue != null)
                {
                    outPut.DEATH_WITHIN_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathWithin.EditValue.ToString());
                }
                else
                {
                    outPut.DEATH_WITHIN_ID = null;
                }

                outPut.DEATH_CERT_BOOK_ID = cboDeathCertBook.EditValue != null ? (long?)cboDeathCertBook.EditValue : null;
                outPut.DEATH_PLACE = txtNoiTuVong.Text.Trim();
                if (cboDeathCertBookFirst.EditValue != null)
                    outPut.DEATH_CERT_BOOK_FIRST_ID = Int64.Parse(cboDeathCertBookFirst.EditValue.ToString());
                else
                    outPut.DEATH_CERT_BOOK_FIRST_ID = null;
                if (!String.IsNullOrEmpty(txtDeathCertNumFirst.Text))
                    outPut.DEATH_CERT_NUM_FIRST = Int64.Parse(txtDeathCertNumFirst.Text.ToString());
                else
                    outPut.DEATH_CERT_NUM_FIRST = null;
                if (cboUser.EditValue != null)
                {
                    outPut.DEATH_CERT_ISSUER_LOGINNAME = cboUser.EditValue.ToString();
                    outPut.DEATH_CERT_ISSUER_USERNAME = cboUser.Text.ToString();
                }

                if (cboLoaiGiayTo.SelectedIndex > -1)
                {
                    outPut.DEATH_DOCUMENT_TYPE_CODE = (short?)(cboLoaiGiayTo.SelectedIndex + 1);
                    outPut.DEATH_DOCUMENT_TYPE = cboLoaiGiayTo.Properties.Items[cboLoaiGiayTo.SelectedIndex].ToString();
                }
                outPut.TDL_PATIENT_RELATIVE_NAME = txtRelativePatient.Text.Trim();

                long? date = null;
                if (dtNgayCap.EditValue != null)
                {
                    date = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNgayCap.DateTime);
                }
                string num = txtSo.Text.Trim();
                string place = txtNoiCap.Text.Trim();
                if (!string.IsNullOrEmpty(hisTreatment.TDL_PATIENT_CCCD_NUMBER))
                {
                    outPut.TDL_PATIENT_CCCD_NUMBER = num;
                    outPut.TDL_PATIENT_CCCD_PLACE = place;
                    outPut.TDL_PATIENT_CCCD_DATE = date;
                }
                else if (!string.IsNullOrEmpty(hisTreatment.TDL_PATIENT_CMND_NUMBER))
                {
                    outPut.TDL_PATIENT_CMND_NUMBER = num;
                    outPut.TDL_PATIENT_CMND_PLACE = place;
                    outPut.TDL_PATIENT_CMND_DATE  = date;
                }
                else if (!string.IsNullOrEmpty(hisTreatment.TDL_PATIENT_PASSPORT_NUMBER))
                {
                    outPut.TDL_PATIENT_PASSPORT_NUMBER = num;
                    outPut.TDL_PATIENT_PASSPORT_PLACE = place;
                    outPut.TDL_PATIENT_PASSPORT_DATE = date;
                }
                else
                {
                    outPut.DEATH_DOCUMENT_NUMBER = num;
                    outPut.DEATH_DOCUMENT_PLACE = place;
                    outPut.DEATH_DOCUMENT_DATE = date;
                }
                if (chkOne.Checked)
                    outPut.DEATH_STATUS = 1;
                else
                    outPut.DEATH_STATUS = 2;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void txtNoiTuVong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoaiGiayTo.Focus();
                    txtLoaiGiayTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoaiGiayTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSo.Focus();
                    txtSo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoiCap.Focus();
                    txtNoiCap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNoiCap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNgayCap.Focus();
                    //dtNgayCap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtNgayCap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDeathCertBookFirst.Focus();
                    cboDeathCertBookFirst.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoaiGiayTo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (txtLoaiGiayTo.Text != "CMND" || txtLoaiGiayTo.Text != "CCCD")
                //{
                //    txtSo.Text = "";
                //    txtNoiCap.Text = "";
                //    dtNgayCap.EditValue = null;
                //}
                //else
                //{
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool ValidateControl()
        {
            return this.dxValidationProvider1.Validate();
        }

        private void cboDeathCertBookFirst_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDeathCertBookFirst.Properties.Buttons[1].Visible = cboDeathCertBookFirst.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathCertBookFirst_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && !cboDeathCertBookFirst.ReadOnly)
                {
                    cboDeathCertBookFirst.EditValue = null;
                    cboDeathCertBookFirst.Properties.Buttons[1].Visible = cboDeathCertBookFirst.EditValue != null;
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtDeathCertNumFirst_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<HIS_EMPLOYEE>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FocusWhileSelectedUser()
        {
            try
            {
                txtDeathCertNumFirst.Focus();
                txtDeathCertNumFirst.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        HIS_EMPLOYEE data = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboUser.Properties.Buttons[1].Visible = cboUser.EditValue != null;
                if (this.cboUser.EditValue != null)
                {
                    HIS_EMPLOYEE data = BackendDataWorker.Get<HIS_EMPLOYEE>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                }
                else
                {
                    this.txtLoginName.Text = null;
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeathCertNumFirst_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if(e.KeyCode == Keys.Enter)
                {
                    txtDeathCauseDetail.Focus();
                    txtDeathCauseDetail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && !cboUser.ReadOnly)
                {
                    cboUser.EditValue = null;
                    txtLoginName.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathCertBookFirst_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboDeathCertBookFirst.EditValue != null)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                    cboDeathCertBookFirst.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOne_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOne.Checked)
                    chkTwo.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTwo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTwo.Checked)
                    chkOne.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiGiayTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboLoaiGiayTo.SelectedIndex > -1)
                    txtLoaiGiayTo.Text = cboLoaiGiayTo.Properties.Items[cboLoaiGiayTo.SelectedIndex].ToString();
                else
                    txtLoaiGiayTo.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
