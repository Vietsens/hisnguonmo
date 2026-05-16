# HIS.Desktop.Plugins.ExamServiceReqExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExamServiceReqExecute |
| Loại | UserControl |
| Mục đích | Xử lý phòng khám — bác sĩ nhập kết quả khám, chẩn đoán (ICD chính + phụ), chỉ định DV, kết thúc khám/ra viện cho bệnh nhân tại phòng khám. |
| Path | HIS/Plugins/HIS.Desktop.Plugins.ExamServiceReqExecute |
| Trạng thái | Đang bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
```
Tiếp nhận BN → Phòng khám hiển thị danh sách BN
  → BS chọn BN → Load thông tin khám
    → Nhập DHST, cân nặng, chiều cao
    → Nhập chẩn đoán ICD chính + phụ
    → Chỉ định DV (XN, CĐHA, kê đơn)
    → Kết thúc khám (chkTreatmentFinish)
      → Hiện UC HIS.UC.ExamTreatmentFinish (kết thúc điều trị/ra viện)
      → Validate (gồm kiểm tra số ICD phụ ra viện)
    → Lưu
    → In phiếu khám / đơn thuốc / giấy ra viện (nếu có)
```

### Điều kiện nghiệp vụ
- BS chỉ thao tác trên BN đang ở phòng khám của mình
- Bắt buộc nhập ICD chính khi kết thúc khám (theo cấu hình `CheckIcdWhenSave`)
- Khi kết thúc điều trị/ra viện: ICD phụ ra viện có thể bị giới hạn số lượng theo cấu hình `IsCheckSubIcdExceedLimit` (mặc định 12)
- Sau kết thúc khám: tùy `AutoExitAfterFinish` có thể tự đóng UC

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE_REQ | View | Yêu cầu khám đang xử lý |
| HIS_SERVICE_REQ | Table | Tạo/cập nhật yêu cầu DV chỉ định |
| V_HIS_TREATMENT | View | Thông tin điều trị của BN |
| HIS_TREATMENT | Table | Cập nhật thông tin ra viện |
| V_HIS_SERE_SERV | View | DV đã thực hiện trong lần khám |
| HIS_ICD | Table | Danh mục ICD chính |
| HIS_ICD_GROUP | Table | Nhóm bệnh |
| V_HIS_PATIENT_TYPE_ALTER | View | Thẻ BHYT của BN |
| V_HIS_ROOM | View | Phòng khám / phòng làm việc |

## 4. UI Layout

### Sơ đồ giao diện
```
+--------------------------------------------------------------+
| Thông tin BN | Mã | Họ tên | Năm sinh | Giới | ĐT BHYT      |
+--------------------------------------------------------------+
| DHST (uc) | Cân nặng | Chiều cao | Nhiệt độ | Mạch | HA      |
+--------------------------------------------------------------+
| ICD chính (UC) | ICD phụ | Lý do vào viện | Quá trình bệnh   |
+--------------------------------------------------------------+
| Khám lâm sàng | Khám cận lâm sàng | Tóm tắt                  |
+--------------------------------------------------------------+
| [ ] Kết thúc khám → mở UC HIS.UC.ExamTreatmentFinish         |
| [ ] Kết thúc điều trị / Ra viện                              |
+--------------------------------------------------------------+
| [Lưu] [Lưu+In] [In phiếu] [Kê đơn] [Chỉ định DV]            |
+--------------------------------------------------------------+
```

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.Icd | ICD chính |
| HIS.UC.SecondaryIcd | ICD phụ (chấp nhận nhiều mã) |
| HIS.UC.DHST | Chỉ số sinh tồn |
| HIS.UC.NextTreatmentInstruction | Hướng xử trí tiếp theo |
| HIS.UC.ExamTreatmentFinish | Kết thúc khám / ra viện (popup) |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Get yêu cầu khám | api/HisServiceReq/Get | MosConsumer |
| Update kết thúc khám | api/HisServiceReq/UpdateExamFinish | MosConsumer |
| Update treatment ra viện | api/HisTreatment/UpdateEnd | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.UC.ExamTreatmentFinish | Form ra viện embedded |
| HIS.Desktop.Plugins.Library.EmrGenerate | Tạo input ký số EMR cho phiếu |
| HIS.Desktop.Plugins.Library.PrintPrescription | In đơn thuốc |
| HIS.Desktop.Plugins.Library.PrintBordereau | In phiếu thanh toán |

### Inter-Plugin (mở plugin khác)
| Plugin đích | Khi nào mở |
|-------------|-----------|
| HIS.Desktop.Plugins.ContentSubclinical | Khi BS chỉ định DV CLS |
| HIS.Desktop.Plugins.AssignPrescriptionPK | Khi BS kê đơn thuốc |

## 7. Cấu Hình HIS_CONFIG Liên Quan

| Key | Vai trò | Giá trị mặc định |
|-----|--------|-----------------|
| HIS.Desktop.Plugins.CheckIcdWhenSave | "1" chặn / "2" cảnh báo ICD sai khi lưu | Không kiểm |
| HIS.Desktop.Plugins.ExamServiceReqExecute.AutoExitAfterFinish | Tự đóng UC sau kết thúc khám | Không tự đóng |
| HIS.Desktop.Plugins.ExamServiceReqExecute.IsEnableEditStartTime | Cho phép sửa thời gian bắt đầu khám | Không cho |
| HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit | "1" chặn / "2" cảnh báo khi ICD phụ ra viện vượt ngưỡng | Không kiểm |
| HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit.IcdSubMaxCount | Ngưỡng tối đa số mã ICD phụ ra viện | 12 |
| MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY | Bật phân loại cấp cứu | Không bật |

### Chi tiết kiểm tra số ICD phụ ra viện (PTTK 4.1.2)

- **Plugin**: `HIS.Desktop.Plugins.ExamServiceReqExecute`
- **Path / Phạm vi áp dụng**: chỉ kiểm tra ICD phụ **ra viện** do `HIS.UC.ExamTreatmentFinish` nhập. **KHÔNG** kiểm tra ICD phụ của phần khám.
- **Thay đổi**: Thêm đọc cấu hình `IcdSubMaxCount`; thay hằng số 12 bằng ngưỡng động cho kiểm tra ICD phụ ra viện.
- **Vị trí code**: `ExamServiceReqExecuteControl.cs` → `ValidIcdLen()` (~dòng 3905), được gọi từ `ExamServiceReqExecuteControl__Process.cs:1340` ngay sau `treatmentFinishProcessor.Validate(...)`.

#### Hai cấu hình phối hợp

| KEY | Vai trò | Khi không khai báo |
|-----|--------|--------------------|
| `HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit` | Bật/tắt kiểm tra; chế độ chặn ("1") hoặc cảnh báo ("2") | Không kiểm tra |
| `HIS.Desktop.Plugins.IsCheckSubIcdExceedLimit.IcdSubMaxCount` | Ngưỡng tối đa số ICD phụ | Ngưỡng mặc định = **12** |

#### Thay đổi giao diện

Không thay đổi giao diện.

#### Thay đổi xử lý

| Bước | Điều kiện | Hành vi |
|------|-----------|---------|
| 1 | `IsCheckSubIcdExceedLimit` không phải "1" hoặc "2" | Bỏ qua kiểm tra, tiếp tục lưu bình thường |
| 2 | Đọc `IcdSubMaxCount` | Số nguyên dương hợp lệ → ngưỡng = giá trị đó; ngược lại → ngưỡng = **12** |
| 3 | Số ICD phụ ra viện ≤ ngưỡng | Tiếp tục lưu bình thường |
| 4a | Số ICD phụ ra viện > ngưỡng và `IsCheckSubIcdExceedLimit` = "1" | Thông báo lỗi, chặn lưu |
| 4b | Số ICD phụ ra viện > ngưỡng và `IsCheckSubIcdExceedLimit` = "2" | Hiển thị cảnh báo: "Chẩn đoán phụ ra viện vượt quá {N} mã. Bạn có muốn tiếp tục không?" |
| 4b-i | Người dùng chọn "Có" | Tiếp tục lưu |
| 4b-ii | Người dùng chọn "Không" | Hủy lưu, giữ nguyên màn hình |

- Source ICD phụ ra viện: `treatmentFinishProcessor.GetValue(ucTreatmentFinish)` trả về `ExamTreatmentFinishResult.TreatmentFinishSDO`. Lấy chuỗi ICD phụ theo thứ tự ưu tiên (lấy field đầu tiên không rỗng):
  1. `ShowIcdText` — chuỗi ICD phụ hiển thị trên giấy ra viện, do user nhập qua popup `frmICDInformation` ("Thông tin chuẩn đoán hiển thị trên giấy ra viện"). **Đây là nguồn chính** vì là dữ liệu thực sự in lên giấy ra viện của BN.
  2. `ShowIcdSubCode` — fallback khi popup chỉ chọn mã ICD mà không có text tổng hợp.
  3. `IcdSubCode` — legacy field, đa số case = null, giữ làm fallback cuối cho tương thích.
- Sau khi có chuỗi → tách theo `icdSeparators` (`;`) → đếm số phần tử không rỗng → so với ngưỡng.
- Message dùng resource đa ngôn ngữ (vi/en/my): `Plugin_ExamServiceReqExecute__ChanDoanPhuRaVienVuotQuaSoLuongChan` (chặn) / `Plugin_ExamServiceReqExecute__ChanDoanPhuRaVienVuotQuaSoLuongCanhBao` (cảnh báo). Format `{0}` = ngưỡng động.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 15/05/2026 | tuanln | Tạo docs module |
| 15/05/2026 | tuanln | PTTK 4.1.2: Thêm đọc cấu hình `IcdSubMaxCount`; thay hằng số 12 bằng ngưỡng động cho kiểm tra ICD phụ ra viện. Chỉ kiểm tra ICD phụ ra viện ở `HIS.UC.ExamTreatmentFinish` (bỏ kiểm tra ICD phụ phần khám). Đọc qua `HisConfigCFG.IsCheckSubIcdExceedLimit` + `HisConfigCFG.IcdSubMaxCount` (mặc định 12). Message chuyển sang resource đa ngôn ngữ. |
| 16/05/2026 | tuanln | Fix nguồn data: chuyển từ đọc `IcdSubCode` (legacy, hầu hết null) sang ưu tiên `ShowIcdText` (chuỗi ICD nhập qua popup "Thông tin chuẩn đoán hiển thị trên giấy ra viện") → fallback `ShowIcdSubCode` → fallback `IcdSubCode`. Kiểm tra mới phản ánh đúng dữ liệu in trên giấy ra viện. |
| 15/05/2026 | tuanln | **PTTK 4.1.3 (tác động gián tiếp)** — UC `HIS.UC.ExamTreatmentFinish` (dùng cho luồng kết thúc khám của plugin này) bổ sung kiểm tra ngày hẹn khớp `HIS_HOLIDAY_POLICIES`. Khi BS kết thúc điều trị với EndType "Hẹn khám lại" và ngày hẹn rơi vào ngày nghỉ lễ (type 2/3) → cảnh báo Yes/No. Toggle `chkNotCheckT7CN` mở rộng nghĩa thành "Không cảnh báo ngày nghỉ" (bao gồm T7/CN + ngày lễ). Chi tiết tại `docs/HIS.UC.ExamTreatmentFinish.md`. Plugin này không thay đổi code, chỉ ghi nhận tác động. |

## 9. Test Cases

### Cấu hình ICD phụ ra viện (PTTK 4.1.2)

- [ ] Không khai `IsCheckSubIcdExceedLimit` → kết thúc khám với 20 ICD phụ ra viện → lưu thành công, không cảnh báo
- [ ] `IsCheckSubIcdExceedLimit = "1"`, không khai `IcdSubMaxCount` → nhập 13 ICD phụ ra viện → hiện thông báo "Chẩn đoán phụ ra viện vượt quá 12 mã. Vui lòng kiểm tra lại", chặn lưu
- [ ] `IsCheckSubIcdExceedLimit = "1"`, `IcdSubMaxCount = "5"` → nhập 6 ICD phụ ra viện → chặn lưu với ngưỡng 5
- [ ] `IsCheckSubIcdExceedLimit = "1"`, `IcdSubMaxCount = "abc"` → ngưỡng fallback về 12; nhập 12 ICD → lưu được; nhập 13 ICD → chặn
- [ ] `IsCheckSubIcdExceedLimit = "2"`, `IcdSubMaxCount = "10"` → nhập 11 ICD → hiện "Chẩn đoán phụ ra viện vượt quá 10 mã. Bạn có muốn tiếp tục?" → Yes → lưu / No → giữ form
- [ ] `IsCheckSubIcdExceedLimit = "1"` → nhập **chẩn đoán phụ phần khám** vượt 12 mã, nhưng ICD phụ ra viện ≤ 12 → KHÔNG cảnh báo (đã bỏ kiểm tra phần khám)
- [ ] Đổi ngôn ngữ sang English → message hiển thị theo Lang.en
