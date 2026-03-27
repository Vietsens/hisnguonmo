# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.MedicineType - Danh sách loại thuốc

---

## 1. Mục đích

Hiển thị danh sách loại thuốc/vật tư y tế dưới dạng cây (tree/grid) với đầy đủ thông tin, hỗ trợ tìm kiếm, lọc, xuất Excel, nhập Excel và khóa/mở khóa loại thuốc. Plugin chỉ hoạt động khi người dùng đang ở phòng kho (room type = STOCK).

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.MedicineType/
├── ADO/
│   ├── MedicineTypeADO.cs          (Mở rộng V_HIS_MEDICINE_TYPE, thêm các bool: IsFood, IsStopImp, IsAutoExpend, IsCPNG, ...)
│   └── MedicineImportADO.cs        (ADO dùng cho import Excel)
├── Form/
│   ├── frmLock.cs                  (Form khóa loại thuốc với lý do)
│   └── frmLock.Designer.cs
├── MedicineTypeList/
│   ├── IMedicineTypeList.cs        (Interface)
│   ├── MedicineTypeListBehavior.cs (Triển khai factory)
│   ├── MedicineTypeListFactory.cs  (Factory pattern)
│   ├── MedicineTypeListProcess.cs  (Xử lý dữ liệu - hiện tại commented out)
│   ├── UCMedicineTypeList.cs       (UserControl chính)
│   └── UCMedicineTypeList.Designer.cs
├── Resources/
│   ├── Lang.vi.resx                (Tiếng Việt)
│   ├── Lang.En.resx                (Tiếng Anh)
│   └── Lang.my.resx                (Tiếng Myanmar)
├── MedicineTypeListProcessor.cs    (Entry point, đăng ký module)
├── HisRequestUri.cs                (Hằng số API endpoints)
├── ModuleLinkString.cs             (Hằng số liên kết module khác)
├── KeyboardWorker.cs               (Phím tắt Ctrl+N)
└── Delegate.cs                     (Delegate dùng nội bộ)
```

---

## 3. Luồng hoạt động

1. `MedicineTypeListProcessor` đăng ký module, gọi `MedicineTypeListFactory` → `MedicineTypeListBehavior` → khởi tạo `UCMedcineTypeList`.
2. `UCMedcineTypeList` gọi `LoadData()` để lấy danh sách `V_HIS_MEDICINE_TYPE` từ API `GetViewDynamic`.
3. Dữ liệu được đổ vào `MedicineTypeProcessor` (từ `HIS.UC.MedicineType`) qua `RefreshData()` / `Reload()`.
4. Tree control hiển thị danh sách với các cột được cấu hình trong `InItMedicineTypeTree()`.

---

## 4. API sử dụng

| Hằng số | Endpoint |
|---|---|
| `HIS_MEDICINE_TYPE_GETVIEW` | `api/HisMedicineType/GetView` |
| `HIS_MEDICINE_TYPE_GetViewDynamic` | `api/HisMedicineType/GetViewDynamic` |
| `HIS_MEDICINE_TYPE_CHANGE_LOCK` | `api/HisMedicineType/ChangeLock` |
| `HIS_MEDICINE_TYPE_DELETE` | `api/HisMedicineType/Delete` |

---

## 5. Danh sách cột hiển thị

| VisibleIndex | FieldName | Caption | Loại |
|---|---|---|---|
| 0 | MEDICINE_TYPE_CODE | Mã loại thuốc | Text (Fixed Left) |
| 1 | MEDICINE_TYPE_NAME | Tên loại thuốc | Text (Fixed Left) |
| 2 | SERVICE_UNIT_NAME_STR | Đơn vị tính | Unbound Object |
| 3 | ACTIVE_INGR_BHYT_NAME | Hoạt chất | Text |
| 4 | ACTIVE_INGR_BHYT_CODE | Mã hoạt chất | Text |
| 5 | CONCENTRA | Nồng độ hàm lượng | Text |
| 6 | HEIN_SERVICE_BHYT_CODE | Mã BHYT | Text |
| 7 | HEIN_SERVICE_BHYT_NAME | Tên BHYT | Text |
| 8 | MEDICINE_USE_FORM_CODE | Mã đường dùng | Text |
| 9 | MEDICINE_USE_FORM_NAME | Tên đường dùng | Text |
| 10 | PARENT_NAME_STR | Nhóm cha | Unbound Object |
| 11 | MEDICINE_GROUP_NAME | Nhóm thuốc | Text |
| **12** | **IS_NUTRITION_FOOD_BOOL** | **Thực phẩm dinh dưỡng** | **Unbound Boolean (Checkbox)** |
| 13 | BYT_NUM_ORDER | STT (TT40) | Text |
| 14 | HEIN_SERVICE_TYPE_NAME | Nhóm BHYT | Text |
| 15 | ATC_CODES | Mã ATC | Text |
| 16 | HEIN_LIMIT_RATIO_STR | Tỷ lệ BHYT | Unbound Object |
| 17 | REGISTER_NUMBER | Số đăng ký | Text |
| 18 | DOSAGE_FORM | Dạng bào chế | Text |
| 19 | NATIONAL_NAME | Quốc gia | Text |
| 20 | MANUFACTURER_NAME | Hãng sản xuất | Text |
| 21 | DESCRIPTION | Ghi chú | Text |
| 22 | LOCKING_REASON | Lý do khóa | Text |
| 23 | IMPORT_PRICE | Giá nhập | Unbound Object |
| 24 | EXPORT_PRICE | Giá bán | Unbound Object |

---

## 6. Thay đổi: Bổ sung cột "Thực phẩm dinh dưỡng"

### 6.1. Yêu cầu

Bổ sung cột **"Thực phẩm dinh dưỡng"** vào danh sách loại thuốc, đặt sau cột **Nhóm thuốc**, hiển thị dạng checkbox:
- `IS_NUTRITION_FOOD = 1` → tích (checked)
- `IS_NUTRITION_FOOD = null` → bỏ tích (unchecked)

### 6.2. Thiết kế database (do team DB/Backend thực hiện)

| Hạng mục | Nội dung |
|---|---|
| Sửa bảng `HIS_MEDICINE_TYPE` | Bổ sung cột `IS_NUTRITION_FOOD NUMBER(2,0)` |
| Sửa `PKG_INSERT_MEDICINE_TYPE` | Bổ sung lưu `IS_NUTRITION_FOOD` |
| Sửa `T_HIS_MEDICINE_TYPE` | Bổ sung `IS_NUTRITION_FOOD NUMBER(2,0)` |
| Sửa view `V_HIS_MEDICINE_TYPE` | Bổ sung `IS_NUTRITION_FOOD = IS_NUTRITION_FOOD (HIS_MEDICINE_TYPE)` |

### 6.3. Thiết kế backend (do team Backend thực hiện)

Sửa `MOS.OracleUDT.THisMedicineType`: bổ sung thuộc tính `IS_NUTRITION_FOOD (short?)`.

### 6.4. Thay đổi frontend (UCMedicineTypeList.cs)

**a) Bổ sung `IS_NUTRITION_FOOD` vào ColumnParams** (cả `LoadData()` và `FillDataToTreeControl()`):
```csharp
// Thêm vào cuối danh sách ColnParams / colunmParam
"IS_NUTRITION_FOOD"
```

**b) Thêm cột checkbox trong `InItMedicineTypeTree()`** (sau cột MEDICINE_GROUP_NAME):
```csharp
MedicineTypeColumn nutritionFoodCol = new MedicineTypeColumn(
    "Thực phẩm dinh dưỡng", "IS_NUTRITION_FOOD_BOOL", 120, false);
nutritionFoodCol.VisibleIndex = 12;
nutritionFoodCol.UnboundColumnType = DevExpress.XtraTreeList.Data.UnboundColumnType.Boolean;
ado.MedicineTypeColumns.Add(nutritionFoodCol);
```

**c) Xử lý giá trị trong `medicineType_CustomUnboundColumnData()`**:
```csharp
if (e.Column.FieldName == "IS_NUTRITION_FOOD_BOOL")
{
    if (data == null) return;
    e.Value = (data.IS_NUTRITION_FOOD == 1);
}
```

---

## 7. Ghi chú kỹ thuật

- Plugin sử dụng `HIS.UC.MedicineType` (DLL) để render tree control. Các cột unbound cần xử lý dữ liệu trong callback `MedicineType_CustomUnboundColumnData`.
- Cột `IS_NUTRITION_FOOD_BOOL` dùng `UnboundColumnType.Boolean` để DevExpress tự render checkbox (read-only).
- Dữ liệu được fetch qua `GetViewDynamic` với danh sách `ColumnParams` tường minh để tối ưu hiệu năng.
- Nếu `IS_NUTRITION_FOOD` chưa có trong `V_HIS_MEDICINE_TYPE` (chờ DB update), cột sẽ luôn hiển thị unchecked mà không gây lỗi runtime.
