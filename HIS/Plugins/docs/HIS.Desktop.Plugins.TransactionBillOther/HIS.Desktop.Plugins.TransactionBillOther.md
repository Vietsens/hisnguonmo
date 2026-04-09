# Tài liệu nghiệp vụ: Module Thanh toán khác (TransactionBillOther)

**Module:** `HIS.Desktop.Plugins.TransactionBillOther`
**Tên hiển thị:** Thanh toán khác
**Namespace:** `HIS.Desktop.Plugins.TransactionBillOther`
**Loại module:** Form (MODULE_TYPE_ID__FORM)

---

## 1. Tổng quan

Module **TransactionBillOther** là chức năng tạo hóa đơn thanh toán khác trong hệ thống HIS. Cho phép nhân viên thu ngân tạo hóa đơn cho các dịch vụ/hàng hóa không thuộc danh mục dịch vụ y tế tiêu chuẩn (ví dụ: bán vật tư, dịch vụ phụ trợ...). Module hỗ trợ quản lý thông tin người mua, danh sách hàng hóa/dịch vụ, chiết khấu, miễn giảm, nhiều hình thức thanh toán, và tích hợp hóa đơn điện tử.

---

## 2. Chức năng chính

### 2.1. Quản lý thông tin người mua
- Tìm kiếm bệnh nhân theo mã điều trị (treatment code)
- Tự động điền thông tin từ hồ sơ điều trị (`HIS_TREATMENT`):
  - Họ tên (`TDL_PATIENT_NAME`)
  - Mã số thuế (`TDL_PATIENT_TAX_CODE`)
  - Số tài khoản (`TDL_PATIENT_ACCOUNT_NUMBER`)
  - Đơn vị (`TDL_PATIENT_WORK_PLACE_NAME` / `TDL_PATIENT_WORK_PLACE`)
  - Địa chỉ (`TDL_PATIENT_ADDRESS`)
  - Mã QH - Mã đơn vị quan hệ ngân sách (`TDL_PATIENT_BUD_REL_UNIT_CODE`, ưu tiên nếu có dữ liệu)
- Hỗ trợ tạo hóa đơn cho đối tượng không phải bệnh nhân (checkbox "Khác")

### 2.2. Quản lý danh sách hàng hóa/dịch vụ
- Thêm hàng hóa/dịch vụ vào danh sách với thông tin: tên, đơn vị tính, số lượng, đơn giá, chiết khấu, VAT
- Hiển thị danh sách dạng lưới (grid) với các cột: STT, Tên dịch vụ, Đơn vị tính, Số lượng, Đơn giá, Chiết khấu, VAT (%), Thành tiền
- Tự động tính thành tiền = (Số lượng × Đơn giá) × (1 + VAT/100) - Chiết khấu
- Xóa từng dòng hàng hóa

### 2.3. Tính toán tài chính
- Tự động tính tổng tiền từ danh sách hàng hóa
- Miễn giảm theo số tiền hoặc tỷ lệ phần trăm
- Hiển thị số tiền chiết khấu (CK), số tiền QT, và cần thu

### 2.4. Quản lý sổ thu chi và hình thức thanh toán
- Chọn sổ thu chi (`AccountBook`) từ danh sách
- Chọn hình thức thanh toán (`PayForm`): tiền mặt, chuyển khoản, quẹt thẻ, kết hợp
- Nhập số hóa đơn (tự động hoặc thủ công)
- Chọn thời gian giao dịch

### 2.5. Lưu giao dịch
- Lưu thông tin giao dịch qua API `HisTransaction/CreateOtherBill`
- Dữ liệu lưu bao gồm:
  - `BUYER_NAME`: Tên người mua
  - `BUYER_ORGANIZATION`: Đơn vị
  - `BUYER_ADDRESS`: Địa chỉ
  - `BUYER_TAX_CODE`: Mã số thuế
  - `BUYER_ACCOUNT_NUMBER`: Số tài khoản
  - `BUYER_SOCIAL_RELATIONS_CODE`: Mã đơn vị quan hệ ngân sách (Mã QH)
- Sau khi tạo giao dịch, cập nhật ngược thông tin lên `HIS_TREATMENT`

### 2.6. In hóa đơn
- In hóa đơn sau khi lưu
- Lưu và in cùng lúc (Ctrl+I)
- Tích hợp hóa đơn điện tử (HDDT) qua các nhà cung cấp: VIETTEL, SAFECERT, CYBERBILL
- Hỗ trợ ký số (Ctrl+D)

### 2.7. Quét mã QR
- Hỗ trợ quét mã QR để nhập thông tin thanh toán nhanh

---

## 3. Giao diện người dùng

### 3.1. Bố cục form chính (`frmTransactionBillOther`)

Form chính sử dụng `DevExpress LayoutControl` với các vùng chức năng:

#### Thanh công cụ (Toolbar)
| Nút | Phím tắt | Mô tả |
|-----|----------|-------|
| Lưu in | `Ctrl+I` | Lưu và in hóa đơn |
| Lưu | `Ctrl+S` | Lưu giao dịch |
| In | `Ctrl+P` | In hóa đơn |
| Mới | `Ctrl+N` | Tạo giao dịch mới |
| Tìm | `Ctrl+F` | Tìm kiếm bệnh nhân |
| Bổ sung | `Ctrl+A` | Thêm hàng hóa vào danh sách |
| F2 | `F2` | Focus vào ô tìm kiếm mã điều trị |
| Lưu ký | `Ctrl+D` | Lưu và ký số |

#### Vùng thông tin người mua (`groupBox1`)
| Trường | Mô tả | Nguồn dữ liệu tự động |
|--------|-------|------------------------|
| Bệnh nhân | Mã điều trị (tìm kiếm F2) | - |
| Tìm kiếm | Nút tìm kiếm bệnh nhân | - |
| Khác (checkbox) | Xuất hóa đơn cho đối tượng không phải bệnh nhân | - |
| Họ tên | Tên người mua | `TDL_PATIENT_NAME` |
| Mã số thuế | Mã số thuế người mua | `TDL_PATIENT_TAX_CODE` |
| Số tài khoản | Số tài khoản ngân hàng | `TDL_PATIENT_ACCOUNT_NUMBER` |
| Đơn vị | Tên đơn vị/tổ chức | `TDL_PATIENT_WORK_PLACE_NAME` |
| Mã QH | Mã đơn vị quan hệ ngân sách (ToolTip: "Mã đơn vị quan hệ ngân sách") | `TDL_PATIENT_BUD_REL_UNIT_CODE` (ưu tiên) |
| Địa chỉ | Địa chỉ người mua | `TDL_PATIENT_ADDRESS` |
| Mô tả | Mô tả giao dịch | - |

#### Vùng nội dung hóa đơn
| Trường | Mô tả |
|--------|-------|
| Tên dịch vụ | Tên hàng hóa/dịch vụ |
| Đơn vị tính | Đơn vị tính |
| Số lượng | Số lượng |
| Đơn giá | Đơn giá |
| VAT | Thuế VAT (%) |
| Chiết khấu | Tỷ lệ chiết khấu (%) |
| Lý do | Lý do chiết khấu |
| Bổ sung (Ctrl+A) | Thêm vào danh sách |

#### Lưới hàng hóa (`gridControlBillGoods`)
| Cột | Mô tả |
|-----|-------|
| STT | Số thứ tự |
| Xóa | Nút xóa dòng hàng hóa |
| Tên dịch vụ | Tên hàng hóa/dịch vụ |
| Đơn vị tính | Đơn vị tính |
| Số lượng | Số lượng |
| Đơn giá | Đơn giá |
| Chiết khấu | Chiết khấu |
| VAT (%) | Thuế VAT |
| Thành tiền | Thành tiền (tự động tính) |

#### Vùng thanh toán
| Trường | Mô tả |
|--------|-------|
| Sổ thu chi | Chọn sổ thu chi |
| Hình thức | Hình thức thanh toán |
| Tg giao dịch | Thời gian giao dịch |
| Miễn giảm | Số tiền miễn giảm và tỷ lệ % |
| Lý do | Lý do miễn giảm |
| Số hóa đơn | Số hóa đơn |
| Số tiền CK | Số tiền chuyển khoản (hiện khi hình thức = kết hợp) |
| Số tiền QT | Số tiền quẹt thẻ (hiện khi hình thức = kết hợp) |
| Cần thu | Số tiền cần thu |
| Số tiền | Tổng số tiền |
| Không hiển thị HĐ ĐT | Checkbox ẩn hóa đơn điện tử |
| QR | Nút quét mã QR |

---

## 4. Luồng xử lý chính

### 4.1. Tạo hóa đơn mới
1. Nhập mã điều trị hoặc tìm kiếm bệnh nhân
2. Hệ thống tự động điền thông tin người mua từ hồ sơ điều trị (bao gồm Mã QH)
3. Nhập danh sách hàng hóa/dịch vụ (tên, đơn vị, số lượng, đơn giá, VAT, chiết khấu)
4. Bấm "Bổ sung (Ctrl+A)" để thêm vào lưới
5. Chọn sổ thu chi và hình thức thanh toán
6. Nhập miễn giảm (nếu có)
7. Bấm "Lưu (Ctrl+S)" hoặc "Lưu in (Ctrl+I)"

### 4.2. Luồng dữ liệu Mã QH
1. Khi tải thông tin bệnh nhân, kiểm tra `TDL_PATIENT_BUD_REL_UNIT_CODE` trong `HIS_TREATMENT`
2. Nếu có dữ liệu → điền vào ô Mã QH
3. Nếu không có → để trống
4. Khi lưu giao dịch → giá trị Mã QH được lưu vào `BUYER_SOCIAL_RELATIONS_CODE` của `HIS_TRANSACTION`

---

## 5. API và tích hợp

### 5.1. API sử dụng
| API | Mô tả |
|-----|-------|
| `api/HisTreatment/Get` | Lấy thông tin hồ sơ điều trị |
| `api/HisTransaction/CreateOtherBill` | Tạo giao dịch hóa đơn khác |
| `api/HisAccountBook/Get` | Lấy danh sách sổ thu chi |

### 5.2. Tích hợp hóa đơn điện tử
- VIETTEL
- SAFECERT  
- CYBERBILL

---

## 6. Cấu trúc dự án

```
HIS.Desktop.Plugins.TransactionBillOther/
├── ADO/
│   └── HisBillGoodADO.cs              # Data object cho hàng hóa
├── Base/
│   ├── ResourceLangManager.cs          # Quản lý ngôn ngữ
│   └── ResourceMessageManager.cs       # Quản lý thông báo
├── TransactionBillOther/
│   ├── TransactionBillOther.cs         # Module definition
│   ├── TransactionBillOtherBehavior.cs # Behavior pattern
│   └── TransactionBillOtherFactory.cs  # Factory pattern
├── Resources/
│   ├── Lang.vi.resx                    # Nhãn tiếng Việt
│   ├── Lang.en.resx                    # Nhãn tiếng Anh
│   ├── Message.vi.resx                 # Thông báo tiếng Việt
│   └── Message.en.resx                 # Thông báo tiếng Anh
├── Validation/                         # Validation rules
│   ├── AccountBookValidationRule.cs
│   ├── BuyerAccountCodeValidationRule.cs
│   ├── BuyerAddressValidationRule.cs
│   ├── BuyerOrganizationValidationRule.cs
│   ├── BuyerTaxCodeValidationRule.cs
│   └── ...
├── frmTransactionBillOther.cs          # Form chính (code-behind)
├── frmTransactionBillOther.Designer.cs # Form designer
├── ModuleLinkString.cs                 # Gọi module khác (HisNoneMediService)
└── TransactionBillOtherProcessor.cs    # Plugin processor
```

---

## 7. Mapping dữ liệu

### 7.1. Treatment → Form (tự động điền)
| HIS_TREATMENT | Control | Mô tả |
|---------------|---------|-------|
| `TDL_PATIENT_NAME` | `txtPatientName` | Họ tên |
| `TDL_PATIENT_TAX_CODE` | `txtBuyerTaxCode` | Mã số thuế |
| `TDL_PATIENT_ACCOUNT_NUMBER` | `txtBuyerAccountNumber` | Số tài khoản |
| `TDL_PATIENT_WORK_PLACE_NAME` / `TDL_PATIENT_WORK_PLACE` | `txtBuyerOrganization` | Đơn vị |
| `TDL_PATIENT_ADDRESS` | `txtBuyerAddress` | Địa chỉ |
| `TDL_PATIENT_BUD_REL_UNIT_CODE` | `txtMaQH` | Mã QH (ưu tiên) |

### 7.2. Form → HIS_TRANSACTION (lưu)
| Control | HIS_TRANSACTION | Mô tả |
|---------|-----------------|-------|
| `txtPatientName` | `BUYER_NAME` | Tên người mua |
| `txtBuyerOrganization` | `BUYER_ORGANIZATION` | Đơn vị |
| `txtBuyerAddress` | `BUYER_ADDRESS` | Địa chỉ |
| `txtBuyerTaxCode` | `BUYER_TAX_CODE` | Mã số thuế |
| `txtBuyerAccountNumber` | `BUYER_ACCOUNT_NUMBER` | Số tài khoản |
| `txtMaQH` | `BUYER_SOCIAL_RELATIONS_CODE` | Mã đơn vị quan hệ ngân sách |
