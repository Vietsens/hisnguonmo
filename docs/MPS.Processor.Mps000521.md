# MPS000521 — Thẻ Barcode Tài Khoản Lấy Mẫu — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000521` (+ `MPS.Processor.Mps000521.PDO`) |
| Mẫu in | Thẻ nhân viên 85,6 × 54 mm (in trên A4, viền cắt nét đứt): tên đơn vị, họ tên, tài khoản, mã NV, khoa, barcode |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Gọi từ | `HIS.Desktop.Plugins.EmpUser` (nút In barcode trên dòng) |
| Trạng thái | Đang phát triển (việc 60557, mã 521 cần nhóm MPS chốt) |

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000521PDO(List<Mps000521ADO> employees)`. Mỗi `Mps000521ADO`: `LOGINNAME`, `USERNAME`, `EMPLOYEE_CODE`, `DEPARTMENT_NAME`, `BARCODE_TEXT` (GĐ1 = `TK-` + LOGINNAME). Danh sách để sau này in nhiều thẻ một lượt mà không đổi PDO; EmpUser hiện truyền 1 phần tử.

## 3. Key / Bảng Đổ Vào Template

| Key | Nguồn |
|-----|-------|
| `<#LOGINNAME;>`, `<#USERNAME;>`, `<#EMPLOYEE_CODE;>`, `<#DEPARTMENT_NAME;>`, `<#BARCODE_TEXT;>` | Nhân viên đầu tiên của danh sách |
| `<#ORGANIZATION_NAME;>` | Key chung của ProcessorBase |
| Bảng `Employees` (band `__Employees__`, `<#Employees.LOGINNAME;>`...) | Toàn bộ danh sách (mẫu in nhiều thẻ) |

**Barcode:** ảnh giữ chỗ trong mẫu đặt tên `<#FlFuncCreateBarcode(<#BARCODE_TEXT;>;500;100)>` (hàm mẫu chung, CODE128, có chữ dưới vạch). Processor không có code barcode. Độ rộng 500 vẽ được tối đa 41 ký tự; dài hơn thì hàm mẫu bỏ qua (ảnh giữ chỗ để nguyên). Trên thẻ 85,6 mm, chuỗi dài hơn khoảng 23 ký tự thì vạch mảnh, cần in thử.

## 4. Mẫu

- File gốc: `PTTK\60557\mau_in\TheBarcodeTaiKhoanLayMau.xlsx`, sinh bằng `PTTK\60557\mau_in\TaoMauThe.cs` (FlexCel 5.7).
- Triển khai: `x64\Tmp\Mps\Mps000521\TheBarcodeTaiKhoanLayMau.xlsx` (giữ đúng 1 file trong thư mục). Viện tự chỉnh khổ giấy theo máy in thẻ.
- Cần 1 dòng SAR_PRINT_TYPE mã `Mps000521` (tạo ở màn Biểu in hoặc script), đăng nhập lại HIS sau khi thêm.

## 5. Build & Deploy

- Build PDO trước, rồi processor (ProjectReference PDO). PDO chưa có trong `lib\MPSv2\MPS.PDO`: máy khác build EmpUser cần PDO được đưa vào lib.
- Deploy: processor → `x64\Plugins\MpsProcessor\MPS.Processor.Mps000521.dll`; PDO → `x64\ReferencedAssemblies\MPS.Processor.Mps000521.PDO.dll`; mẫu như mục 4. Khởi động lại HIS (danh sách processor nạp lúc mở).
- Namespace phải đúng `MPS.Processor.Mps000521` (ProcessorFactory lấy mã in từ namespace).

## 6. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/10/2026 | dangth | **Việc 60557** — Tạo mới: PDO danh sách nhân viên, processor đổ key + bảng `Employees`, mẫu thẻ có barcode bằng `FlFuncCreateBarcode` |

## 7. Test Cases

- [ ] In thẻ tài khoản thường: đủ họ tên, tài khoản, mã NV, khoa, barcode quét ra `TK-` + tài khoản
- [ ] Tài khoản dài (≥ 23 ký tự): kiểm độ mảnh vạch khi in thật
- [ ] Nhân viên không có khoa: ô khoa trống, không lỗi
