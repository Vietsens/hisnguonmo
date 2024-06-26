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
using AutoMapper;
using HIS.UC.UCHeniInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RegisterV3.ADO;
using HIS.Desktop.Plugins.RegisterV3.Run3;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.UCPatientRaw.ADO;
using HIS.Desktop.Common;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCOtherServiceReqInfo.ADO;
using HIS.UC.UCRelativeInfo.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HID.Filter;
using HID.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.HisSyncToHid;

namespace HIS.Desktop.Plugins.RegisterV3.Register
{
    abstract partial class ServiceRequestRegisterBehaviorBase : HIS.Desktop.Common.BusinessBase
    {
        bool Check()
        {
            try
            {
                bool inValid = true;
                inValid = inValid && CheckIsChildWithoutAddress();
                inValid = inValid && CheckLiveAreaCode();
                CallSyncHID();
                if (!inValid)
                    WaitingManager.Hide();
                return inValid;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// Thêm nghiệp vụ Hồ sơ sức khỏe cá nhân. có sử dụng thẻ cấu hình.
        /// Khi nhấn Lưu => Gọi lên HID:
        /// 1. Nếu HID trả về một bản ghi mới tạo => Set mã y tế vào HIS_PATIENT và gọi đăng ký tiếp đón như bình thương.
        /// 2. Trả về một List các dữ liệu => Hiển thị lên cho người dùng chọn.
        /// a. Nếu người dùng không chọn => gọi API tạo dữ liệu trên HID => HID trả về mã y tế => Set mã y tế vào HIS_PATIENT => Đăng ký bình thường
        /// b. Nếu người dùng chọn => Set mã y tế vào HIS_PATIENT => đăng ký tiếp đón bình thương
        /// </summary>
        /// <returns></returns>
        private void CallSyncHID()
        {
            try
            {
                //Các trường có dấu (*) là các trường bắt buộc phải set giá trị
                if (HisConfigCFG.IsSyncHID && (this.patientData == null || String.IsNullOrEmpty(this.patientData.PERSON_CODE)))
                {
                    CommonParam paramHID = new CommonParam();
                    HidPersonFilter filter = new HidPersonFilter();
                    if (ucRequestService != null && ucRequestService.cardSearch != null && !String.IsNullOrEmpty(ucRequestService.cardSearch.CardCode))
                        filter.CARD_CODE = ucRequestService.cardSearch.CardCode;
                    filter.BHYT_NUMBER = ((this.patientProfile.HisPatientTypeAlter != null) ? this.patientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER : "");
                    filter.BRANCH_CODE = BranchDataWorker.Branch.BRANCH_CODE;
                    filter.BRANCH_NAME = BranchDataWorker.Branch.BRANCH_NAME;
                    filter.ADDRESS = this.patientProfile.HisPatient.ADDRESS;
                    filter.COMMUNE_NAME = this.patientProfile.HisPatient.COMMUNE_NAME;
                    filter.DISTRICT_NAME = this.patientProfile.HisPatient.DISTRICT_NAME;
                    filter.PROVINCE_NAME = this.patientProfile.HisPatient.PROVINCE_NAME;
                    filter.CAREER_NAME = (this.patientProfile.HisPatient.CAREER_ID > 0 ? (BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == this.patientProfile.HisPatient.CAREER_ID) ?? new HIS_CAREER()).CAREER_NAME : "");
                    filter.DOB = this.patientProfile.HisPatient.DOB;//(*)
                    filter.GENDER_ID = this.patientProfile.HisPatient.GENDER_ID;//(*)
                    filter.FIRST_NAME = this.patientProfile.HisPatient.FIRST_NAME;
                    filter.LAST_NAME = this.patientProfile.HisPatient.LAST_NAME;
                    if (IsChild())
                    {
                        filter.VIR_PERSON_NAME = this.patientProfile.HisPatient.RELATIVE_NAME;//TODO
                    }
                    else
                    {
                        filter.VIR_PERSON_NAME = this.patientProfile.HisPatient.LAST_NAME + " " + this.patientProfile.HisPatient.FIRST_NAME;
                    }
                    filter.IS_HAS_NOT_DAY_DOB = (this.patientProfile.HisPatient.IS_HAS_NOT_DAY_DOB == GlobalVariables.CommonNumberTrue);
                    filter.ETHNIC_NAME = this.patientProfile.HisPatient.ETHNIC_NAME;
                    filter.EMAIL = this.patientProfile.HisPatient.EMAIL;
                    filter.NATIONAL_NAME = this.patientProfile.HisPatient.NATIONAL_NAME;
                    filter.MOBILE = this.patientProfile.HisPatient.PHONE;
                    filter.HOUSEHOLD_CODE = this.houseHold_Code;
                    filter.HOUSEHOLD_RELATION_NAME = this.hoseHold_Relative;
                    //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                    if (!String.IsNullOrEmpty(this.cMNDNumber))
                    {
                        if (this.cMNDNumber.Length > 9)
                        {
                            filter.CCCD_DATE = this.cMNDDate;
                            filter.CCCD_NUMBER = this.cMNDNumber;
                            filter.CCCD_PLACE = this.cMNDPlace;
                        }
                        else
                        {
                            filter.CMND_DATE = this.cMNDDate;
                            filter.CMND_NUMBER = this.cMNDNumber;
                            filter.CMND_PLACE = this.cMNDPlace;
                        }
                    }
                    filter.HT_ADDRESS = this.addressNow;
                    filter.HT_COMMUNE_NAME = this.communeNowName;
                    filter.HT_DISTRICT_NAME = this.districtNowName;
                    filter.HT_PROVINCE_NAME = this.provinceNowName;
                    filter.MOTHER_NAME = this.motherName;
                    filter.FATHER_NAME = this.fatherName;
                    filter.RELATIVE_PHONE = this.phone;
                    filter.RELATIVE_ADDRESS = this.relativeAddress;
                    filter.RELATIVE_NAME = this.relativeName;
                    filter.RELATIVE_TYPE = this.relativeType;
                    filter.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                    filter.BORN_PROVINCE_CODE = GenerateProvinceCode(this.born_provinceCode);//(*)
                    filter.BORN_PROVINCE_NAME = this.born_provinceName;//(*)
                    filter.BLOOD_ABO_CODE = this.blood_ABO_Code;
                    filter.BLOOD_RH_CODE = this.blood_Rh_Code;
                    //var persons = (new BackendAdapter(paramHID).Post<List<HID_PERSON>>(RequestUriStore.HID_PERSON_GET, ApiConsumers.HidConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramHID));

                    var persons = ApiConsumers.HidWrapConsumer.Post<List<HID_PERSON>>(true, RequestUriStore.HID_PERSON_GET, paramHID, filter);
                    if (persons != null && persons.Count > 0)
                    {
                        if (persons.Count == 1)
                        {
                            SelectPerson(persons[0]);
                        }
                        else
                        {
                            frmPersonSelect frmPersonSelect = new frmPersonSelect(persons, SelectPerson);
                            frmPersonSelect.ShowDialog();
                        }
                    }
                    else
                    {
                        if (paramHID.Messages != null && paramHID.Messages.Count > 0)
                        {
                            this.param.Messages.AddRange(paramHID.Messages);
                        }
                        if (paramHID.BugCodes != null && paramHID.BugCodes.Count > 0)
                        {
                            this.param.BugCodes.AddRange(paramHID.BugCodes);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("Goi len he thong HID lay thong tin ho so suc khoe ca nhan that bai. ____Input data: "
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter) + "____Result data:"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramHID), paramHID)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => persons), persons));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string GenerateProvinceCode(string provinceCode)
        {
            try
            {
                return string.Format("{0:000}", Convert.ToInt64(provinceCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return provinceCode;
        }

        void SelectPerson(HID_PERSON data)
        {
            try
            {
                this.patientProfile.HisPatient.PERSON_CODE = data.PERSON_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool CheckIsChildWithoutAddress()
        {
            try
            {
                bool inValidChild = true;
                inValidChild = inValidChild && (patientProfile.HisPatientTypeAlter != null);
                inValidChild = inValidChild && (this.IsChild());
                inValidChild = inValidChild && (patientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE);
                inValidChild = inValidChild && (String.IsNullOrEmpty(patientProfile.DistrictCode) || String.IsNullOrEmpty(patientProfile.ProvinceCode));
                if (inValidChild)
                {
                    param.Messages.Add(ResourceMessage.TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh);
                    this.ucRequestService.ucAddressCombo1.FocusToProvince();
                }
                return !inValidChild;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        bool CheckLiveAreaCode()
        {
            try
            {
                //issue #3590
                bool inValid = true;
                inValid = inValid && (patientProfile != null && patientProfile.HisPatientTypeAlter != null);
                inValid = inValid && (!string.IsNullOrEmpty(patientProfile.HisPatientTypeAlter.LIVE_AREA_CODE));

                if (inValid)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    ResourceMessage.BanCoMuonNhapThongTinKhuVuc,
                    ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        //Neu chon khong focus con tro vao o khu vuc
                        this.ucRequestService.isShowMess = false;
                        throw new NullReferenceException("LiveAreaCode");
                    }
                }

                return true;
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Truong hop nguoi dung nhap o khu vuc, nguoi dung chon khong muon nhap khu vuc da chon => focus vao o khu vuc cho nguoi dung nhap gia tri khac\n" + ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        bool IsChild()
        {
            bool isChild = false;
            try
            {
                var dtDateOfBird = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientProfile.HisPatient.DOB) ?? DateTime.Now;
                isChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDateOfBird);
            }
            catch (Exception ex)
            {
                isChild = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return isChild;
        }
    }
}
