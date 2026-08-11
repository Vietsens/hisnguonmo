# Thiết Lập Dịch Vụ - Phạm Vi Chuyên Môn (HisServiceSpeciality) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisServiceSpeciality |
| Loại | UC (UserControl, module Thiết lập) |
| Mục đích | Gán quan hệ nhiều-nhiều Dịch vụ ↔ Phạm vi chuyên môn (bảng HIS_SERVICE_SPECIALITY) phục vụ cảnh báo thực hiện KCB theo phạm vi chứng chỉ hành nghề (PTTK 3142, việc 53436). |
| Người tạo | nampp + Claude (clone khuôn từ HIS.Desktop.Plugins.HisServiceRetyCat) |
| Ngày tạo | 05/08/2026 |
| Trạng thái | Hoàn thành FE — chờ backend api/HisServiceSpeciality/* |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở màn "Dịch vụ - Phạm vi chuyên môn" (module Thiết lập).
2. Combo "Chọn theo:" mặc định **Dịch vụ**:
   - Grid trái (Dịch vụ): cột radio hoạt động → chọn 1 dịch vụ.
   - Khi tick radio → gọi api/HisServiceSpeciality/Get (SERVICE_ID) → tự động check các Phạm vi chuyên môn đã gán bên grid phải (dòng đã gán đẩy lên đầu).
   - Grid phải (PVCM): cột checkbox hoạt động → tick/bỏ tick nhiều dòng (click header = check/uncheck cả trang).
3. Đổi combo "Chọn theo:" = **Phạm vi chuyên môn** → đảo vai trò: grid phải radio chọn 1 PVCM, grid trái checkbox chọn nhiều dịch vụ.
4. "Lưu (Ctrl S)": so sánh (diff) trạng thái check hiện tại với danh sách map ban đầu:
   - Phần tick thêm → api/HisServiceSpeciality/CreateList.
   - Phần bỏ tick → api/HisServiceSpeciality/DeleteList (list ID bản ghi map).
   - Vừa thêm vừa bỏ → gọi lần lượt cả 2 (PTTK yêu cầu).

### Điều kiện nghiệp vụ
- Chưa tick radio mà bấm Lưu → thông báo "Chưa chọn dịch vụ" / "Chưa chọn phạm vi chuyên môn".
- Không có thay đổi (diff rỗng) → thông báo "Không có thay đổi nào để lưu", không gọi API.
- Grid phải chỉ hiển thị PVCM đang hoạt động (IS_ACTIVE = 1).
- Grid đang ở chế độ radio thì click header cột checkbox không có tác dụng (chặn check-all).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE | View | Grid trái: SERVICE_CODE, SERVICE_NAME, SERVICE_TYPE_NAME |
| HIS_SPECIALITY | Table | Grid phải: SPECIALITY_CODE, SPECIALITY_NAME |
| HIS_SERVICE_TYPE | Table (cache BackendDataWorker) | Combo "Loại dịch vụ" lọc grid trái |
| HIS_SERVICE_SPECIALITY | Table (MOS.EFMODEL — có từ bản lib 06/08/2026) | Bảng map Dịch vụ ↔ PVCM |

Filter: `MOS.Filter.HisServiceViewFilter` (KEY_WORD, SERVICE_TYPE_ID), `MOS.Filter.HisSpecialityFilter` (KEY_WORD, IS_ACTIVE), `ADO/HisServiceSpecialityFilter.cs` (**filter local** — MOS.Filter bản 06/08/2026 chưa có HisServiceSpecialityFilter; khi lib bổ sung thì thay).

## 4. UI Layout

```
+--[Từ khóa..] [Loại dịch vụ: cbo] [Tìm kiếm (Ctrl D)]--+--[Từ khóa..] [Tìm kiếm (Ctrl F)]--+
| GRID DỊCH VỤ (panelControl1 ← HIS.UC.Service)          | GRID PVCM (panelControl2 ←        |
| (o) | [x] | Mã dịch vụ | Tên dịch vụ | Loại dịch vụ    |  HIS.UC.Speciality)               |
|                                                        | (o) | [x] | Mã | Tên              |
+--[ucPaging1]-------------------------------------------+--[ucPaging2]----------------------+
|                                    [Chọn theo: cbo▼]  [Lưu (Ctrl S)]                       |
+---------------------------------------------------------------------------------------------+
```
Mode "Dịch vụ": trái radio / phải checkbox. Mode "Phạm vi chuyên môn": trái checkbox / phải radio.

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Service | panelControl1 | Grid dịch vụ (radio/checkbox động qua isKeyChooseService) |
| HIS.UC.Speciality (**tạo mới**, clone từ HIS.UC.ReportRetyCat) | panelControl2 | Grid PVCM (radio/checkbox động qua isKeyChoose) |
| Inventec.UC.Paging ×2 | ucPaging1/ucPaging2 | Phân trang riêng từng grid |

## 5. API Endpoints

| Action | URI (HisRequestUriStore) | Consumer | Filter/Payload |
|--------|-----|----------|--------|
| Lấy map | MOSHIS_SERVICE_SPECIALITY_GET = api/HisServiceSpeciality/Get | MosConsumer | HisServiceSpecialityFilter (SERVICE_ID hoặc SPECIALITY_ID) |
| Thêm map | MOSHIS_SERVICE_SPECIALITY_CREATE_LIST = api/HisServiceSpeciality/CreateList | MosConsumer | List&lt;HIS_SERVICE_SPECIALITY&gt; (SERVICE_ID + SPECIALITY_ID) |
| Xóa map | MOSHIS_SERVICE_SPECIALITY_DELETE_LIST = api/HisServiceSpeciality/DeleteList | MosConsumer | List&lt;long&gt; (ID bản ghi map) |
| DS dịch vụ | MOSHIS_SERVICE_GET_VIEW = api/HisService/GetView | MosConsumer | HisServiceViewFilter (KEY_WORD, SERVICE_TYPE_ID) — GetRO paging |
| DS PVCM | MOSHIS_SPECIALITY_GET = api/HisSpeciality/Get | MosConsumer | HisSpecialityFilter (KEY_WORD, IS_ACTIVE=1) — GetRO paging |

**Chờ backend xác nhận**: 3 API HisServiceSpeciality (Get/CreateList/DeleteList) + kiểu tham số DeleteList (FE đang gửi List&lt;long&gt; ID bản ghi map — giống HisServiceRetyCat).

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| (được mở từ plugin khác) | Behavior parse args | Module (bắt buộc), V_HIS_SERVICE (tùy chọn — mở sẵn 1 dịch vụ và load map) |

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/08/2026 | nampp + Claude | Lib MOS mới có entity HIS_SERVICE_SPECIALITY → bỏ DTO local, dùng entity chính thức MOS.EFMODEL; filter vẫn local (MOS.Filter chưa có); rebuild toàn bộ với model mới |
| 05/08/2026 | nampp + Claude | Tạo mới plugin theo PTTK 3142 (clone khuôn HisServiceRetyCat + tạo mới UC HIS.UC.Speciality clone từ HIS.UC.ReportRetyCat). Fix so với khuôn gốc: dùng đúng filter local HisServiceSpecialityFilter (gốc dùng nhầm HisServiceRoomFilter), URI gom vào HisRequestUriStore, bỏ AutoMapper (AddRange trực tiếp), diff dùng HashSet O(n+m), bổ sung WaitingManager.Hide trước các nhánh return, đa ngôn ngữ Lang/Message vi+en + SetCaptionByLanguageKey (gốc hardcode), bỏ nút Nhập khẩu + menu chuột phải Copy/Paste, thêm SessionManager.ProcessTokenLost sau Lưu, IsEnable()=true |

## 9. Test Cases

### Chọn theo Dịch vụ (mặc định)
- [ ] Tick radio 1 dịch vụ → grid phải auto-check các PVCM đã gán, dòng gán lên đầu
- [ ] Tick thêm PVCM → Lưu → gọi CreateList, thông báo thành công
- [ ] Bỏ tick PVCM đã gán → Lưu → gọi DeleteList
- [ ] Vừa tick thêm vừa bỏ tick → Lưu → gọi DeleteList rồi CreateList
- [ ] Chưa tick radio → Lưu → "Chưa chọn dịch vụ"
- [ ] Không đổi gì → Lưu → "Không có thay đổi nào để lưu", không gọi API

### Chọn theo Phạm vi chuyên môn
- [ ] Đổi combo → 2 grid nạp lại, vai trò radio/checkbox đảo ngược
- [ ] Tick radio 1 PVCM → grid trái auto-check các dịch vụ đã gán
- [ ] Lưu diff tương tự chiều kia (CreateList/DeleteList theo SERVICE_ID)

### Tìm kiếm / phân trang
- [ ] Từ khóa + Loại dịch vụ + Tìm kiếm (Ctrl D) → grid trái lọc đúng, phân trang riêng
- [ ] Từ khóa + Tìm kiếm (Ctrl F) → grid phải lọc đúng (chỉ PVCM hoạt động)
- [ ] Chuyển trang khi đang chọn radio → dòng đã gán vẫn được check lại đúng

### Khác
- [ ] Click header cột checkbox → check/uncheck cả trang (chỉ ở grid đang mode checkbox)
- [ ] Ctrl D / Ctrl F / Ctrl S hoạt động đúng
- [ ] Đổi ngôn ngữ en → caption/nút/message tiếng Anh
