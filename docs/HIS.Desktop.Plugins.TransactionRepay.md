# Hoàn ứng (TransactionRepay) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionRepay |
| Loại | Form (FormBase) |
| Mục đích | Tạo giao dịch chi tiền (hoàn ứng) cho bệnh nhân: hoàn tạm ứng, hoàn ngoại trú, hoàn nội trú ra viện, **hoàn theo phiếu nhập lại từ bán lẻ (việc 42727)**. |
| Người tạo | IVT |
| Ngày tạo | — |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (mở từ Transaction list / TransactionBill)
1. Người dùng chọn 1 điều trị → bấm Hoàn ứng.
2. Form mở, tự điền số tiền theo `TOTAL_PATIENT_PRICE - TOTAL_DEPOSIT - TOTAL_BILL_AMOUNT…` (LoadTreatmentAmount).
3. Tự điền lý do dựa trên `TREATMENT_TYPE_ID` (KHAM/DTNGOAITRU/DTNOITRU + IS_PAUSE).
4. Người dùng chọn sổ kế toán + hình thức thanh toán → Lưu.

### Luồng "Hoàn ứng theo gói bệnh nhân" (mới — việc 45677)
1. Mở từ "Danh sách gói" (HIS.Desktop.Plugins.HisPatientPackage — task khác) → nút Hoàn tiền.
2. Form nhận `TransactionRepayADO` có thêm `Patient` (HIS_PATIENT) + `PatientPackage` (HIS_PATIENT_PACKAGE).
3. Khi `PatientPackage != null`:
   - Hiển thị hàng thông tin gói gồm 4 ô có nhãn riêng: `lciPackageName` (Gói bệnh nhân = `PACKAGE_NAME`), `lciRegisterDate` (Ngày đăng ký = `REGISTER_DATE`), `lciTotalPaid` (Số tiền đã đóng = `TOTAL_PAID`), `lciTotalUsed` (Số tiền đã dùng = `TOTAL_USED`).
   - `LoadTreatmentAmount()` set số tiền hoàn mặc định = `TOTAL_PAID - TOTAL_REFUNDED - TOTAL_USED` (bỏ qua tính theo công nợ điều trị).
   - `SaveRepay()` set `HIS_TRANSACTION.PATIENT_PACKAGE_ID = PatientPackage.ID` → lưu gói vào giao dịch.
4. Khi `PatientPackage == null` (hoàn ứng thường): ẩn 4 item gói (`Visibility = Never`) + thu nhỏ form 24px → giữ nguyên giao diện cũ.

### Luồng "Nhập lại xuất bán" (mới — việc 42727)
1. Mở từ Danh sách nhập (HisImportMestMedicine) → click icon "Tạo giao dịch chi tiền" trên dòng phiếu.
2. Form nhận `TransactionRepayADO` có thêm:
   - `ImpMestId` — mã phiếu nhập
   - `AutoAmount` — tổng tiền lấy sẵn từ phiếu xuất bán gốc
   - `RepayReasonCode` — "07" (Nhập lại xuất bán)
3. Form bypass tính toán LoadTreatmentAmount, dùng trực tiếp `AutoAmount`.
4. Form chọn lý do theo `RepayReasonCode` thay vì rule mặc định.
5. Khi save: SDO `HisTransactionRepaySDO.IMP_MEST_ID = ImpMestId` được gửi lên API `CreateRepay` → backend tự ghi `REPAY_ID` ngược lại `HIS_IMP_MEST`.

### Sơ đồ luồng dữ liệu (việc 42727)
```
HisImportMestMedicine.repositoryItemButtonRepayEnable_ButtonClick
  └► api/HisExpMest/GetView (CHMS_EXP_MEST_ID)
  └► PluginInstance.GetPluginInstance(TransactionRepay,
        [ TransactionRepayADO { ImpMestId, AutoAmount, RepayReasonCode="07" }, Module ])
       └► frmTransactionRepay
            ├► constructor: lưu ImpMestId, AutoAmount, preferredRepayReasonCode
            ├► LoadTreatmentAmount(): nếu ImpMestId.HasValue ⇒ txtTotalAmount = AutoAmount
            ├► SetDefaultRepayReason(): nếu preferredRepayReasonCode != null ⇒ chọn theo code
            └► SaveRepay(): data.IMP_MEST_ID = ImpMestId.Value → POST CreateRepay
                  └► Backend: tạo HIS_TRANSACTION + ghi HIS_IMP_MEST.REPAY_ID
```

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TRANSACTION | Table | Giao dịch hoàn ứng (tạo mới). **Set `PATIENT_PACKAGE_ID` khi hoàn theo gói (45677)** |
| HIS_PATIENT_PACKAGE | Table | **(45677) Gói bệnh nhân truyền vào — đọc PACKAGE_NAME, REGISTER_DATE, TOTAL_PAID, TOTAL_USED, TOTAL_REFUNDED** |
| HIS_PATIENT | Table | **(45677) Thông tin bệnh nhân truyền vào (kèm theo gói)** |
| V_HIS_TRANSACTION | View | Hiển thị kết quả sau khi tạo |
| V_HIS_TREATMENT_FEE | View | Tính tổng tiền tự động (LoadTreatmentAmount) |
| V_HIS_PATIENT_TYPE_ALTER | View | Đối tượng BHYT — quyết định lý do mặc định |
| V_HIS_ACCOUNT_BOOK | View | Sổ kế toán cho hoàn ứng |
| HIS_REPAY_REASON | Table | Danh mục lý do hoàn ứng. **Bổ sung record code "07" — "Nhập lại xuất bán" (42727)** |
| HIS_PAY_FORM | Table | Hình thức thanh toán |
| V_HIS_PATIENT_BANK_ACCOUNT | View | Tài khoản ngân hàng BN (chuyển khoản) |

## 4. UI Layout

### Sơ đồ giao diện (KHÔNG đổi cấu trúc — việc 42727)
```
+----------------------------------------------------------+
| Mã điều trị: [TR000123]   Tên BN: [Nguyễn Văn A]         |
+----------------------------------------------------------+
| Số tiền hoàn:   [_______________]                         |
| Lý do hoàn ứng: [Cbo: Nhập lại xuất bán (07)]            |
| Sổ kế toán:     [Cbo]                                     |
| Hình thức:      [Cbo]   TG giao dịch: [_______]          |
| Diễn giải:      [_______________________________]         |
| TIG/POS info:   [...]                                     |
| [Xem trước phiếu in]   [Lưu (Ctrl+S)] [Lưu+In] [Đóng]   |
+----------------------------------------------------------+
```

Chế độ "Nhập lại xuất bán" → các trường Số tiền, Lý do được pre-fill ngay khi form load. Người dùng vẫn có thể sửa trước khi Lưu.

## 5. API Endpoints

| Action | URI | Consumer | DTO/Filter |
|--------|-----|----------|------------|
| Kiểm tra trước khi qua thẻ | /api/HisTransaction/CheckRepay | MosConsumer | HisTransactionRepaySDO |
| **Tạo giao dịch hoàn ứng** | **/api/HisTransaction/CreateRepay** | **MosConsumer** | **HisTransactionRepaySDO (mở rộng `IMP_MEST_ID` — 42727)** |
| Lấy đối tượng BHYT cuối | api/HisPatientTypeAlter/GetLastByTreatmentId | MosConsumer | long treatmentId |
| Lấy V_HIS_TREATMENT_FEE | api/HisTreatment/GetFeeView | MosConsumer | HisTreatmentFeeViewFilter |
| Sổ kế toán | api/HisAccountBook/GetView | MosConsumer | HisAccountBookViewFilter |
| Lý do hoàn ứng | api/HisRepayReason/Get | MosConsumer | (filter rỗng) |

## 6. Dependencies

### ADO (HIS.Desktop.ADO/TransactionRepayADO.cs) — bổ sung 42727
| Property | Kiểu | Mô tả |
|----------|------|-------|
| TreatmentId | long | Mã điều trị |
| CashierRoomId | long | Phòng thu ngân |
| Treatment | V_HIS_TREATMENT_FEE | Tổng phí điều trị (tự load nếu null) |
| PatientTypeAlter | V_HIS_PATIENT_TYPE_ALTER | Đối tượng BHYT cuối |
| **ImpMestId** | **long?** | **(42727) Mã phiếu nhập từ luồng "Nhập lại xuất bán"** |
| **AutoAmount** | **decimal?** | **(42727) Số tiền hoàn được tính sẵn từ phiếu xuất bán gốc** |
| **RepayReasonCode** | **string** | **(42727) Mã lý do mặc định, vd "07"** |
| **Patient** | **HIS_PATIENT** | **(45677) Thông tin bệnh nhân khi hoàn theo gói** |
| **PatientPackage** | **HIS_PATIENT_PACKAGE** | **(45677) Gói bệnh nhân — kích hoạt hiển thị + lưu PATIENT_PACKAGE_ID** |

### Plugin gọi đến TransactionRepay
- HIS.Desktop.Plugins.Transaction (UCTransaction__Plus__Button)
- HIS.Desktop.Plugins.TransactionBill
- HIS.Desktop.Plugins.TransactionBillTwoInOne
- **HIS.Desktop.Plugins.HisImportMestMedicine (mới — 42727)**
- **HIS.Desktop.Plugins.HisPatientPackage (mới — 45677, màn "Danh sách gói" → nút Hoàn tiền)**

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu thu hoàn ứng | Mps000113 | RichEditorStore + MPS.MpsPrinter | PhieuThuHoanUng |
| Phiếu chi nhanh | (CheckBox xtraTabIn) | (đã code trong frm) | — |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-09 | dangth2 | Việc 42727 — Thêm 3 trường tùy chọn vào TransactionRepayADO (ImpMestId, AutoAmount, RepayReasonCode); LoadTreatmentAmount bỏ qua tính tự động khi có AutoAmount; SetDefaultRepayReason ưu tiên RepayReasonCode; SaveRepay truyền `data.IMP_MEST_ID` lên API CreateRepay. |
| 2026-05-14 | dangth2 | Việc 42727 (đọc lại PTTK) — Plugin không thay đổi mới; HisImportMestMedicine giờ thêm cột "In phiếu hoàn ứng" (MPS000113) và auto-refresh grid sau khi đóng form TransactionRepay để bật icon in phiếu. |
| 2026-05-27 | tuanln | Việc 45677 — Hoàn ứng theo gói bệnh nhân. Thêm `Patient` + `PatientPackage` vào TransactionRepayADO; form hiển thị hàng thông tin gói gồm 4 ô có nhãn (`lciPackageName/lciRegisterDate/lciTotalPaid/lciTotalUsed` + label giá trị `lblPackageName/lblRegisterDate/lblTotalPaid/lblTotalUsed`); số tiền hoàn mặc định = TOTAL_PAID - TOTAL_REFUNDED - TOTAL_USED; SaveRepay set `HIS_TRANSACTION.PATIENT_PACKAGE_ID`; khi không có gói thì ẩn 4 item + thu nhỏ form (giữ giao diện cũ). Resource vi/en/my keys `frmTransactionRepay.lci{PackageName/RegisterDate/TotalPaid/TotalUsed}.Text`. |
| 2026-07-01 | tuanln | Fix phím tắt không hoạt động (Ctrl+S Lưu / Ctrl+P In / Ctrl+N Mới / Ctrl+I Lưu in). Nguyên nhân: block cấu hình `barManager1` bị mất khỏi `frmTransactionRepay.Designer.cs` khi đưa plugin vào monorepo → các `BarButtonItem` (`bbtnRC*`) mồ côi, `ItemShortcut` không được xử lý. Fix: khởi tạo lại `barManager1` trong constructor (cùng pattern 42727 đã init `dxValidationProvider1`/`timerInitForm`), gán `barManager1.Form = this` + đăng ký 4 `bbtnRC*` vào `barManager1.Items`. Nút bấm chuột không đổi. |

## 9. Test Cases — Việc 42727

### Mở từ luồng cũ (Transaction list)
- [ ] ADO không có ImpMestId → form chạy như cũ
- [ ] LoadTreatmentAmount tự tính từ V_HIS_TREATMENT_FEE
- [ ] Lý do mặc định theo TREATMENT_TYPE_ID

### Mở từ luồng "Nhập lại xuất bán" (HisImportMestMedicine)
- [ ] ADO có ImpMestId + AutoAmount + RepayReasonCode = "07"
- [ ] Form Load: txtTotalAmount = AutoAmount (KHÔNG tính từ TreatmentFee)
- [ ] Form Load: cboRepayReason = record có code "07"
- [ ] Người dùng sửa số tiền → Lưu được; sửa lý do → Lưu được

### Save
- [ ] Khi ImpMestId.HasValue: HisTransactionRepaySDO truyền IMP_MEST_ID lên API
- [ ] Khi ImpMestId == null: SDO không có IMP_MEST_ID (giữ nguyên luồng cũ)
- [ ] API CreateRepay trả V_HIS_TRANSACTION → reload form, hiển thị mã giao dịch
- [ ] Backend tự ghi REPAY_ID vào HIS_IMP_MEST (xác minh qua API hoặc reload Danh sách nhập)

## 10. Test Cases — Việc 45677 (Hoàn ứng theo gói)

### Mở KHÔNG có gói (hoàn ứng thường)
- [ ] PatientPackage == null → ẩn hàng thông tin gói, form cao như cũ (~196px)
- [ ] Số tiền hoàn vẫn tính theo công nợ điều trị (LoadTreatmentAmount cũ)

### Mở CÓ gói (từ Danh sách gói)
- [ ] Hiển thị đúng: tên gói, ngày đăng ký (dd/MM/yyyy), số tiền đã đóng, số tiền đã dùng
- [ ] Số tiền hoàn mặc định = TOTAL_PAID - TOTAL_REFUNDED - TOTAL_USED
- [ ] Form cao thêm 24px để chứa hàng thông tin gói
- [ ] Đa ngôn ngữ: chuyển vi/en → caption hàng gói đổi theo

### Save (có gói)
- [ ] HIS_TRANSACTION.PATIENT_PACKAGE_ID = PatientPackage.ID
- [ ] Hoàn ứng thành công, in phiếu Mps000113 bình thường
