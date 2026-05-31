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

### Cấu hình
- `HisConfigCFG.IDENTITY_MATERIAL_OPTION` — bật flow truy xuất VT
- `HisConfigCFG.APPROVAL_OR_EXP_OR_IMP_LOGINNAME_OPTION` — quy tắc hủy duyệt theo người duyệt

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
