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
using DevExpress.Utils.Menu;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.ADO;
using HIS.Desktop.Plugins.Register.Process;
using HIS.Desktop.Utility;
using HIS.UC.KskContract;
using HIS.UC.KskContract.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        /// <summary>
        /// Gan gia trị mac dinh cho cac control can khoi tao san gia tri
        /// </summary>
        private void SetDefaultData()
        {
            try
            {
                this.actionType = GlobalVariables.ActionAdd;
                this.dtIntructionTime.EditValue = DateTime.Now;
                try
                {
                    this.departmentId = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().SingleOrDefault(o => o.ID == this.currentModule.RoomId).DEPARTMENT_ID;
                }
                catch { }

                this.LoadPortToCombo();

                ToolTip toolTip = new ToolTip();
                toolTip.AutoPopDelay = 5000;
                toolTip.InitialDelay = 1000;
                toolTip.ReshowDelay = 500;
                toolTip.ShowAlways = true;
                InitTypeFind();

                this.chkEmergency.Checked = false;
                toolTip.SetToolTip(this.chkIsNotRequireFee, Inventec.Common.Resource.Get.Value("UCRegister.chkIsNotRequireFee.Tooltip.Text", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()));

                this.roomExamServiceNumber = 0;

                this.lcibtnBill.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;//Ẩn nút thanh toán đi

                this.InitControlPatientInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_PATIENT_TYPE> LoadPatientTypeExamByPatientType(long patientTypeId)
        {
            List<HIS_PATIENT_TYPE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAllowFilter patientTypeAllowFilter = new HisPatientTypeAllowFilter();
                patientTypeAllowFilter.PATIENT_TYPE_ID = patientTypeId;
                var patientTypeAllows = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALLOW>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, patientTypeAllowFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0)
                {
                    var arrpatientTypeAllowIds = patientTypeAllows.Select(o => o.PATIENT_TYPE_ID).ToArray();
                    result = patientTypes.Where(o => arrpatientTypeAllowIds.Contains(o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                if (btnMenuCodeFind.Tag.Equals("cardCode"))
                    this.txtPatientCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
                else
                    this.txtPatientCode.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                this.typeCodeFind = btnMenuCodeFind.Caption;
                if (this.typeCodeFind == this.typeCodeFind__MaNV)
                {
                    this.lciKskCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.ValidateHrmKskCode(this.dxValidationProviderPlusInfomation);
                }
                else
                {
                    this.lciKskCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.dxValidationProviderPlusInfomation.RemoveControlError(this.txtKskCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadOneBNToControl(HisPatientSDO patientDTO, bool isReloadUCHein)
        {
            try
            {
                if (patientDTO != null)
                {
                    LogSystem.Debug("Bat dau gan du lieu benh nhan len form");
                    this.currentPatientSDO = patientDTO;
                    this.FillDataPatientToControl(patientDTO);

                    LogSystem.Debug("t3.3. Call uc BHYT Load Combo TheBHYT cua benh nhan id = " + patientDTO.ID);
                    if (isReloadUCHein && this.mainHeinProcessor != null && this.ucHeinBHYT != null && patientDTO.HasBirthCertificate != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                        this.mainHeinProcessor.FillDataToHeinInsuranceInfoByOldPatient(this.ucHeinBHYT, patientDTO);

                    this.SetPatientSearchPanel(true);
                    LogSystem.Debug("SetPatientSearchPanel");

                    //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)
                    this.FillDataToExamServiceReqNewestByPatient(patientDTO);
                    LogSystem.Debug("Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)");

                    PeriosTreatmentMessage();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PeriosTreatmentMessage()
        {
            try
            {
                //- Kiểm tra cấu hình trên CCC: MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION
                //1: Khi đăng ký tiếp đón, có kiểm tra xem đợt khám/điều trị trước đó của BN đã uống hết thuốc hay chưa 
                //0: Không kiểm tra
                LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con thuoc chua uong het hay khong: IsCheckPreviousPrescription = " + HisConfigCFG.IsCheckPreviousPrescription);
                string message = "";
                if (HisConfigCFG.IsCheckPreviousPrescription)
                {
                    if (this.currentPatientSDO.PreviousPrescriptions != null
                        && this.currentPatientSDO.PreviousPrescriptions.Count > 0)
                    {
                        LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
                        string treatmentPrevis = this.currentPatientSDO.TreatmentCode;
                        string pressMessages = "";
                        for (int i = 0; i < this.currentPatientSDO.PreviousPrescriptions.Count; i++)
                        {
                            pressMessages += String.Format(ResourceMessage.ThuocCoThoiSuDungDen,
                                (" - " + this.currentPatientSDO.PreviousPrescriptions[i].REQUEST_ROOM_NAME + " ")
                                , Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientSDO.PreviousPrescriptions[i].USE_TIME_TO ?? 0) + "\r\n");
                        }
                        message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanCoThuocChuaUongHet, treatmentPrevis, "\r\n", pressMessages, "");
                    }
                }
                LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con no tien vien phi hay khong: IsCheckPreviousDebt = " + HisConfigCFG.IsCheckPreviousDebt);
                if (HisConfigCFG.IsCheckPreviousDebt == "1")
                {
                    if (this.currentPatientSDO.PreviousDebtTreatments != null
                        && this.currentPatientSDO.PreviousDebtTreatments.Count > 0)
                    {
                        LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
                        string treatmentPrevis = String.Join(",", this.currentPatientSDO.PreviousDebtTreatments);
                        if (!String.IsNullOrEmpty(message))
                        {
                            message += "\r\n";
                        }
                        message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi, treatmentPrevis);
                    }
                }

                if (!String.IsNullOrEmpty(message))
                {
                    MessageManager.Show(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool AlertTreatmentInOutInDayForTreatmentMessage(HisPatientSDO patientDTO)
        {
            bool valid = true;
            this.isAlertTreatmentEndInDay = false;
            try
            {
                //Khi nhập thông tin bệnh nhân => tìm thấy mã BN cũ. Kiểm tra hồ sơ điều trị gần nhất của bn. Nếu có ngày ra = ngày hiện tại thì cảnh báo:
                //BN có hồ sơ điều trị: xxxx ra viện ngày hôm nay. Bạn có muốn tiếp tục?
                //Có, tiếp đón bình thường
                //Không, làm mới màn hình tiếp đón. (xóa tất cả trường dữ liệu)                
                string message = "";
                if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckTodayFinishTreatment && patientDTO.ID > 0 && patientDTO.TodayFinishTreatments != null && patientDTO.TodayFinishTreatments.Count > 0)
                {
                    string treatmentCodeInDay = String.Join(",", patientDTO.TodayFinishTreatments);
                    if (!String.IsNullOrEmpty(treatmentCodeInDay))
                    {
                        LogSystem.Debug("Tiep don: tim thay benh nhan cu co dot dieu tri gan nhat ra vien trong ngay: " + LogUtil.TraceData("HisPatientSDO", patientDTO));
                        message += String.Format(ResourceMessage.DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay, treatmentCodeInDay);
                    }

                    if (!String.IsNullOrEmpty(message))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                       message,
                       ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            valid = false;
                            this.isAlertTreatmentEndInDay = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool LoadOneBNToControlForUCHein(HisPatientSDO patientDTO)
        {
            bool valid = true;
            try
            {
                if (!AlertTreatmentInOutInDayForTreatmentMessage(patientDTO))
                {
                    this.currentPatientSDO = null;
                    this._HeinCardData = null;
                    this.ResultDataADO = null;
                    ResetPatientForm();
                    return false;
                }

                LoadOneBNToControl(patientDTO, false);

                this._HeinCardData = this.ConvertFromPatientData(patientDTO);

                this.CheckTTFull(this._HeinCardData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void FillDataPatientToControl(HisPatientSDO patientDTO)
        {
            try
            {
                if (patientDTO == null) throw new ArgumentNullException("patientDTO is null");
                this.layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!String.IsNullOrEmpty(patientDTO.PERSON_CODE))
                {
                    this.lblDescriptionForHID.Text = "";//Bệnh nhân đã có thông tin trên hệ thống hồ sơ sức khỏe cá nhân
                    //this.dxValidationProviderControl.SetValidationRule(this.txtProvinceCodeKS, null);
                    //this.lciProvinceKS.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else
                {
                    //if (HisConfigCFG.IsSyncHID)
                    //{
                    //    ValidationProvinceKSControl();
                    //}
                    //else
                    //    this.dxValidationProviderControl.SetValidationRule(this.txtProvinceCodeKS, null);

                    //this.lciProvinceKS.AppearanceItemCaption.ForeColor = (HisConfigCFG.IsSyncHID ? Color.Maroon : Color.Black);
                    this.lblDescriptionForHID.Text = "";
                }
                if (this.btnCodeFind.Text == this.typeCodeFind__MaBN)
                {
                    this.txtPatientCode.Text = patientDTO.PATIENT_CODE;
                }
                this.txtPatientName.Text = patientDTO.VIR_PATIENT_NAME;
                if (patientDTO.DOB > 0 && patientDTO.DOB.ToString().Length >= 6)
                {
                    if (patientDTO.IS_HAS_NOT_DAY_DOB == 1)
                        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, true);
                    else
                        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, false);
                }

                MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patientDTO.GENDER_ID);
                if (gioitinh != null)
                {
                    this.cboGender.EditValue = gioitinh.ID;
                    this.txtGenderCode.Text = gioitinh.GENDER_CODE;
                }
                this.txtAddress.Text = patientDTO.ADDRESS;
                var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == patientDTO.NATIONAL_NAME);
                if (national != null)
                {
                    this.cboNational.EditValue = national.NATIONAL_NAME;
                    this.txtNationalCode.Text = national.NATIONAL_CODE;
                }
                var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == patientDTO.ETHNIC_NAME);
                if (ethnic != null)
                {
                    this.cboEthnic.EditValue = ethnic.ETHNIC_NAME;
                    this.txtEthnicCode.Text = ethnic.ETHNIC_CODE;
                }

                //LogSystem.Debug(patientDTO.HeinCardNumber);
                MOS.EFMODEL.DataModels.HIS_CAREER career = this.GetCareerByBhytWhiteListConfig(patientDTO.HeinCardNumber);

                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                if (career != null)
                {
                    // LogSystem.Debug(patientDTO.HeinCardNumber);
                    this.FillDataCareerUnder6AgeByHeinCardNumber(patientDTO.HeinCardNumber);
                }
                else
                {
                    career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == patientDTO.CAREER_ID);
                }
                if (career != null && career.ID > 0)
                {
                    this.cboCareer.EditValue = patientDTO.CAREER_ID;
                    this.txtCareerCode.Text = career.CAREER_CODE;
                }
                //xuandv Kiểm tra xem là trẻ em hay không để validate thông tin người nhà
                bool isTE = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.Now);
                if (isTE)
                {
                    this.SetValidationByChildrenUnder6Years(isTE, false);
                }
                Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                if (ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null)
                {
                    var data = adProc.SplitFromFullAddress(this.ResultDataADO.ResultHistoryLDO.diaChi);
                    if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1 || (typeCodeFind == typeCodeFind__CCCDCMND && (currentPatientSDO == null)) || (currentPatientSDO != null && data != null && (currentPatientSDO.PROVINCE_CODE != data.ProvinceCode || currentPatientSDO.DISTRICT_CODE != data.DistrictCode || currentPatientSDO.COMMUNE_CODE != data.CommuneCode)))
                    {
                        cboProvince.EditValue = data.ProvinceCode;
                        txtProvinceCode.EditValue = data.ProvinceCode;
                        cboDistrict.EditValue = data.DistrictCode;
                        txtDistrictCode.EditValue = data.DistrictCode;
                        cboCommune.EditValue = data.CommuneCode;
                        txtCommuneCode.EditValue = data.CommuneCode;
                    }
                }
                var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_NAME == patientDTO.PROVINCE_NAME);
                if (province != null)
                {
                    this.cboProvince.EditValue = province.PROVINCE_CODE;
                    this.txtProvinceCode.Text = province.PROVINCE_CODE;
                    this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                }
                var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => ((o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDTO.DISTRICT_NAME || o.DISTRICT_CODE == patientDTO.DISTRICT_CODE || o.DISTRICT_NAME == patientDTO.DISTRICT_NAME )&& o.PROVINCE_NAME == patientDTO.PROVINCE_NAME);
                if (district != null)
                {
                    this.cboDistrict.EditValue = district.DISTRICT_CODE;
                    this.txtDistrictCode.Text = district.DISTRICT_CODE;
                    this.LoadXaCombo("", district.DISTRICT_CODE, false);
                }
                var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o =>
                    ((o.INITIAL_NAME + " " + o.COMMUNE_NAME) == patientDTO.COMMUNE_NAME || o.COMMUNE_CODE == patientDTO.COMMUNE_CODE)
                    && ((o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDTO.DISTRICT_NAME || o.DISTRICT_CODE == patientDTO.DISTRICT_CODE || o.DISTRICT_NAME == patientDTO.DISTRICT_NAME)
                    //&& o.COMMUNE_CODE == patientDTO.COMMUNE_CODE//TODO
                    );
                if (commune != null)
                {
                    this.cboCommune.EditValue = commune.COMMUNE_CODE;
                    this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                    this.cboTHX.EditValue = commune.ID;
                    this.txtMaTHX.Text = commune.SEARCH_CODE;
                }
                else if (province != null && district != null)
                {
                    var communeTHX = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                    (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                    && o.ID < 0);
                    if (communeTHX != null)
                    {
                        this.cboTHX.EditValue = communeTHX.ID;
                        this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                    }
                }

                if (AppConfigs.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                {
                    if (this.workPlaceProcessor != null)
                        this.workPlaceProcessor.SetValue(this.ucWorkPlace, patientDTO.WORK_PLACE);
                }
                else
                {
                    if (patientDTO.WORK_PLACE_ID > 0)
                    {
                        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == patientDTO.WORK_PLACE_ID);
                        if (workPlace != null)
                        {
                            if (this.workPlaceProcessor != null)
                                this.workPlaceProcessor.SetValue(this.ucWorkPlace, workPlace);
                        }
                        else
                        {
                            if (this.workPlaceProcessor != null)
                                this.workPlaceProcessor.SetValue(this.ucWorkPlace, null);
                        }
                    }
                }

                var militaryRank = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == (patientDTO.MILITARY_RANK_ID ?? 0));
                this.txtMilitaryRankCode.Text = (militaryRank != null ? militaryRank.MILITARY_RANK_CODE : "");
                this.cboMilitaryRank.EditValue = patientDTO.MILITARY_RANK_ID;
                this.txtPhone.Text = patientDTO.PHONE;
                this.txtHomePerson.Text = patientDTO.RELATIVE_TYPE;
                this.txtCorrelated.Text = patientDTO.RELATIVE_NAME;
                this.txtRelativeAddress.Text = patientDTO.RELATIVE_ADDRESS;
                this.txtRelativeCMNDNumber.Text = patientDTO.RELATIVE_CMND_NUMBER;
                this.txtNote.Text = patientDTO.NOTE;
                string bornProvinceCode = (!String.IsNullOrEmpty(patientDTO.BORN_PROVINCE_CODE) ? (patientDTO.BORN_PROVINCE_CODE.Length > 2 ? patientDTO.BORN_PROVINCE_CODE.Substring(1, 2) : patientDTO.BORN_PROVINCE_CODE) : "");
                this.txtProvinceCodeKS.Text = bornProvinceCode;
                this.cboProvinceKS.EditValue = bornProvinceCode;

                this.chkIsChronic.Checked = (patientDTO.IS_CHRONIC == 1);
                //this.AutoCheckPriorityByPriorityType(patientDTO);
                this.cboPatientClassify.EditValue = patientDTO.PATIENT_CLASSIFY_ID;
                if (!string.IsNullOrEmpty(patientDTO.NOTE))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(patientDTO.NOTE, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// 2. Sửa chức năng "Tiếp đón (2)":
        ///Khi tiếp đón bệnh nhân, thực hiện truy vấn xem có "bản ghi" "đối tượng ưu tiên" nào thỏa mãn không. Nếu có, mặc định điền "TH ưu tiên" theo bản ghi đầu tiên thỏa mãn, đồng thời tự động check vào checkbox "Ưu tiên"
        ///3. Sửa chức năng "Tiếp đón":
        ///Khi tiếp đón bệnh nhân, thực hiện truy vấn xem có "bản ghi" "đối tượng ưu tiên" nào thỏa mãn không. Nếu có, tự động check vào checkbox "Ưu tiên"
        ///Lưu ý: Thuật toán xử lý "truy vấn bản ghi đối tượng ưu tiên thỏa mãn" như sau:
        ///Lấy theo điều kiện sau:
        ///(AGE_FROM NULL OR AGE_FROM >= X)
        ///AND (AGE_TO NULL OR AGE_TO <= X)
        ///AND (BHYT_PREFIXS NULL OR Y START_IN (BHYT_PREFIXS))
        ///AND (AGE_FROM NOT NULL OR AGE_TO NOT NULL OR BHYT_PREFIXS NOT NULL)
        ///Trong đó:
        ///+ X: là tuổi của BN
        ///+ Y: là thẻ BHYT của BN
        ///+ START_IN: là hàm xử lý duyệt tất cả các đầu thẻ được khai báo (tách chuỗi BHYT_PREFIXS bởi dấu phẩy), và thực hiện kiểm tra với từng đầu thẻ, nếu Y có bắt đầu bằng đầu thẻ đấy thì trả về TRUE.
        /// </summary>
        /// <param name="dataPriorityTypes"></param>
        private void AutoCheckPriorityByPriorityType(long patientDob, string heinCardNumber)
        {
            try
            {
                bool hasData = false;
                long patientAge = 0;
                List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE> dataPriorityTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>();
                if (dataPriorityTypes != null && dataPriorityTypes.Count > 0 && (patientDob > 0 || !String.IsNullOrEmpty(heinCardNumber)))
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDob).Value;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    DateTime newDate = new DateTime(tongsogiay);
                    int nam = newDate.Year - 1;
                    patientAge = nam == 0 ? 1 : nam;
                    var checkData = dataPriorityTypes.Where(o =>
                        (o.AGE_FROM == null || (o.AGE_FROM.HasValue && o.AGE_FROM <= patientAge))
                        && (o.AGE_TO == null || (o.AGE_TO.HasValue && o.AGE_TO >= patientAge))
                        && (String.IsNullOrEmpty(o.BHYT_PREFIXS) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS) && StartIn(o.BHYT_PREFIXS, heinCardNumber)))
                        && ((o.AGE_FROM.HasValue && o.AGE_FROM > 0) || (o.AGE_TO.HasValue && o.AGE_TO > 0) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS)))
                        ).ToList();
                    hasData = checkData != null && checkData.Count > 0;
                    chkPriority.Checked = hasData;

                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataPriorityTypes), dataPriorityTypes)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientAge), patientAge)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasData), hasData));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool StartIn(string BHYT_PREFIXS, string heincardnumber)
        {
            bool valid = false;
            try
            {
                List<string> checkData = null;
                if (!String.IsNullOrEmpty(BHYT_PREFIXS) && !String.IsNullOrEmpty(heincardnumber))
                {
                    var arrPrefixs = BHYT_PREFIXS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrPrefixs != null && arrPrefixs.Count() > 0)
                    {
                        checkData = arrPrefixs.ToList().Where(o => heincardnumber.StartsWith(o)).ToList();
                        valid = (checkData != null && checkData.Count > 0) ? true : false;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkData), checkData)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => BHYT_PREFIXS), BHYT_PREFIXS)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heincardnumber), heincardnumber));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void LoadNgayThangNamSinhBNToForm(long dob, bool hasNotDayDob)
        {
            try
            {
                LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p1: tinh toan nam sinh");
                if (dob > 0)
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob) ?? DateTime.MinValue;
                    bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                    this.dtPatientDob.EditValue = dtNgSinh;
                    this.isNotPatientDayDob = hasNotDayDob;
                    if (hasNotDayDob)
                    {
                        this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICEREQUESTREGITER_LCI_PATIENT_DOB_YEAR", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        this.txtPatientDob.Text = dtNgSinh.ToString("yyyy");
                    }
                    else
                    {
                        this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICEREQUESTREGITER_LCI_PATIENT_DOB", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        this.txtPatientDob.Text = dtNgSinh.ToString("dd/MM/yyyy");
                    }

                    HIS.Desktop.Plugins.Register.CalculatePatientAge.AgeObject ageObject = CalculatePatientAge.Calculate(dob);
                    if (ageObject != null)
                    {
                        this.txtAge.EditValue = ageObject.OutDate;
                        this.cboAge.EditValue = ageObject.AgeType;
                    }

                    if (this.mainHeinProcessor != null)
                    {
                        LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p2: goi uc bhyt update checkbox co giay khai sinh hay khong");
                        this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, isGKS);
                        LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p3: ket thuc update");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTinhThanhCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.cboDistrict.Properties.DataSource = null;
                    this.cboDistrict.EditValue = null;
                    this.txtDistrictCode.Text = "";
                    this.cboProvince.EditValue = null;
                    this.FocusShowpopup(this.cboProvince, false);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.SEARCH_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        this.txtProvinceCode.Text = listResult[0].SEARCH_CODE;
                        this.cboProvinceKS.EditValue = this.cboProvince.EditValue;
                        this.txtProvinceCodeKS.Text = txtProvinceCode.Text;
                        this.LoadHuyenCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            this.txtDistrictCode.Focus();
                            this.txtDistrictCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.Properties.DataSource = null;
                        this.cboDistrict.EditValue = null;
                        this.txtDistrictCode.Text = "";
                        this.cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            this.cboProvince.Properties.DataSource = listResult;
                            this.FocusShowpopup(this.cboProvince, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHuyenCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.SEARCH_CODE.ToUpper().Contains(searchCode.ToUpper()) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();
                this.InitComboCommon(this.cboDistrict, listResult, "DISTRICT_CODE", "RENDERER_DISTRICT_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    this.cboCommune.Properties.DataSource = null;
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.txtDistrictCode.Text = "";
                    this.cboDistrict.EditValue = null;
                    this.FocusShowpopup(this.cboDistrict, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].SEARCH_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            this.cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                            this.txtProvinceCode.Text = listResult[0].PROVINCE_CODE;
                        }

                        this.LoadXaCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            this.txtCommuneCode.Focus();
                            this.txtCommuneCode.SelectAll();
                        }
                    }
                    else
                    {
                        this.cboCommune.Properties.DataSource = null;
                        this.cboCommune.EditValue = null;
                        this.txtCommuneCode.Text = "";
                        this.cboDistrict.EditValue = null;
                        if (isExpand)
                        {
                            this.FocusShowpopup(this.cboDistrict, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadXaCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    .Where(o => (o.SEARCH_CODE ?? "").Contains(searchCode ?? "")
                        && (String.IsNullOrEmpty(districtCode) || o.DISTRICT_CODE == districtCode)).ToList();
                this.InitComboCommon(this.cboCommune, listResult, "COMMUNE_CODE", "RENDERER_COMMUNE_NAME", "SEARCH_CODE");
                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    this.cboCommune.EditValue = null;
                    this.txtCommuneCode.Text = "";
                    this.FocusShowpopup(this.cboCommune, false);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        this.cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        this.txtCommuneCode.Text = listResult[0].SEARCH_CODE;
                        this.cboDistrict.EditValue = listResult[0].DISTRICT_CODE;
                        this.txtDistrictCode.Text = listResult[0].DISTRICT_CODE;
                        if (String.IsNullOrEmpty(this.cboProvince.Text))
                        {
                            SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).Where(o => o.ID == listResult[0].DISTRICT_ID).FirstOrDefault();
                            if (district != null)
                            {
                                this.cboProvince.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCode.Text = district.PROVINCE_CODE;

                                this.cboProvinceKS.EditValue = district.PROVINCE_CODE;
                                this.txtProvinceCodeKS.Text = district.PROVINCE_CODE;
                            }
                        }

                        if (isExpand)
                        {
                            this.txtAddress.Focus();
                            this.txtAddress.SelectAll();
                        }
                    }
                    else if (isExpand)
                    {
                        this.cboCommune.EditValue = null;
                        this.FocusShowpopup(this.cboCommune, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchPatientByFilterCombo()
        {
            try
            {
                string strDob = "";
                if (this.txtPatientDob.Text.Length == 4)
                    strDob = "01/01/" + this.txtPatientDob.Text;
                else if (this.txtPatientDob.Text.Length == 8)
                {
                    strDob = this.txtPatientDob.Text.Substring(0, 2) + "/" + this.txtPatientDob.Text.Substring(2, 2) + "/" + this.txtPatientDob.Text.Substring(4, 4);
                }
                else
                    strDob = this.txtPatientDob.Text;
                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                this.dtPatientDob.Update();

                //Trường hợp chưa nhập đủ 3 thông tin: hộ tên, ngày sinh, giới tính thì bỏ qua không thưc hiện tìm kiếm
                if ((this.dtPatientDob.EditValue == null
                    || this.dtPatientDob.DateTime == DateTime.MinValue)
                    || this.cboGender.EditValue == null
                    || String.IsNullOrEmpty(this.txtPatientName.Text.Trim()))
                {
                    return;
                }

                LogSystem.Debug("Bat dau tim kiem benh nhan theo filter.");
                string dateDob = this.dtPatientDob.DateTime.ToString("yyyyMMdd");
                string timeDob = "00";
                if (this.txtAge.Enabled && this.cboAge.Enabled)
                    timeDob = string.Format("{0:00}", DateTime.Now.Hour - Inventec.Common.TypeConvert.Parse.ToInt32(this.txtAge.Text));

                long dob = Inventec.Common.TypeConvert.Parse.ToInt64(dateDob + timeDob + "0000");
                short ismale = Convert.ToInt16(this.cboGender.EditValue);
                this.LoadDataSearchPatient("", this.txtPatientName.Text, dob, ismale, true);
                this.cardSearch = null;
                LogSystem.Debug("Ket thuc tim kiem benh nhan theo filter.");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataSearchPatient(string maBN, string hoten, long? dob, short? isMale, bool isSearchData)
        {
            try
            {
                LogSystem.Debug("LoadDataSearchPatient => t1");
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientAdvanceFilter hisPatientFilter = new MOS.Filter.HisPatientAdvanceFilter();
                hisPatientFilter.DOB = dob;
                hisPatientFilter.VIR_PATIENT_NAME__EXACT = hoten;
                if (!String.IsNullOrEmpty(maBN))
                {
                    hisPatientFilter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Inventec.Common.TypeConvert.Parse.ToInt64(maBN));
                }
                hisPatientFilter.GENDER_ID = isMale;
                this.currentSearchedPatients = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, hisPatientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (this.currentSearchedPatients != null && this.currentSearchedPatients.Count > 0)
                {
                    LogSystem.Debug("LoadDataSearchPatient => t1.1. Tim thay benh nhan cu, hien thi cua so chon benh nhan");
                    frmPatientChoice frm = new frmPatientChoice(this.currentSearchedPatients, this.SelectOnePatientProcess);
                    frm.ShowDialog();
                }
                else
                {
                    this.actionType = GlobalVariables.ActionAdd;
                    this.currentPatientSDO = null;
                    bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime);
                    this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, isGKS);
                }
                LogSystem.Debug("LoadDataSearchPatient => t2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOnePatientProcess(HisPatientSDO patient)
        {
            try
            {
                if (!AlertTreatmentInOutInDayForTreatmentMessage(patient))
                {
                    this.currentPatientSDO = null;
                    ResetPatientForm();
                    return;
                }

                LogSystem.Debug("SelectOnePatientProcess => t1. Gan thong tin cua benh nhan len form");
                this.actionType = GlobalVariables.ActionAdd;
                this.SetPatientSearchPanel(true);

                //Nếu thực hiện tìm kiếm theo (họ tên + ngày sinh + giới tính) -> tìm ra BN cũ -> giữ nguyên ngày sinh đang nhập
                if (this.txtPatientDob.Text.Length == 4)
                    patient.IS_HAS_NOT_DAY_DOB = 1;
                else
                    patient.IS_HAS_NOT_DAY_DOB = null;

                this.currentPatientSDO = patient;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientSDO), currentPatientSDO));
                this.LoadOneBNToControlWithThread();

                LogSystem.Debug("SelectOnePatientProcess => t2. SetDefaultFocusUserControl");
                this.SetDefaultFocusUserControl();
                if (AppConfigs.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan == 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        async void ProcessSearchByCode(string searchCode)
        {
            try
            {
                //Lay gia tri ma nhap vao
                //Kiem tra du lieu ma la qrcode hay ma benh nhan 
                //Neu la qrcode se doc chuoi ma hoa tra ve doi tuong heindata
                //Neu la ma benh nhan thi goi api kiem tra co du lieu benh nhan tuong ung voi ma hay khong, co thi tra ve du lieu BN
                var data = (await ProcessSearchByCodeAsync(searchCode).ConfigureAwait(false));
                this.Invoke(new MethodInvoker(delegate() { ProcessPatientCodeKeydown(data); }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task<object> ProcessSearchByCodeAsync(string searchCode)
        {
            try
            {
                return await SearchByCode(searchCode).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private async void ProcessPatientCodeKeydown(object data)
        {
            try
            {
                //Lay gia tri ma nhap vao
                if (data != null)
                {
                    WaitingManager.Show();
                    CccdCardData cccdCard = new CccdCardData();
                    this.ResultDataADO = null;
                    this._HeinCardData = null;
                    string heinAddressOfPatient = "";
                    this.SetPatientSearchPanel(false);
                    this.isReadQrCode = false;
                    //Trường hợp tìm ra bệnh nhân cũ
                    if (data is HisPatientSDO)
                    {
                        if (!AlertTreatmentInOutInDayForTreatmentMessage(data as HisPatientSDO))
                        {
                            this.currentPatientSDO = null;
                            this._HeinCardData = null;
                            this.ResultDataADO = null;
                            ResetPatientForm();
                            return;
                        }

                        //An hien cac button lam moi thong tin benh nhan
                        this.SetPatientSearchPanel(true);
                        var patient = data as HisPatientSDO;
                        //Fill du lieu vao vung thong tin benh nhan
                        LoadOneBNToControl(patient, true);
                        heinAddressOfPatient = patient.HeinAddress;
                        //heinCardNumder = patient.HeinCardNumber;

                        //xuandv
                        this._HeinCardData = this.ConvertFromPatientData(patient);
                    }
                    //Trường hợp quẹt thẻ bhyt với bệnh nhân mới
                    else
                    {
                        if (data is HeinCardData)
                        {
                            //xuandv
                            this._HeinCardData = (HeinCardData)data;
                            this.isReadQrCode = true;
                           
                            //this.ProcessQrCodeData(this._HeinCardData);
                        }else if(data is CccdCardData)
						{
                            isReadQrCode = true;
                            cccdCard = (CccdCardData)data;
                            _HeinCardData = new HeinCardData();
                            _HeinCardData.HeinCardNumber = cccdCard.CardData;
                            _HeinCardData.PatientName = cccdCard.PatientName;
                            _HeinCardData.Dob = cccdCard.Dob;
                            _HeinCardData.Gender = cccdCard.Gender == "NAM" ? "1" : "2";
                            _HeinCardData.Address = cccdCard.Address;
                            cboPatientType.EditValue = HisConfigCFG.PatientTypeId__BHYT;
                        }
                        string patientName = Inventec.Common.String.Convert.HexToUTF8Fix(this._HeinCardData.PatientName);
                        if (!string.IsNullOrEmpty(patientName))
                            this._HeinCardData.PatientName = patientName;
                        string address = Inventec.Common.String.Convert.HexToUTF8Fix(this._HeinCardData.Address);
                        if (!string.IsNullOrEmpty(address))
                            this._HeinCardData.Address = address;
                        if (data is HeinCardData)
                            FillDataAfterFindQrCodeNoExistsCard(_HeinCardData);
                    }
                    //this.CheckheinCardFromHeinInsuranceApi(this._HeinCardData);
                    if (!(data is CccdCardData))
                    {
                        HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                        this.ResultDataADO = await heinGOVManager.Check(this._HeinCardData, null, false, heinAddressOfPatient, dtIntructionTime.DateTime, this.isReadQrCode);
					}
					else
					{
                        HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                        this.ResultDataADO = await heinGOVManager.CheckCccdQrCode(this._HeinCardData, null, dtIntructionTime.DateTime);

                    }
                    mainHeinProcessor.SetResultDataADOBhyt(ucHeinBHYT, this.ResultDataADO);
                    if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
                    {
                        //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                        if (!String.IsNullOrEmpty(this._HeinCardData.HeinCardNumber))
                        {
                            this._HeinCardData.Gender = this.ResultDataADO.ResultHistoryLDO.gioiTinh.ToUpper() == "NAM" ? "1" : "2";
                            this._HeinCardData.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maThe;
                            if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                            {
                                this._HeinCardData.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maTheMoi;
                            }
                        }
                    }
                    WaitingManager.Hide();
                    if (this.isReadQrCode)
                        this.ProcessQrCodeData(this._HeinCardData);
                    await this.CheckTTProcessResultData(this._HeinCardData);
                    this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(this._HeinCardData, true);
                }
                //#4671
                else
                {
                    if (this.typeCodeFind == typeCodeFind__CCCDCMND)
                        return;
                    int n;
                    bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
                    string codeFind = "";
                    if (isNumeric)
                    {
                        codeFind = string.Format("{0:0000000000}", Convert.ToInt64(this.txtPatientCode.Text));
                    }
                    else
                    {
                        codeFind = this.txtPatientCode.Text;
                    }
                    this.txtPatientCode.Text = codeFind;
                    CommonParam paramCommon = new CommonParam();
                    HisTreatmentViewFilter filterTreatment = new HisTreatmentViewFilter();
                    filterTreatment.PATIENT_CODE__EXACT = codeFind;

                    if (!string.IsNullOrEmpty(codeFind))
                    {
                        var lstTreatment = new BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filterTreatment, paramCommon).ToList();
                        if (lstTreatment != null && lstTreatment.Count > 0)
                        {
                            var treatment = lstTreatment.FirstOrDefault();
                            if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                            {
                                return;
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaBenhNhanKhongTontai + " '" + codeFind + "'", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                            }
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaBenhNhanKhongTontai + " '" + codeFind + "'", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        }
                    }
                    this.layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //this.lciProvinceKS.AppearanceItemCaption.ForeColor = (HisConfigCFG.IsSyncHID ? Color.Maroon : Color.Black);
                    this.lblDescriptionForHID.Text = "";

                    this.txtPatientCode.Focus();
                    this.txtPatientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HeinCardData ConvertFromPatientData(HisPatientSDO patient)
        {
            HeinCardData hein = null;
            try
            {
                if (patient.HasBirthCertificate != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    hein = new HeinCardData();
                    hein.Address = patient.HeinAddress;
                    if (patient.IS_HAS_NOT_DAY_DOB == 1)
                    {
                        hein.Dob = patient.DOB.ToString().Substring(0, 4);
                    }
                    else
                        hein.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                    hein.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardFromTime ?? 0));
                    hein.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardToTime ?? 0));
                    hein.MediOrgCode = patient.HeinMediOrgCode;
                    hein.HeinCardNumber = patient.HeinCardNumber;
                    hein.LiveAreaCode = patient.LiveAreaCode;
                    hein.PatientName = patient.VIR_PATIENT_NAME;
                    hein.FineYearMonthDate = patient.Join5Year;
                    var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patient.GENDER_ID);
                    hein.Gender = (gender != null ? GenderConvert.HisToHein(gender.ID.ToString()) : "2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        private HIS_CAREER GetCareerByBhytWhiteListConfig(string heinCardNumder)
        {
            HIS_CAREER result = null;
            try
            {
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => !String.IsNullOrEmpty(heinCardNumder) && o.BHYT_WHITELIST_CODE.ToUpper() == heinCardNumder.Substring(0, 3).ToUpper());
                    if (bhytWhiteList != null && (bhytWhiteList.CAREER_ID ?? 0) > 0)
                    {
                        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == bhytWhiteList.CAREER_ID.Value);
                        if (result == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("GetCareerByBhytWhiteListConfig => Khong lay duoc nghe nghiep theo id = " + bhytWhiteList.CAREER_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataCareerUnder6AgeByHeinCardNumber(string heinCardNumder)
        {
            try
            {
                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
                if (career == null)
                {
                    if (this.dtPatientDob.DateTime != DateTime.MinValue)
                    {
                        if (this.dtPatientDob.DateTime != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime))
                        {
                            career = HisConfigCFG.CareerUnder6Age;
                        }
                        else if (DateTime.Now.Year - this.dtPatientDob.DateTime.Year <= 18)
                        {
                            career = HisConfigCFG.CareerHS;
                        }
                        else
                        {
                            career = HisConfigCFG.CareerBase;
                        }
                    }
                    else
                    {
                        career = HisConfigCFG.CareerBase;
                    }
                }
                if (career != null && career.ID > 0)
                {
                    this.cboCareer.EditValue = career.ID;
                    this.txtCareerCode.Text = career.CAREER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToExamServiceReqNewestByPatient(HisPatientSDO data)
        {
            try
            {
                if (data == null) throw new ArgumentNullException("FillDataToExamServiceReqNewestByPatient. Get HisPatientSDO is null");
                if (this.roomExamServiceProcessor != null && this.ucRoomExamService != null)
                {
                    V_HIS_PATIENT vPatient = new V_HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(vPatient, data);

                    this.roomExamServiceProcessor.SetValueByPatient(this.ucRoomExamService, (AppConfigs.IsAutoFillDataRecentServiceRoom == "1" ? vPatient : new V_HIS_PATIENT()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessQrCodeData(HeinCardData dataHein)
        {
            try
            {
                // this.isReadQrCode = true;
                string currentHeincardNumber = dataHein.HeinCardNumber;
                if (dataHein == null) throw new ArgumentNullException("ProcessQrCodeData => dataHein is null");
                if (!String.IsNullOrEmpty(dataHein.HeinCardNumber))
                {
                    if (dataHein.HeinCardNumber.Length > 15)
                        dataHein.HeinCardNumber = dataHein.HeinCardNumber.Substring(0, 15);
                    else if (dataHein.HeinCardNumber.Length < 15 && dataHein.HeinCardNumber.Length != 12)
                        LogSystem.Debug("Do dai so the bhyt cua benh nhan khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHeincardNumber), currentHeincardNumber));
                    else
                        LogSystem.Info("Kiem tra Patient theo CCCD: " + dataHein.HeinCardNumber);
                }

                //Kiểm tra đã tồn tại dữ liệu bệnh nhân theo số thẻ bhyt hay không
                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                if (this.typeCodeFind == typeCodeFind__CCCDCMND && (oldValue.Trim().Contains("|") || (oldValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))))
                {
                    if (dataHein.HeinCardNumber.Length != 12 && dataHein.HeinCardNumber.Length != 9)
                        filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber() { HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber, CCCD_NUMBER__EXACT = oldValue.Trim().Contains("|") ? oldValue.Split('|')[0] : oldValue.Trim() };
                    else if (dataHein.HeinCardNumber.Length == 9)
                    {
                        filter.CMND_NUMBER__EXACT = dataHein.HeinCardNumber;
                    }
                    else
                    {
                        filter.CCCD_NUMBER__EXACT = dataHein.HeinCardNumber;
                    }
                }
                else
                {
                    filter.HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber;
                }
                var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                if (patients != null && patients.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("___1____");
                    if (this.typeCodeFind == typeCodeFind__CCCDCMND && (oldValue.Trim().Contains("|") || (oldValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))))
                    {
                        HisPatientSDO patient = patients[0];
                        LogSystem.Debug("Quet the CCCD tim thay thong tin bhyt cua benh nhan cu theo so the CCCD = " + dataHein.HeinCardNumber + ". " + oldValue + LogUtil.TraceData(" HisPatientSDO searched", patient));
                        this.SelectOnePatientProcess(patient);
                        this.SetPatientSearchPanel(true);
                        this.txtPatientCode.Text = patient.PATIENT_CODE;
                        if (String.IsNullOrEmpty(this.txtAddress.Text))
                            this.txtAddress.Text = patient.ADDRESS;
                    }
                    else
                    {
                        if (patients.Count > 1)
                        {
                            LogSystem.Debug("Quet the BHYT tim thay " + patients.Count + " benh nhan cu => mo form chon benh nhan => chon 1 => fill du lieu bn duoc chon.");
                            frmPatientChoice frm = new frmPatientChoice(patients, this.SelectOnePatientProcess);
                            frm.ShowDialog();
                        }
                        else
                        {
                            LogSystem.Debug("Quet the BHYT tim thay thong tin bhyt cua benh nhan cu theo so the HeinCardNumber = " + dataHein.HeinCardNumber + ". " + LogUtil.TraceData("HisPatientSDO searched", patients[0]));
                            //An hien cac button lam moi thong tin benh nhan
                            this.SelectOnePatientProcess(patients[0]);
                            this.SetPatientSearchPanel(true);
                            this.txtPatientCode.Text = patients[0].PATIENT_CODE;
                            if (String.IsNullOrEmpty(this.txtAddress.Text))
                                this.txtAddress.Text = patients[0].ADDRESS;
                        }
                    }
                }
                else//Nguoc lai so the chua ton tai tren he thong, hien thi du lieu da doc duoc tu qrcode len form
                {
                    Inventec.Common.Logging.LogSystem.Warn("___2____");
                    LogSystem.Debug("Quet the BHYT khong tim thay Bn cu => fill du lieu theo du lieu gih tren the bhyt");
                    this.currentPatientSDO = null;
                    FillDataAfterFindQrCodeNoExistsCard(dataHein);                    
                    txtPatientCode.Text = "";
                    txtPatientCode.Update();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadOneBNToControlWithThread()
        {
            try
            {
                this.FillDataPatientToControlWithThread();
                this.FillDataToExamServiceReqNewestByPatientWithThread();
                this.PeriosTreatmentMessage();
                this.GetCareerByConfig();
                this.FocusInServiceRoomInfo();
                if (this.isReadQrCode)
                    this.FillDataAfterFindQrCodeNoExistsCardWithThread();
                else
                    this.FillDataToHeinInsuranceInfoUCHeinByOldPatientWithThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToHeinInsuranceInfoUCHeinByOldPatientWithThread()
        {
            try
            {
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null && (this.typeCodeFind == typeCodeFind__CCCDCMND || this.currentPatientSDO.HasBirthCertificate == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE))
                    this.mainHeinProcessor.FillDataToHeinInsuranceInfoByOldPatient(this.ucHeinBHYT, this.currentPatientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientToControlWithThread()
        {
            try
            {
                this.FillDataPatientToControl(this.currentPatientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToExamServiceReqNewestByPatientWithThread()
        {
            try
            {
                //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)
                this.FillDataToExamServiceReqNewestByPatient(this.currentPatientSDO);
                LogSystem.Debug("Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataAfterFindQrCodeNoExistsCardWithThread()
        {
            try
            {
                FillDataAfterFindQrCodeNoExistsCard(this._HeinCardData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetCareerByConfig()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(this._HeinCardData.HeinCardNumber);

                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                if (career != null)
                    career = HisConfigCFG.CareerUnder6Age;
                else if (dtPatientDob.DateTime != DateTime.MinValue)
                {
                    if (dtPatientDob.DateTime != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtPatientDob.DateTime))
                        career = HisConfigCFG.CareerUnder6Age;
                    else if (DateTime.Now.Year - dtPatientDob.DateTime.Year <= 18)
                        career = HisConfigCFG.CareerHS;
                    else
                        career = HisConfigCFG.CareerBase;
                }

                if (career == null)
                    career = HisConfigCFG.CareerBase;

                if (career != null && career.ID > 0)
                {
                    cboCareer.EditValue = career.ID;
                    txtCareerCode.Text = career.CAREER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeComboSelected()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()));
                if (patientType != null)
                {
                    this.currentPatientTypeAllowByPatientType = this.LoadPatientTypeExamByPatientType(patientType.ID);
                    this.txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;
                    if (this.cboPatientType.OldEditValue == null
                        || ((this.cboPatientType.EditValue) != this.cboPatientType.OldEditValue
                        && this.cboPatientType.OldEditValue != null))
                    {
                        if (patientType.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT
                            || patientType.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__QN)
                        {
                            this.ChoiceTemplateHeinCard(patientType.PATIENT_TYPE_CODE, true);
                        }
                        else if (patientType.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__KSK)
                        {
                            this.pnlUCHeinInformation.Controls.Clear();
                            KskContractInput kskContractInput = new KskContractInput();
                            kskContractInput.DeleOutFocus = OutFocusKskContract;
                            this.kskContractProcessor = new KskContractProcessor(TemplateType.ENUM.TEMPLATE_1);
                            this.ucKskContract = (UserControl)kskContractProcessor.Run(kskContractInput);
                            int height = ucKskContract.Height;
                            this.gboxHeinCardInformation.Expanded = true;
                            this.gboxHeinCardInformation.Height = height + 50;
                            this.pnlUCHeinInformation.Controls.Add(ucKskContract);
                            this.ucKskContract.Dock = DockStyle.Fill;
                            this.ucHeinBHYT = null;
                        }
                        else
                        {
                            this.pnlUCHeinInformation.Controls.Clear();
                            this.pnlUCHeinInformation.Update();
                            this.FocusInServiceRoomInfo();
                            this.ucHeinBHYT = null;
                            this.gboxHeinCardInformation.Expanded = false;
                        }
                        this.SetDefaultFocusUserControl();
                        this.RendererServiceReqControl();
                    }

                    this.txtMaTHX.Focus();
                    this.txtMaTHX.SelectAll();

                    //Tu dong tich kham thu sau theo cau hinh // tiennv
                    this.chkIsNotRequireFee.Checked = false;
                    if (AppConfigs.PatientIdIsNotRequireExamFee != null
                        && AppConfigs.PatientIdIsNotRequireExamFee.Count > 0
                        && AppConfigs.PatientIdIsNotRequireExamFee.Contains(patientType.ID))
                    {
                        this.chkIsNotRequireFee.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OutFocusKskContract()
        {
            try
            {
                this.FocusInServiceRoomInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RendererServiceReqControl()
        {
            try
            {
                this.pnlServiceRoomInfomation.Controls.Clear();
                this.InitExamServiceRoom(true, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool PatientCodeKeyDown()
        {
            bool result = false;
            try
            {
                string strValue = this.txtPatientCode.Text;
                if (!String.IsNullOrEmpty(strValue))
                {
                    if (strValue.Length > 10 && strValue.Contains("|"))
                    {
                        //LogSystem.Debug("PatientCodeKeyDown");
                        //LogSystem.Debug(strValue);
                        //Truong hop quet the bang dau doc qrcode
                        //Hien thi ra thong tin cua benh nhan + the
                        this.ProcessDataQrCodeHeinCard(strValue);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private long GetTreatmentIdFromResultData()
        {
            long result = 0;
            try
            {
                if (this.resultHisPatientProfileSDO != null)
                {
                    result = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    result = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void GetPatientInfoFromResultData(ref AssignServiceADO assignServiceADO)
        {
            try
            {
                if (this.resultHisPatientProfileSDO != null)
                {
                    assignServiceADO.PatientName = this.resultHisPatientProfileSDO.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.resultHisPatientProfileSDO.HisPatient.DOB;
                    assignServiceADO.GenderName = (BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.resultHisPatientProfileSDO.HisPatient.GENDER_ID) ?? new HIS_GENDER()).GENDER_NAME;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    assignServiceADO.PatientName = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.DOB;
                    assignServiceADO.GenderName = (BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.GENDER_ID) ?? new HIS_GENDER()).GENDER_NAME;
                }
                if (this.currentHisExamServiceReqResultSDO != null && this.currentHisExamServiceReqResultSDO.ServiceReqs != null && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0)
                {
                    var serviceReqHasPriority = this.currentHisExamServiceReqResultSDO.ServiceReqs.FirstOrDefault(o => o.PRIORITY == (long)1);

                    assignServiceADO.IsPriority = serviceReqHasPriority != null ? serviceReqHasPriority.PRIORITY == (long)1 : false;
                }
                if (HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService && this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0 && this.serviceReqDetailSDOs.Exists(o => (o.RoomId ?? 0) > 0))
                {
                    assignServiceADO.ExamRegisterRoomId = this.serviceReqDetailSDOs.Where(o => (o.RoomId ?? 0) > 0).FirstOrDefault().RoomId;
                    Inventec.Common.Logging.LogSystem.Info("Tiep don benh nhan___co cau hinh:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService), HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService) + " => luon mac dinh phong chi dinh khi chi dinh dv tu tiep don theo phong kham dau tien nguoi dung chon");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_TREATMENT GetTreatmentViewByResult()
        {
            V_HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.GetTreatmentIdFromResultData();
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result ?? new V_HIS_TREATMENT();
        }

        private V_HIS_TREATMENT_FEE GetTreatmentFeeViewByResult()
        {
            V_HIS_TREATMENT_FEE result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter treatmentFilter = new HisTreatmentFeeViewFilter();
                treatmentFilter.ID = this.GetTreatmentIdFromResultData();
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result ?? new V_HIS_TREATMENT_FEE();
        }

        private void LoadDoiTuongCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    this.cboPatientType.EditValue = null;
                    this.FocusShowpopup(this.cboPatientType, true);
                }
                else
                {
                    var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                    var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.PATIENT_TYPE_CODE.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                    if (searchResult != null && searchResult.Count == 1)
                    {
                        this.cboPatientType.EditValue = searchResult[0].ID;
                        this.txtPatientTypeCode.Text = searchResult[0].PATIENT_TYPE_CODE;
                        if (((cboPatientType.EditValue) != this.cboPatientType.OldEditValue && this.cboPatientType.OldEditValue != null))
                        {
                            this.PatientTypeComboSelected();
                        }

                        this.txtMaTHX.Focus();
                        this.txtMaTHX.SelectAll();
                    }
                    else
                    {
                        this.cboPatientType.EditValue = null;
                        this.FocusShowpopup(this.cboPatientType, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataQrCodeHeinCard(string qrCode)
        {
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                //Reset doi tuong V_HIS_CARD ve null, muc dich de khi luu se khong gui len cardCode
                this.cardSearch = null;

                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                HeinCardData dataHein = readQrCode.ReadDataQrCode(qrCode);
                ProcessQrCodeData(dataHein);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExecuteRoomInfo_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (ExecuteRoomADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (e.Column.FieldName == "EXECUTE_ROOM_NAME_1" || e.Column.FieldName == "AMOUNT_1")
                    {
                        try
                        {
                            if (data.MAX_BY_DAY_1 < data.TOTAL_TODAY_1)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_2" || e.Column.FieldName == "AMOUNT_2")
                    {
                        try
                        {
                            if (data.MAX_BY_DAY_2 < data.TOTAL_TODAY_2)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_3" || e.Column.FieldName == "AMOUNT_3")
                    {
                        try
                        {
                            if (data.MAX_BY_DAY_3 < data.TOTAL_TODAY_3)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_4" || e.Column.FieldName == "AMOUNT_4")
                    {
                        try
                        {
                            if (data.MAX_BY_DAY_4 < data.TOTAL_TODAY_4)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROOM_NAME_5" || e.Column.FieldName == "AMOUNT_5")
                    {
                        try
                        {
                            if (data.MAX_BY_DAY_5 < data.TOTAL_TODAY_5)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTimer()
        {
            try
            {
                if (AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham > 0)
                {
                    timer1.Interval = Convert.ToInt32((int)AppConfigs.DangKyTiepDonThoiGianLoadDanhSachPhongKham);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.FillDataToGirdExecuteRoomInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGirdExecuteRoomInfo()
        {
            try
            {
                this.gridControlExecuteRoomInfo.BeginUpdate();
                this.gridControlExecuteRoomInfo.DataSource = LoadExecuteRoomProcess.listAdo;
                this.gridControlExecuteRoomInfo.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBranch()
        {
            try
            {
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch;
                this._BranchTimes = new List<HIS_BRANCH_TIME>();
                if (branch != null && branch.IS_USE_BRANCH_TIME == 1)
                {
                    _IsUserBranchTime = true;

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBranchTimeFilter filter = new MOS.Filter.HisBranchTimeFilter();
                    filter.BRANCH_ID = branch.ID;
                    this._BranchTimes = new BackendAdapter(param).Get<List<HIS_BRANCH_TIME>>("api/HisBranchTime/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
