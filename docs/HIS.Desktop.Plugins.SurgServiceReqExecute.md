# SurgServiceReqExecute — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.SurgServiceReqExecute |
| Loại | UserControl |
| Mục đích | Thực hiện yêu cầu dịch vụ phẫu thuật/thủ thuật (PT/PTTT): nhập kết quả, thời gian, ekip, ICD, ảnh, ký số EMR và in ấn. |
| Trạng thái | Đang bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Tiếp nhận yêu cầu dịch vụ PT (HIS_SERVICE_REQ với SERVICE_REQ_TYPE = PT).
2. Nhập thời gian y lệnh / bắt đầu / kết thúc PT, chỉ định kíp mổ, phương pháp, ICD, kết quả mô tả.
3. Cập nhật danh sách VTYT/thuốc tiêu hao (HIS_SERVICE_REQ_MATY hoặc qua HIS_EXP_MEST_MATERIAL — đơn trong kho).
4. Validate quan hệ thời gian giữa PT cha và các đơn VTYT con (DONDT / DONTT).
5. Lưu, in phiếu, ký số EMR.

### Validate thời gian PT ↔ VTYT con

Áp dụng cho các đơn VTYT con (DONDT — đơn điều trị, DONTT — đơn tủ trực) liên kết với PT cha qua `PARENT_ID`, chỉ khi chứa material có `IS_REQUIRE_TIME_VALIDATE = 1`.

Thứ tự thời gian kỳ vọng: `PT.INTRUCTION < VTYT.INTRUCTION < PT.START < VTYT.START < VTYT.FINISH < PT.FINISH`.

Phân loại điều kiện (cập nhật 2026-05-12):
- **Unconditional required (luôn check)**:
  - `VTYT.INTRUCTION_TIME > 0` — nếu thiếu, báo lỗi và bỏ qua các cross-check cho VTYT này.
  - `PT.INTRUCTION_TIME > 0` — đọc từ `this.serviceReq.INTRUCTION_TIME` của PT cha.
- **Conditional (chỉ check nếu trường đã có giá trị)**: `VTYT.START_TIME`, `VTYT.FINISH_TIME`, `PT.START_TIME`, `PT.FINISH_TIME`. Không bắt buộc nhập.
- **Cross-checks** — chỉ validate khi **cả hai** trường input của rule đều `> 0`:
  - R2: `PT.INTRUCTION < VTYT.INTRUCTION`
  - R3: `VTYT.INTRUCTION < PT.START`
  - R4: `PT.START < VTYT.START`
  - R5: `VTYT.START < VTYT.FINISH`
  - R6: `VTYT.FINISH < PT.FINISH`

### Chẩn đoán — nguyên nhân tử vong & không khuyến khích bệnh chính (Việc 2.6)

3 combo chẩn đoán dùng control tùy chỉnh (chung `dataIcds`): `cboIcd1` (CĐ chính), `cboIcd2` (CĐ trước PT), `cboIcd3` (CĐ sau PT).

- **Ẩn nguyên nhân tử vong** (`IS_DEATH_CAUSE_ONLY = 1`): lọc trong `DataToComboChuanDoanTD` (nhánh `dataIcds`) → ẩn khỏi cả 3 combo; giữ `dataIcds` đầy đủ để `FillDataToCboIcd` hiển thị giá trị đã lưu. Không áp dụng YHCT.
- **Cảnh báo `IS_NOT_RECOMMEND_MAIN = 1`**: chỉ cho **CĐ chính `cboIcd1`** khi chọn/sửa (`ChangecboChanDoanTD`, `LoadIcdCombo`, gate `cbo == cboIcd1`). Chọn Không → xóa, chọn lại.
- **Không kiểm tra khi lưu (B)**: nút KTĐT mở plugin `HIS.Desktop.Plugins.TreatmentFinish` riêng; kết quả/tử vong xử lý ở plugin đó.
- **CHƯA làm — chẩn đoán phụ**: popup phụ dùng shared plugin `HIS.Desktop.Plugins.SecondaryIcd` (load `HIS_ICD` riêng). Để ẩn death-cause ở popup phụ cần cập nhật shared plugin đó (ảnh hưởng nhiều chức năng + có vấn đề build licx/prebuild) → cần quyết định riêng.

### Lưu lược đồ (ảnh) vào Mẫu PTTT

Tab **Lược đồ** cho phép gắn ảnh vào dịch vụ (lưu file đính kèm `HIS_SERE_SERV_FILE` qua FSS). Khi lưu Mẫu PTTT (`btnSavePtttTemp`), ảnh được lưu cùng mẫu dưới dạng danh sách ID thư viện ảnh trong `HIS_SERE_SERV_PTTT_TEMP.TEXT_LIB_IDS`.

- **ADO ảnh `ImageADO`** có thêm `TextLibId` (long?, **runtime only — KHÔNG persist DB**): ID bản ghi thư viện `HIS_TEXT_LIB` nguồn nếu biết.
  - Tải ảnh từ máy (`btnCreateImageLuuDo`) → `TextLibId = null`.
  - Chọn ảnh từ thư viện (`btnImagePublic` → `SelectListImageTemp`) hoặc tải từ Mẫu PTTT (`cboPtttTemp_EditValueChanged` → `SelectListImageTemp`) → sau khi tạo file đính kèm, lưu mapping `ID file đính kèm → ID thư viện gốc`, rồi gán `TextLibId` cho ADO ảnh sau khi reload (`AssignTextLibIdToImageADOs`).
  - Tải lại ảnh khi mở dịch vụ (`ProcessLoadSereServFile`) → `TextLibId = null` (không biết ID gốc vì không persist).
- **Điều kiện cho lưu mẫu**: có ≥ 1 trường nghiệp vụ HOẶC ≥ 1 ảnh đính kèm. Nếu chỉ có ảnh → tạo `HIS_SERE_SERV_PTTT_TEMP` rỗng để lưu.
- **Khi Save trong `FormPtttTemp`** (theo pattern HisSereServPtttTemp.SaveProcess — build DTO + upload + POST cùng chỗ):
  - Mỗi ảnh: `TextLibId` có giá trị → dùng lại; null → đọc bytes (stream runtime, fallback tải từ URL), encode base64 → bytes UTF-8, POST `api/HisTextLib/Create` (loại ảnh `HIS_LIB_TYPE.ID__IMAGE`, nhãn = tên file, public trong khoa `IS_PUBLIC_IN_DEPARTMENT=1`, gán `DEPARTMENT_ID` hiện tại). Thành công → cập nhật `TextLibId`; **thất bại → cảnh báo và DỪNG, không tạo mẫu**.
  - Loại trùng → ghép chuỗi dấu phẩy → `TEXT_LIB_IDS` → POST `api/HisSereServPtttTemp/Create`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu dịch vụ PT và VTYT con |
| HIS_SERVICE_REQ_MATY | Table | VTYT tiêu hao khai báo theo service request |
| HIS_EXP_MEST / HIS_EXP_MEST_MATERIAL | Table | VTYT thực xuất kho (đơn trong kho) |
| HIS_MATERIAL_TYPE | Table | Loại vật tư — cờ IS_REQUIRE_TIME_VALIDATE |
| V_HIS_SERE_SERV_5 | View | Dịch vụ đã thực hiện kèm giá |
| HIS_SERE_SERV_PTTT | Table | Thông tin PTTT (stent, phương pháp) |
| HIS_SERE_SERV_PTTT_TEMP | Table | Mẫu PTTT — `TEXT_LIB_IDS` lưu danh sách ID thư viện ảnh (lược đồ) |
| HIS_SERE_SERV_FILE | Table | File đính kèm ảnh/lược đồ của dịch vụ (URL trỏ FSS) |
| HIS_TEXT_LIB | Table | Thư viện ảnh/văn bản — lược đồ lưu loại ảnh (`HIS_LIB_TYPE.ID__IMAGE`) |

## 4. UI Layout

UC chính: `SurgServiceReqExecuteControl` (UserControlBase).
Embedded UC: `HIS.UC.Icd`, `HIS.UC.SecondaryIcd`, `HIS.UC.DateEditor`, `UCEkipUser`.

## 5. API Endpoints

| Action | URI |
|--------|-----|
| Lấy service req con | HIS_SERVICE_REQ_GET |
| Lấy SERVICE_REQ_MATY | HIS_SERVICE_REQ_MATY_GET |
| Lấy EXP_MEST | HIS_EXP_MEST_GET |
| Lấy EXP_MEST_MATERIAL | HIS_EXP_MEST_MATERIAL_GET |
| Lấy/Tạo/Xóa file đính kèm ảnh | HIS_SERE_SERV_FILE_GET / _CREATE / _DELETE / _UPDATE |
| Tạo mẫu PTTT | api/HisSereServPtttTemp/Create |
| Tạo thư viện ảnh (lược đồ) | api/HisTextLib/Create |

Tập trung trong `RequestUriStore.cs`. Tất cả qua `ApiConsumers.MosConsumer`.

## 6. Dependencies

### Library Plugins
- `HIS.Desktop.Plugins.Library.EmrGenerate` — ký số EMR cho phiếu in.

### Inter-Plugin
- Mở dialog `frmEkipTemp`, `FormPtttTemp`, `FormPtttMethod`, `FormImageTemp` (cùng plugin).

## 7. Print

Nhiều phiếu in PTTT — quản lý qua `SurgServiceReqExecuteControl__Print_Init.cs` + `Base/PrintTypeCodeWorker.cs`, dùng `MPS.MpsPrinter.Run` trực tiếp với PDO tương ứng.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-12 | sinhnt | Nới điều kiện validate `ValidateVtytTimeWithParentPT`: chỉ INTRUCTION_TIME của VTYT và PT là bắt buộc; START_TIME / FINISH_TIME của cả VTYT và PT chuyển sang conditional — mỗi cross-check (R2–R6) chỉ chạy khi cả hai trường input của rule đều > 0. |
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi 3 combo CĐ (chính/trước/sau PT), giữ giá trị đã lưu (trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa CĐ chính `cboIcd1`; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi/en/my). Chẩn đoán phụ qua shared plugin SecondaryIcd chưa sửa. |
| 19/06/2026 | huyvu20 | **Lưu lược đồ vào Mẫu PTTT**: thêm `ImageADO.TextLibId` (runtime only); `SelectListImageTemp` build mapping file đính kèm → ID thư viện và gán `TextLibId` qua `AssignTextLibIdToImageADOs`; `btnSavePtttTemp` cho lưu mẫu khi chỉ có ảnh, truyền danh sách ảnh sang `FormPtttTemp`; `FormPtttTemp.btnSave` build `TEXT_LIB_IDS` (dùng lại `TextLibId` hoặc tạo mới qua `api/HisTextLib/Create`), thất bại thì dừng; thêm message `LuuLuocDoThatBaiKhongTheLuuMau` (vi/en/my). |
| 23/06/2026 | huyvu20 | **Fix lỗi không load được lược đồ khi chọn Mẫu PTTT**: `cboPtttTemp_EditValueChanged` đổi từ `BackendDataWorker.Get<HIS_TEXT_LIB>()` (filter null, thiếu `CAN_VIEW` → `CONTENT` null) sang gọi `api/HisTextLib/Get` với `filter.IDs` + `filter.CAN_VIEW = true` + `LIB_TYPE_ID` để backend trả về `CONTENT` (bytes ảnh). Thêm guard bỏ qua bản ghi `CONTENT` rỗng trong `SelectListImageTemp` (cả 2 nhánh nhóm/đơn) để tránh 1 bản ghi lỗi làm hỏng cả lô. |

## 9. Test Cases

### Validate thời gian PT ↔ VTYT (sau cập nhật)
- [ ] VTYT thiếu INTRUCTION_TIME → FAIL (unconditional).
- [ ] PT thiếu INTRUCTION_TIME → R2 không trigger (cross-check yêu cầu cả hai > 0).
- [ ] VTYT thiếu START_TIME và FINISH_TIME, có INTRUCTION_TIME hợp lệ → PASS.
- [ ] PT thiếu START_TIME → R3, R4 KHÔNG check, các rule khác vẫn áp dụng nếu đủ điều kiện.
- [ ] PT thiếu FINISH_TIME → R6 KHÔNG check.
- [ ] PT có START + FINISH, VTYT.START < PT.START → FAIL (R4).
- [ ] PT có START + FINISH, VTYT.FINISH > PT.FINISH → FAIL (R6).
- [ ] PT.INTRUCTION > VTYT.INTRUCTION (cả hai > 0) → FAIL (R2).
- [ ] VTYT có cả START và FINISH, FINISH < START → FAIL (R5).
- [ ] VTYT chỉ có START, không có FINISH → R5 KHÔNG check.

### Lưu lược đồ vào Mẫu PTTT
- [ ] Chọn ảnh từ thư viện → Lưu mẫu → mẫu có `TEXT_LIB_IDS` trỏ đúng ID thư viện gốc (không tạo bản ghi thư viện trùng mới).
- [ ] Tải ảnh từ máy → Lưu mẫu → tạo bản ghi `HIS_TEXT_LIB` mới (loại ảnh, public trong khoa) và `TEXT_LIB_IDS` chứa ID mới.
- [ ] Không nhập trường nghiệp vụ nào, chỉ có ảnh → vẫn cho lưu mẫu.
- [ ] Không có trường nghiệp vụ và không có ảnh → báo "Không có nội dung lưu mẫu".
- [ ] Tạo thư viện ảnh thất bại → cảnh báo `LuuLuocDoThatBaiKhongTheLuuMau`, KHÔNG tạo mẫu.
- [ ] Chọn mẫu PTTT có lược đồ → ảnh được tải về dịch vụ và `TextLibId` gán đúng; lưu lại mẫu khác dùng lại ID thư viện.
- [ ] Trùng ID ảnh → `TEXT_LIB_IDS` đã loại trùng.
