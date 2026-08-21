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

### Chủ động gửi kết quả sang hệ thống tích hợp (checkbox `chkSendExt`)

Trước đây mọi lần lưu kết quả CĐHA đều tự động tạo tiến trình gửi sang các hệ thống PACS. Viện không muốn hành vi tự động này, nên bổ sung checkbox cho người dùng quyết định từng lần lưu.

| Thành phần | Chi tiết |
|-----------|----------|
| Control | `chkSendExt` (CheckEdit) — layout item `lciSendExt` trong `layoutControlGroup2`, cùng dòng với `lciDateResult` (Ngày KQ) |
| Label | "Gửi sang hệ thống tích hợp" (3 ngôn ngữ: `UCServiceExecute.chkSendExt.Properties.Caption`) |
| Tooltip | `UCServiceExecute.chkSendExt.ToolTip` |
| Mặc định | **Luôn tích** mỗi lần áp dụng điều kiện hiển thị — set trong `SetDefaultValueControl()` + `ApplySendExtVisibility()`. KHÔNG lưu ControlState (không nhớ trạng thái giữa các phiên) |
| Khi lưu | `data.IsSendExt = GetIsSendExtForSave()` — gán ở cả `SaveProcess()` và `SaveAllProcess()` (`UCServiceExecute.cs`) |
| Tích | `IsSendExt = true` → backend tạo tiến trình gửi sang PACS (bao gồm Pacs Bách Khoa `MOS.PACS.CONNECTION_TYPE = 2` — gửi qua file) |
| Không tích | `IsSendExt = false` → backend KHÔNG tạo tiến trình gửi |
| Checkbox bị ẩn | `IsSendExt = true` (mặc định) → giữ nguyên hành vi luôn gửi, KHÔNG hồi quy |
| Form phụ `frmClsInfo` | Giữ nguyên hành vi cũ — `SaveProcessor()` gán cứng `data.IsSendExt = true` (luôn gửi) |

#### Điều kiện hiển thị checkbox — `ApplySendExtVisibility()`

Chỉ hiển thị khi **cả 3** điều kiện đúng:

| # | Điều kiện | Nguồn |
|---|-----------|-------|
| 1 | `HIS.DESKTOP.HIS_SERE_SERV_EXT.ALLOW_DISPLAY_SEND_ORDER_PACS_CDHA` = `1` | `AppConfigKeys.AllowDisplaySendOrderPacsCdha` (HisConfigs — toàn viện). Trả về **string thô**, so `== "1"`; rỗng/null/khác `"1"` → không hiện, giữ nguyên hành vi (theo pattern `HIS.DESKTOP.HIS_TREATMENT.UNLOCK_FEE_OPTION`, `HIS.DESKTOP.TREATMENT_FINISH.CHECK_SAME_HEIN`) |
| 2 | Y lệnh thuộc loại CĐHA | `ServiceReqConstruct.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA` |

| 3 | `MOS.PACS.ADDRESS` có bản ghi khớp mã phòng | `PacsCFG.PACS_ADDRESS_EXPAND_ROOM.Exists(o => o.RoomCode == room.ROOM_CODE)` — chỉ so `RoomCode`, KHÔNG kiểm tra field kết nối |

Cách lấy `room` cho điều kiện 3:

```csharp
long roomId = (ServiceReqConstruct != null && ServiceReqConstruct.EXECUTE_ROOM_ID > 0)
    ? ServiceReqConstruct.EXECUTE_ROOM_ID     // Ưu tiên phòng THỰC HIỆN của y lệnh
    : moduleData.RoomId;                       // Fallback: phòng làm việc
V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId) ?? new V_HIS_ROOM();
```

- Mặc định config TẮT (khác `1`) → ẩn checkbox → `IsSendExt = true` → hành vi y như trước khi có tính năng (an toàn đa viện).
- Ẩn bằng `lciSendExt.Visibility = LayoutVisibility.Never` → ô thu lại, `dtResult` (Ngày KQ) giãn full bề rộng cột phải.
- Gọi tại 2 điểm: cuối `UCServiceExecute_Load` (sau `ApplyResultTimeFieldVisibility`) và trong `SearchNewTreatmentServiceReqForShowForm()` (sau `SetDisable()`) — để bám theo loại y lệnh mới khi đổi bệnh nhân/y lệnh.

#### `PACS_ADDRESS_EXPAND_ROOM` — parse riêng biệt, KHÔNG chạm luồng cũ

`PACS/PacsCFG.cs` được bổ sung **thành viên mới, tách biệt hoàn toàn** (chỉ thêm dòng, không sửa/xóa dòng nào của bản cũ):

| Thành viên | Vai trò |
|-----------|---------|
| `PacsAddressRoom` | Class riêng — **chỉ có `RoomCode`** |
| `PACS_ADDRESS_EXPAND_ROOM` | Property + **cache riêng** (`pacsAddressExpandRoom`) |
| `GetAddressExpandRoom(string config)` | Parse `MOS.PACS.ADDRESS`, tách `RoomCode` chứa `\|` |
| `ROOM_CODE_SEPARATOR = '\|'` | Ký tự phân cách |

**`MOS.PACS.ADDRESS` có nhiều schema đang chạy thực tế** — nên `PacsAddressRoom` **chỉ khai `RoomCode`**, không kiểm tra field kết nối:

| Schema | JSON |
|--------|------|
| DICOM | `{ "RoomCode": "P01", "Address": "10.0.0.5", "Port": 104 }` |
| Gửi qua file / FTP (Pacs Bách Khoa) | `{ "RoomCode": "XQ2", "Ip": "192.168.1.201", "User": "...", "Password": "...", "SaveFolder": "...", "ReadFolder": "...", "Is_Ftp": "1" }` |

Điều kiện: **có bản ghi khớp `RoomCode`** là đủ — cùng logic với `btnLoadImage_Click` (`Exists(o => o.RoomCode == room.ROOM_CODE)`).

> KHÔNG kiểm tra `Address` / `Ip`: `PacsAddress` (class cũ) chỉ có `RoomCode` / `Address` / `Port`, nên với schema file/FTP thì `Address` luôn = null → nếu ràng buộc `Address` thì checkbox sẽ không bao giờ hiện ở các viện dùng Pacs Bách Khoa.

```
[{ "RoomCode": "P01|P02|P03", "Address": "10.0.0.5", "Port": 104 }]
→ 3 bản ghi: P01 / P02 / P03, cùng Address + Port
```

**Cố ý KHÔNG sửa `PACS_ADDRESS` / `GetAddress()` hiện có.** Lý do: `btnLoadImage_Click` (tải ảnh từ PACS) dùng `PACS_ADDRESS.Exists(o => o.RoomCode == room.ROOM_CODE)`; nếu thêm tách `\|` vào đó sẽ làm phòng cấu hình gộp bắt đầu khớp → **thay đổi hành vi tải ảnh**. Hai đường đọc config dùng cùng key `MOS.PACS.ADDRESS` nhưng cache và parse độc lập.

| Luồng | Dùng | Có tách `\|` |
|-------|------|--------------|
| Tải ảnh từ PACS (`btnLoadImage_Click`) | `PACS_ADDRESS` | KHÔNG (giữ nguyên) |
| Hiển thị checkbox `chkSendExt` | `PACS_ADDRESS_EXPAND_ROOM` | CÓ |

> Lưu ý: `PacsAddress` của plugin này chỉ có `RoomCode` / `Address` / `Port` — KHÔNG có `Api` / `CloudInfo` như bản trong `HIS.Desktop.Plugins.ServiceReqResultView`. Không bổ sung vì tính năng này không dùng đến.

### Xem ảnh PACS Carestream — `ConnectImageOption = 3` (cập nhật 12/08/2026)

Khác PACS Vietsens (`= 1`, HIS tải ảnh về lưới), Carestream chỉ trả **một link xem ảnh**: PACS gọi `/api/studycompleted` đẩy link sang HIS, backend lưu vào `HIS_SERE_SERV_EXT.JSON_FORM_ID`. HIS không tự sinh được link.

```
btnLoadImage_Click  (ConnectImageOption == "3" và có TDL_PACS_TYPE_CODE)
  └─ OpenPacsLinkCarestream(sereServId)
       ├─ GetPacsLinkCarestream: dicSereServExt → thiếu thì ProcessDicSereServExt (1 lần API)
       │    link rỗng → ResourceMessage.ChuaCoHinhAnhTuPacs → dừng
       ├─ ShowServiceReqResultView(sereServId, link)
       │    ├─ kiểm tra module + quyền trong GlobalVariables.currentModuleRaws
       │    └─ ShowModule(ModuleLinkString.ServiceReqResultView, RoomId, RoomTypeId,
       │                  { moduleData, sereServId, link })       ← truyền LINK sang
       └─ false → Process.Start(link)  (mở trình duyệt mặc định)
```

| Điểm | Chi tiết |
|------|----------|
| Vì sao kiểm tra module trước | `PluginInstanceBehavior.ShowModule` trả `void` và tự bắt hết exception → không kiểm tra trước thì nhánh dự phòng không bao giờ chạy |
| Vì sao truyền link | Màn Xem kết quả không phải lấy lại link (không gọi thêm API), và hiển thị được cả khi phòng chưa khai `Api` trong `MOS.PACS.ADDRESS` |
| Link | Đã được PACS mã hóa sẵn (`urltoken`) → truyền **nguyên văn**, KHÔNG encode lại |
| Thông báo | `ResourceMessage.ChuaCoHinhAnhTuPacs` / `KhongMoDuocHinhAnhTuPacs` (vi/en/my) |

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
| Tạo kết quả CĐHA (SDO) | api/HisSereServExt/CreateSdo | MosConsumer | HisSereServExtSDO (có `IsSendExt`) |
| Cập nhật kết quả CĐHA (SDO) | api/HisSereServExt/UpdateSdo | MosConsumer | HisSereServExtSDO (có `IsSendExt`) |

> Danh sách trên là các URI điển hình; URI đầy đủ tập trung trong `RequestUriStore.cs` của plugin.

> `HisSereServExtSDO.IsSendExt` (bool) quyết định backend có tạo tiến trình gửi kết quả sang các hệ thống tích hợp (PACS) hay không. Frontend gán theo checkbox `chkSendExt` — xem Section 2.

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
| HIS.Desktop.Plugins.ServiceReqResultView | Bấm "Tải ảnh" khi `ConnectImageOption = 3` (PACS Carestream) | `Module` + `long sereServId` + `string` link xem ảnh (`JSON_FORM_ID`) |
| HIS.Desktop.Plugins.AssignPaan / AssignService | Nút chỉ định tương ứng | Xem code từng nút |

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
| 12/08/2026 | anhnh2@vietsens.vn | **Sửa luồng mở màn Xem kết quả để xem ảnh PACS Carestream.** (1) `ShowServiceReqResultView(long, string)` — thêm tham số link, và **kiểm tra module + quyền** qua `GlobalVariables.currentModuleRaws` trước khi gọi `PluginInstanceBehavior.ShowModule`; trước đây hàm luôn trả `true` (ShowModule là `void` + nuốt exception) nên nhánh dự phòng `Process.Start(link)` là code chết. (2) Truyền link (`HIS_SERE_SERV_EXT.JSON_FORM_ID`) sang plugin đích để màn đó không phải lấy lại link. (3) Thay 2 chuỗi hardcode bằng `ResourceMessage.ChuaCoHinhAnhTuPacs` / `KhongMoDuocHinhAnhTuPacs` (thêm key vào `Message.Lang.vi/en/my.resx`). (4) Thêm `ModuleLinkString.cs` (const `ServiceReqResultView`) theo `inter_plugin.md`, đăng ký vào `.csproj`. Phía plugin đích `HIS.Desktop.Plugins.ServiceReqResultView` sửa kèm: nhận link qua Behavior, hiển thị link đã lưu khi phòng chưa khai `Api`, không auto-print+Close khi mở để xem ảnh. |
| 31/07/2026 | nampp@vietsens.vn | **Bổ sung điều kiện hiển thị checkbox `chkSendExt`** — thêm hàm `ApplySendExtVisibility()` (`UCServiceExecute.cs`): chỉ hiện khi (1) HIS_CONFIG `HIS.DESKTOP.HIS_SERE_SERV_EXT.ALLOW_DISPLAY_SEND_ORDER_PACS_CDHA` = `1`, (2) y lệnh loại CĐHA (`ServiceReqConstruct.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA`), (3) phòng đang xử lý có địa chỉ PACS hợp lệ trong `MOS.PACS.ADDRESS`. Gọi ở cuối `UCServiceExecute_Load` và trong `SearchNewTreatmentServiceReqForShowForm()` (bám theo y lệnh mới khi đổi bệnh nhân). Thêm accessor `AppConfigKeys.AllowDisplaySendOrderPacsCdha` (trả string thô, so `== "1"` tại nơi dùng — theo pattern `UNLOCK_FEE_OPTION` / `CHECK_SAME_HEIN`) + const `CONFIG_KEY__ALLOW_DISPLAY_SEND_ORDER_PACS_CDHA`. Thêm hàm `GetIsSendExtForSave()` — checkbox ẩn thì trả `true` (mặc định luôn gửi, không hồi quy); 2 chỗ lưu đổi sang dùng hàm này. Điều kiện 3 chỉ so `RoomCode` (không ràng buộc field kết nối vì `MOS.PACS.ADDRESS` có nhiều schema), phòng lấy theo `ServiceReqConstruct.EXECUTE_ROOM_ID` (fallback `moduleData.RoomId`). Bổ sung vào `PACS/PacsCFG.cs` các thành viên MỚI tách biệt: class `PacsAddressRoom` (chỉ có `RoomCode`) + `PACS_ADDRESS_EXPAND_ROOM` (cache riêng) + `GetAddressExpandRoom()` + const `ROOM_CODE_SEPARATOR` — parse cùng key `MOS.PACS.ADDRESS` nhưng có tách `RoomCode` gộp nhiều phòng `"P01\|P02"`. **CỐ Ý KHÔNG sửa `PACS_ADDRESS` / `GetAddress()` cũ** để KHÔNG thay đổi hành vi nút "Tải ảnh" (`btnLoadImage_Click`). Diff `PacsCFG.cs` chỉ có dòng thêm, không có dòng xóa/sửa. Config mặc định TẮT = giữ nguyên hành vi hiện tại. |
| 29/07/2026 | nampp@vietsens.vn | **Chủ động gửi kết quả sang hệ thống tích hợp (PACS).** Bổ sung checkbox `chkSendExt` "Gửi sang hệ thống tích hợp" vào `UCServiceExecute.Designer.cs` — layout item mới `lciSendExt` (169,291) size 199×24 trong `layoutControlGroup2`, thay chỗ `emptySpaceItem3` đã bỏ (cùng dòng với `lciDateResult`). Mặc định **luôn tích** mỗi lần mở màn (set tại `SetDefaultValueControl()`), KHÔNG dùng ControlState. Khi lưu gán `data.IsSendExt` tại `SaveProcess()` và `SaveAllProcess()` (`UCServiceExecute.cs`) — backend chỉ tạo tiến trình gửi PACS khi `IsSendExt = true` (bao gồm Pacs Bách Khoa `MOS.PACS.CONNECTION_TYPE = 2` gửi qua file). Form phụ `frmClsInfo.SaveProcessor()` gán cứng `data.IsSendExt = true` để giữ nguyên hành vi luôn gửi. Thêm caption + tooltip vào `Lang.vi/en/my.resx` và `LoadKeysFromlanguage()`. Phụ thuộc backend: `MOS.SDO.HisSereServExtSDO` phải có property `IsSendExt` (bool). |
| 21/07/2026 | tuanln | **Tài liệu 43719 — Giữ kết nối camera khi chuyển bệnh nhân** (config-gated `HIS.Desktop.Plugins.ServiceExecute.IsKeepCameraConnectionOnSwitchPatient`, mặc định TẮT). (1) Thêm accessor `AppConfigKeys.IsKeepCameraConnectionOnSwitchPatient`. (2) Thêm entry point public `ReloadByServiceReq(V_HIS_SERVICE_REQ)` trong `UCServiceExecute.cs` — tái sử dụng luồng `ProcessSearchByServiceReqCode` → `SearchNewTreatmentServiceReqForShowForm` → `ReloadCameraAfterSearchByPatientThread` sẵn có để nạp BN mới vào cùng instance; camera chỉ đổi `SetClientCode` (KHÔNG mở lại thiết bị). (3) `UCServiceExecute_Leave` KHÔNG gọi `StopClick()` khi bật config (giữ camera sống lúc rời form để chuyển BN). (4) `ProcessDisposeModuleDataAfterClose` gọi `StopClick()` đầu hàm để chắc chắn giải phóng thiết bị camera khi đóng màn. Màn danh sách `HIS.Desktop.Plugins.ExecuteRoom` tái sử dụng instance đang mở thay vì mở tab mới. Config TẮT = giữ nguyên hành vi hiện tại (an toàn đa viện). |
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

### Checkbox "Gửi sang hệ thống tích hợp" (chkSendExt)

#### Điều kiện hiển thị

- [ ] Config `ALLOW_DISPLAY_SEND_ORDER_PACS_CDHA` = `1` + y lệnh CĐHA + phòng có trong `MOS.PACS.ADDRESS` → **hiện** checkbox, đã tích sẵn
- [ ] Config = `0` / rỗng / chưa khai báo → **ẩn** checkbox → lưu vẫn `IsSendExt = true`
- [ ] Config = `1` nhưng y lệnh là XN / TDCN / PTTT (không phải CĐHA) → **ẩn** → `IsSendExt = true`
- [ ] Config = `1` + CĐHA nhưng phòng KHÔNG có trong `MOS.PACS.ADDRESS` → **ẩn** → `IsSendExt = true`
- [ ] Schema DICOM (`{"RoomCode":"P01","Address":"10.0.0.5","Port":104}`), phòng P01 → **hiện**
- [ ] Schema file/FTP (`{"RoomCode":"XQ2","Ip":"192.168.1.201","SaveFolder":"...","ReadFolder":"..."}`), phòng XQ2 → **hiện**
- [ ] Y lệnh có `EXECUTE_ROOM_ID` khác phòng đang đăng nhập → xét theo `EXECUTE_ROOM_ID` của y lệnh
- [ ] `MOS.PACS.ADDRESS` khai `RoomCode = "P01\|P02"`, đang ở P02 → **hiện** (đã tách theo `\|`)
- [ ] **Không hồi quy nút "Tải ảnh"**: phòng cấu hình gộp `"P01\|P02"` → nút "Tải ảnh" vẫn chạy `LoadDataImageLocal()` như trước (KHÔNG chuyển sang `LoadImageFromPacs()`)
- [ ] Nhập mã y lệnh khác (đổi bệnh nhân) từ CĐHA → XN → checkbox **ẩn đi**; từ XN → CĐHA → **hiện lại và tích sẵn**
- [ ] Khi ẩn → ô thu lại, `dtResult` (Ngày KQ) giãn full bề rộng, layout không vỡ

#### Lưu

- [ ] Bỏ tích → đóng màn → mở lại → checkbox **tích lại** (không nhớ trạng thái)
- [ ] Tích + Lưu → log Debug `INPUT DATA` có `"IsSendExt":true` → backend tạo tiến trình gửi PACS
- [ ] Bỏ tích + Lưu → log Debug có `"IsSendExt":false` → backend KHÔNG tạo tiến trình gửi
- [ ] Lưu tất cả (All-in-one, `SaveAllProcess`) → mọi dịch vụ trong danh sách đều mang đúng giá trị `IsSendExt` theo checkbox
- [ ] Lưu lần đầu (`ID == 0` → `CreateSdo`) và lưu cập nhật (`UpdateSdo`) đều truyền `IsSendExt`
- [ ] Pacs Bách Khoa + `MOS.PACS.CONNECTION_TYPE = 2` (gửi qua file) + tích checkbox → backend gửi file
- [ ] Mở form phụ "Thông tin CLS/PTTT" (`frmClsInfo`) → Lưu → luôn gửi PACS (`IsSendExt = true`) bất kể checkbox ở màn cha
- [ ] Đổi ngôn ngữ sang English/Myanmar → caption và tooltip đổi đúng theo resx
- [ ] Độ phân giải 1366×768 → caption không bị cắt chữ, không đè `dtResult`

### Logging

- [ ] Khi enable restore: có log `Debug` với `isAllowRestoreLayout = True`
- [ ] Khi customize được lưu: có log `Info` "Customize layout saved: {layoutControlName} -> {fileName}"
- [ ] Mọi exception trong `InitRestoreLayout` / hook events → log `Warn`, KHÔNG ảnh hưởng quy trình chính
