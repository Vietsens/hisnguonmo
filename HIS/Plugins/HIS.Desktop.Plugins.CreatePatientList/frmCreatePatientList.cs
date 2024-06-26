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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CreatePatientList;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using HIS.UC.WorkPlace;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.CreatePatientList.Resources;
using HIS.Desktop.Plugins.CreatePatientList.Load;
using HIS.Desktop.Common;
using SDA.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.CreatePatientList
{
    public partial class frmCreatePatientList : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        internal int ActionType = 0;// No action     
        internal HisExamServiceReqRegisterSDO HisExamRegisterSDO { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ EVHisServiceReqDTO = null;
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT currentVHisPatientDTO = null;
        internal MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
        internal HisServiceReqUpdateSDO ServiceReqUpdateSDO { get; set; }
        internal int PatientRowCount = 0;
        long currentPatientId = 0;
        int positionHandleControlPatientInfo = -1;
        RefeshReference refeshReference;
        internal HIS.UC.WorkPlace.UCWorkPlaceCombo workPlacecbo;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor workPlaceProcessor;
        internal HIS.UC.WorkPlace.WorkPlaceProcessor.Template workPlaceTemplate;
        UserControl ucWorkPlace;
        #endregion

        public frmCreatePatientList(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
        }

        #region Load

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CreatePatientList.Resources.Lang", typeof(HIS.Desktop.Plugins.CreatePatientList.frmCreatePatientList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControl1.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.groupControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboEthnic.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboRh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodAbo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboBloodAbo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboIB.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboIB.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboMilitaryRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNation.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboNation.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDistricts.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboDistricts.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmCreatePatientList.cboGender1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciEthnic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareer.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciCareer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdress.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciAdress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciCommune.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistricts.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciDistricts.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciProvince.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctCmnd.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lctCmnd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctDR.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lctDR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctIB.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lctIB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctRh.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lctRh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodAbo.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciBloodAbo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPersonFamily.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciPersonFamily.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRelation.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciRelation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctContact.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lctContact.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNation.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciNation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMilitaryRank.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciMilitaryRank.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmCreatePatientList.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmCreatePatientList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                HisExamRegisterSDO = new HisExamServiceReqRegisterSDO();
                HisExamRegisterSDO.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                ActionType = GlobalVariables.ActionEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            try
            {
                //txtAge.Enabled = false;
                //cboAge.Enabled = false;
                txtPatientName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void FillDataPatientToControl(MOS.EFMODEL.DataModels.V_HIS_PATIENT patientDto)
        //{

        //    try
        //    {

        //        //txtPatientCode.Text = patientDto.PATIENT_CODE;
        //        txtPatientName.Text = patientDto.VIR_PATIENT_NAME;
        //        if (patientDto.DOB > 0 && patientDto.DOB.ToString().Length >= 6)
        //        {
        //            LoadDobPatientToForm(patientDto);
        //        }
        //        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == patientDto.GENDER_ID);
        //        if (gender != null)
        //        {
        //            cboGender1.EditValue = gender.ID;
        //            txtGender.Text = gender.GENDER_CODE;
        //        }
        //        txtAddress.Text = patientDto.ADDRESS;
        //        var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == patientDto.NATIONAL_NAME);
        //        if (national != null)
        //        {
        //            cboNation.EditValue = patientDto.NATIONAL_NAME;
        //            txtNation.Text = national.NATIONAL_CODE;
        //        }
        //        var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == patientDto.ETHNIC_NAME);
        //        if (ethnic != null)
        //        {
        //            cboEthnic.EditValue = patientDto.ETHNIC_NAME;
        //            txtEthnic.Text = ethnic.ETHNIC_CODE;
        //        }

        //        //var bloodabo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD>().FirstOrDefault(o => o.ID == patientDto.BLOOD_ABO_ID);
        //        if (patientDto.CMND_DATE != null)
        //        {
        //            txtDR.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDto.CMND_DATE ?? 0);
        //        }
        //        CommonParam param = new CommonParam();
        //        MOS.Filter.HisBloodAboFilter abofilter = new MOS.Filter.HisBloodAboFilter();
        //        abofilter.ID = patientDto.BLOOD_ABO_ID;
        //        var bloodabo = new BackendAdapter(param).Get<List<HIS_BLOOD_ABO>>(HisRequestUriStore.HIS_BLOOD_ABO__GET, ApiConsumers.MosConsumer, abofilter, param);
        //        if (bloodabo != null && bloodabo.Count > 0 && patientDto.BLOOD_ABO_ID != null)
        //        {
        //            cboBloodAbo.EditValue = patientDto.BLOOD_ABO_ID;
        //            txtBloodAbo.Text = bloodabo.FirstOrDefault().BLOOD_ABO_CODE;
        //        }
        //        MOS.Filter.HisBloodRhFilter rhfilter = new MOS.Filter.HisBloodRhFilter();
        //        rhfilter.ID = patientDto.BLOOD_RH_ID;
        //        var bloodrh = new BackendAdapter(param).Get<List<HIS_BLOOD_RH>>(HisRequestUriStore.HIS_BLOOD_RH__GET, ApiConsumers.MosConsumer, rhfilter, param);
        //        if (bloodrh != null && bloodrh.Count > 0 && patientDto.BLOOD_RH_ID != null)
        //        {
        //            cboRh.EditValue = patientDto.BLOOD_RH_ID;
        //            txtRh.Text = bloodrh.FirstOrDefault().BLOOD_RH_CODE;
        //        }
        //        var career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.ID == patientDto.CAREER_ID);
        //        if (career != null)
        //        {
        //            cboCareer.EditValue = patientDto.CAREER_ID;
        //            txtCareer.Text = career.CAREER_CODE;
        //        }

        //        var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
        //        if (province != null)
        //        {
        //            cboProvince.EditValue = province.PROVINCE_CODE;
        //            txtProvince.Text = province.PROVINCE_CODE;
        //            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
        //        }
        //        var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME && o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
        //        if (district != null)
        //        {
        //            cboDistricts.EditValue = district.DISTRICT_CODE;
        //            txtDistricts.Text = district.DISTRICT_CODE;
        //            LoadCommuneCombo("", district.DISTRICT_CODE, false);
        //        }
        //        var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.COMMUNE_NAME) == patientDto.COMMUNE_NAME && (o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDto.DISTRICT_NAME);// && o.PROVINCE_NAME == patientDto.PROVINCE_NAME);
        //        if (commune != null)
        //        {
        //            cboCommune.EditValue = commune.COMMUNE_CODE;
        //            txtCommune.Text = commune.COMMUNE_CODE;
        //            txtAddress.Text = commune.SEARCH_CODE;
        //            //cboTHX.EditValue = commune.ID;
        //        }

        //        txtMilitaryRankCode.Text = patientDto.MILITARY_RANK_CODE;
        //        cboMilitaryRank.EditValue = patientDto.MILITARY_RANK_ID;
        //        txtPhone.Text = patientDto.PHONE;
        //        txtRelation.Text = patientDto.RELATIVE_TYPE;
        //        txtPersonFamily.Text = patientDto.RELATIVE_NAME;
        //        txtContact.Text = patientDto.RELATIVE_ADDRESS;
        //        txtEmail.Text = patientDto.EMAIL;
        //        txtCmnd.Text = patientDto.CMND_NUMBER;
        //        txtIB.Text = patientDto.CMND_PLACE;

        //        InitWorkPlaceControl();
        //        if (GlobalVariables.CheDoHienThiNoiLamViecManHinhDangKyTiepDon != 1
        //            && workPlaceProcessor != null
        //            && ucWorkPlace != null)
        //        {
        //            if (patientDto.WORK_PLACE_ID > 0)
        //            {
        //                var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == patientDto.WORK_PLACE_ID);
        //                if (workPlace != null)
        //                    workPlaceProcessor.SetValue(ucWorkPlace, workPlace);
        //                else
        //                    workPlaceProcessor.SetValue(ucWorkPlace, null);
        //            }
        //        }
        //        else
        //        {
        //            workPlaceProcessor.SetValue(ucWorkPlace, patientDto.WORK_PLACE);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        /// <summary>
        /// Load giữ liệu lên các combobox
        /// </summary>
        public void FillDataToControlsForm()
        {
            try
            {
                LoadDataFromDbToComboGender(this.cboGender1);
                ProvinceProcessBase.FillDataToLookupedit(this.cboProvince);
                LoadDataToComboCarreer(this.cboCareer);
                LoadDataToComboDanToc(this.cboEthnic);
                LoadDataToComboQuocGia(this.cboNation);
                LoadDataToComboQuanHam(this.cboMilitaryRank);
                LoadDataToComboNhomMau(this.cboBloodAbo);
                LoadDataToComboRH(this.cboRh);
                //LoadDataComboNoiCap(this.cboIB);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDobPatientToForm(MOS.EFMODEL.DataModels.V_HIS_PATIENT patientDTO)
        {
            try
            {
                if (patientDTO != null)
                {
                    string nthnm = patientDTO.DOB.ToString();

                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.MinValue;
                    dtDOB.EditValue = dtNgSinh;
                    txtDOB.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
                    bool isGKS = true;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        //txtAge.EditValue = "";
                        //cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    if (nam > 0)
                    {
                        //txtAge.EditValue = nam.ToString();
                        //cboAge.EditValue = 1;
                        //txtAge.Enabled = false;
                        //cboAge.Enabled = false;
                        if (nam >= 6)
                        {
                            isGKS = false;
                        }
                    }
                    else
                    {
                        if (thang > 0)
                        {
                            //txtAge.EditValue = thang.ToString();
                            //cboAge.EditValue = 2;
                            //txtAge.Enabled = false;
                            //cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                //txtAge.EditValue = ngay.ToString();
                                //cboAge.EditValue = 3;
                                //txtAge.Enabled = false;
                                //cboAge.Enabled = false;
                            }
                            else
                            {
                                //txtAge.EditValue = age.ToString();
                                //cboAge.EditValue = 4;
                                //txtAge.Enabled = true;
                                //cboAge.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadCurrentPatient(long patientId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT currentVHisPatientDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                currentVHisPatientDTO = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, null).ToList().First();

                //currentVHisPatientDTO = current.Where(o=>o.ID==patientId).ToList().First();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdatePatientDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_PATIENT patientDTO)
        {
            //try
            //{ }
            //patientDTO.PATIENT_CODE = txtPatientCode.Text;
            try
            {
                int idx = txtPatientName.Text.Trim().LastIndexOf(" ");
                if (idx > -1)
                {
                    patientDTO.FIRST_NAME = txtPatientName.Text.Trim().Substring(idx).Trim();
                    patientDTO.LAST_NAME = txtPatientName.Text.Trim().Substring(0, idx).Trim();
                }
                else
                {
                    patientDTO.FIRST_NAME = txtPatientName.Text.Trim();
                    patientDTO.LAST_NAME = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Loi xu ly cat chuoi ho ten benh nhan: ", ex);
            }
            try
            {
                //dtDOB.EditValue = EXE.UTILITY.UtilHelper.ConvertDateStringToSystemDate(txtDOB.Text);
                if (dtDOB.DateTime != null)
                {
                    string dateDob = dtDOB.DateTime.ToString("yyyyMMdd");
                    string timeDob = "00";
                    patientDTO.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                }
                if (txtDR.DateTime != null)
                {
                    string dateDR = txtDR.DateTime.ToString("yyyyMMdd");
                    string timeDR = "00";
                    patientDTO.CMND_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(dateDR + timeDR + "0000");
                }
                if (cboGender1.EditValue != null)
                    patientDTO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt16((cboGender1.EditValue ?? "").ToString());
                patientDTO.ADDRESS = txtAddress.Text;
                patientDTO.PROVINCE_NAME = ((cboProvince.Text ?? "").ToString());
                patientDTO.DISTRICT_NAME = ((cboDistricts.Text ?? "").ToString());
                patientDTO.COMMUNE_NAME = ((cboCommune.Text ?? "").ToString());
                if (cboCareer.EditValue != null)
                    patientDTO.CAREER_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? "").ToString());
                else
                    patientDTO.CAREER_ID = null;
                patientDTO.ETHNIC_NAME = ((cboEthnic.Text ?? "").ToString());
                patientDTO.NATIONAL_NAME = ((cboNation.Text ?? "").ToString());
                patientDTO.BLOOD_ABO_CODE = ((cboBloodAbo.Text ?? "").ToString());
                patientDTO.BLOOD_RH_CODE = ((cboRh.Text ?? "").ToString());
                patientDTO.PHONE = txtPhone.Text;
                patientDTO.RELATIVE_NAME = txtPersonFamily.Text;
                patientDTO.RELATIVE_TYPE = txtRelation.Text;
                patientDTO.EMAIL = txtEmail.Text;
                patientDTO.RELATIVE_ADDRESS = txtContact.Text;
                patientDTO.CMND_NUMBER = txtCmnd.Text;
                patientDTO.CMND_PLACE = txtIB.Text;


                if (cboMilitaryRank.EditValue != null)
                {
                    patientDTO.MILITARY_RANK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMilitaryRank.EditValue ?? "").ToString());
                }
                else
                {
                    patientDTO.MILITARY_RANK_ID = null;
                }
                if (workPlaceTemplate == WorkPlaceProcessor.Template.Combo)
                {
                    patientDTO.WORK_PLACE_ID = (long?)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    patientDTO.WORK_PLACE = "";
                }
                else
                {
                    patientDTO.WORK_PLACE = (string)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
                    patientDTO.WORK_PLACE_ID = null;
                }
                if (cboBloodAbo.EditValue != null)
                {
                    patientDTO.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboBloodAbo.EditValue ?? "").ToString());
                }
                else
                {
                    patientDTO.BLOOD_ABO_ID = null;
                }
                if (cboRh.EditValue != null)
                {
                    patientDTO.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRh.EditValue ?? "").ToString());
                }
                else
                {
                    patientDTO.BLOOD_RH_ID = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefeshDataAfterSave()
        {
            try
            {
                if (this.refeshReference != null)
                {
                    this.refeshReference();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        public static void LoadDataFromDbToComboGender(DevExpress.XtraEditors.LookUpEdit cboGender1)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisGenderFilter filter = new MOS.Filter.HisGenderFilter();
                var gender = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_GENDER>>(HisRequestUriStore.HIS_GENDER_GET, ApiConsumers.MosConsumer, filter, param);
                cboGender1.Properties.DataSource = gender;
                cboGender1.Properties.DisplayMember = "GENDER_NAME";
                cboGender1.Properties.ValueMember = "ID";
                cboGender1.Properties.ForceInitialize();

                cboGender1.Properties.Columns.Clear();
                cboGender1.Properties.Columns.Add(new LookUpColumnInfo("GENDER_CODE", "", 50));
                cboGender1.Properties.Columns.Add(new LookUpColumnInfo("GENDER_NAME", "", 50));

                cboGender1.Properties.ShowHeader = false;
                cboGender1.Properties.ImmediatePopup = true;
                cboGender1.Properties.DropDownRows = 3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public class ProvinceProcessBase
        {
            public static void FillDataToLookupedit(DevExpress.XtraEditors.LookUpEdit cboProvince)
            {
                try
                {

                    cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    cboProvince.Properties.DisplayMember = "PROVINCE_NAME";
                    cboProvince.Properties.ValueMember = "PROVINCE_CODE";
                    cboProvince.Properties.ForceInitialize();
                    cboProvince.Properties.Columns.Clear();
                    cboProvince.Properties.Columns.Add(new LookUpColumnInfo("PROVINCE_CODE", "", 50));
                    cboProvince.Properties.Columns.Add(new LookUpColumnInfo("PROVINCE_NAME", "", 100));
                    cboProvince.Properties.ShowHeader = false;
                    cboProvince.Properties.ImmediatePopup = true;
                    cboProvince.Properties.DropDownRows = 20;
                    cboProvince.Properties.PopupWidth = 300;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

            }
            public static void FillDataToGridLookUp(DevExpress.XtraEditors.GridLookUpEdit cbo)
            {
                try
                {
                    cbo.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    cbo.Properties.DisplayMember = "PROVINCE_NAME";
                    cbo.Properties.ValueMember = "ID";

                    cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                    cbo.Properties.ImmediatePopup = true;
                    cbo.ForceInitialize();
                    cbo.Properties.View.Columns.Clear();

                    GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("PROVINCE_CODE");
                    aColumnCode.Caption = "Mã";
                    aColumnCode.Visible = true;
                    aColumnCode.VisibleIndex = 1;
                    aColumnCode.Width = 50;

                    GridColumn aColumnName = cbo.Properties.View.Columns.AddField("PROVINCE_NAME");
                    aColumnName.Caption = "Tên";
                    aColumnName.Visible = true;
                    aColumnName.VisibleIndex = 2;
                    aColumnName.Width = 100;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }

        public static void LoadDataToComboCarreer(DevExpress.XtraEditors.LookUpEdit cboJob)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisCareerFilter filter = new MOS.Filter.HisCareerFilter();
                var career = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CAREER>>(HisRequestUriStore.HIS_CAREER_GET, ApiConsumers.MosConsumer, filter, param);
                cboJob.Properties.DataSource = career;
                cboJob.Properties.DisplayMember = "CAREER_NAME";
                cboJob.Properties.ValueMember = "ID";
                cboJob.Properties.ForceInitialize();
                cboJob.Properties.Columns.Clear();
                cboJob.Properties.Columns.Add(new LookUpColumnInfo("CAREER_CODE", "", 50));
                cboJob.Properties.Columns.Add(new LookUpColumnInfo("CAREER_NAME", "", 100));
                cboJob.Properties.ShowHeader = false;
                cboJob.Properties.ImmediatePopup = true;
                cboJob.Properties.DropDownRows = 10;
                cboJob.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboDanToc(DevExpress.XtraEditors.LookUpEdit cboEthnic)
        {
            try
            {
                cboEthnic.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                cboEthnic.Properties.DisplayMember = "ETHNIC_NAME";
                cboEthnic.Properties.ValueMember = "ETHNIC_NAME";
                cboEthnic.Properties.ForceInitialize();
                cboEthnic.Properties.Columns.Clear();
                cboEthnic.Properties.Columns.Add(new LookUpColumnInfo("ETHNIC_CODE", "", 50));
                cboEthnic.Properties.Columns.Add(new LookUpColumnInfo("ETHNIC_NAME", "", 100));
                cboEthnic.Properties.ShowHeader = false;
                cboEthnic.Properties.ImmediatePopup = true;
                cboEthnic.Properties.DropDownRows = 20;
                cboEthnic.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboQuocGia(DevExpress.XtraEditors.LookUpEdit cboNation)
        {
            try
            {
                cboNation.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                cboNation.Properties.DisplayMember = "NATIONAL_NAME";
                cboNation.Properties.ValueMember = "NATIONAL_NAME";
                cboNation.Properties.ForceInitialize();
                cboNation.Properties.Columns.Clear();
                cboNation.Properties.Columns.Add(new LookUpColumnInfo("NATIONAL_CODE", "", 50));
                cboNation.Properties.Columns.Add(new LookUpColumnInfo("NATIONAL_NAME", "", 100));
                cboNation.Properties.ShowHeader = false;
                cboNation.Properties.ImmediatePopup = true;
                cboNation.Properties.DropDownRows = 10;
                cboNation.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboQuanHam(DevExpress.XtraEditors.LookUpEdit cboMilitaryRank)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMilitaryRankFilter filter = new MOS.Filter.HisMilitaryRankFilter();
                var rank = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>>(HisRequestUriStore.HIS_MILITARY_RANK_GET, ApiConsumers.MosConsumer, filter, param);
                cboMilitaryRank.Properties.DataSource = rank;
                cboMilitaryRank.Properties.DisplayMember = "MILITARY_RANK_CODE";
                cboMilitaryRank.Properties.ValueMember = "ID";
                cboMilitaryRank.Properties.ForceInitialize();
                cboMilitaryRank.Properties.Columns.Clear();
                cboMilitaryRank.Properties.Columns.Add(new LookUpColumnInfo("MILITARY_RANK_CODE", "", 50));
                cboMilitaryRank.Properties.Columns.Add(new LookUpColumnInfo("MILITARY_RANK_NAME", "", 100));
                cboMilitaryRank.Properties.ShowHeader = false;
                cboMilitaryRank.Properties.ImmediatePopup = true;
                cboMilitaryRank.Properties.DropDownRows = 10;
                cboMilitaryRank.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboNhomMau(DevExpress.XtraEditors.LookUpEdit cboBloodAbo)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodViewFilter filter = new MOS.Filter.HisBloodViewFilter();
                var BloodAbo = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD>>(HisRequestUriStore.HIS_BLOOD_ABO__GET, ApiConsumers.MosConsumer, filter, param);
                cboBloodAbo.Properties.DataSource = BloodAbo;
                cboBloodAbo.Properties.DisplayMember = "BLOOD_ABO_CODE";
                cboBloodAbo.Properties.ValueMember = "ID";
                cboBloodAbo.Properties.ForceInitialize();
                cboBloodAbo.Properties.Columns.Clear();
                cboBloodAbo.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_ABO_CODE", "", 30));
                cboBloodAbo.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_ABO_CODE", "", 30));
                cboBloodAbo.Properties.ShowHeader = false;
                cboBloodAbo.Properties.ImmediatePopup = true;
                cboBloodAbo.Properties.DropDownRows = 10;
                cboBloodAbo.Properties.PopupWidth = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToComboRH(DevExpress.XtraEditors.LookUpEdit cboRh)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisBloodRhFilter filter = new MOS.Filter.HisBloodRhFilter();
                var BloodRh = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD>>(HisRequestUriStore.HIS_BLOOD_RH__GET, ApiConsumers.MosConsumer, filter, param);
                cboRh.Properties.DataSource = BloodRh;
                cboRh.Properties.DisplayMember = "BLOOD_RH_CODE";
                cboRh.Properties.ValueMember = "ID";
                cboRh.Properties.ForceInitialize();
                cboRh.Properties.Columns.Clear();
                cboRh.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_RH_CODE", "", 30));
                cboRh.Properties.Columns.Add(new LookUpColumnInfo("BLOOD_RH_CODE", "", 30));
                cboRh.Properties.ShowHeader = false;
                cboRh.Properties.ImmediatePopup = true;
                cboRh.Properties.DropDownRows = 10;
                cboRh.Properties.PopupWidth = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCreatePatientList_Load(object sender, EventArgs e)
        {
            try
            {
                //InitializeComponent();
                WaitingManager.Show();
                ValidateForm();
                SetDefaultData();
                SetCaptionByLanguageKey();
                FillDataToControlsForm();
                InitWorkPlaceControl();
                SetIcon();

                if (this.currentPatientId > 0)
                {
                    //Load Current Patient
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisPatientViewFilter hisPatientFilter = new MOS.Filter.HisPatientViewFilter();
                    hisPatientFilter.ID = currentPatientId;
                    //currentVHisPatientDTO = new HisPatientLogic().Get<List<V_HIS_PATIENT>>(hisPatientFilter).SingleOrDefault();
                    currentVHisPatientDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, hisPatientFilter, param).SingleOrDefault();
                }
                //FillDataPatientToControl(this.currentVHisPatientDTO);
                MilitaryRankLoader.LoadDataCombo(cboMilitaryRank, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>());

                //ValidatePatientForm();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadGioiTinhCombo(strValue, cboGender1, txtGender, dtDOB);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEthnic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadDanTocCombo(strValue, false, cboEthnic, txtEthnic, txtCareer);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                bool check = false;
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadQuocTichCombo(strValue, false, cboNation, txtNation, ref check);
                    if (workPlaceProcessor != null && workPlaceTemplate != null)
                        workPlaceProcessor.FocusControl(workPlaceTemplate);
                    //LoadQuocTichCombo(strValue, false, cboNation, txtNation, txtMilitaryRankCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMilitaryRankCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        cboMilitaryRank.EditValue = null;
                        cboMilitaryRank.Focus();
                        cboMilitaryRank.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.ToUpper().Contains(searchCode)).ToList();
                        if (data != null)
                        {
                            if (data.Count == 1)
                            {
                                cboMilitaryRank.EditValue = data[0].ID;
                                txtPhone.Focus();
                                txtPhone.SelectAll();
                            }
                            else
                            {
                                cboMilitaryRank.EditValue = null;
                                cboMilitaryRank.Focus();
                                cboMilitaryRank.ShowPopup();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
            //        //LoadMilitaryRankCombo(strValue);
            //        LoadRankCombo(strValue, false, cboMilitaryRank, txtMilitaryRankCode, txtPhone);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboGender1_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboGender1.EditValue != null && cboGender1.EditValue != cboGender1.OldEditValue)
                    {
                        MOS.EFMODEL.DataModels.HIS_GENDER gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboGender1.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtGender.Text = gt.GENDER_CODE;
                            dtDOB.Focus();
                            dtDOB.ShowPopup();
                        }
                    }
                    else
                    {
                        //Forcus
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEthnic_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == ((cboEthnic.EditValue ?? "").ToString()));
                        if (ethnic != null)
                        {
                            txtEthnic.Text = ethnic.ETHNIC_CODE;
                            txtCareer.Focus();
                            txtCareer.SelectAll();
                        }
                    }
                    else
                    {
                        txtCareer.Focus();
                        txtCareer.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCareer_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCareer.EditValue != null)
                    {
                        CommonParam param = new CommonParam();
                        HisCareerFilter filter = new HisCareerFilter();
                        MOS.EFMODEL.DataModels.HIS_CAREER career = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CAREER>>("api/HisCareer/Get", ApiConsumers.MosConsumer, filter, param).SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? 0).ToString()));
                        if (career != null)
                        {
                            txtCareer.Text = career.CAREER_CODE;
                            txtAddress.Focus();
                            txtAddress.SelectAll();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        txtAddress.Focus();
                        txtAddress.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboProvince.EditValue != null && cboProvince.EditValue != cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.PROVINCE_CODE;
                        }
                    }
                    txtDistricts.Text = "";
                    txtDistricts.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDistricts.EditValue != null && cboDistricts.EditValue != cboDistricts.OldEditValue)
                    {
                        string str = cboDistricts.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()))
                            {
                                cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            LoadCommuneCombo("", district.DISTRICT_CODE, false);
                            txtDistricts.Text = district.DISTRICT_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                        }
                    }
                    txtCommune.Focus();
                    txtCommune.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (cboDistricts.EditValue != null && cboDistricts.EditValue != cboDistricts.OldEditValue)
            //        {
            //            string str = cboDistricts.EditValue.ToString();
            //            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<V_SDA_DISTRICT>().SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString());
            //            if (district != null)
            //            {
            //                LoadCommuneCombo("", district.DISTRICT_CODE, false);
            //                txtDistricts.Text = district.DISTRICT_CODE;
            //                cboCommune.EditValue = null;
            //                txtCommune.Text = "";
            //                txtCommune.Focus();
            //                txtCommune.SelectAll();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null && cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                                    //&& o.PROVINCE_CODE == cboProvince.EditValue.ToString() 
                                    && (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboDistricts.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtCommune.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                cboDistricts.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    txtNation.Focus();
                    txtNation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (cboCommune.EditValue != null && cboCommune.EditValue != cboCommune.OldEditValue)
            //        {
            //            SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<V_SDA_COMMUNE>().SingleOrDefault(o => o.COMMUNE_CODE == (cboCommune.EditValue.ToString()) && o.DISTRICT_CODE == (cboDistricts.EditValue.ToString()) && o.PROVINCE_CODE == (cboProvince.EditValue.ToString()));
            //            if (commune != null)
            //            {
            //                txtCommune.Text = commune.COMMUNE_CODE;
            //                txtTHX.Text = commune.SEARCH_CODE;
            //                cboTHX.EditValue = commune.ID;
            //                txtCereer.Focus();
            //                txtCereer.SelectAll();
            //            }
            //        }
            //        else
            //        {
            //            txtCereer.Focus();
            //            txtCereer.SelectAll();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboMilitaryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                            txtPhone.Focus();
                            txtPhone.SelectAll();
                        }
                    }
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (cboMilitaryRank.EditValue != null)
            //        {
            //            MOS.EFMODEL.DataModels.HIS_MILITARY_RANK rank = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString()));
            //            if (rank != null)
            //            {
            //                txtMilitaryRankCode.Text = rank.MILITARY_RANK_CODE;
            //                txtPhone.Focus();
            //                txtPhone.SelectAll();
            //            }
            //            else
            //            {
            //                txtPhone.Focus();
            //                txtPhone.SelectAll();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboBloodAbo_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBloodAbo.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_ABO abo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboBloodAbo.EditValue.ToString()));
                        if (abo != null)
                        {
                            txtBloodAbo.Text = abo.BLOOD_ABO_CODE;
                            //cboRh.EditValue = rh.ID;
                            txtRh.Focus();
                            txtRh.SelectAll();
                        }
                    }
                    else
                    {
                        txtRh.Focus();
                        txtRh.SelectAll();
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
            CommonParam param = new CommonParam();
            bool success = false;
            bool valid = true;
            try
            {
                this.positionHandleControlPatientInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.V_HIS_PATIENT currentPatientDTO = new MOS.EFMODEL.DataModels.V_HIS_PATIENT();
                //LoadCurrentPatient(this.currentVHisPatientDTO.ID, ref currentPatientDTO);
                UpdatePatientDTOFromDataForm(ref currentPatientDTO);
                var resultData = new BackendAdapter(param).Post<V_HIS_PATIENT>(HisRequestUriStore.HIS_PATIENT_CREATE, ApiConsumers.MosConsumer, currentPatientDTO, param);
                if (resultData != null)
                {
                    success = true;
                    WaitingManager.Hide();
                    SuccessLog(resultData);
                    this.Close();
                    RefeshDataAfterSave();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
            MessageManager.Show(this, param, success);
        }

        void SuccessLog(V_HIS_PATIENT result)
        {
            try
            {
                if (result != null)
                {
                    string message = String.Format(HIS.Desktop.EventLog.EventLogUtil.SetLog(His.EventLog.Message.Enum.SuaThongTinBenhNhan), result.PATIENT_CODE, result.VIR_PATIENT_NAME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result.DOB), cboGender1.Text, currentHisPatientTypeAlter.TREATMENT_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_NAME);
                    His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
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

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboMilitaryRank_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
            //        LoadMilitaryRankCombo(strValue);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProvince.EditValue != null)
                    {
                        string str = cboProvince.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            EventProcessor.GetNotInListValue(sender, e, cboDistricts);
        }

        private void cboCommune_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            EventProcessor.GetNotInListValue(sender, e, cboCommune);
        }

        private void cboNation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                //    LoadQuocTichCombo(strValue, false, cboNation, txtNation, pnlWorkPlace);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboIB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (cboIB.EditValue != null)
        //            {
        //                string str = cboIB.EditValue.ToString();
        //                SDA.EFMODEL.DataModels.SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_NAME == cboIB.EditValue.ToString());
        //                if (cboIB != null)
        //                {
        //                    //LoadDistrictsCombo("", province.PROVINCE_CODE, false);
        //                    txtIB.Text = province.PROVINCE_CODE;
        //                    cboIB.Text = province.PROVINCE_NAME;
        //                    cboIB.EditValue = province.PROVINCE_NAME;
        //                    txtDistricts.Text = "";
        //                    txtDistricts.Focus();
        //                }
        //            }
        //        }
        //        else if (e.KeyCode == Keys.Delete)
        //        {
        //            cboIB.EditValue = null;
        //            //e.Handled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboIB_Closed(object sender, ClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.CloseMode == PopupCloseMode.Normal)
        //        {
        //            if (cboIB.EditValue != null)
        //            {
        //                SDA.EFMODEL.DataModels.V_SDA_PROVINCE ib = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(((cboIB.EditValue ?? 0).ToString())));
        //                if (ib != null)
        //                {
        //                    txtIB.Text = ib.PROVINCE_CODE;
        //                    cboIB.EditValue = ib.PROVINCE_NAME;
        //                    txtIB.Focus();
        //                    txtIB.SelectAll();
        //                }
        //                else
        //                {
        //                    txtIB.Focus();
        //                    txtIB.SelectAll();
        //                }
        //            }
        //            else
        //            {
        //                txtIB.Focus();
        //                txtIB.SelectAll();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtIB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBloodAbo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void cboIB_KeyUp(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (cboIB.EditValue != null)
        //            {
        //                SDA.EFMODEL.DataModels.V_SDA_PROVINCE ib = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == ((cboIB.EditValue ?? "").ToString()));
        //                if (ib != null)
        //                {
        //                    txtIB.Text = ib.PROVINCE_CODE;
        //                    //txtNation.Focus();
        //                    //txtNation.SelectAll();
        //                }
        //            }
        //        }
        //        else if (e.KeyCode == Keys.Delete)
        //        {
        //            cboIB.EditValue = null;
        //            txtIB.Text = "";
        //            //txtNation.Focus();
        //            //txtNation.SelectAll();
        //            e.Handled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboRh_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRh.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_BLOOD_RH rh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRh.EditValue.ToString()));
                        if (rh != null)
                        {
                            txtRh.Text = rh.BLOOD_RH_CODE;
                            //cboRh.EditValue = rh.ID;
                            txtPersonFamily.Focus();
                            txtPersonFamily.SelectAll();
                        }
                    }
                    else
                    {
                        txtPersonFamily.Focus();
                        txtPersonFamily.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNation_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNation.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == ((cboNation.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtNation.Text = data.NATIONAL_CODE;
                            if (workPlaceProcessor != null && workPlaceTemplate != null)
                                workPlaceProcessor.FocusControl(workPlaceTemplate);
                            //workPlaceProcessor.FocusControl(workPlaceTemplate);
                        }
                    }
                    else
                    {
                        //txtNation.Text = data.NATIONAL_CODE;
                        if (workPlaceProcessor != null && workPlaceTemplate != null)
                            workPlaceProcessor.FocusControl(workPlaceTemplate);
                        //workPlaceProcessor.FocusControl(workPlaceTemplate);
                    }
                }
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

        private void txtAddress_Closed(object sender, ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (txtAddress.EditValue != null)
            //        {
            //            VSdaCommuneADO commune = SdaDataLocalStore.SdaCommuneSDOs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((txtAddress.EditValue ?? 0).ToString()));
            //            if (commune != null)
            //            {
            //                var districtDTO = SdaDataLocalStore.VSdaDistricts.FirstOrDefault(o => o.ID == commune.DISTRICT_ID);
            //                if (districtDTO != null)
            //                {
            //                    LoadDistrictsCombo("", districtDTO.PROVINCE_CODE, false);
            //                    cboProvince.EditValue = districtDTO.PROVINCE_CODE;
            //                    txtProvince.Text = districtDTO.PROVINCE_CODE;
            //                }
            //                LoadCommuneCombo("", commune.DISTRICT_CODE, false);
            //                //txtTHX.Text = commune.SEARCH_CODE;
            //                cboDistricts.EditValue = commune.DISTRICT_CODE;
            //                txtDistricts.Text = commune.DISTRICT_CODE;
            //                cboCommune.EditValue = commune.COMMUNE_CODE;
            //                txtCommune.Text = commune.COMMUNE_CODE;
            //            }

            //            txtAddress.Focus();
            //            txtAddress.SelectAll();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
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

        private void txtContact_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
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

        private void txtPatientName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtGender.Focus();
                e.Handled = true;
            }
        }

        private void txtGender_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    cboGender1.Focus();
            //    e.Handled = true;
            //}
        }

        private void txtContact_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //          Keys.Tab;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboBloodAbo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadABOCombo(strValue, false, cboBloodAbo, txtBloodAbo, txtRh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGender.Focus();
                    txtGender.SelectAll();
                }
                if (e.KeyCode == Keys.Tab)
                {
                    e = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBloodAbo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadABOCombo(strValue, false, cboBloodAbo, txtBloodAbo, txtRh);
                    //cboRh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text;
            //        if (String.IsNullOrEmpty(searchCode))
            //        {
            //            cboBloodAbo.EditValue = null;
            //            cboBloodAbo.Focus();
            //            cboBloodAbo.ShowPopup();
            //        }
            //        else
            //        {
            //            var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().Where(o => o.BLOOD_ABO_CODE.Contains(searchCode)).ToList();
            //            if (data != null)
            //            {
            //                if (data.Count == 1)
            //                {
            //                    cboBloodAbo.EditValue = data[0].ID;
            //                    txtRh.Focus();
            //                    txtRh.SelectAll();
            //                }
            //                else
            //                {
            //                    cboBloodAbo.EditValue = null;
            //                    cboBloodAbo.Focus();
            //                    cboBloodAbo.ShowPopup();
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtRh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadRHCombo(strValue, false, cboRh, txtRh, txtPersonFamily);
                    //cboRh.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCareer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadNgheNghiepCombo(strValue, false, cboCareer, txtCareer, txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPersonFamily_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtRelation_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEmail.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        txtCmnd.Focus();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtCmnd_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtDR_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtDR.EditValue != null)
                    {
                        txtIB.Focus();
                        txtIB.SelectAll();
                    }
                    else
                    {
                        txtDR.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    //LoadTinhCombo(strValue, false, cboProvince, txtProvince, txtDistricts);
                    LoadProvinceCombo(strValue.ToUpper(), true);
                    //txtDistricts.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    string provinceCode = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceCode = cboProvince.EditValue.ToString();
                    }
                    LoadDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                    //string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    //LoadHuyenCombo(strValue, false, cboDistricts, txtDistricts, txtCommune);
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
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    string districtCode = "";
                    if (cboDistricts.EditValue != null)
                    {
                        districtCode = cboDistricts.EditValue.ToString();
                    }
                    LoadCommuneCombo(strValue.ToUpper(), districtCode, true);
                    //string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    //LoadXaCombo(strValue, false, cboCommune, txtCommune, txtNation);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCmnd.Focus();
            }
        }

        private void txtCmnd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDR.Focus();
                    txtDR.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNation_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                bool check = false;
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadQuocTichCombo(strValue, false, cboNation, txtNation, ref check);
                    if (check)
                    {
                        if (workPlaceProcessor != null && workPlaceTemplate != null)
                            workPlaceProcessor.FocusControl(workPlaceTemplate);
                    }
                    //LoadQuocTichCombo(strValue, false, cboNation, txtNation, txtMilitaryRankCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDR_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtIB.Focus();
            }

        }

        private void txtDR_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtDR_Closed(object sender, ClosedEventArgs e)
        {

        }

        private void cboRh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadRHCombo(strValue, false, cboRh, txtRh, txtPersonFamily);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPersonFamily_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtPersonFamily_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelation.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRelation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtContact.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCareer_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {

        }

        private void txtContact_Enter(object sender, EventArgs e)
        {
            //btnSave.Focus();
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        btnSave.Focus();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtContact_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        btnSave.Focus();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
            //        LoadHuyenCombo(strValue, false, cboDistricts, txtDistricts, txtCommune);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtProvince.Text))
                {
                    cboProvince.Properties.DataSource = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_MILITARY_RANK commune = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString()));
                        if (commune != null)
                        {
                            txtMilitaryRankCode.Text = commune.MILITARY_RANK_CODE;
                            //txtPhone.Focus();
                            //txtPhone.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validate

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtPatientName);
                ValidationSingleControl(dtDOB);
                ValidateLookupWithTextEdit(cboGender1, txtGender);
                //ValidationSingleControl1();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl1()
        {
            try
            {
                //ValidatespMax validate = new ValidatespMax();
                //validate.spMax = spMaxCapacity;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                //validate.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderEditorInfo.SetValidationRule(spMaxCapacity, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void dtDOB_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtDR_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtDOB.EditValue != null)
                    {
                        txtEthnic.Focus();
                        txtEthnic.SelectAll();
                    }
                    else
                        dtDOB.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAllergic_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ScnAllergic").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ScnAllergic");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    //var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
