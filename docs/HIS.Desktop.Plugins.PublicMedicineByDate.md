# Công Khai Thuốc Theo Ngày — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.PublicMedicineByDate |
| Loại | Form |
| Mục đích | Chọn hồ sơ điều trị + 1 ngày (có thể giới hạn giờ y lệnh), in Phiếu công khai thuốc theo ngày MPS000116 gồm thuốc/vật tư lĩnh kho và tự mua |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

1. Lọc y lệnh của hồ sơ trong ngày (theo khoa/phòng chỉ định tùy chọn), tách y lệnh có phiếu xuất (trong kho) và đơn tự mua (ngoài kho).
2. Nạp song song 4 luồng: thuốc / vật tư lĩnh kho (V_HIS_EXP_MEST_MEDICINE/MATERIAL), thuốc / vật tư tự mua (HIS_SERVICE_REQ_METY/MATY). Mỗi luồng tạo danh sách gộp `Mps000116ADO` và danh sách chi tiết từng y lệnh `Mps000116DetailADO` (ghi có khóa vì chạy song song).
3. In: bỏ dòng số lượng 0, dựng `Mps000116PDO`, gán hoạt chất danh mục (việc 59727), gọi MPS.

## 5. API Endpoints

| Action | URI | Ghi chú |
|--------|-----|---------|
| Y lệnh, phiếu xuất | HIS_SERVICE_REQ_GET, HIS_EXP_MEST_GET | |
| Dòng xuất | api/HisExpMestMedicine/GetView, api/HisExpMestMaterial/GetView | Trong kho |
| Đơn tự mua | api/HisServiceReqMety/Get, api/HisServiceReqMaty/Get | Ngoài kho |
| Hồ sơ, giường | HIS_TREATMENT_GETVIEW, api/HisBedLog/GetView | Lúc in |
| Hoạt chất danh mục | HIS_MEDICINE_TYPE_ACIN_GETVIEW (`MEDICINE_TYPE_IDs`) | Việc 59727 — 1 lần lúc in cho thuốc lĩnh kho + thuốc tự mua có mã; lỗi thì vẫn in, key hoạt chất trống |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu công khai thuốc theo ngày | Mps000116 | `MPS.Processor.Mps000116` (xem `docs/MPS.Processor.Mps000116.md`) | Theo viện, ví dụ NTP `MPS000116_PhieuCongKhaiThuocTheoNgay___001.xlsx` |

Có ký số EMR (chkSign / chkPrintDocumentSigned).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/10/2026 | dangth2 (Claude) | Việc 59727 (NTP): lúc in lấy hoạt chất danh mục của thuốc trên phiếu (`GetMedicineTypeAcins`) gán vào PDO |

## 9. Test Cases

- [ ] Thuốc lĩnh kho và thuốc tự mua có khai hoạt chất → key `ACTIVE_INGREDIENT_NAMES` có giá trị ở cả danh sách gộp và chi tiết
- [ ] Thuốc ngoài danh mục, vật tư → key hoạt chất trống, không gọi API nếu phiếu không có thuốc có mã
- [ ] Mẫu cũ → in như trước
