# Tài liệu phân tích thiết kế
# HIS.Desktop.Plugins.ImpMestCreate - Nhập thuốc vật tư

---

## 1. Mục đích

Cung cấp chức năng tạo phiếu nhập kho thuốc và vật tư y tế vào kho dược của bệnh viện. Module hỗ trợ 4 loại nhập kho khác nhau, quản lý gói thầu, hợp đồng cung ứng, tính giá bán theo đối tượng bệnh nhân, và kiểm soát chứng từ nhà cung cấp.

**Đối tượng sử dụng:** Nhân viên kho dược, nhân viên mua sắm, thủ kho.

**4 loại nhập kho:**
- **DK** (Đăng ký): Nhập kho ban đầu / đăng ký tồn kho
- **KK** (Kiểm kê): Nhập bổ sung sau kiểm kê kho
- **NCC** (Nhà cung cấp): Nhập kho từ nhà cung cấp / nhà sản xuất (phổ biến nhất)
- **KHAC** (Khác): Các trường hợp nhập kho khác

---

## 2. Cấu trúc project

```
HIS.Desktop.Plugins.ImpMestCreate/
├── ADO/
│   ├── BidValidTimeADO.cs
│   ├── DosageFormADO.cs
│   ├── ExpMestMaterialADO.cs
│   ├── ExpMestMedicineADO.cs
│   ├── ImportMediMateADO.cs
│   ├── ManufacturerADO.cs
│   ├── NationalADO.cs
│   ├── ResultImpMestADO.cs
│   ├── SupplierADO.cs
│   ├── VHisServiceADO.cs
│   └── VHisServicePatyADO.cs
├── Base/
│   ├── ResourceLangManager.cs
│   ├── ResourceMessageManager.cs
│   └── StaticMethod.cs
├── Config/
│   ├── HisBidAlertAmountCFG.cs
│   ├── HisConfig.cs
│   ├── HisImpMestTypeAuthorziedCFG.cs
│   ├── IsAutoCheckNoBidCFG.cs
│   ├── IsRoundAutoExpPriceCFG.cs
│   ├── PatientTypeCFG.cs
│   └── WarningExpiredDateCFG.cs
├── Form/
│   ├── FormBidValidTime.cs
│   ├── FormBidValidTime.Designer.cs
│   ├── FormSerial.cs
│   ├── FormSerial.Designer.cs
│   ├── frmImpSourceReturn.cs
│   └── frmImpSourceReturn.Designer.cs
├── ImpMestCreate/
│   ├── IImpMestCreate.cs
│   ├── ImpMestCreateBehavior.cs
│   └── ImpMestCreateFactory.cs
├── Save/
│   ├── ISave.cs
│   ├── ISaveInit.cs
│   ├── SaveAbstract.cs
│   ├── SaveFactory.cs
│   ├── Init/
│   │   └── SaveInitBehavior.cs
│   ├── Inve/
│   │   └── SaveInveBehavior.cs
│   ├── Manu/
│   │   └── SaveManuBehavior.cs
│   └── Other/
│       └── SaveOtherBehavior.cs
├── Validation/
│   ├── BidMaxLengthValidationRule.cs
│   ├── BidValidationRule.cs
│   ├── DiscountValidationRule.cs
│   ├── DocumentDateValidationRule.cs
│   ├── DocumentValidationRule.cs
│   ├── ExpiredDateValidationRule.cs
│   ├── GoiThauNewValidationRule.cs
│   ├── ImpAmountValidationRule.cs
│   ├── ImpMestTypeValidationRule.cs
│   ├── ImpPriceValidationRule.cs
│   ├── ImpVatRatioValidationRule.cs
│   ├── MediStockValidationRule.cs
│   ├── ReUseMaterialValidationRule.cs
│   ├── SoLoValidationRule.cs
│   ├── SupplierValidationRule.cs
│   ├── TemperatureValidationRule.cs
│   └── TxtSeriNumberValidationRule.cs
├── Properties/
│   └── AssemblyInfo.cs
├── Resources/
│   ├── Lang.en.resx
│   └── Lang.vi.resx
├── UCImpMestCreate.cs
├── UCImpMestCreate.Designer.cs
├── UCImpMestCreate__Plus__Button.cs
├── UCImpMestCreate__Plus__Control.cs
├── UCImpMestCreate__Plus__GoiThau.cs
├── UCImpMestCreate__Plus__Load.cs
├── UCImpMestCreate__Plus__Print141.cs
├── UCImpMestCreate__Plus__Task.cs
├── UCImpMestCreate__Plus__TreeList.cs
├── UCImpMestCreate__Plus__Update.cs
├── ImpMestCreateProcessor.cs
├── KeyboardWorker.cs
├── Delegate.cs
└── HIS.Desktop.Plugins.ImpMestCreate.csproj
```

---

## 3. Đăng ký Module

**File:** `ImpMestCreateProcessor.cs`

```
Module Link  : HIS.Desktop.Plugins.ImpMestCreate
Tên hiển thị : Nhập thuốc vật tư
Icon         : nhap-kho.png
Nhóm         : Common
Thứ tự       : 57
Loại         : MODULE_TYPE_ID__UC
```

**Luồng khởi tạo:**
```
ImpMestCreateProcessor.Run(args)
  → ImpMestCreateFactory.MakeIImpMestCreate(param, args)
    → ImpMestCreateBehavior.Run()
      → new UCImpMestCreate(moduleData, roomTypeId, roomId)
```

- `ImpMestCreateProcessor` kế thừa `ModuleBase, IDesktopRoot`
- `ImpMestCreateBehavior` nhận `Module` từ entity array, trích xuất `roomTypeId` và `roomId`
- `UCImpMestCreate` kế thừa `UserControlBase`, nhận context kho thuốc (medistockID) từ module

---

## 4. Thiết kế chi tiết

### 4.1. Tổng quan giao diện

```
┌─────────────────────────────────────────────────────────────────────────────┐
│ [cboImpMestType] Loại nhập │ [cboMediStock] Kho │ [cboImpSource] Nguồn    │
│ [txtDocumentNumber] Số CT  │ [dtDocumentDate] Ngày CT │ [txtDeliverer]      │
├─────────────────────────────────────────────────────────────────────────────┤
│ [txtNhaCC] Nhà cung cấp    │ [cboGoiThau] Gói thầu │ [cboInformationBid]   │
│ [txtBidNumber] Số QĐ thầu  │ [txtBidYear] Năm thầu │ [txtBidGroupCode]     │
├───────────────────────┬─────────────────────────────────────────────────────┤
│ Tab Thuốc / Tab VT    │  Thông tin chi tiết thuốc/vật tư                   │
│ ┌───────────────────┐ │  Số lượng │ Giá nhập │ VAT │ Hạn dùng │ Số lô     │
│ │ ucMedicineTypeTree│ │  Hãng SX │ Quốc gia │ Nồng độ │ Số đăng ký         │
│ │ ucMaterialTypeTree│ │  Dạng bào chế │ Đường dùng │ Nhiệt độ             │
│ └───────────────────┘ │  [btnAdd1] Thêm │ [btnUpdate1] Sửa │ [btnCancel1] │
├───────────────────────┴─────────────────────────────────────────────────────┤
│ gridControlImpMestDetail                                                    │
│ STT │ Xóa │ Sửa │ Tên │ SL │ Giá │ VAT% │ Giá VAT │ HSD │ Số lô │ ...   │
├─────────────────────────────────────────────────────────────────────────────┤
│ gridControlServicePaty (Giá bán theo đối tượng bệnh nhân)                  │
│ Đối tượng │ VAT% │ Giá+VAT │ %Lợi nhuận │ Giá bán │ Không bán            │
├─────────────────────────────────────────────────────────────────────────────┤
│ Tổng tiền: [...] │ Tổng phí: [...] │ Tổng VAT: [...]                       │
│ TK Có: [...] │ TK Nợ: [...]                                                │
├─────────────────────────────────────────────────────────────────────────────┤
│ [btnNew] Mới │ [btnSave] Lưu │ [btnSaveDraft] Lưu nháp                     │
│ [btnImportExcel] Nhập Excel │ [btnPrint] In │ [btnHoiDongKiemNhap]         │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4.2. Các đối tượng dữ liệu (ADO)

#### 4.2.1. VHisServiceADO

Data object chính đại diện cho một dòng thuốc/vật tư trong danh sách nhập kho.

**File:** `ADO/VHisServiceADO.cs`

| Property | Type | Mô tả |
|----------|------|-------|
| MEDI_MATE_ID | long | ID loại thuốc/vật tư |
| MEDI_MATE_CODE | string | Mã thuốc/vật tư |
| MEDI_MATE_NAME | string | Tên thuốc/vật tư |
| IsMedicine | bool | `true` = thuốc, `false` = vật tư |
| IMP_AMOUNT | decimal | Số lượng nhập |
| IMP_PRICE | decimal | Giá nhập (chưa VAT) |
| IMP_VAT_RATIO | decimal | Tỷ lệ VAT (dạng thập phân, VD: 0.1 = 10%) |
| ImpVatRatio | decimal | Tỷ lệ VAT (dạng phần trăm, VD: 10 = 10%) |
| EXPIRED_DATE | long? | Hạn sử dụng (format yyyyMMddHHmmss) |
| PACKAGE_NUMBER | string | Số lô |
| SERIAL_NUMBER | string | Số serial (vật tư tái sử dụng) |
| BidId | long? | ID gói thầu |
| SupplierId | long | ID nhà cung cấp |
| TDL_BID_GROUP_CODE | string | Mã nhóm thầu |
| TDL_BID_NUM_ORDER | string | Số thứ tự thầu |
| TDL_BID_YEAR | string | Năm thầu |
| TDL_BID_NUMBER | string | Số quyết định thầu |
| TDL_BID_PACKAGE_CODE | string | Mã gói thầu |
| TDL_BID_EXTRA_CODE | string | Mã phụ thầu |
| MANUFACTURER_ID | long? | ID hãng sản xuất |
| NATIONAL_NAME | string | Quốc gia sản xuất |
| CONCENTRA | string | Nồng độ/hàm lượng |
| REGISTER_NUMBER | string | Số đăng ký |
| MEDICAL_CONTRACT_ID | long? | ID hợp đồng |
| CONTRACT_PRICE | decimal? | Giá hợp đồng |
| TAX_RATIO | decimal? | Tỷ lệ thuế |
| TEMPERATURE | decimal? | Nhiệt độ bảo quản |
| MAX_REUSE_COUNT | long? | Số lần tái sử dụng tối đa (vật tư) |
| IsReusable | bool | Vật tư có thể tái sử dụng |
| IsIdentity | bool | Vật tư quản lý định danh |
| IsRequireHsd | bool | Bắt buộc nhập hạn sử dụng |
| DOCUMENT_PRICE | long? | Giá chứng từ |
| HeinLimitPrice | decimal? | Giá trần BHYT |
| HisMedicine | HIS_MEDICINE | Đối tượng lô thuốc |
| HisMaterial | HIS_MATERIAL | Đối tượng lô vật tư |
| HisMedicinePatys | List | Giá thuốc theo đối tượng |
| HisMaterialPatys | List | Giá vật tư theo đối tượng |
| VHisServicePatys | List | Giá dịch vụ theo đối tượng (hiển thị trên grid) |

**Constructor:**
- `VHisServiceADO(V_HIS_MEDICINE_TYPE data)` — Khởi tạo từ loại thuốc, set `IsMedicine = true`, map giá nhập mặc định, thông tin BHYT, dạng bào chế, vaccine, điều kiện bảo quản
- `VHisServiceADO(V_HIS_MATERIAL_TYPE data)` — Khởi tạo từ loại vật tư, set `IsMedicine = false`, map thông tin tái sử dụng, định danh, phí vệ sinh

**Error enum:** ThieuMa, ThieuGiaNhap, ThieuVat, ThieuSoLuong, SaiGiaNhap, SaiVat, SaiSoLuong, SaiMa, MaxLengthGoiThau, ...

**Warm enum:** ThangKhongHopLe, NgayKhongHopLe, GioKhongHopLe, HanDungKhongHopLe, KhongCoTuoiTho, ...

#### 4.2.2. VHisServicePatyADO

Giá bán theo từng đối tượng bệnh nhân. Kế thừa `V_HIS_SERVICE_PATY`.

**File:** `ADO/VHisServicePatyADO.cs`

| Property | Type | Mô tả |
|----------|------|-------|
| ExpVatRatio | decimal | Tỷ lệ VAT xuất (%) |
| PercentProfit | decimal | % lợi nhuận |
| ExpPriceVat | decimal | Giá bán có VAT |
| ExpPrice | decimal | Giá bán |
| IsNotSell | bool | Không bán (check = true → bỏ qua đối tượng này) |
| IsNotEdit | bool | Không cho sửa |
| IsSetExpPrice | bool | Đã thiết lập giá bán |
| ServiceId | long | ID dịch vụ |
| ServiceTypeId | long | ID loại dịch vụ |
| PRE_PRICE_Str | decimal? | Giá trước đó |

#### 4.2.3. ResultImpMestADO

Kết quả trả về sau khi lưu phiếu nhập, bọc 4 loại SDO tương ứng.

**File:** `ADO/ResultImpMestADO.cs`

| Property | Type | Mô tả |
|----------|------|-------|
| HisManuSDO | HisImpMestManuSDO | Kết quả nhập NCC |
| HisInitSDO | HisImpMestInitSDO | Kết quả nhập DK |
| HisInveSDO | HisImpMestInveSDO | Kết quả nhập KK |
| HisOtherSDO | HisImpMestOtherSDO | Kết quả nhập KHAC |
| ImpMestTypeId | long | Loại nhập kho đã lưu |
| ImpMestSttId | long | Trạng thái phiếu (luôn = REQUEST sau lưu) |
| ImpMestUpdate | HIS_IMP_MEST | Đối tượng phiếu nhập đã lưu |

#### 4.2.4. ImportMediMateADO

Cấu trúc dữ liệu khi nhập từ Excel.

**File:** `ADO/ImportMediMateADO.cs`

Các trường chính: IS_MEDICINE, MEDI_MATE_CODE, IMP_AMOUNT, IMP_PRICE, IMP_VAT_RATIO, EXPIRED_DATE_STR, PACKAGE_NUMBER, TDL_BID_GROUP_CODE, TDL_BID_NUM_ORDER, TDL_BID_YEAR, TDL_BID_PACKAGE_CODE, TDL_BID_NUMBER, và tối đa 10 cặp (PATIENT_TYPE_CODE, EXP_PRICE) cho giá bán theo đối tượng.

#### 4.2.5. Các ADO phụ trợ

| ADO | File | Mô tả |
|-----|------|-------|
| BidValidTimeADO | ADO/BidValidTimeADO.cs | Hiển thị thời hạn hiệu lực gói thầu |
| SupplierADO | ADO/SupplierADO.cs | Thông tin nhà cung cấp (lookup) |
| ManufacturerADO | ADO/ManufacturerADO.cs | Thông tin hãng sản xuất (lookup) |
| NationalADO | ADO/NationalADO.cs | Thông tin quốc gia (lookup) |
| DosageFormADO | ADO/DosageFormADO.cs | Thông tin dạng bào chế (lookup) |
| ExpMestMedicineADO | ADO/ExpMestMedicineADO.cs | Thuốc xuất kho (cho form trả lại) |
| ExpMestMaterialADO | ADO/ExpMestMaterialADO.cs | Vật tư xuất kho (cho form trả lại) |

---

### 4.3. Loại nhập kho (IMP_MEST_TYPE)

| Code | Tên | Mô tả | API Create | API Update | SDO |
|------|-----|-------|-----------|------------|-----|
| DK | Đăng ký | Nhập tồn kho ban đầu, đăng ký thuốc/VT mới vào hệ thống | api/HisImpMest/InitCreate | api/HisImpMest/InitUpdate | HisImpMestInitSDO |
| KK | Kiểm kê | Nhập bổ sung sau đợt kiểm kê phát hiện chênh lệch tồn kho | api/HisImpMest/InveCreate | api/HisImpMest/InveUpdate | HisImpMestInveSDO |
| NCC | Nhà cung cấp | Nhập từ nhà cung cấp/nhà sản xuất, có chứng từ hóa đơn | api/HisImpMest/ManuCreate | api/HisImpMest/ManuUpdate | HisImpMestManuSDO |
| KHAC | Khác | Các nguồn nhập khác (viện trợ, tài trợ, luân chuyển...) | api/HisImpMest/OtherCreate | api/HisImpMest/OtherUpdate | HisImpMestOtherSDO |

**Đặc điểm riêng loại NCC:**
- Bắt buộc chọn nhà cung cấp (Supplier)
- Có thêm các trường: Người giao hàng (Deliverer), Số chứng từ (DocumentNumber), Ngày chứng từ (DocumentDate), Giá chứng từ (DocumentPrice), Giá VAT chứng từ (DocumentVatPrice), Ký hiệu hóa đơn (InvoiceSymbol)
- Kiểm tra trùng số chứng từ (cấu hình được)
- Hỗ trợ chiết khấu (Discount)

---

### 4.4. Luồng xử lý chính

#### 4.4.1. Khởi tạo form

```
UCImpMestCreate_Load
  ├── TaskAll()
  │     ├── GetImpMestTypeAllow()    → Tải loại nhập kho được phép
  │     ├── backgroundWorker1        → Tải cây thuốc (ucMedicineTypeTree)
  │     └── backgroundWorker2        → Tải cây vật tư (ucMaterialTypeTree)
  ├── GetBid()                       → Tải danh sách gói thầu (V_HIS_BID_1)
  ├── GetContract()                  → Tải hợp đồng (HIS_MEDICAL_CONTRACT)
  ├── GetSupplier()                  → Tải nhà cung cấp (HIS_SUPPLIER)
  ├── GetImpSource()                 → Tải nguồn nhập (HIS_IMP_SOURCE)
  ├── LoadReceiver()                 → Tải người nhận (ACS_USER)
  ├── LoadSaleProfits()              → Tải cấu hình lợi nhuận bán hàng
  ├── LoadManufacturer()             → Tải hãng sản xuất
  ├── LoadDosageForm()               → Tải dạng bào chế
  ├── LoadNation()                   → Tải quốc gia
  ├── InitControls()                 → Khởi tạo combo, grid, validation
  └── LoadControlState()             → Khôi phục lựa chọn lần trước
```

**Mặc định:** Loại NCC được chọn đầu tiên. Nếu kho thuốc có `IS_ALLOW_IMP_SUPPLIER == 0`, phần nhà cung cấp sẽ bị ẩn.

#### 4.4.2. Chọn thuốc/vật tư từ cây

**Tab Thuốc** (`ucMedicineTypeTree`):
```
medicineTypeTree_Click
  ├── Tạo VHisServiceADO từ V_HIS_MEDICINE_TYPE đang chọn
  ├── Gọi api/HisMedicine/Get (filter MEDICINE_TYPE_ID, order IMP_TIME DESC)
  │     → Lấy thông tin lô nhập gần nhất (giá nhập, VAT, số lô, HSD...)
  ├── Pre-fill: IMP_PRICE, IMP_VAT_RATIO từ medicine type defaults
  ├── Load danh sách giá theo đối tượng bệnh nhân → gridControlServicePaty
  └── Hiển thị thông tin chi tiết: dạng bào chế, hoạt chất, số đăng ký...
```

**Tab Vật tư** (`ucMaterialTypeTree`):
```
materialTypeTree_Click
  ├── Tạo VHisServiceADO từ V_HIS_MATERIAL_TYPE đang chọn
  ├── Gọi api/HisMaterial/Get (tương tự thuốc)
  ├── Kiểm tra IS_REUSABLE → hiện trường MAX_REUSE_COUNT
  ├── Kiểm tra IS_IDENTITY_MANAGEMENT → hiện trường SERIAL_NUMBER
  └── Load danh sách giá theo đối tượng bệnh nhân
```

#### 4.4.3. Nhập thông tin chi tiết

**Các trường chung (thuốc & vật tư):**

| Trường | Control | Bắt buộc | Mô tả |
|--------|---------|----------|-------|
| Số lượng | spinImpAmount | Có (> 0) | Số lượng cần nhập kho |
| Giá nhập | spinImpPrice | Có (>= 0) | Đơn giá nhập (chưa VAT) |
| VAT (%) | spinImpVatRatio | Không | Tỷ lệ VAT (0-100%) |
| Hạn sử dụng | dtExpiredDate | Tùy config | Format: dd/MM/yyyy hoặc MM/yyyy |
| Số lô | txtPackageNumber | Không | Mã lô sản xuất |
| Hãng sản xuất | cboHangSX | Không | Chọn từ danh mục |
| Quốc gia | cboNationals | Không | Quốc gia sản xuất |
| Số đăng ký | txtSoDangKy | Không | Số đăng ký thuốc/VT |
| Nhiệt độ | spnTemperature | Không | Nhiệt độ bảo quản (°C) |
| Giá hợp đồng | spinEditGiaTrongThau | Không | Giá trong hợp đồng cung ứng |
| Mã QĐ thầu | txtBidNumber | Tùy | Số quyết định trúng thầu |
| Năm thầu | txtBidYear | Tùy | Năm trúng thầu |
| Nhóm thầu | txtBidGroupCode | Tùy | Mã nhóm trong gói thầu |
| TT thầu | txtTTThau | Không | Thông tin thầu bổ sung |

**Các trường riêng thuốc:**

| Trường | Control | Mô tả |
|--------|---------|-------|
| Dạng bào chế | cboDosageForm | Viên nén, viên nang, dung dịch... |
| Hoạt chất BHYT | txtActiveIngrBhytName | Tên hoạt chất theo danh mục BHYT |
| Đường dùng | cboMedicineUseForm | Uống, tiêm, truyền... |
| Nồng độ/Hàm lượng | txtNognDoHL | Nồng độ hoạt chất |
| Mô tả loại thuốc | txtDescriptionMedicineType | Thông tin bổ sung |

**Các trường riêng vật tư:**

| Trường | Control | Mô tả |
|--------|---------|-------|
| Số serial | txtSerialNumber | Cho vật tư quản lý định danh (IS_IDENTITY) |
| Số lần tái sử dụng | SpMaxReuseCount | Cho vật tư tái sử dụng (IS_REUSABLE) |
| Phí vệ sinh | spinEditGiaVeSinh | Phí vệ sinh vật tư tái sử dụng |

#### 4.4.4. Thêm vào danh sách

```
btnAdd1_Click (phím tắt: Ctrl+A)
  ├── Validate tất cả trường (17 validation rules)
  ├── Kiểm tra thuốc/VT có trong gói thầu (nếu chọn gói thầu)
  ├── Kiểm tra trùng lặp trong listServiceADO
  ├── Tạo HisMedicinePatys hoặc HisMaterialPatys từ grid ServicePaty
  ├── Thêm VHisServiceADO vào listServiceADO
  ├── Cập nhật gridControlImpMestDetail
  ├── Tính lại tổng tiền: CalculTotalPrice(), CalculTotalVatPrice()
  └── Reset các trường nhập về mặc định
```

#### 4.4.5. Sửa/Xóa dòng trong grid

**Sửa:**
```
repositoryItemBtnEdit_ButtonClick
  ├── Lấy VHisServiceADO từ dòng đang chọn
  ├── Load dữ liệu vào các trường nhập
  ├── Xử lý riêng thuốc: load dạng bào chế, đường dùng, hoạt chất
  ├── Xử lý riêng vật tư: load serial, max reuse count
  ├── Load lại gridControlServicePaty với giá đối tượng hiện tại
  └── Chuyển sang chế độ "Cập nhật" (btnUpdate1 active, btnAdd1 disabled)
```

**Cập nhật:**
```
btnUpdate1_Click (phím tắt: Ctrl+U)
  ├── Validate lại tất cả trường
  ├── Cập nhật VHisServiceADO tại vị trí đang sửa trong listServiceADO
  ├── Cập nhật gridControlImpMestDetail
  ├── Tính lại tổng tiền
  └── Chuyển về chế độ "Thêm mới"
```

**Xóa:**
```
repositoryItemBtnDelete_ButtonClick
  ├── Xóa VHisServiceADO khỏi listServiceADO
  ├── Cập nhật gridControlImpMestDetail
  ├── Tính lại tổng tiền
  └── Nếu danh sách rỗng → clear các trường chi tiết
```

#### 4.4.6. Lưu phiếu nhập

```
btnSave_Click (phím tắt: Ctrl+S)
  ├── Kiểm tra listServiceADO không rỗng
  ├── Validate toàn bộ dữ liệu (errors & warnings)
  ├── SaveFactory.MakeIServiceRequestRegister(...)
  │     └── Dựa vào ImpMestTypeId → tạo SaveBehavior tương ứng
  ├── SaveBehavior.Run()
  │     ├── CheckValid()
  │     ├── [NCC] CheckValidateDocumentNumberAndDocumentDate()
  │     │         → Cảnh báo nếu số/ngày chứng từ rỗng
  │     ├── [NCC] CheckDocumentNumber()
  │     │         → Kiểm tra trùng số chứng từ (xem mục 4.4.6.1)
  │     ├── InitBase()
  │     │     ├── GenerateListMediMaty()  → Chuyển ADO → SDO
  │     │     └── GenerateImpMestData()   → Tạo HIS_IMP_MEST
  │     └── Gọi API (Create hoặc Update)
  ├── Thành công → Hiển thị kết quả, enable nút In
  └── Thất bại → Hiển thị thông báo lỗi
```

**4.4.6.1. Kiểm tra trùng số chứng từ (chỉ loại NCC):**

```
CheckDocumentNumber()
  ├── Nếu DocumentNumber rỗng → bỏ qua
  ├── Gọi api/HisImpMest/Get (filter: DOCUMENT_NUMBER__EXACT)
  ├── Nếu có InvoiceSymbol:
  │     → Lọc: cùng Supplier + cùng DocumentNumber + cùng InvoiceSymbol
  ├── Nếu không có InvoiceSymbol:
  │     → Lọc: cùng Supplier + cùng DocumentNumber + InvoiceSymbol rỗng
  ├── Loại trừ phiếu đang sửa (khi Update)
  ├── Nếu tìm thấy trùng:
  │     ├── IsAllowDuplicateDocument = true → cho qua
  │     ├── IsAllowDuplicateDocument = false → CHẶN, hiện thông báo lỗi
  │     └── IsShowMessDocument = true → CẢNH BÁO nhưng vẫn cho lưu
  └── Không trùng → tiếp tục lưu
```

#### 4.4.7. Lưu nháp

```
btnSaveDraft_Click (phím tắt: Ctrl+D)
  → Luồng tương tự btnSave nhưng xử lý trạng thái nháp
```

#### 4.4.8. Tạo mới

```
btnNew_Click (phím tắt: Ctrl+N)
  ├── Clear listServiceADO
  ├── Clear gridControlImpMestDetail
  ├── Reset tất cả trường nhập về mặc định
  ├── Reset tổng tiền về 0
  └── Disable nút In, Hội đồng kiểm nhập
```

---

### 4.5. Xử lý gói thầu

#### 4.5.1. Chọn gói thầu trước

```
cboGoiThau_EditValueChanged
  ├── Load V_HIS_BID_MEDICINE_TYPE → dicBidMedicine (key: MEDI_MATE_ID + BID_GROUP_CODE)
  ├── Load V_HIS_BID_MATERIAL_TYPE → dicBidMaterial
  ├── Lọc cây thuốc/vật tư theo gói thầu đã chọn
  └── Auto-fill: BID_GROUP_CODE, BID_NUM_ORDER, BID_YEAR, BID_PACKAGE_CODE
```

#### 4.5.2. Chọn gói thầu theo loại thuốc/vật tư

Khi chọn một thuốc/vật tư từ cây mà chưa chọn gói thầu:
```
ProcessBidByType()
  ├── Tìm gói thầu chứa loại thuốc/vật tư đang chọn
  ├── Nếu có nhà cung cấp → lọc thêm theo supplier
  └── Cập nhật cboGoiThau với danh sách gói thầu phù hợp
```

#### 4.5.3. Kiểm tra thuốc trong gói thầu

Khi lưu, `CheckInBid()` trong `SaveAbstract` kiểm tra:
- Tạo key: `StaticMethod.GetTypeKey(MEDI_MATE_ID, TDL_BID_GROUP_CODE)`
- Tra cứu trong `dicBidMedicine` hoặc `dicBidMaterial`
- Nếu không tìm thấy → cảnh báo "thuốc/VT không thuộc gói thầu"

#### 4.5.4. Ngoài thầu

Checkbox `chkNgoaiThau`: Khi tích, cho phép nhập thuốc/VT không nằm trong gói thầu nào.
- Các trường thầu (BID_NUMBER, BID_YEAR, BID_GROUP_CODE...) được mở cho phép nhập tay
- Không validate gói thầu khi lưu

#### 4.5.5. Cảnh báo số lượng nhập thầu

Config: `HIS.DESKTOP.IMP_MEST_CREATE.BID_MEDI_MATE.ALERT_AMOUNT`
- Khi số lượng đã nhập trong kỳ thầu vượt ngưỡng cảnh báo → hiện cảnh báo cho người dùng

---

### 4.6. Xử lý hợp đồng

```
Chọn hợp đồng (cboContract)
  ├── Tải V_HIS_MEDI_CONTRACT_METY (thuốc trong hợp đồng)
  ├── Tải V_HIS_MEDI_CONTRACT_MATY (vật tư trong hợp đồng)
  └── Khi chọn thuốc/VT → map CONTRACT_PRICE từ hợp đồng
```

- Hợp đồng được lọc theo nhà cung cấp và gói thầu đang chọn
- Giá hợp đồng (CONTRACT_PRICE) được gán vào `VHisServiceADO.CONTRACT_PRICE` và `MEDICAL_CONTRACT_ID`

---

### 4.7. Xử lý nhà cung cấp

```
txtNhaCC_ButtonClick (GridLookUpEdit)
  ├── Hiển thị danh sách HIS_SUPPLIER
  └── Khi chọn supplier:
        ├── Reload gói thầu (lọc theo supplier)
        ├── Reload hợp đồng (lọc theo supplier)
        └── Set SupplierId cho VHisServiceADO
```

- **Bắt buộc** khi loại nhập = NCC
- Khi đổi nhà cung cấp → reset gói thầu, hợp đồng
- Mỗi thuốc/VT thêm vào sẽ được gán `SupplierId` hiện tại

---

### 4.8. Chính sách giá theo đối tượng bệnh nhân

Grid `gridControlServicePaty` hiển thị giá bán cho từng đối tượng bệnh nhân:

| Cột | Mô tả | Sửa được |
|-----|-------|----------|
| Đối tượng BN | Tên đối tượng (BHYT, Thu phí, Viện phí...) | Không |
| VAT xuất (%) | Tỷ lệ VAT khi bán | Có |
| Giá + VAT | Giá bán đã bao gồm VAT | Có |
| % Lợi nhuận | Phần trăm lợi nhuận trên giá nhập | Tùy config |
| Giá bán | Giá bán chưa VAT | Có |
| Không bán | Check = không bán cho đối tượng này | Có |

**Quy tắc nghiệp vụ:**
- Config `EditSaleProfit = 1`: Cho phép sửa % lợi nhuận trên grid
- Config `ApplyServicePatyPrice = 1`: Áp dụng giá dịch vụ theo đối tượng bệnh nhân
- Khi lưu: `PROFIT_RATIO` = PercentProfit / 100 của đối tượng đầu tiên **không phải BHYT**, có PercentProfit > 0, và không tích "Không bán"

---

### 4.9. Công thức tính giá

| Công thức | Diễn giải |
|-----------|-----------|
| `Giá VAT = Giá nhập × (1 + Tỷ lệ VAT / 100)` | Đơn giá nhập bao gồm VAT |
| `Tổng tiền có VAT = Số lượng × Giá nhập × (1 + Tỷ lệ VAT)` | Tổng tiền 1 dòng thuốc/VT |
| `DOCUMENT_PRICE = Round(SL × Giá × (1 + VAT_RATIO), 0, AwayFromZero)` | Giá chứng từ (làm tròn) |
| `Giá bán = Giá nhập × (1 + % Lợi nhuận / 100)` | Giá bán theo đối tượng |
| `Chiết khấu = Tổng giá nhập × Tỷ lệ chiết khấu / 100` | Chỉ áp dụng loại NCC |
| `DISCOUNT_RATIO = DiscountRatio / 100` | Tỷ lệ chiết khấu (lưu dạng thập phân) |

**Lưu ý:**
- `IMP_VAT_RATIO` lưu dạng thập phân (0.1 = 10%), `ImpVatRatio` lưu dạng phần trăm (10 = 10%)
- Giá chứng từ (DOCUMENT_PRICE) sử dụng `MidpointRounding.AwayFromZero` để làm tròn
- Config `IsAutoRoundExpPrice = 1`: Tự động làm tròn giá bán

---

### 4.10. Nhập từ Excel

```
btnImportExcel_Click (phím tắt: Ctrl+I)
  ├── Mở dialog chọn file Excel
  ├── Đọc file theo cấu trúc ImportMediMateADO
  ├── Validate từng dòng (mã thuốc/VT, số lượng, giá...)
  ├── Thêm vào listServiceADO
  └── Cập nhật grid và tổng tiền
```

**Tải template:** `btnDownloadTemplate_Click` — Tải file Excel mẫu với cấu trúc cột đúng format.

**Cấu trúc cột Excel:**
- IS_MEDICINE (1=thuốc, 0=VT), MEDI_MATE_CODE, IMP_AMOUNT, IMP_PRICE, IMP_VAT_RATIO
- EXPIRED_DATE_STR, PACKAGE_NUMBER
- TDL_BID_GROUP_CODE, TDL_BID_NUM_ORDER, TDL_BID_YEAR, TDL_BID_PACKAGE_CODE, TDL_BID_NUMBER
- Tối đa 10 cặp (PATIENT_TYPE_CODE_n, EXP_PRICE_n) cho giá bán theo đối tượng

---

### 4.11. In phiếu

```
dropDownButton__Print_Click
  ├── "Biên bản kiểm nhập" → PrintType: Mps000199
  │     → In biên bản kiểm nhập thuốc/vật tư
  └── "Phiếu nhập nhà cung cấp"
        → Chỉ hiện khi loại = NCC và trạng thái = IMPORT
```

Sử dụng thư viện `HIS.Desktop.Print` để xử lý in ấn.

---

### 4.12. Hội đồng kiểm nhập

Nút `btnHoiDongKiemNhap`: Liên quan đến quy trình phê duyệt/kiểm nhập. Hiển thị sau khi phiếu nhập đã được lưu thành công.

---

## 5. Quy tắc validation

| STT | Rule | File | Trường | Điều kiện | Thông báo |
|-----|------|------|--------|-----------|-----------|
| 1 | ImpMestTypeValidationRule | ImpMestTypeValidationRule.cs | cboImpMestType | Phải có giá trị | Trường dữ liệu bắt buộc |
| 2 | MediStockValidationRule | MediStockValidationRule.cs | cboMediStock | Phải có giá trị | Trường dữ liệu bắt buộc |
| 3 | SupplierValidationRule | SupplierValidationRule.cs | txtNhaCC | Bắt buộc khi loại = NCC | Trường dữ liệu bắt buộc |
| 4 | ImpAmountValidationRule | ImpAmountValidationRule.cs | spinImpAmount | Phải > 0 | Số lượng phải lớn hơn 0 |
| 5 | ImpPriceValidationRule | ImpPriceValidationRule.cs | spinImpPrice | Phải >= 0 | Giá nhập không hợp lệ |
| 6 | ImpVatRatioValidationRule | ImpVatRatioValidationRule.cs | spinImpVatRatio | Tỷ lệ VAT hợp lệ | VAT không hợp lệ |
| 7 | DocumentValidationRule | DocumentValidationRule.cs | txtDocumentNumber | Max 50 ký tự, bắt buộc cho NCC | Vượt quá maxlength (50 ký tự) |
| 8 | DocumentDateValidationRule | DocumentDateValidationRule.cs | dtDocumentDate | Format ngày hợp lệ | Ngày không hợp lệ |
| 9 | ExpiredDateValidationRule | ExpiredDateValidationRule.cs | dtExpiredDate | Format ngày hợp lệ | Hạn dùng không hợp lệ |
| 10 | BidValidationRule | BidValidationRule.cs | Mã nhóm thầu | Max length | Vượt quá độ dài cho phép |
| 11 | BidMaxLengthValidationRule | BidMaxLengthValidationRule.cs | Các trường thầu | Max length | Vượt quá độ dài cho phép |
| 12 | GoiThauNewValidationRule | GoiThauNewValidationRule.cs | Thông tin thầu mới | Hợp lệ | Gói thầu không hợp lệ |
| 13 | DiscountValidationRule | DiscountValidationRule.cs | Chiết khấu | Giá trị hợp lệ | Chiết khấu không hợp lệ |
| 14 | SoLoValidationRule | SoLoValidationRule.cs | txtPackageNumber | Hợp lệ | Số lô không hợp lệ |
| 15 | ReUseMaterialValidationRule | ReUseMaterialValidationRule.cs | Tái sử dụng | Thông tin tái sử dụng hợp lệ | Thông tin không hợp lệ |
| 16 | TemperatureValidationRule | TemperatureValidationRule.cs | spnTemperature | Nhiệt độ hợp lệ | Nhiệt độ không hợp lệ |
| 17 | TxtSeriNumberValidationRule | TxtSeriNumberValidationRule.cs | txtSerialNumber | Serial hợp lệ | Số serial không hợp lệ |

---

## 6. Cấu hình hệ thống

| Config Key | Kiểu | Mô tả |
|------------|------|-------|
| HIS.Desktop.Plugins.ImpMestCreate.EditSaleProfit | 0/1 | Cho phép sửa % lợi nhuận bán hàng trên grid đối tượng BN. `1` = sửa được |
| HIS.Desktop.Plugins.ImpMestCreate.AllowDuplicate | 0/1 | Cho phép trùng số chứng từ khi nhập NCC. `1` = bỏ qua kiểm tra trùng |
| HIS.Desktop.Plugins.ImpMestCreate.ApplyServicePatyPrice | 0/1 | Sử dụng giá dịch vụ theo đối tượng bệnh nhân. `1` = áp dụng |
| HIS.Desktop.Plugins.ImpMestCreate.OnlyShowBidInfo | 0/1 | Chỉ hiển thị thông tin gói thầu, ẩn các trường khác |
| HIS.Desktop.Plugins.ImpMestCreate__IsShowMessDocument | 0/1 | Hiện cảnh báo khi trùng số chứng từ (không chặn, chỉ thông báo) |
| HIS.Desktop.Plugins.ImpMestCreate__IsDisableChkImprice | 0/1 | Vô hiệu hóa checkbox giá nhập trước |
| HIS.Desktop.Plugins.ImpMestCreate__DefaultImpVAT | decimal | Giá trị VAT mặc định khi thêm dòng mới |
| HIS.Desktop.Plugins.ImpMestCreate.AllowDuplicateDocumentNumberInTheSameSupplier | 0/1 | Cho phép trùng số CT trong cùng NCC |
| HIS.Desktop.Plugins.ImpMestCreate.IsShowingApprovalBid | 0/1 | Hiển thị thông tin thầu đã duyệt |
| HIS.Desktop.Plugins.ImpMestCreate.TaxRatioOption | string | Tùy chọn hiển thị tỷ lệ thuế |
| HIS.Desktop.Plugins.ImpMestCreate.IsAutoRoundExpPrice | 0/1 | Tự động làm tròn giá bán |
| HIS.DESKTOP.IMP_MEST_CREATE.BID_MEDI_MATE.ALERT_AMOUNT | long | Ngưỡng cảnh báo số lượng nhập thầu |
| MOS.HIS_IMP_MEST.HIS_IMP_MEST_TYPE.AUTHORIZED | 0/1 | Phân quyền loại nhập kho theo user |
| MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT | string | Mã đối tượng BHYT (dùng để loại trừ khi tính PROFIT_RATIO) |
| MOS.HIS_IMP_MEST.IDENTITY_MATERIAL_OPTION | string | Tùy chọn quản lý định danh vật tư |
| WarningExpiredDate | long | Số ngày cảnh báo trước khi thuốc/VT hết hạn |
| MOS.HIS_MEDICINE.IS_SET_BHYT_INFO_FROM_TYPE_BY_DEFAULT | 0/1 | Tự động fill thông tin BHYT từ loại thuốc |

---

## 7. Xử lý lưu chi tiết theo loại (Save Behaviors)

### 7.1. SaveAbstract - Lớp cơ sở

**File:** `Save/SaveAbstract.cs`

Lớp trừu tượng chứa logic chung cho tất cả loại nhập:

**Constructor:** Trích xuất dữ liệu từ UCImpMestCreate:
- `ImpMestTypeId`, `MediStockId`, `ImpSourceId`, `SupplierId` (từ combo box)
- `dicBidMedicine`, `dicBidMaterial` (dictionary gói thầu)
- `Description`, `LogginName`, `UserName` (người nhận)
- `InvoiceSymbol`, `CREDIT_ACCOUNT`, `DEBIT_ACCOUNT`

**InitBase():**
```
InitBase()
  ├── GenerateListMediMaty()   → Chuyển VHisServiceADO → SDO
  └── GenerateImpMestData()    → Tạo HIS_IMP_MEST header
```

**GenerateListMediMaty() - Xử lý thuốc:**
```
Với mỗi VHisServiceADO có IsMedicine = true:
  ├── Tạo HisMedicineWithPatySDO
  ├── Map: BID_ID, TDL_BID_*, AMOUNT, IMP_PRICE, IMP_VAT_RATIO
  ├── Map: IMP_SOURCE_ID, SUPPLIER_ID, PACKAGE_NUMBER
  ├── Map: CONCENTRA, MEDICINE_REGISTER_NUMBER, MANUFACTURER_ID, NATIONAL_NAME
  ├── Map: HEIN_SERVICE_BHYT_NAME, PACKING_TYPE_NAME, ACTIVE_INGR_BHYT_*
  ├── Map: DOSAGE_FORM, MEDICINE_USE_FORM_ID, TAX_RATIO
  ├── Map: MEDICAL_CONTRACT_ID, CONTRACT_PRICE, HEIN_LIMIT_PRICE, DESCRIPTION
  ├── Tính: DOCUMENT_PRICE = Round(AMOUNT × IMP_PRICE × (1 + IMP_VAT_RATIO), 0)
  ├── Tính: PROFIT_RATIO = PercentProfit / 100
  │     (từ đối tượng BN đầu tiên không phải BHYT, có profit > 0, không tích "Không bán")
  ├── Set Temperature (nếu có)
  └── Set MedicinePaties (danh sách giá theo đối tượng)
```

**GenerateListMediMaty() - Xử lý vật tư:**
```
Với mỗi VHisServiceADO có IsMedicine = false:
  ├── Tạo HisMaterialWithPatySDO
  ├── Map tương tự thuốc (BID_*, AMOUNT, IMP_*, SUPPLIER_ID...)
  ├── Map: MATERIAL_REGISTER_NUMBER, CONCENTRA, MANUFACTURER_ID, NATIONAL_NAME
  ├── Tính: DOCUMENT_PRICE (công thức giống thuốc)
  ├── Tính: PROFIT_RATIO (logic giống thuốc)
  ├── Set SerialNumbers (cho vật tư tái sử dụng/định danh)
  └── Set MaterialPaties (danh sách giá theo đối tượng)
```

**GenerateImpMestData():**
```
Tạo HIS_IMP_MEST mới:
  ├── REQ_ROOM_ID = RoomId (phòng yêu cầu)
  ├── MEDI_STOCK_ID = MediStockId (kho thuốc)
  ├── IMP_MEST_STT_ID = ID__REQUEST (trạng thái: Yêu cầu)
  ├── IMP_MEST_TYPE_ID = ImpMestTypeId (loại nhập)
  ├── DESCRIPTION = Description (mô tả)
  ├── CREDIT_ACCOUNT = CREDIT_ACCOUNT (tài khoản có)
  └── DEBIT_ACCOUNT = DEBIT_ACCOUNT (tài khoản nợ)
```

### 7.2. SaveFactory

**File:** `Save/SaveFactory.cs`

Phương thức `MakeIServiceRequestRegister()` chọn SaveBehavior dựa trên `ImpMestTypeId`:

| ImpMestTypeId | SaveBehavior |
|--------------|-------------|
| ID__DK | SaveInitBehavior |
| ID__KK | SaveInveBehavior |
| ID__KHAC | SaveOtherBehavior |
| ID__NCC | SaveManuBehavior |

### 7.3. SaveInitBehavior (DK)

**File:** `Save/Init/SaveInitBehavior.cs`

```
Run()
  ├── CheckValid()
  ├── InitBase()
  ├── Tạo HisImpMestInitSDO
  │     ├── ImpMest = _ImpMestUp (nếu update) hoặc ImpMest mới
  │     ├── InitMedicines = MedicineWithPatySDOs
  │     └── InitMaterials = MaterialWithPatySDOs
  ├── Nếu tạo mới → POST api/HisImpMest/InitCreate
  └── Nếu cập nhật → POST api/HisImpMest/InitUpdate
        (reset IMP_MEST_STT_ID = ID__REQUEST)
```

### 7.4. SaveInveBehavior (KK)

**File:** `Save/Inve/SaveInveBehavior.cs`

Logic tương tự SaveInitBehavior, sử dụng `HisImpMestInveSDO` với `InveMedicines` / `InveMaterials`.

### 7.5. SaveOtherBehavior (KHAC)

**File:** `Save/Other/SaveOtherBehavior.cs`

Logic tương tự SaveInitBehavior, sử dụng `HisImpMestOtherSDO` với `OtherMedicines` / `OtherMaterials`.

### 7.6. SaveManuBehavior (NCC)

**File:** `Save/Manu/SaveManuBehavior.cs`

Phức tạp nhất, có thêm các xử lý riêng:

**Dữ liệu bổ sung (không có ở các loại khác):**

| Trường | Mô tả |
|--------|-------|
| Deliverer | Người giao hàng |
| DocumentNumber | Số chứng từ |
| DocumentDate | Ngày chứng từ (format yyyyMMddHHmmss) |
| DocumentPrice | Giá chứng từ |
| DocumentVatPrice | Giá VAT chứng từ |
| InvoiceSymbol | Ký hiệu hóa đơn |
| DiscountPrice | Số tiền chiết khấu |
| DiscountRatio | Tỷ lệ chiết khấu (%) |

**Luồng Run():**
```
Run()
  ├── CheckValid()
  ├── CheckValidateDocumentNumberAndDocumentDate()
  │     → Cảnh báo nếu Số CT hoặc Ngày CT rỗng (không chặn)
  ├── CheckDocumentNumber()
  │     → Kiểm tra trùng (xem mục 4.4.6.1), nếu trùng → return null
  ├── InitBase()
  ├── Tạo HisImpMestManuSDO
  │     ├── ImpMest (DELIVERER, DOCUMENT_*, DISCOUNT_*, INVOICE_SYMBOL)
  │     ├── RECEIVER_LOGINNAME, RECEIVER_USERNAME
  │     ├── SUPPLIER_ID
  │     ├── DISCOUNT = DiscountPrice
  │     ├── DISCOUNT_RATIO = DiscountRatio / 100
  │     ├── ManuMedicines = MedicineWithPatySDOs
  │     └── ManuMaterials = MaterialWithPatySDOs
  ├── Nếu tạo mới → POST api/HisImpMest/ManuCreate
  └── Nếu cập nhật → POST api/HisImpMest/ManuUpdate
        (reset IMP_MEST_STT_ID = ID__REQUEST)
```

---

## 8. Các form phụ

### 8.1. FormSerial - Quản lý số serial

**File:** `Form/FormSerial.cs`

Dùng cho vật tư tái sử dụng / quản lý định danh. Nhập danh sách serial number.

```
┌─────────────────────────────┐
│ STT │ Xóa │ Số Serial       │
├─────┼─────┼─────────────────┤
│  1  │  ×  │ SN-001          │
│  2  │  ×  │ SN-002          │
│  3  │  +  │                 │
└─────────────────────────────┘
```

- **Input:** Serial string hiện tại (phân cách bằng dấu `;`), số lượng expected
- **Add row:** Thêm dòng serial mới
- **Remove row:** Xóa dòng serial
- **Validation:** Tất cả serial phải được điền
- **Output:** Trả về chuỗi serial phân cách bằng `;` qua delegate

### 8.2. FormBidValidTime - Thời hạn hiệu lực gói thầu

**File:** `Form/FormBidValidTime.cs`

Hiển thị danh sách thời hạn hiệu lực của các gói thầu dưới dạng grid. Hỗ trợ xuất Excel.

### 8.3. frmImpSourceReturn - Nhập nguồn trả lại

**File:** `Form/frmImpSourceReturn.cs`

Nhập thuốc/VT từ nguồn trả lại (bán hàng trả lại):
- Tìm kiếm theo mã phiếu xuất (EXP_MEST_CODE) hoặc mã giao dịch (TRANSACTION_CODE)
- Tải danh sách thuốc/VT đã xuất từ phiếu xuất hoàn thành
- 2 tab: Thuốc và Vật tư
- Tính tỷ lệ trả lại dựa trên config:
  - `MOS.HIS_IMP_MEST.SALE_RETURN_RATIO.IN_DAY` (trả trong ngày)
  - `MOS.HIS_IMP_MEST.SALE_RETURN_RATIO.OTHER_DAY` (trả khác ngày)
- Validate: Số lượng trả <= Số lượng đã xuất
- Chọn dòng cần trả → tạo VHisServiceADO tương ứng

---

## 9. Thực thể dữ liệu & API

### 9.1. Thực thể chính

| Entity | Mô tả |
|--------|-------|
| HIS_IMP_MEST | Phiếu nhập kho (header) |
| HIS_MEDICINE | Lô thuốc (chi tiết từng lô nhập) |
| HIS_MATERIAL | Lô vật tư (chi tiết từng lô nhập) |
| HIS_MEDICINE_PATY | Giá thuốc theo đối tượng bệnh nhân |
| HIS_MATERIAL_PATY | Giá vật tư theo đối tượng bệnh nhân |
| HIS_IMP_MEST_TYPE | Danh mục loại nhập kho (DK, KK, NCC, KHAC) |
| HIS_SUPPLIER | Danh mục nhà cung cấp |
| HIS_BID / V_HIS_BID_1 | Gói thầu |
| V_HIS_BID_MEDICINE_TYPE | Thuốc trong gói thầu |
| V_HIS_BID_MATERIAL_TYPE | Vật tư trong gói thầu |
| HIS_MEDICAL_CONTRACT | Hợp đồng cung ứng |
| V_HIS_MEDI_CONTRACT_METY | Thuốc trong hợp đồng |
| V_HIS_MEDI_CONTRACT_MATY | Vật tư trong hợp đồng |
| HIS_IMP_SOURCE | Nguồn nhập |
| V_HIS_MEDICINE_TYPE | View loại thuốc |
| V_HIS_MATERIAL_TYPE | View loại vật tư |
| V_HIS_SERVICE_PATY | Giá dịch vụ theo đối tượng |
| V_HIS_MEDI_STOCK | View kho thuốc |
| HIS_SALE_PROFIT_CFG | Cấu hình lợi nhuận bán hàng |
| ACS_USER | Người dùng hệ thống (người nhận hàng) |

### 9.2. Danh sách API

| API Endpoint | Method | Mô tả | SDO |
|-------------|--------|-------|-----|
| api/HisImpMest/InitCreate | POST | Tạo phiếu nhập DK | HisImpMestInitSDO |
| api/HisImpMest/InitUpdate | POST | Cập nhật phiếu nhập DK | HisImpMestInitSDO |
| api/HisImpMest/InveCreate | POST | Tạo phiếu nhập KK | HisImpMestInveSDO |
| api/HisImpMest/InveUpdate | POST | Cập nhật phiếu nhập KK | HisImpMestInveSDO |
| api/HisImpMest/ManuCreate | POST | Tạo phiếu nhập NCC | HisImpMestManuSDO |
| api/HisImpMest/ManuUpdate | POST | Cập nhật phiếu nhập NCC | HisImpMestManuSDO |
| api/HisImpMest/OtherCreate | POST | Tạo phiếu nhập KHAC | HisImpMestOtherSDO |
| api/HisImpMest/OtherUpdate | POST | Cập nhật phiếu nhập KHAC | HisImpMestOtherSDO |
| api/HisImpMest/Get | GET | Lấy danh sách phiếu nhập (kiểm tra trùng CT) | HisImpMestFilter |
| api/HisMedicine/Get | GET | Lấy lô thuốc theo loại (lô gần nhất) | HisMedicineFilter |
| api/HisMaterial/Get | GET | Lấy lô vật tư theo loại (lô gần nhất) | HisMaterialFilter |
| api/HisBidMedicineType/GetView | GET | Lấy thuốc trong gói thầu | HisBidMedicineTypeViewFilter |
| api/HisBidMaterialType/GetView | GET | Lấy vật tư trong gói thầu | HisBidMaterialTypeViewFilter |

### 9.3. Trạng thái phiếu nhập

| Trạng thái | Mô tả |
|-----------|-------|
| ID__REQUEST | Yêu cầu — trạng thái mặc định khi tạo mới hoặc cập nhật |
| ID__IMPORT | Đã nhập — sau khi duyệt phiếu nhập (enable in phiếu NCC) |

---

## 10. Phím tắt

| Phím | Chức năng | Mô tả |
|------|-----------|-------|
| Ctrl+A | btnAdd1 | Thêm dòng thuốc/vật tư vào danh sách |
| Ctrl+U | btnUpdate1 | Cập nhật dòng đang sửa |
| Ctrl+R | btnCancel1 | Hủy sửa, quay lại chế độ thêm mới |
| Ctrl+S | btnSave | Lưu phiếu nhập |
| Ctrl+D | btnSaveDraft | Lưu nháp |
| Ctrl+N | btnNew | Tạo mới phiếu |
| Ctrl+I | btnImportExcel | Nhập từ Excel |
| Ctrl+P | btnPrint | In phiếu |
| F2 | Focus | Focus vào ô tìm kiếm |

---

## 11. Design Pattern & Dependency

### 11.1. Design Pattern

**Factory Pattern:**
```
ISaveInit                    ← Interface (Run())
SaveFactory                  ← Factory (MakeIServiceRequestRegister())
  ├── SaveInitBehavior       ← DK
  ├── SaveInveBehavior       ← KK
  ├── SaveManuBehavior       ← NCC
  └── SaveOtherBehavior      ← KHAC
```

**Abstract Pattern:**
```
SaveAbstract (abstract)      ← Lớp cơ sở chung
  ├── GenerateListMediMaty() ← Chuyển ADO → SDO (thuốc + vật tư)
  ├── GenerateImpMestData()  ← Tạo HIS_IMP_MEST header
  ├── CheckValid()           ← Validation cơ sở
  └── CheckInBid()           ← Kiểm tra thuốc/VT trong gói thầu
```

**Module Registration Pattern:**
```
[ExtensionOf(typeof(DesktopRootExtensionPoint), ...)]
ImpMestCreateProcessor : ModuleBase, IDesktopRoot
  └── Run(args) → Factory.MakeIImpMestCreate() → behavior.Run()
```

### 11.2. Project References

| Project | Mục đích |
|---------|---------|
| HIS.Desktop.ApiConsumer | ApiConsumers.MosConsumer |
| HIS.Desktop.Common | BusinessBase |
| HIS.Desktop.Controls.Session | SessionManager |
| HIS.Desktop.LocalStorage.BackendData | BackendDataWorker |
| HIS.Desktop.LocalStorage.ConfigApplication | Config ứng dụng |
| HIS.Desktop.LocalStorage.HisConfig | HisConfigs.Get |
| HIS.Desktop.Library.CacheClient | ControlStateWorker |
| HIS.Desktop.Print | Xử lý in phiếu |
| HIS.UC.MedicineType | Cây chọn loại thuốc |
| HIS.UC.MaterialType | Cây chọn loại vật tư |

### 11.3. DLL References

| DLL | Mục đích |
|-----|---------|
| DevExpress.Data.v15.2 | Data layer |
| DevExpress.Utils.v15.2 | Utility |
| DevExpress.XtraEditors.v15.2 | SpinEdit, LookUpEdit, TextEdit, DateEdit, CheckEdit |
| DevExpress.XtraGrid.v15.2 | GridControl, GridView, GridColumn |
| DevExpress.XtraLayout.v15.2 | LayoutControl |
| DevExpress.XtraBars.v15.2 | DropDownButton, BarManager |
| DevExpress.XtraTab.v15.2 | XtraTabControl |
| DevExpress.XtraTreeList.v15.2 | TreeList |
| MOS.EFMODEL | HIS_IMP_MEST, HIS_MEDICINE, HIS_MATERIAL, V_HIS_* |
| MOS.SDO | HisImpMestInitSDO, HisMedicineWithPatySDO... |
| MOS.Filter | HisImpMestFilter, HisMedicineFilter... |
| Inventec.Common.Adapter | BackendAdapter |
| Inventec.Core | CommonParam |
