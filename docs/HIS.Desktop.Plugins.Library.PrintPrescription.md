# Thư viện in đơn thuốc (Library.PrintPrescription) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Project | HIS.Desktop.Plugins.Library.PrintPrescription |
| Loại | Library dùng chung (DLL ở `ReferencedAssemblies`, không phải plugin chạy độc lập) |
| Mục đích | Dựng dữ liệu và gọi in các mẫu đơn thuốc: Mps000044, Mps000050, Mps000118, Mps000234, Mps000296, Mps000338 (kèm các mẫu tách riêng gây nghiện Mps000181, hướng thần Mps000192, thực phẩm chức năng Mps000191, sản phẩm hỗ trợ Mps000353) |
| Nơi gọi | Màn Kê đơn (AssignPrescriptionPK/CLS/YHCT/Kidney) sau khi lưu, Danh sách y lệnh (ServiceReqList), Danh sách đơn thuốc, Xuất bán... qua `PrintPrescriptionProcessor` |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
`PrintPrescriptionProcessor.Print(mã in)` → `RichEditorStore.RunPrintTemplate` → `DelegateRunPrinter` gom dữ liệu đơn (`ProcessDataForPrint`) → lớp `PrintMps0000xx` theo mã in → `ThreadLoadDataMediMate` dựng danh sách dòng in (thuốc/vật tư trong kho theo phiếu xuất, ngoài kho theo y lệnh) → tách nhóm (thường, ngoài kho, ngoài viện, gây nghiện, hướng thần, lao, TPCN, sản phẩm hỗ trợ, chứa dược chất GN/HT theo cấu hình tài khoản `CHE_DO_IN_TACH_DON_THUOC`) → dựng PDO → `Print.PrintData` → `MpsPrinter`.

### Quy tắc gom dòng in (Việc 46680, tài liệu 2799)
- **Một dòng in = một dòng bác sĩ kê.** Các bản ghi xuất thuốc được gom theo các thông tin như trước **cộng liều dùng (`TUTORIAL`) và cách dùng (`HTU_TEXT`)**, chuẩn hóa bằng `PrescriptionLineKey.Normalize` (bỏ khoảng trắng đầu/cuối, trống = khoảng trắng).
  - `ThreadLoadDataMediMate` — thuốc trong kho: (loại thuốc, giá, hao phí, phiếu xuất, liều dùng, cách dùng); thuốc ngoài kho: (loại thuốc, tên, đường dùng, y lệnh, giá, đơn phụ, liều dùng, cách dùng).
  - `PrintMps000044/050/118/296`: 13 chỗ gom lại theo loại thuốc (các lô/giá khác nhau của cùng dòng kê thành 1 dòng in) có thêm liều dùng + cách dùng. Mẫu gây nghiện Mps000181 gom thêm theo y lệnh (mỗi y lệnh 1 đợt) như trước.
- Liều dùng, số lô, hạn dùng, kho, nhà cung cấp, giá nhập của dòng in lấy theo **đúng bản ghi của dòng** (`V_HIS_EXP_MEST_MEDICINE` khớp `ID` bản ghi đầu của nhóm). Trước đây lấy bản ghi đầu tiên cùng (thuốc, giá, hao phí, phiếu xuất) nên dòng thứ 2 cùng thuốc có thể nhận nhầm lô/liều dùng của dòng 1.
- Không có cấu hình bật/tắt: đây là sửa cái sai (trước đây 2 dòng khác liều dùng in thành 1 dòng, số lượng cộng dồn, chỉ có liều dùng dòng đầu).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_EXP_MEST_MEDICINE / V_HIS_EXP_MEST_MEDICINE | Table / View | Dòng xuất thuốc trong kho (số lượng, liều dùng, cách dùng, lô, hạn dùng) |
| HIS_EXP_MEST_MATERIAL / V_HIS_EXP_MEST_MATERIAL | Table / View | Dòng xuất vật tư |
| HIS_SERVICE_REQ_METY / HIS_SERVICE_REQ_MATY | Table | Thuốc / vật tư ngoài kho theo y lệnh |
| HIS_SERVICE_REQ, HIS_EXP_MEST, HIS_TREATMENT | Table | Y lệnh, phiếu xuất, hồ sơ |

## 5. API Endpoints

| Action | URI | Ghi chú |
|--------|-----|---------|
| Y lệnh đơn của hồ sơ | `api/HisServiceReq/Get` | Đếm số lần dùng thuốc |
| Dòng xuất thuốc của hồ sơ | `api/HisExpMestMedicine/GetView` | Lô, hạn dùng, liều dùng theo ID dòng |
| Dòng xuất vật tư của hồ sơ | `api/HisExpMestMaterial/GetView` | |
| Thuốc ngoài kho của hồ sơ | `api/HisServiceReqMety/Get` | |

## 7. Print

| Mã in | Lớp dựng dữ liệu | Ghi chú |
|-------|------------------|---------|
| Mps000044 | `PrintMps000044` | Đơn thuốc; processor có thêm list `ServiceMedicinesMerge` |
| Mps000050 | `PrintMps000050` | |
| Mps000118 | `PrintMps000118` | Mã in mặc định khi không cấu hình `HIS.Desktop.Plugins.Library.PrintPrescription.Mps` |
| Mps000234 | `PrintMps000234` | Đơn tổng hợp phòng khám |
| Mps000296 | `PrintMps000296` | |
| Mps000338 | `PrintMps000338` | Đơn cận lâm sàng |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/10/2026 | dangth2 | **Việc 46680 (Yên Bình, tài liệu 2799)** — tách dòng in theo liều dùng + cách dùng: `PrescriptionLineKey.cs` (MỚI); `ThreadLoadDataMediMate` gom thêm `TUTORIAL`/`HTU_TEXT` (trong kho + ngoài kho) và lấy thông tin lô/liều dùng theo ID bản ghi của dòng (từ điển theo ID, thay `FirstOrDefault` theo thuốc/giá); `PrintMps000044/050/118/296` thêm liều dùng + cách dùng vào 13 chỗ gom lại theo thuốc. Test: harness `PTTK\46680\harness_test_FE` (đối chứng DLL cũ: 2 dòng Nước cất tiêm khác cách dùng in thành 1 dòng số lượng cộng dồn, dòng 2 nhận lô của dòng 1). |

## 9. Test Cases
- [ ] 2 dòng cùng thuốc khác cách dùng (đã lưu riêng) → đơn in 2 dòng, mỗi dòng số lượng, liều dùng, cách dùng, số lô của chính nó.
- [ ] 2 dòng cùng thuốc khác liều dùng → đơn in 2 dòng (trước đây 1 dòng cộng dồn).
- [ ] 1 dòng kê xuất từ 2 lô → in 1 dòng, số lượng tổng.
- [ ] Thuốc ngoài kho 2 dòng khác cách dùng → in 2 dòng.
- [ ] Các nhóm tách riêng (gây nghiện, hướng thần, TPCN, sản phẩm hỗ trợ, ngoài viện) cùng quy tắc.
