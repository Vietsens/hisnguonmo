# Xuất Bán (Kho Nhà Thuốc) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestSaleCreate |
| Loại | UC (UCExpMestSaleCreate — UserControlBase), 8 partial (`___Proccess`, `__Load`, `__InitPrint`, `__Plus_Search`, `__Shortcut`, `__MenuMouseRight`, `__Validate`, `___SaveSignPrintInvoice`) |
| Mục đích | Bán thuốc/vật tư tại kho nhà thuốc: tạo/sửa phiếu xuất bán (theo đơn hoặc vãng lai), tạo bill khi tick "Xuất biên lai/hóa đơn", in phiếu xuất bán, mở form Xuất hóa đơn (MedicineSaleBill), hủy xuất, xác nhận nợ. Menu "Nhập xuất tồn > Xuất bán". |
| Người tạo | team (việc 36371) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
Chọn kho xuất → nhập BN (mã điều trị/mã đơn/vãng lai) → kê thuốc/vật tư (kiểm tra tồn khả dụng khi thêm dòng) → **Lưu (F5)** hoặc **Lưu in (F9)** → `ProcessSave`: POST `api/HisExpMest/SaleCreateListSdo` (1 BN) / `SaleCreateBillList` (nhiều BN) / `SaleUpdateListSdo` (sửa); tick **"Xuất biên lai/hóa đơn"** (`chkCreateBill`) → `CreateBill = true` → BE tạo luôn bill (`resultSDO.Transaction`) theo sổ/điểm thu/hình thức chọn ở panel phải → Lưu in in phiếu xuất bán (Mps) hoặc hóa đơn/biên lai giấy (Mps000339) → **Xuất hóa đơn (F10)** mở form `HIS.Desktop.Plugins.MedicineSaleBill` (chỉ với phiếu chưa có bill).

### Trạng thái phiếu sau lưu (phụ thuộc BE)
Kho `HIS_MEDI_STOCK_EXTY.IS_AUTO_APPROVE/IS_AUTO_EXECUTE` + `MOS.EXP_MEST.EXPORT_SALE.MUST_BILL` + `MOS.TRANSACTION.EXP_MEST_SALE.IS_AUTO_EXPORT` quyết định phiếu về Yêu cầu / Đã duyệt / Hoàn thành ngay khi lưu hoặc khi tạo bill; `btnCancelExport` (Hủy xuất) chỉ enable khi có phiếu HOÀN THÀNH.

### Nút "Lưu ký in" (việc 3082 — v3.2 29/08/2026)
- Nút mới `btnSaveSignPrint` **"Lưu ký in (Ctrl E / F11)"** ngay trước "Lưu in"; **luôn hiện, không key config**.
- **Enable/disable theo checkbox "Xuất biên lai/hóa đơn"**: `btnSaveSignPrint.Enabled = chkCreateBill.Checked && btnSavePrint.Enabled` (`RefreshSaveSignPrintButton`, đồng bộ qua `chkCreateBill.CheckedChanged`, `btnSavePrint.EnabledChanged`, `EnabledChanged/VisibleChanged` của UC + `BeginInvoke` refresh sau Load — vì `Control.Enabled` là trạng thái hiệu dụng, = false khi WaitingManager khóa form cha lúc load). Checkbox "Xuất biên lai/hóa đơn" giữ nguyên code gốc.
- Bấm: `savePrintInvoice = true`, `savePrint = false` → `btnSave_Click` → `ProcessSave` lưu phiếu **+ BE tạo bill** (vì đang tick), **không in phiếu xuất bán** → sau `MessageManager.Show` gọi `OpenMedicineSaleBillAutoSignPrint()`: mỗi kết quả lưu có `Transaction` → mở form Xuất hóa đơn với args `(Module, List<long> expMestIds, DelegateSelectData EnableControlAfterSaveSaleBill, List<string> { "AUTO_ISSUE_EXISTING_BILL", "TRANSACTION_ID=<id>" })` → form phát hành HĐĐT cho bill đó, tự duyệt/thực xuất, in thẳng, tự đóng (xem docs MedicineSaleBill). Bill hình thức QR → bỏ qua (ProcessSave đã mở module QR). Không có Transaction → `{ "AUTO_SAVE_SIGN_PRINT" }` (form tự tạo bill, chỉ phiếu chưa có bill). Nhánh key `ExpMestSaleCreate__Show_MedicineSaleBill` bị bỏ qua trong lượt này.
- Flag `savePrintInvoice` reset trong `finally` của `btnSaveSignPrint_Click` và catch của `ProcessSave`.
- Nút "Lưu in" giữ nguyên luồng cũ.

### Ô tích "In HDSD" (việc 56651 — 20/10/2026, BV HAGL)
- Ô `chkInHDSD` **"In HDSD:"** ngay sau "Xem trước khi in" (tooltip "In hướng dẫn sử dụng thuốc ngay sau khi lưu thành công"). Mặc định không tích, nhớ theo máy (ControlState khóa `chkInHDSD`). Không key config.
- Tích → `ProcessSave`, sau khối in của Lưu in và sau khi trả `savePrint = false`: `if (success && chkInHDSD.Checked) PrintHdsdAfterSave()` → in **Mps000099** sau mọi lần lưu thành công (Lưu, Lưu in, Lưu ký in), trước `MessageManager.Show` và trước form Xuất hóa đơn (Lưu ký in / key `Show_MedicineSaleBill`).
- `PrintHdsdAfterSave` bật cờ `isAutoPrintHdsd` rồi gọi `onClickInHuongDanSuDung` (đường in có sẵn, nhiều bệnh nhân → mỗi bệnh nhân 1 bản). `GetHdsdPreviewType`: in tự động theo "Xem trước khi in" (tích → ShowDialog, không tích → PrintNow); in từ menu In giữ theo `CheDoInChoCacChucNangTrongPhanMem`.
- Key `IS_MUST_BE_FINISHED_BEFORED_PRINTING` chỉ chặn phiếu xuất bán, không chặn HDSD (như in HDSD từ menu).

### Đính kèm đơn thuốc (việc 57853 — 29/09/2026)
- Gate: `MOS.HAS_CONNECTION_EMR = 1` **và** `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable = 1` (mặc định 0). Tắt → **không tạo nút**, giao diện như cũ.
- Nút **"Đính kèm đơn"** tạo lúc runtime (`InitAttachPrescriptionButton`, partial `UCExpMestSaleCreate___AttachPrescription.cs`), chèn bên trái "Hủy xuất" bằng item-move (`LayoutControlItem.Move(layoutControlItem1, InsertType.Left)`), cố định 120px.
- **Chỉ enable khi phiếu đã lưu** (cập nhật 02/10/2026): phiếu chưa lưu (ADD) → nút **disable**. Trạng thái nút cập nhật trong setter `moduleAction` (về ADD thì bỏ luôn phiếu đang giữ) và `SetAttachExpMests`, nên mọi luồng Mới / Đơn mới / tìm đơn / mở sửa / sau lưu đều đúng. Bỏ chế độ giữ tệp tạm trước khi lưu trên màn này.
- **Bán nhiều bệnh nhân** (`SaleCreateBillList`): sau lưu nút enable nhưng bấm → cảnh báo đính kèm từng phiếu tại Danh sách xuất (mỗi bệnh nhân một đơn riêng).
- **Phiếu đã lưu (EDIT)** — sau lưu, mở sửa từ danh sách, tìm theo đơn (`SetAttachExpMests` ở `LoadDataExpMestByEdit` / `LoadDataExpMestByEditMulti` / `LoadDataExpMestBySearch` / `ProcessAttachPrescriptionAfterSave`): 1 phiếu → mở **Danh sách đơn đính kèm** (xem/in, đính kèm mới, xóa theo trạng thái); nhiều phiếu cùng bệnh nhân → chọn tệp rồi đính kèm cho từng phiếu (tên văn bản theo mã của từng phiếu).
- Không cho xóa khi phiếu Hoàn thành / có `BILL_ID` / có `DEBT_ID` (phiếu đã chốt kỳ kho luôn Hoàn thành). **Chặn ở frontend** — xem rủi ro trong docs thư viện.

### Form DS (F3) — hiện thuốc/vật tư đã kê (việc 57699, tài liệu 3485 — 24/10/2026, BV HAGL)
- Form `FormChoosePrescription/FormPrescription` ("Danh sách đơn mua ngoài"): lưới phải (đơn đã chọn) là master-detail. Mỗi dòng bệnh nhân có nút +/− ở đầu dòng; mở rộng thì hiện bảng thuốc/vật tư đã kê: Tên ("STT - tên"), Mã thuốc, Hàm lượng, Hoạt chất, Số lượng, ĐVT. Dòng bệnh nhân giữ nguyên cột cũ.
- Tích chọn (`gridViewServiceReqSearch_SelectionChanged`) → `LoadMediMateByServiceReqs`: chỉ lấy các y lệnh chưa có trong `dicMediMateByServiceReq`, gọi `api/HisServiceReqMety/Get` + `api/HisServiceReqMaty/Get` (`SERVICE_REQ_IDs`, tối đa 100/lượt). Một trong hai trả null (lỗi) → không ghi nhớ, lần tích sau lấy lại. **Chỉ đọc, không TakeBean** — giá/tồn vẫn chỉ tính khi bấm Đồng ý.
- Dựng dòng giống `ProcessorSearch` của màn chính: gộp theo loại trong cùng y lệnh (cộng AMOUNT). Dòng kê ngoài danh mục (không có `MEDICINE_TYPE_ID`/`MATERIAL_TYPE_ID`) vẫn hiện (BA VIETNT 24/10), gộp theo tên (không phân biệt hoa thường, trim), tên kèm "(ngoài danh mục)" (`ResourceMessageLang.NgoaiDanhMuc`), cột mã/hàm lượng/hoạt chất trống — Đồng ý vẫn không đưa các dòng này sang màn chính. Mã/hàm lượng/hoạt chất/ĐVT từ `V_HIS_MEDICINE_TYPE`/`V_HIS_MATERIAL_TYPE` (ĐVT dự phòng `UNIT_NAME`), tên nối hàm lượng khi `HIS.HIS_MEDI_STOCK.IS_JOIN_NAME_WITH_CONCENTRA` = 1, sắp theo `NUM_ORDER` rồi tên. Không có cột giá (giá trên dòng kê không phải giá bán).
- `UpdateDataGridSelect` gắn lại lưới rồi `ExpandMediMateRows`: `CollapseAllDetails` rồi mở mọi dòng trừ dòng người dùng đã thu gọn (`collapsedServiceReqIds`, ghi bởi `MasterRowCollapsed/Expanded`, bỏ qua lúc đang gắn lưới). **Bẫy DevExpress 15.2:** gán lại cùng List vào `DataSource` không làm gì, chi tiết đang mở bám theo row handle → sau khi xóa dòng sẽ hiện thuốc của bệnh nhân khác nếu không thu hết trước. Xóa dòng thì quên trạng thái thu gọn của dòng đó. Y lệnh không có thuốc/vật tư → `MasterRowEmpty` → nút mở rộng mờ (`SmartDetailExpandButtonMode = CheckAllDetails`).
- View con `EnableMasterViewMode = false` (ADO có `List<long> BeanIds` → DevExpress tự tạo quan hệ cấp 3). Cột Tên `Fixed = Left`. Cột STT dòng bệnh nhân 30 → 45 px vì nút +/− nằm trong ô STT. Lấy dữ liệu có `WaitingManager` (chỉ hiện khi quá `PendingTime`).
- Nút `btnCollapseAll` "Thu gọn tất cả" (thêm mọi y lệnh đang có vào `collapsedServiceReqIds` + `CollapseAllDetails`) và `btnExpandAll` "Mở rộng tất cả" (xóa `collapsedServiceReqIds` + `ExpandMediMateRows`), bên trái nút Đồng ý (thay `emptySpaceItem2`). Không gọi API.
- Chưa có cột giá: BA chốt cần Đơn giá bán, VAT bán, Thành tiền bán (24/10), đang hỏi lại nguồn giá vì form DS chưa giữ tồn.
- Bảng thuốc/vật tư bắt đầu thẳng cột xóa (`LevelIndent = 45` = độ rộng cột STT) và không có thanh cuộn ngang riêng (`HorzScrollVisibility = Never`): cuộn theo thanh cuộn của lưới phải (cột Tên không ghim trái để trượt cùng nhịp với dòng bệnh nhân) — `LeftCoordChanged`/`TopRowChanged` của lưới phải gán `LeftCoord` cho mọi chi tiết đang mở (`SyncDetailLeftCoord`; TopRowChanged vì chi tiết nằm ngoài màn chưa nhận được vị trí cuộn); chi tiết tự cuộn (bấm ô bị khuất) thì bị kéo về theo lưới phải. Theo góp ý tester 26/10.

### Điều kiện nghiệp vụ
- Không kê vượt tồn khả dụng (validation khi thêm dòng).
- Hình thức Tiền mặt/CK, Tiền mặt/QT: số tiền CK/QT không vượt tổng phải thanh toán.
- POS (`ChkKetNoiPOS` + `chkCreateBill`): gọi máy POS trước khi lưu.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_MEDI_STOCK | View | Kho xuất |
| HisMedicineTypeInStockSDO / HisMaterialTypeInStockSDO | SDO | Tồn khả dụng theo loại thuốc/vật tư |
| HisExpMestSaleListSDO / HisExpMestSaleListResultSDO | SDO | Dữ liệu lưu phiếu bán (CreateBill) + kết quả (ExpMestSdos, Transaction) |
| HIS_EXP_MEST | Table | Phiếu xuất bán (EXP_MEST_STT_ID, BILL_ID) |
| HIS_TRANSACTION | Table | Bill tạo khi tick "Xuất biên lai/hóa đơn" (ID truyền sang form hóa đơn) |
| V_HIS_SERVICE_REQ_11 | View | Đơn thuốc nguồn |
| HIS_SERVICE_REQ_METY / HIS_SERVICE_REQ_MATY | Table | Thuốc/vật tư kê ngoài kho của đơn — bảng chi tiết ở form DS (F3) (57699) |
| V_HIS_MEDICINE_TYPE / V_HIS_MATERIAL_TYPE | View (cache) | Mã, hàm lượng, hoạt chất, ĐVT cho bảng chi tiết form DS (F3) |
| HIS_PATIENT | Table | Bệnh nhân |
| HIS_PAY_FORM, V_HIS_ACCOUNT_BOOK, V_HIS_CASHIER_ROOM | Table/View | Hình thức thanh toán, sổ, phòng thu |

## 4. UI Layout

```
+------------------------------------------------------------------------------+
| Kho xuất | Đối tượng | Vãng lai | Đơn cũ            Xuất biên lai/hóa đơn ☐  |
| Mã | Mã đơn thuốc | Mã điều trị | DS (F3) | Bệnh nhân | Giới tính | Ngày sinh |
| ... thông tin BN / phiếu / chẩn đoán ...                Tổng tiền / Phải TT   |
| Chọn thuốc/vật tư (Ctrl F) | Số lượng | Số ngày | Giá | VAT | Chiết khấu ...  |
| Lưới thuốc/vật tư đã kê                         | Lưới tồn kho (bên phải)    |
+------------------------------------------------------------------------------+
| Tự động hiển thị tồn ☐ | Ký đơn nhà thuốc ☐ | Xem trước khi in ☑ | In HDSD ☐  |
| Hủy xuất (Ctrl H) | [Lưu ký in (Ctrl E)] | Lưu in (F9) | Lưu (F5) | Mới | Đơn mới |
| In ▾ | QR | Xuất hóa đơn (F10) | Xác nhận nợ                                    |
+------------------------------------------------------------------------------+
```
`[Lưu ký in]` = `btnSaveSignPrint` (việc 3082) — chỉ enable khi tick "Xuất biên lai/hóa đơn".
`[Đính kèm đơn]` (việc 57853) — nút tạo runtime bên trái "Hủy xuất", chỉ có khi bật config `...AttachPrescription.IsEnable`. Phím tắt F5/F7/F8/F9/F10/F11 khi key `ExpMestSaleCreate.IsUsingFunctionKeyInsteadOfCtrlKey` = 1, ngược lại Ctrl S/D/N/I/T/E.

### UC sử dụng
| UC | Mục đích |
|----|----------|
| HIS.UC.Icd + HIS.UC.SecondaryIcd | Chẩn đoán chính/phụ |
| HIS.UC.MedicineTypeInStock / MaterialTypeInStock | Chọn thuốc/vật tư còn tồn |
| HIS.UC.ExpMestMedicineGrid / ExpMestMaterialGrid | Lưới chi tiết phiếu |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lưu phiếu bán 1 BN (+ bill nếu CreateBill) | api/HisExpMest/SaleCreateListSdo | MosConsumer |
| Lưu phiếu bán nhiều BN | api/HisExpMest/SaleCreateBillList | MosConsumer |
| Sửa phiếu bán | api/HisExpMest/SaleUpdateListSdo | MosConsumer |
| Đơn thuốc nguồn | api/HisServiceReq/Get | MosConsumer |
| Form DS (F3) — danh sách đơn | api/HisServiceReq/GetView11 | MosConsumer |
| Form DS (F3) — thuốc/vật tư đã kê của đơn đã chọn (57699, chỉ đọc) | api/HisServiceReqMety/Get, api/HisServiceReqMaty/Get — `SERVICE_REQ_IDs` ≤ 100/lượt | MosConsumer |
| Bệnh nhân | api/HisPatient/Get | MosConsumer |
| Đính kèm đơn — tạo tài liệu (57853) | api/EmrDocument/CreateByTdo (qua Library.ExpMestAttachFile) | EmrConsumer |
| Đơn đính kèm của phiếu / đánh dấu (57853) | api/EmrDocument/GetView — `TREATMENT_CODEs` = EXP_MEST_CODE + `DOCUMENT_TYPE_ID` = EXPSA + `IS_DELETE=false` | EmrConsumer |
| Xem / xóa mềm đơn (57853) | api/EmrDocument/DownloadFile, api/EmrDocument/Delete | EmrConsumer |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.MedicineSaleBill | Nút Xuất hóa đơn (F10); key `Show_MedicineSaleBill` = 1 sau lưu | Module, List<long> expMestIds, DelegateSelectData |
| HIS.Desktop.Plugins.MedicineSaleBill (chế độ tự động, 3082 v3.2) | Nút "Lưu ký in" (tick "Xuất biên lai/hóa đơn"), sau lưu thành công | Module, List<long> expMestIds, DelegateSelectData, **List<string> { "AUTO_ISSUE_EXISTING_BILL", "TRANSACTION_ID=<id>" }** (bill đã tạo) hoặc **{ "AUTO_SAVE_SIGN_PRINT" }** (không có bill) |
| HIS.Desktop.Plugins.CreateTransReqQR | Lưu có tạo bill với hình thức QR | TransReqQRADO |

### Library
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Library.CacheClient.ControlStateWorker | Nhớ checkbox (Xem trước khi in, Xuất biên lai/hóa đơn, POS, Ký đơn, In HDSD) |
| Inventec.Common.RichEditor.RichEditorStore | In phiếu xuất bán / hóa đơn biên lai / HDSD |
| **HIS.Desktop.Plugins.Library.ExpMestAttachFile** (mới, 57853) | Chọn tệp/chụp ảnh (HIS.Desktop.Plugins.Camera), kiểm tra định dạng/dung lượng, gộp PDF, lưu EMR (EXPSA), danh sách đơn đính kèm |

## 7. Print

| Loại in | PrintTypeCode | Ghi chú |
|---------|--------------|---------|
| Phiếu xuất bán | theo `InitMenuPrint` (menu In) | `onClickInPhieuXuatBan` — Lưu in không tick "Xuất biên lai/hóa đơn" |
| Hóa đơn/biên lai xuất bán | Mps000339 | `onClickInHoaDonBienLaiXuatBan` — Lưu in khi tick "Xuất biên lai/hóa đơn" |
| Hướng dẫn sử dụng thuốc | Mps000099 | menu In (theo cấu hình máy); tự in sau mọi lần lưu thành công khi tích "In HDSD" (theo "Xem trước khi in") — việc 56651 |
| Hóa đơn điện tử (3082) | — | In thẳng tại form MedicineSaleBill (DocumentViewerManager.Print) sau "Lưu ký in" |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 24/10/2026 | dangth | **Việc 57699 / tài liệu 3485 (BV HAGL)** — Form DS (F3) "Danh sách đơn mua ngoài": lưới đơn đã chọn thành master-detail, mỗi dòng bệnh nhân có nút +/− hiện/ẩn thuốc/vật tư đã kê (Tên, Mã thuốc, Hàm lượng, Hoạt chất, Số lượng, ĐVT). Lấy bằng `api/HisServiceReqMety/Get` + `api/HisServiceReqMaty/Get` theo y lệnh mới chọn (chỉ đọc, không giữ tồn, ghi nhớ trong lần mở form), dựng dòng như `ProcessorSearch` của màn chính; mặc định mở rộng, nhớ dòng người dùng thu gọn. Không có cột giá. Partial mới `FormPrescription__MediMate.cs`. Đồng ý và màn chính không đổi. |
| 24/10/2026 | dangth | **Việc 57699 / tài liệu 3485 — theo BA VIETNT trả lời mục I.6:** hiện thuốc/vật tư kê ngoài danh mục (tên kèm "(ngoài danh mục)", gộp theo tên; key `Plugins_ExpMestSaleCreate__NgoaiDanhMuc` trong Message.Lang vi/en); thêm nút "Thu gọn tất cả" / "Mở rộng tất cả" dưới lưới phải. Cột giá bán: chờ chốt nguồn giá. |
| 26/10/2026 | dangth | **Việc 57699 / tài liệu 3485 — góp ý tester:** bảng thuốc/vật tư thẳng cột xóa, bỏ thanh cuộn ngang riêng của từng bảng, cuộn theo thanh cuộn của lưới phải. |
| 20/10/2026 | dangth | **Việc 56651 (BV HAGL)** — Ô tích "In HDSD" sau "Xem trước khi in": tích thì tự in Hướng dẫn sử dụng thuốc (Mps000099) sau mọi lần lưu thành công, chế độ in theo "Xem trước khi in"; nhớ theo máy (ControlState `chkInHDSD`). Hàng nút dưới: bỏ `emptySpaceItem1` (47px), thu "Ký đơn nhà thuốc" 185→154px; các nút từ "Hủy xuất" giữ nguyên vị trí. In HDSD từ menu In không đổi. Sửa kèm `InitControlState`: `SPIN_BASE_VALUE` đọc bằng `Convert.ToDecimal` (lưu bằng `decimal.ToString()`, `ToInt64` ném lỗi với giá trị lẻ làm dừng nạp các khóa sau) và trả cờ `isNotLoadWhileChangeControlStateInFirst` trong catch. |
| 02/10/2026 | nampp | **Việc 57853 (chỉ enable khi đã lưu)** — Nút "Đính kèm đơn" chỉ enable khi phiếu xuất đã lưu (EDIT + có phiếu); phiếu chưa lưu disable. `moduleAction` đổi từ auto-property sang property có setter gọi `RefreshAttachPrescriptionButtonState` (về ADD thì bỏ `attachExpMests`). Bỏ chế độ giữ tệp tạm trên màn Xuất bán (gỡ hỏi xác nhận ở Mới / Đơn mới, giải phóng ảnh khi đóng tab). Bán nhiều bệnh nhân → cảnh báo đính kèm tại Danh sách xuất. |
| 02/10/2026 | nampp | **Việc 57853 (tên văn bản mặc định)** — Tên mặc định đổi thành **"Đơn thuốc ngoại viện của mã phiếu xuất {EXP_MEST_CODE}"** (key `frmExpMestAttachFile.DefaultDocumentName` có `{0}`). Phiếu đã lưu: điền sẵn theo mã phiếu. Phiếu chưa lưu: ô tên để trống + gợi ý (`...txtDocumentName.NullText.Pending`), lưu phiếu xong tên được dựng theo mã của TỪNG phiếu (`ExpMestAttachFileProcessor.GetDefaultDocumentName`); người dùng tự gõ tên thì giữ nguyên. |
| 02/10/2026 | nampp | **Việc 57853 (gắn đúng mã hồ sơ + mã xuất)** — Tài liệu EMR: `TreatmentCode` = `TDL_TREATMENT_CODE` (bán vãng lai không có hồ sơ → `EXP_MEST_CODE`), vẫn `IsOutsideTreatment = true`; `HIS_CODE` thêm khối chuẩn `|EXP_MEST_CODE:x|EXP_STOCK:y|REQ_DEPT:z` để EMR (`HisCodeStockParser`) ghi cột `EMR_DOCUMENT.EXP_MEST_CODE` / `EXP_MEDI_STOCK_CODE` / `REQ_DEPARTMENT_CODE` (trước đây không có `|` nên các cột này NULL). Danh sách đơn + đánh dấu lọc `TREATMENT_CODEs` = mã hồ sơ + mã phiếu rồi khớp theo `EMR_DOCUMENT.EXP_MEST_CODE` (fallback tách từ `HIS_CODE`) → tài liệu tạo trước đó vẫn nhận ra. |
| 29/09/2026 | khainq | **Việc 57853** — Đính kèm đơn thuốc (tệp/ảnh chụp) trên phiếu xuất bán: nút "Đính kèm đơn" (runtime, gated config `HIS.Desktop.Plugins.ExpMestSaleCreate.AttachPrescription.IsEnable` + `MOS.HAS_CONNECTION_EMR`); giữ tệp tạm khi chưa lưu, tự đính kèm sau lưu thành công (`ProcessSave` → `ProcessAttachPrescriptionAfterSave`); phiếu đã có → danh sách đơn đính kèm. Partial mới `UCExpMestSaleCreate___AttachPrescription.cs`; ProjectReference thư viện mới `HIS.Desktop.Plugins.Library.ExpMestAttachFile`; thêm reference `DevExpress.Images.v15.2`. Script: `docs/SQL_57853_ExpMestSaleAttachPrescription.sql`. |
| 29/08/2026 | nampp | Việc 3082 **v3.2** (chốt DANGTH): **bỏ key** `SaveSignPrintAutoExport` và **bỏ checkbox "Xuất HĐĐT"** riêng (Designer/ControlState); nút `btnSaveSignPrint` luôn hiện, **enable theo `chkCreateBill` ("Xuất biên lai/hóa đơn") && `btnSavePrint.Enabled`**; gỡ khóa `chkCreateBill` (08/08) → code gốc; `OpenMedicineSaleBillAutoSignPrint` truyền marker `AUTO_ISSUE_EXISTING_BILL` + `TRANSACTION_ID=` (bill đã tạo lúc lưu), QR bỏ qua, không bill → `AUTO_SAVE_SIGN_PRINT`. Hàng đáy: emptySpaceItem1 47, Ký đơn 185, Xem trước 130. V2: gỡ khóa `chkExp` → không còn thay đổi. |
| 26/08/2026 | nampp | Việc 3082 v3.1: nút mới "Lưu ký in (Ctrl E / F11)" enable theo checkbox "Xuất HĐĐT" riêng + key; fix `Enabled` hiệu dụng (WaitingManager khóa form cha lúc load) bằng guard `this.Enabled` + `EnabledChanged/VisibleChanged` UC + `BeginInvoke`. |
| 25/08/2026 | nampp | Việc 3082 v3: checkbox "In" tại màn Xuất bán, "Lưu in" mở form hóa đơn tự động; partial `UCExpMestSaleCreate___SaveSignPrintInvoice.cs`; tạo tài liệu module. |
| 08/08/2026 | nampp | Việc 3082: khóa checkbox "Xuất biên lai/hóa đơn" khi key bật (đã gỡ 29/08); gỡ entry `licenses.licx` rỗng trong csproj. |
| (trước 2026) | team | Việc 36371 tạo chức năng Xuất bán; 38766 sửa xuất bán; các sửa POS/QR/2 sổ… |

## 9. Test Cases

### Lưu / Lưu in
- [ ] Kê thuốc đủ tồn → Lưu (F5): phiếu tạo, trạng thái theo cấu hình kho; nút Hủy xuất enable khi phiếu HOÀN THÀNH.
- [ ] Lưu in (F9): in phiếu xuất bán / hóa đơn biên lai giấy như cũ (có hoặc không tick "Xuất biên lai/hóa đơn").
- [ ] Kê vượt tồn khả dụng: bị chặn khi thêm dòng.

### Việc 3082 v3.2 — nút "Lưu ký in"
- [ ] Mở màn, chưa tick "Xuất biên lai/hóa đơn": nút "Lưu ký in" hiện nhưng **disable**.
- [ ] Tick → enable ngay; bỏ tick → disable ngay; sau khi lưu (Lưu in disable) → nút disable; Mới → theo tick. Đóng mở lại màn (checkbox nhớ ControlState) → nút theo.
- [ ] Tick + Lưu ký in (Ctrl E / F11), đủ tồn: phiếu + bill tạo (Mã giao dịch hiện), không in phiếu xuất bán; form Xuất hóa đơn hiện, tự phát hành HĐĐT + thực xuất + in rồi đóng; màn khóa Lưu/Lưu in/Lưu ký in.
- [ ] Tick + Lưu ký in, thiếu tồn: popup thiếu; bill có, HĐĐT không; form đóng.
- [ ] Lưu ký in nhưng validate màn fail (thiếu BN…): không lưu; bấm Lưu (F5) ngay sau đó KHÔNG mở form hóa đơn (flag đã reset).
- [ ] Hình thức QR + Lưu ký in: lưu + module QR như cũ, không mở form tự động.
- [ ] Nhiều bệnh nhân 1 lượt lưu: form mở lần lượt từng bill.
- [ ] Key `Show_MedicineSaleBill` = 1 + Lưu ký in: form chỉ mở 1 lần (chế độ tự động).

### Việc 57853 — Đính kèm đơn thuốc
- [ ] Config `...AttachPrescription.IsEnable` = 0 (mặc định) hoặc `MOS.HAS_CONNECTION_EMR` ≠ 1 → KHÔNG có nút "Đính kèm đơn", hàng nút như cũ.
- [ ] Bật config → có nút "Đính kèm đơn" bên trái "Hủy xuất", không che/chia nút khác ở 1366x768.
- [ ] Phiếu mới chưa lưu → nút "Đính kèm đơn" **disable**; Lưu thành công → nút enable; bấm → Danh sách đơn của phiếu vừa lưu → Đính kèm mới → chụp 2 ảnh → Lưu → 1 tài liệu EMR 2 trang, tên "Đơn thuốc ngoại viện của mã phiếu xuất <mã>" (kịch bản 1).
- [ ] Chọn tệp > dung lượng config hoặc sai định dạng → cảnh báo tên tệp, không nhận; tệp hợp lệ cùng lượt vẫn nhận (kịch bản 2).
- [ ] Sau lưu (EDIT) → nút mở Danh sách đơn đính kèm của phiếu; bổ sung thêm lần 2 → danh sách có 2 dòng.
- [ ] Phiếu Hoàn thành / đã thanh toán / xác nhận nợ → danh sách hiện dòng cảnh báo; nút Xóa xám, bấm → thông báo không được xóa (kịch bản 4); Xem/In và Đính kèm mới vẫn dùng được.
- [ ] Bán nhiều bệnh nhân 1 lượt lưu → bấm "Đính kèm đơn" → cảnh báo đính kèm tại Danh sách xuất, không tạo tài liệu.
- [ ] Sau lưu bấm Mới / Đơn mới → nút disable lại; Lưu thất bại → nút vẫn disable.
- [ ] Mở sửa phiếu từ Danh sách xuất / tìm theo mã đơn đã có phiếu → nút enable ngay.
- [ ] Tài khoản không có quyền module Camera → bấm Chụp ảnh báo thông báo, không lỗi.

### Việc 56651 — Ô tích "In HDSD"
- [ ] Mở màn: ô "In HDSD" ngay sau "Xem trước khi in", chưa tích, có tooltip; 1366x768 không mất chữ hàng nút.
- [ ] Không tích + Lưu in: chỉ in phiếu xuất bán như cũ.
- [ ] Tích "In HDSD" + tích "Xem trước khi in" + Lưu in: xem trước phiếu xuất bán → đóng → xem trước HDSD đúng thuốc của phiếu.
- [ ] Bỏ tích "Xem trước khi in" + Lưu in: in thẳng phiếu xuất bán rồi HDSD, không mở màn nào.
- [ ] Tích "Xuất biên lai/hóa đơn" + Lưu in: in hóa đơn/biên lai rồi HDSD.
- [ ] Lưu (Ctrl S / F5): chỉ in HDSD. Lưu ký in: HDSD rồi luồng HĐĐT 3082 như cũ.
- [ ] Lưu thất bại: không in HDSD. Nhiều bệnh nhân 1 lượt: mỗi bệnh nhân 1 bản HDSD.
- [ ] Đóng mở lại màn / tắt client: ô giữ trạng thái; máy khác chưa tích vẫn chưa tích. Menu In ▸ Hướng dẫn sử dụng thuốc như cũ.

### Việc 57699 — Form DS (F3) hiện thuốc/vật tư đã kê
- [ ] Tích đơn có thuốc + vật tư: dòng bệnh nhân như cũ, ngay dưới là bảng thuốc/vật tư (Tên "1 - ...", Mã thuốc, Hàm lượng, Hoạt chất, Số lượng, ĐVT) khớp đơn kê, mở sẵn.
- [ ] Đơn kê 1 thuốc 2 dòng: hiện 1 dòng, số lượng cộng. Đơn không có thuốc/vật tư: nút "+" mờ, bấm không mở. Từ 10 đơn trở lên: STT hiện đủ số.
- [ ] Bấm "−" → chỉ còn dòng bệnh nhân; tích thêm đơn khác → dòng đó vẫn thu gọn, đơn mới mở sẵn; "+" → hiện lại.
- [ ] Xóa dòng rồi tích lại: hiện lại, mở sẵn. Mất mạng khi tích: dòng vẫn thêm, chưa có chi tiết; tích tiếp thì tự lấy lại.
- [ ] Đồng ý: màn Xuất bán nhận đúng các đơn, giá/tồn như cũ. Đóng form không Đồng ý: không giữ tồn.
