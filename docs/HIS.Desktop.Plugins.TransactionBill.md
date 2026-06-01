# HIS.Desktop.Plugins.TransactionBill — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionBill |
| Loại | Form |
| Mục đích | Thanh toán viện phí (1 sổ). Hiển thị danh sách dịch vụ, tạm ứng, tính tiền BN phải trả, sinh giao dịch thu/hoàn ứng, in hóa đơn, in bảng kê, ký số EMR, kết nối POS, QR code, tích hợp hóa đơn điện tử. |
| Trạng thái | Hoàn thành / Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Thu ngân nhập mã BN/quét thẻ → load thông tin điều trị + cây dịch vụ
2. Tick chọn dịch vụ cần thanh toán (radio Tất cả / Phí khám / Phí CLS+PT / Phí thuốc / Suất ăn)
3. Chọn sổ thu chi, hình thức thanh toán, số tiền BN đưa
4. (Tùy chọn) Chọn sổ tạm ứng để tự động hoàn ứng phần dư; chọn quỹ hỗ trợ
5. Cấu hình các checkbox tự động: Tự động đóng, Hoàn tiền ngân hàng, In phiếu hoàn ứng, In HĐĐT, In đơn PK, In đơn THPK, In bảng kê BH ngoại trú, Kết nối POS
6. Bấm Lưu (Ctrl+S) / Lưu in (Ctrl+I) / Lưu ký (Ctrl+A) / Lưu ký Emr
7. ProcessSave gọi `api/HisTransaction/CreateBill` → backend trả `HisTransactionBillResultSDO` (TransactionBill, TransactionRepay, TransactionDeposit)
8. Sau khi lưu thành công: in các phiếu/hóa đơn theo cấu hình; (nếu tick) tự động mở form Hoàn tiền ngân hàng khi có giao dịch hoàn ứng; tự động đóng form

### Sơ đồ trạng thái giao dịch
```
Tạo giao dịch (IS_ACTIVE=1)
  ├─ TransactionBill (loại TT - Thu)
  ├─ TransactionRepay (loại HU - Hoàn ứng) khi BN còn dư tạm ứng
  └─ TransactionDeposit (loại TU - Tạm ứng) khi BN nộp thêm
```

### Điều kiện nghiệp vụ
- Phải có sổ thu chi cho hình thức thanh toán đã chọn
- Khi `HisConfigCFG.AutoCreateDepositTransaction` bật và có dư → bắt buộc chọn sổ tạm ứng
- Thời gian giao dịch ≥ thời gian ra viện (theo `HIS_TREATMENT_TYPE.TRANS_TIME_OUT_TIME_OPTION`)
- Hình thức QR (`HIS_PAY_FORM.ID__QR`) → bắt buộc dùng nút tạo QR khi phòng có cấu hình `QR_CONFIG_JSON`
- Hình thức Quẹt thẻ (`HIS_PAY_FORM.ID__THE`) + tick "Kết nối POS" → gọi WCF SaleCard / RefundCard / VoidCard
- **Lý do giao dịch** (`cboTransactionReason`, lấy từ `HIS_TRANSACTION_REASON`) là trường ĐỘC LẬP với ô lý do miễn giảm (chiết khấu) — không thay thế nhau. Khi mở form, mặc định theo diện điều trị hiện tại (`V_HIS_TREATMENT_FEE.TDL_TREATMENT_TYPE_ID`): ngoại trú (`HIS_TREATMENT_TYPE.ID__KHAM`) → chọn "Khám"; nội trú/điều trị → "Điều trị". Khi Lưu, gán `data.Transaction.TRANSACTION_REASON_ID`. Lý do miễn giảm tiếp tục chỉ phục vụ nghiệp vụ giảm trừ (`EXEMPTION_REASON`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TRANSACTION / V_HIS_TRANSACTION | Table/View | Giao dịch thu/hoàn ứng/tạm ứng |
| V_HIS_TREATMENT_FEE | View | Thông tin điều trị + tổng phí |
| V_HIS_SERE_SERV_5 | View | Cây dịch vụ + giá BN phải trả |
| V_HIS_SERE_SERV_DEPOSIT | View | Dịch vụ kèm tạm ứng |
| HIS_SERE_SERV_BILL | Table | Chi tiết bill cho từng dịch vụ |
| V_HIS_BILL_FUND / V_HIS_ACCOUNT_BOOK | View | Quỹ thanh toán + sổ thu chi |
| V_HIS_PATIENT_TYPE_ALTER | View | Đối tượng BHYT |
| HIS_TRANSACTION_REASON | Table | Danh mục Lý do giao dịch (độc lập với lý do miễn giảm). `HIS_TRANSACTION.TRANSACTION_REASON_ID` tham chiếu bản ghi này |
| V_HIS_PATIENT_BANK_ACCOUNT | View | Thông tin thụ hưởng (ngân hàng) của BN |
| V_HIS_CASHIER_ROOM | View | Phòng thu ngân hiện tại |
| HIS_BANK / HIS_CARD | Table | Ngân hàng + thẻ thanh toán |
| HIS_CONFIG | Table | Cấu hình (đọc qua `BackendDataWorker.Get<HIS_CONFIG>`) |

## 4. UI Layout

### Sơ đồ giao diện
```
+---------------------------------------------------------------------------+
| [Mã BN] [Tìm Ctrl+F] [☐ Không lấy thuốc/VT]   Chọn nhanh: ⦿ Tất cả ...    |
| Mã BN: ...   Tên BN: ...   Giới: ...   Ngày sinh: ...   Địa chỉ: ...      |
| Đối tượng: ...   Số thẻ BH: ...   Hạn từ/đến: ...   Tuyến/Mức hưởng: ...  |
+---------------------------------------------------------------------------+
| [Tree dịch vụ] (HIS.UC.SereServTree v5)                                   |
|   Tên DV | SL | Đơn giá | Thành tiền | Đồng chi trả | BN trả | Chiết khấu|
|   ☑ Viện Phí ...                                                          |
|     ☑ Khám ... ☑ Khám Nội                                                 |
+---------------------------------------------------------------------------+
| Sổ thu chi: ...  Hình thức: ...  Số tiền: ...   Tg giao dịch: ...         |
| Ngân hàng: ...  Chiết khấu(đ): ...  Chiết khấu(%): ...                    |
| Lý do: ...                                                                 |
| Quỹ hỗ trợ: [grid: Tên quỹ | Số tiền | Hạn mức]                            |
| Ghi chú: ...                                                              |
| Sổ tạm ứng: ...  Số chứng từ: ...  Tg giao dịch: ...                      |
| ☑ Tự động hoàn ứng  Số hoàn ứng: ...  Số chứng từ: ...                    |
| Lý do: [Khám/Điều trị ▼] (cboTransactionReason — caption "Lý do")          |
| ☑ Có kết chuyển  Hiện dư: ...  Cần thu: ...  Số tiền BN đưa: ...          |
+---------------------------------------------------------------------------+
| [☐ Hoàn tiền ngân hàng] (mới)  [☐ Kết nối POS][⚙] [☐ Tự động đóng]         |
| [☐ In phiếu hoàn ứng] [☐ Không hiển thị HĐĐT] [☐ In HĐĐT] [☐ In đơn PK]  |
| [☐ In đơn THPK] [☐ In bảng kê BH ngoại trú]                               |
+---------------------------------------------------------------------------+
| [In▼] [BK ngoại trú 6556][BK tổng hợp 6556][QR][Lưu ký Emr][Lưu ký Ctrl+A]|
| [Lưu in Ctrl+I] [Lưu Ctrl+S] [In▼] [Mới Ctrl+N]                            |
+---------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.SereServTree | panelControlTreeSereServ | Cây dịch vụ thanh toán + tính giá |
| HIS.UC.MenuPrint | panelMenuPrintBill | Menu chọn mẫu in |
| HIS.UC.TotalPriceInfo | (group cuối) | Hiển thị tổng tiền cần thu |

### Các Checkbox cấu hình (lưu trạng thái qua ControlStateWorker)
| Checkbox | Mục đích | Tooltip | Lưu trạng thái |
|----------|----------|---------|----------------|
| `chkAutoClose` | Tự động đóng form sau Lưu thành công | "Tự động đóng chức năng khi 'Lưu in' hoặc 'Lưu ký' hoặc 'In' thành công" | Có (ControlState) |
| `chkRefundByTransfer` (mới) | Tự động mở form Hoàn tiền ngân hàng khi Lưu có giao dịch hoàn ứng | "Tự động mở form Hoàn tiền ngân hàng sau khi thanh toán có phát sinh giao dịch hoàn ứng" | **KHÔNG** — mỗi lần mở form luôn unchecked |
| `chkInHoanUng` | In phiếu hoàn ứng (Mps000113) tự động khi có HU | "Tự động in phiếu hoàn ứng trong trường hợp có phát sinh giao dịch hoàn ứng" |
| `chkPrintHddt` / `chkHideHddt` | In/ẩn hóa đơn điện tử |  |
| `chkPrintPrescription` / `chkPrintTHPK` | In đơn thuốc PK / THPK |  |
| `chkPrintBKBHNT` | In bảng kê BH ngoại trú (Mps000279) |  |
| `chkConnectPOS` | Kết nối POS thẻ ngân hàng |  |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Tạo bill thanh toán | `HIS_TRANSACTION_CREATE_BILL` (UriStores) | MosConsumer |
| Get treatment | `api/HisTreatment/Get` | MosConsumer |
| Get sere serv | `api/HisSereServ/...` | MosConsumer |
| Get bill fund | `api/HisBillFund/...` | MosConsumer |
| Get patient bank account | `api/HisPatientBankAccount/...` | MosConsumer |
| Update invoice info | `api/HisTransaction/UpdateInvoiceInfo` | MosConsumer |
| Get danh mục lý do giao dịch | `api/HisTransactionReason/Get` (filter `HisTransactionReasonFilter`: IS_ACTIVE=1, ORDER theo TRANSACTION_REASON_CODE) | MosConsumer |
| Get attach assign print | `api/HisServiceReq/GetAttachAssignPrint` | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| `HIS.Desktop.Plugins.Library.ElectronicBill` | Phát hành/hủy hóa đơn điện tử |
| `HIS.Desktop.Plugins.Library.EmrGenerate` | Tạo input ký số EMR |
| `HIS.Desktop.Plugins.Library.PrintBordereau` | In phiếu thanh toán |
| `HIS.Desktop.Plugins.Library.PrintPrescription` | In đơn thuốc |
| `HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee` | Kiểm tra bảo lãnh chi phí |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền | Module Link |
|-------------|-----------|-------------|-------------|
| `HIS.Desktop.Plugins.HisPatientBankAccount` | Bấm `btnPatientBankAccount` (icon) — sau lưu hoặc khi nhập thụ hưởng | `HIS_TREATMENT`, `V_HIS_PATIENT_BANK_ACCOUNT`, `Module`, `DelegateSelectData` (RefreshPatientBankAccount) | `HIS.Desktop.Plugins.HisPatientBankAccount` |
| `HIS.Desktop.Plugins.TransactionRepay` | Khi Sửa giao dịch / Hoàn ứng thủ công | `Module`, `TransactionRepayADO` | `HIS.Desktop.Plugins.TransactionRepay` |
| `HIS.Desktop.Plugins.RefundByTransfer` (mới) | Lưu thanh toán thành công + tick `chkRefundByTransfer` + có `TransactionRepay` + có cấu hình + BN có thụ hưởng | `HIS_TREATMENT`, `HIS_TRANSACTION` (mapped từ `V_HIS_TRANSACTION` của TransactionRepay), `string bankCode` (extract từ HIS_CONFIG), `HIS.Desktop.Common.RefeshReference` (callback) | `HIS.Desktop.Plugins.RefundByTransfer` |

## 7. Print

| Loại in | PrintTypeCode | Cách gọi |
|---------|--------------|----------|
| Phiếu thu hoàn ứng | Mps000113 | `onClickPhieuThuHoanUng` (tự động khi tick `chkInHoanUng`) |
| Hóa đơn / phiếu thanh toán | Mps000086, Mps000446... | PrintBordereauProcessor |
| Bảng kê BH ngoại trú 6556 | Mps000279 | `InBangKe_6556_BHYT_Mps000279` |
| Bảng kê tổng hợp 6556 | Mps000281 | (riêng) |
| Đơn thuốc PK | Mps000118 / variants | PrintPrescriptionProcessor |
| Đơn thuốc THPK | (theo cấu hình) | `InTHPK` |

## 8. Cấu Hình Ảnh Hưởng

| KEY | Hành vi khi BẬT (>=1 bản ghi `HIS_CONFIG` với KEY bắt đầu bằng prefix và có VALUE) | Hành vi khi TẮT (không có bản ghi nào) |
|-----|-------------------------------------------------------------------------------|----------------------------------------|
| `HIS.Desktop.Plugins.RefundByTransfer.*` | Nếu thu ngân tick ô "Hoàn tiền ngân hàng" + Lưu có giao dịch hoàn ứng + BN có thông tin thụ hưởng → tự động mở form `HIS.Desktop.Plugins.RefundByTransfer` sau khi Lưu thành công với 4 đầu vào (HIS_TREATMENT, HIS_TRANSACTION, bankCode, RefeshReference). Bank code được trích từ tên KEY (vd `HIS.Desktop.Plugins.RefundByTransfer.MBBInfo` → `MBB`). | Nếu thu ngân vẫn tick → hiển thị thông báo "Chưa cấu hình hoàn tiền ngân hàng!" và không mở form. |

### Logic chi tiết khi `chkRefundByTransfer` tick + Lưu thành công

```
1. Nếu KHÔNG có giao dịch hoàn ứng (rs.TransactionRepay == null) → không làm gì
2. Nếu có giao dịch hoàn ứng:
   2.1. Đọc HIS_CONFIG có KEY bắt đầu "HIS.Desktop.Plugins.RefundByTransfer." và VALUE != null
   2.2. Nếu danh sách rỗng → MessageBox "Chưa cấu hình hoàn tiền ngân hàng!" → dừng
   2.3. Nếu repayPatientBankAccount == null → MessageBox "BN chưa có thông tin thụ hưởng. Vui lòng nhập thông tin thụ hưởng trước." → dừng
        (Lưu thanh toán đã thành công, KHÔNG rollback)
   2.4. GetTreatment(treatmentId) → HIS_TREATMENT
   2.5. Map V_HIS_TRANSACTION → HIS_TRANSACTION (DataObjectMapper)
   2.6. Trích bankCode từ KEY (substring giữa prefix và suffix "Info")
   2.7. PluginInstance.GetPluginInstance(...) → ShowDialog
3. Nếu Lưu thanh toán THẤT BẠI → KHÔNG mở form (bất kể tick hay không)
```

### Các config khác đã có
| KEY | Mục đích |
|-----|----------|
| `HIS.Desktop.Print.TransactionDetail` | Cấu hình mẫu in 106 |
| `HIS.Desktop.Plugins.PaymentQrCode` | QR thanh toán |
| `HIS.Desktop.Plugins.Transaction.IsSplitTotalReceivePrice` | Tách tổng tiền nhận |

## 9. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/05/2026 | tuanln | Thêm checkbox `chkRefundByTransfer` "Hoàn tiền ngân hàng" (**mặc định mỗi lần mở form luôn KHÔNG tick — KHÔNG lưu trạng thái qua ControlState**). Khi tick + Lưu thành công + có giao dịch hoàn ứng: kiểm tra cấu hình `HIS.Desktop.Plugins.RefundByTransfer.*` + thông tin thụ hưởng BN → tự động mở plugin `HIS.Desktop.Plugins.RefundByTransfer` (truyền HIS_TREATMENT, HIS_TRANSACTION, bankCode, RefeshReference). Thêm 2 thông báo riêng plugin: `ChuaCauHinhHoanTienNganHang`, `BNChuaCoThongTinThuHuong` (vi/en/my). Cập nhật Resources/Lang.*.resx, ResourceMessageLang.cs, SetCaptionByLanguageKey. |
| 01/06/2026 | tuanln | Bổ sung ô **Lý do giao dịch** (caption hiển thị "Lý do", control `cboTransactionReason` — LookUpEdit) độc lập với lý do miễn giảm, lấy từ danh mục `HIS_TRANSACTION_REASON` (rule FE-COMMON-01/03, pattern theo TransactionDeposit). Khi mở form: `FillDataToReason()` gọi `api/HisTransactionReason/Get` rồi `SetDefaultReasonByTreatment(currentTreatment)` đặt mặc định theo diện điều trị (ngoại trú→"Khám", nội trú→"Điều trị"); cũng áp lại default trong `btnSearch_Click` khi đổi BN. Khi Lưu (`ProcessSave`): gán `data.Transaction.TRANSACTION_REASON_ID`. Files mới: `frmTransactionBill__Plus__TransactionReason.cs`. Cập nhật: `frmTransactionBill.Designer.cs` (thêm cboTransactionReason + LciTransactionReason + emptySpace ở hàng mới trên "Số tiền BN đưa", thu lưới giao dịch 241→213px để chừa chỗ), `frmTransactionBill.cs` (Load + btnSearch_Click + LoadKeyFrmLanguage), `frmTransactionBill__Plus__Button.cs`, `.csproj`, `Resources/Lang.vi/en/my.resx` (key `frmTransactionBill.LciTransactionReason.Text`). |
| 28/05/2026 | phuongnm | Bổ sung nhập nhiều dòng Chiết khấu cho 1 phiếu thanh toán theo config `MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT`. Khi config = 1: ẩn 3 ô đơn `txtDiscount`/`txtDiscountRatio`/`txtReason`, hiển thị `gridControlDiscount` (cột Chiết khấu (đ), Chiết khấu (%), Lý do, nút Xóa) tại vị trí cũ; 2 cột tự tính cho nhau theo `totalPatientPrice`; cột Lý do tối đa 250 ký tự. `totalDiscount` lấy tổng cột Chiết khấu (đ) → `CalcuCanThu` tự cập nhật label Cần thu. Khi Lưu: `EXEMPTION = tổng cột Chiết khấu (đ)`, `EXEMPTION_REASON = các Lý do nối bằng dấu ';' (cắt 4000)`, gắn `data.Transaction.HIS_TRANSACTION_DISCOUNT` = list dòng grid (ID/DISCOUNT/DISCOUNT_RATIO/REASON/TREATMENT_ID, TRANSACTION_ID do BE gán). Sau Lưu thành công: gọi `api/HisTransactionDiscount/Delete` cho các dòng đã bị user xóa trên grid và reload grid theo TRANSACTION_ID mới. Khi mở `currentTransaction` (chế độ thay thế hóa đơn): gọi `api/HisTransactionDiscount/Get` (filter TRANSACTION_ID) để fill grid. Files mới: `ADO/HisTransactionDiscountADO.cs`, `frmTransactionBill__Plus__GridDiscount.cs`. Cập nhật: `Config/HisConfigCFG.cs` (key `EnableMultiDiscount`), `RequestUriStore.cs` (2 URI mới), `frmTransactionBill.cs` (Load → `BuildDiscountGrid`, ResetControlValue, SetDefaultValueTransaction), `frmTransactionBill__Plus__Button.cs` (ProcessSave, post-save reload), `.csproj`, `Resources/Lang.vi/en/my.resx` (5 key mới). |

## 10. Test Cases

### Cấu hình
- [ ] Khi DB có cấu hình `HIS.Desktop.Plugins.RefundByTransfer.MBBInfo` (VALUE != null) → coi là BẬT
- [ ] Khi DB KHÔNG có cấu hình nào với prefix `HIS.Desktop.Plugins.RefundByTransfer.` → coi là TẮT

### Hoàn tiền ngân hàng — flow chính
- [ ] BẬT cấu hình + tick chkRefundByTransfer + Lưu thành công + có TransactionRepay + BN có thụ hưởng → mở form RefundByTransfer với đủ 4 đầu vào (treatment, transaction, bankCode="MBB", callback)
- [ ] BẬT cấu hình + tick chkRefundByTransfer + Lưu thành công + có TransactionRepay + BN KHÔNG có thụ hưởng → MessageBox "BN chưa có thông tin thụ hưởng..." → không mở form, lưu vẫn thành công
- [ ] TẮT cấu hình + tick chkRefundByTransfer + Lưu thành công + có TransactionRepay → MessageBox "Chưa cấu hình hoàn tiền ngân hàng!" → không mở form
- [ ] BẬT cấu hình + tick chkRefundByTransfer + Lưu thành công + KHÔNG có TransactionRepay (BN không dư) → không làm gì, không hiện thông báo
- [ ] BẬT cấu hình + KHÔNG tick chkRefundByTransfer + Lưu thành công + có TransactionRepay → không làm gì
- [ ] BẬT cấu hình + tick chkRefundByTransfer + Lưu THẤT BẠI → không mở form

### Mặc định không tick (KHÔNG lưu trạng thái)
- [ ] Lần đầu mở form → checkbox unchecked
- [ ] Tick chkRefundByTransfer → đóng form → mở lại → **vẫn unchecked** (KHÔNG nhớ trạng thái)

### Trùng cấu hình
- [ ] Khi có nhiều bản ghi prefix (vd MBBInfo + VCBInfo) → bankCode lấy từ bản ghi đầu tiên (FirstOrDefault)

### Đa ngôn ngữ
- [ ] Switch ngôn ngữ vi/en → caption + tooltip + 2 thông báo mới hiển thị đúng theo culture
