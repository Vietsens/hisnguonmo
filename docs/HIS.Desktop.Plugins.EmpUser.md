# Tài Khoản / Nhân Viên — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.EmpUser |
| Loại | Form (`frmEmpUser : FormBase`) |
| Mục đích | Quản lý nhân viên và tài khoản ACS tương ứng: thêm/sửa, khóa/mở khóa, đặt lại mật khẩu, gán vai trò, nhập khẩu, in thẻ barcode tài khoản lấy mẫu |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### In barcode tài khoản (việc 60557)
- Nút **In barcode** (biểu tượng máy in) trên mỗi dòng của lưới nhân viên, ngay sau các nút khóa / đặt lại mật khẩu / gán vai trò.
- In thẻ MPS000521 cho nhân viên của dòng: họ tên, tài khoản, mã nhân viên, khoa, barcode CODE128 của chuỗi `TK-` + LOGINNAME.
- Thẻ dùng để đăng nhập máy quét ở màn Kết nối thiết bị lấy mẫu bệnh phẩm (`HIS.Desktop.Plugins.TestConnectDeviceSample`). Tiền tố `TK-` (`PrintTypeCodeWorker.ACCOUNT_BARCODE_PREFIX`) phải trùng với màn đó.
- GĐ1: mã thẻ không thu hồi được (ai biết tài khoản cũng in được); GĐ2 (chờ duyệt) cấp mã ngẫu nhiên ở BE.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_EMPLOYEE (EmployeeADO) | Table | Dòng nhân viên: LOGINNAME, TDL_USERNAME, EMPLOYEE_CODE, DEPARTMENT_ID |
| HIS_DEPARTMENT | Cache | Tên khoa in trên thẻ |

## 4. UI Layout

Cột nút In barcode được thêm bằng code (`frmEmpUser__PrintBarcode.cs`, `InitPrintBarcodeColumn()` gọi trong Load sau `SetCaptionByLanguageKey`), không sửa Designer. Ảnh nút: `images/print/printer_16x16.png` (DevExpress.Images), chú thích lấy từ khóa `frmEmpUser.gridColumnPrintBarcode.Caption`.

## 5. API Endpoints

Nút In barcode không gọi API nghiệp vụ; RichEditorStore đọc loại in/mẫu từ SAR.

## 6. Dependencies

| Thành phần | Ghi chú |
|------------|---------|
| MPS.Processor.Mps000521 (+ PDO) | Mẫu thẻ |
| Inventec.Common.RichEditor | Chọn mẫu, gọi `MPS.MpsPrinter.Run` |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Thẻ barcode tài khoản lấy mẫu | Mps000521 | MPS.Processor.Mps000521 | `Tmp\Mps\Mps000521\TheBarcodeTaiKhoanLayMau.xlsx` |

Chế độ in theo `ConfigApplications.CheDoInChoCacChucNangTrongPhanMem` (2 = in ngay, khác = xem trước); máy in theo `GlobalVariables.dicPrinter`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/10/2026 | dangth | **Việc 60557** — Nút In barcode trên dòng (MPS000521, `TK-` + LOGINNAME). Thêm `frmEmpUser__PrintBarcode.cs`, `PrintTypeCodeWorker.cs`, khóa ngôn ngữ, tham chiếu MPS/RichEditor/PDO 521/DevExpress.Images |

## 9. Test Cases

- [ ] Cột In barcode hiện sau cột gán vai trò, có tooltip
- [ ] Bấm In barcode ở dòng A: xem trước thẻ có họ tên, tài khoản, mã NV, khoa, barcode; quét được trên máy quét COM ra `TK-` + tài khoản
- [ ] Tài khoản dài: in thử tài khoản dài nhất của viện (barcode > 41 ký tự không vẽ được)
