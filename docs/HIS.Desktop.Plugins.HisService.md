# HIS Service — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisService |
| Loại | UserControl |
| Mục đích | Quản lý danh mục dịch vụ kỹ thuật (CRUD): thêm, sửa, xóa, khóa/mở khóa, tìm kiếm theo loại và keyword, cấu hình các thông số đi kèm (BHYT, ICD, mẫu in, phòng thực hiện…). |
| Ngày cập nhật | 21/05/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
User mở module "Danh mục dịch vụ kỹ thuật"
  → Chọn loại dịch vụ (cboSearchType) + nhập keyword (txtKeyword)
  → Click "Tìm kiếm" → LoaddataToTreeList()
    → Gọi API api/HisService/GetView với HisServiceViewFilter
    → Hiển thị kết quả lên TreeList (KeyFieldName=ID, ParentFieldName=PARENT_ID)
  → User chọn dòng → form chi tiết hiển thị thông tin
  → Sửa / Thêm / Xóa / Khóa / Mở khóa
```

### Tìm kiếm + bộ lọc

- **cboSearchType**: bắt buộc nếu txtKeyword trống — nếu cả 2 đều trống thì xóa grid và return.
- **txtKeyword**: tìm theo mã/tên dịch vụ.
- **chkLock** ("Ẩn dịch vụ bị khóa"):
  - Checked → `filter.IS_ACTIVE = TRUE` → chỉ trả dịch vụ active
  - Unchecked → trả tất cả (cả active và khóa)
- Sau khi có kết quả: lọc bỏ các loại không phải dịch vụ kỹ thuật (THUOC, VT, MAU).
- Khi có keyword: tìm node cha ra cha, tìm node con ra con. Node con "mồ côi" (PARENT_ID trỏ tới ID không match keyword) được set `PARENT_ID = null` để TreeList hiển thị ở root level (flat), không cần bổ sung node cha.

### Điều kiện nghiệp vụ

- Mỗi dịch vụ có thể có cha (PARENT_ID) — tree phân cấp đa cấp.
- Khóa dịch vụ → IS_ACTIVE = 0, dịch vụ không hiện trong các form sử dụng (kê đơn, chỉ định) nhưng vẫn lưu trong dữ liệu cũ.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE | Table | Danh mục dịch vụ kỹ thuật (CRUD chính) |
| V_HIS_SERVICE | View | Đọc dữ liệu hiển thị (denormalized: tên loại, tên cha, mẫu in…) |
| HIS_SERVICE_TYPE | Table | Loại dịch vụ (Khám, XN, CĐHA, TT, PT, Giường…) |
| HIS_PATIENT_TYPE | Table | Đối tượng bệnh nhân (BHYT, Thu phí…) |
| HIS_ICD_CM | Table | Mã ICD-CM |
| HIS_PTTT_GROUP | Table | Nhóm phẫu thuật/thủ thuật |
| HIS_RATION_GROUP | Table | Nhóm ration |
| HIS_FILM_SIZE | Table | Kích thước phim CĐHA |
| HIS_EMR_FORM | Table | Biểu mẫu EMR |
| LIS_SAMPLE_TYPE | Table | Loại mẫu XN (LIS) |
| HIS_TEST_SAMPLE_TYPE | Table | Loại mẫu XN (HIS) |
| HIS_FUEX_TYPE | Table | Loại nhiên liệu |

## 4. UI Layout

### Sơ đồ giao diện

```
+--------------------------------------------------------------+
| Tìm kiếm: [Loại DV ▼] [Keyword]   [☐ Ẩn DV bị khóa] [Tìm]   |
+--------------------------------------------------------------+
| TreeList danh mục dịch vụ (phân cấp cha-con)                |
|   Mã | Tên | Loại | Cha | Trạng thái | ... | Thao tác        |
+--------------------------------------------------------------+
| Form chi tiết:                                                |
|   Mã, Tên, Loại, Cha, Tỉ lệ BHYT, Mã BHYT, ICD, ...           |
|   Mẫu in, Phòng thực hiện, Phòng lấy mẫu, ...                 |
+--------------------------------------------------------------+
| [Mới] [Sửa] [Xóa] [Khóa] [Hủy] [Xuất Excel]                  |
+--------------------------------------------------------------+
```

### Controls chính

| Control | Mục đích |
|---------|----------|
| `cboSearchType` | Lọc theo loại dịch vụ |
| `txtKeyword` | Tìm theo mã/tên |
| `chkLock` | Ẩn dịch vụ bị khóa |
| `treeList1` | Hiển thị tree danh mục |
| `cboParent` | Chọn dịch vụ cha (form chi tiết) |
| `cboServiceType`, `cboPatientType`, `cboBillOption` | Cấu hình DV |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách (view) | api/HisService/GetView | MosConsumer | HisServiceViewFilter |
| Tạo mới | HisRequestUriStore.MOSHIS_SERVICE_CREATE | MosConsumer | HIS_SERVICE |
| Cập nhật | HisRequestUriStore.MOSHIS_SERVICE_UPDATE | MosConsumer | HIS_SERVICE |
| Xóa | HisRequestUriStore.MOSHIS_SERVICE_DELETE | MosConsumer | long (ID) |
| Khóa/Mở khóa | HisRequestUriStore.MOSHIS_SERVICE_CHANGE_LOCK | MosConsumer | long (ID) |

## 6. Dependencies

### EFMODEL / Cache
- `BackendDataWorker.Get<V_HIS_SERVICE>()` — cache toàn bộ danh mục để bổ sung node tổ tiên trong TreeList khi tìm kiếm.
- `BackendDataWorker.Reset<V_HIS_SERVICE>()` — reset cache sau mỗi lần Save/Delete/Lock.

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| frmServiceIndex | Click chỉ số (cấu hình chỉ số dịch vụ) | service data |
| frmServiceRati | Click tỉ lệ (cấu hình tỉ lệ BHYT) | service data |

## 7. Print

Không có chức năng in. Có chức năng xuất Excel (`listVServiceExport`) cho danh sách dịch vụ.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/05/2026 | huannh | Fix bug tìm kiếm tại `LoaddataToTreeList()`: Bỏ hoàn toàn logic auto-add node cha/con khi tìm kiếm. Tìm node cha → chỉ ra cha, tìm node con → chỉ ra con. Node con "mồ côi" được set `PARENT_ID = null` để DevExpress TreeList hiển thị ở root level (flat). Đồng thời sửa luôn bug `chkLock` ("Ẩn DV bị khóa") vốn không hoạt động khi tìm kiếm (do bổ sung node tổ tiên từ cache mà không lọc IS_ACTIVE). |

## 9. Test Cases

### Tìm kiếm

- [ ] Không nhập keyword + không chọn loại → grid rỗng
- [ ] Chỉ chọn loại DV → ra tất cả dịch vụ thuộc loại đó
- [ ] Chỉ nhập keyword (không chọn loại) → ra dịch vụ match keyword, kèm theo các node cha (để TreeList render cây)
- [ ] Tìm dịch vụ CON match keyword → CHỈ ra con (không ra cha, không ra anh em)
- [ ] Tìm dịch vụ CHA match keyword → CHỈ ra cha (không liệt kê con)
- [ ] Tìm dịch vụ con cùng cha → các node con hiển thị flat ở root level (không lồng dưới cha)
- [ ] Tick "Ẩn DV bị khóa" → KHÔNG được hiện dịch vụ nào có IS_ACTIVE = 0, kể cả node cha bị khóa
- [ ] Bỏ tick "Ẩn DV bị khóa" → hiện cả dịch vụ active và bị khóa

### CRUD

- [ ] Thêm mới → Lưu thành công → TreeList refresh, dịch vụ mới xuất hiện
- [ ] Sửa → Lưu → TreeList cập nhật đúng
- [ ] Xóa → Confirm dialog → TreeList refresh, dịch vụ biến mất
- [ ] Khóa dịch vụ active → IS_ACTIVE = 0, đổi icon, đổi màu (đỏ)
- [ ] Mở khóa dịch vụ bị khóa → IS_ACTIVE = 1, đổi icon, đổi màu (xanh)

### Tree

- [ ] Dịch vụ có nhiều cấp cha-con → TreeList hiển thị đúng phân cấp
- [ ] Tìm node ở cấp sâu nhất → tất cả tổ tiên hiển thị đúng cây
