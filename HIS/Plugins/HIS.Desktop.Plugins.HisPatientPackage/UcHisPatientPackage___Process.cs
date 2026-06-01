/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisPatientPackage.ADO;
using HIS.Desktop.Plugins.HisPatientPackage.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
// Tránh nhập nhằng giữa System.Windows.Forms.Message và Message của thông báo.
using Message = Inventec.Desktop.Common.LibraryMessage.Message;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    public partial class UcHisPatientPackage
    {
        #region Filter buttons / date navigation

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try { FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try { SetDefaultControl(); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnPrevDate_Click(object sender, EventArgs e)
        {
            try { ShiftDate(-1); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnNextDate_Click(object sender, EventArgs e)
        {
            try { ShiftDate(1); FillDataToGrid(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void cboTimeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Đổi UI (mask date, ẩn/hiện dteToDate, enable Prev/Next) theo loại thời gian.
                ApplyTimeTypeUi();
                FillDataToGrid();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nút thu/mở nhóm "Thời gian tạo" — ẩn/hiện cụm chọn thời gian.</summary>
        private void btnToggleTime_Click(object sender, EventArgs e)
        {
            try
            {
                timeExpanded = !timeExpanded;
                cboTimeType.Visible = timeExpanded;
                dteDate.Visible = timeExpanded;
                btnPrevDate.Visible = timeExpanded;
                btnNextDate.Visible = timeExpanded;
                btnToggleTime.Text = timeExpanded ? "▲" : "▼";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    FillDataToGrid();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Dịch ngày lọc theo bước của loại thời gian (ngày/tuần/tháng).</summary>
        private void ShiftDate(int direction)
        {
            try
            {
                if (dteDate.EditValue == null) dteDate.EditValue = DateTime.Now;
                DateTime d = Convert.ToDateTime(dteDate.EditValue);
                switch (cboTimeType.SelectedIndex)
                {
                    case 1: d = d.AddDays(7 * direction); break;   // Trong tuần
                    case 2: d = d.AddMonths(direction); break;      // Trong tháng
                    default: d = d.AddDays(direction); break;       // Trong ngày / Tùy chọn
                }
                dteDate.EditValue = d;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        #region Row actions

        /// <summary>
        /// Sửa gói -> mở plugin PatientPackageRegister với HIS_PATIENT + HIS_PATIENT_PACKAGE
        /// (chính 2 kiểu mà PatientPackageRegisterBehavior parse). Form bên kia bật isEditMode,
        /// đổi tiêu đề "Sửa gói dịch vụ" và fill thông tin BN + gói.
        /// Icon đã được disable theo ma trận trạng thái -> click chỉ fire khi hợp lệ.
        /// </summary>
        private void EditProcess(PatientPackageADO row)
        {
            try
            {
                if (row == null) { ShowChonGoi(); return; }
                OpenModuleByLink(ModuleLinkString.PatientPackageRegister, BuildPackageArgs(row), true);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Thanh toán -> mở Thanh toán khác, truyền gói + BN (theo mục 6.5 tài liệu).</summary>
        private void PayProcess(PatientPackageADO row)
        {
            try
            {
                if (row == null) { ShowChonGoi(); return; }
                OpenModuleByLink(ModuleLinkString.TransactionBillOther, BuildPackageArgs(row), true);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Hoàn tiền -> mở Hoàn ứng dịch vụ. Plugin TransactionRepay yêu cầu TransactionRepayADO
        /// (xem comment "45677" trong ADO) với Patient + PatientPackage. KHÔNG truyền raw HIS_PATIENT/HIS_PATIENT_PACKAGE.
        /// </summary>
        private void RefundProcess(PatientPackageADO row)
        {
            try
            {
                if (row == null) { ShowChonGoi(); return; }

                long cashierRoomId = GetCashierRoomId();
                if (cashierRoomId <= 0)
                {
                    // IsActionAllowed đã chặn icon, nhưng safety check.
                    Inventec.Common.Logging.LogSystem.Warn(
                        "[Refund] Khong tim thay cashier room cho RoomId=" +
                        (currentModule != null ? currentModule.RoomId.ToString() : "null"));
                    return;
                }

                HIS_PATIENT patient = LoadPatient(row.PATIENT_ID);
                if (patient == null) { patient = new HIS_PATIENT(); patient.ID = row.PATIENT_ID; }

                HIS_PATIENT_PACKAGE pkg = new HIS_PATIENT_PACKAGE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_PACKAGE>(pkg, (V_HIS_PATIENT_PACKAGE)row);

                // HisPatientPackage không gắn treatment trực tiếp -> treatmentId = 0.
                // Plugin TransactionRepay sẽ check PatientPackage != null -> chạy luồng "hoàn theo gói BN".
                var ado = new TransactionRepayADO(0L, cashierRoomId);
                ado.Patient = patient;
                ado.PatientPackage = pkg;

                List<object> args = new List<object>();
                args.Add(ado);

                OpenModuleByLink(ModuleLinkString.TransactionRepay, args, true);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>Lấy ID của cashier room (V_HIS_CASHIER_ROOM.ID, KHÔNG phải ROOM_ID) theo phòng hiện tại.</summary>
        private long GetCashierRoomId()
        {
            try
            {
                long roomId = currentModule != null ? currentModule.RoomId : 0;
                if (roomId <= 0) return 0;
                var cashier = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>()
                    .FirstOrDefault(o => o.ROOM_ID == roomId);
                return cashier != null ? cashier.ID : 0;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return 0; }
        }

        /// <summary>
        /// Đối số truyền cho TẤT CẢ plugin con (Sửa / Thanh toán khác / Hoàn ứng):
        /// HIS_PATIENT (bệnh nhân — load tươi theo PATIENT_ID, fallback minimal nếu API fail) + HIS_PATIENT_PACKAGE
        /// (map từ view) + V_HIS_PATIENT_PACKAGE (view gói). Plugin con tự parse theo `is`. Spec §6.4/§6.5/§6.6.
        /// </summary>
        private List<object> BuildPackageArgs(PatientPackageADO row)
        {
            List<object> args = new List<object>();
            try
            {
                HIS_PATIENT_PACKAGE pkg = new HIS_PATIENT_PACKAGE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_PACKAGE>(pkg, (V_HIS_PATIENT_PACKAGE)row);

                // LUÔN add HIS_PATIENT non-null (kể cả khi API load fail) -> plugin con (TransactionBillOther/
                // TransactionRepay/PatientPackageRegister) chắc chắn có HIS_PATIENT để Factory parse.
                HIS_PATIENT patient = LoadPatient(row.PATIENT_ID);
                if (patient == null)
                {
                    patient = new HIS_PATIENT();
                    patient.ID = row.PATIENT_ID;  // minimal — plugin con có thể tự reload nếu cần
                }
                TransactionRepayADO ado = new TransactionRepayADO(0, GetCashierRoomIdForCurrentRoom());
                // Chỉ set AutoAmount khi tính được > 0 (phiếu có link), null thì form để trống/dùng default
                ado.Patient = patient;
                ado.PatientPackage = pkg;
                args.Add(ado);
                args.Add(patient);
                args.Add(pkg);
                args.Add((V_HIS_PATIENT_PACKAGE)row);               
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return args;
        }
        private long GetCashierRoomIdForCurrentRoom()
        {
            try
            {
                var allCashierRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                    .Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                if (allCashierRooms == null || allCashierRooms.Count == 0)
                    return 0;

                // Match chính xác ROOM_ID + ROOM_TYPE_ID
                var exact = allCashierRooms.FirstOrDefault(
                    o => o.ROOM_ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                if (exact != null) return exact.ID;

                // Fallback 1: chỉ match ROOM_ID
                var byRoom = allCashierRooms.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                if (byRoom != null) return byRoom.ID;

                return 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }
        /// <summary>Load HIS_PATIENT theo ID — dùng chung cho Sửa/Thanh toán/Hoàn tiền/In.</summary>
        private HIS_PATIENT LoadPatient(long patientId)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (patientId <= 0) return null;
                WaitingManager.Show();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = patientId;
                List<HIS_PATIENT> patients = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT>>(
                        HisRequestUriStore.MOSHIS_HIS_PATIENT_GET,
                        ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                return patients != null ? patients.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>Xóa gói (kèm chi tiết) — hỏi xác nhận trước khi gọi API.</summary>
        private void DeleteProcess(PatientPackageADO row)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (row == null) { ShowChonGoi(); return; }
                if (XtraMessageBox.Show(
                        ResourceMessage.BanCoMuonXoaGoiKhong,
                        MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                WaitingManager.Show();
                bool success = new BackendAdapter(param).Post<bool>(
                    HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_DELETE,
                    ApiConsumers.MosConsumer, row.ID, param);
                WaitingManager.Hide();

                MessageManager.Show(this.FindForm(), param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success) FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Khóa gói — bắt buộc nhập lý do khóa.
        /// KHÔNG phụ thuộc trạng thái (theo spec bảng §5.2 không có cột "Khóa"); icon "khóa mở"
        /// chỉ hiện trên dòng chưa khóa (gridView_CustomRowCellEdit), nên click chắc chắn hợp lệ.
        /// </summary>
        private void LockProcess(PatientPackageADO row)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (row == null) { ShowChonGoi(); return; }

                string reason;
                using (frmLockReason frm = new frmLockReason(
                    ResourceMessage.TieuDeKhoaGoi, ResourceMessage.LyDoKhoa))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;
                    reason = frm.Reason;
                }

                if (string.IsNullOrEmpty(reason))
                {
                    XtraMessageBox.Show(
                        ResourceMessage.VuiLongNhapLyDoKhoa,
                        MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // DEBUG: snapshot row trước khi map
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Lock] BEFORE Map - row from grid:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));

                HIS_PATIENT_PACKAGE dto = new HIS_PATIENT_PACKAGE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_PACKAGE>(dto, (V_HIS_PATIENT_PACKAGE)row);
                dto.STATUS_CODE = PatientPackageStatusCode.ToRaw(dto.STATUS_CODE);
                // Backend ChangeLock chỉ UPDATE entity (không tự toggle). PHẢI explicit set IS_ACTIVE = 0 để lock.
                dto.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
                dto.LOCKED_REASON = reason;

                // DEBUG: dto sau khi map + set field
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Lock] AFTER set fields - dto sẽ POST:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto));

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<HIS_PATIENT_PACKAGE>(
                    HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_CHANGE_LOCK,
                    ApiConsumers.MosConsumer, dto, param);
                WaitingManager.Hide();

                // DEBUG: response từ backend
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Lock] RESPONSE từ backend:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));

                bool success = result != null;
                MessageManager.Show(this.FindForm(), param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success) FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Mở khóa gói — hỏi xác nhận. KHÔNG check trạng thái: icon "khóa đóng" chỉ hiện trên dòng
        /// đang khóa (CANCELED) qua gridView_CustomRowCellEdit, nên click chắc chắn hợp lệ.
        /// </summary>
        private void UnlockProcess(PatientPackageADO row)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (row == null) { ShowChonGoi(); return; }
                if (XtraMessageBox.Show(
                        ResourceMessage.BanCoMuonMoKhoaGoiKhong,
                        MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                // DEBUG: snapshot row trước khi map
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Unlock] BEFORE Map - row from grid:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));

                HIS_PATIENT_PACKAGE dto = new HIS_PATIENT_PACKAGE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_PACKAGE>(dto, (V_HIS_PATIENT_PACKAGE)row);
                dto.STATUS_CODE = PatientPackageStatusCode.ToRaw(dto.STATUS_CODE);
                // Explicit set IS_ACTIVE = 1 để mở khóa. Clear LOCKED_REASON.
                dto.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                dto.LOCKED_REASON = null;

                // DEBUG: dto sẽ POST
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Unlock] AFTER set fields - dto sẽ POST:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto));

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<HIS_PATIENT_PACKAGE>(
                    HisRequestUriStore.MOSHIS_HIS_PATIENT_PACKAGE_CHANGE_LOCK,
                    ApiConsumers.MosConsumer, dto, param);
                WaitingManager.Hide();

                // DEBUG: response từ backend
                Inventec.Common.Logging.LogSystem.Info("[ChangeLock-Unlock] RESPONSE từ backend:");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));

                bool success = result != null;
                MessageManager.Show(this.FindForm(), param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success) FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // PrintProcess đã chuyển sang UcHisPatientPackage___Print.cs
        // (pattern y hệt PatientPackageRegister: RichEditorStore → DelegatePrintMps000514 → MpsPrinter.Run).

        #endregion

        #region Helpers

        private void ShowChonGoi()
        {
            try
            {
                XtraMessageBox.Show(
                    ResourceMessage.VuiLongChonGoi,
                    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Mở plugin khác theo ModuleLink — dùng PluginInstanceBehavior.ShowModule (ghim tab trong shell,
        /// KHÔNG popup ShowDialog) — luồng giống XmlChungTu / XuLyKham / AssignService.
        /// Refresh danh sách KHÔNG dựa vào ShowDialog return; dùng VisibleChanged của UC này khi user
        /// quay lại tab Danh sách gói (xem WireEvents).
        /// </summary>
        private void OpenModuleByLink(string moduleLink, List<object> args, bool refreshAfter)
        {
            try
            {
                // Pre-check ACS: nếu module không có trong currentModuleRaws -> hiện thông báo rõ ràng
                // (TaiKhoanKhongCoQuyen) thay vì để ShowModule fail silently.
                Inventec.Desktop.Common.Modules.Module moduleData =
                    GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == moduleLink);

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "[OpenModuleByLink] moduleLink={0} | moduleData={1}",
                    moduleLink,
                    moduleData == null ? "NULL (user chưa được cấp quyền ACS)" : "OK"));

                if (moduleData == null)
                {
                    XtraMessageBox.Show(
                        MessageUtil.GetMessage(Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang)
                        + "\n\nModule: " + moduleLink,
                        MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                long roomId = currentModule != null ? currentModule.RoomId : 0;
                long roomTypeId = currentModule != null ? currentModule.RoomTypeId : 0;

                // Bật flag refresh khi UC này VisibleChanged trở lại (user quay về tab Danh sách).
                if (refreshAfter) needsRefreshOnReturn = true;

                // Ghim tab trong shell — KHÔNG popup. Shell tự manage tab lifecycle + activate tab vừa mở.
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
                    moduleLink, roomId, roomTypeId, args);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
