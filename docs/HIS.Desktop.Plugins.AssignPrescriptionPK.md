# Kê Đơn Phòng Khám (AssignPrescriptionPK) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionPK |
| Loại | Form (`frmAssignPrescription` kế thừa `FormBase`) |
| Mục đích | Kê đơn thuốc/vật tư tại phòng khám (ngoại trú); có thể kèm kết thúc điều trị qua UC `HIS.UC.TreatmentFinish` |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
- Chọn bệnh nhân → nhập chẩn đoán (chính/phụ/YHCT) → chọn thuốc/vật tư (kho/ngoài kho) → (tùy chọn) kết thúc điều trị → Lưu / Lưu & In.

### Chẩn đoán — quy tắc bệnh chính / nguyên nhân tử vong (Việc 2.6)
Áp dụng cho chẩn đoán **chính** và **phụ** (KHÔNG áp dụng cho chẩn đoán YHCT — `IS_TRADITIONAL`):

- **Cảnh báo không khuyến khích bệnh chính** (`IS_NOT_RECOMMEND_MAIN = 1`): chỉ cảnh báo khi user **chọn/sửa** chẩn đoán chính. Hiển thị "Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?".
  - Chọn **Có** → giữ nguyên. Chọn **Không** → xóa chẩn đoán, chọn lại.
  - KHÔNG cảnh báo khi chỉ hiển thị dữ liệu đã lưu (load hồ sơ).
- **Loại bỏ chẩn đoán nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`) khỏi bệnh chính và bệnh phụ ở MỌI đường vào:
  - Danh sách chọn (dropdown `cboIcds`, grid phụ `icdSubcodeAdoChecks`, popup `frmSecondaryIcd`): không hiển thị.
  - Gõ tay/chọn mã (`LoadIcdCombo`, `ChangecboChanDoanTD`, `CheckIcdWrongCode`): chặn + báo "Bệnh {0} là nguyên nhân tử vong, không được dùng làm chẩn đoán chính/phụ." + xóa khỏi ô.
  - Load hồ sơ đã lưu (`LoadIcdToControl` cho chính, `LoadDataToIcdSub` cho phụ): bỏ qua, không đổ vào ô. **Lưu ý:** ca tử vong có sẵn mã này sẽ bị bỏ khỏi form khi mở.
  - Ô **CĐ nguyên nhân** (Cause) KHÔNG bị ảnh hưởng — `IS_DEATH_CAUSE_ONLY` vẫn hợp lệ tại đây (nơi kiểm tra khi lưu phát huy tác dụng).

### Kiểm tra khi lưu (Việc 2.6)
- **Chỉ khi có kết thúc điều trị** (`treatUC.IsAutoTreatmentFinish = true`): kiểm tra toàn bộ chẩn đoán chính + phụ (không gồm YHCT).
- Nếu tồn tại chẩn đoán `IS_DEATH_CAUSE_ONLY = 1` nhưng kết quả **không phải tử vong** → hiển thị "Bệnh {0} là nguyên nhân tử vong không được sử dụng cho các trường hợp không phải tử vong." và **dừng lưu**.
- **Lưu ý nguồn "kết quả":** UC `HIS.UC.TreatmentFinish` chỉ thu `TREATMENT_END_TYPE_ID` (không có `TREATMENT_RESULT_ID` riêng). `HIS_TREATMENT_END_TYPE` chỉ có 1 loại tử vong `ID__CHET` — bao trùm cả "tử vong" (`HIS_TREATMENT_RESULT.ID__CHET`) lẫn "tử vong ngoại viện" (`HIS_TREATMENT_RESULT.ID__TVNV`), phân biệt qua `DeathWithinId`. Do đó `TreatmentEndTypeId == ID__CHET` tương đương "kết quả là tử vong HOẶC tử vong ngoại viện" theo spec.

### Lọc kho theo loại — cấu hình `ENABLE_TREATMENT_PRESCRIPTION` (kê đơn theo loại kho)
Cấu hình: `HIS.Desktop.Plugins.AssignPrescription.ENABLE_TREATMENT_PRESCRIPTION`.
- **BẬT (1)**: cho phép kê đơn điều trị; lọc danh sách kho theo loại tương ứng với chức năng kê đơn (điều trị → kho điều trị, tủ trực → kho tủ trực, phòng khám/ngoại trú → kho ngoại trú), dựa theo thiết lập kho xuất - phòng; không ràng buộc thanh toán, không chặn theo đối tượng.
- **TẮT / null (mặc định)**: KHÔNG lọc kho theo loại (giữ nguyên danh sách kho hiện tại); không kê đơn điều trị; luồng kê đơn giữ nguyên hoàn toàn.
- Lọc dựa trên field loại kho trên `HIS_MEDI_STOCK` / `V_HIS_MEDI_STOCK` (đã có sẵn): `IS_CABINET` (tủ trực), `IS_TREATMENT_STOCK` (điều trị), `IS_OUTPATIENT_STOCK` (ngoại trú). Xác định loại theo ngữ cảnh: `GlobalStore.IsCabinet` → tủ trực; `GlobalStore.IsTreatmentIn` → điều trị; còn lại → ngoại trú.
- Điểm chèn: method `FilterMestRoomByStockCategory()` đặt trong `frmAssignPrescription__InitCombo.cs` (cạnh các `FilterMestRoomBy*`), gọi trong `InitComboMediStockAllow()` **chỉ khi config BẬT**, sau các bước lọc kho hiện có.
- (Tùy chọn) API "lấy kho theo thiết lập kho xuất - phòng" có thể bổ sung tham số lọc loại kho phía server (API #2) để tối ưu; hiện lọc phía client.
- Áp dụng đồng bộ cho cả 3 plugin: **AssignPrescriptionPK, AssignPrescriptionYHCT, AssignPrescriptionCLS**.

### Đơn mẫu (Exp Mest Template) — lưu/đọc HDSD vật tư
- Tạo đơn mẫu (`frmHisExpMestTemplateCreate`): lưu HDSD (`TUTORIAL`) cho cả thuốc và **vật tư** (`HIS_EMTE_MATERIAL_TYPE`) — nhánh VATTU + VATTU_DM. ✅ Đã làm (bảng đã có `TUTORIAL`).
- Chọn đơn mẫu (`MediMatyTypeADO`): đã wire load lại HDSD cho vật tư — `this.TUTORIAL = inputData.TUTORIAL` (ctor `V_HIS_EMTE_MATERIAL_TYPE`, cả 3 plugin). Load đọc qua view `HIS_EMTE_MATERIAL_TYPE_GETVIEW` nên **yêu cầu EFMODEL có cột `TUTORIAL` trên view** `V_HIS_EMTE_MATERIAL_TYPE` mới build được (đang chờ cập nhật DLL EFMODEL).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_ICD | Table | Danh mục chẩn đoán (`IS_DEATH_CAUSE_ONLY`, `IS_NOT_RECOMMEND_MAIN`, `IS_TRADITIONAL`) |
| V_HIS_ICD | View | Danh mục chẩn đoán cho chọn chẩn đoán phụ (F1) |
| HIS_TREATMENT | Table | Hồ sơ điều trị, `TREATMENT_END_TYPE_ID` |
| HIS_EXP_MEST / V_HIS_EXP_MEST_MEDICINE | Table/View | Phiếu xuất + dòng thuốc kê đơn |
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ kê đơn |
| V_HIS_MEST_ROOM | View | Thiết lập kho xuất - phòng (nạp combo chọn kho) |
| V_HIS_MEDI_STOCK | View | Danh mục kho + field loại kho: `IS_CABINET`, `IS_TREATMENT_STOCK`, `IS_OUTPATIENT_STOCK` (Int16) |
| HIS_EMTE_MATERIAL_TYPE | Table | Đơn mẫu — dòng vật tư (bảng có `TUTORIAL`) |
| V_HIS_EMTE_MATERIAL_TYPE | View | Đọc đơn mẫu vật tư (view **CHƯA** có `TUTORIAL` — BE bổ sung) |
| HIS_EMTE_MEDICINE_TYPE / V_HIS_EMTE_MEDICINE_TYPE | Table/View | Đơn mẫu — dòng thuốc (có `TUTORIAL`) |

## 4. UI Layout

### UC sử dụng
| UC / Control | Vai trò |
|----|---------|
| Combo chẩn đoán chính (`cboIcds` + `txtIcdCode`/`txtIcdMainText`) | Chẩn đoán chính (control tùy chỉnh, không phải UC) |
| `txtIcdSubCode`/`txtIcdText` + popup + `frmSecondaryIcd` | Chẩn đoán phụ (control tùy chỉnh) |
| HIS.UC.Icd / HIS.UC.SecondaryIcd | Chẩn đoán YHCT (`ucIcdYhct`, `ucSecondaryIcdYhct`) |
| HIS.UC.TreatmentFinish | Kết thúc điều trị (`ucTreatmentFinish`) |
| HIS.UC.DateEditor, HIS.UC.PatientSelect, HIS.UC.PeriousExpMestList | Ngày chỉ định, chọn BN, đơn trước |

## 5. API Endpoints
Lưu đơn theo `SaveFactory` (Create/Update, In/Out) — xem `Save/`.

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| PrintPrescription / PrintBordereau / PrintTreatmentFinish / PrintTreatmentEndTypeExt | In đơn, biên lai, giấy ra viện/biểu mẫu kết thúc |
| ConnectWhoCnd | Kiểm tra bệnh không lây nhiễm khi kết thúc điều trị |

## 7. Print
In đơn thuốc, biên lai, giấy tờ kết thúc điều trị qua các Print Library nêu trên.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/08/2026 | anhnh2 | **PT-53438 Chặn hẹn khám khi bệnh án ngoại trú quá 1 năm** (áp dụng gián tiếp — sửa ở UC dùng chung `HIS.UC.TreatmentFinish`, plugin này KHÔNG đổi code). Khi xử trí "Hẹn khám" từ màn hình kê đơn, cả 2 biến thể màn hình hẹn khám của khối kết thúc điều trị kiểm tra thời hạn bệnh án ngoại trú trước mọi cảnh báo hiện có; quá 1 năm → chặn, hiển thị "Bệnh nhân đã hết thời gian hẹn khám trong năm, không được phép hẹn khám". Bật/tắt bằng cấu hình toàn viện `MOS.HIS_TREATMENT.IS_BLOCK_APPOINTMENT_WHEN_OUT_PATIENT_MEDI_RECORD_OVER_ONE_YEAR` (`1` = chặn); tắt thì không phát sinh truy vấn nào. Bệnh án lấy qua `api/HisMediRecord/Get` theo `MEDI_RECORD_ID` của lượt điều trị, tối đa 1 lần/lần mở màn hình. Files (UC): `AppointmentMediRecordExpiryWorker.cs` (mới), `EndTypeForm/FormAppointment.cs`, `CloseTreatment/FormAppointment.cs`, `ConfigKeyCFG.cs`, `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs`. |
| 29/07/2026 | nampp | **MIMS Drug Pregnancy/Lactation** — Truyền `PatientProfile` (PN mang thai / cho con bú) vào request MIMS khi lưu đơn (`CheckMIMS`) và menu chuột phải "Đánh giá thông tin thuốc". Config mới `HIS.Desktop.Mims.IsCheckPregnancyLactation` (mặc định TẮT — request MIMS không đổi 1 byte). BN nữ: prefetch async bản ghi `HIS_MIMS_PATIENT_PROFILE` (`api/HisMimsPatientProfile/Get` — BE đang làm) trong `timerInitForm_Tick`; khi lưu đơn nếu BN có tick → build profile (Gender F, Age từ `TDL_PATIENT_DOB`, Pregnancy.Month, Nursing=true) truyền `CheckAndAlert(..., patientProfile:)` → MIMS trả thêm tab "Thai kỳ"/"Cho con bú" trong CÙNG popup (không thêm request). Files: `Config/HisConfigCFG.cs`, `AssignPrescription/frmAssignPrescription__MimsPatientProfile.cs` (MỚI), `frmAssignPrescription.cs`, `__InitMenuMouseRight.cs`. Thư viện `HIS.Desktop.MIMS.Integration` bổ sung: builder `<PatientProfile>`, parser + enum Pregnancy (FDA A/B/C/D/X/+) / Lactation (Caution/Avoid if possible/Contraindicated), popup từ mức Pregnancy C trở lên, gán cột log `IS_PREGNANT/IS_LACTATING/PREGNANCY_COUNT/LACTATION_COUNT/PATIENT_AGE/PATIENT_GENDER`. |
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** (dùng chung ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`, ngưỡng trống coi như 0); (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí (hồ sơ + đang kê) vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY` theo hiệu lực FROM_TIME/TO_TIME) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp (chỉ log). Mặc định 2 key tắt — không đổi hành vi hiện tại. |
| 16/06/2026 | huyvu20 | **Việc 2.6** — Chẩn đoán nguyên nhân tử vong & không khuyến khích bệnh chính:<br>• Ẩn chẩn đoán `IS_DEATH_CAUSE_ONLY=1` khỏi danh sách chọn bệnh chính (`cboIcds`) và bệnh phụ (popup inline + `frmSecondaryIcd`), giữ `currentIcds` đầy đủ để hiển thị giá trị đã lưu. Không áp dụng cho YHCT.<br>• Cảnh báo `IS_NOT_RECOMMEND_MAIN=1` khi chọn/sửa chẩn đoán chính (`ChangecboChanDoanTD`, `LoadIcdCombo`).<br>• Thêm `CheckDeathCauseIcdValid()` chặn lưu khi có kết thúc điều trị nhưng dùng chẩn đoán nguyên nhân tử vong cho ca không tử vong (`HIS_TREATMENT_END_TYPE.ID__CHET`).<br>• Thêm message `BenhKhongKhuyenKhichDungLamBenhChinh`, `BenhLaNguyenNhanTuVongKhongDuocSuDung` (vi/en/my). |
| 16/06/2026 | huyvu20 | **Việc 2.6 (bổ sung)** — `IS_DEATH_CAUSE_ONLY=1` lọt qua khi gõ tay & khi load hồ sơ đã lưu. Bổ sung chặn ở các đường còn lại của bệnh chính + phụ:<br>• Gõ tay/chọn bệnh chính (`LoadIcdCombo`, `ChangecboChanDoanTD`) → báo + xóa.<br>• Gõ tay bệnh phụ (`CheckIcdWrongCode`) → báo + loại mã.<br>• Load hồ sơ đã lưu (`LoadIcdToControl`, `LoadDataToIcdSub`) → bỏ qua không load.<br>• Thêm message `BenhLaNguyenNhanTuVongKhongDuocDungLamChanDoan` (vi/en/my). Ô CĐ nguyên nhân (Cause) giữ nguyên. |
| 15/07/2026 | huannh | **Kê đơn lọc kho theo loại (4.1.3)** — Thêm cấu hình `HIS.Desktop.Plugins.AssignPrescription.ENABLE_TREATMENT_PRESCRIPTION` (`HisConfigCFG.EnableTreatmentPrescription`, mặc định TẮT → giữ nguyên luồng). Lọc **thật** bằng field sẵn có `IS_CABINET`/`IS_TREATMENT_STOCK`/`IS_OUTPATIENT_STOCK` của `V_HIS_MEDI_STOCK`; method `FilterMestRoomByStockCategory` đặt trong `frmAssignPrescription__InitCombo.cs`, gọi trong `InitComboMediStockAllow` khi config BẬT. Áp dụng cho cả PK + YHCT + CLS. (Đã bỏ file scaffold `EnumMediStockCategory.cs`/`frmAssignPrescription__StockCategory.cs`.) |
| 15/07/2026 | huannh | **Đơn mẫu — HDSD vật tư** — Lưu `TUTORIAL` cho đơn mẫu vật tư (`frmHisExpMestTemplateCreate`, nhánh VATTU + VATTU_DM → bảng `HIS_EMTE_MATERIAL_TYPE`) cho cả PK + YHCT + CLS. |
| 16/07/2026 | huannh | **Đơn mẫu — HDSD vật tư (load)** — Wire load lại HDSD vật tư: `MediMatyTypeADO` ctor `V_HIS_EMTE_MATERIAL_TYPE` set `this.TUTORIAL = inputData.TUTORIAL` (cả 3 plugin). Cần EFMODEL bổ sung cột `TUTORIAL` trên view `V_HIS_EMTE_MATERIAL_TYPE` mới build được (đang chờ cập nhật DLL). |
| 17/07/2026 | huannh | **Fix NRE khi lưu thiếu cđ chính** — `ProcessSaveData` (`frmAssignPrescription__Save.cs`) crash `NullReferenceException` tại `txtIcdCode.EditValue.ToString()` / `txtIcdMainText.EditValue.ToString()` khi ô cđ chính trống (xảy ra rõ khi nạp đơn mẫu chạy thận không mang ICD). Sửa null-safe: `EditValue == null ? "" : EditValue.ToString()`. Sau fix, các validation sẵn có (`CheckIcd`...) chạy đúng thay vì crash "xử lý thất bại". |
| 17/07/2026 | huannh | **Chặn sớm khi thiếu cđ chính** — `ProcessSaveData` kiểm tra `txtIcdCode.Text` trống trước khi map: set `ErrorText` tại control + báo `ChuaNhapChanDoanChinh` + focus lại ô cđ chính, `return`. Thêm message `ChuaNhapChanDoanChinh` (vi/en/my) + accessor `ResourceMessage`. Thay vì lưu ICD rỗng, người dùng được báo rõ phải nhập cđ chính. |
| 18/07/2026 | huannh | **Fix cột MV (Mang về) không load khi chọn đơn mẫu** — Constructor `MediMatyTypeADO(V_HIS_EMTE_MEDICINE_TYPE...)` không set `IsHomePresMedicine` nên cột MV trống khi nạp thuốc từ đơn mẫu. Bổ sung `this.IsHomePresMedicine = (mety.IS_HOME_PRES_MEDICINE == 1)` — lấy mặc định theo danh mục `V_HIS_MEDICINE_TYPE`, đồng nhất với luồng thêm dòng (`RowAdd/AddAbstract`). Lưu ý: đơn mẫu chưa lưu MV riêng theo dòng → MV load theo cờ danh mục, chưa nhớ tick thủ công (cần BE thêm cột nếu muốn nhớ đúng tick). |

## 9. Test Cases

### Cảnh báo bệnh chính (IS_NOT_RECOMMEND_MAIN = 1)
- [ ] Chọn ICD chính có cờ → hiện cảnh báo. Chọn "Có" → giữ. Chọn "Không" → xóa, focus lại combo.
- [ ] Gõ mã ICD chính có cờ + Enter (`LoadIcdCombo`) → hiện cảnh báo tương tự.
- [ ] Mở hồ sơ đã lưu có ICD chính có cờ → KHÔNG cảnh báo (chỉ hiển thị).

### Ẩn nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1)
- [ ] Danh sách chọn bệnh chính/bệnh phụ KHÔNG hiển thị ICD có cờ.
- [ ] Gõ tay mã nguyên nhân tử vong vào ô chính (`LoadIcdCombo`/`ChangecboChanDoanTD`) → báo + xóa, không nhận.
- [ ] Gõ tay mã nguyên nhân tử vong vào ô phụ (`CheckIcdWrongCode`) → báo + loại mã đó.
- [ ] Mở hồ sơ đã lưu có ICD nguyên nhân tử vong ở chính/phụ → mã đó bị bỏ khỏi ô (không load).
- [ ] Ô CĐ nguyên nhân (Cause) vẫn nhận được ICD nguyên nhân tử vong.
- [ ] Chẩn đoán YHCT KHÔNG bị ảnh hưởng.

### Kiểm tra khi lưu
- [ ] Có kết thúc điều trị, kết quả KHÔNG tử vong, có ICD nguyên nhân tử vong → chặn lưu + thông báo liệt kê mã.
- [ ] Có kết thúc điều trị, kết quả tử vong (`ID__CHET`) → cho lưu.
- [ ] KHÔNG kết thúc điều trị → KHÔNG kiểm tra.
