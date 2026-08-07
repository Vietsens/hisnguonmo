# Danh Mục Phạm Vi Chuyên Môn (HisSpeciality) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisSpeciality |
| Loại | Form |
| Mục đích | Quản lý danh mục "Phạm vi chuyên môn" (bảng HIS_SPECIALITY): thêm, sửa, xóa, khóa/mở khóa. Phục vụ nghiệp vụ cảnh báo thực hiện KCB theo phạm vi chứng chỉ hành nghề (PTTK 3142, việc 53436). |
| Người tạo | (plugin có sẵn, chỉnh theo PTTK 3142) |
| Ngày sửa | 05/08/2026 |
| Trạng thái | Hoàn thành FE — chờ backend Lock/Unlock |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở "Phạm vi chuyên môn" từ menu Danh mục chung.
2. Tìm kiếm theo từ khóa (Mã/Tên) → grid trái hiển thị danh sách có phân trang.
3. Thêm mới: nhập Mã + Tên bên phải → "Thêm (Ctrl N)".
4. Sửa: click dòng trên grid → dữ liệu đổ vào form phải (Mã ReadOnly) → "Sửa (Ctrl S)".
5. Xóa: click icon X đỏ trên dòng → confirm → gọi Delete theo ID.
6. Khóa/Mở khóa: click icon khóa trên dòng → confirm → gọi Lock (IS_ACTIVE=0) / Unlock (IS_ACTIVE=1).

### Điều kiện nghiệp vụ
- Mã, Tên bắt buộc nhập (validation maroon + dxValidationProvider).
- Bản ghi đang khóa không cho Sửa (btnEdit disable).
- Trạng thái hiển thị: "Hoạt động" (xanh) / "Tạm khóa" (đỏ) theo IS_ACTIVE.
- Trường BHYT_LIMIT (Giới hạn CP BHYT) bị ẩn theo PTTK 3142 (ẩn runtime cả ô nhập lẫn cột grid, không xóa code cũ).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SPECIALITY | Table | Danh mục phạm vi chuyên môn (ID, SPECIALITY_CODE, SPECIALITY_NAME, BHYT_LIMIT, IS_ACTIVE, audit) |

Filter: `MOS.Filter.HisSpecialityFilter` (KEY_WORD, ID, IS_ACTIVE...).

Lưu ý: HIS_SPECIALITY nằm trong cache BackendDataWorker (EmpUser dùng) → sau Create/Update/Delete/Lock/Unlock có gọi `BackendDataWorker.Reset<HIS_SPECIALITY>()`.

## 4. UI Layout

```
+---------------------------------------------------------------+----------------------+
| [Từ khóa........] [Tìm kiếm (Ctrl F)]                         |  Mã:  [_________]    |
+---------------------------------------------------------------+  Tên: [_________]    |
| STT | 🔒 | ❌ | Mã | Tên | Trạng thái | TG tạo | Người tạo    |                      |
|     |    |    |    |     | (xanh/đỏ)  | TG sửa | Người sửa    |  [Sửa (Ctrl S)]      |
+---------------------------------------------------------------+  [Thêm (Ctrl N)]     |
| [ucPaging]                                                     |  [Làm lại (Ctrl R)]  |
+---------------------------------------------------------------+----------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| Inventec.UC.Paging | dưới grid | Phân trang server-side |

## 5. API Endpoints

| Action | URI | Consumer | Filter/Payload |
|--------|-----|----------|--------|
| Lấy danh sách | HisRequestUriStore.MOSHIS_SPECIALITY_GET = api/HisSpeciality/Get | MosConsumer | HisSpecialityFilter (KEY_WORD) |
| Tạo mới | MOSHIS_SPECIALITY_CREATE = api/HisSpeciality/Create | MosConsumer | HIS_SPECIALITY |
| Cập nhật | MOSHIS_SPECIALITY_UPDATE = api/HisSpeciality/Update | MosConsumer | HIS_SPECIALITY (theo ID) |
| Xóa | MOSHIS_SPECIALITY_DELETE = api/HisSpeciality/Delete | MosConsumer | long ID |
| Khóa | MOSHIS_SPECIALITY_LOCK = api/HisSpeciality/Lock | MosConsumer | long ID → IS_ACTIVE=0 |
| Mở khóa | MOSHIS_SPECIALITY_UNLOCK = api/HisSpeciality/Unlock | MosConsumer | long ID → IS_ACTIVE=1 |

## 6. Dependencies

Không mở plugin khác. Được plugin HIS.Desktop.Plugins.HisServiceSpeciality dùng chung dữ liệu HIS_SPECIALITY (qua API).

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/08/2026 | nampp + Claude | PTTK 3142: đổi ChangeLock → Lock/Unlock riêng (icon khóa gọi đúng chiều), thêm confirm xóa + WaitingManager, ẩn BHYT_LIMIT (ô nhập + cột grid), Mã ReadOnly khi Sửa, thêm MessageManager.Show sau Lock/Unlock, BackendDataWorker.Reset sau thao tác ghi, sửa caption resx vi/en ("Mã"/"Tên"/"Phạm vi chuyên môn"/"Tìm kiếm (Ctrl F)"), bỏ licenses.licx khỏi csproj |

## 9. Test Cases

### Tạo mới
- [ ] Nhập Mã + Tên → Thêm → grid refresh, thông báo thành công
- [ ] Bỏ trống Mã hoặc Tên → validation warning tại control

### Sửa
- [ ] Click dòng → Mã ReadOnly, Tên sửa được → Sửa → grid cập nhật
- [ ] Dòng đang "Tạm khóa" → nút Sửa disable

### Xóa
- [ ] Click X đỏ → hiện confirm → Yes → xóa, grid refresh; No → không xóa

### Khóa / Mở khóa
- [ ] Dòng "Hoạt động" click icon → confirm khóa → gọi api/HisSpeciality/Lock → trạng thái "Tạm khóa" đỏ
- [ ] Dòng "Tạm khóa" click icon → confirm bỏ khóa → gọi api/HisSpeciality/Unlock → "Hoạt động" xanh

### Nghiệp vụ đặc biệt
- [ ] Không còn ô "Giới hạn CPBHYT" và cột "Giới hạn BHYT chi trả" trên giao diện
- [ ] Sau thêm/sửa, combo Phạm vi chuyên môn ở màn EmpUser thấy dữ liệu mới (cache đã reset)
