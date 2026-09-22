# Kê Đơn Cận Lâm Sàng (AssignPrescriptionCLS) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AssignPrescriptionCLS |
| Loại | Form (`frmAssignPrescription` kế thừa `FormBase`) |
| Mục đích | Kê đơn thuốc/vật tư phục vụ chỉ định cận lâm sàng |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ — Chẩn đoán (Việc 2.6)

Chẩn đoán **chính** và **phụ** dùng control tùy chỉnh (không qua UC), nên xử lý 2.6 trực tiếp trong plugin (KHÔNG áp dụng cho YHCT — `IS_TRADITIONAL`):

- **Cảnh báo không khuyến khích bệnh chính** (`IS_NOT_RECOMMEND_MAIN = 1`): chỉ cảnh báo khi user **chọn/sửa** chẩn đoán chính (`ChangecboChanDoanTD`, `LoadIcdCombo`). Hiển thị "Bệnh {0} không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?". Chọn Không → xóa, chọn lại. Không cảnh báo khi hiển thị dữ liệu đã lưu.
- **Loại bỏ chẩn đoán nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`) khỏi bệnh chính và bệnh phụ ở MỌI đường vào:
  - Danh sách chọn (dropdown `cboIcds`, popup `frmSecondaryIcd`): không hiển thị.
  - Gõ tay/chọn (`LoadIcdCombo`, `ChangecboChanDoanTD` cho chính; `CheckIcdWrongCode` cho phụ): báo "Bệnh {0} là nguyên nhân tử vong, không được dùng làm chẩn đoán chính/phụ." + loại khỏi ô.
  - Load hồ sơ đã lưu (`LoadIcdToControl` cho chính; `LoadDataToIcdSub`/`LoadIcdToControlIcdSub` qua helper `RemoveDeathCauseFromSubIcd` cho phụ): bỏ qua, không đổ vào ô.
- **Không có kiểm tra khi lưu (B)**: plugin này KHÔNG có luồng kết thúc điều trị (đã comment), nên không áp dụng kiểm tra death-cause khi lưu — việc chặn ở các đường nhập/load nêu trên là cơ chế duy nhất.

## 3. EFMODEL
HIS_ICD (`IS_DEATH_CAUSE_ONLY`, `IS_NOT_RECOMMEND_MAIN`, `IS_TRADITIONAL`), HIS_EXP_MEST, HIS_SERVICE_REQ.

## 4. Files chính (Việc 2.6)
- `AssignPrescription/frmAssignPrescription__InitUC.cs` — nạp `currentIcds`.
- `AssignPrescription/frmAssignPrescription__InitUCIcd.cs` — bind combo bệnh chính (lọc death-cause).
- `AssignPrescription/frmAssignPrescription.cs` — `ChangecboChanDoanTD`/`LoadIcdCombo` (cảnh báo A1), `frmSecondaryIcd` (lọc death-cause A2).
- `Resources/ResourceMessage.cs` + `Message.Lang.vi/en.resx` — message `BenhKhongKhuyenKhichDungLamBenhChinh`.

## 5. Changelog

| Ngày | Người sửa | Mô tả |
|------|-----------|-------|
| 22/09/2026 | dangth | **Việc 56273 (PT-56273) — Tự động xuất thuốc, vật tư đi kèm khi thực hiện DVKT** (tích hợp tính năng viện Nghệ An, code + mô tả do anh Bùi Nam gửi qua Zalo; bản Nghệ An là decompile chỉ có đoạn AutoClose). (1) Key mới `HIS.Desktop.Plugins.AssignPrescriptionCLS.AutoClose` (giữ đúng tên Nghệ An) → `HisConfigCFG.IsAutoCloseAfterSave`; **= 1** thì sau Lưu thành công (cả Lưu / Lưu In / Lưu Xem) form tự `Close()` — đặt **sau** `WaitingManager.Hide` + `MessageManager.Show` trong `ProcessSaveData` (`__Save.cs`); bản Nghệ An dùng `!= "0"` (chưa khai key cũng đóng) và Close trước 2 bước đó → đổi thành opt-in để viện khác không đổi hành vi. (2) Checkbox **"Tự động lưu"** `chkAutoSave` ngay bên trái nút Lưu (Designer: 3 nút Lưu mẫu/Lưu xem/Lưu In dịch trái 100px, `emptySpaceItem3` 528→428), trạng thái nhớ **trên máy trạm** (SQLite ControlState của client, như mọi checkbox nhớ trạng thái khác của HIS) qua `HIS.Desktop.Library.CacheClient.ControlStateWorker` (KEY = `chkAutoSave`, MODULE_LINK = `HIS.Desktop.Plugins.AssignPrescriptionCLS`) — file mới `frmAssignPrescription__AutoSave.cs`, thêm Reference CacheClient vào csproj, caption/tooltip đa ngữ `frmAssignPrescription.chkAutoSave.Text/.ToolTip` (vi/en). (3) **Tự lưu**: chỉ khi mở từ luồng thực hiện DVKT (`isOpenFromServiceExecute` = `IsCabinet` + có `SereServ` + không phải sửa đơn: nút Tủ trực ở Thực hiện dịch vụ, Kê đơn tủ trực ở Thực hiện XN, Kê đơn CLS kê mới ở Phòng thực hiện) và lưới đã có thuốc/VT đi kèm (`HIS_SERVICE_METY/MATY`) và checkbox tick → cuối `frmAssignPrescription_Load` gọi `TryAutoSaveAttachedMediMaty()` → `BeginInvoke` async: **chờ** `afterLoadWarningTask` (Task gộp 2 kiểm tra async của Load có thể `Close()` form: nợ viện phí `CheckWarningOverTotalPatientPrice` + trần BHYT `LoadTotalSereServByHeinWithTreatment`, gán ở `FillSomePatientInfoSelectedInFormGeneralAfterLoad` trong `__Load.cs`, 2 hàm vẫn fire-and-forget như cũ) → kiểm lại `IsDisposed/Visible/actionType/btnSave.Enabled` → `ProcessSaveData(SAVE)`, tương đương bấm Lưu nên mọi kiểm tra (ICD, MIMS…) vẫn chạy. Lưới có dòng **vượt tồn kho** (`AmountAlert > 0` / `ErrorTypeMediMatyBean = Warning`, Load đã cảnh báo) thì **không** gọi lưu để khỏi hiện 2 hộp cảnh báo liên tiếp. Khi không lưu được (còn `ActionAdd` sau ProcessSaveData, hoặc bỏ qua vì tồn kho) hiện alert nhẹ `ResourceMessage.ChuaTuLuuDuocThuocVatTuDiKem` (key mới `Message.Lang.vi/en.resx`) để người dùng biết vì sao form còn mở. Review 3 góc nhìn (22/09) bắt được race giữa BeginInvoke và 2 kiểm tra async → đã xích chờ Task như trên. Checkbox **ẩn** ở luồng khác (Kê đơn CLS từ Danh sách y lệnh / Tờ điều trị, Sửa đơn). SQL key: `PTTK\56273_insert_his_config.sql`; thiết kế: `PTTK\56273 - Thiet ke - Tu dong xuat thuoc vat tu di kem khi thuc hien DVKT.md/.docx`. |
| 09/09/2026 | khainq | **Việc 52540 — Kiểm tra tương tác thuốc giữa các đơn khác nhau của hồ sơ bằng MIMS.** Khi lưu đơn (và menu chuột phải "Đánh giá thông tin thuốc"), ngoài thuốc đơn đang kê còn gửi MIMS các thuốc **còn hiệu lực của các đơn KHÁC** trong hồ sơ. 3 config mới (mặc định TẮT — giữ nguyên hành vi cũ): `HIS.Desktop.Mims.InteractionScopeOption` (rỗng/1 = chỉ đơn hiện tại · 2 = cùng hồ sơ điều trị `TDL_TREATMENT_ID` · 3 = toàn lịch sử bệnh nhân `TDL_PATIENT_ID`), `HIS.Desktop.Mims.PreviousPrescriptionDayRange` (mặc định 30), `HIS.Desktop.Mims.CrossPrescriptionRequestMode` (1 = gộp `<Prescribing>` · 2 = khối `<Prescribed>`). Nguồn dữ liệu `api/HisExpMestMedicine/GetView` (`V_HIS_EXP_MEST_MEDICINE`), prefetch async khi mở form, cache theo giờ chỉ định. Lọc: `IS_DELETE != 1`, loại đơn đang sửa (`oldServiceReq.ID`), loại thuốc trùng đơn hiện tại, `USE_TIME_TO >= giờ chỉ định` (thuốc không có `USE_TIME_TO` → theo `TDL_INTRUCTION_TIME` trong N ngày), `Distinct` theo `MEDICINE_TYPE_CODE`, tối đa 30 thuốc. Chỉ hiển thị cảnh báo liên quan thuốc đang kê bằng tham số form MIMS `alertfilterbydrug` (`<GUIDS>` GUID thuốc đơn hiện tại); popup được chèn thêm khối HTML "Thuốc đang dùng từ đơn khác trong hồ sơ" (tên thuốc, ngày kê, ngày dùng đến, mã đơn/khoa/BS kê). Cặp chống chỉ định VN cũng được kiểm tra trên tập thuốc gộp. Log `HIS_MIMS_INTERACTION_LOG` bổ sung `CHECKED_GUIDS` (gộp GUID đơn khác), `REQUEST_PARAMS`, `NOTE`. Lỗi lấy dữ liệu đơn khác KHÔNG chặn lưu đơn. Thư viện `HIS.Desktop.MIMS.Integration`: `MimsClient.PostXml` nhận tham số form phụ; `MimsRequestBuilder.BuildDrugHealthAlertRequest(..., previousDrugs, requestMode)` + `BuildAlertFilterByDrug`; model mới `MimsPreviousDrugInfo`, `MimsCrossPrescriptionOption`; `DrugHealthService` thêm overload `Check`/`CheckAndAlert`/`ShowResultAsync` — **giữ nguyên toàn bộ chữ ký cũ** (binary-compat với 6 plugin đang dùng). Files: `Config/HisConfigCFG.cs`, `AssignPrescription/frmAssignPrescription__MimsCrossPrescription.cs` (MỚI), `frmAssignPrescription.cs` (gọi prefetch cạnh `PrefetchMimsPatientProfile`), `__Save.cs` (`CheckMIMS`), `__InitMenuMouseRight.cs`, `Resources/Message.Lang.{vi,en}.resx` + `ResourceMessage.cs`, csproj. Build sạch. |
| 29/07/2026 | nampp | **MIMS Drug Pregnancy/Lactation** — Truyền `PatientProfile` (PN mang thai / cho con bú) vào request MIMS khi lưu đơn (`CheckMIMS` — `__Save.cs`) và menu chuột phải "Đánh giá thông tin thuốc". Config mới `HIS.Desktop.Mims.IsCheckPregnancyLactation` (mặc định TẮT — request MIMS không đổi). BN nữ: prefetch async `HIS_MIMS_PATIENT_PROFILE` khi Load (cạnh `LoadAllergenic`); có tick → build profile (Gender F, Age từ `TDL_PATIENT_DOB`, Pregnancy.Month, Nursing) truyền `CheckAndAlert(..., patientProfile:)` — cảnh báo thai kỳ/cho con bú trả về trong CÙNG request/popup. Files: `Config/HisConfigCFG.cs`, `AssignPrescription/frmAssignPrescription__MimsPatientProfile.cs` (MỚI), `frmAssignPrescription.cs`, `__Save.cs`, `__InitMenuMouseRight.cs`; csproj fix HintPath `HIS.Desktop.MIMS.Integration` về `lib\HIS\...` (trước trỏ `IVT TEST\histest` không tồn tại). |
| 28/07/2026 | nampp | Ho\u00e0n thi\u1ec7n 46465 theo test th\u1ef1c t\u1ebf: (1) c\u1ea3nh b\u00e1o v\u01b0\u1ee3t t\u1ea1m \u1ee9ng ngo\u1ea1i tr\u00fa ch\u1ec9 n\u1ed5 1 l\u1ea7n l\u00fac m\u1edf form (guard theo treatmentId), kh\u00f4ng n\u1ed5 l\u1ea1i sau L\u01b0u; b\u1ea5m n\u00fat M\u1edbi th\u00ec reset guard \u0111\u1ec3 c\u1ea3nh b\u00e1o l\u1ea1i; (2) ti\u1ec1n trong popup format vi-VN d\u1ea5u ch\u1ea5m, l\u00e0m tr\u00f2n s\u1ed1 nguy\u00ean (#,##0); (3) YHCT/Kidney: fix cross-thread (b\u1ecdc Invoke) v\u00e0 chuy\u1ec3n g\u1ecdi check t\u1eeb Task.Run \u0111\u1ea7u lu\u1ed3ng Load xu\u1ed1ng cu\u1ed1i lu\u1ed3ng \u0111\u1ec3 c\u1ea3nh b\u00e1o vi\u1ec7n ph\u00ed n\u1ed5 SAU c\u00e1c c\u1ea3nh b\u00e1o d\u1ecbch v\u1ee5. |
| 23/07/2026 | nampp | Việc 46465: bổ sung 2 cảnh báo viện phí theo config mới — (1) key `HIS.Desktop.WarningOverTotalPatientPrice__IsCheckOutpatient` = 1: mở rộng cảnh báo thiếu viện phí (vượt tạm ứng) cho BN **điều trị ngoại trú** (dùng chung ngưỡng `HIS.Desktop.WarningOverTotalPatientPrice`, ngưỡng trống coi như 0); (2) key `HIS.Desktop.WarningOver15PercentBaseSalary__IsCheckExam` = 1: khi Lưu, cảnh báo BN **diện khám** nếu tổng chi phí (hồ sơ + đang kê) vượt 15% Lương cơ bản (`HIS_BHYT_PARAM.BASE_SALARY` theo hiệu lực FROM_TIME/TO_TIME) — hàm mới `ValidFee15PercentBaseSalaryForExam()`, message mới `TongChiPhiVuot15PhanTramLuongCoBan` (vi/en). Bỏ qua BN bảo lãnh; thiếu Tham số BHYT hoặc lỗi check thì cho đi tiếp (chỉ log). Mặc định 2 key tắt — không đổi hành vi hiện tại. |
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi danh sách chọn bệnh chính + phụ (giữ giá trị đã lưu, trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa bệnh chính; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi/en). Không có kiểm tra khi lưu (plugin không có kết thúc điều trị). |
| 17/06/2026 | huyvu20 | **Việc 2.6 (bổ sung)**: `IS_DEATH_CAUSE_ONLY` vẫn lọt qua khi gõ tay & khi load hồ sơ đã lưu. Chặn nốt: gõ/chọn bệnh chính (`LoadIcdCombo`, `ChangecboChanDoanTD`) + gõ bệnh phụ (`CheckIcdWrongCode`) → báo + loại; load bệnh chính (`LoadIcdToControl`) + phụ (`LoadDataToIcdSub`, `LoadIcdToControlIcdSub` qua helper `RemoveDeathCauseFromSubIcd`) → bỏ qua không load. Thêm message `BenhLaNguyenNhanTuVongKhongDuocDungLamChanDoan` (vi/en). |

## 6. Test Cases

### Tự động lưu thuốc/vật tư đi kèm — việc 56273 (22/09/2026)
- [ ] Viện CHƯA khai key, CHƯA tick "Tự động lưu": mọi màn (Tủ trực, Kê đơn CLS từ DS y lệnh, Sửa đơn) chạy y như cũ — form mở, bấm Lưu, form vẫn mở.
- [ ] Mở từ Thực hiện dịch vụ → Tủ trực: có ô "Tự động lưu" ngay trái nút "Lưu (Ctrl S)", hàng nút không nhảy dòng, không chồng.
- [ ] Mở Kê đơn CLS từ Danh sách y lệnh / Tờ điều trị / nhánh Sửa đơn ở Phòng thực hiện: ô "Tự động lưu" **ẩn**, khoảng trống tự lấp.
- [ ] Tick "Tự động lưu" → đóng form → mở lại: vẫn tick; tài khoản khác đăng nhập **cùng máy trạm** cũng thấy tick (nhớ theo máy, như các checkbox nhớ trạng thái khác của HIS).
- [ ] Tủ trực + tick + DV có cấu hình HIS_SERVICE_METY/MATY + kho mặc định đủ tồn → form mở rồi tự lưu ngay (thông báo thành công), không phải bấm Lưu; log `56273 AutoSave: tu luu N dong...`.
- [ ] Tủ trực + tick + BN **thiếu viện phí** (viện bật cảnh báo nợ viện phí) → hộp hỏi "thiếu viện phí… tiếp tục?" hiện **trước**; chọn Không → form đóng, **không** lưu; chọn Có → tự lưu. Tương tự cảnh báo vượt trần BHYT.
- [ ] Tủ trực + tick + 1 dòng **vượt tồn kho** → chỉ 1 cảnh báo tồn kho của Load + alert nhẹ "Chưa tự động lưu được thuốc/vật tư đi kèm…"; KHÔNG gọi lưu, form vẫn mở để sửa số lượng; log `56273 AutoSave: bo qua vi co dong vuot ton kho`.
- [ ] Tủ trực + tick + thiếu ICD / MIMS chặn → hộp thoại / icon lỗi như khi bấm Lưu tay + alert nhẹ "Chưa tự động lưu được…"; form **vẫn mở**, không tự lưu lại.
- [ ] Tủ trực + tick nhưng DV KHÔNG có METY/MATY hoặc không có kho mặc định → lưới rỗng, KHÔNG tự lưu, không thêm thông báo mới; log `56273 AutoSave: bo qua...`.
- [ ] Key AutoClose = 1 + Lưu thành công (tay hoặc tự) → thông báo thành công hiện xong form tự đóng; không hỏi "thuốc chưa lưu".
- [ ] Key AutoClose = 1 + Lưu In / Lưu Xem → in/preview xong rồi form đóng.
- [ ] Key AutoClose = 1 + Lưu thất bại (BE lỗi) → form KHÔNG đóng.
- [ ] Key AutoClose rỗng/khác 1 → sau lưu form vẫn mở như cũ.
- [ ] Phòng thực hiện (ExecuteRoom) kê mới + tick + key=1: sau tự lưu, lưới Phòng thực hiện đổi trạng thái y lệnh (callback `DlgProcessDataResult`) rồi form đóng.

### Việc 2.6 (chẩn đoán)
- [ ] Combo bệnh chính/phụ KHÔNG hiển thị ICD nguyên nhân tử vong.
- [ ] Gõ tay/chọn ICD nguyên nhân tử vong vào bệnh chính → báo + xóa, không nhận.
- [ ] Gõ tay ICD nguyên nhân tử vong vào bệnh phụ → báo + loại mã đó.
- [ ] Mở hồ sơ đã lưu có ICD nguyên nhân tử vong ở chính/phụ → mã đó bị bỏ khỏi ô (không load).
- [ ] Chọn/gõ ICD chính có cờ `IS_NOT_RECOMMEND_MAIN` → cảnh báo; chọn Không → xóa. Mở hồ sơ đã lưu → KHÔNG cảnh báo.
- [ ] YHCT không bị ảnh hưởng.
