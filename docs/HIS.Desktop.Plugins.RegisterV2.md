# RegisterV2 (Tiếp đón 2) — Tài Liệu Module

> Tài liệu khởi tạo tập trung vào nghiệp vụ kiểm tra cổng BHYT & cập nhật Địa chỉ liên hệ.
> Các phần khác của plugin chưa được khảo sát đầy đủ — bổ sung khi có thay đổi liên quan.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.RegisterV2 |
| Loại | UC (màn hình Tiếp đón 2) |
| Mục đích | Tiếp nhận bệnh nhân: nhập thông tin BN, kiểm tra thẻ BHYT (cổng BHXH), đăng ký khám, tạm ứng, in phiếu |
| Cập nhật gần nhất | 02/06/2026 — huannh |

## 2. Quy Trình Nghiệp Vụ — Kiểm tra cổng BHYT & cập nhật thông tin

### Luồng cập nhật theo cổng BHYT
```
Nhân viên check thẻ BHYT (cổng BHXH) qua Library CheckHeinGOV
  → Cổng phát hiện thông tin nhập (họ tên / địa chỉ / nơi ĐKBĐ...) khác cổng
    → HeinGOVManager hiển thị dialog xác nhận
      → Nhân viên nhấn "Đồng ý"  (set IsThongTinNguoiDungThayDoiSoVoiCong__Choose = true)
        → UCRegister.CheckTTFull cập nhật thông tin theo cổng:
            • Họ tên, ngày sinh, giới tính, thông tin BHYT (ucHeinInfo) — luôn cập nhật
            • Địa chỉ liên hệ (Tỉnh/Huyện/Xã/Địa chỉ chi tiết) — theo cấu hình bên dưới
```

### Cấu hình: giữ Địa chỉ liên hệ khi cập nhật theo cổng BHYT

| KEY | `MOS.HIS_DESKTOP_PLUGINS_REGISTERV2.KEEP_CONTACT_ADDRESS_ON_BHYT_CHECK` |
|-----|------------------------------------------------------------------------|
| Loại | HisConfigs (HIS_CONFIG — toàn viện), kiểu string |
| Đọc khi | Mở màn hình Tiếp đón 2 (constructor `UCRegister`) → field `isKeepContactAddressOnBhytCheck` |
| = 1 (BẬT) | Giữ nguyên Địa chỉ liên hệ (Tỉnh + Huyện + Xã + Địa chỉ chi tiết) nhân viên đã nhập. Các thông tin khác (họ tên, ngày sinh, giới tính, BHYT) **vẫn cập nhật** theo cổng |
| ≠ 1 (mặc định, kể cả chưa khởi tạo = 0) | Giữ hành vi hiện tại: ghi đè toàn bộ địa chỉ liên hệ theo địa chỉ trên thẻ BHYT cổng trả về |

**Điều kiện nghiệp vụ:**
- Quyết định rẽ nhánh đặt **trước** điều kiện ghi đè địa chỉ đã có (`IsAddress || IsThongTinNguoiDungThayDoiSoVoiCong__Choose` + `CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1`). Điều kiện cũ **không đổi** → backward compatible tuyệt đối khi config = 0.
- Không thay đổi giao diện.

## 3. EFMODEL / Dữ liệu liên quan

| Entity / Type | Mục đích |
|---------------|----------|
| ResultHistoryLDO (His.Bhyt.InsuranceExpertise.LDO) | Dữ liệu thẻ trả về từ cổng BHXH — field `diaChi`, `hoTen`, `maThe`, `maDKBD`, `gioiTinh`, `ngaySinh` |
| HeinCardData (Inventec.Common.QrCodeBHYT) | Dữ liệu thẻ BHYT |
| HIS.UC.AddressCombo.ADO.UCAddressADO | DTO địa chỉ Tỉnh/Huyện/Xã + Địa chỉ chi tiết (control `ucAddressCombo1`) |
| V_SDA_PROVINCE / V_SDA_DISTRICT / V_SDA_COMMUNE | Danh mục địa giới hành chính (AddressProcessor tách địa chỉ) |
| HIS_PATIENT_TYPE_ALTER | `CO_PAID_ACCUMULATE_AMOUNT` (long?), `PAID_6_MONTH` (C/K), `FREE_CO_PAID_TIME` (long? `yyyyMMdd`) — cập nhật từ cổng BHYT |
| HIS_BHYT_PARAM | `BASE_SALARY` — nguồn tính ngưỡng 06 tháng lương cơ sở |
| ResultMCCTADO (Library.CheckHeinGOV) | Kết quả tra cứu tiền cùng chi trả — `DataCCT[]`, `GhiChu`, `MaKetQua` |

## 4. Dependencies

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.CheckHeinGOV | Kiểm tra thẻ BHYT qua cổng BHXH, dialog xác nhận cập nhật theo cổng |
| HIS.Desktop.Plugins.Library.RegisterConfig | Cấu hình dùng chung của màn hình tiếp đón (HisConfigCFG, AppConfigs) |
| HIS.UC.AddressCombo | Combo Tỉnh/Huyện/Xã |
| HIS.UC.UCPatientRaw | Nhập thông tin bệnh nhân |
| HIS.UC.UCHeniInfo | Vùng thẻ BHYT (F3). `SetCoPaidAccumulateFromGov()` nhận kết quả tra cứu MCCT và tính 3 trường cùng chi trả |
| His.Bhyt.InsuranceExpertise (repo `common`) | `ApiInsuranceExpertise.TraCuuTienMCCT()` — HTTP header + body JSON |

## 5. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 25/08/2026 | khainq | **Cập nhật tiền cùng chi trả / miễn cùng chi trả qua cổng BHYT** (`api/TraCuuCCT/TraCuuTienMCCT`). Tự động gọi sau khi kiểm tra thẻ thành công (theo cấu hình `HIS.CHECK_HEIN_CARD.BHXH__AUTO_CHECK_MCCT`), kèm nút tra cứu thủ công trên ô lũy kế. Suy ra 3 trường `CO_PAID_ACCUMULATE_AMOUNT` / `PAID_6_MONTH` / `FREE_CO_PAID_TIME`. Thêm ô `txtCoPaidAccumulate` vào group `BHYT (F3)` (hàng mới `y=218`, group 239→263), `ADO/CoPaidMcctADO.cs`, `Design/UCHeinInfo__CoPaidMCCT.cs`, `GetCurrentBhytParam()`, cờ `IsAutoCheck`, 10 câu thông báo vi/en/my (`HIS.UC.UCHeniInfo`); móc `CheckTienMCCT` trong `UCRegister__CheckHeinGOV.cs` + nối delegate trong `UCRegister__SetDelegate.cs` (`RegisterV2`). **Tôn trọng cấu hình `IsNotAutoCheck5Y6M`** — khi bật thì không tự tick checkbox 6 tháng. Đường dẫn API cố định trong code, không có config. Thiết kế: `docs/B_KyThuat_TraCuuTienMCCT_CungChiTraLuyKe.md` Phần 5 |
| 04/08/2026 | sinhnt | Bổ sung mục **5b — Cấu hình đang áp dụng cho màn hình Tiếp đón 2**: liệt kê 18 khóa cấu hình tài khoản/máy trạm + ~75 khóa cấu hình hệ thống theo 8 nhóm, và 2 cơ chế chi phối đối tượng bệnh nhân nằm ở danh mục phòng tiếp đón (`HIS_RECEPTION_ROOM.DEFAULT_PATIENT_TYPE_ID`, `PATIENT_TYPE_IDS`) — không tra được bằng cách tìm khóa `HIS_CONFIG` |
| 04/08/2026 | sinhnt | **Tự động chuyển đối tượng bệnh nhân sang BHYT** khi bệnh nhân tìm được có thẻ BHYT (áp dụng tất cả viện, KHÔNG có cấu hình bật/tắt). Thêm `RunV3/UCRegister__AutoPatientTypeBhyt.cs` (`ProcessAutoSetPatientTypeBhytByHeinCard` + kiểm tra hiệu lực thẻ + nạp dự phòng vùng BHYT từ dữ liệu thẻ trong SDO), gọi trong `UCRegister.FillDataAfterSearchPatientInUCPatientRaw` — đặt SAU mọi cơ chế đối tượng mặc định và TRƯỚC các hàm nạp thông tin thẻ. Loại trừ đối tượng BHYT/Quân nhân; chỉ chặn khi biết chắc thẻ đã hết hạn; không chuyển được thì giữ nguyên + ghi log. Giữ nguyên cơ chế tự chuyển khi quét QR cho bệnh nhân mới. Tham chiếu `PTTK_XXXXX_Tu_Dong_Chuyen_Doi_Tuong_BHYT_Khi_Co_The.md` |
| 02/06/2026 | huannh | Thêm cấu hình `MOS.HIS_DESKTOP_PLUGINS_REGISTERV2.KEEP_CONTACT_ADDRESS_ON_BHYT_CHECK`: khi = 1 thì giữ nguyên Địa chỉ liên hệ nhân viên đã nhập khi đồng ý cập nhật thông tin theo cổng BHYT; mặc định (≠1) giữ hành vi cũ. Sửa `UCRegister__CheckHeinGOV.cs` (thêm nhánh guard trước khối ghi đè địa chỉ) + đọc config khi mở form trong `UCRegister.cs` |
| 02/06/2026 | huannh | Thêm mục menu In ấn "In gộp dịch vụ khám" (gọi **MPS000515**, gộp tất cả phòng khám của BN). Chỉ hiển thị khi config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` = 1. Sửa `Config/HisConfigCFG.cs` (method `IsEnablePrintMergedExamService()` đọc config on-demand), `RunV3/UCRegister__Print.cs` (enum `InGopDvKham` + menu item + `DelegateRunPrinterInGopDichVuKham` gọi `Mps000515PDO`), resource key `Plugin_Register_Title_InGopDichVuKham` (vi/en/my) + `ResourceMessage.Title_InGopDichVuKham`, csproj reference `MPS.Processor.Mps000515.PDO` |

## 5b. Cấu hình đang áp dụng cho màn hình Tiếp đón 2

> Khảo sát 04/08/2026. Nguồn: `RegisterV2/Config/` (HisConfigCFG, AppConfigs, BHXHLoginCFG, GateAndStepCFG) + `Library.RegisterConfig/` (HisConfigCFG, AppConfigs).
> **Chưa bao gồm** cấu hình riêng của các UC nhúng trong màn hình (UCPatientRaw, UCHein, UCOtherServiceReqInfo, UCServiceRoomInfo, AddressCombo) và của các Library gọi kèm (CheckHeinGOV, PrintServiceReq...).
> Cột "Đã xác minh" = đã đọc code xác nhận hành vi trong phiên khảo sát; các key còn lại chỉ liệt kê tên, **không suy đoán hành vi**.

### A. Cấu hình tài khoản / máy trạm — `ConfigApplicationWorker.Get<T>()`

| KEY | Đã xác minh |
|-----|-------------|
| `CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE` | ✔ Mã đối tượng bệnh nhân mặc định. Che cơ chế đối tượng mặc định của phòng tiếp đón (chỉ khi key này trống mới lấy theo phòng) |
| `CONFIG_KEY__DEFAULT_CONFIG_IS_NOT_REQUIRE_FEE` | |
| `CONFIG_KEY__ALERT_EXPRIED_TIME_HEIN_CARD_BHYT` | |
| `CONFIG_KEY__CHE_DO_IN_PHIEU_DANG_KY_DICH_VU_KHAM_BENH` | |
| `CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_CPA` | |
| `CONFIG_KEY__DANG_KY_TIEP_DON__HIEN_THI_THONG_BAO_TIM_THAY_BN_THEO_THONG_TIN_NHAP` | ✔ Hiện thông báo "tìm được 1 bệnh nhân..." sau khi tìm thấy |
| `CONFIG_KEY__DANG_KY_TIEP_DON__THOI_GIAN_LOAD_DANH_SACH_PHONG_KHAM` | |
| `CONFIG_KEY__FILL_DU_LIEU_TU_DONG_VAO_O_DIA_CHI_BENH_NHAN_CHIP_THE_MAN_HINH_DANG_KY` | ✔ Có tự fill địa chỉ trên thẻ vào ô địa chỉ bệnh nhân hay không |
| `CONFIG_KEY__HIEN_THI_NOI_LAM_VIEC_THEO_DINH_DANG_MAN_HINH_DANG_KY` | |
| `CONFIG_KEY__HIS_DESKTOP__CHANGE_ETHNIC` | |
| `CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO` | |
| `CONFIG_KEY__HIS_DESKTOP__REGISTER__OWE_TYPE_DEFAULT` | |
| `CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_DEPOSIT_SERVICE` | |
| `CONFIG_KEY__HIS_DESKTOP__REGISTER__SHOW_LINE_FIRST_ADDRESS` | |
| `CONFIG_KEY__HIS_DESKTOP__REGISTER__TIME__AUTO___CALL_REGISTER_REQ` | |
| `CONFIG_KEY__INSURANCEEXPERTISE_CHECKHEINCONFIG` | |
| `CONFIG_KEY__IS_USE_HID_SYNC` | |
| `CONFIG_KEY__TIEP_DON_HIEN_THI_THONG_TIN_THEM` | |

### B. Cấu hình hệ thống toàn viện — `HisConfigs.Get<T>()` (bảng `HIS_CONFIG`)

**B1. Đối tượng bệnh nhân**

| KEY | Đã xác minh |
|-----|-------------|
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` | ✔ Mã đối tượng BHYT — dùng để xác định đối tượng BHYT, không cố định ID trong code |
| `HIS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` | ✔ Key dự phòng cùng mục đích (đọc khi key `MOS.*` trống) |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN` | ✔ Mã đối tượng Quân nhân |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK` | |
| `HIS.Desktop.Plugins.Register.UsingPatientTypeOfPreviousPatient` | ✔ Bấm "Mới" thì giữ đối tượng của bệnh nhân liền trước thay vì xóa trắng |
| `HIS.DESKTOP.REGISTER__DEFAULT_PATIENT_TYPE_CODE_IS_NOT_REQUIRE_EXAM_FEE` | |
| `HIS_DESKTOP_REGISTER__ROOM_CODES__PATIENT_TYPE` | |
| `MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE` | ✔ Hiện/ẩn ô đối tượng thanh toán chính (phụ thu) |
| `HIS.Desktop.Plugins.RegisterV2.PrimaryPatientTypeByService` | |
| `HIS.Desktop.Plugins.RegisterV2.WarningHeinPatientTypeCode` | |

**B2. Thẻ BHYT / cổng BHXH**

`HIS.Desktop.Plugins.Register.IsCheckHeinCard` (✔ chế độ tự kiểm tra thẻ trên cổng BHXH) · `HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS` · `HIS.DESKTOP.REGISTER.HEIN_CARD.NOT_CHECK_EXPIRED.IS_SHOW` · `HIS.Desktop.Plugins.RegisterV2.IsRequiredToUpdateNewBhytCardInCaseOfExpiry` · `HIS.Desktop.Plugins.Register.WarningInvalidCheckHistoryHeinCard` · `HIS.Desktop.Plugins.IsBlockingInvalidBhyt` · `MOS.HIS_PATIENT_TYPE_ALTER.NOT_AUTO_CHECK_5_YEAR_6_MONTH` (✔ không tự tích 5 năm / 6 tháng) · `HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG` · `HIS.UC.UCHein.IsTempQN` · `HIS.Desktop.WarningOverExamBhyt` · `HIS.Desktop.Plugins.RegisterV2.WarningOverMonthsTransfer` · `MOS.HIS_DESKTOP_PLUGINS_REGISTERV2.KEEP_CONTACT_ADDRESS_ON_BHYT_CHECK` (mục 2)

**B3. Tuyến / thông tuyến**

`HIS.Desktop.Plugins.Register.IsDefaultRightRouteType` · `HIS.Desktop.Plugins.IsAllowedRouteTypeByDefault` · `HIS.Desktop.Plugins.Register.NotDisplayedRouteTypeOver` · `HIS.Desktop.Plugins.Register.IsNotRequiredRightTypeInCaseOfHavingAreaCode` · `HIS.Desktop.Plugins.Register.IsAutoShowTransferFormInCaseOfAppointment`

**B4. Phòng khám / đăng ký khám / số thứ tự**

`HIS.Desktop.Plugins.Register.IsShowingExamRoomInArea` · `HIS.Desktop.Plugins.Register.IsShowingExamRoomInDepartment` · `HIS.HIS_DESKTOP_REGISTER.EXECUTE_ROOM_CODE.SHOW` · `HIS.Desktop.Plugins.RegisterV2.FocusExecuteRoomOption` · `HIS.Desktop.Plugins.Register.ByPassTextboxRoomCode` · `HIS.Desktop.Plugins.Register.SetDefaultRequestRoomByExamRoomWhenAssigningService` · `HIS.Desktop.Plugins.RegisterV2.IsDefaultTreatmentTypeExam` (✔ mặc định loại điều trị = Khám) · `MOS.HIS_SERVICE_REQ.NUM_ORDER_ISSUE_OPTION` · `MOS.HIS_SERVICE_REQ.RESERVED_NUM_ORDER` · `HIS.Desktop.Plugins.Register.AutoFocusToSavePrintAfterChoosingExam` · `HIS.IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM`

**B5. Thông tin bệnh nhân / validate / danh mục mặc định**

`HIS.Desktop.Plugins.RegisterV2.PhoneRequired` · `MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD` (✔ bắt buộc thông tin người nhà với trẻ < 6 tuổi) · `MOS.HIS_PATIENT.CCCD_NUMBER.CHECK_DUPLICATION` · `HIS.DESKTOP.REGISTER.VALIDATE__ETHNIC` · `HIS.DESKTOP.REGISTER.VALIDATE__T_H_X` · `HIS.Desktop.Plugins.Register.HideAddressLevel` · `HIS.Desktop.Plugins.Register.RelativesInforOption` · `HIS.Desktop.Plugins.RegisterV2.EditOldPatientInformationOption` · `HIS.Desktop.Plugins.Register.IsNotAutoFocusOnExistsPatient` · `HIS.Desktop.Plugins.Register.SuggestCardHolderInformationByUsingPhoneNumber` · `HIS.HIS_DESKTOP_REGISTER.VISIBILITY_CONTROL_FOR_TIM` · `MOS.HR.ADDRESS` · `HIS.DESKTOP.VVN_KYC.IS_USING_RECOGNITION` · `RAE.HIS_GENDER_CODE__BASE` · `EXE.HIS_CAREER_CODE__BASE` · `EXE.HIS_CAREER_CODE__UNDER_6_AGE` · `HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS` · `EXE.ETHNIC_CODE_BASE` · `EXE.NATIONAL_CODE_BASE`

**B6. Kiểm tra trùng / nợ / đơn thuốc / đợt điều trị**

`HIS.Desktop.Plugins.Register.IS_CHECK_EXAM_HISTORY_TODAY` · `HIS.Desktop.Plugins.Register.IsCheckExamination` · `MOS.HIS_TREATMENT.CHECK_PREVIOUS_DEBT_OPTION` · `MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT` · `MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION` · `MOS.HIS_TREATMENT.IS_CHECK_TODAY_FINISH_TREATMENT` · `MOS.HIS_TREATMENT.IS_MANUAL_IN_CODE` · `HIS.Desktop.Plugins.RegisterV2.IsAllowProgramPatientOld` · `MOS.HIS_TREATMENT.GUARANTEE_CONNECTION_INFO` · `HIS.Desktop.Plugins.ExamServiceReqExecute.InHospitalizationReasonRequired`

**B7. In ấn / hóa đơn / tạm ứng**

`EXE.SERVICE_REQUEST_REGISTER.IS_PRINT_AFTER_SAVE` · `EXE.SERVICE_REQUEST_REGISTER.IS_VISIBLE_BILL` · `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` (mục 7) · `HIS.Desktop.Plugins.Register.AutoCheckPrintExam.PatientTypeCode` · `HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT`

**B8. Khác**

`HIS.Desktop.Plugins.AutoCheckIcd` · `HIS.Desktop.ApplyRestoreLayout.ModuleLinks` · `HIS.Desktop.FormClosingOption` · `HIS.Desktop.FormClosingOption.ModuleLinkApply` · `HIS.DESKTOP.CALL_PATIENT_CPA.OPTION` · `HIS.IS_DANG_KY_QUA_TONG_DAI`

### C. Cơ chế chi phối đối tượng bệnh nhân KHÔNG phải khóa cấu hình

Tra bằng cách tìm khóa `HIS_CONFIG` sẽ **không thấy** 2 cơ chế sau — chúng nằm ở danh mục **phòng tiếp đón** (`HIS_RECEPTION_ROOM`):

| Cột | Tác dụng |
|-----|----------|
| `DEFAULT_PATIENT_TYPE_ID` | Đối tượng mặc định của phòng tiếp đón — chỉ áp dụng khi cấu hình tài khoản `CONFIG_KEY__DEFAULT_CONFIG_PATIENT_TYPE_CODE` để trống |
| `PATIENT_TYPE_IDS` | Giới hạn danh sách đối tượng hiển thị trong ô Đối tượng của phòng. Đối tượng không nằm trong danh sách thì **không đặt được bằng code** (hàm đặt đối tượng chỉ ghi log rồi bỏ qua) |

## 6. Test Cases

### Tra cứu tiền cùng chi trả (MCCT)

- [ ] `MaKetQua=200`, lũy kế > LIMIT, có đợt vượt ngưỡng → điền đủ 3 trường, TDMC CT = ngày ra viện đợt vượt **lần đầu**
- [ ] `MaKetQua=204` → **giữ nguyên** 3 control, không xóa giá trị đang có
- [ ] Cấu hình `IsNotAutoCheck5Y6M` bật → điền lũy kế và TDMC CT nhưng **không tự tick** checkbox 6 tháng
- [ ] Mã thẻ 11 ký tự → chặn tại client, không gọi cổng
- [ ] Cổng timeout → không treo form, luồng kiểm tra thẻ vẫn hoàn tất bình thường
- [ ] Số cổng khác số trên form → hộp thoại Có/Không; chọn Không thì giữ nguyên
- [ ] `HIS_BHYT_PARAM` không có bản ghi hiệu lực → chỉ điền lũy kế
- [ ] Lũy kế vượt ngưỡng mà TDMC CT trống → bấm Lưu bị chặn kèm cảnh báo
- [ ] `BHXH__AUTO_CHECK_MCCT=0` → không gọi tự động, nút tra cứu thủ công vẫn chạy
- [ ] Làm mới form → ô lũy kế reset trắng
- [ ] Lưu xong kiểm tra DB: `CO_PAID_ACCUMULATE_AMOUNT` có giá trị


- [ ] Config chưa khởi tạo (không có key) → check cổng + Đồng ý → địa chỉ ghi đè theo cổng (hành vi cũ)
- [ ] Config = 0 → check cổng + Đồng ý → địa chỉ ghi đè theo cổng (hành vi cũ)
- [ ] Config = 1 → nhập địa chỉ liên hệ tay → check cổng + Đồng ý → Tỉnh/Huyện/Xã/Địa chỉ chi tiết **giữ nguyên**; họ tên/ngày sinh/giới tính/BHYT vẫn cập nhật theo cổng
- [ ] Config = 1 → đổi giá trị config khi đang mở form → chỉ có hiệu lực sau khi mở lại màn hình (config đọc lúc mở form)

### In gộp dịch vụ khám
- [ ] Config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` ≠ 1 → mở menu In ấn → KHÔNG có mục "In gộp dịch vụ khám"
- [ ] Config = 1 → mở menu In ấn → có mục "In gộp dịch vụ khám" ngay dưới "In dịch vụ khám", trên "In phiếu yêu cầu khám"
- [ ] Config = 1 → chưa đăng ký khám / đang ở chế độ thêm mới → chọn mục → hiện thông báo "không có dữ liệu đăng ký khám" và dừng
- [ ] Config = 1 → đã đăng ký nhiều phòng khám → chọn mục → phiếu MPS000515 gộp đủ tất cả phòng khám của BN
- [ ] Checkbox "Xem trước" bật → hiện preview; tắt + CheDoInChoCacChucNangTrongPhanMem = 2 → in trực tiếp

## 7. Print — In ấn

| Loại in | PrintTypeCode | MPS / PDO | Ghi chú |
|---------|--------------|-----------|---------|
| In dịch vụ khám (phòng được chọn) | Mps000001 | PrintServiceReqProcessor | Không đổi |
| In gộp dịch vụ khám (tất cả phòng) | Mps000515 | Mps000515PDO | Chỉ khi config `HIS.REGISTERV2.PRINT_MERGED_EXAM_SERVICE` = 1 |
| In phiếu yêu cầu khám | Mps000309 | Mps000309PDO | |
| In thẻ bệnh nhân | Mps000178 | Mps000178PDO | |
| Bảng kiểm trước tiêm chủng | Mps000358 | Mps000358PDO | |
| In biên lai/hóa đơn | Mps000420 | Mps000420PDO | |

**Gọi PDO MPS000515 (In gộp dịch vụ khám):** RegisterV2 gọi
`Mps000515PDO(V_HIS_PATIENT currentPatient, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, HIS_TREATMENT treatment, List<V_HIS_SERVICE_REQ> serviceReqs, List<V_HIS_SERE_SERV> sereServs, string gate)`
với `serviceReqs`/`sereServs` = toàn bộ phòng khám BN đã đăng ký lần này (lấy từ `currentHisExamServiceReqResultSDO`, không lọc theo dòng chọn), `gate` = số quầy từ `txtGateNumber`.
