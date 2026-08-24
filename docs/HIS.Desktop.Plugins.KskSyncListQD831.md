# Khám sức khỏe — Đồng bộ QĐ831 — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.KskSyncListQD831 |
| Loại | UC |
| Mục đích | Danh sách hồ sơ sức khỏe QĐ831 (V_HIS_KSK_PROFILE), sinh XML 831 (DATA/HEADER/BODY/HOSOSUCKHOE) và đẩy lên cổng liên thông. |
| Trạng thái | Bảo trì |

## 2. Cổng liên thông (tick chọn ở nút Cài đặt — ControlState)

| Cổng | Khóa HIS_CONFIG | Giao thức |
|------|-----------------|-----------|
| HSSK QĐ831 (CSDL dùng chung tỉnh) | `MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO` = tài khoản\|mật khẩu\|địa chỉ gốc\|api-login\|api-push | POST multipart (xmlFile + nguoi_gui), token /get-token 3h |
| **Cổng tiếp nhận KDLYT Vĩnh Long (mới 13/08/2026)** | `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` = MaDonVi\|Username\|Password\|TokenUrl\|PushUrl (dùng chung khóa với liên thông KSK VLG; 2 URL trống = cổng chính thức) | POST `/api/ho-so-suc-khoe/qd-831-2017/tiep-nhan` — XML trực tiếp (application/xml), Bearer token JSON `/api/xac-thuc/token` |

Ràng buộc cổng VLG (Ksk831VlgSyncer tự xử lý): `HEADER/SENDER_CODE` = MaDonVi (khớp token, sai bị 403 ORG_MISMATCH); `HEADER/REQUEST_ID` = `HIS831-<mã hồ sơ>-<12hex SHA256 nội dung>` ≤100 ký tự (gửi lại y nguyên → ACCEPTED_DUPLICATE giữ tracking cũ; đổi nội dung → mã mới, tránh 409); giới hạn 10 MiB; fail-fast cả lô khi đăng nhập lỗi 0/401/403/429; không log mật khẩu. Đẩy cổng 831 cũ TRƯỚC rồi mới đẩy VLG (VLG sửa header của Data).

Kết quả lưu `HIS_KSK_PROFILE` (api/HisKskProfile/SaveSyncResult): SYNC_RESULT_TYPE = 2 khi TẤT CẢ cổng tick thành công, 3 khi có cổng lỗi (SYNC_FAILD_REASON tiền tố `831:` / `VLG:`). Tra kết quả xử lý phía cổng VLG: màn "Tra cứu Cổng KDLYT Vĩnh Long" — loại hồ sơ HSSK 831 (theo tracking_id).

## 3. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 14/08/2026 | khainq | Chốt "Cổng tiếp nhận thay thế hoàn toàn hệ CSDL cũ": viện KHÔNG cấu hình key `MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO` nữa (checkbox cổng cũ tự khóa) — code cổng cũ giữ nguyên làm đường lui. GỠ `Ksk831SyncConfig.TempDefault()` (dead code chứa tài khoản test hardcode). Build 16:23. |
| 14/08/2026 | khainq | Fix review vòng 2 (build 00:51): cast `jo["data"] as JObject` trong Ksk831VlgSyncer (body `{"data":null}` làm trượt latch fail-fast 401/403/429); lỗi một phần ghi rõ "831: OK — KHÔNG cần đẩy lại" trong SYNC_FAILD_REASON để tránh retry đẩy trùng lên cổng đã nhận; GỠ log TEMP full bearer token + full XML bệnh nhân trong Ksk831Syncer cũ. |
| 13/08/2026 | khainq | Thêm cổng thứ 2 "Cổng tiếp nhận KDLYT Vĩnh Long (831)": `Sync/Ksk831VlgSyncer.cs` mới; popup Cài đặt 2 checkbox (tick lưu ControlState, key `btnSettings_Vlg831`, auto-tick lần đầu khi có khóa); SyncRecords đẩy tuần tự các cổng đã tick, gộp trạng thái; nút Đồng bộ mở khi tick ≥1 cổng. Viện không có khóa VLG: checkbox khóa, hành vi y cũ. Bẫy build: tạo `Properties/licenses.licx` rỗng. |

## 4. Test Cases
- [ ] Viện chỉ có khóa 831 cũ: popup 1 checkbox khả dụng, hành vi y cũ.
- [ ] Viện có cả 2 khóa: tick cả 2 → 1 hồ sơ đẩy 2 cổng, trạng thái 2 khi cả 2 OK; 1 cổng lỗi → 3 kèm lý do có tiền tố.
- [ ] Đẩy lại y nguyên lên VLG → ACCEPTED_DUPLICATE vẫn tính thành công.
- [ ] Sai mật khẩu VLG → hồ sơ đầu lỗi, các hồ sơ sau fail-fast không đăng nhập lại.
