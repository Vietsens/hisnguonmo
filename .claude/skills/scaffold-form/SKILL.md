---
name: scaffold-form
description: Tạo Form mới đầy đủ chuẩn — FormBase, SetIcon, Load order, BarManager, ControlState, Validation, CRUD, Logging
user-invocable: true
argument-hint: <tên form VD: frmHisMachine, frmAssignService>
---

# Tạo Form Mới

Tạo form: $ARGUMENTS

## Bước 1: Tạo cấu trúc files

```
{PluginName}/
├── {Name}Processor.cs          ← MEF + Run(args)
├── {Name}/
│   ├── I{Name}.cs              ← Interface
│   ├── {Name}Factory.cs        ← Factory
│   └── {Name}Behavior.cs       ← Business logic (kế thừa BusinessBase)
├── frm{Name}.cs                ← Form code-behind (kế thừa FormBase)
├── frm{Name}.Designer.cs       ← Designer (DevExpress LayoutControl)
└── Properties/
    └── AssemblyInfo.cs          ← [assembly: Plugin] BẮT BUỘC
```

## Bước 2: Sinh Processor.cs

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.{Name}",
    "{Display Name tiếng Việt}",
    "Bussiness", 4, "icon.png", "A",
    Module.MODULE_TYPE_ID__FORM, true, true)]
public class {Name}Processor : ModuleBase, IDesktopRoot
{
    CommonParam param;
    public {Name}Processor() { param = new CommonParam(); }
    public {Name}Processor(CommonParam paramBusiness)
    { param = (paramBusiness != null ? paramBusiness : new CommonParam()); }

    public object Run(object[] args)
    {
        object result = null;
        try
        {
            I{Name} behavior = {Name}Factory.MakeIControl(param, args);
            result = behavior != null ? (object)(behavior.Run()) : null;
        }
        catch (Exception ex)
        {
            Inventec.Common.Logging.LogSystem.Error(ex);
            result = null;
        }
        return result;
    }

    public override bool IsEnable()
    {
        bool result = false;
        try { result = true; }
        catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        return result;
    }
}
```

## Bước 3: Sinh Interface, Factory, Behavior

### I{Name}.cs
```csharp
interface I{Name} { object Run(); }
```

### {Name}Factory.cs
```csharp
class {Name}Factory
{
    internal static I{Name} MakeIControl(CommonParam param, object[] data)
    {
        I{Name} result = null;
        try
        {
            result = new {Name}Behavior(param, data);
            if (result == null) throw new NullReferenceException();
        }
        catch (NullReferenceException ex)
        {
            Inventec.Common.Logging.LogSystem.Error(
                "Factory không khởi tạo được đối tượng."
                + data.GetType().ToString()
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
        }
        catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        return result;
    }
}
```

### {Name}Behavior.cs
```csharp
class {Name}Behavior : BusinessBase, I{Name}
{
    object[] entity;
    internal {Name}Behavior(CommonParam param, object[] filter) : base()
    { this.entity = filter; }

    object I{Name}.Run()
    {
        try
        {
            Module moduleData = null;
            DelegateSelectData delegateSelect = null;
            if (entity != null && entity.Count() > 0)
            {
                for (int i = 0; i < entity.Count(); i++)
                {
                    if (entity[i] is Module) moduleData = (Module)entity[i];
                    if (entity[i] is DelegateSelectData) delegateSelect = (DelegateSelectData)entity[i];
                }
            }
            return (delegateSelect != null)
                ? new frm{Name}(moduleData, delegateSelect)
                : new frm{Name}(moduleData);
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

## Bước 4: Sinh frm{Name}.cs

```csharp
public partial class frm{Name} : HIS.Desktop.Utility.FormBase
{
    #region Declare
    int rowCount = 0;
    int dataTotal = 0;
    int startPage = 0;
    int ActionType = -1;
    int positionHandle = -1;
    DelegateSelectData delegateSelect = null;
    Inventec.Desktop.Common.Modules.Module currentModule;
    HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
    List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
    bool isNotLoadWhileChangeControlStateInFirst = false;
    #endregion

    public frm{Name}(Module module) : base(module)
    {
        InitializeComponent();
        currentModule = module;
        SetIcon();
    }

    public frm{Name}(Module module, DelegateSelectData delegateData) : base(module)
    {
        InitializeComponent();
        currentModule = module;
        this.delegateSelect = delegateData;
        SetIcon();
    }

    private void SetIcon()
    {
        try
        {
            string iconPath = System.IO.Path.Combine(
                HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
        }
        catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
    }

    private void frm{Name}_Load(object sender, EventArgs e)
    {
        try
        {
            WaitingManager.Show();
            InitComboData();              // 1. Combos
            SetCaptionByLanguageKey();    // 2. Language
            ValidateForm();               // 3. Validation
            InitTabIndex();               // 4. Tab order
            SetDefaultValue();            // 5. Defaults
            InitControlState();           // 6. ControlState
            FillDataToGrid();             // 7. Grid
            WaitingManager.Hide();
        }
        catch (Exception ex)
        {
            WaitingManager.Hide();
            Inventec.Common.Logging.LogSystem.Error(ex);
        }
    }

    // Sinh thêm: InitComboData, SetCaptionByLanguageKey, ValidateForm,
    // InitTabIndex, SetDefaultValue, InitControlState, FillDataToGrid,
    // GridPaging, SaveProcess, DeleteProcess, ResetFormData,
    // UpdateDTOFromDataForm, SetFilterNavBar
    // Theo mẫu trong coding_rules.md và ui_rules.md
}
```

## Bước 5: Sinh AssemblyInfo.cs

```csharp
[assembly: AssemblyTitle("HIS.Desktop.Plugins.{Name}")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Inventec")]
[assembly: AssemblyProduct("HIS.Desktop.Plugins.{Name}")]
[assembly: AssemblyCopyright("Copyright © Inventec 2024")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: Inventec.Desktop.Core.Plugin]
```

## Bước 6: Sinh docs/ (BẮT BUỘC)

Tạo file `hisnguonmo/docs/{ModuleLink}.md` theo template trong `module_docs.md` — gọi skill `/document-module`.
VD: `docs/HIS.Desktop.Plugins.HisMachine.md`
Nếu file đã tồn tại → chỉ cập nhật Changelog + sections liên quan.

## Bước 7: Kiểm tra

- [ ] FormBase kế thừa đúng
- [ ] SetIcon() trong constructor
- [ ] Load order: Combo → Language → Validate → TabIndex → Default → ControlState → Grid
- [ ] SaveProcess: Validate → WaitingManager → BackendAdapter → MessageManager → SessionManager
- [ ] DeleteProcess: XtraMessageBox confirm YesNo
- [ ] Mọi method có try-catch + LogSystem
- [ ] AssemblyInfo có [assembly: Plugin]
- [ ] BarManager: this.AddBarManager(this.barManager1)
- [ ] docs/{ModuleLink}.md có đầy đủ 9 sections (folder chung hisnguonmo/docs/)
