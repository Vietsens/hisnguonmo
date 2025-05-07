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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using HIS.Desktop.Utility;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.Icd.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.TreatmentFinish.TreatmentFinish;
using MOS.SDO;
using Inventec.Common.QrCodeBHYT;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        internal void SetPrintMenu(long endTypeId, long? endTypeExtId, long? treatmentTypeId)
        {
            try
            {
                DevExpress.Utils.Menu.DXPopupMenu menu_print = new DevExpress.Utils.Menu.DXPopupMenu();

                if ((endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__KHAC
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN
                    || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON)
                    && (treatmentTypeId ?? 0) != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayRaVienItem = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_RA_VIEN"));
                    inGiayRaVienItem.Tag = ModuleTypePrint.IN_GIAY_RA_VIEN;
                    inGiayRaVienItem.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayRaVienItem);
                }
                else if (endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON)
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayRaVienItem = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_RA_VIEN"));
                    inGiayRaVienItem.Tag = ModuleTypePrint.IN_GIAY_RA_VIEN;
                    inGiayRaVienItem.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayRaVienItem);
                }

                if (endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)//Hen kham
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayHenKhamLaiItem = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_HEN_KHAM_LAI"));
                    inGiayHenKhamLaiItem.Tag = ModuleTypePrint.HEN_KHAM_LAI;
                    inGiayHenKhamLaiItem.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayHenKhamLaiItem);
                }
                else if (endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayChuyenVienItem = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_CHUYEN_VIEN"));
                    inGiayChuyenVienItem.Tag = ModuleTypePrint.IN_GIAY_CHUYEN_VIEN;
                    inGiayChuyenVienItem.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayChuyenVienItem);

                    DevExpress.Utils.Menu.DXMenuItem PhieuBanGiaoNguoiBenhCV = new DevExpress.Utils.Menu.DXMenuItem(
                   GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_PHIEU_BAN_GIAO_NGUOI_BENH_CV"));
                    PhieuBanGiaoNguoiBenhCV.Tag = FormTreatmentFinish.ModuleTypePrint.Mps000382;
                    PhieuBanGiaoNguoiBenhCV.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(PhieuBanGiaoNguoiBenhCV);
                }
                else if (endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayBaoTu = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_BAO_TU"));
                    inGiayBaoTu.Tag = ModuleTypePrint.GIAY_BAO_TU;
                    inGiayBaoTu.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayBaoTu);

                    DevExpress.Utils.Menu.DXMenuItem inMps476 = new DevExpress.Utils.Menu.DXMenuItem(
                                            "Phiếu chẩn đoán nguyên nhân tử vong (Form động)");
                    inMps476.Tag = ModuleTypePrint.Mps000476;
                    inMps476.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inMps476);

                    DevExpress.Utils.Menu.DXMenuItem inMps485 = new DevExpress.Utils.Menu.DXMenuItem(
                                           "Phiếu chẩn đoán nguyên nhân tử vong");
                    inMps485.Tag = ModuleTypePrint.Mps000485;
                    inMps485.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inMps485);
                }

                if (hisTreatmentResult != null && hisTreatmentResult.APPOINTMENT_TIME.HasValue
                    && (endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || endTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN))
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayHenKhamLaiItem = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_HEN_KHAM_LAI"));
                    inGiayHenKhamLaiItem.Tag = ModuleTypePrint.HEN_KHAM_LAI;
                    inGiayHenKhamLaiItem.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayHenKhamLaiItem);
                }

                if (endTypeExtId != null && (endTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI || endTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM))
                {
                    DevExpress.Utils.Menu.DXMenuItem inGiayNghiOm = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_NGHI_OM"));
                    inGiayNghiOm.Tag = FormTreatmentFinish.ModuleTypePrint.GIAY_NGHI_OM;
                    inGiayNghiOm.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(inGiayNghiOm);
                }

                DevExpress.Utils.Menu.DXMenuItem bangKeThanhToan = new DevExpress.Utils.Menu.DXMenuItem(
                    GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_BANG_KE_THANH_TOAN"));
                bangKeThanhToan.Tag = ModuleTypePrint.BANG_KE_THANH_TOAN;
                bangKeThanhToan.Click += PrintCloseTreatment_Click;
                menu_print.Items.Add(bangKeThanhToan);

                DevExpress.Utils.Menu.DXMenuItem bieuMauKhac = new DevExpress.Utils.Menu.DXMenuItem(
                    GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_BIEU_MAU_KHAC"));
                bieuMauKhac.Tag = ModuleTypePrint.BIEU_MAU_KHAC;
                bieuMauKhac.Click += PrintCloseTreatment_Click;
                menu_print.Items.Add(bieuMauKhac);

                DevExpress.Utils.Menu.DXMenuItem bieuGiayPTTT = new DevExpress.Utils.Menu.DXMenuItem(
                    GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_CHUNG_NHAN_PTTT"));
                bieuGiayPTTT.Tag = FormTreatmentFinish.ModuleTypePrint._IN_GIAY_CHUNG_NHAN_PTTT;
                bieuGiayPTTT.Click += PrintCloseTreatment_Click;
                menu_print.Items.Add(bieuGiayPTTT);

                DevExpress.Utils.Menu.DXMenuItem bieuGiayTT = new DevExpress.Utils.Menu.DXMenuItem(
                    GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_CHUNG_NHAN_TT"));
                bieuGiayTT.Tag = FormTreatmentFinish.ModuleTypePrint._IN_GIAY_CHUNG_NHAN_TT;
                bieuGiayTT.Click += PrintCloseTreatment_Click;
                menu_print.Items.Add(bieuGiayTT);

                DevExpress.Utils.Menu.DXMenuItem tomTatBenhAn = new DevExpress.Utils.Menu.DXMenuItem(
                    GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_TOM_TAT_BENH_AN"));
                tomTatBenhAn.Tag = FormTreatmentFinish.ModuleTypePrint.TOM_TAT_BENH_AN;
                tomTatBenhAn.Click += PrintCloseTreatment_Click;
                menu_print.Items.Add(tomTatBenhAn);

                if (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    DevExpress.Utils.Menu.DXMenuItem mps00399Item = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_GIAY_XAC_NHAN_DIEU_TRI_NOI_TRU"));
                    mps00399Item.Tag = FormTreatmentFinish.ModuleTypePrint.Mps000399;
                    mps00399Item.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(mps00399Item);
                }
                if (this.treatmentId > 0)
                {
                    DevExpress.Utils.Menu.DXMenuItem Mps000478Item = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC"));
                    Mps000478Item.Tag = FormTreatmentFinish.ModuleTypePrint.Mps000478_TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC;
                    Mps000478Item.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(Mps000478Item);

                    DevExpress.Utils.Menu.DXMenuItem Mps000222Item = new DevExpress.Utils.Menu.DXMenuItem(
                        GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__PRINT_TOM_TAT_KET_QUA_CLS"));
                    Mps000222Item.Tag = FormTreatmentFinish.ModuleTypePrint.Mps000222_TOM_TAT_KET_QUA_CLS;
                    Mps000222Item.Click += PrintCloseTreatment_Click;
                    menu_print.Items.Add(Mps000222Item);
                }

                btnPrint.DropDownControl = menu_print;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void TranPatiDataTreatmentFinish(MOS.SDO.HisTreatmentFinishSDO treatmentFinish)
        {
            try
            {
                //hisTreatmentFinishSDO = treatmentFinish; // Sửa để gán vào hisTreatmentFinishSDO
                hisTreatmentFinishSDO_process = treatmentFinish;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SaveTreatmentFinish(MOS.SDO.HisTreatmentFinishSDO hisTreatmentFinishSDO, ref bool success, ref CommonParam param)
        {
            success = false;
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO), hisTreatmentFinishSDO));
                hisTreatmentResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_TREATMENT>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_FINISH, ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentFinishSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                CallApiSevereIllnessInfo();

                WaitingManager.Hide();
                if (hisTreatmentResult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Ket qua khi goi api HisTreatment/Finish " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentResult), hisTreatmentResult));
                    success = true;
                    btnPrint.Enabled = true;
                    btnSave.Enabled = false;
                    btnSaveTemp.Enabled = false;
                    txtEndOrder.Text = hisTreatmentResult.END_CODE;
                    txtSoChuyenVien.Text = hisTreatmentResult.OUT_CODE;
                    txtMaBHXH.Text = hisTreatmentFinishSDO.SocialInsuranceNumber;
                    txtExtraEndCode.Text = hisTreatmentResult.EXTRA_END_CODE;
                    txtStoreCode.Text = hisTreatmentResult.STORE_CODE;
                    cboDepartmentOut.EditValue = hisTreatmentResult.EXIT_DEPARTMENT_ID;
                    SetReadOnly();

                    if (RefeshReference != null)
                    {
                        this.RefeshReference();
                    }
                }

                #region Show message

                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallApiSevereIllnessInfo()
        {
            try
            {
                if (causeResult != null && (causeResult.SevereIllNessInfo != null || (causeResult.ListEventsCausesDeath != null && causeResult.ListEventsCausesDeath.Count > 0)))
                {
                    CommonParam param = new CommonParam();
                    SevereIllnessInfoSDO sdo = new SevereIllnessInfoSDO();
                    sdo.SevereIllnessInfo = causeResult.SevereIllNessInfo;
                    sdo.SevereIllnessInfo.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.module.RoomId).DepartmentId;
                    sdo.EventsCausesDeaths = causeResult.ListEventsCausesDeath;
                    var dt = new BackendAdapter(param)
                       .Post<HisServiceReqExamUpdateResultSDO>("api/HisSevereIllnessInfo/CreateOrUpdate", ApiConsumers.MosConsumer, sdo, param);
                    string message = string.Format("Lưu thông tin tử vong. TREATMENT_CODE: {0}", currentHisTreatment.TREATMENT_CODE);
                    string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    SdaEventLogCreate eventlog = new SdaEventLogCreate();
                    eventlog.Create(login, null, true, message);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        internal void ShowPopupBenhNhanChuongTrinh()
        {
            try
            {
                if (currentHisTreatment != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(currentHisTreatment.PATIENT_ID);

                    if (this.module == null)
                    {
                        CallModule.Run(CallModule.HisPatientProgram, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule.Run(CallModule.HisPatientProgram, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
                else
                {
                    throw new ArgumentNullException("Patient is null");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal async Task<bool> ProcessDataBeforeSaveAsync(FormTreatmentFinish Form, bool isSave)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisTreatmentFinishSDO_process), hisTreatmentFinishSDO_process));
                Save.ISave iSave = Save.SaveFactory.MakeISave(this.WorkPlaceSDO.RoomId, null, isSave, currentHisTreatment, hisTreatmentFinishSDO_process, Form);
                var sdo = iSave.Run();
                if (sdo == null)
                {
                    return true;//
                }
                hisTreatmentFinishSDO = sdo as MOS.SDO.HisTreatmentFinishSDO;
                if (hisTreatmentFinishSDO.TreatmentId <= 0)
                {
                    result = true;
                    if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        XtraMessageBox.Show(ResourceMessage.ChuaNhapThongTinChuyenVien);
                        this.FormTransfer = new CloseTreatment.FormTransfer(this.module, currentHisTreatment);
                        this.FormTransfer.MyGetData = new CloseTreatment.FormTransfer.GetString(this.TranPatiDataTreatmentFinish);
                        this.FormTransfer.Form = this;
                        this.FormTransfer.ShowDialog();
                    }
                    else if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                    {
                        if (Form != null)
                        {
                            if (!hisTreatmentFinishSDO_process.AppointmentTime.HasValue || hisTreatmentFinishSDO_process.AppointmentTime.Value <= 0)
                            {
                                var dataRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId);

                                XtraMessageBox.Show(ResourceMessage.ChuaNhapThoiGianHenKham);
                                long dtTreatmentEnd = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                                this.FormAppointment = new CloseTreatment.FormAppointment(this.module, dtTreatmentEnd, dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false);
                                this.FormAppointment.MyGetData = new CloseTreatment.FormAppointment.GetString(this.TranPatiDataTreatmentFinish);
                                this.FormAppointment.Form = this;
                                this.FormAppointment.ShowDialog();
                            }
                        }
                    }
                    else if (hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                    {
                        XtraMessageBox.Show(ResourceMessage.ChuaNhapThongTinTuVong);
                        this.FormDeath = new CloseTreatment.FormDeath(currentHisTreatment, this.module, TranPatiDataTreatmentFinish);

                        this.FormDeath.Form = this;
                        this.FormDeath.ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(this, null, false);
                        Inventec.Common.Logging.LogSystem.Error("TreatmentFinish error other!");
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("hisTreatmentFinishSDO", hisTreatmentFinishSDO));
                    }
                }

                if (hisTreatmentFinishSDO.TreatmentEndTypeExtId.HasValue
                    && ConfigKey.ExportXml2076Option == ConfigKey.IS__TRUE
                    && (hisTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI || hisTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                    )
                {
                    List<string> names = new List<string>();
                    if (!hisTreatmentFinishSDO.SickLeaveFrom.HasValue)
                    {
                        names.Add(ResourceMessage.ThoiGianNghiTu);
                    }
                    if (!hisTreatmentFinishSDO.SickLeaveTo.HasValue)
                    {
                        names.Add(ResourceMessage.ThoiGianNghiDen);
                    }
                    if (String.IsNullOrWhiteSpace(hisTreatmentFinishSDO.SickLoginname))
                    {
                        names.Add(ResourceMessage.NguoiCap);
                    }
                    if (String.IsNullOrWhiteSpace(hisTreatmentFinishSDO.PatientWorkPlace) && !hisTreatmentFinishSDO.WorkPlaceId.HasValue)
                    {
                        names.Add(ResourceMessage.NoiLamViecCuaBenhNhan);
                    }

                    if (names.Count > 0)
                    {
                        XtraMessageBox.Show(String.Format(ResourceMessage.BanCanNhapCacThongTinSauDePhucVuXuatXmlHoSoChungTu, String.Join(", ", names)), ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        return true;
                    }
                }
                if (!string.IsNullOrEmpty(txtMaBHXH.Text))
                {
                    hisTreatmentFinishSDO.SocialInsuranceNumber = txtMaBHXH.Text;
                }

                if (!string.IsNullOrEmpty(txtKskCode.Text.Trim()))
                    hisTreatmentFinishSDO.HrmKskCode = txtKskCode.Text.Trim();

                if (string.IsNullOrEmpty(hisTreatmentFinishSDO.IcdCode))
                {
                    result = true;
                    MessageManager.Show(this, null, false);
                    Inventec.Common.Logging.LogSystem.Error("TreatmentFinish error ICD!");
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("hisTreatmentFinishSDO", hisTreatmentFinishSDO));
                }

                if (Base.GlobalStore.END_ORDER_STR == "1")
                {
                    hisTreatmentFinishSDO.EndCode = txtEndOrder.Text.Trim();
                    hisTreatmentFinishSDO.EndCodeRequest = true;
                }
                else if (chkSinhLaiSoRaVien.Checked)
                {
                    hisTreatmentFinishSDO.EndCodeRequest = true;
                }

                if (chkSinhLaiSoChuyenVien.Checked)
                    hisTreatmentFinishSDO.OutCodeRequest = true;
                if (chKTaoHoSoMoi.Checked == true)
                {
                    hisTreatmentFinishSDO.IsCreateNewTreatment = true;
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Debug("SET DATA HEIN 1");
                    GetPatientTypeAlter();
                    HeinCardData heinCardDataForCheckGOV = new HeinCardData();
                    heinCardDataForCheckGOV.PatientName = currentHisTreatment.TDL_PATIENT_NAME;
                    heinCardDataForCheckGOV.Address = currentHisTreatment.TDL_PATIENT_ADDRESS;
                    heinCardDataForCheckGOV.Dob = currentHisTreatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4) : ProcessDate(currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(6, 2) + currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(4, 2) + currentHisTreatment.TDL_PATIENT_DOB.ToString().Substring(0, 4));
                    heinCardDataForCheckGOV.Gender = currentHisTreatment.TDL_PATIENT_GENDER_NAME;
                    if (patientTypeAlter != null)
                    {
                        heinCardDataForCheckGOV.HeinCardNumber = patientTypeAlter.HEIN_CARD_NUMBER;
                        heinCardDataForCheckGOV.FromDate = ProcessDate(patientTypeAlter.HEIN_CARD_FROM_TIME.HasValue ? patientTypeAlter.HEIN_CARD_FROM_TIME.ToString().Substring(6, 2) + patientTypeAlter.HEIN_CARD_FROM_TIME.ToString().Substring(4, 2) + patientTypeAlter.HEIN_CARD_FROM_TIME.ToString().Substring(0, 4) : null);
                        heinCardDataForCheckGOV.MediOrgCode = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                    }
                    HeinGOVManager heinGOVManager = new HeinGOVManager(null);
                    this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, null, dtNewTreatmentTime.DateTime, false, false);
                    Inventec.Common.Logging.LogSystem.Debug("SET DATA HEIN 2");
                    ProcessHein(ref hisTreatmentFinishSDO);
                    WaitingManager.Hide();
                }
                //13763
                if (!string.IsNullOrEmpty(txtDaysBedTreatment.Text.Trim()))
                {
                    hisTreatmentFinishSDO.TreatmentDayCount = Inventec.Common.TypeConvert.Parse.ToDecimal(txtDaysBedTreatment.Text.Trim());
                }
                if (!string.IsNullOrEmpty(txtKskCode.Text.Trim()))
                    hisTreatmentFinishSDO.HrmKskCode = txtKskCode.Text.Trim();
                if (this.ucIcdCause != null)
                {
                    var icdValue = this.IcdCauseProcessor.GetValue(this.ucIcdCause, Template.NoFocus);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        hisTreatmentFinishSDO.IcdCauseCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        hisTreatmentFinishSDO.IcdCauseName = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct, Template.NoFocus);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        hisTreatmentFinishSDO.TraditionalIcdCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        hisTreatmentFinishSDO.TraditionalIcdName = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                hisTreatmentFinishSDO.Surgery = txtSurgery.Text;

                if (lciChkCapSoLuuTruBA.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    hisTreatmentFinishSDO.CreateOutPatientMediRecord = chkCapSoLuuTruBA.Checked ? true : false;
                }

                if (lciPatientProgram.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && cboProgram.EditValue != null)
                {
                    hisTreatmentFinishSDO.ProgramId = Inventec.Common.TypeConvert.Parse.ToInt64(cboProgram.EditValue.ToString());
                }
                if (cboProgram.EditValue == null)
                {
                    hisTreatmentFinishSDO.ProgramId = null;
                }

                hisTreatmentFinishSDO.IsExpXml4210Collinear = ChkExpXml4210.Checked;

                if (ucSecondaryIcdYhct != null)     
                {
                    var subIcd = subIcdYhctProcessor.GetValue(ucSecondaryIcdYhct);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        hisTreatmentFinishSDO.TraditionalIcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        hisTreatmentFinishSDO.TraditionalIcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                if (prosCD != null)
                {
                    hisTreatmentFinishSDO.ShowIcdCode = prosCD.SHOW_ICD_CODE;
                    hisTreatmentFinishSDO.ShowIcdName = prosCD.SHOW_ICD_NAME;
                    if (!string.IsNullOrEmpty(prosCD.SHOW_ICD_SUB_CODE))
                    {
                        hisTreatmentFinishSDO.ShowIcdSubCode = prosCD.SHOW_ICD_SUB_CODE;
                        hisTreatmentFinishSDO.ShowIcdText = prosCD.SHOW_ICD_TEXT;
                    }
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prosCD), prosCD));
                }

                if (lcDepartmentOut.Enabled == true)
                {
                    if (cboDepartmentOut.EditValue != null)
                    {
                        hisTreatmentFinishSDO.ExitDepartmentId = (long)cboDepartmentOut.EditValue;
                    }
                }
                else
                {
                    hisTreatmentFinishSDO.ExitDepartmentId = null;
                }
                hisTreatmentFinishSDO.EyesightGlassLeft = txtEyesightGlassLeft.Text;
                hisTreatmentFinishSDO.EyesightGlassRight = txtEyesightGlassRight.Text;
                hisTreatmentFinishSDO.EyesightLeft = txtEyesightLeft.Text;
                hisTreatmentFinishSDO.EyesightRight = txtEyesightRight.Text;
                hisTreatmentFinishSDO.EyeTensionLeft = txtEyeTensionLeft.Text;
                hisTreatmentFinishSDO.EyeTensionRight = txtEyeTensionRight.Text;

                if (lciOutPatientDateFrom.Enabled == true
                && lciOutPatientDateTo.Enabled == true)
                {
                    hisTreatmentFinishSDO.OutPatientDateFrom = dtOutPatientDateFrom.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtOutPatientDateFrom.DateTime) : null;
                    hisTreatmentFinishSDO.OutPatientDateTo = dtOutPatientDateFrom.EditValue != null ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtOutPatientDateTo.DateTime) : null;
                }

                hisTreatmentFinishSDO.NewTreatmentInTime = hisTreatmentFinishSDO_process.NewTreatmentInTime;

                if (!string.IsNullOrEmpty(txtEndDeptSubsHead.Text) && cboEndDeptSubsHead.EditValue != null)
                {
                    var _EndDeptSubsHead = listUser.FirstOrDefault(o => o.LOGINNAME == cboEndDeptSubsHead.EditValue.ToString());
                    hisTreatmentFinishSDO.EndDeptSubsHeadLoginname = _EndDeptSubsHead != null ? _EndDeptSubsHead.LOGINNAME : null;
                    hisTreatmentFinishSDO.EndDeptSubsHeadUsername = _EndDeptSubsHead != null ? _EndDeptSubsHead.USERNAME : null;
                }
                else
                {
                    hisTreatmentFinishSDO.EndDeptSubsHeadLoginname = null;
                    hisTreatmentFinishSDO.EndDeptSubsHeadUsername = null;
                }
                if (!string.IsNullOrEmpty(txtHospSubsDirector.Text) && cboHospSubsDirector.EditValue != null)
                {
                    var _HospSubsDirector = listUser.FirstOrDefault(o => o.LOGINNAME == cboHospSubsDirector.EditValue.ToString());
                    hisTreatmentFinishSDO.HospSubsDirectorLoginname = _HospSubsDirector != null ? _HospSubsDirector.LOGINNAME : null;
                    hisTreatmentFinishSDO.HospSubsDirectorUsername = _HospSubsDirector != null ? _HospSubsDirector.USERNAME : null;
                }
                else
                {
                    hisTreatmentFinishSDO.HospSubsDirectorLoginname = null;
                    hisTreatmentFinishSDO.HospSubsDirectorUsername = null;
                }
                if (cboHosReason.EditValue != null)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_HOSPITALIZE_REASON>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.ID == Int64.Parse(cboHosReason.EditValue.ToString()));
                    if (data != null && data.HOSPITALIZE_REASON_NAME == txtHosReasonNt.Text.Trim())
                    {
                        hisTreatmentFinishSDO.HospitalizeReasonCode = data.HOSPITALIZE_REASON_CODE;
                        hisTreatmentFinishSDO.HospitalizeReasonName = data.HOSPITALIZE_REASON_NAME;
                    }
                    else
                    {
                        hisTreatmentFinishSDO.HospitalizeReasonCode = null;
                        hisTreatmentFinishSDO.HospitalizeReasonName = txtHosReasonNt.Text.Trim();
                    }
                }
                else
                {
                    hisTreatmentFinishSDO.HospitalizeReasonCode = null;
                    hisTreatmentFinishSDO.HospitalizeReasonName = txtHosReasonNt.Text.Trim();
                }
                if (cboCareer.EditValue != null)
                {
                    hisTreatmentFinishSDO.CareerId = (long)cboCareer.EditValue;
                }

                if (currentTreatmentFinishSDO != null)
                {
                    hisTreatmentFinishSDO.SurgeryName = currentTreatmentFinishSDO.SurgeryName;
                    hisTreatmentFinishSDO.SurgeryBeginTime = currentTreatmentFinishSDO.SurgeryBeginTime;
                    hisTreatmentFinishSDO.SurgeryEndTime = currentTreatmentFinishSDO.SurgeryEndTime;
                    hisTreatmentFinishSDO.Valid1Year = currentTreatmentFinishSDO.Valid1Year;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        internal void ProcessDataForTreatmentFinishSDO(MOS.EFMODEL.DataModels.HIS_TREATMENT data)
        {
            try
            {
                currentTreatmentFinishSDO = new MOS.SDO.HisTreatmentFinishSDO();
                currentTreatmentFinishSDO.TreatmentId = data.ID;
                currentTreatmentFinishSDO.Advise = data.ADVISE;
                currentTreatmentFinishSDO.ClinicalNote = this.currentTreatmentExt != null ? this.currentTreatmentExt.CLINICAL_NOTE:"";
                currentTreatmentFinishSDO.DeathCauseId = data.DEATH_CAUSE_ID;
                currentTreatmentFinishSDO.DeathTime = data.DEATH_TIME;
                currentTreatmentFinishSDO.DeathWithinId = data.DEATH_WITHIN_ID;
                currentTreatmentFinishSDO.DoctorLoginname = data.DOCTOR_LOGINNAME;
                currentTreatmentFinishSDO.DoctorUsernname = data.DOCTOR_USERNAME;
                currentTreatmentFinishSDO.IcdCode = data.ICD_CODE;
                currentTreatmentFinishSDO.IcdName = data.ICD_NAME;
                currentTreatmentFinishSDO.IcdSubCode = data.ICD_SUB_CODE;
                currentTreatmentFinishSDO.IcdText = data.ICD_TEXT;
                currentTreatmentFinishSDO.MainCause = data.MAIN_CAUSE;
                currentTreatmentFinishSDO.PatientCondition = data.PATIENT_CONDITION;
                currentTreatmentFinishSDO.SubclinicalResult = this.currentTreatmentExt != null ? this.currentTreatmentExt.SUBCLINICAL_RESULT:"";
                currentTreatmentFinishSDO.Surgery = data.SURGERY;
                currentTreatmentFinishSDO.TranPatiFormId = data.TRAN_PATI_FORM_ID;
                currentTreatmentFinishSDO.TranPatiReasonId = data.TRAN_PATI_REASON_ID;
                currentTreatmentFinishSDO.TransferOutMediOrgCode = data.MEDI_ORG_CODE;
                currentTreatmentFinishSDO.TransferOutMediOrgName = data.MEDI_ORG_NAME;
                currentTreatmentFinishSDO.Transporter = data.TRANSPORTER;
                currentTreatmentFinishSDO.TransporterLoginnames = data.TRANSPORTER_LOGINNAMES;
                currentTreatmentFinishSDO.TransportVehicle = data.TRANSPORT_VEHICLE;
                currentTreatmentFinishSDO.TreatmentFinishTime = data.OUT_TIME ?? 0;
                currentTreatmentFinishSDO.TreatmentMethod = data.TREATMENT_METHOD;
                currentTreatmentFinishSDO.TreatmentResultId = data.TREATMENT_RESULT_ID;
                currentTreatmentFinishSDO.DeathDocumentDate = data.DEATH_DOCUMENT_DATE;
                currentTreatmentFinishSDO.DeathDocumentNumber = data.DEATH_DOCUMENT_NUMBER;
                currentTreatmentFinishSDO.DeathDocumentPlace = data.DEATH_DOCUMENT_PLACE;
                currentTreatmentFinishSDO.DeathDocumentType = data.DEATH_DOCUMENT_TYPE;
                currentTreatmentFinishSDO.DeathPlace = data.DEATH_PLACE;
                currentTreatmentFinishSDO.SickLeaveDay = data.SICK_LEAVE_DAY;
                currentTreatmentFinishSDO.SickLeaveFrom = data.SICK_LEAVE_FROM;
                currentTreatmentFinishSDO.SickLeaveTo = data.SICK_LEAVE_TO;
                currentTreatmentFinishSDO.SickHeinCardNumber = data.SICK_HEIN_CARD_NUMBER;
                currentTreatmentFinishSDO.MotherName = data.TDL_PATIENT_MOTHER_NAME;
                currentTreatmentFinishSDO.FatherName = data.TDL_PATIENT_FATHER_NAME;
                currentTreatmentFinishSDO.PatientRelativeName = data.TDL_PATIENT_RELATIVE_NAME;
                currentTreatmentFinishSDO.PatientRelativeType = data.TDL_PATIENT_RELATIVE_TYPE;
                currentTreatmentFinishSDO.PatientWorkPlace = data.TDL_PATIENT_WORK_PLACE;
                currentTreatmentFinishSDO.TreatmentEndTypeExtId = data.TREATMENT_END_TYPE_EXT_ID;
                currentTreatmentFinishSDO.TreatmentDirection = data.TREATMENT_DIRECTION;
                currentTreatmentFinishSDO.UsedMedicine = data.USED_MEDICINE;
                currentTreatmentFinishSDO.TranPatiTechId = data.TRAN_PATI_TECH_ID;
                currentTreatmentFinishSDO.SickLoginname = data.SICK_LOGINNAME;
                currentTreatmentFinishSDO.SickUsername = data.SICK_USERNAME;
                currentTreatmentFinishSDO.SocialInsuranceNumber = data.TDL_SOCIAL_INSURANCE_NUMBER;
                currentTreatmentFinishSDO.EndTypeExtNote = data.END_TYPE_EXT_NOTE;
                currentTreatmentFinishSDO.IsPregnancyTermination = data.IS_PREGNANCY_TERMINATION == 1 ? true : false;
                currentTreatmentFinishSDO.GestationalAge = data.GESTATIONAL_AGE;
                currentTreatmentFinishSDO.PregnancyTerminationReason = data.PREGNANCY_TERMINATION_REASON;
                currentTreatmentFinishSDO.PregnancyTerminationTime = data.PREGNANCY_TERMINATION_TIME;
                if (!string.IsNullOrEmpty(data.APPOINTMENT_EXAM_ROOM_IDS))
                {
                    string[] str = data.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                    long[] longArr = Array.ConvertAll<string, long>(str, new Converter<string, long>(Convert.ToInt64));
                    List<long> _str = longArr.OfType<long>().ToList();
                    currentTreatmentFinishSDO.AppointmentExamRoomIds = _str;
                }

                currentTreatmentFinishSDO.AppointmentPeriodId = data.APPOINTMENT_PERIOD_ID;
                currentTreatmentFinishSDO.AppointmentSurgery = data.APPOINTMENT_SURGERY;
                currentTreatmentFinishSDO.AppointmentTime = data.APPOINTMENT_TIME;
                currentTreatmentFinishSDO.ApproveFinishNote = data.APPROVE_FINISH_NOTE;
                currentTreatmentFinishSDO.DocumentBookId = data.DOCUMENT_BOOK_ID;
                currentTreatmentFinishSDO.EndCode = data.END_CODE;
                currentTreatmentFinishSDO.EyesightGlassLeft = data.EYESIGHT_GLASS_LEFT;
                currentTreatmentFinishSDO.EyesightGlassRight = data.EYESIGHT_GLASS_RIGHT;
                currentTreatmentFinishSDO.EyesightLeft = data.EYESIGHT_LEFT;
                currentTreatmentFinishSDO.EyesightRight = data.EYESIGHT_RIGHT;
                currentTreatmentFinishSDO.EyeTensionLeft = data.EYE_TENSION_LEFT;
                currentTreatmentFinishSDO.EyeTensionRight = data.EYE_TENSION_RIGHT;
                currentTreatmentFinishSDO.IcdCauseCode = data.ICD_CAUSE_CODE;
                currentTreatmentFinishSDO.IcdCauseName = data.ICD_CAUSE_NAME;
                currentTreatmentFinishSDO.MediRecordTypeId = data.MEDI_RECORD_TYPE_ID;
                currentTreatmentFinishSDO.ProgramId = data.PROGRAM_ID;
                currentTreatmentFinishSDO.SurgeryAppointmentTime = data.SURGERY_APPOINTMENT_TIME;
                currentTreatmentFinishSDO.TraditionalIcdCode = data.TRADITIONAL_ICD_CODE;
                currentTreatmentFinishSDO.TraditionalIcdName = data.TRADITIONAL_ICD_NAME;
                currentTreatmentFinishSDO.TraditionalIcdSubCode = data.TRADITIONAL_ICD_SUB_CODE;
                currentTreatmentFinishSDO.TraditionalIcdText = data.TRADITIONAL_ICD_TEXT;
                currentTreatmentFinishSDO.IsChronic = data.IS_CHRONIC == 1;
                currentTreatmentFinishSDO.DeathCertBookId = data.DEATH_CERT_BOOK_ID;
                if (!String.IsNullOrWhiteSpace(data.TDL_PATIENT_WORK_PLACE_NAME))
                {
                    var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.WORK_PLACE_NAME == data.TDL_PATIENT_WORK_PLACE_NAME);
                    if (workPlace != null)
                    {
                        currentTreatmentFinishSDO.WorkPlaceId = workPlace.ID;
                    }
                }
                currentTreatmentFinishSDO.DeathIssuedDate = data.DEATH_ISSUED_DATE;
                currentTreatmentFinishSDO.DeathCertIssuerLoginname = data.DEATH_CERT_ISSUER_LOGINNAME;
                currentTreatmentFinishSDO.DeathCertIssuerUsername = data.DEATH_CERT_ISSUER_USERNAME;
                currentTreatmentFinishSDO.DeathDocumentTypeCode = data.DEATH_DOCUMENT_TYPE_CODE;              
                currentTreatmentFinishSDO.DeathStatus = data.DEATH_STATUS;
                currentTreatmentFinishSDO.SurgeryName = data.SURGERY_NAME;
                currentTreatmentFinishSDO.SurgeryBeginTime = data.SURGERY_BEGIN_TIME;
                currentTreatmentFinishSDO.SurgeryEndTime = data.SURGERY_END_TIME;
                if (data.VALID_1_YEAR == 1)
                {
                    currentTreatmentFinishSDO.Valid1Year = true;
                }
                else
                {
                    currentTreatmentFinishSDO.Valid1Year = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiem tra no vien phi
        /// Mức tiền cảnh báo nợ viện phí đối với BN nội trú và tủ trực
        /// </summary>
        private void CheckWarningOverTotalPatientPrice()
        {
            try
            {
                if (this.currentHisTreatment != null)
                {
                    var treatmentType = this.hisTreatmentTypes.FirstOrDefault(o => o.ID == this.currentHisTreatment.TDL_TREATMENT_TYPE_ID);
                    if (treatmentType != null && (treatmentType.FEE_DEBT_OPTION == 1
                                               || treatmentType.FEE_DEBT_OPTION == 2))
                    {
                        CommonParam param = new CommonParam();
                        HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                        filter.ID = treatmentId;
                        var treatmentFees = new BackendAdapter(param)
                            .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);

                        //So tien benh nhan can thu
                        if (treatmentFees == null || treatmentFees.Count == 0)
                            return;

                        decimal totalPrice = 0;
                        decimal totalHeinPrice = 0;
                        decimal totalPatientPrice = 0;
                        decimal totalDeposit = 0;
                        decimal totalDebt = 0;
                        decimal totalBill = 0;
                        decimal totalBillTransferAmount = 0;
                        decimal totalRepay = 0;
                        decimal total_obtained_price = 0;
                        totalPrice = treatmentFees[0].TOTAL_PRICE ?? 0; // tong tien
                        totalHeinPrice = treatmentFees[0].TOTAL_HEIN_PRICE ?? 0;
                        totalPatientPrice = treatmentFees[0].TOTAL_PATIENT_PRICE ?? 0; // bn tra
                        totalDeposit = treatmentFees[0].TOTAL_DEPOSIT_AMOUNT ?? 0;
                        totalDebt = treatmentFees[0].TOTAL_DEBT_AMOUNT ?? 0;
                        totalBill = treatmentFees[0].TOTAL_BILL_AMOUNT ?? 0;
                        totalBillTransferAmount = treatmentFees[0].TOTAL_BILL_TRANSFER_AMOUNT ?? 0;
                        totalRepay = treatmentFees[0].TOTAL_REPAY_AMOUNT ?? 0;
                        total_obtained_price = (totalDeposit + totalDebt + totalBill - totalBillTransferAmount - totalRepay);//Da thu benh nhan
                        decimal transfer = totalPatientPrice - total_obtained_price;//Phai thu benh nhan

                        if (transfer > 0)
                        {
                            if (treatmentType.FEE_DEBT_OPTION == 1)
                            {
                                DialogResult myResult;
                                myResult = DevExpress.XtraEditors.XtraMessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangThieuVienPhiBanCoMuonTiepTuc, Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                if (myResult != DialogResult.OK)
                                {
                                    this.Close();
                                }
                            }
                            else if (treatmentType.FEE_DEBT_OPTION == 2)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(this, String.Format(ResourceMessage.BenhNhanDangThieuVienPhi, Inventec.Common.Number.Convert.NumberToString(transfer, ConfigApplications.NumberSeperator)), ResourceMessage.ThongBao, MessageBoxButtons.OK);
                                this.Close();
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

        #region init
        private async Task InitUcIcdTotal()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("InitUcIcdTotal 1");
                InitUcIcd();
                InitUcSecondaryIcd();
                InitUcIcdYhct();
                InitUcSecondaryIcdYhct();
                InitUcCauseIcd();
                Inventec.Common.Logging.LogSystem.Warn("InitUcIcdTotal 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = LoadRequiredCause;
                ado.DelegateRefreshSubIcd = LoadSubIcd;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 24;
                ado.IsColor = true;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ado.hisTreatment = currentHisTreatment;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadSubIcd(string icdCodes, string icdNames)
        {
            try
            {
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.SetAttachIcd(this.ucSecondaryIcd, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                if (this.IcdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.SetRequired(this.ucIcdCause, isRequired, HIS.UC.Icd.ADO.Template.NoFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null && ucSecondaryIcd.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcd);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdSubCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        subIcdProcessor.FocusControl(ucSecondaryIcd);
                    }
                    else
                    {
                        NextForcusOut();
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán phụ";
                ado.TextNullValue = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_ICD_TEXT__NULL_VALUE");
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ado.hisTreatment = currentHisTreatment;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void NextForcusOut()
        {
            try
            {
                if (icdYhctProcessor != null && ucIcdYhct != null && ucIcdYhct.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucIcdYhct);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "chkEditIcd")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "cboIcds")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdMainText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        icdYhctProcessor.FocusControl(ucIcdYhct, Template.NoFocus);
                    }
                    else
                    {
                        NextForcusOutYhct();
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusOutYhct;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 24;
                ado.Template = Template.NoFocus;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ado.LblIcdMain = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_YHCT");
                ado.ToolTipsIcdMain = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_YHCT__TOOL_TIP");
                ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (ucIcdYhct != null)
                {
                    this.panelControlIcdYhct.Controls.Add(ucIcdYhct);
                    ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                var icdYhct = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_YHCT_SEC");
                ado.TootiplciIcdSubCode = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_YHCT_SEC__TOOL_TIP");
                ado.TextNullValue = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_ICD_TEXT__NULL_VALUE");
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.panelControlSecondIcdYhct.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOutYhct()
        {
            try
            {
                if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null && ucSecondaryIcdYhct.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcdYhct);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdSubCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                    }
                    else
                    {
                        NextForcusSubIcdToDo();
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCodeYhct()
        {
            string mainCode = "";
            try
            {
                if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct, Template.NoFocus);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void InitUcCauseIcd()
        {
            try
            {
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.IcdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = true;
                ado.Width = 440;
                ado.LblIcdMain = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_NNN");
                ado.ToolTipsIcdMain = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_NNN__TOOL_TIP");
                ado.Height = 24;
                ado.IsColor = false;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_CAUSE == 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                ado.Template = Template.NoFocus;
                this.ucIcdCause = (UserControl)this.IcdCauseProcessor.Run(ado);

                if (this.ucIcdCause != null)
                {
                    this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
                    this.ucIcdCause.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcdToDo()
        {
            try
            {
                if (IcdCauseProcessor != null && ucIcdCause != null && ucIcdCause.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucIcdCause);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "chkEditIcd")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "cboIcds")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdMainText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        IcdCauseProcessor.FocusControl(ucIcdCause, Template.NoFocus);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void ThreadLoadSereServ()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadSereServ. 1");
                CommonParam param = new CommonParam();
                //Load dịch vụ
                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.TREATMENT_ID = this.treatmentId;
                SereServTreatment = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadSereServ. 2");
                SereServCheck = SereServTreatment.Where(o => o.IS_NO_EXECUTE == null || o.IS_NO_EXECUTE == 0).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ThreadLoadDepartmentTran()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDepartmentTran. 1");
                CommonParam param = new CommonParam();
                //load chuyển khoa
                HisDepartmentTranFilter depaTranFilter = new HisDepartmentTranFilter();
                depaTranFilter.TREATMENT_ID = this.treatmentId;
                ListDepartmentTran = await new BackendAdapter(param).GetAsync<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, depaTranFilter, param);
                Inventec.Common.Logging.LogSystem.Debug("ThreadLoadDepartmentTran. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
