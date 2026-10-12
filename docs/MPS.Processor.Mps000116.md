# MPS000116 — Phiếu Công Khai Thuốc Theo Ngày — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000116` (+ `MPS.Processor.Mps000116.PDO`) |
| Mẫu in | Phiếu công khai thuốc theo ngày (1 ngày y lệnh) |
| Chức năng gọi in | `HIS.Desktop.Plugins.PublicMedicineByDate` |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Trạng thái | Bảo trì |

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000116PDO`: `_Treatment`, `_Mps000116ADOs` (danh sách gộp theo loại thuốc/vật tư), `_Mps000116DetailADOs` (chi tiết từng y lệnh, không gộp), `_WorkPlace`, `_vHisBedLog`, `_IntructionTime`, `_SingleKeys`, `_MedicineTypeAcins` (việc 59727 — hoạt chất danh mục của các thuốc trên phiếu, chức năng gọi in gán sau khi khởi tạo).

`Mps000116ADO` / `Mps000116DetailADO` dựng từ: thuốc lĩnh kho (V_HIS_EXP_MEST_MEDICINE), vật tư lĩnh kho (V_HIS_EXP_MEST_MATERIAL), thuốc tự mua (HIS_SERVICE_REQ_METY + danh mục), vật tư tự mua (HIS_SERVICE_REQ_MATY + danh mục). `TypeId` 1 = thuốc, 2 = vật tư; `TYPE_NAME` "Lĩnh ở kho" / "Tự mua"; thuốc ngoài danh mục có `MEDI_MATY_TYPE_ID = 0`.

## 3. Bảng Dữ Liệu Đổ Vào Template

| Bảng | Nội dung |
|------|----------|
| `Type` | THUỐC / VẬT TƯ (`ID` = TypeId) |
| `MedicinesADO` | Danh sách gộp, relationship `Type.ID → TypeId` |
| `MedicineDetailADO` | Danh sách chi tiết từng y lệnh, relationship `Type.ID → TypeId` |

**Key hoạt chất (việc 59727)** — có ở cả `MedicinesADO` và `MedicineDetailADO`:

| Key | Ý nghĩa |
|-----|---------|
| `ACTIVE_INGREDIENT_NAMES` / `ACTIVE_INGREDIENT_CODES` | Hoạt chất khai ở danh mục thuốc (HIS_MEDICINE_TYPE_ACIN), nhiều hoạt chất nối " + ". Vật tư, thuốc ngoài danh mục luôn rỗng |
| `ACTIVE_INGR_BHYT_NAME` / `ACTIVE_INGR_BHYT_CODE` | Hoạt chất BHYT: thuốc lĩnh kho lấy từ dòng xuất, thuốc tự mua lấy từ danh mục |

Mẫu gợi ý (lồng trong `<#if>` ẩn dòng tự mua của mẫu NTP): `<#MedicinesADO.MEDI_MATY_TYPE_NAME;><#if(<#MedicinesADO.ACTIVE_INGREDIENT_NAMES;>="";; (<#MedicinesADO.ACTIVE_INGREDIENT_NAMES;>))> (<#MedicinesADO.CONCENTRA;>)`.

## 4. Xử Lý Trong Processor

`SetSingleKey()`: nhóm Type, key bệnh nhân/giường, sắp xếp 2 danh sách theo `IsOderMedicine` → `SetActiveIngredient()` (59727) gán hoạt chất cho dòng `SERVICE_TYPE_ID == THUOC` của cả 2 danh sách.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/10/2026 | dangth2 (Claude) | Việc 59727 (NTP): thêm key `ACTIVE_INGREDIENT_NAMES/CODES`, `ACTIVE_INGR_BHYT_NAME/CODE` cho danh sách gộp và chi tiết; PDO nhận `_MedicineTypeAcins`; processor thêm tham chiếu IMSys.DbConfig.HIS_RS. Mẫu cũ in như trước |
