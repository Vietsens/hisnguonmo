# Import Danh Mục Nghề Nghiệp — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisImportCareer |
| Loại | Form |
| Mục đích | Import danh mục nghề nghiệp từ Excel; từ việc 2841 đọc format chuẩn QĐ 34/2020/QĐ-TTg (cột Cấp 1…Cấp 5 + Tên gọi), upsert cho phép ghi đè mã trùng |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (việc 2841, cập nhật 15/09 — file mẫu dạng phẳng)
1. Tải file mẫu `Tmp\Imp\IMPORT_CAREER.xlsx`. Sheet 1 `DanhMucNgheNghiep` chỉ có **2 dòng**: dòng 1 tiêu đề tiếng Việt, dòng 2 dòng tag `{%IMPORT%}.{PROP}`; **KHÔNG có dòng dữ liệu mẫu** (cơ chế đọc lấy mọi dòng sau dòng tag nên dòng ví dụ sẽ thành bản ghi rác). Ví dụ minh họa + hướng dẫn nằm ở sheet 2 `HuongDan` — import chỉ đọc sheet đầu (`GetWithCheck<CareerADO>(0)`).
2. **8 cột phẳng theo đúng thứ tự màn Danh mục**: Mã nghề | Tên nghề | Mã cấp 2 | Tên cấp 2 | Mã cấp 3 | Tên cấp 3 | Mã cấp 4 | Tên cấp 4. Mỗi dòng có dữ liệu = 1 nghề nghiệp (không còn dòng nhóm cấp cha).
3. Mã cấp: ưu tiên giá trị trong file; để trống → tự tách từ mã nghề (2/3/4 ký tự đầu, chỉ khi mã đúng 5 ký tự).
4. Tên cấp: ưu tiên giá trị trong file; để trống → tra theo mã cấp từ danh mục đã có trong hệ thống (`BuildLevelNameDictionariesFromDb` dựng từ `_ListCareers`); không tìm thấy → để trống (quản trị nhập tay ở màn danh mục).
   → Nhờ vậy **file 2 cột cũ (Mã + Tên nghề) vẫn import được** — tương thích ngược.
5. Label thống kê: "Tổng X dòng · Y dòng lỗi" (`SetStatisticText`, cập nhật cả khi lọc dòng lỗi/xóa dòng).
6. Validate: mã bắt buộc ≤ 5, tên bắt buộc ≤ 1000, tên cấp ≤ 1000. **BỎ check trùng mã** (yêu cầu ghi đè theo bản chuẩn).
7. Lưu: POST `api/HisCareer/ImportList` (upsert theo CAREER_CODE) rồi `BackendDataWorker.Reset<HIS_CAREER>()`.

### Tag mapping (Inventec.Common.ExcelImport)
Property nhận: `CAREER_CODE, CAREER_NAME, LEVEL2_CODE, LEVEL2_NAME, LEVEL3_CODE, LEVEL3_NAME, LEVEL4_CODE, LEVEL4_NAME` — đều là property của `HIS_CAREER` (EFMODEL mới), `ADO\CareerADO.cs` chỉ thêm `ERROR`.

**Lưu ý khi sinh lại file mẫu:** sheet PHẢI có `sheetFormatPr defaultRowHeight` / `row ht` — `Import.cs` bỏ qua toàn bộ sheet nếu `Cells[0,0].RowHeight = 0` (đọc ra rỗng, báo "Import thất bại").

## 3. EFMODEL / ADO

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_CAREER | Table | Danh mục nghề nghiệp |
| CareerADO | ADO | Kế thừa HIS_CAREER + LEVEL1/2/3/4 + ERROR — đọc excel + preview grid |
| CareerImportDTO | ADO | DTO plain gửi ImportList (đủ 8 trường, không phụ thuộc phiên bản EFMODEL client) |

## 4. UI Layout

```
[Tải file mẫu][Import][Dòng lỗi][Lưu (Ctrl S)]        Tổng X dòng · Y dòng lỗi
Grid preview: STT|Lỗi|Xóa|Mã nghề|Tên nghề|Mã C2|Tên C2|Mã C3|Tên C3|Mã C4|Tên C4
```

Thứ tự cột lưới xem trước = thứ tự cột file Excel = thứ tự cột lưới màn Danh mục nghề nghiệp.

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
| 15/09/2026 | nampp (Claude) | Việc 2841 (đổi theo review): **file mẫu chuyển sang 8 cột PHẲNG khớp màn Danh mục**, bỏ hoàn toàn dòng phân cấp + dòng dữ liệu ví dụ (chống bản ghi rác), ví dụ chuyển sang sheet 2 `HuongDan`. Bỏ `BuildLevelNameDictionaries`; thêm `BuildLevelNameDictionariesFromDb` + `ValueOrFallback`/`FindLevelNameInDb` (ưu tiên giá trị file, trống thì tự tách mã cấp / tra tên nhóm từ danh mục → file 2 cột cũ vẫn chạy). Nhãn thống kê → "Tổng X dòng · Y dòng lỗi" (`SetStatisticText`). `CareerADO` bỏ `LEVEL1_CODE`. Template thêm `sheetFormatPr/row ht` để `Import.cs` không bỏ qua sheet. |

## 9. Test Cases

- [ ] Tải file mẫu → sheet 1 có 8 cột, **chỉ 2 dòng** (tiêu đề + dòng tag), không có dòng dữ liệu; sheet 2 `HuongDan` có ví dụ
- [ ] Điền dữ liệu từ dòng 3 rồi Import → preview đúng số dòng, thống kê "Tổng X dòng · Y dòng lỗi"
- [ ] Điền đủ 8 cột → preview lấy nguyên tên cấp trong file
- [ ] Chỉ điền Mã + Tên nghề (mã 5 ký tự) → mã cấp tự tách 2/3/4 ký tự đầu; tên cấp tra từ danh mục đã có (trống nếu chưa có)
- [ ] Mã nghề 2 ký tự (04) → cột cấp trống, KHÔNG báo lỗi, import được
- [ ] Mã > 5 ký tự / thiếu tên → nút Dòng lỗi hiện đúng dòng, chặn Lưu
- [ ] File chứa mã đã tồn tại → KHÔNG báo trùng, import ghi đè
- [ ] Import xong không xuất hiện bản ghi lạ (dòng ví dụ không bị nhập)
