# Chỉ định dịch vụ kỹ thuật (AssignService) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignService |
| Loại | Form (`frmAssignService` kế thừa `FormBase`) |
| Mục đích | Màn 7.2 — Chỉ định dịch vụ kỹ thuật (khám, XN, CĐHA, TDCN, PTTT...) cho 1 lần điều trị. Chọn dịch vụ từ cây, đặt số lượng/phòng/điều kiện, lưu thành yêu cầu dịch vụ. |
| Trạng thái | Bảo trì / mở rộng |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Chọn chẩn đoán (CĐ chính/phụ) qua HIS.UC.Icd / SecondaryIcd.
2. Tích chọn dịch vụ trên cây dịch vụ (`treeService`) → chuyển sang grid đã chọn (`gridControlServiceProcess`, DataSource = `List<SereServADO>` giữ trong `ServiceIsleafADOs`).
3. Đặt số lượng, phòng xử lý, đối tượng thanh toán, điều kiện.
4. Lưu → tạo `HIS_SERVICE_REQ` + `HIS_SERE_SERV`.

### Đưa dịch vụ vào grid bằng code (cơ chế dùng chung)
Tìm dịch vụ trong `ServiceIsleafADOs` theo `SERVICE_ID` → set `IsChecked = true`, `AMOUNT`, đối tượng, phòng → reload grid. Nguồn dùng: nhóm DV (`SelectOneServiceGroupProcess`), gói KSK (`ShowKskServiceProcess`), và **gói bệnh nhân** (`OnPatientPackageServicesSelected`).

### Gói bệnh nhân (Màn 7.2 — bổ sung)
- Nút **"Gói bệnh nhân"** (`btnPatientPackage`, hàng "Tờ điều trị") mở popup `frmPatientPackage`.
- Popup:
  - Trái: gói của bệnh nhân (`HIS_PATIENT_PACKAGE`, `IS_ACTIVE = 1`) — Tên gói, Ngày ĐK, Ghi chú + 4 cột audit.
  - Phải: dịch vụ trong gói (`V_HIS_PATIENT_PACKAGE_DT`), **loại trừ thuốc/vật tư/máu/suất ăn** (`SV_SERVICE_TYPE_ID != THUOC/VT/MAU/AN`). Cột: checkbox, Mã DV, Tên DV, Loại DV, Trong gói (`AMOUNT`), Đã dùng (`AMOUNT_USED`), Lần này (mặc định 1).
  - Nút **Chọn** → trả DV đã tích về form cha → đưa ra grid chỉ định, gán tên gói đại diện.
- Cột **"Gói bệnh nhân"** (read-only, **unbound runtime**) sau cột "Điều kiện" trong grid chỉ định, lấy giá trị từ `Dictionary<long,string> patientPackageNameByServiceId` theo `SERVICE_ID`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu dịch vụ |
| HIS_SERE_SERV / V_HIS_SERE_SERV | Table/View | Dịch vụ thực hiện (base của `SereServADO`) |
| HIS_PATIENT_PACKAGE | Table | Gói bệnh nhân (grid trái popup) |
| V_HIS_PATIENT_PACKAGE_DT | View | Chi tiết DV trong gói (base của `PatientPackageDtADO`) |

### ADO bổ sung
- `PatientPackageDtADO` (kế thừa `V_HIS_PATIENT_PACKAGE_DT`) + `IsChecked`, `AmountThisTime (=1)`, `PATIENT_PACKAGE_NAME`.
- **KHÔNG sửa** `SereServADO` (file dùng chung). Tên gói đại diện lưu trong `Dictionary<long,string>` ngay trong plugin; cột "Gói bệnh nhân" là **cột unbound**, fill qua handler `CustomUnboundColumnData` riêng (đăng ký thêm, không sửa handler gốc).

## 4. UI Layout

### Form chính (frmAssignService)
```
+--------------------------------------------------------------------------------+
| CĐ chính/phụ | ... | Người tư vấn (label 75px) | TK | Gói KSK | Gói DV |       |
| Ghi chú | Tờ điều trị [..] x + | [Gói bệnh nhân] |  Không tự chọn DV          |
+--------------------------------------------------------------------------------+
| Grid DV đã chọn: Mã DV | ... | Điều kiện | Gói bệnh nhân | Lần thứ            |
+--------------------------------------------------------------------------------+
| Thanh toán | Tạm thu | ... | Lưu (Ctrl S) | Sửa | In | Mới |                   |
+--------------------------------------------------------------------------------+
```

### Popup frmPatientPackage (1366×768 hoặc lớn hơn)
```
+----------------------------------+  +-------------------------------------------+
| Danh sách gói dịch vụ            |  | Dịch vụ trong gói                         |
| Tìm: [Tên gói...]                |  | Tìm: [Mã/tên DV...]                       |
| # | Tên gói | Ngày ĐK | Ghi chú  |  | ☑ | Mã DV | Tên DV | Loại | SL | Đã | Lần|
|   | ... + 4 cột audit            |  |                                           |
+----------------------------------+  +-------------------------------------------+
|                                          [Hủy bỏ]  [Chọn] |
```

### UC sử dụng
HIS.UC.Icd, HIS.UC.SecondaryIcd, HIS.UC.DateEditor, Inventec.UC.Paging (giữ nguyên).

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy gói bệnh nhân | `RequestUriStore.HIS_PATIENT_PACKAGE_GET` = `api/HisPatientPackage/Get` | MosConsumer | `HisPatientPackageFilter` (PATIENT_ID, IS_ACTIVE) |
| Lấy DV trong gói | `RequestUriStore.HIS_PATIENT_PACKAGE_DT_GETVIEW` = `api/HisPatientPackageDt/GetView` | MosConsumer | `HisPatientPackageDtViewFilter` (PATIENT_PACKAGE_ID, IS_ACTIVE) |

> BE đã làm `api/HisPatientPackage/Get`. `api/HisPatientPackageDt/GetView` cần BE bổ sung (cuộc test ngày 28/05 trả 404).

## 6. Dependencies
Inter-plugin / Library giữ nguyên (không thay đổi cho tính năng gói bệnh nhân).

## 7. Print
Không thay đổi.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** (dùng chung ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`, ngưỡng trống coi như 0); (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí (hồ sơ + đang kê) vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY` theo hiệu lực FROM_TIME/TO_TIME) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp (chỉ log). Mặc định 2 key tắt — không đổi hành vi hiện tại. |
| 28/05/2026 | tuanln | Bổ sung tính năng **Gói bệnh nhân**: nút mở popup `frmPatientPackage` (gói trái + DV gói phải, loại trừ thuốc/VT/máu/suất ăn, cột "Lần này" mặc định 1), đưa DV được chọn ra grid chỉ định, thêm cột read-only "Gói bệnh nhân" (unbound) sau cột "Điều kiện". Thêm `PatientPackageDtADO`, filter POCO, 2 URI gói bệnh nhân. **Không** sửa file dùng chung `SereServADO` — dùng Dictionary theo `SERVICE_ID` + cột unbound trong plugin. Căn lại nhãn "Người tư vấn" cho cân (TextSize 90→75). Cột checkbox trong popup: Caption rỗng + AllowSort=False. |
| 31/05/2026 | tuanln | Popup `frmPatientPackage`: thêm cột **Đơn giá** (`UNIT_PRICE`, format `#,##0`, căn phải, read-only, VisibleIndex=4) giữa "Loại DV" và "Trong gói". Thêm key `frmPatientPackage.gColDtUnitPrice.Caption` ở 3 file Lang (vi/en/my). |
| 31/05/2026 | tuanln | Grid chỉ định chính: DV inject từ gói BN nay hiển thị **Đơn giá = `UNIT_PRICE` của gói** (không phải giá default của DV). Thêm dict `patientPackageUnitPriceByServiceId`, override `e.Value` cho cột `PRICE_DISPLAY` trong handler `gridViewServiceProcess_CustomUnboundColumnData_PatientPackage` (chạy sau handler gốc → ghi đè). Cleanup khi uncheck DV (cả tree + grid). |
| 31/05/2026 | tuanln | Fix lỗi save fail "DV không tồn tại chính sách giá tương ứng với ĐTTT": khi inject DV từ gói, pre-check `BranchDataWorker.HasServicePatyWithListPatientType(SERVICE_ID, [PATIENT_PACKAGE_PATIENT_TYPE_ID])` — nếu DV không có config giá theo ĐTTT của gói → **fallback** dùng `currentHisPatientTypeAlter.PATIENT_TYPE_ID` (ĐTTT mặc định BN). DV vẫn link với gói qua `PATIENT_PACKAGE_ID`. |
| 12/06/2026 | tuanln | Popup `frmPatientPackage`: (1) Fix cột **Ngày ĐK** hiển thị literal `dd/MM/yyyy` thay vì giá trị — chuyển sang unbound `REGISTER_DATE_STR`, format `long → "dd/MM/yyyy"` qua `Inventec.Common.DateTime.Convert.TimeNumberToTimeString(REGISTER_DATE).Substring(0,10)` trong handler `gridViewPackage_CustomUnboundColumnData` (nguyên nhân: `REGISTER_DATE` kiểu `Int64` (yyyyMMddHHmmss), DevExpress `FormatType.DateTime` không tự cast long → DateTime). (2) Tăng kích thước popup `ClientSize` (984×561) → (1280×760), `SplitContainer.Size` (984×521) → (1280×720), `SplitterPosition` 480 → 580; cập nhật size con (grids, search box, label) + reposition nút Chọn/Hủy bỏ. |
| 29/07/2026 | tuanln | PT-44730: ĐTTT mặc định của dịch vụ theo **ĐT bệnh nhân + ĐT phụ thu** (bảng mới `HIS_SERVICE_DEFAULT_PATY`, chờ backend). Thêm partial `frmAssignService__Plus__ServiceDefaultPaty.cs` (worker nạp cấu hình 1 lần/form, `GetDefaultPatientTypeIdByServiceConfig`, `IsAllowEditPatientTypeByServiceConfig`). Trong `ChoosePatientTypeDefaultlService` chèn bước tra cấu hình **sau** khối `sereServADO.DEFAULT_PATIENT_TYPE_ID` và **trước** khối `DO_NOT_USE_BHYT` → cấu hình mới ưu tiên trên ĐTTT mặc định khai ở danh mục DV; đối tượng tra được phải nằm trong `currentPatientTypeTemps` (BN được hưởng + DV có khai giá), không hợp lệ thì rơi về luồng cũ, không báo lỗi. `gridViewServiceProcess_CustomRowCellEdit` cột `PATIENT_TYPE_ID`: dịch vụ có khai cấu hình + không đủ quyền theo key `HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption` → dùng `repositoryItemCboPatientTypeReadOnly`. Bảng cấu hình rỗng hoặc API chưa có → giữ nguyên toàn bộ hành vi hiện tại. Cờ `DO_NOT_USE_BHYT` không sửa, chạy song song |

## 9. Test Cases

### Gói bệnh nhân
- [ ] BN chưa chọn → bấm "Gói bệnh nhân" → cảnh báo, không mở popup.
- [ ] Mở popup → grid trái hiển thị đúng gói `IS_ACTIVE = 1` của BN, có 4 cột audit.
- [ ] Chọn 1 gói → grid phải hiển thị dịch vụ, KHÔNG có thuốc/vật tư/máu/suất ăn.
- [ ] Tìm kiếm gói (tên/ghi chú) và tìm dịch vụ (mã/tên) lọc đúng.
- [ ] Tích dịch vụ, sửa "Lần này", bấm Chọn → DV ra grid chỉ định với đúng số lượng, cột "Gói bệnh nhân" hiển thị tên gói.
- [ ] Không tích dịch vụ nào → bấm Chọn → cảnh báo "Vui lòng chọn ít nhất một dịch vụ trong gói".
- [ ] DV trong gói không có trong danh mục phòng → cảnh báo liệt kê mã DV.
