# CheckHeinGOV — Tài Liệu Module (Library)

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Library.CheckHeinGOV (library dùng chung, không lên menu) |
| Loại | Library |
| Mục đích | Xác thực thẻ BHYT qua cổng BHXH (kiểm tra thông tuyến): gọi CheckHistory, phân tích lịch sử KCB + lịch sử kiểm tra thẻ, sinh cảnh báo tại tiếp đón và các màn liên quan. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

`HeinGOVManager.Check()` / `CheckCccdQrCode()`: gọi cổng (`apiInsuranceExpertise.CheckHistory`, DLL `His.Bhyt.InsuranceExpertise`) → nhận `ResultHistoryLDO` → 2 khối cảnh báo:
- **Khối A — lịch sử KCB** (`dsLichSuKCB2018`, `LoadDataOld`): hồ sơ có `ngayRa >= hôm nay` → "bệnh nhân có khám, chữa bệnh tại cơ sở X..." (chưa ra viện: chặn maKetQua 9999; ra viện trong ngày: hỏi Yes/No).
- **Khối B — lịch sử kiểm tra thẻ** (`dsLichSuKT2018`): lần kiểm tra thẻ tại CSKCB khác (`userKT` không chứa `BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE`), mã lỗi hợp lệ (000-003), thời gian kiểm tra SAU lần ra viện gần nhất (`thoiGianKT > maxNgayRa`) → "Thẻ BHYT có thông tin kiểm tra thẻ tại {tên CSKCB} ({mã}) [dd/MM/yyyy HH:mm]", maKetQua 8888, map vào grid lịch sử (tinhTrang "5").
- Khối B lặp tại 4 vị trí: HeinGOVManager.cs trong `Check()` (~:514), `LoadDataOld()` (~:724, bản chính), `CheckCccdQrCode()` (~:1380) và bản copy tại plugin `CheckInfoBHYT/frmCheckInfoBHYT___BHYT.cs` — **sửa phải đồng bộ 4 chỗ**.

## 3. EFMODEL / Dữ liệu

`ResultHistoryLDO{dsLichSuKCB2018: ExamHistoryLDO[maCSKCB, ngayVao, ngayRa, tinhTrang, kqDieuTri], dsLichSuKT2018[userKT, thoiGianKT, maLoi]}` (DLL His.Bhyt.InsuranceExpertise); `HIS_MEDI_ORG` (tra tên CSKCB); `HIS_BRANCH.HEIN_MEDI_ORG_CODE` (mã viện mình); `HIS_BHYT_WHITELIST` (bỏ qua check).

## 4. Config

| Key | Ý nghĩa |
|-----|---------|
| `HIS.Desktop.Plugins.Register.WarningInvalidCheckHistoryHeinCard` | Bật/tắt cảnh báo lịch sử kiểm tra thẻ (khối B) |
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO` (per-user) | > 0 mới gọi cổng |
| `HIS.CHECK_HEIN_CARD.BHXH.*` | Tài khoản/địa chỉ cổng BHXH |

## 5. Nơi sử dụng

Tiếp đón 1/2 (Register, RegisterV2), UC nhập BN (UCPatientRaw), Đổi đối tượng (CallPatientTypeAlter), Phòng khám (ExecuteRoom, showMessage=false), Kết thúc điều trị (TreatmentFinish, showMessage=false), màn Kiểm tra thông tin BHYT (CheckInfoBHYT), Kiosk (bản cảnh báo rút gọn riêng).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | nampp | Tạo tài liệu module. PTTK_3155 (PT-53435): khối cảnh báo lịch sử kiểm tra thẻ — BỎ ràng buộc bắt buộc có `dsLichSuKCB2018` (BN chưa có hồ sơ quyết toán trên cổng vẫn được cảnh báo, kể cả lần kiểm tra thẻ khác ngày hiện tại; KCB rỗng → maxNgayRa=0); gia cố parse `thoiGianKT` 12/14 ký tự, sai định dạng vẫn hiển thị chuỗi thô. Sửa đồng bộ 3 vị trí trong HeinGOVManager.cs (+ vị trí thứ 4 tại CheckInfoBHYT). Giữ nguyên message, maKetQua 8888, config, lọc viện khác, điều kiện thoiGianKT > maxNgayRa. |

## 9. Test Cases

- [ ] Lịch sử KT tại viện khác, BN KHÔNG có lịch sử KCB → vẫn cảnh báo (điểm mới).
- [ ] Lịch sử KT trước lần ra viện gần nhất → không cảnh báo.
- [ ] Lần KT tại chính viện mình / maLoi ngoài 000-003 → không cảnh báo.
- [ ] Config WarningInvalidCheckHistoryHeinCard tắt → không cảnh báo khối B.
- [ ] thoiGianKT 14 ký tự → vẫn hiển thị đúng dd/MM/yyyy HH:mm.
- [ ] Hồi quy: quét QR CCCD, khối A (đang điều trị theo hồ sơ KCB), grid lịch sử UCCheckTT.
