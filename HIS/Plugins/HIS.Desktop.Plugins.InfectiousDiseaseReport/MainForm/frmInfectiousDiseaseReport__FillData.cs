/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * FillDataFromHis: đổ dữ liệu điều trị/bệnh nhân từ HIS vào header + các tab.
 * Các trường hành chính chi tiết (dân tộc, nghề nghiệp, CCCD, địa chỉ mã GSO)
 * sẽ lấy từ V_HIS_PATIENT / view V_HIS_ECDS_DISEASE_CASE — bổ sung khi có API.
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport
    {
        private void FillDataFromHis()
        {
            try
            {
                if (treatment == null) return;

                LoadFullTreatment();   // nạp đầy đủ V_HIS_TREATMENT (ICD phụ, kết thúc điều trị, tử vong...)
                FillHeader();
                FillNguoiBaoCaoTab();  // nhãn mã đơn vị/cơ sở + người báo cáo mặc định

                // §20b GetFull TRƯỚC: nếu đã có ca lưu -> map từ đó; nếu chưa -> lấy từ hồ sơ HIS.
                // Bọc try RIÊNG: MOS.SDO/EFMODEL môi trường chạy có thể lệch phiên bản (thiếu/khác type
                // HisEcdsDiseaseCaseFullSDO) -> TypeLoadException; chỉ bỏ qua đối soát, KHÔNG vỡ form.
                bool mappedFromSaved = false;
                try
                {
                    mappedFromSaved = TryLoadAndMapSavedCase();
                }
                catch (Exception exFull)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exFull);
                }

                if (mappedFromSaved)
                {
                    FillHanhChinhTab();   // Hành chính vẫn lấy từ V_HIS_PATIENT (dữ liệu gốc BN)
                }
                else
                {
                    FillCaBenhTab();
                    FillHanhChinhTab();
                }
                EnsureRequiredDefaults();   // các trường cổng BẮT BUỘC còn trống -> đặt mặc định
                UpdatePushStatusLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nạp ca đã lưu (GetFull) rồi map vào form. Trả true nếu có ca lưu để map.
        /// TÁCH RIÊNG để cô lập tham chiếu <c>MOS.SDO.HisEcdsDiseaseCaseFullSDO</c>: nếu MOS.SDO/EFMODEL
        /// đang chạy lệch phiên bản, TypeLoadException ném khi JIT method này và được FillDataFromHis
        /// bắt (form vẫn load, chỉ mất bước đối soát trạng thái đẩy).
        /// </summary>
        private bool TryLoadAndMapSavedCase()
        {
            var full = LoadEcdsCaseFull();
            if (full != null && full.DiseaseCase != null)
            {
                MapFromSavedCase(full.DiseaseCase);   // Ca bệnh + Triệu chứng + Người báo cáo từ ca đã lưu
                return true;
            }
            return false;
        }

        /// <summary>
        /// Đặt mặc định cho các trường CỔNG BẮT BUỘC còn trống (chỉ set khi null — KHÔNG đè lựa chọn của user/ca đã lưu):
        /// - Hình thức điều trị: theo DIỆN ĐIỀU TRỊ hồ sơ (TDL_TREATMENT_TYPE_ID nội trú -> "1", còn lại -> "2").
        /// - Vắc xin / Lấy mẫu XN: mặc định "Không rõ" / "Không".
        /// </summary>
        private void EnsureRequiredDefaults()
        {
            try
            {
                if (cboHinhThucDieuTri.EditValue == null)
                {
                    var t = vTreatment;
                    bool noiTru = t != null && t.TDL_TREATMENT_TYPE_ID.HasValue
                        && t.TDL_TREATMENT_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                    cboHinhThucDieuTri.EditValue = noiTru ? 1L : 2L;
                }
                if (cboSuDungVacXin.EditValue == null)
                    cboSuDungVacXin.EditValue = (long)EcdsSuDungVacXin.KhongRo;   // Không rõ
                if (cboLayMau.EditValue == null)
                    cboLayMau.EditValue = (long)EcdsLayMauXetNghiem.Khong;        // Không lấy mẫu
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillHeader()
        {
            try
            {
                lblTreatmentCodeVal.Text = treatment.TREATMENT_CODE ?? "";
                lblPatientNameVal.Text = treatment.TDL_PATIENT_NAME ?? "";
                lblDobVal.Text = treatment.TDL_PATIENT_DOB > 0
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB)
                    : "";
                lblGenderVal.Text = treatment.TDL_PATIENT_GENDER_NAME ?? "";
                lblIcdVal.Text = (treatment.ICD_CODE ?? "") +
                    (string.IsNullOrEmpty(treatment.ICD_NAME) ? "" : " — " + treatment.ICD_NAME);

                var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == treatment.LAST_DEPARTMENT_ID);
                lblDepartmentVal.Text = dept != null ? dept.DEPARTMENT_NAME : "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>V_HIS_TREATMENT đầy đủ (nạp từ api/HisTreatment/GetView) — có ICD phụ, kết thúc điều trị, tử vong.</summary>
        private V_HIS_TREATMENT vTreatment;

        /// <summary>Nạp đầy đủ điều trị theo ID (bản `treatment` truyền vào chỉ là subset).</summary>
        private void LoadFullTreatment()
        {
            try
            {
                vTreatment = null;
                if (treatment == null || treatment.ID <= 0) return;
                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisTreatmentViewFilter { ID = treatment.ID };
                var list = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(
                    "api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);
                vTreatment = (list != null && list.Count > 0) ? list[0] : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillCaBenhTab()
        {
            try
            {
                var t = vTreatment;   // view đầy đủ (có thể null -> fallback treatment)
                string icdCode = (t != null && !string.IsNullOrEmpty(t.ICD_CODE)) ? t.ICD_CODE : treatment.ICD_CODE;
                string icdName = (t != null && !string.IsNullOrEmpty(t.ICD_NAME)) ? t.ICD_NAME : treatment.ICD_NAME;
                string icdText = (t != null) ? t.ICD_TEXT : null;
                string icdPrimary = PrimaryIcdCode(icdCode);   // ICD_CODE có thể là chuỗi nhiều mã -> lấy mã chính

                // Bệnh (ICD-10): combo cổng tự chọn theo MÃ ICD CHÍNH (giữ combo)
                if (cboBenh.Properties.DataSource != null && !string.IsNullOrEmpty(icdPrimary))
                {
                    long? benhId = catalogCache.FindIdByMa(
                        catalogCache.GetStatic(Worker.EcdsCatalogCache.DM_BENH), icdPrimary);
                    if (benhId.HasValue) cboBenh.EditValue = benhId.Value;
                }

                // Phân độ bệnh: nạp danh mục cổng "phan-loai-lam-sang" theo mã ICD chính (cascade)
                LoadCapDoBenhByIcd(icdPrimary);

                SetDateLong(dteNgayNhapVien, t != null ? (long?)t.IN_TIME : (long?)treatment.IN_TIME);
                // Ngày ra viện / tử vong + Tình trạng hiện nay: xử lý trong FillCurrentStateFromTreatment.

                // Chẩn đoán từ hồ sơ
                txtChanDoanRaVien.Text = icdName ?? "";     // chẩn đoán chính (ra viện)
                txtSubDiagnosis.Text = icdText ?? "";       // chẩn đoán phụ/kèm theo (ICD_TEXT)
                // Chẩn đoán biến chứng: HIS không có trường riêng -> để trống, nhập tay khi cần.

                // Tình trạng hiện nay + ngày tử vong (theo kết thúc điều trị)
                FillCurrentStateFromTreatment(t);

                // Mặc định phân loại chẩn đoán = Xác định
                cboLoaiChanDoan.EditValue = (long)EcdsPhanLoaiChuanDoan.XacDinh;
                // Hình thức điều trị đặt trong EnsureRequiredDefaults (theo diện điều trị của hồ sơ).
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đổi bệnh (hoặc lần đầu) -> nạp lại danh sách phân độ bệnh theo ICD của bệnh đang chọn.</summary>
        private void cboBenh_EditValueChanged(object sender, EventArgs e)
        {
            try { LoadCapDoBenhByIcd(GetSelectedBenhMa()); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Mã ICD của bệnh đang chọn trên combo (fallback ICD hồ sơ nếu chưa chọn).</summary>
        private string GetSelectedBenhMa()
        {
            try
            {
                long? id = GetLookupLong(cboBenh);
                if (id.HasValue && catalogCache != null)
                {
                    var item = catalogCache.GetStatic(Worker.EcdsCatalogCache.DM_BENH)
                        .FirstOrDefault(o => o.id == id.Value);
                    if (item != null && !string.IsNullOrEmpty(item.ma)) return item.ma;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return treatment != null ? PrimaryIcdCode(treatment.ICD_CODE) : null;
        }

        /// <summary>
        /// Mã ICD CHÍNH (token đầu). V_HIS_TREATMENT.ICD_CODE có thể chứa nhiều mã ("A00, A00.0, A00.1, A00.9").
        /// Cột REPORTED_ICD_CODE (DB) tối đa 10 ký tự nên phải tách lấy mã chính, tránh ORA-12899.
        /// </summary>
        private static string PrimaryIcdCode(string icd)
        {
            try
            {
                if (string.IsNullOrEmpty(icd)) return icd;
                var parts = icd.Split(new[] { ',', ';', ' ', '/', '|' }, StringSplitOptions.RemoveEmptyEntries);
                string first = (parts.Length > 0 ? parts[0] : icd).Trim();
                if (first.Length > 10) first = first.Substring(0, 10);
                return first;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return icd; }
        }

        /// <summary>
        /// Map form từ ca bệnh ĐÃ LƯU (V_HIS_ECDS_DISEASE_CASE) — dùng khi GetFull có dữ liệu.
        /// Ca bệnh + Triệu chứng & XN + Người báo cáo. (Hành chính lấy từ V_HIS_PATIENT riêng.)
        /// </summary>
        private void MapFromSavedCase(V_HIS_ECDS_DISEASE_CASE c)
        {
            try
            {
                if (c == null) return;

                // ---- Ca bệnh ----
                string icdCode = !string.IsNullOrEmpty(c.REPORTED_ICD_CODE) ? c.REPORTED_ICD_CODE
                    : (treatment != null ? PrimaryIcdCode(treatment.ICD_CODE) : null);
                LoadCapDoBenhByIcd(icdCode);                       // nạp danh sách phân độ theo ICD trước
                SetLookupDec(cboBenh, c.REPORTED_DISEASE_ID);      // (kéo cascade phân độ qua event)
                SetLookupDec(cboCapDoBenh, c.DISEASE_SEVERITY_ID);
                SetLookupShort(cboLoaiChanDoan, c.DIAGNOSIS_TYPE);
                SetLookupDec(cboTinhTrang, c.CURRENT_STATE);
                txtTinhTrangKhac.Text = c.OTHER_STATE_DESC ?? "";
                SetDateLong(dteNgayKhoiPhat, c.ONSET_DATE);
                SetDateLong(dteNgayNhapVien, c.TREATMENT_IN_TIME);
                SetDateLong(dteNgayRaVien, c.TREATMENT_OUT_TIME);
                SetDateLong(dteNgayTuVong, c.DEATH_DATE);
                SetLookupDec(cboTinhTrangRaVien, c.DISCHARGE_STATE);   // Tình trạng ra viện (TINHTRANGRAVIEN)
                SetLookupDec(cboBenhVienChuyenToi, c.TRANSFER_HOSPITAL_ID);
                txtChanDoanRaVien.Text = c.DISCHARGE_DIAGNOSIS ?? "";
                txtSubDiagnosis.Text = c.SUB_DIAGNOSIS ?? "";
                txtComplication.Text = c.COMPLICATION ?? "";
                txtGhiChu.Text = c.GENERAL_NOTE ?? "";

                // ---- Triệu chứng & XN ----
                txtTienSuDichTe.Text = c.EPIDEMIOLOGY_HISTORY ?? "";
                SetLookupShort(cboSuDungVacXin, c.VACCINE_USE);
                if (c.VACCINE_USE_COUNT.HasValue) spnSoLan.EditValue = c.VACCINE_USE_COUNT.Value;
                SetLookupShort(cboLayMau, c.IS_SPECIMEN_TAKEN);
                SetLookupDec(cboLoaiXN, c.TEST_TYPE);
                txtLoaiXNKhac.Text = c.OTHER_TEST_NAME ?? "";
                SetLookupDec(cboKetQuaXN, c.TEST_RESULT);
                SetDateLong(dteNgayThucHienXN, c.TEST_TIME);
                SetDateLong(dteNgayTraKQ, c.RESULT_TIME);
                SetLookupDec(cboDonViXN, c.TEST_FACILITY_ID);
                SetLookupDec(cboLoaiPhatHien, c.DETECTION_FACILITY_TYPE);

                // Mang thai (Hành chính) — lấy theo ca đã lưu
                chkMangThai.Checked = c.IS_PREGNANT.HasValue && c.IS_PREGNANT.Value == (int)EcdsMangThai.Co;

                // ---- Người báo cáo ----
                if (!string.IsNullOrEmpty(c.REPORTER_NAME)) txtNguoiBaoCao.Text = c.REPORTER_NAME;
                if (!string.IsNullOrEmpty(c.REPORTER_PHONE)) txtDienThoaiBaoCao.Text = c.REPORTER_PHONE;
                if (!string.IsNullOrEmpty(c.REPORTER_EMAIL)) txtEmailBaoCao.Text = c.REPORTER_EMAIL;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetLookupDec(LookUpEdit cbo, decimal? v)
        {
            try { if (v.HasValue) cbo.EditValue = (long)v.Value; }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void SetLookupShort(LookUpEdit cbo, short? v)
        {
            try { if (v.HasValue) cbo.EditValue = (long)v.Value; }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nạp phân độ/phân loại lâm sàng theo mã ICD (danh mục cổng, cascade theo bệnh).</summary>
        private void LoadCapDoBenhByIcd(string icdCode)
        {
            try
            {
                if (string.IsNullOrEmpty(icdCode) || catalogCache == null || !Config.EcdsConfigCFG.IsValid()) return;
                var list = catalogCache.GetCascade(
                    Worker.EcdsCatalogCache.DM_CAPDOBENH,
                    new SearchDanhMucFastDto { maIcd10Benh = icdCode },
                    icdCode);
                SetupLookup(cboCapDoBenh, list, "id", "ten");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tình trạng hiện nay theo hồ sơ:
        /// - Có TREATMENT_END_TYPE_ID: RaVien(6)→2, Chet(1)→3, Chuyen(2)→4, khác→5(Khác).
        /// - Không có: theo TDL_TREATMENT_TYPE_ID: nội trú(3)→1, còn lại→0(ngoại trú).
        /// Ngày ra viện = OUT_DATE; Ngày tử vong = DEATH_TIME.
        /// </summary>
        private void FillCurrentStateFromTreatment(V_HIS_TREATMENT t)
        {
            try
            {
                if (t == null) return;

                long tinhTrang;
                long? endType = t.TREATMENT_END_TYPE_ID;
                if (endType.HasValue && endType.Value > 0)
                {
                    if (endType.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN)        // 6 ra viện
                        tinhTrang = (long)EcdsTinhTrangHienNay.RaVien;         // 2
                    else if (endType.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)     // 1 tử vong
                        tinhTrang = (long)EcdsTinhTrangHienNay.TuVong;         // 3
                    else if (endType.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)   // 2 chuyển viện
                        tinhTrang = (long)EcdsTinhTrangHienNay.ChuyenVien;     // 4
                    else
                        tinhTrang = (long)EcdsTinhTrangHienNay.Khac;           // 5 khác
                }
                else
                {
                    // Chưa kết thúc: theo loại điều trị hiện tại.
                    bool noiTru = t.TDL_TREATMENT_TYPE_ID.HasValue
                        && t.TDL_TREATMENT_TYPE_ID.Value == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU; // 3
                    tinhTrang = noiTru ? (long)EcdsTinhTrangHienNay.NoiTru      // 1
                                       : (long)EcdsTinhTrangHienNay.NgoaiTru;   // 0
                }
                cboTinhTrang.EditValue = tinhTrang;

                // Tình trạng ra viện (TINHTRANGRAVIEN): map trực tiếp mã HIS_TREATMENT_END_TYPE nếu có.
                if (endType.HasValue && endType.Value > 0)
                    cboTinhTrangRaVien.EditValue = endType.Value;

                SetDateLong(dteNgayRaVien, t.OUT_DATE);      // ngày ra viện
                SetDateLong(dteNgayTuVong, t.DEATH_TIME);    // ngày tử vong (đổi sang ngày)
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Đổi xã hiện nay -> nạp lại danh sách thôn theo xã đang chọn (danh mục cổng "thon").</summary>
        private void cboXa_EditValueChanged(object sender, EventArgs e)
        {
            try { LoadThonByXa(GetLookupString(cboXa)); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Nạp danh mục thôn/ấp theo mã xã (danh mục cổng, cascade theo xã). Best-effort.</summary>
        private void LoadThonByXa(string xaCode)
        {
            try
            {
                if (string.IsNullOrEmpty(xaCode) || catalogCache == null || !Config.EcdsConfigCFG.IsValid())
                {
                    SetupLookup(cboThon, new System.Collections.Generic.List<DanhMucItemDto>(), "id", "ten");
                    return;
                }
                var list = catalogCache.GetCascade(
                    Worker.EcdsCatalogCache.DM_THON,
                    new SearchDanhMucFastDto { maXa = xaCode },
                    xaCode);
                SetupLookup(cboThon, list, "id", "ten");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillHanhChinhTab()
        {
            try
            {
                txtHoTen.Text = treatment.TDL_PATIENT_NAME ?? "";
                SetDateLong(dteNgaySinh, treatment.TDL_PATIENT_DOB);

                // Tuổi
                if (treatment.TDL_PATIENT_DOB > 0)
                {
                    DateTime? dob = Inventec.Common.DateTime.Convert
                        .TimeNumberToSystemDateTime(treatment.TDL_PATIENT_DOB);
                    if (dob.HasValue)
                        spnTuoi.EditValue = (decimal)Math.Max(0, DateTime.Now.Year - dob.Value.Year);
                }

                // Giới tính: HIS_GENDER.ID -> EcdsGioiTinh
                bool isMale = treatment.TDL_PATIENT_GENDER_ID
                    == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE;
                cboGioiTinh.EditValue = isMale ? (long)EcdsGioiTinh.Nam : (long)EcdsGioiTinh.Nu;

                // Nạp V_HIS_PATIENT để điền CCCD/điện thoại/dân tộc/nghề nghiệp/địa chỉ (hiện nay + thường trú).
                FillPatientAdminFields();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Lấy thông tin hành chính bệnh nhân từ V_HIS_PATIENT (theo PATIENT_ID) và điền vào tab Hành chính.
        /// Combo (dân tộc/nghề/tỉnh/xã) là danh mục SDA — chọn theo MÃ trong V_HIS_PATIENT.
        /// Quy ước địa bàn: HT_* = hiện nay, không tiền tố = thường trú.
        /// </summary>
        private void FillPatientAdminFields()
        {
            try
            {
                if (treatment == null || treatment.PATIENT_ID <= 0) return;

                CommonParam param = new CommonParam();
                var filter = new MOS.Filter.HisPatientViewFilter { ID = treatment.PATIENT_ID };
                var list = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(
                    HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);

                var p = (list != null && list.Count > 0) ? list[0] : null;
                if (p == null) return;

                // ---- Text ----
                txtCccd.Text = !string.IsNullOrEmpty(p.CCCD_NUMBER) ? p.CCCD_NUMBER : (p.CMND_NUMBER ?? "");
                txtDienThoai.Text = p.PHONE ?? "";
                txtNoiLamViec.Text = !string.IsNullOrEmpty(p.WORK_PLACE_NAME) ? p.WORK_PLACE_NAME : (p.WORK_PLACE ?? "");

                // ---- Danh mục SDA: chọn theo mã ----
                SetLookupStr(cboDanToc, p.ETHNIC_CODE);
                // Nghề nghiệp: combo bind danh mục CỔNG -> tự chọn item cổng khớp TÊN nghề HIS (mã HIS khác hệ mã cổng).
                SelectPortalNgheByName(p.CAREER_CODE);

                // Hiện nay: ưu tiên HT_*; thiếu thì lấy không tiền tố.
                SetLookupStr(cboTinh, !string.IsNullOrEmpty(p.HT_PROVINCE_CODE) ? p.HT_PROVINCE_CODE : p.PROVINCE_CODE);
                SetLookupStr(cboXa, !string.IsNullOrEmpty(p.HT_COMMUNE_CODE) ? p.HT_COMMUNE_CODE : p.COMMUNE_CODE);
                // Địa chỉ hiện nay: HT_ADDRESS; nếu trống -> lấy địa chỉ thường trú (ADDRESS).
                txtDiaChi.Text = !string.IsNullOrEmpty(p.HT_ADDRESS) ? p.HT_ADDRESS
                    : (!string.IsNullOrEmpty(p.ADDRESS) ? p.ADDRESS : (p.VIR_HT_ADDRESS ?? ""));

                // Thường trú: không tiền tố.
                SetLookupStr(cboTinhTru, p.PROVINCE_CODE);
                SetLookupStr(cboXaTru, p.COMMUNE_CODE);
                txtDiaChiTru.Text = !string.IsNullOrEmpty(p.ADDRESS) ? p.ADDRESS : (p.VIR_ADDRESS ?? "");

                // Địa chỉ HIỆN NAY là BẮT BUỘC (cổng): trống -> lấy từ địa chỉ THƯỜNG TRÚ.
                if (cboTinh.EditValue == null && cboTinhTru.EditValue != null) cboTinh.EditValue = cboTinhTru.EditValue;
                if (cboXa.EditValue == null && cboXaTru.EditValue != null) cboXa.EditValue = cboXaTru.EditValue;
                if (string.IsNullOrEmpty(txtDiaChi.Text)) txtDiaChi.Text = txtDiaChiTru.Text;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tự chọn nghề nghiệp trên combo (đã bind danh mục CỔNG) theo TÊN nghề HIS (HIS_CAREER.CAREER_NAME):
        /// khớp `ten` cổng (bằng chính xác trước, rồi khớp chứa). Không khớp -> để trống cho người dùng chọn.
        /// </summary>
        private void SelectPortalNgheByName(string careerCode)
        {
            try
            {
                if (string.IsNullOrEmpty(careerCode) || catalogCache == null) return;
                var career = BackendDataWorker.Get<HIS_CAREER>()
                    .FirstOrDefault(o => o.CAREER_CODE == careerCode);
                string name = career != null ? (career.CAREER_NAME ?? "").Trim() : "";
                if (string.IsNullOrEmpty(name)) return;

                var list = catalogCache.GetStatic(Worker.EcdsCatalogCache.DM_NGHENGHIEP);
                if (list == null) return;
                var item = list.FirstOrDefault(o => o != null && !string.IsNullOrEmpty(o.ten)
                                && string.Equals(o.ten.Trim(), name, StringComparison.OrdinalIgnoreCase))
                         ?? list.FirstOrDefault(o => o != null && !string.IsNullOrEmpty(o.ten)
                                && (o.ten.Trim().IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0
                                    || name.IndexOf(o.ten.Trim(), StringComparison.OrdinalIgnoreCase) >= 0));
                if (item != null && !string.IsNullOrEmpty(item.ma))
                    cboNgheNghiep.EditValue = item.ma;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Chọn combo theo mã (ValueMember là mã chuỗi); rỗng thì bỏ qua.</summary>
        private void SetLookupStr(LookUpEdit cbo, string code)
        {
            try { if (cbo != null && !string.IsNullOrEmpty(code)) cbo.EditValue = code; }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void FillNguoiBaoCaoTab()
        {
            try
            {
                txtNguoiBaoCao.Text = Inventec.UC.Login.Base.ClientTokenManagerStore
                    .ClientTokenManager.GetUserName() ?? "";
                lblMaDonViVal.Text = Config.EcdsConfigCFG.MaDonVi ?? "";

                var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault();
                lblCoSoDieuTriVal.Text = branch != null ? branch.BRANCH_NAME : "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void UpdatePushStatusLabel()
        {
            try
            {
                if (!string.IsNullOrEmpty(ecdsCaseCode))
                    lblPushStatus.Text = "✔ Đã đẩy — Mã ca bệnh: " + ecdsCaseCode;
                else
                    lblPushStatus.Text = "● Chưa đẩy lên cổng";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
    }
}
