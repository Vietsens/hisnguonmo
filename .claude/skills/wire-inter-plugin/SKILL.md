---
name: wire-inter-plugin
description: Sinh code mở plugin khác — đọc Behavior.Run đích, xác định đầu vào, tạo ModuleLinkString, sinh code PluginInstance đầy đủ
user-invocable: true
argument-hint: <plugin hiện tại + plugin đích VD: "từ ExecuteRoom mở ContentSubclinical" hoặc "từ TreatmentList mở AssignPrescription">
---

# Wire Inter-Plugin — Sinh Code Mở Plugin Khác

Target: $ARGUMENTS

## Bước 1: Xác định 2 plugin

Từ mô tả, xác định:
- **Plugin HIỆN TẠI** (plugin cha — nơi sinh code)
- **Plugin ĐÍCH** (plugin con — cần mở)

Tìm path:
- Plugin hiện tại: `HIS/Plugins/HIS.Desktop.Plugins.{PluginHienTai}/`
- Plugin đích: `HIS/Plugins/HIS.Desktop.Plugins.{PluginDich}/`

## Bước 2: Đọc Behavior.Run() của plugin ĐÍCH (BẮT BUỘC)

**Đây là bước QUAN TRỌNG NHẤT — xác định plugin đích CẦN những tham số gì.**

Đọc file `{PluginDich}/{PluginDich}Behavior.cs` (hoặc file Behavior tương ứng).
Tìm method `Run()` hoặc `I{Name}.Run()`.
Tìm vòng lặp parse args:

```csharp
// Tìm đoạn code này trong Behavior.Run():
for (int i = 0; i < entity.Count(); i++)
{
    if (entity[i] is Module) moduleData = (Module)entity[i];
    if (entity[i] is long) treatmentId = (long)entity[i];
    if (entity[i] is DelegateSelectData) delegateSelect = ...;
    if (entity[i] is V_HIS_TREATMENT) treatment = ...;
    if (entity[i] is List<Action<Type>>) listAction = ...;
    if (entity[i] is List<ApiConsumer>) listConsumer = ...;
    if (entity[i] is List<long>) listLong = ...;
    if (entity[i] is List<string>) listString = ...;
}
```

Liệt kê TẤT CẢ kiểu được parse:

```
| # | Kiểu               | Tên biến        | Bắt buộc? |
|---|---------------------|-----------------|-----------|
| 1 | Module              | moduleData      | TỰ ĐỘNG (PluginInstance thêm) |
| 2 | long                | treatmentId     | CÓ / KHÔNG |
| 3 | DelegateSelectData  | delegateSelect  | CÓ / KHÔNG |
| 4 | V_HIS_TREATMENT     | treatment       | CÓ / KHÔNG |
| 5 | List<Action<Type>>  | listAction      | CÓ / KHÔNG |
| ...                                                    |
```

Xác định tham số nào BẮT BUỘC (plugin đích dùng mà không null check) và TÙY CHỌN (có null check).

## Bước 3: Kiểm tra/Tạo ModuleLinkString.cs

Tìm file `ModuleLinkString.cs` trong plugin HIỆN TẠI.

Nếu CHƯA CÓ → tạo mới:
```csharp
namespace HIS.Desktop.Plugins.{PluginHienTai}
{
    internal class ModuleLinkString
    {
        internal const string {PluginDich} = "HIS.Desktop.Plugins.{PluginDich}";
    }
}
```

Nếu ĐÃ CÓ → thêm constant mới cho plugin đích:
```csharp
internal const string {PluginDich} = "HIS.Desktop.Plugins.{PluginDich}";
```

## Bước 4: Sinh code trong plugin HIỆN TẠI

### 4a. Tìm vị trí gọi (button click, menu click, grid row action)

Tìm method sẽ trigger mở plugin đích.

### 4b. Sinh code đầy đủ

```csharp
private void Open{PluginDich}()
{
    try
    {
        // 1. Tìm module theo ID
        Inventec.Desktop.Common.Modules.Module moduleData =
            GlobalVariables.currentModuleRaws
                .FirstOrDefault(o => o.ModuleLink == ModuleLinkString.{PluginDich});

        if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
        {
            Inventec.Common.Logging.LogSystem.Warn(
                "Không tìm thấy plugin: " + ModuleLinkString.{PluginDich});
            return;
        }

        // 2. Tạo args — DỰA TRÊN Behavior.Run() đã đọc ở Bước 2
        List<object> listArgs = new List<object>();

        // Tham số BẮT BUỘC (từ bảng Bước 2):
        listArgs.Add((long)this.treatmentId);                      // long — PHẢI cast đúng kiểu

        // Tham số TÙY CHỌN:
        listArgs.Add(new DelegateSelectData(On{PluginDich}DataSelected));  // Callback
        // listArgs.Add(this.treatmentData);                       // V_HIS_TREATMENT (nếu cần)
        // listArgs.Add(new List<Action<Type>> { OnRefreshData }); // Action (nếu cần)

        // 3. Lấy instance với room context
        var instance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
            HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(
                moduleData,
                this.currentModule.RoomId,
                this.currentModule.RoomTypeId),
            listArgs);

        // 4. Null check + hiển thị
        if (instance == null)
        {
            Inventec.Common.Logging.LogSystem.Warn(
                "Không khởi tạo được plugin: " + ModuleLinkString.{PluginDich});
            return;
        }

        if (instance is Form)
            ((Form)instance).ShowDialog();
        else if (instance is UserControl)
        {
            // Nếu plugin đích là UC — add vào panel
            // panel{PluginDich}.Controls.Clear();
            // panel{PluginDich}.Controls.Add((UserControl)instance);
            // ((UserControl)instance).Dock = DockStyle.Fill;
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}
```

### 4c. Sinh callback method

```csharp
private void On{PluginDich}DataSelected(object data)
{
    try
    {
        if (data == null) return;

        // Cast data về kiểu THỰC TẾ mà plugin đích trả về
        // (xem trong Behavior.Run() hoặc Form của plugin đích — delegateSelect.Invoke(data))
        var result = data as {KieuTraVe};
        if (result != null)
        {
            // Xử lý data trả về
            FillDataToGrid();  // Refresh grid
        }
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

## Bước 5: Đối chiếu kiểu dữ liệu

Tạo bảng đối chiếu đảm bảo KHỚP:

```
| Plugin đích parse            | Plugin cha truyền                        | Khớp? |
|------------------------------|------------------------------------------|-------|
| entity[i] is long            | listArgs.Add((long)treatmentId)          | ✓     |
| entity[i] is DelegateSelectData | listArgs.Add(new DelegateSelectData()) | ✓     |
| entity[i] is V_HIS_TREATMENT | listArgs.Add(treatmentData)              | ✓     |
```

Kiểm tra:
- long KHÔNG phải int?
- DelegateSelectData KHÔNG phải Action<object>?
- List<object> KHÔNG phải object[]?
- V_HIS_TREATMENT KHÔNG phải HIS_TREATMENT (view vs entity)?

## Bước 6: Verify

- [ ] ModuleLinkString.cs có constant cho plugin đích
- [ ] Đã đọc Behavior.Run() của plugin đích
- [ ] Args truyền ĐỦ tham số BẮT BUỘC
- [ ] Kiểu dữ liệu KHỚP (long, DelegateSelectData, V_HIS_*)
- [ ] Null check moduleData trước khi gọi
- [ ] Null check instance trước khi Show
- [ ] Callback method có try-catch
- [ ] Toàn bộ code trong try-catch + LogSystem
- [ ] GetModuleWithWorkingRoom nếu plugin đích cần room context
