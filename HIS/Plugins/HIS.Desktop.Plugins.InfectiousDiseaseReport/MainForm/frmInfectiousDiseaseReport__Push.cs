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
using System.Linq;
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

                    // Lưu bản ghi đối soát vào HIS (Create/Update entity — §20)
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
                // Combo giữ MÃ SDA -> đẩy cổng cần ID cổng: đối chiếu mã -> ID (best-effort).
                dto.DanTocId = ResolveEcdsIdStatic(EcdsCatalogCache.DM_DANTOC, GetLookupString(cboDanToc));
                dto.NgheNghiepId = ResolveEcdsIdStatic(EcdsCatalogCache.DM_NGHENGHIEP, GetLookupString(cboNgheNghiep));
                dto.NoiLamViec = txtNoiLamViec.Text;
                dto.TinhId = ResolveEcdsIdStatic(EcdsCatalogCache.DM_TINH, GetLookupString(cboTinh));
                dto.XaId = ResolveEcdsIdXa(GetLookupString(cboTinh), GetLookupString(cboXa));
                // Thôn/ấp: combo đã nạp từ danh mục cổng (cascade theo xã) -> ValueMember là ID cổng.
                dto.ThonId = GetLookupLong(cboThon);
                dto.DiaChi = txtDiaChi.Text;
                dto.TinhIdThuongTru = ResolveEcdsIdStatic(EcdsCatalogCache.DM_TINH, GetLookupString(cboTinhTru));
                dto.XaIdThuongTru = ResolveEcdsIdXa(GetLookupString(cboTinhTru), GetLookupString(cboXaTru));
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
                // Tình trạng ra viện: đẩy mã HIS_TREATMENT_END_TYPE (best-effort — QĐ 4039 chưa liệt kê enum cổng).
                dto.TinhTrangRaVien = ToNullableInt(GetLookupLong(cboTinhTrangRaVien));
                // Tên bệnh viện chuyển tới: lấy theo tên đang hiển thị trên combo.
                dto.BenhVienChuyenToi = (cboBenhVienChuyenToi.EditValue != null) ? cboBenhVienChuyenToi.Text : null;

                // ---- Người báo cáo ----
                dto.NguoiBaoCao = txtNguoiBaoCao.Text;
                dto.DienThoaiNguoiBaoCao = txtDienThoaiBaoCao.Text;
                dto.EmailNguoiBaoCao = txtEmailBaoCao.Text;

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

        /// <summary>Đối chiếu MÃ (SDA/HIS) -> ID danh mục cổng (tĩnh). Null nếu không có/không thấy.</summary>
        private long? ResolveEcdsIdStatic(string danhMuc, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || catalogCache == null) return null;
                return catalogCache.FindIdByMa(catalogCache.GetStatic(danhMuc), code);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đối chiếu mã xã -> ID xã cổng (cascade theo mã tỉnh). Best-effort.</summary>
        private long? ResolveEcdsIdXa(string provinceCode, string communeCode)
        {
            try
            {
                if (string.IsNullOrEmpty(communeCode) || catalogCache == null) return null;
                var list = catalogCache.GetCascade(EcdsCatalogCache.DM_XA,
                    new SearchDanhMucFastDto { maTinh = provinceCode }, provinceCode ?? "");
                return catalogCache.FindIdByMa(list, communeCode);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>ID bản ghi HIS_ECDS_DISEASE_CASE (0 = chưa lưu). Dùng để Update khi đẩy lại.</summary>
        private long hisEcdsCaseId = 0;

        /// <summary>
        /// Nút "Lưu": lưu ca bệnh vào HIS (Create nếu mới / Update nếu đã có) — KHÔNG đẩy cổng.
        /// Giữ nguyên trạng thái đẩy hiện tại (mã ca/ID/PUSH_STATE).
        /// </summary>
        private void SaveToHisProcess()
        {
            try
            {
                string err;
                if (!ValidateForm(out err)) return;

                var c = BuildCaseEntity();
                // Giữ trạng thái đẩy hiện tại (lưu HIS không phải là đẩy cổng).
                c.ECDS_CASE_ID = this.ecdsCaseId;
                c.ECDS_CASE_CODE = this.ecdsCaseCode;
                c.PUSH_STATE = (short)(string.IsNullOrEmpty(this.ecdsCaseCode)
                    ? (int)EcdsPushState.ChuaDay : (int)EcdsPushState.DaDay);

                btnSave.Enabled = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                string uri = this.hisEcdsCaseId > 0
                    ? HisRequestUriStore.HIS_ECDS_UPDATE
                    : HisRequestUriStore.HIS_ECDS_CREATE;
                // Create/Update nhận & trả thẳng entity HIS_ECDS_DISEASE_CASE (không bọc SDO).
                var saved = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ECDS_DISEASE_CASE>(
                    uri, ApiConsumers.MosConsumer, c, param);
                WaitingManager.Hide();

                bool ok = saved != null && saved.ID > 0;
                if (ok) this.hisEcdsCaseId = saved.ID;
                SessionManager.ProcessTokenLost(param);

                if (ok)
                {
                    Inventec.Common.Logging.LogUtil.LogActionSuccess(
                        "InfectiousDiseaseReport", "Save",
                        Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                    if (dlgRefresh != null) dlgRefresh();
                    XtraMessageBox.Show("Lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show("Lưu thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                XtraMessageBox.Show("Có lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { btnSave.Enabled = true; }
        }

        /// <summary>Lưu ca bệnh vào HIS qua backend MOS (Create/Update entity) SAU khi đẩy cổng. Lỗi không chặn luồng đẩy.</summary>
        private void PersistToHis(EcdsDiseaseCaseDto dto, string maCaBenh, string ecdsId, int pushState, string message)
        {
            try
            {
                var c = BuildCaseEntity();
                c.ECDS_CASE_ID = ecdsId;
                c.ECDS_CASE_CODE = maCaBenh;
                c.PUSH_STATE = (short)pushState;
                c.LAST_PUSH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                c.PUSH_MESSAGE = message;

                CommonParam param = new CommonParam();
                string uri = this.hisEcdsCaseId > 0
                    ? HisRequestUriStore.HIS_ECDS_UPDATE
                    : HisRequestUriStore.HIS_ECDS_CREATE;

                // Create/Update nhận & trả thẳng entity HIS_ECDS_DISEASE_CASE (không bọc SDO).
                var saved = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ECDS_DISEASE_CASE>(
                    uri, ApiConsumers.MosConsumer, c, param);
                if (saved != null && saved.ID > 0)
                    this.hisEcdsCaseId = saved.ID;
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                // Không chặn: đẩy cổng đã thành công, chỉ lưu HIS lỗi -> ghi log.
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Map dữ liệu form → HIS_ECDS_DISEASE_CASE (chỉ field ca bệnh; field đẩy do caller set).</summary>
        private MOS.EFMODEL.DataModels.HIS_ECDS_DISEASE_CASE BuildCaseEntity()
        {
            var c = new MOS.EFMODEL.DataModels.HIS_ECDS_DISEASE_CASE();
            try
            {
                System.Func<long?, short?> toShort = v => v.HasValue ? (short?)v.Value : null;
                System.Func<long?, decimal?> toDec = v => v.HasValue ? (decimal?)v.Value : null;

                c.ID = this.hisEcdsCaseId;
                c.TREATMENT_ID = treatment != null ? treatment.ID : 0;
                c.IS_ACTIVE = 1;
                c.IS_DELETE = 0;

                // ---- Ca bệnh ----
                c.REPORTED_DISEASE_ID = toDec(GetLookupLong(cboBenh));
                c.REPORTED_ICD_CODE = PrimaryIcdCode(GetSelectedBenhMa());   // chỉ mã chính (cột DB tối đa 10 ký tự)
                c.DISEASE_SEVERITY_ID = toDec(GetLookupLong(cboCapDoBenh));
                c.DIAGNOSIS_TYPE = (short)GetLookupInt(cboLoaiChanDoan, (int)EcdsPhanLoaiChuanDoan.XacDinh);
                c.CURRENT_STATE = (decimal)GetLookupInt(cboTinhTrang, (int)EcdsTinhTrangHienNay.NgoaiTru);
                c.OTHER_STATE_DESC = txtTinhTrangKhac.Text;
                c.ONSET_DATE = GetDateLong(dteNgayKhoiPhat);
                c.DEATH_DATE = GetDateLong(dteNgayTuVong);
                c.DISCHARGE_STATE = toDec(GetLookupLong(cboTinhTrangRaVien));                         // Tình trạng ra viện (TINHTRANGRAVIEN)
                c.TRANSFER_HOSPITAL_ID = toDec(GetLookupLong(cboBenhVienChuyenToi));
                c.TRANSFER_HOSPITAL_NAME = (cboBenhVienChuyenToi.EditValue != null) ? cboBenhVienChuyenToi.Text : null;  // tên BV chuyển tới
                c.DISCHARGE_DIAGNOSIS = txtChanDoanRaVien.Text;
                c.SUB_DIAGNOSIS = txtSubDiagnosis.Text;
                c.COMPLICATION = txtComplication.Text;
                c.GENERAL_NOTE = txtGhiChu.Text;
                c.IS_PREGNANT = (short)(chkMangThai.Checked ? (int)EcdsMangThai.Co : (int)EcdsMangThai.Khong);
                c.VILLAGE_ID = toDec(GetLookupLong(cboThon));                                         // Thôn/ấp hiện nay (VILLAGE_ID = THON_ID)
                c.WORKPLACE = txtNoiLamViec.Text;                                                     // Nơi làm việc (NOILAMVIEC)

                // ---- Triệu chứng & XN ----
                c.EPIDEMIOLOGY_HISTORY = txtTienSuDichTe.Text;
                c.VACCINE_USE = toShort(GetLookupLong(cboSuDungVacXin));
                c.VACCINE_USE_COUNT = (spnSoLan.EditValue != null) ? (decimal?)Convert.ToDecimal(spnSoLan.Value) : null;
                c.IS_SPECIMEN_TAKEN = toShort(GetLookupLong(cboLayMau));
                c.TEST_TYPE = toDec(GetLookupLong(cboLoaiXN));
                c.OTHER_TEST_NAME = txtLoaiXNKhac.Text;
                c.TEST_RESULT = toDec(GetLookupLong(cboKetQuaXN));
                c.TEST_TIME = GetDateLong(dteNgayThucHienXN);
                c.RESULT_TIME = GetDateLong(dteNgayTraKQ);
                c.TEST_FACILITY_ID = toDec(GetLookupLong(cboDonViXN));
                c.DETECTION_FACILITY_TYPE = (decimal)GetLookupInt(cboLoaiPhatHien, (int)EcdsLoaiPhatHien.Khac);
                c.TREATMENT_FACILITY_NAME = lblCoSoDieuTriVal.Text;

                // ---- Người báo cáo ----
                c.REPORTER_NAME = txtNguoiBaoCao.Text;
                c.REPORTER_PHONE = txtDienThoaiBaoCao.Text;
                c.REPORTER_EMAIL = txtEmailBaoCao.Text;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return c;
        }

        /// <summary>
        /// GetFull: lấy ca bệnh đã lưu theo TREATMENT_CODE (chỉ dùng bản ghi cha để xác định trạng thái đối soát).
        /// Best-effort: backend chưa sẵn -> trả null, coi như "chưa đẩy".
        /// </summary>
        private MOS.SDO.HisEcdsDiseaseCaseFullSDO LoadEcdsCaseFull()
        {
            try
            {
                if (treatment == null || string.IsNullOrEmpty(treatment.TREATMENT_CODE))
                    return null;

                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisEcdsDiseaseCaseViewFilter { TREATMENT_CODE = treatment.TREATMENT_CODE };
                // GetFull gọi GET (backend cấu hình [HttpGet]).
                var list = new BackendAdapter(param).Get<System.Collections.Generic.List<MOS.SDO.HisEcdsDiseaseCaseFullSDO>>(
                    HisRequestUriStore.HIS_ECDS_GET_FULL, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                var full = (list != null && list.Count > 0) ? list[0] : null;
                if (full != null && full.DiseaseCase != null)
                {
                    var c = full.DiseaseCase;   // V_HIS_ECDS_DISEASE_CASE
                    this.hisEcdsCaseId = c.ID;
                    this.ecdsCaseId = c.ECDS_CASE_ID;
                    this.ecdsCaseCode = c.ECDS_CASE_CODE;
                }
                return full;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
