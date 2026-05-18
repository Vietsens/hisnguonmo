# Tạo Loại Vật Tư (MaterialTypeCreate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.MaterialTypeCreate |
| Loại | Form (frmMaterialTypeCreate) |
| Mục đích | Tạo mới / sửa loại vật tư — bao gồm thông tin định danh, đơn vị, giá, BHYT, chính sách giá theo đối tượng, khoa/phòng chặn kê đơn, ánh xạ vật tư, nhà cung cấp. |
| Ngày cập nhật | 2026-04-28 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở form ở chế độ **Tạo mới** hoặc **Sửa** (double-click từ danh sách `MaterialType`).
2. Ở chế độ Tạo mới có thể chọn 1 vật tư mẫu qua `cboMaterialType` để load toàn bộ dữ liệu mẫu xuống form.
3. User chỉnh dữ liệu (mã, tên, đơn vị, giá, BHYT, chính sách giá, khoa/phòng chặn, ánh xạ, nhà cung cấp...).
4. Bấm **Lưu** — tạo qua `api/HisMaterialType/Create` hoặc cập nhật qua `api/HisMaterialType/UpdateSdo`; lưu kèm `HIS_DEPA_PATIENT_TYPE`, `HIS_SERVICE_PATY`, `HIS_MEST_MATY_DEPA`, ánh xạ vật tư.

### Sao chép (PTTK 42762 — BV HAGL)
- Bấm **Sao chép** ở chế độ Sửa hoặc khi đã chọn vật tư mẫu → form CHUYỂN sang chế độ Tạo mới nhưng GIỮ NGUYÊN toàn bộ dữ liệu trên form (chính sách giá, khoa/phòng chặn, ánh xạ, đối tượng…).
- Hành vi `btnCopy_Click`:
  - Reset context: `materialTypeId = null`, `currentVHisMaterialTypeDTODefault = null`, `currentVHisServiceDTODefault = null`, `HisMaterial = null`, `resultData = null`, `ActionType = ActionAdd`.
  - Clear template selector: `cboMaterialType.EditValue = null`, `txtMaterialType.Text = ""`, disable cả hai (đúng pattern Add mode).
  - Disable `rdoUpdateAll/rdoUpdateNotFee` (chỉ dùng khi Sửa).
  - Reset ID + SERVICE_ID = 0 cho mỗi item trong `lsVHisServicePaty` và `depaPatientTypes` để Save tạo bản ghi mới.
  - Set `oldBlockDepartmentIds = oldBlockRoomIds = oldMaterialTypeMapIds = null` để `SaveBlockDepartment / SaveBlockRoom / SaveMaterialTypeMap` tạo quan hệ mới với `resultData.ID` mới.
  - Enable `btnSave`, `btnRefresh`; focus `txtMedicineTypeCode` (= ô nhập mã vật tư) để user sửa Mã trước khi Lưu.

### Quy tắc Enable nút Sao chép
| Tình huống | btnCopy.Enabled |
|-----------|----------------|
| Form mở chế độ Tạo mới + chưa chọn vật tư mẫu | `false` |
| Form mở chế độ Sửa | `true` |
| User chọn vật tư mẫu qua `cboMaterialType` | `true` |
| Sau Sao chép (chưa Lưu) | `false` |
| Sau Lưu thành công | `true` |
| Sau Làm lại (`btnRefresh`) | `false` |

State được tính bằng `UpdateBtnCopyState()`:
```csharp
bool hasMaterialId = materialTypeId.HasValue && materialTypeId.Value > 0;
bool hasResultId   = resultData != null && resultData.ID > 0;
btnCopy.Enabled    = hasMaterialId || hasResultId;
```
Auto-cập nhật bằng cách subscribe vào `cboMaterialType.EditValueChanged`, `btnRefresh.Click`, `btnSave.Click` (chạy SAU handler hiện có).

**Fallback qua `resultData`**: Khác với Medicine (`btnSave_Click` re-assign `currentMedicineTypeId = resultData.ID`), Material's `btnSave_Click` KHÔNG re-assign `materialTypeId` sau Save. Trong scenario **Add mode mới hoàn toàn → Save (Create)**, `materialTypeId` vẫn `null` nên cần fallback qua `resultData.ID` để btnCopy enable lại sau Save.

## 3. EFMODEL / SDO Sử Dụng

| Entity / SDO | Mục đích |
|--------------|---------|
| HIS_MATERIAL_TYPE / V_HIS_MATERIAL_TYPE | Loại vật tư gốc / view |
| V_HIS_SERVICE | Service gắn với loại vật tư |
| HisMaterialTypeSDO | Payload UpdateSdo |
| HIS_SERVICE_PATY / V_HIS_SERVICE_PATY | Chính sách giá theo đối tượng |
| HIS_DEPA_PATIENT_TYPE | Đối tượng theo khoa |
| HIS_MEST_MATY_DEPA | Khoa/phòng chặn |
| HIS_EXP_MEST_TYPE | Loại phiếu xuất chặn |
| HIS_SUPPLIER | Nhà cung cấp |
| VHisServicePatyADO (ADO/) | ADO chính sách giá hiển thị grid |

## 4. UI Layout

```
+--------------------------------------------------------------+
| [txtMaterialType] [cboMaterialType (chọn vật tư mẫu)]       |
| [Thông tin định danh — txtMedicineTypeCode / Name / ...]    |
| [Đơn vị, giá, BHYT, ánh xạ vật tư]                           |
| [Chính sách giá — gridControlServicePaty]                    |
| [Khoa/phòng chặn — cboBlockDepartment / cboBlockRoom]        |
| [Loại phiếu xuất chặn — cboBlockExpMestType]                 |
| [Nhà cung cấp — cboSupplier]                                 |
+--------------------------------------------------------------+
| [btnRefresh Làm lại] [btnCopy Sao chép] [btnSave Lưu]        |
+--------------------------------------------------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo mới | api/HisMaterialType/Create | MosConsumer |
| Cập nhật (SDO) | api/HisMaterialType/UpdateSdo | MosConsumer |
| Lấy view | api/HisMaterialType/GetView | MosConsumer |
| Tạo / xóa danh sách đối tượng theo khoa | api/HisDepaPatientType/CreateList, DeleteList | MosConsumer |
| Tạo / cập nhật chính sách giá | api/HisServicePaty/CreateList, UpdateList | MosConsumer |
| Tạo khoa/phòng chặn | api/HisMestMatyDepa/CreateByMaterial | MosConsumer |

## 6. Dependencies

### Library Plugins
- `HIS.UC.National` — chọn quốc gia (qua `NationalProcessor`).

### Inter-Plugin
- Không trực tiếp mở plugin khác.

## 7. Print

Plugin không có chức năng in trực tiếp.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-06 | dangth2 / Claude | PTTK 42762 (BV HAGL): Fix UX **"luồng cũ sửa liên tục"** sau Sao chép. Trước đây sau khi Sao chép + Save (Create), `ActionType` vẫn là `Add` → user pick vật tư khác qua `cboMaterialType` → load data nhưng Save sẽ **Create duplicate** thay vì Update. Sửa: trong `btnSave_Click` khi `resultData != null`, thêm `this.materialTypeId = resultData.ID;` (Material trước đây không re-assign sau Save) và `this.ActionType = ActionEdit;`. Hiệu ứng: sau Save thành công (cả Update lẫn Create), form chuyển về Edit mode focus vào record vừa lưu → user pick vật tư khác qua combo → Edit vật tư đó (Update khi Save). Sao chép tiếp khôi phục Add mode khi user click `btnCopy`. |
| 2026-04-28 | dangth2 / Claude | PTTK 42762 (BV HAGL): Fix bug **Add mode + chọn vật tư mẫu KHÔNG fill data**. `SetDataToControl()` trước đây chỉ load data khi `ActionType == Edit` → trong Add mode chọn template thì rơi vào `SetNullToSpinControl` thay vì fill. Sửa: load data khi `materialTypeId > 0` (bất kể ActionType); chỉ giữ behavior Edit-only (`btnRefresh.Enabled = false`, `chkIsBusiness.Checked = false`) trong nhánh `if (ActionType == Edit)` riêng. |
| 2026-04-28 | dangth2 / Claude | PTTK 42762 (BV HAGL): Enable `cboMaterialType` + `txtMaterialType` ở **CẢ Add lẫn Edit mode** (trước đây chỉ Edit) để user mở thẳng "Tạo mới" cũng chọn được vật tư mẫu. Sửa: (1) `SetDataToControl()` — bỏ if/else, set Enabled=true cho cả 2 mode (giữ riêng `chkIsBusiness.Checked` theo ActionType). (2) `btnRefresh_Click` — set `cboMaterialType.Enabled=true` thay vì false (đồng thời fix typo cũ `cboMaterialType.EditValue = false` → `cboMaterialType.Enabled = true`). (3) `frmMaterialTypeCreate__Plus__Copy.cs / ResetTemplateSelectorControls()` — set Enabled=true sau Sao chép. |
| 2026-04-28 | dangth2 / Claude | PTTK 42762 (BV HAGL): Thêm chức năng **Sao chép vật tư từ danh mục**. (1) Designer đã có sẵn `btnCopy` (Text="Sao chép"). (2) Thêm key `frmMaterialTypeCreate.btnCopy.Text` vào `Resources/Lang.vi.resx`, `Lang.En.resx`. (3) Thêm dòng load text `btnCopy.Text` vào `frmMaterialTypeCreate_InitResource.cs`. (4) Tạo partial class `frmMaterialTypeCreate__Plus__Copy.cs` với `WireBtnCopy / btnCopy_Click / UpdateBtnCopyState` và 5 helper reset (template selector, edit-mode buttons, service paty, depa patient types, old block & map IDs). Click handler reset context (ID, ActionType→Add, currentVHis*DTO, HisMaterial, resultData), giữ nguyên dữ liệu form, đặt ID=0 cho `lsVHisServicePaty` & `depaPatientTypes`, set `oldBlock*Ids / oldMaterialTypeMapIds = null` để Save tạo bản ghi mới. (5) Gọi `WireBtnCopy()` ở cuối `frmMaterialTypeCreate_Load`. (6) Đăng ký file mới vào `.csproj`. Auto-update Enabled state qua subscribe vào `cboMaterialType.EditValueChanged / btnRefresh.Click / btnSave.Click`. |
| 2026-04-28 | dangth2 / Claude | Fix edge case: Material's `btnSave_Click` KHÔNG re-assign `materialTypeId = resultData.ID` sau Save (khác Medicine). Hệ quả: scenario `Add mode mới → Save (Create)` → `materialTypeId` vẫn `null` → `btnCopy` bị disable nhầm. Fix: `UpdateBtnCopyState()` thêm fallback `\|\| (resultData != null && resultData.ID > 0)`. Test lại các flow Edit→Save / Add+template→Save / Add mới→Save / Sao chép→Save / Refresh — tất cả enable đúng theo state. |

## 9. Test Cases (PTTK 42762)

### Mở form trực tiếp (Tạo mới)
- [ ] `btnCopy.Enabled = false` ban đầu.
- [ ] `cboMaterialType` enable; chọn 1 vật tư mẫu → form load toàn bộ dữ liệu.
- [ ] Sau khi chọn vật tư mẫu → `btnCopy.Enabled = true`.

### Mở form từ danh sách (Sửa)
- [ ] `btnCopy.Enabled = true` ngay khi Load xong.
- [ ] Bấm Sao chép → context reset, dữ liệu giữ nguyên, focus về `txtMedicineTypeCode`.
- [ ] Sửa `MATERIAL_TYPE_CODE` → bấm Lưu → tạo bản ghi mới qua `Create`, KHÔNG ảnh hưởng bản ghi gốc.
- [ ] Sau Lưu thành công, `btnCopy.Enabled = true`.

### Sao chép → Làm lại
- [ ] Bấm Sao chép → bấm Làm lại → form trống, `btnCopy.Enabled = false`.

### Lưu chính sách giá / khoa chặn sau Sao chép
- [ ] `lsVHisServicePaty` items có `ID = 0` → POST `HisServicePaty/CreateList` với `SERVICE_ID` mới.
- [ ] `depaPatientTypes` items có `ID = 0` → POST `HisDepaPatientType/CreateList` với SERVICE_ID mới.
- [ ] `oldBlockDepartmentIds`/`oldBlockRoomIds` = null → SaveBlockDepartment / SaveBlockRoom POST `HisMestMatyDepa/CreateByMaterial` với MaterialTypeId = resultData.ID mới.

### Đa ngôn ngữ
- [ ] Switch language vi → "Sao chép". Switch en → "Copy".
