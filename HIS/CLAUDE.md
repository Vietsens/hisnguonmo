# FRONTEND — HIS Desktop (WinForms)

Desktop application WinForms với 992 plugin modules. Framework: .NET Framework 4.5, MEF plugin system.

## Plugin Architecture

```
User click menu → Processor.Run() → Factory.Make() → Behavior.Run() → UI Form/UserControl
```

### Pattern code cho mỗi plugin

```
HIS.Desktop.Plugins.{Name}/
├── {Name}Processor.cs              ← [ExtensionOf] — đăng ký plugin với MEF
├── {Name}/
│   ├── I{Name}.cs                  ← Interface
│   ├── {Name}Factory.cs            ← Factory — tạo behavior instance
│   └── {Name}Behavior.cs           ← Core logic — implements interface
├── frm{Name}.cs                    ← WinForms UI
├── frm{Name}.Designer.cs
└── Properties/
```

### Registration

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.{Name}",   // Plugin ID
    "Tên hiển thị",                  // Vietnamese display name
    "Category",                      // Grouping
    14,                              // Sort order
    "icon.png",                      // Icon
    "A",                             // Keyboard shortcut
    Module.MODULE_TYPE_ID__FORM,     // FORM or UC
    true, true)]
```

## Core Libraries

| Library | Vai trò |
|---------|---------|
| `HIS.Desktop.Common` | BusinessBase, EntityBase, delegates, interfaces |
| `HIS.Desktop.ApiConsumer` | 15 backend consumers (MOS, EMR, LIS, ACS, SAR, SDA, HTC, SCN, QCS, VVA, CRM, MCH...) |
| `HIS.Desktop.LocalStorage.*` | 14 modules: LocalData, ConfigSystem, HisConfig, EmrConfig, LisConfig, PubSub, Branch... |
| `HIS.Desktop.Print` | Print/report generation |
| `HIS.Desktop.Utility` | Utility functions |
| `HIS.Desktop.Notify` | Notification system |
| `HIS.Desktop.ADO` | Data access objects |
| `HIS.Desktop.DelegateRegister` | Plugin delegation |
| `HIS.Desktop.ModuleExt` | Module extensions |

## API Endpoints

File `HIS.Desktop.ApiConsumer/HisRequestUriStore.cs` (52KB) chứa 100+ API endpoint URIs.
Mỗi consumer riêng: `AcsRequestUriStore`, `LisRequestUriStore`, `SarRequestUriStore`...

## Plugin Index

Xem `Plugins/CLAUDE.md` để tìm plugin theo chức năng y tế (992 plugins phân nhóm).

---

## Convention & Style Guide

### Plugin naming
- **Plugin folder**: `HIS.Desktop.Plugins.{PascalCaseName}` (VD: `HIS.Desktop.Plugins.CallBriefPatient`)
- **Processor**: `{Name}Processor.cs` — bắt buộc có `[ExtensionOf]` attribute
- **Factory**: `{Name}Factory.cs` — static method `MakeI{Name}(CommonParam param, object[] data)`
- **Behavior**: `{Name}Behavior.cs` — implement `I{Name}`, method `Run()` trả về `object`
- **Interface**: `I{Name}.cs` — định nghĩa contract
- **Form**: `frm{Name}.cs` + `frm{Name}.Designer.cs` (cho FORM type)
- **UserControl**: `UC{Name}.cs` + `UC{Name}.Designer.cs` (cho UC type)

### Code style
- **Error handling**: `try/catch` + `LogSystem.Error(ex)` — **KHÔNG** swallow exception (catch trống)
- **API calls**: Qua `HIS.Desktop.ApiConsumer` — **KHÔNG** gọi `HttpClient`/`WebRequest` trực tiếp
- **Messages**: Dùng `HIS.Desktop.LibraryMessage` cho message strings — **KHÔNG** hardcode text
- **Config**: Dùng `HIS.Desktop.LocalStorage.ConfigSystem` — **KHÔNG** tự đọc config file
- **Events**: Dùng `HIS.Desktop.LocalStorage.PubSub` cho inter-plugin communication
- **Null check**: Luôn check null trước khi dùng Factory result (`if (result == null) throw new NullReferenceException()`)

## Yêu cầu Testing

- **Hiện tại**: Không có automated test cho Frontend WinForms
- **Test thủ công** khi thêm/sửa plugin:
  1. Build `HIS.Desktop.sln` thành công
  2. Deploy DLL vào thư mục Plugins
  3. Mở HIS Desktop → verify menu hiện đúng plugin
  4. Click menu → form/UC mở đúng, không crash
  5. Thao tác nghiệp vụ → API call thành công (check response)
  6. Đóng form → không memory leak (check TaskManager)
- **Test regression**: Kiểm tra plugins liên quan không bị ảnh hưởng bởi thay đổi shared libraries

## SVN Workflow

- Frontend nằm trong `FRONTEND/hisnguonmo/` — commit cả solution
- **Commit message**: `[vCong] FE: Mô tả thay đổi`
- **KHÔNG commit**: `bin/`, `obj/`, `packages/`, `*.suo`, `*.user`, `*.v11.suo`
- **Trước khi commit**: Build `HIS.Desktop.sln` thành công, `svn update`, resolve conflicts
- **Lưu ý**: Solution có 992+ projects — build lần đầu có thể lâu

## Bảo mật & Tuân thủ

- **Token**: Quản lý qua `TokenCodeStore` — **KHÔNG** lưu token vào biến static tự tạo hoặc file local
- **Token renewal**: Qua `BusinessBase.TokenCheck()` — **KHÔNG** tự implement logic refresh token
- **Sensitive data**: **KHÔNG** hiển thị thông tin bệnh nhân nhạy cảm trên log/console (`LogSystem`)
- **Login**: Dùng `Inventec.UC.Login.Base.ClientTokenManagerStore` — **KHÔNG** tự tạo form login
- **Local storage**: Chỉ dùng `HIS.Desktop.LocalStorage.*` cho cache — **KHÔNG** lưu dữ liệu nhạy cảm ra file local

## Checklist trước khi commit

- [ ] Build `HIS.Desktop.sln` thành công
- [ ] Plugin đăng ký đúng `[ExtensionOf]` attribute (ID, display name, category, type)
- [ ] Factory trả về interface (không trả về concrete class trực tiếp)
- [ ] Error handling: `try/catch` + `LogSystem.Error` (không catch trống)
- [ ] API call qua `ApiConsumer` (không HttpClient/WebRequest trực tiếp)
- [ ] Form/UC `Dispose` đúng (không memory leak — unsubscribe events, dispose controls)
- [ ] **KHÔNG** commit `bin/`, `obj/`, `packages/`, `*.suo`, `*.user`
- [ ] Test thủ công: menu → form → thao tác nghiệp vụ → API call thành công
- [ ] Commit message: `[vCong] FE: Mô tả`
