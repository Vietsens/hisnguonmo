# Kê Đơn Cận Lâm Sàng (AssignPrescriptionCLS) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionCLS |
| Loại | Form (`frmAssignPrescription` kế thừa `FormBase`) |
| Mục đích | Kê đơn thuốc/vật tư phục vụ chỉ định cận lâm sàng |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ — Chẩn đoán (Việc 2.6)

Chẩn đoán **chính** và **phụ** dùng control tùy chỉnh (không qua UC), nên xử lý 2.6 trực tiếp trong plugin (KHÔNG áp dụng cho YHCT — `IS_TRADITIONAL`):

- **Cảnh báo không khuyến khích bệnh chính** (`IS_NOT_RECOMMEND_MAIN = 1`): chỉ cảnh báo khi user **chọn/sửa** chẩn đoán chính (`ChangecboChanDoanTD`, `LoadIcdCombo`). Hiển thị "Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?". Chọn Không → xóa, chọn lại. Không cảnh báo khi hiển thị dữ liệu đã lưu.
- **Loại bỏ chẩn đoán nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`) khỏi bệnh chính và bệnh phụ ở MỌI đường vào:
  - Danh sách chọn (dropdown `cboIcds`, popup `frmSecondaryIcd`): không hiển thị.
  - Gõ tay/chọn (`LoadIcdCombo`, `ChangecboChanDoanTD` cho chính; `CheckIcdWrongCode` cho phụ): báo "Bệnh {0} là nguyên nhân tử vong, không được dùng làm chẩn đoán chính/phụ." + loại khỏi ô.
  - Load hồ sơ đã lưu (`LoadIcdToControl` cho chính; `LoadDataToIcdSub`/`LoadIcdToControlIcdSub` qua helper `RemoveDeathCauseFromSubIcd` cho phụ): bỏ qua, không đổ vào ô.
- **Không có kiểm tra khi lưu (B)**: plugin này KHÔNG có luồng kết thúc điều trị (đã comment), nên không áp dụng kiểm tra death-cause khi lưu — việc chặn ở các đường nhập/load nêu trên là cơ chế duy nhất.

## 3. EFMODEL
HIS_ICD (`IS_DEATH_CAUSE_ONLY`, `IS_NOT_RECOMMEND_MAIN`, `IS_TRADITIONAL`), HIS_EXP_MEST, HIS_SERVICE_REQ.

## 4. Files chính (Việc 2.6)
- `AssignPrescription/frmAssignPrescription__InitUC.cs` — nạp `currentIcds`.
- `AssignPrescription/frmAssignPrescription__InitUCIcd.cs` — bind combo bệnh chính (lọc death-cause).
- `AssignPrescription/frmAssignPrescription.cs` — `ChangecboChanDoanTD`/`LoadIcdCombo` (cảnh báo A1), `frmSecondaryIcd` (lọc death-cause A2).
- `Resources/ResourceMessage.cs` + `Message.Lang.vi/en.resx` — message `BenhKhongKhuyenKhichDungLamBenhChinh`.

## 5. Changelog

| Ngày | Người sửa | Mô tả |
|------|-----------|-------|
| 29/07/2026 | nampp | **MIMS Drug Pregnancy/Lactation** — Truyền `PatientProfile` (PN mang thai / cho con bú) vào request MIMS khi lưu đơn (`CheckMIMS` — `__Save.cs`) và menu chuột phải "Đánh giá thông tin thuốc". Config mới `HIS.Desktop.Mims.IsCheckPregnancyLactation` (mặc định TẮT — request MIMS không đổi). BN nữ: prefetch async `HIS_MIMS_PATIENT_PROFILE` khi Load (cạnh `LoadAllergenic`); có tick → build profile (Gender F, Age từ `TDL_PATIENT_DOB`, Pregnancy.Month, Nursing) truyền `CheckAndAlert(..., patientProfile:)` — cảnh báo thai kỳ/cho con bú trả về trong CÙNG request/popup. Files: `Config/HisConfigCFG.cs`, `AssignPrescription/frmAssignPrescription__MimsPatientProfile.cs` (MỚI), `frmAssignPrescription.cs`, `__Save.cs`, `__InitMenuMouseRight.cs`; csproj fix HintPath `HIS.Desktop.MIMS.Integration` về `lib\HIS\...` (trước trỏ `IVT TEST\histest` không tồn tại). |
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** (dùng chung ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`, ngưỡng trống coi như 0); (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí (hồ sơ + đang kê) vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY` theo hiệu lực FROM_TIME/TO_TIME) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp (chỉ log). Mặc định 2 key tắt — không đổi hành vi hiện tại. |
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi danh sách chọn bệnh chính + phụ (giữ giá trị đã lưu, trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa bệnh chính; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi/en). Không có kiểm tra khi lưu (plugin không có kết thúc điều trị). |
| 17/06/2026 | huyvu20 | **Việc 2.6 (bổ sung)**: `IS_DEATH_CAUSE_ONLY` vẫn lọt qua khi gõ tay & khi load hồ sơ đã lưu. Chặn nốt: gõ/chọn bệnh chính (`LoadIcdCombo`, `ChangecboChanDoanTD`) + gõ bệnh phụ (`CheckIcdWrongCode`) → báo + loại; load bệnh chính (`LoadIcdToControl`) + phụ (`LoadDataToIcdSub`, `LoadIcdToControlIcdSub` qua helper `RemoveDeathCauseFromSubIcd`) → bỏ qua không load. Thêm message `BenhLaNguyenNhanTuVongKhongDuocDungLamChanDoan` (vi/en). |

## 6. Test Cases
- [ ] Combo bệnh chính/phụ KHÔNG hiển thị ICD nguyên nhân tử vong.
- [ ] Gõ tay/chọn ICD nguyên nhân tử vong vào bệnh chính → báo + xóa, không nhận.
- [ ] Gõ tay ICD nguyên nhân tử vong vào bệnh phụ → báo + loại mã đó.
- [ ] Mở hồ sơ đã lưu có ICD nguyên nhân tử vong ở chính/phụ → mã đó bị bỏ khỏi ô (không load).
- [ ] Chọn/gõ ICD chính có cờ `IS_NOT_RECOMMEND_MAIN` → cảnh báo; chọn Không → xóa. Mở hồ sơ đã lưu → KHÔNG cảnh báo.
- [ ] YHCT không bị ảnh hưởng.
