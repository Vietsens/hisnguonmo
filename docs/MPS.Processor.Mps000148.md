# MPS000148 — Phiếu Biên lai thanh toán — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000148 |
| Project | `MPS.Processor.Mps000148` + `MPS.Processor.Mps000148.PDO` |
| Loại | MPS Processor (template Excel FlexCel) |
| Template | `MPS000148_PhieuBienLaiThanhToan_001.xlsx` |
| Mục đích | In biên lai thanh toán cho 1 giao dịch, tách phần Bệnh nhân tự túc và Chênh lệch BHYT |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
`Mps000148PDO(V_HIS_TRANSACTION, ssBills, sereServs, patientTypeIdBhyt)` (+ `_Treatment` optional) → `MpsPrinter.Run` → `ProcessData()`:

```
ReadTemplate → ProcessSingleKey() → ProcessListData() → SetBarcodeKey() → SetQrCodeKey()
  → ProcessPrintLogData() → SetNumOrderKey(GetNumOrderPrint(...))
  → singleTag → objectTag ("Services1"/"Services2") → barCodeTag
```

### Điều kiện nghiệp vụ — gộp dòng dịch vụ (`ProcessListData`)
Duyệt `HIS_SERE_SERV_BILL`, gộp thành tối đa **2 dòng**:

| Dòng | Điều kiện |
|------|-----------|
| "Bệnh nhân tự túc" | Dịch vụ không thuộc đối tượng BHYT, hoặc phần chênh vượt giá BHYT (`ssBill.PRICE - bhyt_price`), hoặc `PATIENT_PAY_PRICE > 0` |
| "Chênh lệch BHYT" | Dịch vụ thuộc đối tượng BHYT — dịch vụ loại KH lấy `ssBill.PRICE`, loại khác lấy `VIR_TOTAL_PATIENT_PRICE_BHYT` |

- Có `TDL_AMOUNT` thì tính theo bill; không có thì tra ngược `HIS_SERE_SERV` theo `SERE_SERV_ID`.
- Mọi ảnh (mã vạch, QR) chỉ sinh khi trường nguồn có giá trị.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TRANSACTION | View | Nguồn key chính (số tiền, mã điều trị, mã BN, thông tin HĐĐT) |
| HIS_SERE_SERV_BILL | Table | Chi tiết dịch vụ trên bill |
| HIS_SERE_SERV | Table | Tra ngược giá BHYT khi bill không có `TDL_AMOUNT` |
| HIS_TREATMENT | Table | Optional (`_Treatment`) — lấy key `IN_TIME` / `CLINICAL_IN_TIME` |

**Cơ chế sinh key**: `AddObjectKeyIntoListkey<V_HIS_TRANSACTION>` — reflection toàn bộ property thành key (`ProcessorBase.cs:488-545`).

## 4. Key Ảnh (khai trong `Mps000148ExtendSingleKey.cs`)

| Key | Loại | Nguồn | Ghi chú |
|-----|------|-------|---------|
| `TRANSACTION_CODE_BAR` | Mã vạch | `TRANSACTION_CODE` | Có sẵn |
| `TREATMENT_CODE_BAR` | Mã vạch | `TREATMENT_CODE` | Có sẵn |
| `PATIENT_CODE_BAR` | Mã vạch | `TDL_PATIENT_CODE` | Có sẵn |
| `INVOICE_LOOKUP_CODE_QR` | QR | `INVOICE_LOOKUP_CODE` (Số bảo mật) | Thêm 17/08/2026 (việc 54622) |
| `EINVOICE_URL_QR` | QR | `EINVOICE_URL` | Thêm 17/08/2026 (việc 54622) |

**Key chữ — số tiền theo loại dịch vụ (việc 54487):**

| Key | Nội dung | Nguồn |
|-----|----------|-------|
| `SERVICE_TYPE_AMOUNTs` | `Tên loại(số tiền); ...` — chỉ loại có tiền > 0 | 1) `TRANSACTION_INFO`; 2) fallback: gộp `_ListSereServBill` theo loại |
| `SERVICE_TYPE_AMOUNT_ALLs` | Như trên nhưng gồm cả loại tiền = 0 | như trên |

`SetServiceTypeAmountKey()`: ưu tiên parse `TRANSACTION_INFO` (JSON `MOS.SDO.TransactionInfoSDO`, cộng `AMOUNT_<MÃ>_BH + _VP + _DV` cho 16 mã loại); nếu rỗng thì **fallback riêng biểu này** — gộp `_ListSereServBill` theo `SERE_SERV_ID` → tra `_ListSereServ` lấy `TDL_SERVICE_TYPE_ID` → cộng `PRICE` theo loại. Tên loại từ `BackendDataWorker.Get<HIS_SERVICE_TYPE>()`.

Ngoài ra các key số tiền mở rộng: `AMOUNT_TEXT_UPPER_FIRST`, `AMOUNT_AFTER_EXEMPTION(_TEXT)(_UPPER_FIRST)`, `AMOUNT_AWAY_ZERO_TEXT_UPPER_FIRST`, `CT_AMOUNT(_TEXT_UPPER_FIRST)`.

## 5. API Endpoints

Không gọi API — dữ liệu do plugin gọi in truyền sẵn trong PDO.

## 6. Dependencies

| Thành phần | Vai trò |
|-----------|---------|
| `MPS.ProcessorBase` | `AbstractProcessor`, `SetQrCodeByKeyBase`, `AddObjectKeyIntoListkey` |
| `Inventec.Common.BarcodeLib` / `Inventec.Common.QRCoder` | Sinh mã vạch / QR |
| `Inventec.Common.FlexCellExport` | Đổ key + ảnh vào template |
| `IMSys.DbConfig.HIS_RS` | `HIS_SERVICE_TYPE.ID__KH` khi gộp dòng BHYT |
| `MOS.SDO` | `TransactionInfoSDO` — parse `TRANSACTION_INFO` (thêm 18/08/2026, việc 54487) |
| `HIS.Desktop.LocalStorage.BackendData` | `BackendDataWorker.Get<HIS_SERVICE_TYPE>()` — tên loại dịch vụ (thêm 18/08/2026, việc 54487) |

## 7. Print

Là chính biểu in. Không gọi biểu khác.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 17/08/2026 | nampp | Việc 54622 (BV Nguyễn Tri Phương): Thêm `SetQrCodeKey()` sinh 2 QR tra cứu hóa đơn điện tử — `INVOICE_LOOKUP_CODE_QR` (Số bảo mật) và `EINVOICE_URL_QR` (đường dẫn tra cứu), dùng `SetQrCodeByKeyBase` của `AbstractProcessor`; gọi sau `SetBarcodeKey()` trong `ProcessData()`. Mã vạch mã điều trị và key `INVOICE_LOOKUP_CODE` đã có sẵn từ trước, giữ nguyên. Chỉ THÊM key mới ⇒ template chưa cập nhật thì phiếu in không đổi. |
| 18/08/2026 | nampp | Việc 54487 (BV Nguyễn Tri Phương): Thêm `SetServiceTypeAmountKey()` sinh 2 key số tiền đã thu tách theo loại dịch vụ — `SERVICE_TYPE_AMOUNTs` (chỉ loại > 0) và `SERVICE_TYPE_AMOUNT_ALLs` (gồm cả loại = 0), định dạng `Tên loại(số tiền); ...`. Nguồn chính: parse `HIS_TRANSACTION.TRANSACTION_INFO` (JSON `MOS.SDO.TransactionInfoSDO`); fallback riêng biểu này khi rỗng: gộp `_ListSereServBill` + `_ListSereServ` theo `TDL_SERVICE_TYPE_ID`. Tên loại từ `BackendDataWorker.Get<HIS_SERVICE_TYPE>()`. Thêm 2 Reference `MOS.SDO`, `HIS.Desktop.LocalStorage.BackendData`. Không cần API, không sửa plugin. |

## 9. Test Cases

- [ ] Giao dịch đã xuất HĐĐT → "Danh sách key" thấy `INVOICE_LOOKUP_CODE_QR`, `EINVOICE_URL_QR`
- [ ] Template đặt tag QR → in ra QR ở cuối phiếu; mã vạch + số bảo mật giữ nguyên như trước
- [ ] Quét QR ra đúng số bảo mật / mở đúng trang tra cứu
- [ ] Giao dịch chưa xuất HĐĐT → không sinh QR, không báo lỗi
- [ ] **Hồi quy**: 2 dòng "Bệnh nhân tự túc" / "Chênh lệch BHYT" tính đúng như trước; số tiền, bảng chữ, số lần in không đổi
- [ ] **Hồi quy**: in bằng template CŨ → phiếu ra y hệt trước khi sửa

### Số tiền theo loại dịch vụ (việc 54487)
- [ ] Giao dịch thu nhiều loại DV → "Danh sách key" thấy `SERVICE_TYPE_AMOUNTs`, `SERVICE_TYPE_AMOUNT_ALLs` dạng `Tên(số tiền); ...`
- [ ] Cộng tay các số trong chuỗi = ô "Số tiền" trên phiếu
- [ ] Chuỗi **giống hệt** MPS000147 trên cùng giao dịch
- [ ] Loại có tiền = 0 → `SERVICE_TYPE_AMOUNTs` bỏ qua, `SERVICE_TYPE_AMOUNT_ALLs` vẫn in
- [ ] Giao dịch cũ `TRANSACTION_INFO` rỗng → **fallback** tính từ danh sách dịch vụ của bill, vẫn ra chuỗi đúng
- [ ] Đổi tên loại DV trong danh mục → phiếu in ra tên mới, không sửa code
