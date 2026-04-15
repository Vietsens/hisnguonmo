---
name: migrate-ui
description: Upgrade form/UC cũ lên chuẩn ui_rules mới — SetIcon, LayoutControl, Maroon required, EmptySpace, Load order, ControlState
user-invocable: true
argument-hint: <file path của form/UC cần upgrade>
---

# Migrate UI Lên Chuẩn Mới

Target: $ARGUMENTS

## Bước 1: Đọc file hiện tại và đánh giá

Đọc form/UC .cs file (KHÔNG đọc .Designer.cs) và kiểm tra:

### Checklist hiện trạng
- [ ] Kế thừa FormBase/UserControlBase?
- [ ] Có SetIcon() trong constructor?
- [ ] Load order chuẩn? (Config → Combo → Language → Validate → TabIndex → Default → ControlState → Grid)
- [ ] Có ControlState ĐẦY ĐỦ (4 fields, InitControlState, flag, CheckedChanged handlers)?
- [ ] Có ValidateForm() với DXValidationProvider?
- [ ] Có Resources/ với Lang.vi.resx + Lang.en.resx + Message.Lang.vi/en.resx?
- [ ] Có SetCaptionByLanguageKey() trong Load event?
- [ ] Có ResourceLanguageManager.cs + ResourceMessage.cs?
- [ ] TẤT CẢ text UI khai báo trong Lang.resx (KHÔNG hardcode tiếng Việt)?
- [ ] Lang.en.resx ĐẦY ĐỦ số entries bằng Lang.vi.resx?
- [ ] SaveProcess theo pattern: Validate → WaitingManager → BackendAdapter → MessageManager → SessionManager?
- [ ] DeleteProcess có confirm dialog?
- [ ] Mọi method có try-catch + LogSystem?
- [ ] GridControl có BeginUpdate/EndUpdate?

### Đọc .Designer.cs để kiểm tra:
- [ ] LayoutControl có EnableIndentsWithoutBorders = True?
- [ ] Required fields có AppearanceItemCaption.ForeColor = Maroon?
- [ ] Vùng trống có EmptySpaceItem?
- [ ] LayoutControlItem TextAlignMode = CustomSize?
- [ ] GridView: ShowGroupPanel = false, ShowIndicator = false, RowAutoHeight = true?
- [ ] Grid load 1 bảng/view có ĐỦ 4 cột audit cuối (CREATE_TIME_STR, CREATOR, MODIFY_TIME_STR, MODIFIER)?

## Bước 2: Liệt kê thay đổi cần làm

Phân loại:
- **CRITICAL**: Thiếu FormBase kế thừa, thiếu try-catch, catch rỗng
- **HIGH**: Thiếu SetIcon, sai Load order, thiếu WaitingManager.Hide trong catch
- **MEDIUM**: Thiếu ControlState, thiếu ValidateForm, thiếu EmptySpaceItem
- **LOW**: Caption chưa tiếng Việt có dấu, thiếu Maroon required

## Bước 3: Áp dụng thay đổi .cs file

### 3a. Thêm SetIcon (nếu thiếu)
```csharp
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
```

### 3b. Sửa Load order (nếu sai)
Đảm bảo thứ tự: InitCombo → SetCaptionByLanguageKey → ValidateForm → InitTabIndex → SetDefaultValue → InitControlState → FillDataToGrid

### 3c. Thêm ControlState (nếu thiếu)
```csharp
HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
bool isNotLoadWhileChangeControlStateInFirst = false;
```

### 3d. Sửa SaveProcess (nếu sai pattern)
Đảm bảo: Validate → WaitingManager.Show → BackendAdapter → WaitingManager.Hide → MessageManager.Show → SessionManager.ProcessTokenLost
WaitingManager.Hide PHẢI trong catch

### 3e. Thêm try-catch (nếu thiếu)
Mọi method chưa có try-catch → bao bằng try-catch + LogSystem.Warn(ex)

### 3f. Thêm Keyboard (cho UC)
Nếu là UC và chưa có KeyboardWorker.cs → tạo file theo mẫu scaffold-uc

### 3g. Thêm Language (nếu thiếu Resources/)
Gọi skill `/setup-localization` để tạo đầy đủ:
- Resources/Lang.vi.resx + Lang.en.resx
- Resources/Message.Lang.vi.resx + Message.Lang.en.resx
- ResourceLanguageManager.cs + ResourceMessage.cs
- SetCaptionByLanguageKey() trong Load
- Thay tất cả hardcode tiếng Việt → MessageUtil hoặc ResourceMessage

### 3h. Thêm ControlState đầy đủ (nếu thiếu hoặc thiếu fields)
```csharp
// 4 fields BẮT BUỘC
HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
bool isNotLoadWhileChangeControlStateInFirst = false;
string moduleLink = "HIS.Desktop.Plugins.{PluginName}";

// InitControlState() — flag TRUE đầu, FALSE cuối + trong catch
// CheckedChanged handler — check flag ĐẦU TIÊN, SetData()
```

### 3i. Thêm 4 cột audit cho Grid (nếu grid load 1 bảng/view)
Thêm vào Designer.cs — 4 cột cuối:
- gcCreateTime (Unbound, "Thời gian tạo", TimeNumberToTimeString)
- gcCreator (Bound, "Người tạo")
- gcModifyTime (Unbound, "Thời gian sửa", TimeNumberToTimeString)
- gcModifier (Bound, "Người sửa")
AllowEdit = false cho cả 4 cột.
Thêm CustomUnboundColumnData cho CREATE_TIME_STR và MODIFY_TIME_STR.

## Bước 4: Sửa .Designer.cs (CHỈ những thay đổi an toàn)

CHÚ Ý: Chỉ sửa properties, KHÔNG sửa layout structure.

### 4a. LayoutControl
```csharp
this.layoutControlGroup1.EnableIndentsWithoutBorders =
    DevExpress.Utils.DefaultBoolean.True;
```

### 4b. Required fields Maroon
```csharp
this.lciFieldName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
this.lciFieldName.AppearanceItemCaption.Options.UseForeColor = true;
```

### 4c. GridView options
```csharp
this.gridView.OptionsView.ShowGroupPanel = false;
this.gridView.OptionsView.ShowIndicator = false;
this.gridView.OptionsView.RowAutoHeight = true;
```

## Bước 5: Verify

- [ ] Build thành công
- [ ] Form/UC hiển thị đúng
- [ ] Required fields hiện Maroon
- [ ] Save/Delete hoạt động
- [ ] ControlState lưu/đọc đúng
- [ ] Phím tắt hoạt động (Form: BarManager, UC: KeyboardWorker)
- [ ] WaitingManager Show/Hide đúng cặp
- [ ] Language: Resources/ đầy đủ, SetCaptionByLanguageKey, KHÔNG hardcode tiếng Việt
- [ ] ControlState: 4 fields, flag, CheckedChanged handlers
- [ ] Grid: 4 cột audit cuối (nếu load 1 bảng/view)

## Output

```
FILE: {path}
THAY ĐỔI: {số thay đổi}
  [CRITICAL] {mô tả} — DONE/SKIP
  [HIGH] {mô tả} — DONE/SKIP
  [MEDIUM] {mô tả} — DONE/SKIP
  [LOW] {mô tả} — DONE/SKIP
```
