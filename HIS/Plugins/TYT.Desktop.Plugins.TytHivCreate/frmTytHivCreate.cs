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
using Inventec.Desktop.Common.Controls.ValidationRule;

namespace TYT.Desktop.Plugins.TytHivCreate
{
    public partial class frmTytHivCreate : HIS.Desktop.Utility.FormBase
    {
        #region Khai báo

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_PATIENT currentPatient;
        TYT_HIV currentHiv;

        #endregion

        #region Contruct

        public frmTytHivCreate()
        {
            InitializeComponent();
            SetIcon();
        }
        public frmTytHivCreate(Inventec.Desktop.Common.Modules.Module module, V_HIS_PATIENT patient)
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
                this.currentHiv = new BackendAdapter(param).Get<List<TYT_HIV>>("api/TytHiv/Get", ApiConsumers.TytConsumer, filter, null).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public frmTytHivCreate(Inventec.Desktop.Common.Modules.Module module, TYT_HIV hiv)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentHiv = hiv;
                this.currentModule = module;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmTytHivCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadComboObjectType();
                SetDefaultValue();
                LoadInfoForm();
                Setvalidation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Setvalidation()
        {
            ValidationControlMaxLength(txtArvPlace, 100);
            ValidationControlMaxLength(txtHiv, 100);
            ValidationControlMaxLength(txtNote, 100);

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
            //if (btnRefresh.Enabled)
            //{
            //    btnRefresh_Click(null, null);
            //}
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                TYT_HIV tytHiv = new TYT_HIV();
                TYT_HIV rs = null;
                bool success = false;
                if (!dxValidationProvider1.Validate())
                    return;
                if (this.currentHiv != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_HIV>(tytHiv, this.currentHiv);
                }
                UpdateDataFormToSave(ref tytHiv);

                if (this.currentHiv != null)
                {
                    rs = new BackendAdapter(param).Post<TYT_HIV>(RequestUriStore.TytHiv_Update, ApiConsumers.TytConsumer, tytHiv, param);
                }
                else
                {
                    rs = new BackendAdapter(param).Post<TYT_HIV>(RequestUriStore.TytHiv_Create, ApiConsumers.TytConsumer, tytHiv, param);
                }

                if (rs != null)
                {
                    success = true;
                    btnDelete.Enabled = true;
                    this.currentHiv = rs;
                    //btnRefresh.Enabled = false;
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

        #endregion

        #region Event Control

        private void cboObjectType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFetusTime.Focus();
                    dtFetusTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboObjectType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboObjectType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtFetusTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtHIV_DIAGNOSIS_TIME.Focus();
                    dtHIV_DIAGNOSIS_TIME.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtHiv_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHiv.Focus();
                    txtHiv.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtHiv_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtArvPlace.Focus();
                    txtArvPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtArvPlace_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtDeathTime.Focus();
                    dtDeathTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtDeathTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtNote_KeyUp(object sender, KeyEventArgs e)
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

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                throw;
            }
        }
        #endregion

        #region Method

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("TYT.Desktop.Plugins.TytHivCreate.Resources.Lang", typeof(TYT.Desktop.Plugins.TytHivCreate.frmTytHivCreate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTytHivCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmTytHivCreate.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboObjectType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTytHivCreate.cboObjectType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("frmTytHivCreate.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTytHivCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                cboObjectType.EditValue = null;
                dtFetusTime.EditValue = null;
                dtHIV_DIAGNOSIS_TIME.EditValue = null;
                txtArvPlace.Text = "";
                txtHiv.Text = "";
                txtNote.Text = "";
                btnDelete.Enabled = false;
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
                //Sửa
                if (this.currentHiv != null)
                {

                    dtDeathTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHiv.DEATH_TIME ?? 0);
                    cboObjectType.EditValue = this.currentHiv.OBJECT_TYPE;
                    dtFetusTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHiv.FETUS_TIME ?? 0);
                    dtHIV_DIAGNOSIS_TIME.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHiv.HIV_DIAGNOSIS_TIME ?? 0);
                    txtArvPlace.Text = this.currentHiv.ARV_PLACE;
                    txtHiv.Text = this.currentHiv.HIV_DIAGNOSIS_PLACE;
                    txtNote.Text = this.currentHiv.NOTE;
                    btnDelete.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboObjectType()
        {
            try
            {
                List<ObjectTypeADO> data = new List<ObjectTypeADO>();
                data.Add(new ObjectTypeADO("Nghiện chích ma túy"));
                data.Add(new ObjectTypeADO("Người bán dâm, tiếp viên nhà hàng"));
                data.Add(new ObjectTypeADO("Người có quan hệ tình dục đồng giới"));
                data.Add(new ObjectTypeADO("Người nhiễm HIV và bệnh nhân AIDS"));
                data.Add(new ObjectTypeADO("Thành viên gia đình người nhiễm HIV"));
                data.Add(new ObjectTypeADO("Nhóm dân đi biến động"));
                data.Add(new ObjectTypeADO("Phụ nữ mang thai"));
                data.Add(new ObjectTypeADO("Nhóm tuổi từ 15-19"));
                data.Add(new ObjectTypeADO("Nhóm các đối tượng khác"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 400, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "NAME", columnInfos, false, 350);
                ControlEditorLoader.Load(cboObjectType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataFormToSave(ref TYT_HIV updateData)
        {
            try
            {
                if (cboObjectType.EditValue != null)
                {
                    updateData.OBJECT_TYPE = cboObjectType.EditValue.ToString();
                }
                else
                    updateData.OBJECT_TYPE = "";

                if (dtDeathTime.EditValue != null)
                {
                    updateData.DEATH_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDeathTime.DateTime);
                }
                else
                    updateData.DEATH_TIME = null;

                if (dtFetusTime.EditValue != null)
                    updateData.FETUS_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtFetusTime.DateTime);
                else
                    updateData.FETUS_TIME = null;

                if (dtHIV_DIAGNOSIS_TIME.EditValue != null)
                    updateData.HIV_DIAGNOSIS_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtHIV_DIAGNOSIS_TIME.DateTime);
                else
                    updateData.HIV_DIAGNOSIS_TIME = null;

                updateData.NOTE = txtNote.Text;
                updateData.HIV_DIAGNOSIS_PLACE = txtHiv.Text;
                updateData.ARV_PLACE = txtArvPlace.Text;

                if (this.currentHiv == null)
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.currentHiv != null)
            {
                bool success = false;
                CommonParam param = new CommonParam();
                success = new BackendAdapter(param).Post<bool>("api/TytHiv/Delete", ApiConsumers.TytConsumer, this.currentHiv.ID, param);
                if (success)
                {
                    btnDelete.Enabled = true;
                    MessageManager.Show(this, param, success);
                    this.Close();
                }
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = false;
            validate.ErrorText = "Nhập quá kí tự cho phép";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(control, validate);
        }
        #region Public method



        #endregion

    }
}
