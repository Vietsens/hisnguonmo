# MPS000044 — Đơn thuốc — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000044 |
| Project | `MPS.Processor.Mps000044` + `MPS.Processor.Mps000044.PDO` |
| Loại | MPS Processor (template Excel FlexCel) |
| Mục đích | In đơn thuốc (thuốc/vật tư trong kho và ngoài kho) theo phiếu xuất của y lệnh đơn |
| Nơi gọi | Thư viện `HIS.Desktop.Plugins.Library.PrintPrescription` (`PrintMps000044`) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
`Mps000044PDO` (y lệnh, danh sách `ExpMestMedicineSDO`, hồ sơ, thẻ BHYT, phiếu xuất...) → `Mps000044Processor.ProcessData()`:

```
ReadTemplate → SetSingleKey() → MedicinesSort() → ProcessListData() → MedicinesMerge() → SetQrCode()
  → ProcessPrintLogData() → SetNumOrderKey(...) → SetTreatmentQrCodeBase() → SetSignatureKeyImageByCFG()
  → objectTag: "type" (đối tượng thanh toán), "ServiceMedicines", "ServiceMedicinesMerge"
```

### Danh sách dòng thuốc trên mẫu
- `ServiceMedicines`: danh sách dòng như thư viện in truyền vào (mỗi dòng kê × giá), sắp theo `KeyUseForm`.
- `ServiceMedicinesMerge` (Nampp 17/09/2026): gộp các lô khác nhau của **cùng 1 dòng kê** thành 1 dòng (không gom theo giá). Khóa gộp: loại thuốc, Type, đối tượng thanh toán, hao phí, phiếu xuất, đơn vị, liều dùng (`TUTORIAL`), `HTU_ID`, **cách dùng (`HTU_TEXT`, Việc 46680)**, liều theo buổi, ngày dùng đến. Dòng gộp từ nhiều lô bỏ số lô/hạn dùng; nếu các lô khác giá thì bỏ đơn giá.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/10/2026 | dangth2 | **Việc 46680 (Yên Bình, tài liệu 2799)** — `MedicinesMerge` thêm cách dùng `HTU_TEXT` (chuẩn hóa bỏ khoảng trắng đầu/cuối) vào khóa gộp, để 2 dòng cùng thuốc, cùng liều dùng, khác cách dùng không bị gộp lại trên list `ServiceMedicinesMerge`. |

## 9. Test Cases
- [ ] Đơn có 2 dòng Nước cất tiêm cùng liều dùng, khác cách dùng → `ServiceMedicinesMerge` có 2 dòng, mỗi dòng số lượng và cách dùng riêng.
- [ ] 1 dòng kê xuất từ 2 lô khác giá → `ServiceMedicinesMerge` 1 dòng, số lượng tổng, không in đơn giá.
