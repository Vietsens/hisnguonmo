---
name: fe-create-plugin
description: Tạo module plugin HIS Desktop hoàn chỉnh theo chuẩn wiki Vietsens/hisnguonmo — scaffold đầy đủ Processor, Factory, Behavior, Form/UC, ControlState, và hướng dẫn release.
user-invocable: true
argument-hint: <tên plugin> <loại: FORM|UC|COMBO>
---

# Tạo Module Plugin HIS Desktop

> Dựa trên tài liệu chuẩn: [Vietsens/hisnguonmo Wiki — Các bước xây dựng & release 1 module](https://github.com/Vietsens/hisnguonmo/wiki)

## Bước 1: Đọc tài liệu tham chiếu

Đọc `FRONTEND/hisnguonmo/HIS/CLAUDE.md` để nắm plugin architecture.
Đọc 1 plugin mẫu đơn giản trong `FRONTEND/hisnguonmo/HIS/Plugins/` (VD: `HIS.Desktop.Plugins.HisMachine` hoặc `HIS.Desktop.Plugins.CallBriefPatient`) để xem cấu trúc thực tế.

## Bước 2: Xác định thông tin

Từ `$ARGUMENTS`:
- **$0** = Tên plugin (VD: `HisMachine`, `AssignBlood`)
- **$1** = Loại hiển thị:
  - `FORM` → Popup form riêng (`MODULE_TYPE_ID__FORM`)
  - `UC` → Tab nhúng (`MODULE_TYPE_ID__UC`)
  - `COMBO` → Dropdown (`MODULE_TYPE_ID__COMBO`)

**Quy tắc đặt tên**: `HIS.Desktop.Plugins.{$0}`
- Tiền tố `HIS` (3 ký tự mã phần mềm)
- Tên mô tả ngắn gọn, PascalCase, theo chức năng module

## Bước 3: Tạo cấu trúc folder và files

Tạo trong `FRONTEND/hisnguonmo/HIS/Plugins/HIS.Desktop.Plugins.{$0}/`:

```
HIS.Desktop.Plugins.{Name}/
├── {Name}Processor.cs                  ← Đăng ký plugin + hàm Run()
├── {Name}/
│   ├── I{Name}.cs                      ← Interface contract
│   ├── {Name}Factory.cs                ← Factory tạo behavior
│   └── {Name}Behavior.cs               ← Core logic
├── frm{Name}.cs                        ← Form UI (nếu FORM)
├── frm{Name}.Designer.cs               ← Form Designer (nếu FORM)
├── UC{Name}.cs                         ← UserControl (nếu UC)
├── UC{Name}.Designer.cs                ← UC Designer (nếu UC)
├── ControlStateConstant.cs             ← Constants cho lưu trạng thái local (tùy chọn)
├── Properties/
│   └── AssemblyInfo.cs
└── HIS.Desktop.Plugins.{Name}.csproj
```

## Bước 4: Code Templates

### 4.1 Processor — `{Name}Processor.cs`

```csharp
using System;
using System.Linq;
using Inventec.Desktop.Common.Modules;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.{Name}
{
    // Khai báo module type:
    // - Module.MODULE_TYPE_ID__FORM   → popup form
    // - Module.MODULE_TYPE_ID__UC     → tab (UserControl)
    // - Module.MODULE_TYPE_ID__COMBO  → dropdown
    public class {Name}Processor
    {
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (args != null && args.Count() > 0)
                {
                    for (int i = 0; i < args.Count(); i++)
                    {
                        if (args[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)args[i];
                        }
                    }
                }
                // FORM type:
                result = new frm{Name}(moduleData);
                // UC type:
                // result = new UC{Name}(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
```

### 4.2 Form — `frm{Name}.cs` (nếu loại FORM)

Kế thừa `HIS.Desktop.Utility.FormBase`:

```csharp
using System;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.{Name}
{
    public partial class frm{Name} : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;

        public frm{Name}(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;

                // BẮT BUỘC: Đăng ký barManager để hỗ trợ Ctrl+Shift+S (tùy chỉnh nút)
                // Ctrl+Shift+H (ẩn nút) và Ctrl+Shift+C (cấu hình) tự động từ base
                this.AddBarManager(this.barManager1);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
```

### 4.3 UserControl — `UC{Name}.cs` (nếu loại UC)

Kế thừa `HIS.Desktop.Utility.UserControlBase`:

```csharp
using System;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.{Name}
{
    public partial class UC{Name} : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UC{Name}(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.moduleData = module;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
```

### 4.4 ControlState — Lưu trạng thái checkbox tại local (tùy chọn)

Nếu plugin có checkbox cần nhớ trạng thái, thêm pattern sau:

**ControlStateConstant.cs:**
```csharp
namespace HIS.Desktop.Plugins.{Name}
{
    internal class ControlStateConstant
    {
        internal const string chkOption1 = "chkOption1";
        internal const string chkOption2 = "chkOption2";
    }
}
```

**Trong Form/UC — khai báo biến:**
```csharp
bool isNotLoadWhileChangeControlStateInFirst;
HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
string moduleLink = "HIS.Desktop.Plugins.{Name}";
```

**InitControlState() — gọi trong constructor sau InitializeComponent:**
```csharp
void InitControlState()
{
    try
    {
        isNotLoadWhileChangeControlStateInFirst = true;
        this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
        this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
        if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
        {
            foreach (var item in this.currentControlStateRDO)
            {
                if (item.KEY == ControlStateConstant.chkOption1)
                {
                    chkOption1.Checked = item.VALUE == "1";
                }
            }
        }
        isNotLoadWhileChangeControlStateInFirst = false;
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

**CheckedChanged event handler:**
```csharp
private void chkOption1_CheckedChanged(object sender, EventArgs e)
{
    try
    {
        if (isNotLoadWhileChangeControlStateInFirst) return;

        var csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
            ? this.currentControlStateRDO
                .Where(o => o.KEY == ControlStateConstant.chkOption1 && o.MODULE_LINK == moduleLink)
                .FirstOrDefault()
            : null;

        if (csAddOrUpdate != null)
        {
            csAddOrUpdate.VALUE = chkOption1.Checked ? "1" : "";
        }
        else
        {
            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO
            {
                KEY = ControlStateConstant.chkOption1,
                VALUE = chkOption1.Checked ? "1" : "",
                MODULE_LINK = moduleLink
            };
            if (this.currentControlStateRDO == null)
                this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
            this.currentControlStateRDO.Add(csAddOrUpdate);
        }
        this.controlStateWorker.SetData(this.currentControlStateRDO);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

### 4.5 Required Namespaces

```csharp
using Inventec.Desktop.Common.Modules;    // Module class
using Inventec.Common.Logging;             // LogSystem
using HIS.Desktop.Utility;                // FormBase, UserControlBase
using HIS.Desktop.Library.CacheClient;    // ControlStateWorker (tùy chọn)
```

## Bước 5: Keyboard Shortcuts tích hợp sẵn

Kế thừa `FormBase`/`UserControlBase` tự động có:

| Phím tắt | Chức năng |
|----------|-----------|
| **Ctrl+Shift+S** | Tùy chỉnh tên nút & phím tắt (cần `AddBarManager`) |
| **Ctrl+Shift+H** | Ẩn/hiện nút (tự động từ base) |
| **Ctrl+Shift+C** | Cấu hình riêng cho chức năng (tự động từ base) |

## Bước 6: Build & Release

### Build
```bash
# MSBuild command
msbuild HIS.Desktop.Plugins.{Name}.csproj /p:Configuration=Release /p:Platform=AnyCPU
```

### Release — Copy DLL vào đúng vị trí

```
RELEASE/
├── Integrate/
├── Plugins/
│   ├── FrdProcessor/          ← DLL form động
│   ├── Module/                ← ⭐ COPY DLL MODULE VÀO ĐÂY
│   └── MpsProcessor/          ← DLL print/report
├── ReferencedAssemblies/      ← ⭐ COPY THƯ VIỆN PHỤ THUỘC VÀO ĐÂY
├── x64/
└── x86/
```

**Quy tắc release:**
- Copy file `.dll` và `resources.dll` vào `Plugins/Module/`
- Copy thư viện phụ thuộc (nếu mới) vào `ReferencedAssemblies/`
- **KHÔNG copy**: `.pdb`, `.xml` (tránh rác)
- **Tuyệt đối tuân thủ** cấu trúc folder này để đảm bảo kiến trúc chuẩn

## Bước 7: Confirm trước khi tạo

Trình bày cho user:
1. Tên đầy đủ: `HIS.Desktop.Plugins.{$0}`
2. Loại: FORM / UC / COMBO
3. Danh sách files sẽ tạo
4. Có cần ControlState không?

Chờ user xác nhận rồi mới tạo files.

## Input từ user

$ARGUMENTS
