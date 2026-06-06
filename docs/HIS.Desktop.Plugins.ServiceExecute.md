# HIS.Desktop.Plugins.ServiceExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceExecute |
| Loại | UC (UserControl) |
| Mục đích | Xử lý yêu cầu khám/cận lâm sàng/PTTT trong Phòng thực hiện — trả kết quả CDHA, Siêu âm, TDCN, PTTT cho bệnh nhân |
| Người tạo | IVT |
| Ngày cập nhật gần nhất | 07/05/2026 |
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

### Tính năng "Sinh ảnh chữ ký theo key tài khoản" (mới — 07/05/2026 — PTTK 4.1.2)

```
1. Quản trị khai báo cấu hình ánh xạ key tài khoản → key chữ ký trong form danh mục mẫu DV
   (HIS.Desktop.Plugins.SereServTemplate) → lưu JSON vào HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG
   VD: [{"LoginnameKey":"REQ_LOGINNAME","SignatureKey":"REQ_LOGINNAME_SIGNATURE"},
        {"LoginnameKey":"EXECUTE_LOGINNAME","SignatureKey":"EXECUTE_LOGINNAME_SIGNATURE"}]

2. Khi UCServiceExecute build xong dicParam (gồm các key như REQ_LOGINNAME, EXECUTE_LOGINNAME...)
   → ProcessGenSignatureByKey() được gọi 1 lần ở cuối ProcessDicParam (phục vụ cả xem và in).

3. Với mỗi cặp { LoginnameKey, SignatureKey } trong cấu hình:
   - Đọc dicParam[LoginnameKey] để lấy giá trị loginname.
   - Truy vấn EMR_SIGNER (api/EmrSigner/Get + EmrConsumer) theo loginname → lấy SIGN_IMAGE (byte[]).
   - Convert byte[] → System.Drawing.Image (clone qua Bitmap để an toàn dispose).
   - Set dicImage[SignatureKey] = ảnh chữ ký → RichEditor sẽ tự thay vào template khi render.

4. Skip silent (không lỗi) khi:
   - Mẫu chưa được chọn / GEN_SIGNATURE_BY_KEY_CFG null/rỗng.
   - JSON sai cấu trúc (parse exception → Warn log, return).
   - LoginnameKey hoặc SignatureKey rỗng.
   - LoginnameKey không tồn tại trong dicParam (chưa được fill bởi luồng ProcessDicParam trước đó).
   - Loginname không tìm thấy trong EMR_SIGNER, hoặc SIGN_IMAGE null/rỗng.
```

### Kiểm tra thông tin máy cận lâm sàng khi lưu (key `SubclinicalMachineOption`)

Khi lưu dịch vụ CLS mà `sereServ.MACHINE_ID == null`, plugin xử lý theo key `HIS.Desktop.Plugins.ServiceExecute.SubclinicalMachineOption`:

| Key | Hành vi | Điều kiện thêm |
|-----|---------|----------------|
| 1 | Cảnh báo (Yes/No) | — |
| 2 | Chặn (OK, dừng lưu) | — |
| 3 | Cảnh báo (Yes/No) | Đối tượng BHYT |
| 4 | Chặn | Đối tượng BHYT |
| 5 | Cảnh báo (Yes/No) | Dịch vụ đã cấu hình Dịch vụ - Máy khả dụng tại phòng |
| 6 | Chặn | Dịch vụ đã cấu hình Dịch vụ - Máy khả dụng tại phòng |
| 7 | Cảnh báo (Yes/No) | BHYT **và** đã cấu hình Dịch vụ - Máy khả dụng tại phòng |
| 8 | Chặn | BHYT **và** đã cấu hình Dịch vụ - Máy khả dụng tại phòng |
| khác | Không cảnh báo/chặn | — |

> "Khả dụng tại phòng" (helper `HasConfiguredMachineInRoom`): tồn tại bản ghi `HIS_SERVICE_MACHINE` với `SERVICE_ID` của dịch vụ mà `MACHINE_ID` trỏ tới một `HIS_MACHINE` có `IS_ACTIVE = 1` và `ROOM_IDS` (phân cách dấu phẩy) chứa phòng đang xử lý (`moduleData.RoomId`). Với key 5-8, nếu không có máy khả dụng thì lưu bình thường, không cảnh báo/chặn.

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
| HIS_SERE_SERV_TEMP | Table | Mẫu dịch vụ — đọc thêm cột `GEN_SIGNATURE_BY_KEY_CFG` (JSON) để sinh ảnh chữ ký |
| EMR_SIGNER (EMR) | Table | Tra cứu `SIGN_IMAGE` theo `LOGINNAME` để render vào kết quả |

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
| Lấy thông tin chữ ký (cho `GEN_SIGNATURE_BY_KEY_CFG`) | api/EmrSigner/Get | EmrConsumer | EmrSignerFilter (KEY_WORD = loginname) |

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
| 05/06/2026 | phuongnm@vietsens.vn | **Tài liệu 2539 — Bổ sung kiểm tra cấu hình Dịch vụ - Máy cho key `HIS.Desktop.Plugins.ServiceExecute.SubclinicalMachineOption` = 5/6/7/8.** Trong khối kiểm tra "chưa nhập thông tin máy" tại `SaveProcess` (`UCServiceExecute.cs`), thêm 4 nhánh: `5` (chỉ cảnh báo), `6` (chặn), `7` (chỉ cảnh báo + BHYT), `8` (chặn + BHYT). Cả 4 nhánh chỉ kích hoạt khi dịch vụ có máy CLS khả dụng tại phòng đang xử lý — kiểm tra qua helper mới `HasConfiguredMachineInRoom(long serviceId)`: A = `HIS_SERVICE_MACHINE` theo `SERVICE_ID`, B = `HIS_MACHINE` có `IS_ACTIVE = 1` và `moduleData.RoomId` nằm trong `ROOM_IDS` (CSV); trả true khi tồn tại `A.MACHINE_ID = B.ID`. Dùng lại `ListMachine`/`ListServiceMachine` (đã cache `BackendDataWorker`) và message `DichVuChuaCoMay` / `BanCoMuonTiepTucKhong`. Key khác 1-8 không cảnh báo/chặn. |
| 09/05/2026 | sinhnt@vietsens.vn | **Refactor đồng nhất với plugin `HIS.Desktop.Plugins.ServiceReqResultView`** (cùng feature đã chạy production). (1) Đổi tên class ADO → `GenSignatureByKeyCFGADO`, file → `ADO/GenSignatureByKeyCFGADO.cs`. (2) Method chính → `SetSignatureKeyImageByCFG()`. (3) Lấy EMR_SIGNER qua **`BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_SIGNER>()`** — cache HIS local, không call API, không phụ thuộc `MPS.ProcessorBase.PrintConfig.EmrSigners` (cache này có thể chưa được nạp tại UC giai đoạn). (4) Tách 2 method `InsertSignatureImagesIntoDocument(RichEditControl)` + `ReplaceKeyWithImage(...)`: tìm cả 2 format `<#{SignatureKey};>` (MPS chuẩn) và `<#{SignatureKey}_PRINT;>` (convention plugin). (5) Insert image dùng `Document.CreatePosition(startOffset)` thay vì `range.Start` — an toàn sau khi `Document.Delete(range)` invalidate range. (6) Bỏ resize ảnh — insert kích thước gốc, đồng nhất với ServiceReqResultView. Gọi `InsertSignatureImagesIntoDocument(GettxtDescription())` sau `processImageTag.ProcessData` trong `ProcessDescriptionContent`. |
| 08/05/2026 | sinhnt@vietsens.vn | **Refactor align với `MPS.ProcessorBase.AbstractProcessor.SetSignatureKeyImageByCFG()`**: rename `ADO/GenSignatureByKeyCfgADO.cs` → `ADO/GenSignatureImageKeyADO.cs` (cùng tên với MPS), rename method `ProcessGenSignatureByKey()` → `SetSignatureKeyImageByCFG()`. Chuẩn hóa pattern foreach config (giống MPS): kiểm tra `dicParam.ContainsKey(loginNameKey)` → tìm signer → set vào dictionary. Tách `LoadEmrSignerByLoginname()` thành helper riêng. Thêm log với format giống MPS (`Bieu in co cau hinh GEN_SIGNATURE_BY_KEY_CFG=...`). **Khác biệt cần thiết với MPS**: UCServiceExecute dùng RichEditor 2 dictionary (dicParam text, dicImage image) thay vì 1 dictionary thống nhất như MPS — vì engine FlexCel/Aspose của MPS tự render `byte[]` thành ảnh, còn RichEditor cần `Image` object trong `dicImage` cho image placeholder + tự insert inline (`ProcessSignatureImageIntoDocument`) cho TEXT key thuần. |
| 08/05/2026 | sinhnt@vietsens.vn | **Bổ sung: tự chèn ảnh chữ ký inline cho template chỉ có TEXT key** (không có image placeholder dựng sẵn). Thêm hàm `ProcessSignatureImageIntoDocument(Document)` trong `UCServiceExecute_PlusDescription.cs`: với mỗi `SignatureKey` đã có ảnh trong `dicImage`, `Document.FindAll("<#SignatureKey;>")` → `Delete(range)` + `Document.Images.Insert(pos, DocumentImageSource.FromImage(img))` (size 150×40). Gọi sau `processImageTag.ProcessData` trong `ProcessDescriptionContent()`. Thêm log Debug tại các bước: cfgRaw, parse OK/fail, dicParam thiếu key, query EMR_SIGNER count, set dicImage, tìm thấy/không text key trong document. Lý do: `Inventec.Common.RichEditor.ProcessTag.ProcessImageTag` chỉ tìm image placeholder có sẵn — không thay được TEXT key `<#SignatureKey;>` thuần (xảy ra khi admin chưa thiết kế lại template với image placeholder). |
| 07/05/2026 | sinhnt@vietsens.vn | **PTTK 4.1.2 — Sinh ảnh chữ ký theo cấu hình `GEN_SIGNATURE_BY_KEY_CFG`** trong mẫu dịch vụ. Thêm `ADO/GenSignatureByKeyCfgADO.cs` (POCO `LoginnameKey`/`SignatureKey`). Thêm field `currentSereServTempl` trong `UCServiceExecute.cs` lưu mẫu DV đang chọn (set tại đầu `ProcessChoiceSereServTempl`). Thêm hàm `ProcessGenSignatureByKey()` + `ConvertSignImageBytesToImage()` trong `UCServiceExecute_PlusDescription.cs`, gọi 1 lần ở cuối `ProcessDicParam()` (phục vụ cả xem kết quả lẫn in). Hàm parse JSON cấu hình → với mỗi cặp `(LoginnameKey, SignatureKey)` hợp lệ, tra cứu `EMR_SIGNER` theo `LOGINNAME` lấy từ `dicParam[LoginnameKey]` → set `dicImage[SignatureKey]` = ảnh chữ ký. Skip silent khi: JSON sai, key rỗng, không có trong dicParam, không tìm thấy ảnh — không làm vỡ luồng in. Đọc `GEN_SIGNATURE_BY_KEY_CFG` qua reflection để an toàn nếu `MOS.EFMODEL` chưa cập nhật theo PTTK Section II.1. |
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

### Tính năng "Sinh ảnh chữ ký theo key tài khoản" (PTTK 4.1.2)

#### Happy path
- [ ] Mẫu DV có `GEN_SIGNATURE_BY_KEY_CFG = [{"LoginnameKey":"REQ_LOGINNAME","SignatureKey":"REQ_LOGINNAME_SIGNATURE"}]`, BN có `REQ_LOGINNAME = "doctor1"` và `EMR_SIGNER` có record loginname `doctor1` với `SIGN_IMAGE` không null:
  - Mở UC chọn DV → render mô tả → key `<#REQ_LOGINNAME_SIGNATURE;>` được thay bằng ảnh chữ ký BS chỉ định
  - Bấm In → preview phiếu kết quả có ảnh chữ ký BS chỉ định bên dưới mục "Lời dặn"
- [ ] 2 cặp cấu hình (REQ_LOGINNAME + EXECUTE_LOGINNAME) → 2 ảnh chữ ký được render đúng vị trí

#### Bỏ qua không lỗi
- [ ] Cấu hình JSON sai cú pháp → Warn log, kết quả/in vẫn render bình thường (không có ảnh chữ ký), không crash
- [ ] LoginnameKey rỗng / SignatureKey rỗng → entry đó skip
- [ ] LoginnameKey không có trong biểu mẫu (không có trong dicParam) → entry đó skip
- [ ] EMR_SIGNER không tồn tại record với loginname đó → skip, không lỗi
- [ ] EMR_SIGNER có record nhưng `SIGN_IMAGE = null` → skip
- [ ] `GEN_SIGNATURE_BY_KEY_CFG` null/rỗng → không gọi API, không log error
- [ ] Cột `GEN_SIGNATURE_BY_KEY_CFG` chưa có trong EFMODEL (`MOS.EFMODEL` cũ) → reflection trả null property, skip silent

#### Performance / API
- [ ] 3 cấu hình cùng dùng `LoginnameKey = "REQ_LOGINNAME"` → chỉ gọi `api/EmrSigner/Get` 1 lần (distinct loginname)
- [ ] Mất kết nối EMR backend khi tra cứu → Warn log, kết quả/in vẫn render mô tả gốc

### Kiểm tra thông tin máy CLS khi lưu (key SubclinicalMachineOption = 5/6/7/8)

Tiền đề chung: lưu dịch vụ CLS khi chưa chọn máy (`MACHINE_ID == null`).

- [ ] Key=5, DV có cấu hình Dịch vụ - Máy + máy `IS_ACTIVE=1` thuộc phòng đang xử lý → hiện cảnh báo Yes/No; "Không" → dừng lưu, "Có" → tiếp tục lưu
- [ ] Key=5, DV KHÔNG có cấu hình máy khả dụng tại phòng → lưu bình thường, không cảnh báo
- [ ] Key=6, DV có máy khả dụng tại phòng → hiện thông báo OK và dừng lưu
- [ ] Key=6, DV không có máy khả dụng → lưu bình thường
- [ ] Key=7, đối tượng BHYT + có máy khả dụng → cảnh báo Yes/No; đối tượng KHÔNG BHYT → bỏ qua dù có máy
- [ ] Key=8, đối tượng BHYT + có máy khả dụng → chặn lưu; đối tượng KHÔNG BHYT → bỏ qua
- [ ] Máy có `ROOM_IDS` nhiều phòng "111,222,333" → khớp đúng phòng giữa danh sách (không false-positive do substring, VD phòng `22` không khớp `222`)
- [ ] Máy `IS_ACTIVE=0` dù đúng phòng → không tính là khả dụng
- [ ] Key khác 1-8 → không cảnh báo/chặn

### Logging

- [ ] Khi enable restore: có log `Debug` với `isAllowRestoreLayout = True`
- [ ] Khi customize được lưu: có log `Info` "Customize layout saved: {layoutControlName} -> {fileName}"
- [ ] Mọi exception trong `InitRestoreLayout` / hook events → log `Warn`, KHÔNG ảnh hưởng quy trình chính
