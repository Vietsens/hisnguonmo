# Import Danh Mục Nghề Nghiệp — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisImportCareer |
| Loại | Form |
| Mục đích | Import danh mục nghề nghiệp từ Excel; từ việc 2841 đọc format chuẩn QĐ 34/2020/QĐ-TTg (cột Cấp 1…Cấp 5 + Tên gọi), upsert cho phép ghi đè mã trùng |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (từ việc 2841)
1. Tải file mẫu `Tmp\Imp\IMPORT_CAREER.xlsx` (format mới: Cấp 1, Cấp 2, Cấp 3, Cấp 4, Cấp 5 (Mã nghề), Tên gọi nghề nghiệp; dòng 2 là dòng tag `{%IMPORT%}.{PROP}` map cột→property).
2. Chọn file: dòng CÓ giá trị cột Cấp 5 → bản ghi nghề chi tiết; dòng CHỈ có Cấp 2/3/4 → dictionary mã→tên nhóm cấp tương ứng (không tạo bản ghi); dòng Cấp 1 bỏ qua.
3. Mỗi nghề cấp 5: suy mã cấp (substring 2/3/4 ký tự đầu nếu mã đúng 5 ký tự) + tra tên nhóm từ dictionary; thiếu tên → để trống.
4. Label thống kê: "Cấp 2: X | Cấp 3: Y | Cấp 4: Z | Nghề cấp 5: T dòng".
5. Validate: mã bắt buộc max 5, tên bắt buộc max 1000. **BỎ check trùng mã** (yêu cầu ghi đè theo bản chuẩn).
6. Lưu: POST `api/HisCareer/ImportList` (upsert theo CAREER_CODE — API mới, Backend bàn giao) rồi `BackendDataWorker.Reset<HIS_CAREER>()`.

### Tag mapping (Inventec.Common.ExcelImport)
Property nhận: `LEVEL1_CODE, LEVEL2_CODE, LEVEL3_CODE, LEVEL4_CODE, CAREER_CODE, CAREER_NAME` (khai báo trong `ADO\CareerADO.cs`).

## 3. EFMODEL / ADO

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_CAREER | Table | Danh mục nghề nghiệp |
| CareerADO | ADO | Kế thừa HIS_CAREER + LEVEL1/2/3/4 + ERROR — đọc excel + preview grid |
| CareerImportDTO | ADO | DTO plain gửi ImportList (đủ 8 trường, không phụ thuộc phiên bản EFMODEL client) |

## 4. UI Layout

```
[Tải file mẫu][Import][Dòng lỗi][Lưu (Ctrl S)]  Cấp 2: X | Cấp 3: Y | Cấp 4: Z | Nghề cấp 5: T dòng
Grid preview: STT|Lỗi|Xóa|Mã nghề|Tên nghề|Mã C2|Tên C2|Mã C3|Tên C3|Mã C4|Tên C4
```

## 5. API Endpoints

| Action | URI | Consumer | Ghi chú |
|--------|-----|----------|---------|
| Lấy danh mục | api/HisCareer/Get | MosConsumer | |
| Import (mới, việc 2841) | api/HisCareer/ImportList | MosConsumer | Upsert theo CAREER_CODE — Backend bàn giao |
| Import (cũ, không dùng nữa) | api/HisCareer/CreateList | MosConsumer | Chặn trùng mã |

## 6. Dependencies

Inventec.Common.ExcelImport (đọc tag `{%IMPORT%}`), HIS.Desktop.LocalStorage.BackendData.

## 7. Print

Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/09/2026 | nampp (Claude) | Việc 2841: đọc format chuẩn QĐ 34 (6 cột), dictionary tên nhóm cấp 2/3/4, bỏ chặn trùng mã, grid preview 6 cột mới, label thống kê, gọi api/HisCareer/ImportList với CareerImportDTO, template IMPORT_CAREER.xlsx mới; xóa licenses.licx stale |

## 9. Test Cases

- [ ] Tải file mẫu → có 6 cột format chuẩn + dòng ví dụ nhóm 17
- [ ] Chọn file có dòng cấp 2/3/4 + cấp 5 → preview đủ mã/tên cấp; thống kê X/Y/Z/T đúng
- [ ] File chứa mã đã tồn tại → KHÔNG báo trùng, import ghi đè (cần BE ImportList)
- [ ] Dòng thiếu mã/tên hoặc mã >5 ký tự → nút Dòng lỗi hiển thị đúng, không cho Lưu
- [ ] File template cũ 2 cột (Mã/Tên) → vẫn import được, cột cấp trống
