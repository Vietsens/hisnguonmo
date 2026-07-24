# Danh sách nhập (HisImportMestMedicine) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisImportMestMedicine |
| Loại | UserControl (UC) |
| Mục đích | Tra cứu, duyệt, xử lý phiếu nhập kho dược (NCC, BL, KK, DK, HM…). Cho phép xem chi tiết, sửa, hủy, duyệt, từ chối duyệt, hủy duyệt, thực nhập. |
| Người tạo | IVT |
| Ngày tạo | — |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở module Danh sách nhập từ menu phòng kho.
2. Nhập điều kiện lọc (mã, loại, trạng thái, khoảng ngày) → bấm Tìm kiếm.
3. Grid hiển thị các phiếu nhập tương ứng. Mỗi dòng có dải cột icon thao tác.
4. Tùy theo trạng thái (DRAFT/REQUEST/APPROVAL/IMPORT/REJECT) các icon được enable/disable.

### Sơ đồ trạng thái phiếu nhập
```
DRAFT ──► REQUEST ──► APPROVAL ──► IMPORT
            ▲              │
            └─── REJECT ◄──┘
```

### Quy tắc icon hành động (theo cột)
| Cột | FieldName | Enable khi |
|-----|-----------|-----------|
| Chi tiết | DETAIL_DATA_DISPLAY | Luôn |
| Sửa | EDIT | DRAFT/REJECT, đúng kho, đúng người tạo hoặc admin |
| Hủy | DISCARD_DISPLAY | DRAFT/REQUEST/REJECT, đúng người tạo hoặc admin |
| Duyệt | APPROVAL_DISPLAY | REQUEST, có quyền BtnApprove |
| Hủy duyệt | REQUEST_DISPLAY | APPROVAL, có quyền BtnHuyDuyet hoặc người duyệt |
| Từ chối | DIS_APPROVAL | REQUEST, có quyền BtnApprove |
| Thực nhập | IMPORT_DISPLAY | APPROVAL, có quyền BtnImport |
| Sửa NCC | EditNCC | NCC, đúng điều kiện |
| Tạo XK trả NCC | CreateExpNCC | Đã nhập NCC |
| Hủy thực nhập | DONE | IMPORT, có quyền BtnHuyThucNhap |
| Lịch sử hoạt động | EVENT_LOG_TYPE_ID | Luôn |
| **Tạo giao dịch chi tiền (mới — 42727)** | **REPAY_DISPLAY** | Icon **đen trắng** — Enable khi `REPAY_ID = null` **VÀ** thỏa 1 trong 2 điều kiện: **(A)** `IMP_MEST_TYPE_ID = BTL` (Bán Trả Lại = 15); **(B)** `IMP_MEST_TYPE_ID = KHAC` (= 7) **VÀ** có ít nhất 1 dòng thuốc/VT thuộc loại nguồn nhập `HIS_IMP_SOURCE.IMP_SOURCE_CODE = 'BN'` (Bệnh nhân mua thuốc trả lại). |
| **In phiếu hoàn ứng (mới — 42727)** | **PRINT_REPAY_DISPLAY** | Icon **màu** — Phiếu nhập **có REPAY_ID** (đã tạo giao dịch chi tiền) |

### Đính kèm file hóa đơn/chứng từ (việc 42244 — cập nhật thiết kế v1.3)
Mục **"Đính kèm file"** trong menu chuột phải (`gridViewImportMestList_PopupMenuShowing`), hiển thị khi config `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE = 1` **và** đang chọn đúng 1 phiếu (không multi-select).

**Thiết kế v1.3:** chuột phải **mở màn hình "Danh sách tài liệu đính kèm"** (`frmImpMestAttachList`) — KHÔNG mở thẳng form đính kèm. Mọi thao tác Xem/Xóa/Đính kèm mới thực hiện trên màn hình này (theo pattern "Danh sách văn bản" của EMR — `EMR.Desktop.Plugins.EmrDocumentList.UCEmrDocumentList`).
```
Chuột phải phiếu → "Đính kèm file" → OpenAttachFile(impMest)
  HIS_CODE = "{MaSite} IMP_MEST_CODE:{IMP_MEST_CODE} DOCUMENT_NUMBER:{DOCUMENT_NUMBER}"
    MaSite = HIS.Desktop.Utility.StringUtil.CustomerCode (config HIS.Desktop.VPLUS_CUSTOMER_INFO)
  → frmImpMestAttachList(hisCode, IMP_MEST_CODE, loginName, roomId, FillDataImportMestList).ShowDialog()
      Grid: STT · Xem · Xóa (nút ICON) · Tên văn bản · Thời gian đính kèm · Người đính kèm · Thời gian sửa · Người sửa  (cột "Loại" ẩn)
      Load : api/EmrDocument/GetView (HIS_CODE__EXACT + DOCUMENT_TYPE_ID=IMPAT + IS_DELETE=false, order CREATE_TIME desc)
      - Đính kèm mới → frmImpMestAttachFile(hisCode, IMP_MEST_CODE, loginName, null); IsSaved → refresh
      - Xem  → api/EmrDocument/DownloadFile (ID, IsMerge) → ghi PDF tạm → SignLibraryGUIProcessor.ShowPopup(file, InputADO)
      - Xóa  → xác nhận → api/EmrDocument/Delete (documentId) → refresh
```
- `frmImpMestAttachList` (mới, v1.3) là màn hình danh sách; `frmImpMestAttachFile` (clone từ `EmrDocument.frmAttackFile`) chỉ còn nhiệm vụ **Đính kèm mới** (Loại/Tên/Nhóm văn bản, Scan/Chụp ảnh/2 mặt/Chọn file). Đã gỡ toàn bộ code "tài liệu đã lưu" khỏi form này; form trả cờ `IsSaved` khi lưu thành công.
- Quyền Xóa: **mọi người dùng** (không gate theo người đính kèm). **Không còn nút "Sửa"** (đã bỏ theo yêu cầu — sửa = xóa rồi đính kèm mới).
- Lưu (Ctrl S): gộp file thành 1 PDF → `api/EmrDocument/CreateByTdo` (`DocumentTDO.HisCode = HIS_CODE`, `TreatmentCode = IMP_MEST_CODE`, `IsOutsideTreatment = true`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_IMP_MEST | View | Phiếu nhập (load grid). **Cần Gencode lại** sau backend bổ sung cột `REPAY_ID` (việc 42727) |
| V_HIS_IMP_MEST_1 | View | Phiếu nhập detail (kiểm tra IS_BLOOD khi sửa) |
| V_HIS_EXP_MEST | View | Phiếu xuất bán gốc — đọc `TOTAL_PRICE` + `TDL_TREATMENT_ID` để tự điền vào TransactionRepay |
| HIS_IMP_MEST | Table | Update status / Delete |
| HIS_IMP_MEST_TYPE | Table | Loại phiếu nhập (NCC, BL, KK, DK, HM…) |
| HIS_IMP_MEST_STT | Table | Trạng thái phiếu nhập |
| V_HIS_MEDI_STOCK | View | Kho hiện tại của phòng |
| V_HIS_ROOM | View | Phòng làm việc |
| V_HIS_BID | View | Gói thầu |
| ACS_CONTROL | Table | Kiểm tra quyền nút |
| V_HIS_CASHIER_ROOM | View | Tra cứu phòng thu ngân theo phòng kho hiện tại (luồng 42727) |
| EMR_DOCUMENT / V_EMR_DOCUMENT | Table/View | Tài liệu đính kèm — lưu `HIS_CODE` (việc 42244) |
| EMR_DOCUMENT_TYPE | Table | Loại văn bản; cần bản ghi CODE = `IMPAT` — "Đính kèm phiếu nhập" (việc 42244) |
| EMR_DOCUMENT_GROUP | Table | Nhóm văn bản (combo) (việc 42244) |
| EMR_ATTACHMENT | Table | File đính kèm — URL trên FSS (việc 42244) |

## 4. UI Layout

### Sơ đồ
```
+----------------------------------------------------------+
| [Bộ lọc trái] | [Grid danh sách phiếu nhập]              |
| - Mã, Sub      |  STT, Chi tiết, Sửa, Hủy, Duyệt,         |
|   code         |  Hủy duyệt, Thực nhập, Sửa NCC,          |
| - Khoảng ngày  |  Tạo XK NCC, Lịch sử, **Tạo GD chi tiền**|
| - Loại, Trạng  |  Mã phiếu, Mã sub, Kho, NCC, Khoa…       |
|   thái                                                    |
| - Thuốc/VT     |  4 cột audit: Tạo, Người tạo, Sửa, ...   |
+----------------------------------------------------------+
| [Phân trang ucPaging]                          [Xuất XLS]|
+----------------------------------------------------------+
```

### Repository items mới (việc 42727)
- `repositoryItemButtonRepayEnable` — icon enable
- `repositoryItemButtonRepayDisable` — icon disable
- `gridColumnRepay` — cột icon, FieldName = `REPAY_DISPLAY`, Fixed = Right, không có caption, width 24, ToolTip = "Tạo giao dịch chi tiền"

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách phiếu nhập | api/HisImpMest/GetView | MosConsumer | HisImpMestViewFilter |
| Lấy phiếu nhập detail (V1) | api/HisImpMest/GetView1 | MosConsumer | HisImpMestView1Filter |
| Update status (REQUEST/APPROVAL/REJECT) | api/HisImpMest/UpdateStatus | MosConsumer | HIS_IMP_MEST |
| Thực nhập | api/HisImpMest/Import | MosConsumer | HIS_IMP_MEST |
| Hủy phiếu | api/HisImpMest/Delete | MosConsumer | HIS_IMP_MEST |
| Hủy thực nhập | api/HisImpMest/CancelImport | MosConsumer | HIS_IMP_MEST |
| **Lấy phiếu xuất bán gốc (42727)** | **api/HisExpMest/GetView** | **MosConsumer** | **HisExpMestViewFilter (filter.ID = CHMS_EXP_MEST_ID)** |
| **Tải danh sách tài liệu đính (42244, v1.3)** | **api/EmrDocument/GetView** | **EmrConsumer** | **EmrDocumentViewFilter (HIS_CODE__EXACT + DOCUMENT_TYPE_ID + IS_DELETE=false) → V_EMR_DOCUMENT** |
| **Xem tài liệu — tải nội dung (42244)** | **api/EmrDocument/DownloadFile** | **EmrConsumer** | **EmrDocumentDownloadFileSDO (ID, IsMerge) → SignLibraryGUIProcessor.ShowPopup** |
| **Lưu tài liệu (42244)** | **api/EmrDocument/CreateByTdo** | **EmrConsumer** | **DocumentTDO (HisCode, TreatmentCode, base64 PDF, IsOutsideTreatment)** |
| **Xóa mềm tài liệu (42244, v1.3)** | **api/EmrDocument/Delete** | **EmrConsumer** | **documentId (long)** |
| **Loại / nhóm văn bản (42244)** | **api/EmrDocumentType/Get, api/EmrDocumentGroup/Get** | **EmrConsumer** | **EmrDocumentTypeFilter / EmrDocumentGroupFilter** |

## 6. Dependencies

### Inter-Plugin (mở plugin khác)
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.ImpMestViewDetail | Click icon "Chi tiết" | `ImpMestViewDetailADO`, `DelegateSelectData` |
| HIS.Desktop.Plugins.ManuImpMestUpdate | Click icon "Sửa" (NCC/DK/KK/Khac/HM) | `long impMestId`, `RefeshReference` |
| HIS.Desktop.Plugins.ImportBlood | Click icon "Sửa" (HM với IS_BLOOD = 1) | `long impMestId`, `RefeshReference` |
| HIS.Desktop.Plugins.ApproveAggrImpMest | Click icon "Chi tiết" cho phiếu type TH | `long impMestId` |
| HIS.Desktop.Plugins.IdentityMaterialInformation | Click "Thực nhập" cho VT có truy xuất | `bool`, `long impMestId`, `DelegateImpTime`, `Module` |
| Inventec.Desktop.Plugins.EventLog | Click "Lịch sử hoạt động" | Phiếu hiện tại |
| **HIS.Desktop.Plugins.TransactionRepay (42727)** | **Click icon "Tạo giao dịch chi tiền"** | **`TransactionRepayADO` (đã set ImpMestId, AutoAmount, RepayReasonCode = "07"), `Module`** |
| **HIS.Desktop.Plugins.Camera (42244)** | **Nút "Chụp ảnh" trong frmImpMestAttachFile** | **`DelegateSelectData`** |

### Reference mới (việc 42244)
EMR.EFMODEL, EMR.Filter, EMR.TDO, EMR.SDO, EMR.URI, itextsharp, DevExpress.XtraPdfViewer/Pdf.Core/Drawing, WIA (COM), HIS.Desktop.Library.CacheClient, Inventec.Common.Controls.EditorLoader.

### Cấu hình
- `HisConfigCFG.IDENTITY_MATERIAL_OPTION` — bật flow truy xuất VT
- `HisConfigCFG.APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION` — quy tắc hủy duyệt theo người duyệt
- `HisConfigCFG.ALLOW_ATTACH_FILE` ← `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE` — bật mục "Đính kèm file" (mặc định OFF, việc 42244)

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| **Phiếu thu hoàn ứng (mới — 42727)** | **Mps000113** | MPS.MpsPrinter + Mps000113PDO + RichEditorStore (sao chép pattern từ TransactionList) | PhieuThuHoanUng |
| In gộp biên bản kiểm nhập từ NCC | Mps000505 | (đã có sẵn) | — |

### Cách triển khai In phiếu hoàn ứng (42727)
- Click icon cột `PRINT_REPAY_DISPLAY` hoặc menu chuột phải "In phiếu hoàn ứng"
- Plugin gọi `api/HisTransaction/GetView` với `filter.ID = impMest.REPAY_ID` để lấy V_HIS_TRANSACTION
- Build `MPS.Processor.Mps000113.PDO.Mps000113PDO(repay, patient, ratio, null, departmentTran, treatmentFee, transactions)`
- Gọi `MPS.MpsPrinter.Run(new PrintData(...))` với `PreviewType.PrintNow` hoặc `ShowDialog` theo config
- File implement: [UCHisImportMestMedicine__PrintRepay.cs](../HIS/Plugins/HIS.Desktop.Plugins.HisImportMestMedicine/UCHisImportMestMedicine__PrintRepay.cs)

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-09 | dangth2 | Việc 42727 — Thêm cột icon "Tạo giao dịch chi tiền" cho phiếu nhập có liên kết phiếu xuất bán gốc; mở plugin TransactionRepay với args ImpMestId + AutoAmount + RepayReasonCode "07" |
| 2026-05-14 | dangth2 | Việc 42727 (đọc lại PTTK) — Thêm cột thứ 2 "In phiếu hoàn ứng" (icon màu, enable khi phiếu có REPAY_ID), in MPS000113 theo pattern TransactionList; thêm menu chuột phải "Tạo giao dịch chi tiền" + "In phiếu hoàn ứng"; chuyển icon cột "Tạo GD" sang đen trắng (grayscale runtime); auto refresh grid sau khi đóng TransactionRepay để cập nhật trạng thái REPAY_ID |
| 2026-05-14 | dangth2 | Việc 42727 (theo tài liệu phân tích) — Bỏ check IMP_MEST_TYPE/CHMS_EXP_MEST_ID; icon "Tạo GD" enable cho **mọi phiếu nhập** chưa có REPAY_ID. Khi click: nếu phiếu có link CHMS/MOBA → auto-fill số tiền; nếu không → form mở trống, user nhập tay. Phù hợp với cả luồng C1 (Tìm phiếu xuất bán → tạo nhập thu hồi) và C2 (loại Khác + nguồn BN trả lại). |
| 2026-05-14 | dangth2 | Việc 42727 (chốt điều kiện enable) — Logic mới: enable khi REPAY_ID null **VÀ** (A) type=BTL hoặc (B) type=KHAC + có thuốc/VT với `HIS_IMP_SOURCE.IMP_SOURCE_CODE='BN'`. Pre-compute cache `_impMestIdsWithBNSource` mỗi lần `ImportMestPaging` để không spam API. Load `_bnMedicineIds`/`_bnMaterialIds` 1 lần khi UC khởi tạo qua `BackendDataWorker.Get<HIS_MEDICINE/MATERIAL>()`. |
| 2026-06-25 | tuanln | **Việc 42244** — Thêm "Đính kèm file" hóa đơn/chứng từ vào menu chuột phải danh sách nhập (gated config `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE`, mặc định OFF). Clone form `frmImpMestAttachFile` từ `EmrDocument.frmAttackFile` (Scan/Chụp ảnh/2 mặt/chọn file). HIS_CODE = `{MaSite} IMP_MEST_CODE:.. DOCUMENT_NUMBER:..` (MaSite = `StringUtil.CustomerCode`). Mặc định Loại văn bản = `IMP_MEST_ATTACH`; lưu qua `api/EmrDocument/CreateByTdo` (HisCode); tải & xem lại tài liệu đã đính theo HIS_CODE. Thêm reference EMR.*/itextsharp/PdfViewer/WIA/CacheClient/EditorLoader. |
| 2026-07-20 | tuanln | **Việc 42244 (cập nhật thiết kế v1.3)** — Chuột phải "Đính kèm file" **mở màn hình Danh sách tài liệu đính kèm** mới (`frmImpMestAttachList`) thay vì mở thẳng form đính kèm (pattern "Danh sách văn bản" / `UCEmrDocumentList`). Grid Xem/Sửa/Xóa + nút Đính kèm mới/Làm mới. **Xem** = `api/EmrDocument/DownloadFile` → ghi PDF tạm → `SignLibraryGUIProcessor.ShowPopup` (viewer toàn màn hình). **Sửa** = mở form chọn file thay thế → lưu bản mới → xóa mềm bản cũ (`api/EmrDocument/Delete`). **Xóa** = xác nhận → `api/EmrDocument/Delete`. Gỡ khỏi `frmImpMestAttachFile` toàn bộ code "tài liệu đã lưu" (`LoadExistingDocuments`/`DownloadAndPreviewExisting`/hậu tố `[đã lưu]`/chặn xóa); thêm cờ `IsSaved`. Quyền Sửa/Xóa: mọi người dùng. Build OK (msbuild VS2022, PostBuildEvent tắt). |
| 2026-07-23 | tuanln | **Việc 42244 (fix sau test)** — (1) **Hiệu năng**: đổi nạp danh sách từ `api/EmrDocument/Get` (chỉ HIS_CODE, ~10s khi viện ký nhiều) sang `api/EmrDocument/GetView` với `EmrDocumentViewFilter` lọc thêm `DOCUMENT_TYPE_ID = IMPAT` (chọn lọc, tránh quét toàn EMR_DOCUMENT). (2) **Văn bản đã xóa vẫn hiển thị**: `EmrDocumentFilter` không có `IS_DELETE`; chuyển GetView + `IS_DELETE=false` để ẩn bản đã xóa mềm. (3) **Nút Xem/Sửa/Xóa** đổi từ chữ sang **ICON** (gallery DevExpress `ImageResourceCache`: preview/edit/delete 16x16), có tooltip, fallback chữ nếu thiếu ảnh. (4) **Sửa** nạp sẵn Tên + Loại văn bản của bản cũ (overload `frmImpMestAttachFile` + prefill trong Load). Thêm reference `DevExpress.Images.v15.2`. Grid rebind sang `V_EMR_DOCUMENT`. |
| 2026-07-24 | tuanln | **Việc 42244 (fix test đợt 4)** — Lưới Danh sách tài liệu: **ẩn cột "Loại"** (mọi tài liệu đều PDF nên thừa); **chuyển cột "Người đính kèm" vào giữa 2 cột thời gian** (thứ tự: Tên văn bản · Thời gian đính kèm · Người đính kèm · Thời gian sửa · Người sửa). Chỉ đổi VisibleIndex/Visible trong Designer. Đặt `ColumnAutoWidth = true` (bỏ `Fixed=Left`) để các cột tự chia đủ chiều rộng lưới trên mọi độ phân giải (hết dư khoảng trắng bên phải). |
| 2026-07-23 | tuanln | **Việc 42244 (fix test đợt 3)** — **Bỏ nút "Sửa"** trên màn hình Danh sách tài liệu đính kèm (theo yêu cầu: không cần nữa; muốn sửa thì Xóa rồi Đính kèm mới). Gỡ cột `gcEdit`/`repoBtnEdit` + handler + `ReplaceDocument`/`SoftDeleteDocumentSilent` ở `frmImpMestAttachList`; revert overload prefill + field prefill ở `frmImpMestAttachFile` (không còn ai dùng). Chỉ còn Xem/Xóa/Đính kèm mới. |
| 2026-07-23 | tuanln | **Việc 42244 (fix test đợt 2)** — (1) **Mã loại văn bản**: DB thực tế dùng `DOCUMENT_TYPE_CODE = "IMPAT"` (không phải `IMP_MEST_ATTACH` như tài liệu cũ) → sửa hằng số ở `frmImpMestAttachFile` + `frmImpMestAttachList` thành `"IMPAT"` (nếu không form KHÔNG tự chọn được loại + bộ lọc DOCUMENT_TYPE_ID không áp được). Cập nhật script DB dùng `IMPAT`. (2) **Khóa Loại văn bản** = "Đính kèm phiếu nhập" (ReadOnly sau khi set mặc định) — luôn đúng loại, khớp bộ lọc. (3) **Nút Xem** đổi icon preview→**con mắt** dùng lại ảnh `repositoryItemButtonViewDetail.Buttons` trong `UCHisImportMestMedicine.resx` (ComponentResourceManager) cho giống lưới Danh sách nhập, fallback preview gallery. |

## 9. Test Cases — Việc 42727

### Hiển thị icon "Tạo giao dịch chi tiền" (đen trắng)
**Enable** khi `REPAY_ID = null` VÀ thỏa 1 trong:
- [ ] (A) Phiếu type = **BTL** (Bán Trả Lại, ID=15) → enable
- [ ] (B) Phiếu type = **KHAC** (ID=7) + có thuốc với `HIS_MEDICINE.IMP_SOURCE_ID` → `HIS_IMP_SOURCE.IMP_SOURCE_CODE = 'BN'` → enable
- [ ] (B) Phiếu type = **KHAC** + có vật tư với `HIS_MATERIAL.IMP_SOURCE_ID` → `HIS_IMP_SOURCE.IMP_SOURCE_CODE = 'BN'` → enable

**Disable** khi:
- [ ] Phiếu type = **KHAC** nhưng KHÔNG có thuốc/VT nguồn BN → disable
- [ ] Phiếu type khác BTL/KHAC (NCC, KK, DK, CK, TH, BCS, HM...) → disable
- [ ] Phiếu có `REPAY_ID > 0` → disable (chuyển sang icon "In phiếu" enable)

### Khi click icon "Tạo GD chi tiền"
- [ ] Phiếu có `CHMS_EXP_MEST_ID > 0`: tự đọc phiếu xuất bán gốc → auto-fill số tiền + mã điều trị
- [ ] Phiếu có `MOBA_EXP_MEST_ID > 0`: tương tự, dùng MOBA làm fallback
- [ ] Phiếu KHÔNG có cả 2 link: form Hoàn ứng mở với số tiền trống → user nhập tay
- [ ] Lý do hoàn ứng tự chọn record code "07" (Nhập lại xuất bán)

### Hiển thị icon "In phiếu hoàn ứng" (màu)
- [ ] Phiếu nhập có REPAY_ID > 0 → icon MÀU enable
- [ ] Phiếu nhập không có REPAY_ID → icon disable

### Click icon Tạo GD chi tiền
- [ ] Click → API `api/HisExpMest/GetView` được gọi với filter.ID = CHMS_EXP_MEST_ID
- [ ] Form TransactionRepay mở dialog
- [ ] Trường "Số tiền" tự điền bằng tổng từ V_HIS_EXP_MEST_MEDICINE + V_HIS_EXP_MEST_MATERIAL
- [ ] Combo "Lý do hoàn ứng" tự chọn record có code "07" — Nhập lại xuất bán
- [ ] Trường "Mã điều trị" lấy theo `TDL_TREATMENT_ID` của phiếu xuất gốc
- [ ] Người dùng vẫn có thể chỉnh sửa số tiền + lý do
- [ ] Sau khi đóng dialog → grid auto refresh, dòng vừa lưu chuyển: tắt icon "Tạo GD", bật icon "In phiếu"

### Click icon In phiếu hoàn ứng
- [ ] Click → API `api/HisTransaction/GetView` lấy V_HIS_TRANSACTION theo REPAY_ID
- [ ] Build PDO Mps000113 với: V_HIS_TRANSACTION + V_HIS_PATIENT + ratio BHYT + DepartmentTran + TreatmentFee + All transactions
- [ ] MpsPrinter.Run hiển thị preview / in luôn theo config `CheDoInChoCacChucNangTrongPhanMem`

### Menu chuột phải
- [ ] Right-click row có thể tạo hoàn ứng → menu hiện "Tạo giao dịch chi tiền"
- [ ] Right-click row đã có REPAY_ID → menu hiện "In phiếu hoàn ứng"
- [ ] Right-click row không liên quan → menu KHÔNG có 2 mục Repay
- [ ] Click menu "Tạo giao dịch chi tiền" → tương đương click icon Tạo GD
- [ ] Click menu "In phiếu hoàn ứng" → tương đương click icon In phiếu

### Save
- [ ] Khi nhấn "Lưu" trong TransactionRepay → API CreateRepay nhận thêm `IMP_MEST_ID = impMest.ID`
- [ ] Backend response thành công → REPAY_ID được ghi vào HIS_IMP_MEST

## 10. Test Cases — Việc 42244 (Đính kèm file)

### Hiển thị menu
- [ ] Config `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE` OFF (mặc định) → KHÔNG có mục "Đính kèm file".
- [ ] Config ON + chuột phải đúng 1 phiếu → có mục "Đính kèm file".
- [ ] Chọn nhiều phiếu (>1) hoặc chuột phải vùng trống → KHÔNG hiện mục "Đính kèm file".

### Màn hình Danh sách tài liệu đính kèm (frmImpMestAttachList — v1.3)
- [ ] Chọn "Đính kèm file" → mở màn hình danh sách (KHÔNG mở thẳng form đính kèm).
- [ ] Grid nạp đúng tài liệu của phiếu theo HIS_CODE; nhãn "N tài liệu" đúng số dòng.
- [ ] Cột: STT · Xem · Xóa · Tên văn bản · Thời gian đính kèm · Người đính kèm · Thời gian sửa · Người sửa (KHÔNG có cột "Sửa" và "Loại"; Người đính kèm nằm giữa 2 cột thời gian).

### Đính kèm mới
- [ ] Nút "Đính kèm mới" → mở frmImpMestAttachFile; Loại văn bản mặc định = IMPAT ("Đính kèm phiếu nhập"), **khóa không cho đổi**.
- [ ] Chọn file sai định dạng / quá lớn → không thêm vào lưới.
- [ ] Scan / Chụp ảnh có-không thiết bị → quét/mở camera hoặc báo lỗi tương ứng.
- [ ] Lưu (Ctrl S) → gộp PDF, `api/EmrDocument/CreateByTdo` (HisCode/TreatmentCode/IsOutsideTreatment) → đóng form → danh sách tự refresh (thêm 1 dòng).

### Xem
- [ ] Nút "Xem" → tải nội dung (`DownloadFile`), mở viewer toàn màn hình (SignLibrary) hiển thị PDF.
- [ ] Không tải được nội dung → báo "Không tải được nội dung tài liệu.".

### Xóa
- [ ] Nút "Xóa" → hộp thoại xác nhận; đồng ý → `api/EmrDocument/Delete` (IS_DELETE=1, IS_ACTIVE=0) → refresh, dòng biến mất.
- [ ] Chọn Không → không xóa.
- [ ] Mọi người dùng đều Xóa được (không gate theo người đính kèm).

## 11. Triển Khai — Script DB (BẮT BUỘC trước deploy, việc 42244)

Insert loại văn bản `IMPAT` vào `EMR_DOCUMENT_TYPE` (schema EMR). **Mã `DOCUMENT_TYPE_CODE` PHẢI = `IMPAT`** (code frontend khớp đúng mã này để mặc định + lọc; nếu site dùng mã khác thì form không tự chọn được loại). DBA điều chỉnh tên sequence/cột audit theo chuẩn site:

```sql
-- EMR schema — loại văn bản đính kèm hóa đơn/chứng từ phiếu nhập (việc 42244)
INSERT INTO EMR_DOCUMENT_TYPE
    (ID, DOCUMENT_TYPE_CODE, DOCUMENT_TYPE_NAME, IS_ACTIVE, IS_DELETE,
     CREATE_TIME, CREATOR, IS_ALLOW_DUPLICATE_HIS_CODE)
VALUES
    (SEQ_EMR_DOCUMENT_TYPE.NEXTVAL, 'IMPAT',
     N'Đính kèm phiếu nhập', 1, 0,
     TO_NUMBER(TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')), 'admin', 1);
COMMIT;
```

- `IS_ALLOW_DUPLICATE_HIS_CODE = 1`: cho phép 1 phiếu nhập có nhiều tài liệu đính kèm (cùng HIS_CODE).
- Config `MOS.HIS_IMP_MEST.ALLOW_ATTACH_FILE` mặc định = 0 (OFF). Viện bật theo cấu hình site — KHÔNG bật toàn hệ thống.
- File lưu trên FSS; `EMR_ATTACHMENT.URL` là đường dẫn truy cập qua FSS API (không lưu binary vào DB).
