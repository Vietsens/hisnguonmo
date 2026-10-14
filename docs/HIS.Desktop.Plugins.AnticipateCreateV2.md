# Tạo dự trù 2 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AnticipateCreateV2 |
| Loại | UC (`UCAnticipateCreateV2`); cửa sổ sửa phiếu `frmAnticipateCreateV2Edit` dùng chung UC |
| Mục đích | Lập phiếu dự trù thuốc, vật tư, máu: xem tồn kho và số liệu nhập/xuất/tồn theo loại (gộp các kho đã chọn), nhập SL dự trù trên cây rồi đưa xuống lưới dự trù để lưu, in |
| Tên menu | "Tạo dự trù 2" (ACS_MODULE.MODULE_NAME); caption khai báo trong `ExtensionOf` là "Tạo dự trù (nhập xuất tồn)" |
| Người tạo | anhnh_guest (BV HAGL, 07/2026) |
| Ngày tạo | 24/07/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở chức năng: panel "Chọn kho" (thu gọn mặc định) tích sẵn kho; màn tự tìm một lần sau khi hiển thị.
2. Chọn bộ lọc: Từ khóa tìm kiếm thuốc/vật tư, Thông tin (BHYT), Từ ngày, Đến ngày; Thuốc / Vật tư / Máu; Dự trù theo Loại mặt hàng hoặc Thầu (Nhà thầu, Gói thầu); Hiển thị dòng hết; Tồn ở mức cảnh báo tối thiểu; Chi tiết / Thu gọn tất cả; Ẩn dòng nhóm.
3. Tìm (Ctrl F) hoặc Ctrl T → `ShowUCControl`: lấy cây tồn theo loại + số liệu nhập/xuất theo kho (`GetWithImpExp`) + số liệu dự trù theo loại (`GetForAnticipate`), rồi `BindPivotTree`.
4. Nhập SL dự trù, Ghi chú ở các cột sửa được trên cây (lưu theo typeId, không mất khi tìm lại).
5. Bổ sung (Ctrl A) → đưa các loại có SL dự trù > 0 xuống lưới dự trù.
6. Lưu (Ctrl S) → `HisAnticipate/Create` (lần đầu) hoặc `Update`; In (Ctrl P) → Mps000117; Mới (Ctrl N) xóa lưới dự trù và SL đã nhập.

### Điều kiện nghiệp vụ
- Thứ tự lọc trên cây (`BindPivotTree`, nối tiếp nhau, dòng phải thỏa tất cả): Tồn ở mức cảnh báo tối thiểu → Gói thầu (chế độ Thầu) → Thông tin BHYT → Từ khóa → Ẩn dòng nhóm. Mỗi bộ lọc chỉ xét dòng loại `IS_LEAF = 1`, giữ dòng nhóm tổ tiên (`FilterAlertMinStock`).
- Ô **Thông tin** (Tất cả / Đầy đủ thông tin BHYT / Thiếu thông tin BHYT), từ 13/10/2026:
  - Áp dụng khi tìm (Tìm, Ctrl T, đổi Thuốc/Vật tư). Đổi giá trị mà chưa tìm thì danh sách giữ nguyên.
  - Thuốc: `api/HisMedicineType/GetViewDynamic` với `IS_MISS_BHYT_INFO` (6 trường ACTIVE_INGR_BHYT_CODE, ACTIVE_INGR_BHYT_NAME, REGISTER_NUMBER, MEDICINE_USE_FORM_ID, CONCENTRA, HEIN_SERVICE_TYPE_ID — giống Danh sách loại thuốc).
  - Vật tư: `api/HisMaterialType/GetViewDynamic` với `IS_MISS_BHYT_INFO` (3 trường HEIN_SERVICE_BHYT_CODE, HEIN_SERVICE_BHYT_NAME, HEIN_SERVICE_TYPE_ID).
  - Chỉ lấy cột ID, không truyền IS_ACTIVE, IS_BUSINESS, IDs, Start/Limit. Tập ID giữ riêng cho thuốc và vật tư.
  - Máu: ô bị mờ, giữ giá trị. Làm lại: về Tất cả. Không lưu ControlState (mở màn luôn Tất cả).
  - Gọi lỗi: danh sách phía trên để trống, báo "Xử lý thất bại" sau khi tắt màn chờ.
  - "Đầy đủ thông tin BHYT" = đã khai đủ trường trong danh mục, KHÔNG đồng nghĩa BHYT thanh toán.
- Bổ sung lấy loại từ `lstMediInStocks`/`lstMateInStocks` (chưa lọc theo cây), nên loại đang bị ẩn bởi Từ khóa, Gói thầu, Thông tin mà đã nhập SL vẫn được đưa xuống.
- Kho chọn toàn kinh doanh hoặc toàn không kinh doanh thì cây lọc loại theo `IS_BUSINESS` tương ứng.
- Chế độ sửa phiếu (mở từ Danh sách dự trù khi `HIS.Desktop.Plugins.AnticipateList.USE_ANTICIPATE_CREATE_V2_FOR_EDIT = 1`): khóa đổi Dự trù theo, nạp lại dòng phiếu vào lưới dự trù.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HisMedicineInStockSDO, HisMaterialInStockSDO, HisBloodInStockSDO | SDO | Cây tồn theo loại (dòng nhóm + dòng loại) |
| V_HIS_MEDI_STOCK, HIS_MEDI_STOCK | View/Table | Danh sách kho, kho kinh doanh |
| HIS_MEDICINE_TYPE, HIS_MATERIAL_TYPE | Table (cache) | Mô tả, nước SX, số đăng ký, quy cách, IS_BUSINESS |
| V_HIS_MEDICINE_TYPE, V_HIS_MATERIAL_TYPE | View | Chỉ lấy ID theo IS_MISS_BHYT_INFO (ô Thông tin) |
| HIS_BID, HIS_SUPPLIER, V_HIS_BID_MEDICINE_TYPE, V_HIS_BID_MATERIAL_TYPE | Table/View | Chế độ Dự trù theo Thầu |
| HIS_ANTICIPATE (+ HIS_ANTICIPATE_METY/MATY/BLTY) | Table | Lưu phiếu dự trù |
| V_HIS_ANTICIPATE_METY/MATY/BLTY | View | Nạp lại phiếu khi sửa, in |

### Quan hệ chính
- Dòng loại trên cây: `MEDICINE_TYPE_ID`/`MATERIAL_TYPE_ID` = ID loại (AutoMapper suy từ ID), `NodeId` = "T" + ID, `ParentNodeId` = "T" + PARENT_ID.
- HIS_ANTICIPATE → HIS_ANTICIPATE_METY/MATY/BLTY (1-n, qua ANTICIPATE_ID).

## 4. UI Layout

### Sơ đồ giao diện
```
+--------+---------------------------------------------------------------------------------------------+
| Chọn   | [Từ khóa tìm kiếm thuốc/vật tư...] [Thông tin ▼] [Từ ngày] [Đến ngày] (o)Chi tiết (o)Thu gọn [ ]Ẩn dòng nhóm |
| kho    | (o)Thuốc (o)Vật tư (o)Máu  Dự trù theo: (o)Loại mặt hàng (o)Thầu [Nhà thầu▼] [Gói thầu▼]      |
| (thu   |                                  [Làm lại] [Tìm] [ ]Hiển thị dòng hết [ ]Tồn ở mức cảnh báo  |
| gọn)   +---------------------------------------------------------------------------------------------+
|        | Cây tồn theo loại (TreeList, nhập SL dự trù / Ghi chú)                                      |
|        +---------------------------------------------------------------------------------------------+
|        | Lưới dự trù                                                                                  |
|        | [Bổ sung] [Import] [Lưu] [In] [Mới]                                                         |
+--------+---------------------------------------------------------------------------------------------+
```
- Hàng lọc dựng bằng code lúc chạy (`layoutControl3.AddItem`), không nằm trong Designer.
- Ô Thông tin: `cboBhytInfo` (ImageComboBoxEdit, không gõ tay) trong `lciBhytInfo` rộng 220 px, chèn Right của ô Từ khóa (chèn Left của Từ ngày cố định thì DevExpress mở hàng mới). Chữ ở khóa `UCAnticipateCreateV2.lciBhytInfo.*`, `UCAnticipateCreateV2.cboBhytInfo.All/Full/Missing` (Lang.vi/en/my).
- Ở 1366 px hàng 2 cần tối thiểu 1408 px nên khung lọc tràn sẵn khoảng 80 px.

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| DevExpress TreeList (`treeListPivot`, `treeListBlood`) | Panel trên | Cây tồn theo loại |
| DevExpress GridControl (`gridViewAnticipate`) | Panel dưới | Lưới dự trù |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Cây tồn thuốc | api/HisMedicine/GetInStockMedicineWithTypeTree | MosConsumer | HisMedicineStockViewFilter |
| Cây tồn vật tư | api/HisMaterial/GetInStockMaterialWithTypeTree | MosConsumer | HisMaterialStockViewFilter |
| Tồn máu | api/HisBlood/GetInStockBloodWithTypeTree | MosConsumer | HisBloodStockViewFilter |
| Nhập/xuất theo kho | api/HisMediStockMety/GetWithImpExp, api/HisMediStockMaty/GetWithImpExp | MosConsumer | — |
| Số liệu dự trù theo loại | api/HisMediStockMety/GetForAnticipate, api/HisMediStockMaty/GetForAnticipate | MosConsumer | — |
| Ô Thông tin (BHYT) | api/HisMedicineType/GetViewDynamic, api/HisMaterialType/GetViewDynamic | MosConsumer | HisMedicineTypeViewFilter / HisMaterialTypeViewFilter (IS_MISS_BHYT_INFO, ColumnParams = ID) |
| Dữ liệu thầu | api/HisBid/Get, api/HisSupplier/Get, api/HisBidMedicineType/GetView, api/HisBidMaterialType/GetView | MosConsumer | — |
| Lưu phiếu | api/HisAnticipate/Create, api/HisAnticipate/Update | MosConsumer | — |
| Nạp phiếu sửa / in | api/HisAnticipateMety/GetView, api/HisAnticipateMaty/GetView, api/HisAnticipateBlty/GetView | MosConsumer | — |
| Khóa lô | api/HisMedicine/Lock, api/HisMaterial/Lock | MosConsumer | — |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Library.CacheClient | ControlState (lưu trạng thái lọc) |
| HIS.Desktop.Plugins.Library.EmrGenerate | Ký số khi in |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AnticipateList | Danh sách dự trù mở màn này ở chế độ Sửa (cấu hình USE_ANTICIPATE_CREATE_V2_FOR_EDIT = 1) | V_HIS_ANTICIPATE + delegate làm mới |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu dự trù | Mps000117 | MPS.Processor.Mps000117 | Theo cấu hình mẫu in Mps000117 |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/07/2026 | anhnh_guest | Tạo mới plugin (BV HAGL - Tạo dự trù 2) |
| 13/10/2026 | phuongnm | Tài liệu 3511: thêm ô lọc Thông tin (Tất cả / Đầy đủ thông tin BHYT / Thiếu thông tin BHYT) cho Thuốc, Vật tư; MOS thêm IS_MISS_BHYT_INFO cho HisMaterialTypeViewFilter |

## 9. Test Cases

### Ô Thông tin (BHYT)
- [ ] Mở màn: ô ở "Tất cả", danh sách như trước
- [ ] Thuốc, "Đầy đủ thông tin BHYT" → Tìm: chỉ còn thuốc đủ 6 trường; nhóm không còn thuốc nào bị ẩn
- [ ] Thuốc, "Thiếu thông tin BHYT" → Tìm: chỉ còn thuốc thiếu ít nhất 1 trường; thuốc đang khóa còn tồn vẫn được xét
- [ ] Vật tư, "Đầy đủ" / "Thiếu" → Tìm: lọc theo Mã BHYT, Tên BHYT, Nhóm BHYT
- [ ] Đổi giá trị mà chưa bấm Tìm: danh sách giữ nguyên
- [ ] Kết hợp Từ khóa, Gói thầu, Tồn ở mức cảnh báo, Ẩn dòng nhóm: dòng phải thỏa tất cả
- [ ] Chọn Máu: ô mờ; quay lại Thuốc/Vật tư: tự tìm và lọc theo giá trị đang có
- [ ] Làm lại (Ctrl R): ô về "Tất cả"
- [ ] Nhập SL dự trù cho thuốc B lúc "Tất cả", chọn "Đầy đủ" (B thiếu) → Tìm → Bổ sung: B vẫn xuống lưới dự trù
- [ ] Mất kết nối khi Tìm với "Đầy đủ"/"Thiếu": báo "Xử lý thất bại", danh sách phía trên trống
- [ ] Màn hình 1366x768: ô Thông tin nằm giữa ô Từ khóa và Từ ngày, chữ "Đầy đủ thông tin BHYT" không bị cắt

### Lưu / In
- [ ] Bổ sung → Lưu → In Mps000117 đúng các dòng đã lưu
