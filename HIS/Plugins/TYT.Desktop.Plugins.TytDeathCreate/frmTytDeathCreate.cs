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
using MOS.EFMODEL.DataModels;
using TYT.EFMODEL.DataModels;
using MOS.Filter;
using TYT.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;

namespace TYT.Desktop.Plugins.TytDeathCreate
{
    public partial class frmTytDeathCreate : HIS.Desktop.Utility.FormBase
    {
        #region Khai báo

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_PATIENT currentPatient;
        TYT_DEATH currentDeath;
        int positionHandle = -1;

        #endregion

        #region Contruct

        public frmTytDeathCreate()
        {
            InitializeComponent();
            SetIcon();
        }
        public frmTytDeathCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT patient)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentPatient = patient;
                this.currentModule = module;
                SetIcon();
                CommonParam param = new CommonParam();
                TytDeathFilter filter = new TytDeathFilter();
                filter.PATIENT_CODE__EXACT = patient.PATIENT_CODE;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.currentDeath = new BackendAdapter(param).Get<List<TYT_DEATH>>("api/TytDeath/Get", ApiConsumers.TytConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public frmTytDeathCreate(Inventec.Desktop.Common.Modules.Module module, TYT_DEATH death)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentDeath = death;
                this.currentModule = module;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTytDeathCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetValidateForm();
                SetCaptionByLanguageKey();
                LoadComboDeathLocationType();
                SetDefaultValue();
                LoadInfoForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Event Form

        #region Phím tắt

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnDelete.Enabled)
                {
                    btnDelete_Click(null, null);
                }

            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                CommonParam param = new CommonParam();
                TYT_DEATH tytHiv = new TYT_DEATH();
                TYT_DEATH rs = null;
                bool success = false;

                if (this.currentDeath != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_DEATH>(tytHiv, this.currentDeath);
                }
                UpdateDataFormToSave(ref tytHiv);

                if (this.currentDeath != null)
                {
                    rs = new BackendAdapter(param).Post<TYT_DEATH>(RequestUriStore.TytDeath_Update, ApiConsumers.TytConsumer, tytHiv, param);
                }
                else
                {
                    rs = new BackendAdapter(param).Post<TYT_DEATH>(RequestUriStore.TytDeath_Create, ApiConsumers.TytConsumer, tytHiv, param);
                }

                if (rs != null)
                {
                    success = true;
                    this.currentDeath = rs;
                    btnDelete.Enabled = true;
                    //btnSave.Enabled = false;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            success = new BackendAdapter(param).Post<bool>("api/TytDeath/Delete", ApiConsumers.TytConsumer, this.currentDeath.ID, param);
            if (success)
            {
                btnDelete.Enabled = true;
                MessageManager.Show(this, param, success);
                this.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void dtDeathTime_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDeathLocationType.Focus();
                    cboDeathLocationType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDeathLocationType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCollector.Focus();
                    txtCollector.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeathLocationType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDeathLocationType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtCollector_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCBYTCare.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCBYTCare_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDeathCause.Focus();
                    txtDeathCause.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDeathCause_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("TYT.Desktop.Plugins.TytDeathCreate.Resources.Lang", typeof(TYT.Desktop.Plugins.TytDeathCreate.frmTytDeathCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.groupBoxHivInfo.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.groupBoxHivInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.groupBoxPatientInfo.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.groupBoxPatientInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTytDeathCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModule != null)
                    this.Text = this.currentModule.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                dtDeathTime.EditValue = null;
                cboDeathLocationType.EditValue = null;
                txtCollector.Text = "";
                txtDeathCause.Text = "";
                txtDescription.Text = "";
                chkCBYTCare.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadInfoForm()
        {
            try
            {
                if (this.currentDeath != null)
                {
                    dtDeathTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentDeath.DEATH_TIME ?? 0);
                    cboDeathLocationType.EditValue = this.currentDeath.DEATH_LOCATION_TYPE_ID;
                    dtDeathTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentDeath.DEATH_TIME ?? 0);
                    txtCollector.Text = this.currentDeath.COLLECTOR;
                    txtDeathCause.Text = this.currentDeath.DEATH_CAUSE;
                    txtDescription.Text = this.currentDeath.DESCRIPTION;
                    chkCBYTCare.Checked = this.currentDeath.IS_CBYT_CARE == 1 ? true : false;
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnDelete.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboDeathLocationType()
        {
            try
            {
                List<DeathLocationTypeADO> data = new List<DeathLocationTypeADO>();
                data.Add(new DeathLocationTypeADO(1, "Tại CSYT"));
                data.Add(new DeathLocationTypeADO(2, "Tại nhà"));
                data.Add(new DeathLocationTypeADO(3, "Khác"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 400, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDeathLocationType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataFormToSave(ref TYT_DEATH updateData)
        {
            try
            {
                if (cboDeathLocationType.EditValue != null)
                {
                    updateData.DEATH_LOCATION_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDeathLocationType.EditValue.ToString());
                }
                else
                    updateData.DEATH_LOCATION_TYPE_ID = null;

                updateData.DEATH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDeathTime.DateTime);
                updateData.IS_CBYT_CARE = chkCBYTCare.Checked ? (short?)1 : null;
                updateData.COLLECTOR = txtCollector.Text;
                updateData.DEATH_CAUSE = txtDeathCause.Text;
                updateData.DESCRIPTION = txtDescription.Text;

                if (this.currentDeath == null)
                {
                    updateData.CAREER_NAME = this.currentPatient.CAREER_NAME;
                    updateData.DOB = this.currentPatient.DOB;
                    updateData.ETHNIC_NAME = this.currentPatient.ETHNIC_NAME;
                    updateData.GENDER_NAME = this.currentPatient.GENDER_NAME;
                    updateData.IS_HAS_NOT_DAY_DOB = this.currentPatient.IS_HAS_NOT_DAY_DOB;
                    updateData.FIRST_NAME = this.currentPatient.FIRST_NAME;
                    updateData.LAST_NAME = this.currentPatient.LAST_NAME;
                    updateData.PATIENT_CODE = this.currentPatient.PATIENT_CODE;
                    updateData.PERSON_ADDRESS = this.currentPatient.VIR_ADDRESS;
                    updateData.PERSON_CODE = this.currentPatient.PERSON_CODE;
                    updateData.VIR_PERSON_NAME = this.currentPatient.VIR_PATIENT_NAME;

                    var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (branch != null)
                    {
                        updateData.BRANCH_CODE = branch.BRANCH_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion

        #region Public method



        #endregion

        #region Validate

        private void SetValidateForm()
        {
            try
            {
                ValidateMaxlength(txtDeathCause, 500);
                ValidateMaxlength(txtDescription, 500);
                ValidationSingleControl(dtDeathTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlength(Control control, int maxLength)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.ErrorType = ErrorType.Warning;
                validate.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                this.dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
