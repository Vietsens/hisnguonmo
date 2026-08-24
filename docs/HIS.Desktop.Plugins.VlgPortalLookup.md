# Tra cứu Cổng KDLYT Vĩnh Long — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.VlgPortalLookup |
| Loại | UC (MODULE_TYPE_ID__UC) |
| Mục đích | Màn hình tra cứu/đối soát hồ sơ KSK QĐ 2062 đã đẩy lên Cổng tiếp nhận — Kho dữ liệu y tế tỉnh Vĩnh Long, thay cho việc tra Postman thủ công. Chỉ ĐỌC dữ liệu từ cổng (GET), không đẩy. |
| Người tạo | khainq |
| Ngày tạo | 13/08/2026 |
| Trạng thái | Hoàn thành FE — chờ viện test |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở module (menu "Tra cứu Cổng KDLYT Vĩnh Long").
2. Load: tự kiểm tra kết nối cổng (GET `/api/xac-thuc/thong-tin`) — hiện tên đơn vị/tài khoản ở label trạng thái. Viện không có key cấu hình → báo "chưa cấu hình", các nút tìm kiếm bị khóa, KHÔNG gọi cổng.
3. Tra cứu 2 cách:
   - **Theo khoảng ngày khám** (tối đa 3 tháng — giới hạn của cổng) + bộ lọc trạng thái lỗi (Tất cả / Chỉ hồ sơ lỗi / Chỉ hồ sơ đạt) → GET `/api/kham-suc-khoe/qd-2062/ho-so` (phân trang 100/lần).
   - **Theo mã điều trị** (ô "Mã điều trị", Enter hoặc nút Tìm) → GET `/api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk=`.
4. Double-click 1 dòng → tải chi tiết kết quả kiểm tra của cổng (mã lỗi, mức độ, mô tả từng trường) vào ô chi tiết bên dưới (có cache theo dòng).
5. Nút **Đối soát với HIS**: lấy danh sách cổng theo khoảng ngày + gọi `api/HisKskSync/GetView` (lọc CONCLUSION_TIME cùng khoảng) → ghép theo mã điều trị, phân loại:
   - `✓ Khớp (VALID)` — cổng đạt, HIS ghi nhận Đã đồng bộ.
   - `⚠ XANH GIẢ` — HIS ghi Đã đồng bộ nhưng cổng chấm INVALID (nhận rồi mới validate async).
   - `⚠ Cổng chấm KHÔNG ĐẠT` — hồ sơ INVALID cần sửa dữ liệu và đẩy lại.
   - `⚠ Cổng ĐẠT nhưng HIS đang Thất bại` — cần đẩy lại/cập nhật KQ ở màn Đồng bộ KSK.
   - Hồ sơ chỉ có trên cổng (không thấy trong HIS theo khoảng lọc).
   Dòng lệch được sort lên đầu + tô đỏ; tổng hợp đếm từng loại hiện ở ô chi tiết.
6. Nút **Xuất Excel**: xuất đúng grid đang hiển thị (ExportToXlsx).

### Điều kiện nghiệp vụ
- Khoảng ngày ≤ 92 ngày (cổng giới hạn 3 tháng, lọc theo NGÀY KHÁM).
- Viện KHÔNG có key `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` → module vô hiệu hóa an toàn, không ảnh hưởng gì.
- Token cache theo `expires_in − 60s`; lỗi đăng nhập (0/401/403/429) → chốt fail-fast, không bão đăng nhập.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| (API view) api/HisKskSync/GetView | View backend | Trạng thái đồng bộ phía HIS để đối soát (SYNC_RESULT_TYPE, SYNC_FAILD_REASON, TRANSACTION_CODE) |

Dữ liệu cổng parse bằng Newtonsoft (JObject) → `ADO/VlgHoSoADO`.

## 4. UI Layout

```
+---------------------------------------------------------------------------------+
| Từ ngày [..] Đến ngày [..] Lọc lỗi [cbo] Mã điều trị [....] [Tìm kiếm]          |
| [Kiểm tra kết nối] [Đối soát với HIS] [Xuất Excel]   (label trạng thái kết nối)|
+---------------------------------------------------------------------------------+
| Grid: STT | Mã ĐT (fixed) | Họ tên (fixed) | CCCD | Ngày khám | Mẫu | KQ cổng  |
|       | TT mới nhất | TG nhận | Tracking | TT HIS | Đối soát | (12 cột)        |
+---------------------------------------------------------------------------------+
| (Splitter)                                                                       |
| Ô chi tiết (memo, read-only): lỗi từng trường / tổng hợp đối soát               |
+---------------------------------------------------------------------------------+
```

Màu: VALID xanh, INVALID đỏ, dòng lệch đối soát đỏ đậm.

## 5. API Endpoints

| Action | URI | Ghi chú |
|--------|-----|---------|
| Token | POST `{Base}/api/xac-thuc/token` | Body ma_don_vi/username/password; cache đến expires_in−60s |
| Thông tin TK | GET `{Base}/api/xac-thuc/thong-tin` | Kiểm tra kết nối |
| DS hồ sơ KSK | GET `{Base}/api/kham-suc-khoe/qd-2062/ho-so?from_date&to_date&error_status&page&page_size` | Trang 100, tối đa 50 trang |
| Trạng thái 1 hồ sơ | GET `{Base}/api/kham-suc-khoe/qd-2062/ho-so/trang-thai?ma_lk=` | ma_lk = TDL_TREATMENT_CODE |
| HIS đối soát | GET `api/HisKskSync/GetView` (MosConsumer) | Filter CONCLUSION_TIME range |

`{Base}` suy ra từ TokenUrl trong key cấu hình (cắt `/api/xac-thuc/token`). Dùng chung key với plugin Đồng bộ KSK: `MOS.HIS_KSK_SYNC.VLG_2062_CONNECTION_INFO` = `MaDonVi|Username|Password|TokenUrl|PushUrl`.

## 6. Dependencies

| Thành phần | Mục đích |
|-----------|----------|
| HIS.Desktop.ApiConsumer / BackendAdapter | Gọi api/HisKskSync/GetView |
| HIS.Desktop.LocalStorage.BackendData | HisConfigs.Get key cấu hình |
| Inventec.Desktop.Common.Message | WaitingManager/MessageManager |
| Newtonsoft.Json | Parse JSON cổng |
| HIS.Desktop.Plugins.KskSyncList | KHÔNG tham chiếu code — chỉ dùng chung key cấu hình |

## 7. Print
Không có in MPS — chỉ Xuất Excel (gridControl.ExportToXlsx).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/08/2026 | khainq | Tạo mới plugin: tra cứu DS hồ sơ/trạng thái theo mã điều trị, đối soát cổng↔HIS, xuất Excel, kiểm tra kết nối. SQL đăng ký module clone từ KskSyncList (VlgPortalLookup_INSERT_ACS_MODULE.sql). |
| 14/08/2026 | khainq | Fix review vòng 2 (build 00:51): MA_YEU_CAU hủy/khôi phục đổi sang timestamp (hash lý do bị cổng dedup khi hủy→khôi phục→hủy lại cùng lý do); mọi chỗ đọc `jo["data"]` cast `as JObject` (body `{"data":null}` là JValue ≠ null → exception làm trượt latch 401/403/429); ẩn cột "Lần gửi cuối" ở mode HSSK; nới caption "Ngày … từ:" 50→80px. |
| 13/08/2026 | khainq | Mở rộng theo ý tưởng mới của anh Nam: combo **Loại hồ sơ** (KSK 2062 / KCB / HSSK 831) — tra danh sách + chi tiết cho cả 3 nhóm (`/api/kham-chua-benh/ho-so[,/trang-thai]`, `/api/ho-so-suc-khoe/qd-831-2017/ho-so[,/trang-thai?tracking_id=]`); nút **Hủy hồ sơ (cổng)** / **Khôi phục hồ sơ** cho nhóm KCB (bắt nhập lý do, xác nhận, PROCESSED ngay, 404 khi chưa từng đẩy, MA_YEU_CAU idempotency); cột/caption đổi theo mode; Đối soát chỉ mở ở mode KSK; fix nút Đối soát bị khóa sau Load (ApplyMode chạy lại sau InitClient); khóa combo loại hồ sơ khi đang bận. |
| 13/08/2026 | khainq | Fix theo review đối kháng (build 15:10): (1) `ResetBatchError()` đầu mỗi thao tác — 1 lần mất mạng không khóa màn vĩnh viễn; (2) `FetchHisRows` trả null khi API HIS lỗi → chặn đối soát thay vì báo cáo sai; (3) paging không dừng sớm khi cổng thiếu `total`, cảnh báo khi cắt ở 5000 hồ sơ; (4) lock `GetToken` chống đăng nhập trùng (429); (5) check `ev.Error` trước `ev.Result` ở 3 handler + guard dispose; (6) guard viện chưa cấu hình: Enter txtMaLk không vượt rào, `SetBusy` không mở khóa nút; (7) đối soát: xác minh trực tiếp qua `/trang-thai` (≤30 mã) trước khi kết luận XANH GIẢ (lệch trục ngày khám/kết luận); tách 3 trạng thái VALID/INVALID/đang xử lý; map SYNC_RESULT_TYPE=4 "Có chỉnh sửa — đẩy lại"; type 1 có trên cổng = lệch trạng thái; gom nhiều dòng HIS cùng mã điều trị (ưu tiên 4>2>3>1); header đối soát dùng khoảng ngày đã chụp; (8) `FontStyleDelta` thay `new Font` mỗi cell; `EndUpdate` trong finally; luôn tải lại chi tiết khi nhấp đúp; Excel đếm theo dòng hiển thị; `colStt` Fixed Left; sửa suy BaseUrl lệch 1 ký tự với TokenUrl dạng lạ. |

## 9. Test Cases

- [ ] Viện KHÔNG có key VLG → mở module không lỗi, nút bị khóa, thông báo chưa cấu hình.
- [ ] Kiểm tra kết nối → hiện tên đơn vị 83009.
- [ ] Tìm theo khoảng ngày ≤ 3 tháng → grid có dữ liệu, lọc "Chỉ hồ sơ lỗi" đúng.
- [ ] Khoảng ngày > 92 ngày → chặn có thông báo.
- [ ] Tìm theo mã điều trị có thật → 1 dòng + chi tiết; mã không tồn tại → thông báo không tìm thấy.
- [ ] Double-click dòng → ô chi tiết hiện lỗi từng trường.
- [ ] Đối soát: hồ sơ VALID + HIS type 2 → Khớp; INVALID + type 2 → XANH GIẢ; VALID + type 3 → cảnh báo.
- [ ] Xuất Excel mở được file, đúng số dòng grid.
