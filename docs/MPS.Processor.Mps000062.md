# MPS000062 — Tờ Điều Trị — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000062` (+ `MPS.Processor.Mps000062.PDO`) |
| Mẫu in | Tờ điều trị nội trú (NGÀY GIỜ / DIỄN BIẾN BỆNH / Y LỆNH) |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Trạng thái | Bảo trì |

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000062PDO` nhận: `_Treatment`, `_Trackings` (V_HIS_TRACKING), `_Dhsts`, `_DicServiceReqs` (HIS_SERVICE_REQ theo id), `_DicHisExpMests` (HIS_EXP_MEST theo **service req id**), `_DicExpMestMedicines` / `_DicExpMestMaterials` (dòng thuốc/vật tư theo exp mest id), `_Cares`, `_CareDetails`, danh mục thuốc/vật tư/loại DV, suất ăn, máu, v.v.

## 3. Các Bảng Dữ Liệu Đổ Vào Template (chính)

| Bảng | Nguồn | Nội dung |
|------|-------|----------|
| `TrackingADOs` | `_Mps000062ADOs` (`Mps000062ADO : V_HIS_TRACKING`) | Mỗi dòng = 1 tờ điều trị (kèm DHST, chẩn đoán) |
| `Medicines` | `_ExpMestMetyReqADOCommons` | Thuốc thường trong ngày |
| `MedicinesInfusion` | `_ExpMestMetyReqADOCommonsMix` | Thuốc pha truyền |
| `MedicinesDuTru` / `MedicinesTHDT` | `..DuTru` / `..THDT` | Thuốc dự trù / thực hiện dự trù |
| `MedicinesTreatment` / `MedicinesHomePres` | Lọc `Medicines` theo `IS_HOME_PRES` | **Việc 26771** — thuốc điều trị / thuốc cấp về |
| `MedicinesDuTruTreatment` / `MedicinesDuTruHomePres` | Lọc `MedicinesDuTru` theo `IS_HOME_PRES` | **Việc 26771** — bản dự trù |
| `Materials*`, `ServiceCLS*`, `TTServices*`, `Bloods`, `ExamServices`, `MedicalInstruction`, `RemedyCount`, `MedicineLines`, `ServiceReq*`, `Ration`, `ImpMest*` | — | Vật tư, CLS, y lệnh text, số thang, dòng thuốc, suất ăn, tổng hợp |

Relationship: các bảng thuốc đều link `TrackingADOs.ID → TRACKING_ID`; `Medicines*` còn link `MedicineLines`, `RemedyCount` (EXP_MEST_ID), `ServiceReq` (TDL_SERVICE_REQ_ID); cặp DuTru link `ServiceReqDuTru`. 4 bảng tách mới (26771) có **đủ bộ relationship mirror** của bảng gốc.

## 4. Tách Thuốc Cấp Về (Việc 26771)

**Nhận diện** (trong `Medicines()`, khi dựng từng nhóm `ExpMestMetyReqADO` — giống tiền lệ `Mps000262`):

```
isHomePres = serviceReq.IS_HOME_PRES == 1      (checkbox "Mang đơn về" cả đơn)
          || expMest.IS_HOME_PRES == 1         (phiếu xuất)
          || nhóm dòng có IS_HOME_PRES_LINE==1 (cột MV từng thuốc)
```

**Field mới trên `ExpMestMetyReqADO`** (PDO — lan truyền mọi bảng thuốc): `IS_HOME_PRES` (1/0), `HOME_PRES_STR` ("(Mang về)" khi = 1).

**Field mới trên `Mps000062ADO`** (bảng `TrackingADOs`): `HOME_PRES_TITLE` = "Thuốc cấp về:" khi tờ có ≥ 1 thuốc mang về, ngược lại rỗng (mẫu không dư nhãn trống).

**Tương thích ngược:** các bảng/band cũ giữ nguyên dữ liệu (gồm cả thuốc mang về) — mẫu chưa sửa in y hệt bản cũ. Mẫu muốn tách khối dùng cặp bảng `*Treatment` / `*HomePres` + dòng `<#TrackingADOs.HOME_PRES_TITLE;>`; hoặc chỉ thêm `<#Medicines.HOME_PRES_STR;>` sau tên thuốc.

## 5. Luồng Mẫu REPX (XtraReport)

Ngoài Excel FlexCel, MPS62 còn hỗ trợ mẫu `.repx` qua `ProcessDataXtraReport()` — cơ chế khác hẳn: thuốc KHÔNG đổ theo band mà được build sẵn thành **chuỗi HTML** trên từng dòng `TrackingADOs` (`Mps000062ExtADO`), repx bind qua ExpressionBindings (PropertyName=Html). Các field chính: `MEDICINES___DATA` / `___DATA1..3`, `MEDICINES_MERGE*`, `MEDICINES_INFUSION___DATA`, `MEDICINES_DuTru/THDT___DATA`... Mẫu chuẩn `062-Tờ điều trị chuẩn_ DT.repx` bind `MEDICINES___DATA3`.

**Việc 26771 (repx):** thêm 2 field trên `Mps000062ExtADO`, build trong cùng loop DATA3 (format y hệt):
- `MEDICINES_TREATMENT___DATA3` — chỉ thuốc điều trị
- `MEDICINES_HOME_PRES___DATA3` — chỉ thuốc cấp về, mở đầu bằng tiêu đề đậm "Thuốc cấp về (BN mang về):" khi có dữ liệu

Mẫu repx muốn tách: thay `[MEDICINES___DATA3]` trong ExpressionBindings bằng `(ToStr([MEDICINES_TREATMENT___DATA3]) + ToStr([MEDICINES_HOME_PRES___DATA3]))`. Field cũ giữ nguyên → mẫu viện khác không đổi.

## 6. Build & Deploy

- Build PDO trước → copy `MPS.Processor.Mps000062.PDO.dll` vào `HIS.Desktop\bin\Debug\ReferencedAssemblies` → build processor (HintPath lib\MPSv2 không tồn tại trên máy backup, resolve qua ReferencePath).
- Deploy: PDO → `x64\ReferencedAssemblies`, processor → `x64\Plugins\MpsProcessor`.

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/09/2026 | nampp | **Việc 26771** — Tách thuốc cấp về / thuốc điều trị: thêm `IS_HOME_PRES` + `HOME_PRES_STR` (`ExpMestMetyReqADO`), `HOME_PRES_TITLE` (`Mps000062ADO`); thêm 4 bảng template `MedicinesTreatment`, `MedicinesHomePres`, `MedicinesDuTruTreatment`, `MedicinesDuTruHomePres` kèm relationship mirror. Chỉ THÊM, không sửa band cũ. Thiết kế: `PTTK\26771 - Thiet ke - Tach thuoc cap ve tren to dieu tri MPS000062.md` |
| 07/09/2026 | nampp | **Việc 26771 (repx viện — `todieutriphoiFIX.repx`)** — Repx viện bind `MEDICINES_MERGE_HTU___DATA` + `MEDICINES_MERGE_DATE_DUTRU_HTU___DATA`. Thêm 4 field: `MEDICINES_MERGE_HTU_TREATMENT/HOME_PRES___DATA` (build trong loop `medicine_Merges`, khối cấp về có tiêu đề đậm) và `MEDICINES_MERGE_DATE_DUTRU_HTU_TREATMENT/HOME_PRES___DATA` (partition `MMDuTruDataSort` theo `IS_HOME_PRES` — 2 chỗ Add đã gắn cờ từ `ReqDT.IS_HOME_PRES`/dòng thuốc; header ngày dự trù cấp về kèm nhãn). Repx thay `[FIELD]` → `(ToStr([..TREATMENT..]) + ToStr([..HOME_PRES..]))` (5 chỗ) |
| 07/09/2026 | nampp | **Việc 26771 (repx)** — Viện dùng mẫu `.repx`: thêm 2 field HTML `MEDICINES_TREATMENT___DATA3` + `MEDICINES_HOME_PRES___DATA3` (`Mps000062ExtADO`, build trong loop DATA3 của `ProcessDataXtraReport()`, format y hệt DATA3, khối cấp về có tiêu đề đậm); sửa `062-Tờ điều trị chuẩn_ DT.repx` (backup `.bak_20260907`) thay `[MEDICINES___DATA3]` → `(ToStr([MEDICINES_TREATMENT___DATA3]) + ToStr([MEDICINES_HOME_PRES___DATA3]))` (3 chỗ trong ExpressionBindings) |
| 07/09/2026 | nampp | **Việc 26771 (mẫu in)** — Sửa 2 mẫu test `D:\HISTEST\histest\x64\Tmp\Mps\Mps000062\062-Tờ điều trị chuẩnDT.xlsx` + `__CoMau.xlsx` (backup `.bak_20260907`): band `__Medicines__` (R16) → `__MedicinesTreatment__`, header R14 dùng `MedicinesTreatment.INTRUCTION_DATE`; chèn R17 header "Thuốc cấp về (BN mang về): Ngày sử dụng …" (tự xóa khi rỗng) + R18 band `__MedicinesHomePres__`; header đơn dự trù (R20) thêm nhãn `(Mang về)` theo `ServiceReqDuTru.IS_HOME_PRES`. Sửa bằng FlexCel API (script `edit_tpl62.ps1`). **Mẫu mới yêu cầu DLL mới** (có bảng MedicinesTreatment/HomePres) |

## 8. Test Cases

- [ ] Ngày có cả 2 loại thuốc, mẫu tách band → Y LỆNH ra 2 khối, tiêu đề "Thuốc cấp về:" đúng chỗ
- [ ] Ngày không có thuốc cấp về → không in tiêu đề, khối HomePres rỗng
- [ ] Tick "MV" từng thuốc (không check cả đơn) → tách đúng theo dòng
- [ ] Thuốc cấp về kê dạng dự trù → nằm ở cặp `MedicinesDuTru*`
- [ ] Hồi quy: mẫu CŨ chưa sửa → phiếu in y hệt trước khi thay DLL
- [ ] Hồi quy: pha truyền / vật tư / CLS / suất ăn / DHST / đánh số ngày dùng thuốc không đổi
