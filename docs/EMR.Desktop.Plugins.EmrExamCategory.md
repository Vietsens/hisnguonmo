# Phân loại Xét nghiệm & Cấu hình ghép cặp — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | EMR.Desktop.Plugins.EmrExamCategory |
| Loại | Form |
| Mục đích | Danh mục 11 loại xét nghiệm (Huyết học, Sinh hóa, X-Quang...) và cấu hình rule parse HIS_CODE để ghép cặp phiếu chỉ định-kết quả trong hồ sơ bệnh án điện tử |
| Mã việc | 14301 |
| Ngày tạo | 17/04/2026 |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Admin mở form → 2 grid: "Loại xét nghiệm" (trái) + "Cấu hình ghép cặp văn bản" (phải).
2. Grid trái CRUD danh mục loại xét nghiệm (Huyết học, Nội tiết, Miễn dịch, Sinh hóa máu, Nước tiểu, Vi sinh, Tế bào, X-Quang/CT, Siêu âm, Điện tim, Giải phẫu bệnh). Hỗ trợ kéo-thả đổi thứ tự, double-click sửa, tự động shift NUM_ORDER khi đổi.
3. Click 1 loại xét nghiệm bên trái → grid phải hiển thị rule thuộc loại đó. Admin CRUD rule: pattern, match type (PREFIX/CONTAINS/REGEX), key extractor.
4. Bấm "Lưu" → gọi `api/EmrExamCategory/SaveAll` gửi cả 2 grid trong 1 transaction.

### Điều kiện nghiệp vụ
- NEW category có temp ID âm (-1, -2...) để rule NEW có thể tham chiếu trước khi BE tạo.
- BE map lại EXAM_CATEGORY_ID cho rule sau khi create category mới.
- Xóa category sẽ cascade soft-delete rule thuộc loại đó (BE xử lý).
- UNCHANGED items bị bỏ qua, BE chỉ xử lý NEW/UPDATED/DELETED.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| EMR_EXAM_CATEGORY | Table | Danh mục loại xét nghiệm (CATEGORY_CODE, CATEGORY_NAME, NUM_ORDER, GROUP_CODE) |
| EMR_DOCUMENT_PAIR_RULE | Table | Rule parse HIS_CODE (PATTERN, MATCH_TYPE, KEY_EXTRACTOR, EXAM_CATEGORY_ID, NUM_ORDER) |

### Quan hệ chính
- EMR_DOCUMENT_PAIR_RULE → EMR_EXAM_CATEGORY (n-1, qua EXAM_CATEGORY_ID FK)

## 4. UI Layout

### Sơ đồ giao diện
```
+-----------------------------------------------------------------+
| Loại xét nghiệm           [+] | Cấu hình ghép cặp văn bản  [+] |
| STT | (X) | Mã | Tên | ...    | Ưu tiên | (X) | Pattern | ...  |
| 1   | X   | HUYETHOC | H.học  | 1       | X   | ^XQ    | PREFIX|
| 2   | X   | SINHHOA  | S.hóa  | 2       | X   | ^SA    | PREFIX|
| ...                           | ...                            |
+-----------------------------------------------------------------+
| [Làm mới] [Lưu]                                                 |
+-----------------------------------------------------------------+
```

### Controls chính
| Control | Vai trò |
|---------|---------|
| gridControlCat | Grid loại xét nghiệm với drag-drop reorder |
| gridControlRule | Grid rule, filter theo category đang chọn |
| btnSave / bbtnSave (Ctrl+S) | Lưu cả 2 grid qua SaveAll |
| btnRefresh / bbtnRefresh (Ctrl+R) | Reload từ server |

## 5. API Endpoints

| Action | URI | Consumer | Request |
|--------|-----|----------|---------|
| Lấy danh sách category | EmrRequestUriStore.EMR_EXAM_CATEGORY_GET | EmrConsumer | EmrExamCategoryFilter |
| Lấy danh sách rule | EmrRequestUriStore.EMR_DOCUMENT_PAIR_RULE_GET | EmrConsumer | EmrDocumentPairRuleFilter |
| **Lưu tất cả** | **EmrRequestUriStore.EMR_EXAM_CATEGORY_SAVE_ALL** | **EmrConsumer** | **EmrExamCategorySaveAllSDO** |

### Request SaveAll
```
EmrExamCategorySaveAllSDO {
    List<EMR_EXAM_CATEGORY> Categories   // ROW_STATE: NEW/UPDATED/DELETED
    List<EMR_DOCUMENT_PAIR_RULE> Rules
}
```

| ROW_STATE | Điều kiện ID | Thao tác BE |
|-----------|--------------|-------------|
| NEW | ID <= 0 (temp âm) hoặc NULL | Create, BE sinh ID mới |
| UPDATED | ID > 0 | Update |
| DELETED | ID > 0 | Soft delete (IS_DELETE=1) |
| UNCHANGED | — | Bỏ qua |

## 6. Dependencies

### Library Plugins
Không dùng library riêng.

### Inter-Plugin
Không mở plugin khác.

## 7. Print
Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 17/04/2026 | phuongnm | Tạo plugin. Grid dual CRUD, drag-drop reorder, SaveAll API với temp ID âm cho NEW category |
| 17/04/2026 | phuongnm | Thêm filter rule theo category đang chọn, ẩn cột EXAM_CATEGORY_ID trên grid rule |
| 17/04/2026 | phuongnm | Hỗ trợ DELETED qua SaveAll (BE cascade rule), bỏ gọi API Delete riêng |
| 17/04/2026 | phuongnm | Thêm Message.Lang.resx + ResourceMessage.cs, bỏ hardcode tiếng Việt |
| 17/04/2026 | phuongnm | Mở rộng SetCaptionByLanguageKey set tất cả labels/buttons/grid columns |
| 17/04/2026 | phuongnm | Thêm LogActionSuccess/Fail audit cho SaveAll |

## 9. Test Cases

### Tạo mới category
- [ ] Bấm [+] thêm category → nhập Code, Name → Lưu → thành công, reload thấy bản ghi mới có ID thực
- [ ] Thiếu CATEGORY_CODE → cảnh báo "Mã loại xét nghiệm không được để trống"
- [ ] Thiếu CATEGORY_NAME → cảnh báo "Tên loại xét nghiệm không được để trống"
- [ ] Trùng CATEGORY_CODE → cảnh báo trùng

### Sửa category
- [ ] Double-click cell → sửa → Lưu → BE Update

### Đổi thứ tự NUM_ORDER
- [ ] Kéo-thả item 1 → vị trí 3 → các item 2,3 shift lên → Lưu → đúng thứ tự trên DB
- [ ] Nhập NUM_ORDER trùng → tự động shift

### Xóa category
- [ ] Click icon (X) → confirm → đánh dấu DELETED → Lưu → BE soft-delete + cascade rule thuộc loại đó

### Tạo rule
- [ ] Chưa chọn category → bấm [+] rule → cảnh báo "Vui lòng chọn loại xét nghiệm trước khi thêm rule"
- [ ] Chọn category → thêm rule → EXAM_CATEGORY_ID tự gán
- [ ] Category NEW (temp ID âm) → thêm rule → Lưu → BE map rule đến ID thực của category

### Rule ghép cặp filter
- [ ] Click category bên trái → grid rule bên phải chỉ hiện rule thuộc category đó
- [ ] Chuyển category khác → grid rule refresh

### Validation rule
- [ ] Pattern trống → cảnh báo
- [ ] MATCH_TYPE khác PREFIX/CONTAINS/REGEX → cảnh báo
- [ ] MATCH_TYPE = REGEX + pattern không compile → cảnh báo "Pattern REGEX không hợp lệ"
- [ ] KEY_EXTRACTOR trống → cảnh báo
- [ ] NUM_ORDER rule trùng trong cùng category → cảnh báo

### Đa ngôn ngữ
- [ ] Đổi ngôn ngữ → labels/buttons/grid columns đổi theo
- [ ] Thông báo validation đổi theo ngôn ngữ

### Audit
- [ ] Save thành công → LogAction có entry "EmrExamCategoryForm.SaveAll.Username=X.Xử lý thành công"
- [ ] Save fail → LogAction có entry "Xử lý thất bại"
