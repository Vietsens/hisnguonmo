---
name: scaffold-uc
description: Tạo UserControl mới đầy đủ chuẩn — UCBase, KeyboardWorker, Load pattern, ControlState
user-invocable: true
argument-hint: <tên UC VD: ucRegister, UCExecuteRoom>
---

# Tạo UserControl Mới

Tạo UC: $ARGUMENTS

## Bước 1: Tạo cấu trúc files

```
{PluginName}/
├── {Name}Processor.cs
├── {Name}/
│   ├── I{Name}.cs
│   ├── {Name}Factory.cs
│   └── {Name}Behavior.cs
├── uc{Name}.cs                 ← UC code-behind (kế thừa UserControlBase)
├── uc{Name}.Designer.cs
├── KeyboardWorker.cs           ← Phím tắt (khác với Form dùng BarManager)
└── Properties/
    └── AssemblyInfo.cs
```

## Bước 2: Sinh Processor, Factory, Behavior

Giống scaffold-form nhưng:
- Module.MODULE_TYPE_ID__UC thay vì MODULE_TYPE_ID__FORM
- Behavior.Run() trả về `new uc{Name}(moduleData)` thay vì `new frm{Name}(moduleData)`

## Bước 3: Sinh uc{Name}.cs

```csharp
public partial class uc{Name} : HIS.Desktop.Utility.UserControlBase
{
    #region Declare
    int rowCount = 0;
    int dataTotal = 0;
    int startPage = 0;
    int ActionType = -1;
    Inventec.Desktop.Common.Modules.Module currentModule;
    string loginName;
    HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
    List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
    bool isNotLoadWhileChangeControlStateInFirst = false;
    #endregion

    public uc{Name}(Module module) : base(module)
    {
        InitializeComponent();
        currentModule = module;
        this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore
            .ClientTokenManager.GetLoginName();
    }

    private void uc{Name}_Load(object sender, EventArgs e)
    {
        try
        {
            WaitingManager.Show();
            InitComboData();
            SetCaptionByLanguageKey();
            ValidateForm();
            InitTabIndex();
            SetDefaultValue();
            InitControlState();
            FillDataToGrid();
            WaitingManager.Hide();
        }
        catch (Exception ex)
        {
            WaitingManager.Hide();
            Inventec.Common.Logging.LogSystem.Error(ex);
        }
    }
}
```

## Bước 4: Sinh KeyboardWorker.cs

```csharp
[KeyboardAction("FindShortcut", "HIS.Desktop.Plugins.{PluginName}.uc{Name}", "FindShortcut",
    KeyStroke = XKeys.Control | XKeys.F)]
[KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.{PluginName}.uc{Name}", "SaveShortcut",
    KeyStroke = XKeys.Control | XKeys.S)]

[ExtensionOf(typeof(DesktopToolExtensionPoint))]
public sealed class KeyboardWorker : Tool<IDesktopToolContext>
{
    public KeyboardWorker() : base() { }
    public override IActionSet Actions { get { return base.Actions; } }
    public override void Initialize() { base.Initialize(); }
}
```

## Bước 5: Sinh docs/ (BẮT BUỘC)

Tạo file `hisnguonmo/docs/{ModuleLink}.md` theo template trong `module_docs.md` — gọi skill `/document-module`.
Nếu file đã tồn tại → chỉ cập nhật Changelog + sections liên quan.

## Bước 6: Kiểm tra

- [ ] UserControlBase kế thừa đúng
- [ ] GetLoginName() trong constructor
- [ ] KeyboardWorker.cs có [ExtensionOf(typeof(DesktopToolExtensionPoint))]
- [ ] MODULE_TYPE_ID__UC trong Processor
- [ ] Load order chuẩn
- [ ] AssemblyInfo có [assembly: Plugin]
- [ ] docs/{ModuleLink}.md có đầy đủ 9 sections (folder chung hisnguonmo/docs/)
