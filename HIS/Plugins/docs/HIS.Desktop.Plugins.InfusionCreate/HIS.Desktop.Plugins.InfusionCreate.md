# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.InfusionCreate - Tạo thông tin truyền dịch

**Module:** `HIS.Desktop.Plugins.InfusionCreate`
**Tên hiển thị:** Tạo thông tin truyền dịch
**Namespace:** `HIS.Desktop.Plugins.InfusionCreate`
**Loại module:** Form (MODULE_TYPE_ID__FORM)
**Nhóm:** Common

---

## 1. Tổng quan

Module **InfusionCreate** cho phép tạo và quản lý thông tin truyền dịch cho bệnh nhân trong hệ thống HIS. Chức năng chính:

- **Chọn thuốc** truyền dịch từ danh sách thuốc trong kho và ngoài kho
- **Nhập thông tin** truyền dịch: tốc độ, thời gian bắt đầu/kết thúc, bác sĩ chỉ định, điều dưỡng thực hiện
- **Lọc thuốc** theo thời gian chỉ định hoặc thời gian dự trù
- **In phiếu** truyền dịch

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.InfusionList/HIS.Desktop.Plugins.InfusionList/
├── InfusionCreate/
│   ├── IInfusionCreate.cs                (Interface hành vi)
│   ├── InfusionCreateFactory.cs          (Factory tạo behavior)
│   └── InfusionCreateBehavior.cs         (Behavior - tạo form chính)
├── ADO/
│   └── ComboSelectMedicineADO.cs         (ADO cho combo chọn thuốc)
├── Config/                               (Cấu hình)
├── Enum/                                 (Enum hằng số)
├── Validation/                           (Validation rules)
├── Properties/
│   └── AssemblyInfo.cs
├── InfusionCreateProcessor.cs            (Entry point - đăng ký plugin)
├── InfusionCreateADO.cs                  (ADO dữ liệu truyền dịch)
├── MEDITYPE.cs                           (Model thuốc cho combo)
├── frmInfusionCreate.cs                  (Form chính - logic nghiệp vụ)
├── frmInfusionCreate.Designer.cs         (Giao diện form - DevExpress)
├── frmInfusionCreate__Dispose.cs         (Cleanup và hủy event)
├── frmInfusionCreate.resx                (Resources)
└── HIS.Desktop.Plugins.InfusionCreate.csproj
```

---

## 3. Đăng ký Module

**File:** `InfusionCreateProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.InfusionCreate
Tên hiển thị : Tạo thông tin truyền dịch
Nhóm         : Common
Loại         : MODULE_TYPE_ID__FORM
Thuộc tính   : ExtensionOf(DesktopRootExtensionPoint)
```

**Luồng khởi tạo:**
```
InfusionCreateProcessor.Run(object[] args)
  → InfusionCreateFactory.MakeIInfusionCreate()
    → InfusionCreateBehavior.Run()
      → Trích xuất Module từ args
        → new frmInfusionCreate(moduleData)
```

---

## 4. Thiết kế chi tiết

### 4.1. frmInfusionCreate (`frmInfusionCreate.cs`)

Form chính kế thừa `HIS.Desktop.Utility.FormBase`, thực hiện toàn bộ nghiệp vụ tạo thông tin truyền dịch.

#### Các phương thức quan trọng

| Phương thức | Chức năng |
|-------------|-----------|
| `frmInfusionCreate_Load()` | Khởi tạo form: set icon, validate, load grid, load combo, set default |
| `FillDataCombo()` | Load danh sách thuốc trong kho + ngoài kho theo filter thời gian |
| `LoadDataToComboSelectMedicine()` | Load thuốc cho combo chọn thuốc (dùng khi đổi ngày truyền dịch) |
| `LoadDatatoCombo()` | Load dữ liệu vào combo loại thuốc |
| `SetDefaultValue()` | Set giá trị mặc định cho các control |
| `Loaddatatogrid()` | Load danh sách truyền dịch đã tạo lên grid |
| `SaveProcess()` | Lưu thông tin truyền dịch |
| `ValidControl()` | Thiết lập validation |
| `InitMenuToButtonPrint()` | Khởi tạo menu in phiếu |

### 4.2. MEDITYPE (`MEDITYPE.cs`)

Model chứa thông tin thuốc cho combo chọn:

| Field | Type | Mô tả |
|-------|------|-------|
| `ID` | long | ID bản ghi exp_mest_medicine |
| `MEDICINE_ID` | long? | ID thuốc |
| `MEDICINE_TYPE_CODE` | string | Mã loại thuốc |
| `MEDICINE_TYPE_NAME` | string | Tên loại thuốc |
| `AMOUNT` | decimal? | Số lượng |
| `SPEED` | string | Tốc độ truyền |
| `SERVICE_UNIT_ID` | long? | ID đơn vị tính |
| `SERVICE_UNIT_NAME` | string | Tên đơn vị tính |
| `INSTRUCTION_TIME` | long? | Thời gian chỉ định |
| `EXPIRED_DATE` | long? | Hạn sử dụng |
| `LOGGINNAME` | string | Người kê |
| `ngoaikho` | bool | Thuốc ngoài kho (true/false) |

---

## 5. Các API sử dụng

| Endpoint | Method | Mô tả |
|----------|--------|-------|
| `api/HisExpMestMedicine/GetView6` | GET | Lấy danh sách thuốc xuất kho (V_HIS_EXP_MEST_MEDICINE_6) |
| `api/HisServiceReqMety/GetView` | GET | Lấy danh sách thuốc kê ngoài kho (V_HIS_SERVICE_REQ_METY) |
| `api/HisInfusion/Create` | POST | Tạo thông tin truyền dịch |
| `api/HisInfusion/Get` | GET | Lấy danh sách truyền dịch |

---

## 6. Lịch sử thay đổi

### 6.1. Tài liệu 2388: Bổ sung lọc theo ngày dự trù ở màn truyền dịch

**Ngày:** 2026-04-02
**Yêu cầu:** Bổ sung cho phép lọc thuốc theo cả thời gian y lệnh và thời gian dự trù

#### Phân tích trước khi sửa

Trước đây, màn truyền dịch chỉ lọc thuốc theo **thời gian chỉ định** (TDL_INTRUCTION_DATE). Khi bệnh nhân có thuốc dự trù cho nhiều ngày, người dùng không thể lọc theo ngày dự trù để biết ngày nào bệnh nhân sẽ dùng thuốc nào.

#### Nội dung sửa đổi

**A. UI - Thêm ComboBox chọn loại thời gian (`cboTimeType`)**

- **Control:** `DevExpress.XtraEditors.ComboBoxEdit`
- **Layout item:** `layoutControlItemCboTimeType`
- **Vị trí:** Dòng filter thời gian, trước "Từ ngày"
- **Size:** 150 x 20 px
- **TextEditStyle:** `DisableTextEditor` (chỉ chọn, không gõ)
- **Items:**
  - Index 0: "Thời gian chỉ định" (mặc định)
  - Index 1: "Thời gian dự trù"
- **Event:** `SelectedIndexChanged` → reload danh sách thuốc

**B. Logic filter - Sửa `FillDataCombo()` và `LoadDataToComboSelectMedicine()`**

Khi `cboTimeType.SelectedIndex == 0` (Thời gian chỉ định):
```
HisExpMestMedicineView6Filter:
  TDL_INTRUCTION_DATE_FROM = ngày người dùng chọn (yyyyMMdd000000)
  TDL_INTRUCTION_DATE_TO   = ngày người dùng chọn (yyyyMMdd000000)

HisServiceReqMetyViewFilter:
  INTRUCTION_DATE_FROM = ngày người dùng chọn
  INTRUCTION_DATE_TO   = ngày người dùng chọn
```

Khi `cboTimeType.SelectedIndex == 1` (Thời gian dự trù):
```
HisExpMestMedicineView6Filter:
  TDL_USE_TIME_FROM = ngày người dùng chọn (yyyyMMdd000000)
  TDL_USE_TIME_TO   = ngày người dùng chọn + 235959

HisServiceReqMetyViewFilter:
  Không truyền filter ngày (lấy tất cả, lọc client-side)
```

**C. Client-side filter bổ sung**

Khi chọn "Thời gian dự trù", sau khi nhận kết quả từ API, áp dụng thêm filter phía client:
```csharp
lstExpMestMedicine6 = lstExpMestMedicine6
    .Where(o => o.TDL_USE_TIME.HasValue 
        && o.TDL_USE_TIME.Value >= ngayDuTru 
        && o.TDL_USE_TIME.Value <= ngayDuTru + 235959)
    .ToList();
```

Filter client-side được áp dụng tại 3 vị trí:
1. `LoadDataToComboSelectMedicine()` - thuốc trong kho (line ~302-309)
2. `FillDataCombo()` - thuốc trong kho (line ~614-621)
3. `FillDataCombo()` - thuốc ngoài kho (line ~666-673)

**D. Event handler mới**

```csharp
private void cboTimeType_SelectedIndexChanged(object sender, EventArgs e)
{
    // Khi đổi loại thời gian, reload danh sách thuốc
    // Chỉ reload nếu chưa chọn thuốc (cboMedicineType.EditValue == null)
    if (cboMedicineType.EditValue == null)
    {
        SetDefaultValue();
        glstMediType = FillDataCombo();
        cboMedicineType.Properties.DataSource = glstMediType;
        txtMedicinetype.Focus();
    }
}
```

**E. Khởi tạo mặc định**

Trong `frmInfusionCreate_Load()`:
```csharp
cboTimeType.SelectedIndex = 0;  // Mặc định: "Thời gian chỉ định"
```

#### Tóm tắt filter parameters

| Loại thời gian | SelectedIndex | Server-side filter | Client-side filter |
|----------------|---------------|--------------------|--------------------|
| Thời gian chỉ định | 0 | `TDL_INTRUCTION_DATE_FROM/TO` hoặc `TDL_INTRUCTION_DATE__EQUAL` | Không |
| Thời gian dự trù | 1 | `TDL_USE_TIME_FROM/TO` | `TDL_USE_TIME >= ngày && TDL_USE_TIME <= ngày + 235959` |

#### Files đã sửa (Frontend)

| File | Nội dung sửa |
|------|-------------|
| `frmInfusionCreate.Designer.cs` | Thêm control `cboTimeType` (ComboBoxEdit) + `layoutControlItemCboTimeType` |
| `frmInfusionCreate.cs` | Thêm default `cboTimeType.SelectedIndex = 0`, sửa logic filter trong `FillDataCombo()` và `LoadDataToComboSelectMedicine()`, thêm event `cboTimeType_SelectedIndexChanged` |

#### Phụ thuộc Backend (do người khác làm)

| Phụ thuộc | Mô tả |
|-----------|-------|
| V_HIS_EXP_MEST_MEDICINE_6 bổ sung field `TDL_USE_TIME`, `TDL_USE_DATE` | View DB cần thêm cột |
| api/HisExpMestMedicine/GetView6 hỗ trợ filter `TDL_USE_DATE_FROM/TO`, `TDL_USE_TIME_FROM/TO` | Backend filter mới |

---

## 7. Ghi chú kỹ thuật

### Format thời gian

Hệ thống sử dụng `long` dạng `yyyyMMddHHmmss`:
- Ngày: `20260401000000`
- Cuối ngày: `20260401235959`
- Hàm chuyển đổi: `Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber()`
