# Danh Sách Giao Dịch — Tài Liệu Module

> Tài liệu này tập trung vào tính năng **Đính kèm bảng kê HĐĐT (VNPT)** bổ sung cho plugin. Các chức năng cũ (in phiếu, hủy/khôi phục giao dịch, xuất HĐĐT, sửa lý do...) chưa được tài liệu hóa đầy đủ ở đây.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionList |
| Loại | Form (frmTransactionList : FormBase) |
| Mục đích | Danh sách giao dịch thu/chi — in phiếu, xuất/quản lý HĐĐT, và **đính kèm bảng kê thanh toán lên cổng HĐĐT VNPT** |
| Ngày cập nhật | 09/06/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ — Đính Kèm Bảng Kê HĐĐT (VNPT)

Toàn bộ tính năng được bật/tắt qua 1 config:
`MOS.HIS_TRANSACTION.AUTO_ATTACH_BORDEREAU_HDDT__VNPT`
- **Có giá trị** = mã `PrintTypeCode` bảng kê → bật tính năng (render qua mã này).
- **Rỗng/null** → ẩn toàn bộ (bộ lọc, menu, cột) — viện không bật thì giao diện như cũ.

**Helper dùng chung `GuiDinhKemBangKe(transaction, ref param)`** (file `frmTransactionList__Plus__AttachBordereau.cs`):
```
1. Kiểm tra: config có giá trị + EINVOICE_TYPE_ID == ID__VNPT + có INVOICE_CODE
2. GỌI Library.PrintBordereau.RenderHddtBordereauToPdf (do mục 3.3 cung cấp) → PDF base64; truyền `BordereauInitData.HddtInfo` (kiểu `HddtInfoADO`: InvoiceNumOrder + InvoiceTime) để template in "Kèm theo số hóa đơn: {N}" + "Ngày DD tháng MM năm YYYY"
3. Đính kèm vào HĐĐT (SOAP)      (Library.ElectronicBill.Run(ATTACH_BORDEREAU))
4. Thành công → API UpdateBordereauAttachInfo { IDs=[transaction.ID], BordereauAttachStatus=1 }
   Lỗi → thông báo "Gửi thông tin bảng kê thất bại", giữ BORDEREAU_ATTACH_STATUS = null
```

2 luồng gọi helper:
| Luồng | Kích hoạt | Key HĐĐT |
|-------|-----------|----------|
| Xuất lại HĐĐT | Tự động sau `XuatHoaDonDienTu` thành công (sau UpdateInvoiceInfo) | `electronicBillResult.*` (đã lưu vào transaction) |
| Gửi đính kèm bảng kê | Menu chuột phải (HĐĐT đã có) | `transaction.EINVOICE_NUM_ORDER / EINVOICE_TIME` |

## 3. EFMODEL Sử Dụng

| Entity | Mục đích |
|--------|----------|
| V_HIS_TRANSACTION | Giao dịch — dùng `INVOICE_SYS`, `INVOICE_CODE`, `EINVOICE_NUM_ORDER`, `EINVOICE_TIME`, `EINVOICE_TYPE_ID`, **`BORDEREAU_ATTACH_STATUS`** |
| V_HIS_TREATMENT_FEE | Thông tin điều trị/phí (đầu vào ElectronicBill) |
| HIS_BRANCH | Chi nhánh (đầu vào ElectronicBill) |

## 4. UI Layout (chỉ hiện khi config bật)

| Phần tử | Vị trí | Mô tả |
|---------|--------|-------|
| Radio "Đính kèm bảng kê" (Tất cả / Đã đính kèm BK / Chưa đính kèm BK) | NavBar nhóm "Trạng Thái" (`layoutControl8`), **hàng thứ 3** | Khai báo trong **Designer.cs** (`cbBordereauAll/Done/None` + `lciBordereau*`) → hiện trong designer cho maintainer thấy/sửa. Runtime: config rỗng → ẩn (`Visibility.Never` + `GroupClientHeight` về 70); config có → localize caption + nạp ControlState. Lọc `BordereauAttachStatus` 1/0/null |
| Cột "Đính kèm BK" | Grid giao dịch | Tạo runtime, hiển thị "Đã đính kèm BK"/trống theo `BORDEREAU_ATTACH_STATUS` |
| Menu "Gửi đính kèm bảng kê" | Chuột phải grid | Chỉ hiện khi VNPT + `BORDEREAU_ATTACH_STATUS` null + có `INVOICE_CODE` |

ControlState: nhớ lựa chọn radio giữa các phiên (KEY `cbBordereauAttachStatusFilter`).

## 5. API Endpoints

| Action | URI | Consumer | Input |
|--------|-----|----------|-------|
| Lưu trạng thái đính kèm | `RequestUri.HIS_TRANSACTION_UPDATE_BORDEREAU_ATTACH_INFO` = `api/HisTransaction/UpdateBordereauAttachInfo` (**API mới**) | MosConsumer | `MOS.SDO.HisTransactionBordereauAttachInfoSDO { Ids: List<long>, BordereauAttachStatus: short }` |
| Lấy danh sách (lọc đính kèm) | `api/HisTransaction/GetView` | MosConsumer | `HisTransactionViewFilter.BordereauAttachStatus` |

> ✅ Dùng SDO chính thức `MOS.SDO.HisTransactionBordereauAttachInfoSDO` (MOS.SDO.dll 10/06/2026): `Ids` (List&lt;long&gt;) + `BordereauAttachStatus` (Int16). DTO tạm đã gỡ bỏ.

## 6. Dependencies (Library)

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintBordereau | Render bảng kê → PDF base64 — GỌI `RenderHddtBordereauToPdf(printTypeCode)` + truyền `BordereauInitData.HddtInfo` (`HddtInfoADO`). **Do mục 3.3 (người khác) bổ sung** — plugin này chỉ gọi. |
| HIS.Desktop.Plugins.Library.ElectronicBill | Đính kèm file (SOAP) vào HĐĐT (`Run(ElectronicBillType.ENUM.ATTACH_BORDEREAU)`) — do backend bổ sung |

> ⚠️ **Phạm vi & build order**: Việc này chỉ gồm **mục 3.2 (plugin TransactionList)**. Mục **3.3 (PrintBordereau: HddtInfoADO, BordereauInitData.HddtInfo, Mps000321Behavior forward, RenderHddtBordereauToPdf)** và **3.4 (MPS000321: Mps000321PDO.HddtInfo + template Excel mới)** là **việc của người khác**. TransactionList chỉ COMPILE/CHẠY được sau khi 3.3 + 3.4 hoàn thành và DLL `PrintBordereau`/MPS trong `lib` được cập nhật.

## 7. Print

| Loại | PrintTypeCode | Library |
|------|--------------|---------|
| Bảng kê đính kèm HĐĐT | = giá trị config `AUTO_ATTACH_BORDEREAU_HDDT__VNPT` | PrintBordereau (render SaveFile → PDF) |

Template MPS hiển thị "Kèm theo số hóa đơn: {N}" + "Ngày DD tháng MM năm YYYY" do MPS-side xử lý (không thuộc plugin này).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 11/06/2026 | tuanln | **Chốt phạm vi: việc này CHỈ là mục 3.2 (TransactionList)** — mục 3.3 (PrintBordereau) + 3.4 (MPS000321) do người khác làm; KHÔNG sửa 2 thư viện đó (đã revert các thay đổi PrintBordereau lỡ làm trước). TransactionList **GỌI** `Library.PrintBordereau.RenderHddtBordereauToPdf(printTypeCode)` + truyền `BordereauInitData.HddtInfo` (kiểu **`HddtInfoADO`**, lấy `transaction.EINVOICE_NUM_ORDER/EINVOICE_TIME`) → nhận PDF base64 → đính kèm qua `ElectronicBill.Run(ATTACH_BORDEREAU)` (`AttachFileName`="Bang ke thanh toan.pdf"). Check VNPT `EINVOICE_TYPE_ID == ID__VNPT` (menu + helper). Bộ lọc "Đính kèm bảng kê" (3 radio `cbBordereauAll/Done/None` + 3 `lciBordereau*`) đưa vào `Designer.cs` thành hàng 3 nhóm "Trạng Thái" (mirror hàng "Khoá"); runtime: config rỗng → ẩn (`LayoutVisibility.Never`) + `GroupClientHeight` về 70, config có → localize + ControlState. Cột grid tạo runtime. **Sau review (không CRITICAL/HIGH) — sửa 4 MEDIUM:** (1) `WaitingManager` bao TOÀN BỘ luồng ở caller (`GuiDinhKemBangKe` không tự Show/Hide nữa → không tắt form chờ giữa chừng ở luồng Xuất lại HĐĐT; menu chuột phải Show/finally Hide); (2) ẩn menu "Gửi đính kèm" khi giao dịch không có `TREATMENT_ID`; (3) cột "Đính kèm BK" đặt VisibleIndex TRƯỚC 4 cột audit; (4) refresh grid sau khi đính kèm thành công từ menu. |
| 10/06/2026 | tuanln | Backend đã đẩy SDO chính thức → gỡ DTO tạm `ADO/HisTransactionBordereauAttachInfoSDO.cs`, chuyển helper sang `MOS.SDO.HisTransactionBordereauAttachInfoSDO` (`Ids`: List&lt;long&gt;, `BordereauAttachStatus`: Int16). Bỏ entry csproj của DTO tạm. Filter `BordereauAttachStatus` (int?) giữ nguyên. |
| 09/06/2026 | tuanln | Thêm tính năng đính kèm bảng kê HĐĐT VNPT: config gate `AUTO_ATTACH_BORDEREAU_HDDT__VNPT`; bộ lọc + cột "Đính kèm BK" (runtime); menu "Gửi đính kèm bảng kê"; chèn vào luồng xuất lại HĐĐT; API mới `UpdateBordereauAttachInfo` (DTO tạm); Resources vi/en/my |

## 9. Test Cases

- [ ] Config rỗng → không thấy bộ lọc/menu/cột (giao diện như cũ).
- [ ] Config có mã → thấy bộ lọc "Đính kèm bảng kê", cột trạng thái.
- [ ] Lọc "Đã đính kèm BK" → gửi `BordereauAttachStatus=1`; "Chưa đính kèm BK" → `=0`; "Tất cả" → không gửi.
- [ ] Menu "Gửi đính kèm bảng kê" chỉ hiện với giao dịch VNPT + có INVOICE_CODE + chưa đính kèm.
- [ ] Xuất lại HĐĐT VNPT → tự render + đính kèm + cập nhật trạng thái = 1.
- [ ] Đính kèm lỗi → thông báo "Gửi thông tin bảng kê thất bại", trạng thái vẫn null (HĐĐT không bị ảnh hưởng).
