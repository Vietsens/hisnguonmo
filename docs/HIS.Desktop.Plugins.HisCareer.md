# Danh Mục Nghề Nghiệp — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisCareer |
| Loại | Form |
| Mục đích | Quản trị danh mục nghề nghiệp (HIS_CAREER): thêm/sửa/khóa, tìm kiếm; từ việc 2841 hiển thị thêm thông tin cấp 2/3/4 theo QĐ 34/2020/QĐ-TTg |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Quản trị viên mở màn Danh mục nghề nghiệp → tìm kiếm/lọc → chọn dòng để sửa hoặc nhấn Thêm → nhập Mã (max 5 ký tự) + Tên (max 1000) → Lưu.

### Nghiệp vụ cấp 2/3/4 (việc 2841 — QĐ 34/2020/QĐ-TTg)
- Mỗi nghề cấp 5 (mã đúng 5 ký tự) mang 6 thông tin: mã/tên cấp 2, cấp 3, cấp 4. Mã cấp = 2/3/4 ký tự đầu của mã cấp 5.
- Khi nhập/rời ô Mã: 6 ô cấp (chỉ đọc) tự điền — mã cấp suy substring, tên cấp tra từ danh mục cache (`CareerLevelWorker.FindLevelName`); không tìm thấy → để trống.
- Mã ≠ 5 ký tự (bộ mã cũ 01…99): 6 ô để trống, không chặn.
- FE KHÔNG gửi 6 trường khi Lưu — BE tự auto-fill (double safety).
- Grid 6 cột cấp là cột UNBOUND (FieldName L2_CODE/L2_NAME/L3_CODE/L3_NAME/L4_CODE/L4_NAME), giá trị đọc qua reflection `CareerLevelWorker.GetProp` → tương thích cả MOS.EFMODEL cũ (chưa có field, hiện trống) lẫn mới.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_CAREER | Table | Danh mục nghề nghiệp; việc 2841 DB thêm LEVEL2/3/4_CODE + LEVEL2/3/4_NAME |

## 4. UI Layout

```
+--------------------------------------------------+----------------------+
| [Từ khóa] [Tìm (Ctrl F)]                         | Mã:        [       ] |
| Grid: STT|khóa|sửa|Mã|Tên|Mã C2|Tên C2|Mã C3|    | Tên:       [       ] |
|   Tên C3|Mã C4|Tên C4|Trạng thái|4 cột audit     | Mã cấp 2:  [ro     ] |
|                                                  | Tên cấp 2: [ro     ] |
|                                                  | Mã cấp 3:  [ro     ] |
|                                                  | ... cấp 4  [ro     ] |
| [paging]                                         | [Sửa][Thêm][Làm lại] |
+--------------------------------------------------+----------------------+
```

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy danh sách | api/HisCareer/Get | MosConsumer |
| Tạo mới | api/HisCareer/Create | MosConsumer |
| Cập nhật | api/HisCareer/Update | MosConsumer |
| Xóa / Khóa | api/HisCareer/Delete, ChangeLock | MosConsumer |

Tìm kiếm KEY_WORD: BE mở rộng tìm theo mã/tên cấp 2/3/4 (bàn giao Backend việc 2841).

## 6. Dependencies

Config toàn viện (đọc ở các combo chọn nghề nghiệp, KHÔNG đọc ở màn này): `MOS.HIS_CAREER.IS_SHOW_LEVEL_3`, `MOS.HIS_CAREER.IS_SHOW_LEVEL_4`.

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/09/2026 | nampp (Claude) | Việc 2841: grid thêm 6 cột cấp 2/3/4 (unbound + reflection), panel thêm 6 ô chỉ đọc tự điền theo mã (txtCareerCode_Leave), CareerLevelWorker.cs, Lang.vi/en 12 entries, ColumnAutoWidth=false; xóa licenses.licx stale khỏi csproj |
| 09/09/2026 | nampp (Claude) | Việc 2841 (chỉnh theo review): **3 ô TÊN cấp 2/3/4 cho phép nhập/sửa tay** (mã cấp vẫn khóa vì tách từ mã nghề) — auto-fill không đè tên đã gõ khi mã cấp không đổi (`FillOneLevel`); `UpdateDTOFromDataForm` gửi đủ 6 trường LEVEL lên BE (code thẳng property EFMODEL mới); validate maxlength 1000 cho 3 ô tên |

## 9. Test Cases

- [ ] Nhập mã 5 ký tự (VD 17310) → 6 ô cấp tự điền mã; tên cấp điền nếu danh mục chuẩn đã import
- [ ] Nhập mã ≠ 5 ký tự → 6 ô trống, lưu bình thường
- [ ] Grid hiển thị 6 cột cấp sau khi BE cập nhật EFMODEL + data; trước đó cột trống, KHÔNG lỗi
- [ ] Tìm "171" → ra các nghề thuộc cấp 3 "171" (cần BE mở rộng KEY_WORD)
