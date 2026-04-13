---
description: Hướng dẫn sử dụng 134 User Controls — public API, giao diện, khi nào dùng, code mẫu. ƯU TIÊN UC có sẵn thay vì tạo mới
paths:
  - "HIS/Plugins/**"
  - "UC/**"
---

# Hướng Dẫn Sử Dụng User Controls

Source: `hisnguonmo/UC/`. LUÔN kiểm tra UC có sẵn trước khi tự tạo control.

---

## Pattern Chung

```csharp
// Processor → InitADO → Run → Panel → GetValue/Reload
var proc = new IcdProcessor(commonParam);
var ado = new IcdInitADO();
ado.DataIcds = icdList;
UserControl uc = (UserControl)proc.Run(ado);
panelIcd.Controls.Add(uc);
uc.Dock = DockStyle.Fill;

// Các thao tác chung
proc.GetValue(uc);              // Lấy data
proc.SetValue(uc, newData);     // Đặt data
proc.Reload(uc, newInputADO);   // Reload
proc.FocusControl(uc);          // Focus
proc.ReadOnly(uc, true);        // Chỉ đọc
proc.ValidationIcd(uc);         // Validate
proc.ResetValidate(uc);         // Xóa lỗi
```

---

## A. CHẨN ĐOÁN / ICD

### HIS.UC.Icd — Chẩn đoán chính (105 plugins)
**Giao diện**: ComboBox + grid popup. Gõ mã/tên ICD để tìm. Color validation (đỏ/xanh).
**Khi nào dùng**: Mọi form cần chẩn đoán chính (khám, kê đơn, ra viện).

| Processor Method | Return | Mô tả |
|-----------------|--------|-------|
| Run(IcdInitADO) | UC | Tạo |
| GetValue(uc) | IcdInputADO | Lấy ICD_CODE, ICD_NAME |
| SetValue(uc, data) | void | Đặt ICD |
| Reload(uc, IcdInputADO) | void | Reload |
| FocusControl(uc) | void | Focus |
| ReadOnly(uc, bool) | void | Chỉ đọc |
| SetRequired(uc, bool) | void | Bắt buộc |
| ValidationIcd(uc) | bool | Validate |
| ValidationIcdWithMessage(uc, errEmpty, errOther) | bool | Validate + lỗi |
| ResetValidate(uc) | void | Xóa lỗi |
| ResetValidationIcd(uc) | void | Reset messages |

**InitADO**: DataIcds, IcdInput, Height/Width, IsColor, IsYHCT, AutoCheckIcd, DepamentId, Template, DelegateNextFocus, DelegateRefeshIcd, DelegateRefreshSubIcd, DelegateCheckICD

### HIS.UC.SecondaryIcd — Chẩn đoán phụ (124 plugins)
**Giao diện**: TextBox + popup grid multi-select. Nhiều ICD, cách dấu phẩy.
**Khi nào dùng**: LUÔN đi kèm HIS.UC.Icd.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(SecondaryIcdInitADO) | UC | Tạo |
| GetValue(uc) | object | Lấy codes/names |
| SetValue(uc, data) | void | Đặt |
| Reload(uc, data) | void | Reload |
| FocusControl(uc) | void | Focus |
| ReadOnly(uc, bool) | void | Chỉ đọc |

### HIS.UC.Sick — Bệnh/Nghỉ ốm
**Giao diện**: ComboBox dropdown đơn giản.
**Khi nào dùng**: Chọn bệnh khi không cần ICD đầy đủ.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(SickInitADO) | UC | Tạo |
| GetValue(uc) | object | Lấy data |
| SetValue(uc, data) | void | Đặt |
| Reload(uc, HIS_TREATMENT) | void | Reload theo điều trị |
| FocusControl(uc) | void | Focus |
| ReadOnly(uc, bool) | void | Chỉ đọc |
| ValidControl(uc) | bool | Validate |

### HIS.UC.UCCauseOfDeath — Nguyên nhân tử vong
**Giao diện**: ComboBox.
**Khi nào dùng**: Form ghi nhận tử vong.

---

## B. THUỐC / VẬT TƯ / TỒN KHO

### HIS.UC.ExpMestMedicineGrid — Lưới thuốc xuất/nhập (60 plugins)
**Giao diện**: Grid lớn: tên thuốc, SL, đơn vị, giá, tồn kho, lô, hạn dùng. Edit inline.
**Khi nào dùng**: Form kê đơn, xuất/nhập/duyệt thuốc.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(ExpMestMedicineInitADO) | UC | Tạo |
| Reload(uc, List\<V_HIS_EXP_MEST_MEDICINE\>) | void | Reload thuốc |

**InitADO**: ListExpMestMedicine, ListExpMestMedicineColumn, IsShowSearchPanel, CustomUnboundColumnData delegate

### HIS.UC.ExpMestMaterialGrid — Lưới vật tư xuất/nhập (54 plugins)
**Giao diện**: Tương tự ExpMestMedicineGrid cho vật tư.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(ExpMestMaterialInitADO) | UC | Tạo |
| Reload(uc, List\<V_HIS_EXP_MEST_MATERIAL\>) | void | Reload |

### HIS.UC.MedicineTypeInStock — Thuốc tồn kho (23 plugins)
**Giao diện**: Grid thuốc còn tồn. Lọc theo kho, nhóm. Checkbox multi-select.
**Khi nào dùng**: Chọn thuốc khi kê đơn — chỉ hiện hàng còn.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(MedicineTypeInStockInitADO) | UC | Tạo |
| Reload(uc, List\<HisMedicineTypeInStockSDO\>) | void | Reload |
| Search(uc) | void | Tìm kiếm |
| GetListCheck(uc) | List\<MedicineTypeInStockADO\> | Lấy danh sách đã check |
| FocusKeyword(uc) | void | Focus ô tìm kiếm |

### HIS.UC.MaterialTypeInStock — Vật tư tồn kho (23 plugins)
**Giao diện + API**: Tương tự MedicineTypeInStock cho vật tư.

### HIS.UC.HisMedicineInStock — Thuốc tồn kho (chi tiết lô)
**Khi nào dùng**: Xem chi tiết theo lô, hạn dùng.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(HisMedicineInStockInitADO) | UC | Tạo |
| Reload(uc, List\<HisMedicineInStockSDO\>) | void | Reload |
| GetDataGridView(uc) | object | Lấy data |
| FocusKeyword(uc) | void | Focus tìm kiếm |

### HIS.UC.HisMaterialInStock — Vật tư tồn kho (chi tiết lô)
**API**: Tương tự HisMedicineInStock.

### HIS.UC.Medicine — Danh mục thuốc
**Giao diện**: Grid với right-click menu, search, radio enable, checkbox.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(MedicineInitADO) | UC | Tạo |
| Reload(uc, List\<MedicineADO\>) | void | Reload |
| GetDataGridView(uc) | List\<MedicineADO\> | Lấy data |

**InitADO đặc biệt**: MedicinType (List\<V_HIS_MEDICINE_TYPE\>), delegates cho MouseDown, RadioClick, CheckChanged, RowCellClick, MouseRightClick, ReloadRowChoose

### HIS.UC.Material — Danh mục vật tư
**API**: Tương tự Medicine (standard grid).

### HIS.UC.MediStock — Chọn kho (12 plugins)
**Giao diện**: Grid danh sách kho.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(MediStockInitADO) | UC | Tạo |
| Reload(uc, List\<MediStockADO\>) | void | Reload |
| GetDataGridView(uc) | List\<MediStockADO\> | Lấy data |
| GetGridControl(uc) | GridControl | Truy cập grid trực tiếp |

### HIS.UC.ConflictActiveIngredent — Tương tác hoạt chất
**Giao diện**: Grid cảnh báo tương tác thuốc. Cột: hoạt chất A, hoạt chất B, cơ chế, hậu quả, hướng xử lý.
**Khi nào dùng**: Kiểm tra tương tác khi kê đơn.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(ConflictActiveIngredientInitADO) | UC | Tạo |
| Reload(uc, List\<ConflictActiveIngredientADO\>) | void | Reload |
| GetDataGridView(uc) | object | Lấy data |
| GetGridControl(uc) | GridControl | Grid trực tiếp |
| SetIsKeyChooseTrue() | void | Hiện cột chọn |
| SetIsKeyChooseFalse() | void | Ẩn cột chọn |

### HIS.UC.BidMedicineTypeGrid — Thuốc theo thầu
**Giao diện**: Grid thuốc trong gói thầu.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(BidMedicineTypeGridInitADO) | UC | Tạo |
| Reload(uc, List\<MedicineTypeADO\>) | void | Reload |
| GetDataGridView(uc) | object | Lấy data |
| FocusKeyword(uc) | void | Focus tìm kiếm |
| ResetKeyword(uc) | void | Xóa keyword tìm kiếm |

### HIS.UC.BidMaterialTypeGrid — Vật tư theo thầu
**API**: Tương tự BidMedicineTypeGrid.

---

## C. DỊCH VỤ

### HIS.UC.SereServTree — Cây dịch vụ (32 plugins)
**Giao diện**: TreeList phân cấp + checkbox. Nhóm DV > DV > chi tiết. Cột SL, giá, tiền.
**Khi nào dùng**: Thanh toán, duyệt DV, xem chi tiết.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(SereServTreeADO) | UC | Tạo |
| Reload(uc, List\<V_HIS_SERE_SERV_5\>) | void | Reload DV |
| Reload(uc, sereServs, sereServBills) | void | Reload DV + hóa đơn |
| Reload(uc, List\<V_HIS_SERE_SERV_DEPOSIT\>) | void | Reload DV + tạm ứng |
| GetListCheck(uc) | List\<SereServADO\> | Lấy DV đã check |
| CheckAllNode(uc) | void | Check tất cả |
| Search(uc) | void | Tìm kiếm |

**ADO**: SereServs, SereServBills, IsShowCheckNode, IsShowSearchPanel, IsAutoWidth, HideCheckColumn, 20+ event delegates

### HIS.UC.TreeSereServ7 — Cây dịch vụ v7 (19 plugins)
**Giao diện**: TreeList cải tiến. Nhiều overload Reload.
**Khi nào dùng**: Thay thế SereServTree trong form mới.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(TreeSereServ7ADO) | UC | Tạo |
| Reload(uc, List\<V_HIS_SERE_SERV_7\>) | void | Reload |
| Reload(uc, List\<DHisSereServ2\>) | void | Reload (format 2) |
| Reload(uc, List\<SereServADO\>) | void | Reload (ADO) |
| Reload(uc, departmentId, sereServs) | void | Reload theo khoa |
| GetListCheck(uc) | List\<SereServADO\> | Lấy đã check |
| GetValueFocus(uc) | object | Lấy dòng đang focus |
| Expand(uc, bool) | void | Mở/đóng cây |
| SetTreatment(uc, HIS_TREATMENT) | void | Đặt context điều trị |
| Search(uc) | void | Tìm kiếm |
| DisposeControl(uc) | void | Giải phóng |

### HIS.UC.Service — Chọn dịch vụ (21 plugins)
**Giao diện**: Grid danh mục DV với hoạt chất, giá.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(ServiceInitADO) | UC | Tạo |
| Reload(uc, List\<ServiceADO\>) | void | Reload |
| GetDataGridView(uc) | List\<ServiceADO\> | Lấy data |

### HIS.UC.ServiceTree — Cây dịch vụ phân cấp
| Method | Mô tả |
|--------|-------|
| Run(ServiceTreeADO) | Tạo |
| Search(uc) | Tìm kiếm |

### HIS.UC.ServiceGroup — Nhóm dịch vụ
| Method | Mô tả |
|--------|-------|
| Run, Reload, GetDataGridView, GetGridControl | Standard grid |

### HIS.UC.HisServiceType — Loại dịch vụ
Standard grid: Run, Reload, GetDataGridView.

---

## D. BỆNH NHÂN

### HIS.UC.PatientSelect — Chọn bệnh nhân (20 plugins)
**Giao diện**: Grid BN theo giường-buồng-phòng. Multi-select, keyboard, tooltip.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(PatientSelectInitADO) | UC | Tạo |
| Load(uc) | void | Load data |
| LoadWithFilter(uc, filter) | void | Load với filter |
| Reload(uc, List\<V_HIS_TREATMENT_BED_ROOM\>) | void | Reload |
| GetFocusRow(uc) | V_HIS_TREATMENT_BED_ROOM | Dòng đang chọn |
| GetSelectedRows(uc) | List\<...\> | Tất cả dòng chọn |
| FocusSearchTextbox(uc) | void | Focus tìm kiếm |
| SetOnlyOneRow(uc, bool) | void | Chỉ chọn 1 |
| ReloadStatePrescriptionPerious(uc) | void | Reload trạng thái đơn |

**ADO**: RoomId, TreatmentId, IsShowSearchPanel, IsAutoWidth, IsShowColumnBedRoomName, SelectedSingleRow delegate

### HIS.UC.UCPatientRaw — Nhập thông tin BN (23 plugins)
**Giao diện**: Form phức tạp: họ tên, ngày sinh, giới tính, CMND, BHYT, địa chỉ, nghề nghiệp. QR code.
**Khi nào dùng**: Đăng ký BN mới, cập nhật.
**Files**: 15 partial class files.
**Tính năng**: Đọc QR BN, đọc QR thẻ BHYT, tìm BN cũ (theo mã, CMND, SĐT, số hẹn), phân loại đối tượng.

### HIS.UC.AddressCombo — Tỉnh/Huyện/Xã (16 plugins)
**Giao diện**: 3 combo liên kết Tỉnh → Huyện → Xã. Tự động lọc phân cấp.
**KHÔNG tự tạo 3 ComboBox riêng**.
**Data**: Tự động load `BackendDataWorker.Get<CommuneADO>()`.
**Delegates**: DelegateFocusNextUserControl, DelegateSetAddressUCHein, DelegateSetAddressUCPlusInfo, DelegateSendCodeProvince, DelegateReloadData.

### HIS.UC.PlusInfo — Thông tin bổ sung BN
**Giao diện**: Form 40+ trường: địa chỉ, SĐT, email, liên hệ.

| Method | Return | Mô tả |
|--------|--------|-------|
| GetValue() | PatientInformationADO | Lấy data |
| SetValue(PatientInformationADO) | void | Đặt data |

### HIS.UC.UCRelativeInfo — Người nhà BN
**Giao diện**: Form: họ tên, quan hệ, SĐT, địa chỉ người nhà.

| Method | Return | Mô tả |
|--------|--------|-------|
| GetValue() | UCRelativeADO | Lấy data |
| SetValue(UCRelativeADO) | void | Đặt data |
| FocusUserControl() | void | Focus |
| FocusNextUserControl() | void | Focus tiếp |

### HIS.UC.WorkPlace — Nơi làm việc (20 plugins)
**Giao diện**: ComboBox hoặc TextBox (template-based).

| Method | Return | Mô tả |
|--------|--------|-------|
| Generate(WorkPlaceInitADO) | Task\<object\> | Tạo (async) |
| GetValue(uc, template) | object | Lấy data |
| SetValue(uc, value) | void | Đặt data |
| Reload(template, data) | void | Reload |
| FocusControl(template) | void | Focus |
| ValidationCombo(uc, template) | object | Validate |
| ResetValidation(uc) | void | Xóa lỗi |

**Templates**: Combo, Textbox, Combo1, Textbox1

### HIS.UC.PatientType — Đối tượng BN
Standard grid: Run, Reload, GetDataGridView.

### HIS.UC.National — Quốc gia
| Method | Mô tả |
|--------|-------|
| Run, GetValue, SetValue, Reload, FocusControl, ReadOnly, ValidationNational | Standard form pattern |

---

## E. PHÒNG / KHOA / GIƯỜNG

### HIS.UC.Department — Chọn khoa (14 plugins)
**Giao diện**: Grid. Search panel, radio enable.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(DepartmentInitADO) | UC | Tạo |
| Reload(uc, List\<DepartmentADO\>) | void | Reload |
| GetDataGridView(uc) | List\<DepartmentADO\> | Lấy data |

### HIS.UC.Room — Chọn phòng (16 plugins)
**Giao diện**: Grid + right-click menu.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(RoomInitADO) | UC | Tạo |
| Reload(uc, List\<RoomAccountADO\>) | void | Reload |
| ReloadColumn(uc, List\<RoomColumn\>) | void | Reload cột |
| GetDataGridView(uc) | List\<RoomAccountADO\> | Lấy data |
| GetGridControl(uc) | GridControl | Grid trực tiếp |

### HIS.UC.Bed — Giường
| Method | Return | Mô tả |
|--------|--------|-------|
| Run(BedInitADO) | UC | Tạo |
| Reload(uc, List\<BedADO\>) | void | Reload |
| GetDataGridView(uc) | List\<BedADO\> | Lấy data |
| GetGridControl(uc) | GridControl | Grid trực tiếp |

UC chính có thêm: `SetIsKeyChooseTrue()`, `SetIsKeyChooseFalse()` — toggle cột chọn.

### HIS.UC.ExecuteRoom — Phòng thực hiện
Standard grid: Run, Reload, GetDataGridView.

### HIS.UC.CashierRoom — Phòng thu ngân
Standard grid.

### HIS.UC.SampleRoom — Phòng lấy mẫu
Standard grid.

---

## F. ĐIỀU TRỊ

### HIS.UC.TreatmentFinish — Ra viện (17 plugins)
**Giao diện**: Form phức tạp: kết quả, tình trạng, hướng xử trí, ngày ra viện.
**UC phức tạp nhất** — nhiều method đặc biệt.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(TreatmentFinishInitADO, bhyt) | UC | Tạo |
| Reload(uc, DataInputADO) | void | Reload |
| GetData(uc) | HisTreatmentFinishSDO | Lấy data kết thúc |
| GetDataOutput(uc) | DataOutputADO | Lấy output |
| GetValidate(uc) | bool | Validate |
| GetValidateWithMessage(uc, errEmpty, errOther) | bool | Validate + lỗi |
| FocusControl(uc) | void | Focus |
| GetUseDay(uc) | decimal | Tính số ngày |
| CheckChangeAutoTreatmentFinish(uc, bool) | void | Tự động ra viện |
| EnableChangeAutoTreatmentFinish(uc, bool) | void | Bật/tắt auto |
| InitNeedSickLeaveCert(uc, bool) | void | Cần giấy nghỉ |
| UpdateTreatmentData(uc, treatment) | void | Cập nhật điều trị |
| UpdateStoreCode(uc, storeCode) | void | Cập nhật mã kho |
| ShowPopupAppointmentControl(uc) | void | Hiện popup hẹn khám |
| ShowPopupWhenNotFinishingIncaseOfOutPatient(uc) | void | Cảnh báo ngoại trú |
| SetDelegateCreateEMRVBA(uc, delegate) | void | Tích hợp EMR |

### HIS.UC.TreatmentInfo — Hiển thị thông tin điều trị
**Giao diện**: Display chỉ đọc: số BHYT, đối tượng.

| Method | Mô tả |
|--------|-------|
| SetValueToControl(TreatmentInfoADO) | Cập nhật hiển thị |

### HIS.UC.UCTransPati — Chuyển viện
**Giao diện**: Form: lý do, nơi chuyển, BS chỉ định. Có embedded HIS.UC.Icd + SecondaryIcd.

### HIS.UC.NextTreatmentInstruction — Hướng dẫn điều trị tiếp
| Method | Mô tả |
|--------|-------|
| Run, Reload, SetValue, GetValue, FocusControl, SetEnabled, ReadOnly, ValidationNextTreatmentInstruction | Standard form |

### HIS.UC.Death — Tử vong
| Method | Return | Mô tả |
|--------|--------|-------|
| Run(DeathInitADO) | UC | Tạo |
| GetValue(uc) | object | Lấy data |
| GetValueHisTreatment(uc) | object | Lấy data điều trị |
| SetValue(uc, data) | void | Đặt data |
| Reload(uc, DeathDataSourcesADO) | void | Reload |
| ReadOnly(uc, bool) | void | Chỉ đọc 1 phần |
| ReadOnlyAll(uc, bool) | void | Chỉ đọc toàn bộ |
| ValidControl(uc) | bool | Validate |
| FocusControl(uc) | void | Focus |

### HIS.UC.ExamFinish — Kết thúc khám
| Method | Mô tả |
|--------|-------|
| Run(ExamFinishInitADO) | Tạo |
| GetValue(uc) | Lấy data |
| ReLoad(uc, ExamFinishInitADO) | Reload |

---

## G. XÉT NGHIỆM

### HIS.UC.TestIndex — Chỉ số XN
Standard grid: Run, Reload, GetDataGridView.
**EFMODEL**: `HIS_TEST_INDEX`

### HIS.UC.TestSample — Loại mẫu XN
Standard grid: Run, Reload, GetDataGridView.
**EFMODEL**: `HIS_TEST_SAMPLE`

### HIS.UC.Machine — Máy thiết bị
Standard grid: Run, Reload, GetDataGridView.
**EFMODEL**: `HIS_MACHINE`

---

## H. NGÀY GIỜ

### HIS.UC.DateEditor — Chọn ngày/giờ (25 plugins)
**Giao diện**: DateEdit + TimePicker + checkbox multi-date.
**KHÔNG tự tạo DateEdit riêng**.

| Method | Return | Mô tả |
|--------|--------|-------|
| Run(DateInitADO) | UC | Tạo |
| GetValue(uc) | List\<long\> | Ngày dạng yyyyMMddHHmm00 |
| GetChkMultiDateState(uc) | bool | Trạng thái multi-date |
| SetValue(uc, data) | void | Đặt ngày |
| Reload(uc, DateInputADO) | void | Reload |
| FocusControl(uc) | void | Focus |
| ReadOnly(uc, bool) | void | Chỉ đọc |
| EnableCheckBoxMultiIntructionTime(uc, bool) | void | Bật/tắt multi-date |
| ValidationForm(uc) | bool | Validate |
| ValidationFormWithMessage(uc, errEmpty, errOther) | bool | Validate + lỗi |
| ResetValidation(uc) | bool | Xóa lỗi |
| NextFocus(uc, data) | void | Chuyển focus |

**InitADO**: DateInputADO, Height/Width, IsValidate, IsVisibleMultiDate, DelegateNextFocus, DelegateSelectMultiDate, DelegateChangeIntructionTime

---

## I. TÀI CHÍNH

### HIS.UC.AccountBook — Sổ kế toán
Standard grid: Run, Reload, GetDataGridView.

### HIS.UC.MenuPrint — Chọn mẫu in (22 plugins)
**Giao diện**: Menu dropdown mẫu in.

| Method | Mô tả |
|--------|-------|
| Run(MenuPrintInitADO) | Tạo (chỉ có Run) |

---

## J. EMR / PHÂN QUYỀN

### EMR.UC.EmrFlow — Luồng bệnh án
Standard grid: Run, Reload, GetDataGridView, GetGridControl.

### EMR.UC.EmrSign — Ký bệnh án
Standard grid.

### ACS.UC.User — Người dùng
Standard grid: Run, Reload, GetDataGridView.

---

## K. TIỆN ÍCH

### HIS.UC.UCImageInfo — Ảnh/đính kèm
| Method | Mô tả |
|--------|-------|
| SetValue(UCImageInfoADO) | Đặt ảnh |
| GetValue() | Lấy ảnh |
| DisablePictureboxControlHeni(bool) | Ẩn/hiện ảnh BHYT |

### Inventec.UC.Paging — Phân trang
Xem ui_rules.md.

---

## Bảng Tra Cứu Nhanh

| Nhu cầu | UC | Plugins |
|---------|-----|---------|
| Chẩn đoán chính | HIS.UC.Icd | 105 |
| Chẩn đoán phụ | HIS.UC.SecondaryIcd | 124 |
| Thuốc xuất/nhập | HIS.UC.ExpMestMedicineGrid | 60 |
| Vật tư xuất/nhập | HIS.UC.ExpMestMaterialGrid | 54 |
| Cây DV thanh toán | HIS.UC.SereServTree | 32 |
| Chọn ngày | HIS.UC.DateEditor | 25 |
| Nhập BN | HIS.UC.UCPatientRaw | 23 |
| Thuốc tồn kho | HIS.UC.MedicineTypeInStock | 23 |
| VT tồn kho | HIS.UC.MaterialTypeInStock | 23 |
| Mẫu in | HIS.UC.MenuPrint | 22 |
| Chọn DV | HIS.UC.Service | 21 |
| Nơi làm việc | HIS.UC.WorkPlace | 20 |
| Chọn BN | HIS.UC.PatientSelect | 20 |
| Cây DV v7 | HIS.UC.TreeSereServ7 | 19 |
| Ra viện | HIS.UC.TreatmentFinish | 17 |
| Chọn phòng | HIS.UC.Room | 16 |
| Địa chỉ | HIS.UC.AddressCombo | 16 |
| Đơn trước | HIS.UC.PeriousExpMestList | 16 |
| Chọn khoa | HIS.UC.Department | 14 |
| Chọn kho | HIS.UC.MediStock | 12 |

## Quy Tắc

1. **LUÔN kiểm tra UC/ trước khi tự tạo control**
2. **Dùng Processor.Run(InitADO)** — KHÔNG new UC trực tiếp
3. **Dock = Fill** vào PanelControl
4. **KHÔNG sửa source UC** — tạo wrapper hoặc báo team
5. **Icd + SecondaryIcd** luôn đi cặp
6. **Processor.GetValue()** lấy data, **Processor.Reload()** cập nhật
7. **Callbacks qua InitADO** — KHÔNG subscribe event trực tiếp
