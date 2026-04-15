---
name: setup-localization
description: Sinh Resources đa ngôn ngữ cho plugin — tạo Lang.resx, Message.resx, ResourceLanguageManager, ResourceMessage, SetCaptionByLanguageKey
user-invocable: true
argument-hint: <plugin folder path VD: HIS/Plugins/HIS.Desktop.Plugins.MyPlugin>
---

# Setup Localization — Sinh Resources Đa Ngôn Ngữ

Target: $ARGUMENTS

## Bước 1: Kiểm tra hiện trạng

Đọc plugin folder, kiểm tra:
- Có folder `Resources/` chưa?
- Có `Lang.vi.resx`, `Lang.en.resx` chưa?
- Có `Message.Lang.vi.resx`, `Message.Lang.en.resx` chưa?
- Có `ResourceLanguageManager.cs` chưa?
- Có `ResourceMessage.cs` chưa?
- Form/UC có `SetCaptionByLanguageKey()` chưa?

## Bước 2: Tạo cấu trúc Resources/

```
{PluginName}/
└── Resources/
    ├── Lang.vi.resx                   ← UI labels tiếng Việt
    ├── Lang.en.resx                   ← UI labels English
    ├── Lang.my.resx                   ← UI labels Myanmar (tùy chọn)
    ├── Message.Lang.vi.resx           ← Thông báo tiếng Việt
    ├── Message.Lang.en.resx           ← Thông báo English
    ├── Message.Lang.my.resx           ← Thông báo Myanmar (tùy chọn)
    ├── ResourceLanguageManager.cs     ← Holds ResourceManager
    └── ResourceMessage.cs             ← Typed message accessor
```

## Bước 3: Tạo ResourceLanguageManager.cs

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

## Bước 4: Tạo ResourceMessage.cs

Đọc form/UC hiện tại, liệt kê TẤT CẢ thông báo đang hardcode (XtraMessageBox.Show, dxErrorProvider.SetError, MessageManager...).

Với mỗi thông báo:

### 4a. Kiểm tra Message.Enum có sẵn

Tìm trong 76 Message.Enum:
- Xác nhận xóa → `Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong`
- Trường bắt buộc → `Message.Enum.TruongDuLieuBatBuoc`
- Xử lý thành công → `Message.Enum.HeThongTBKQXLYCCuaFrontendThanhCong`
- Tiêu đề thông báo → `Message.Enum.TieuDeCuaSoThongBaoLaThongBao`

Nếu CÓ → dùng `MessageUtil.GetMessage(Message.Enum.XXX)` — KHÔNG tạo ResourceMessage.

### 4b. Tạo ResourceMessage cho thông báo RIÊNG plugin

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
        internal static string BanCoMuonHuyDonThuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "BanCoMuonHuyDonThuoc",
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

        /// <summary>Số lượng thuốc vượt quá giới hạn cho phép</summary>
        internal static string SoLuongThuocVuotGioiHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value(
                        "SoLuongThuocVuotGioiHan",
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

### 4c. Thêm entries vào .resx files

**Message.Lang.vi.resx:**
```xml
<data name="BanCoMuonHuyDonThuoc" xml:space="preserve">
  <value>Bạn có muốn hủy đơn thuốc này không?</value>
</data>
<data name="SoLuongThuocVuotGioiHan" xml:space="preserve">
  <value>Số lượng thuốc vượt quá giới hạn cho phép</value>
</data>
```

**Message.Lang.en.resx:**
```xml
<data name="BanCoMuonHuyDonThuoc" xml:space="preserve">
  <value>Do you want to cancel this prescription?</value>
</data>
<data name="SoLuongThuocVuotGioiHan" xml:space="preserve">
  <value>Medicine quantity exceeds the allowed limit</value>
</data>
```

## Bước 5: Tạo SetCaptionByLanguageKey()

### 5a. Liệt kê tất cả controls cần dịch

Đọc form Designer.cs, liệt kê tất cả controls có Text/Caption:
- LayoutControlItem → `.Text`
- SimpleButton → `.Text`
- BarButtonItem → `.Caption`
- XtraTabPage → `.Text`
- GroupControl → `.Text`

### 5b. Thêm entries vào Lang.resx

**Lang.vi.resx:**
```xml
<data name="frm{Name}.lciPatientName.Text" xml:space="preserve">
  <value>Họ tên BN</value>
</data>
<data name="frm{Name}.btnSave.Text" xml:space="preserve">
  <value>Lưu (Ctrl+S)</value>
</data>
<data name="frm{Name}.btnNew.Text" xml:space="preserve">
  <value>Mới (Ctrl+N)</value>
</data>
```

**Lang.en.resx:**
```xml
<data name="frm{Name}.lciPatientName.Text" xml:space="preserve">
  <value>Patient Name</value>
</data>
<data name="frm{Name}.btnSave.Text" xml:space="preserve">
  <value>Save (Ctrl+S)</value>
</data>
<data name="frm{Name}.btnNew.Text" xml:space="preserve">
  <value>New (Ctrl+N)</value>
</data>
```

### 5c. Sinh method SetCaptionByLanguageKey

```csharp
private void SetCaptionByLanguageKey()
{
    try
    {
        Resources.ResourceLanguageManager.LanguageResource =
            new ResourceManager(
                "HIS.Desktop.Plugins.{PluginName}.Resources.Lang",
                typeof(frm{Name}).Assembly);

        // LayoutControlItems
        this.lciPatientName.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.lciPatientName.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        // Buttons
        this.btnSave.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.btnSave.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        this.btnNew.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.btnNew.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

        // Tabs
        // this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value(...);

        // Groups
        // this.grpPatientInfo.Text = Inventec.Common.Resource.Get.Value(...);
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

## Bước 6: Thay thế hardcode strings trong code

### Trước (SAI):
```csharp
XtraMessageBox.Show("Bạn có muốn xóa?", "Thông báo", ...);
dxErrorProvider.SetError(txtField, "Trường bắt buộc", ...);
```

### Sau (ĐÚNG):
```csharp
// Thông báo CHUNG → dùng MessageUtil
XtraMessageBox.Show(
    MessageUtil.GetMessage(Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao), ...);

dxErrorProvider.SetError(txtField,
    MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc), ...);

// Thông báo RIÊNG plugin → dùng ResourceMessage
XtraMessageBox.Show(
    Resources.ResourceMessage.BanCoMuonHuyDonThuoc,
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao), ...);
```

## Bước 7: Gọi SetCaptionByLanguageKey trong Load

Đảm bảo gọi trong Load event, SAU InitComboData:

```csharp
private void frm{Name}_Load(object sender, EventArgs e)
{
    try
    {
        InitComboData();              // 1
        SetCaptionByLanguageKey();    // 2 — SAU combo, TRƯỚC validation
        ValidateForm();               // 3
        // ...
    }
}
```

## Bước 8: Verify

- [ ] Resources/ folder có đủ files (Lang.vi/en, Message.Lang.vi/en, Manager, Message)
- [ ] ResourceLanguageManager.cs đúng namespace
- [ ] ResourceMessage.cs mỗi property có try-catch + return ""
- [ ] .resx keys KHỚP với code Get.Value()
- [ ] Key format: "frm{Name}.{controlName}.Text"
- [ ] SetCaptionByLanguageKey() gọi trong Load event
- [ ] KHÔNG còn hardcode tiếng Việt trong XtraMessageBox, dxErrorProvider
- [ ] MessageUtil.GetMessage dùng cho thông báo có trong 76 Enum
- [ ] ResourceMessage dùng cho thông báo RIÊNG plugin
- [ ] Build thành công + UI hiển thị đúng
