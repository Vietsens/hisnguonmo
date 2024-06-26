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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Register.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.PatientExtend
{
    public partial class frmPatientExtend : HIS.Desktop.Utility.FormBase
    {
        #region Reclare
        public delegate void PatientInfoResult(object data);
        PatientInformationADO patientInformation { get; set; }
        PatientInfoResult delegatePatientInfoResult;
        int positionHandleControl = -1;
        List<HID.EFMODEL.DataModels.HID_HOUSEHOLD_RELATION> houseHoldRelates;
        #endregion

        #region Construct
        public frmPatientExtend()
        {
            InitializeComponent();
        }

        public frmPatientExtend(PatientInformationADO patientInformation, PatientInfoResult delegatePatientInfoResult)
        {
            InitializeComponent();
            this.patientInformation = patientInformation;
            this.delegatePatientInfoResult = delegatePatientInfoResult;
        }
        #endregion

        #region Load
        private void frmPatientExtend_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();

                InitComboBloodABO();
                InitComboBloodRH();
                InitComboHoldHouse();
                InitComboProvince(cboProvince, BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList());

                //Load data default
                LoadDefaultPatientInfo();
                ValidateForm();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmPatientExtend = new System.Resources.ResourceManager("HIS.Desktop.Plugins.Register.Resources.Lang", typeof(HIS.Desktop.Plugins.Register.PatientExtend.frmPatientExtend).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.bar1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmPatientExtend.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.groupControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.groupControl1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl3.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.cboProvinceKS.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboProvinceKS.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboDistrict.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboDistrict.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboBloodRh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboBloodRh.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.cboBlood.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientExtend.cboBlood.Properties.NullText", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.groupControl3.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.groupControl3.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl4.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.groupControl2.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.groupControl2.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControl2.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.chkIsHead.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPatientExtend.chkIsHead.Properties.Caption", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientExtend.Text", Resources.ResourceLanguageManager.LanguagefrmPatientExtend, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Control
        //private void txtHouseHold_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        //        {
        //            if (!String.IsNullOrEmpty(txtHouseHold.Text))
        //            {
        //                HIS_HOUSEHOLD houseHold = GetHouseHoldByCode(txtHouseHold.Text.Trim());
        //                if (houseHold != null)
        //                {
        //                    lblIsHouseHold.Text = "[Có thông tin sổ hộ khẩu]";
        //                }
        //                else
        //                {
        //                    lblIsHouseHold.Text = "[Không có thông tin sổ hộ khẩu]";
        //                }
        //            }
        //            chkIsHead.Properties.FullFocusRect = true;
        //            chkIsHead.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceCombo(strValue, true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboProvince.EditValue != null && cboProvince.EditValue != cboProvince.OldEditValue)
                    {
                        var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboProvince.EditValue.ToString()));
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.ID.ToString(), false);
                            txtProvince.Text = province.PROVINCE_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                    else
                    {
                        txtDistricts.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)cboProvince.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceId = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceId = cboProvince.EditValue.ToString();
                    }
                    LoadDistrictsCombo(strValue.ToUpper(), provinceId, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDistrict.EditValue != null && cboDistrict.EditValue != cboDistrict.OldEditValue)
                    {
                        string str = cboDistrict.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDistrict.EditValue.ToString()));
                        if (district != null)
                        {
                            LoadCommuneCombo("", district.ID.ToString(), false);
                            txtDistricts.Text = district.DISTRICT_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                            txtCommune.Focus();
                            txtCommune.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistrict_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)cboDistrict.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommune_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtID = "";
                    if (cboDistrict.EditValue != null)
                    {
                        districtID = cboDistrict.EditValue.ToString();
                    }
                    LoadCommuneCombo(strValue.ToUpper(), districtID, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)cboCommune.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null && cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboCommune.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtCommune.Text = commune.COMMUNE_CODE;
                            txtAdressRelation.Focus();
                            txtAdressRelation.SelectAll();
                        }
                    }
                    else
                    {
                        txtAdressRelation.Focus();
                        txtAdressRelation.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                bool validPatientInfo = dxValidationProviderControl.Validate();
                if (!validPatientInfo)
                {
                    IList<Control> invalidControls = dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    return;
                }
                if (!string.IsNullOrEmpty(txtHOUSEHOLD_CODE.Text)
                    && Inventec.Common.String.CheckString.IsOverMaxLength(txtHOUSEHOLD_CODE.Text, 9))
                {
                    txtHOUSEHOLD_CODE.Focus();
                    txtHOUSEHOLD_CODE.SelectAll();
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NhapQuaKyTuSoHoKhau,
                ResourceMessage.TieuDeCuaSoThongBaoLaThongBao);
                    return;
                }

                PatientInformationADO patientInfomatient = new PatientInformationADO();
                if (this.patientInformation != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<PatientInformationADO>(patientInfomatient, this.patientInformation);
                }

                if (cboBlood.EditValue != null)
                {
                    patientInfomatient.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBlood.EditValue.ToString());
                    patientInfomatient.BLOOD_ABO_CODE = cboBlood.Text;
                }
                else
                {
                    patientInfomatient.BLOOD_ABO_ID = null;
                    patientInfomatient.BLOOD_ABO_CODE = "";
                }
                if (cboBlood.EditValue != null)
                {
                    patientInfomatient.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodRh.EditValue.ToString());
                    patientInfomatient.BLOOD_RH_CODE = cboBloodRh.Text;
                }
                else
                {
                    patientInfomatient.BLOOD_RH_ID = null;
                    patientInfomatient.BLOOD_RH_CODE = "";
                }
                //if (chkIsHead.Checked)
                //    patientInfomatient.IS_HEAD = 1;
                //else
                //    patientInfomatient.IS_HEAD = null;
                //if (!String.IsNullOrEmpty(txtHohRelationShip.Text.Trim()))
                //    patientInfomatient.HOH_RELATIONSHIP = txtHohRelationShip.Text.Trim();
                //else
                //    patientInfomatient.HOH_RELATIONSHIP = null;

                patientInfomatient.CMND_NUMBER = txtCmndNumber.Text.Trim();
                if (!String.IsNullOrEmpty(dtCmndDate.Text))
                    patientInfomatient.CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtCmndDate.DateTime) ?? 0;
                else
                    patientInfomatient.CMND_DATE = null;
                patientInfomatient.CMND_PLACE = txtCmndPlace.Text;
                patientInfomatient.EMAIL = txtEmail.Text;
                if (cboHOUSEHOLD_RELATION_ID.EditValue != null)
                {
                    patientInfomatient.HOUSEHOLD_RELATION_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboHOUSEHOLD_RELATION_ID.EditValue.ToString());
                }
                else
                {
                    patientInfomatient.HOUSEHOLD_RELATION_ID = null;
                }
                patientInfomatient.HOUSEHOLD_RELATION_NAME = cboHOUSEHOLD_RELATION_ID.Text;

                patientInfomatient.HOUSEHOLD_CODE = txtHOUSEHOLD_CODE.Text;
                patientInfomatient.HT_PROVINCE_NAME = cboProvince.Text;
                patientInfomatient.HT_PROVINCE_CODE = txtProvince.Text;
                patientInfomatient.HT_DISTRICT_NAME = cboDistrict.Text;
                patientInfomatient.HT_DISTRICT_CODE = txtDistricts.Text;
                patientInfomatient.HT_COMMUNE_NAME = cboCommune.Text;
                patientInfomatient.HT_COMMUNE_CODE = txtCommune.Text;
                patientInfomatient.HT_ADDRESS = txtAdressRelation.Text;
                patientInfomatient.MOTHER_NAME = txtMotherName.Text;
                patientInfomatient.FATHER_NAME = txtFatherName.Text;

                //patientInfomatient.NCS_RELATIVE = txtNcsRelative.Text;
                //patientInfomatient.NCS_PHONE = txtNCSPhone.Text;
                //if (!String.IsNullOrEmpty(txtHouseHold.Text))
                //    patientInfomatient.HOUSEHOLD_CODE = txtHouseHold.Text;
                //else
                //    patientInfomatient.HOUSEHOLD_CODE = null;
                patientInfomatient.IsEdited = true;
                if (delegatePatientInfoResult != null) delegatePatientInfoResult(patientInfomatient);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void chkIsHead_Click(object sender, EventArgs e)
        {

        }

        private void cboBlood_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboBloodRh.Focus();
                cboBloodRh.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBloodRh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                txtCmndNumber.Focus();
                txtCmndNumber.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBlood_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBlood.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCmndNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    dtCmndDate.Focus();
                    dtCmndDate.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCmndDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtCmndPlace.Focus();
                    txtCmndPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCmndPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtProvince.Focus();
                    txtProvince.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFatherName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtMotherName.Focus();
                    txtMotherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMotherName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtNCSName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        //        {
        //            txtNcsRelative.Focus();
        //            txtNcsRelative.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtNcsRelative_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        //        {
        //            txtNCSPhone.Focus();
        //            txtNCSPhone.SelectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtNCSPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
        //        {
        //            btnSave.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void dtCmndDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                txtCmndPlace.Focus();
                txtCmndPlace.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHohRelationShip_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtProvince.Focus();
                    txtProvince.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAdressRelation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    txtHOUSEHOLD_CODE.Focus();
                    txtHOUSEHOLD_CODE.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHouseHold_Leave(object sender, EventArgs e)
        {

        }

        private void txtCmndNumber_Leave(object sender, EventArgs e)
        {
            //try
            //{
            //    if (txtCmndNumber.Text.Trim().Length != 9 || txtCmndNumber.Text.Trim().Length != 12)
            //    {
            //        MessageBox.Show("Độ dài CMND là 9 hoặc 12", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        txtCmndNumber.Focus();
            //        txtCmndNumber.SelectAll();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }
        #endregion

        private void cboHOUSEHOLD_RELATION_ID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHOUSEHOLD_RELATION_ID.EditValue != null)
                    {
                        txtFatherName.Focus();
                        txtFatherName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHOUSEHOLD_RELATION_ID_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHOUSEHOLD_RELATION_ID.EditValue != null)
                    {
                        //txtFatherName.Focus();
                        //txtFatherName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHOUSEHOLD_CODE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    cboHOUSEHOLD_RELATION_ID.Focus();
                    cboHOUSEHOLD_RELATION_ID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
