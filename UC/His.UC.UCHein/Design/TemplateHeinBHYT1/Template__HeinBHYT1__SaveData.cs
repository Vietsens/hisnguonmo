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
    {   /// <summary>
        /// Hàm hiển thị dữ liệu khi BN dưới 6 tuổi
        /// </summary>
        /// <param name="IsChild"></param>
        internal void FillDataPatientOldYnder6(bool IsChild)
        {
            try
            {
                chkBaby.Enabled = IsChild;
                if (!chkBaby.Enabled)
                    this.chkBaby.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        /// <summary>
        /// Hàm lấy dữ liệu từ form cập nhật vào đối tượng HisPatientProfileSDO
        /// </summary>
        /// <param name="patientProfileSDO"></param>
        internal void UpdateDataFormIntoPatientProfile(HisPatientProfileSDO patientProfileSDO)
        {
            try
            {
                if (patientProfileSDO != null && patientProfileSDO.HisPatientTypeAlter != null && patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY)
                {
                    if ((this.cboChanDoanTD.Enabled == true && this.txtMaChanDoanTD.Enabled == true) || entity.IsInitFromCallPatientTypeAlter)
                    {
                        if (patientProfileSDO.HisTreatment == null)
                            patientProfileSDO.HisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();

                        //if (this.cboChanDoanTD.EditValue != null)
                        //    patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboChanDoanTD.EditValue ?? "0").ToString());
                        //else
                        //    patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_ID = null;
                        patientProfileSDO.HisTreatment.IS_TRANSFER_IN = 1;
                        patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_CODE = this.txtMaChanDoanTD.Text.Trim();
                        if (chkHasDialogText.Checked)
                            patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_NAME = this.txtDialogText.Text.Trim();
                        else
                            patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_NAME = this.cboChanDoanTD.Text;

                        patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE = (this.txtMaNoiChuyenDen.EditValue ?? "").ToString();
                        patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME = this.cboNoiChuyenDen.Text;
                        if (this.txtMaHinhThucChuyen.Enabled || entity.IsInitFromCallPatientTypeAlter)
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_CMKT = this.GetInCMKT();
                            if (cboHinhThucChuyen.EditValue != null)
                                patientProfileSDO.HisTreatment.TRANSFER_IN_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboHinhThucChuyen.EditValue ?? "0").ToString());
                            if (cboLyDoChuyen.EditValue != null)
                                patientProfileSDO.HisTreatment.TRANSFER_IN_REASON_ID = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboLyDoChuyen.EditValue ?? "0").ToString());
                        }
                        else
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_CMKT = null;
                            patientProfileSDO.HisTreatment.TRANSFER_IN_FORM_ID = null;
                            patientProfileSDO.HisTreatment.TRANSFER_IN_REASON_ID = null;
                        }
                        if ((dtTransferInTimeFrom.Enabled || entity.IsInitFromCallPatientTypeAlter)
                            && dtTransferInTimeFrom.EditValue != null
                            && dtTransferInTimeFrom.DateTime != DateTime.MinValue)
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTransferInTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                        }
                        else
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_FROM = null;
                        }
                        if ((dtTransferInTimeTo.Enabled || entity.IsInitFromCallPatientTypeAlter)
                            && dtTransferInTimeTo.EditValue != null
                            && dtTransferInTimeTo.DateTime != DateTime.MinValue)
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtTransferInTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                        }
                        else
                        {
                            patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_TO = null;
                        }
                        patientProfileSDO.HisTreatment.TRANSFER_IN_CODE = this.txtInCode.Text.Trim();
                    }
                }
                else if (patientProfileSDO.HisTreatment != null)
                {
                    patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_CODE = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_ICD_NAME = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_CMKT = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_FORM_ID = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_REASON_ID = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_FROM = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_TIME_TO = null;
                    patientProfileSDO.HisTreatment.TRANSFER_IN_CODE = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm lấy dữ liệu từ form 
        /// </summary>
        /// <param name="patientProfileSDO"></param>
        internal void UpdateDataFormIntoPatientTypeAlter(HisPatientProfileSDO patientProfileSDO)
        {
            try
            {
                if (patientProfileSDO.HisPatientTypeAlter == null)
                    patientProfileSDO.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();

                if (this.chkHasDobCertificate.Checked)
                {
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER = "";
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = null;
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME = null;
                    patientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE = MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE;
                }
                else
                    patientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE = MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.FALSE;

                patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER = Utils.HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text);
                patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = (this.cboHeinRightRoute.EditValue ?? "").ToString();
                patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR = (string)(this.chkJoin5Year.Checked == true ? MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE : MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.FALSE);
                patientProfileSDO.HisPatientTypeAlter.PAID_6_MONTH = (string)(this.chkPaid6Month.Checked == true ? MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE : MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.FALSE);
                patientProfileSDO.HisPatientTypeAlter.LIVE_AREA_CODE = (this.cboNoiSong.EditValue ?? "").ToString();
                patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE = (this.cboDKKCBBD.EditValue ?? "").ToString();
                patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_NAME = this.cboDKKCBBD.Text;
                patientProfileSDO.HisPatientTypeAlter.LEVEL_CODE = this.HeinLevelCodeCurrent;
                patientProfileSDO.HisPatientTypeAlter.TREATMENT_TYPE_ID = this.TreatmentTypeIdExam;

                if (this.chkTempQN.Checked)
                    patientProfileSDO.HisPatientTypeAlter.IS_TEMP_QN = 1;
                else
                    patientProfileSDO.HisPatientTypeAlter.IS_TEMP_QN = null;

                ////Xử lý luôn luôn fix là đúng tuyến với các trường hợp cơ sở kcbbd là đúng tuyến/thông tuyến/tuyến dưới
                //this.ProcessCaseWrongRoute(patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE, patientProfileSDO.HisPatientTypeAlter.LIVE_AREA_CODE);

                if (!this.rdoRightRoute.Checked && !this.rdoWrongRoute.Checked)
                {
                    this.rdoRightRoute.Checked = true;
                    this.rdoWrongRoute.Checked = !this.rdoRightRoute.Checked;
                    Inventec.Common.Logging.LogSystem.Info("Tiep don BN bhyt, khong xac dinh duoc dung tuyen - trai tuyen, mac dinh lay dung tuyen. Du lieu dau vao: HEIN_CARD_NUMBER = " + txtSoThe.Text + " | HEIN_MEDI_ORG_CODE = " + (cboDKKCBBD.EditValue ?? "").ToString());
                }

                if (this.rdoRightRoute.Checked)
                    patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;
                else
                    patientProfileSDO.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;

                this.dtHeinCardFromTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardFromTime.Text);
                if (this.dtHeinCardFromTime.EditValue != null)
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtHeinCardFromTime.DateTime.ToString("yyyyMMdd") + "000000");
                else
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = null;

                this.dtHeinCardToTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtHeinCardToTime.Text);
                if (dtHeinCardToTime.EditValue != null)
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtHeinCardToTime.DateTime.ToString("yyyyMMdd") + "000000");
                else
                    patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME = null;
                if (dtDu5Nam.EditValue != null || !string.IsNullOrEmpty(this.txtDu5Nam.Text))
                {
                    DateTime? dt = His.UC.UCHein.Utils.HeinUtils.ConvertDateStringToSystemDate(this.txtDu5Nam.Text);
                    if (dtDu5Nam.DateTime != dt)
                    {
                        dtDu5Nam.EditValue = dt;
                    }
                    patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtDu5Nam.DateTime.ToString("yyyyMMdd") + "000000");
                }
                else
                    patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME = null;

                string inputDate = this.txtFreeCoPainTime.Text.Trim();
                if (inputDate.Length == 8)
                    inputDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);

                this.dtFreeCoPainTime.EditValue = Utils.HeinUtils.ConvertDateStringToSystemDate(inputDate);
                if (!String.IsNullOrEmpty(inputDate) && this.dtFreeCoPainTime.EditValue != null)
                {
                    this.txtFreeCoPainTime.Text = inputDate;
                    patientProfileSDO.HisPatientTypeAlter.FREE_CO_PAID_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(this.dtFreeCoPainTime.DateTime.ToString("yyyyMMdd"));
                }
                else
                    patientProfileSDO.HisPatientTypeAlter.FREE_CO_PAID_TIME = null;

                patientProfileSDO.HisPatientTypeAlter.ADDRESS = this.txtAddress.Text.Trim();
                patientProfileSDO.HisPatientTypeAlter.HNCODE = this.txtHNCode.Text.Trim();

                if (this.lciKhongKTHSD.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && this.checkKhongKTHSD.Checked)
                    patientProfileSDO.HisPatientTypeAlter.IS_NO_CHECK_EXPIRE = (short)1;
                else
                    patientProfileSDO.HisPatientTypeAlter.IS_NO_CHECK_EXPIRE = null;
                patientProfileSDO.HisPatientTypeAlter.IS_NEWBORN = chkBaby.Checked ? (short?)1 : null;
                patientProfileSDO.HisPatientTypeAlter.HAS_WORKING_LETTER = chkHasWorkingLetter.Checked ? (short?)1 : null;
                patientProfileSDO.HisPatientTypeAlter.HAS_ABSENT_LETTER = chkHasAbsentLetter.Checked ? (short?)1 : null;
                patientProfileSDO.HisPatientTypeAlter.IS_TT46 = chkTt46.Checked ? (short?)1 : null;
                patientProfileSDO.HisPatientTypeAlter.TT46_NOTE = txtTt46.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
