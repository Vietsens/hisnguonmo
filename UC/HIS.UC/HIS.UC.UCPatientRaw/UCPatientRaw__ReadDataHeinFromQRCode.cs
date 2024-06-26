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
using Inventec.Core;
using MOS.SDO;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.QrCodeBHYT;
using HIS.UC.UCPatientRaw.Base;
using HIS.Desktop.ApiConsumer;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : UserControl
    {

        // trả về thông tin thẻ nếu bệnh nhân có thẻ
        private async Task<object> SearchByCode(string code)
        {
            try
            {
                //LogSystem.Info("SearchByCode => 1");
                var data = new HisPatientSDO();
                if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code is null");
                if (code.Length > 10 && code.Contains("|"))
                {
                    return await GetDataQrCodeHeinCard(code).ConfigureAwait(false);
                }
                else
                {
                    //ex khi mã sai==> nhạp la mã bhyt
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    data = (await new BackendAdapter(param).GetAsync<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).ConfigureAwait(false)).SingleOrDefault();
                }
                //LogSystem.Info("SearchByCode => 2");
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }

        private async Task<HeinCardData> GetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                // tạo một ado của register bao gồm các đối tuộng ado của các uc con
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private void SetPatientDTOFromCardSDO(HisCardSDO cardSDO, HisPatientSDO patientByCard)
        {
            try
            {
                if (cardSDO == null) throw new ArgumentNullException("cardSDO");
                if (patientByCard == null) throw new ArgumentNullException("patientByCard");

                patientByCard.ID = (cardSDO.PatientId ?? 0);
                patientByCard.PATIENT_CODE = cardSDO.PatientCode;
                patientByCard.FIRST_NAME = cardSDO.FirstName;
                patientByCard.LAST_NAME = cardSDO.LastName;
                patientByCard.ADDRESS = cardSDO.Address;
                patientByCard.CAREER_ID = cardSDO.CareerId;
                patientByCard.CMND_DATE = cardSDO.CmndDate;
                patientByCard.CMND_NUMBER = cardSDO.CmndNumber;
                patientByCard.CMND_PLACE = cardSDO.CmndPlace;
                patientByCard.COMMUNE_NAME = cardSDO.CommuneName;
                patientByCard.DISTRICT_NAME = cardSDO.DistrictName;
                patientByCard.PROVINCE_NAME = cardSDO.ProvinceName;
                patientByCard.DOB = cardSDO.Dob;
                patientByCard.EMAIL = cardSDO.Email;
                patientByCard.ETHNIC_NAME = cardSDO.EthnicName;
                if (cardSDO.Dob > 0 && cardSDO.Dob.ToString().Length == 4)
                    patientByCard.IS_HAS_NOT_DAY_DOB = 1;
                else
                    patientByCard.IS_HAS_NOT_DAY_DOB = 0;
                patientByCard.PHONE = cardSDO.Phone;
                //patientByCard.RECENT_ROOM_ID = cardSDO.ReligionName;//TODO
                //patientByCard.RECENT_SERVICE_ID = cardSDO.Address;//TODO
                patientByCard.RELIGION_NAME = cardSDO.ReligionName;
                patientByCard.VIR_ADDRESS = cardSDO.VirAddress;
                patientByCard.VIR_PATIENT_NAME = patientByCard.LAST_NAME + " " + patientByCard.FIRST_NAME;
                //patientByCard.GENDER_CODE = cardSDO.GenderCode;
                //patientByCard.GENDER_NAME = cardSDO.GenderName;
                //var geneder = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == cardSDO.GenderId);
                patientByCard.GENDER_ID = cardSDO.GenderId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataPatientToControl(HisPatientSDO patientDTO)
        {
            try
            {
                //if (patientDTO == null) throw new ArgumentNullException("patientDTO is null");
                //this.txtRelativeCMNDNumber.Text = patientDTO.RELATIVE_CMND_NUMBER;
                //if (this.btnCodeFind.Text == this.typeCodeFind__MaBN)
                //{
                //    this.txtPatientCode.Text = patientDTO.PATIENT_CODE;
                //}
                //this.txtPatientName.Text = patientDTO.VIR_PATIENT_NAME;
                //if (patientDTO.DOB > 0 && patientDTO.DOB.ToString().Length >= 6)
                //{
                //    if (patientDTO.IS_HAS_NOT_DAY_DOB == 1)
                //        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, true);
                //    else
                //        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, false);
                //}

                //MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patientDTO.GENDER_ID);
                //if (gioitinh != null)
                //{
                //    this.cboGender.EditValue = gioitinh.ID;
                //    this.txtGenderCode.Text = gioitinh.GENDER_CODE;
                //}
                //this.txtAddress.Text = patientDTO.ADDRESS;
                //var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_NAME == patientDTO.NATIONAL_NAME);
                //if (national != null)
                //{
                //    this.cboNational.EditValue = national.NATIONAL_NAME;
                //    this.txtNationalCode.Text = national.NATIONAL_CODE;
                //}
                //var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_NAME == patientDTO.ETHNIC_NAME);
                //if (ethnic != null)
                //{
                //    this.cboEthnic.EditValue = ethnic.ETHNIC_NAME;
                //    this.txtCareerCode.Text = ethnic.ETHNIC_CODE;
                //}

                ////LogSystem.Debug(patientDTO.HeinCardNumber);
                //MOS.EFMODEL.DataModels.HIS_CAREER career = this.GetCareerByBhytWhiteListConfig(patientDTO.HeinCardNumber);

                ////Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //if (career != null)
                //{
                //    // LogSystem.Debug(patientDTO.HeinCardNumber);
                //    this.FillDataCareerUnder6AgeByHeinCardNumber(patientDTO.HeinCardNumber);
                //}
                //else
                //{
                //    career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == patientDTO.CAREER_ID);
                //}
                //if (career != null)
                //{
                //    this.cboCareer.EditValue = patientDTO.CAREER_ID;
                //    this.txtCareerCode.Text = career.CAREER_CODE;
                //}
                ////xuandv Kiểm tra xem là trẻ em hay không để validate thông tin người nhà
                //bool isTE = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.Now);
                //if (isTE)
                //{
                //    this.SetValidationByChildrenUnder6Years(isTE);
                //}
                //var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_NAME == patientDTO.PROVINCE_NAME);
                //if (province != null)
                //{
                //    this.cboProvince.EditValue = province.PROVINCE_CODE;
                //    this.txtProvinceCode.Text = province.PROVINCE_CODE;
                //    this.LoadHuyenCombo("", province.PROVINCE_CODE, false);
                //}
                //var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().FirstOrDefault(o => (o.INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDTO.DISTRICT_NAME && o.PROVINCE_NAME == patientDTO.PROVINCE_NAME);
                //if (district != null)
                //{
                //    this.cboDistrict.EditValue = district.DISTRICT_CODE;
                //    this.txtDistrictCode.Text = district.DISTRICT_CODE;
                //    this.LoadXaCombo("", district.DISTRICT_CODE, false);
                //}
                //var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().FirstOrDefault(o =>
                //    (o.INITIAL_NAME + " " + o.COMMUNE_NAME) == patientDTO.COMMUNE_NAME
                //    && (o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == patientDTO.DISTRICT_NAME
                //    //&& o.COMMUNE_CODE == patientDTO.COMMUNE_CODE//TODO
                //    );
                //if (commune != null)
                //{
                //    this.cboCommune.EditValue = commune.COMMUNE_CODE;
                //    this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                //    this.cboTHX.EditValue = commune.ID;
                //    this.txtMaTHX.Text = commune.SEARCH_CODE;
                //}
                //else if (province != null && district != null)
                //{
                //    var communeTHX = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().FirstOrDefault(o =>
                //    (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                //    && o.ID < 0);
                //    if (communeTHX != null)
                //    {
                //        this.cboTHX.EditValue = communeTHX.ID;
                //        this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                //    }
                //}

                //if (Config.AppConfigs.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                //{
                //    if (this.workPlaceProcessor != null)
                //        this.workPlaceProcessor.SetValue(this.ucWorkPlace, patientDTO.WORK_PLACE);
                //}
                //else
                //{
                //    if (patientDTO.WORK_PLACE_ID > 0)
                //    {
                //        var workPlace = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == patientDTO.WORK_PLACE_ID);
                //        if (workPlace != null)
                //        {
                //            if (this.workPlaceProcessor != null)
                //                this.workPlaceProcessor.SetValue(this.ucWorkPlace, workPlace);
                //        }
                //        else
                //        {
                //            if (this.workPlaceProcessor != null)
                //                this.workPlaceProcessor.SetValue(this.ucWorkPlace, null);
                //        }
                //    }
                //}

                //var militaryRank = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MILITARY_RANK>().FirstOrDefault(o => o.ID == (patientDTO.MILITARY_RANK_ID ?? 0));
                //this.txtMilitaryRankCode.Text = (militaryRank != null ? militaryRank.MILITARY_RANK_CODE : "");
                //this.cboMilitaryRank.EditValue = patientDTO.MILITARY_RANK_ID;
                //this.txtPhone.Text = patientDTO.PHONE;
                //this.txtHomePerson.Text = patientDTO.RELATIVE_TYPE;
                //this.txtCorrelated.Text = patientDTO.RELATIVE_NAME;
                //this.txtRelativeAddress.Text = patientDTO.RELATIVE_ADDRESS;

                //this.chkIsChronic.Checked = (patientDTO.IS_CHRONIC == 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToHeinCardControlByCardSDO(HisCardSDO cardSDO)
        {
            try
            {
                if (!String.IsNullOrEmpty(cardSDO.HeinCardNumber))
                {
                    if (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(cardSDO.HeinCardNumber))
                    {
                        HIS_PATIENT_TYPE_ALTER patientTypeALter = new HIS_PATIENT_TYPE_ALTER();
                        patientTypeALter.HEIN_CARD_NUMBER = cardSDO.HeinCardNumber;
                        patientTypeALter.HEIN_CARD_FROM_TIME = cardSDO.HeinCardFromTime;
                        patientTypeALter.HEIN_CARD_TO_TIME = cardSDO.HeinCardToTime;
                        patientTypeALter.HEIN_MEDI_ORG_CODE = cardSDO.HeinOrgCode;
                        patientTypeALter.HEIN_MEDI_ORG_NAME = cardSDO.HeinOrgName;
                        patientTypeALter.ADDRESS = cardSDO.HeinAddress;
                        patientTypeALter.JOIN_5_YEAR = cardSDO.Join5Year;
                        patientTypeALter.PAID_6_MONTH = cardSDO.Paid6Month;
                        patientTypeALter.LEVEL_CODE = cardSDO.LevelCode;
                        patientTypeALter.LIVE_AREA_CODE = cardSDO.LiveAreaCode;
                        patientTypeALter.RIGHT_ROUTE_CODE = cardSDO.RightRouteCode;

                        // kiểm tra nếu đang mở UCHeinInfo thì fill data vào UCHeinInfo
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("So the bhyt (tu du lieu tra ve khi quet the thong minh vao dau doc) khong hop le. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cardSDO), cardSDO));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
