# Bảng Kê Thanh Toán — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Bordereau |
| Loại | Form (frmBordereau) |
| Mục đích | Bảng kê chi tiết dịch vụ điều trị cho 1 BN — thu ngân kiểm tra, sửa thông tin thanh toán, gán dịch vụ vào/ra gói (DV / BN / nguồn khác), tính lại tiền, in phiếu. |
| Nhóm | Common (Viện phí) |
| Module priority | 14 |
| Trạng thái | Đang bảo trì — cập nhật theo PTTK 2663 |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Thu ngân/kế toán mở bảng kê cho 1 BN/điều trị (mở từ menu hoặc inter-plugin từ Viện phí, Tạm ứng…).
2. Form load: BN, điều trị, danh sách `HIS_SERE_SERV` thuộc điều trị, các danh mục (`HIS_OTHER_PAY_SOURCE`, `HIS_PACKAGE`, `HIS_PATIENT_PACKAGE`…), cấu hình.
3. Thu ngân sửa từng dòng:
   - Đổi đối tượng thanh toán (BHYT ↔ Viện phí…)
   - Đánh dấu hao phí / không hưởng BHYT
   - Gán nguồn khác chi trả (HIS_OTHER_PAY_SOURCE)
   - **Gán gói bệnh nhân (HIS_PATIENT_PACKAGE) — PTTK 2663**
   - Đổi phòng thực hiện, điều kiện DV, lý do không thực hiện…

### Cấu hình Khoa-ĐTTT cho thuốc/VT (config `MOS.MEDICINE_MATERIAL.USE_PAYMENT_OBJECT_BY_DEPT`)
Khi config bật, với mỗi dòng **thuốc/VT** xét theo **khoa chỉ định = `TDL_REQUEST_DEPARTMENT_ID`** của dòng (config tắt → xử lý như hiện tại):
1. Tra `HIS_DEPA_PATIENT_TYPE` theo `SERVICE_ID` (tất cả khoa).
2. **Service có cấu hình + có bản ghi `DEPARTMENT_ID` = khoa chỉ định** → lọc combo ĐTTT theo các `PATIENT_TYPE_ID` được thiết lập; ô hao phí (HP) **disable** ở dòng khớp rule. **Khi mở form/reload: HP hiển thị đúng giá trị DB `IS_EXPEND` — KHÔNG tự tích trong RAM** (tránh lệch DB). Việc tích/bỏ tích theo `IS_AUTO_EXPEND/IS_NOT_EXPEND` chỉ thực hiện (và persist DB) khi user đổi ĐTTT — xem gạch đầu dòng dưới.
3. **Service có cấu hình nhưng KHÔNG bản ghi nào khớp khoa chỉ định** → **không cho đổi ĐTTT** (khóa cả combo ĐTTT chính `PATIENT_TYPE_ID` lẫn phụ thu `PRIMARY_PATIENT_TYPE_ID`).
4. **Service không có cấu hình** → xử lý như hiện tại (không lọc, không khóa).
- Khi user đổi ĐTTT thành công → tra lại với ĐTTT mới (thuốc/VT, khoa chỉ định, ĐTTT mới); nếu khớp → set `IS_EXPEND` theo `IS_AUTO_EXPEND/IS_NOT_EXPEND` rồi gọi `UpdatePayslipInfo` (Field = `IS_EXPEND`).
4. Mỗi thao tác sửa gọi API `HisSereServ/UpdatePayslipInfo` — backend tự tính lại giá theo cờ tương ứng.
5. In phiếu bảng kê / phiếu thu (dùng Library `PrintBordereau`).

### Vai trò
- **Thu ngân (HIS_ROOM_TYPE.ID__TN)**: full quyền sửa trong ngày.
- **Kế toán**: full quyền (kể cả ngày lùi).
- **Vai trò khác**: chỉ xem (cột bị disable).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT | View | Thông tin điều trị BN hiện tại |
| HIS_SERE_SERV | Table | Dòng dịch vụ chi tiết (mỗi dòng = 1 cell trong bảng kê) |
| V_HIS_SERVICE | View | Tên/mã dịch vụ |
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ (mỗi DV thuộc 1 phiếu yêu cầu) |
| HIS_PATIENT_TYPE | Table | Đối tượng thanh toán (BHYT, Viện phí…) |
| V_HIS_PATIENT_TYPE_ALTER | View | Đối tượng thay thế của BN |
| HIS_OTHER_PAY_SOURCE | Table | Danh mục nguồn khác chi trả |
| HIS_PACKAGE | Table | Mẫu gói dịch vụ (giá CSG) |
| HIS_PATIENT_PACKAGE | Table | **Gói bệnh nhân (PTTK 2663) — gói "mua trước dùng sau"** |
| HIS_SERE_SERV_DEPOSIT | Table | Tạm ứng đã dùng cho dòng DV |
| HIS_SERE_SERV_BILL | Table | Hoá đơn đã phát hành cho dòng DV |
| HIS_SERVICE_CONDITION | Table | Điều kiện DV (PCC/BHYT) |
| HIS_EQUIPMENT_SET | Table | Bộ thiết bị (VT theo bộ) |
| HIS_DEPA_PATIENT_TYPE | Table | **Cấu hình Khoa-ĐTTT cho thuốc/VT** — (DEPARTMENT_ID, SERVICE_ID, PATIENT_TYPE_ID, IS_AUTO_EXPEND, IS_NOT_EXPEND). Lọc/khóa ĐTTT + auto hao phí theo khoa chỉ định |

### Quan hệ
- HIS_TREATMENT → HIS_SERVICE_REQ → HIS_SERE_SERV (cascade qua TREATMENT_ID)
- HIS_SERE_SERV → HIS_OTHER_PAY_SOURCE (qua OTHER_PAY_SOURCE_ID, nullable)
- HIS_SERE_SERV → HIS_PATIENT_PACKAGE (qua **PATIENT_PACKAGE_ID** — PTTK 2663, nullable)
- HIS_SERE_SERV → HIS_PACKAGE (qua PACKAGE_ID, nullable)

## 4. UI Layout

```
+----------------------------------------------------------------------+
| [Thông tin BN: tên, mã, ngày sinh, đối tượng, fund...]               |
+----------------------------------------------------------------------+
| [Từ ngày] [Đến ngày] [Keyword] [Tìm kiếm]  [Chỉ SL>0] [Có máu]      |
+----------------------------------------------------------------------+
| Grid bảng kê (gridControlBordereau / gridViewBordereau)              |
| STT|MãYL|TG y lệnh|Mã DV|Tên DV|...|ĐT|...|HP|...                   |
| ...|Điều kiện|Nguồn khác|... |Gói bệnh nhân|Gói dịch vụ|...         |
+----------------------------------------------------------------------+
| Tổng hợp chi phí: [Phải thu][Phải thu BN][Đã thu][Cần thu thêm]      |
| Số phim sử dụng:  [Xquang][MRI][CT]                                  |
| CP theo ĐK lọc:   [Phải thu][Phải thu BN][Đã thu][Cần thu thêm]      |
+----------------------------------------------------------------------+
| [In ▼] [Đóng]                                                        |
+----------------------------------------------------------------------+
```

### Vùng "CP theo ĐK lọc" (PTTK 2883 — mục 1.1)
| Label | Công thức (trên danh sách đang hiển thị ở grid) |
|-------|--------------------------------------------------|
| `lblFilterTotalPrice` (Phải thu) | SUM(`VIR_TOTAL_PRICE`) các DV đang hiển thị |
| `lblFilterTotalPatientPrice` (Phải thu BN) | SUM(`VIR_TOTAL_PATIENT_PRICE`) các DV đang hiển thị |
| `lblFilterTotalObtainedPrice` (Đã thu) | SUM(`VIR_TOTAL_PATIENT_PRICE`) các DV **đã thanh toán** (có trong `SereServBills` với `IS_CANCEL` null/0 — dòng màu xanh) |
| `lblFilterTotalDepositPrice` (Cần thu thêm) | SUM(`VIR_TOTAL_PATIENT_PRICE`) các DV **chưa thanh toán** (dòng màu đen) |

- Tính trong `LoadFilteredFeeSummary()` (`frmBordereau___FilterFeeSummary.cs`), gọi sau MỖI lần gán `gridControlBordereau.DataSource` (Tìm kiếm, Enter keyword, load lần đầu, reload menu chuột phải).
- Dòng dự trù máu (`isAssignBlood`) không tính vào tổng.

### UC sử dụng
| UC / Control | Mục đích |
|--------------|----------|
| GridControl + GridView | Hiển thị danh sách HIS_SERE_SERV |
| RepositoryItemGridLookUpEdit | Combo trong cell: ĐT, đối tượng PT, nguồn khác, **gói BN**, equipment |
| RepositoryItemCheckEdit | Cell checkbox: HP, không thực hiện, không hưởng BHYT, ngoài KTC |
| RepositoryItemSpinEdit | Stent order, share count |
| DateEdit, TextEdit | Bộ lọc trên cùng |

### Cột "Gói bệnh nhân" (PTTK 2663)
| Thuộc tính | Giá trị |
|-----------|---------|
| Tên cột | `gridColumnPatientPackage` |
| Caption | "Gói bệnh nhân" (theo Lang.resx) |
| FieldName | `PATIENT_PACKAGE_ID` |
| ColumnEdit | `repositoryItemGridLookUpEdit_PatientPackage` |
| Vị trí | Trước cột "Gói dịch vụ" (`gridCol_Package`) |
| Enable | Chỉ khi `currentModule.RoomTypeId == HIS_ROOM_TYPE.ID__TN` |
| Source data | `HIS_PATIENT_PACKAGE` lọc theo `PATIENT_ID = currentTreatment.PATIENT_ID` & `IS_ACTIVE = 1` |
| Nút Delete | Gỡ gói khỏi dòng DV (`PATIENT_PACKAGE_ID = null`) |

## 5. API Endpoints

| Action | URI | Consumer | DTO/Filter |
|--------|-----|----------|------------|
| Get HIS_SERE_SERV theo điều trị | `api/HisSereServ/GetByTreatmentId` | MosConsumer | long treatmentId |
| **Update payslip info** (sửa cell trong bảng kê) | `api/HisSereServ/UpdatePayslipInfo` | MosConsumer | `HisSereServPayslipSDO { Field, SereServs, TreatmentId }` |
| Get HIS_PATIENT_TYPE_ALTER | `api/HisPatientTypeAlter/GetView` | MosConsumer | `HisPatientTypeAlterViewFilter` |
| Get HIS_PACKAGE | `api/HisPackage/Get` | MosConsumer | `HisPackageFilter` |
| Get HIS_OTHER_PAY_SOURCE | (qua BackendDataWorker cache) | — | — |
| **Get HIS_PATIENT_PACKAGE** (PTTK 2663) | `api/HisPatientPackage/Get` | MosConsumer | `HisPatientPackageFilter { PATIENT_ID, IS_ACTIVE }` |
| Get HIS_SERE_SERV_DEPOSIT | `api/HisSereServDeposit/Get` | MosConsumer | `HisSereServDepositFilter` |

### UpdateField enum (đã có / cần bổ sung)

Hiện có trong `MOS.SDO.UpdateField`:
`IS_FUND_ACCEPTED`, `IS_EXPEND`, `EXPEND_TYPE_ID`, `IS_NO_EXECUTE`, `IS_NOT_USE_BHYT`,
`IS_OUT_PARENT_FEE`, `PARENT_ID`, `PATIENT_TYPE_ID`, `OTHER_PAY_SOURCE_ID`,
`PRIMARY_PATIENT_TYPE_ID`, `SHARE_COUNT`, `EQUIPMENT_SET_ORDER__AND__EQUIPMENT_SET_ID`,
`SERVICE_CONDITION_ID`.

**Cần backend bổ sung (PTTK 2663):** `PATIENT_PACKAGE_ID` — để chỉ định API sửa cột Gói BN trong bảng kê.

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintBordereau | In phiếu bảng kê, hoá đơn, ấn chỉ |

### Inter-Plugin (được gọi từ ngoài)
| Plugin gọi | Args truyền |
|-----------|-------------|
| Viện phí (TransactionBill, TransactionBillTwoInOne) | treatmentId, currentModule, refresh delegate |
| Tạm ứng | treatmentId, currentModule |
| Menu chính | currentModule |

## 7. Print

| Loại in | Library | PrintTypeCode (config) |
|---------|---------|------------------------|
| Phiếu bảng kê chi tiết | `PrintBordereauProcessor` | Cấu hình per cơ sở (vd `Mps000446`) |
| Hoá đơn | `PrintBordereauProcessor` (IsActionButtonPrintBill = true) | — |
| Ấn chỉ in qua menu | `PrintBordereauProcessor` | — |

Print flow chuẩn — xem `Print/frmBordereau___Print__Init.cs`. Gọi `PrintBordereauProcessor(RoomId, RoomTypeId, treatmentId, patientId, initData, reloadMenu, getDocSigned)`.

### Bảng kê theo QĐ 697/QĐ-BYT (PTTK 2689 — mục 3.1 + 3.5)

Menu dropdown của nút In được mở rộng thêm 5 lựa chọn, đặt làm 1 nhóm xếp ngay dưới các lựa chọn QĐ 6556 tương ứng:

> ⚠️ **PrintTypeCode VIẾT HOA `MPS000...`** để khớp record `SAR_PRINT_TYPE` trong DB (so sánh `m.PrintTypeCode == n.PRINT_TYPE_CODE` phân biệt hoa/thường). DB phải có 5 record `MPS000508→512`, `IS_ACTIVE=1`, tên đúng cột Caption dưới đây.

| # | Caption | PrintTypeCode (DB) | MpsBehavior xử lý (PDO type) | Vị trí menu (sau anchor 6556) |
|---|---------|---------------|-------------------|-------------------------------|
| 1 | Bảng kê ngoại trú BHYT (697/QĐ-BYT) | `MPS000508` | `Mps000508Behavior` (PDO Mps000508) | Sau `Mps000279` |
| 2 | Bảng kê nội trú BHYT (697/QĐ-BYT) | `MPS000509` | reuse `Mps000508Behavior` (redirect printCode→`MPS000508`) | Sau `Mps000280` |
| 3 | Bảng kê ngoại trú Viện phí (697/QĐ-BYT) | `MPS000510` | `Mps000510Behavior` (PDO Mps000510, view `V_HIS_SERE_SERV_2`) | Sau `Mps000281` |
| 4 | Bảng kê nội trú Viện phí (697/QĐ-BYT) | `MPS000511` | reuse `Mps000510Behavior` (redirect printCode→`MPS000510`) | Sau `Mps000282` |
| 5 | Bảng kê tổng hợp 697 | `MPS000512` | `Mps000512Behavior` (PDO Mps000512) | Sau `Mps000302` |

**Cơ chế ẩn/hiện**: kế thừa hoàn toàn từ nhóm 6556 — `InitMenuDynamic` đã lọc theo `TREATMENT_TYPE_ID + isBHYT + isVienPhi`, các mã 697 nằm cùng khối điều kiện với mã 6556. **Không** tạo config riêng.

**Phạm vi sửa đổi — TẤT CẢ ở `HIS.Desktop.Plugins.Library.PrintBordereau` (plugin `Bordereau` tự động có menu mới qua Library)**:

*Mục 3.1 — Bổ sung menu items*:
- `Base/PrintTypeCodeWorker.cs` — 5 const `PRINT_TYPE_CODE___..._697_QĐ_BYT` (Mps000508→Mps000512).
- `InitMenuProcessor.cs` — thêm 5 menu item trong cả `InitMenuNormal` (caption hardcoded) và `InitMenuDynamic` (caption từ `SAR_PRINT_TYPE.PRINT_TYPE_NAME` theo mục 1.1).

*Mục 3.5 — Bổ sung MpsBehavior + dispatch*:
- `MpsBehavior/Mps000508/Mps000508Behavior.cs` (mới) — clone từ `Mps000279Behavior`, namespace PDO = `MPS.Processor.Mps000508.PDO`.
- `MpsBehavior/Mps000510/Mps000510Behavior.cs` (mới) — clone từ `Mps000281Behavior`, namespace PDO = `MPS.Processor.Mps000510.PDO`. PDO yêu cầu `List<V_HIS_SERE_SERV_2>` → fetch qua `api/HisSereServ/GetView2` với `TREATMENT_IDs` rồi lọc client-side theo SereServs.
- `MpsBehavior/Mps000512/Mps000512Behavior.cs` (mới) — clone từ `Mps000302Behavior`, namespace PDO = `MPS.Processor.Mps000512.PDO`.
- `PrintBordereauProcessor.cs.DelegateRunPrinter` — 5 case dispatch (`Mps000509`→`Mps000508` redirect, `Mps000511`→`Mps000510` redirect — pattern giống `Mps000280`→`Mps000279`).
- `HIS.Desktop.Plugins.Library.PrintBordereau.csproj` — 3 Reference MPS PDO DLL + 3 Compile Include cho 3 file Behavior mới.

**Phụ thuộc (chưa hoàn thành — ngoài tầm HIS Library)**:
- `LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000508.PDO.dll` — chưa build (source code đã có ở `MPS/MPS.Processor/MPS.Processor.Mps000508.PDO/`).
- `LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000510.PDO.dll` — chưa build (source code đã có).
- `LIB\MPSv2\MPS.PDO\MPS.Processor.Mps000512.PDO.dll` — chưa build và source code Mps000512 chưa được tạo (cần MPS team tạo project `MPS.Processor.Mps000512` + `.PDO` clone từ Mps000302).
- 5 SAR_PRINT_TYPE record (mục 1.1) — chưa thêm vào DB.

Khi 3 DLL được build và 5 SAR_PRINT_TYPE được seed, chức năng 697 sẽ hoạt động end-to-end.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-26 | sinhnt | **PTTK 2663 — mục 6.2**: Bổ sung cột "Gói bệnh nhân" (`gridColumnPatientPackage`) đặt trước cột "Gói dịch vụ". Combo `HIS_PATIENT_PACKAGE` của BN đang hoạt động (`IS_ACTIVE = 1`). Enable cho sửa/xoá khi phòng làm việc là thu ngân (`HIS_ROOM_TYPE.ID__TN`). Khi user thay đổi → gọi `api/HisSereServ/UpdatePayslipInfo` với `Field = UpdateField.PATIENT_PACKAGE_ID` (tính tiền do backend xử lý). Bổ sung `SereServADO.PATIENT_PACKAGE_NAME`, `frmBordereau.patientPackages`, `LoadPatientPackage()`, `LoadAndInItComboPatientPackage()`. Lang.vi/en/my.resx + `InitLanguage`. <br/> **Phụ thuộc backend:** cần MOS.EFMODEL thêm `HIS_PATIENT_PACKAGE` + `HIS_SERE_SERV.PATIENT_PACKAGE_ID`; MOS.SDO thêm `UpdateField.PATIENT_PACKAGE_ID`; API `HisPatientPackage/Get` + xử lý `PATIENT_PACKAGE_ID` trong `HisSereServ/UpdatePayslipInfo`. |
| 2026-05-29 | sinhnt | **PTTK 2689 — mục 3.1 + 3.5**: Bổ sung 5 lựa chọn QĐ 697/QĐ-BYT vào dropdown nút In `Bordereau` (Mps000508→Mps000512). Toàn bộ thay đổi nằm trong `HIS.Desktop.Plugins.Library.PrintBordereau` — plugin `Bordereau` tự động có menu mới qua Library, **không cần sửa code plugin**. <br/> **Mục 3.1**: Thêm 5 const trong `Base/PrintTypeCodeWorker.cs`; thêm 5 menu item trong `InitMenuProcessor.cs` (cả `InitMenuNormal` lẫn `InitMenuDynamic`), đặt ngay sau anchor 6556 tương ứng cùng ngữ cảnh. <br/> **Mục 3.5**: Tạo 3 MpsBehavior mới (`Mps000508Behavior` clone Mps000279, `Mps000510Behavior` clone Mps000281 với fetch `V_HIS_SERE_SERV_2` qua `api/HisSereServ/GetView2`, `Mps000512Behavior` clone Mps000302). Thêm 5 dispatch case trong `PrintBordereauProcessor.DelegateRunPrinter`: Mps000509 redirect printCode→Mps000508 + dùng Mps000508Behavior (pattern giống 280→279); Mps000511 redirect→Mps000510 + dùng Mps000510Behavior (giống 282→281). Cập nhật `.csproj` thêm 3 Reference MPS PDO DLL + 3 Compile Include. <br/> **Phụ thuộc chưa hoàn thành (ngoài HIS Library)**: 3 DLL `MPS.Processor.Mps000508/510/512.PDO.dll` chưa build trong `LIB\MPSv2\MPS.PDO\` (source 508/510 đã có ở MPS folder, 512 cần MPS team tạo mới); 5 SAR_PRINT_TYPE record (mục 1.1) chưa seed vào DB. |
| 2026-06-04 | sinhnt | **PTTK 2689 — sửa mã PrintTypeCode 697 cho khớp DB**: 5 mã đổi sang **VIẾT HOA** `MPS000508/509/510/511/512` (trước là `Mps000...` chữ thường → không khớp record `SAR_PRINT_TYPE` trong DB do so sánh phân biệt hoa/thường → menu bị ẩn). Giữ dãy tuần tự đúng thiết kế 1.1: 508=ngoại trú BHYT, 509=nội trú BHYT (reuse 508), 510=ngoại trú VP, 511=nội trú VP (reuse 510), 512=tổng hợp. Chỉ sửa giá trị const trong `Base/PrintTypeCodeWorker.cs` (mọi nơi tham chiếu qua const nên tự cập nhật). <br/> **Lưu ý vận hành**: deploy DLL `PrintBordereau` mới + restart app (refresh cache `SAR_PRINT_TYPE`) thì menu mới hiện; DB phải có 5 record `SAR_PRINT_TYPE` `MPS000508→512` (`IS_ACTIVE=1`, tên đúng) — riêng `MPS000508` trong DB phải là "Bảng kê ngoại trú BHYT (697)" (không để biểu khác chiếm). <br/> Đồng thời fix build (lỗi cũ): `Mps000510/512.PDO.csproj` hạ TargetFramework **v4.8 → v4.5** (khớp project tiêu thụ); đồng bộ các PDO 6556 cũ trong `LIB\MPSv2\MPS.PDO\` bằng bản mới (2026-03-05) — LIB working copy bị tụt hậu. |
| 2026-06-06 | sinhnt | **Khoa-ĐTTT (`MOS.MEDICINE_MATERIAL.USE_PAYMENT_OBJECT_BY_DEPT`) — match theo khoa chỉ định**: Đổi chiều khoa của cấu hình `HIS_DEPA_PATIENT_TYPE` từ "khoa làm việc hiện tại" sang **khoa chỉ định của từng dòng (`TDL_REQUEST_DEPARTMENT_ID`)** + bổ sung case khóa ĐTTT. `LoadDepaPatientType` bỏ lọc `DEPARTMENT_ID` khi query (lấy mọi khoa, IS_ACTIVE=1), build 3 cache: `depaServiceIdsHasConfig` (service có config bất kỳ), `depaAllowedPatyByDeptService` (key `dept_service` → tập ĐTTT, lọc combo), `depaPatientTypeDict` (key `dept_service_paty`, chỉ bản ghi IS_AUTO/IS_NOT_EXPEND → rule hao phí). Thêm enum `DepaPatientTypeMode` + `GetDepaPatientTypeMode()`/`GetDepaAllowedPaty()`. 3 case: (1) khớp khoa chỉ định → lọc combo + áp hao phí; (2) có config nhưng không khớp khoa chỉ định → **khóa combo ĐTTT chính + phụ thu** (`CustomRowCellEditForEditing` dùng repository disable); (3) không config → giữ nguyên. `GetDepaPatientTypeRule` (2 overload) + `ProcessDepaPatientTypeAfterPatientTypeChanged` keyed thêm khoa chỉ định. Dọn 3 cache trong Dispose. <br/> **Không thay đổi backend** — dùng lại API `HisDepaPatientType/Get` + filter `IS_ACTIVE`. |
| 2026-06-08 | sinhnt | **Khoa-ĐTTT — sửa lỗi HP tích nhưng DB `IS_EXPEND=null` khi mở form**: Bỏ ghi đè `IS_EXPEND` trong RAM khi **load** (`frmBordereau___Load.cs`, bỏ gọi `ApplyDepaPatientTypeRules`) và khi **reload** (`ReloadDataToGridAndPrint` trong `frmBordereau___InitMenuMouseRight.cs`). Xóa method dead `ApplyDepaPatientTypeRules`. Kết quả: mở form/reload → HP hiển thị đúng giá trị DB; cột HP vẫn **disable theo DB** ở dòng có (khoa chỉ định, service, ĐTTT) khớp rule. Rule hao phí (`IS_AUTO_EXPEND`→tích, `IS_NOT_EXPEND`→bỏ tích) chỉ áp dụng + **persist DB** (`UpdateField.IS_EXPEND`) khi user **đổi ĐTTT** (`ProcessDepaPatientTypeAfterPatientTypeChanged`, giữ nguyên). Không đổi backend. |
| 2026-05-31 | sinhnt | **PTTK 2663 — bổ sung filter combo "Gói bệnh nhân" theo SERVICE_ID dòng đang focus** (cách C, hạn chế user gán DV không thuộc gói — vốn backend reject với `HisPatientPackageDt_KhongTimThayThongTinGiaTrongGoi`). <br/> Thêm field `patientPackageDts` (List<V_HIS_PATIENT_PACKAGE_DT>); thêm `LoadPatientPackageDt()` gọi `api/HisPatientPackageDt/GetView` lọc theo danh sách `PATIENT_PACKAGE_IDs` đã load (efficient — 1 API call cho tất cả gói BN); wire vào Load flow sau `LoadPatientPackage`. <br/> Thêm method `FilterPatientPackageComboByService(editor, data)` set `ActiveFilterString` trên popup view (không động vào DataSource → display selected value vẫn ổn). Logic: nếu `data.SERVICE_ID` có ≥ 1 gói trong `patientPackageDts` chứa → filter `[ID] In (...)`; không có → `[ID] = -1` (combo rỗng). <br/> Hook vào `gridViewBordereau_ShownEditor` — branch mới cho `FocusedColumn.FieldName == "PATIENT_PACKAGE_ID"`. <br/> **Phụ thuộc backend**: `V_HIS_PATIENT_PACKAGE_DT` view + `api/HisPatientPackageDt/GetView` + `HisPatientPackageDtFilter.PATIENT_PACKAGE_IDs` (đều theo PTTK 3.1.1 + 4.1 gen code default — cùng nhóm phụ thuộc với HIS_PATIENT_PACKAGE). |

| 2026-07-04 | dangth | **PTTK 2883 — mục 1.1: vùng "CP theo ĐK lọc"**: thêm hàng label thứ 3 dưới grid hiển thị tổng chi phí theo điều kiện lọc đang hiển thị (Phải thu = SUM `VIR_TOTAL_PRICE`; Phải thu BN = SUM `VIR_TOTAL_PATIENT_PRICE`; Đã thu = SUM `VIR_TOTAL_PATIENT_PRICE` các DV đã thanh toán — màu xanh; Cần thu thêm = SUM `VIR_TOTAL_PATIENT_PRICE` các DV chưa thanh toán — màu đen). Partial mới `frmBordereau___FilterFeeSummary.cs` (`LoadFilteredFeeSummary()`), gọi sau mỗi lần gán DataSource (btnFind_Click, txtKeyword Enter, LoadDataToBorderauAndPrint/V2, ReloadDataToGridAndPrint). Designer: thu grid 24px, thêm 4 label + 5 layout item + 2 empty space. Lang.vi/en/my.resx + InitLanguage. Loại dòng dự trù máu (`isAssignBlood`) khỏi tổng. |

## 9. Test Cases

### Cột "Gói bệnh nhân" (PTTK 2663)
- [ ] Mở bảng kê ở phòng **thu ngân** → cột "Gói bệnh nhân" enable (combo gõ được).
- [ ] Mở bảng kê ở phòng **khác thu ngân** → cột "Gói bệnh nhân" disable (chỉ xem).
- [ ] BN có >= 1 gói `HIS_PATIENT_PACKAGE` đang hoạt động → combo hiện danh sách.
- [ ] BN không có gói → combo rỗng.
- [ ] Chọn 1 gói → gọi `HisSereServ/UpdatePayslipInfo` thành công → dòng DV được gán, giá tính lại (theo backend).
- [ ] API trả lỗi → giá trị cũ được rollback (UI hiển thị lại giá trị trước khi sửa).
- [ ] Bấm nút Delete trong combo → gỡ gói khỏi dòng → gọi API với `PATIENT_PACKAGE_ID = null`.
- [ ] Chuyển sang dòng khác → state hoạt động đúng (không lẫn data).

### Khoa-ĐTTT cho thuốc/VT (USE_PAYMENT_OBJECT_BY_DEPT)
- [ ] Config **tắt** → mọi dòng xử lý như cũ (không lọc, không khóa ĐTTT).
- [ ] Config **bật**, service có config + có bản ghi khớp khoa chỉ định (`TDL_REQUEST_DEPARTMENT_ID`) → combo ĐTTT chỉ hiển thị các ĐTTT được thiết lập + giá trị hiện tại.
- [ ] Bản ghi khớp có `IS_AUTO_EXPEND=1` → ô hao phí tự tích + disable; `IS_NOT_EXPEND=1` → bỏ tích + disable.
- [ ] Service có config nhưng **không** bản ghi nào khớp khoa chỉ định → **không cho đổi** ĐTTT chính lẫn phụ thu (combo disable).
- [ ] Service **không** có config → combo ĐTTT đầy đủ như cũ.
- [ ] Bảng kê có các dòng thuộc **khoa chỉ định khác nhau** → mỗi dòng match đúng theo khoa chỉ định của chính nó (không dùng khoa làm việc).
- [ ] Đổi ĐTTT thành công sang ĐTTT mới có cấu hình hao phí → `IS_EXPEND` được cập nhật theo config (gọi `UpdatePayslipInfo`).
- [ ] **Mở form**: dòng có ĐTTT (DB) khớp rule `IS_AUTO_EXPEND=1` nhưng DB `IS_EXPEND=null` → HP **bỏ tích** (đúng DB) + disable; KHÔNG còn tình trạng HP tích trong khi DB null (lỗi cũ).
- [ ] **Reload sau thao tác khác** (vd đổi nguồn khác) → HP không bị tự tích lại theo rule, vẫn đúng DB.
- [ ] Đổi ĐTTT sang rule `IS_AUTO_EXPEND=1` → HP **tích + disable**, DB `IS_EXPEND=1`; sang `IS_NOT_EXPEND=1` → HP **bỏ tích + disable**, DB `IS_EXPEND=null`.

### Vùng "CP theo ĐK lọc" (PTTK 2883 — mục 1.1)
- [ ] Mở form lần đầu (không lọc) → 4 giá trị = tổng của TOÀN BỘ danh sách đang hiển thị.
- [ ] Lọc "Từ ngày/Đến ngày" + Tìm kiếm → 4 giá trị tính LẠI chỉ trên các DV trong khoảng lọc.
- [ ] Phải thu = SUM `VIR_TOTAL_PRICE`; Phải thu BN = SUM `VIR_TOTAL_PATIENT_PRICE` các dòng hiển thị.
- [ ] Đã thu = SUM `VIR_TOTAL_PATIENT_PRICE` các dòng màu XANH (đã có `HIS_SERE_SERV_BILL` chưa hủy); Cần thu thêm = các dòng màu ĐEN. Đã thu + Cần thu thêm = Phải thu BN.
- [ ] Lọc keyword (Enter) → 4 giá trị cập nhật theo danh sách sau lọc.
- [ ] Tích "Có máu" (dòng dự trù máu đỏ hiện trên grid) → dòng máu KHÔNG tính vào 4 tổng.
- [ ] Kết quả lọc rỗng → 4 giá trị = 0.
- [ ] Vùng "Tổng hợp chi phí" (cả đợt điều trị) phía trên KHÔNG thay đổi theo lọc — vẫn theo `treatmentFees`.

### Tổng quát (regression)
- [ ] Đổi đối tượng thanh toán → vẫn hoạt động (không vỡ flow cũ).
- [ ] Đổi nguồn khác chi trả → vẫn hoạt động.
- [ ] In phiếu → vẫn hoạt động.
- [ ] Tổng tiền dưới grid cập nhật đúng sau mỗi lần sửa.
- [ ] Giao diện 1366x768: grid thu 24px vẫn hiển thị đủ, 3 hàng tổng dưới grid không đè nút In.
