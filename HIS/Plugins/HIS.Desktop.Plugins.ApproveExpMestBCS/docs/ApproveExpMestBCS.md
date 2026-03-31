# HIS.Desktop.Plugins.ApproveExpMestBCS

## Tổng quan

Plugin **ApproveExpMestBCS** thực hiện quy trình **duyệt phiếu xuất vật tư y tế** (thuốc và vật tư) theo luồng **BCS (Binary Combination Substitution)**. Người dùng mục tiêu là nhân viên kho/dược của bệnh viện, chịu trách nhiệm phê duyệt và phân bổ vật tư từ kho xuất theo yêu cầu.

Tính năng chính:
- Duyệt số lượng thuốc và vật tư xuất kho theo phiếu yêu cầu
- Kiểm tra tồn kho thực tế, so sánh với số lượng yêu cầu
- Hỗ trợ thay thế thuốc/vật tư khi mặt hàng gốc không đủ tồn kho
- Tự động phân bổ thay thế nếu cấu hình cho phép
- Kiểm soát xuất lẻ (không chia phần lẻ) theo cấu hình

---

## Cấu trúc thư mục

```
HIS.Desktop.Plugins.ApproveExpMestBCS/
├── ApproveExpMestBCSProcessor.cs       # Entry point, khai báo extension
├── frmApproveExpMestBCS.cs             # Form chính xử lý duyệt phiếu
├── frmApproveExpMestBCS.Designer.cs    # UI definition (auto-generated)
├── frmReplace.cs                       # Form chọn thuốc/vật tư thay thế
├── frmReplace.Designer.cs              # UI definition (auto-generated)
├── ApproveExpMestBCS/
│   ├── IApproveExpMestBCS.cs           # Interface
│   ├── ApproveExpMestBCSFactory.cs     # Factory tạo behavior
│   └── ApproveExpMestBCSBehavior.cs    # Behavior, parse tham số đầu vào
├── ADO/
│   ├── MedicineTypeADO.cs              # Model dữ liệu thuốc
│   ├── MaterialTypeADO.cs              # Model dữ liệu vật tư
│   └── MetyMatyTypeADO.cs             # Adapter từ StockSDO sang ADO
├── Config/
│   └── HisConfig.cs                    # Đọc cấu hình hệ thống
├── Validation/
│   ├── SpinAmountValidationRule.cs     # Validate số lượng thay thế
│   └── ComboMediMatyValidationRule.cs  # Validate lựa chọn thay thế
└── Util/
    └── StringUtil.cs                   # Chuẩn hoá chuỗi tiếng Việt
```

---

## Kiến trúc Plugin

Plugin tuân theo mô hình **Processor → Factory → Behavior → Form**:

```
ApproveExpMestBCSProcessor.Run(args)
    └── ApproveExpMestBCSFactory.MakeIApproveExpMestBCS()
            └── ApproveExpMestBCSBehavior.Run()
                    └── frmApproveExpMestBCS (hiển thị modal)
```

### Entry Point

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
   "HIS.Desktop.Plugins.ApproveExpMestBCS",
   "Duyệt phiếu xuất",
   "Common",
   23,
   "bidList.png",
   "A",
   Module.MODULE_TYPE_ID__FORM,
   true, true)]
public class ApproveExpMestBCSProcessor : ModuleBase, IDesktopRoot
```

### Tham số đầu vào (`args`)

| Index | Kiểu       | Mô tả                             |
|-------|------------|-----------------------------------|
| 0     | `object`   | Module context hiện tại           |
| 1     | `long`     | ID phiếu xuất (`expMestId`)       |
| 2     | `Delegate` | Callback sau khi duyệt thành công |

---

## Form Chính: frmApproveExpMestBCS

### Luồng xử lý khi mở form

```
Form_Load
  ├── LoadConfig()              - Đọc cấu hình hệ thống
  ├── LoadExpMest()             - Lấy thông tin phiếu xuất và kho
  ├── VisibleColumnBCS()        - Ẩn/hiện cột thay thế theo config
  ├── LoadDataInStock()         - Lấy danh sách thuốc/vật tư tồn kho
  ├── LoadDataMedicine()        - Xử lý yêu cầu thuốc + tính số lượng
  ├── LoadDataMaterial()        - Xử lý yêu cầu vật tư + tính số lượng
  └── LoadDataAutoReplace()     - Tự động gán thay thế (nếu config bật)
```

### API Backend sử dụng

| Mục đích                         | Endpoint                                        |
|----------------------------------|-------------------------------------------------|
| Lấy thông tin phiếu xuất         | `GET api/HisExpMest/Get`                        |
| Lấy thuốc tồn kho                | `HisRequestUriStore.HIS_MEDICINE_TYPE_IN_STOCK` |
| Lấy vật tư tồn kho               | `HisRequestUriStore.HIS_MATERIAL_TYPE_GET_IN_STOCK` |
| Lấy yêu cầu thuốc                | `GET api/HisExpMestMetyReq/Get`                 |
| Lấy yêu cầu vật tư               | `GET api/HisExpMestMatyReq/Get`                 |
| Lấy thuốc thay thế đã gán        | `GET api/HisExpMestMedicine/Get`                |
| Lấy vật tư thay thế đã gán       | `GET api/HisExpMestMaterial/Get`                |
| Lấy bảng ánh xạ thay thế         | `GET api/HisMediStock/GetReplaceSDO`            |
| **Gửi phê duyệt**                | `POST api/HisExpMest/Approve`                   |

### Quy tắc tính số lượng duyệt (`YCD_AMOUNT`)

```
YCD_AMOUNT = MIN(
    (AMOUNT - DD_AMOUNT),       // Yêu cầu còn lại chưa cấp
    AVAIL_AMOUNT                // Tồn kho khả dụng
)
```

Nếu `IS_ALLOW_EXPORT_ODD != "1"`: chỉ cho phép số lượng nguyên (không xuất lẻ).

### Màu sắc hiển thị lưới

| Màu    | Ý nghĩa                                          |
|--------|--------------------------------------------------|
| Đỏ     | Tồn kho không đủ (yêu cầu > khả dụng)           |
| Xanh   | Hàng thay thế (substitute item)                  |
| Mặc định | Hàng bình thường, đủ tồn kho                   |

### Trạng thái ô nhập liệu

| Điều kiện             | Ô nhập số lượng | Nút thay thế |
|-----------------------|-----------------|--------------|
| Chưa check            | Disabled        | Disabled     |
| Đã check              | Enabled         | Enabled      |
| Đã duyệt (IsApproved) | Disabled (readonly) | Disabled |
| Là hàng thay thế      | Enabled         | Disabled     |

---

## Form Thay Thế: frmReplace

Cho phép người dùng chọn thuốc/vật tư thay thế từ kho khi mặt hàng gốc không đủ.

### Validation

| Rule                          | Mô tả                                                              |
|-------------------------------|--------------------------------------------------------------------|
| `SpinAmountValidationRule`    | Số lượng thay thế > 0 và ≤ (yêu cầu - đã duyệt)                  |
| `ComboMediMatyValidationRule` | Phải chọn thuốc/vật tư thay thế từ danh sách                      |

### Tùy chọn lọc thay thế

- **Lọc theo hoạt chất** (`chkMappingMediMaty`): khi bật, chỉ hiển thị các mặt hàng có cùng hoạt chất BHYT với mặt hàng gốc.

---

## Mô hình dữ liệu (ADO)

### MedicineTypeADO

| Thuộc tính                  | Mô tả                                      |
|-----------------------------|--------------------------------------------|
| `MEDICINE_TYPE_ID/CODE/NAME`| Định danh loại thuốc                       |
| `ACTIVE_INGR_BHYT_CODE/NAME`| Hoạt chất BHYT                            |
| `SERVICE_UNIT_NAME`         | Đơn vị tính                               |
| `AMOUNT`                    | Số lượng yêu cầu tổng                     |
| `DD_AMOUNT`                 | Đã phân phối                              |
| `YCD_AMOUNT`                | Số lượng duyệt cấp (đang nhập)            |
| `AVAIL_AMOUNT`              | Tồn kho khả dụng                          |
| `TON_KHO`                   | Tổng tồn kho                              |
| `REPLACE_MEDICINE_TYPE_ID/NAME` | Thông tin thuốc thay thế              |
| `IS_ALLOW_EXPORT_ODD`       | Cho phép xuất lẻ                          |
| `IsReplace`                 | Flag: đây là hàng thay thế               |
| `IsApproved`                | Flag: đã được duyệt trước đó             |
| `IsCheck`                   | Flag: người dùng đã check để duyệt       |
| `Requests`                  | Danh sách yêu cầu gốc (`HIS_EXP_MEST_METY_REQ`) |

`MaterialTypeADO` có cấu trúc tương tự, dùng tiền tố `MATERIAL_TYPE_*`.

---

## Cấu hình hệ thống (HisConfig)

| Config Key                                               | Giá trị  | Tác dụng                                        |
|----------------------------------------------------------|----------|-------------------------------------------------|
| `MOS.HIS_EXP_MEST.BCS.APPROVE_OTHER_TYPE.IS_ALLOW`      | `"1"`    | Hiển thị cột thay thế, cho phép chọn thay thế  |
| `HIS.HIS_EXP_MEST.BCS.APPROVE.IS_AUTO_REPLACE`          | `"1"`    | Tự động gán thay thế khi load form             |
| `MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM`             | `"1"`    | Loại trừ hàng hết hạn khỏi tồn kho hiển thị   |

---

## Quy trình nghiệp vụ

```
1. Mở phiếu xuất
      ↓
2. Hiển thị danh sách thuốc & vật tư yêu cầu
   - Màu đỏ: không đủ hàng
   - Màu xanh: hàng thay thế đã gán
      ↓
3. Người dùng check item cần duyệt
      ↓
4. Nhập số lượng duyệt (YCD_AMOUNT)
   - Validate: > 0, ≤ tồn kho, ≤ còn lại yêu cầu
   - Nếu IS_ALLOW_EXPORT_ODD ≠ 1: chỉ cho số nguyên
      ↓
5. (Tùy chọn) Chọn thay thế qua nút [Replace]
   - Mở frmReplace, chọn mặt hàng, nhập số lượng
      ↓
6. Nhập ghi chú (txtDescription)
      ↓
7. Nhấn Lưu (Ctrl+S hoặc nút Save)
   - Build HisExpMestApproveSDO
   - POST api/HisExpMest/Approve
      ↓
8. Thành công: reload lưới, hiển thị thông báo, gọi callback delegate
```

### Logic phân bổ số lượng (`MakeMedicine` / `MakeMaterial`)

Khi một loại thuốc/vật tư có nhiều bản ghi yêu cầu từ nhiều điều trị khác nhau, hệ thống phân bổ số lượng duyệt theo thứ tự ưu tiên:

1. Sắp xếp các yêu cầu theo số lượng tăng dần
2. Lần lượt cấp đủ cho từng yêu cầu (hoặc cấp hết phần còn lại nếu không đủ)
3. Tạo bản ghi `ExpMedicineTypeSDO` / `ExpMaterialTypeSDO` cho từng yêu cầu được cấp

---

## Phụ thuộc chính

| Assembly / Thư viện                  | Mục đích                              |
|--------------------------------------|---------------------------------------|
| DevExpress v15.2                     | UI: Grid, Tab, Layout, Editors        |
| AutoMapper                           | Mapping object-to-object              |
| Inventec.Common.Adapter              | HTTP adapter gọi backend              |
| MOS.EFMODEL                          | Entity models (EF)                    |
| MOS.Filter / MOS.SDO                 | Filter và data transfer objects       |
| HIS.Desktop.LocalStorage.HisConfig   | Đọc cấu hình từ backend               |
| Inventec.Core (CommonParam)          | Context, logging, exception handling  |

Framework: **.NET Framework 4.5**

---

## Lưu ý kỹ thuật

- **Ngày hết hạn** được so sánh theo chuỗi định dạng `yyyyMMdd000000` (ví dụ: `"20260326000000"`).
- **StringUtil** chuẩn hoá chuỗi tiếng Việt (bỏ dấu) phục vụ tìm kiếm trong lưới.
- **MetyMatyTypeADO** dùng reflection để copy property từ `HisMaterialTypeInStockSDO` sang cấu trúc medicine — cần chú ý nếu có thay đổi model.
- Form sử dụng `repositoryItem` kép (Enable/Disable) thay vì toggle `Enabled` trực tiếp để kiểm soát trạng thái từng ô lưới theo hàng.
