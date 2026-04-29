# HIS.Desktop.Plugins.ServiceExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceExecute |
| Loại | UC (UserControl) |
| Mục đích | Xử lý yêu cầu khám/cận lâm sàng/PTTT trong Phòng thực hiện — trả kết quả CDHA, Siêu âm, TDCN, PTTT cho bệnh nhân |
| Người tạo | IVT |
| Ngày cập nhật gần nhất | 29/04/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
1. Người dùng vào Phòng thực hiện (HIS.Desktop.Plugins.ExecuteRoom)
2. Mở Xử lý yêu cầu khám/cls/pttt → Chọn bệnh nhân
3. Bấm "Xử lý" → mở UCServiceExecute (plugin này)
4. Plugin load: thông tin BN, dịch vụ, máy, ekip, hình ảnh PACS, mô tả
5. Người dùng nhập kết quả, mô tả, đính kèm ảnh, ký số
6. Lưu → Cập nhật HIS_SERE_SERV/SERE_SERV_EXT/SERE_SERV_PTTT/SERE_SERV_SUIN
7. (Tùy chọn) In phiếu trả kết quả → MPS print template
```

### Tính năng "Giữ lại Customize Layout" (mới — 29/04/2026)

```
Lần đầu sử dụng:
  1. Vào UC → chuột phải vào LayoutControl → "Customize Layout..."
  2. Kéo thả các LayoutControlItem (panel ảnh, panel mô tả, panel ekip, ...)
  3. Đóng Customization Form (X)
  → AUTO-SAVE: layout tự lưu thành file XML vào
     ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/{layoutControlName}.xml
  (KHÔNG hỏi user chọn file — KHÔNG cần bấm nút Save trong Customization Form)
Lần mở kế tiếp (cùng máy):
  UC.Load → InitRestoreLayout đọc file XML → RestoreLayoutFromXml → giữ nguyên layout
  (Áp dụng cho TẤT CẢ bệnh nhân — không phụ thuộc treatment/sere_serv)
Đồng bộ cho toàn máy trạm:
  Admin copy file XML từ ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/ sang folder
  ModuleDesign tương ứng trên máy trạm khác (tương tự cơ chế deploy file design của BarManager)
Khôi phục layout mặc định khi user kéo sai:
  Phím tắt Ctrl+Shift+R trên UC → confirm dialog → nếu Yes:
    - Restore mọi LayoutControl về snapshot designer GỐC (lưu trong RAM)
    - XÓA tất cả file XML trong ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/
  → Lần mở kế tiếp UC sẽ về layout designer gốc.
```

### Điều kiện nghiệp vụ

- Bật/tắt tính năng giữ layout: cần `HIS_CONFIG` key `HIS.Desktop.ApplyRestoreLayout.ModuleLinks` chứa `HIS.Desktop.Plugins.ServiceExecute` (CSV/SCSV ModuleLink)
- Layout tự lưu trên event `LayoutControl.CustomizationVisibleChanged` (khi user đóng Customization Form) — KHÔNG dùng MouseUp, KHÔNG hỏi file Save As
- File design lưu local mỗi máy trạm tại `{StartupPath}/ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/{layoutControlName}.xml`
- 6 LayoutControl được hỗ trợ: `layoutControl1`, `layoutControl2`, `layoutControl3`, `layoutControl4`, `layoutControl5`, `lciContentLibrary`
- Phím tắt khôi phục: `Ctrl+Shift+R` (đăng ký qua `KeyboardWorker.cs` — `[KeyboardAction("ResetLayoutToDefault", ...)]`)
- Snapshot layout designer GỐC được lưu trong RAM khi UC Load lần đầu — tồn tại đến khi UC dispose

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu dịch vụ chính cần xử lý |
| HIS_SERE_SERV / V_HIS_SERE_SERV | Table/View | Dịch vụ thực hiện trong yêu cầu |
| HIS_SERE_SERV_EXT | Table | Mô tả/kết quả mở rộng cho dịch vụ |
| HIS_SERE_SERV_PTTT | Table | Thông tin phẫu thuật/thủ thuật |
| V_HIS_SERE_SERV_SUIN | View | Chỉ số kết quả siêu âm/CDHA |
| HIS_MACHINE / HIS_SERVICE_MACHINE | Table | Máy thực hiện dịch vụ |
| V_HIS_BED_LOG | View | Lịch sử giường (cho phiếu kết quả nội trú) |
| HIS_DHST | Table | Dấu hiệu sinh tồn |
| HIS_TEXT_LIB | Table | Thư viện đoạn văn mô tả |
| HIS_EXECUTE_ROLE | Table | Vai trò ekip thực hiện |
| HIS_DEPARTMENT, V_HIS_SERVICE | Table/View | Khoa, dịch vụ — danh mục cache |
| SAR_PRINT | Table | Bản ghi phiếu in / ký số EMR |

### Quan hệ chính

- `HIS_SERVICE_REQ` 1-n `HIS_SERE_SERV` (qua `SERVICE_REQ_ID`)
- `HIS_SERE_SERV` 1-1 `HIS_SERE_SERV_EXT` / `HIS_SERE_SERV_PTTT` (qua `SERE_SERV_ID`)
- `HIS_SERE_SERV` 1-n `V_HIS_SERE_SERV_SUIN` (qua `SERE_SERV_ID`)

## 4. UI Layout

### Sơ đồ giao diện (đơn giản hóa)

```
+------------------------------------------------------------------+
| Toolbar: [Lưu][Lưu&In][Mới][Khám lại][Hủy][In][Ký số][Đóng]      |
+------------------------------------------------------------------+
| layoutControl1 (Root — chứa các nhóm chính)                      |
|  ├── lcgPatientInfo: Họ tên BN / Mã / Tuổi / Đối tượng / Khoa   |
|  ├── lcgServiceInfo: Tên DV / Máy / Ekip / Thời gian / STT      |
|  ├── lciContentLibrary (LayoutControl): Thư viện đoạn văn        |
|  ├── lcgImage (LayoutControl5): Panel ảnh PACS + capture          |
|  ├── lcgDescription: panelDescription (UcWord/UcTelerik)         |
|  ├── lcgEkip (LayoutControl3): Lưới ekip thực hiện               |
|  ├── lcgSuin (LayoutControl4): Chỉ số kết quả                    |
|  └── lcgPttt (LayoutControl2): Phẫu thuật / Thủ thuật            |
+------------------------------------------------------------------+
| Footer: trackBarZoom, status, thời gian server                  |
+------------------------------------------------------------------+
```

> Người dùng có thể chuột phải vào bất kỳ LayoutControl nào → "Customize Layout..." để mở Customization Form và kéo thả lại các panel cho phù hợp với độ phân giải/quy trình từng phòng. Layout đã chỉnh sửa được tự lưu sau MouseUp.

### UC sử dụng

| UC | Vị trí | Mục đích |
|----|--------|----------|
| UcWord / UcWordFull | panelDescription | Soạn mô tả/kết luận (DevExpress RichEdit) |
| UcWords.UcTelerik / UcTelerikFullWord | panelDescription | Soạn mô tả (Telerik RichTextEditor) — theo cấu hình |
| UCEkipUser | lcgEkip | Bảng người trong ekip |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy dịch vụ thực hiện | api/HisSereServ/Get | MosConsumer | HisSereServFilter |
| Lưu kết quả CLS | api/HisSereServ/UpdateResult | MosConsumer | HisSereServUpdateResultSDO |
| Lấy chỉ số siêu âm | api/HisSereServSuin/Get | MosConsumer | HisSereServSuinFilter |
| Lấy bệnh án giường | api/HisBedLog/GetView | MosConsumer | HisBedLogViewFilter |
| Lấy DHST | api/HisDhst/Get | MosConsumer | HisDhstFilter |
| Tải template SAR | (qua RichEditorStore) | SarConsumer | — |

> Danh sách trên là các URI điển hình; URI đầy đủ tập trung trong `RequestUriStore.cs` của plugin.

## 6. Dependencies

### Library Plugins

| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT | Cảnh báo khoản phí không BHYT khi chỉ định DV |
| HIS.Desktop.Plugins.Library.EmrGenerate | Sinh InputADO ký số EMR cho phiếu kết quả |
| HIS.Desktop.Plugins.Library.FormOtherSereServ | Biểu mẫu phụ theo dịch vụ |

### Inter-Plugin

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.ExecuteRoom | UC plugin này được embed bên trong | Module + V_HIS_SERVICE_REQ + DelegateRefresh |
| EMR signing form | Khi bấm Ký số kết quả | InputADO từ EmrGenerateProcessor |

### LIB framework

- `HIS.Desktop.Utility` (UserControlBase): khởi động `CustomizaButtonAndRestoreLayoutInControlProcess` ở `OnLoad` (đã hỗ trợ GridView/TreeList)
- `HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation`: lấy `ApplicationStartupPath` để xác định folder `ModuleDesign/`
- `HIS.Desktop.LocalStorage.HisConfig.HisConfigs`: đọc config `HIS.Desktop.ApplyRestoreLayout.ModuleLinks`
- `DevExpress.XtraLayout.LayoutControl`: dùng `SaveLayoutToXml` / `RestoreLayoutFromXml` / `SaveLayoutToStream`

## 7. Print

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| Phiếu kết quả CDHA / Siêu âm / TDCN | Theo SAR_PRINT.PRINT_TYPE_CODE | RichEditorStore + MPS Processor (động) | Template SAR (Word/Excel) |
| Phiếu phẫu thuật/thủ thuật | Mps000xxx | MPS Processor | Template từ SAR |

> Template được lấy động qua `RichEditorStore` từ SAR backend; PrintTypeCode được khai báo trong `SAR_PRINT` của từng dịch vụ.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/04/2026 | sinhnt@vietsens.vn | **Thêm tính năng "Giữ lại Customize Layout"** — sau khi user kéo thả các LayoutControlItem qua menu chuột phải > Customize Layout, layout được tự lưu thành file XML vào `ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/{layoutControlName}.xml`. Lần mở kế tiếp UC tự động RestoreLayoutFromXml. Bật/tắt qua `HIS_CONFIG` key `HIS.Desktop.ApplyRestoreLayout.ModuleLinks`. Thêm partial class `UCServiceExecute___RestoreLayout.cs`. Hook `InitRestoreLayout()` vào cuối `UCServiceExecute_Load`, sau `ProcessCustomizeUI()`. |
| 29/04/2026 | sinhnt@vietsens.vn | **Sửa hành vi auto-save** — đổi từ event `MouseUp` sang `LayoutControl.CustomizationVisibleChanged`. Khi user đóng Customization Form thì auto save vào file XML của plugin, KHÔNG bật "Save As" dialog của DevExpress yêu cầu user chọn file thủ công. Áp dụng cho tất cả bệnh nhân (không phụ thuộc treatment/serviceReq). |
| 29/04/2026 | sinhnt@vietsens.vn | **Thêm phím tắt Ctrl+Shift+R khôi phục layout mặc định** — snapshot layout designer GỐC được lưu trong RAM khi UC Load. Phím tắt confirm dialog → restore từ snapshot + xóa file XML đã lưu. Đăng ký `[KeyboardAction("ResetLayoutToDefault", ...)]` trong `KeyboardWorker.cs`. Thêm message resource `BanCoMuonKhoiPhucLayoutMacDinh` (3 ngôn ngữ vi/en/my). |

## 9. Test Cases

### Tính năng "Giữ lại Customize Layout"

#### Auto-save & Auto-apply
- [ ] Khi `HIS_CONFIG.HIS.Desktop.ApplyRestoreLayout.ModuleLinks` KHÔNG chứa `HIS.Desktop.Plugins.ServiceExecute`:
  - User customize layout → đóng UC → mở lại → layout về mặc định (không lưu)
- [ ] Khi config chứa `HIS.Desktop.Plugins.ServiceExecute`:
  - User chuột phải > "Customize Layout..." → kéo thả → đóng Customization Form (X)
  - **KHÔNG** hiện "Save As" dialog yêu cầu chọn file
  - File `ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/{layoutControlName}.xml` tự xuất hiện
  - Đóng tab UC → mở UC với BN khác → layout giữ nguyên (áp dụng cho mọi BN)
- [ ] User customize 6 LayoutControl khác nhau (`layoutControl1..5`, `lciContentLibrary`) → mỗi LayoutControl có 1 file XML riêng
- [ ] User mở UC, KHÔNG customize → KHÔNG sinh thêm file XML mới (event không fire)
- [ ] Copy file XML từ máy A sang `ModuleDesign/HIS.Desktop.Plugins.ServiceExecute/` của máy B → máy B mở UC → áp dụng đúng layout máy A

#### Reset layout (Ctrl+Shift+R)
- [ ] User customize layout → bấm Ctrl+Shift+R → confirm "Có" → layout về mặc định designer + xóa file XML
- [ ] User customize layout → bấm Ctrl+Shift+R → confirm "Không" → giữ nguyên layout đã sửa
- [ ] Sau Reset, đóng UC mở lại → vẫn về layout mặc định (file XML đã bị xóa)
- [ ] Sau Reset, customize lại → file XML mới được sinh (auto-save vẫn hoạt động bình thường)

#### Edge cases
- [ ] Config bị xóa giữa phiên → UC vẫn mở bình thường (ghi WARN log nếu lỗi)
- [ ] Folder `ModuleDesign/` không có quyền ghi → log WARN, không crash UC
- [ ] File XML hỏng (không phải XML hợp lệ) → log Error, UC vẫn load với layout designer

### Quy trình nghiệp vụ chính (giữ nguyên)

- [ ] Mở UC từ Phòng thực hiện → load đúng BN + DV
- [ ] Nhập mô tả + chỉ số → Lưu thành công, grid yêu cầu refresh
- [ ] In kết quả → preview/in trực tiếp (theo config `CheDoInChoCacChucNangTrongPhanMem`)
- [ ] Ký số EMR → tạo SAR_PRINT đúng
- [ ] Camera capture, chọn ảnh → upload PACS hoặc lưu local thành công

### Logging

- [ ] Khi enable restore: có log `Debug` với `isAllowRestoreLayout = True`
- [ ] Khi customize được lưu: có log `Info` "Customize layout saved: {layoutControlName} -> {fileName}"
- [ ] Mọi exception trong `InitRestoreLayout` / hook events → log `Warn`, KHÔNG ảnh hưởng quy trình chính
