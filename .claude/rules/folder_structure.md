---
description: Cấu trúc folder plugin — phân chia vai trò rõ ràng, partial class naming, khi nào tạo folder con. BẮT BUỘC theo khi tạo plugin mới
paths:
  - "HIS/Plugins/**"
---

# Folder Structure — Tổ Chức Plugin Theo Vai Trò

Mỗi folder/file có 1 vai trò duy nhất. Không trộn lẫn UI, business logic, data, config.

---

## 1. CẤU TRÚC CHUẨN THEO ĐỘ PHỨC TẠP

### Simple (1 form, 10-20 files)

```
HIS.Desktop.Plugins.{Name}/
│
├── {Name}/                        ← BEHAVIOR (logic nghiệp vụ)
│   ├── I{Name}.cs                 │  Interface
│   ├── {Name}Factory.cs           │  Factory tạo behavior
│   └── {Name}Behavior.cs          │  Business logic (kế thừa BusinessBase)
│
├── frm{Name}.cs                   ← UI (form code-behind, kế thừa FormBase)
├── frm{Name}.Designer.cs          │  Auto-generated — KHÔNG sửa
├── frm{Name}.resx                 │  Resources
│
├── ADO/                           ← DATA (data transfer objects)
│   └── {Entity}ADO.cs
│
├── Resources/                     ← LOCALIZATION + IMAGES
│   ├── Lang.vi.resx               │  UI labels tiếng Việt
│   ├── Lang.en.resx               │  UI labels English
│   ├── Message.Lang.vi.resx       │  Thông báo tiếng Việt
│   ├── Message.Lang.en.resx       │  Thông báo English
│   ├── ResourceLanguageManager.cs │  Language manager
│   └── ResourceMessage.cs         │  Message accessor
│
├── Properties/                    ← METADATA
│   └── AssemblyInfo.cs            │  [assembly: Plugin] BẮT BUỘC
│
├── {Name}Processor.cs             ← ENTRY POINT (MEF registration)
├── HisRequestUriStore.cs          ← API ENDPOINTS
└── HIS.Desktop.Plugins.{Name}.csproj
```

### Medium (nhiều form, config, 30-50 files)

```
HIS.Desktop.Plugins.{Name}/
│
├── {Name}/                        ← BEHAVIOR chính
│   ├── I{Name}.cs
│   ├── {Name}Factory.cs
│   ├── {Name}Behavior.cs
│   ├── frm{Name}.cs              │  Form TRONG behavior folder
│   ├── frm{Name}.Designer.cs
│   └── frm{Name}.resx
│
├── {SubFeature}/                  ← FORM PHỤ (feature riêng)
│   ├── frm{SubFeature}.cs
│   ├── frm{SubFeature}.Designer.cs
│   └── frm{SubFeature}.resx
│
├── ADO/                           ← DATA
│   ├── {Entity1}ADO.cs
│   └── {Entity2}ADO.cs
│
├── Config/                        ← CẤU HÌNH (load từ backend)
│   └── HisConfigCFG.cs
│
├── Resources/                     ← LOCALIZATION
│
├── Properties/
│
├── {Name}Processor.cs
├── KeyboardWorker.cs              ← PHÍM TẮT (cho UC)
├── GlobalStore.cs                 ← MODULE CACHE (static)
├── HisRequestUriStore.cs
└── HIS.Desktop.Plugins.{Name}.csproj
```

### Complex (partial classes, worker, 100+ files)

```
HIS.Desktop.Plugins.{Name}/
│
├── {Name}/                        ← BEHAVIOR + FORM CHÍNH
│   ├── I{Name}.cs
│   ├── {Name}Factory.cs
│   ├── {Name}Behavior.cs
│   │
│   ├── frm{Name}.cs              ← FORM CHÍNH
│   ├── frm{Name}.Designer.cs
│   ├── frm{Name}.resx
│   │── PARTIAL CLASSES:           ← TÁCH THEO VAI TRÒ
│   ├── frm{Name}__Load.cs        │  Khởi tạo, load data
│   ├── frm{Name}__InitCombo.cs   │  Setup combos/lookups
│   ├── frm{Name}__InitUC.cs      │  Setup User Controls
│   ├── frm{Name}__Process.cs     │  Logic xử lý chính
│   ├── frm{Name}__Save.cs        │  Lưu dữ liệu
│   ├── frm{Name}__Edit.cs        │  Chỉnh sửa
│   ├── frm{Name}__Check.cs       │  Validation logic
│   ├── frm{Name}__Print.cs       │  In ấn
│   └── frm{Name}__TabIndex.cs    │  Thứ tự tab
│   │
│   ├── {SubDialog}/              ← DIALOG CON
│   │   ├── frm{Dialog}.cs
│   │   ├── frm{Dialog}.Designer.cs
│   │   └── frm{Dialog}.resx
│
├── ADO/                           ← DATA
│   ├── {Entity1}ADO.cs
│   └── {Entity2}ADO.cs
│
├── Worker/                        ← BUSINESS LOGIC HELPERS
│   ├── {Feature1}Worker.cs        │  Logic xử lý riêng
│   └── {Feature2}Worker.cs
│
├── Save/                          ← SAVE BEHAVIORS (factory)
│   ├── ISave.cs
│   ├── SaveFactory.cs
│   ├── Create/
│   │   └── SaveCreateBehavior.cs
│   └── Update/
│       └── SaveUpdateBehavior.cs
│
├── RowAdd/                        ← GRID ROW LOGIC
│   ├── IAdd.cs
│   ├── AddFactory.cs
│   └── MedicineType/
│       └── MedicineTypeRowAddBehavior.cs
│
├── Config/                        ← CẤU HÌNH
│   ├── HisConfigCFG.cs
│   └── AppConfigKeys.cs
│
├── Validate/                      ← VALIDATION
│   ├── ValidateExam.cs
│   └── ValidateRule/
│       └── IcdValidationRule.cs
│
├── Extension/                     ← CUSTOM CONTROLS
│   └── CustomGridLookUpEdit.cs
│
├── Base/                          ← UTILITIES DÙNG CHUNG
│   ├── DataLocalStore.cs
│   ├── PrintTypeCodeWorker.cs
│   └── ResourceLangManager.cs
│
├── Print/                         ← IN ẤN
│   └── {Name}__Print.cs
│
├── Popup/                         ← CONTEXT MENU
│   └── PopupMenuProcessor.cs
│
├── Image/                         ← HÌNH ẢNH (nhiều file)
│   └── *.png, *.ico
│
├── Resources/                     ← LOCALIZATION
│
├── Properties/
│
├── {Name}Processor.cs
├── GlobalStore.cs
├── Delegate.cs                    ← DELEGATE RIÊNG PLUGIN
├── Enum{Name}.cs                  ← ENUM CONSTANTS
├── KeyboardWorker.cs
├── HisRequestUriStore.cs
└── HIS.Desktop.Plugins.{Name}.csproj
```

---

## 2. VAI TRÒ MỖI FOLDER

| Folder | Vai trò | Chứa gì | Khi nào tạo |
|--------|---------|---------|-------------|
| `{Name}/` | Behavior | Interface, Factory, Behavior, Form chính | LUÔN TẠO |
| `ADO/` | Data transfer | {Entity}ADO.cs — KHÔNG logic | Có >= 1 ADO class |
| `Config/` | Cấu hình | *CFG.cs load từ HisConfig/ConfigApp | Cần đọc config backend |
| `Worker/` | Business logic | *Worker.cs — xử lý phức tạp | Logic > 100 dòng, tái sử dụng |
| `Save/` | Lưu dữ liệu | ISave, Factory, Create/, Update/ | Nhiều kiểu save (Create vs Update, In vs Out) |
| `RowAdd/` `RowEdit/` | Grid rows | Factory + behavior theo entity type | Grid cho phép thêm/sửa nhiều loại |
| `Validate/` | Validation | ValidationRule classes | > 3 validation rules |
| `Extension/` | Custom controls | Custom DevExpress controls | Tùy chỉnh control DevExpress |
| `Base/` | Utilities | Managers, helpers, caching | > 3 utility classes dùng chung |
| `Print/` | In ấn | Print logic, template mapping | Có chức năng in |
| `Popup/` | Context menu | PopupMenuProcessor | Grid có right-click menu |
| `Image/` | Hình ảnh | PNG, ICO, GIF | > 5 image files |
| `Resources/` | Localization | Lang.*.resx, Message.*.resx, ResourceMessage.cs | LUÔN TẠO |
| `Properties/` | Metadata | AssemblyInfo.cs | LUÔN TẠO |
| `{SubFeature}/` | Form phụ | Form + Designer + resx | Form phụ có logic riêng |
| `Delegate/` | Delegates | Delegate riêng plugin | Plugin định nghĩa delegate mới |

---

## 3. PARTIAL CLASS — TÁCH FORM/UC LỚN

### Khi nào tách

- Form/UC > **500 dòng** → BẮT ĐẦU tách
- Form/UC > **1000 dòng** → BẮT BUỘC tách
- Mỗi partial file <= **500 dòng**

### Naming convention

**Form (double underscore `__`):**
```
frm{Name}.cs                      ← Main: declarations, constructor, fields
frm{Name}__Load.cs                ← Khởi tạo: combos, defaults, ControlState
frm{Name}__InitCombo.cs           ← Setup combos/lookups (nếu nhiều)
frm{Name}__InitUC.cs              ← Setup User Controls (Icd, DateEditor...)
frm{Name}__Process.cs             ← Logic xử lý chính
frm{Name}__Save.cs                ← SaveProcess, UpdateDTOFromDataForm
frm{Name}__Edit.cs                ← Edit mode, LoadCurrent
frm{Name}__Check.cs               ← Validation, CheckData
frm{Name}__Print.cs               ← Print logic
frm{Name}__TabIndex.cs            ← Tab order, keyboard navigation
frm{Name}__Dispose.cs             ← Cleanup, dispose
```

**UserControl (triple underscore `___`):**
```
UC{Name}.cs                        ← Main
UC{Name}___Load.cs                 ← Khởi tạo
UC{Name}___Process.cs              ← Logic chính
UC{Name}___CallPatient.cs          ← Feature cụ thể
UC{Name}___PACS_Process.cs         ← Tích hợp hệ thống ngoài
UC{Name}___Popup_Menu_Showing.cs   ← Context menu
UC{Name}___Shortcut.cs             ← Phím tắt
UC{Name}___ThreadLoad.cs           ← Async loading
UC{Name}__Dispose.cs               ← Cleanup (có thể __ hoặc ___)
```

**Variant: `__Plus__` cho nhóm tính năng:**
```
frm{Name}__Plus__Button.cs         ← Nhóm xử lý button
frm{Name}__Plus__GridFund.cs       ← Nhóm xử lý grid fund
frm{Name}__Plus__TreeList.cs       ← Nhóm xử lý tree
```

### Mỗi partial file PHẢI

```csharp
// Cùng namespace, cùng class (partial)
namespace HIS.Desktop.Plugins.{PluginName}
{
    public partial class frm{Name} : FormBase
    {
        // CHỈ logic thuộc vai trò của file này
    }
}
```

---

## 4. FILES GỐC (ROOT LEVEL)

| File | Vai trò | Khi nào tạo |
|------|---------|-------------|
| `{Name}Processor.cs` | MEF entry point | LUÔN |
| `HisRequestUriStore.cs` | API endpoint constants | Có gọi API |
| `GlobalStore.cs` | Module-level cache (static) | Cần share state giữa instances |
| `KeyboardWorker.cs` | Phím tắt ([KeyboardAction]) | Plugin là UserControl |
| `Delegate.cs` | Delegate riêng plugin | Plugin định nghĩa event mới |
| `Enum{Name}.cs` | Enum constants | Có > 3 magic values |
| `ModuleLinkString.cs` | Plugin ID string | ControlState cần moduleLink |

---

## 5. GLOBALSTORE / DATALOCALSTORE

```csharp
// GlobalStore.cs — static, lazy-load, module-level
internal class GlobalStore
{
    // Lazy-load pattern: load 1 lần, dùng nhiều
    private static List<V_HIS_MEDICINE_TYPE> _medicineTypes;
    public static List<V_HIS_MEDICINE_TYPE> MedicineTypes
    {
        get
        {
            if (_medicineTypes == null || _medicineTypes.Count == 0)
            {
                _medicineTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()
                    .OrderBy(o => o.MEDICINE_TYPE_CODE).ToList();
            }
            return _medicineTypes;
        }
        set { _medicineTypes = value; }
    }

    // Simple static fields cho trạng thái
    internal static bool isEnableDeleteAggregate;
    internal static long currentDepartmentId;
}
```

**Quy tắc**: CHỈ tạo GlobalStore khi plugin cần chia sẻ state giữa nhiều form/UC trong cùng plugin.

---

## 6. RESOURCES — LOCALIZATION

```
Resources/
├── Lang.vi.resx                   ← UI labels: button, caption, tooltip
├── Lang.en.resx
├── Lang.my.resx                   ← Myanmar (nếu cần)
├── Message.Lang.vi.resx           ← Thông báo: dialog, error, success
├── Message.Lang.en.resx
├── Message.Lang.my.resx
├── ResourceLanguageManager.cs     ← Holds ResourceManager instance
├── ResourceMessage.cs             ← Typed message accessor
└── *.png, *.gif, *.ico            ← Icons, images
```

**ResourceLanguageManager.cs:**
```csharp
class ResourceLanguageManager
{
    internal static ResourceManager LanguageResource;
}
```

**ResourceMessage.cs:**
```csharp
class ResourceMessage
{
    static ResourceManager languageMessage = new ResourceManager(
        "HIS.Desktop.Plugins.{Name}.Resources.Message.Lang",
        Assembly.GetExecutingAssembly());

    internal static string BanCoMuonXoaKhong
    {
        get
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    "BanCoMuonXoaKhong", languageMessage,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }
    }
}
```

---

## 7. CONFIG — LOAD TỪ BACKEND

```csharp
// Config/HisConfigCFG.cs
internal class HisConfigCFG
{
    internal static string SomeConfigValue;
    internal static long SomeConfigId;

    internal static void LoadConfig()
    {
        try
        {
            SomeConfigValue = HisConfigs.Get<string>("CONFIG_KEY_NAME");
            SomeConfigId = HisConfigs.Get<long>("CONFIG_KEY_ID");
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Warn(ex);
        }
    }
}

// Gọi trong Form_Load:
HisConfigCFG.LoadConfig();
```

---

## 8. .CSPROJ — HINTPATH REFERENCES

Plugin .csproj dùng HintPath tương đối để reference DLLs. Pattern nhất quán:

### Từ plugin (4 cấp `..\..\..\..\`) đến LIB/

```xml
<!-- EFMODEL, backend models -->
<Reference Include="MOS.EFMODEL">
  <HintPath>..\..\..\..\LIB\MOS\MOS.EFMODEL.dll</HintPath>
</Reference>

<!-- Inventec.* core libs -->
<Reference Include="Inventec.Common.Adapter">
  <HintPath>..\..\..\..\LIB\Inventec.Common\Inventec.Common.Adapter\Inventec.Common.Adapter.dll</HintPath>
</Reference>

<!-- IMSys.DbConfig constants -->
<Reference Include="IMSys.DbConfig.HIS_RS">
  <HintPath>..\..\..\..\LIB\IMSys.DbConfig\IMSys.DbConfig.HIS_RS\IMSys.DbConfig.HIS_RS.dll</HintPath>
</Reference>

<!-- MPS Print PDO (khi plugin có chức năng in) -->
<Reference Include="MPS.Processor.Mps000102.PDO">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000102.PDO.dll</HintPath>
</Reference>
<Reference Include="MPS.ProcessorBase">
  <HintPath>..\..\..\..\LIB\MPSv2\MPS.ProcessorBase\MPS.ProcessorBase.dll</HintPath>
</Reference>
```

### Từ plugin (2 cấp `..\..\`) đến HIS.Desktop.*

```xml
<!-- ProjectReference đến shared HIS libs -->
<ProjectReference Include="..\..\HIS.Desktop.ADO\HIS.Desktop.ADO.csproj">
  <Name>HIS.Desktop.ADO</Name>
</ProjectReference>
<ProjectReference Include="..\..\HIS.Desktop.Print\HIS.Desktop.Print.csproj">
  <Name>HIS.Desktop.Print</Name>
</ProjectReference>
```

### Đường dẫn giải thích

```
hisnguonmo/                          ← root repo
├── LIB/                             ← ..\..\..\..\LIB\ (4 cấp từ plugin)
│   ├── MOS/                         ← MOS.EFMODEL.dll, AutoMapper.dll
│   ├── IMSys.DbConfig/              ← Constants DLLs
│   ├── Inventec.Common/             ← Core libs (pre-built)
│   ├── Inventec.Desktop/            ← Desktop framework
│   ├── MPSv2/MPS.PDO/              ← Print PDO DLLs
│   └── HIS/                         ← HIS library DLLs
├── HIS/
│   ├── HIS.Desktop.ADO/            ← ..\..\  (2 cấp từ plugin)
│   ├── HIS.Desktop.Print/
│   └── Plugins/
│       └── HIS.Desktop.Plugins.X/  ← Plugin hiện tại
```

### Quy tắc

- KHÔNG thay đổi số cấp `..\..\` — phụ thuộc vị trí plugin trong tree
- Thêm reference mới → copy pattern từ plugin tương tự
- MPS PDO → thêm khi plugin có chức năng in (xem print_integration.md)
- DevExpress → GAC reference, KHÔNG cần HintPath

---

## 9. QUY TẮC TỔ CHỨC

| Quy tắc | Chi tiết |
|---------|----------|
| 1 vai trò / folder | ADO chỉ chứa data, Worker chỉ chứa logic, Config chỉ chứa config |
| Form trong Behavior folder | Form chính nằm trong {Name}/ cùng Interface/Factory/Behavior |
| Form phụ = subfolder | Mỗi form phụ có folder riêng: {SubFeature}/ |
| Partial khi > 500 dòng | frm__Feature (double `__`), UC___Feature (triple `___`) |
| GlobalStore chỉ khi cần | Static cache cho module — KHÔNG tạo nếu chỉ 1 form |
| Resources LUÔN tạo | Lang + Message tách riêng, có ResourceMessage.cs |
| Config/ khi đọc backend | *CFG.cs với LoadConfig() static |
| Worker/ khi logic > 100 dòng | Tách logic phức tạp ra Worker classes |
| Validate/ khi > 3 rules | Nhóm validation rules riêng folder |
| Root files là entry points | Processor, RequestUriStore, KeyboardWorker, GlobalStore |

### KHÔNG LÀM

- KHÔNG để business logic trong Form/UC → dùng Behavior hoặc Worker
- KHÔNG trộn ADO với logic → ADO chỉ chứa properties
- KHÔNG để > 2000 dòng trong 1 file → tách partial hoặc Worker
- KHÔNG tạo folder rỗng → chỉ tạo khi có >= 1 file
- KHÔNG để image trong root → dùng Resources/ hoặc Image/
- KHÔNG sửa .Designer.cs thủ công
- KHÔNG hardcode string → dùng Resources, Constants, Enum
