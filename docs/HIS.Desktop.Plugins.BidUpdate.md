# BidUpdate — Sửa Gói Thầu

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BidUpdate |
| Loại | Form |
| Mục đích | Sửa thông tin gói thầu (HIS_BID) + 3 sub-list: thuốc / vật tư / máu trong thầu |
| Module ID | 255 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở danh sách thầu → chọn 1 gói thầu cần sửa → mở form Sửa thầu (frmBidUpdate)
2. Form load thông tin master (HIS_BID) + 3 sub-list (thuốc/vật tư/máu) qua API GetView
3. User sửa từng dòng — bấm "Cập nhật" để commit thay đổi vào list local (KHÔNG gọi API)
4. User có thể Import từ file Excel để bổ sung dòng mới
5. User bấm "Lưu" (Ctrl+S) → gọi `POST api/HisBid/Update` với toàn bộ payload

### Điều kiện nghiệp vụ
- Mặc định: dòng có `AMOUNT > 0` bắt buộc; `AMOUNT = 0` chỉ được nếu có `ADJUST_AMOUNT > 0`
- Khi config `MOS.HIS_BID.ALLOW_ZERO_AMOUNT_IMPORT = 1`: cho phép dòng `AMOUNT = 0` và trùng mã thuốc/vật tư (phục vụ xuất XML TT12 BHYT)
- Khi sửa: dòng nhập thuốc/vật tư đã có giao dịch (IMP/EXP) cần kiểm tra `Min_AMOUNT` để không bị âm tồn

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_BID | Table | Master gói thầu |
| HIS_BID_MEDICINE_TYPE | Table | Dòng thuốc trong thầu |
| HIS_BID_MATERIAL_TYPE | Table | Dòng vật tư trong thầu |
| HIS_BID_BLOOD_TYPE | Table | Dòng máu trong thầu |
| V_HIS_BID_MEDICINE_TYPE | View | Load dòng thuốc lên UI |
| V_HIS_BID_MATERIAL_TYPE | View | Load dòng vật tư lên UI |
| V_HIS_BID_BLOOD_TYPE | View | Load dòng máu lên UI |
| HIS_CONFIG | Table | Đọc config `MOS.HIS_BID.ALLOW_ZERO_AMOUNT_IMPORT` |

## 4. UI Layout

### Sơ đồ giao diện
```
+------------------------------------------------------------+
| [Master thầu: Mã, Tên, Hình thức, Năm, VALID_FROM-TO]      |
+------------------------------------------------------------+
| [Tab: Thuốc] [Tab: Vật tư] [Tab: Máu]                      |
+------------------------------------------------------------+
| Form trái:                  | Grid phải:                   |
| Mã/tên thuốc, SL, giá,      | List records đang sửa        |
| CSKCB chuyển, ...           |                              |
| [Cập nhật] [Bỏ qua]         |                              |
+------------------------------------------------------------+
| [Import Excel] [Lưu (Ctrl+S)] [Hủy]                         |
+------------------------------------------------------------+
```

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.MediOrgPicker | Picker chọn mã CSKCB chuyển |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Update thầu + sub-list | `api/HisBid/Update` | MosConsumer |
| Load thuốc | `api/HisBidMedicineType/GetView` | MosConsumer |
| Load vật tư | `api/HisBidMaterialType/GetView` | MosConsumer |
| Load máu | `api/HisBidBloodType/GetView` | MosConsumer |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở |
|-------------|-----------|
| HIS.UC.MediOrgPicker | Bấm `+` ô CSKCB chuyển |

## 7. Config

| Config Key | Mặc định | Tác dụng |
|------------|----------|----------|
| `MOS.HIS_MEDICINE.IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT` | 0 | Set thông tin BHYT mặc định khi thêm thuốc |
| `MOS.HIS_BID.ALLOW_ZERO_AMOUNT_IMPORT` | 0 (tắt) | Bật: cho phép import dòng SL=0, cho phép trùng mã trong thầu (TT12 BHYT) |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|----------------|
| 2026-05-12 | phuongnm | **PTTK_43931** — Hỗ trợ import thầu đặc biệt theo TT12 BHYT.<br>• Thêm config key `MOS.HIS_BID.ALLOW_ZERO_AMOUNT_IMPORT` vào `Config/HisConfigCFG.cs`<br>• Khi config bật: bỏ check trùng mã thuốc/vật tư trong `BtnImport_Click` ([frmBidUpdate_Import.cs:57-72](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate_Import.cs#L57))<br>• Khi config bật: bỏ validate `AMOUNT=0 && ADJUST_AMOUNT=0` ở Save ([frmBidUpdate.cs:3414](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate.cs#L3414))<br>• Vẫn giữ check `AMOUNT < 0` (số âm vẫn cấm) |
| 2026-05-15 | phuongnm | **PTTK_43931 — quyết định Save-only validate (theo tester)**.<br>• KHÔNG thêm check `AMOUNT` ở tầng Import Excel (`addListMedicineTypeToProcessList`, `addListMaterialTypeToProcessList`, `BtnImport_Click`). Tất cả dòng (kể cả `AMOUNT < 0` hoặc `= 0`) được đẩy thẳng vào grid.<br>• Lý do: bước Lưu (`CheckValidDataInGridService`) đã chặn `AMOUNT < 0` ([frmBidUpdate.cs:3409](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate.cs#L3409)) và `AMOUNT=0 && ADJUST=0` khi config tắt ([:3417](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate.cs#L3417)). 2 MessageBox liên tiếp gây phiền cho user.<br>• Phần Import chỉ giữ check duplicate (đã có, đúng theo PTTK_43931 entry 12/05). |
| 2026-05-15 | phuongnm | **Pre-validate UK1 ở Save — chặn trước khi BE trả MOS348**.<br>• Thêm method `CheckDuplicateUK1(List<MedicineTypeADO>)` tại [frmBidUpdate_Validation.cs:502](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate_Validation.cs#L502) — group 3 loại (thuốc/vật tư/máu) theo UK1 đầy đủ, hiện cảnh báo nếu có dòng trùng.<br>• Wire vào `btnSave_Click` sau `CheckBatchDivisionCodeAllRules`, trước `getDataForProcess()` ([frmBidUpdate.cs:2927](../HIS/Plugins/HIS.Desktop.Plugins.BidUpdate/frmBidUpdate.cs#L2927)).<br>• UK1 thuốc: MEDICINE_TYPE_ID + BID_NUM_ORDER + TT_THAU + SUPPLIER_ID + MEDICINE_USE_FORM_ID + REGISTER_NUMBER + PACKING_TYPE_ID + CONCENTRA + DOSAGE_FORM.<br>• UK1 vật tư: MATERIAL_TYPE_ID (+ flag IsMaterialTypeMap) + BID_NUM_ORDER + TT_THAU + SUPPLIER_ID + CONCENTRA.<br>• UK1 máu: BLOOD_TYPE_ID + BID_NUM_ORDER + SUPPLIER_ID.<br>• Message format: `TrungUK_Title` + 3 format keys `TrungUK_{Thuoc,VatTu,Mau}_Format` đã thêm vào Message.Lang.{vi,en}.resx và `ResourceMessage.cs`.<br>• Check chạy ở Save (không ở Import) — theo decision Save-only của entry trên. |

## 9. Test Cases

### PTTK_43931 — Config bật (`ALLOW_ZERO_AMOUNT_IMPORT = 1`)
- [ ] Import file Excel có dòng SL = 0 → dòng được append vào grid, KHÔNG có MessageBox cảnh báo
- [ ] Import file Excel có dòng SL âm → dòng được append vào grid, KHÔNG có MessageBox (validate dồn về Lưu)
- [ ] Import file Excel có dòng trùng mã với records hiện có → dòng được append (cho phép trùng)
- [ ] Bấm Lưu khi grid có dòng SL = 0 và ADJUST_AMOUNT = 0 → Save thành công, không báo lỗi
- [ ] Bấm Lưu khi grid có 2 dòng cùng mã thuốc + cùng nhà thầu + cùng nhóm thầu → Save thành công, không báo "bị lặp lại"
- [ ] Bấm Lưu khi grid có dòng SL âm → vẫn báo lỗi "Số lượng không được âm"

### Config tắt (default, `ALLOW_ZERO_AMOUNT_IMPORT != 1`)
- [ ] Import file Excel có dòng SL = 0 hoặc âm → dòng được append vào grid, KHÔNG có MessageBox (validate dồn về Lưu)
- [ ] Import file Excel có dòng trùng mã → dòng vào listError, hiện cảnh báo "bị trùng"
- [ ] Bấm Lưu khi grid có dòng SL = 0 và ADJUST_AMOUNT = 0 → báo lỗi "bắt buộc phải nhập số lượng điều tiết"
- [ ] Bấm Lưu khi grid có 2 dòng cùng mã thuốc + cùng nhà thầu + cùng nhóm thầu → báo "Thuốc {tên} bị lặp lại"
- [ ] Hành vi như trước khi sửa (không bị ảnh hưởng)

### Pre-validate UK1 ở Save (2026-05-15)
- [ ] Thêm 2 dòng thuốc TRÙNG đủ 9 trường UK1 (MEDICINE_TYPE_ID + BID_NUM_ORDER + TT_THAU + SUPPLIER_ID + MEDICINE_USE_FORM_ID + REGISTER_NUMBER + PACKING_TYPE_ID + CONCENTRA + DOSAGE_FORM) → bấm Lưu → MessageBox "Phát hiện dữ liệu trùng UK..." hiện trước khi gọi BE, KHÔNG nhận MOS348
- [ ] Thêm 2 dòng thuốc cùng MEDICINE_TYPE_ID nhưng khác MEDICINE_USE_FORM_ID → Lưu thành công (không phải UK1)
- [ ] Thêm 2 dòng vật tư trùng MATERIAL_TYPE_ID + BID_NUM_ORDER + TT_THAU + SUPPLIER_ID + CONCENTRA → bấm Lưu → cảnh báo "Trùng UK vật tư..."
- [ ] Thêm 1 dòng MATERIAL_TYPE_ID + 1 dòng MATERIAL_TYPE_MAP_ID cùng giá trị → KHÔNG bị coi là trùng (key tách bằng IsMaterialTypeMap)
- [ ] Thêm 2 dòng máu trùng BLOOD_TYPE_ID + BID_NUM_ORDER + SUPPLIER_ID → bấm Lưu → cảnh báo "Trùng UK máu..."
- [ ] Message liệt kê cụ thể mã + các trường UK + số dòng trùng (không phải thông báo chung chung)
- [ ] Chạy với culture en-US → message hiển thị tiếng Anh (TrungUK_*_Format có bản en)
- [ ] KHÔNG có check UK1 ở bước Import Excel (chỉ ở Save)
