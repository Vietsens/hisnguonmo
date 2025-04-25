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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
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
        ///                 Sửa chức năng "Tiếp đón" (tiếp đón 1 và tiếp đón 2):
        ///Khi nhập thông tin chuyển tuyến, căn cứ vào tuyến của viện mà người dùng đang làm việc (LEVEL_CODE của HIS_BRANCH mà người dùng chọn làm việc) với tuyến của viện mà người dùng nhập "Nơi chuyển đến" để tự động điền "Hình thức chuyển" (LEVEL_CODE của HIS_MEDI_ORG), theo công thức sau:

        ///                - Nếu L2 - L1 = 1 --> chọn "Hình thức chuyển" mã "01" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE)
        ///                - Nếu L2 - L1 > 1 --> chọn "Hình thức chuyển" mã "02" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE)
        ///                - Nếu L2 - L1 < 0 --> chọn "Hình thức chuyển" mã "03" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG)
        ///                - Nếu L2 - L1 = 0 --> chọn "Hình thức chuyển" mã "04" (ID = IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN)

        ///                Trong đó:
        ///                - LEVEL_CODE của "Tuyến của viện mà người dùng đang làm việc" là L1
        ///                - LEVEL_CODE của "Nơi chuyển đến" là L2

        ///                Lưu ý:
        ///                Hệ thống cũ, dữ liệu LEVEL_CODE của HIS_MEDI_ORG đang lưu dưới dạng text (TW, T, H, X), để tránh việc update cache có thể ảnh hưởng đến hiệu năng, lúc xử lý cần "if-else" để xử lý được với dữ liệu cũ, cụ thể cần check LEVEL_CODE của HIS_MEDI_ORG, gán lại giá trị trước khi tính toán:
        ///                - Nếu LEVEL_CODE = TW --> LEVEL_CODE = 1
        ///                - Nếu LEVEL_CODE = T --> LEVEL_CODE = 2
        ///                - Nếu LEVEL_CODE = H --> LEVEL_CODE = 3
        ///                - Nếu LEVEL_CODE = X --> LEVEL_CODE = 4
        ///                - Khác: --> giữ nguyên giá trị
        /// </summary>
        private void ProcessLevelOfMediOrg()
        {
            try
            {
                string lvBranch = FixWrongLevelCode(BranchDataWorker.Branch.HEIN_LEVEL_CODE);

                if (!String.IsNullOrEmpty(txtMaNoiChuyenDen.Text) && cboNoiChuyenDen.EditValue != null)
                {
                    var mediTrans = DataStore.MediOrgs.Where(o => o.MEDI_ORG_CODE == txtMaNoiChuyenDen.Text).FirstOrDefault();
                    if (mediTrans != null)
                    {
                        string lvTrans = FixWrongLevelCode(mediTrans.LEVEL_CODE);

                        int iLvBranch = int.Parse(lvBranch);
                        int iLvTrans = int.Parse(lvTrans);
                        int iKq = iLvTrans - iLvBranch;
                        MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM tranPatiDefault = null;
                        if (iKq == 1)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq > 1)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__DUOI_LEN_KHONG_LIEN_KE).FirstOrDefault();
                        }
                        else if (iKq < 0)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__TREN_XUONG).FirstOrDefault();
                        }
                        else if (iKq == 0)
                        {
                            tranPatiDefault = DataStore.TranPatiForms.Where(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TRAN_PATI_FORM.ID__CUNG_TUYEN).FirstOrDefault();
                        }

                        cboHinhThucChuyen.EditValue = tranPatiDefault != null ? (long?)tranPatiDefault.ID : null;
                        txtMaHinhThucChuyen.Text = tranPatiDefault != null ? tranPatiDefault.TRAN_PATI_FORM_CODE : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FixWrongLevelCode(string code)
        {
            string rs = "";
            try
            {
                if (code == "TW")
                {
                    rs = "1";
                }
                else if (code == "T")
                {
                    rs = "2";
                }
                else if (code == "H")
                {
                    rs = "3";
                }
                else if (code == "X")
                {
                    rs = "4";
                }
                else
                    rs = code;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        /// <summary>
        /// Hàm xử lý khi thay đổi cơ sở kcbbd, kết quả xử lý hoặc fill dữ liệu vào form hoặc chỉ xử lý việc thay đổi vùng dữ liệu liên quan đến cơ sở kcbbd của thẻ bhyt
        /// </summary>
        private void ProcessChangeMediOrgCombo()
        {
            try
            {
                //Lấy dữ liệu Bn cũ chính xác theo số thẻ bhyt
                var listResult = HisPatientGet.GetSDO(this.txtSoThe.Text);
                listResult = (listResult != null ? listResult.Where(o => (o.HeinMediOrgCode == this.txtMaDKKCBBD.Text)).ToList() : null);

                //Kiểm tra nếu bệnh nhân mới || (bệnh nhân cũ có thông tin thẻ bhyt & ô số thẻ bhyt nhập khác với số thẻ gắn với BN) => thực hiện fill dữ liệu BN vào form
                if ((listResult != null && listResult.Count > 0) &&
                    ((this.currentPatientSdo == null || this.currentPatientSdo.ID == 0)
                    || (this.currentPatientSdo != null
                    && this.currentPatientSdo.ID > 0
                    && this.currentPatientSdo.HeinCardNumber != Utils.HeinUtils.TrimHeinCardNumber(this.txtSoThe.Text.Replace(" ", "").ToUpper().Trim()))))
                {
                    //Trường hợp có bệnh nhân cũ theo điều kiện lọc => fill dữ liệu bệnh nhân
                    if (listResult.Count > 1)
                    {
                        frmPatientChoice frm = new frmPatientChoice(listResult, this.FillDataAfterSelectOnePatient, DataStore.Genders);
                        frm.ShowDialog();
                    }
                    else
                    {
                        //Fill dữ liệu bệnh nhân (gọi sang module tiếp đón) & dữ liệu đối tượng điều trị vào form
                        this.FillDataAfterSelectOnePatient(listResult[0]);
                    }
                }
                else
                {
                    //Trường hợp không thấy Bn cũ => chỉ load lại vùng từ nơi đkkcbbd trở đi, không load lại dữ liệu Bn cũ đã fill từ trước đó
                    this.MediOrgSelectRowChange(true, (cboNoiSong.EditValue ?? "").ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ẩn ô nhập text bệnh & hiển thị combo chọn bệnh chính hoặc ngược lại
        /// </summary>
        /// <param name="hasDialogText"></param>
        private void VisibleIcdControl(bool hasDialogText)
        {
            try
            {
                this.txtDialogText.Text = this.cboChanDoanTD.Text;
                this.txtDialogText.Visible = hasDialogText;
                this.cboChanDoanTD.Visible = !hasDialogText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Fill dữ liệu thẻ bhyt & thông tin chuyển tuyến vào form
        /// </summary>
        /// <param name="patientTypeAlterBHYT"></param>
        private void HeinCardSelectRowHandler(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            try
            {
                if (patientTypeAlter != null)
                {
                    this.ChangeDataHeinInsuranceInfoByPatientTypeAlter(patientTypeAlter);
                    this.ProcessFillDataTranPatiInForm(patientTypeAlter.TREATMENT_ID);
                    this.ResetTranspatiInfoWithMediOrg(patientTypeAlter != null ? patientTypeAlter.HEIN_MEDI_ORG_CODE : "");
                }
                else
                {
                    this.txtHeinCardFromTime.Focus();
                    this.txtHeinCardFromTime.SelectAll();
                }
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
        private void MediOrgSelectRowChange(bool isFocus, string liveArea = "")
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_MEDI_ORG mediorg = DataStore.MediOrgs.SingleOrDefault(o => o.MEDI_ORG_CODE == (this.cboDKKCBBD.EditValue ?? "").ToString());
                if (mediorg != null)
                {
                    this.txtMaDKKCBBD.Text = mediorg.MEDI_ORG_CODE;
                    if (this.IsDungTuyenCapCuuByTime)
                    {
                        this.rdoRightRoute.Checked = true;
                        this.cboHeinRightRoute.EditValue = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                        this.txtHeinRightRouteCode.Text = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                        this.cboHeinRightRoute.Properties.Buttons[1].Visible = true;

                        chkJoin5Year.Focus();
                    }
                    else
                    {
                        bool hasTuDongChonLoaiThongTuyen = false;
                        if (Config.HisConfigCFG.IsAllowedRouteTypeByDefault == "1" && mediorg != null)
                        {
                            TuDongChonLoaiThongTuyen(mediorg, null);
                            if (this.cboHeinRightRoute.EditValue != null && !String.IsNullOrEmpty(this.txtHeinRightRouteCode.Text))
                                hasTuDongChonLoaiThongTuyen = true;
                        }
                        else
                        {
                            this.InitDefaultRightRouteTypeAppointment(mediorg.MEDI_ORG_CODE);
                        }
                        //
                        if (currentPatientSdo == null || (currentPatientSdo != null && string.IsNullOrEmpty(currentPatientSdo.AppointmentCode)))
                            this.InitDefaultValidRightRouteType(isFocus, mediorg.MEDI_ORG_CODE, liveArea);
                        if (this.currentPatientSdo != null
                            && !String.IsNullOrEmpty(this.currentPatientSdo.AppointmentCode)
                            && !this.MediOrgCodeCurrent.Equals(mediorg.MEDI_ORG_CODE))
                        {
                            this.SetEnableControlHein(RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT, isFocus);
                        }
                        else if (this.MediOrgCodeCurrent == mediorg.MEDI_ORG_CODE
                            || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent
                            || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == this.HeinLevelCodeCurrent)
                        {
                            this.SetEnableControlHein(RightRouterFactory.RIGHT_ROUTER, isFocus);
                        }
                        else
                        //The sai tuyen mac dinh control truong hop -> disable
                        //neu chon dung tuyen thi -> enable control truong hop, bat buoc phai nhap truong hop
                        //neu chon truong hop la cap cuu -> focus vao noi song
                        //neu chon truong hop la c giam -> enable toan bo vung con lai, focus vao noi chuyen den
                        {
                            if (this.IsMediOrgRightRouteByCurrent(mediorg.MEDI_ORG_CODE))
                            {
                                this.SetEnableControlHein((this.IsDefaultRightRouteType ? RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT : RightRouterFactory.RIGHT_ROUTER), isFocus);
                            }
                            else
                            {
                                this.cboNoiSong.Focus();
                                this.cboNoiSong.SelectAll();
                            }
                        }

                        if (hasTuDongChonLoaiThongTuyen == false)
                        {
                            this.ReloadDataCboRightRoute(mediorg.MEDI_ORG_CODE, liveArea);
                        }
                        this.ResetTranspatiInfoWithMediOrg(mediorg.MEDI_ORG_CODE);
                        this.ChangeDefaultHeinRatio();
                        if (entity.IsAutoSelectEmergency)
                        {
                            this.AutoSelectEmergency(entity);
                        }
                    }
                }

                ValidateRightRouteType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataCboRightRoute(string mediOrgCode, string liveArea = "")
        {
            try
            {
                if (entity.IsAutoSelectEmergency)
                {
                    this.AutoSelectEmergency(entity);
                }
                else
                {
                    List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> datas = new List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData>();
                    datas.AddRange(DataStore.HeinRightRouteTypes);
                    if (this.MediOrgCodeCurrent == mediOrgCode
                        || (MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL != this.HeinLevelCodeCurrent &&
                        MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE != this.HeinLevelCodeCurrent))
                    {
                        datas = datas.Where(p => p.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.OVER).ToList();
                    }

                    if (!string.IsNullOrEmpty(mediOrgCode) && (this.MediOrgCodeCurrent == mediOrgCode
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == this.HeinLevelCodeCurrent
                    || this.IsMediOrgRightRouteByCurrent(mediOrgCode)
                    || ((IsNotRequiredRightTypeInCaseOfHavingAreaCode && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                    )
                        )
                    {
                        InitComboCommon(this.cboHeinRightRoute, datas, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                    }
                    else
                    {
                        datas = datas.Where(p => p.HeinRightRouteTypeCode != MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE).ToList();
                        InitComboCommon(this.cboHeinRightRoute, datas, "HeinRightRouteTypeCode", "HeinRightRouteTypeName", "HeinRightRouteTypeCode");
                        if ((cboHeinRightRoute.EditValue ?? "").ToString() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            cboHeinRightRoute.EditValue = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetTranspatiInfoWithMediOrg(string mediOrgCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(mediOrgCode) && this.MediOrgCodeCurrent.Equals(mediOrgCode))
                {
                    this.txtHeinRightRouteCode.EditValue = null;
                    ResetEditorControl.Reset(this.cboHeinRightRoute);
                    ResetEditorControl.Reset(this.cboHinhThucChuyen);
                    ResetEditorControl.Reset(this.cboLyDoChuyen);
                    this.chkMediRecordNoRouteTransfer.Checked = false;
                    this.chkMediRecordRouteTransfer.Checked = false;
                    this.txtMaHinhThucChuyen.Text = "";
                    this.txtMaLyDoChuyen.Text = "";
                    this.txtMaNoiChuyenDen.Text = "";
                    ResetEditorControl.Reset(this.cboNoiChuyenDen);
                    this.txtMaChanDoanTD.Text = "";
                    this.txtMaChanDoanTD.ErrorText = "";
                    this.txtDialogText.Text = "";
                    this.chkHasDialogText.Checked = false;
                    ResetEditorControl.Reset(this.cboChanDoanTD);
                    this.txtInCode.Text = "";
                    dtTransferInTimeFrom.EditValue = null;
                    dtTransferInTimeTo.EditValue = null;

                    dxValidationProvider1.SetValidationRule(this.txtInCode, null);
                    dxValidationProvider1.SetValidationRule(this.chkMediRecordRouteTransfer, null);
                    dxValidationProvider1.SetValidationRule(this.chkMediRecordNoRouteTransfer, null);
                    dxValidationProvider1.SetValidationRule(this.dtTransferInTimeFrom, null);
                    dxValidationProvider1.SetValidationRule(this.dtTransferInTimeTo, null);
                    dxValidationProvider1.SetValidationRule(this.txtMaHinhThucChuyen, null);
                    dxValidationProvider1.SetValidationRule(this.cboHinhThucChuyen, null);
                    dxValidationProvider1.SetValidationRule(this.txtMaLyDoChuyen, null);
                    dxValidationProvider1.SetValidationRule(this.cboLyDoChuyen, null);
                }
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
                patientTypeData.HAS_BIRTH_CERTIFICATE = (this.chkHasDobCertificate.Checked ? MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE : MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.FALSE);
                patientTypeData.LIVE_AREA_CODE = this.cboNoiSong.Text;
                patientTypeData.RIGHT_ROUTE_CODE = this.rdoRightRoute.Checked ? MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE : MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
                patientTypeData.JOIN_5_YEAR = this.chkJoin5Year.Checked ? MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE : MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.FALSE;
                patientTypeData.PAID_6_MONTH = this.chkPaid6Month.Checked ? MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE : MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.FALSE;
                patientTypeData.RIGHT_ROUTE_TYPE_CODE = (this.cboHeinRightRoute.EditValue ?? "").ToString();
                patientTypeData.HEIN_MEDI_ORG_CODE = (string)this.cboDKKCBBD.EditValue;
                patientTypeData.HEIN_MEDI_ORG_NAME = this.cboDKKCBBD.Text;
                patientTypeData.LEVEL_CODE = this.HeinLevelCodeCurrent;
                this.txtMucHuong.Text = new His.UC.UCHein.ControlProcess.ServiceRequestProcess().GetDefaultHeinRatio(patientTypeData, heincardNumber, treatmentTypeCode);
                patientTypeData.IS_NEWBORN = chkBaby.Checked ? (short?)1 : null;
                patientTypeData.HAS_ABSENT_LETTER = chkHasAbsentLetter.Checked ? (short?)1 : null;
                patientTypeData.HAS_WORKING_LETTER = chkHasWorkingLetter.Checked ? (short?)1 : null;
                patientTypeData.IS_TT46 = chkTt46.Checked ? (short?)1 : null;
                patientTypeData.TT46_NOTE = txtTt46.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Hàm xử lý việc thay đổi trạng thái các control (Enable, Visible) khi thay đổi đúng tuyến <=> trái tuyến
        /// </summary>
        /// <param name="rightRouterType"></param>
        /// <param name="isFocus"></param>
        private void SetEnableControlHein(RightRouterFactory rightRouterType, bool isFocus)
        {
            try
            {
                switch (rightRouterType)
                {
                    case RightRouterFactory.RIGHT_ROUTER:
                        this.txtHeinRightRouteCode.Enabled = true;
                        this.cboHeinRightRoute.Enabled = true;
                        ResetValueByDTCC(false);
                        if (isFocus)
                        {
                            if (this.txtHeinRightRouteCode.Enabled)
                            {
                                this.txtHeinRightRouteCode.Focus();
                                this.txtHeinRightRouteCode.SelectAll();
                            }
                            else
                            {
                                this.cboNoiSong.Focus();
                            }
                        }
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT_FOR_MEDI_ORG_ROUTE:
                        this.txtHeinRightRouteCode.Enabled = false;
                        this.cboHeinRightRoute.Enabled = false;
                        ResetValueByDTCC(false);
                        if (isFocus)
                        {
                            this.rdoWrongRoute.Focus();
                        }
                        break;
                    case RightRouterFactory.WRONG_ROUTER:
                        this.txtHeinRightRouteCode.Enabled = false;
                        this.cboHeinRightRoute.Enabled = false;
                        ResetValueByDTCC(false);
                        if (isFocus)
                        {
                            if (this.txtHeinRightRouteCode.Enabled)
                            {
                                this.txtHeinRightRouteCode.Focus();
                                this.txtHeinRightRouteCode.SelectAll();
                            }
                            else
                            {
                                this.cboNoiSong.Focus();
                            }
                        }
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT:
                        this.txtHeinRightRouteCode.Enabled = true;
                        this.cboHeinRightRoute.Enabled = true;
                        ResetValueByDTCC(false);
                        if (isFocus)
                        {
                            if (this.txtHeinRightRouteCode.Enabled)
                            {
                                this.txtHeinRightRouteCode.Focus();
                                this.txtHeinRightRouteCode.SelectAll();
                            }
                            else
                            {
                                this.cboNoiSong.Focus();
                            }
                        }
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC:
                        if (isFocus)
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.ShowPopup();
                        }
                        this.txtHeinRightRouteCode.Enabled = true;
                        this.cboHeinRightRoute.Enabled = true;
                        ResetValueByDTCC(false);
                        this.ValidateTranferMediOrg();
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_HASAPPOINTMENT:
                        if (isFocus)
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.ShowPopup();
                        }
                        this.txtHeinRightRouteCode.Enabled = true;
                        this.cboHeinRightRoute.Enabled = true;
                        ResetValueByDTCC(false);
                        this.ValidateTranferMediOrg();
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT:
                        this.txtHeinRightRouteCode.Enabled = true;
                        this.cboHeinRightRoute.Enabled = true;
                        ResetValueByDTCC(true);
                        if (this.cboChanDoanTD.EditValue != null)
                        {
                            this.lblEditIcd.Enabled = true;
                        }
                        else this.lblEditIcd.Enabled = false;
                        if (isFocus)
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.SelectAll();
                        }
                        this.ValidateTranferMediOrg();
                        break;
                    case RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__DELETE_CHOICE_TYPE:
                        if (isFocus)
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.SelectAll();
                        }
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

        private RightRouterFactory GetRouteFactory(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patyAlterBhyt)
        {
            RightRouterFactory result = RightRouterFactory.WRONG_ROUTER;
            try
            {
                switch (patyAlterBhyt.RIGHT_ROUTE_CODE)
                {
                    case MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE:
                        if (!String.IsNullOrEmpty(patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE))
                        {
                            switch (patyAlterBhyt.RIGHT_ROUTE_TYPE_CODE)
                            {
                                case MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY:
                                    result = RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTCC;
                                    break;
                                case MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT:
                                    result = RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT;
                                    break;
                            }
                        }
                        else
                        {
                            result = RightRouterFactory.RIGHT_ROUTER;
                        }
                        break;
                    case MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE:
                        result = RightRouterFactory.WRONG_ROUTER;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
                List<string> codesAccept = this.MediOrgCodesAccepts;
                result = (codesAccept != null && codesAccept.Contains(checkCurrentCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void VisibleButtonDeleteHeinRightRoute()
        {
            try
            {
                cboHeinRightRoute.Properties.Buttons[1].Visible = (cboHeinRightRoute.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu tuyến của bệnh viện là tuyến "Trung ương" và tuyến "Tỉnh" => bắt buộc chọn ô trường hợp
        /// </summary>
        /// <param name="heinMediOrgCode"></param>
        /// <returns>true nếu bắt buộc nhập, false nếu không bắt buộc</returns>
        private bool HasChangeValidRightRouteType(string heinMediOrgCode, string liveArea = "")
        {
            bool hasValid = false;
            try
            {
                //Nếu tuyến của bệnh viện là tuyến "Trung ương" & tuyến "Tỉnh" => bắt buộc chọn ô trường hợp
                //bệnh viện tuyến dưới sẽ không phải chọn trường hợp
                //Nếu tuyến của bệnh viện là tuyến "Trung ương" & tuyến "Tỉnh" => bắt buộc chọn ô trường hợp
                if (((MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.NATIONAL == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT //this.HeinLevelCodeCurrent
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.PROVINCE == HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT// this.HeinLevelCodeCurrent
                    )
                    && !HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT.Equals(heinMediOrgCode)))
                {
                    Inventec.Common.Logging.LogSystem.Debug("HasChangeValidRightRouteType.1");
                    hasValid = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return hasValid;
        }

        /// <summary>
        /// Hàm xử lý việc kiểm tra dữ liệu bhyt (PatientTypeAlter) có phải dữ liệu BN cũ không, nếu đúng thì fill dữ liệu thẻ bhyt của Bn cũ, ngược lại fill dữ liệu theo thông tin truyền vào
        /// </summary>
        /// <param name="patyAlterBhyt"></param>
        internal void ChangeDataHeinInsuranceInfoByPatientTypeAlter(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER patyAlterBhyt)
        {
            try
            {
                this.patientTypeAlterOld = patyAlterBhyt;
                this.chkTempQN.Checked = patyAlterBhyt.IS_TEMP_QN == 1 ? true : false;

                //Trường hợp dữ liệu BN cũ, thông tin đối tượng thanh toán, số thẻ,... => Fill dữ liệu theo dữ liệu của BN
                if (patyAlterBhyt.ID > 0)
                {
                    this.FillDataHeinInsuranceBySelectedPatientTypeAlter(patyAlterBhyt, false);
                    this.HasChangeValidRightRouteType(patyAlterBhyt.HEIN_MEDI_ORG_CODE, patyAlterBhyt.LIVE_AREA_CODE);
                }
                //Trường hợp dữ liệu truyền vào là dữ liệu BN mới chưa đăng ký => Kiểm tra các trường hợp hẹn khám, kiểm tra tuyến bệnh viện,... để hiển thị đúng theo trường hợp đấy
                else
                {
                    if (!String.IsNullOrEmpty(patyAlterBhyt.HEIN_MEDI_ORG_CODE))
                    {
                        this.cboDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                        this.txtMaDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                        this.MediOrgSelectRowChange(false, patyAlterBhyt.LIVE_AREA_CODE);
                        this.FillDataHeinInsuranceBySelectedPatientTypeAlter(patyAlterBhyt, false);
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

                        this.chkHasDobCertificate.Checked = true;
                        this.chkHasDobCertificate.Enabled = false;
                        //this.rdoWrongRoute.Enabled = false;
                        //this.rdoRightRoute.Enabled = false;
                        this.txtHeinRightRouteCode.Enabled = false;
                        this.cboHeinRightRoute.Enabled = false;
                        this.chkMediRecordRouteTransfer.Enabled = false;
                        this.chkMediRecordNoRouteTransfer.Enabled = false;
                        this.cboHinhThucChuyen.Enabled = false;
                        this.txtMaHinhThucChuyen.Enabled = false;
                        dtTransferInTimeTo.Enabled = false;
                        dtTransferInTimeFrom.Enabled = false;
                        this.txtMaLyDoChuyen.Enabled = false;
                        this.cboLyDoChuyen.Enabled = false;

                        this.txtMaNoiChuyenDen.Enabled = false;
                        this.chkHasDialogText.Enabled = false;
                        this.cboNoiChuyenDen.Enabled = false;
                        this.txtDialogText.Enabled = false;
                        this.txtMaChanDoanTD.Enabled = false;
                        this.cboChanDoanTD.Enabled = false;
                        this.lblEditIcd.Enabled = false;
                        this.txtAddress.Enabled = false;
                        this.txtMaDKKCBBD.Enabled = cboDKKCBBD.Enabled = false;

                        this.txtMaDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                        this.cboDKKCBBD.EditValue = patyAlterBhyt.HEIN_MEDI_ORG_CODE;

                        DataStore.MediOrgForHasDobCretidentials = new List<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                        MOS.EFMODEL.DataModels.HIS_MEDI_ORG heinMediOrgData = new MOS.EFMODEL.DataModels.HIS_MEDI_ORG();
                        heinMediOrgData.MEDI_ORG_CODE = patyAlterBhyt.HEIN_MEDI_ORG_CODE;
                        heinMediOrgData.MEDI_ORG_NAME = patyAlterBhyt.HEIN_MEDI_ORG_NAME;
                        DataStore.MediOrgForHasDobCretidentials.Add(heinMediOrgData);
                        MediOrgProcess.LoadDataToComboNoiDKKCBBD(this.cboDKKCBBD, DataStore.MediOrgForHasDobCretidentials);

                        if (this.cboNoiSong.Enabled)
                        {
                            this.cboNoiSong.Focus();
                            this.cboNoiSong.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private short? GetInCMKT()
        {
            short? result = null;
            try
            {
                if (this.chkMediRecordRouteTransfer.Checked)
                    result = 1;
                else if (this.chkMediRecordNoRouteTransfer.Checked)
                    result = 0;
                else
                    result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessCaseWrongRoute(string mediOrgCode, string liveArea = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(mediOrgCode) && (this.MediOrgCodeCurrent == mediOrgCode
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.DISTRICT == this.HeinLevelCodeCurrent
                    || MOS.LibraryHein.Bhyt.HeinLevel.HeinLevelCode.COMMUNE == this.HeinLevelCodeCurrent
                    || ((IsNotRequiredRightTypeInCaseOfHavingAreaCode && (liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K1 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K2 || liveArea == MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaCode.K3)))
                    || this.IsMediOrgRightRouteByCurrent(mediOrgCode)))
                {
                    this.rdoRightRoute.Checked = true;
                    this.SetEnableControlHein((this.IsDefaultRightRouteType ? RightRouterFactory.WRONG_ROUTER__CHOICE_RIGHT__CHOICE_TYPE_DTGT : RightRouterFactory.RIGHT_ROUTER), false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValueAddress(string heinAddress)
        {
            try
            {
                this.txtAddress.Text = heinAddress ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ResetValue()
        {
            try
            {
                this.chkTt46.Checked = false;
                this.txtTt46.Enabled = false;
                this.chkHasAbsentLetter.Checked = false;
                this.chkHasWorkingLetter.Checked = false;
                this.chkBaby.Checked = false;
                this.chkBaby.Enabled = false;
                this.cboSoThe.Visible = false;
                this.chkTempQN.Checked = false;
                this.cboSoThe.Properties.DataSource = null;
                this.chkJoin5Year.Checked = false;
                this.chkHasDobCertificate.Checked = false;
                this.chkHasDialogText.Checked = false;
                this.VisibleIcdControl(this.chkHasDialogText.Checked);
                this.dtFreeCoPainTime.Visible = false;
                ResetEditorControl.Reset(this.dtFreeCoPainTime);
                ResetEditorControl.Reset(this.txtHeinCardToTime);
                ResetEditorControl.Reset(this.txtSoThe);
                ResetEditorControl.Reset(this.cboSoThe);
                ResetEditorControl.Reset(this.dtHeinCardFromTime);
                ResetEditorControl.Reset(this.dtHeinCardToTime);
                ResetEditorControl.Reset(this.txtHeinCardFromTime);
                ResetEditorControl.Reset(this.txtHeinCardToTime);
                ResetEditorControl.Reset(this.txtMaDKKCBBD);
                ResetEditorControl.Reset(this.cboDKKCBBD);
                ResetEditorControl.Reset(this.txtHeinRightRouteCode);
                ResetEditorControl.Reset(this.cboHeinRightRoute);
                ResetEditorControl.Reset(this.txtMaChanDoanTD);
                ResetEditorControl.Reset(this.cboChanDoanTD);
                ResetEditorControl.Reset(this.txtMaNoiChuyenDen);
                ResetEditorControl.Reset(this.cboNoiChuyenDen);
                ResetEditorControl.Reset(this.dtTransferInTimeFrom);
                ResetEditorControl.Reset(this.dtTransferInTimeTo);
                ResetEditorControl.Reset(this.cboNoiSong);
                ResetEditorControl.Reset(this.txtMaHinhThucChuyen);
                ResetEditorControl.Reset(this.cboHinhThucChuyen);
                ResetEditorControl.Reset(this.txtMaLyDoChuyen);
                ResetEditorControl.Reset(this.cboLyDoChuyen);
                ResetEditorControl.Reset(this.txtAddress);
                ResetEditorControl.Reset(this.txtMucHuong);

                this.txtMaChanDoanTD.ErrorText = "";
                this.txtHeinRightRouteCode.Enabled = false;
                this.chkHasDobCertificate.Enabled = false;
                this.cboHeinRightRoute.Enabled = false;
                this.txtHeinCardToTime.Enabled = true;
                this.txtMaNoiChuyenDen.Enabled = true;
                this.chkHasDialogText.Enabled = true;
                this.cboNoiChuyenDen.Enabled = true;
                this.txtDialogText.Enabled = true;
                this.txtMaChanDoanTD.Enabled = true;
                this.cboChanDoanTD.Enabled = true;
                this.chkJoin5Year.Enabled = true;
                this.chkMediRecordRouteTransfer.Enabled = true;
                this.chkMediRecordNoRouteTransfer.Enabled = true;
                this.cboHinhThucChuyen.Enabled = true;
                this.txtMaHinhThucChuyen.Enabled = true;
                dtTransferInTimeFrom.Enabled = true;
                dtTransferInTimeTo.Enabled = true;
                this.txtMaLyDoChuyen.Enabled = true;
                this.cboLyDoChuyen.Enabled = true;

                this.ResetValidationControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetValueByDTCC(bool enable)
        {
            try
            {
                this.chkMediRecordRouteTransfer.Enabled = enable;
                this.chkMediRecordNoRouteTransfer.Enabled = enable;
                this.cboHinhThucChuyen.Enabled = enable;
                this.txtMaHinhThucChuyen.Enabled = enable;
                this.dtTransferInTimeTo.Enabled = enable;
                this.dtTransferInTimeFrom.Enabled = enable;
                this.txtMaLyDoChuyen.Enabled = enable;
                this.cboLyDoChuyen.Enabled = enable;
                this.txtMaNoiChuyenDen.Enabled = enable;
                this.chkHasDialogText.Enabled = enable;
                this.cboNoiChuyenDen.Enabled = enable;
                this.txtDialogText.Enabled = enable;
                this.txtMaChanDoanTD.Enabled = enable;
                this.cboChanDoanTD.Enabled = enable;
                this.txtInCode.Enabled = enable;
                this.lblEditIcd.Enabled = enable;

                if (!enable)
                {
                    this.chkMediRecordRouteTransfer.Checked = enable;
                    this.chkMediRecordNoRouteTransfer.Checked = enable;
                    this.cboHinhThucChuyen.EditValue = null;
                    this.txtMaHinhThucChuyen.EditValue = null;
                    this.dtTransferInTimeTo.EditValue = null;
                    this.dtTransferInTimeFrom.EditValue = null;
                    this.txtMaLyDoChuyen.EditValue = null;
                    this.cboLyDoChuyen.EditValue = null;
                    this.txtMaNoiChuyenDen.EditValue = null;
                    this.cboNoiChuyenDen.EditValue = null;
                    this.txtDialogText.EditValue = null;
                    this.txtMaChanDoanTD.EditValue = null;
                    this.cboChanDoanTD.EditValue = null;
                    this.txtInCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void CoPhaiUCDuocGoiTuModuleTiepDonHayKhong(bool isByRegistor)
        {
            try
            {
                this.isCallByRegistor = isByRegistor;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Template_HeinBHYT1_Process/CoPhaiUCDuocGoiTuModuleTiepDonHayKhong:\n" + ex);
            }
        }

        internal void SetRsDataADO(ResultDataADO resultDataADO)
        {

            try
            {
                this.ResultDataADO = resultDataADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
