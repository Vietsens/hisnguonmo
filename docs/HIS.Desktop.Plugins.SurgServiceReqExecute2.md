# Xử lý thủ thuật (SurgServiceReqExecute2) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.SurgServiceReqExecute2 |
| Loại | UserControl (kế thừa HIS.Desktop.Utility.UserControlBase) |
| Mục đích | Xử lý yêu cầu thủ thuật trong phòng thủ thuật: chọn 1 y lệnh, cập nhật thông tin PTTT (phương pháp, vô cảm, kíp thực hiện, ICD, thời gian xử lý), lưu kết quả. |
| Người tạo | Inventec |
| Ngày tạo | 2026-05-20 (việc 45072) |
| Trạng thái | Đang phát triển |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở UC từ menu "Xử lý thủ thuật" tại phòng thủ thuật.
2. Bộ lọc: TG y lệnh từ/đến, Dịch vụ (multi-select), trạng thái (Tất cả/Chưa xử lý/Đang xử lý/Hoàn thành), Mã BN, từ khóa.
3. Click **Tìm (Ctrl+F)** → load danh sách y lệnh từ `api/HisSereServ/GetView8` (`V_HIS_SERE_SERV_8`).
4. Grid trái nhóm theo bệnh nhân, hiển thị các cột: trạng thái icon, mã y lệnh, tên DV, ngày chỉ định, ĐTTT, BS chỉ định, TG bắt đầu, TG kết thúc, đơn giá.
5. Click 1 dòng → bắt đầu xử lý: gọi `api/HisServiceReq/Start` (truyền y lệnh ID). Form phải hiển thị thông tin BN + load các field PTTT/EXT vào buffer (ICD, vô cảm, máy, mô tả, kết luận, ghi chú, BEGIN_TIME, END_TIME).
6. Click chuột phải vào dòng:
   - Trạng thái **Đang xử lý** → menu **"Hủy bắt đầu"** → gọi `api/HisServiceReq/UnStart`.
   - Trạng thái **Hoàn thành** → menu **"Hủy kết thúc"** → check EMR document (nếu config `AutoDeleteEmrDocumentWhenEditReq = "1"` và `IsHasConnectionEmr`), confirm, delete document, sau đó gọi `api/HisServiceReq/Unfinish`.
7. Bên phải nhập thông tin: phương pháp, vô cảm, máy, ICD, kíp thực hiện, mô tả/ghi chú/kết luận, TG xử lý.
8. Checkbox **KT** + click **Lưu (Ctrl+S)** → gọi `api/HisServiceReq/SurgUpdate` với `HisSurgServiceReqUpdateSDO` (bao gồm `SereServPttt`, `SereServExt`, `EkipUsers`, `IsFinished`).
9. Có thể chọn **Mẫu PTTT** từ combo để fill nhanh hoặc click **Lưu mẫu** để tạo mẫu mới (tái sử dụng `FormPtttTemp` từ plugin `HIS.Desktop.Plugins.SurgServiceReqExecute`).
10. Button **Danh sách y lệnh** → mở plugin `HIS.Desktop.Plugins.ServiceReqList` với `HIS_TREATMENT { ID = TREATMENT_ID }` của dòng đang chọn.

### Sơ đồ trạng thái y lệnh
```
Chưa xử lý (CXL) → Đang xử lý (DXL) → Hoàn thành (HT)
                       ↑ Hủy bắt đầu (UnStart)
                                          ↑ Hủy kết thúc (Unfinish — kèm xóa EMR document nếu cần)
```

### Điều kiện nghiệp vụ
- BEGIN_TIME mặc định theo cấu hình `HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq`:
  - `1` + y lệnh là Thủ thuật → INTRUCTION_TIME
  - `2` + y lệnh là Thủ thuật/Phẫu thuật → INTRUCTION_TIME
  - `3` + y lệnh là Thủ thuật/Phẫu thuật → DateTime.Now
- END_TIME tự tính = BEGIN_TIME + TG xử lý (phút) khi END_TIME chưa có và TG xử lý đã nhập.
- IsFinished khi Save:
  - Nếu KT chưa check → giữ logic cũ (`true`).
  - Nếu KT check + config `MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = "1"` + user KHÔNG phải bác sĩ (`HIS_EMPLOYEE.IS_DOCTOR != 1`) → ép `IsFinished = false`.
  - Ngược lại: nếu KT check + END_TIME có giá trị → `true`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERE_SERV_8 | View | Danh sách y lệnh thủ thuật (đổi từ V_HIS_SERE_SERV_1 — bổ sung TDL_PATIENT_TYPE_ID, TDL_REQUEST_USERNAME, TDL_REQUEST_LOGINNAME, BEGIN_TIME, END_TIME, PRICE, EMOTIONLESS_ID, MANNER, MACHINE_ID, CONCLUDE, INSTRUCTION_NOTE, DESCRIPTION, NOTE...) |
| V_HIS_SERE_SERV_PTTT | View | Thông tin PTTT đã lưu (ICD, EMOTIONLESS, MANNER) |
| HIS_SERE_SERV_PTTT | Table | Lưu khi Save (PTTT_GROUP_ID/METHOD_ID/REAL_METHOD_ID + ICD_*, EMOTIONLESS_METHOD_ID, MANNER) |
| HIS_SERE_SERV_EXT | Table | Lưu khi Save (BEGIN_TIME, END_TIME, MACHINE_ID, CONCLUDE, DESCRIPTION, NOTE, INSTRUCTION_NOTE, MACHINE_CODE) |
| HIS_SERE_SERV_PTTT_TEMP | Table | Mẫu PTTT (qua `BackendDataWorker.Get<HIS_SERE_SERV_PTTT_TEMP>()`) |
| HIS_EKIP_USER / HIS_EKIP_TEMP | Table | Kíp thực hiện |
| HIS_PATIENT_TYPE | Cache | Tên đối tượng thanh toán (ĐTTT) |
| V_EMR_DOCUMENT | View (EMR) | Văn bản ký số — check khi Hủy kết thúc |

## 4. UI Layout

### Sơ đồ giao diện
```
+-------------------------+-------------------------------------------+
| TG y lệnh: [from][to]   | Thông tin bệnh nhân                       |
| Dịch vụ:[multi] Trạng:[]| Mã | Tên | DOB | Số thẻ | KCBBĐ | Loại  |
| Mã BN [_] Từ khóa [Tìm] | Ghi chú                                   |
+-------------------------+-------------------------------------------+
| Grid trái — V_HIS_SERE_SERV_8                                       |
| (icon) Mã | Tên DV | Ngày | ĐTTT | BS CĐ | BĐ | KT | Đơn giá       |
| → click phải: "Hủy bắt đầu" / "Hủy kết thúc"                        |
+-------------------------+-------------------------------------------+
| Tổng số BN: X | Tổng số DV: Y         [Danh sách y lệnh]            |
+-------------------------+-------------------------------------------+
                          | Mẫu PTTT: [cboPtttTemp_v45072] [SaveTemp]    |
                          | CĐ chính: [Code][cboIcd1][txtIcd1][Sửa]      |
                          | CĐ phụ:   [Code][cboIcd2][txtIcd2][Sửa] + txt|
                          | ICD9 chính:[Code][cboIcdCm][txtIcdCm][Sửa]   |
                          | ICD9 phụ: [SubCode][SubName]                 |
                          | TG xử lý: [spnTimeProcess_v45072] phút       |
                          | Vô cảm chính: [cboEmotionLess_v45072]        |
                          | Cách thức: [txtMANNER_v45072 — MemoEdit]     |
                          | Máy: [txtMachineCode][cboMachine_v45072]     |
                          | Kết luận: [txtConclude_v45072 — MemoEdit]    |
                          | Ghi chú BS CĐ: [txtIntructionNote] ReadOnly  |
                          | Tab [Mô tả/Ghi chú]: txtDescription / txtResultNote |
                          | --- controls cũ vẫn giữ nguyên ---           |
                          | Khoa | Bắt đầu | Kết thúc                    |
                          | Phương pháp | Phương pháp 2                  |
                          | Phương pháp TT | Phân loại                   |
                          | Vô cảm phụ | Kíp mẫu / Kíp thực hiện         |
                          |                  [chkKT_v45072] [Lưu]        |
```

### Controls bổ sung Việc 45072 — Designer-driven, suffix `_v45072`
Toàn bộ controls bổ sung đã được CHUYỂN VÀO Designer.cs với suffix `_v45072` để tránh xung đột naming với controls Execute2 hiện có. Thay vì tạo runtime trong `___InitUI.cs`, các control hiện được sinh bởi `InitializeComponent()`. `___InitUI.cs` chỉ wire event handlers.

| Control | Type | Mục đích |
|---------|------|----------|
| `chkKT_v45072` | CheckEdit | Checkbox KT — góc dưới phải btnSave, ControlState persist trạng thái |
| `cboPtttTemp_v45072` | CustomGridLookUpEditWithFilterMultiColumn | Combo Mẫu PTTT |
| `btnSavePtttTemp_v45072` | SimpleButton | Lưu mẫu — mở `FormPtttTemp` từ plugin SurgServiceReqExecute |
| `lblTotalPatient_v45072` / `lblTotalService_v45072` | LabelControl | Tổng số BN / DV theo bộ lọc hiện tại |
| `btnDanhSachYLenh_v45072` | SimpleButton | Mở plugin ServiceReqList |
| `spnTimeProcess_v45072` | SpinEdit | TG xử lý (phút) — tự tính END_TIME từ BEGIN_TIME + phút |
| `txtIcdCode1` / `cboIcd1` / `txtIcd1` / `chkIcd1` (`_v45072`) | TextEdit / CustomGridLookUp / TextEdit / CheckEdit | ICD chính + nút Sửa toggle |
| `txtIcdCode2` / `cboIcd2` / `txtIcd2` / `chkIcd2` (`_v45072`) | (như trên) | ICD phụ |
| `txtIcdText` / `txtIcdExtraCode` (`_v45072`) | TextEdit | Text + mã bệnh phụ |
| `txtIcdCmCode` / `cboIcdCmName` / `txtIcdCmName` / `chkIcdCm` (`_v45072`) | (như trên) | ICD9 chính |
| `txtIcdCmSubCode` / `txtIcdCmSubName` (`_v45072`) | TextEdit | ICD9 phụ |
| `cboEmotionLess_v45072` | LookUpEdit | Vô cảm chính → EMOTIONLESS_METHOD_ID (KHÁC cboEmotionLessMethod cũ map sang SECOND_ID) |
| `txtMANNER_v45072` | MemoEdit | Cách thức |
| `cboMachine_v45072` / `txtMachineCode_v45072` | GridLookUpEdit / TextEdit | Máy thực hiện + mã máy |
| `txtConclude_v45072` | MemoEdit | Kết luận (MaxLength=1000) |
| `txtIntructionNote_v45072` | MemoEdit | Ghi chú BS CĐ (ReadOnly = true theo thiết kế "DISABLE") |
| `xtraTabControl_v45072` | XtraTabControl | Tab "Mô tả/Ghi chú" — 2 page |
| `txtDescription_v45072` | MemoEdit | Mô tả (trong tab Mô tả) |
| `txtResultNote_v45072` | MemoEdit | Ghi chú kết quả (trong tab Ghi chú) |

**Lưu ý naming và adapter:**
- Suffix `_v45072` để phân biệt với controls đã có sẵn trong Designer (đặc biệt `cboEmotionLessMethod` cũ vẫn dùng cho EMOTIONLESS_METHOD_SECOND_ID, `dteStart`/`dteFinish` cũ vẫn dùng cho BEGIN/END_TIME).
- Event handlers được wire trong `InitExtendedRuntimeControls()` ở `___InitUI.cs`.
- Buffer values fill vào controls khi click row qua `FillBufferToExtendedControls()` ở `___Extended.cs`.
- Type adapter: `cboPtttTemp` chuyển từ `LookUpEdit` runtime → `CustomGridLookUpEditWithFilterMultiColumn` Designer; `OnClickSavePtttTemp` đã update signature tương ứng và gọi `LoadDataToComboPtttTemp` thay `LoadDataToComboPtttTempSimple` (đã xóa).

### UC nội bộ
| UC | Mục đích |
|----|----------|
| frmEkipTemp | Lưu mẫu kíp thực hiện |
| HIS.Desktop.Plugins.SurgServiceReqExecute.PtttTemp.FormPtttTemp | Form lưu mẫu PTTT (qua reference DLL) |

## 5. API Endpoints

| Action | URI | Consumer | Filter / Body |
|--------|-----|----------|---------------|
| Lấy danh sách y lệnh | `api/HisSereServ/GetView8` | MosConsumer | `HisSereServView8Filter` |
| Bắt đầu y lệnh | `api/HisServiceReq/Start` | MosConsumer | `long serviceReqId` (TODO: chuyển sang `HisServiceReqStartSDO` khi BE phát triển) |
| Hủy bắt đầu | `api/HisServiceReq/UnStart` | MosConsumer | `long serviceReqId` |
| Hủy kết thúc | `api/HisServiceReq/Unfinish` | MosConsumer | `long serviceReqId` |
| Cập nhật PTTT | `api/HisServiceReq/SurgUpdate` | MosConsumer | `HisSurgServiceReqUpdateSDO` |
| Lấy view EMR | `api/EmrDocument/GetView` | EmrConsumer | `EmrDocumentViewFilter` |
| Xóa EMR document | `api/EmrDocument/Delete` | EmrConsumer | `long documentId` |
| Tạo mẫu PTTT | `api/HisSereServPtttTemp/Create` | MosConsumer | `HIS_SERE_SERV_PTTT_TEMP` |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Library.EmrGenerate | Tạo input ký số EMR cho in (Mps000102) |
| HIS.Desktop.Library.CacheClient | ControlStateWorker (persist checkbox KT) |
| HIS.Desktop.Plugins.SurgServiceReqExecute | Reference DLL để dùng FormPtttTemp |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.ServiceReqList | Click "Danh sách y lệnh" | `HIS_TREATMENT { ID = currentRow.TDL_TREATMENT_ID }` |

### Config Keys
| Key | Mục đích |
|-----|----------|
| `MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT` | Cho phép thu phí tại phòng thực hiện |
| `HIS.Desktop.Plugins.SurgServiceReqExecute.IsNotRequiredPtttExecuteRole` | Không bắt buộc kíp thực hiện |
| `HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime` | Cảnh báo vượt thời gian xử lý |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` | Mã đối tượng BHYT |
| `HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime` | TG bắt đầu > TG y lệnh |
| `HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq` | Tự xóa văn bản ký khi sửa y lệnh |
| `HIS.Desktop.Plugins.IsHasConnectionEmr` | Có module EMR |
| `HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq` | Default BEGIN_TIME (1/2/3) |
| `MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR` | Cho phép user không phải BS finish |

## 7. Print

Plugin tái sử dụng MPS000102 (phiếu thu tạm ứng) — gọi qua `Inventec.Common.RichEditor.RichEditorStore` khi flow thu phí tại phòng thực hiện kích hoạt. Build `Mps000102PDO` thủ công, có EMR sign input.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-23 | tuanln | **Việc 45072 (Fix nút x clear cả txt code — round 3)** — Test báo 4 chỗ "Phương pháp / Phương pháp 2 / Phương pháp TT / Phân loại" click x chỉ xóa combo, txt code bên trái VẪN CÒN giá trị ("00541", "02", "00541", "06"). Root cause: pattern cũ chỉ `cbo.EditValue = null` rồi dựa EditValueChanged nhánh `else { txtXxx.Text = null; }` để clear txt. Nhưng nếu user GÕ CODE TAY vào txt (combo `EditValue` đã null vì code không match data), set `EditValue = null` lần nữa KHÔNG trigger EditValueChanged → txt giữ nguyên giá trị user gõ. Fix: 4 handler `cboPtttMethod_ButtonClick` / `cboEmotionLessMethod_ButtonClick` / `cboPtttMethodReal_ButtonClick` / `cboPtttGroup_ButtonClick` (_Right.cs) — bổ sung clear explicit `txtXxx.Text = string.Empty` ngay sau set cbo.EditValue = null. Bỏ phụ thuộc EditValueChanged để đảm bảo clear cả 2 control kể cả khi combo đã null. **Build verified: PASS**. |
| 2026-05-23 | tuanln | **Việc 45072 (Fix nốt các nút x còn lại — round 2)** — Test báo nút x trên ICD đã OK nhưng các nút x khác trong form: cái thì xóa được 1 bên trong 2 bên, cái thì không xóa được. Soi từng combo có button Delete: (1) **BUG COPY-PASTE NGHIÊM TRỌNG** trong `cboPtttMethodReal_ButtonClick` (_Right.cs line 478-479): code set `cboEmotionLessMethod.EditValue = null` thay vì `cboPtttMethodReal.EditValue = null` → user click x trên "Phương pháp TT" thì combo "Phương pháp 2" (Vô cảm cũ) bị xóa, còn Phương pháp TT KHÔNG đổi. Đây là pattern "xóa 1 bên trong 2 bên" test báo. Sửa: đổi target sang `cboPtttMethodReal`. (2) **3 combo `_v45072` có button Delete trong Designer NHƯNG không có ButtonClick handler** → click x KHÔNG làm gì: `cboPtttTemp_v45072` (Mẫu PTTT, Designer line 928-930), `cboEmotionLess_v45072` (Vô cảm mới, line 1096-1098), `cboMachine_v45072` (Máy thực hiện, line 1118-1120). Thêm 3 handler `CboPtttTemp_v45072_ButtonClick` + `CboEmotionLess_v45072_ButtonClick` + `CboMachine_v45072_ButtonClick` trong `___Extended.cs` cùng vị trí với 2 handler ICD đã thêm trước. Wire trong `Wire45072Events()` (sau dòng wire `EditValueChanged` cho cboPtttTemp_v45072). Với cboPtttTemp: chỉ clear EditValue, KHÔNG ép xoá các field đã fill từ template (Vô cảm/Cách thức/Kết luận/Mô tả/Ghi chú) — user có thể đã sửa. Với cboEmotionLess_v45072: SyncCode handler đã wire trên EditValueChanged tự xử lý đồng bộ. Các combo cũ `cboPtttMethod` / `cboEmotionLessMethod` / `cboPtttGroup` hoạt động đúng (EditValueChanged đã có nhánh `else { txtXxx.Text = null; }` tự clear text khi cbo = null). **Build verified: PASS**. |
| 2026-05-23 | tuanln | **Việc 45072 (Fix bug ICD test báo)** — Test báo 2 lỗi: (1) Nút "x" trên CĐ chính + ICD9-CM chính KHÔNG xóa được giá trị. Root cause: Designer.cs đã add `ButtonPredefines.Delete` vào `cboIcdName_v45072.Properties.Buttons` (line 962-964) và `cboIcdCmName_v45072` (line 1017-1019) nhưng KHÔNG có handler nào wire `ButtonClick` event → click x không trigger gì. Fix: thêm 2 handler `CboIcdName_v45072_ButtonClick` + `CboIcdCmName_v45072_ButtonClick` trong `___Extended.cs`, wire trong `InitLookupIcd_v45072()` sau khối `WireIcdSync` — khi `e.Button.Kind == ButtonPredefines.Delete` → `cbo.EditValue = null` + clear `txtCode.Text = string.Empty`. `EditValueChanged` đã wire trong `WireIcdSync_v45072` tự sync txt theo cbo (defensive double-clear để chắc). (2) ICD KHÔNG default từ sere-serv (V_HIS_SERVICE_REQ) khi chưa có bản ghi PTTT — code cũ ở nhánh `else (sp == null)` chỉ gọi `ClearExtendedControls_v45072()` → các trường ICD rỗng. Fix theo pattern plugin gốc `SurgServiceReqExecute`: (a) Thêm field `currentServiceReq_v45072` (V_HIS_SERVICE_REQ) + method `LoadDataServiceReq_v45072()` trong `_LoadData.cs` — load qua `api/HisServiceReq/GetView` với `ID = currentRow.SERVICE_REQ_ID`, add vào `CreatThreadLoadDataInfor.methods` cùng `LoadDataPttt`/`LoadDataEkipUser`/`LoadDataPatientTypeAlter`. (b) Thêm method `DefaultIcdFromServiceReq_v45072()` trong `___Extended.cs` adapt 2 pattern của Execute: `SetIcdFromServiceReq` (CĐ chính + sub + text từ `serviceReq.ICD_CODE/ICD_SUB_CODE/ICD_TEXT`) + `SetDefaultCboICD9CmChinh` (ICD9-CM chính lookup `HIS_SERVICE.ICD_CM_ID` theo `currentRow.SERVICE_ID` → tra `HIS_ICD_CM`). (c) Sửa `FillExtendedDataWhenClickRow` nhánh `else` gọi `ClearExtendedControls_v45072()` rồi `DefaultIcdFromServiceReq_v45072()` thay vì chỉ clear. Phần "có thay đổi gì thì đổi" đã hoạt động sẵn — save logic ở `___Extended.cs` line 944-963 đọc từ control hiện tại nên user chọn/sửa sau khi default sẽ được lưu đúng. **Build verified: PASS** (ExitCode=0). |
| 2026-05-21 | tuanln | **Việc 45072 (Fix CRITICAL + HIGH issues sau review)** — Fix theo danh sách review: **CRITICAL**: (CR1) Sửa URI hardcode trong `___Popup.cs` từ `"api/HisServiceReq/UnStart"` / `"api/HisServiceReq/Unfinish"` → dùng constants từ `HisRequestUriStore` mới (verify chuẩn lib chung: `UnStart` chữ S hoa, `/Unfinish` có leading slash). (CR2) Verify field `this.sp` (V_HIS_SERE_SERV_PTTT) đã có sẵn ở `_LoadData.cs:116`, load qua `LoadDataPttt()` — KHÔNG cần thêm. (CR3) Bỏ reflection trong `FillView45072Fields` (`_Left.cs`) — verify qua dnSpy: V_HIS_SERE_SERV_1 có `PATIENT_TYPE_ID` (long, không có `TDL_*`), `TDL_REQUEST_USERNAME`/`TDL_REQUEST_LOGINNAME` (string), `PRICE` (Decimal) — chuyển hết sang strong-typed. (CR4) Batch pre-load `HIS_SERE_SERV_EXT` theo `SERE_SERV_IDs` (1 API call) trong `FillDataToGrid()` để fill `BEGIN_TIME_STR`/`END_TIME_STR` ngay lần đầu render — bỏ N+1 query khi click row. **HIGH**: (H1) Đổi format "Bác sĩ chỉ định" từ `"{0} ({1})"` → `"{0} - {1}"` theo thiết kế. (H2) Tạo `HisRequestUriStore.cs` tập trung 6 URI mới (UNSTART/UNFINISH/SERE_SERV_EXT_GET/EMR_DOCUMENT_GET_VIEW/EMR_DOCUMENT_DELETE/SERE_SERV_PTTT_GET) + re-declare 3 URI cũ (START/TREATMENT_GETFEEVIEW/PATIENT_GETVIEW) tránh shadow `HIS.Desktop.ApiConsumer.HisRequestUriStore`. (H3) Tạo `ModuleLinkString.cs` chứa `ServiceReqList`, `SurgServiceReqExecute`, `SurgServiceReqExecute2`; thay hardcode trong `BtnDanhSachYLenh_v45072_Click`. (H4) Thêm 5 keys vào Message.Lang.vi/en.resx (`TongSoBN`, `TongSoDichVu`, `HuyBatDau`, `HuyKetThuc`, `ChucNangLuuMauChuaKhaDung`) + property tương ứng trong `ResourceMessage.cs`; thay hardcode tiếng Việt ở `_Left.cs` (footer), `___Popup.cs` (menu items), `___PtttTemp.cs` (cảnh báo). (H5) Refactor `ComputeIsFinished_v45072` thành pure function — bỏ XtraMessageBox, tách logic show UI sang method mới `CheckCanFinishByDoctorRole_v45072()` gọi từ `btnSave_Click` TRƯỚC khi save (SRP — Compute không show UI). Thêm helper `IsCurrentUserDoctor_v45072()`. (H6) Clean up Assembly.Load trong `BtnSavePtttTemp_v45072_Click`: tách thành `OpenFormPtttTempByReflection_v45072()` với try-catch + LogSystem.Warn chi tiết khi Assembly.Load/GetType/CreateInstance fail, có XML comment giải thích lý do KHÔNG dùng PluginInstance (FormPtttTemp là Form thường, không có Processor riêng); cảnh báo user dùng ResourceMessage. (H7) Thêm `LogActionSuccess` audit cho cả Unstart và Unfinish (loginName từ `ClientTokenManagerStore`). (H8) Gọi `UpdateFooter45072()` sau khi update grid trong Unstart/Unfinish để footer Tổng BN/DV refresh. csproj: thêm `HisRequestUriStore.cs` + `ModuleLinkString.cs`. **Build verified: PASS** (ExitCode=0). |
| 2026-05-21 | tuanln | **Việc 45072 (Code-behind đầy đủ 8 nhóm chức năng)** — Hoàn thành code-behind sau khi Designer đã có sẵn controls `_v45072`: (1) **Anh1.1 Grid 5 cột**: thêm `FillView45072Fields()` trong `_Left.cs` map PATIENT_TYPE_NAME (Dictionary lookup O(1)), REQUEST_DOCTOR_DISPLAY (reflection-safe vì V_HIS_SERE_SERV_1 có/không có TDL_REQUEST_*), BEGIN_TIME_STR/END_TIME_STR (rỗng — V1 không có, fill khi click row qua HIS_SERE_SERV_EXT), PRICE_V45072 (reflection). (2) **Anh1.2 Footer**: `UpdateFooter45072()` set lblTotalPatient = distinct(TDL_PATIENT_ID) + lblTotalService = count(rows). (3) **Anh1.3 Start SDO**: đổi 2 chỗ gọi `api/HisServiceReq/Start` từ truyền `long serviceReqId` sang `HisServiceReqStartSDO { ID = ... }` (xác nhận property là `ID` không phải `Id`). (4) **Anh1.4 Context menu** (`___Popup.cs` mới): ContextMenuStrip right-click, "Hủy bắt đầu" cho DXL (`api/HisServiceReq/UnStart`), "Hủy kết thúc" cho HT — check `AutoDeleteEmrDocumentWhenEditReq` + `IsHasConnectionEmr` → load `api/EmrDocument/GetView` (EmrConsumer, filter `TREATMENT_CODE__EXACT` + `DOCUMENT_TYPE_ID = ID__SERVICE_RESULT`), lọc `HIS_CODE.Contains("SERVICE_REQ_CODE:" + TDL_SERVICE_REQ_CODE)`, confirm → loop delete → gọi `api/HisServiceReq/Unfinish`. (5) **Anh2 Combo Mẫu PTTT** (`___PtttTemp.cs` mới): `LoadDataToComboPtttTemp_v45072()` filter `IS_PUBLIC=1 OR (IS_PUBLIC_IN_DEPARTMENT=1 && DEPARTMENT_ID=room.DEPARTMENT_ID) OR CREATOR=loginName`, dùng `ControlEditorLoader.Load` + 2 cột Mã/Tên. `CboPtttTemp_v45072_EditValueChanged` fill cboEmotionLess, txtManner, txtConclude, txtDescription, txtNote (bỏ qua FSS TEXT_LIB_IDS — TODO ở phần dưới). (6) **Anh3-4 Lưu mẫu**: `GetDataForTemp_v45072()` build HIS_SERE_SERV_PTTT_TEMP từ controls, validate "không có nội dung" → cảnh báo. Click button → mở `FormPtttTemp` qua reflection (Assembly.Load("HIS.Desktop.Plugins.SurgServiceReqExecute") + Activator.CreateInstance) — tránh ràng buộc cứng. Reload combo sau popup. (7) **Anh5 Fill controls khi click row** (`___Extended.cs` mới): `FillExtendedDataWhenClickRow()` gọi cuối `ShowInforPatient()` — fill 4 ICD từ `this.sp` (V_HIS_SERE_SERV_PTTT đã load ở _LoadData), EMOTIONLESS_METHOD_ID + MANNER. Load HIS_SERE_SERV_EXT qua `api/HisSereServExt/Get` (HisSereServExtFilter.SERE_SERV_ID), fill MACHINE_ID, INSTRUCTION_NOTE, CONCLUDE, DESCRIPTION, NOTE. Begin/End time logic: ext có giá trị → dùng; không có → check config TakeIntrucionTimeByServiceReq (1=Thủ thuật/2=Cả PT&TT → INTRUCTION_TIME; 3 → DateTime.Now). spnTimeProcess/dteStart EditValueChanged → recompute dteFinish = dteStart + minutes. (8) **Anh6 Save SDO mở rộng + IsFinished**: thêm `FillPtttFields_v45072(pttt)` (4 ICD + ICD_NAME lookup từ HIS_ICD + EMOTIONLESS_METHOD_ID + MANNER) và `FillExtFields_v45072(ext)` (MACHINE_ID + MACHINE_CODE lookup từ HIS_MACHINE + 4 memo) — gọi trong `btnSave_Click` _Right.cs. `ComputeIsFinished_v45072()`: nếu config ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR="1" && user không phải BS (V_HIS_EMPLOYEE.IS_DOCTOR != 1) → hiện cảnh báo + false; ngược lại = (chkKT_v45072.Checked && dteFinish có giá trị). **ControlState** cho chkKT_v45072: thêm fields trong UCSurgServiceReqExecute2.cs + InitControlState_v45072() trong Load + ChkKT_v45072_CheckedChanged lưu vào SQLite qua ControlStateWorker. Config bổ sung: AutoDeleteEmrDocumentWhenEditReq, IsHasConnectionEmr, TakeIntrucionTimeByServiceReq, AllowFinishWhenAccountIsDoctor. Resources mới: Resources/Message.Lang.vi/en.resx + ResourceMessage.cs cho 6 message riêng plugin. csproj thêm references EMR.EFMODEL/Filter, IMSys.DbConfig.EMR_RS, Inventec.Common.Resource + ProjectReference HIS.Desktop.Library.CacheClient. **Build verified: PASS** — dll tạo thành công, chỉ còn warnings DevExpress.XtraTab benign. **TODO**: (a) Upload ảnh FSS từ TEXT_LIB_IDS khi chọn mẫu — đã skip với TODO comment. (b) ICD_NAME lookup hiện đơn giản qua HIS_ICD by ICD_CODE — đầy đủ logic validation/sub-code cần copy từ Plus_ICD của plugin Execute. |
| 2026-05-20 | tuanln | **Việc 45072 (Hint text mờ + label "phút" riêng)** — (1) Anh y/c "Nhấn F1 để chọn bệnh phụ" phải là **hint mờ** (placeholder, biến mất khi click vào) giống "Nhập mã bệnh ph" ở ô cạnh — KHÔNG phải text đậm thật. Sửa `cboIcdText_v45072`: `Properties.NullText = ""` (rỗng) + `Properties.NullValuePrompt = "Nhấn F1 để chọn bệnh phụ"` + `NullValuePromptShowForEmptyValue = true` → text xám mờ, tự biến mất khi focus/nhập. (2) Spin TG xử lý hiển thị "0 phút" BÊN TRONG editor là sai — anh y/c spin chỉ số + chữ "phút" RIÊNG ở NGOÀI. Sửa: bỏ `DisplayFormat = "#,##0 phút"` → `"#,##0"`. Thêm `lblPhut_v45072` (LabelControl Text="phút") + `lciLblPhut_v45072` (LayoutControlItem Size 50x28) cạnh spin. Thu nhỏ `lciTimeProcess_v45072.Size` 225→175 (chừa 50px cho label). Thêm declarations + BeginInit/EndInit + Add vào layoutControl3.Controls + Add vào layoutControlGroup3.Items + private fields cuối file. **Build verified: PASS**. |
| 2026-05-20 | tuanln | **Việc 45072 (Fix checkbox KT — chữ TRƯỚC, checkbox SAU)** — Em hiểu nhầm ý anh ở lần trước: anh muốn `KT: ☐` (chữ "KT:" trước, checkbox trống sau) — đây là pattern chuẩn HIS form. Rollback: `chkKT_v45072.Properties.Caption = ""` (xóa caption nội bộ), `lciChkKT_v45072.Text = "KT:"` + `TextVisible = true` + `TextAlignMode = CustomSize` + `TextSize = (30, 20)` + `HAlignment = Far` (caption căn phải, sát checkbox). Render đúng "KT: ☐". **Build verified: PASS**. |
| 2026-05-20 | tuanln | **Việc 45072 (UI polish — icon "Lưu mẫu" + đảo checkbox KT)** — Anh y/c: (1) Button "Lưu mẫu" sau Mẫu PTTT phải là icon nhỏ giống icon 💾 sau Kíp mẫu (btnSaveEkip), KHÔNG phải button to có chữ. Sửa `btnSavePtttTemp_v45072`: bỏ Text="Lưu mẫu", set `Image = resources.GetObject("btnSaveEkip.Image")` (tái dùng image cùng từ resx), `ImageLocation = MiddleCenter`, Size 22x20. LayoutItem `lciBtnSavePtttTemp_v45072`: thu nhỏ về Size 28x33 + `MinSize/MaxSize = 28x26` + `SizeConstraintsType = Custom` (pattern giống lciBtnSaveEkip = item33). Mở rộng `lciPtttTemp_v45072` từ 309 → 372 để combo Mẫu PTTT chiếm nhiều space hơn. (2) Checkbox KT đang là `KT: ☐` (text trước, checkbox sau) — anh muốn `☐ KT` (checkbox trước, text sau). Sửa: `chkKT_v45072.Properties.Caption = "KT"` (bỏ dấu hai chấm), `Properties.GlyphAlignment = HorzAlignment.Default` (Default = Near/Left, đặt glyph trước text). **Build verified: PASS**. |
| 2026-05-20 | tuanln | **Việc 45072 (Layout 2 cột khớp Anh5)** — Anh báo form chưa khớp thiết kế Anh5 (Vô cảm sai vị trí, Bắt đầu/Kết thúc/Phương pháp/PPTT chiếm full width). Re-layout panel right (`layoutControlGroup3` 858x423) thành 2 cột (trái 0-428, phải 428-858): (1) Y=89 Khoa(trái) \| TG xử lý(phải, mở rộng full 430). (2) Y=117 Bắt đầu(trái 428) \| Kết thúc(phải 430). (3) Y=145 Phương pháp txt+cbo(trái 160+268) \| Phương pháp 2 txt+cbo(phải 160+270). (4) Y=171 Phương pháp TT(trái) \| Phân loại(phải). (5) Y=197 Vô cảm chuyển từ Y=89 → cột TRÁI \| Máy thực hiện chuyển từ Y=306 → cột PHẢI. (6) Y=225 Cách thức cột TRÁI. (7) Y=254 Kết luận cột TRÁI. (8) Y=282 Ghi chú BSCĐ cột TRÁI. (9) Tab Mô tả/Ghi chú cột PHẢI Y=225, height 198 (cover Cách thức+Kết luận+Ghi chú BSCĐ). (10) Y=310 Kíp mẫu(trái) + btnSaveEkip. (11) Y=338 Grid Kíp thực hiện cột TRÁI height 85. **Build verified: PASS**. |
| 2026-05-20 | tuanln | **Việc 45072 (Layout fix — Mẫu PTTT caption)** — Sửa `lciPtttTemp_v45072.TextSize` từ `(110, 20)` → `(70, 20)` để khớp thiết kế Anh5. Lý do: caption "Mẫu PTTT:" chỉ ~70px nhưng TextSize=110px tạo khoảng trống ~40px bên trái combo, trông như có 1 ô input thừa ở góc trên-trái khu vực bên phải (groupControl2). Sau fix: caption sát combo, layout sạch hơn — combo chiếm 519-70-5 = 444px (rộng hơn 40px). **Build verified: PASS**. |
| 2026-05-20 | tuanln | **Việc 45072 (Designer visibility fix)** — Sau build PASS, anh báo controls `_v45072` KHÔNG hiển thị trong Designer view: subagent đã add vào `groupControl2.Controls` nhưng bị `layoutControl3` (Dock=Fill) che. Sửa: (1) Xóa toàn bộ `groupControl2.Controls.Add(_v45072)` + `BringToFront()` khỏi Designer.cs. (2) Tạo file mới `UCSurgServiceReqExecute2___DesignerExt.cs` với method `AddV45072LayoutItems()` — runtime tạo `LayoutControlGroup` mới `lcgV45072` chứa toàn bộ 33 controls dưới dạng `LayoutControlItem`, add vào `layoutControlGroup3` (root của layoutControl3) bằng `AddGroup()`. (3) Footer grid trái (`lblTotalPatient_v45072`, `lblTotalService_v45072`, `btnDanhSachYLenh_v45072`) add vào parent của gridControl1 với Anchor Bottom. (4) `chkKT_v45072` add cạnh btnSave. (5) Gọi `AddV45072LayoutItems()` đầu Load event (trước `InitExtendedRuntimeControls`). Layout sẽ tự co giãn theo LayoutControl behavior — không còn dùng absolute position. **Build verified: PASS** — `HIS.Desktop.Plugins.SurgServiceReqExecute2.dll` đã rebuild. Anh mở UC trong VS Designer hoặc runtime sẽ thấy nhóm "PTTT - Việc 45072" với đầy đủ Mẫu PTTT, 4 ICD, TG xử lý, Vô cảm, Cách thức, Máy, Kết luận, Ghi chú BS CĐ, tab Mô tả/Ghi chú, checkbox KT. |
| 2026-05-20 | tuanln | **Việc 45072 (Build fix)** — Sửa các lỗi compile sau khi copy Designer: (1) Filter query đổi từ `HisSereServView8Filter` → `HisSereServView1Filter` (filter view 8 chưa support `EXECUTE_ROOM_IDs`, `SERVICE_REQ_TYPE_ID`, `SERVICE_IDs`, `TDL_PATIENT_CODE`). Sau khi nhận `V_HIS_SERE_SERV_1`, map sang `SereServView1ADO` (kế thừa V_HIS_SERE_SERV_8) bằng `DataObjectMapper.Map` — các field bổ sung của V8 sẽ tự null cho đến khi BE thêm vào view. (2) `LAST_DEPARTMENT_ID` không có ở V1/V8 → dùng reflection-safe tới `TDL_EXECUTE_DEPARTMENT_ID`. (3) `___Popup.cs`: thêm `using DevExpress.XtraGrid.Views.Grid.ViewInfo` (GridHitInfo), `using Inventec.Desktop.Common.Message` (WaitingManager/MessageManager). Đổi `MessageManager.Show(this, ...)` → `MessageManager.Show(this.ParentForm, ...)` (UC khác Form). Đổi `filter.TREATMENT_CODE` → `filter.TREATMENT_CODE__EXACT` (đúng property name của EmrDocumentViewFilter). `row.SERVICE_REQ_STT_ID` là `long` (không nullable) → bỏ `.HasValue` check. (4) Csproj: HintPath `HIS.Desktop.Plugins.SurgServiceReqExecute` đổi từ `LIB\` (không tồn tại) sang `..\HIS.Desktop.Plugins.SurgServiceReqExecute\bin\Debug\` + `<Private>False</Private>`. **Build verified: PASS** — `bin\Debug\HIS.Desktop.Plugins.SurgServiceReqExecute2.dll` đã tạo thành công. |
| 2026-05-20 | tuanln | **Việc 45072 (Designer step)** — Chuyển toàn bộ controls bổ sung từ runtime (`___InitUI.cs`) vào Designer.cs với suffix `_v45072`. Thêm 30+ field declarations + BeginInit/EndInit. Thêm controls vào `groupControl2` (panel phải) + `layoutControl1` (góc grid trái). Wire event handlers trong `InitExtendedRuntimeControls()`. Thêm `FillBufferToExtendedControls()` (trong `___Extended.cs`) để load buffer values lên controls mới khi click row. Xóa `LoadDataToComboPtttTempSimple` (không cần nữa). Adapter: `OnClickSavePtttTemp` đổi signature `LookUpEdit` → `CustomGridLookUpEditWithFilterMultiColumn`. Csproj: thêm reference `DevExpress.XtraTab.v15.2`; sửa HintPath `HIS.Desktop.Library.CacheClient` về `..\HIS.Desktop.Library.CacheClient\bin\Debug\`. Build verified: ZERO new errors trong Designer.cs/`___InitUI.cs`/`___Extended.cs` (pre-existing errors trong `_Left.cs/___Popup.cs/___PtttTemp.cs` không liên quan task này). |
| 2026-05-20 | tuanln | **Việc 45072** — Đổi V_HIS_SERE_SERV_1 sang V_HIS_SERE_SERV_8; thêm 5 cột grid (ĐTTT, BS chỉ định, BĐ, KT, Đơn giá) + Tổng số BN/DV + button Danh sách y lệnh; thêm context menu "Hủy bắt đầu" / "Hủy kết thúc" (kèm xử lý xóa EMR document); thêm combo Mẫu PTTT + button Lưu mẫu (tái sử dụng FormPtttTemp); thêm checkbox KT + ControlState persist; mở rộng SurgUpdate SDO truyền thêm ICD_*, ICD_CM_*, EMOTIONLESS_METHOD_ID, MANNER (PTTT), MACHINE_ID/MACHINE_CODE, INSTRUCTION_NOTE, CONCLUDE, DESCRIPTION, NOTE (EXT); IsFinished tính theo KT + config ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR; logic default BEGIN_TIME theo TakeIntrucionTimeByServiceReq; END_TIME tự tính từ BEGIN_TIME + TG xử lý. **Logic core copy từ plugin HIS.Desktop.Plugins.SurgServiceReqExecute**: `LoadDataToComboPtttTemp` (Combo.cs:475-500), `cboPtttTemp_EditValueChanged` (Control.cs:3560-3599), `GetDataForTemp`/`btnSavePtttTemp_Click` (Process.cs:1400-1524, Control.cs:3537-3558), `ProcessSereServPttt`/`ProcessSereServExt` (Process.cs:564-820, 916-965). Đã adapt: FormBase→UserControlBase, this.Module→this.moduleData, set field theo controls Execute2 hiện có. |

### Phần cần TODO (chưa copy đầy đủ từ Execute):
1. **Validation + binding logic cho 4 ô ICD** — Designer.cs Execute2 đã có controls `cboIcd1/2`, `txtIcdCode1/2`, `cboIcdCmName`, `chkIcd1/2/IcdCm` (suffix `_v45072`). Còn TODO: copy logic `ComboMethodICD`, `DataToComboChuanDoanTD`, `FillDataToCboIcd` từ `SurgServiceReqExecuteControl___Plus_ICD.cs` để bind data từ BackendDataWorker vào combo + validate code/name match. Hiện tại chỉ có event sync buffer.
2. **Upload ảnh FSS + tab Lược đồ** — `SelectListImageTemp` (stub trong Execute2). Cần copy logic upload Inventec.Fss.Client.FileUpload.UploadFile + tạo HIS_SERE_SERV_FILE + reload grid card từ Execute. (Tab Lược đồ chưa thêm vào Designer — xtraTabControl_v45072 hiện chỉ có Mô tả/Ghi chú; cần thêm xtraTabPageLuocDo_v45072 + CardControl khi triển khai).
3. **HisServiceReqStartSDO** — BE chưa có. Hiện vẫn truyền `long serviceReqId` (giữ behavior Execute hiện tại).
4. **Controls UI bổ sung Designer.cs**: ĐÃ HOÀN THÀNH ở Việc 45072 (Designer step) — txtConclude_v45072, txtDescription_v45072, txtResultNote_v45072, txtIntructionNote_v45072, txtMachineCode_v45072, cboMachine_v45072, txtMANNER_v45072, spnTimeProcess_v45072, cboEmotionLess_v45072, 4 ô ICD, lblTotal*, btnDanhSachYLenh, chkKT. Tất cả wire event và fill buffer đã có.
5. **Load data cho cboMachine_v45072 và cboEmotionLess_v45072**: chưa có method bind data từ `BackendDataWorker.Get<HIS_MACHINE>()` / `Get<HIS_EMOTIONLESS_METHOD>()` vào combo. Cần thêm vào `InitCombo` flow (AddDataToCombo) — bind sau khi InitializeComponent xong.
6. **Layout positioning**: Controls bổ sung hiện đặt absolute position trong `groupControl2` (Location point). Có thể cần dồn vào `LayoutControlGroup_v45072` riêng để layout tự co giãn — designer tay trên VS Designer thuận tiện hơn.

## 9. Test Cases

### V_HIS_SERE_SERV_8
- [ ] Load grid: cột ĐTTT hiển thị tên đối tượng từ HIS_PATIENT_TYPE
- [ ] Cột BS chỉ định hiển thị "USERNAME (LOGINNAME)" — null-safe
- [ ] Cột BĐ/KT format `dd/MM/yyyy HH:mm`
- [ ] Cột Đơn giá format tiền tệ

### Tổng số / Danh sách y lệnh
- [ ] Tổng số BN = số bệnh nhân DISTINCT theo bộ lọc
- [ ] Tổng số DV = số dòng grid
- [ ] Click "Danh sách y lệnh" khi không chọn row → cảnh báo
- [ ] Click "Danh sách y lệnh" → mở plugin ServiceReqList với đúng treatment

### Context menu
- [ ] Click phải y lệnh DXL → menu "Hủy bắt đầu" → API UnStart thành công → grid hiện trạng thái CXL
- [ ] Click phải y lệnh HT (không có EMR doc) → menu "Hủy kết thúc" → API Unfinish thành công → grid hiện DXL
- [ ] Click phải y lệnh HT (có EMR doc trùng SERVICE_REQ_CODE) → confirm dialog → chọn No → return
- [ ] Click phải y lệnh HT + Yes confirm → EMR docs xóa hết → API Unfinish → grid update

### Mẫu PTTT
- [ ] Combo Mẫu PTTT load đủ 3 nhóm: IS_PUBLIC = 1, IS_PUBLIC_IN_DEPARTMENT = 1 (đúng khoa), CREATOR = user hiện tại
- [ ] Chọn mẫu → fill phương pháp vô cảm, mô tả, kết luận, ghi chú, cách thức
- [ ] Click "Lưu mẫu" khi form trống → cảnh báo "Không có nội dung"
- [ ] Click "Lưu mẫu" có data → mở FormPtttTemp, nhập mã/tên/phạm vi → lưu → combo refresh

### Checkbox KT
- [ ] Lần đầu mở UC: KT theo trạng thái lưu lần trước (mặc định bỏ chọn)
- [ ] Toggle KT → trạng thái lưu vào SQLite
- [ ] Mở UC lần sau → KT giữ trạng thái

### Save SDO
- [ ] Save có KT check + END_TIME → IsFinished = true
- [ ] Save có KT check + config ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = "1" + user không phải BS → IsFinished = false
- [ ] Save KT không check → IsFinished = true (như cũ)
- [ ] Body SurgUpdateSDO.SereServPttt có ICD_*, EMOTIONLESS_METHOD_ID, MANNER
- [ ] Body SurgUpdateSDO.SereServExt có MACHINE_ID, MACHINE_CODE, NOTE, INSTRUCTION_NOTE, DESCRIPTION, CONCLUDE
