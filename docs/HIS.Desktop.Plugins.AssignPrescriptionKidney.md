# Kê Đơn Thận Nhân Tạo — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionKidney |
| Loại | Form (frmAssignPrescription) |
| Mục đích | Kê đơn thuốc/vật tư cho bệnh nhân chạy thận nhân tạo. Cùng họ với các plugin kê đơn PK/CLS/YHCT nhưng xử lý riêng nghiệp vụ thận (chỉ số EGFR, thuốc chạy thận, dịch vụ TEIN). |
| Người tạo | (kế thừa codebase) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Bác sĩ mở màn kê đơn từ ngữ cảnh điều trị thận nhân tạo → chọn thuốc/vật tư trong kho → nhập hướng dẫn sử dụng → Lưu/Lưu In → tạo phiếu xuất (HIS_EXP_MEST).

### Cảnh báo viện phí (việc 46465)
- **Vượt tạm ứng:** khi mở màn kê đơn (thêm mới), hàm `CheckWarningOverTotalPatientPrice()` so "Phải thu" (từ `V_HIS_TREATMENT_FEE`) với ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`:
  - BN **nội trú**: bật bằng key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheck` = 1 (có sẵn).
  - BN **điều trị ngoại trú**: bật bằng key mới `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1; bỏ qua BN bảo lãnh.
  - Popup "Bệnh nhân đang thiếu viện phí…" — Không → đóng form.
- **Vượt 15% lương cơ bản (BN diện khám):** khi Lưu đơn, hàm `ValidFee15PercentBaseSalaryForExam()` (key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1) so tổng chi phí (hồ sơ + đơn đang kê) với 15% × `HIS_BHYT_PARAM.BASE_SALARY` hiệu lực tại thời điểm y lệnh. Vượt → popup xác nhận; Không → không lưu. Bỏ qua BN bảo lãnh; thiếu Tham số BHYT/lỗi check → cho lưu (chỉ log).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_FEE | View | Viện phí hồ sơ (phải thu, tổng chi phí) |
| HIS_BHYT_PARAM | Table (cache) | Lương cơ bản cho ngưỡng 15% |
| HIS_EXP_MEST / HIS_EXP_MEST_MEDICINE | Table | Phiếu xuất/chi tiết đơn thuốc |
| HisTreatmentWithPatientTypeInfoSDO | SDO | Thông tin điều trị + đối tượng (GUARANTEE_CODE) |

## 4. UI Layout

Form kê đơn chuẩn họ AssignPrescription: grid thuốc/vật tư (`gridViewServiceProcess`), panel viện phí, khu hướng dẫn sử dụng, nút Lưu/Lưu In.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Viện phí hồ sơ | `HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW` | MosConsumer |

## 6. Dependencies

Cùng bộ thư viện với các plugin kê đơn khác (PrintPrescription, BackendDataWorker cache, LibraryMessage).

## 7. Print

In đơn thuốc qua `HIS.Desktop.Plugins.Library.PrintPrescription`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** trong `CheckWarningOverTotalPatientPrice()`; (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY`) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp. Mặc định 2 key tắt. |
| (trước 2026) | team | Tạo plugin kê đơn thận nhân tạo (kế thừa họ AssignPrescription). |

## 9. Test Cases

- [ ] 2 config tắt: mở màn/kê đơn chạy như bản cũ, cảnh báo nội trú theo key cũ vẫn đúng.
- [ ] Config ngoại trú bật, BN ngoại trú thiếu viện phí: popup khi mở màn; Không → đóng form.
- [ ] Config 15% bật, BN khám tổng chi phí vượt 15% lương cơ bản: popup khi Lưu; Không → không lưu, Có → lưu.
- [ ] BN bảo lãnh / không có Tham số BHYT hiệu lực: không popup, không lỗi.
