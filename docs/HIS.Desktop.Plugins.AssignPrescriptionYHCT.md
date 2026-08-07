# AssignPrescriptionYHCT — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionYHCT |
| Loại | Form (FormBase) |
| Mục đích | Kê đơn thuốc / vật tư Y học cổ truyền (YHCT). Hỗ trợ thang thuốc, ngày dùng, ICD YHCT chính/phụ, in đơn, tích hợp tờ điều trị (tracking). |
| Trạng thái | Đang bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form từ phòng khám YHCT / buồng → load thông tin điều trị (HIS_TREATMENT) và bệnh nhân.
2. Chọn vị thuốc / vật tư YHCT, số lượng, số thang, hướng dẫn sử dụng, thời điểm chỉ định.
3. (Tuỳ cấu hình) chọn tờ điều trị nguồn (HIS_TRACKING) cho đơn.
4. Validate ICD chính + ICD YHCT, tương tác hoạt chất, hạn dùng, kho, BHYT, …
5. Lưu → tạo HIS_SERVICE_REQ + HIS_EXP_MEST_MEDICINE / HIS_EXP_MEST_MATERIAL; in đơn nếu cần.

### Cấu hình `MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED`

Tham chiếu mô tả các option trong `HIS.Desktop.Plugins.AssignPrescriptionPK.md`. Plugin YHCT áp dụng option = 4 (đơn YHCT là một trong các loại đơn nằm trong phạm vi: phòng khám / tủ trực / điều trị / CLS / **YHCT**).

### Hành vi option = 4 chi tiết
- Điều kiện áp dụng: `TrackingRequiredOption == 4` AND (BN nội trú `Histreatment.IN_TREATMENT_TYPE_ID == HIS_TREATMENT_TYPE.ID__DTNOITRU` OR cấp cứu `Histreatment.IS_EMERGENCY == 1`).
- **Khi load form** (`ApplyTrackingRequiredOption4` — `frmAssignPrescription__Check.cs`):
  - Hiển thị `cboPhieuDieuTri` (set `lciPhieuDieuTri.Visibility = Always`) qua nhánh đặc biệt trong `LoadDataTracking`.
  - Caption `lciPhieuDieuTri` Maroon — đánh dấu trường quan trọng, KHÔNG validate cứng.
  - Nếu `trackingADOs` rỗng → cảnh báo `BenhNhanChuaCoToDieuTri_KeDonVTMaKhongKeThuoc` (Information, không chặn).
- **Khi lưu** (`CheckTrackingRequiredOption4` — gọi trong `ProcessSaveData` của `frmAssignPrescription__Save.cs`):
  - Nếu `cboPhieuDieuTri.EditValue != null` → cho lưu.
  - Nếu chưa chọn + `mediMatyTypeADOs` có ít nhất 1 thuốc → chặn lưu + cảnh báo `KhongChoPhepKeDonCoThuocKhiChuaChonToDieuTri`, focus combo.
  - Nếu chỉ vật tư → cho lưu bình thường.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT / V_HIS_TREATMENT | Table/View | Thông tin điều trị, cờ `IS_EMERGENCY`, `IN_TREATMENT_TYPE_ID` |
| HIS_TRACKING | Table | Tờ điều trị nguồn cho `cboPhieuDieuTri` |
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ kê đơn YHCT |
| HIS_EXP_MEST / HIS_EXP_MEST_MEDICINE / HIS_EXP_MEST_MATERIAL | Table | Phiếu xuất kho đơn |
| HIS_SERVICE_TYPE | Table | Phân biệt thuốc (`ID__THUOC`) / vật tư (`ID__VT`) |

## 4. UI Layout

### Các control chính
- `cboPhieuDieuTri` (GridLookUpEdit) + `lciPhieuDieuTri` — tờ điều trị nguồn.
- `gridViewServiceProcess` — DataSource `List<MediMatyTypeADO>` (field `mediMatyTypeADOs`).
- `ucIcd`, `ucSecondaryIcd`, `ucIcdCause`, `ucIcdYHCT`, `ucSecondaryIcdYHCT` — ICD đa nguồn (Tây y + YHCT).
- `ucDate` (HIS.UC.DateEditor) — thời điểm chỉ định.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.Icd / HIS.UC.SecondaryIcd | ICD chính / phụ (Tây y) + YHCT |
| HIS.UC.DateEditor | Chọn thời điểm chỉ định |
| HIS.UC.TreatmentFinish | Kết thúc khám (ngoại trú) |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu kê đơn (mới/sửa) | URI trong `RequestUriStore` của plugin | MosConsumer |
| Lấy tờ điều trị | `api/HisTracking/Get` | MosConsumer |
| Lấy điều trị | `api/HisTreatment/Get` | MosConsumer |
| BHYT applied type | `HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED` | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| PrintPrescription | In đơn thuốc YHCT |
| CheckIcd | Validate ICD |
| CheckHeinGOV | Validate thẻ BHYT |
| ConnectWhoCnd | Đồng bộ bệnh không lây nhiễm WHO |

## 7. Print

| Loại in | PrintTypeCode | Library |
|---------|--------------|---------|
| Đơn thuốc YHCT | Mps000118 / Mps000102 (mẫu YHCT riêng) | PrintPrescription |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/07/2026 | nampp | **MIMS Drug Pregnancy/Lactation** — Truyền `PatientProfile` (PN mang thai / cho con bú) vào request MIMS khi lưu đơn (`CheckMIMS` — `__Save.cs`) và menu chuột phải "Đánh giá thông tin thuốc". Config mới `HIS.Desktop.Mims.IsCheckPregnancyLactation` (mặc định TẮT). BN nữ: prefetch async `HIS_MIMS_PATIENT_PROFILE` khi Load (cạnh `Task.Run(LoadAllergenic)`); có tick → build profile truyền `CheckAndAlert(..., patientProfile:)`. Files: `Config/HisConfigCFG.cs`, `AssignPrescription/frmAssignPrescription__MimsPatientProfile.cs` (MỚI), `frmAssignPrescription.cs`, `__Save.cs`, `__InitMenuMouseRight.cs`; csproj fix HintPath `HIS.Desktop.MIMS.Integration` về `lib\HIS\...` (trước trỏ `HISTEST\histest` không tồn tại). |
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** (dùng chung ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`, ngưỡng trống coi như 0); (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí (hồ sơ + đang kê) vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY` theo hiệu lực FROM_TIME/TO_TIME) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp (chỉ log). Mặc định 2 key tắt — không đổi hành vi hiện tại. |
| 2026-05-15 | Trần Hải Đăng | Task 2609 — Thêm option `IS_TRACKING_REQUIRED = 4`: bắt buộc nhập tờ điều trị khi kê đơn thuốc cho BN nội trú/cấp cứu, đơn chỉ vật tư không bắt buộc. Thêm `EnumAssignPrescription.TRACKING_REQUIRED_OPTION`, `HisConfigCFG.TrackingRequiredOption`, `ApplyTrackingRequiredOption4()` + `CheckTrackingRequiredOption4()` trong `frmAssignPrescription__Check.cs`. Cập nhật `LoadDataTracking` hiển thị `cboPhieuDieuTri` khi option = 4 + BN nội trú/cấp cứu. Hook `CheckTrackingRequiredOption4` vào `ProcessSaveData`. Thêm message `BenhNhanChuaCoToDieuTri_KeDonVTMaKhongKeThuoc` + `KhongChoPhepKeDonCoThuocKhiChuaChonToDieuTri` (vi/en). |
| 2026-07-18 | huannh | Fix lỗi lọc kho trong `InitComboMediStockAllow` (`frmAssignPrescription__InitCombo.cs`): thiếu ngoặc ở điều kiện `IS_ACTIVE` khiến `&&` ưu tiên hơn `\|\|`, mọi kho có `IS_ACTIVE == null` (kể cả Kho Máu) lọt qua bất kể loại kho, và tick "Kho YHCT" không có tác dụng. Bọc ngoặc `(IS_ACTIVE == null \|\| IS_ACTIVE == 1)` ở cả 2 nhánh checked/unchecked để điều kiện loại kho (`IS_TRADITIONAL_MEDICINE`) áp dụng đúng. |
| 2026-07-18 | huannh | Loại hẳn kho máu khỏi combo chọn kho: thêm lọc `IS_BLOOD = 1` (loại) vào đầu `FilterMestRoomByIsCabinet` (`frmAssignPrescription__InitCombo.cs`), theo đúng pattern plugin `AssignPrescriptionPK`. Áp dụng cho cả trường hợp tick/bỏ tick "Kho YHCT". |
| 2026-07-18 | huannh | Fix checkbox "Kho YHCT" tick/bỏ tick không load lại danh sách kho: event `chkYhct.CheckedChanged` chưa được đăng ký ở đâu (handler `chkYhct_CheckedChanged` có sẵn nhưng không bao giờ chạy). Gắn event bằng code ở cuối `frmAssignPrescription_Load` (sau khi load lần đầu hoàn tất) để tránh fire trong lúc khởi tạo. |
## 9. Test Cases

### Option = 4, BN nội trú, chưa có tờ điều trị
- [ ] Mở form → hiện cảnh báo "Bệnh nhân chưa có tờ điều trị…", caption "Tờ điều trị" Maroon, KHÔNG chặn form.
- [ ] Kê chỉ vật tư → Lưu thành công.
- [ ] Kê có thuốc YHCT → Bấm Lưu → cảnh báo "Không cho phép kê đơn có thuốc…", chặn lưu, focus combo.

### Option = 4, BN cấp cứu, đã có tờ điều trị
- [ ] Combo có dữ liệu, không hiển thị cảnh báo khi load.
- [ ] Chọn tờ điều trị → kê có thuốc → Lưu thành công.
- [ ] Không chọn tờ điều trị → kê có thuốc → chặn lưu.
- [ ] Không chọn tờ điều trị → kê chỉ vật tư → Lưu thành công.

### Option = 4, BN ngoại trú
- [ ] Logic option 4 KHÔNG áp dụng — form hoạt động như cấu hình mặc định.

### Option ≠ 4
- [ ] Hành vi với option `0/1/2/3` không thay đổi.
