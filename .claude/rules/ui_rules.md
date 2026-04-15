# UI Rules — WinForms + DevExpress 15.2.9
## 1. Technology Direction (BẮT BUỘC RÕ)
- UI Framework: WinForms
- UI Library: DevExpress 15.2.9
- Không sử dụng control WinForms mặc định nếu DevExpress đã có thay thế
- Ưu tiên DevExpress cho toàn bộ UI
- Giao diện design ở các form, userControl cần phải tách biệt rõ ràng, không có chứa code gọi API bên trong.
- Không xử lý giao diện trong các phần thread.

## 2. Form Load - Thứ tự khởi tạo
- Nếu là Form thì bắt buộc kế thừa lại từ HIS.Desktop.Utility.FormBase.
- Nếu là UserControl thì bắt buộc kế thừa lại từ HIS.Desktop.Utility.UserControlBase.
- Đối vối WinForms thì phải có Icon
Đoạn code mẫu Icon: 
private void SetIcon()
{
    try
    {
        string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
        this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Error(ex);
    }
}

## 3. Layout
- LayoutControlGroup: EnableIndentsWithoutBorders = True.
- Các LayoutControlItem (labelControl, SpinEdit, TextEdit, ...) AppearanceitemCaption/TextOptions/HAlignment = Far, TextAlignMode = CustomSize, TextSize = (x,20) (x là giá trị căn theo design để giao diện không bị mất chữ, thẳng hằng với các control dòng dưới)
- Với các control là MemoEdit, chú ý tạo design để người dùng hiểu là nhập nhiều dòng. AppearanceitemCaption/TextOptions/WordWrap = Wrap.
- Các trường dữ liệu bắt buộc nhập, hiển thị mẫu chữ ở LayoutControlItem là màu Marroon.
- Các vùng trống trên giao diện cần chèn EmptySpace vào vùng đó.
- Các từ hint (từ ẩn) luôn chỉnh thiết lập giá trị khi không có value thì hiển thị.
- Hạn chế sử dụng quá nhiều layout.
- Tiêu chuẩn thiết kế giao diện là trên độ phân giải 1366x768, đảm bảo giao diện nhỏ cũng hiển thị đủ control, giao diện lớn không bị thừa khoảng trắng.
- Với những mục, nhóm nhỏ có 1 tập control trong đó cần thêm 1 cái group bao toàn bộ control, sử dụng control cho phép scroll ngang, dọc khi giao diện không hiển thị đủ.
- Caption dài quá có thể viết tắt nhưng trong tooltip phải có tên, chú thích đầy đủ.
- Ưu tiên tái sử dụng các HIS.UC hiện đã có trong file uc_guide.md

## 4. GridControl, TreeView
- Caption tiếng việt, có dấu, khi co kéo cột phải đảm bảo hiển thị đủ caption (có thể ngắt xuống dòng).
- Các cột hiển thị Icon thì không cần caption, Width co đủ bằng icon.
- Mặc định AutoWidth = true. Nếu có quá nhiều cột thì để AutoWidth = false và có thiết lập Fixed = Left với các cột quan trọng (icon, mã bệnh nhân, tên bệnh nhân, mã điều trị, tên điều trị, ...).
- Các RepositoryItemButton ưu tiên sử dụng Image trong Dx Image gallery.

### 4 cột audit BẮT BUỘC khi grid load 1 bảng/view

Khi GridControl hiển thị dữ liệu từ 1 bảng hoặc 1 view duy nhất → BẮT BUỘC có 4 cột audit **ở cuối grid**, theo đúng thứ tự:

| # | FieldName | Caption | UnboundType | Format | Mô tả |
|---|-----------|---------|-------------|--------|-------|
| 1 | `CREATE_TIME_STR` | Thời gian tạo | Object (Unbound) | `TimeNumberToTimeString` | Thời gian tạo bản ghi |
| 2 | `CREATOR` | Người tạo | Bound | string | Tài khoản tạo |
| 3 | `MODIFY_TIME_STR` | Thời gian sửa | Object (Unbound) | `TimeNumberToTimeString` | Thời gian sửa cuối |
| 4 | `MODIFIER` | Người sửa | Bound | string | Tài khoản sửa cuối |

**Trong Designer.cs:**
```csharp
// 4 cột cuối grid — theo đúng thứ tự
this.gcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
this.gcCreator = new DevExpress.XtraGrid.Columns.GridColumn();
this.gcModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
this.gcModifier = new DevExpress.XtraGrid.Columns.GridColumn();

// Thời gian tạo — Unbound (format từ long sang string)
this.gcCreateTime.Caption = "Thời gian tạo";
this.gcCreateTime.FieldName = "CREATE_TIME_STR";
this.gcCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
this.gcCreateTime.OptionsColumn.AllowEdit = false;
this.gcCreateTime.Width = 120;

// Người tạo — Bound trực tiếp
this.gcCreator.Caption = "Người tạo";
this.gcCreator.FieldName = "CREATOR";
this.gcCreator.OptionsColumn.AllowEdit = false;
this.gcCreator.Width = 100;

// Thời gian sửa — Unbound
this.gcModifyTime.Caption = "Thời gian sửa";
this.gcModifyTime.FieldName = "MODIFY_TIME_STR";
this.gcModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
this.gcModifyTime.OptionsColumn.AllowEdit = false;
this.gcModifyTime.Width = 120;

// Người sửa — Bound trực tiếp
this.gcModifier.Caption = "Người sửa";
this.gcModifier.FieldName = "MODIFIER";
this.gcModifier.OptionsColumn.AllowEdit = false;
this.gcModifier.Width = 100;
```

**Trong CustomUnboundColumnData — format datetime:**
```csharp
if (e.Column.FieldName == "CREATE_TIME_STR")
{
    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
        data.CREATE_TIME ?? 0);
}
else if (e.Column.FieldName == "MODIFY_TIME_STR")
{
    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(
        data.MODIFY_TIME ?? 0);
}
```

**Lưu ý:**
- 4 cột này nằm ở CUỐI grid — sau tất cả cột nghiệp vụ.
- CREATOR và MODIFIER là Bound (field có sẵn trong EFMODEL).
- CREATE_TIME và MODIFY_TIME là Unbound vì cần format từ `long` sang `string` hiển thị.
- Nếu grid hiển thị data tổng hợp từ NHIỀU bảng (join) → không bắt buộc 4 cột này.
- AllowEdit = false — user KHÔNG ĐƯỢC sửa cột audit.

## 5. Phím tắt
- Đối với FormBase. Sử dụng BarManager để tạo phím tắt cho các nút.
- Đối với UserControl. Tạo file KeyboardWorker.cs để tạo phím tắt, method trong đó.
Đoạn Code mẫu:
 [KeyboardAction("FindShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "FindShortcut", KeyStroke = XKeys.Control | XKeys.F)]
 [KeyboardAction("ExecuteShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "ExecuteShortcut", KeyStroke = XKeys.Control | XKeys.X)]

 [ExtensionOf(typeof(DesktopToolExtensionPoint))]
 public sealed class KeyboardWorker : Tool<IDesktopToolContext>
 {
     public KeyboardWorker() : base() { }

     public override IActionSet Actions
     {
         get
         {
             return base.Actions;
         }
     }

     public override void Initialize()
     {
         base.Initialize();
     }
 }

 ## 6. Validation
 - Các control bắt buộc nhập hay cảnh báo về độ dài, rule gì khác đều phải thể hiện tại vị trí control. Icon warning phía trước control, ErrorText là nội dung.
 - Cần Clear Icon, ErrorText khi đã đúng valid hoặc khi nhấn mới.

## 7. Tối ưu tốc độ Load Form/UC

### SuspendLayout khi khởi tạo nhiều controls
```csharp
this.SuspendLayout();
this.layoutControl1.SuspendLayout();
this.panelControl1.SuspendLayout();
// ... thêm controls, set properties ...
this.panelControl1.ResumeLayout(false);
this.layoutControl1.ResumeLayout(false);
this.ResumeLayout(false);
```
- Designer.cs tự động có SuspendLayout/ResumeLayout — KHÔNG xóa.
- Nếu thêm controls bằng code (runtime) → BẮT BUỘC bọc trong SuspendLayout.

### Load data theo thứ tự ưu tiên
```
1. UI controls hiển thị ngay (combo, labels)     ← Load trước
2. Grid data (cần API call)                       ← Load sau, có WaitingManager
3. Sub-features (print config, shortcuts)         ← FormBase tự load thread riêng
```
- Combo/LookUp lấy từ BackendDataWorker (cache RAM) → nhanh, load trước.
- Grid data gọi API (GetRO) → chậm hơn, load sau cùng trong Load event.
- KHÔNG load tất cả data rồi mới hiển thị form — cho form hiện trước, data load sau.

### Lazy-load tab pages
- Tab chưa click → KHÔNG load data.
- Chỉ load khi user click vào tab lần đầu.
```csharp
bool isTabDetailLoaded = false;
private void xtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
{
    if (e.Page == xtraTabPageDetail && !isTabDetailLoaded)
    {
        isTabDetailLoaded = true;
        LoadDetailData();
    }
}
```

## 8. Tối ưu GridControl

### BeginUpdate/EndUpdate — BẮT BUỘC
```csharp
gridView.BeginUpdate();
try
{
    gridControl.DataSource = data;
}
finally
{
    gridView.EndUpdate();
}
```
- Không có BeginUpdate → grid repaint MỖI DÒNG khi bind → cực chậm với >100 rows.

### Tắt features không cần
```csharp
gridView.OptionsView.ShowGroupPanel = false;       // Tắt group panel
gridView.OptionsView.ShowIndicator = false;        // Tắt indicator column
gridView.OptionsFind.AllowFindPanel = false;       // Tắt find panel
gridView.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
gridView.OptionsCustomization.AllowSort = false;   // Tắt sort cho cột checkbox
gridView.OptionsView.AnimationType = GridAnimationType.NeverAnimate; // Tắt animation
```

### Pre-compute data TRƯỚC khi bind — KHÔNG tính trong CustomUnboundColumnData
```csharp
// SAI: Mỗi dòng grid repaint → gọi lại CustomUnboundColumnData → chậm
private void gridView_CustomUnboundColumnData(...)
{
    var dept = BackendDataWorker.Get<HIS_DEPARTMENT>()  // GỌI MỖI DÒNG!
        .FirstOrDefault(o => o.ID == data.DEPT_ID);
    e.Value = dept?.DEPARTMENT_NAME;
}

// ĐÚNG: Tính trước, lưu vào ADO
var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID);
var adoList = rawData.Select(o => {
    var ado = new TreatmentADO();
    Mapper.Map(o, ado);
    deptDict.TryGetValue(o.DEPARTMENT_ID, out var dept);
    ado.DepartmentName = dept?.DEPARTMENT_NAME;
    return ado;
}).ToList();
gridControl.DataSource = adoList;

// CustomUnboundColumnData CHỈ còn: STT, icon status → rất nhẹ
```

### Paging — KHÔNG load toàn bộ records
- Luôn dùng server-side paging qua `ucPaging.Init(GridPaging, param, pageSize, gridControl)`.
- Default pageSize từ `ConfigApplications.NumPageSize` (thường 50-100).
- Grid chỉ hiển thị 1 trang → render nhanh.

### RowCellStyle nhẹ
```csharp
// RowCellStyle gọi MỖI CELL khi repaint → phải nhẹ nhất có thể
private void gridView_RowCellStyle(...)
{
    // ĐÚNG: So sánh đơn giản, không LINQ, không API call
    if (data.IS_DELETE == 1)
        e.Appearance.ForeColor = Color.Red;

    // SAI: Gọi BackendDataWorker, FirstOrDefault, string processing phức tạp
}
```

## 9. Tối ưu LookUpEdit / ComboBox

### Dùng ControlEditorLoader — tự động optimize
```csharp
ControlEditorLoader.Load(cboEntity, listData, controlEditorADO);
// Nội bộ đã optimize: set DataSource, columns, display/value member
```

### Cache data combo — KHÔNG load lại mỗi lần mở
```csharp
// ĐÚNG: Load 1 lần trong InitComboData(), lưu field
private List<V_HIS_ROOM> listRoom;
private void InitComboData()
{
    listRoom = BackendDataWorker.Get<V_HIS_ROOM>()
        .Where(o => o.IS_ACTIVE == 1).ToList();
    ControlEditorLoader.Load(cboRoom, listRoom, roomEditorADO);
}

// SAI: Mỗi lần cần → gọi lại BackendDataWorker + ControlEditorLoader
```

### PopupFormWidth vừa đủ
```csharp
cbo.Properties.PopupFormWidth = 350; // Đủ rộng hiển thị columns
// Quá rộng → popup che form, quá hẹp → cắt text
```

## 10. Tối ưu Layout Rendering

### Hạn chế nested LayoutControl
- Tối đa 2 cấp nested: LayoutControl > LayoutControlGroup > LayoutControlItem.
- KHÔNG nest 3-4 cấp LayoutControl → chậm rendering, phức tạp khi maintain.
- Thay vì nest thêm LayoutControl → dùng LayoutControlGroup với GroupBordersVisible = false.

### EmptySpaceItem thay vì size cố định
```csharp
// ĐÚNG: EmptySpaceItem tự co giãn → responsive
this.emptySpaceItem1.Size = new Size(0, 0);
this.emptySpaceItem1.SizeConstraintsType = SizeConstraintsType.Default;

// SAI: Set Width/Height cố định cho LayoutControlItem → vỡ layout trên độ phân giải khác
```

### MinSize cho controls quan trọng
```csharp
// Đảm bảo TextEdit không bị co quá nhỏ
this.lciPatientName.MinSize = new Size(200, 24);
this.lciPatientName.SizeConstraintsType = SizeConstraintsType.Custom;
```

## 11. Tối ưu Memory cho Form/UC

### Dispose đúng khi đóng
- FormBase.OnFormClosing tự động: DisposeAllControl, null references, GC.Collect.
- UCBase.DisposeExt() tự động: tương tự + GC.WaitForPendingFinalizers.
- Override `ProcessDisposeModuleDataAfterClose()` nếu có data riêng cần clear:
```csharp
protected override void ProcessDisposeModuleDataAfterClose()
{
    try
    {
        listData = null;
        currentData = null;
        dictCache = null;
    }
    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
}
```

### KHÔNG giữ reference lớn không cần
```csharp
// SAI: Field lưu 10000 records suốt đời form
private List<V_HIS_TREATMENT> allTreatments;

// ĐÚNG: Clear sau khi xử lý xong
allTreatments = null;
// Hoặc dùng local variable thay field nếu chỉ dùng 1 method
```

### Image/Icon tối ưu
- Dùng ImageList cho grid icons — KHÔNG load từ file mỗi lần.
- Icon size: 16x16 cho grid, 24x24 cho button — KHÔNG dùng ảnh lớn rồi scale down.
- Ưu tiên DevExpress Image Gallery thay PNG tùy chỉnh.

## 12. UX — Trải nghiệm người dùng

### WaitingManager cho mọi thao tác > 0.5s
```csharp
WaitingManager.Show();
try { /* API call, data processing */ }
finally { WaitingManager.Hide(); }
```
- User KHÔNG ĐƯỢC thấy form đóng băng mà không có loading indicator.

### Focus đúng control sau mỗi thao tác
- Sau save thành công → focus control đầu tiên (code/name).
- Sau delete → focus dòng tiếp theo trong grid.
- Enter trên TextEdit → focus sang control tiếp theo.
```csharp
private void SetDefaultFocus()
{
    txtCode.Focus();
    txtCode.SelectAll();
}
```

### Responsive trên nhiều độ phân giải
- Thiết kế chuẩn: 1366x768 (laptop phổ biến bệnh viện).
- LayoutControl tự co giãn → test trên 1366x768, 1920x1080, 1280x720.
- Grid AutoWidth = true mặc định → columns tự chia đều.
- Nhiều cột (>10) → AutoWidth = false + Fixed = Left cho cột quan trọng.

### Phản hồi ngay khi thao tác
- Button click → disable ngay để tránh double-click.
```csharp
btnSave.Enabled = false;
try { SaveProcess(); }
finally { btnSave.Enabled = true; }
```
- Grid row click → highlight ngay, KHÔNG đợi API trả về.
- Validation error → hiện ngay tại control, KHÔNG popup MessageBox cho từng field.

## 13. SetCaptionByLanguageKey — Đa Ngôn Ngữ (BẮT BUỘC)

**MỌI plugin PHẢI có đa ngôn ngữ** — tối thiểu 2 file: `Lang.vi.resx` + `Lang.en.resx`.
**MỌI text hiển thị trên UI** (label, button, caption, tooltip, thông báo) PHẢI khai báo trong .resx — KHÔNG hardcode tiếng Việt.

### 13a. Files BẮT BUỘC tạo

```
{PluginName}/Resources/
├── Lang.vi.resx                   ← TẤT CẢ tên hiển thị tiếng Việt
├── Lang.en.resx                   ← TẤT CẢ tên hiển thị tiếng Anh
├── Message.Lang.vi.resx           ← Câu thông báo tiếng Việt
├── Message.Lang.en.resx           ← Câu thông báo tiếng Anh
├── ResourceLanguageManager.cs     ← Giữ ResourceManager instance
└── ResourceMessage.cs             ← Accessor cho Message (mỗi câu 1 property)
```

### 13b. ResourceLanguageManager.cs (BẮT BUỘC)

```csharp
using System.Resources;

namespace HIS.Desktop.Plugins.{PluginName}.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource;
    }
}
```

### 13c. Lang.vi.resx — Khai báo TẤT CẢ tên UI

**Mọi control có Text/Caption PHẢI có entry trong Lang.resx.**
Key format: `{FormName}.{ControlName}.Text`

```xml
<!-- Lang.vi.resx -->
<data name="frmHisMachine.lciMachineCode.Text" xml:space="preserve">
  <value>Mã máy</value>
</data>
<data name="frmHisMachine.lciMachineName.Text" xml:space="preserve">
  <value>Tên máy</value>
</data>
<data name="frmHisMachine.btnSave.Text" xml:space="preserve">
  <value>Lưu (Ctrl+S)</value>
</data>
<data name="frmHisMachine.btnNew.Text" xml:space="preserve">
  <value>Mới (Ctrl+N)</value>
</data>
<data name="frmHisMachine.btnFind.Text" xml:space="preserve">
  <value>Tìm kiếm</value>
</data>
<data name="frmHisMachine.xtraTabPage1.Text" xml:space="preserve">
  <value>Thông tin máy</value>
</data>
```

```xml
<!-- Lang.en.resx — PHẢI ĐẦY ĐỦ tương ứng -->
<data name="frmHisMachine.lciMachineCode.Text" xml:space="preserve">
  <value>Machine Code</value>
</data>
<data name="frmHisMachine.lciMachineName.Text" xml:space="preserve">
  <value>Machine Name</value>
</data>
<data name="frmHisMachine.btnSave.Text" xml:space="preserve">
  <value>Save (Ctrl+S)</value>
</data>
<data name="frmHisMachine.btnNew.Text" xml:space="preserve">
  <value>New (Ctrl+N)</value>
</data>
<data name="frmHisMachine.btnFind.Text" xml:space="preserve">
  <value>Search</value>
</data>
<data name="frmHisMachine.xtraTabPage1.Text" xml:space="preserve">
  <value>Machine Info</value>
</data>
```

### 13d. Message.Lang.resx — Khai báo TẤT CẢ câu thông báo riêng plugin

```xml
<!-- Message.Lang.vi.resx -->
<data name="BanCoMuonHuyDonThuocKhong" xml:space="preserve">
  <value>Bạn có muốn hủy đơn thuốc này không?</value>
</data>
<data name="SoLuongVuotGioiHan" xml:space="preserve">
  <value>Số lượng vượt quá giới hạn cho phép</value>
</data>
```

```xml
<!-- Message.Lang.en.resx — PHẢI ĐẦY ĐỦ tương ứng -->
<data name="BanCoMuonHuyDonThuocKhong" xml:space="preserve">
  <value>Do you want to cancel this prescription?</value>
</data>
<data name="SoLuongVuotGioiHan" xml:space="preserve">
  <value>Quantity exceeds the allowed limit</value>
</data>
```

### 13e. ResourceMessage.cs — Accessor cho Message (BẮT BUỘC)

```csharp
using System;
using System.Resources;

namespace HIS.Desktop.Plugins.{PluginName}.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage =
            new System.Resources.ResourceManager(
                "HIS.Desktop.Plugins.{PluginName}.Resources.Message.Lang",
                System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Bạn có muốn hủy đơn thuốc này không?</summary>
        internal static string BanCoMuonHuyDonThuocKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "BanCoMuonHuyDonThuocKhong",
                        languageMessage,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        /// <summary>Số lượng vượt quá giới hạn cho phép</summary>
        internal static string SoLuongVuotGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "SoLuongVuotGioiHan",
                        languageMessage,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
```

### 13f. Hàm SetCaptionByLanguageKey (BẮT BUỘC trong mọi Form/UC)

```csharp
/// <summary>
/// BẮT BUỘC gọi trong Load event — set tên hiển thị theo ngôn ngữ
/// Gọi SAU InitComboData, TRƯỚC ValidateForm
/// </summary>
private void SetCaptionByLanguageKey()
{
    try
    {
        // 1. Khởi tạo ResourceManager
        Resources.ResourceLanguageManager.LanguageResource =
            new ResourceManager(
                "HIS.Desktop.Plugins.{PluginName}.Resources.Lang",
                typeof(frm{Name}).Assembly);

        // 2. Set caption cho TẤT CẢ LayoutControlItems
        this.lciMachineCode.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.lciMachineCode.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        this.lciMachineName.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.lciMachineName.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        // 3. Set caption cho Buttons
        this.btnSave.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.btnSave.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        // 4. Set caption cho Tabs
        this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.xtraTabPage1.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        // 5. Set caption cho GridColumns (nếu cần)
        // this.gcPatientName.Caption = Inventec.Common.Resource.Get.Value(...);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

### 13g. Sử dụng thông báo — ĐỦ 2 nguồn

```csharp
// THÔNG BÁO CHUNG (có sẵn 76 enum) → MessageUtil
XtraMessageBox.Show(
    MessageUtil.GetMessage(Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

// THÔNG BÁO RIÊNG PLUGIN → ResourceMessage
XtraMessageBox.Show(
    Resources.ResourceMessage.BanCoMuonHuyDonThuocKhong,
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
```

### 13h. Checklist đa ngôn ngữ

- [ ] Có Resources/ folder với Lang.vi.resx + Lang.en.resx
- [ ] Có Message.Lang.vi.resx + Message.Lang.en.resx
- [ ] Có ResourceLanguageManager.cs + ResourceMessage.cs
- [ ] TẤT CẢ LayoutControlItem.Text khai báo trong Lang.resx
- [ ] TẤT CẢ Button.Text khai báo trong Lang.resx
- [ ] TẤT CẢ TabPage.Text khai báo trong Lang.resx
- [ ] TẤT CẢ câu thông báo riêng plugin trong Message.Lang.resx
- [ ] Lang.en.resx có ĐẦY ĐỦ số entries bằng Lang.vi.resx
- [ ] Message.Lang.en.resx có ĐẦY ĐỦ số entries bằng Message.Lang.vi.resx
- [ ] SetCaptionByLanguageKey() gọi trong Load event
- [ ] KHÔNG còn hardcode tiếng Việt trong code

## 14. ControlState — Lưu Trạng Thái Giữa Phiên (BẮT BUỘC cho checkbox/toggle)

**Mọi checkbox, toggle, radio có tính năng "nhớ lần dùng trước"** (VD: checkbox In, Ký, In file đã ký) PHẢI dùng ControlStateWorker.

### 14a. Khai báo fields (BẮT BUỘC)

```csharp
#region ControlState
/// <summary>Worker đọc/ghi trạng thái local</summary>
HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;

/// <summary>Danh sách trạng thái hiện tại</summary>
List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

/// <summary>
/// Flag CHẶN CheckedChanged fire khi đang load.
/// = true khi đang InitControlState → set Checked từ cache
/// = false sau khi load xong → cho phép lưu khi user click
/// </summary>
bool isNotLoadWhileChangeControlStateInFirst = false;

/// <summary>Module ID — key phân biệt plugin</summary>
string moduleLink = "HIS.Desktop.Plugins.{PluginName}";
#endregion
```

### 14b. InitControlState — Đọc trạng thái đã lưu (gọi trong Load)

```csharp
/// <summary>
/// Đọc trạng thái checkbox từ SQLite local.
/// BẮT BUỘC gọi trong Load event, SAU SetDefaultValue, TRƯỚC FillDataToGrid.
/// </summary>
private void InitControlState()
{
    try
    {
        // BẬT flag — chặn CheckedChanged fire khi set giá trị từ cache
        isNotLoadWhileChangeControlStateInFirst = true;

        controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
        currentControlStateRDO = controlStateWorker.GetData(moduleLink);

        if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
        {
            foreach (var item in currentControlStateRDO)
            {
                // Mỗi KEY = tên control, VALUE = "1" (checked) hoặc "" (unchecked)
                if (item.KEY == chkPrint.Name)
                    chkPrint.Checked = item.VALUE == "1";

                if (item.KEY == chkSign.Name)
                    chkSign.Checked = item.VALUE == "1";

                if (item.KEY == chkPrintSigned.Name)
                    chkPrintSigned.Checked = item.VALUE == "1";

                // Thêm cho mỗi checkbox cần nhớ trạng thái...
            }
        }

        // TẮT flag — từ giờ CheckedChanged sẽ lưu khi user click
        isNotLoadWhileChangeControlStateInFirst = false;
    }
    catch (Exception ex)
    {
        isNotLoadWhileChangeControlStateInFirst = false; // PHẢI tắt flag trong catch
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

### 14c. CheckedChanged — Lưu trạng thái khi user thay đổi

```csharp
/// <summary>
/// Lưu trạng thái mỗi khi user click checkbox.
/// PHẢI check flag isNotLoadWhileChangeControlStateInFirst ĐẦU TIÊN.
/// </summary>
private void chkPrint_CheckedChanged(object sender, EventArgs e)
{
    // CHẶN: Không lưu khi đang load (InitControlState đang set Checked)
    if (isNotLoadWhileChangeControlStateInFirst) return;

    try
    {
        // Tìm item đã có
        var item = currentControlStateRDO?.FirstOrDefault(
            o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink);

        if (item != null)
        {
            // Cập nhật giá trị
            item.VALUE = chkPrint.Checked ? "1" : "";
        }
        else
        {
            // Tạo mới
            if (currentControlStateRDO == null)
                currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

            currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO
            {
                KEY = chkPrint.Name,
                MODULE_LINK = moduleLink,
                VALUE = chkPrint.Checked ? "1" : ""
            });
        }

        // Ghi xuống SQLite
        controlStateWorker.SetData(currentControlStateRDO);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}

// COPY pattern trên cho MỖI checkbox cần nhớ:
// chkSign_CheckedChanged, chkPrintSigned_CheckedChanged, v.v.
```

### 14d. Lưu giá trị phức tạp (JSON)

Ngoài "1"/"" cho checkbox, có thể lưu JSON cho data phức tạp:

```csharp
// Lưu object dạng JSON (VD: cài đặt ký số)
if (item.KEY == chkSign.Name)
{
    SettingSignADO settingSign = JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
    chkSign.Checked = settingSign != null && !string.IsNullOrEmpty(settingSign.SerialNumber);
}

// Khi lưu
item.VALUE = JsonConvert.SerializeObject(new SettingSignADO
{
    SerialNumber = selectedSerial,
    SignType = signType
});
controlStateWorker.SetData(currentControlStateRDO);
```

### 14e. Lưu trạng thái theo SESSION (không persist qua phiên)

Dùng khi chỉ cần nhớ trong 1 phiên làm việc (đóng app mất):

```csharp
// Lưu theo session (RAM only, không SQLite)
controlStateWorker.SetDataBySession(currentControlStateRDO);

// Đọc theo session
currentControlStateRDO = controlStateWorker.GetDataBySession();
```

### 14f. Vị trí trong Load order

```
1. InitComboData()
2. SetCaptionByLanguageKey()    ← Section 13
3. ValidateForm()
4. InitTabIndex()
5. SetDefaultValue()
6. InitControlState()            ← Section 14 — SAU SetDefaultValue
7. FillDataToGrid()
```

InitControlState() PHẢI sau SetDefaultValue() — vì SetDefaultValue reset checkbox về mặc định, InitControlState ghi đè lại từ cache.

### 14g. Checklist ControlState

- [ ] Có 4 fields: controlStateWorker, currentControlStateRDO, isNotLoadWhileChangeControlStateInFirst, moduleLink
- [ ] moduleLink = đúng Plugin ID (trùng với [ExtensionOf])
- [ ] InitControlState() gọi trong Load, SAU SetDefaultValue
- [ ] Flag bật TRUE đầu InitControlState, tắt FALSE cuối (và trong catch)
- [ ] Mỗi checkbox cần nhớ có CheckedChanged handler
- [ ] CheckedChanged handler check flag ĐẦU TIÊN: `if (isNotLoadWhileChangeControlStateInFirst) return;`
- [ ] Gọi controlStateWorker.SetData() mỗi khi user thay đổi
- [ ] KEY = control.Name — KHÔNG hardcode string
