---
description: Tìm Message.Enum phù hợp cho thông báo — trả về enum, MessageUtil code mẫu
argument-hint: <nội dung thông báo VD: thành công, thất bại, bắt buộc, xóa dữ liệu, cảnh báo>
---

# Tìm Message Enum

Thông báo cần: $ARGUMENTS

## Bước 1: Map sang Message.Enum

### Kết quả xử lý
| Nội dung | Enum |
|----------|------|
| Xử lý thành công | HeThongTBKQXLYCCuaFrontendThanhCong |
| Xử lý thất bại | HeThongTBKQXLYCCuaFrontendThatBai |
| Exception chưa kiểm soát | HeThongTBXuatHienExceptionChuaKiemDuocSoat |

### Tiêu đề dialog
| Nội dung | Enum |
|----------|------|
| Thông báo | TieuDeCuaSoThongBaoLaThongBao |
| Cảnh báo | TieuDeCuaSoThongBaoLaCanhBao |
| Lỗi | TieuDeCuaSoThongBaoLaLoi |

### Confirm
| Nội dung | Enum |
|----------|------|
| Bạn có muốn hủy? | HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong |
| Bạn có muốn khóa? | HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong |
| Bạn có muốn xóa? | HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong |
| Bạn có muốn duyệt? | HeThongTBCuaSoThongBaoBanCoMuonDuyetKhoaDuLieuKhong |
| Bạn có muốn bỏ duyệt? | HeThongTBCuaSoThongBaoBanCoMuonBoDuyetKhoaDuLieuKhong |
| Bạn có muốn bỏ khóa? | HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong |

### Validation
| Nội dung | Enum |
|----------|------|
| Trường bắt buộc | TruongDuLieuBatBuoc |
| Thiếu trường bắt buộc | ThieuTruongDuLieuBatBuoc |
| Dữ liệu không hợp lệ | NguoiDungNhapDuLieuKhongHopLe |
| Giá trị phải lớn hơn 0 | SoLuongKhongDuocBeHonKhong |
| Không nhận giá trị âm | TruongDuLieuKhongNhanGiaTriAm |
| File đính kèm quá lớn | DungLuongFileDinhKemQuaLon |

### Hệ thống
| Nội dung | Enum |
|----------|------|
| Không kết nối server | PhanMemKhongKetNoiDuocToiMayChuHeThong |
| Hết phiên làm việc | HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai |
| Không có quyền | TaiKhoanKhongCoQuyenThucHienChucNang |
| Đang phát triển | ChucNangDangPhatTrienVuiLongThuLaiSau |
| Dữ liệu đang khóa | DuLieuDangKhoa |
| Dữ liệu đang mở | DuLieuDangMo |
| Không tìm thấy plugin | HeThongTBKhongTimThayPluginsCuaChucNangNay |
| Bản quyền không hợp lệ | HeThongTBBanQuyenKhongHopLe |

## Bước 2: Code mẫu

```csharp
// Lấy message text theo ngôn ngữ hiện tại
string msg = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);

// Message có tham số
string msg = MessageUtil.GetMessage(Message.Enum.SomeEnum, new string[] { "param1" });

// Tiêu đề dialog
string title = MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao);

// Confirm dialog
XtraMessageBox.Show(
    MessageUtil.GetMessage(Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
    MessageUtil.GetMessage(Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

// Sau API call
MessageUtil.SetResultParam(param, success);
MessageManager.Show(this, param, success);
```

## Bước 3: Nếu không tìm thấy Enum phù hợp

Tạo message riêng plugin qua ResourceMessage pattern:

```csharp
class ResourceMessage {
    static ResourceManager languageMessage = new ResourceManager(
        "HIS.Desktop.Plugins.{Name}.Resources.Message.Lang",
        Assembly.GetExecutingAssembly());
    internal static string MyCustomMessage {
        get { return Inventec.Common.Resource.Get.Value(
            "MyCustomMessage", languageMessage, LanguageManager.GetCulture()); }
    }
}
```
