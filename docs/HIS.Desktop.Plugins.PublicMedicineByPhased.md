# Công Khai Thuốc Theo Giai Đoạn — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.PublicMedicineByPhased |
| Loại | Form |
| Mục đích | Chọn hồ sơ điều trị + khoảng ngày, liệt kê thuốc/vật tư/hóa chất/máu đã dùng theo ngày y lệnh, in Phiếu công khai thuốc MPS000088 (mỗi trang `congKhaiThuoc_DaySize` ngày) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

1. Lọc y lệnh đơn (DONDT, DONTT, DONM) của hồ sơ trong khoảng ngày, theo khoa/phòng chỉ định, bỏ y lệnh không thực hiện.
2. Lấy phiếu xuất của y lệnh → dòng xuất thuốc / vật tư / máu (3 luồng song song), quy về `ExpMestMediAndMateADO` (vật tư, máu dùng chung trường `MEDICINE_TYPE_ID` để chứa id loại vật tư/máu, phân biệt bằng `Service_Type_Id`).
3. Người dùng chọn dòng trên lưới → In: `AddDataToDatetime()` gom theo loại thuốc (± HDSD khi tích "Tách HDSD") và chia trang theo ngày, rồi in từng trang.

## 5. API Endpoints

| Action | URI | Ghi chú |
|--------|-----|---------|
| Y lệnh | HIS_SERVICE_REQ_GET | |
| Phiếu xuất | HIS_EXP_MEST_GET | |
| Dòng xuất thuốc / vật tư / máu | api/HisExpMestMedicine/GetView, api/HisExpMestMaterial/GetView, HIS_EXP_MEST_BLOOD_GETVIEW | |
| Hồ sơ, giường | HIS_TREATMENT_GETVIEW, api/HisBedLog/GetView | Lúc in |
| Hoạt chất danh mục | HIS_MEDICINE_TYPE_ACIN_GETVIEW (`MEDICINE_TYPE_IDs`) | Việc 59727 — 1 lần lúc in, chỉ dòng thuốc; lỗi thì vẫn in, key hoạt chất trống |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu công khai thuốc | Mps000088 | `MPS.Processor.Mps000088` (xem `docs/MPS.Processor.Mps000088.md`) | Theo viện, ví dụ NTP `PhieuCongKhaiThuocHangNgay.xlsx` |

Có ký số EMR (chkSign / chkPrintDocumentSigned).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/10/2026 | dangth2 (Claude) | Việc 59727 (NTP): lúc in lấy hoạt chất danh mục của các thuốc đang chọn (`GetMedicineTypeAcins`) gán vào PDO; dòng thuốc mang thêm H/C BHYT |

## 9. Test Cases

- [ ] Thuốc khai 1/nhiều hoạt chất → mẫu có key `ACTIVE_INGREDIENT_NAMES` in đúng hoạt chất
- [ ] Vật tư, máu trùng id với thuốc có hoạt chất → không hiện hoạt chất
- [ ] Mẫu cũ (không dùng key mới) → in như trước
