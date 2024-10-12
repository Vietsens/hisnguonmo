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
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.QrCodeCCCD;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private void SetFocusDelegate()
        {
            try
            {
                this.txtIntructionTime.Focus();
                this.txtIntructionTime.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ChoiceTemplateHeinCard(string patientTypeCode, bool focusMoveOut)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("t3.1: begin process ChoiceTemplateHeinCard");
                this.pnlUCHeinInformation.Controls.Clear();
                this.pnlUCHeinInformation.Update();
                this.pnlUCHeinInformation.Enabled = true;
                this.ucHeinBHYT = new UserControl();
                this.mainHeinProcessor = new His.UC.UCHein.MainHisHeinBhyt();
                His.UC.UCHein.Base.LanguageManager.Init(Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (patientTypeCode == HisConfigCFG.PatientTypeCode__BHYT || patientTypeCode == HisConfigCFG.PatientTypeCode__QN)
                {
                    Inventec.Common.Logging.LogSystem.Debug("t3.1.1: set default data to control hein");
                    His.UC.UCHein.Data.DataInitHeinBhyt dataHein = new His.UC.UCHein.Data.DataInitHeinBhyt();
                    dataHein.BhytWhiteLists = BackendDataWorker.Get<HIS_BHYT_WHITELIST>();
                    dataHein.BhytBlackLists = BackendDataWorker.Get<HIS_BHYT_BLACKLIST>();
                    dataHein.Genders = BackendDataWorker.Get<HIS_GENDER>();
                    dataHein.Template = His.UC.UCHein.MainHisHeinBhyt.TEMPLATE__BHYT1;
                    dataHein.IsChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtPatientDob.DateTime);
                    dataHein.HEIN_LEVEL_CODE__CURRENT = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    dataHein.HeinRightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                    dataHein.Icds = BackendDataWorker.Get<HIS_ICD>();
                    dataHein.LiveAreas = MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.Get();
                    dataHein.MEDI_ORG_CODE__CURRENT = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                    dataHein.MEDI_ORG_CODES__ACCEPTs = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_CODES__ACCEPT;
                    dataHein.SYS_MEDI_ORG_CODE = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE;
                    dataHein.MediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    dataHein.PATIENT_TYPE_ID__BHYT = HisConfigCFG.PatientTypeId__BHYT;
                    dataHein.PatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                    if (patientTypeCode == HisConfigCFG.PatientTypeCode__QN && !string.IsNullOrEmpty(HisConfigCFG.CheckTempQN))
                    {
                        dataHein.IsTempQN = HisConfigCFG.CheckTempQN.Contains(patientTypeCode);
                    }

                    dataHein.TranPatiForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                    dataHein.TranPatiReasons = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                    dataHein.TREATMENT_TYPE_ID__EXAM = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    dataHein.TreatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                    dataHein.PatientTypeId = HisConfigCFG.PatientTypeId__BHYT;
                    dataHein.isVisibleControl = AppConfigs.TiepDon_HienThiMotSoThongTinThemBenhNhan;
                    dataHein.IsShowCheckKhongKTHSD = HisConfigCFG.IsShowCheckExpired;
                    dataHein.AutoCheckIcd = HisConfigCFG.AutoCheckIcd;
                    dataHein.IsDefaultRightRouteType = (HisConfigCFG.IsDefaultRightRouteType == IsDefaultRightRouteType__True ? true : false);
                    dataHein.FillDataPatientSDOToRegisterForm = this.LoadOneBNToControlForUCHein;
                    dataHein.SetFocusMoveOut = this.FocusDelegate;
                    dataHein.SetShortcutKeyDown = this.ShortcutDelegate;
                    dataHein.AutoCheckCC = this.AutoSetCheckCC;
                    dataHein.CheckExamHistory = this.CheckTTFull;
                    dataHein.ProcessFillDataCareerUnder6AgeByHeinCardNumber = this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber;
                    dataHein.IsDungTuyenCapCuuByTime = this._IsDungTuyenCapCuuByTime;
                    dataHein.IsObligatoryTranferMediOrg = HisConfigCFG.IsObligatoryTranferMediOrg;
                    dataHein.ActChangePatientDob = this.ProcessWhileChangeDOb;
                    Inventec.Common.Logging.LogSystem.Debug("t3.1.2: uCMainHein init");
                    this.ucHeinBHYT = this.mainHeinProcessor.InitUC(dataHein, ApiConsumers.MosConsumer, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage());
                    if (this.ucHeinBHYT != null)
                    {
                        int height = this.ucHeinBHYT.Height;

                        //Hien thi vung nhap thong tin the bhyt
                        this.gboxHeinCardInformation.Expanded = true;
                        this.gboxHeinCardInformation.Height = height + 50;

                        this.pnlUCHeinInformation.Controls.Add(this.ucHeinBHYT);
                        this.ucHeinBHYT.Dock = DockStyle.Fill;
                        Inventec.Common.Logging.LogSystem.Debug("t3.1.3: set delegate");
                        Inventec.Common.Logging.LogSystem.Debug("t3.1.4: end");
                    }
                }
                //else if (patientTypeCode == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisPatientTypeCFG.PATIENT_TYPE_CODE__KSK)
                //{
                //    His.UC.UCHein.Data.DataInitKskContract dataHein = new His.UC.UCHein.Data.DataInitKskContract();
                //    dataHein.Template = His.UC.UCHein.MainHisHeinBhyt.TEMPLATE__KSK_CONTRACT1;
                //    dataHein.SetFocusMoveOut = FocusDelegate;
                //    dataHein.KskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                //    ucHein__BHYT = await uCMainHein.InitUC(dataHein, ApiConsumers.MosConsumer, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage()).ConfigureAwait(false);
                //    if (ucHein__BHYT != null)
                //    {
                //        int height = ucHein__BHYT.Height;
                //        ucHein__BHYT.Dock = DockStyle.Fill;
                //        pnlUCHeinInformation.Controls.Add(ucHein__BHYT);
                //        //His.UC.UCHein.Data.DataSetDelegate dataSetDelegate = new His.UC.UCHein.Data.DataSetDelegate();
                //        //dataSetDelegate.UC = ucHein__BHYT;
                //        //dataSetDelegate.FocusDelegate = FocusDelegate;
                //        //uCMainHein.SetDelegateSetFocusMoveOut(dataSetDelegate);

                //        //Hien thi vung nhap thong tin kham suc khoe
                //        gboxHeinCardInformation.Expanded = true;
                //        gboxHeinCardInformation.Height = height + 40;
                //    }
                //}
                else
                {
                    this.pnlUCHeinInformation.Controls.Clear();
                    this.pnlUCHeinInformation.Update();
                    if (focusMoveOut)
                        this.FocusInServiceRoomInfo();
                    this.ucHeinBHYT = null;
                    this.gboxHeinCardInformation.Expanded = false;
                }

                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.ResetValidationControl(this.ucHeinBHYT);

                //Lấy danh sách id các đối tượng thanh toán được phép chuyển đổi từ đối tượng bệnh nhân
                GlobalStore.PatientTypeIdAllows = (!String.IsNullOrEmpty(patientTypeCode) ? BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>()
                    .Where(o => o.PATIENT_TYPE_CODE == patientTypeCode)
                    .Select(o => o.PATIENT_TYPE_ALLOW_ID).ToList() : null);

                Inventec.Common.Logging.LogSystem.Debug("t3.2: end process ChoiceTemplateHeinCard");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessWhileChangeDOb()
        {
            try
            {
                if (mainHeinProcessor != null && ucHeinBHYT != null)
                {
                    DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                    HisPatientProfileSDO patientProfileSDO = new HisPatientProfileSDO();
                    this.mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, patientProfileSDO);
                    this.mainHeinProcessor.PatientOldUnder6(this.ucHeinBHYT, MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtPatientDob.DateTime));
                    this.AutoCheckPriorityByPriorityType(Inventec.Common.TypeConvert.Parse.ToInt64(dt.Value.ToString("yyyyMMdd") + "000000"), patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(HeinCardData heinCard, bool isSearchHeinCardNumber)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinCard), heinCard) + "____this.txtPatientDob.Text=" + this.txtPatientDob.Text);
                if (heinCard != null && !String.IsNullOrEmpty(heinCard.HeinCardNumber))
                {
                    this.FillDataCareerUnder6AgeByHeinCardNumber(heinCard.HeinCardNumber);
                }
                DateTime? dt = DateTimeHelper.ConvertDateStringToSystemDate(this.txtPatientDob.Text);
                this.AutoCheckPriorityByPriorityType(Inventec.Common.TypeConvert.Parse.ToInt64(dt.Value.ToString("yyyyMMdd") + "000000"), heinCard.HeinCardNumber);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusDelegate()
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

        private void ShortcutDelegate(Keys key)
        {
            try
            {
                switch (key)
                {
                    case Keys.I:
                        this.btnSaveAndPrint.PerformClick();
                        break;
                    case Keys.S:
                        this.btnSave.PerformClick();
                        break;
                    case Keys.N:
                        this.btnNewContinue.PerformClick();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoSetCheckCC(bool value)
        {
            try
            {
                this.chkEmergency.Checked = value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetPatyAlterBhyt(HIS_PATIENT_TYPE_ALTER patyAlterBhyt, HisPatientSDO patientSdo)
        {
            try
            {
                if (patyAlterBhyt == null) throw new ArgumentNullException("patyAlterBhyt");
                if (patientSdo == null) throw new ArgumentNullException("patientSdo");

                patyAlterBhyt.ADDRESS = patientSdo.HeinAddress;
                patyAlterBhyt.HEIN_CARD_FROM_TIME = (patientSdo.HeinCardFromTime ?? 0);
                patyAlterBhyt.HEIN_CARD_TO_TIME = (patientSdo.HeinCardToTime ?? 0);
                patyAlterBhyt.HEIN_CARD_NUMBER = patientSdo.HeinCardNumber;
                patyAlterBhyt.HEIN_MEDI_ORG_CODE = patientSdo.HeinMediOrgCode;
                patyAlterBhyt.HEIN_MEDI_ORG_NAME = patientSdo.HeinMediOrgName;
                patyAlterBhyt.JOIN_5_YEAR = patientSdo.Join5Year;
                patyAlterBhyt.LIVE_AREA_CODE = patientSdo.LiveAreaCode;
                patyAlterBhyt.PAID_6_MONTH = patientSdo.Paid6Month;
                patyAlterBhyt.RIGHT_ROUTE_CODE = patientSdo.RightRouteCode;
                patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE = patientSdo.RightRouteTypeCode;
                patyAlterBhyt.TDL_PATIENT_ID = patientSdo.ID;
                patyAlterBhyt.PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                //patyAlterBhyt.HAS_BIRTH_CERTIFICATE = patientSdo.;
                patyAlterBhyt.IS_NO_CHECK_EXPIRE = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataAfterFindQrCodeNoExistsCard(HeinCardData dataHein)
        {
            try
            {
                string paName = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                this.txtPatientName.Text = String.IsNullOrEmpty(paName) ? dataHein.PatientName : paName;

                if (!String.IsNullOrEmpty(dataHein.Dob))
                {
                    if (dataHein.Dob.Length == 4)
                    {
                        this.isNotPatientDayDob = true;
                        this.dtPatientDob.EditValue = new DateTime(Inventec.Common.TypeConvert.Parse.ToInt32(dataHein.Dob), 1, 1);
                        this.txtPatientDob.Text = dataHein.Dob;
                    }
                    else
                    {
                        this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(dataHein.Dob);
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    }
                    this.dtPatientDob.Update();
                }
                if (!String.IsNullOrEmpty(dataHein.Gender))
                {

                    var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                    var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == dataGenderId);
                    if (gender != null)
                    {
                        this.txtGenderCode.Text = gender.GENDER_CODE;
                        this.cboGender.EditValue = gender.ID;
                    }
                }
                this.CalulatePatientAge(txtPatientDob.Text, false);
                this.SetValueCareerComboByCondition();

                //Kiem tra cau hinh co tu dong fill du lieu dia chi ghi tren the vao o dia chi benh nhan, co thi fill du lieu, khong thi bo qua
                string paAddress = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address);
                if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                    this.txtAddress.Text = String.IsNullOrEmpty(paAddress) ? dataHein.Address : paAddress;
                else
                    this.txtAddress.Text = "";
                Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                var data = adProc.SplitFromFullAddress(dataHein.Address);
                if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1 || (typeCodeFind == typeCodeFind__CCCDCMND && (currentPatientSDO == null)) || (currentPatientSDO != null && data != null && (currentPatientSDO.PROVINCE_CODE != data.ProvinceCode || currentPatientSDO.DISTRICT_CODE != data.DistrictCode || currentPatientSDO.COMMUNE_CODE != data.CommuneCode)))
                {
                    cboProvince.EditValue = data.ProvinceCode;
                    txtProvinceCode.EditValue = data.ProvinceCode;
                    cboDistrict.EditValue = data.DistrictCode;
                    txtDistrictCode.EditValue = data.DistrictCode;
                    cboCommune.EditValue = data.CommuneCode;
                    txtCommuneCode.EditValue = data.CommuneCode;
                }
                //Cap nhat thong tin doc tu the vao vung thong tin bhyt
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.FillDataAfterFindQrCode(this.ucHeinBHYT, dataHein);

                this.txtPatientCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<object> SearchByCode(string code)
        {
            try
            {
                //LogSystem.Debug("SearchByCode => 1");
                var data = new HisPatientSDO();
                if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code is null");
                if (code.Length > 12 && code.Contains("|") && this.typeCodeFind == typeCodeFind__CCCDCMND)
                {
                    this.typeReceptionForm = Base.ReceptionForm.QrCCCD;
                    return GetDataQrCodeCccdCard(code);
                }
                else if (code.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null) && this.typeCodeFind == typeCodeFind__CCCDCMND)
                {
                    this.typeReceptionForm = Base.ReceptionForm.QrCCCD;
                    CccdCardData cccd = new CccdCardData();
                    cccd.CardData = code.Trim();
                    if (!string.IsNullOrEmpty(txtPatientDob.Text))
                        cccd.Dob = txtPatientDob.Text.Contains("/") ? txtPatientDob.Text : ProcessDate(txtPatientDob.Text);
                    else
                        cccd.Dob = dtPatientDob.Text;
                    cccd.PatientName = txtPatientName.Text.Trim();
                    return cccd;
                }
                else if ((code.Trim().Length == 9 || code.Trim().Length == 12) && this.typeCodeFind == typeCodeFind__CCCDCMND)
                {
                    isReadQrCccdData = false;
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    if (code.Trim().Length == 9)
                    {
                        filter.CMND_NUMBER__EXACT = code.Trim();
                    }
                    else
                    {
                        filter.CCCD_NUMBER__EXACT = code.Trim();
                    }
                    var dataList = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (dataList != null && dataList.Count > 0)
                    {
                        if (dataList.Count > 1)
                        {
                            frmPatientChoice frm = new frmPatientChoice(dataList, this.SelectOnePatientProcess);
                            frm.ShowDialog();
                            return null; 
                        }
                        return data = dataList.First();
                    }
                    else
                    {
                        this.txtPatientCode.Focus();
                        this.txtPatientCode.SelectAll();
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaCmndCccdKhongTontai + " '" + code + "'", MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                        return null;
                    }    
                }
                else if (code.Length > 10 && code.Contains("|"))
                {
                    this.typeReceptionForm = Base.ReceptionForm.TheBHYT;
                    this.isReadQrCode = true;
                    return await GetDataQrCodeHeinCard(code);
                }
                else
                {
                    //ex khi mã sai==> nhạp la mã bhyt
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    data = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
                }
                CommonParam paramCommon = new CommonParam();
                HisTreatmentFilter filterTreatment = new HisTreatmentFilter();
                filterTreatment.TDL_PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));

                var lstTreatment = new BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filterTreatment, paramCommon).ToList();
                if (lstTreatment != null && lstTreatment.Count > 0)
                {
                    var treatment = lstTreatment.Where(o => o.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET).ToList();
                    
                    //if (treatment.TREATMENT_END_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                    if (treatment != null && treatment.Count > 0)
                    {
                        //WaitingManager.Hide();
                        if (DevExpress.XtraEditors.XtraMessageBox.
                       Show("Bệnh nhân " + treatment.FirstOrDefault().TDL_PATIENT_NAME + " đã tử vong", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                        {
                            this.ResetPatientForm();
                            data = null;
                        }
                    }
                }
                //LogSystem.Debug("SearchByCode => 2");
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
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);

                BhytHeinProcessor _BhytHeinProcessor = new BhytHeinProcessor();
                if (!_BhytHeinProcessor.IsValidHeinCardNumber(dataHein.HeinCardNumber))
                {
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Mã QR không hợp lệ. Vui lòng kiểm tra lại.", "Thông báo");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private void CalulatePatientAge(string strDob, bool isHasReset)
        {
            try
            {
                //LogSystem.Debug("p1. Bat dau ham tinh tuoi benh nhan.");

                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strDob);
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    bool isGKS = true;
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        this.txtAge.EditValue = "";
                        this.cboAge.EditValue = 4;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    //LogSystem.Debug("p2. Goi thu vien kiem tra nam sinh co phai la tre em hay khong (MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild), Namsinh = " + dtNgSinh.ToString("dd/MM/yyyy HH:mm:ss") + ", ket qua tra ve = " + isGKS);
                    isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);

                    if (nam >= 7)
                    {
                        this.cboAge.EditValue = 1;
                        this.txtAge.Enabled = false;
                        this.cboAge.Enabled = false;
                        if (!isGKS)
                        {
                            this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                        }
                        else
                        {
                            this.txtAge.EditValue = nam.ToString();
                        }
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.cboAge.EditValue = 1;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                                if (!isGKS)
                                {
                                    this.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                                }
                                else
                                {
                                    this.txtAge.EditValue = nam.ToString();
                                }
                            }
                            else
                            {
                                this.txtAge.EditValue = nam * 12 - 1;
                                this.cboAge.EditValue = 2;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.txtAge.EditValue = nam * 12 + thang;
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.txtAge.EditValue = thang.ToString();
                            this.cboAge.EditValue = 2;
                            this.txtAge.Enabled = false;
                            this.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.txtAge.EditValue = ngay.ToString();
                                this.cboAge.EditValue = 3;
                                this.txtAge.Enabled = false;
                                this.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.txtAge.EditValue = "";
                                this.cboAge.EditValue = 4;
                                this.txtAge.Enabled = true;
                                this.cboAge.Enabled = false;
                            }
                        }
                    }
                    var chkHasDob = this.mainHeinProcessor.GetchkHasDobCertificate(this.ucHeinBHYT);
                    if (!(chkHasDob != null && chkHasDob.Checked && isGKS)
                        && this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    {
                        //LogSystem.Debug("p3. Goi ucBhyt cap nhat an hien cac control theo dieu kien la tre em hay khong");
                        this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, isGKS);
                    }

                    this.SetValidationByChildrenUnder6Years(isGKS, isHasReset);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueCareerComboByCondition()
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_CAREER career = null;
                bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(this.dtPatientDob.DateTime);
                if (isGKS)
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
                if (career != null && career.ID > 0)
                {
                    this.cboCareer.EditValue = career.ID;
                    this.txtCareerCode.Text = career.CAREER_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocusUserControl()
        {
            try
            {
                if (this.cboPatientType.EditValue != null)
                {
                    long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT && this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        this.mainHeinProcessor.DefaultFocusUserControl(this.ucHeinBHYT);
                    else if (patientTypeId == HisConfigCFG.PatientTypeId__KSK && this.kskContractProcessor != null && this.ucKskContract != null)
                    {
                        this.kskContractProcessor.InFocus(this.ucKskContract);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetRelativeAddress(bool value)
        {
            try
            {
                //value   false la tick KS
                #region ------
                string adress = "";
                if (this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor == System.Drawing.Color.Maroon)
                {
                    if (!string.IsNullOrEmpty(this.txtAddress.Text))
                    {
                        adress = this.txtAddress.Text + ", ";
                    }
                    if (this.cboCommune.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                        .SingleOrDefault(o =>
                            o.COMMUNE_CODE == this.cboCommune.EditValue.ToString()
                                && (String.IsNullOrEmpty((this.cboDistrict.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (this.cboDistrict.EditValue ?? "").ToString())
                            );
                        if (commune != null)
                        {
                            adress += "Xã " + commune.COMMUNE_NAME + ", ";
                        }
                    }

                    if (this.cboDistrict.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                       .SingleOrDefault(o => o.DISTRICT_CODE == this.cboDistrict.EditValue.ToString()
                           && (String.IsNullOrEmpty((this.cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (this.cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            adress += "Huyện " + district.DISTRICT_NAME + ", ";
                        }
                    }
                    if (this.cboProvince.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            adress += province.PROVINCE_NAME;
                        }
                    }
                }
                this.txtRelativeAddress.Text = adress;
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
