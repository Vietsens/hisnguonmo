# MPS000147 — Phiếu Biên nhận thanh toán — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000147 |
| Project | `MPS.Processor.Mps000147` + `MPS.Processor.Mps000147.PDO` |
| Loại | MPS Processor (template Excel FlexCel) |
| Template | `MPS000147_PhieuHoaDonThanhToan_001.xlsx` |
| Mục đích | In biên nhận thanh toán cho 1 giao dịch thu tiền |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Plugin gọi in (màn Thanh toán / Danh sách giao dịch) dựng `Mps000147PDO(V_HIS_TRANSACTION)` → `MpsPrinter.Run` → `Mps000147Processor.ProcessData()`:

```
ReadTemplate → ProcessSingleKey() → ProcessListData() → SetBarcodeKey() → SetQrCodeKey()
  → ProcessPrintLogData() → SetNumOrderKey(GetNumOrderPrint(...))
  → singleTag.ProcessData (key chữ + ảnh QR byte[])
  → objectTag.AddObjectData("Services1"/"Services2")
  → barCodeTag.ProcessData (ảnh mã vạch)
```

### Điều kiện nghiệp vụ
- Mọi ảnh (mã vạch, QR) **chỉ sinh khi trường nguồn có giá trị** — thiếu dữ liệu thì ô trên biểu để trống, không chặn in.
- Danh sách dịch vụ in gộp thành 1 dòng "Dịch vụ theo yêu cầu" bằng đúng số tiền giao dịch.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TRANSACTION | View | Nguồn dữ liệu duy nhất của biểu (số tiền, mã điều trị, mã BN, thông tin hóa đơn điện tử) |

**Cơ chế sinh key**: `AddObjectKeyIntoListkey<V_HIS_TRANSACTION>(rdo._Transaction, false)` — reflection toàn bộ property non-virtual thành key `<#TÊN_CỘT;>` (`ProcessorBase.cs:488-545`). Thêm cột vào view là tự có key, **không cần khai báo tay**. Các key hóa đơn điện tử (`INVOICE_LOOKUP_CODE`, `EINVOICE_URL`, `INVOICE_CODE`, `EINVOICE_TIME`...) đều đến từ đây.

## 4. Key Ảnh (khai trong `Mps000147ExtendSingleKey.cs`)

| Key | Loại | Nguồn | Ghi chú |
|-----|------|-------|---------|
| `TRANSACTION_CODE_BAR` | Mã vạch | `TRANSACTION_CODE` | Có sẵn từ trước |
| `TREATMENT_CODE_BAR` | Mã vạch | `TREATMENT_CODE` | Thêm 17/08/2026 (việc 54622) |
| `PATIENT_CODE_BAR` | Mã vạch | `TDL_PATIENT_CODE` | Thêm 17/08/2026 (việc 54622) |
| `INVOICE_LOOKUP_CODE_QR` | QR | `INVOICE_LOOKUP_CODE` (Số bảo mật) | Thêm 17/08/2026 (việc 54622) |
| `EINVOICE_URL_QR` | QR | `EINVOICE_URL` | Thêm 17/08/2026 (việc 54622) |

## 4b. Key chữ mở rộng — số tiền theo loại dịch vụ (việc 54487)

| Key | Nội dung | Nguồn |
|-----|----------|-------|
| `SERVICE_TYPE_AMOUNTs` | `Tên loại(số tiền); ...` — **chỉ loại có tiền > 0** | `TRANSACTION_INFO` |
| `SERVICE_TYPE_AMOUNT_ALLs` | Như trên nhưng **gồm cả loại tiền = 0** | `TRANSACTION_INFO` |

**Cơ chế** (`SetServiceTypeAmountKey()`): parse `HIS_TRANSACTION.TRANSACTION_INFO` (JSON của `MOS.SDO.TransactionInfoSDO` — backend ghi sẵn tại `HisTransactionUtil.cs:507`) → mỗi loại lấy `AMOUNT_<MÃ>_BH + _VP + _DV` bằng reflection theo mảng hằng `SERVICE_TYPE_CODES` (16 mã: AN, CL, CN, GB, GI, HA, KH, MA, NS, PH, PT, SA, TH, TT, VT, XN) → tên loại tra từ `BackendDataWorker.Get<HIS_SERVICE_TYPE>()` theo `SERVICE_TYPE_CODE` (Dictionary, O(1)). `TRANSACTION_INFO` rỗng / JSON hỏng → không set key, ô để trống, không lỗi.

**Đặc tả ảnh** (bằng đúng MPS000148 để 2 biểu in ra giống nhau):
- Mã vạch: `Inventec.Common.BarcodeLib.Barcode`, CODE128, 120×40, `IncludeLabel = true`, nhãn dưới, căn giữa — dựng qua `CreateBarcode(string)`, đổ vào `dicImage`.
- QR: `SetQrCodeByKeyBase(keyInDic, keyQrcode)` của `AbstractProcessor` (`AbstractProcessor.cs:576-604`) — `QRCodeGenerator` ECCLevel.Q, `GetGraphic(20)`, đổ vào `singleValueDictionary` dạng `byte[]`.

## 5. API Endpoints

Không gọi API. Toàn bộ dữ liệu do plugin gọi in truyền sẵn trong PDO.

## 6. Dependencies

| Thành phần | Vai trò |
|-----------|---------|
| `MPS.ProcessorBase` | `AbstractProcessor` (base), `SetQrCodeByKeyBase`, `AddObjectKeyIntoListkey`, `CommonKey` |
| `MOS.SDO` | `TransactionInfoSDO` — parse `TRANSACTION_INFO` (thêm 18/08/2026, việc 54487) |
| `HIS.Desktop.LocalStorage.BackendData` | `BackendDataWorker.Get<HIS_SERVICE_TYPE>()` — tên loại dịch vụ (thêm 18/08/2026, việc 54487) |
| `Inventec.Common.BarcodeLib` | Sinh mã vạch CODE128 |
| `Inventec.Common.QRCoder` | Sinh QR |
| `Inventec.Common.FlexCellExport` | `ProcessSingleTag` / `ProcessObjectTag` / `ProcessBarCodeTag` |

## 7. Print

Là chính biểu in. Không gọi biểu khác.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 17/08/2026 | nampp | Việc 54622 (BV Nguyễn Tri Phương): Thêm mã vạch mã điều trị `TREATMENT_CODE_BAR` và mã vạch mã bệnh nhân `PATIENT_CODE_BAR` trong `SetBarcodeKey()` (tách hàm dùng chung `CreateBarcode(string)` copy nguyên từ MPS000148 để 2 biểu in mã vạch giống hệt). Thêm `SetQrCodeKey()` sinh 2 QR tra cứu hóa đơn điện tử: `INVOICE_LOOKUP_CODE_QR` (Số bảo mật) và `EINVOICE_URL_QR` (đường dẫn tra cứu), gọi sau `SetBarcodeKey()` trong `ProcessData()`. Key `INVOICE_LOOKUP_CODE` KHÔNG phải thêm code — đã tự sinh từ `V_HIS_TRANSACTION`, viện thiếu là do DLL cũ, build lại là có. Chỉ THÊM key mới, không sửa/bỏ key cũ ⇒ template chưa cập nhật thì phiếu in không đổi. |
| 18/08/2026 | nampp | Việc 54487 (BV Nguyễn Tri Phương): Thêm `SetServiceTypeAmountKey()` sinh 2 key số tiền đã thu tách theo loại dịch vụ — `SERVICE_TYPE_AMOUNTs` (chỉ loại > 0) và `SERVICE_TYPE_AMOUNT_ALLs` (gồm cả loại = 0), định dạng `Tên loại(số tiền); ...`. Nguồn: parse `HIS_TRANSACTION.TRANSACTION_INFO` (JSON `MOS.SDO.TransactionInfoSDO`, backend ghi sẵn) — **không cần API, không sửa plugin**; tên loại lấy từ `BackendDataWorker.Get<HIS_SERVICE_TYPE>()` theo `SERVICE_TYPE_CODE`. Thêm 2 Reference `MOS.SDO`, `HIS.Desktop.LocalStorage.BackendData` (HintPath theo convention `MPS.ProcessorBase.csproj`). Trước đó dòng "Lý do" trên phiếu in ra JSON thô vì template dùng thẳng `<#TRANSACTION_INFO;>`. |

## 9. Test Cases

- [ ] Giao dịch đã xuất HĐĐT → "Danh sách key" thấy đủ `TREATMENT_CODE_BAR`, `PATIENT_CODE_BAR`, `INVOICE_LOOKUP_CODE` (có giá trị), `INVOICE_LOOKUP_CODE_QR`, `EINVOICE_URL_QR`
- [ ] Template đặt tag mới → in ra: mã vạch mã điều trị ở đầu, số bảo mật ở dòng "Số bảo mật:", QR ở cuối
- [ ] Quét QR `INVOICE_LOOKUP_CODE_QR` ra đúng chuỗi số bảo mật; `EINVOICE_URL_QR` mở đúng trang tra cứu
- [ ] Giao dịch **chưa** xuất HĐĐT → không sinh QR, ô trống, **không báo lỗi**
- [ ] Mã vạch mã điều trị của MPS000147 và MPS000148 trên cùng hồ sơ phải giống hệt
- [ ] **Hồi quy**: in bằng template CŨ → phiếu ra y hệt trước khi sửa (số tiền, bảng chữ, danh sách dịch vụ, số lần in, mã vạch mã giao dịch)

### Số tiền theo loại dịch vụ (việc 54487)
- [ ] Giao dịch thu nhiều loại DV → "Danh sách key" thấy `SERVICE_TYPE_AMOUNTs`, `SERVICE_TYPE_AMOUNT_ALLs` dạng `Tên(số tiền); ...`
- [ ] Cộng tay các số trong chuỗi = ô "Số tiền" trên phiếu
- [ ] Chuỗi **giống hệt** MPS000148 trên cùng giao dịch
- [ ] Loại có tiền = 0 → `SERVICE_TYPE_AMOUNTs` bỏ qua, `SERVICE_TYPE_AMOUNT_ALLs` vẫn in
- [ ] `TRANSACTION_INFO` rỗng → 2 key rỗng, ô trống, **không lỗi**
- [ ] Đổi tên loại DV trong danh mục → phiếu in ra tên mới, không sửa code
