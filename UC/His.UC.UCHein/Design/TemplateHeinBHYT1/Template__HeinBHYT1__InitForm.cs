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
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein.Base;
using His.UC.UCHein.Config;
using His.UC.UCHein.ControlProcess;
using His.UC.UCHein.HisPatient;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.LibraryHein.Bhyt;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {
        /// <summary>
        /// Hàm xử lý việc khởi tạo sẵn đối tượng lưu dữ liệu bệnh nhân cũ gồm các thông tin: thông tin bệnh nhân, số thẻ,...
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="heinCardNumber"></param>
        internal void InitOldPatientData(long patientId, string heinCardNumber)
        {
            try
            {
                if (this.currentPatientSdo == null)
                    this.currentPatientSdo = new HisPatientSDO();
                this.currentPatientSdo.ID = patientId;
                this.currentPatientSdo.HeinCardNumber = heinCardNumber;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khởi tạo, đổ dữ liệu khởi tạo vào control chứa dữu liệu (combo,...)
        /// </summary>
        private void InitDataToControl()
        {
            try
            {
                MediOrgProcess.LoadDataToComboNoiDKKCBBD(this.cboDKKCBBD, DataStore.MediOrgs);
                MediOrgProcess.LoadDataToComboNoiDKKCBBD(this.cboNoiChuyenDen, DataStore.MediOrgs);
                IcdProcess.LoadDataToCombo(this.cboChanDoanTD, DataStore.IcdADOs);
                LiveAreaProcess.LoadDataToComboNoiSong(this.cboNoiSong, DataStore.LiveAreas);
                TranPatiFormProcess.LoadDataToCombo(this.cboHinhThucChuyen, DataStore.TranPatiForms);
                TranPatiReasonProcess.LoadDataToComboLyDoChuyen(this.cboLyDoChuyen, DataStore.TranPatiReasons);
                if (HisConfigCFG.NotDisplayedRouteTypeOver == "1") DataStore.HeinRightRouteTypes = DataStore.HeinRightRouteTypes.Where(p => p.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER).ToList();
                HeinRightRouterTypeProcess.FillDataToComboHeinRightRouterType(this.cboHeinRightRoute, DataStore.HeinRightRouteTypes);
                this.chkHasDobCertificate.Enabled = (this.entity != null && this.entity.IsChild);
                if (this.chkHasDobCertificate.Enabled)
                    this.chkHasDobCertificate.Focus();
                else
                    this.FocusMoveOut();
                chkBaby.Enabled = this.entity.IsChild;
                if (!chkBaby.Enabled)
                    this.chkBaby.Checked = false;
                chkTt46.Checked = false;
                txtTt46.Enabled = false;
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
                this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

                if (valid)
                    this.FillDataToHeinInsuranceInfoByOldPatient(this.currentPatientSdo);

                //Call register module fill data to control
                if (this.dlgfillDataPatientSDOToRegisterForm != null)
                    valid = this.dlgfillDataPatientSDOToRegisterForm(this.currentPatientSdo);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void InitDefaultValidRightRouteType(bool isFocus, string mediOrgCode, string liveArea = "")
        {
            try
            {
                if (entity.IsAutoSelectEmergency)
                {
                    this.AutoSelectEmergency(entity);
                }
                else
                {
                    if ((this.HasChangeValidRightRouteType(mediOrgCode, liveArea)
                          && isFocus
                          && this.cboHeinRightRoute.EditValue == null
                          && this.rdoRightRoute.Checked) || this.IsDefaultRightRouteType)
                    {
                        if (this.IsDefaultRightRouteType)
                            this.InitDefaultRightRouteType();
                        else
                        {
                            if (cboHeinRightRoute.EditValue == null && !SetDefaultRightCode())
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop), Base.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), DefaultBoolean.True);
                            }
                        }
                    }

                    //if (!this.IsDefaultRightRouteType && this.currentPatientSdo == null)
                    //{
                    //    this.txtHeinRightRouteCode.EditValue = null;
                    //    this.cboHeinRightRoute.EditValue = null;
                    //    this.cboHeinRightRoute.Properties.Buttons[1].Visible = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDefaultRightRouteTypeAppointment(string mediOrgCode)
        {
            try
            {
                if (entity.IsAutoSelectEmergency)
                {
                    this.AutoSelectEmergency(entity);
                }
                else if (this.currentPatientSdo != null
                    && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode)
                    && !this.MediOrgCodeCurrent.Equals(mediOrgCode))
                {
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                }
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
        private void FillDataHeinInsuranceBySelectedPatientTypeAlter(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool isFocus)
        {
            try
            {
                if (patientTypeAlter == null) throw new ArgumentNullException("patientTypeAlter is null");

                this.txtSoThe.Text = patientTypeAlter.HEIN_CARD_NUMBER;
                this.txtHeinCardFromTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0);
                this.dtHeinCardFromTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                this.txtHeinCardToTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.HEIN_CARD_TO_TIME ?? 0);
                this.dtHeinCardToTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                if (patientTypeAlter.JOIN_5_YEAR_TIME != null)
                {
                    this.txtDu5Nam.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.JOIN_5_YEAR_TIME ?? 0);
                    this.dtDu5Nam.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                }
                else
                {
                    this.txtDu5Nam.Text = "";
                    this.dtDu5Nam.EditValue = null;
                }
                if (!this.IsDungTuyenCapCuuByTime && !entity.IsAutoSelectEmergency && DataStore.HeinRightRouteTypes.Exists(o => o.HeinRightRouteTypeCode == patientTypeAlter.RIGHT_ROUTE_TYPE_CODE))
                {
                    this.cboHeinRightRoute.EditValue = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                    this.txtHeinRightRouteCode.Text = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = (!String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE));
                }

                if (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    var listMediOrg = this.cboDKKCBBD.Properties.DataSource as List<His.UC.UCHein.Data.MediOrgADO>;
                    His.UC.UCHein.Data.MediOrgADO heinMediOrgDataNew = new His.UC.UCHein.Data.MediOrgADO();
                    heinMediOrgDataNew.MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                    heinMediOrgDataNew.MEDI_ORG_NAME = patientTypeAlter.HEIN_MEDI_ORG_NAME;
                    heinMediOrgDataNew.MEDI_ORG_NAME_UNSIGNED = Inventec.Common.String.Convert.UnSignVNese2(patientTypeAlter.HEIN_MEDI_ORG_NAME);
                    listMediOrg.Add(heinMediOrgDataNew);
                    this.cboDKKCBBD.Properties.DataSource = listMediOrg;
                    this.chkHasDobCertificate.Enabled = false;
                }

                if (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE && patientTypeAlter.PATIENT_TYPE_ID == PatientTypeIdBHYT &&
                    this.entity.IsSampleDepartment && this.entity.HisTreatment != null && this.entity.HisTreatment.IS_PAUSE != 1)
                {
                    this.chkHasDobCertificate.Enabled = true;
                }

                this.txtMaDKKCBBD.Text = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                this.cboDKKCBBD.EditValue = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                this.cboNoiSong.EditValue = patientTypeAlter.LIVE_AREA_CODE;
                IsAutoCheck = true;
                this.chkJoin5Year.Checked = (patientTypeAlter.JOIN_5_YEAR == MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE);
                this.chkPaid6Month.Checked = (patientTypeAlter.PAID_6_MONTH == MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE);
                if(chkBaby.Enabled) 
                    this.chkBaby.Checked = (patientTypeAlter.IS_NEWBORN == 1);
                this.chkHasWorkingLetter.Checked = patientTypeAlter.HAS_WORKING_LETTER == 1;
                this.chkHasAbsentLetter.Checked = patientTypeAlter.HAS_ABSENT_LETTER == 1;
                this.chkTt46.Checked = patientTypeAlter.IS_TT46 == 1;
                this.txtTt46.Text = patientTypeAlter.TT46_NOTE;
                IsAutoCheck = false;
                this.txtAddress.Text = patientTypeAlter.ADDRESS;
                this.txtHNCode.Text = patientTypeAlter.HNCODE;
                if ((patientTypeAlter.FREE_CO_PAID_TIME ?? 0) > 0)
                {
                    this.txtFreeCoPainTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientTypeAlter.FREE_CO_PAID_TIME ?? 0);
                    this.dtFreeCoPainTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtFreeCoPainTime.Text);
                }
                else
                {
                    this.txtFreeCoPainTime.Text = "";
                    this.dtFreeCoPainTime.EditValue = null;
                }

                if (this.currentPatientSdo != null
                    && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode)
                    && !this.MediOrgCodeCurrent.Equals(patientTypeAlter.HEIN_MEDI_ORG_CODE))
                {
                    this.rdoRightRoute.Checked = true;
                    this.rdoWrongRoute.Checked = !this.rdoRightRoute.Checked;
                    this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT;
                    this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT, isFocus);
                }
                else
                {
                    //Kiem tra du lieu trong DB, neu la dung tuyen
                    if (patientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        this.rdoRightRoute.Checked = true;
                        this.rdoWrongRoute.Checked = !this.rdoRightRoute.Checked;
                        if (DataStore.HeinRightRouteTypes.Exists(o => o.HeinRightRouteTypeCode == patientTypeAlter.RIGHT_ROUTE_TYPE_CODE))
                        {
                            this.cboHeinRightRoute.EditValue = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                            this.txtHeinRightRouteCode.Text = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        }
                        //Kiem tra loại đúng tuyến là đúng tuyến cấp cứu/ đúng tuyến giới thiệu hoặc đúng tuyến(la noi kham chua benh hoac tuyen duoi)
                        if (this.IsDungTuyenCapCuuByTime)//Uu tien su dung thoi gian hanh chinh truoc
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC, isFocus);
                        }
                        else if (patientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)//Dung tuyen gioi thieu
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT, isFocus);
                            this.txtHeinRightRouteCode.Enabled = true;
                            this.cboHeinRightRoute.Enabled = true;
                        }
                        else if (patientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)//Dung tuyen cap cuu
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC, isFocus);
                            //Tu dong check vao radio cap cuu ben duoi vung yeu cau
                            //Bo xung them delegate de truong hop nay goi update du lieu ben usercontrol ben ngoai
                            //if (this._AutoCheckCC != null)
                            //    this._AutoCheckCC(true);
                        }
                        else if (patientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT)//Dung tuyen hen kham
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT, isFocus);
                            this.txtHeinRightRouteCode.Enabled = true;
                            this.cboHeinRightRoute.Enabled = true;
                        }
                        else if (String.IsNullOrEmpty(patientTypeAlter.RIGHT_ROUTE_TYPE_CODE))//Dung tuyen la noi kham chua benh hoac tuyen duoi
                        {
                            this.SetEnableControlHein(RightRouterFactory.RIGHT_ROUTER, isFocus);
                        }
                    }
                    else//Truong hop trai tuyen               
                    {
                        this.rdoRightRoute.Checked = false;
                        this.rdoWrongRoute.Checked = !this.rdoRightRoute.Checked;
                        this.txtHeinRightRouteCode.Enabled = false;
                        this.cboHeinRightRoute.Enabled = false;
                        this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER, isFocus);
                    }
                }
                this.chkHasDobCertificate.Checked = (patientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE);
                var treatmentType = DataStore.TreatmentTypes.FirstOrDefault(o => o.ID == patientTypeAlter.TREATMENT_TYPE_ID);
                this.ChangeDefaultHeinRatio((treatmentType != null ? treatmentType.HEIN_TREATMENT_TYPE_CODE : ""));
                if (this.entity.IsChild)
                    this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER, isFocus);
                this.VisibleButtonDeleteHeinRightRoute();
                this.ValidateRightRouteType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm kiểm tra Bn truyền vào có thông tin thẻ BHYt cũ không, nếu tìm thấy 1 thẻ bhyt thì tự động fill dữ liệu vào form, ngược lại không xử lý gì
        /// </summary>
        /// <param name="patientsdo"></param>
        internal void FillDataToHeinInsuranceInfoByOldPatient(HisPatientSDO patientsdo)
        {
            try
            {
                if (patientsdo == null || patientsdo.ID == 0) throw new ArgumentNullException("Du lieu dau vao khong hop le => patientsdo is null");

                this.dxErrorProvider1.ClearErrors();//xuandv clear error

                this.currentPatientSdo = patientsdo;

                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientTypeAlterFilter patientTypeAlterFilter = new MOS.Filter.HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TDL_PATIENT_ID = patientsdo.ID;
                patientTypeAlterFilter.PATIENT_TYPE_ID = PatientTypeIdBHYT;
                List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER> patyAlters = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>(RequestUriStore.HIS_PATIENT_TYPE_ALTER__GET, ApiConsumerStore.MosConsumer, patientTypeAlterFilter, param);

                //Nếu không có dữ liệu thẻ bhytn theo bệnh nhân => không xử lý gì
                if (patyAlters == null || patyAlters.Count == 0) throw new ArgumentNullException("Khong tim thay du lieu HIS_PATIENT_TYPE_ALTER theo benh nhan co PatientId = " + patientsdo.ID + " => patyAlters is null");

                patyAlters = patyAlters.OrderByDescending(o => o.LOG_TIME).ToList();
                var patientTypeAlters = patyAlters.GroupBy(it => new { it.HEIN_CARD_NUMBER, it.HEIN_CARD_FROM_TIME, it.HEIN_CARD_TO_TIME, it.HEIN_MEDI_ORG_CODE, it.JOIN_5_YEAR, it.PAID_6_MONTH, it.RIGHT_ROUTE_CODE, it.RIGHT_ROUTE_TYPE_CODE }).Select(group => group.First()).Distinct().ToList();

                if (patientTypeAlters != null)
                    patientTypeAlters = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).ToList();

                //Load lại dữ liệu combo chọn thẻ bhyt
                ReloadComboSoThe(cboSoThe, patientTypeAlters);
                //HeinCardProcess.LoadDataToCombo(patientTypeAlters, cboSoThe);

                //Nếu dữ liệu bệnh nhân truyền vào có thông tin số thẻ bhyt thì lọc tiếp theo số thẻ bhyt
                if (!String.IsNullOrEmpty(patientsdo.HeinCardNumber))
                    patientTypeAlters = patientTypeAlters.Where(o => o.HEIN_CARD_NUMBER == patientsdo.HeinCardNumber).ToList();
                if (!String.IsNullOrEmpty(patientsdo.AppointmentCode))
                    patientTypeAlters = patientTypeAlters.Where(o => o.HEIN_MEDI_ORG_CODE == patientsdo.HeinMediOrgCode
                        && o.JOIN_5_YEAR == patientsdo.Join5Year
                        && o.PAID_6_MONTH == patientsdo.Paid6Month
                        && o.RIGHT_ROUTE_CODE == patientsdo.RightRouteCode
                        && o.RIGHT_ROUTE_TYPE_CODE == patientsdo.RightRouteTypeCode).ToList();

                //Nếu dữ liệu thẻ bhyt đã có thẻ bhyt, hiển thị mặc định thẻ gần nhất của BN => fill dữ liệu thẻ vào form
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    if (patientTypeAlters.Count > 1)
                        patientTypeAlters = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).ToList();
                    this.cboSoThe.EditValue = patientTypeAlters[0].ID;
                    this.HeinCardSelectRowHandler(patientTypeAlters[0]);
                    if(this.cboHeinRightRoute.EditValue != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.APPOINTMENT)
                        this.MediOrgSelectRowChange(false, (cboNoiSong.EditValue ?? "").ToString());
                    Inventec.Common.Logging.LogSystem.Info("FillDataToHeinInsuranceInfoByOldPatient => Benh nhan co the bhyt, tu dong lay the gan nhat (so the: " + patientTypeAlters[0].HEIN_CARD_NUMBER + ")");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("FillDataToHeinInsuranceInfoByOldPatient => Khong tim thay thong tin the bhyt cua BN, PatientId = " + patientsdo.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
