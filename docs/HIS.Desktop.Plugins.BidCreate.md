# HIS.Desktop.Plugins.BidCreate — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BidCreate |
| Loại | UC (UserControl) |
| Mục đích | Tạo gói thầu thuốc/vật tư/máu — quản lý thông tin thầu, chi tiết thuốc/vật tư/máu trúng thầu, hỗ trợ import từ Excel |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở UC → load form 3 tab: Thuốc / Vật tư / Máu
2. Nhập thông tin chung gói thầu (tên thầu, QĐ thầu, năm, loại thầu, hiệu lực từ/đến...)
3. Tại từng tab:
   - Chọn thuốc/vật tư/máu từ danh mục → hiển thị các trường chi tiết
   - Nhập số lượng, đơn giá, VAT, nhà thầu, gói thầu, nhóm thầu, mã phần lô, hiệu lực dòng, **CSKCB chuyển**...
   - Nhấn "Bổ sung (Ctrl+A)" → đẩy 1 dòng vào grid xử lý
   - Có thể sửa/xóa từng dòng đã thêm
4. Nhấn "Lưu (Ctrl+S)" → gọi API `HIS_BID_CREATE` lưu gói thầu cùng list chi tiết
5. Tùy chọn: "Tải file mẫu" + "Import" để import dữ liệu từ Excel
6. Tùy chọn: "In (Ctrl+P)" → in phiếu thầu sau khi lưu thành công

### Điều kiện nghiệp vụ
- Mỗi dòng phải có: số lượng > 0, giá nhập >= 0, nhà thầu, gói thầu hợp lệ
- Mã phần lô: tùy chọn (đã bỏ bắt buộc do nhiều gói thầu không có)
- Hiệu lực từ ≤ Hiệu lực đến (cấp tổng và cấp dòng chi tiết)
- CSKCB chuyển: tùy chọn — TextEdit có button "+". User có thể nhập tự do (max 10 ký tự, > 10 ký tự sẽ cảnh báo) hoặc click "+" mở popup `frmTransferMediOrgSelect` để tìm chọn từ danh mục `HIS_MEDI_ORG` (IS_ACTIVE=1, IS_DELETE=0). Sau khi chọn, hệ thống tự ghép `"C." + MEDI_ORG_CODE` vào TextEdit (user có thể sửa prefix `"C."`). Khi import Excel có validate mã trong danh mục

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_BID | Table | Bản ghi gói thầu chính |
| HIS_BID_MEDICINE_TYPE | Table | Chi tiết thuốc trúng thầu (có cột TRANSFER_MEDI_ORG_CODE) |
| HIS_BID_MATERIAL_TYPE | Table | Chi tiết vật tư trúng thầu (có cột TRANSFER_MEDI_ORG_CODE) |
| HIS_BID_BLOOD_TYPE | Table | Chi tiết máu trúng thầu (có cột TRANSFER_MEDI_ORG_CODE) |
| V_HIS_MEDICINE_TYPE | View | Danh mục thuốc |
| V_HIS_MATERIAL_TYPE | View | Danh mục vật tư |
| V_HIS_BLOOD_TYPE | View | Danh mục máu |
| HIS_SUPPLIER | Table | Nhà thầu |
| HIS_MEDI_ORG | Table | Cơ sở khám chữa bệnh (cho CSKCB chuyển) |
| HIS_MANUFACTURER | Table | Hãng sản xuất |
| SDA_NATIONAL | Table | Quốc gia sản xuất |
| HIS_MEDICINE_USE_FORM | Table | Đường dùng |

### Quan hệ chính
- `HIS_BID` → `HIS_BID_MEDICINE_TYPE` (1-n, qua `BID_ID`)
- `HIS_BID` → `HIS_BID_MATERIAL_TYPE` (1-n)
- `HIS_BID` → `HIS_BID_BLOOD_TYPE` (1-n)

## 4. UI Layout

### Sơ đồ giao diện
```
┌──────────────────────────────────────────────────────────────────────┐
│  [Tạo thầu (kho nội trú)]  [Danh sách thầu]  [Tiếp đón 2]            │
├──────────────────────────────────────────────────────────────────────┤
│  ┌─Tab chính─────────────────┐ ┌─Thông tin thầu chung──────────────┐ │
│  │ [Thuốc][Vật tư][Máu]      │ │ Tên thầu | QĐ thầu | Năm | Loại  │ │
│  │ ┌Grid danh mục──────────┐ │ │ Hiệu lực từ/đến | Mã áp thầu      │ │
│  │ │ Mã | Tên | ĐVT | ...  │ │ ├──────────────────────────────────┤ │
│  │ └───────────────────────┘ │ │ STT|Mã|Tên|ĐVT|Số lượng|Giá nhập │ │
│  │                            │ │ (grid thầu chi tiết)              │ │
│  │ ┌Vùng nhập chi tiết──────┐ │ └──────────────────────────────────┘ │
│  │ │ Số lượng|Đơn giá|VAT % │ │                                       │
│  │ │ Vượt thầu|Nhà thầu...  │ │                                       │
│  │ │ Hiệu lực từ/đến (dòng) │ │                                       │
│  │ │ Mã phần lô | Giá trần BHYT | CSKCB chuyển ◄── MỚI THÊM        │
│  │ │ [Bổ sung]              │ │                                       │
│  │ └───────────────────────┘ │ │ [Tải file mẫu][Import][Lưu][In]    │
│  └────────────────────────────┘ └────────────────────────────────────┘
└──────────────────────────────────────────────────────────────────────┘
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.MedicineType | panelControlMedicineType | Grid danh mục thuốc (tab Thuốc) |
| HIS.UC.MaterialType | panelControlMaterialType | Grid danh mục vật tư (tab Vật tư) |
| HIS.UC.BloodType | panelControlBloodType | Grid danh mục máu (tab Máu) |

### Controls chính (cấp dòng chi tiết — `layoutControl2`)
| Control | Vai trò |
|---------|---------|
| spinAmount | Số lượng (Maroon, bắt buộc) |
| spinImpPrice | Đơn giá |
| spinImpVat | VAT % |
| cboSupplier | Nhà thầu (Maroon) |
| txtBidGroupCode | Nhóm thầu |
| txtBidPackageCode | Gói thầu |
| txtBidNumOrder | STT thầu |
| dtItemFromTime / dtItemToTime | Hiệu lực từ/đến của dòng |
| spinGiaTran | Giá trần BHYT |
| txtBatchDivisionCode | Mã phần lô |
| **txtTransferMediOrg** | **CSKCB chuyển — TextEdit có button "+" (max 10 ký tự, tùy chọn). Click "+" mở popup `frmTransferMediOrgSelect` chọn từ danh mục, sau đó tự ghép `"C." + MEDI_ORG_CODE` vào ô** |

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Tạo gói thầu | `HIS_BID_CREATE` (HisRequestUriStore) | MosConsumer | `MOS.EFMODEL.DataModels.HIS_BID` (kèm 3 list chi tiết) |
| Lấy chi tiết thuốc theo BID | `api/HisBidMedicineType/get` | MosConsumer | `HisBidMedicineTypeFilter { BID_ID }` |
| Lấy chi tiết vật tư theo BID | `api/HisBidMaterialType/get` | MosConsumer | `HisBidMaterialTypeFilter { BID_ID }` |

## 6. Dependencies

### Library / Utilities
| Library | Mục đích |
|---------|----------|
| Inventec.Common.ExcelImport | Đọc file Excel khi import |
| HIS.Desktop.LocalStorage.BackendData (BackendDataWorker) | Cache danh mục (HIS_MEDI_ORG, HIS_SUPPLIER...) |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| frmImportError | Khi import Excel có dòng lỗi | `List<MedicineTypeADO>` (lỗi) |
| frmTransferMediOrgSelect | Khi click button "+" của TextEdit "CSKCB chuyển" | — (load HIS_MEDI_ORG từ BackendDataWorker filter IS_ACTIVE=1+IS_DELETE!=1; popup gồm: TextEdit search + Grid Mã/Tên + **TextEdit preview ở dưới grid** (auto-fill `"C."+MEDI_ORG_CODE` khi user click 1 dòng, user có thể sửa prefix) + Button "Chọn (Ctrl S)". Click "Chọn"/Ctrl+S → trả `SelectedTransferCode` từ preview; double-click row → trả `"C."+MEDI_ORG_CODE` và đóng popup) |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu thầu | (set qua MenuPrint config) | RichEditorStore + MpsPrinter | Theo cấu hình |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/05/2026 | anhnh2 | Thêm chức năng "Cơ sở KCB chuyển" cấp dòng chi tiết: TextEdit `txtTransferMediOrg` có button "+" (max 10 ký tự, tùy chọn) đặt cùng dòng "Giá trần BHYT" trong vùng nhập chi tiết — áp dụng cho cả 3 tab Thuốc/Vật tư/Máu. Click button "+" mở popup `frmTransferMediOrgSelect` (search + grid Mã/Tên CSKCB từ `HIS_MEDI_ORG` filter `IS_ACTIVE=1, IS_DELETE=0`); user click chọn / double-click / Ctrl+S → tự ghép `"C." + MEDI_ORG_CODE` vào TextEdit (user có thể sửa prefix `"C."`). Validate khi Bổ sung/Cập nhật: nếu length > 10 ký tự cảnh báo "Mã CSKCB chuyển tối đa 10 ký tự". Lưu giá trị (đã ghép) vào field `TRANSFER_MEDI_ORG_CODE` của 3 EFMODEL `HIS_BID_MEDICINE_TYPE` / `HIS_BID_MATERIAL_TYPE` / `HIS_BID_BLOOD_TYPE`. Bổ sung import Excel cho 3 trường `FROM_TIME`, `TO_TIME` (dạng số `yyyyMMddHHmmss`) và `TRANSFER_MEDI_ORG_CODE` (dạng chuỗi mã CSKCB). Validate khi import: nếu mã CSKCB không tồn tại trong `HIS_MEDI_ORG` thì thêm dòng vào danh sách lỗi với message "Mã CSKCB chuyển không chính xác". Cập nhật key đa ngôn ngữ `LCI_TRANSFER_MEDI_ORG` + ResourceMessage `MaCSKCBChuyenToiDa10KyTu`. |

## 9. Test Cases

### Tạo gói thầu mới với CSKCB chuyển
- [ ] Mở UC → ô "CSKCB chuyển" hiện rỗng tại vùng chi tiết (cùng dòng "Giá trần BHYT") + button "+" bên cạnh
- [ ] **Nhập tay**: gõ mã CSKCB (≤10 ký tự) → giá trị lưu vào `txtTransferMediOrg.Text`
- [ ] **Click button "+"**: mở popup "Tìm chọn CSKCB":
  - Search keyword → grid filter realtime
  - **Click 1 dòng**: TextEdit preview ở dưới grid tự điền `"C." + MEDI_ORG_CODE` (Default `"C."`)
  - **Sửa prefix trong preview** (ví dụ "C." → "X.") → bấm "Chọn (Ctrl+S)" → ô "CSKCB chuyển" hiển thị giá trị đã sửa
  - **Double-click 1 dòng**: lấy luôn `"C." + MEDI_ORG_CODE` (skip bước sửa prefix)
- [ ] User có thể sửa lại sau khi popup đóng (TextEdit "CSKCB chuyển" cho phép edit)
- [ ] **Validate**: nhập > 10 ký tự → click "Bổ sung" hiện cảnh báo "Mã CSKCB chuyển tối đa 10 ký tự"
- [ ] Nhấn "Bổ sung" → dòng được thêm vào grid xử lý, lưu giữ `TRANSFER_MEDI_ORG_CODE`
- [ ] Nhấn "Lưu" → API nhận đúng `TRANSFER_MEDI_ORG_CODE` trong DTO của 3 list chi tiết
- [ ] Để trống "CSKCB chuyển" → vẫn lưu được (không bắt buộc)

### Sửa dòng đã có
- [ ] Click icon "Sửa" trong grid xử lý → form load lại, `txtTransferMediOrg.Text` = giá trị cũ
- [ ] Đổi giá trị → nhấn "Cập nhật" → dòng grid update, lưu được giá trị mới

### Reset / Mới
- [ ] Nhấn "Hủy" hoặc "Mới" → `txtTransferMediOrg.Text` về rỗng
- [ ] Chuyển tab Thuốc → Vật tư → Máu → control reset đúng

### Import Excel
- [ ] File Excel có cột `TRANSFER_MEDI_ORG_CODE` chứa mã hợp lệ → các dòng vào grid với CSKCB chuyển đúng
- [ ] Mã CSKCB không tồn tại trong `HIS_MEDI_ORG` → dòng vào danh sách lỗi với message "Mã CSKCB chuyển không chính xác: {mã}"
- [ ] Cột Excel `FROM_TIME` / `TO_TIME` (chuỗi `dd/MM/yyyy`) → parse vào FROM_TIME_STR / TO_TIME_STR → convert sang long → set vào FROM_TIME / TO_TIME
- [ ] FROM_TIME > TO_TIME → dòng vào danh sách lỗi với message "Hiệu lực từ không được lớn hơn hiệu lực đến (cấp dòng chi tiết)"
- [ ] Bỏ trống TRANSFER_MEDI_ORG_CODE trong Excel → import vẫn thành công, dòng không có CSKCB chuyển

### Đa ngôn ngữ
- [ ] Chuyển ngôn ngữ EN → caption "CSKCB chuyển:" → "Transfer Medical Org:"

### Backend prerequisites (BẮT BUỘC trước khi deploy)
- [ ] Chạy migration thêm cột `TRANSFER_MEDI_ORG_CODE VARCHAR2(20)` vào 3 bảng `HIS_BID_MEDICINE_TYPE`, `HIS_BID_MATERIAL_TYPE`, `HIS_BID_BLOOD_TYPE`
- [ ] Rebuild project `mrs/MOS.EFMODEL` (source đã update) → copy `MOS.EFMODEL.dll` mới vào `LIB/MOS/`
- [ ] Cập nhật API `HIS_BID_CREATE` (backend mrs) lưu trường `TRANSFER_MEDI_ORG_CODE` vào DB cho 3 entity
