# HIS.Desktop.Plugins.InfectiousDiseaseSyncList

Plugin **đồng bộ danh sách** ca bệnh truyền nhiễm lên cổng ECDS (mô hình `KskSyncList`).
Cặp với plugin chi tiết **`HIS.Desktop.Plugins.InfectiousDiseaseReport`**.

## Chức năng
- Tìm kiếm + grid `V_HIS_TREATMENT` + phân trang (`ucPaging`).
- Chọn nhiều ca bằng **checkbox** → **Đồng bộ lên cổng (N)** (BackgroundWorker, login 1 lần) → dialog kết quả `frmEcdsSyncResult`.
- **Xem/Sửa chi tiết** (hoặc double-click) → mở plugin chi tiết `InfectiousDiseaseReport` qua inter-plugin (`PluginInstanceBehavior.ShowModule`, truyền `HIS_TREATMENT` + `RefeshReference`).
- Callback refresh → nạp lại danh sách sau khi đóng form chi tiết.

## Quan hệ 2 plugin (như KskSyncList ↔ EnterKskInfomantion)
```
[InfectiousDiseaseSyncList]  danh sách + đồng bộ hàng loạt
       │  Xem/Sửa (ShowModule + HIS_TREATMENT)
       ▼
[InfectiousDiseaseReport]   form chi tiết 5 tab (đẩy từng ca)
```

## Cấu trúc
- Root: `Processor`, `ModuleLinkString`, `EnumEcds`
- `InfectiousDiseaseSyncList/`: Interface · Factory · Behavior
- `MainForm/`: `frmInfectiousDiseaseSyncList.*` (list + search + paging + đồng bộ)
- `SyncResult/frmEcdsSyncResult.cs`: dialog kết quả
- `ADO/` · `Config/` · `Worker/` · `Resources/`: **nhân bản tầng ECDS dùng chung** (độc lập với plugin chi tiết) — `EcdsApiWorker`, `EcdsTokenStore`, `EcdsCatalogCache`, `DiseaseCaseMapper`, `EcdsConfigCFG`, DTO, Enum.

## CẦN LÀM (trong Visual Studio)
1. Thêm project vào `HIS.Desktop.sln`; kiểm tra path `Newtonsoft.Json`.
2. Xác minh property của `MOS.Filter.HisTreatmentViewFilter` (`TREATMENT_CODE__EXACT`, `TDL_PATIENT_NAME`, `IN_TIME_FROM/TO`).
3. Lọc grid **chỉ ICD bệnh truyền nhiễm**; cột **trạng thái đẩy** (cần bảng đối soát `HIS_ECDS_DISEASE_CASE` + API).
4. **Đối soát với cổng** (`btnReconcile`) và **tự động đẩy** (Timer).

## Cấu hình (HisConfigs — dùng chung với plugin chi tiết)
`ECDS.API.BASE_URL`, `ECDS.API.USERNAME`, `ECDS.API.PASSWORD`, `ECDS.API.MA_DON_VI`, `ECDS.API.MA_CO_SO_DIEU_TRI`, `ECDS.API.TIMEOUT_SECOND`.
