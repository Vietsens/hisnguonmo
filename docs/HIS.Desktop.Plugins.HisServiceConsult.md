# HIS.Desktop.Plugins.HisServiceConsult — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisServiceConsult |
| Loại | Form (popup) |
| Mục đích | Ghi nhận kết quả tư vấn dịch vụ cho mỗi hồ sơ điều trị: người tư vấn, các gói tư vấn, loại kết quả (Không sử dụng / Có khả năng / Đồng ý), thời gian, lý do, mô tả. Dùng để phân loại bệnh nhân và tính công cho nhân viên tư vấn. |
| Người tạo | Trần Hải Đăng |
| Ngày tạo | 28/05/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng đang ở màn `HIS.Desktop.Plugins.ExamServiceReqExecute` (Xử lý yêu cầu khám) bấm nút "KQ tư vấn DV".
2. Plugin `HisServiceConsult` mở popup, nhận `TreatmentId` đầu vào.
3. Khi `Load` plugin gọi `POST /api/HisServiceConsult/GetByTreatment` với `treatmentId`.
   - Nếu trả về `HisServiceConsultSDO` có dữ liệu → Mode **Edit** (fill thông tin vào form).
   - Nếu trả về null → Mode **Create** (mặc định người tư vấn = tài khoản đang đăng nhập, thời gian = hiện tại).
4. Người dùng nhập/sửa: Người tư vấn, danh sách gói (multi-check), Kết quả tư vấn, Thời gian, Lý do, Mô tả.
5. Bấm **Lưu (Ctrl+S)**:
   - Mode Create → `POST /api/HisServiceConsult/Create` với `HisServiceConsultCreateTDO`.
   - Mode Edit → `POST /api/HisServiceConsult/Update` với `HisServiceConsultUpdateTDO`.
   - Thành công → hiện "Xử lý thành công", form chuyển sang Mode Edit dùng SDO trả về.
   - Thất bại → hiện "Xử lý thất bại".
6. Bấm **Làm lại (Ctrl+R)** → revert toàn bộ về dữ liệu ban đầu khi mở form.

### Điều kiện nghiệp vụ
- Mỗi `TREATMENT_ID` chỉ có tối đa 1 bản ghi `HIS_SERVICE_CONSULT` active (BR1: 1 treatment ↔ 1 consult).
- Khi Mode Edit: TREATMENT_ID không được phép đổi (giữ nguyên trong DB).
- `ConsultantLoginName` + `ConsultantUserName` luôn được FE lookup từ cache `ACS_USER` theo `LOGINNAME` đang chọn trên combobox để gửi snapshot lên BE.
- Nút "KQ tư vấn DV" (ở plugin cha `ExamServiceReqExecute`) bị disable nếu y lệnh khám đã kết thúc (`HIS_SERVICE_REQ_STT.ID__HT`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_CONSULT | Table (mới) | Ghi nhận kết quả tư vấn theo treatment |
| HIS_CONSULT_PACKAGE | Table (mới) | Quan hệ n-n giữa consult và gói (HIS_PACKAGE) |
| HIS_CONSULT_RESULT_TYPE | Table (mới) | Danh mục loại kết quả tư vấn |
| HIS_PACKAGE | Table | Danh sách gói dịch vụ — bind vào grid "Gói tư vấn" |
| ACS_USER | Table | Lookup người tư vấn (LOGINNAME, USERNAME) |

### Quan hệ chính
- HIS_TREATMENT (1) ↔ HIS_SERVICE_CONSULT (0..1) qua `TREATMENT_ID` (UNIQUE).
- HIS_SERVICE_CONSULT (1) → HIS_CONSULT_PACKAGE (n) → HIS_PACKAGE (1).
- HIS_SERVICE_CONSULT.CONSULT_RESULT_TYPE_ID → HIS_CONSULT_RESULT_TYPE.ID.
- HIS_SERVICE_CONSULT.CONSULTANT_LOGINNAME ↦ ACS_USER.LOGINNAME (giá trị logic, KHÔNG có FK constraint).

> Lưu ý: Các entity mới (HIS_SERVICE_CONSULT, HIS_CONSULT_PACKAGE, HIS_CONSULT_RESULT_TYPE) đã có trong `MOS.EFMODEL.DataModels` (BE phát hành). FE dùng trực tiếp qua `using MOS.EFMODEL.DataModels;`.

## 4. UI Layout

### Sơ đồ giao diện
```
+----------------------------------------------------------+
| [Từ khóa tìm kiếm______________________________________] |
+----------------------------------------------------------+
| [ ] | STT | Mã gói | Tên gói                              |
| [ ] |  1  | GT     | Gói vật tư                           |
| [v] |  2  | abc    | Gói abc                              |
| ...                                                       |
+----------------------------------------------------------+
| Người tư vấn: [txtLogin][cboUser] Kết quả tư vấn: [cbo]  |
|                                  Thời gian: [dteConsult] |
| Lý do:   [memoEdit reason ...........................]   |
| Mô tả:   [memoEdit description ......................]   |
|                          [Làm lại (Ctrl R)] [Lưu (Ctrl S)]|
+----------------------------------------------------------+
```

### Control map
| Control | Tên | Mô tả |
|---------|-----|-------|
| TextEdit | `txtKeyword` | Tìm kiếm tự động trên grid theo `PACKAGE_CODE` / `PACKAGE_NAME` |
| GridControl | `gridControlPackage` | Grid multi-select gói. Cột: Checkbox, STT, Mã gói, Tên gói |
| TextEdit | `txtConsultantLoginname` | Hiển thị LOGINNAME đã chọn (readonly) |
| GridLookUpEdit | `cboConsultantUser` | Combo người tư vấn từ `ACS_USER` (IS_ACTIVE=1). Display = USERNAME, Value = LOGINNAME, 2 cột "Tên đăng nhập" + "Họ tên" |
| GridLookUpEdit | `cboResultType` | Combo loại kết quả tư vấn. Display = CONSULT_RESULT_TYPE_NAME, Value = ID, 2 cột Mã + Tên |
| DateEdit | `dteConsultTime` | Thời gian tư vấn, format dd/MM/yyyy HH:mm |
| MemoEdit | `txtReason` | Lý do (≤ 2000 ký tự) |
| MemoEdit | `txtDescription` | Mô tả (≤ 2000 ký tự) |
| SimpleButton | `btnReset` | Làm lại (Ctrl+R) — revert về dữ liệu ban đầu |
| SimpleButton | `btnSave` | Lưu (Ctrl+S) — Create/Update theo mode |

### Trường bắt buộc (caption Maroon)
- Người tư vấn
- Kết quả tư vấn

## 5. API Endpoints

| Action | URI | Consumer | Input | Output |
|--------|-----|----------|-------|--------|
| Lấy danh mục kết quả tư vấn | `api/HisConsultResultType/Get` | MosConsumer | `HisConsultResultTypeFilter` | `List<HIS_CONSULT_RESULT_TYPE>` |
| Lấy SDO theo treatment | `api/HisServiceConsult/GetByTreatment` | MosConsumer | `long treatmentId` (truyền thẳng) | `HisServiceConsultSDO` (null nếu chưa có) |
| Tạo mới kết quả tư vấn | `api/HisServiceConsult/Create` | MosConsumer | `HIS_SERVICE_CONSULT` (entity + nav `HIS_CONSULT_PACKAGE`) | `HIS_SERVICE_CONSULT` (entity sau insert) |
| Sửa kết quả tư vấn | `api/HisServiceConsult/Update` | MosConsumer | `HIS_SERVICE_CONSULT` (entity với ID set + nav `HIS_CONSULT_PACKAGE` mong muốn) | `HIS_SERVICE_CONSULT` (entity sau update) |

URI tập trung tại `HisRequestUriStore.cs`.

> **Lưu ý**: BE Controller `Create`/`Update` bind thẳng `ApiParam<HIS_SERVICE_CONSULT>` — không có TDO trung gian. FE gửi entity `HIS_SERVICE_CONSULT` trực tiếp, navigation collection `HIS_CONSULT_PACKAGE` được EF auto-insert. FE pre-fill `IS_ACTIVE=1`, `IS_DELETE=0` cho cả parent + mỗi child package vì BE decorator chỉ chạy trên parent entity, không chạy trên children collection.

## 6. Dependencies

### Library Plugins
Không sử dụng library plugin nào (plugin nội bộ, không in, không ký số).

### Inter-Plugin
| Plugin gọi | Khi nào | Args truyền vào (parse trong Behavior) |
|------------|---------|------------------------------------------|
| `HIS.Desktop.Plugins.ExamServiceReqExecute` | User bấm nút "KQ tư vấn DV" tại màn xử lý khám | `Module moduleData`, `long treatmentId`, `DelegateSelectData delegateSelect` (tuỳ chọn) |

### Cache
- `BackendDataWorker.Get<ACS_USER>()` — lookup người tư vấn.
- `BackendDataWorker.Get<HIS_PACKAGE>()` — bind grid gói.

## 7. Print
Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | Trần Hải Đăng | Tạo mới plugin theo PTTK Việc 2681 mục 3.2 — popup ghi nhận kết quả tư vấn dịch vụ, hỗ trợ Mode Create/Edit, 4 API mới (`/api/HisConsultResultType/Get`, `/api/HisServiceConsult/GetByTreatment`, `/api/HisServiceConsult/Create`, `/api/HisServiceConsult/Update`). |
| 28/05/2026 | Trần Hải Đăng | Xoá 3 placeholder entity (`HIS_SERVICE_CONSULT`, `HIS_CONSULT_PACKAGE`, `HIS_CONSULT_RESULT_TYPE`) — chuyển sang dùng trực tiếp `MOS.EFMODEL.DataModels` sau khi BE phát hành. Fix lỗi CS0019 do xung đột type giữa placeholder và MOS.EFMODEL. |
| 01/06/2026 | Trần Hải Đăng | Refactor Create/Update: BE Controller bind thẳng `ApiParam<HIS_SERVICE_CONSULT>` (không có TDO). Xoá `HisServiceConsultCreateTDO` + `HisServiceConsultUpdateTDO`, gửi/nhận entity `HIS_SERVICE_CONSULT` trực tiếp. FE pre-fill `IS_ACTIVE=1` / `IS_DELETE=0` trên parent + mỗi child `HIS_CONSULT_PACKAGE` để bù BE decorator không chạy trên children collection. |

## 9. Test Cases

### Mở popup
- [ ] Plugin cha truyền đầy đủ `TreatmentId` → form hiện, gọi `GetByTreatment`.
- [ ] `TreatmentId` không hợp lệ (= 0 hoặc null) → Behavior trả null, không mở form.

### Mode Create (chưa có bản ghi)
- [ ] Mặc định người tư vấn = tài khoản đang đăng nhập (txtConsultantLoginname + cboConsultantUser).
- [ ] Mặc định thời gian = hiện tại.
- [ ] Nhập đủ → bấm Lưu → API Create trả về SDO khác null → hiện "Xử lý thành công", form chuyển sang Mode Edit.
- [ ] Save thất bại → hiện "Xử lý thất bại".

### Mode Edit (đã có bản ghi)
- [ ] Form load: Người tư vấn, Kết quả tư vấn, Thời gian, Lý do, Mô tả được fill đúng.
- [ ] Grid gói: các gói đã chọn được tick và sắp xếp lên đầu danh sách.
- [ ] Sửa rồi Lưu → API Update → trả về SDO mới → hiện "Xử lý thành công".

### Validate
- [ ] Để trống Người tư vấn → icon warning + ErrorText "Trường dữ liệu bắt buộc".
- [ ] Để trống Kết quả tư vấn → icon warning + ErrorText "Trường dữ liệu bắt buộc".
- [ ] Lý do > 2000 ký tự → warning "Lý do không được vượt quá 2000 ký tự".
- [ ] Mô tả > 2000 ký tự → warning "Mô tả không được vượt quá 2000 ký tự".
- [ ] Không tick gói nào → popup "Vui lòng chọn gói dịch vụ", chặn lưu.

### Tìm kiếm & multi-select
- [ ] Gõ keyword → grid lọc tự động theo Mã gói / Tên gói.
- [ ] Tick/unxóa checkbox trên grid → giữ trạng thái khi lưu.

### Phím tắt
- [ ] Ctrl+S → kích hoạt SaveProcess.
- [ ] Ctrl+R → kích hoạt ResetForm (revert về dữ liệu ban đầu khi mở form).

### Combo người tư vấn
- [ ] Sổ xuống hiển thị 2 cột "Tên đăng nhập" (LOGINNAME) và "Họ tên" (USERNAME).
- [ ] Gõ text để tìm kiếm (PopupFilterMode = Contains).
- [ ] Chọn user → `txtConsultantLoginname` tự động cập nhật LOGINNAME.

### Combo kết quả tư vấn
- [ ] Sổ xuống hiển thị 2 cột "Mã" và "Tên hiển thị" theo `NUM_ORDER` tăng dần.
- [ ] Display value = CONSULT_RESULT_TYPE_NAME, Value = ID.
