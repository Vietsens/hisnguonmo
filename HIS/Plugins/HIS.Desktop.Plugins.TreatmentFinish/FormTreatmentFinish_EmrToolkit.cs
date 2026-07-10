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
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.EmrToolkitImport;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// Tính năng "Liên thông EmrToolKit dữ liệu chuyển tuyến":
    /// - Checkbox ChkLienThongEmrToolkit hiển thị khi đã cấu hình ConnectionInfo,
    ///   enable khi loại ra viện = chuyển viện, lưu trạng thái qua ControlState.
    /// - Sau khi lưu thành công (chuyển viện) → build dữ liệu thật của hồ sơ rồi
    ///   gọi thư viện HIS.Desktop.Plugins.Library.EmrToolkitImport đồng bộ lên cổng.
    /// </summary>
    public partial class FormTreatmentFinish
    {
        /// <summary>
        /// Ẩn/hiện checkbox theo cấu hình hệ thống EmrToolkit (ConnectionInfo có giá trị).
        /// Gọi trong Load, sau InitControlState.
        /// </summary>
        private void UpdateLienThongEmrToolkitVisibility()
        {
            try
            {
                bool configured = EmrToolkitImportProcessor.IsConfigured();
                LciLienThongEmrToolkit.Visibility = configured
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (!configured && ChkLienThongEmrToolkit.Checked)
                    ChkLienThongEmrToolkit.Checked = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Lưu trạng thái checkbox khi người dùng thay đổi.</summary>
        private void ChkLienThongEmrToolkit_CheckedChanged(object sender, EventArgs e)
        {
            if (isNotLoadWhileChangeControlStateInFirst) return;
            try
            {
                SaveLienThongEmrToolkitState();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveLienThongEmrToolkitState()
        {
            try
            {
                if (this.controlStateWorker == null)
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                string key = ChkLienThongEmrToolkit.Name;
                string value = ChkLienThongEmrToolkit.Checked ? "1" : "";

                var item = this.currentControlStateRDO.FirstOrDefault(
                    o => o.KEY == key && o.MODULE_LINK == moduleLink);
                if (item != null)
                {
                    item.VALUE = value;
                }
                else
                {
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
                    {
                        KEY = key,
                        MODULE_LINK = moduleLink,
                        VALUE = value
                    });
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Đồng bộ dữ liệu chuyển tuyến qua EmrToolkit (nếu checkbox bật + là chuyển viện).
        /// Gọi sau khi lưu hồ sơ thành công.
        /// </summary>
        private void SyncEmrToolkitIfNeeded(MOS.SDO.HisTreatmentFinishSDO sdo)
        {
            try
            {
                if (ChkLienThongEmrToolkit == null
                    || !ChkLienThongEmrToolkit.Visible
                    || !ChkLienThongEmrToolkit.Checked)
                    return;

                if (sdo == null
                    || sdo.TreatmentEndTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    return;

                EmrImportModel model = BuildEmrModelFromTreatment(sdo);

                EmrToolkitImportProcessor processor = new EmrToolkitImportProcessor();
                WaitingManager.Show();
                EmrToolkitImportResult result = processor.ImportEmr(model);
                WaitingManager.Hide();

                processor.ShowResult(result, this);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Dựng model Giấy Chuyển Viện từ dữ liệu THẬT của hồ sơ đang kết thúc:
        /// thông tin bệnh nhân + BHYT (patientTypeAlter) + chẩn đoán (SHOW_ICD_*) +
        /// dữ liệu chuyển tuyến (SDO) + ngày vào/ra viện.
        /// </summary>
        private EmrImportModel BuildEmrModelFromTreatment(MOS.SDO.HisTreatmentFinishSDO sdo)
        {
            EmrImportModel model = new EmrImportModel();
            try
            {
                model.LoaiKy = 0;
                model.ID = 0;
                model.TenMauPhieu = "Giấy Chuyển Viện";
                model.DanhSachChuoiKy = new List<object>();

                var t = this.currentHisTreatment;
                if (t != null)
                {
                    model.HoVaTenBenhNhan = t.TDL_PATIENT_NAME;
                    model.HoTenBN = t.TDL_PATIENT_NAME;
                    model.NgaySinh = ToDateTime(t.TDL_PATIENT_DOB);
                    model.GioiTinh = ParseInt(t.TDL_PATIENT_GENDER_ID);
                    model.DiaChi = t.TDL_PATIENT_ADDRESS;

                    model.ChanDoan = t.SHOW_ICD_NAME;
                    model.ChanDoanND = string.IsNullOrEmpty(t.SHOW_ICD_TEXT) ? t.SHOW_ICD_NAME : t.SHOW_ICD_TEXT;
                    model.ChanDoanNgay = ToDateTime(t.OUT_TIME);

                    model.NgayVaoVien = ToDateTime(t.CLINICAL_IN_TIME);
                    model.NgayBDDieuTri = ToDateTime(t.CLINICAL_IN_TIME);

                    model.SoNhapVien = t.TREATMENT_CODE;
                    model.Ma_LK = t.TREATMENT_CODE;
                    model.MA_LK = t.TREATMENT_CODE;
                }

                // Ngày ra viện + số chuyển viện theo bản ghi vừa lưu
                long? outTime = (this.hisTreatmentResult != null)
                    ? this.hisTreatmentResult.OUT_TIME
                    : (t != null ? t.OUT_TIME : (long?)null);
                model.NgayRaVien = ToDateTime(outTime);
                model.NgayKTDieuTri = ToDateTime(outTime);
                if (this.hisTreatmentResult != null)
                    model.SoGiayCV = this.hisTreatmentResult.OUT_CODE;

                // BHYT
                if (this.patientTypeAlter != null)
                {
                    model.SoTheBHYT = this.patientTypeAlter.HEIN_CARD_NUMBER;
                    model.SoThe = this.patientTypeAlter.HEIN_CARD_NUMBER;
                    model.BatDauBHYT = ToDateTime(this.patientTypeAlter.HEIN_CARD_FROM_TIME);
                    model.KetThucBHYT = ToDateTime(this.patientTypeAlter.HEIN_CARD_TO_TIME);
                    model.NgayHetHanBHYT = ToDateTime(this.patientTypeAlter.HEIN_CARD_TO_TIME);
                }

                // Dữ liệu chuyển tuyến (từ SDO)
                if (sdo != null)
                {
                    model.KinhGui = sdo.TransferOutMediOrgName;
                    model.DauHieuLamSan = sdo.ClinicalSigns;
                    model.TTLucNhapVien = sdo.PatientCondition;
                    model.PTVanChuyen = sdo.TransportVehicle;
                    model.HoTenNDD = sdo.Transporter;
                    model.ThuocKhac = string.IsNullOrEmpty(sdo.UsedMedicine) ? sdo.TreatmentMethod : sdo.UsedMedicine;
                    model.LiDoCV = GetTranPatiReasonName(sdo.TranPatiReasonId);
                }

                // IDMauPhieu + MaCoSoKhamChuaBenh để trống → thư viện tự điền theo cấu hình/token
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return model;
        }

        /// <summary>Lấy tên lý do chuyển tuyến theo ID (từ cache HisTranPatiReasons).</summary>
        private string GetTranPatiReasonName(long? reasonId)
        {
            try
            {
                if (reasonId == null || reasonId.Value <= 0)
                    return "";
                var reason = Base.GlobalStore.HisTranPatiReasons.FirstOrDefault(o => o.ID == reasonId.Value);
                return reason != null ? reason.TRAN_PATI_REASON_NAME : "";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Chuyển ngày kiểu long (yyyyMMddHHmmss) sang DateTime?.</summary>
        private DateTime? ToDateTime(long? timeNumber)
        {
            try
            {
                if (timeNumber == null || timeNumber.Value <= 0)
                    return null;
                return Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeNumber.Value);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Ép kiểu an toàn sang int (chịu được long/long?/null).</summary>
        private int ParseInt(object value)
        {
            try
            {
                if (value == null) return 0;
                return System.Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
