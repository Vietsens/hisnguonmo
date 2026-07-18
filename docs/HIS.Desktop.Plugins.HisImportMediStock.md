# Nhập Khẩu Danh Mục Kho (HisImportMediStock) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisImportMediStock |
| Loại | Form (FormBase) |
| Mục đích | Nhập khẩu hàng loạt danh mục kho (HIS_MEDI_STOCK) từ file Excel mẫu; kiểm tra dữ liệu; lưu qua CreateList |

## 2. Quy Trình Nghiệp Vụ

1. "Tải file mẫu" → copy `Tmp/Imp/IMPORT_MEDI_STOCK.xlsx` ra máy người dùng.
2. "Nhập khẩu" → đọc Excel bằng `Inventec.Common.ExcelImport.Import.GetWithCheck<MediStockImportADO>(0)`.
   - Cột Excel ánh xạ theo **tag trong ô header dòng 2**: `{%IMPORT%}.{TênProperty}` (khớp theo TÊN, không theo vị trí).
3. `addMediStockToProcessList` map từng dòng → ADO, validate (mã khoa, mã kho trùng, độ dài…), các cột cờ "x" → cờ tương ứng.
4. Lưới hiển thị (grid) + lọc "Dòng lỗi/không lỗi". Chỉ Lưu được khi không còn dòng lỗi.
5. "Lưu" → `api/HisMediStock/CreateList` (List&lt;HisMediStockSDO&gt;).

### PTTK_42516 — Bổ sung 2 loại kho
- Thêm 2 cột nhập: **Là kho điều trị** (`IS_TREATMENT_STOCK_STR`) và **Là kho thuốc ngoại trú** (`IS_OUTPATIENT_STOCK_STR`).
- Ô đánh dấu bằng ký tự `"x"` → set `IS_TREATMENT_STOCK` / `IS_OUTPATIENT_STOCK` = 1 (giống các cờ hiện có như "Là tủ trực").
- Hiển thị 2 cột checkbox trên lưới: `TREATMENT_STOCK`, `OUTPATIENT_STOCK`.

## 3. EFMODEL / ADO

| Đối tượng | Ghi chú |
|-----------|---------|
| `MediStockImportADO : V_HIS_MEDI_STOCK` | Thêm `IS_TREATMENT_STOCK_STR`, `IS_OUTPATIENT_STOCK_STR` (string map Excel) + `TREATMENT_STOCK`, `OUTPATIENT_STOCK` (bool hiển thị lưới) |
| HIS_MEDI_STOCK / V_HIS_MEDI_STOCK | **Cần backend gencode IS_TREATMENT_STOCK, IS_OUTPATIENT_STOCK** (ADO kế thừa V_HIS_MEDI_STOCK) |

> ⚠️ **Phụ thuộc backend**: cùng 2 cột như plugin HisMediStock. Chưa có → lỗi build tại các dòng `mateAdo.IS_TREATMENT_STOCK` / `IS_OUTPATIENT_STOCK`.

## 4. File Excel Mẫu (IMPORT_MEDI_STOCK.xlsx)

- Vị trí runtime: `{StartupPath}/Tmp/Imp/IMPORT_MEDI_STOCK.xlsx` (đã cập nhật cả `histest\x64\...` và `HIS.Desktop\bin\Debug\...`).
- Dòng 1 = caption tiếng Việt; Dòng 2 = tag `{%IMPORT%}.{...}`; Dòng 3+ = dữ liệu.
- PTTK_42516 thêm 2 cột (U, V) dạng inline-string:
  - U1 "Là kho điều trị" / U2 `{%IMPORT%}.{IS_TREATMENT_STOCK_STR}`
  - V1 "Là kho thuốc ngoại trú" / V2 `{%IMPORT%}.{IS_OUTPATIENT_STOCK_STR}`
- Đã kiểm chứng đọc lại bằng DevExpress.Spreadsheet (đúng engine plugin dùng).

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu hàng loạt | api/HisMediStock/CreateList | MosConsumer |

## 6-7. Dependencies / Print
- `Inventec.Common.ExcelImport`. Không in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/07/2026 | phuongnm | PTTK_42516: Thêm 2 cột import "Là kho điều trị"/"Là kho thuốc ngoại trú" (IS_TREATMENT_STOCK_STR/IS_OUTPATIENT_STOCK_STR → cờ IS_TREATMENT_STOCK/IS_OUTPATIENT_STOCK). Thêm field ADO + bool hiển thị, parse trong addMediStockToProcessList, checkNull trong Refresh, 2 cột lưới, 2 cột tag trong file Excel mẫu (2 bản copy). |

## 9. Test Cases

- [ ] Tải file mẫu → có 2 cột mới "Là kho điều trị", "Là kho thuốc ngoại trú".
- [ ] Nhập file có "x" ở 2 cột mới → lưới tick đúng 2 cột; Lưu → DB set IS_TREATMENT_STOCK / IS_OUTPATIENT_STOCK.
- [ ] Ô 2 cột mới khác "x" và khác rỗng → báo lỗi dòng.
- [ ] Dòng chỉ có 2 cột mới (không có cột khác) → không bị loại nhầm (checkNull đã tính 2 cột).
