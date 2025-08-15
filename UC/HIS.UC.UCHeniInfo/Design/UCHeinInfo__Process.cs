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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.SDO;
using HIS.UC.UCHeniInfo.ControlProcess;
using HIS.UC.UCHeniInfo.Utils;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using HIS.UC.UCHeniInfo.Data;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.LibraryHein.Bhyt;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using HIS.UC.UCHeniInfo.CustomValidateRule;
using DevExpress.Utils;
using DevExpress.Utils.OAuth.Provider;
using HIS.Desktop.Plugins.Library.RegisterConfig;

namespace HIS.UC.UCHeniInfo
{
    public partial class UCHeinInfo : UserControlBase
    {
        #region Public Method

        /// <summary>
        /// Kiểm tra hạn của thẻ bhyt, nếu hạn thẻ nhỏ hơn cấu hình giới hạn số ngày cảnh báo hạn thẻ => show cảnh báo cho người dùng
        /// số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1
        /// </summary>
        /// <param name="alertExpriedTimeHeinCardBhyt"></param>
        /// <param name="resultDayAlert"></param>
        /// <returns>số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1</returns>
        public long GetExpriedTimeHeinCardBhyt(long alertExpriedTimeHeinCardBhyt, ref long resultDayAlert)
        {
            long result = -1;
            try
            {
                if (this.chkHasCardTemp.Checked)
                {
                    result = -1;//Truong hop chung sinh tra ve hop le vi ngay server se tu sinh
                }
                else
                {
                    DateTime? dtHeinCardFromTime = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                    DateTime? dtHeinCardToTime = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    if (dtHeinCardFromTime != null && dtHeinCardFromTime.Value != DateTime.MinValue
                        && dtHeinCardToTime != null && dtHeinCardToTime.Value != DateTime.MinValue)
                    {
                        DateTime dtToTime = dtHeinCardToTime.Value;
                        result = (long)((dtToTime.Date - DateTime.Now.Date).TotalDays);
                        if (result > alertExpriedTimeHeinCardBhyt)
                        {
                            //Chua het han, set lai gia tri tra ve la 0
                            //Cho su dung se kiem tra neu tra ve khong la ngay hop le
                            result = -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Hàm xử lý việc kiểm tra dữ liệu bhyt (PatientTypeAlter) có phải dữ liệu BN cũ không, nếu đúng thì fill dữ liệu thẻ bhyt của Bn cũ, ngược lại fill dữ liệu theo thông tin truyền vào
        /// </summary>
        /// <param name="patyAlterBhyt"></param>
        public void ChangeDataHeinInsuranceInfoByPatientTypeAlter(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patyAlterBhyt)
        {
            try
            {
                if (patyAlterBhyt.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    return;
                }

                this.isTempQN = patyAlterBhyt.IS_TEMP_QN == 1 ? true : false;
                this.chkHasCardTemp.Checked = (patyAlterBhyt.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE || patyAlterBhyt.IS_TEMP_QN == 1);

                //#17877
                //- Với bệnh nhân cũ, khi tiếp đón, *không tự động lấy thông tin "đúng tuyến/trái tuyến/cấp cứu/giới thiệu/hẹn khám" của lần điều trị gần nhất, mà xử lý như khi tiếp đón mới (suy ra từ các thông tin mã nơi KCB ban đầu, ...)
                //if (patyAlterBhyt.ID > 0)
                //{
                //this.FillDataHeinInsuranceBySelectedPatientTypeAlter(patyAlterBhyt);
                //}
                //Trường hợp dữ liệu truyền vào là dữ liệu BN mới chưa đăng ký => Kiểm tra các trường hợp hẹn khám, kiểm tra tuyến bệnh viện,... để hiển thị đúng theo trường hợp đấy
                //else
                //{
                if (!String.IsNullOrEmpty(patyAlterBhyt.HEIN_MEDI_ORG_CODE))
                {
                    this.cboDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                    this.txtMaDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                    this.MediOrgSelectRowChange(false, patyAlterBhyt.LIVE_AREA_CODE);
                    this.FillDataHeinInsuranceBySelectedPatientTypeAlter(patyAlterBhyt);
                }
                else
                {
                    this.cboDKKCBBD.EditValue = null;
                    this.txtMaDKKCBBD.EditValue = null;
                }

                //Nếu BN là trẻ em & tích vào có giấy chứng sinh => disable tất cả các control
                if (patyAlterBhyt.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    this.txtFreeCoPainTime.Enabled = false;
                    this.dtFreeCoPainTime.Visible = false;

                    this.chkHasCardTemp.Checked = true;
                    this.chkHasCardTemp.Enabled = false;
                    this.cboHeinRightRoute.Enabled = false;
                    this.txtAddress.Enabled = false;
                    this.txtMaDKKCBBD.Enabled = cboDKKCBBD.Enabled = false;

                    this.txtMaDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                    this.cboDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                    if (!String.IsNullOrEmpty(patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE))
                        this.cboHeinRightRoute.EditValue = patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE;
                    else if (!String.IsNullOrEmpty(patyAlterBhyt.RIGHT_ROUTE_CODE))
                        this.cboHeinRightRoute.EditValue = patyAlterBhyt.RIGHT_ROUTE_CODE;
                    else
                        this.cboHeinRightRoute.EditValue = null;

                    DataStore.MediOrgForHasDobCretidentials = new List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO>();
                    HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO heinMediOrgData = new HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO();
                    heinMediOrgData.MEDI_ORG_CODE = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                    heinMediOrgData.MEDI_ORG_NAME = patyAlterBhyt.HEIN_MEDI_ORG_NAME;
                    heinMediOrgData.MEDI_ORG_NAME_UNSIGNED = Inventec.Common.String.Convert.UnSignVNese2(patyAlterBhyt.HEIN_MEDI_ORG_NAME);
                    DataStore.MediOrgForHasDobCretidentials.Add(heinMediOrgData);
                    EditorLoaderProcessor.InitComboCommon(this.cboDKKCBBD, DataStore.MediOrgForHasDobCretidentials, "ID", "MEDI_ORG_NAME", "MEDI_ORG_CODE");
                    //MediOrgProcess.LoadDataToComboNoiDKKCBBD(this.cboDKKCBBD, DataStore.MediOrgForHasDobCretidentials);

                    if (this.cboNoiSong.Enabled)
                    {
                        this.cboNoiSong.Focus();
                        this.cboNoiSong.ShowPopup();
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ChangeCheckCardTemp(bool? isChild, bool? isQN)
        {
            try
            {
                if (isChild != null)
                {
                    //chkHasCardTemp.Checked = false;//Ktra lai xem co can phai uncheck k//xuandv
                    if (!chkHasCardTemp.Checked)
                    {
                        this.chkHasCardTemp.Enabled = isChild.Value;
                        this.lcicboKCBBĐ.Enabled = isChild.Value;
                        this.lciFreeCoPainTime.Enabled = isChild.Value;
                        this.lciHeinCardAddress.Enabled = isChild.Value;
                        this.lciHeinCardFromTime.Enabled = isChild.Value;
                        this.lciHeinCardNumber.Enabled = isChild.Value;
                        this.lciHeinCardToTime.Enabled = isChild.Value;
                        this.lciHeinRightRoute.Enabled = isChild.Value;
                        this.lciJoin5Year.Enabled = isChild.Value;
                        this.lciKCBBĐ.Enabled = isChild.Value;
                        this.lciKV.Enabled = isChild.Value;
                        this.lciPaid6Month.Enabled = isChild.Value;
                        //this.ResetValidate();
                    }
                }
                if (isQN != null)
                {
                    chkHasCardTemp.Checked = false;
                    chkHasCardTemp.Enabled = isQN.Value;

                    this.lcicboKCBBĐ.Enabled = !isQN.Value;
                    this.lciFreeCoPainTime.Enabled = !isQN.Value;
                    this.lciHeinCardAddress.Enabled = !isQN.Value;
                    this.lciHeinCardFromTime.Enabled = !isQN.Value;
                    this.lciHeinCardNumber.Enabled = !isQN.Value;
                    this.lciHeinCardToTime.Enabled = !isQN.Value;
                    this.lciHeinRightRoute.Enabled = !isQN.Value;
                    this.lciJoin5Year.Enabled = !isQN.Value;
                    this.lciKCBBĐ.Enabled = !isQN.Value;
                    this.lciKV.Enabled = !isQN.Value;
                    this.lciPaid6Month.Enabled = !isQN.Value;
                    //this.ResetValidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataByHeinCardData(HeinCardData dataHein)
        {
            try
            {
                this.txtSoThe.Text = dataHein.HeinCardNumber;
                HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO dataMediOrg = MediOrgDataWorker.MediOrgADOs.FirstOrDefault(o => o.MEDI_ORG_CODE == dataHein.MediOrgCode);
                if (dataMediOrg != null)
                {
                    this.txtMaDKKCBBD.Text = dataMediOrg.MEDI_ORG_CODE;
                    this.cboDKKCBBD.EditValue = dataMediOrg.MEDI_ORG_CODE;
                    this.MediOrgSelectRowChange(false, dataHein.LiveAreaCode);
                }
                else
                {
                    this.cboDKKCBBD.EditValue = null;
                    this.txtMaDKKCBBD.EditValue = null;
                }
                if (this.currentPatientSdo != null)
                {
                    if (!String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode))
                    {
                        if (!HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(dataHein.MediOrgCode))
                        {
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                            this.ProcessCaseWrongRoute(this.currentPatientSdo.HeinMediOrgCode);
                        }
                        else
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                        //if (this.dlgAutoCheckCC != null)
                        //    this.dlgAutoCheckCC(false);
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                    }
                }
                //else
                //{
                //    if (this.IsDungTuyenCapCuuByTime)
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                //    else
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                //}
                if (!String.IsNullOrEmpty(dataHein.FromDate))
                {
                    this.txtHeinCardFromTime.Text = dataHein.FromDate;
                    this.dtHeinCardFromTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                }
                else
                {
                    this.txtHeinCardFromTime.Text = "";
                    this.dtHeinCardFromTime.EditValue = null;
                }

                if (!String.IsNullOrEmpty(dataHein.ToDate))
                {
                    this.ChangeDataByCard = true;
                    this.txtHeinCardToTime.Text = dataHein.ToDate;
                    this.dtHeinCardToTime.EditValue = HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                    this.ChangeDataByCard = false;
                }
                else
                {
                    this.txtHeinCardToTime.Text = "";
                    this.dtHeinCardToTime.EditValue = null;
                }

                if (!String.IsNullOrEmpty(dataHein.FineYearMonthDate) && !HisConfigCFG.IsNotAutoCheck5Y6M)
                {
                    DateTime? _dt = HeinUtils.ConvertDateStringToSystemDate(dataHein.FineYearMonthDate);
                    this.txtDu5Nam.Text = dataHein.FineYearMonthDate;
                    this.dtDu5Nam.EditValue = _dt;
                    string ngayHeThong = DateTime.Now.ToString("dd/MM/yyyy");
                    DateTime? _dtNow = HeinUtils.ConvertDateStringToSystemDate(ngayHeThong);
                    if (_dt < _dtNow)
                    {
                        this.chkJoin5Year.Checked = true;
                    }
                    else
                    {
                        this.chkJoin5Year.Checked = false;
                    }
                }
                else
                {
                    this.txtDu5Nam.Text = "";
                    this.dtDu5Nam.EditValue = null;
                    this.chkJoin5Year.Checked = false;
                }

                MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaData liveArea = MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.GetByCode(dataHein.LiveAreaCode);
                this.cboNoiSong.EditValue = ((liveArea != null) ? liveArea.HeinLiveCode : null);
                //this.chkJoin5Year.Checked = this.chkPaid6Month.Checked = false;
                this.chkPaid6Month.Checked = false;
                if (!String.IsNullOrEmpty(dataHein.MediOrgCode)
                    && !String.IsNullOrEmpty(dataHein.PatientName)
                    && !String.IsNullOrEmpty(dataHein.Dob)
                    && !String.IsNullOrEmpty(dataHein.Gender))
                {
                    //xuandv
                    string _address = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address);
                    if (string.IsNullOrEmpty(_address))
                    {
                        this.txtAddress.Text = dataHein.Address;
                    }
                    else
                        this.txtAddress.Text = _address;
                }
                else
                    this.txtAddress.Text = dataHein.Address;
                if ((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                        && !HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(this.txtMaDKKCBBD.Text)
                        && this.cboHeinRightRoute.EditValue != null && (this.cboHeinRightRoute.EditValue ?? "") == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                {
                    if (this.IsDefaultRightRouteType && HisConfigCFG.IsAllowedRouteTypeByDefault)
                    {
                        this.InitDefaultRightRouteType();
                        Inventec.Common.Logging.LogSystem.Debug("Quet the bhyt load thong tin the. this.IsDefaultRightRouteType = true");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Quet the bhyt load thong tin the. show thong bao phai chon truong hop");
                        this.cboHeinRightRoute.Focus();
                        this.cboHeinRightRoute.SelectAll();
                        this.cboHeinRightRoute.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private Method
        private void ProcessCaseWrongRoute(string mediOrgCode, string liveArea = "")
        {
            try
            {
                //List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> datas = new List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>();
                //datas.AddRange(DataStore.HeinRightRouteTypes);
                //if (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediOrgCode
                //    || (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL != HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT &&
                //    MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE != HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT))
                //{
                //    datas = datas.Where(p => p.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER).ToList();
                //}

                if (!string.IsNullOrEmpty(mediOrgCode) &&
                    (!String.IsNullOrWhiteSpace(HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE) && HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE.Contains(mediOrgCode)
                || (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediOrgCode
                || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                || this.IsMediOrgRightRouteByCurrent(mediOrgCode)
                || ((HisConfigCFG.IsNotRequiredRightTypeInCaseOfHavingAreaCode && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                )))
                    
                {
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    dataSourceCboHeinRightRouteTemp = DataStore.HeinRightRouteTypes;
                }
                else
                {
                    if (chkHasAbsentLetter.Checked || chkHasWorkingLetter.Checked || chkIsTt46.Checked || chkSs.Checked)
                        return;
                    var datas = DataStore.HeinRightRouteTypes.Where(p => p.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE).ToList();
                    EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, datas, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    dataSourceCboHeinRightRouteTemp = datas;
                    if ((cboHeinRightRoute.EditValue ?? "").ToString() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        cboHeinRightRoute.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm kiểm tra cơ sở kcbbd theo mã cơ sở hiện tại có phải là đúng tuyến tuyến dưới hay không
        /// </summary>
        /// <param name="checkCurrentCode"></param>
        /// <returns></returns>
        private bool IsMediOrgRightRouteByCurrent(string checkCurrentCode)
        {
            bool result = false;
            try
            {
                List<string> codesAccept = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_CODES__ACCEPT;
                result = (codesAccept != null && codesAccept.Contains(checkCurrentCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Hàm xử lý sau khi tìm ra 1 BN cũ => fill dữ liệu bệnh nhân & dữ liệu đối tượng điều trị, thông tin thẻ, chuyển tuyến,... lên form tiếp đón
        /// </summary>
        /// <param name="patientSDO"></param>
        private bool FillDataAfterSelectOnePatient(HisPatientSDO patientSDO)
        {
            bool valid = true;
            try
            {
                if (patientSDO == null) throw new ArgumentNullException("Du lieu dau vao khong hop le => patientSDO is null");

                this.currentPatientSdo = patientSDO;

                //Call register module fill data to control
                if (this.dlgfillDataPatientSDOToRegisterForm != null)
                    valid = this.dlgfillDataPatientSDOToRegisterForm(this.currentPatientSdo);

                if (valid)
                    this.SetValue(this.currentPatientSdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        string oldHeinCardNumber = null;
        private void CheckExamHistoryFromBHXHApi(Action focusNextControl)
        {
            try
            {
                // this.heinCardData = new HeinCardData();
                heinCardData.HeinCardNumber = this.txtSoThe.Text.Trim();
                heinCardData.MediOrgCode = txtMaDKKCBBD.Text;
                heinCardData.Address = txtAddress.Text;
                if (!String.IsNullOrEmpty(this.txtHeinCardFromTime.Text.Trim()))
                    heinCardData.FromDate = this.txtHeinCardFromTime.Text.Trim();
                if (!String.IsNullOrEmpty(this.txtHeinCardToTime.Text.Trim()))
                    heinCardData.ToDate = this.txtHeinCardToTime.Text.Trim();
                else
                    heinCardData.ToDate = "";
                heinCardData.Gender = null;
                if (ResultDataADO != null && (ResultDataADO.ResultHistoryLDO.maKetQua == "001" || ResultDataADO.ResultHistoryLDO.maKetQua == "002"|| ResultDataADO.ResultHistoryLDO.maKetQua == "050") && oldHeinCardNumber == heinCardData.HeinCardNumber)
                {
                    return;
                }
                oldHeinCardNumber = heinCardData.HeinCardNumber;
                if (String.IsNullOrEmpty(heinCardData.HeinCardNumber)
                    //|| String.IsNullOrEmpty(heinCardData.Address)
                    //|| String.IsNullOrEmpty(heinCardData.FromDate)
                    //|| String.IsNullOrEmpty(heinCardData.MediOrgCode)
                    )
                    return;
                else
                    this.dlgCheckTT(heinCardData, focusNextControl);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Fill dữ liệu vùng thông tin thẻ bhyt theo dữ liệu thẻ cảu BN cũ (PatientTypeAlter)
        /// </summary>
        /// <param name="patientTypeAlter"></param>
        /// <param name="isFocus"></param>
        private void FillDataHeinInsuranceBySelectedPatientTypeAlter(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("FillDataHeinInsuranceBySelectedPatientTypeAlter");
                this.isPatientOld = true;
                if (patientTypeAlter == null) throw new ArgumentNullException("patientTypeAlter is null");

                this.txtSoThe.Text = patientTypeAlter.HEIN_CARD_NUMBER;
                if (!String.IsNullOrEmpty(patientTypeAlter.HEIN_MEDI_ORG_CODE))
                    this.ProcessCaseWrongRoute(patientTypeAlter.HEIN_MEDI_ORG_CODE, patientTypeAlter.LIVE_AREA_CODE);
                this.txtHeinCardFromTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0);
                this.dtHeinCardFromTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                this.txtHeinCardToTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_TO_TIME ?? 0);
                this.dtHeinCardToTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                if (patientTypeAlter.JOIN_5_YEAR_TIME != null && !HisConfigCFG.IsNotAutoCheck5Y6M)
                {
                    this.txtDu5Nam.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.JOIN_5_YEAR_TIME ?? 0);
                    this.dtDu5Nam.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    this.chkJoin5Year.Checked = (patientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE);
                }
                else
                {
                    this.txtDu5Nam.Text = "";
                    this.dtDu5Nam.EditValue = null;
                    this.chkJoin5Year.Checked = false;
                }

                this.cboNoiSong.EditValue = patientTypeAlter.LIVE_AREA_CODE;
                if (!HisConfigCFG.IsNotAutoCheck5Y6M)
                    this.chkPaid6Month.Checked = (patientTypeAlter.PAID_6_MONTH == MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE);
                else
                    this.chkPaid6Month.Checked = false;
                if (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    var listMediOrg = this.cboDKKCBBD.Properties.DataSource as List<HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO>;
                    HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO heinMediOrgDataNew = new HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO();
                    heinMediOrgDataNew.MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                    heinMediOrgDataNew.MEDI_ORG_NAME = patientTypeAlter.HEIN_MEDI_ORG_NAME;
                    listMediOrg.Add(heinMediOrgDataNew);
                    this.cboDKKCBBD.Properties.DataSource = listMediOrg;
                    this.chkHasCardTemp.Enabled = false;
                }

                this.txtMaDKKCBBD.Text = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                this.cboDKKCBBD.EditValue = patientTypeAlter.HEIN_MEDI_ORG_CODE;

                this.txtAddress.Text = patientTypeAlter.ADDRESS;
                //if ((patientTypeAlter.FREE_CO_PAID_TIME ?? 0) > 0)
                //{
                //    this.txtFreeCoPainTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.FREE_CO_PAID_TIME ?? 0);
                //    this.dtFreeCoPainTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                //}
                //else
                //{
                this.txtFreeCoPainTime.Text = "";
                this.dtFreeCoPainTime.EditValue = null;
                //}

                //if (this.currentPatientSdo != null
                //    && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode))
                //{
                //    if (!HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(patientTypeAlter.HEIN_MEDI_ORG_CODE))
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                //    else
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                //}
                //else if (!this.IsDungTuyenCapCuuByTime)
                //{
                //    //Kiem tra du lieu trong DB, neu la dung tuyen
                //    if (patientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                //    {
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                //        //Kiem tra loại đúng tuyến là đúng tuyến cấp cứu/ đúng tuyến giới thiệu hoặc đúng tuyến(la noi kham chua benh hoac tuyen duoi)
                //        if (!String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE))
                //            cboHeinRightRoute.EditValue = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                //        else if (String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE))//Dung tuyen la noi kham chua benh hoac tuyen duoi
                //        {
                //            cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                //        }
                //    }
                //    else//Truong hop trai tuyen               
                //    {
                //        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
                //    }
                //}
                this.chkHasCardTemp.Checked = (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE || patientTypeAlter.IS_TEMP_QN == 1);
                if (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                    this.SetTuyenTruongHopTreEmCoTheTam(patientTypeAlter);
                var treatmentType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeAlter.TREATMENT_TYPE_ID);
                this.ChangeDefaultHeinRatio((treatmentType != null ? treatmentType.TREATMENT_TYPE_CODE : ""));

                //if (this.IsDungTuyenCapCuuByTime == false)
                //    this.cboHeinRightRoute.EditValue = (String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE) == true ? patientTypeAlter.RIGHT_ROUTE_CODE : patientTypeAlter.RIGHT_ROUTE_TYPE_CODE);
                //else
                //    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeAlter), patientTypeAlter));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTuyenTruongHopTreEmCoTheTam(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            try
            {
                bool isEnable = (!this.chkHasCardTemp.Checked);
                this.lcicboKCBBĐ.Enabled = isEnable;
                this.lciFreeCoPainTime.Enabled = isEnable;
                this.lciHeinCardAddress.Enabled = isEnable;
                this.lciHeinCardFromTime.Enabled = isEnable;
                this.lciHeinCardNumber.Enabled = isEnable;
                this.lciHeinCardToTime.Enabled = isEnable;
                this.lciHeinRightRoute.Enabled = isEnable;
                this.lciJoin5Year.Enabled = isEnable;
                this.lciKCBBĐ.Enabled = isEnable;
                if (this.lciKhongKTHSD.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
                {
                    this.lciKhongKTHSD.Enabled = isEnable;
                }

                this.lciKV.Enabled = isEnable;
                this.lciPaid6Month.Enabled = isEnable;
                this.lci5Y.Enabled = isEnable;

                EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                cboHeinRightRoute.EditValue = (String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE) == true ? patientTypeAlter.RIGHT_ROUTE_CODE : patientTypeAlter.RIGHT_ROUTE_TYPE_CODE);
                dataSourceCboHeinRightRouteTemp = DataStore.HeinRightRouteTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm tính lại tỉ lệ bhyt chi trả
        /// </summary>
        /// <param name="isReightRoute"></param>
        /// <param name="treatmentTypeCode"></param>
        private void ChangeDefaultHeinRatio()
        {
            try
            {
                this.ChangeDefaultHeinRatio(MOS.LibraryHein.Bhyt.HeinTreatmentType.HeinTreatmentTypeCode.EXAM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm tính lại tỉ lệ bhyt chi trả
        /// </summary>
        /// <param name="isReightRoute"></param>
        /// <param name="treatmentTypeCode"></param>
        private void ChangeDefaultHeinRatio(string treatmentTypeCode)
        {
            try
            {
                string heincardNumber = Utils.HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text.Replace(" ", "").ToUpper());
                BhytPatientTypeData patientTypeData = new BhytPatientTypeData();
                patientTypeData.HAS_BIRTH_CERTIFICATE = (this.chkHasCardTemp.Checked ? MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE : MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.FALSE);
                patientTypeData.LIVE_AREA_CODE = this.cboNoiSong.Text;
                patientTypeData.RIGHT_ROUTE_CODE = (((cboHeinRightRoute.EditValue ?? "").ToString() != MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE) ? MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE : MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE);
                patientTypeData.JOIN_5_YEAR = this.chkJoin5Year.Checked ? MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE : MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.FALSE;
                patientTypeData.PAID_6_MONTH = this.chkPaid6Month.Checked ? MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE : MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.FALSE;
                patientTypeData.RIGHT_ROUTE_TYPE_CODE = (this.cboHeinRightRoute.EditValue ?? "").ToString();
                patientTypeData.HEIN_MEDI_ORG_CODE = (string)this.cboDKKCBBD.EditValue;
                patientTypeData.HEIN_MEDI_ORG_NAME = this.cboDKKCBBD.Text;
                patientTypeData.LEVEL_CODE = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                this.txtMucHuong.Text = GetDefaultHeinRatio(patientTypeData, heincardNumber, treatmentTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetDefaultHeinRatio(BhytPatientTypeData patientTypeData, string heinCardNumber, string treatmentTypeCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, patientTypeData.LEVEL_CODE, patientTypeData.RIGHT_ROUTE_CODE) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Hàm xử lý lấy thông tin cơ sở kcb theo mã, sau đó gán kết quả tìm được vào giao diện
        /// </summary>
        /// <param name="searchCode"></param>
        /// <param name="isExpand"></param>
        private void LoadNoiDKKCBBDCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    ResetEditorControl.ResetAndFocus(this.cboDKKCBBD, false, true);
                }
                else
                {
                    var data = MediOrgDataWorker.MediOrgADOs.Where(o => o.MEDI_ORG_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO one = null;
                        one = (data.Count == 1 ? data[0] : MediOrgDataWorker.MediOrgADOs.FirstOrDefault(o => o.MEDI_ORG_CODE.Equals(searchCode)));
                        if (one != null)
                        {
                            this.cboDKKCBBD.EditValue = one.MEDI_ORG_CODE;
                            this.txtMaDKKCBBD.Text = one.MEDI_ORG_CODE;
                            this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());

                            string heinCardNumber = txtSoThe.Text;
                            heinCardNumber = heinCardNumber.Replace(" ", "").ToUpper().Trim();
                            heinCardNumber = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber);
                            this.CheckExamHistoryFromBHXHApi(FocusOutOfUc);
                            if (cboHeinRightRoute.EditValue == null)
                            {
                                this.cboHeinRightRoute.Focus();
                                this.cboHeinRightRoute.ShowPopup();
                            }
                            else if (dlgFocusNextUserControl != null)
                                dlgFocusNextUserControl();
                        }
                        else
                        {
                            ResetEditorControl.ResetAndFocus(this.cboDKKCBBD, false, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusOutOfUc()
        {
            try
            {
                if (cboHeinRightRoute.EditValue == null)
                {
                    this.cboHeinRightRoute.Focus();
                    this.cboHeinRightRoute.ShowPopup();
                }
                else if (dlgFocusNextUserControl != null)
                    dlgFocusNextUserControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý thay đổi cơ sở kcbbd theo luồng cũ (chỉ refesh vùng thông tin liên quan đến cơ sở kcb, tuyến,..)
        /// </summary>
        /// <param name="isFocus"></param>
        private void MediOrgSelectRowChange(bool isFocus, string liveArea = "") // tientv : #7631
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("MediOrgSelectRowChange_____");
                HIS.Desktop.LocalStorage.BackendData.ADO.MediOrgADO mediorg = MediOrgDataWorker.MediOrgADOs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                if (mediorg != null)
                {
                    this.txtMaDKKCBBD.Text = mediorg.MEDI_ORG_CODE;
                    /*  ngoài giờ hành chính luôn hiển thị là cấp cứu.
                        trong giờ hành chính:
                     +  nếu đúng CSKCBBD hoặc tuyến huyện/xã: luôn hiển thị đúng tuyến.
                     +  nếu key cấu hình là 1 thì là "Giới thiệu", key khác 1 thì để null cho người dùng chọn.
                        trường hợp hẹn khám thì hiển thị "Hẹn khám", không quan tâm trong giờ người giờ.
                     * */

                   
                        if (this.IsDungTuyenCapCuuByTime)
                        {
                         this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                            this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                       
                    }
                        else if (!this.IsDungTuyenCapCuuByTime && this.currentPatientSdo != null
                            && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode)
                            && !HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(mediorg.MEDI_ORG_CODE))
                        {
                        
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                            this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;                       
                    }
                        // Tu dong chon dung tuyen
                        else if (
                            HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediorg.MEDI_ORG_CODE
                            || this.IsMediOrgRightRouteByCurrent(mediorg.MEDI_ORG_CODE)
                            || (!String.IsNullOrWhiteSpace(HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE) && HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.SYS_MEDI_ORG_CODE.Contains(mediorg.MEDI_ORG_CODE))
                        )
                        {
                            this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                        }
                        else
                        {
                            InitDefaultValidRightRouteType(isFocus, mediorg.MEDI_ORG_CODE);
                            if (IsDefaultRightRouteType)
                            {                             
                                this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;            
                            }
                            else
                            {
                                if (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT == mediorg.MEDI_ORG_CODE
                                                           || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                                                           || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                                {
                                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                                }
                                else
                                {
                                    this.cboHeinRightRoute.EditValue = null;
                                Inventec.Common.Logging.LogSystem.Debug("MediOrgSelectRowChange. Truong hop khong set mac dinh cho combo truong hop");
                                }
                            }
                        }
					
                    //xuandv6734
                    //Xử lý luôn luôn fix là đúng tuyến với các trường hợp cơ sở kcbbd là đúng tuyến/thông tuyến/tuyến dưới
                    this.ProcessCaseWrongRoute(mediorg.MEDI_ORG_CODE, liveArea);
                    this.ChangeDefaultHeinRatio();

                    this.TuDongChonLoaiThongTuyen(mediorg);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultValidRightRouteType(bool isFocus, string mediOrgCode)
        {
            try
            {
                if (this.HasChangeValidRightRouteType(mediOrgCode)
                        && isFocus
                        && this.cboHeinRightRoute.EditValue == null)
                {
                    if (this.IsDefaultRightRouteType)
                        this.InitDefaultRightRouteType();
                    else
                    {
                        //string tieude = "Mã đăng ký KCB BĐ khác với của viện. Bạn phải chọn trường hợp!";
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaDangKyKCBBDKhacVoiCuaVien, "Cảnh báo", DefaultBoolean.True);
                    }
                }

                if (!this.IsDefaultRightRouteType && this.currentPatientSdo == null)
                {
                    this.cboHeinRightRoute.EditValue = null;
                    //  this.cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool HasChangeValidRightRouteType(string heinMediOrgCode)
        {
            bool hasValid = false;
            try
            {
                //Nếu tuyến của bệnh viện là tuyến "Trung ương" & tuyến "Tỉnh" => bắt buộc chọn ô trường hợp
                if (((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT //this.HeinLevelCodeCurrent
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT// this.HeinLevelCodeCurrent
                    )
                    && !HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(heinMediOrgCode)))
                {
                    //Neu CSKCBBD khong phai dung co so benh vien thi phai check valid cho truong 'truong hop'
                    this.ValidRightRouteType(heinMediOrgCode);
                    hasValid = true;
                }
                //Ngược lại không bắt buộc nhập trường hợp
                else
                {
                    this.dxValidationProviderControl.SetValidationRule(cboHeinRightRoute, null);
                    this.lciHeinRightRoute.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return hasValid;
        }

        private void ValidRightRouteType(string heinMediOrgCode)
        {
            try
            {
                //if (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT
                //        || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT)
                //{
                //    if (!String.IsNullOrEmpty(heinMediOrgCode)
                //        && HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT != heinMediOrgCode)
                //    {
                TemplateHeinBHYT1__RightRouteType__ValidationRule oDobDateRule = new TemplateHeinBHYT1__RightRouteType__ValidationRule();
                oDobDateRule.cboHeinRightRoute = cboHeinRightRoute;
                oDobDateRule.cboNoiSong = cboNoiSong;
                oDobDateRule.txtMaDKKCBBD = txtMaDKKCBBD;
                oDobDateRule.IsTempQN = isTempQN;
                oDobDateRule.chkTempQN = chkHasCardTemp;
                oDobDateRule.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(cboHeinRightRoute, oDobDateRule);
                this.lciHeinRightRoute.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultRightRouteType()
        {
            try
            {
                this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefautlRightRouteTypeByKeyAndTime() //tientv : #7631
        {
            try
            {
                EditorLoaderProcessor.InitComboCommon(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                dataSourceCboHeinRightRouteTemp = DataStore.HeinRightRouteTypes;
                if (IsDungTuyenCapCuuByTime == true && HisConfigCFG.IsAllowedRouteTypeByDefault)
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                //else if (IsDefaultRightRouteType == true)
                //    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                else
                    this.cboHeinRightRoute.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCHeinInfo/LoadDefautlRightRouteTypeByKeyAndTime: \n" + ex);
            }
        }
        #endregion
    }
}
