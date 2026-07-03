# Kê đơn máu (HisAssignBlood) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisAssignBlood |
| Loại | Form (`frmHisAssignBlood` kế thừa `FormBase`) |
| Mục đích | Kê đơn máu cho 1 lần điều trị — chọn nhóm máu (ABO/RH), số lượng, ghi chú và lưu thành yêu cầu dịch vụ máu (`HIS_SERVICE_REQ.SERVICE_REQ_TYPE_ID = DONM`) + chi tiết yêu cầu máu (`HIS_EXP_MEST_BLTY_REQ`). |
| Trạng thái | Bảo trì / mở rộng |

## 2. Quy Trình Nghiệp Vụ

### Chế độ mở form (Tạo mới vs Sửa)
- **Mở từ nút "Kê đơn máu"** (BedRoomPartial, ExamServiceReqExecute, ExecuteRoom, PayClinicalResult, TrackingCreate, SurgServiceReqExecute) → caller chỉ truyền `AssignBloodADO`, KHÔNG truyền `HIS_SERVICE_REQ` → `_ServiceReqEdit == null` → form mở ở chế độ **TẠO MỚI** (`actionType = ActionAdd`), grid đơn trống.
- **Sửa từ Danh sách y lệnh** (`ServiceReqList`, right-click dòng đơn máu `ID__DONM` → Sửa) → caller truyền `AssignBloodADO` + `HIS_SERVICE_REQ` → `_ServiceReqEdit != null` → form mở ở chế độ **SỬA** (`actionType = ActionEdit`), `LoadServiceReqOld` nạp lại đơn cũ.

### Luồng chính
1. Chọn kho xuất (`cboMediStockExport_TabBlood`) → load danh sách máu theo kho.
2. Chọn chẩn đoán (CĐ chính/phụ) qua HIS.UC.Icd / SecondaryIcd.
3. Chọn 1 dòng máu trong grid bên trái (`gridControlBloodType__BloodPage`) → set vào input bổ sung.
4. Nhập **Số lượng**, **ABO**, **RH**, **Số lần đã truyền**, **Lưu ý bất thường** → bấm **Bổ sung (Ctrl A)** → dòng được thêm vào grid bên phải (`gridControlServiceProcess__TabBlood`, DataSource = `List<BloodTypeADO>` giữ trong `ListBloodTypeADOProcess`).
5. Bấm **Lưu (Ctrl S)** → gọi API tạo/cập nhật phiếu kê máu.

### Validation khi bổ sung 1 dòng (`dxValidProviderBoXung__MedicinePage`)
- `spinAmount__BloodPage` > 0 — bắt buộc nhập số lượng > 0 (cấm âm theo `EditMask = "######0;"`).
- `spinTransfusedNum` ≥ 0 — cấm âm (cùng EditMask), default = 0.
- `memoAbnormalNote` ≤ 1000 ký tự — vượt sẽ báo lỗi `LuuYBatThuongVuotQua1000KyTu`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ kê máu (`SERVICE_REQ_TYPE_ID = DONM`) |
| HIS_EXP_MEST_BLTY_REQ | Table | Chi tiết từng dòng máu (BLOOD_TYPE_ID, AMOUNT, BLOOD_ABO_ID, BLOOD_RH_ID, **TRANSFUSED_NUM**, **ABNORMAL_NOTE**) |
| V_HIS_EXP_MEST_BLTY_REQ_1 | View | Load lại chi tiết khi vào chế độ Edit (chứa `TRANSFUSED_NUM` + `ABNORMAL_NOTE`) |
| HIS_BLOOD_TYPE / V_HIS_BLOOD_TYPE | Table/View | Danh sách loại máu — base của `BloodTypeADO` |
| HIS_BLOOD_ABO | Table | Nhóm máu hệ ABO |
| HIS_BLOOD_RH | Table | Nhóm máu hệ Rh |
| V_HIS_PATIENT / V_HIS_TREATMENT | View | Thông tin bệnh nhân & điều trị |

### ADO bổ sung
- `BloodTypeADO` (kế thừa `V_HIS_BLOOD_TYPE`) — thêm các property nghiệp vụ: `AMOUNT`, `BLOOD_ABO_ID`, `BLOOD_RH_ID`, `PATIENT_TYPE_ID`, `PRICE`, `TOT_PRICE`, `IsOutParentFee`, và **`TRANSFUSED_NUM` (int?)**, **`ABNORMAL_NOTE` (string)** — lưu giá trị nhập tại vùng input bổ sung và truyền xuống `HIS_EXP_MEST_BLTY_REQ` khi save.

## 4. UI Layout

### Form chính (frmHisAssignBlood)
```
+--------------------------------------------------------------------------------+
| Ngày y lệnh | CĐ chính | CĐ phụ | Tờ ĐT | Người chỉ định | Đối tượng         |
| Kho xuất | Nhóm xử lý                                                          |
| [chkShowGroupBlood] Hiển thị nhóm máu, Rh         Mức độ:                       |
| [txtKeyword]                                                                    |
+-----------------------------------+---------------------------------------------+
| Grid máu theo kho (trái)          | Grid máu đã chọn (phải)                    |
|  Mã | Tên | Dung tích | Tồn        |  Mã | Tên | ĐT TT | Đơn giá | SL |        |
|                                   |  **Số lần đã truyền** | Thành tiền | ABO    |
|                                   |  RH | CP ngoài gói                          |
|                                   |                                             |
+-----------------------------------+                                             |
| Số lượng [spin] ABO [cbo] RH [cbo] Số lần đã truyền [spin] [Bổ sung Ctrl A]   |
| Lưu ý bất thường: [memoedit full width — max 1000 ký tự]                      |
+--------------------------------------------------------------------------------+
| [Tóm lược viện phí] [Lưu in (Ctrl I)] [Lưu (Ctrl S)] [In ▼] [Mới (Ctrl N)]    |
+--------------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Icd (Icd) | `panelControlIcd` | Chọn chẩn đoán chính |
| HIS.UC.SecondaryIcd (SecondaryIcd) | `panelControlSubIcd` | Chọn chẩn đoán phụ |
| HIS.UC.DateEditor (UCDate) | `pnlUCDate` | Ngày y lệnh |

## 5. API Endpoints

| Action | URI | Consumer | Filter / DTO |
|--------|-----|----------|--------------|
| Kê đơn máu mới | `RequestUriStore.HIS_SERVICE_REQ__BLOODPRESCREATE` (`/api/HisServiceReq/BloodPresCreate`) | MosConsumer | `PatientBloodPresSDO` (gồm `List<HIS_EXP_MEST_BLTY_REQ>` ExpMestBltyReqs có TRANSFUSED_NUM + ABNORMAL_NOTE) |
| Cập nhật đơn máu | `RequestUriStore.HIS_SERVICE_REQ__BLOOD_UPDATE` (`/api/HisServiceReq/BloodPresUpdate`) | MosConsumer | `PatientBloodPresSDO` (cùng cấu trúc, có `Id` = SERVICE_REQ_ID) |
| Load chi tiết khi Edit | `api/HisExpMestBltyReq/GetView1` (trả về `List<V_HIS_EXP_MEST_BLTY_REQ_1>` — chứa `TRANSFUSED_NUM`, `ABNORMAL_NOTE`) | MosConsumer | `HisExpMestBltyReqViewFilter` (EXP_MEST_IDs) |
| Load danh sách máu | `RequestUriStore.HIS_BLOOD__GETVIEW` (`/api/HisBlood/GetView`) | MosConsumer | — |

### Cấu trúc save
`SaveProcess(bool isPrintNow)` → `ProcessDataInputApiAssignBlood()` build `HisPrescriptionSDO.ExpMestBltyReqs`:
- Group `BloodTypeADO` theo `(PATIENT_TYPE_ID, ID)` → mỗi nhóm tạo 1 `HIS_EXP_MEST_BLTY_REQ`.
- Mỗi `HIS_EXP_MEST_BLTY_REQ`: `BLOOD_TYPE_ID`, `AMOUNT` (sum), `BLOOD_ABO_ID`/`BLOOD_RH_ID`/**`TRANSFUSED_NUM`/`ABNORMAL_NOTE`** lấy từ `firstBlood` trong group.

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.UC.Icd / SecondaryIcd / DateEditor | UC chẩn đoán & ngày y lệnh |
| MPS Processors | In phiếu (Mps000102…) |

## 7. Print

| Loại in | PrintTypeCode | Mô tả |
|---------|--------------|-------|
| Phiếu kê đơn máu | Mps000102 | In phiếu yêu cầu cấp máu |
| Phiếu hội chẩn (nếu cần) | — | `frmServiceDebateConfirmNew` kiểm tra biên bản hội chẩn trước khi save |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/05/2026 | tuanln | Bổ sung trường **Số lần đã truyền** (`TRANSFUSED_NUM`) và **Lưu ý bất thường** (`ABNORMAL_NOTE`) trên vùng input bổ sung của form `frmHisAssignBlood`: thêm `spinTransfusedNum` (cấm số âm, default 0), `memoAbnormalNote` (max 1000 ký tự); thêm cột "Số lần đã truyền" trong grid bên phải (`grcTransfusedNum__TabBlood`, `VisibleIndex = 6`). 2 trường lưu vào `BloodTypeADO` mỗi khi bấm Bổ sung và được gán vào `HIS_EXP_MEST_BLTY_REQ` khi gọi `BloodPresCreate` / `BloodPresUpdate`. Đổi load chi tiết khi Edit từ `V_HIS_EXP_MEST_BLTY_REQ` sang `V_HIS_EXP_MEST_BLTY_REQ_1` qua URI `api/HisExpMestBltyReq/GetView1` để đọc lại 2 trường mới. Đẩy hàng buttons xuống Y=665, tăng ClientSize form từ 694→729. |
| 11/06/2026 | tuanln | Fix lỗi: kê đơn máu xong mở lại **Kê đơn máu** thì grid bên phải mất hết dòng máu đã kê. Root cause (xác định qua log `LogSystem.txt`): khi mở từ nút "Kê đơn máu" (ExecuteRoom/ExamServiceReqExecute), caller KHÔNG truyền `HIS_SERVICE_REQ` nên `_ServiceReqEdit` luôn null → `LoadServiceReqOld` (hàm nạp máu đã kê) không bao giờ chạy → form luôn mở rỗng. Fix: thêm `LoadExistBloodServiceReqByTreatment()` — khi `_ServiceReqEdit` null, tự truy vấn đơn máu mới nhất của ca điều trị (`HIS_SERVICE_REQ` loại `ID__DONM`, lọc `TREATMENT_ID`, order `INTRUCTION_TIME` desc) rồi gọi `LoadServiceReqOld` nạp lại grid + chuyển `actionType = ActionEdit`. Kèm theo: trong `LoadServiceReqOld` đổi nguồn nạp chi tiết từ view `V_HIS_EXP_MEST_BLTY_REQ_1` (`GetView1`) sang view gốc `V_HIS_EXP_MEST_BLTY_REQ` (`GetView`) + lấy `TRANSFUSED_NUM`/`ABNORMAL_NOTE` từ bảng `HIS_EXP_MEST_BLTY_REQ` (`Get`, lọc `EXP_MEST_IDs`) merge theo `ID` — không phụ thuộc view `_1`. |
| 18/06/2026 | tuanln | Fix 2 lỗi của auto-load đơn máu cũ khi đơn thuộc **Kho máu Minh Tâm (external)** — chỉ sửa trong phạm vi auto-load (việc 45846), không đổi logic Minh Tâm (việc 37402 của tungpd): (1) **Form đơ ~40-60s khi mở**: auto-load đơn Minh Tâm chạy `PreloadMinhTamAboRhForOldOrder` → `CallMinhTamGetInventory` gọi đồng bộ (`.GetAwaiter().GetResult()`, timeout 30s) tới server `192.168.4.17:5268` không tới được trên test → chặn UI thread. Fix: thêm cờ `_isAutoLoadExistingOrder` (set trong `LoadExistBloodServiceReqByTreatment`, try/finally), bỏ qua `PreloadMinhTamAboRhForOldOrder` khi auto-load (ABO/RH dùng fallback list local). (2) **Đơn load lên rồi biến mất**: `FillDataToControlsForm` có `BeginInvoke(HandleMediStockChanged)` (init kho mặc định) chạy TRỄ sau khi Load đồng bộ xong → `HandleMediStockChanged` đặt `gridControlServiceProcess__TabBlood.DataSource = null` SAU khi `LoadServiceReqOld` đã bind lưới đơn (xác định qua log: `gridRowCount=2` rồi 32ms sau bị CLEAR với `isInitForm=False`). Fix: chặn lệnh trễ này khi `_ServiceReqEdit != null && ID > 0` (đang mở/sửa đơn đã có) — lúc đó `LoadServiceReqOld` đã set kho + load lưới trái + bind lưới đơn; tạo đơn mới vẫn init kho mặc định như cũ. |
| 29/06/2026 | tuanln | **Sửa luồng mở form**: gỡ bỏ auto-load đơn máu cũ (thêm ở việc 45846) vì sai nghiệp vụ — mở từ nút "Kê đơn máu" tự nạp đơn cũ của BN và chuyển sang chế độ Sửa. Yêu cầu đúng: mở "Kê đơn máu" = **Tạo mới**; chỉ Sửa khi vào từ **Danh sách y lệnh** (truyền `HIS_SERVICE_REQ`). Thay đổi: bỏ nhánh `else { LoadExistBloodServiceReqByTreatment(); }` trong `frmHisAssignBlood_Load`, xóa hẳn method `LoadExistBloodServiceReqByTreatment()` (dead code) và cờ `_isAutoLoadExistingOrder` (hoàn nguyên `PreloadMinhTamAboRhForOldOrder` về gọi trực tiếp). Luồng Sửa từ `ServiceReqList` (nhánh `_ServiceReqEdit != null` → `LoadServiceReqOld`) độc lập nên không ảnh hưởng — vẫn nạp đủ đơn cũ. |

## 9. Test Cases

### Bổ sung dòng máu
- [ ] Nhập đủ Số lượng > 0, ABO, RH → bấm Bổ sung → grid bên phải có dòng mới với cột "Số lần đã truyền" hiển thị giá trị spin (mặc định 0 nếu không nhập).
- [ ] Nhập "Số lần đã truyền" = âm → không cho nhập (mask "######0;").
- [ ] Nhập "Lưu ý bất thường" > 1000 ký tự → bấm Bổ sung → hiện thông báo "Lưu ý bất thường không được vượt quá 1000 ký tự", không thêm dòng.
- [ ] Sau khi bổ sung thành công → `spinTransfusedNum` reset về 0, `memoAbnormalNote` reset về rỗng.

### Sửa dòng đã bổ sung
- [ ] Double-click dòng trong grid bên phải → input load lại Số lượng/ABO/RH/Số lần đã truyền/Lưu ý bất thường của dòng đó.
- [ ] Sửa → bấm Bổ sung → dòng được cập nhật, giữ nguyên 2 trường mới.

### Lưu mới
- [ ] Bổ sung ≥ 1 dòng → Lưu → API `/api/HisServiceReq/BloodPresCreate` gửi `ExpMestBltyReqs` chứa `TRANSFUSED_NUM` + `ABNORMAL_NOTE`.

### Lưu sửa (Edit)
- [ ] Mở lại đơn đã lưu → form load 2 trường từ `V_HIS_EXP_MEST_BLTY_REQ_1` về `BloodTypeADO` → hiển thị đúng trên grid (cột Số lần đã truyền).
- [ ] Sửa 2 trường → Lưu → API `/api/HisServiceReq/BloodPresUpdate` gửi 2 trường mới.
