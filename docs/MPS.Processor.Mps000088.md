# MPS000088 — Phiếu Công Khai Thuốc (Theo Giai Đoạn) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000088` (+ `MPS.Processor.Mps000088.PDO`) |
| Mẫu in | Phiếu công khai thuốc (MS: 01D/BV-01), các ngày trong giai đoạn là các cột Day1..Day60 |
| Chức năng gọi in | `HIS.Desktop.Plugins.PublicMedicineByPhased` (mỗi trang ngày = 1 PDO) |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Trạng thái | Bảo trì |

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000088PDO`: `currentTreatment` (V_HIS_TREATMENT), `mps000088ADO` (nhãn ngày Day1..Day60), `mps000088ByMediEndMate` (dòng thuốc/vật tư/máu), `_SingleKeys` (khoa, người in, IsTachHDSD, IsOderMedicine), `_vHisBedLog`, `_MedicineTypeAcins` (việc 59727 — hoạt chất danh mục của các thuốc trên phiếu, chức năng gọi in gán sau khi khởi tạo).

`Mps000088ByMediEndMate : V_HIS_EXP_MEST_MEDICINE` — dòng vật tư/máu dùng chung lớp này, **`MEDICINE_TYPE_ID` chứa id loại vật tư / loại máu**, phân biệt bằng `Service_Type_Id`.

## 3. Bảng Dữ Liệu Đổ Vào Template

| Bảng | Nội dung |
|------|----------|
| `ServiceType` | Nhóm Thuốc / Vật tư / Máu (`Service_Type_Id`, `TYPE_NAME`) |
| `SereServ` | Dòng thuốc/vật tư/máu, relationship `ServiceType.Service_Type_Id → SereServ.Service_Type_Id` |

Key dòng `SereServ` thường dùng: `MEDICINE_TYPE_NAME`, `CONCENTRA`, `SERVICE_UNIT_NAME`, `Day1..Day60`, `MORNING_/NOON_/AFTERNOON_/EVENING_/SUM_DayN`, `AMOUNT`, `TUTORIAL`.

**Key hoạt chất (việc 59727):**

| Key | Ý nghĩa |
|-----|---------|
| `ACTIVE_INGREDIENT_NAMES` / `ACTIVE_INGREDIENT_CODES` | Hoạt chất khai ở danh mục thuốc (HIS_MEDICINE_TYPE_ACIN), nhiều hoạt chất nối " + ". Vật tư, máu luôn rỗng |
| `ACTIVE_INGR_BHYT_NAME` / `ACTIVE_INGR_BHYT_CODE` | Hoạt chất BHYT của thuốc (trước việc 59727 luôn rỗng) |

Mẫu gợi ý: `<#SereServ.MEDICINE_TYPE_NAME;><#if(<#SereServ.ACTIVE_INGREDIENT_NAMES;>="";; (<#SereServ.ACTIVE_INGREDIENT_NAMES;>))> (<#SereServ.CONCENTRA;>)` → "Agifuros (Furosemid) (40mg)". **Trong `<#if>` để nhánh rỗng bằng `;;` — gõ `""` sẽ in ra nguyên 2 dấu nháy.**

## 4. Xử Lý Trong Processor

`SetSingleKey()`: key ngày, thời gian vào/ra viện, tuổi, khoa → `GroupByService()` gộp dòng (theo dịch vụ, giá, nồng độ, VAT, ± HDSD) → sắp xếp khi `IsOderMedicine == 1` → `SetActiveIngredient()` (59727) gán hoạt chất cho dòng `Service_Type_Id == THUOC`, bỏ trùng theo `ACTIVE_INGREDIENT_ID`.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/10/2026 | dangth2 (Claude) | Việc 59727 (NTP): thêm key hoạt chất danh mục `ACTIVE_INGREDIENT_NAMES/CODES` cho dòng thuốc, PDO nhận `_MedicineTypeAcins`; H/C BHYT có giá trị. Mẫu cũ in như trước |
