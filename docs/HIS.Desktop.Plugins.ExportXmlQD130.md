# Xuất XML QĐ130 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExportXmlQD130 |
| Loại | UC |
| Mục đích | Xuất/gửi XML giám định BHYT theo QĐ 130/4210 (GIAMDINHHS); đồng bộ dữ liệu KCB (Kết thúc khám/Xuất viện) lên các hệ thống ngoài. |
| Trạng thái | Bảo trì |

## 2. Đích đồng bộ KCB (luồng kcb4750Only: menu "Đồng bộ Khám chữa bệnh" + Luồng 1 tự động)

| Đích | Gate | Khóa HIS_CONFIG | Giao thức |
|------|------|-----------------|-----------|
| CSDL 4750 (CSDL dùng chung ngành y tế) | `MOS.CSDL_4750.IS_AUTO_SYNC=1` + checkbox "Đồng bộ KCB" (Cài đặt) | `HIS.CSDL_4750.CONNECTION_INFO` | multipart file XML, token /get-token |
| **Cổng tiếp nhận KDLYT Vĩnh Long (mới 13/08/2026)** | có khóa VLG + checkbox "Đồng bộ KCB lên Cổng tiếp nhận KDLYT Vĩnh Long (hoàn tất)" (Cài đặt — chỉ hiện khi có khóa) | `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` (dùng chung với liên thông KSK VLG) | POST `/api/kham-chua-benh/hoan-tat` — XML GIAMDINHHS trực tiếp (application/xml), Bearer token JSON `/api/xac-thuc/token`; ACCEPTED* = vào hàng đợi (bất đồng bộ) |

Ghi chú `VlgKcbHoanTatWorker`: cùng file XML với luồng 4750 (NOIDUNGFILE Base64 sẵn trong batch); giới hạn 10 MiB; retry 401 một lần; fail-fast cả lô khi login lỗi; không log mật khẩu. **Trạng thái finish (UpdateCsdl4750FinishInfo): khi bật CẢ 2 đích lấy theo 4750 (VLG chỉ ghi dòng kết quả + log); khi CHỈ bật VLG lấy theo VLG** để chu kỳ tự động không chọn lại hồ sơ đã đẩy. Kết quả xử lý thật phía cổng tra ở màn "Tra cứu Cổng KDLYT Vĩnh Long" — loại hồ sơ KCB (theo MA_LK = mã điều trị).

## 3. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/08/2026 | khainq | Fix review vòng 2 (build 00:52): gửi tay qua menu khi chế độ auto đang bật không còn fire-and-forget (dialog từng đọc 0 kết quả → bấm lại gây đẩy trùng) — dùng cờ `manualSyncKcb4750` (trước đây khai báo nhưng không bao giờ đọc), lượt gửi tay cũng không bị hủy khi tắt auto giữa chừng; `kcb4750ResultLines` capture local trong task (task lượt trước không Add vào list lượt sau); chặn menu khi `backgroundWorker1.IsBusy` (flags `isNotFileSign` dùng chung — chạy chồng làm lô BHYT tự động gửi KHÔNG ký); VLG-only không còn bị chặn khi IS_AUTO_SYNC=1 mà thiếu key 4750; header dialog đếm theo "lượt gửi"; cast `j["data"] as JObject` trong VlgKcbHoanTatWorker. |
| 13/08/2026 | khainq | Thêm đích đẩy KCB thứ 2 "Cổng tiếp nhận KDLYT Vĩnh Long": `Base/VlgKcbHoanTatWorker.cs` mới; checkbox mới trong frmSettingConfigSync (`isSyncKcbVlg` — lưu ngay khi tick, ẩn khi viện không có khóa); menu "Đồng bộ Khám chữa bệnh" hiện thêm khi có khóa VLG; luồng đẩy nền chung Task với 4750 (lỗi 2 đích cô lập nhau); điều kiện dispatch tự động + hủy giữa lô tính cả isSyncKcbVlg. Sửa build máy backup: 3 chỗ `ado.TotalHeinPatientTypeData` (property chưa có trong DLL His.Bhyt.ExportXml.XML130 bản máy này) → gán reflection `SetAdoPropIfExists` để build cả DLL cũ lẫn mới; tạo `Properties/licenses.licx` rỗng. |

## 4. Test Cases
- [ ] Viện không có khóa VLG + không bật 4750: menu Đồng bộ KCB không hiện; form Cài đặt không có checkbox VLG — hành vi y cũ.
- [ ] Viện chỉ VLG: tick checkbox → menu gửi tay đẩy hoan-tat, dialog kết quả kèm tracking; hồ sơ đã đẩy không bị chọn lại ở chu kỳ tự động.
- [ ] Viện cả 2: mỗi hồ sơ 2 dòng kết quả (4750 + VLG); lỗi VLG không làm hỏng kết quả 4750 và ngược lại.
- [ ] ConfigSync cũ (JSON không có isSyncKcbVlg) → mặc định bỏ tick, không lỗi.
