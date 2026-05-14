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

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/05/2026 | dangth2 | **2608** Bổ sung trigger popup `HIS.Desktop.Plugins.HisDeathInfo` "Thông tin người bệnh nặng xin về" khi BS chọn Loại ra viện thuộc config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES`. Files: `Config/HisConfigCFG.cs` (thêm `MustInputSevereIllnessHomeCodes`), `Base/SevereIllnessHomeWorker.cs` (mới), `ExamServiceReqExecuteControl__Process.cs` (chèn check trong `ProcessTreatmentFinish`), `Resources/Message.Lang.{vi,en,my}.resx` + `ResourceMessage.cs` (`ChuaNhapThongTinBenhNangXinVe`). |

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
