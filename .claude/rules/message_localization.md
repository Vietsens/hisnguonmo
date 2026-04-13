---
description: Đa ngôn ngữ — LibraryMessage (76 Message.Enum), MessageUtil API, ResourceMessage pattern, FontendMessage cache. Áp dụng khi hiện thông báo, validation, dialog
paths:
  - "HIS/Plugins/**"
  - "HIS/HIS.Desktop.LibraryMessage/**"
---

# Message & Localization — Đa Ngôn Ngữ

3 ngôn ngữ: Vietnamese (vi), English (en), Myanmar (my).
Source: `HIS/HIS.Desktop.LibraryMessage/`

---

## 1. MessageUtil API — Dùng Cho Thông Báo Chung

### Lấy message text

```csharp
// Lấy theo ngôn ngữ hiện tại
string msg = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
// Vi: "Trường dữ liệu bắt buộc" | En: "Required field"

// Message có tham số
string msg = MessageUtil.GetMessage(
    Message.Enum.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ,
    new string[] { "500000", "1000000" });
// Vi: "Số tiền đã kê 500000 cho BHYT đã vượt mức giới hạn 1000000"
```

### Thêm message vào CommonParam (sau API call)

```csharp
CommonParam param = new CommonParam();
// ... API call ...

// Thêm kết quả thành công/thất bại vào đầu param.Messages
MessageUtil.SetResultParam(param, success);
// success=true → "Xử lý thành công" | false → "Xử lý thất bại"

// Thêm message cụ thể
MessageUtil.SetMessage(param, Message.Enum.TruongDuLieuBatBuoc);
MessageUtil.SetMessage(param, Message.Enum.SomeEnum, "extra info");
MessageUtil.SetMessage(param, Message.Enum.SomeEnum, new string[] { "p1", "p2" });

// Thêm message ưu tiên (insert đầu danh sách)
MessageUtil.SetParamFirstPostion(param, Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai);

// Hiển thị
MessageManager.Show(this, param, success);
```

### Lấy tất cả messages + mã sự cố

```csharp
// Lấy chuỗi messages từ param (bao gồm bug codes)
string allMessages = MessageUtil.GetMessageAlert(param);
// Output: "Message1\nMessage2\r\nMã sự cố: BUGCODE1,BUGCODE2"
```

---

## 2. Message.Enum — 76 Giá Trị (Phân Nhóm)

### Kết quả xử lý

| Enum | Nghĩa |
|------|-------|
| `HeThongTBKQXLYCCuaFrontendThanhCong` | Xử lý thành công |
| `HeThongTBKQXLYCCuaFrontendThatBai` | Xử lý thất bại |
| `HeThongTBXuatHienExceptionChuaKiemDuocSoat` | Exception chưa kiểm soát |
| `HeThongThongBaoKetQuaTraVeCuaBackendKhongHopLe` | Backend trả về không hợp lệ |

### Tiêu đề dialog

| Enum | Nghĩa |
|------|-------|
| `TieuDeCuaSoThongBaoLaThongBao` | Thông báo |
| `TieuDeCuaSoThongBaoLaCanhBao` | Cảnh báo |
| `TieuDeCuaSoThongBaoLaLoi` | Lỗi |

### Xác nhận (Confirm)

| Enum | Nghĩa |
|------|-------|
| `HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong` | Bạn có muốn hủy? |
| `HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong` | Bạn có muốn khóa? |
| `HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong` | Bạn có muốn xóa? |
| `HeThongTBCuaSoThongBaoBanCoMuonDuyetKhoaDuLieuKhong` | Bạn có muốn duyệt? |
| `HeThongTBCuaSoThongBaoBanCoMuonBoDuyetKhoaDuLieuKhong` | Bạn có muốn bỏ duyệt? |
| `HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong` | Bạn có muốn bỏ khóa? |

### Validation

| Enum | Nghĩa |
|------|-------|
| `TruongDuLieuBatBuoc` | Trường bắt buộc |
| `ThieuTruongDuLieuBatBuoc` | Thiếu trường bắt buộc |
| `ThieuTruongDuLieuCanThiet` | Thiếu trường cần thiết |
| `HeThongTBTruongDuLieuBatBuocPhaiNhap` | Trường bắt buộc phải nhập |
| `HeThongTBDuLieuNhapVaoKhongHopLe` | Dữ liệu không hợp lệ |
| `NguoiDungNhapDuLieuKhongHopLe` | Dữ liệu nhập không hợp lệ |
| `TruongDuLieuKhongNhanGiaTriAm` | Không nhận giá trị âm |
| `SoLuongKhongDuocBeHonKhong` | Số lượng >= 0 |
| `Common__SoLuongPhaiLonHonKhong` | Số lượng > 0 |
| `DungLuongFileDinhKemQuaLon` | File quá lớn |

### Đăng nhập / Phân quyền

| Enum | Nghĩa |
|------|-------|
| `TaiKhoanKhongCoQuyenThucHienChucNang` | Không có quyền |
| `NguoiDungChuaNhapTaiKhoanDeDangNhap` | Chưa nhập tài khoản |
| `NguoiDungChuaNhapMatKhauDeDangNhap` | Chưa nhập mật khẩu |
| `NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac` | Xác nhận mật khẩu sai |
| `NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap` | Sai tài khoản/mật khẩu |

### Kết nối

| Enum | Nghĩa |
|------|-------|
| `PhanMemKhongKetNoiDuocToiMayChuHeThong` | Không kết nối server |
| `HeThongTBKetNoiDenMayChuTot` | Kết nối tốt |
| `HeThongTBKetNoiDenMayChuKhongTot` | Mất kết nối |
| `HeThongTBKetNoiDenMayChuKhongOnDinh` | Kết nối không ổn định |
| `HeThongTBKetNoiDenMayChuThatBai` | Kết nối thất bại |

### Ngày tháng

| Enum | Nghĩa |
|------|-------|
| `NguoiDungNhapNamSinhKhongHopLe` | Năm sinh không hợp lệ |
| `NguoiDungNhapThangSinhKhongHopLe` | Tháng sinh không hợp lệ |
| `NguoiDungNhapNgaySinhKhongHopLe` | Ngày sinh không hợp lệ |
| `NguoiDungNhapThangSinhLonHonHienTai` | Tháng sinh > hiện tại |
| `NguoiDungNhapNgaySinhLonHonHienTai` | Ngày sinh > hiện tại |
| `NguoiDungNhapNamKhongHopLe` | Năm không hợp lệ |
| `NguoiDungNhapThangKhongHopLe` | Tháng không hợp lệ |
| `NguoiDungNhapNgayKhongHopLe` | Ngày không hợp lệ |
| `NguoiDungNhapNgayPhaiNhoHonNgayHienTai` | Ngày < hiện tại |
| `NguoiDungNhapThangBatDauCoHieuLucTheBHYTKhongHopLe` | Tháng BHYT bắt đầu sai |
| `NguoiDungNhapNgayBatDauCoHieuLucTheBHYTKhongHopLe` | Ngày BHYT bắt đầu sai |
| `NguoiDungNhapNamHetHieuLucTheBHYTKhongHopLe` | Năm BHYT hết hạn sai |
| `NguoiDungNhapThangHetHieuLucTheBHYTKhongHopLe` | Tháng BHYT hết hạn sai |
| `NguoiDungNhapNgayHetHieuLucTheBHYTKhongHopLe` | Ngày BHYT hết hạn sai |

### Nghiệp vụ

| Enum | Nghĩa |
|------|-------|
| `NguoiDungNhapTruongHopBHYTKhongChiTraChiPhiVuiLongXemLaiCacDuLieuLienQuan` | BHYT không chi trả |
| `NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong` | Chưa gán quyền phòng |
| `NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh` | Config phòng sai |
| `NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec` | Chưa config phòng |
| `DuLieuDangKhoa` | Dữ liệu đang khóa |
| `DuLieuDangMo` | Dữ liệu đang mở |
| `TruongDuLieuLaVAT` | Trường là VAT |

### Hệ thống / UI

| Enum | Nghĩa |
|------|-------|
| `HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai` | Hết phiên |
| `HeThongTBBanQuyenKhongHopLe` | Bản quyền không hợp lệ |
| `ChucNangDangPhatTrienVuiLongThuLaiSau` | Đang phát triển |
| `HeThongTBKhongTimThayPluginsCuaChucNangNay` | Không tìm thấy plugin |
| `HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa` | Plugin không có với mã |
| `HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa` | Không khởi tạo plugin |
| `Plugins_HisDesktop__KhongKhoiTaoDuocModule` | Không khởi tạo module |
| `TieuDeThongTinHienThiPhanTrang` | Info phân trang |
| `HeThongThongBaoMoTaChoWaitDialogForm` | Please wait... |
| `HeThongThongBaoTieuDeChoWaitDialogForm` | Processing |
| `HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting` | Please wait |
| `HeThongThongBaoMoTaChoUpdatingDialogForm` | Updating data |
| `ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram` | Loading config |
| `HeThongTruyCapVaoPhanMemThanhCong` | Truy cập thành công |
| `ThongBaoDuLieuTrong` | Dữ liệu trống |
| `ImportExcel__DuLieuDocTuFileExcelRong` | Excel file trống |

### BHYT

| Enum | Nghĩa |
|------|-------|
| `AlertHospitalFeeNotBHYT` | Cảnh báo phí không BHYT |
| `AlertWarningHeinFee` | Cảnh báo phí BHYT |
| `SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ` | Vượt trần BHYT |

---

## 3. Code Mẫu Sử Dụng

### Confirm dialog

```csharp
if (XtraMessageBox.Show(
    MessageUtil.GetMessage(Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question) == DialogResult.Yes)
{
    // Thực hiện xóa
}
```

### Validation error

```csharp
dxErrorProvider1.SetError(txtField,
    MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc),
    ErrorType.Warning);
```

### Sau API call

```csharp
CommonParam param = new CommonParam();
var result = new BackendAdapter(param).Post<HIS_ENTITY>(uri, consumer, dto, param);
bool success = (result != null);
MessageUtil.SetResultParam(param, success);
MessageManager.Show(this, param, success);
```

### GetMessageAlert — Lấy tất cả lỗi + mã sự cố

```csharp
string allErrors = MessageUtil.GetMessageAlert(param);
// "Xử lý thất bại\nTrường dữ liệu bắt buộc\r\nMã sự cố: BUG001"
```

---

## 4. FontendMessage — Cache Thread-Safe

```csharp
// Cache structure: Language → (Enum → Message)
Dictionary<Message.LanguageEnum, Dictionary<Message.Enum, Message>> dicMultiLanguage;

// Thread-safe: lock khi tạo message mới
lock (thisLock) { result = new Message(languageEnum, enumBC); }

// Sau lần đầu: O(1) lookup từ cache
// 3 ngôn ngữ: LanguageEnum.Vietnamese, .English, .Mianmar
```

---

## 5. ResourceMessage — Thông Báo Riêng Plugin

Khi Message.Enum KHÔNG có thông báo cần dùng → tạo ResourceMessage trong plugin:

### Tạo files

```
Resources/
├── Message.Lang.vi.resx       ← Key-value tiếng Việt
├── Message.Lang.en.resx       ← Key-value English
├── Message.Lang.my.resx       ← Key-value Myanmar (nếu cần)
└── ResourceMessage.cs         ← Accessor class
```

### ResourceMessage.cs

```csharp
class ResourceMessage
{
    static System.Resources.ResourceManager languageMessage =
        new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.{PluginName}.Resources.Message.Lang",
            System.Reflection.Assembly.GetExecutingAssembly());

    /// <summary>Bạn có muốn xóa đơn thuốc này không?</summary>
    internal static string BanCoMuonXoaDonThuocKhong
    {
        get
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    "BanCoMuonXoaDonThuocKhong",
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
```

### Sử dụng

```csharp
XtraMessageBox.Show(
    Resources.ResourceMessage.BanCoMuonXoaDonThuocKhong,
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
```

---

## 6. SetCaptionByLanguageKey — Label UI

```csharp
// Trong Load event — set caption đa ngôn ngữ cho controls
private void SetCaptionByLanguageKey()
{
    try
    {
        Resources.ResourceLanguageManager.LanguageResource =
            new ResourceManager(
                "HIS.Desktop.Plugins.{PluginName}.Resources.Lang",
                typeof(frm{Name}).Assembly);

        this.lciPatientName.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.lciPatientName.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            LanguageManager.GetCulture());

        this.btnSave.Text = Inventec.Common.Resource.Get.Value(
            "frm{Name}.btnSave.Text",
            Resources.ResourceLanguageManager.LanguageResource,
            LanguageManager.GetCulture());
    }
    catch (Exception ex)
    {
        Inventec.Common.Logging.LogSystem.Warn(ex);
    }
}
```

### Files cần tạo

```
Resources/
├── Lang.vi.resx                   ← frm{Name}.lciPatientName.Text = "Họ tên BN"
├── Lang.en.resx                   ← frm{Name}.lciPatientName.Text = "Patient Name"
├── Lang.my.resx                   ← (nếu cần Myanmar)
└── ResourceLanguageManager.cs     ← Holds ResourceManager
```

---

## 7. Quy Tắc

| Quy tắc | Chi tiết |
|---------|----------|
| **Thông báo chung** | `MessageUtil.GetMessage(Message.Enum.XXX)` — KHÔNG hardcode tiếng Việt |
| **Thông báo riêng plugin** | `ResourceMessage.PropertyName` — tạo trong Resources/ |
| **Tiêu đề dialog** | `MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao)` |
| **Confirm** | Dùng Message.Enum.HeThongTB*BanCoMuon* |
| **Validation** | `Message.Enum.TruongDuLieuBatBuoc` hoặc `ThieuTruongDuLieuBatBuoc` |
| **Sau API** | `MessageUtil.SetResultParam(param, success)` → `MessageManager.Show()` |
| **Label UI** | `SetCaptionByLanguageKey()` + Lang.vi/en/my.resx |
| **KHÔNG hardcode** | KHÔNG `"Bạn có muốn xóa?"` trực tiếp — dùng Enum hoặc ResourceMessage |
| **Try-catch** | ResourceMessage getter PHẢI có try-catch, return "" khi lỗi |
| **3 ngôn ngữ** | Mỗi .resx có 3 files: .vi, .en, .my (Myanmar tùy chọn) |
