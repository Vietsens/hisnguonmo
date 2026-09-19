# Danh Mục Dịch Vụ Không Được Chỉ Định Đồng Thời (HisServiceExclusive) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisServiceExclusive |
| Loại | UC (UserControl, nhóm Danh mục) |
| Mục đích | Khai báo các cặp dịch vụ **không được phép chỉ định đồng thời** trong cùng một lần điều trị (bảng `HIS_SERVICE_EXCLUSIVE`), kèm mức xử lý **Cảnh báo** / **Chặn** cho từng cặp. |
| Người tạo | dangth + Claude |
| Ngày tạo | 16/09/2026 |
| Việc | 57452 — TTMB-TK-56258 (việc cha 57451 PT-56258) |
| Trạng thái | Hoàn thành FE — chờ backend `api/HisServiceExclusive/*` và bảng `HIS_SERVICE_EXCLUSIVE` |

Thư viện đi kèm: **`HIS.Desktop.Plugins.Library.CheckServiceExclusive`** — dùng chung cho các màn chỉ định dịch vụ để kiểm tra khi lưu.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở màn "Dịch vụ không được chỉ định đồng thời" (nhóm Danh mục).
2. Grid **trái** (dịch vụ gốc): lọc theo Loại dịch vụ + từ khóa, tick **radio** chọn 1 dịch vụ.
3. Khi tick radio → gọi `api/HisServiceExclusive/Get` với `SERVICE_ID__OR__EXCLUSIVE_ID` (tra **2 chiều**) → grid phải tự tick sẵn các dịch vụ đã khai, đúng mức xử lý đang lưu; dòng đã khai đẩy lên đầu.
4. Grid **phải** (dịch vụ loại trừ): tick 1 trong 2 cột mức xử lý cho từng dòng:
   - **Cảnh báo** → nhắc nhưng vẫn cho chỉ định.
   - **Chặn** → không cho chỉ định.
   Hai cột loại trừ lẫn nhau (cơ chế sẵn có của `HIS.UC.Service`). Click header cột = tick/bỏ tick cả trang.
5. "Lưu (Ctrl S)": so diff trạng thái hiện tại với danh sách cặp ban đầu:
   - Tick thêm → `api/HisServiceExclusive/CreateList`
   - Đổi mức xử lý → `api/HisServiceExclusive/UpdateList`
   - Bỏ tick → `api/HisServiceExclusive/DeleteList` (danh sách ID bản ghi cặp)
   Lưu xong gọi `CheckServiceExclusiveManager.ResetData()` để các màn chỉ định nạp lại danh mục.

### Điều kiện nghiệp vụ
- Chưa tick radio dịch vụ gốc mà bấm Lưu → "Chưa chọn dịch vụ gốc ở lưới bên trái".
- Tick chính dịch vụ gốc ở lưới phải → "Không được khai báo dịch vụ loại trừ với chính nó", không lưu.
- Diff rỗng → "Không có thay đổi nào để lưu", không gọi API.
- Quan hệ là **đối xứng**: chỉ lưu 1 bản ghi cho mỗi cặp; lúc đọc và lúc kiểm tra đều tra 2 chiều.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE | View | Nguồn dữ liệu cả 2 lưới (SERVICE_CODE, SERVICE_NAME, SERVICE_TYPE_NAME) |
| HIS_SERVICE_TYPE | Table (cache BackendDataWorker) | 2 combo "Loại dịch vụ" lọc 2 lưới |
| HIS_SERVICE_EXCLUSIVE | Table — **chưa có trong MOS.EFMODEL.dll** | Bảng cặp dịch vụ loại trừ |

**Lưu ý kỹ thuật:** bảng `HIS_SERVICE_EXCLUSIVE` / view `V_HIS_SERVICE_EXCLUSIVE` và filter tương ứng chưa có trong `MOS.EFMODEL.dll` + `MOS.Filter.dll` đang bàn giao. FE tự khai lớp cục bộ trong thư viện `HIS.Desktop.Plugins.Library.CheckServiceExclusive/ADO/` (tên property trùng tuyệt đối với tên cột backend nên JSON map đúng). **Khi backend bàn giao DLL mới thì thay bằng `MOS.EFMODEL.DataModels.*` và `MOS.Filter.HisServiceExclusiveFilter`.**

Filter: `MOS.Filter.HisServiceViewFilter` (KEY_WORD, SERVICE_TYPE_ID) cho 2 lưới; `ADO/HisServiceExclusiveFilter.cs` (filter local, có `SERVICE_ID__OR__EXCLUSIVE_ID` để tra 2 chiều).

## 4. UI Layout

```
+--[Từ khóa..] [Loại dịch vụ: cbo] [Tìm kiếm (Ctrl D)]--+--[Từ khóa..] [Loại dịch vụ: cbo] [Tìm kiếm (Ctrl F)]--+
| GRID DỊCH VỤ GỐC (panelControl1 ← HIS.UC.Service)     | GRID DỊCH VỤ LOẠI TRỪ (panelControl2 ← HIS.UC.Service)|
| (o) | Mã dịch vụ | Tên dịch vụ | Loại dịch vụ         | [x] Cảnh báo | [x] Chặn | Mã | Tên | Loại dịch vụ    |
+--[ucPaging1]------------------------------------------+--[ucPaging2]------------------------------------------+
|                                                                                       [Lưu (Ctrl S)]         |
+--------------------------------------------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Service (instance 1) | panelControl1 | Lưới dịch vụ gốc, `isKeyChooseService = true` → cột `radioService` hoạt động |
| HIS.UC.Service (instance 2) | panelControl2 | Lưới dịch vụ loại trừ, `isKeyChooseService = false` → 2 cột tick hoạt động |
| Inventec.UC.Paging | ucPaging1 / ucPaging2 | Phân trang từng lưới |

**Ánh xạ cột mức xử lý** (không sửa `HIS.UC.Service` — có 24 project đang tham chiếu UC này):

| Cột hiển thị | Thuộc tính `ServiceADO` | HANDLE_TYPE_ID |
|---|---|---|
| Cảnh báo | `checkWarning` | 1 |
| Chặn | `checkServiceNotUse` | 2 |
| (không tick) | — | không có quan hệ |

Ba cờ `checkService` / `checkServiceNotUse` / `checkWarning` trong `UCService.cs` (dòng 440, 503, 657) đã loại trừ lẫn nhau sẵn nên không cần thêm xử lý. Cờ `checkService` không dùng ở màn này.

## 5. API Endpoints

| Action | URI (hằng trong `HisRequestUriStore` của thư viện) | Consumer | Filter/Payload |
|---|---|---|---|
| Nạp 2 lưới dịch vụ | `MOSHIS_SERVICE_GET_VIEW` = `api/HisService/GetView` | MosConsumer | `HisServiceViewFilter` |
| Lấy cặp của 1 dịch vụ | `MOSHIS_SERVICE_EXCLUSIVE_GET` = `api/HisServiceExclusive/Get` | MosConsumer | `HisServiceExclusiveFilter { SERVICE_ID__OR__EXCLUSIVE_ID, IS_ACTIVE }` — **chờ backend** |
| Nạp toàn bộ danh mục vào RAM | `MOSHIS_SERVICE_EXCLUSIVE_GET_VIEW` = `api/HisServiceExclusive/GetView` | MosConsumer | `HisServiceExclusiveFilter { IS_ACTIVE }` — **chờ backend** |
| Thêm cặp | `.../CreateList` | MosConsumer | `List<HIS_SERVICE_EXCLUSIVE>` — **chờ backend** |
| Đổi mức xử lý | `.../UpdateList` | MosConsumer | `List<HIS_SERVICE_EXCLUSIVE>` — **chờ backend** |
| Xóa cặp | `.../DeleteList` | MosConsumer | `List<long>` — **chờ backend** |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|---|---|---|
| (được mở từ menu Danh mục) | — | `Module`; có thể truyền thêm `V_HIS_SERVICE` để mở sẵn theo 1 dịch vụ |

### Thư viện dùng chung
`HIS.Desktop.Plugins.Library.CheckServiceExclusive` — tham chiếu bằng DLL HintPath `lib\HIS\HIS.Desktop.Plugins.Library.CheckServiceExclusive\...dll` (theo đúng mô hình `Library.CheckIcd`). Màn danh mục dùng lớp ADO + `HisRequestUriStore` + `ResetData()` của thư viện này.

### Đăng ký menu (ACS_MODULE)
| Cột | Giá trị |
|---|---|
| MODULE_LINK | `HIS.Desktop.Plugins.HisServiceExclusive` |
| MODULE_NAME | Dịch vụ không được chỉ định đồng thời |
| APPLICATION_ID | 2 |
| MODULE_GROUP_ID | 2 (cùng nhóm "Dịch vụ đi kèm" id 38, "Dịch vụ cùng cơ chế" id 635) |
| IS_ACTIVE / IS_VISIBLE / IS_LEAF | 1 / 1 / 1 |

Script: `PTTK\57452_db.sql` (bước 3).

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|---|---|---|
| 16/09/2026 | dangth + Claude | Tạo mới plugin theo thiết kế việc 57452. Clone khuôn 2 lưới từ `HIS.Desktop.Plugins.HisServiceSpeciality`, thay lưới phải bằng instance thứ 2 của `HIS.UC.Service`, thêm 2 cột mức xử lý Cảnh báo/Chặn, lưu theo diff Create/Update/Delete. |

## 9. Test Cases

- [ ] Mở màn hình: 2 lưới hiển thị, tìm kiếm + phân trang + lọc theo Loại dịch vụ chạy đúng ở cả 2 bên.
- [ ] Tick radio 1 dịch vụ bên trái → lưới phải tick sẵn đúng các dịch vụ đã khai và đúng mức xử lý.
- [ ] Tick **Chặn** cho 1 dịch vụ → Lưu → mở lại: giữ nguyên mức Chặn.
- [ ] Đổi từ **Chặn** sang **Cảnh báo** → Lưu → gọi `UpdateList`, mở lại đúng mức mới.
- [ ] Bỏ tick → Lưu → gọi `DeleteList`, mở lại không còn cặp đó.
- [ ] Tick đồng thời 2 cột trên cùng 1 dòng → chỉ giữ 1 (2 cột loại trừ lẫn nhau).
- [ ] Tick chính dịch vụ gốc ở lưới phải → báo lỗi, không lưu.
- [ ] Không thay đổi gì mà bấm Lưu → "Không có thay đổi nào để lưu".
- [ ] Chưa chọn dịch vụ gốc mà bấm Lưu → "Chưa chọn dịch vụ gốc ở lưới bên trái".
- [ ] Khai A→B rồi chọn radio B → lưới phải phải tick sẵn A (tra 2 chiều).
- [ ] Click header cột Cảnh báo / Chặn → tick/bỏ tick cả trang, riêng dòng trùng dịch vụ gốc không bị tick.
- [ ] Ctrl+D / Ctrl+F / Ctrl+S hoạt động.
- [ ] Backend chưa triển khai API → mở màn không văng lỗi, chỉ ghi log Warn.
