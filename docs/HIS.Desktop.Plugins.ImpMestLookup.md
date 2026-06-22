# Tra Cứu Phiếu Nhập — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ImpMestLookup |
| Loại | Form (Popup) |
| Mục đích | Tra cứu nhanh 1 phiếu nhập theo Mã nhập (so khớp chính xác) và xem chi tiết Thuốc/Vật tư/Máu của phiếu. Clone từ plugin Chi tiết nhập (HIS.Desktop.Plugins.ImpMestViewDetail) — chỉ tra cứu/xem, không thay đổi dữ liệu. |
| Người tạo | sinhnt (theo PTTK 2726 / việc 42888 — BV HAGL) |
| Ngày tạo | 10/06/2026 |
| Trạng thái | Đang phát triển |

Nguồn yêu cầu: **PTTK_42888 — BV HAGL - Thêm chức năng Tra cứu phiếu nhập đã nhập** (tài liệu 2726).

## 2. Quy Trình Nghiệp Vụ

Plugin có **2 chế độ vận hành**:

### Chế độ Tra cứu (mở từ menu)
- Tiêu đề popup: **"Tra cứu phiếu nhập"**.
- Ô **Mã nhập** cho phép nhập; các trường còn lại để trống và chỉ đọc.
- Hiển thị nút **Làm mới**; ẩn các nút thay đổi dữ liệu (Lưu / Duyệt / Thực nhập / Hội đồng kiểm nhập).
- Focus tự động vào ô Mã nhập.

Luồng nhập **Mã nhập + Enter**:
1. Cắt khoảng trắng đầu/cuối.
2. Nếu rỗng → cảnh báo trường bắt buộc tại ô Mã nhập, **không** gọi tra cứu.
3. Nếu < 12 ký tự và toàn bộ là chữ số → tự động chuẩn hoá thành chuỗi 12 chữ số (pad zero bên trái), cập nhật lại ô Mã nhập.
4. Gọi tra cứu phiếu nhập theo Mã nhập (so khớp chính xác — `IMP_MEST_CODE__EXACT`).
5. Không tìm thấy → thông báo "Không tìm thấy phiếu nhập có mã …", giữ nguyên ô Mã nhập, không xáo trộn vùng chi tiết.
6. Tìm thấy → load chi tiết Thuốc/Vật tư/Máu và đổ dữ liệu lên các tab tương ứng.

Nút **Làm mới**: xóa ô Mã nhập, dọn vùng thông tin chung + 3 tab về trạng thái trống, focus về ô Mã nhập.

### Chế độ Xem chi tiết (mở từ chức năng khác, có truyền sẵn phiếu nhập)
- Tiêu đề popup: **"Chi tiết nhập"**.
- Ô Mã nhập hiển thị mã phiếu nhập tương ứng, **không** cho sửa/xóa.
- Ẩn nút Làm mới; ẩn các nút thay đổi dữ liệu; **giữ nút In ấn**.
- Tự động tra cứu và đổ đầy đủ thông tin phiếu nhập lên màn hình.

### In ấn
- Chỉ thực hiện được khi đã có dữ liệu phiếu nhập trên màn hình.
- In theo mẫu in phiếu nhập hiện hành — giống Chi tiết nhập (xem Section 7).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_IMP_MEST | View | Thông tin chung phiếu nhập (tra cứu theo mã) |
| V_HIS_IMP_MEST_MEDICINE | View | Chi tiết thuốc của phiếu nhập (tab Thuốc) |
| V_HIS_IMP_MEST_MATERIAL | View | Chi tiết vật tư của phiếu nhập (tab Vật tư) |
| V_HIS_IMP_MEST_BLOOD | View | Chi tiết máu/chế phẩm máu (tab Máu) |
| HIS_MEDICINE / HIS_MEDICINE_PATY | Table | Giá thuốc (BHYT/VP) phục vụ hiển thị |
| HIS_MATERIAL / HIS_MATERIAL_PATY | Table | Giá vật tư (BHYT/VP) phục vụ hiển thị |
| V_HIS_MEDICINE_TYPE / V_HIS_MATERIAL_TYPE / V_HIS_BLOOD_TYPE | View | Danh mục thuốc/vật tư/máu cho lookup hiển thị |

## 4. UI Layout

```
+--------------------------------------------------------------+
| [Mã nhập: ____________]  [Làm mới]      (panelSearch - Top)  |  ← chế độ tra cứu: nhập được + Làm mới
+--------------------------------------------------------------+   ← chế độ xem chi tiết: chỉ đọc, ẩn Làm mới
| Thông tin chung: Kho nhập | Mã nhập | Người nhập | TG nhập   |
|                  Số CT | Đơn giá CT | Chiết khấu | Mô tả ... |
+--------------------------------------------------------------+
| [Tab Thuốc] [Tab Vật tư] [Tab Máu]                           |
|   Grid chi tiết theo tab                                     |
+--------------------------------------------------------------+
| [In ấn ▼]   (các nút Lưu/Duyệt/Thực nhập/Hội đồng: ẩn)       |
+--------------------------------------------------------------+
```

- `panelSearch` (PanelControl dock Top) thêm mới so với bản gốc: `lblMaNhapSearch` + `txtImpMestCode` + `btnReset`.
- Các nút thay đổi dữ liệu (`btnSave`, `btnApproval`, `btnImport`, `btnHoiDongKiemNhap`) bị ẩn (Visibility = Never) ở cả 2 chế độ.

## 5. API Endpoints (tái sử dụng nguyên trạng — KHÔNG thay đổi Backend)

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Tra cứu phiếu nhập theo Mã nhập | `api/HisImpMest/GetView` (HisRequestUriStore.HIS_IMP_MEST_GETVIEW) | MosConsumer | HisImpMestViewFilter.IMP_MEST_CODE__EXACT |
| Chi tiết thuốc | `api/HisImpMestMedicine/GetView` | MosConsumer | HisImpMestMedicineViewFilter.IMP_MEST_ID |
| Chi tiết vật tư | `api/HisImpMestMaterial/GetView` | MosConsumer | HisImpMestMaterialViewFilter.IMP_MEST_ID |
| Chi tiết máu | `api/HisImpMestBlood/GetView` | MosConsumer | HisImpMestBloodViewFilter.IMP_MEST_ID |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Tạo input ký số EMR khi in (GenerateInputADOWithPrintTypeCode) |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.HisRoleUser | (Kế thừa từ bản gốc — nút Hội đồng kiểm nhập đã bị ẩn nên không dùng) | HIS_IMP_MEST + dữ liệu phiếu |
| HIS.Desktop.Plugins.IdentityMaterialInformation | (Kế thừa từ bản gốc — luồng Thực nhập đã bị ẩn) | impMest.ID + DelegateImpTime |

### Đầu vào plugin (Behavior.Run parse args)
| Kiểu | Bắt buộc | Ý nghĩa |
|------|----------|---------|
| Inventec.Desktop.Common.Modules.Module | Có (tự thêm bởi PluginInstance) | Context module / phòng |
| HIS.Desktop.ADO.ImpMestViewDetailADO | Tùy chọn | Nếu **có** → chế độ Xem chi tiết; nếu **không** → chế độ Tra cứu |
| DelegateSelectData | Tùy chọn | Callback trả dữ liệu |

## 7. Print

Tái sử dụng nguyên trạng logic in của bản gốc (frmImpMestLookupPlus_Print.cs) qua `RichEditorStore.RunPrintTemplate` → `MPS.MpsPrinter.Run`. Một số mẫu in theo loại phiếu nhập:

| Loại in | PrintTypeCode | Mô tả |
|---------|--------------|-------|
| Biên bản kiểm nhập từ NCC | Mps000085 | Nhập NCC |
| Phiếu nhập thuốc, vật tư từ NCC | Mps000141 | Nhập NCC đã nhập |
| Phiếu nhập máu từ NCC | Mps000149 | Nhập máu NCC |
| Phiếu nhập chuyển kho | Mps000143 / Mps000226 | Nhập chuyển kho |
| Phiếu nhập kiểm kê/đầu kỳ/khác | Mps000199 | KK/ĐK/Khác |
| In tem theo số Seri | Mps000494 | Vật tư có Serial |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 10/06/2026 | sinhnt | Tạo mới plugin **Tra cứu phiếu nhập** — clone từ HIS.Desktop.Plugins.ImpMestViewDetail; thêm ô Mã nhập + nút Làm mới; hỗ trợ 2 chế độ (Tra cứu từ menu / Xem chi tiết khi có phiếu truyền vào); ẩn các nút thay đổi dữ liệu; tra cứu theo Mã nhập so khớp chính xác + pad zero 12 ký tự. Theo PTTK 2726 (việc 42888). |

## 9. Test Cases

### Chế độ Tra cứu (mở từ menu)
- [ ] Mở plugin → tiêu đề "Tra cứu phiếu nhập", ô Mã nhập nhập được, hiện nút Làm mới, focus vào Mã nhập, các vùng khác trống.
- [ ] Ẩn các nút Lưu / Duyệt / Thực nhập / Hội đồng kiểm nhập.
- [ ] Nhập rỗng + Enter → cảnh báo trường bắt buộc, không gọi API.
- [ ] Nhập "12345" (5 chữ số) + Enter → tự pad thành "000000000012345"? (đúng quy tắc 12 ký tự → "000000012345") và cập nhật ô Mã nhập.
- [ ] Nhập mã không tồn tại + Enter → thông báo "Không tìm thấy phiếu nhập có mã …", giữ nguyên ô, vùng chi tiết không đổi.
- [ ] Nhập mã hợp lệ + Enter → đổ đầy đủ thông tin chung + 3 tab Thuốc/Vật tư/Máu.
- [ ] Ấn Làm mới → xóa Mã nhập, dọn toàn bộ vùng dữ liệu, focus về Mã nhập.

### Chế độ Xem chi tiết (truyền sẵn phiếu)
- [ ] Mở với ImpMestViewDetailADO → tiêu đề "Chi tiết nhập", ô Mã nhập hiển thị mã (chỉ đọc), ẩn Làm mới, tự đổ dữ liệu.
- [ ] Giữ nút In ấn; ẩn các nút thay đổi dữ liệu.

### In ấn
- [ ] Chưa có dữ liệu → không in được.
- [ ] Có dữ liệu → In đúng mẫu theo loại phiếu nhập (preview / in trực tiếp theo cấu hình).

### Đa ngôn ngữ
- [ ] Chuyển ngôn ngữ vi/en → nhãn Mã nhập, nút Làm mới, tiêu đề, thông báo đổi theo ngôn ngữ.
