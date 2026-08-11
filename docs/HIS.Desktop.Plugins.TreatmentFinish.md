# HIS.Desktop.Plugins.TreatmentFinish — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentFinish |
| Loại | Form (FormBase) |
| Mục đích | Kết thúc điều trị độc lập cho BN nội trú / ngoại trú / ban ngày (chọn KQĐT, nhập kết quả, in giấy ra viện/chuyển viện/báo tử) |
| Người tạo | IVT |
| Ngày cập nhật gần nhất | 14/05/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
1. Tiếp nhận → Hồ sơ điều trị → Chọn BN → "Kết thúc điều trị"
2. Mở FormTreatmentFinish (plugin này)
3. Form load: thông tin BN, ICD đã có, KQĐT mặc định (config), DHST, danh sách phòng
4. BS chọn Loại ra viện (cboTreatmentEndType) → ShowPopupEndType:
   - ID__CHET → mở FormDeath (Cause of Death)
   - ID__CHUYEN → mở FormTransfer (chuyển viện)
   - ID__HEN → mở FormAppointment
   - ID__RAVIEN/XINRAVIEN → enable form chính
   - (Mới — 2608) KQĐT thuộc config MUST_INPUT_SEVERE_ILLNESS_HOME_CODES (và != ID__CHET)
     → mở popup HIS.Desktop.Plugins.HisDeathInfo "Thông tin người bệnh nặng xin về"
5. Nhập ICD, kết quả điều trị, phương pháp điều trị
6. Bấm Lưu → ProcessDataBeforeSaveAsync:
   - (Mới — 2608) Re-check HIS_SEVERE_ILLNESS_INFO. Nếu KQĐT thuộc config mà chưa có
     bản ghi → cảnh báo + mở lại popup. Chưa nhập → CHẶN commit.
   - Save qua Save.SaveFactory.MakeISave → API HisTreatment/Finish
7. Sau khi Save → In giấy ra viện/chuyển viện/báo tử (theo PrintConfig)
```

### Điều kiện nghiệp vụ

- Config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` null/rỗng → giữ luồng cũ 100%.
- Config có khai báo mã (vd `"01"`, `"TLN"`) → khi BS chọn Loại ra viện thuộc danh sách → bắt buộc nhập popup HisDeathInfo.
- Luồng tử vong cũ (`ID__CHET`) vẫn dùng FormDeath nội bộ — không thay đổi.
- Popup HisDeathInfo có thể trigger 2 lần (lần 1 khi chọn cbo trong ShowPopupEndType, lần 2 khi Save nếu BS đóng popup không lưu) — đảm bảo không thể commit nếu chưa lưu.

### Quy tắc chẩn đoán nguyên nhân tử vong (mục 2.7)

- UC chẩn đoán (chính/phụ/YHCT chính/YHCT phụ + chẩn đoán hiển thị trên giấy ra viện) được truyền `IsShowDeathCause = true` → hiển thị cả các chẩn đoán đánh dấu `IS_DEATH_CAUSE_ONLY = 1`.
- Khi Lưu (kết thúc điều trị): kiểm tra **tất cả chẩn đoán chính + phụ** (KHÔNG gồm YHCT theo `IS_TRADITIONAL`). Nếu tồn tại chẩn đoán `IS_DEATH_CAUSE_ONLY = 1` mà kết quả điều trị (`TREATMENT_RESULT_ID`) không phải Tử vong (`ID__CHET = 5`) hoặc Tử vong ngoại viện (`ID__TVNV = 8`) → hiển thị thông báo `"Bệnh XXX,YYY là nguyên nhân tử vong không được sử dụng cho các trường hợp không phải tử vong."` và **dừng xử lý**.
- Cảnh báo "không khuyến khích dùng là bệnh chính" (`IS_NOT_RECOMMEND_MAIN`) giữ mặc định của UC (vẫn cảnh báo khi sửa chẩn đoán chính) — mục 2.7 không thay đổi hành vi này.
- **Cảnh báo "chỉ dùng cho bệnh nhân tử vong" khi chọn (của UC):** UC dùng chung `HIS.UC.Icd` luôn bật hộp thoại xác nhận khi chọn ICD `IS_DEATH_CAUSE_ONLY = 1`, không xét kết quả điều trị và **không có tham số để tắt**. Vì **không được sửa UC**, plugin xử lý bằng cách truyền vào UC (chỉ các UCIcd: CĐ chính, CĐ YHCT chính, CĐ hiển thị) một **bản sao danh sách đã clone + bỏ cờ** death-cause (`MaskDeathCauseIcds`) → UC không cảnh báo nhưng vẫn hiển thị ICD. Không mutate cache. CĐ phụ/YHCT phụ dùng `IsShowDeathCause=true` (UC phụ không có cảnh báo). Ràng buộc thực sự (chặn dùng sai) đặt ở **kiểm tra lúc lưu** đọc cache thật.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT / V_HIS_TREATMENT | Table/View | Hồ sơ điều trị cần kết thúc |
| HIS_TREATMENT_END_TYPE | Table | Danh mục loại ra viện (CODE → ID) |
| HIS_TREATMENT_END_TYPE_EXT | Table | Loại ra viện mở rộng (nghỉ ốm, hẹn mổ...) |
| HIS_TREATMENT_RESULT | Table | Kết quả điều trị |
| HIS_SEVERE_ILLNESS_INFO | Table | (Đọc) Kiểm tra đã nhập thông tin tử vong/nặng xin về |
| HIS_EVENTS_CAUSES_DEATH | Table | Sự kiện gây tử vong |
| HIS_HOSPITALIZE_REASON | Table | Lý do nhập viện |

## 4. UI Layout

### Sơ đồ giao diện (rút gọn)

```
+------------------------------------------------------------------+
| Header BN: Mã BN / Họ tên / BHYT / Khoa / Ngày vào                |
+------------------------------------------------------------------+
| ICD chính | ICD phụ | Loại ra viện (cboTreatmentEndType) | TT Ext |
+------------------------------------------------------------------+
| Kết quả ĐT | Phương pháp ĐT | Lời dặn | Thời gian ra              |
+------------------------------------------------------------------+
| Tab: DHST | Lý do nhập viện | Số CT | Mã BHXH | Số hẹn khám       |
+------------------------------------------------------------------+
| [Lưu (Ctrl+S)] [In] [Xóa thông tin ra viện] [Đóng]                |
+------------------------------------------------------------------+
```

### Form phụ (subforms)

| Form | Trigger | Mục đích |
|------|---------|----------|
| FormDeath | KQĐT = Tử vong | Nhập nguyên nhân tử vong, cấp giấy báo tử |
| FormTransfer | KQĐT = Chuyển viện | Nhập thông tin chuyển viện |
| FormAppointment | KQĐT = Hẹn | Nhập thông tin hẹn tái khám |
| frmCheckBedRoom | BN nội trú trước save | Kiểm tra giường/phòng |
| frmWarning | Có cảnh báo | Hiện cảnh báo trước save |

## 5. API Endpoints

| Action | URI | Consumer | Filter/DTO |
|--------|-----|----------|------------|
| Lưu kết thúc | api/HisTreatment/Finish | MosConsumer | HisTreatmentFinishSDO |
| Xóa thông tin ra viện | api/HisTreatment/DeleteEndInfo | MosConsumer | long (treatmentId) |
| Lấy thông tin nặng/tử vong | api/HisSevereIllnessInfo/Get | MosConsumer | HisSevereIllnessInfoFilter |
| Lấy sự kiện gây tử vong | api/HisEventsCausesDeath/Get | MosConsumer | HisEventsCausesDeathFilter |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintTreatmentFinish | In giấy ra viện / chuyển viện / báo tử |
| HIS.Desktop.Plugins.Library.PrintOtherForm | In các biểu mẫu phụ |
| HIS.Desktop.Plugins.Library.EmrGenerate | Sinh input ký số EMR |
| HIS.Desktop.Plugins.Library.CheckIcd | Check ICD theo giới tính, tuổi |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.InformationAllowGoHome | KQĐT = Xin ra viện + Kết quả nặng | treatmentId, true |
| HIS.Desktop.Plugins.HisDeathInfo | **(2608)** KQĐT thuộc `MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` (không phải `ID__CHET`) | treatmentId, Module |

## 7. Print

| Loại in | PrintTypeCode | Library | Trigger |
|---------|--------------|---------|---------|
| Giấy ra viện | Mps000008 | PrintTreatmentFinish | KQĐT = Ra viện/Xin ra viện |
| Giấy chuyển viện | Mps000010 | PrintTreatmentFinish | KQĐT = Chuyển viện |
| Phiếu bàn giao BN chuyển | Mps000382 | PrintTreatmentFinish | KQĐT = Chuyển viện |
| Giấy báo tử | Mps000268 | PrintTreatmentFinish | KQĐT = Tử vong |
| Bảng kê thanh toán | Mps000446 | PrintBordereau | After save |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/05/2026 | dangth2 | **2608** Bổ sung trigger popup `HIS.Desktop.Plugins.HisDeathInfo` "Thông tin người bệnh nặng xin về" khi BS chọn Loại ra viện thuộc config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES`. Hai điểm trigger: (1) `ShowPopupEndType()` — auto mở popup khi chọn cbo, (2) `ProcessDataBeforeSaveAsync()` — re-check trước commit, chặn save nếu chưa nhập. Files: `Config/ConfigKey.cs` (thêm `MustInputSevereIllnessHomeCodes`), `Base/SevereIllnessHomeWorker.cs` (mới), `FormTreatmentFinish.cs` (thêm branch trong `ShowPopupEndType`), `FormTreatmentFinish_Event.cs` (check trong `ProcessDataBeforeSaveAsync`), `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`ChuaNhapThongTinBenhNangXinVe`). |
| 15/06/2026 | huyvu20 | **ICD nguyên nhân tử vong (mục 2.7)** Hiển thị chẩn đoán nguyên nhân tử vong (`HIS_ICD.IS_DEATH_CAUSE_ONLY = 1`) trong các UC chẩn đoán + bổ sung kiểm tra khi lưu: nếu chẩn đoán chính/phụ (không gồm YHCT) là nguyên nhân tử vong nhưng kết quả `TREATMENT_RESULT_ID` không phải Tử vong (`ID__CHET=5`) / Tử vong ngoại viện (`ID__TVNV=8`) → chặn lưu + thông báo. Files: `FormTreatmentFinish.cs` (`CheckDeathCauseIcdValid()` gọi trong `save()`), `FormTreatmentFinish_Event.cs`, `CloseTreatment/FormTTCDHienThiTrenGiayRaVien.cs`, `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`BenhLaNguyenNhanTuVongKhongDuocSuDung`). |
| 15/06/2026 | huyvu20 | **Bỏ cảnh báo "chỉ dùng cho BN tử vong" lúc chọn (theo phản hồi review)** UC dùng chung `HIS.UC.Icd` luôn bật cảnh báo khi chọn ICD death-cause (kể cả khi kết quả đã là Tử vong) và không có tham số tắt; **không được sửa UC**. Plugin truyền `DataIcds` đã clone + bỏ cờ death-cause (`MaskDeathCauseIcds`) cho các UCIcd (CĐ chính, CĐ YHCT chính, CĐ hiển thị) → UC không còn cảnh báo nhưng vẫn hiển thị. Bỏ `IsShowDeathCause` ở các UCIcd này (giữ ở CĐ phụ/YHCT phụ). Ràng buộc vẫn được bảo đảm bởi kiểm tra lúc lưu (đọc cache thật). Files: `FormTreatmentFinish.cs` (thêm `MaskDeathCauseIcds`), `FormTreatmentFinish_Event.cs`, `CloseTreatment/FormTTCDHienThiTrenGiayRaVien.cs`. |
| 06/08/2026 | anhnh2 | **PT-53438 Chặn hẹn khám khi bệnh án ngoại trú quá 1 năm** Bổ sung chốt kiểm tra tại bước xác nhận hẹn khám (`CloseTreatment/FormAppointment.btnSave_Click`, đặt ngay sau `dxValidationProvider.Validate()` — trước mọi cảnh báo hiện có). Điều kiện chặn: cấu hình `MOS.HIS_TREATMENT.IS_BLOCK_APPOINTMENT_WHEN_OUT_PATIENT_MEDI_RECORD_OVER_ONE_YEAR = 1` + lượt điều trị gắn bệnh án thuộc chương trình quản lý loại ngoại trú (`HIS_PROGRAM.TREATMENT_TYPE_ID = DTNGOAITRU` hoặc `AUTO_CHANGE_TO_OUT_PATIENT = 1`) + ngày thao tác > `HIS_MEDI_RECORD.CREATE_TIME` + 1 năm. Thiếu dữ liệu / lỗi kỹ thuật → không chặn, chỉ ghi log. Files: `Config/ConfigKey.cs`, `Base/AppointmentMediRecordExpiryWorker.cs` (mới), `CloseTreatment/FormAppointment.cs`, `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`BenhNhanDaHetThoiGianHenKhamTrongNam`), `.csproj`. |
| 01/07/2026 | nampp | **Liên thông EmrToolKit dữ liệu chuyển tuyến** Bổ sung checkbox `ChkLienThongEmrToolkit` (Designer, cạnh "Xuất XML thông tuyến"): ẩn/hiện theo cấu hình `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo` (qua `EmrToolkitImportProcessor.IsConfigured()`), enable khi loại ra viện = chuyển viện (`ShowPopupEndType`), lưu trạng thái qua ControlState (`InitControlState` + `ChkLienThongEmrToolkit_CheckedChanged`). Sau khi lưu chuyển viện thành công (`SaveTreatmentFinish`) → build `EmrImportModel` từ dữ liệu thật (BN, BHYT `patientTypeAlter`, chẩn đoán `SHOW_ICD_*`, lý do/nơi chuyển/phương tiện/người hộ tống từ SDO) rồi gọi thư viện `HIS.Desktop.Plugins.Library.EmrToolkitImport` đồng bộ. Files: `FormTreatmentFinish.Designer.cs`, `FormTreatmentFinish.cs`, `FormTreatmentFinish_Event.cs`, `FormTreatmentFinish_EmrToolkit.cs` (mới), `.csproj` (ProjectReference EmrToolkitImport). |

## 9. Test Cases

### Luồng cũ (giữ nguyên 100%)

- [ ] Config `MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` null/rỗng → chọn bất kỳ KQĐT → KHÔNG hiện popup HisDeathInfo mới.
- [ ] KQĐT = Tử vong → FormDeath mở như cũ.
- [ ] KQĐT = Chuyển viện → FormTransfer mở như cũ.
- [ ] KQĐT = Hẹn → FormAppointment mở như cũ.
- [ ] KQĐT = Ra viện → form chính enable, không popup phụ.

### Luồng mới (2608)

- [ ] Config = `"01"`. BS chọn "Ra viện" trong cboTreatmentEndType → popup HisDeathInfo tự động mở ngay.
- [ ] Trong popup, BS bấm Cancel/đóng (chưa lưu) → bấm Lưu kết thúc điều trị → MessageBox cảnh báo → popup mở lại.
- [ ] BS điền + Lưu trong popup → đóng popup → Lưu kết thúc điều trị thành công.
- [ ] Config = `"01,TLN"`. BS chọn "Xin ra viện" (CODE=TLN) → popup mở.
- [ ] Config = `"01"`. BS chọn "Chuyển viện" → FormTransfer mở (giữ luồng cũ).
- [ ] Config = `"TUVONG,01"`. BS chọn "Tử vong" → FormDeath mở (luồng cũ ưu tiên).

### Chẩn đoán nguyên nhân tử vong (2.7)

- [ ] ICD đánh dấu `IS_DEATH_CAUSE_ONLY = 1` HIỂN THỊ trong danh sách chọn của UC chẩn đoán chính/phụ (do `IsShowDeathCause = true`).
- [ ] Chọn ICD nguyên nhân tử vong làm bệnh chính + KQĐT = Tử vong (`ID__CHET`) → Lưu thành công.
- [ ] Chọn ICD nguyên nhân tử vong làm bệnh chính + KQĐT = Tử vong ngoại viện (`ID__TVNV`) → Lưu thành công.
- [ ] Chọn ICD nguyên nhân tử vong làm bệnh chính + KQĐT = Khỏi/Đỡ/... → Lưu bị chặn + thông báo "Bệnh ... là nguyên nhân tử vong không được sử dụng...".
- [ ] Chọn ICD nguyên nhân tử vong làm bệnh phụ + KQĐT không phải tử vong → Lưu bị chặn (áp dụng cả bệnh phụ).
- [ ] Nhiều ICD nguyên nhân tử vong → thông báo liệt kê đầy đủ các mã, cách nhau dấu phẩy.
- [ ] Chẩn đoán YHCT có IS_DEATH_CAUSE_ONLY → KHÔNG bị check (loại trừ theo IS_TRADITIONAL).

### Edge cases

- [ ] BN đã có HIS_SEVERE_ILLNESS_INFO từ trước → `HasValidSevereIllnessInfo` trả true → KHÔNG cảnh báo, cho lưu.
- [ ] API HisSevereIllnessInfo/Get lỗi → trả false → chặn commit để an toàn (log Error).
- [ ] BN mở lại hồ sơ đã finish (isFinished=true) → `cboTreatmentEndType_EditValueChanged` skip ShowPopupEndType → không trigger popup lặp lại.
