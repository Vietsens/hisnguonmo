# Tồn Kho — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.MediStockSummary |
| Loại | UC |
| Mục đích | Hiển thị tổng hợp tồn kho thuốc / vật tư / máu theo nhiều kho. Hỗ trợ xem chi tiết theo lô, hạn dùng, đối tượng thanh toán; in báo cáo (Mrs00067, Mrs00075, Mrs00076, Mrs00085) và xuất Excel danh mục thuốc / vật tư / máu. |
| Người tạo | — |
| Ngày tạo | — |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

1. Người dùng mở module Tồn kho — UC tải danh sách kho theo phòng/khoa hiện tại (lstMediStock).
2. Người dùng chọn 1 hoặc nhiều kho ở grid bên trái (cột check), chọn loại đối tượng (chkMedicine / chkMaterial / chkBlood) và nhấn `Tìm (Ctrl+F)`.
3. UC gọi API tương ứng để lấy dữ liệu dạng cây (Type-node cha + dòng chi tiết con).
4. Người dùng có thể lọc bổ sung: ở mức cảnh báo tối thiểu, lọc dòng không HSD, hết HSD đến ngày, hạn thầu đến ngày, hiển thị dòng hết, hiển thị lô hết, trạng thái khóa lô (Tất cả / Khóa / Mở), tìm theo từ khóa.
5. Thao tác trên 1 dòng chi tiết: Khóa / Mở khóa lô (`HIS_MEDICINE`, `HIS_MATERIAL`), Trả về tồn (RETURN_AVAILABLE), Xem chi tiết, In thẻ kho theo ngày.
6. Xuất kết quả: In báo cáo MPS (Mrs00067 / Mrs00075 / Mrs00076 / Mrs00085) hoặc Xuất Excel theo template `DanhMucThuoc.xls` / `DanhMucVatTu.xls` / xuất máu qua `hisBloodProcessor.Export`.

### Sơ đồ luồng xuất Excel

```
btnXuatExcel_Click
  → LoadPrint
    ├── Đọc kho đã chọn (mediStockIds)
    ├── chkExportExcel = ON  → dùng cây hiện trên màn hình (this.lstMediInStocks / lstMateInStocks)
    │   chkExportExcel = OFF → gọi API GetInStockMedicineWithTypeTree / GetInStockMaterialWithTypeTree để lấy lại toàn bộ
    ├── Lọc dòng chi tiết (!isTypeNode && ID > 0)
    ├── Build dictionary V_HIS_MEDICINE_TYPE.ID → row (từ BackendDataWorker)
    ├── Map sang MedicineInStockExportADO / MaterialInStockExportADO
    │   - Lấy V_HIS_MEDICINE_TYPE / V_HIS_MATERIAL_TYPE theo MEDICINE_TYPE_ID / MATERIAL_TYPE_ID
    │   - Nếu loại thuốc/vật tư có PARENT_ID → tra dictionary lấy MEDICINE_TYPE_NAME / MATERIAL_TYPE_NAME của cha → PARENT_GROUP_NAME
    │   - Gán giá theo từng đối tượng (PATIENT_TYPE) qua HIS_MEDICINE_PATY / HIS_MATERIAL_PATY
    ├── ReadTemplate(DanhMucThuoc.xls hoặc DanhMucVatTu.xls)
    └── objectTag.AddObjectData(store, "ExportResult", lstExport)
```

### Điều kiện nghiệp vụ

- Phải chọn ít nhất 1 kho trước khi xuất Excel hoặc tìm kiếm.
- Chỉ xuất các dòng chi tiết (`isTypeNode == false && ID > 0`); node cha (Type-node) không xuất thành dòng.
- Cột `Nhóm cha` lấy từ `V_HIS_MEDICINE_TYPE.PARENT_ID` (tương ứng `V_HIS_MATERIAL_TYPE.PARENT_ID` cho vật tư) — tra lại sang `V_HIS_MEDICINE_TYPE.MEDICINE_TYPE_NAME` / `V_HIS_MATERIAL_TYPE.MATERIAL_TYPE_NAME` của loại cha.
- Khi loại thuốc/vật tư không có `PARENT_ID` → cột `Nhóm cha` để trống.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_MEDI_STOCK | View | Danh sách kho |
| V_HIS_MEDICINE_TYPE | View | Danh mục loại thuốc (đường dùng, dòng thuốc, nhóm GN/KS/HT) |
| V_HIS_MEDICINE_BEAN | View | Base class của `MedicineInStockExportADO` — thông tin thuốc tồn |
| V_HIS_MATERIAL_TYPE | View | Danh mục loại vật tư |
| V_HIS_MATERIAL_BEAN | View | Base class của `MaterialInStockExportADO` |
| HIS_MEDICINE_PATY | Table | Giá thuốc theo đối tượng thanh toán |
| HIS_MATERIAL_PATY | Table | Giá vật tư theo đối tượng thanh toán |
| HIS_PATIENT_TYPE | Table | Đối tượng thanh toán (BHYT, Viện phí…) |
| HIS_MEDICINE_GROUP | Table | Nhóm thuốc (GN, KS, HT) |
| HIS_MEDICINE / HIS_MATERIAL | Table | Khóa / mở khóa lô, trả về tồn |

### SDO sử dụng

| SDO | Mục đích |
|-----|----------|
| MOS.SDO.HisMedicineInStockSDO | Cây tồn kho thuốc — có `NodeId`, `ParentNodeId`, `isTypeNode`, `IS_LEAF`, `MEDICINE_TYPE_NAME`, `AvailableAmount`, … |
| MOS.SDO.HisMaterialInStockSDO | Cây tồn kho vật tư — tương tự |
| MOS.SDO.HisBloodInStockSDO | Cây tồn kho máu |
| MOS.SDO.HisMedicineChangeLockSDO | Khóa / mở khóa lô thuốc |

## 4. UI Layout

### Sơ đồ giao diện

```
+------------------------------------------------------------------+
| [Khác / Tab module]                                              |
+------------------+-----------------------------------------------+
| Grid kho (left)  | [Bộ lọc nâng cao: cảnh báo, HSD, lô, …]      |
| - Check chọn kho | [chkExportExcel] [In ấn] [Xuất Excel]         |
| - Mã / Tên kho   |-----------------------------------------------|
| - Khoa phòng     | TreeList tồn kho (Thuốc / Vật tư / Máu)       |
|                  | - Type-node (nhóm cha)                         |
|                  |   - Dòng chi tiết (mã, tên, hoạt chất, lô,…) |
|                  | (Right-click: Khóa, Mở khóa, Trả về tồn,    |
|                  |  Xem chi tiết, In thẻ kho)                   |
|                  |-----------------------------------------------|
|                  | [Chi tiết / Thu gọn theo thuốc / Tất cả]     |
+------------------+-----------------------------------------------+
```

### UC sử dụng

| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.HisMedicineInStock | ucMedicineInfo | TreeList tồn kho thuốc |
| HIS.UC.HisMaterialInStock | ucMaterialInfo | TreeList tồn kho vật tư |
| HisBloodInStock (UC máu) | ucBloodInfo | TreeList tồn kho máu |
| HIS.UC.MediStock | gridControlMediStock | Grid kho bên trái |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy cây tồn kho thuốc | `api/HisMedicine/GetInStockMedicineWithTypeTree` | MosConsumer | HisMedicineStockViewFilter |
| Lấy cây tồn kho vật tư | `api/HisMaterial/GetInStockMaterialWithTypeTree` | MosConsumer | HisMaterialStockViewFilter |
| Lấy cây tồn kho máu | `api/HisBlood/GetInStockBloodWithTypeTree` | MosConsumer | HisBloodStockViewFilter |
| Giá thuốc theo đối tượng | `api/HisMedicinePaty/Get` | MosConsumer | HisMedicinePatyFilter |
| Giá vật tư theo đối tượng | `api/HisMaterialPaty/Get` | MosConsumer | HisMaterialPatyFilter (chunk 200 ID) |
| Khóa / mở khóa thuốc | `/api/HisMedicine/Lock`, `/api/HisMedicine/Unlock` | MosConsumer | HisMedicineChangeLockSDO |
| Khóa / mở khóa vật tư | `/api/HisMaterial/Lock`, `/api/HisMaterial/Unlock` | MosConsumer | HisMaterialChangeLockSDO |
| Trả về tồn | `api/HisMedicine/ReturnAvailable`, `api/HisMaterial/ReturnAvailable` | MosConsumer | RETURN_AVAILABLE_MEDICINE / RETURN_AVAILABLE_MATERIAL |

## 6. Dependencies

### Library Plugins / UC chia sẻ

| Library / UC | Mục đích |
|--------------|----------|
| HIS.UC.HisMedicineInStock | TreeList tồn kho thuốc |
| HIS.UC.HisMaterialInStock | TreeList tồn kho vật tư |
| HIS.UC.HisBloodInStock | TreeList tồn kho máu |
| HIS.UC.MediStock | Grid kho |
| Inventec.Common.FlexCellExport | Đọc template `.xls`, đổ data, xuất Excel |
| HIS.Desktop.Common.MessageManager | Hiển thị thông báo sau API |

### Inter-Plugin

Plugin này không mở plugin ngoài (chỉ mở form con `frmMediCardByDateReport`, `frmReasonLock` cùng plugin).

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template / Class |
|---------|--------------|-------------|------------------|
| Báo cáo tồn kho thuốc theo loại | Mrs00067 | MPS | CreateReport/Mrs00067.cs |
| Báo cáo tồn kho thuốc tổng hợp | Mrs00075 | MPS | CreateReport/Mrs00075.cs |
| Báo cáo tồn kho vật tư | Mrs00076 | MPS | CreateReport/Mrs00076.cs |
| Báo cáo tồn kho vật tư mở rộng | Mrs00085 | MPS | CreateReport/Mrs00085.cs |
| Thẻ kho thuốc / vật tư theo ngày | Tham số `KeyConfigReport` | MPS | CreateReport/frmMediCardByDateReport.cs |
| Excel danh mục thuốc | (template Excel) | FlexCellExport | `Tmp/Exp/DanhMucThuoc.xls` |
| Excel danh mục vật tư | (template Excel) | FlexCellExport | `Tmp/Exp/DanhMucVatTu.xls` |
| Excel danh mục máu | (template Excel) | hisBloodProcessor.Export | (Library tồn kho máu) |

### Trường dữ liệu phục vụ template Excel

`MedicineInStockExportADO` / `MaterialInStockExportADO` — bind dưới key `ExportResult.<TÊN_TRƯỜNG>` trong template:

| Trường | Ý nghĩa |
|--------|---------|
| `PARENT_GROUP_NAME` | Nhóm cha — tên node cha trực tiếp trong cây tồn kho (mới bổ sung 24/04/2026) |
| `MEDICINE_CODE` / `MATERIAL_CODE` | Mã thuốc / vật tư |
| `MEDICINE_TYPE_NAME` / `MATERIAL_TYPE_NAME` | Tên thuốc / vật tư |
| `MEDICINE_TYPE_PROPRIETARY_NAME` | Tên biệt dược |
| `AMOUNT`, `AVAILABLE_AMOUNT` | Tổng tồn / khả dụng |
| `DOCUMENT_NUMBER` | Số hóa đơn |
| `EXPIRED_DATE_STR`, `ALERT_EXPIRED_DATE_STR` | Hạn dùng / cảnh báo HSD |
| `IMP_PRICE`, `IMP_PRICE_VAT`, `IMP_VAT_RATIO` | Giá nhập / sau VAT / tỷ lệ VAT |
| `EXP_PRICE_1..10`, `PATIENT_TYPE_NAME_1..10`, `EXP_VAT_RATIO` | Giá theo 10 đối tượng đầu tiên |
| `DicExpPrice`, `DicExpVatRatio`, `DicPatientTypeName` | Dictionary động cho > 10 đối tượng |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/04/2026 | dangth | [42819] BV HAGL — Bổ sung cột `Nhóm cha` vào file Excel xuất tồn kho thuốc và vật tư. Code: thêm trường `PARENT_GROUP_NAME` trong `MedicineInStockExportADO` / `MaterialInStockExportADO`; `UCMediStockSummary.LoadPrint` tra `V_HIS_MEDICINE_TYPE.PARENT_ID` / `V_HIS_MATERIAL_TYPE.PARENT_ID` qua dictionary (O(1)) để lấy `MEDICINE_TYPE_NAME` / `MATERIAL_TYPE_NAME` của loại cha. Template: `Tmp/Exp/DanhMucThuoc.xls` chèn cột C "Nhóm cha" (merge dòng 3-4, dòng 6 chứa tag `<#ExportResult.PARENT_GROUP_NAME;>`); `Tmp/Exp/DanhMucVatTu.xls` chèn cột C "Nhóm cha" (dòng 3, dòng 5 chứa tag `<#ExportResult.PARENT_GROUP_NAME;>`). |
| 20/05/2026 | dangth | [42819] Fix bug "Xuất Excel theo ĐK" — màn hình hiện N thuốc nhưng Excel chỉ ra 1 thuốc. Trước đây luồng xuất chỉ giữ detail-lot (`!isTypeNode && ID > 0`) nên loại thuốc đang hiện ở chế độ thu gọn mà không có lot hợp lệ sẽ bị mất. Sửa `UCMediStockSummary.LoadPrint`: bổ sung fallback — với mỗi type-node `IS_LEAF=1` chưa có detail-lot pass filter, xuất chính type-node làm 1 dòng (dùng AMOUNT/AvailableAmount đã aggregate). Điều chỉnh `GroupBy` để tách type-node (`TN_<NodeId>`) và detail (`DT_<ID>`); `dicData` dùng key âm `-MEDICINE_TYPE_ID` / `-MATERIAL_TYPE_ID` cho type-node để tránh collision khi ID=0; `medicineIds` / `materialIds` truyền vào `HisMedicinePaty/Get` chỉ chứa ID detail-lot dương. Áp dụng cho cả thuốc và vật tư. |
| 05/06/2026 | tuanln | [FE] Bổ sung cột `Thời gian đóng gói` (FieldName `PackingTimeStr`) đứng ngay sau cột `Hạn sử dụng` trong cây tồn kho **Máu** (`InitHisBloodTree`). Cột unbound, fill trong `bloodType_CustomUnboundColumnData` từ `HisBloodInStockSDO.PackingTime` (long, backend đã bổ sung) định dạng bằng `TimeNumberToTimeString`. Dời `VisibleIndex` các cột sau: Ngày còn lại 9→10, Nhà cung cấp 10→11, Dung tích 11→12, Số lượng tồn 12→13. |

## 9. Test Cases

### Xuất Excel — Thuốc

- [ ] Chọn kho → tích `chkMedicine` → tích `chkExportExcel` (lọc theo màn hình) → bấm Xuất Excel → Excel có cột `Nhóm cha` đứng sau STT, mỗi dòng chi tiết hiển thị đúng tên node cha.
- [ ] Chọn kho → tích `chkMedicine` → KHÔNG tích `chkExportExcel` (toàn bộ tồn kho) → bấm Xuất Excel → Excel có cột `Nhóm cha` được điền đúng từ dữ liệu cây trả về của API.
- [ ] Thuốc không có node cha trong cây → cột `Nhóm cha` để trống, không lỗi runtime.

### Xuất Excel — Vật tư

- [ ] Chọn kho → tích `chkMaterial` → tích `chkExportExcel` → Xuất Excel → cột `Nhóm cha` hiển thị đúng tên loại vật tư cha.
- [ ] Chọn kho → tích `chkMaterial` → KHÔNG tích `chkExportExcel` → Xuất Excel → cột `Nhóm cha` được điền đầy đủ từ API.
- [ ] Vật tư không có node cha → cột `Nhóm cha` trống, không phá vỡ format file.

### Hồi quy

- [ ] Các trường cũ (Mã thuốc, Tên thuốc, Số lượng, Hoạt chất, Số lô, Đơn vị, Đóng gói, Nước sản xuất, Hãng …) vẫn hiển thị đúng.
- [ ] Báo cáo MPS (Mrs00067 / Mrs00075 / Mrs00076 / Mrs00085) không bị ảnh hưởng.
- [ ] In thẻ kho theo ngày (`frmMediCardByDateReport`) hoạt động bình thường.
- [ ] Tab Máu — xuất Excel máu qua `hisBloodProcessor.Export` không thay đổi.
- [ ] Tab Máu — cột `Thời gian đóng gói` hiển thị ngay sau `Hạn sử dụng`, đúng giá trị `PackingTime` (ngày giờ) ở dòng túi máu; dòng node cha để trống. Các cột Ngày còn lại / Nhà cung cấp / Dung tích / Số lượng tồn vẫn đúng thứ tự.
