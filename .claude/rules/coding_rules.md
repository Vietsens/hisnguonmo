---
description: Quy chuẩn coding C# — naming, architecture (Processor/Factory/Behavior), API calls, exception handling, DateTime, Config, Delegate, Constants. Áp dụng khi viết code trong HIS Desktop
paths:
  - "HIS/**"
  - "MPS/**"
  - "UC/**"
  - "Common/**"
---

# Coding Rules — C# HIS Desktop (Inventec Standard)

## 1. Naming Convention

### Quy tắc viết hoa

| Quy tắc | Áp dụng |
|---------|---------|
| PascalCase | Class, Method, Property, Namespace, Enum, Public Field |
| camelCase | Variable, Parameter, Private/Protected Field |
| UPPERCASE | Constant (2 ký tự: IO, UI) |

### Đặt tên chi tiết

- **Class**: Danh từ, PascalCase, KHÔNG prefix C/Class, KHÔNG dùng `_`. VD: `PatientService`, `HisMachineForm`
- **Interface**: Prefix `I` + PascalCase. VD: `IHisBed`, `IAppDelegacy`
- **Method**: Động từ, PascalCase. VD: `GetPatients()`, `SaveProcess()`, `FillDataToGrid()`
- **Property**: Danh từ, PascalCase. VD: `PatientName`
- **Parameter**: camelCase. VD: `string patientName`
- **Enum**: PascalCase, KHÔNG hậu tố `Enum`
- **Event**: Động từ dạng `-ing`/`-ed`: `Closing`, `Closed`

### UI Control Naming (BẮT BUỘC prefix)

| Prefix | Control | Prefix | Control |
|--------|---------|--------|---------|
| `lbl` | Label | `cbo` | ComboBox/LookUpEdit |
| `txt` | TextEdit | `lstv` | ListView |
| `btn` | SimpleButton | `tre` | TreeView/TreeList |
| `chk` | CheckEdit | `tab` | XtraTabControl |
| `rdo` | RadioButton | `dte` | DateEdit |
| `grp` | GroupBox | `spn` | SpinEdit |
| `pic` | PictureBox | `rtxt` | RichTextBox |
| `grd` | GridControl | `img` | ImageList |
| `lst` | ListBox | `tmr` | Timer |
| `mnu` | MainMenu | `err` | ErrorProvider |
| `pnl` | PanelControl | `bar` | BarManager |
| `lyt` | LayoutControl | `frm` | Form |

Menu: `mnu` + đường dẫn — `mnuFile`, `mnuFileNew`, `mnuEditCopy`
Data binding: field `CustomerCode` → `txtCustomerCode`, `cboCustomerCode`

### Lưu ý

- KHÔNG đặt tên chỉ khác hoa/thường
- KHÔNG viết tắt khó hiểu
- KHÔNG trùng keyword .NET
- Viết tắt chuẩn OK: XML, UI, IO, HTML, DNS
- Viết tắt > 2 ký tự dùng PascalCase: `HtmlButton` (KHÔNG `HTMLButton`)

---

## 2. Tổ Chức File

- Mỗi class 1 file, tên file = tên class, tối đa **2000 LOC**
- Thứ tự: `using` → `namespace` → `class/interface`
- Thứ tự trong class:
  1. `#region Declare` — Fields (private → public)
  2. Properties
  3. Constructors (đơn giản trước)
  4. Methods (nhóm theo chức năng)

### Namespace

Format: `CompanyName.Technology.Feature`
VD: `HIS.Desktop.Plugins.HisMachine`, `Inventec.Common.Logging`

---

## 3. Kiến Trúc Plugin (BẮT BUỘC)

```
Processor.Run(args) → Factory.MakeIControl(param, args) → Behavior.Run() → Form/UC
```

### Processor (MEF + entry point)

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.HisBed",    // Plugin ID (unique)
    "Giường bệnh",                   // Display name
    "Bussiness",                      // Category
    4,                                // Priority
    "bed.png",                        // Icon
    "A",                              // Group
    Module.MODULE_TYPE_ID__FORM,      // FORM / UC / COMBO
    true, true)]
public class HisBedProcessor : ModuleBase, IDesktopRoot
{
    CommonParam param;

    public HisBedProcessor() { param = new CommonParam(); }
    public HisBedProcessor(CommonParam paramBusiness)
    {
        param = (paramBusiness != null ? paramBusiness : new CommonParam());
    }

    public object Run(object[] args)
    {
        object result = null;
        try
        {
            IHisBed behavior = HisBedFactory.MakeIControl(param, args);
            result = behavior != null ? (object)(behavior.Run()) : null;
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
            result = null;
        }
        return result;
    }
}
```

### Factory

```csharp
class HisBedFactory
{
    internal static IHisBed MakeIControl(CommonParam param, object[] data)
    {
        IHisBed result = null;
        try
        {
            result = new HisBedBehavior(param, data);
            if (result == null) throw new NullReferenceException();
        }
        catch (NullReferenceException ex)
        {
            Inventec.Common.Logging.LogSystem.Error(
                "Factory không khởi tạo được đối tượng."
                + data.GetType().ToString()
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                ex);
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
        }
        return result;
    }
}
```

### Behavior

```csharp
class HisBedBehavior : BusinessBase, IHisBed
{
    object[] entity;

    internal HisBedBehavior(CommonParam param, object[] filter) : base()
    {
        this.entity = filter;
    }

    object IHisBed.Run()
    {
        try
        {
            Module moduleData = null;
            if (entity != null && entity.Count() > 0)
            {
                for (int i = 0; i < entity.Count(); i++)
                {
                    if (entity[i] is Module) moduleData = (Module)entity[i];
                }
            }
            return new frmHisBed(moduleData);
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
            param.HasException = true;
            return null;
        }
    }
}
```

---

## 4. API Call Patterns

Luồng: `Plugin → BackendAdapter → WebApiClient → HTTP`

### Post (Create / Update / Delete)

```csharp
CommonParam param = new CommonParam();
var result = new BackendAdapter(param).Post<HIS_MACHINE>(
    HisRequestUriStore.MOSHIS_HIS_MACHINE_CREATE,
    ApiConsumers.MosConsumer,
    updateDTO,
    param);
if (result != null) { success = true; }
```

### Get (trả về List)

```csharp
var data = new BackendAdapter(param).Get<List<HIS_MACHINE>>(
    HisRequestUriStore.MOSHIS_HIS_MACHINE_GET,
    ApiConsumers.MosConsumer, filter, param);
```

### GetRO (có paging info)

```csharp
var apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_MACHINE>>(
    HisRequestUriStore.MOSHIS_HIS_MACHINE_GET,
    ApiConsumers.MosConsumer, filter, paramCommon);

if (apiResult?.Data != null)
{
    gridControl.DataSource = apiResult.Data;
    rowCount = apiResult.Data.Count;
    dataTotal = apiResult.Param?.Count ?? 0;
}
```

### Filter pattern

```csharp
HisMachineFilter filter = new HisMachineFilter();
SetFilterNavBar(ref filter);
filter.ORDER_DIRECTION = "DESC";
filter.ORDER_FIELD = "MODIFY_TIME";
filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
```

### URI Store (tập trung trong HisRequestUriStore.cs)

```csharp
// Pattern: MOSHIS_HIS_{ENTITY}_{ACTION}
internal const string MOSHIS_HIS_MACHINE_CREATE = "api/HisMachine/Create";
internal const string MOSHIS_HIS_MACHINE_UPDATE = "api/HisMachine/Update";
internal const string MOSHIS_HIS_MACHINE_DELETE = "api/HisMachine/Delete";
internal const string MOSHIS_HIS_MACHINE_GET = "api/HisMachine/Get";
```

### Sau mỗi API call (BẮT BUỘC)

```csharp
WaitingManager.Hide();
MessageManager.Show(this, param, success);
SessionManager.ProcessTokenLost(param);
```

---

## 5. Exception Handling + Logging

Framework: **log4net 1.2.10** qua `Inventec.Common.Logging`

### 5 Logger classes

| Class | File output | Mục đích |
|-------|-------------|----------|
| `LogSystem` | Logs/LogSystem.txt | Exception, debug, trace |
| `LogAction` | Logs/LogAction.txt | Audit hành động user |
| `LogSession` | Logs/LogSession.txt | Phiên làm việc |
| `LogFilter` | Logs/LogFilter.txt | Filter/query |
| `LogTime` | Logs/LogTime.txt | Performance |

### Level — khi nào dùng

| Level | Dùng khi | Code |
|-------|----------|------|
| Error | Processor, Factory, Save/API fail | `LogSystem.Error(ex)` |
| Warn | UI events, Init combo, Load form | `LogSystem.Warn(ex)` |
| Debug | Trace data trước/sau API | `LogSystem.Debug(LogUtil.TraceData(...))` |
| Info | Audit action | `LogUtil.LogActionSuccess(class, method, user)` |

### Try-catch mỗi method (BẮT BUỘC)

```csharp
try { /* logic */ }
catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
```

### Debug trace data

```csharp
Inventec.Common.Logging.LogSystem.Debug(
    Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto));
// Output: ___dto:{"ID":1,"CODE":"M001"}___
```

### Error kèm context

```csharp
Inventec.Common.Logging.LogSystem.Error(
    "SaveProcess thất bại."
    + Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto),
    ex);
```

### Audit action (LogAction)

```csharp
Inventec.Common.Logging.LogUtil.LogActionSuccess("HisMachineBehavior", "Create", loginName);
Inventec.Common.Logging.LogUtil.LogActionFail("HisMachineBehavior", "Create", loginName);
```

### KHÔNG LÀM

- `catch (Exception ex) { }` — nuốt exception
- `Console.WriteLine(ex.Message)` — dùng LogSystem
- Log trong vòng lặp lớn
- Log PIN, password, CMND bệnh nhân

---

## 6. DateTime — Kiểu long (yyyyMMddHHmmss)

```csharp
// Form → long
long dateValue = Int64.Parse(dteFromTime.DateTime.ToString("yyyyMMdd000000"));

// long → DateTime
DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CREATE_TIME ?? 0);

// long → string hiển thị
string display = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);

// Chỉ lấy ngày (10 ký tự đầu)
string dateOnly = display.Length >= 10 ? display.Substring(0, 10) : display;
```

---

## 7. Constants — KHÔNG HARDCODE GIÁ TRỊ SỐ

**BẮT BUỘC**: Mọi giá trị trạng thái, loại, mã định danh từ database đều PHẢI dùng `IMSys.DbConfig` constants.
TUYỆT ĐỐI KHÔNG dùng số trực tiếp (1, 2, 3...) trong code.

### Pattern: `IMSys.DbConfig.[Schema].[Table].[Column/Enum]`

```csharp
// Schema của từng hệ thống:
IMSys.DbConfig.HIS_RS      // HIS (MOS backend)
IMSys.DbConfig.ACS_RS      // ACS (phân quyền)
IMSys.DbConfig.SDA_RS      // SDA (dữ liệu dùng chung)
IMSys.DbConfig.EMR_RS      // EMR (bệnh án điện tử)
IMSys.DbConfig.LIS_RS      // LIS (xét nghiệm)
```

### COMMON — Dùng chung tất cả bảng

```csharp
IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE    // = 1 (hoạt động)
IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE   // = 0 (khóa)
IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE     // = 1 (đã xóa)
IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__FALSE    // = 0 (chưa xóa)
```

### Trạng thái theo bảng cụ thể

```csharp
// Phiếu xuất (HIS_EXP_MEST_STT)
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT      // Nháp
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST     // Yêu cầu
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT      // Từ chối
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__APPROVAL    // Duyệt
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE        // Hoàn thành
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXPORT      // Đã xuất

// Phiếu nhập (HIS_IMP_MEST_STT)
IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT
IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST
IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL
IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT

// Loại xuất (HIS_EXP_MEST_TYPE)
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__SALE       // Bán
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PRESCRIPTION // Theo đơn
IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__AGGREGATION  // Tổng hợp

// Loại dịch vụ (HIS_SERVICE_TYPE)
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__EXAM         // Khám bệnh
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TEST         // Xét nghiệm
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__DIIM         // CĐHA
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN         // TDCN
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SURG         // Phẫu thuật
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PROC         // Thủ thuật
IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__BED          // Giường

// Đối tượng bệnh nhân (HIS_PATIENT_TYPE)
IMSys.DbConfig.HIS_RS.HIS_PATIENT_TYPE.ID__BHYT         // Bảo hiểm y tế
IMSys.DbConfig.HIS_RS.HIS_PATIENT_TYPE.ID__FEE          // Thu phí

// Tình trạng ra viện (HIS_TREATMENT_END_TYPE)
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CURED          // Khỏi
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RELIEVED       // Đỡ
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__UNCHANGED      // Không thay đổi
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__WORSE          // Nặng hơn
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__DEATH          // Tử vong

// Trạng thái điều trị (HIS_TREATMENT_TYPE)
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__EXAM       // Khám
IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__TREATMENT   // Điều trị nội trú

// Loại yêu cầu (HIS_SERVICE_REQ_TYPE)
IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__EXAM
IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TEST
IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DIIM
```

### Cách tìm constant đúng

```
Bước 1: Xác định BẢNG liên quan → VD: HIS_EXP_MEST có trạng thái
Bước 2: Tìm BẢNG STATUS tương ứng → HIS_EXP_MEST_STT
Bước 3: Dùng pattern: IMSys.DbConfig.HIS_RS.{BẢNG_STT}.ID__{TRẠNG_THÁI}
```

Nếu không biết constant → search trong codebase:
```csharp
// Search: IMSys.DbConfig.HIS_RS.{TABLE_NAME}
// Hoặc xem file IMSys.DbConfig.dll bằng ILSpy/dnSpy
```

### SAI vs ĐÚNG

```csharp
// SAI: Hardcode số — không biết 2 là gì, dễ sai khi backend đổi giá trị
if (data.EXP_MEST_STT_ID == 2) { ... }
if (data.SERVICE_TYPE_ID == 1) { ... }
if (data.IS_ACTIVE == 1) { ... }
if (data.PATIENT_TYPE_ID == 3) { ... }

// ĐÚNG: Dùng constant — rõ nghĩa, an toàn khi backend thay đổi
if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST) { ... }
if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__EXAM) { ... }
if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE) { ... }
if (data.PATIENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_PATIENT_TYPE.ID__BHYT) { ... }
```

### Khi KHÔNG tìm thấy constant trong IMSys.DbConfig → TẠO ENUM RIÊNG

Nếu giá trị chưa có trong `IMSys.DbConfig` (VD: trạng thái riêng plugin, loại xử lý nội bộ) → BẮT BUỘC tạo Enum có XML comment chú thích.

**File đặt tại**: `Enum{FeatureName}.cs` trong root folder plugin.

```csharp
/// <summary>
/// Trạng thái xử lý đơn thuốc trong plugin AssignPrescription.
/// Mapping với cột PROCESS_STATUS trong bảng nội bộ.
/// </summary>
public enum EnumPrescriptionProcessStatus
{
    /// <summary>Chưa xử lý — đơn mới tạo, chưa có hành động nào</summary>
    NotProcessed = 0,

    /// <summary>Đang xử lý — dược sĩ đang kiểm tra đơn</summary>
    Processing = 1,

    /// <summary>Đã cấp phát — thuốc đã được cấp cho bệnh nhân</summary>
    Dispensed = 2,

    /// <summary>Từ chối — dược sĩ từ chối đơn vì lý do X</summary>
    Rejected = 3,

    /// <summary>Hoàn thành — bệnh nhân đã nhận thuốc</summary>
    Completed = 4
}

/// <summary>
/// Loại hình thanh toán trong plugin Transaction.
/// </summary>
public enum EnumPaymentMethod
{
    /// <summary>Tiền mặt</summary>
    Cash = 1,

    /// <summary>Chuyển khoản ngân hàng</summary>
    BankTransfer = 2,

    /// <summary>Quẹt thẻ</summary>
    Card = 3,

    /// <summary>Ví điện tử (MoMo, ZaloPay...)</summary>
    EWallet = 4
}
```

**Quy tắc tạo Enum**:

| Quy tắc | Chi tiết |
|---------|----------|
| Naming | `Enum{FeatureName}{Purpose}` — PascalCase |
| XML comment class | Mô tả enum là gì, mapping với cột/bảng nào |
| XML comment mỗi value | Mô tả rõ nghĩa — developer khác đọc hiểu ngay |
| Giá trị tường minh | Luôn gán `= 0, = 1, = 2` — KHÔNG để compiler tự gán |
| File riêng | `Enum{Name}.cs` tại root plugin — KHÔNG để chung với form |
| Reuse | Kiểm tra plugin khác đã có enum tương tự chưa — KHÔNG tạo trùng |

**Sử dụng**:

```csharp
// ĐÚNG: Rõ nghĩa, có IntelliSense, có XML comment
if (data.ProcessStatus == (long)EnumPrescriptionProcessStatus.Dispensed) { ... }
switch ((EnumPaymentMethod)data.PAYMENT_METHOD)
{
    case EnumPaymentMethod.Cash: /* xử lý tiền mặt */ break;
    case EnumPaymentMethod.BankTransfer: /* xử lý chuyển khoản */ break;
}

// SAI: Vẫn hardcode số
if (data.ProcessStatus == 2) { ... }  // 2 là gì???
```

**Thứ tự ưu tiên khi gặp giá trị số**:

```
1. Tìm trong IMSys.DbConfig.[Schema].[Table] → CÓ → dùng ngay
2. Tìm trong codebase (Enum*.cs) của plugin khác → CÓ → reuse
3. KHÔNG có → TẠO Enum mới có XML comment đầy đủ
4. TUYỆT ĐỐI KHÔNG để số trực tiếp trong code
```

### CRUD Action Types (GlobalVariables — KHÔNG phải IMSys)

```csharp
GlobalVariables.ActionAdd      // = 1
GlobalVariables.ActionEdit     // = 2
GlobalVariables.ActionView     // = 3
GlobalVariables.ActionViewForEdit  // = 4
```

---

## 8. Config Access

```csharp
// Config toàn viện (HIS_CONFIG table)
string val = HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
// Key pattern: {MODULE}.{ENTITY}.{PROPERTY}[.QUALIFIER]

// Config per-user (SDA_CONFIG_APP)
int pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
```

---

## 9. Delegate — Giao tiếp giữa Plugins

```csharp
// Các delegate trong HIS.Desktop.Common/Delegate.cs
DelegateSelectData(object data)         // Trả data về plugin cha
DelegateRefreshData()                   // Thông báo refresh
DelegateReturnSuccess(bool success)     // Kết quả
RefeshReference()                       // Refresh references
DelegateCloseForm_Uc(object data)       // Đóng form trả data

// Parse trong Processor.Run()
for (int i = 0; i < args.Count(); i++)
{
    if (args[i] is Module) moduleData = (Module)args[i];
    if (args[i] is DelegateSelectData) delegateSelect = (DelegateSelectData)args[i];
}

// Gọi trong Form sau save
if (this.delegateSelect != null) this.delegateSelect(data);
```

---

## 10. Data Load

```csharp
// Danh mục (cache RAM) → BackendDataWorker
List<V_HIS_ROOM> rooms = BackendDataWorker.Get<V_HIS_ROOM>()
    .Where(o => o.IS_ACTIVE == 1).ToList();

// Dữ liệu nghiệp vụ (paging) → BackendAdapter
var apiResult = new BackendAdapter(param).GetRO<List<V_HIS_TREATMENT>>(
    HisRequestUriStore.URI, ApiConsumers.MosConsumer, filter, param);
```

---

## 11. Token / Auth

```csharp
string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore
    .ClientTokenManager.GetLoginName();
// Token tự động renew trong BusinessBase khi còn < 1 phút
```

---

## 12. Mapper

```csharp
Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EMPLOYEE>(updateDTO, currentData);
```

---

## 13. ADO Pattern

```csharp
// Mở rộng EFMODEL với property cho UI
public class MachineADO : HIS_MACHINE
{
    public bool IsChecked { get; set; }
}
```

---

## 14. Localization

```csharp
Resources.ResourceLanguageManager.LanguageResource =
    new ResourceManager("HIS.Desktop.Plugins.HisMachine.Resource.Lang",
        typeof(HisMachineForm).Assembly);

this.layoutItem.Text = Inventec.Common.Resource.Get.Value(
    "HisMachineForm.layoutItem.Text",
    Resources.ResourceLanguageManager.LanguageResource,
    LanguageManager.GetCulture());
```

---

## 15. Clean Code

- KHÔNG duplicate code
- Tên biến/method rõ nghĩa
- Method < 50 dòng, 1 nhiệm vụ
- KHÔNG nested quá 3 cấp
- KHÔNG hardcode credentials, connection string, token
- KHÔNG commit bin/, obj/, packages/, *.suo, *.user, Logs/

---

## Tổng Hợp Nhanh

| Quy tắc | Chi tiết |
|---------|----------|
| Architecture | Processor → Factory → Behavior → Form/UC |
| Base class | Form: `FormBase`, UC: `UserControlBase` |
| API Post | `BackendAdapter(param).Post<T>(URI, Consumer, DTO, param)` |
| API Get | `BackendAdapter(param).GetRO<List<T>>(URI, Consumer, Filter, param)` |
| Cache | `BackendDataWorker.Get<T>()` cho danh mục |
| After API | `WaitingManager.Hide()` → `MessageManager.Show()` → `SessionManager.ProcessTokenLost()` |
| DateTime | Kiểu `long`, format `yyyyMMddHHmmss` |
| Active | `IS_ACTIVE = 1`, soft delete `IS_ACTIVE = 0` |
| Action | `GlobalVariables.ActionAdd` (1), `ActionEdit` (2) |
| Exception | try-catch MỌI method. Error: critical. Warn: UI events |
| Debug | `LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => var), var))` |
| Audit | `LogAction.Info()` cho hành động user |
| Delegate | `DelegateSelectData` truyền data giữa plugins |
| Config | `HisConfigs.Get<T>(key)` toàn viện, `ConfigApplicationWorker.Get<T>(key)` per-user |
| Constants | `IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE` |
| Naming | PascalCase class/method, camelCase var, prefix control (btn, txt, cbo) |
| File | 1 class/file, max 2000 LOC |
