# Danh Mục Dịch Vụ Không Chỉ Định Đồng Thời (HisServiceExclusive) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisServiceExclusive |
| Loại | UC (UserControl, nhóm Danh mục — menu "Dịch vụ kỹ thuật") |
| Mục đích | Khai báo **bản ghi cấu hình** "Dịch vụ không chỉ định đồng thời": 1 dịch vụ gốc + nhiều dịch vụ không được chỉ định cùng, kèm Mức xử lý (Cảnh báo / Chặn), Trạng thái (Còn / Ngừng sử dụng) và Ghi chú. Lưu xuống bảng `HIS_SERVICE_EXCLUSIVE` theo từng cặp. |
| Người tạo | dangth + Claude |
| Ngày tạo | 16/09/2026 |
| Việc | 57452 — TTMB-TK-56258 (thiết kế) / 57644 (code FE) / tài liệu phân tích 3342 (PT-56258) |
| Trạng thái | Hoàn thành FE — chờ backend `api/HisServiceExclusive/*` và bảng `HIS_SERVICE_EXCLUSIVE` |

Thư viện đi kèm: **`HIS.Desktop.Plugins.Library.CheckServiceExclusive`** — dùng chung cho các màn chỉ định dịch vụ để kiểm tra khi lưu.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở màn "Dịch vụ không chỉ định đồng thời".
2. Grid **trái** (dịch vụ gốc): lọc theo Loại dịch vụ + từ khóa mã/tên; tick **"Đã khai báo"** để chỉ hiện các dịch vụ đã có bản ghi cấu hình; tick **radio** chọn 1 dịch vụ gốc.
3. Khi tick radio → gọi `api/HisServiceExclusive/Get` với `SERVICE_ID__OR__EXCLUSIVE_ID` (tra **2 chiều**, **không lọc IS_ACTIVE**) → grid phải tick sẵn các dịch vụ đã khai (dòng đã khai đẩy lên đầu); panel dưới đổ **Mức xử lý** (theo cặp phổ biến nhất), **Còn sử dụng** (còn ít nhất 1 cặp `IS_ACTIVE = 1`), **Ghi chú** (ghi chú đầu tiên khác rỗng). Chưa có cặp → panel về mặc định **Cảnh báo / Còn sử dụng / rỗng**.
4. Grid **phải**: tick cột **Chọn** cho từng dịch vụ không được chỉ định cùng dịch vụ gốc; click header = tick/bỏ tick cả trang (không bao giờ tick chính dịch vụ gốc).
5. Panel bản ghi: chọn **Mức xử lý** (Cảnh báo mặc định / Chặn), tick/bỏ **Còn sử dụng**, nhập **Ghi chú** (tối đa 2000 ký tự) — áp dụng cho **cả bản ghi** (mọi cặp của dịch vụ gốc).
6. "Lưu (Ctrl S)" — so diff bản ghi:
   - Cặp tick thêm → `api/HisServiceExclusive/CreateList` (mang Mức / Trạng thái / Ghi chú của panel)
   - Cặp bỏ tick (trong trang lưới đang hiện) → `api/HisServiceExclusive/DeleteList`
   - Cặp giữ lại (kể cả cặp không hiện trên trang) mà Mức / Ghi chú / Trạng thái khác panel → `api/HisServiceExclusive/UpdateList`; cặp đang **Ngừng sử dụng** phải `ChangeLock` mở khóa trước (backend chặn Update bản ghi `IS_ACTIVE = 0`)
   - Lưu xong: `CheckServiceExclusiveManager.ResetData()` → các màn chỉ định nạp lại danh mục; nạp lại bản ghi từ backend để grid + panel đúng dữ liệu đã lưu.

### Ánh xạ thao tác theo tài liệu 3342
| Thao tác 3342 | Cách làm trên màn |
|---|---|
| Thêm mới | Chọn gốc → tick dịch vụ loại trừ → chọn Mức → Lưu |
| Sửa | Đổi tick / Mức xử lý / Ghi chú → Lưu |
| Xóa | Bỏ tick dịch vụ → Lưu |
| Chuyển Ngừng sử dụng | Bỏ tick "Còn sử dụng" → Lưu (bản ghi vẫn hiện để bật lại) |
| Tìm kiếm theo mã / tên | 2 ô từ khóa + combo Loại dịch vụ; lọc "Đã khai báo" |

### Điều kiện nghiệp vụ
- Chưa tick radio dịch vụ gốc mà bấm Lưu → "Chưa chọn dịch vụ gốc ở lưới bên trái".
- Tick chính dịch vụ gốc ở lưới phải → "Không được khai báo dịch vụ loại trừ với chính nó", không lưu.
- Không có cặp nào được tick và chưa có cặp cũ → "Chưa chọn dịch vụ loại trừ và mức xử lý"; diff rỗng → "Không có thay đổi nào để lưu".
- Quan hệ **đối xứng**: chỉ lưu 1 bản ghi cho mỗi cặp; chọn gốc B sẽ thấy A đã tick (khai từ phía A) và chỉ cập nhật cặp cũ, không tạo cặp ngược trùng.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE | View | Nguồn dữ liệu cả 2 lưới (SERVICE_CODE, SERVICE_NAME, SERVICE_TYPE_NAME) |
| HIS_SERVICE_TYPE | Table (cache BackendDataWorker) | 2 combo "Loại dịch vụ" lọc 2 lưới |
| HIS_SERVICE_EXCLUSIVE | Table — **chưa có trong MOS.EFMODEL.dll** | Bảng cặp dịch vụ loại trừ: SERVICE_ID, EXCLUSIVE_ID, HANDLE_TYPE_ID (1 Cảnh báo / 2 Chặn), IS_ACTIVE (Trạng thái), NOTE (Ghi chú) |

**Lưu ý kỹ thuật:** bảng `HIS_SERVICE_EXCLUSIVE` / view `V_HIS_SERVICE_EXCLUSIVE` và filter tương ứng chưa có trong `MOS.EFMODEL.dll` + `MOS.Filter.dll` đang bàn giao. FE tự khai lớp cục bộ trong thư viện `HIS.Desktop.Plugins.Library.CheckServiceExclusive/ADO/` (tên property trùng tuyệt đối với tên cột backend nên JSON map đúng). **Khi backend bàn giao DLL mới thì thay bằng `MOS.EFMODEL.DataModels.*` và `MOS.Filter.HisServiceExclusiveFilter`.**

Filter: `MOS.Filter.HisServiceViewFilter` (KEY_WORD, SERVICE_TYPE_ID, IDs cho lọc "Đã khai báo") cho 2 lưới; `ADO/HisServiceExclusiveFilter.cs` (filter local, có `SERVICE_ID__OR__EXCLUSIVE_ID` để tra 2 chiều).

## 4. UI Layout

```
+--[Từ khóa] [Loại dịch vụ ▼] [Tìm (Ctrl D)] [☐ Đã khai báo]--+--[Từ khóa] [Loại dịch vụ ▼] [Tìm (Ctrl F)]--+
| GRID DỊCH VỤ GỐC (panelControl1 ← HIS.UC.Service)          | GRID DV KHÔNG CHỈ ĐỊNH ĐỒNG THỜI (panelControl2)|
| (o) | Mã dịch vụ | Tên dịch vụ | Loại dịch vụ              | [x] Chọn | Mã | Tên | Loại dịch vụ           |
+--[ucPaging1]-----------------------------------------------+--[ucPaging2]----------------------------------+
| Mức xử lý: [cboHandleType ▼]  [x] Còn sử dụng (chkIsActive)                          [Lưu (Ctrl S)]      |
| Ghi chú:   [txtNote — MemoEdit 2 dòng .............................................................]      |
+----------------------------------------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Service (instance 1) | panelControl1 | Lưới dịch vụ gốc, `isKeyChooseService = true` → cột `radioService` hoạt động |
| HIS.UC.Service (instance 2) | panelControl2 | Lưới dịch vụ loại trừ, `isKeyChooseService = false` → cột tick hoạt động |
| Inventec.UC.Paging | ucPaging1 / ucPaging2 | Phân trang từng lưới |

**Cột "Chọn"** dùng repository `checkWarning` sẵn có của `HIS.UC.Service` (handler chỉ bật/tắt cờ, không tra cứu phòng/giá như `checkService`) → không sửa UC dùng chung (24 project đang tham chiếu). Combo Mức xử lý nạp từ `HandleTypeItem` (1 = Cảnh báo, 2 = Chặn) qua `ControlEditorLoader`.

## 5. API Endpoints

| Action | URI (hằng trong `HisRequestUriStore` của thư viện) | Consumer | Filter/Payload |
|---|---|---|---|
| Nạp 2 lưới dịch vụ | `MOSHIS_SERVICE_GET_VIEW` = `api/HisService/GetView` | MosConsumer | `HisServiceViewFilter` (KEY_WORD, SERVICE_TYPE_ID, IDs) |
| Lấy cặp của 1 dịch vụ / toàn bảng | `MOSHIS_SERVICE_EXCLUSIVE_GET` = `api/HisServiceExclusive/Get` | MosConsumer | `HisServiceExclusiveFilter { SERVICE_ID__OR__EXCLUSIVE_ID }` hoặc rỗng — **chờ backend** |
| Nạp danh mục vào RAM (thư viện) | `MOSHIS_SERVICE_EXCLUSIVE_GET_VIEW` = `api/HisServiceExclusive/GetView` | MosConsumer | `HisServiceExclusiveFilter { IS_ACTIVE }` — **chờ backend** |
| Thêm cặp | `.../CreateList` | MosConsumer | `List<HIS_SERVICE_EXCLUSIVE>` (HANDLE_TYPE_ID, NOTE, IS_ACTIVE) — **chờ backend** |
| Đổi Mức / Ghi chú / Trạng thái | `.../UpdateList` | MosConsumer | `List<HIS_SERVICE_EXCLUSIVE>` — **chờ backend** |
| Xóa cặp | `.../DeleteList` | MosConsumer | `List<long>` — **chờ backend** |
| Mở khóa bản ghi Ngừng sử dụng trước khi sửa | `MOSHIS_SERVICE_EXCLUSIVE_CHANGE_LOCK` = `api/HisServiceExclusive/ChangeLock` | MosConsumer | `long` (ID cặp) — **chờ backend** |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|---|---|---|
| (được mở từ menu Danh mục) | — | `Module`; có thể truyền thêm `V_HIS_SERVICE` để mở sẵn theo 1 dịch vụ |

### Thư viện dùng chung
`HIS.Desktop.Plugins.Library.CheckServiceExclusive` — tham chiếu bằng DLL HintPath `lib\HIS\HIS.Desktop.Plugins.Library.CheckServiceExclusive\...dll` (mô hình `Library.CheckIcd`). Màn danh mục dùng lớp ADO (`HIS_SERVICE_EXCLUSIVE`, `HisServiceExclusiveFilter`, enum `HandleType`) + `HisRequestUriStore` + `ResetData()` của thư viện này.

### Đăng ký menu (ACS_MODULE)
| Cột | Giá trị |
|---|---|
| MODULE_LINK | `HIS.Desktop.Plugins.HisServiceExclusive` |
| MODULE_NAME | Dịch vụ không chỉ định đồng thời |
| APPLICATION_ID | 2 |
| MODULE_GROUP_ID | nhóm "Dịch vụ kỹ thuật" (tra `ACS_MODULE_GROUP`; script để sẵn 2 — nhóm của "Dịch vụ đi kèm" id 38, "Dịch vụ cùng cơ chế" id 635) |
| IS_ACTIVE / IS_VISIBLE / IS_LEAF | 1 / 1 / 1 |

Script: `PTTK\57452_db.sql` (bước 3).

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|---|---|---|
| 21/09/2026 | dangth + Claude | Rà lại theo tài liệu phân tích 3342: đổi tên chức năng "Dịch vụ không chỉ định đồng thời"; lưới phải còn 1 cột Chọn; thêm panel bản ghi Mức xử lý (mặc định Cảnh báo) / Còn sử dụng / Ghi chú áp dụng cho cả bản ghi; lưu diff có `UpdateList` + `ChangeLock`; lọc "Đã khai báo" ở lưới trái; combo Loại dịch vụ riêng cho lưới phải. Thư viện: thêm `NOTE`, cột Ghi chú trên form cảnh báo, câu thông báo đúng từng chữ 3342 (nối ". Ghi chú: …" nếu có). |
| 16/09/2026 | dangth + Claude | Tạo mới plugin theo thiết kế việc 57452. Clone khuôn 2 lưới từ `HIS.Desktop.Plugins.HisServiceSpeciality`, thay lưới phải bằng instance thứ 2 của `HIS.UC.Service`, 2 cột mức xử lý Cảnh báo/Chặn, lưu theo diff Create/Update/Delete. |

## 9. Test Cases

- [ ] Mở màn hình: 2 lưới hiển thị, tìm kiếm + phân trang + lọc Loại dịch vụ chạy đúng ở cả 2 bên; panel mặc định Cảnh báo / Còn sử dụng / Ghi chú rỗng.
- [ ] Tick radio 1 dịch vụ bên trái → lưới phải tick sẵn đúng các dịch vụ đã khai; panel đổ đúng Mức / Trạng thái / Ghi chú của bản ghi.
- [ ] KB1: chọn gốc, tick dịch vụ, Mức "Chặn", Lưu → mở lại giữ nguyên.
- [ ] Đổi Mức Chặn → Cảnh báo, nhập Ghi chú → Lưu → `UpdateList`, mở lại đúng.
- [ ] Bỏ tick → Lưu → `DeleteList`, mở lại không còn cặp đó.
- [ ] Bỏ tick "Còn sử dụng" → Lưu → bản ghi Ngừng sử dụng, vẫn hiện tick khi mở lại; tick lại "Còn sử dụng" → Lưu → gọi `ChangeLock` rồi `UpdateList`, bản ghi hiệu lực lại.
- [ ] Tick chính dịch vụ gốc ở lưới phải → báo lỗi, không lưu.
- [ ] Không thay đổi gì mà bấm Lưu → "Không có thay đổi nào để lưu"; chưa tick gì và chưa có cặp → "Chưa chọn dịch vụ loại trừ và mức xử lý".
- [ ] Chưa chọn dịch vụ gốc mà bấm Lưu → "Chưa chọn dịch vụ gốc ở lưới bên trái".
- [ ] Khai A→B rồi chọn radio B → lưới phải tick sẵn A (tra 2 chiều); đổi Mức ở B cập nhật đúng cặp cũ, không tạo cặp trùng.
- [ ] Tick "Đã khai báo" → lưới trái chỉ còn dịch vụ đã có bản ghi (cả Ngừng sử dụng); bỏ tick → đủ danh mục.
- [ ] Click header cột Chọn → tick/bỏ tick cả trang, dòng trùng dịch vụ gốc không bị tick.
- [ ] Ctrl+D / Ctrl+F / Ctrl+S hoạt động.
- [ ] Backend chưa triển khai API → mở màn không văng lỗi, chỉ ghi log Warn.
