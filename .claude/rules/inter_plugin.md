---
description: Giao tiếp giữa plugins — PluginInstance.GetPluginInstance, ModuleLink, ShowModule. Áp dụng khi plugin A cần mở plugin B
paths:
  - "HIS/Plugins/**"
---

# Inter-Plugin Communication — Mở Plugin Động

## 1. Cách Mở Plugin Khác Từ Code

### Pattern chính: PluginInstance.GetPluginInstance()

```csharp
// Bước 1: Tìm module theo ModuleLink ID
Inventec.Desktop.Common.Modules.Module moduleData =
    GlobalVariables.currentModuleRaws
        .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical")
        .FirstOrDefault();

// Bước 2: Kiểm tra plugin có sẵn
if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
    return;

// Bước 3: Tạo args truyền cho plugin con
List<object> listArgs = new List<object>();
listArgs.Add(treatmentId);                                    // Data cần truyền
listArgs.Add((DelegateSelectData)OnChildDataSelected);        // Callback delegate
listArgs.Add(currentModule);                                  // Module context (nếu cần)

// Bước 4: Lấy instance (có room context)
var instance = PluginInstance.GetPluginInstance(
    PluginInstance.GetModuleWithWorkingRoom(
        moduleData,
        this.currentModule.RoomId,
        this.currentModule.RoomTypeId),
    listArgs);

// Bước 5: Hiển thị
if (instance == null) throw new ArgumentNullException("Plugin instance is null");

if (instance is Form)
    ((Form)instance).ShowDialog();       // Popup form
else if (instance is UserControl)
    panelHost.Controls.Add((UserControl)instance);  // Embed UC
```

### Pattern đơn giản (không cần room context)

```csharp
var moduleData = GlobalVariables.currentModuleRaws
    .FirstOrDefault(o => o.ModuleLink == "HIS.Desktop.Plugins.TargetPlugin");

var instance = PluginInstance.GetPluginInstance(moduleData, new List<object> { data });
if (instance is Form form) form.ShowDialog();
```

### Pattern qua ModuleExt (ShowModule helper)

```csharp
// Dùng HIS.Desktop.ModuleExt.PluginInstanceBehavior
HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(
    "HIS.Desktop.Plugins.TargetPlugin",  // ModuleLink
    currentModule.RoomId,                 // Room context
    currentModule.RoomTypeId,             // Room type
    new List<object> { data, delegate }   // Args
);
```

---

## 2. ModuleLink — Định Danh Plugin

ModuleLink là string ID duy nhất của mỗi plugin, trùng với Plugin ID trong [ExtensionOf].

### Khai báo constants (BẮT BUỘC)

```csharp
// File: ModuleLinkString.cs tại root plugin
internal class ModuleLinkString
{
    internal const string ExamServiceReqExecute = "HIS.Desktop.Plugins.ExamServiceReqExecute";
    internal const string ServiceExecute = "HIS.Desktop.Plugins.ServiceExecute";
    internal const string TestServiceExecute = "HIS.Desktop.Plugins.TestServiceExecute";
    internal const string ContentSubclinical = "HIS.Desktop.Plugins.ContentSubclinical";
}

// Sử dụng:
var module = GlobalVariables.currentModuleRaws
    .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.ExamServiceReqExecute);
```

### KHÔNG hardcode ModuleLink string trực tiếp

```csharp
// SAI:
.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ExamServiceReqExecute")

// ĐÚNG:
.Where(o => o.ModuleLink == ModuleLinkString.ExamServiceReqExecute)
```

---

## 3. XÁC ĐỊNH ĐẦU VÀO PLUGIN ĐÍCH (BƯỚC QUAN TRỌNG NHẤT)

**TRƯỚC KHI gọi PluginInstance.GetPluginInstance()** → BẮT BUỘC đọc Behavior.Run() của plugin đích để biết nó cần những tham số gì.

### Bước 3a: Mở Behavior.cs của plugin ĐÍCH

```
Tìm file: HIS/Plugins/{PluginĐích}/{PluginĐích}Behavior.cs
Đọc method: object I{Name}.Run() { ... }
Tìm vòng lặp parse args:
  for (int i = 0; i < entity.Count(); i++)
  {
      if (entity[i] is Module) ...
      if (entity[i] is long) ...
      if (entity[i] is DelegateSelectData) ...
      // → Đây là DANH SÁCH đầu vào plugin đích cần
  }
```

### Bước 3b: Liệt kê TẤT CẢ tham số plugin đích nhận

Ví dụ đọc được Behavior của plugin `ContentSubclinical`:
```
entity[i] is Module                  → BẮT BUỘC (tự động thêm bởi PluginInstance)
entity[i] is long                    → treatmentId (BẮT BUỘC)
entity[i] is DelegateSelectData      → callback trả data (TÙY CHỌN)
entity[i] is V_HIS_TREATMENT        → treatment data (TÙY CHỌN)
entity[i] is List<Action<Type>>      → refresh callback (TÙY CHỌN)
entity[i] is List<ApiConsumer>       → API consumers (TÙY CHỌN)
entity[i] is List<long>              → IDs (TÙY CHỌN)
entity[i] is List<string>            → string params (TÙY CHỌN)
```

### Bước 3c: Truyền ĐỦ các tham số BẮT BUỘC

```csharp
// Xây dựng args DỰA TRÊN những gì Behavior.Run() THỰC SỰ parse
List<object> listArgs = new List<object>();

// Tham số BẮT BUỘC — plugin đích sẽ CRASH nếu thiếu
listArgs.Add(treatmentId);                              // long — Behavior cần

// Tham số TÙY CHỌN — plugin đích check null trước khi dùng
listArgs.Add(new DelegateSelectData(OnDataReturn));     // Callback
listArgs.Add(treatmentData);                            // V_HIS_TREATMENT
listArgs.Add(new List<Action<Type>> { OnRefresh });     // Action list

// Nếu plugin đích cần List<ApiConsumer>:
listArgs.Add(new List<ApiConsumer> { ApiConsumers.MosConsumer, ApiConsumers.SarConsumer });

// Nếu plugin đích cần List<long>:
listArgs.Add(new List<long> { roomId });

// Nếu plugin đích cần List<string>:
listArgs.Add(new List<string> { appCode, iconPath });
```

### Bước 3d: ĐỐI CHIẾU — đảm bảo khớp kiểu

| Plugin đích parse | Plugin cha truyền | Khớp? |
|-------------------|-------------------|-------|
| `entity[i] is long` | `listArgs.Add(treatmentId)` — treatmentId là long | ✓ |
| `entity[i] is DelegateSelectData` | `listArgs.Add(new DelegateSelectData(callback))` | ✓ |
| `entity[i] is V_HIS_TREATMENT` | `listArgs.Add(treatmentData)` — kiểu V_HIS_TREATMENT | ✓ |
| `entity[i] is List<Action<Type>>` | `listArgs.Add(new List<Action<Type>> { ... })` | ✓ |

**SAI PHỔ BIẾN**:
```csharp
// SAI: Truyền int thay vì long — plugin đích parse `is long` sẽ KHÔNG match
listArgs.Add((int)treatmentId);
// ĐÚNG:
listArgs.Add((long)treatmentId);

// SAI: Truyền Action thay vì DelegateSelectData — kiểu khác nhau
listArgs.Add(new Action<object>(callback));
// ĐÚNG:
listArgs.Add(new DelegateSelectData(callback));

// SAI: Truyền object[] thay vì List<object>
// PluginInstance.GetPluginInstance nhận List<object>, KHÔNG phải object[]
```

### Ví dụ đầy đủ — Mở plugin ContentSubclinical

```csharp
// 1. XÁC ĐỊNH: Đọc ContentSubclinicalBehavior.Run() → cần: long treatmentId, DelegateSelectData
// 2. TÌM MODULE:
var moduleData = GlobalVariables.currentModuleRaws
    .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.ContentSubclinical);
if (moduleData == null || !moduleData.IsPlugin) return;

// 3. TRUYỀN ĐỦ tham số (dựa trên bước 1):
List<object> listArgs = new List<object>();
listArgs.Add((long)this.treatmentId);                              // BẮT BUỘC: long
listArgs.Add(new DelegateSelectData(OnSubclinicalDataSelected));   // TÙY CHỌN: callback

// 4. GỌI:
var instance = PluginInstance.GetPluginInstance(
    PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId),
    listArgs);

// 5. HIỂN THỊ:
if (instance != null && instance is Form)
    ((Form)instance).ShowDialog();
```

### KHÔNG LÀM

```csharp
// SAI: Đoán tham số — không đọc Behavior.Run() trước
listArgs.Add(treatmentId);
listArgs.Add(patientId);     // Plugin đích có cần patientId không? KHÔNG BIẾT!
listArgs.Add(someString);    // Plugin đích parse string nào? KHÔNG BIẾT!
// → Dễ lỗi runtime khi plugin đích không nhận đúng kiểu

// SAI: Truyền thừa — plugin đích chỉ cần long, nhưng truyền thêm object thừa
// → Không lỗi nhưng code confusing, khó maintain
```

---

## 4. Callback Từ Plugin Con

```csharp
// Plugin cha — định nghĩa callback:
private void OnChildDataSelected(object data)
{
    try
    {
        var result = data as HIS_ENTITY;
        if (result != null)
        {
            FillDataToGrid();  // Refresh data
        }
    }
    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
}

// Plugin con — gọi callback sau save:
if (this.delegateSelect != null)
    this.delegateSelect(savedData);
```

---

## 5. Quy Tắc

| Quy tắc | Chi tiết |
|---------|----------|
| ModuleLink là constant | Khai báo trong ModuleLinkString.cs — KHÔNG hardcode string |
| Null check module | Kiểm tra moduleData != null && IsPlugin trước khi gọi |
| Null check instance | Kiểm tra instance != null trước khi Show/Add |
| Args parse bằng `is` | KHÔNG ép kiểu trực tiếp: `(long)args[0]` |
| Delegate nullable | Plugin con KHÔNG bắt buộc có delegate |
| Try-catch bao quanh | Toàn bộ quá trình mở plugin trong try-catch |
| Room context | Dùng `GetModuleWithWorkingRoom()` khi plugin cần biết phòng hiện tại |
