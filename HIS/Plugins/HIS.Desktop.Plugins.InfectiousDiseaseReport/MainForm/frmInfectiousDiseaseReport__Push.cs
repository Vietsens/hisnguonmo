/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Build DTO từ form -> đẩy lên cổng ECDS -> đối soát kết quả.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Config;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private void PushProcess()
        {
            try
            {
                if (!EcdsConfigCFG.IsValid())
                {
                    XtraMessageBox.Show(Resources.ResourceMessage.ChuaCauHinhKetNoiEcds,
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string err;
                if (!ValidateForm(out err)) return;

                if (XtraMessageBox.Show(Resources.ResourceMessage.XacNhanDayCaBenh,
                        "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                EcdsDiseaseCaseDto dto = BuildDtoFromForm();

                btnPush.Enabled = false;
                WaitingManager.Show();
                KetQuaEcdsDto<CaBenhResultDto> result = apiWorker.DayCaBenh(dto);
                WaitingManager.Hide();

                if (result != null && result.thanhCong && result.duLieu != null)
                {
                    this.ecdsCaseId = result.duLieu.id;
                    this.ecdsCaseCode = result.duLieu.maCaBenh;
                    UpdatePushStatusLabel();

                    // Lưu bản ghi đối soát vào HIS (SaveCreate/SaveUpdate — §20)
                    PersistToHis(dto, result.duLieu.maCaBenh, result.duLieu.id, (int)EcdsPushState.DaDay, "");

                    if (dlgRefresh != null) dlgRefresh();

                    Inventec.Common.Logging.LogUtil.LogActionSuccess(
                        "InfectiousDiseaseReport", "Push",
                        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());

                    XtraMessageBox.Show("Đã đẩy thành công. Mã ca bệnh: " + this.ecdsCaseCode,
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string msg = result != null ? result.thongDiep : "Không nhận được phản hồi từ cổng ECDS.";
                    Inventec.Common.Logging.LogUtil.LogActionFail(
                        "InfectiousDiseaseReport", "Push",
                        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                    XtraMessageBox.Show("Đẩy thất bại: " + msg,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi đẩy ca bệnh: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPush.Enabled = true;
            }
        }

        private EcdsDiseaseCaseDto BuildDtoFromForm()
        {
            var dto = new EcdsDiseaseCaseDto();
            try
            {
                dto.Id = this.ecdsCaseId;   // đẩy lại -> update

                // ---- Ca bệnh ----
                dto.BenhChuanDoanId = GetLookupLong(cboBenh) ?? 0;
                dto.CapDoBenhId = GetLookupLong(cboCapDoBenh);
                dto.PhanLoaiChuanDoan = GetLookupInt(cboLoaiChanDoan, (int)EcdsPhanLoaiChuanDoan.XacDinh);
                dto.TinhTrangHienNay = GetLookupInt(cboTinhTrang, (int)EcdsTinhTrangHienNay.NgoaiTru);
                dto.NgayKhoiPhat = ToIso(dteNgayKhoiPhat);
                dto.NgayNhapVien = ToIso(dteNgayNhapVien);
                dto.NgayRaVien = ToIso(dteNgayRaVien);
                dto.NgayTuVong = ToIso(dteNgayTuVong);
                dto.TinhTrangKhac = txtTinhTrangKhac.Text;
                dto.BenhVienChuyenToiId = GetLookupLong(cboBenhVienChuyenToi);
                dto.ChanDoanRaVien = txtChanDoanRaVien.Text;
                dto.BenhChuanDoanPhu = txtSubDiagnosis.Text;
                dto.ChuanDoanBienChung = txtComplication.Text;
                dto.GhiChu = txtGhiChu.Text;

                // ---- Hành chính ----
                dto.HoTen = txtHoTen.Text;
                dto.NgaySinh = ToIso(dteNgaySinh);
                dto.GioiTinh = GetLookupInt(cboGioiTinh, (int)EcdsGioiTinh.Nam);
                dto.IsMangThai = chkMangThai.Checked ? (int)EcdsMangThai.Co : (int)EcdsMangThai.Khong;
                dto.Cccd = txtCccd.Text;
                dto.DienThoai = txtDienThoai.Text;
                dto.DanTocId = GetLookupLong(cboDanToc);
                dto.NgheNghiepId = GetLookupLong(cboNgheNghiep);
                dto.NoiLamViec = txtNoiLamViec.Text;
                dto.TinhId = GetLookupLong(cboTinh);
                dto.XaId = GetLookupLong(cboXa);
                dto.DiaChi = txtDiaChi.Text;
                dto.TinhIdThuongTru = GetLookupLong(cboTinhTru);
                dto.XaIdThuongTru = GetLookupLong(cboXaTru);
                dto.DiaChiThuongTru = txtDiaChiTru.Text;

                // ---- Triệu chứng & XN ----
                dto.TienSuDichTe = txtTienSuDichTe.Text;
                dto.SuDungVacXin = ToNullableInt(GetLookupLong(cboSuDungVacXin));
                dto.SoLanSuDung = (spnSoLan.EditValue != null) ? (int?)Convert.ToInt32(spnSoLan.Value) : null;
                dto.LayMauXetNghiem = ToNullableInt(GetLookupLong(cboLayMau));
                dto.LoaiXetNghiem = ToNullableInt(GetLookupLong(cboLoaiXN));
                dto.LoaiXetNghiemKhac = txtLoaiXNKhac.Text;
                dto.KetQuaXetNghiem = ToNullableInt(GetLookupLong(cboKetQuaXN));
                dto.NgayThucHienXn = ToIso(dteNgayThucHienXN);
                dto.NgayTraKetQuaXn = ToIso(dteNgayTraKQ);
                dto.DonViThucHienXn = GetLookupLong(cboDonViXN);
                dto.LoaiPhatHien = GetLookupInt(cboLoaiPhatHien, (int)EcdsLoaiPhatHien.Khac);
                dto.CoSoDieuTri = lblCoSoDieuTriVal.Text;

                // ---- Người báo cáo ----
                dto.NguoiBaoCao = txtNguoiBaoCao.Text;
                dto.DienThoaiNguoiBaoCao = txtDienThoaiBaoCao.Text;
                dto.EmailNguoiBaoCao = txtEmailBaoCao.Text;

                // TODO: nhóm sốt rét + 2 mảng (thuốc sốt rét, lịch sử di chuyển) — bổ sung khi có DTO/tài liệu.

                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dto;
        }

        private string ToIso(DevExpress.XtraEditors.DateEdit dte)
        {
            return DiseaseCaseMapper.ToIsoDate(GetDateLong(dte));
        }

        private int? ToNullableInt(long? value)
        {
            return value.HasValue ? (int?)value.Value : null;
        }

        /// <summary>ID bản ghi HIS_ECDS_DISEASE_CASE (0 = chưa lưu). Dùng để SaveUpdate khi đẩy lại.</summary>
        private long hisEcdsCaseId = 0;

        /// <summary>Lưu bản ghi đối soát vào HIS qua backend MOS (§20). Lỗi không chặn luồng đẩy.</summary>
        private void PersistToHis(EcdsDiseaseCaseDto dto, string maCaBenh, string ecdsId, int pushState, string message)
        {
            try
            {
                var save = new HisEcdsDiseaseCaseSaveADO
                {
                    ID = this.hisEcdsCaseId,
                    TREATMENT_ID = treatment != null ? treatment.ID : 0,
                    PATIENT_ID = treatment != null ? (long?)treatment.PATIENT_ID : null,
                    ECDS_CASE_ID = ecdsId,
                    ECDS_CASE_CODE = maCaBenh,
                    PUSH_STATE = pushState,
                    LAST_PUSH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")),
                    PUSH_MESSAGE = message,
                    CASE_DATA = dto
                };

                CommonParam param = new CommonParam();
                string uri = this.hisEcdsCaseId > 0
                    ? HisRequestUriStore.HIS_ECDS_SAVE_UPDATE
                    : HisRequestUriStore.HIS_ECDS_SAVE_CREATE;

                var saved = new BackendAdapter(param).Post<HisEcdsDiseaseCaseSaveADO>(
                    uri, ApiConsumers.MosConsumer, save, param);
                if (saved != null && saved.ID > 0) this.hisEcdsCaseId = saved.ID;
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                // Không chặn: đẩy cổng đã thành công, chỉ lưu HIS lỗi -> ghi log.
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nạp bản ghi đối soát đã có theo điều trị (khi mở form) — để biết đã đẩy chưa.</summary>
        private void LoadExistingReconcile()
        {
            try
            {
                if (treatment == null || treatment.ID <= 0) return;
                CommonParam param = new CommonParam();
                var filter = new HisEcdsDiseaseCaseFilterADO { TREATMENT_ID = treatment.ID };
                var list = new BackendAdapter(param).Get<System.Collections.Generic.List<HisEcdsDiseaseCaseSaveADO>>(
                    HisRequestUriStore.HIS_ECDS_GET, ApiConsumers.MosConsumer, filter, param);
                var rec = (list != null && list.Count > 0) ? list[0] : null;
                if (rec != null)
                {
                    this.hisEcdsCaseId = rec.ID;
                    this.ecdsCaseId = rec.ECDS_CASE_ID;
                    this.ecdsCaseCode = rec.ECDS_CASE_CODE;
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
