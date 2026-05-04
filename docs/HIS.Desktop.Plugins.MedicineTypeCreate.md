# Tạo Loại Thuốc (MedicineTypeCreate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.MedicineTypeCreate |
| Loại | Form (frmMedicineTypeCreate) |
| Mục đích | Tạo mới / sửa loại thuốc — bao gồm thông tin định danh, đơn vị, giá, BHYT, chính sách giá theo đối tượng, khoa/phòng chặn kê đơn, chống chỉ định, hoạt chất, ATC. |
| Ngày cập nhật | 2026-04-28 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở form ở chế độ **Tạo mới** (chỉ định mới hoàn toàn) hoặc **Sửa** (double-click từ danh sách `MedicineType`).
2. Ở chế độ Tạo mới có thể chọn 1 thuốc mẫu qua `cboMedicineType` để load toàn bộ dữ liệu mẫu xuống form.
3. User chỉnh dữ liệu (mã, tên, đơn vị, giá, BHYT, chính sách giá, khoa/phòng chặn, chống chỉ định, hoạt chất...).
4. Bấm **Lưu** — tạo qua `api/HisMedicineType/Create` hoặc cập nhật qua `api/HisMedicineType/UpdateSdo`; lưu kèm `HIS_DEPA_PATIENT_TYPE`, `HIS_SERVICE_PATY`, `HIS_MEST_METY_DEPA`.

### Sao chép (PTTK 42762 — BV HAGL)
- Bấm **Sao chép** ở chế độ Sửa hoặc khi đã chọn thuốc mẫu → form CHUYỂN sang chế độ Tạo mới nhưng GIỮ NGUYÊN toàn bộ dữ liệu trên form (chính sách giá, khoa/phòng chặn, chống chỉ định, đối tượng…).
- Hành vi `btnCopy_Click`:
  - Reset context: `currentMedicineTypeId = null`, `currentVHisMedicineTypeDTODefault = null`, `currentVHisServiceDTODefault = null`, `resultData = null`, `ActionType = ActionAdd`.
  - Clear template selector: `cboMedicineType.EditValue = null`, `txtMedicineType.Text = ""`, disable cả hai (đúng pattern Add mode).
  - Disable nút chỉ dùng cho Sửa: `btnDieuChinhLieu`, `btnEditInfo`; set `rdoUpdateAll/rdoUpdateNotFee.ReadOnly = true`.
  - Reset ID + SERVICE_ID = 0 cho mỗi item trong `lsVHisServicePaty` và `depaPatientTypes` để Save tạo bản ghi mới.
  - Set `oldBlockDepartmentIds = oldBlockRoomIds = oldContraindicationSelecteds = null` để `SaveBlockDepartment / SaveBlockRoom / SaveContraindication` tạo quan hệ mới với `resultData.ID` mới.
  - Enable `btnSave`, `btnRefresh`; focus `txtMedicineTypeCode` để user sửa Mã trước khi Lưu.

### Quy tắc Enable nút Sao chép
| Tình huống | btnCopy.Enabled |
|-----------|----------------|
| Form mở chế độ Tạo mới + chưa chọn thuốc mẫu | `false` |
| Form mở chế độ Sửa | `true` |
| User chọn thuốc mẫu qua `cboMedicineType` | `true` |
| Sau Sao chép (chưa Lưu) | `false` |
| Sau Lưu thành công (`currentMedicineTypeId = resultData.ID`) | `true` |
| Sau Làm lại (`btnRefresh`) | `false` |

State được tính bằng `UpdateBtnCopyState()`:
```csharp
btnCopy.Enabled = currentMedicineTypeId.HasValue && currentMedicineTypeId.Value > 0;
```
Auto-cập nhật bằng cách subscribe vào `cboMedicineType.EditValueChanged`, `btnRefresh.Click`, `btnSave.Click` (chạy SAU handler hiện có).

## 3. EFMODEL / SDO Sử Dụng

| Entity / SDO | Mục đích |
|--------------|---------|
| HIS_MEDICINE_TYPE / V_HIS_MEDICINE_TYPE | Loại thuốc gốc / view |
| V_HIS_SERVICE | Service gắn với loại thuốc |
| HisMedicineTypeSDO | Payload UpdateSdo |
| HIS_SERVICE_PATY / V_HIS_SERVICE_PATY | Chính sách giá theo đối tượng |
| HIS_DEPA_PATIENT_TYPE | Đối tượng theo khoa |
| HIS_MEST_METY_DEPA | Khoa/phòng chặn kê đơn |
| HIS_CONTRAINDICATION | Chống chỉ định |
| HIS_ACTIVE_INGREDIENT, HIS_ATC, HIS_ATC_GROUP | Hoạt chất, ATC |
| HIS_DOSAGE_FORM, HIS_PROCESSING_METHOD | Dạng bào chế, sơ chế / phức chế |
| HIS_SOURCE_MEDICINE, HIS_SUPPLIER | Nguồn gốc, nhà cung cấp |
| VHisServicePatyADO (ADO/) | ADO chính sách giá hiển thị grid |

## 4. UI Layout

```
+--------------------------------------------------------------+
| [txtMedicineType] [cboMedicineType (chọn thuốc mẫu)]        |
| [Thông tin định danh — txtMedicineTypeCode / Name / ...]    |
| [Đơn vị, giá, BHYT, hoạt chất, ATC, ...]                     |
| [Chính sách giá theo đối tượng — gridControlServicePaty]    |
| [Khoa/phòng chặn — cboBlockDepartment / cboBlockRoom]        |
| [Chống chỉ định — cboContraindication]                       |
| [Cảnh báo, hướng dẫn, sơ/phức chế]                           |
+--------------------------------------------------------------+
| [btnRefresh Làm lại] [btnCopy Sao chép] [btnSave Lưu]        |
+--------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo mới | api/HisMedicineType/Create | MosConsumer |
| Cập nhật (SDO) | api/HisMedicineType/UpdateSdo | MosConsumer |
| Lấy view | api/HisMedicineType/GetView | MosConsumer |
| Tạo danh sách đối tượng theo khoa | api/HisDepaPatientType/CreateList | MosConsumer |
| Xóa danh sách đối tượng theo khoa | api/HisDepaPatientType/DeleteList | MosConsumer |
| Tạo / cập nhật chính sách giá | api/HisServicePaty/CreateList, UpdateList | MosConsumer |
| Tạo khoa/phòng chặn | api/HisMestMetyDepa/CreateByMedicine | MosConsumer |

## 6. Dependencies

### Library Plugins
- `HIS.UC.National` — chọn quốc gia (qua `NationalProcessor`).

### Inter-Plugin
- Mở popup chỉnh liều `frmDieuChinhLieu` (cùng plugin).
- Mở popup chỉnh sơ/phức chế `frmProcessingMethod` (cùng plugin).
- Mở module `HisMedicineTypeAcin` qua `CallModule` để chọn hoạt chất.
- Mở module `HisProductInfo` qua `GlobalVariables.currentModuleRaws` để xem thông tin sản phẩm.

## 7. Print

Plugin không có chức năng in trực tiếp.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-04-28 | dangth2 / Claude | PTTK 42762 (BV HAGL): Thêm chức năng **Sao chép thuốc từ danh mục**. (1) Designer đã có sẵn `btnCopy` (Text="Sao chép"). (2) Thêm key `frmMedicineTypeCreate.btnCopy.Text` vào `Resources/Lang.vi.resx`, `Lang.En.resx`, `Lang.my.resx`. (3) Thêm dòng load text `btnCopy.Text` vào `frmMedicineTypeCreate__InitResource.cs`. (4) Tạo partial class `frmMedicineTypeCreate__Plus__Copy.cs` với `WireBtnCopy / btnCopy_Click / UpdateBtnCopyState` và 5 helper reset (template selector, edit-mode buttons, service paty, depa patient types, old block IDs). Click handler reset context (ID, ActionType→Add, currentVHis*DTO, resultData), giữ nguyên dữ liệu form, đặt ID=0 cho `lsVHisServicePaty` & `depaPatientTypes`, set `oldBlock*Ids = null` để Save tạo bản ghi mới. (5) Gọi `WireBtnCopy()` ở cuối `frmEditInfoPatient_Load`. (6) Đăng ký file mới vào `.csproj`. Auto-update Enabled state qua subscribe vào `cboMedicineType.EditValueChanged / btnRefresh.Click / btnSave.Click`. |

## 9. Test Cases (PTTK 42762)

### Mở form trực tiếp (Tạo mới)
- [ ] `btnCopy.Enabled = false` ban đầu.
- [ ] `cboMedicineType` enable; chọn 1 thuốc mẫu → form load toàn bộ dữ liệu.
- [ ] Sau khi chọn thuốc mẫu → `btnCopy.Enabled = true`.

### Mở form từ danh sách (Sửa)
- [ ] `btnCopy.Enabled = true` ngay khi Load xong.
- [ ] Bấm Sao chép → context reset, dữ liệu giữ nguyên, focus về `txtMedicineTypeCode`.
- [ ] Sửa `MEDICINE_TYPE_CODE` (vì cùng mã sẽ bị validate trùng) → bấm Lưu → tạo bản ghi mới qua `Create`, KHÔNG ảnh hưởng bản ghi gốc.
- [ ] Sau Lưu thành công, `btnCopy.Enabled = true` (có thể tiếp tục sao chép từ bản vừa tạo).

### Sao chép → Làm lại
- [ ] Bấm Sao chép → bấm Làm lại → form trống, `btnCopy.Enabled = false`.

### Lưu chính sách giá / khoa chặn / chống chỉ định sau Sao chép
- [ ] `lsVHisServicePaty` items có `ID = 0` → POST `HisServicePaty/CreateList` với `SERVICE_ID = resultData.SERVICE_ID` mới.
- [ ] `depaPatientTypes` items có `ID = 0` → POST `HisDepaPatientType/CreateList` với SERVICE_ID mới.
- [ ] `oldBlockDepartmentIds`/`oldBlockRoomIds` = null → SaveBlockDepartment / SaveBlockRoom POST `HisMestMetyDepa/CreateByMedicine` với MedicineTypeId = resultData.ID mới.

### Đa ngôn ngữ
- [ ] Switch language vi → "Sao chép". Switch en → "Copy". Switch my → "ကူးယူပါ".
