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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PatientUpdate.Resources;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            try
            {
                MemoryStream memory = new MemoryStream();
                var bitMap = new System.Drawing.Bitmap(imageIn);
                bitMap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memory.ToArray();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// TH1: Chức năng được mở từ chức năng "Hồ sơ bệnh nhân" 
        /// Api: HisPatient/UpdateSdo
        /// Input: HisPatientUpdateSDO
        /// Output: HIS_PATIENT
        /// Thêm checkbox "Sửa HSĐT mới nhất" mặc định không check
        /// Nếu người dùng check vào checkbox trên thì set trương UpdateTreatment trong HisPatientUpdateSDO= true
        /// 
        /// TH2: Chức năng được mở từ chức năng "Hồ Sơ Điều trị" 
        /// Api: HisTreatment/UpdatePatientInfo
        /// Input: HisTreatmentPatientInfoSDO
        /// Output: HIS_TREATMENT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControlPatientInfo = -1;
                if (!layoutControlItem1.Visible)
                    dxValidationProvider1.SetValidationRule(cboClassify, null);
                if (!dxValidationProvider1.Validate())
                {
                    return;
                }
                if (!string.IsNullOrEmpty(txtPatientDob.ErrorText))
                    return;

                if (Inventec.Common.String.CountVi.Count(txtCCCD_CMTNumber.Text.Trim()) == 12)
                {
                    Int64 k;
                    bool isNumeric = Int64.TryParse(txtCCCD_CMTNumber.Text, out k);
                    if (isNumeric == false)
                    {
                        XtraMessageBox.Show("Trường CCCD chỉ cho phép nhập 12 ký tự số. Vui lòng nhập lại!", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtPatientDob.ErrorText))
                    return;

                bool IsSendTreatmentId = true;

                HIS_TREATMENT treatment = null;
                if (this.TreatmentId > 0)
                {
                    HisTreatmentFilter tFilter = new HisTreatmentFilter();
                    tFilter.ID = this.TreatmentId;
                    var treatmentList = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, tFilter, null);
                    if (treatmentList != null && treatmentList.Count > 0)
                    {
                        treatment = treatmentList[0];
                    }
                }
                else
                {
                    LoadTreatmentLast(PatientId, ref treatment);
                }

                if (treatment != null && (treatment.IS_TEMPORARY_LOCK == 1 || treatment.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                {
                    if (XtraMessageBox.Show("Hồ sơ điều trị đã khóa (tạm khóa) viện phí. Hệ thống sẽ chỉ cập nhật thông tin của bệnh nhân (để có thể sử dụng trong các lần tiếp đón sau) mà không cập nhật thông tin bệnh nhân trong hồ sơ điều trị hiện tại. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    IsSendTreatmentId = false;
                }
                WaitingManager.Show();

                //Nếu mở từ hồ sơ bệnh nhân thì currentPatient!=null và không có TreatmentId trong HisPatientUpdateSDO
                //Nếu mở từ hồ sơ điều trị thì TreatmentId !=null và có TreatmentId trong HisPatientUpdateSDO

                this.patientUpdateSdo = new MOS.SDO.HisPatientUpdateSDO();
                MOS.EFMODEL.DataModels.HIS_PATIENT currentPatientDTO = new MOS.EFMODEL.DataModels.HIS_PATIENT();

                LoadCurrentPatient(this.PatientId, ref currentPatientDTO);
                UpdatePatientDTOFromDataForm(ref currentPatientDTO);

                this.patientUpdateSdo.HisPatient = currentPatientDTO;

                if (this.currentPatient != null)
                {
                    this.patientUpdateSdo.UpdateTreatment = chkUpdateNew.Checked;
                }

                else if (this.TreatmentId != null && IsSendTreatmentId)
                {
                    this.patientUpdateSdo.TreatmentId = this.TreatmentId;
                }

                this.patientUpdateSdo.IsUpdateVaccinationExam = chkUpdate.Checked ? true : false;

                this.patientUpdateSdo.IsUpdateEmr = chkEmrUpdate.Checked ? true : false;
                if (pictureBox1.Tag != "NoImage")
                {
                    this.patientUpdateSdo.ImgAvatarData = ImageToByteArray(pictureBox1.Image);
                }
                else
                {
                    this.patientUpdateSdo.ImgAvatarData = null;
                }

                if (pictureBox2.Tag != "NoImage")
                {
                    this.patientUpdateSdo.ImgBhytData = ImageToByteArray(pictureBox2.Image);
                }
                else
                {
                    this.patientUpdateSdo.ImgBhytData = null;
                }

                if ((string.IsNullOrEmpty(this.patientUpdateSdo.HisPatient.PROVINCE_CODE) && (!string.IsNullOrEmpty(this.patientUpdateSdo.HisPatient.DISTRICT_CODE) || !string.IsNullOrEmpty(this.patientUpdateSdo.HisPatient.COMMUNE_NAME))) || (string.IsNullOrEmpty(this.patientUpdateSdo.HisPatient.DISTRICT_CODE) && (!string.IsNullOrEmpty(this.patientUpdateSdo.HisPatient.COMMUNE_NAME))))
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Nhập thiếu thông tin tỉnh huyện", "Thông báo");
                    return;
                }

                Inventec.Common.Logging.LogSystem.Info("patientUpdateSdo: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientUpdateSdo), patientUpdateSdo));

                var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, this.patientUpdateSdo, param);
                if (resultData != null)
                {
                    ProcessPrint();
                    success = true;
                    WaitingManager.Hide();
                    this.InitThreadPrint();
                    this.Close();
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

        private void InitThreadPrint()
        {
            try
            {
                if (refeshReference != null)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(this.RunRefresh);

                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RunRefresh()
        {
            try
            {
                List<HIS_TREATMENT> _Treatments = new List<HIS_TREATMENT>();
                if (this.currentPatient != null)
                {
                    HisTreatmentFilter tFilter = new HisTreatmentFilter();
                    tFilter.PATIENT_ID = this.currentPatient.ID;
                    tFilter.ORDER_DIRECTION = "DESC";
                    tFilter.ORDER_FIELD = "MODIFY_TIME";
                    _Treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, tFilter, null);
                }
                else if (this.TreatmentId != null)
                {
                    HisTreatmentView4Filter tFilter = new HisTreatmentView4Filter();
                    tFilter.ID = this.TreatmentId;
                    _Treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, tFilter, null);
                }

                this.refeshReference(_Treatments);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void UpdateDataForTreatmentPatientInfoSdo()
        //{
        //    try
        //    {
        //        //HIS_TREATMENT treatmentMap = new HIS_TREATMENT();
        //        //Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatmentMap, this.currentTreatment);

        //        treatmentPatientInfoSdo.PatientStoreCode = txtPatientStoreCode.Text.Trim();
        //        treatmentPatientInfoSdo.PathologicalHistory = txtTienSuBenh.Text;
        //        treatmentPatientInfoSdo.PathologicalHistoryFamily = txtTienSuGiaDinh.Text;
        //        treatmentPatientInfoSdo.IsUpdateEmr = chkEmrUpdate.Checked ? true : false;

        //        if (cboClassify.EditValue != null && cboClassify.EditValue.ToString() != "")
        //        {
        //            //treatmentMap.TDL_PATIENT_CLASSIFY_ID = (long)cboClassify.EditValue;
        //            treatmentPatientInfoSdo.ClassifyId = (long)cboClassify.EditValue;
        //        }
        //        else
        //        {
        //            //treatmentMap.TDL_PATIENT_CLASSIFY_ID = null;
        //            treatmentPatientInfoSdo.ClassifyId = null;
        //        }

        //        if (cboCareer.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.CareerId = Inventec.Common.TypeConvert.Parse.ToInt64((cboCareer.EditValue ?? "0").ToString());
        //            //treatmentMap.TDL_PATIENT_CAREER_NAME = cboCareer.Text;
        //            treatmentPatientInfoSdo.CareerName = cboCareer.Text;
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.CareerId = null;
        //            treatmentPatientInfoSdo.CareerName = "";
        //            //treatmentMap.TDL_PATIENT_CAREER_NAME = "";
        //        }

        //        if (cboMilitaryRank.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.MilitaryRankId = Inventec.Common.TypeConvert.Parse.ToInt64(cboMilitaryRank.EditValue.ToString());
        //            treatmentPatientInfoSdo.MilitaryRankName = cboMilitaryRank.Text;
        //            //treatmentMap.TDL_PATIENT_MILITARY_RANK_NAME = cboMilitaryRank.Text;
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.MilitaryRankId = null;
        //            treatmentPatientInfoSdo.MilitaryRankName = "";
        //            //treatmentMap.TDL_PATIENT_MILITARY_RANK_NAME = "";
        //        }

        //        if (cboDistricts.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.DistrictName = cboDistricts.Text;
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.DistrictName = "";
        //        }

        //        int idx = txtPatientName.Text.Trim().LastIndexOf(" ");
        //        if (idx > -1)
        //        {
        //            treatmentPatientInfoSdo.FirstName = txtPatientName.Text.Trim().Substring(idx).Trim();
        //            treatmentPatientInfoSdo.LastName = txtPatientName.Text.Trim().Substring(0, idx).Trim();
        //            //treatmentMap.TDL_PATIENT_FIRST_NAME = txtPatientName.Text.Trim().Substring(idx).Trim();
        //            //treatmentMap.TDL_PATIENT_LAST_NAME = txtPatientName.Text.Trim().Substring(0, idx).Trim();
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.FirstName = txtPatientName.Text.Trim();
        //            treatmentPatientInfoSdo.LastName = "";
        //            //treatmentMap.TDL_PATIENT_FIRST_NAME = txtPatientName.Text.Trim();
        //            //treatmentMap.TDL_PATIENT_LAST_NAME = "";
        //        }

        //        if (this.dtPatientDob.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.Dob = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtPatientDob.DateTime) ?? 0;
        //            //treatmentMap.TDL_PATIENT_DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtPatientDob.DateTime) ?? 0;
        //        }
        //        else
        //        {
        //            DateUtil.DateValidObject dateValidObject = DateUtil.ValidPatientDob(this.txtPatientDob.Text);
        //            if (dateValidObject != null && dateValidObject.HasNotDayDob)
        //            {
        //                this.dtPatientDob.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(dateValidObject.OutDate);
        //                this.dtPatientDob.Update();
        //            }
        //        }

        //        if (this.isNotPatientDayDob)
        //        {
        //            treatmentPatientInfoSdo.IsHasNotDayDob = 1;
        //            //treatmentMap.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = 1;
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.IsHasNotDayDob = null;
        //            //treatmentMap.TDL_PATIENT_IS_HAS_NOT_DAY_DOB = null;
        //        }

        //        if (cboGender1.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.GenderId = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender1.EditValue.ToString());
        //            //treatmentMap.TDL_PATIENT_GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender1.EditValue.ToString());
        //            var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == treatmentPatientInfoSdo.GenderId);
        //            if (gender != null)
        //            {
        //                treatmentPatientInfoSdo.GenderName = gender.GENDER_NAME;
        //                //treatmentMap.TDL_PATIENT_GENDER_NAME = gender.GENDER_NAME;
        //            }
        //        }

        //        treatmentPatientInfoSdo.Address = txtAddress.Text;

        //        if (cboProvince.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.ProvinceName = cboProvince.Text;
        //            treatmentPatientInfoSdo.ProvinceCode = cboProvince.EditValue.ToString();
        //            //treatmentMap.TDL_PATIENT_PROVINCE_CODE = cboProvince.EditValue.ToString();
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.ProvinceName = "";
        //            treatmentPatientInfoSdo.ProvinceCode = "";
        //            //treatmentMap.TDL_PATIENT_PROVINCE_CODE = "";
        //        }

        //        if (cboDistricts.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.DistrictName = cboDistricts.Text;
        //            treatmentMap.TDL_PATIENT_DISTRICT_CODE = cboDistricts.EditValue.ToString();
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.DistrictName = "";
        //            treatmentMap.TDL_PATIENT_DISTRICT_CODE = "";
        //        }

        //        if (cboCommune.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.CommuneName = cboCommune.Text;
        //            treatmentMap.TDL_PATIENT_COMMUNE_CODE = cboCommune.EditValue.ToString();
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.CommuneName = "";
        //            treatmentMap.TDL_PATIENT_COMMUNE_CODE = "";
        //        }

        //        if (cboEthnic.EditValue != null)
        //        {
        //            var ethnic = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == cboEthnic.EditValue.ToString());
        //            if (ethnic != null)
        //            {
        //                treatmentPatientInfoSdo.EthnicName = ethnic.ETHNIC_NAME;
        //                treatmentPatientInfoSdo.EthnicCode = ethnic.ETHNIC_CODE;
        //            }
        //        }

        //        if (cboNation.EditValue != null)
        //        {
        //            var nation = BackendDataWorker.Get<SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == cboNation.EditValue.ToString());
        //            if (nation != null)
        //            {
        //                treatmentPatientInfoSdo.NationalCode = nation.NATIONAL_CODE;
        //                treatmentPatientInfoSdo.NationalName = nation.NATIONAL_NAME;
        //                treatmentMap.TDL_PATIENT_NATIONAL_NAME = nation.NATIONAL_NAME;
        //            }
        //        }

        //        treatmentMap.TDL_SOCIAL_INSURANCE_NUMBER = txtSocialInsuranceNumber.Text.Trim();

        //        treatmentMap.TDL_PATIENT_PHONE = txtPhone.Text;
        //        treatmentPatientInfoSdo.Phone = txtPhone.Text;
        //        treatmentPatientInfoSdo.Email = txtEmail.Text;
        //        treatmentMap.IS_CHRONIC = (short)(chkBNManTinh.Checked ? 1 : 0);
        //        treatmentPatientInfoSdo.TaxCode = txtTaxCode.Text;
        //        treatmentMap.TDL_PATIENT_TAX_CODE = txtTaxCode.Text;

        //        if (workPlaceTemplate == WorkPlaceProcessor.Template.Combo)
        //        {
        //            treatmentPatientInfoSdo.WorkPlaceId = (long?)workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate);
        //            var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == treatmentPatientInfoSdo.WorkPlaceId);
        //            if (workPlace != null)
        //                treatmentMap.TDL_PATIENT_WORK_PLACE_NAME = treatmentPatientInfoSdo.WorkPlaceId == null ? "" : workPlace.WORK_PLACE_NAME;
        //        }
        //        else if (workPlaceTemplate == WorkPlaceProcessor.Template.Textbox)
        //        {
        //            treatmentMap.TDL_PATIENT_WORK_PLACE_NAME = workPlaceProcessor.GetValue(ucWorkPlace, workPlaceTemplate).ToString();
        //            treatmentPatientInfoSdo.WorkPlaceId = null;
        //        }

        //        treatmentMap.TDL_PATIENT_WORK_PLACE = txtOtherAddress.Text;
        //        treatmentPatientInfoSdo.WorkPlace = txtOtherAddress.Text;
        //        treatmentPatientInfoSdo.AccountNumber = txtAccountNumber.Text;
        //        if (cboBloodABOCode.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.BloodAboCode = cboBloodABOCode.EditValue.ToString();
        //        }
        //        else
        //            treatmentPatientInfoSdo.BloodAboCode = "";

        //        if (cboBloodRHCode.EditValue != null)
        //        {
        //            treatmentPatientInfoSdo.BloodRhCode = cboBloodRHCode.EditValue.ToString();
        //        }
        //        else
        //            treatmentPatientInfoSdo.BloodRhCode = "";

        //        treatmentPatientInfoSdo.HtAddres = txtHTAddress.Text;
        //        if (cboHTProvinceName.EditValue != null)
        //        {
        //            var province = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == cboHTProvinceName.EditValue.ToString());
        //            if (province != null)
        //            {
        //                treatmentPatientInfoSdo.HtProvinceName = province.PROVINCE_NAME;
        //            }
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.HtProvinceName = "";
        //        }

        //        if (cboHTDistrictName.EditValue != null)
        //        {
        //            var district = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == cboHTDistrictName.EditValue.ToString());
        //            if (district != null)
        //            {
        //                treatmentPatientInfoSdo.HtDistrictName = district.DISTRICT_NAME;
        //            }
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.HtDistrictName = "";
        //        }

        //        if (cboHTCommuneName.EditValue != null)
        //        {
        //            var commune = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == cboHTCommuneName.EditValue.ToString());
        //            if (commune != null)
        //            {
        //                treatmentPatientInfoSdo.HtCommuneName = commune.COMMUNE_NAME;
        //            }
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.HtCommuneName = "";
        //        }

        //        treatmentPatientInfoSdo.FatherName = txtFatherName.Text.Trim();
        //        treatmentPatientInfoSdo.MotherName = txtMotherName.Text.Trim();
        //        treatmentPatientInfoSdo.MotherCareer = txtMotherCareer.Text.Trim();
        //        treatmentPatientInfoSdo.MotherEducationalLevel = txtMotherEducationalLevel.Text.Trim();
        //        treatmentPatientInfoSdo.FatherCareer = txtFatherCareer.Text.Trim();
        //        treatmentPatientInfoSdo.FatherEducationalLevel = txtFatherEducationalLevel.Text.Trim();
        //        treatmentPatientInfoSdo.RelativeMobile = txtRelativeMobile.Text.Trim();
        //        treatmentPatientInfoSdo.RelativePhone = txtRelativePhone.Text.Trim();
        //        treatmentPatientInfoSdo.RelativeName = txtPersonFamily.Text.Trim();
        //        treatmentPatientInfoSdo.RelativeType = txtRelation.Text;
        //        treatmentPatientInfoSdo.RelativeAddress = txtContact.Text;
        //        treatmentPatientInfoSdo.RelativeCmndNumber = txtCMNDRelative.Text;
        //        treatmentMap.TDL_PATIENT_RELATIVE_NAME = txtPersonFamily.Text.Trim();
        //        treatmentMap.TDL_PATIENT_RELATIVE_TYPE = txtRelation.Text.Trim();
        //        treatmentMap.TDL_PATIENT_ACCOUNT_NUMBER = txtAccountNumber.Text;

        //        if (!String.IsNullOrEmpty(txtCCCD_CMTNumber.Text))
        //        {
        //            if (Inventec.Common.String.CountVi.Count(txtCCCD_CMTNumber.Text.Trim()) == 12)
        //            {
        //                if (txtCCCD_CMTDate.EditValue != null)
        //                    treatmentPatientInfoSdo.CccdDate = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
        //                else
        //                    treatmentPatientInfoSdo.CccdDate = null;
        //                treatmentPatientInfoSdo.CccdNumber = txtCCCD_CMTNumber.Text.Trim();
        //                treatmentPatientInfoSdo.CccdPlace = txtCCCD_CMTPlace.Text.Trim();
        //                treatmentPatientInfoSdo.CmndDate = null;
        //                treatmentPatientInfoSdo.CmndNumber = "";
        //                treatmentPatientInfoSdo.CmndPlace = "";
        //            }
        //            else
        //            {
        //                treatmentPatientInfoSdo.CccdDate = null;
        //                treatmentPatientInfoSdo.CccdNumber = "";
        //                treatmentPatientInfoSdo.CccdPlace = "";
        //                if (txtCCCD_CMTDate.EditValue != null)
        //                    treatmentPatientInfoSdo.CmndDate = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
        //                else
        //                    treatmentPatientInfoSdo.CmndDate = null;
        //                treatmentPatientInfoSdo.CmndNumber = txtCCCD_CMTNumber.Text.Trim();
        //                treatmentPatientInfoSdo.CmndPlace = txtCCCD_CMTPlace.Text.Trim();
        //            }
        //        }
        //        else
        //        {
        //            treatmentPatientInfoSdo.CccdDate = null;
        //            treatmentPatientInfoSdo.CccdNumber = "";
        //            treatmentPatientInfoSdo.CccdPlace = "";
        //            treatmentPatientInfoSdo.CmndDate = null;
        //            treatmentPatientInfoSdo.CmndNumber = "";
        //            treatmentPatientInfoSdo.CmndPlace = "";
        //        }

        //        // Luu vao treatment
        //        if (!String.IsNullOrEmpty(txtCCCD_CMTNumber.Text))
        //        {
        //            if (Inventec.Common.String.CountVi.Count(txtCCCD_CMTNumber.Text.Trim()) == 12)
        //            {
        //                if (txtCCCD_CMTDate.EditValue != null)
        //                    treatmentMap.TDL_PATIENT_CCCD_DATE = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
        //                else
        //                    treatmentMap.TDL_PATIENT_CCCD_DATE = null;
        //                treatmentMap.TDL_PATIENT_CCCD_NUMBER = txtCCCD_CMTNumber.Text.Trim();
        //                treatmentMap.TDL_PATIENT_CCCD_PLACE = txtCCCD_CMTPlace.Text.Trim();
        //                treatmentMap.TDL_PATIENT_CMND_DATE = null;
        //                treatmentMap.TDL_PATIENT_CMND_NUMBER = "";
        //                treatmentMap.TDL_PATIENT_CMND_PLACE = "";
        //            }
        //            else
        //            {
        //                treatmentMap.TDL_PATIENT_CCCD_DATE = null;
        //                treatmentMap.TDL_PATIENT_CCCD_NUMBER = "";
        //                treatmentMap.TDL_PATIENT_CCCD_PLACE = "";
        //                if (txtCCCD_CMTDate.EditValue != null)
        //                    treatmentMap.TDL_PATIENT_CMND_DATE = Convert.ToInt64(txtCCCD_CMTDate.DateTime.ToString("yyyyMMdd") + "000000");
        //                else
        //                    treatmentMap.TDL_PATIENT_CMND_DATE = null;
        //                treatmentMap.TDL_PATIENT_CMND_NUMBER = txtCCCD_CMTNumber.Text.Trim();
        //                treatmentMap.TDL_PATIENT_CMND_PLACE = txtCCCD_CMTPlace.Text.Trim();
        //            }
        //        }
        //        else
        //        {
        //            treatmentMap.TDL_PATIENT_CCCD_DATE = null;
        //            treatmentMap.TDL_PATIENT_CCCD_NUMBER = "";
        //            treatmentMap.TDL_PATIENT_CCCD_PLACE = "";
        //            treatmentMap.TDL_PATIENT_CMND_DATE = null;
        //            treatmentMap.TDL_PATIENT_CMND_NUMBER = "";
        //            treatmentMap.TDL_PATIENT_CMND_PLACE = "";
        //        }


        //        if (cboTonGiao.EditValue != null)
        //        {
        //            var itemdata = BackendDataWorker.Get<SDA_RELIGION>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboTonGiao.EditValue));
        //            if (itemdata != null)
        //                treatmentPatientInfoSdo.ReligionName = itemdata.RELIGION_NAME;
        //        }

        //        treatmentPatientInfoSdo.HisTreatment = treatmentMap;
        //        treatmentPatientInfoSdo.UuidBhytNumber = txtTheBHYT.Text.Trim();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void txtGender_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadGioiTinhCombo(strValue, cboGender1, txtGender, txtPatientDob);
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
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadQuocTichCombo(strValue, false, cboNation, txtNation, pnlWorkPlace);
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
                        FocusShowPopup(this.cboMilitaryRank, gridView9);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().Where(o => o.MILITARY_RANK_CODE.ToUpper().Contains(searchCode)).ToList();
                        List<HIS_MILITARY_RANK> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.MILITARY_RANK_CODE.ToUpper() == searchCode).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboMilitaryRank.EditValue = result[0].ID;
                            FocusMoveText(this.txtPhone);
                        }
                        else
                        {
                            cboMilitaryRank.EditValue = null;
                            FocusShowPopup(this.cboMilitaryRank, gridView9);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                            if (txtPatientCode.Enabled)
                            {
                                txtPatientDob.Focus();
                                txtPatientDob.SelectAll();
                            }
                            else
                            {
                                txtNation.Focus();
                                txtNation.SelectAll();
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

        private void cboEthnic_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEthnic.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_ETHNIC ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == ((cboEthnic.EditValue ?? "").ToString()));
                        if (ethnic != null)
                        {
                            txtEthnic.Text = ethnic.ETHNIC_CODE;
                        }
                    }
                    FocusMoveText(this.cboTonGiao);
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
                        cboCareer.Properties.Buttons[1].Visible = true;
                        var careerId = Inventec.Common.TypeConvert.Parse.ToInt64(cboCareer.EditValue.ToString());
                        var career = BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == careerId);
                        txtCareer.Text = career.CAREER_CODE;
                    }
                    else
                    {
                        cboCareer.Properties.Buttons[1].Visible = false;
                        txtCareer.Text = "";
                    }
                    txtEthnic.Focus();
                    txtEthnic.SelectAll();
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
                    if (cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                        }
                    }

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
                    if (cboDistricts.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()))
                            {
                                cboProvince.EditValue = district.PROVINCE_CODE;
                            }
                            LoadCommuneCombo("", district.DISTRICT_CODE, false);
                            txtDistricts.Text = district.SEARCH_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                        }
                    }
                    FocusMoveText(this.txtCommune);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboDistricts.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtCommune.Text = commune.SEARCH_CODE;
                            if (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) && String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                cboDistricts.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null)
                                {
                                    cboProvince.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMilitaryRank_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        FocusMoveText(this.cboPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPosition_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMilitaryRank.EditValue != null)
                    {
                        FocusMoveText(this.cboClassify);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SuccessLog(HIS_PATIENT result)
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

        void SuccessLog(HIS_TREATMENT result)
        {
            try
            {
                if (result != null)
                {
                    string message = String.Format(HIS.Desktop.EventLog.EventLogUtil.SetLog(His.EventLog.Message.Enum.SuaThongTinBenhNhan), "", result.TDL_PATIENT_NAME, "", cboGender1.Text, currentHisPatientTypeAlter.TREATMENT_ID, currentHisPatientTypeAlter.TREATMENT_TYPE_NAME);
                    His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }

        private void cboDistricts_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboDistricts);
        }

        private void cboCommune_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboCommune);
        }

        private void txtProvinceName_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {

        }

        private void cboHTCommuneName_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboHTCommuneName);
        }

        private void cboHTDistrictName_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                GetNotInListValue(sender, e, cboHTDistrictName);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                        SDA.EFMODEL.DataModels.SDA_NATIONAL data = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == ((cboNation.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtNation.Text = data.NATIONAL_CODE;
                            txtTheBHYT.Focus();
                            txtTheBHYT.SelectAll();
                        }
                    }
                    else
                    {
                        txtTheBHYT.Focus();
                        txtTheBHYT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FocusMoveText(this.txtHTProvinceCode);
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
                    txtRelativePhone.Focus();
                    txtRelativePhone.SelectAll();
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

        private void dtDOB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNation.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
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

        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceCombo(strValue.ToUpper(), true);
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
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceCode = cboProvince.EditValue.ToString();
                        LoadDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin tỉnh", "Thông báo");
                    }
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
                    string districtCode = "";
                    if (cboDistricts.EditValue != null)
                    {
                        districtCode = cboDistricts.EditValue.ToString();
                        LoadCommuneCombo(strValue.ToUpper(), districtCode, true);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện", "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPersonFamily_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelation.Focus();
                    txtRelation.SelectAll();
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
                    txtCMNDRelative.Focus();
                    txtCMNDRelative.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientName.Focus();
                    txtPatientName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadHTProvinceCombo(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtHTDistrictCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (cboHTProvinceName.EditValue != null)
                    {
                        provinceCode = cboHTProvinceName.EditValue.ToString();
                        LoadHTDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin tỉnh", "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTCommuneCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtCode = "";
                    if (cboHTDistrictName.EditValue != null)
                    {
                        districtCode = cboHTDistrictName.EditValue.ToString();
                        LoadHTCommuneCombo(strValue.ToUpper(), districtCode, true);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có thông tin huyện", "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHTAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCCCD_CMTNumber.Focus();
                    txtCCCD_CMTNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtCCCD_CMTNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCCCD_CMTPlace.Focus();
                    txtCCCD_CMTPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtCCCD_CMTPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCCCD_CMTDate.Focus();
                    txtCCCD_CMTDate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCareer.Focus();
                    txtCareer.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtAccountNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTaxCode.Focus();
                    txtTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtFatherName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFatherEducationalLevel.Focus();
                    txtFatherEducationalLevel.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtRelativePhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelativeMobile.Focus();
                    txtRelativeMobile.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void textEdit45_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMotherEducationalLevel.Focus();
                    txtMotherEducationalLevel.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void chkBNManTinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTienSuBenh.Focus();
                    txtTienSuBenh.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtPatientDob.Visible = true;

                    dtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy HH:mm");
                    string strDob = dtPatientDob.Text;
                    TimBenhNhanTheoDieuKien(true);

                    FocusMoveText(this.txtNation);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDOB_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPatientDob.Visible = true;
                    dtPatientDob.Update();
                    dtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy HH:mm");

                    TimBenhNhanTheoDieuKien(true);

                    System.Threading.Thread.Sleep(100);
                    FocusMoveText(this.txtNation);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!dtPatientDob.ReadOnly && e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(dtPatientDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtPatientDob.EditValue = dt;
                        dtPatientDob.Update();
                    }
                    dtPatientDob.Visible = true;
                    dtPatientDob.ShowPopup();
                    dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(dtPatientDob.Text))
                {
                    if (!dtPatientDob.ReadOnly)
                    {
                        dtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(dtPatientDob.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientDOB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(dtPatientDob.Text))
                    {
                        dtPatientDob.Text = PatientDobUtil.PatientDobToDobRaw(dtPatientDob.Text);
                    }

                    if (!String.IsNullOrEmpty(dtPatientDob.Text))
                    {
                        string strDob = "";

                        if (dtPatientDob.Text.Length == 2 || dtPatientDob.Text.Length == 1)
                        {
                            int patientDob = Int32.Parse(dtPatientDob.Text);
                            if (patientDob < 7)
                            {
                                MessageBox.Show(ResourceLanguageManager.NgaySinhKhongDuocNhoHon7);
                                FocusMoveText(this.dtPatientDob);
                                return;
                            }
                            else
                            {
                                dtPatientDob.Text = (DateTime.Now.Year - patientDob).ToString();
                            }
                        }
                        else if (dtPatientDob.Text.Length == 4)
                        {
                            if (Inventec.Common.TypeConvert.Parse.ToInt64(dtPatientDob.Text) <= DateTime.Now.Year)
                            {
                                strDob = "01/01/" + dtPatientDob.Text;
                                isNotPatientDayDob = true;
                            }
                            else
                            {
                                MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                                FocusMoveText(this.dtPatientDob);
                                return;
                            }
                        }
                        else if (dtPatientDob.Text.Length == 8)
                        {
                            strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                            if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                            {
                                strDob = dtPatientDob.Text.Substring(0, 2) + "/" + dtPatientDob.Text.Substring(2, 2) + "/" + dtPatientDob.Text.Substring(4, 4);
                                dtPatientDob.Text = strDob;
                                isNotPatientDayDob = false;
                            }
                            else
                            {
                                MessageBox.Show(ResourceLanguageManager.ThongTinNgaySinhPhaiNhoHonNgayHienTai);
                                FocusMoveText(this.dtPatientDob);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(ResourceLanguageManager.NhapNgaySinhKhongDungDinhDang);
                            FocusMoveText(this.dtPatientDob);
                            return;
                        }

                        if (String.IsNullOrWhiteSpace(strDob))
                        {
                            strDob = dtPatientDob.Text;
                        }

                        TimBenhNhanTheoDieuKien(true);
                        isTxtPatientDobPreviewKeyDown = true;

                        FocusMoveText(this.txtNation);
                    }
                    else
                    {
                        FocusMoveText(this.dtPatientDob);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(dtPatientDob.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        dtPatientDob.EditValue = dt;
                        dtPatientDob.Update();
                    }

                    dtPatientDob.Visible = true;
                    dtPatientDob.ShowPopup();
                    dtPatientDob.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
