# Đồng Bộ Danh Sách Ca Bệnh Truyền Nhiễm (ECDS) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | `HIS.Desktop.Plugins.InfectiousDiseaseSyncList` |
| Loại | Form (kế thừa `HIS.Desktop.Utility.FormBase`) |
| Mục đích | Danh sách ca bệnh truyền nhiễm + **đồng bộ hàng loạt** lên cổng ECDS; mở form chi tiết để xem/sửa từng ca |
| Cặp với | `HIS.Desktop.Plugins.InfectiousDiseaseReport` (form chi tiết 5 tab) |
| Mô hình tham chiếu | `KskSyncList` (đồng bộ danh sách) + `EnterKskInfomantion` (master → detail) |
| Ngày tạo | 25/07/2026 |

## 2. Quy Trình Nghiệp Vụ

```
Mở từ menu → frmInfectiousDiseaseSyncList
  Load → EcdsConfigCFG.LoadConfig → SearchList()   (nạp danh sách điều trị)
  Tìm kiếm (Mã ĐT / Tên BN / Từ-Đến ngày) → LoadListPaging (V_HIS_TREATMENT, ucPaging)
  ┌─ Chọn nhiều ca (checkbox) → "Đồng bộ lên cổng (N)"
  │     → BackgroundWorker: EnsureLogin (1 lần) → lặp DayCaBenh → thu EcdsSyncResultADO
  │     → frmEcdsSyncResult (Tổng / Đã đẩy / Lỗi) → SearchList refresh
  └─ Xem/Sửa (double-click) → ShowModule("...InfectiousDiseaseReport", HIS_TREATMENT, RefeshReference)
        → đóng form chi tiết → RefeshReference → SearchList refresh
```

## 3. EFMODEL / API

| Đối tượng | Loại | Dùng cho |
|-----------|------|----------|
| `api/HisTreatment/GetView` (`HisTreatmentViewFilter`, `V_HIS_TREATMENT`) | HIS | Danh sách y lệnh/điều trị |
| `/api/fast/v1/auth/login` | ECDS | Đăng nhập lấy token |
| `/api/fast/v1/ca-benh/cap-nhat-nhieu` (hoặc lặp `/cap-nhat`) | ECDS | **Đồng bộ hàng loạt** |
| `/api/fast/v1/ca-benh/danh-sach` | ECDS | **Đối soát với cổng** (tránh trùng) |
| `/api/fast/v1/danh-muc/benh` | ECDS | Map ICD→ID bệnh khi build DTO |
| `api/HisEcdsDiseaseCase/UpdatePushResultList` | MOS | **Lưu kết quả đẩy hàng loạt** vào HIS |

> Chi tiết bản đồ API↔chức năng đầy đủ (2 plugin): xem §23 trong `docs/HIS.Desktop.Plugins.InfectiousDiseaseReport.md`.

## 4. UI Layout

```
+-----------------------------------------------------------+
| [Mã ĐT] [Tên BN] [Từ ngày][Đến ngày] [Tìm kiếm]           |
+-----------------------------------------------------------+
| ☑ | Mã ĐT | Mã BN | Bệnh nhân | ICD                       |
|   grid V_HIS_TREATMENT (checkbox multi-select)            |
+-----------------------------------------------------------+
| [Đồng bộ lên cổng (N)]  [☑ Tự động đẩy mỗi [5] phút]  [trạng thái auto] |
| [« 1 2 3 »]  paging                                       |
+-----------------------------------------------------------+
| [Xem/Sửa chi tiết] [Đối soát với cổng] [Đóng]            |
+-----------------------------------------------------------+
```

Dialog kết quả `frmEcdsSyncResult`: Tổng · Đã đẩy · Lỗi + grid (dòng lỗi đỏ). Ảnh: `ecds-ui-synclist.png`.

### Tự động đẩy (Timer)
- Checkbox **"Tự động đẩy mỗi [N] phút"** + label trạng thái trên thanh đồng bộ.
- `System.Windows.Forms.Timer`: mỗi N phút → reload trang hiện tại → đẩy các ca **chưa auto-đẩy trong phiên** (`HashSet autoAttemptedIds` → mỗi ca auto tối đa 1 lần/phiên, tránh trùng/spam trên cổng).
- Chạy nền `BackgroundWorker` im lặng qua `RunSyncForRows(rows, silent:true)` (dùng chung với đẩy tay) — không popup, chỉ cập nhật label + `LogAction`.
- Nhớ trạng thái checkbox + số phút qua **ControlState** (`moduleLink` = plugin ID); dừng/giải phóng Timer trong `ProcessDisposeModuleDataAfterClose`.
- Guard mỗi tick: đang bật, ECDS đã cấu hình, không có phiên đẩy đang chạy (`isSyncing`).

## 5. Dependencies

| Loại | Mục |
|------|-----|
| Inter-plugin | Mở `HIS.Desktop.Plugins.InfectiousDiseaseReport` (`PluginInstanceBehavior.ShowModule`, args: `HIS_TREATMENT`, `RefeshReference`) |
| Tầng ECDS (nhân bản) | `EcdsApiWorker`, `EcdsTokenStore`, `EcdsCatalogCache`, `DiseaseCaseMapper`, `EcdsConfigCFG`, DTO, `EnumEcds` |
| Project ref | Common, ApiConsumer, Controls.Session, ConfigApplication, HisConfig, Location, ModuleExt |

## 6. Cấu Trúc Thư Mục

```
HIS.Desktop.Plugins.InfectiousDiseaseSyncList/
├── InfectiousDiseaseSyncListProcessor.cs
├── ModuleLinkString.cs · EnumEcds.cs
├── InfectiousDiseaseSyncList/  (Interface · Factory · Behavior)
├── MainForm/frmInfectiousDiseaseSyncList.* (form + __Process)
├── SyncResult/frmEcdsSyncResult.cs
├── ADO/ · Config/ · Worker/ · Resources/ (tầng ECDS dùng chung)
└── Properties/AssemblyInfo.cs
```

## 7. TODO
- Lọc grid **chỉ ICD bệnh truyền nhiễm**; cột **trạng thái đẩy** (cần bảng đối soát `HIS_ECDS_DISEASE_CASE` + API).
- **Đối soát với cổng** (`ca-benh/danh-sach`).
- Đẩy dùng batch `cap-nhat-nhieu` thay vì lặp từng ca.
- Tự động đẩy: hiện lọc theo phiên (`autoAttemptedIds`); khi có **cột trạng thái đẩy** thật (từ `HIS_ECDS_DISEASE_CASE`) nên lọc "chưa đẩy" theo DB thay vì HashSet phiên.
- ~~Tự động đẩy (Timer)~~ — ĐÃ LÀM (27/07/2026).

## 8. Changelog
| Ngày | Người | Thay đổi |
|------|-------|----------|
| 25/07/2026 | nampp | Tách plugin danh sách + đồng bộ từ InfectiousDiseaseReport thành project riêng |
| 25/07/2026 | nampp | Nối API lưu kết quả đẩy vào HIS: `PersistPushResults` → `api/HisEcdsDiseaseCase/UpdatePushResultList` (§21) sau khi đồng bộ hàng loạt (thêm `HisRequestUriStore` + `HisEcdsPushResultADO`). |
| 27/07/2026 | nampp | **Tự động đẩy (Timer)**: checkbox "Tự động đẩy mỗi N phút" + label trạng thái; `Timer` reload trang + đẩy ca chưa auto-đẩy trong phiên (`HashSet` chống trùng); refactor `SyncSelected` → `RunSyncForRows(rows, silent)` dùng chung; ControlState nhớ trạng thái; dừng Timer khi đóng. Thêm ProjectRef `HIS.Desktop.Library.CacheClient` + partial `__AutoPush.cs`. |
| 27/07/2026 | nampp | **Chuyển Form → UserControl** + đồng bộ UX theo `KskSyncListQD831`: (1) cột **Trạng thái đẩy** tô màu cam/xanh/đỏ (đối soát `api/HisEcdsDiseaseCase/Get` theo danh sách TREATMENT_ID — best-effort); (2) **combo lọc trạng thái** (Tất cả/Chưa/Đã/Thất bại) + ô lọc **mã BN** (`PATIENT_CODE__EXACT`) và **mã điều trị** (`TREATMENT_CODE__EXACT`); (3) **cột thao tác Xem/Đẩy** từng dòng (`RowCellClick`). Processor→`MODULE_TYPE_ID__UC`, Behavior trả UC, đổi tên file `frm*`→`UC*`, thêm ADO `EcdsSyncGridRowADO` + `HisEcdsReconcileFilterADO` + URI `HIS_ECDS_GET`. Bỏ nút Đóng (UC), Timer dừng trong `Dispose`. |
| 27/07/2026 | nampp | **Backend MOS đã ship** → thay placeholder bằng **type MOS thật**: `ReconcilePushState` dùng `MOS.Filter.HisEcdsDiseaseCaseViewFilter { TREATMENT_CODES }` → `GetView` `V_HIS_ECDS_DISEASE_CASE` (đọc `PUSH_STATE` + `ECDS_CASE_CODE` + lưu map `caseIdByTreatment`). `PersistPushResults` dùng `MOS.SDO.HisEcdsPushResultSDO` (khóa theo ID bản ghi từ đối soát; ca chưa có bản ghi → bỏ qua + log, cần tạo qua form chi tiết). Thêm ref `MOS.SDO` + URI `HIS_ECDS_GET_VIEW`; xóa placeholder `HisEcdsReconcileFilterADO` + `HisEcdsPushResultADO`. |
| 28/07/2026 | nampp | **UI không tạo ở runtime**: dời TOÀN BỘ code dựng giao diện (`BuildUi/AddLabel/AddGridCol/NewDate`) + khai báo control vào `UCInfectiousDiseaseSyncList.Designer.cs` (`InitializeComponent`); constructor KHÔNG còn gọi `BuildUi()`; trim khai báo control + `#region Build UI` khỏi `UC.cs`/`__Process.cs` (chỉ giữ logic/data/event). Bố cục giữ nguyên. |
| 03/09/2026 | khainq | **Khớp DTO + đẩy danh sách theo Swagger**: viết lại `EcdsDiseaseCaseDto` **camelCase** + `maIcd10Benh` (mã ICD-10 string) thay `BENHCHUANDOAN_ID`; ngày `dd/MM/yyyy` (`ToPortalDate`); `maGioiTinh`="M"/"F"; `maCoSoDieuTri`/`maDonViNguoiBaoCao`/`maXaPhuongQuanLy` từ config. **Đẩy danh sách dùng `/api/fast/v1/ca-benh/cap-nhat-nhieu`** (mảng thô): `RunSyncForRows` gọi `DayNhieuCaBenh` khi >1 ca (map kết quả theo `chiTiet[].chiSo`/`thanhCong`/`idCaBenh`), 1 ca vẫn `/cap-nhat` (có `maCaBenh`). Thêm typed result `DayNhieuKetQuaDto`/`ChiTietCaBenhDto` + log request/response. Thêm enum `EcdsTrangThaiCaBenh`/`EcdsTrangThaiLuu`. |
