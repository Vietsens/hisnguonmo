# HIS.Desktop.Plugins.ExamServiceReqExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExamServiceReqExecute |
| Loại | UC (UserControl) |
| Mục đích | Xử lý yêu cầu khám tại phòng khám (BS nhập DHST, ICD, ICD phụ, kết luận, lý do, kết thúc điều trị, in giấy ra viện/báo tử/chuyển viện) |
| Người tạo | IVT |
| Ngày cập nhật gần nhất | 14/05/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
1. Tiếp nhận → Phòng khám → Chọn yêu cầu khám của BN
2. Mở UCExamServiceReqExecute (plugin này)
3. BS nhập: DHST, ICD chính/phụ, kết luận, lý do nhập viện, phương pháp điều trị
4. (Tùy chọn) Tích chọn "Kết thúc điều trị" → mở UC HIS.UC.ExamTreatmentFinish
   - BS chọn Loại ra viện (TREATMENT_END_TYPE): Hẹn / Chuyển / Tử vong / Ra viện / Xin ra viện / Trốn
5. (Mới — 2608) Nếu KQĐT thuộc config MUST_INPUT_SEVERE_ILLNESS_HOME_CODES
   → Plugin mở popup HIS.Desktop.Plugins.HisDeathInfo "Thông tin người bệnh nặng xin về"
   → BS bắt buộc nhập đủ field, lưu vào HIS_SEVERE_ILLNESS_INFO
   → Nếu chưa nhập → chặn commit kết thúc điều trị
6. Bấm Lưu → API HisServiceReq/UpdateExam:
   - Cập nhật HIS_SERVICE_REQ
   - Cập nhật/Tạo HIS_TREATMENT_FINISH nếu có
   - In phiếu phù hợp (hẹn / chuyển / ra viện / báo tử) qua MpsPrinter
```

### Điều kiện nghiệp vụ

- Config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` null/rỗng → giữ luồng cũ 100%, không trigger popup HisDeathInfo mới.
- Config có khai báo mã (vd `"01"`, `"TLN"`) → khi BS chọn Loại ra viện thuộc danh sách (và không phải Tử vong) → bắt buộc nhập popup HisDeathInfo.
- Luồng tử vong cũ (`ID__CHET`) vẫn dùng `frmPopUpSick` trong UC `HIS.UC.ExamTreatmentFinish` — không thay đổi.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu khám của BN |
| HIS_TREATMENT / V_HIS_TREATMENT | Table/View | Hồ sơ điều trị |
| HIS_TREATMENT_END_TYPE | Table | Danh mục loại ra viện (CODE → ID) |
| HIS_SERE_SERV | Table | Dịch vụ đã thực hiện |
| HIS_SEVERE_ILLNESS_INFO | Table | (Đọc) Kiểm tra đã có thông tin tử vong/nặng xin về cho treatment |
| HIS_DHST | Table | Dấu hiệu sinh tồn |

## 4. UI Layout

### Sơ đồ giao diện (rút gọn)

```
+------------------------------------------------------------------+
| Thông tin BN (UCPatientHeader)                                   |
+------------------------------------------------------------------+
| [chk Khám bổ sung] [chk Nhập viện] [chk Kết thúc điều trị]      |
+------------------------------------------------------------------+
| Tab Dấu hiệu sinh tồn | ICD | ICD phụ | UC nhập viện | ...      |
+------------------------------------------------------------------+
| Panel ExamTreatmentFinish (HIS.UC.ExamTreatmentFinish)            |
|   cboTreatmentEndType — Loại ra viện                              |
|   cboTreatmentEndTypeExt — Thông tin bổ sung                      |
|   ...                                                             |
+------------------------------------------------------------------+
| [Lưu (Ctrl+S)] [In] [Hủy]                                        |
+------------------------------------------------------------------+
```

### UC sử dụng

| UC | Mục đích |
|----|----------|
| HIS.UC.ExamTreatmentFinish | Chọn loại ra viện + nhập thông tin kết thúc |
| HIS.UC.Icd | ICD chính |
| HIS.UC.SecondaryIcd | ICD phụ |
| HIS.UC.DHST | Dấu hiệu sinh tồn |
| HIS.UC.Hospitalize | Nhập viện |
| HIS.UC.ExamAddition | Khám bổ sung |
| HIS.UC.NextTreatmentInstruction | Hướng điều trị tiếp |

## 5. API Endpoints

| Action | URI | Consumer | Filter/DTO |
|--------|-----|----------|------------|
| Lấy yêu cầu khám | api/HisServiceReq/GetView | MosConsumer | HisServiceReqViewFilter |
| Cập nhật yêu cầu khám | api/HisServiceReq/UpdateExam | MosConsumer | HisServiceReqExamUpdateSDO |
| Lấy thông tin nặng/tử vong | api/HisSevereIllnessInfo/Get | MosConsumer | HisSevereIllnessInfoFilter |
| Lấy sự kiện gây tử vong | api/HisEventsCausesDeath/Get | MosConsumer | HisEventsCausesDeathFilter |

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintTreatmentFinish | In giấy ra viện / chuyển viện / hẹn / báo tử |
| HIS.Desktop.Plugins.Library.PrintPrescription | In đơn thuốc |
| HIS.Desktop.Plugins.Library.EmrGenerate | Sinh input ký số EMR |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.InfomationExecute | Click button "Thông tin xử lý" | treatmentId, dataSelectedToPTDT, DelegateSelectData, Module |
| HIS.Desktop.Plugins.HisDeathInfo | **(2608)** KQĐT thuộc `MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` (không phải `ID__CHET`) | treatmentId, Module |

## 7. Print

| Loại in | PrintTypeCode | Library | Trigger |
|---------|--------------|---------|---------|
| Giấy ra viện | Mps000008 | PrintTreatmentFinish | KQĐT = Ra viện/Xin ra viện/Trốn |
| Giấy chuyển viện | Mps000010 | PrintTreatmentFinish | KQĐT = Chuyển |
| Giấy hẹn tái khám | Mps000268 | PrintTreatmentFinish | KQĐT = Hẹn |
| Giấy báo tử | Mps000268 | PrintTreatmentFinish | KQĐT = Tử vong |
| Đơn thuốc | Mps000118... | PrintPrescription | Kê đơn |

## 7B. Cấu Hình HIS_CONFIG Liên Quan

| Key | Vai trò | Khi không khai báo |
|-----|--------|--------------------|
| `HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit` | Bật/tắt kiểm tra số ICD phụ ra viện; chế độ chặn ("1") hoặc cảnh báo ("2") | Không kiểm tra |
| `HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit.IcdSubMaxCount` | Ngưỡng tối đa số mã ICD phụ ra viện | Mặc định = **12** |
| `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` | Danh sách TREATMENT_END_TYPE_CODE bật popup HisDeathInfo | Không bật popup |

### Chi tiết kiểm tra số ICD phụ ra viện (PTTK 4.1.2)

- **Plugin**: `HIS.Desktop.Plugins.ExamServiceReqExecute`
- **Path / Phạm vi áp dụng**: chỉ kiểm tra ICD phụ **ra viện** do `HIS.UC.ExamTreatmentFinish` nhập. **KHÔNG** kiểm tra ICD phụ của phần khám.
- **Thay đổi**: Thêm đọc cấu hình `IcdSubMaxCount`; thay hằng số 12 bằng ngưỡng động cho kiểm tra ICD phụ ra viện.
- **Vị trí code**: `ExamServiceReqExecuteControl.cs` → `ValidIcdLen()` (~dòng 3905), gọi từ `ExamServiceReqExecuteControl__Process.cs:1340` ngay sau `treatmentFinishProcessor.Validate(...)`.
- **Thay đổi giao diện**: Không thay đổi giao diện.

#### Thay đổi xử lý

| Bước | Điều kiện | Hành vi |
|------|-----------|---------|
| 1 | `IsCheckSubIcdExceedLimit` không phải "1" hoặc "2" | Bỏ qua kiểm tra, tiếp tục lưu bình thường |
| 2 | Đọc `IcdSubMaxCount` | Số nguyên dương hợp lệ → ngưỡng = giá trị đó; ngược lại → ngưỡng = **12** |
| 3 | Số ICD phụ ra viện ≤ ngưỡng | Tiếp tục lưu bình thường |
| 4a | Số ICD phụ ra viện > ngưỡng và `IsCheckSubIcdExceedLimit` = "1" | XtraMessageBox icon Warning, chỉ nút OK → chặn lưu |
| 4b | Số ICD phụ ra viện > ngưỡng và `IsCheckSubIcdExceedLimit` = "2" | XtraMessageBox icon Question, nút Yes/No: "Chẩn đoán phụ ra viện vượt quá {N} mã. Bạn có muốn tiếp tục?" |
| 4b-i | Người dùng chọn "Có" | Tiếp tục lưu |
| 4b-ii | Người dùng chọn "Không" | Hủy lưu, giữ nguyên màn hình |

- **Source ICD phụ ra viện**: `treatmentFinishProcessor.GetValue(ucTreatmentFinish)` trả về `ExamTreatmentFinishResult.TreatmentFinishSDO`. Lấy chuỗi ICD phụ theo thứ tự ưu tiên (lấy field đầu tiên không rỗng):
  1. `ShowIcdText` — chuỗi ICD phụ hiển thị trên giấy ra viện, do user nhập qua popup `frmICDInformation` ("Thông tin chuẩn đoán hiển thị trên giấy ra viện"). **Đây là nguồn chính** vì là dữ liệu thực sự in lên giấy ra viện của BN.
  2. `ShowIcdSubCode` — fallback khi popup chỉ chọn mã ICD mà không có text tổng hợp.
  3. `IcdSubCode` — legacy field, đa số case = null, giữ làm fallback cuối cho tương thích.
- Tách theo `icdSeparators` (`;`) → đếm số phần tử không rỗng → so với ngưỡng.
- Message dùng resource đa ngôn ngữ (vi/en/my): `Plugin_ExamServiceReqExecute__ChanDoanPhuRaVienVuotQuaSoLuongChan` (chặn) / `Plugin_ExamServiceReqExecute__ChanDoanPhuRaVienVuotQuaSoLuongCanhBao` (cảnh báo). Format `{0}` = ngưỡng động.
- MessageBox dùng `DevExpress.XtraEditors.XtraMessageBox` với `MessageBoxIcon.Warning` (chặn) / `MessageBoxIcon.Question` (cảnh báo) — đồng bộ skin với toàn bộ HIS Desktop.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/05/2026 | dangth2 | **2608** Bổ sung trigger popup `HIS.Desktop.Plugins.HisDeathInfo` "Thông tin người bệnh nặng xin về" khi BS chọn Loại ra viện thuộc config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES`. Files: `Config/HisConfigCFG.cs` (thêm `MustInputSevereIllnessHomeCodes`), `Base/SevereIllnessHomeWorker.cs` (mới), `ExamServiceReqExecuteControl__Process.cs` (chèn check trong `ProcessTreatmentFinish`), `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`ChuaNhapThongTinBenhNangXinVe`). |
| 16/05/2026 | tuanln | **PTTK 4.1.2** Thay hằng số 12 bằng ngưỡng động `IcdSubMaxCount` cho kiểm tra số ICD phụ ra viện. Chỉ kiểm tra ICD phụ ra viện ở `HIS.UC.ExamTreatmentFinish` (bỏ kiểm tra ICD phụ phần khám). Đọc qua `HisConfigCFG.IsCheckSubIcdExceedLimit` + `HisConfigCFG.IcdSubMaxCount` (mặc định 12). Source data đọc theo thứ tự ưu tiên: `ShowIcdText` → `ShowIcdSubCode` → `IcdSubCode` (đúng dữ liệu in trên giấy ra viện). Message chuyển sang resource đa ngôn ngữ, MessageBox dùng `XtraMessageBox` với icon Warning/Question. Files: `Config/HisConfigCFG.cs`, `ExamServiceReqExecuteControl.cs` (`ValidIcdLen()`), `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`ChanDoanPhuRaVienVuotQuaSoLuongChan` / `...CanhBao`). |

## 9. Test Cases

### Luồng cũ (giữ nguyên 100%)

- [ ] Config `MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` null/rỗng → BS chọn bất kỳ KQĐT (kể cả "Ra viện") → KHÔNG hiện popup HisDeathInfo mới.
- [ ] Chọn KQĐT = Tử vong → popup cũ `frmPopUpSick` vẫn hoạt động bình thường.
- [ ] Chọn KQĐT = Chuyển viện → `FormTransfer` mở như cũ.
- [ ] Chọn KQĐT = Hẹn → `FormAppointment` mở như cũ.

### Luồng mới (2608)

- [ ] Config = `"01"` (mã Ra viện). BS chọn "Ra viện" → bấm Lưu → popup HisDeathInfo mở.
- [ ] Trong popup, BS bấm Cancel/đóng (chưa lưu HIS_SEVERE_ILLNESS_INFO) → MessageBox "Bạn chưa nhập đầy đủ thông tin Người bệnh nặng xin về" → chặn commit.
- [ ] Trong popup, BS điền + bấm Lưu → API CreateOrUpdate thành công → đóng popup → commit kết thúc điều trị tiếp tục thành công.
- [ ] Config = `"01,TLN"` (nhiều mã). BS chọn "Xin ra viện" (CODE = TLN) → popup mở.
- [ ] Config = `"01"`. BS chọn "Chuyển viện" → KHÔNG hiện popup HisDeathInfo (giữ luồng cũ FormTransfer).
- [ ] BS chọn "Tử vong" + config có "TUVONG" → KHÔNG hiện popup HisDeathInfo (vì check `ID__CHET` ưu tiên trước).

### Edge cases

- [ ] Treatment chưa có HIS_SEVERE_ILLNESS_INFO → popup mở.
- [ ] Treatment đã có HIS_SEVERE_ILLNESS_INFO → `HasValidSevereIllnessInfo` trả true → KHÔNG mở popup, cho commit.
- [ ] API HisSevereIllnessInfo/Get lỗi → chặn commit để an toàn (log Warn).

### PTTK 4.1.2 — Kiểm tra số ICD phụ ra viện

- [ ] Không khai `IsCheckSubIcdExceedLimit` → kết thúc khám với 20 ICD phụ ra viện → lưu thành công, không cảnh báo.
- [ ] `IsCheckSubIcdExceedLimit = "1"`, không khai `IcdSubMaxCount` → nhập 13 ICD phụ ra viện → XtraMessageBox icon Warning "Chẩn đoán phụ ra viện vượt quá 12 mã. Vui lòng kiểm tra lại" → chặn lưu.
- [ ] `IsCheckSubIcdExceedLimit = "1"`, `IcdSubMaxCount = "5"` → nhập 6 ICD phụ ra viện → chặn lưu với ngưỡng 5.
- [ ] `IsCheckSubIcdExceedLimit = "1"`, `IcdSubMaxCount = "5"` → nhập đúng 5 ICD → lưu OK (≤ ngưỡng).
- [ ] `IsCheckSubIcdExceedLimit = "1"`, `IcdSubMaxCount = "abc"` → ngưỡng fallback về 12; nhập 12 ICD → lưu được; nhập 13 ICD → chặn.
- [ ] `IsCheckSubIcdExceedLimit = "2"`, `IcdSubMaxCount = "10"` → nhập 11 ICD → XtraMessageBox icon Question Yes/No "Chẩn đoán phụ ra viện vượt quá 10 mã. Bạn có muốn tiếp tục?" → Yes → lưu / No → giữ form.
- [ ] `IsCheckSubIcdExceedLimit = "1"` → nhập **chẩn đoán phụ phần khám** vượt 12 mã, nhưng ICD phụ ra viện ≤ 12 → KHÔNG cảnh báo (đã bỏ kiểm tra phần khám).
- [ ] Đổi ngôn ngữ sang English → MessageBox hiển thị "The number of discharge sub-diagnoses exceeds N. Please check again" / "...Do you want to continue?".
- [ ] User nhập ICD phụ ra viện qua popup `frmICDInformation` → đếm từ `ShowIcdText` (không phải `IcdSubCode` legacy).
