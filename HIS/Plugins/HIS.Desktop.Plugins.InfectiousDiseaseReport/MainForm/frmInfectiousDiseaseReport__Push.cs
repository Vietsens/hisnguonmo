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
                dto.Id = this.ecdsCaseId;   // đẩy lại -> update ca cổng

                // ---- Bệnh / chẩn đoán ----
                // Cổng nhận MÃ ICD-10 (string), KHÔNG phải ID danh mục -> fix "bạn phải chọn bệnh".
                dto.MaIcd10Benh = PrimaryIcdCode(GetSelectedBenhMa());
                dto.MaPhanLoaiLamSang = GetSelectedMa(cboCapDoBenh);
                dto.LoaiChanDoan = GetLookupInt(cboLoaiChanDoan, (int)EcdsPhanLoaiChuanDoan.XacDinh);
                dto.TrangThaiCaBenh = (int)EcdsTrangThaiCaBenh.MacDinh;      // = 1 (theo ví dụ cổng)
                dto.TrangThaiLuu = (int)EcdsTrangThaiLuu.LuuChinhThuc;       // = 2 (đẩy chính thức)

                // ---- Hành chính bệnh nhân ----
                dto.HoVaTen = txtHoTen.Text;
                dto.NgaySinh = ToPortalDate(dteNgaySinh);
                dto.Tuoi = (spnTuoi.EditValue != null) ? (int?)Convert.ToInt32(spnTuoi.Value) : null;
                dto.MaGioiTinh = (GetLookupLong(cboGioiTinh) == (long)EcdsGioiTinh.Nam) ? "M" : "F";
                dto.DangMangThai = chkMangThai.Checked;
                dto.MaDanToc = ResolveEcdsMa(EcdsCatalogCache.DM_DANTOC, GetLookupString(cboDanToc));
                // cboNgheNghiep bind THẲNG danh mục cổng (ValueMember = ma) -> lấy mã cổng trực tiếp.
                dto.MaNgheNghiep = GetLookupString(cboNgheNghiep);
                dto.NoiLamViec = txtNoiLamViec.Text;
                dto.SoCccdCmnd = txtCccd.Text;
                dto.SoDienThoai = txtDienThoai.Text;

                // ---- Địa bàn hiện nay (BẮT BUỘC) — trống thì lấy từ địa chỉ THƯỜNG TRÚ ----
                // Mã xã: dùng thẳng mã hành chính (GSO) đang chọn — cổng nhận trực tiếp.
                string maXa = GetLookupString(cboXa);
                dto.MaXaHienNay = !string.IsNullOrEmpty(maXa) ? maXa : GetLookupString(cboXaTru);
                dto.MaThonHienNay = GetSelectedMa(cboThon);
                dto.DiaChiChiTietHienNay = !string.IsNullOrEmpty(txtDiaChi.Text) ? txtDiaChi.Text : txtDiaChiTru.Text;
                dto.MaXaPhuongQuanLy = Config.EcdsConfigCFG.MaDonVi;

                // ---- Diễn biến ca bệnh ----
                dto.TinhTrangHienTai = GetLookupInt(cboTinhTrang, (int)EcdsTinhTrangHienNay.NgoaiTru);
                dto.MaHinhThucDieuTri = GetLookupString(cboHinhThucDieuTri);   // "1"=Nội trú, "2"=Ngoại trú
                dto.NgayKhoiPhat = ToPortalDate(dteNgayKhoiPhat);
                dto.NgayNhapVien = ToPortalDate(dteNgayNhapVien);
                dto.NgayRaVien = ToPortalDate(dteNgayRaVien);
                dto.ChanDoanRaVien = txtChanDoanRaVien.Text;
                dto.ThongTinTiemVacXin = ToNullableInt(GetLookupLong(cboSuDungVacXin));
                dto.BenhKemTheo = txtSubDiagnosis.Text;      // chẩn đoán phụ/kèm theo
                dto.BienChung = txtComplication.Text;
                dto.GhiChuChung = txtGhiChu.Text;
                dto.TienSuDichTe = txtTienSuDichTe.Text;

                // ---- Xét nghiệm ----
                long? layMau = GetLookupLong(cboLayMau);
                dto.CoLayMauXetNghiem = layMau.HasValue ? (bool?)(layMau.Value == (long)EcdsLayMauXetNghiem.Co) : null;
                dto.TenXetNghiem = txtLoaiXNKhac.Text;
                dto.LoaiXetNghiemChung = ToNullableInt(GetLookupLong(cboLoaiXN));
                dto.KetQuaXetNghiemChung = ToNullableInt(GetLookupLong(cboKetQuaXN));
                dto.NgayLayMau = ToPortalDate(dteNgayThucHienXN);
                dto.NgayTraKetQua = ToPortalDate(dteNgayTraKQ);
                dto.MaDonViXetNghiem = GetSelectedMa(cboDonViXN);

                // ---- Cơ sở điều trị + người báo cáo ----
                dto.MaCoSoDieuTri = !string.IsNullOrEmpty(Config.EcdsConfigCFG.MaCoSoDieuTri)
                    ? Config.EcdsConfigCFG.MaCoSoDieuTri : Config.EcdsConfigCFG.MaDonVi;
                dto.HoTenNguoiBaoCao = txtNguoiBaoCao.Text;
                dto.SoDienThoaiNguoiBaoCao = txtDienThoaiBaoCao.Text;
                dto.EmailNguoiBaoCao = txtEmailBaoCao.Text;
                dto.MaDonViNguoiBaoCao = Config.EcdsConfigCFG.MaDonVi;

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

        private string ToPortalDate(DevExpress.XtraEditors.DateEdit dte)
        {
            return DiseaseCaseMapper.ToPortalDate(GetDateLong(dte));
        }

        private int? ToNullableInt(long? value)
        {
            return value.HasValue ? (int?)value.Value : null;
        }

        /// <summary>
        /// Lấy MÃ cổng đang chọn của combo bind từ danh mục cổng (ValueMember = id, DataSource là List&lt;DanhMucItemDto&gt;).
        /// VD cboCapDoBenh, cboThon, cboDonViXN. Null nếu chưa chọn.
        /// </summary>
        private string GetSelectedMa(LookUpEdit cbo)
        {
            try
            {
                long? id = GetLookupLong(cbo);
                if (!id.HasValue || cbo == null) return null;
                var list = cbo.Properties.DataSource as System.Collections.Generic.IEnumerable<DanhMucItemDto>;
                if (list == null) return null;
                var item = list.FirstOrDefault(o => o != null && o.id == id.Value);
                return item != null ? item.ma : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Đối chiếu MÃ nội bộ (SDA/HIS) -> MÃ cổng (danh mục tĩnh). Trả null nếu không đối chiếu được
        /// -> trường optional bị bỏ khỏi payload (tránh gửi mã sai khiến cổng từ chối cả ca).
        /// </summary>
        private string ResolveEcdsMa(string danhMuc, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || catalogCache == null) return null;
                return catalogCache.FindMaByMa(catalogCache.GetStatic(danhMuc), code);
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
