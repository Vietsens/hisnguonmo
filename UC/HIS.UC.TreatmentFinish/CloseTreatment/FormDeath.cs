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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.TreatmentFinish.Resources;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreatmentFinish.CloseTreatment
{
    public partial class FormDeath : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment { get; set; }
        private int positionHandle = -1;
        private MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO { get; set; }
        private TreatmentEndInputADO treatmentEndInputADO;
        internal delegate void GetString(MOS.SDO.HisTreatmentFinishSDO currentTreatmentFinishSDO);
        internal GetString MyGetData;
        #endregion

        #region Construct
        public FormDeath()
        {
            InitializeComponent();
        }

        public FormDeath(TreatmentEndInputADO _treatmentEndInputADO)
            : this()
        {
            try
            {
                if (_treatmentEndInputADO != null)
                {
                    this.treatmentEndInputADO = _treatmentEndInputADO;
                    this.hisTreatment = _treatmentEndInputADO.Treatment;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormDeath_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                LoadKeysFromlanguage();

                LoadDataToCombo();

                SetDefaultValueControl();
                loadDataTranPatiOld(this.hisTreatment);
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void loadDataTranPatiOld(MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    //var data = currentTreatmentFinishSDO.HisDeath;

                    MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE deathCause = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == treatment.DEATH_CAUSE_ID);
                    if (deathCause != null)
                    {
                        cboDeathCause.EditValue = deathCause.ID;
                        txtDeathCause.Text = deathCause.DEATH_CAUSE_CODE;
                    }

                    MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN deathWithin = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == treatment.DEATH_WITHIN_ID);
                    if (deathWithin != null)
                    {
                        cboDeathWithin.EditValue = deathWithin.ID;
                        txtDeathWithin.Text = deathWithin.DEATH_WITHIN_CODE;
                    }

                    if (treatment.IS_HAS_AUPOPSY == 1)
                    {
                        chkAupopsy.Checked = true;
                    }
                    else
                    {
                        chkAupopsy.Checked = false;
                    }

                    txtDeathAnatomize.Text = treatment.SURGERY;
                    txtDeathCauseDetail.Text = treatment.MAIN_CAUSE;

                    if (treatment.DEATH_TIME.HasValue)
                    {
                        dtDeathTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.DEATH_TIME.Value);
                    }
                    else
                    {
                        dtDeathTime.DateTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                Resources.ResourceLanguageManager.LanguageFormDeath = new ResourceManager("HIS.UC.TreatmentFinish.Resources.Lang", typeof(FormDeath).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormDeath.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormDeath.btnSave.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.chkAupopsy.Properties.Caption = Inventec.Common.Resource.Get.Value("FormDeath.chkAupopsy.Properties.Caption", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.cboDeathWithin.Properties.NullText = Inventec.Common.Resource.Get.Value("FormDeath.cboDeathWithin.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.cboDeathCause.Properties.NullText = Inventec.Common.Resource.Get.Value("FormDeath.cboDeathCause.Properties.NullText", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.lciDeathCause.Text = Inventec.Common.Resource.Get.Value("FormDeath.lciDeathCause.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.lciDeathWithin.Text = Inventec.Common.Resource.Get.Value("FormDeath.lciDeathWithin.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.lciDeathTime.Text = Inventec.Common.Resource.Get.Value("FormDeath.lciDeathTime.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.lciDeathCauseDetail.Text = Inventec.Common.Resource.Get.Value("FormDeath.lciDeathCauseDetail.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.lciDeathAnatomize.Text = Inventec.Common.Resource.Get.Value("FormDeath.lciDeathAnatomize.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormDeath.bar1.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("FormDeath.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormDeath.Text", Resources.ResourceLanguageManager.LanguageFormDeath, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {

                string ma = CommonBaseEditor.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_MA", Resources.ResourceLanguageManager.LanguageFormDeath);
                string ten = CommonBaseEditor.GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FIMISH_TEN", Resources.ResourceLanguageManager.LanguageFormDeath);
                CommonBaseEditor.LoadDataGridLookUpEdit(cboDeathCause, "DEATH_CAUSE_CODE", ma, "DEATH_CAUSE_NAME", ten, "ID", BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>());
                CommonBaseEditor.LoadDataGridLookUpEdit(cboDeathWithin, "DEATH_WITHIN_CODE", ma, "DEATH_WITHIN_NAME", ten, "ID", BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                currentTreatmentFinishSDO = treatmentEndInputADO.HisTreatmentFinishSDO;
                if (currentTreatmentFinishSDO != null)
                {
                    //var data = currentTreatmentFinishSDO.HisDeath;

                    MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE deathCause = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == currentTreatmentFinishSDO.DeathCauseId);
                    if (deathCause != null)
                    {
                        cboDeathCause.EditValue = deathCause.ID;
                        txtDeathCause.Text = deathCause.DEATH_CAUSE_CODE;
                    }

                    MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN deathWithin = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == currentTreatmentFinishSDO.DeathWithinId);
                    if (deathWithin != null)
                    {
                        cboDeathWithin.EditValue = deathWithin.ID;
                        txtDeathWithin.Text = deathWithin.DEATH_WITHIN_CODE;
                    }

                    if (currentTreatmentFinishSDO.IsHasAupopsy == 1)
                    {
                        chkAupopsy.Checked = true;
                    }
                    else
                    {
                        chkAupopsy.Checked = false;
                    }

                    txtDeathAnatomize.Text = currentTreatmentFinishSDO.Surgery;
                    txtDeathCauseDetail.Text = currentTreatmentFinishSDO.MainCause;

                    if (currentTreatmentFinishSDO.DeathTime.HasValue)
                    {
                        dtDeathTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentTreatmentFinishSDO.DeathTime.Value);
                    }
                    else
                    {
                        dtDeathTime.DateTime = DateTime.Now;
                    }
                }
                txtDeathCause.Focus();
                txtDeathCause.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void ValidateForm()
        {
            try
            {
                CommonBaseEditor.ValidateGridLookupWithTextEdit(this.cboDeathCause, this.txtDeathCause, this.dxValidationProviderControl);
                CommonBaseEditor.ValidateGridLookupWithTextEdit(this.cboDeathWithin, this.txtDeathWithin, this.dxValidationProviderControl);
                CommonBaseEditor.ValidationSingleControl(this.dtDeathTime, this.dxValidationProviderControl);
                CommonBaseEditor.ValidationSingleControl(this.txtDeathAnatomize, this.dxValidationProviderControl, ResourceMessage.TruongThongTinCoDoDaiVuotQuaGioiHan, IsValidDeathAnatomize);
                CommonBaseEditor.ValidationControlDeathCauseDetail(this.txtDeathCauseDetail, IsValidDeathCauseDetail, this.dxValidationProviderControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsValidDeathCauseDetail()
        {
            return true;
        }

        bool IsValidDeathAnatomize()
        {
            return (txtDeathAnatomize.Text.Length <= 2000);
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
        #endregion

        #region Event
        private void txtDeathCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDeathCause.Text.Trim()))
                    {
                        string code = txtDeathCause.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>().Where(o => o.DEATH_CAUSE_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.DEATH_CAUSE_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDeathCause.Text = result.First().DEATH_CAUSE_CODE;
                            cboDeathCause.EditValue = result.First().ID;
                            txtDeathWithin.Focus();
                            txtDeathWithin.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboDeathCause.Focus();
                        cboDeathCause.ShowPopup();
                    }
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
                    if (cboDeathCause.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>().SingleOrDefault(o => o.ID == (long)(cboDeathCause.EditValue ?? 0));
                        if (data != null)
                        {
                            txtDeathCause.Text = data.DEATH_CAUSE_CODE;
                            txtDeathWithin.Focus();
                            txtDeathWithin.SelectAll();
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
                    if (cboDeathCause.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_CAUSE>().SingleOrDefault(o => o.ID == (long)(cboDeathCause.EditValue ?? 0));
                        if (data != null)
                        {
                            txtDeathCause.Text = data.DEATH_CAUSE_CODE;
                            txtDeathWithin.Focus();
                            txtDeathWithin.SelectAll();
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

        private void txtDeathWithin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtDeathWithin.Text.Trim()))
                    {
                        string code = txtDeathWithin.Text.Trim();
                        var listData = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().Where(o => o.DEATH_WITHIN_CODE.Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.DEATH_WITHIN_CODE == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtDeathWithin.Text = result.First().DEATH_WITHIN_CODE;
                            cboDeathWithin.EditValue = result.First().ID;
                            dtDeathTime.Focus();
                            dtDeathTime.ShowPopup();
                        }
                    }
                    if (showCbo)
                    {
                        cboDeathWithin.Focus();
                        cboDeathWithin.ShowPopup();
                    }
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
                    if (cboDeathWithin.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().SingleOrDefault(o => o.ID == (long)(cboDeathWithin.EditValue ?? 0));
                        if (data != null)
                        {
                            txtDeathWithin.Text = data.DEATH_WITHIN_CODE;
                            dtDeathTime.Focus();
                            dtDeathTime.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathWithin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDeathWithin.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().SingleOrDefault(o => o.ID == (long)(cboDeathWithin.EditValue ?? 0));
                        if (data != null)
                        {
                            txtDeathWithin.Text = data.DEATH_WITHIN_CODE;
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
                        chkAupopsy.Focus();
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
                        chkAupopsy.Focus();
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
                    txtDeathCauseDetail.Focus();
                    txtDeathCauseDetail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderControl.Validate()) return;
                if (dtDeathTime.DateTime > DateTime.Now)
                {
                    MessageBox.Show(String.Format(ResourceMessage.ThoiGianTuVongKhongDuocLonHonThoiGianHienTai, DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return;
                }
                var deathTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtDeathTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                if (deathTime < hisTreatment.IN_TIME)
                {
                    MessageBox.Show(String.Format(ResourceMessage.ThoiGianTuVongKhongDuocBeHonThoiGianVaoVien, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(hisTreatment.IN_TIME)));
                    dtDeathTime.Focus();
                    dtDeathTime.SelectAll();
                    return;
                }

                currentTreatmentFinishSDO.TreatmentId = hisTreatment.ID;
                currentTreatmentFinishSDO.MainCause = txtDeathCauseDetail.Text;
                currentTreatmentFinishSDO.Surgery = txtDeathAnatomize.Text;
                currentTreatmentFinishSDO.DeathTime = deathTime;

                if (chkAupopsy.Checked)
                    currentTreatmentFinishSDO.IsHasAupopsy = 1;
                if (cboDeathCause.EditValue != null)
                {
                    currentTreatmentFinishSDO.DeathCauseId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathCause.EditValue.ToString());
                }
                else
                    currentTreatmentFinishSDO.DeathCauseId = null;

                MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN data_within = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEATH_WITHIN>().SingleOrDefault(o => o.DEATH_WITHIN_CODE == txtDeathWithin.Text);
                if (cboDeathWithin.EditValue != null)
                {
                    currentTreatmentFinishSDO.DeathWithinId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathWithin.EditValue.ToString());
                }
                else
                {
                    currentTreatmentFinishSDO.DeathWithinId = null;
                }
                MyGetData(currentTreatmentFinishSDO);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Shotcut
        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
