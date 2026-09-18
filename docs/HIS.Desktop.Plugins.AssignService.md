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

### Dự trù dịch vụ (ngày + giờ)
- Ô **Dự trù** (`txtDutruTime`) chọn 1 hoặc nhiều ngày qua popup lịch (`popupControlContainer4` / `calendarControl1`), hiển thị `dd/MM;dd/MM`. Ô **giờ dự trù** (`timeDutru`, `HH:mm`, mặc định 00:00) ngay bên phải, **một giờ dùng chung cho mọi ngày** (giống TG chỉ định chế độ Nhiều ngày).
- `USE_TIME` = `yyyyMMdd` + `HHmm` + `00` cho từng ngày (`BuildDutruUseTimes`), gửi qua `AssignServiceSDO.UseTimes`; BE tạo mỗi ngày dự trù một `HIS_SERVICE_REQ` với `USE_TIME` tương ứng. Không chọn ngày → không gửi `UseTimes`.
- Ẩn với diện Khám; khóa cả 2 ô khi tick **Nhiều ngày** ở TG chỉ định. Dịch vụ loại Khám/Khác không cho dự trù (validate khi Lưu).

### Xác nhận phòng xử lý khi lưu (config, mặc định tắt)
- Key `HIS.Desktop.Plugins.AssignService.ConfirmExecuteRoomWhenSave` = `1` → bật; khác `1`/không khai báo → giữ nguyên hành vi cũ (`HisConfigCFG.IsConfirmExecuteRoomWhenSave`).
- Điểm chèn: `ConfirmExecuteRoomBeforeSave(serviceCheckeds__Send)` gọi ở **cuối chuỗi validate**, ngay trong `if (isValid)` trước `ChangeLockButtonWhileProcess(false)` — 2 nơi: `ProcessSaveData` (luồng 1 BN, phủ Lưu/Lưu In/Lưu Ký/Lưu hiển thị) và nhánh `assignMulti` trong `frmAssignService.cs` (chỉ định nhiều BN).
- Gom `TDL_EXECUTE_ROOM_ID` của các dòng `IsChecked`, gộp phòng trùng + đếm số DV (`ExecuteRoomConfirmADO`), sort theo số DV giảm dần rồi chuỗi hiển thị. Hiển thị dạng `EXECUTE_ROOM_CODE + " - " + EXECUTE_ROOM_NAME` (thiếu mã → chỉ tên, thiếu tên → chỉ mã), lấy từ `DataSource` của `repositoryItemcboExcuteRoom_TabService` (đã gồm buồng bệnh), fallback `allDataExecuteRooms`.
- Cột **Mã dịch vụ**: `TDL_SERVICE_CODE` của các DV về phòng đó, gom theo thứ tự dòng trên lưới, `String.Join(", ", ...)`. **Không loại trùng** để số mã khớp cột "Số dịch vụ" (1 DV chỉ định nhiều lần = nhiều dòng).
- Dòng `TDL_EXECUTE_ROOM_ID <= 0` → danh sách "Dịch vụ chưa chọn phòng xử lý", hiển thị dạng `"MÃ DV - Tên DV"`. Nếu `HisConfigCFG.IsAssignRoomByLoadBalance` (BE tự phân phòng) → **xóa danh sách này** vì để trống là đúng thiết kế.
- Popup `frmConfirmExecuteRoom` (modal, focus mặc định **Không đồng ý**), grid 3 cột: Phòng xử lý (260) | Số dịch vụ (70, căn phải) | Mã dịch vụ (200). Đồng ý → `IsAgreed = true` lưu tiếp; Không đồng ý / đóng bằng X → dừng lưu, giữ nguyên form và dữ liệu đang nhập.
- Chỉ xác nhận — KHÔNG validate DV↔phòng, KHÔNG đổi logic gán phòng mặc định. Exception khi dựng popup → log Error và cho lưu tiếp (không chặn nghiệp vụ).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu dịch vụ |
| HIS_SERE_SERV / V_HIS_SERE_SERV | Table/View | Dịch vụ thực hiện (base của `SereServADO`) |
| HIS_PATIENT_PACKAGE | Table | Gói bệnh nhân (grid trái popup) |
| V_HIS_PATIENT_PACKAGE_DT | View | Chi tiết DV trong gói (base của `PatientPackageDtADO`) |

### ADO bổ sung
- `PatientPackageDtADO` (kế thừa `V_HIS_PATIENT_PACKAGE_DT`) + `IsChecked`, `AmountThisTime (=1)`, `PATIENT_PACKAGE_NAME`.
- `ExecuteRoomConfirmADO` (POCO thuần) — `EXECUTE_ROOM_DISPLAY` (dạng `"MÃ - Tên phòng"`), `SERVICE_COUNT`, `SERVICE_CODES` (mã DV ngăn cách `", "`): 1 dòng phòng xử lý đã gộp trên popup xác nhận trước khi lưu.
- **KHÔNG sửa** `SereServADO` (file dùng chung). Tên gói đại diện lưu trong `Dictionary<long,string>` ngay trong plugin; cột "Gói bệnh nhân" là **cột unbound**, fill qua handler `CustomUnboundColumnData` riêng (đăng ký thêm, không sửa handler gốc).

## 4. UI Layout

### Form chính (frmAssignService)
```
+--------------------------------------------------------------------------------+
| CĐ chính/phụ | ... | Người tư vấn (label 75px) | TK | Gói KSK | Gói DV |       |
| Ghi chú | Tờ điều trị [..] x + | [Gói bệnh nhân] |  Không tự chọn DV          |
| TG chỉ định [dd/MM/yyyy][HH:mm] [x] Nhiều ngày | Dự trù [dd/MM;dd/MM 📅][HH:mm] | TK ...       |
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
| 11/08/2026 | sinhnt | Xác nhận **phòng xử lý** trước khi lưu chỉ định (config `HIS.Desktop.Plugins.AssignService.ConfirmExecuteRoomWhenSave` = 1, mặc định tắt). Thêm `ExecuteRoomConfirmADO`, popup `frmConfirmExecuteRoom` (grid Phòng xử lý + Số dịch vụ, memo "Dịch vụ chưa chọn phòng xử lý", nút Đồng ý / Không đồng ý — focus mặc định **Không đồng ý**, đóng bằng X = không lưu), partial `frmAssignService__ConfirmExecuteRoom.cs` (gom + gộp phòng trùng, tra tên phòng từ DataSource combo cột Phòng xử lý → fallback `allDataExecuteRooms`). Chèn 1 dòng gọi vào `if (isValid)` của `ProcessSaveData` và nhánh `assignMulti` → phủ mọi luồng lưu. Khi `IsAssignRoomByLoadBalance` bật (BE tự phân phòng) thì bỏ phần liệt kê dòng chưa chọn phòng để tránh cảnh báo nhiễu. Thêm 7 key ở 3 file Lang (vi/en/my). Config tắt → thoát sớm, không tổng hợp dữ liệu, hành vi lưu giữ nguyên. |
| 18/09/2026 | sinhnt | Việc 57754 (TTMB-TK-56263): thêm ô **Giờ dự trù** (`timeDutru`, TimeSpanEdit mask `HH:mm`, sao y ô giờ của TG chỉ định) ngay bên phải ô Dự trù, layout item `lciTimeDutru` (412,100 / 87×26), `emptySpaceItem7` co còn 97px. `USE_TIME` gửi BE đổi từ `yyyyMMdd000000` thành `yyyyMMddHHmm00`, một giờ dùng chung cho mọi ngày dự trù đã chọn (`BuildDutruUseTimes`, gọi lại khi đổi giờ); **mặc định 00:00** nên không nhập giờ thì dữ liệu y như bản cũ. Ô giờ đồng bộ Enabled/Visibility với ô ngày (khóa khi tick Nhiều ngày, ẩn với diện Khám). Không sửa BE: `MakeServiceReq` lưu nguyên `useTime`, `VerifyUseTimeWithIntructionTime` chỉ so phần ngày. Thêm key tooltip `frmAssignService.lciTimeDutru.OptionsToolTip.ToolTip` (vi/en/my). |

## 9. Test Cases

### Giờ dự trù (57754)
- [ ] BN nội trú: hàng Dự trù có ô giờ `00:00` ngay bên phải ô ngày, thẳng cột với ô giờ TG chỉ định.
- [ ] Chọn 1 ngày, không sửa giờ, Lưu → `USE_TIME` = `yyyyMMdd000000` (giống bản cũ).
- [ ] Chọn 3 ngày, bấm Đồng ý → focus nằm ở ô giờ; gõ `08:30`, Lưu → 3 y lệnh `USE_TIME …083000`.
- [ ] Nhập giờ trước rồi mới chọn ngày → `USE_TIME` mang đúng giờ đã nhập.
- [ ] Đổi giờ sau khi đã chọn ngày → `USE_TIME` dựng lại theo giờ mới.
- [ ] Nhập giờ nhưng không chọn ngày → y lệnh thường, `USE_TIME` null.
- [ ] Xóa trắng ô ngày → không gửi `UseTimes`; ô giờ giữ giá trị.
- [ ] Tick Nhiều ngày ở TG chỉ định → ô ngày + ô giờ dự trù cùng mờ; bỏ tick → cùng sáng lại.
- [ ] BN diện Khám → ẩn cả ô ngày và ô giờ dự trù.
- [ ] Dự trù cùng ngày chỉ định + giờ 08:00 (TG chỉ định 10:00) → BE lưu OK; Tờ điều trị xếp vào nhóm "Dự trù ngày…" (hành vi mới, xác nhận với viện).
- [ ] Chỉ định nhiều BN với dự trù 2 ngày + 08:30 → mỗi BN 2 y lệnh `…083000`.
- [ ] Sửa thời gian y lệnh (ServiceReqUpdateInstruction) hiển thị `dd/MM/yyyy 08:30`; Sửa y lệnh (AssignServiceEdit) chọn lại ngày → giờ về 00:00 (hạn chế đã biết).
- [ ] Enter ở ô giờ dự trù → sang control kế tiếp; tooltip "Giờ dự trù".

### Xác nhận phòng xử lý khi lưu
- [ ] Config tắt / chưa khai báo → nhấn Lưu → lưu ngay, không hiện popup.
- [ ] Config bật, chỉ định nhiều DV nhiều phòng → nhấn Lưu → popup liệt kê đủ các phòng đang chọn; Đồng ý → lưu thành công.
- [ ] Config bật, phát hiện sai phòng → Không đồng ý → không lưu, form giữ nguyên dữ liệu đang nhập, các nút thao tác hoạt động lại bình thường.
- [ ] Nhiều DV cùng 1 phòng → popup chỉ 1 dòng phòng đó, cột Số dịch vụ đúng số lượng, cột Mã dịch vụ liệt kê đủ mã DV cách nhau `", "`.
- [ ] Phòng xử lý hiển thị đúng dạng `MÃ - Tên phòng`; DV chưa chọn phòng hiển thị `MÃ DV - Tên DV`.
- [ ] Có dòng DV chưa chọn phòng xử lý → popup hiện thêm vùng "Dịch vụ chưa chọn phòng xử lý" liệt kê đúng tên DV.
- [ ] Bật `MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_PRIORITY_OPTION` = 1 (BE tự phân phòng) → popup KHÔNG hiện vùng "Dịch vụ chưa chọn phòng xử lý".
- [ ] Nút Lưu In / Lưu Ký → popup vẫn hiện; Không đồng ý → không lưu, không in, không ký.
- [ ] Chỉ định nhiều bệnh nhân (assignMulti) → popup hiện 1 lần trước khi lưu cả loạt.
- [ ] Đóng popup bằng nút X → coi như Không đồng ý, không lưu.
- [ ] Popup mở lên focus sẵn nút "Không đồng ý"; nhấn Enter ngay → không lưu.

### Gói bệnh nhân
- [ ] BN chưa chọn → bấm "Gói bệnh nhân" → cảnh báo, không mở popup.
- [ ] Mở popup → grid trái hiển thị đúng gói `IS_ACTIVE = 1` của BN, có 4 cột audit.
- [ ] Chọn 1 gói → grid phải hiển thị dịch vụ, KHÔNG có thuốc/vật tư/máu/suất ăn.
- [ ] Tìm kiếm gói (tên/ghi chú) và tìm dịch vụ (mã/tên) lọc đúng.
- [ ] Tích dịch vụ, sửa "Lần này", bấm Chọn → DV ra grid chỉ định với đúng số lượng, cột "Gói bệnh nhân" hiển thị tên gói.
- [ ] Không tích dịch vụ nào → bấm Chọn → cảnh báo "Vui lòng chọn ít nhất một dịch vụ trong gói".
- [ ] DV trong gói không có trong danh mục phòng → cảnh báo liệt kê mã DV.
