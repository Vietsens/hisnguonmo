# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.MedicineTypeCreate - Tạo, sửa loại thuốc

---

## 1. Mục đích

Cung cấp form để tạo mới và chỉnh sửa thông tin loại thuốc trong hệ thống HIS. Plugin cho phép quản lý toàn diện thông tin thuốc bao gồm: thông tin cơ bản, đơn vị tính, dạng bào chế, nhà cung cấp, giá BHYT/dịch vụ, chống chỉ định, phương pháp sơ chế/phức chế, chặn xuất theo khoa/phòng, và liên kết chính sách giá đối tượng bệnh nhân.

Plugin chỉ hoạt động (IsEnable = true) khi người dùng đang ở phòng kho (room type = STOCK).

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.MedicineTypeCreate/
├── ADO/
│   ├── AMedicineTypeADO.cs
│   ├── AConfigADO.cs
│   ├── DepartmentADO.cs
│   ├── PatientTypeADO.cs
│   ├── RankADO.cs
│   ├── SourceMedicineADO.cs
│   ├── SupplierADO.cs
│   └── VHisServicePatyADO.cs
├── Config/
│   └── HisConfigCFG.cs
├── MedicineTypeCreate/
│   ├── IMedicineTypeCreate.cs
│   ├── MedicineTypeCreateFactory.cs
│   ├── MedicineTypeCreateBehavior.cs
│   ├── frmMedicineTypeCreate.cs
│   ├── frmMedicineTypeCreate.Designer.cs
│   ├── frmMedicineTypeCreate.resx
│   ├── frmMedicineTypeCreate__InitResource.cs
│   ├── frmMedicineTypeUpdate_LoadComboControl.cs
│   ├── frmMedicineTypeUpdate_Validate.cs
│   ├── frmDepartmentPatientType.cs
│   └── frmDepartmentPatientType.resx
├── Popup/
│   ├── frmDieuChinhLieu.cs
│   ├── frmDieuChinhLieu.Designer.cs
│   ├── frmDieuChinhLieu.resx
│   ├── frmProcessingMethod.cs
│   ├── frmProcessingMethod.Designer.cs
│   ├── frmProcessingMethod.resx
│   └── IcdUtil.cs
├── Resources/
│   ├── ResourceLanguageManager.cs
│   ├── Lang.vi.resx
│   ├── Lang.en.resx
│   └── Lang.my.resx
├── Validtion/
│   ├── HeinLimitValidationRule.cs
│   ├── HeinLimitPriceDateTimeValidationRule.cs
│   ├── HeinLimitRatioValidationRule.cs
│   ├── HeinServiceTypeBhytValidationRule.cs
│   ├── MedicineNationalCodeValidationRule.cs
│   ├── BenhPhuValidationRule.cs
│   ├── SpinVatBlack.cs
│   ├── SpinVatRed.cs
│   ├── SpinNotVatAndBlack.cs
│   ├── SpinNotVatAndRed.cs
│   ├── ValidateCombox.cs
│   ├── ValidateMaxlength.cs
│   ├── ValidateMaxlengthBaseEdit.cs
│   ├── ValidMaxlengthTxtMedicineTypeCodeName.cs
│   ├── ValidComboUseMedicine.cs
│   ├── ValidationAge.cs
│   └── ValidationImpUnitConverRatio.cs
├── Properties/
│   └── AssemblyInfo.cs
├── MedicineTypeCreateProcessor.cs
├── CallModule.cs (ModuleLinkString.cs)
├── CustomParse.cs
├── Delegate.cs
├── HisRequestUri.cs
├── KeyboardWorker.cs
└── HIS.Desktop.Plugins.MedicineTypeCreate.csproj
```

---

## 3. Đăng ký Module

**File:** `MedicineTypeCreateProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.MedicineTypeCreate
Tên hiển thị : Tạo, sửa loại thuốc
Icon         : thuoc.png
Nhóm         : Bussiness
Priority     : 2680
Loại         : MODULE_TYPE_ID__FORM
IsEnable     : Chỉ true khi CurrentRoomTypeCode chứa ROOM_TYPE_CODE__STOCK
```

**Luồng khởi tạo:**
```
MedicineTypeCreateProcessor.Run(args)
  → MedicineTypeCreateFactory.MakeIMaterialType(param, args)
    → new MedicineTypeCreateBehavior(param, args)
      → IMedicineTypeCreate.Run()
        → Phân tích args: Module, long? medicineTypeId, int actionType, DelegateSelectData
        → new frmMedicineTypeCreate(moduleData, medicineTypeId, actionType, delegateSelect)
```

**Tham số đầu vào (args[]):**

| Kiểu | Mô tả |
|------|-------|
| `Module` | Thông tin module đang gọi |
| `long?` | ID loại thuốc (null = tạo mới) |
| `int` | ActionType: ActionAdd / ActionEdit |
| `DelegateSelectData` | Callback trả dữ liệu về form gọi |

---

## 4. Thiết kế chi tiết

### 4.1. ADO Classes (ADO/)

#### AMedicineTypeADO
Danh sách loại thuốc tĩnh (hardcode).

| Property | Type | Mô tả |
|----------|------|-------|
| ID | long | Mã loại (1–13) |
| NAME | string | Tên loại thuốc |

Các giá trị: Hóa chất, Sản phẩm không phải là thuốc, Thuốc dấu sao *, Generic, Vaccine, Vitamin A, Tiêm chủng mở rộng, Sinh phẩm, Ô xy, Gây tê, Biệt dược gốc, Thuốc chạy thận, Nguyên liệu điều chế, **Thực phẩm dinh dưỡng** _(ID=14, thêm mới)_.

> **Hành vi đặc biệt — Thực phẩm dinh dưỡng (ID=14):** Khi được chọn, trường "Dòng thuốc" (cboMedicineLine) tự động chuyển sang **không bắt buộc** (label đổi sang màu đen, clear validation rule). Khi bỏ chọn, "Dòng thuốc" trở lại bắt buộc.

#### AConfigADO
Cấu hình thuốc (đa chọn).

| Property | Type | Mô tả |
|----------|------|-------|
| ID | long | Mã cấu hình (1–16) |
| NAME | string | Tên cấu hình |

Các giá trị: Dừng nhập, Chi phí ngoài gói, Phải nhập hạn sử dụng, Cho kê lẻ, Cho xuất lẻ, Tách phần bù, Không bắt buộc số lô/hạn, Phải dự trù, Tự động hao phí, Có nguồn chi trả khác, Đếm số ngày dùng, Không đếm số ngày dùng, Ngoài DRG, Thuốc ngoại viện, Thuốc kinh doanh, Thuốc quầy thuốc.

> **Lưu ý ràng buộc:** "Thuốc quầy thuốc" (ID=16) chỉ có thể chọn khi đã chọn "Thuốc kinh doanh" (ID=15). Row "Thuốc quầy thuốc" bị disable (xám) nếu chưa chọn "Thuốc kinh doanh".

#### SupplierADO
Kế thừa `HIS_SUPPLIER`, bổ sung:

| Property | Type | Mô tả |
|----------|------|-------|
| SUPPLIER_NAME_UNSIGN | string | Tên nhà cung cấp không dấu (dùng tìm kiếm) |
| isChecked | bool | Trạng thái đã chọn trong multi-select combo |

#### VHisServicePatyADO
Data transfer object cho chính sách giá theo đối tượng bệnh nhân.

#### DepartmentADO / PatientTypeADO / RankADO / SourceMedicineADO
Các DTO đơn giản (ID + tên/mã) tương ứng với entity trong MOS.EFMODEL.

---

### 4.2. frmMedicineTypeCreate - Form chính

**File:** `frmMedicineTypeCreate.cs`, `frmMedicineTypeCreate.Designer.cs` (partial class gồm 4 file)

#### 4.2.1. Giao diện tổng quan (DevExpress)

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│ [Toolbar] [bbtnSave: Lưu] [btnRefresh: Làm mới] [btnEditInfo] [btnDieuChinhLieu]│
│           [btnDepartmentPatientType] [btnGiaTran]                                │
├──────────────────────────────────────────────────────────────────────────────────┤
│ THÔNG TIN LOẠI THUỐC                                                             │
│ ┌──────────────────────────────────────────────────────────────────────────────┐ │
│ │ Mã loại thuốc: [txtMedicineTypeCode]  Tên: [txtMedicineTypeName]            │ │
│ │ Đơn vị tính (nhập): [cboImpUnit] [txtImpUnitCode] Tỷ lệ: [spUnitConvertRatio]│
│ │ Đơn vị tính (y lệnh): [cboServiceUnit] [txtServiceUnitCode]                 │ │
│ │ Dạng bào chế: [cboDosageForm]  Cách dùng: [cboMedicineUseForm] [txtMedicineUseFormCode]│
│ │ Đường dùng: [cboHtu]  Nồng độ: [txtConcentra]  Quy cách đóng gói: [txtPackingTypeCode]│
│ │ Nhóm DT: [cboMedicineLine]  Loại thuốc: [cboLoaiThuoc] (đa chọn)           │ │
│ │ NCC: [cboNCC] (đa chọn)  Hãng SX: [cboManufacturer]  Nguồn gốc: [cboNguonGoc]│
│ │ Mã quốc gia: [txtMedicineNationalCode]  Mã hoạt chất: [txtActiveIngrBhytCode]│
│ │ Tên hoạt chất: [txtActiveIngrBhytName]  Số ĐK: [txtRegisterNumber]         │ │
│ │ Tuổi từ (tháng): [spinAgeFrom]  đến: [spinAgeTo]                            │ │
│ │ Số thứ tự: [spinNumOrder]  Cấu hình: [cboConfig] (đa chọn)                  │ │
│ ├──────────────────────────────────────────────────────────────────────────────┤ │
│ │ GIÁ NHẬP / GIÁ BÁN / BHYT                                                   │ │
│ │ Giá nhập (chưa VAT): [spinImpPrice]  VAT%: [spinImpVatRatio]                │ │
│ │ Giá bán nội bộ: [spinInternalPrice]  Giá xuất cuối: [spinLastExpPrice]       │ │
│ │ VAT xuất%: [spinLastExpVatRatio]  Số ngày dùng/lần: [spinUseOnDay]          │ │
│ │ [ChkIsSpecificHeinPrice] Giá BHYT riêng  [rdoWarning/rdoWarning1]           │ │
│ │ Giá BHYT: [txtHeinPrice]  Từ ngày: [dtHeinLimitDate]                        │ │
│ │ Tỷ lệ giới hạn BHYT cũ: [txtHeinLimitRatioOld]  mới: [txtHeinLimitRatio]   │ │
│ ├──────────────────────────────────────────────────────────────────────────────┤ │
│ │ THÔNG TIN THÊM                                                               │ │
│ │ Tên KH: [txtScientificName]  Mô tả: [txtDescription]  Cảnh báo: [txtContentWarning]│
│ │ Chống chỉ định: [txtContraindication]  Ghi chú giao dịch: [txtRecordingTransaction]│
│ │ Sơ chế: [txtPreprocessing]  Phức chế: [txtProcessing]  Bộ phận dùng: [txtUsedPart]│
│ │ Lượng phân phối: [txtDistributedAmount]  Nguồn chi trả khác: [txtOTHER_PAY_SOURCE]│
│ │ Số TCY: [txtTcyNumOrder]  Số BYT: [txtBytNumOrder]  TCCL: [txtTCCL]         │ │
│ │ Hướng dẫn: [txtTutorial]  Chú ý: [memoContainer]                            │ │
│ │ [rdoUpdateAll / rdoUpdateNotFee] - Chế độ cập nhật khi sửa                  │ │
│ ├──────────────────────────────────────────────────────────────────────────────┤ │
│ │ CHẶN XUẤT THEO KHOA/PHÒNG                                                    │ │
│ │ Khoa: [cboBlockDepartment]  Phòng: [cboBlockRoom]  Loại xuất: [cboNoExpMestTypeIds]│
│ │ [SelectionGrid__BlockDepartment] [SelectionGrid__BlockRoom] [SelectionGrid__BlockExpMest]│
│ ├──────────────────────────────────────────────────────────────────────────────┤ │
│ │ CHỐNG CHỈ ĐỊNH / CHÍNH SÁCH GIÁ / ICD / QUỐC GIA                            │ │
│ │ Chống chỉ định: [cboContraindication] [SelectionGrid__Contraindication]      │ │
│ │ Dịch vụ - Loại đối tượng: [gridServicePaty]                                  │ │
│ │ Quốc gia: [ucNational] (UC tích hợp)                                         │ │
│ └──────────────────────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────────────────────┘
```

#### 4.2.2. Danh sách control chính

| Control | Loại DevExpress | Mô tả |
|---------|----------------|-------|
| txtMedicineTypeCode | TextEdit | Mã loại thuốc (bắt buộc hoặc tùy cấu hình) |
| txtMedicineTypeName | TextEdit | Tên loại thuốc (bắt buộc, max 500) |
| cboImpUnit | GridLookUpEdit | Đơn vị nhập kho |
| cboServiceUnit | GridLookUpEdit | Đơn vị y lệnh |
| txtServiceUnitCode | TextEdit | Mã đơn vị y lệnh (readonly, sync từ cbo) |
| spUnitConvertRatio | SpinEdit | Tỷ lệ quy đổi đơn vị nhập → y lệnh |
| cboDosageForm | GridLookUpEdit | Dạng bào chế |
| cboMedicineUseForm | GridLookUpEdit | Cách dùng |
| txtMedicineUseFormCode | TextEdit | Mã cách dùng (readonly) |
| cboHtu | GridLookUpEdit | Đường dùng (How To Use) |
| cboMedicineLine | GridLookUpEdit | Nhóm dược (bắt buộc - Warning) |
| cboLoaiThuoc | GridLookUpEdit | Loại thuốc (đa chọn, multi-select) |
| cboNCC | GridLookUpEdit | Nhà cung cấp (đa chọn, tìm kiếm không dấu) |
| cboManufacturer | GridLookUpEdit | Hãng sản xuất |
| cboNguonGoc | GridLookUpEdit | Nguồn gốc (từ HIS_SOURCE_MEDICINE) |
| txtMedicineNationalCode | TextEdit | Mã thuốc quốc gia (max 30) |
| txtActiveIngrBhytCode | TextEdit | Mã hoạt chất BHYT (max 1000) |
| txtActiveIngrBhytName | TextEdit | Tên hoạt chất BHYT (max 1000) |
| txtRegisterNumber | TextEdit | Số đăng ký (max 500) |
| spinAgeFrom / spinAgeTo | SpinEdit | Tuổi từ / đến (tháng) |
| spinNumOrder | SpinEdit | Số thứ tự |
| cboConfig | GridLookUpEdit | Cấu hình thuốc (đa chọn, có ràng buộc) |
| spinImpPrice | SpinEdit | Giá nhập chưa VAT (>= 0) |
| spinImpVatRatio | SpinEdit | VAT nhập % (>= 0) |
| spinInternalPrice | SpinEdit | Giá bán nội bộ |
| spinLastExpPrice | SpinEdit | Giá xuất cuối |
| spinLastExpVatRatio | SpinEdit | VAT xuất % |
| spinUseOnDay | SpinEdit | Số ngày dùng mỗi lần |
| ChkIsSpecificHeinPrice | CheckEdit | Có giá BHYT riêng |
| txtHeinPrice | TextEdit | Giá BHYT |
| dtHeinLimitDate | DateEdit | Ngày áp dụng giá BHYT |
| txtHeinLimitRatioOld / txtHeinLimitRatio | TextEdit | Tỷ lệ giới hạn BHYT cũ/mới (0-100) |
| chkIsNoHeinLimitForSpecial | CheckEdit | Không giới hạn BHYT cho đối tượng đặc biệt |
| txtScientificName | TextEdit | Tên khoa học (max 500) |
| txtDescription | TextEdit | Mô tả (max 500) |
| txtContentWarning | TextEdit | Cảnh báo nội dung (max 2000) |
| txtContraindication | TextEdit | Chống chỉ định (max 4000) |
| txtRecordingTransaction | TextEdit | Ghi chú giao dịch (max 20) |
| txtPreprocessing | TextEdit | Mã sơ chế (max 1000) |
| txtProcessing | TextEdit | Mã phức chế (max 1000) |
| txtUsedPart | TextEdit | Bộ phận dùng (max 500) |
| txtDistributedAmount | TextEdit | Lượng phân phối (max 500) |
| txtOTHER_PAY_SOURCE | TextEdit | Nguồn chi trả khác (max 200) |
| txtTcyNumOrder | TextEdit | Số thứ tự TCY (max 20) |
| txtBytNumOrder | TextEdit | Số thứ tự BYT (max 50) |
| txtTCCL | TextEdit | TCCL (max 1000) |
| txtTutorial | TextEdit | Hướng dẫn sử dụng (max 2000) |
| memoContainer | MemoEdit | Chú ý / ghi chú dài (max 2000) |
| rdoUpdateAll / rdoUpdateNotFee | RadioButton | Chế độ cập nhật khi sửa |
| cboBlockDepartment | GridLookUpEdit | Chọn khoa chặn xuất (multi-select) |
| cboBlockRoom | GridLookUpEdit | Chọn phòng chặn xuất (multi-select, room type 1 hoặc 4) |
| cboNoExpMestTypeIds | GridLookUpEdit | Loại xuất bị chặn (chỉ ID=2) |
| SelectionGrid__BlockDepartment | GridControl | Hiển thị khoa đã chặn |
| SelectionGrid__BlockRoom | GridControl | Hiển thị phòng đã chặn |
| SelectionGrid__BlockExpMest | GridControl | Hiển thị loại xuất đã chặn |
| cboContraindication | GridLookUpEdit | Chọn chống chỉ định |
| SelectionGrid__Contraindication | GridControl | Danh sách chống chỉ định đã chọn |
| gridServicePaty | GridControl | Dịch vụ - chính sách giá theo đối tượng |
| ucNational | UserControl | UC tìm kiếm ICD quốc gia |
| btnSave / bbtnSave | SimpleButton / BarButtonItem | Nút lưu |
| btnRefresh | SimpleButton | Làm mới form |
| btnEditInfo | SimpleButton | Chỉnh sửa thông tin (chế độ sửa) |
| btnDieuChinhLieu | SimpleButton | Mở popup điều chỉnh liều (chỉ enable khi đã lưu) |
| btnDepartmentPatientType | SimpleButton | Thiết lập đối tượng bệnh nhân theo khoa |
| btnGiaTran | SimpleButton | Thiết lập giá trần (enable khi ChkIsSpecificHeinPrice checked) |

#### 4.2.3. Luồng Load Form

**frmEditInfoPatient_Load:**
```
frmEditInfoPatient_Load
  ├── HisConfigCFG.LoadConfig()
  │     → Đọc AtcCodeOverlarWarningOption, ServiceCodeOption từ HisConfig
  ├── Load dataDosageForm từ BackendDataWorker<HIS_DOSAGE_FORM> (IS_ACTIVE = 1)
  ├── InitUcNational() - Khởi tạo UserControl ICD quốc gia
  ├── InitComboServiceUnit() - Gọi api/HisServiceUnit/Get
  ├── InitComboOtherPay()
  ├── SetDefaultData()
  ├── SetCaptionByLanguageKey() - Áp dụng ngôn ngữ
  ├── ValidataForm() - Đăng ký tất cả validation rules
  ├── InitCheck + InitCombo cho cboBlockDepartment (HIS_DEPARTMENT)
  ├── InitCheck + InitCombo cho cboBlockRoom (V_HIS_ROOM, room_type 1 hoặc 4)
  ├── InitCheck + InitCombo cho cboNoExpMestTypeIds (HIS_EXP_MEST_TYPE, ID=2)
  ├── InitContraindicationCheck() + InitComboContraindication()
  ├── InitComboPreserveCodition()
  ├── InitComboNguonGoc()
  ├── LoadCboLoaiThuoc() - Danh sách loại thuốc tĩnh 14 giá trị (bao gồm "Thực phẩm dinh dưỡng")
  ├── LoadCboConfig() - Danh sách cấu hình tĩnh 16 giá trị
  ├── LoadCboNCC() - BackendDataWorker<HIS_SUPPLIER> (IS_ACTIVE=1), build SupplierADO
  │
  ├── [Nếu ActionType = ActionEdit và currentMedicineTypeId != null]
  │     ├── Gọi api/HisMedicineType/GetView (filter: ID = currentMedicineTypeId)
  │     ├── Gọi api/HisService/GetView (filter: ID = service_id)
  │     ├── FillDataMedicineTypeToControl(V_HIS_MEDICINE_TYPE, V_HIS_SERVICE)
  │     └── btnSave.Enabled = (IS_ACTIVE == 1)
  │
  ├── [Nếu ActionType = ActionAdd]
  │     ├── rdoWarning.Checked = true, rdoWarning1.Checked = true
  │     └── btnRefresh_Click() để set cấu hình mặc định
  │
  ├── FillBlockRoom() - Điền grid chặn phòng đã lưu
  ├── FillBlockDepartment() - Điền grid chặn khoa đã lưu
  ├── FillContraindication() - Điền grid chống chỉ định đã lưu
  ├── FillDataToGridConrolServicePaty() - Load chính sách giá đối tượng
  ├── RegisterTimer + StartTimer(timerInitForm, 1000ms) - Load combo sau khi form render
  └── WaitingManager.Hide()
```

#### 4.2.4. Luồng Lưu (btnSave_Click)

```
btnSave_Click
  ├── dxValidationMedicineType.Validate() → nếu lỗi: return
  ├── [Nếu ServiceCodeOption = "1" và có nhập mã]
  │     → Kiểm tra trùng mã trong BackendDataWorker<HIS_MEDICINE_TYPE>
  │     → Nếu trùng: hiện XtraMessageBox, focus txtMedicineTypeCode, return
  ├── nationalProcessor.ValidationNational(ucNational) → nếu lỗi: return
  ├── Kiểm tra ActionType != ActionView
  ├── Kiểm tra tổng độ dài (sơ chế + phức chế) <= 255 bytes UTF8
  ├── Kiểm tra dxErrorProvider1.HasErrors
  ├── WaitingManager.Show()
  ├── UpdatePatientDTOFromDataForm(ref currentMedicineTypeDTO) - Map control → DTO
  │
  ├── [ActionAdd]
  │     → POST api/HisMedicineType/Create → resultData: HIS_MEDICINE_TYPE
  │     → Nếu có depaPatientTypes: POST api/HisDepaPatientType/CreateList
  │     → BackendDataWorker.Reset<HIS_MATERIAL_TYPE, HIS_DEPA_PATIENT_TYPE>
  │
  ├── [ActionEdit]
  │     → Tạo HisMedicineTypeSDO, gọi UpdateData(sdo)
  │     → POST api/HisMedicineType/UpdateSdo → resultData
  │     → Xử lý depaPatientTypes (Delete cũ nếu isClickPick, Create mới)
  │     → BackendDataWorker.Reset<HIS_MATERIAL_TYPE, HIS_DEPA_PATIENT_TYPE>
  │
  ├── [Nếu resultData != null]
  │     ├── Cập nhật UI (disable btnSave, enable btnRefresh, btnEditInfo, btnDieuChinhLieu)
  │     ├── BackendDataWorker.Reset<HIS_MEDICINE_TYPE>()
  │     ├── InitMedicineTypeParent() - Cập nhật thuốc cha/con
  │     ├── SendDataAfterSave() - Gửi kết quả qua delegateSelect
  │     ├── [Nếu chkIsNoHeinLimitForSpecial.Checked]
  │     │     → GET api/HisService/GetView
  │     │     → POST api/HisService/UpdateSdo (cập nhật IS_NO_HEIN_LIMIT_FOR_SPECIAL)
  │     ├── Check() → SaveProcessorsHisServicePaty() - Lưu chính sách giá đối tượng
  │     ├── SaveBlockDepartment() - Lưu danh sách khoa chặn xuất
  │     ├── SaveBlockRoom() - Lưu danh sách phòng chặn xuất
  │     └── SaveContraindication() - Lưu chống chỉ định
  │
  └── MessageManager.Show() / SessionManager.ProcessTokenLost()
```

#### 4.2.5. Luồng cboNCC (Nhà cung cấp - Multi-select đặc biệt)

```
LoadCboNCC()
  ├── Build List<SupplierADO> (ID, SUPPLIER_CODE, SUPPLIER_NAME, SUPPLIER_NAME_UNSIGN, isChecked=false)
  ├── Khởi tạo lstSupplierChecked (bản sao để track trạng thái)
  └── InitComboNCC() + InitComboNCCCheck()

CboNCC_RowCellClick
  ├── Toggle clickedItem.isChecked
  └── Cập nhật lstSupplierChecked (thêm/cập nhật trạng thái)

CboNCC_Closed
  ├── Sync isChecked từ lstSupplierChecked vào datasource
  ├── Sắp xếp: items đã check lên đầu (OrderByDescending isChecked)
  └── Refresh cboNCC.Properties.DataSource
```

#### 4.2.6. Luồng cboLoaiThuoc (Loại thuốc - Multi-select + logic nghiệp vụ)

```
Event_Check (SelectionChanged của cboLoaiThuoc)
  ├── Cập nhật lstMedicine từ selection hiện tại
  ├── Build display text (tên các loại được chọn, phân cách ", ")
  ├── cboLoaiThuoc.Text = display text
  ├── [Kiểm tra IS_NUTRITION_FOOD]
  │     isNutritionFood = lstMedicine.Any(ID == 14)
  │     → ValidatecboMedicineLine(!isNutritionFood)
  │           true  → lciMedicineLine: màu Maroon, đặt rule bắt buộc
  │           false → lciMedicineLine: màu Black, clear rule
  └── cboLoaiThuoc.Properties.View.RefreshData()

FillDataMedicineTypeToControl (khi ActionEdit)
  ├── Đọc các IS_* từ V_HIS_MEDICINE_TYPE → build arr[] tên
  ├── IS_NUTRITION_FOOD == 1 → arr.Add("Thực phẩm dinh dưỡng")
  └── ProcessSelectBS(arr, gridCheck) → restore checkbox + trigger Event_Check
        → ValidatecboMedicineLine(false) tự động khi ID=14 được restore
```

#### 4.2.7. Validation Rules

| Control | Rule Class | Điều kiện |
|---------|-----------|-----------|
| txtMedicineTypeCode | ValidMaxlengthTxtMedicineTypeCodeName / ControlMaxLengthValidationRule | Phụ thuộc ServiceCodeOption; max 100 (option=1) hoặc bắt buộc khi sửa |
| txtMedicineTypeName | ControlMaxLengthValidationRule | Bắt buộc, max 500 |
| cboServiceUnit + txtServiceUnitCode | GridLookupEditWithTextEditValidationRule | Bắt buộc |
| cboMedicineUseForm + txtMedicineUseFormCode | GridLookupEditWithTextEditValidationRule | Bắt buộc khi cấu hình yêu cầu |
| spinImpPrice | SpinNotVatAndBlack | >= 0 |
| spinImpVatRatio | SpinVatBlack | >= 0 |
| spinInternalPrice | SpinNotVatAndBlack | >= 0 |
| spinLastExpPrice | SpinNotVatAndBlack | >= 0 |
| spinLastExpVatRatio | SpinNotVatAndBlack | >= 0 |
| spinUseOnDay | SpinNotVatAndBlack | >= 0 |
| spinAgeFrom / spinAgeTo | ValidationAgeMonth | spinAgeFrom <= spinAgeTo |
| spUnitConvertRatio | ValidationImpUnitConverRatio | Bắt buộc khi cboImpUnit có giá trị |
| txtHeinLimitRatio / txtHeinLimitRatioOld | HeinLimitRatioValidationRule | Trong khoảng 0–100 |
| txtMedicineNationalCode | MedicineNationalCodeValidationRule | max 30 ký tự |
| txtRegisterNumber | ValidateMaxLength | max 500 (Warning) |
| txtTcyNumOrder | ValidateMaxLength | max 20 (Warning) |
| txtBytNumOrder | ValidateMaxLength | max 50 (Warning) |
| txtPackingTypeCode | ValidateMaxLength | max 300 (Warning) |
| txtConcentra | ValidateMaxLength | max 1000 (Warning) |
| txtTCCL | ValidateMaxLength | max 1000 (Warning) |
| txtTutorial | ValidateMaxLength | max 2000 (Warning) |
| txtActiveIngrBhytCode | ValidateMaxLength | max 1000 (Warning) |
| txtActiveIngrBhytName | ValidateMaxLength | max 1000 (Warning) |
| txtDescription | ValidateMaxLength | max 500 |
| txtContentWarning | ValidateMaxLength | max 2000 |
| txtContraindication | ValidateMaxLength | max 4000 |
| txtRecordingTransaction | ValidateMaxLength | max 20 |
| txtScientificName | ControlMaxLengthValidationRule | max 500 |
| txtPreprocessing | ControlMaxLengthValidationRule | max 1000 |
| txtProcessing | ControlMaxLengthValidationRule | max 1000 |
| txtUsedPart | ControlMaxLengthValidationRule | max 500 |
| txtDistributedAmount | ControlMaxLengthValidationRule | max 500 |
| txtOTHER_PAY_SOURCE | ControlMaxLengthValidationRule | max 200 |
| memoContainer | ValidateMaxLength | max 2000 |
| cboMedicineLine | ValidateCombox | Bắt buộc (Warning) — **tự động tắt** khi chọn "Thực phẩm dinh dưỡng" |

**Kiểm tra bổ sung (ngoài ValidationProvider):**
- Tổng độ dài txtProcessing + txtPreprocessing (UTF8) <= 255 bytes
- Trùng mã MEDICINE_TYPE_CODE khi ServiceCodeOption = "1"
- Không cho lưu khi dxErrorProvider1.HasErrors

#### 4.2.8. Cấu hình (HisConfigCFG)

| Config Key | Property | Ảnh hưởng |
|-----------|----------|-----------|
| `HIS.DESKTOP.PRESCRIPTION.ATC_CODE_OVERLAP.WARNING_OPTION` | AtcCodeOverlarWarningOption | Cảnh báo trùng mã ATC khi kê đơn |
| `MOS.HIS_SERVICE.SERVICE_CODE_OPTION` | ServiceCodeOption | "1" = mã tự nhập + kiểm tra trùng; khác = mã tự sinh |

---

### 4.3. frmDieuChinhLieu - Popup điều chỉnh liều

**File:** `Popup/frmDieuChinhLieu.cs`

Form popup hiển thị và chỉnh sửa thông tin liều dùng, bao gồm:
- Danh sách ICD (chẩn đoán) liên quan: `List<HIS_ICD>` với separator ";"
- Danh sách dịch vụ chạy thận: `List<HIS_MEDICINE_SERVICE>` (listMedinceServiceCnThan)
- Danh sách dịch vụ xét nghiệm: `List<HIS_MEDICINE_SERVICE>` (listMedinceServiceDVXN)
- Tab control chia theo nhóm liều
- Lưu qua `bbtnItemSave_ItemClick` → `btnSave_Click`

**Đầu vào:** `Module _Module, long? medicineTypeId`

---

### 4.4. frmProcessingMethod - Popup chọn phương pháp xử lý

**File:** `Popup/frmProcessingMethod.cs`

Popup cho phép chọn phương pháp sơ chế/phức chế (`HIS_PROCESSING_METHOD`) từ danh sách để đưa vào trường txtPreprocessing/txtProcessing của form chính.

---

### 4.5. frmDepartmentPatientType - Thiết lập đối tượng BN theo khoa

**File:** `MedicineTypeCreate/frmDepartmentPatientType.cs`

Form con quản lý `HIS_DEPA_PATIENT_TYPE` - thiết lập đối tượng bệnh nhân được sử dụng thuốc theo từng khoa. Kết quả callback về form chính qua sự kiện `Frm_OnDepaPatientTypeSaved`.

---

### 4.6. Keyboard Shortcuts

**File:** `KeyboardWorker.cs`

| Phím tắt | Hành động |
|----------|-----------|
| Ctrl+N | Tạo mới (btnRefresh_Click) |
| Ctrl+S | Lưu (btnSave_Click) |

---

## 5. Design Pattern

### Factory Pattern

```
IMedicineTypeCreate            ← Interface (Run())
MedicineTypeCreateFactory      ← Factory (MakeIMaterialType())
MedicineTypeCreateBehavior     ← Behavior (kế thừa BusinessBase, implement IMedicineTypeCreate)
  └── Run() → new frmMedicineTypeCreate(moduleData, medicineTypeId, actionType, delegateSelect)
```

### Module Registration Pattern

```
[ExtensionOf(typeof(DesktopRootExtensionPoint), ...)]
MedicineTypeCreateProcessor : ModuleBase, IDesktopRoot
  └── Run(args) → Factory.MakeIMaterialType() → behavior.Run()
  └── IsEnable() → GlobalVariables.CurrentRoomTypeCode.Contains(STOCK)
```

### Multi-select Combo Pattern

```
cboNCC, cboLoaiThuoc, cboConfig, cboBlockDepartment, cboBlockRoom
  └── GridCheckMarksSelection → SelectionChanged event
  └── RowCellClick để toggle isChecked
  └── Closed event → re-sort datasource (checked items first)
```

### Delegate Callback Pattern

```
DelegateSelectData   ← Trả về HIS_MEDICINE_TYPE đã lưu về form cha
DelegateRefreshData  ← Yêu cầu form cha reload dữ liệu
```

---

## 6. Dependency

### Project References

| Project | Mục đích |
|---------|---------|
| HIS.Desktop.ApiConsumer | ApiConsumers.MosConsumer |
| HIS.Desktop.Common | BusinessBase, WaitingManager |
| HIS.Desktop.Controls.Session | SessionManager.ProcessTokenLost |
| HIS.Desktop.LocalStorage.BackendData | BackendDataWorker (cache local) |
| HIS.Desktop.LocalStorage.HisConfig | HisConfigs.Get (cấu hình Oracle) |
| HIS.Desktop.LocalStorage.LocalData | GlobalVariables (ActionAdd/Edit/View, RoomTypeCode) |
| HIS.Desktop.LocalStorage.Location | ApplicationStoreLocation |
| HIS.Desktop.LibraryMessage | MessageManager, MessageUtil |
| HIS.Desktop.Utility | FormBase |
| HIS.UC.National | NationalProcessor (ICD quốc gia) |
| HIS.UC.SecondaryIcd | ICD phụ |

### DLL References

| DLL | Mục đích |
|-----|---------|
| DevExpress.XtraEditors.v15.2 | TextEdit, SpinEdit, CheckEdit, GridLookUpEdit, DXErrorProvider |
| DevExpress.XtraGrid.v15.2 | GridControl, GridView, GridColumn, GridCheckMarksSelection |
| DevExpress.XtraLayout.v15.2 | LayoutControl, LayoutControlItem |
| DevExpress.XtraBars.v15.2 | BarManager, BarButtonItem |
| DevExpress.XtraCharts.v15.2 | (dependency) |
| MOS.EFMODEL | V_HIS_MEDICINE_TYPE, HIS_MEDICINE_TYPE, V_HIS_SERVICE, HIS_SERVICE, HIS_SUPPLIER, HIS_DEPARTMENT, V_HIS_ROOM, HIS_CONTRAINDICATION, HIS_PROCESSING_METHOD, HIS_DOSAGE_FORM, HIS_DEPA_PATIENT_TYPE, HIS_EXP_MEST_TYPE, HIS_MEDICINE_SERVICE, HIS_ICD, HIS_ATC, HIS_ATC_GROUP |
| MOS.Filter | HisMedicineTypeViewFilter, HisServiceViewFilter, HisServiceUnitFilter, HisDepaPatientTypeFilter |
| MOS.SDO | HisMedicineTypeSDO, HisServiceSDO |
| SDA.EFMODEL | (SDA model entities) |
| Inventec.Common.Adapter | BackendAdapter |
| Inventec.Common.Controls.EditorLoader | ControlEditorLoader, ControlEditorADO |
| Inventec.Core | CommonParam |
| Inventec.Desktop.Common.Message | MessageUtil |
| AutoMapper 4.0.4 | DataObjectMapper.Map |
| log4net 1.2.10 | LogSystem |

### Modules được gọi (CallModule.cs)

| Constant | Module Link | Mục đích |
|----------|------------|---------|
| HisMedicineLine | HIS.Desktop.Plugins.HisMedicineLine | Quản lý nhóm dược |
| HisManufacturer | HIS.Desktop.Plugins.HisManufacturer | Quản lý hãng sản xuất |
| HisDosageForm | HIS.Desktop.Plugins.HisDosageForm | Quản lý dạng bào chế |
| HisHowToUse | HIS.Desktop.Plugins.HisHtu | Quản lý đường dùng |
| HisMedicineUseForm | HIS.Desktop.Plugins.HisMedicineUseForm | Quản lý cách dùng |
| HisPackingType | HIS.Desktop.Plugins.HisPackingType | Quản lý quy cách đóng gói |
| HisMedicineTypeAcin | HIS.Desktop.Plugins.HisMedicineTypeAcin | Quản lý hoạt chất |
| HisServicePatyList | HIS.Desktop.Plugins.HisServicePatyList | Chính sách giá đối tượng |
| HisServiceHein | HIS.Desktop.Plugins.HisServiceHein | Giá BHYT dịch vụ |
| MedicineTypeCreateParent | HIS.Desktop.Plugins.MedicineTypeCreateParent | Thuốc cha/con |
| HisATC | HIS.Desktop.Plugins.HisATCSetUp | Thiết lập mã ATC |
| HisSourceMedicine | HIS.Desktop.Plugins.HisSourceMedicine | Nguồn gốc thuốc |
| HisAtcGroup | HIS.Desktop.Plugins.HisAtcGroup | Nhóm ATC |

---

## 7. API Endpoints

| Method | Endpoint | Mục đích |
|--------|---------|---------|
| GET | `api/HisMedicineType/GetView` | Lấy chi tiết loại thuốc (filter ID) |
| POST | `api/HisMedicineType/UpdateSdo` | Cập nhật loại thuốc (ActionEdit) |
| POST | `api/HisMedicineType/Create` | Tạo mới loại thuốc (ActionAdd) |
| GET | `api/HisService/GetView` | Lấy chi tiết dịch vụ |
| POST | `api/HisService/UpdateSdo` | Cập nhật dịch vụ (IS_NO_HEIN_LIMIT_FOR_SPECIAL) |
| GET | `api/HisServiceUnit/Get` | Lấy danh sách đơn vị dịch vụ |
| GET | `api/HisDepaPatientType/Get` | Lấy chính sách giá theo khoa/đối tượng |
| POST | `api/HisDepaPatientType/CreateList` | Tạo danh sách chính sách giá |
| POST | `api/HisDepaPatientType/DeleteList` | Xóa danh sách chính sách giá |

---

## 8. Điều kiện tiên quyết

1. **Room Type:** Người dùng phải đăng nhập tại phòng có loại là STOCK (`ROOM_TYPE_CODE__STOCK`), module mới được enable.
2. **Backend API:** Các endpoint `api/HisMedicineType`, `api/HisService`, `api/HisServiceUnit`, `api/HisDepaPatientType` phải hoạt động.
3. **Database:** Các bảng/view: `HIS_MEDICINE_TYPE`, `V_HIS_MEDICINE_TYPE`, `HIS_SERVICE`, `V_HIS_SERVICE`, `HIS_SERVICE_UNIT`, `HIS_SUPPLIER`, `HIS_DEPARTMENT`, `V_HIS_ROOM`, `HIS_CONTRAINDICATION`, `HIS_PROCESSING_METHOD`, `HIS_DOSAGE_FORM`, `HIS_DEPA_PATIENT_TYPE`, `HIS_EXP_MEST_TYPE`.
4. **HisConfig (Oracle):** Hai khóa cấu hình cần có:
   - `HIS.DESKTOP.PRESCRIPTION.ATC_CODE_OVERLAP.WARNING_OPTION`
   - `MOS.HIS_SERVICE.SERVICE_CODE_OPTION` (giá trị "1" = tự nhập mã)
5. **Module Link:** Insert record modulelink trong database cho `HIS.Desktop.Plugins.MedicineTypeCreate`.
6. **Model:** `HIS_MEDICINE_TYPE`, `V_HIS_MEDICINE_TYPE`, `HisMedicineTypeSDO`, `HisMedicineTypeViewFilter` phải được gen vào MOS.EFMODEL và MOS.Filter.
