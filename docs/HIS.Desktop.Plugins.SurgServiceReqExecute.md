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

### Mps000324 — Phiếu thanh toán PT/TT

Builder: `Mps000324(printTypeCode, fileName, ref result)`. PDO: `MPS.Processor.Mps000324.PDO.Mps000324PDO`.

**Key/dataset bổ sung (2026-08-20)** — mẫu `Mps000324.xlsx` cũ không tham chiếu nên vẫn chạy y nguyên; mẫu mới muốn dùng thì thêm tag tương ứng.

Dataset master-detail mới (song song với `ServiceTypes` ↔ `SereServFollow` cũ):

| Dataset | Trường | Ý nghĩa |
|---------|--------|---------|
| `Groups` | `ID`, `NUM_ORDER`, `NUM_ORDER_ROMAN`, `SERVICE_TYPE_ROMAN`, `SERVICE_TYPE_CODE`, `SERVICE_TYPE_NAME`, `ITEM_COUNT`, `TOTAL_AMOUNT` | Nhóm theo `HIS_SERVICE_TYPE`, kèm số La Mã + tổng tiền nhóm |
| `Items` | `GROUP_ID`, `NUM_ORDER`, `NUM_ORDER_IN_GROUP`, `SERE_SERV_ID`, `SERVICE_CODE`, `SERVICE_NAME`, `SERVICE_UNIT_NAME`, `AMOUNT`, `PRICE`, `INTO_MONEY`, `IS_EXPEND`, `NOTE` | Dòng chi tiết; `INTO_MONEY = AMOUNT × PRICE`, `NOTE` = "Hao Phí"/"Thu Phí" |
| `EkipRoles` | `EXECUTE_ROLE_ID`, `EXECUTE_ROLE_CODE`, `EXECUTE_ROLE_NAME`, `NUM_ORDER`, `USER_COUNT`, `USERNAMES`, `LOGINNAMES`, `IS_SURG_MAIN` | **Vai trò kíp mổ lấy từ bảng `HIS_EXECUTE_ROLE`** — mẫu in KHÔNG hardcode mã vai |

Quan hệ: `Groups.ID` ↔ `Items.GROUP_ID`. `PRICE`/`INTO_MONEY`/`TOTAL_AMOUNT` để `null` khi không có giá → ô trên mẫu để trống.

**Khối kíp mổ — 2 cách dùng:**
- Khối động (khuyến nghị): lặp `<#EkipRoles.EXECUTE_ROLE_NAME;>` + `<#EkipRoles.USERNAMES;>`. Nhãn vai trò lấy từ danh mục nên đổi danh mục là mẫu tự đổi theo. Vai trò không có người vẫn liệt kê với `USER_COUNT = 0` → lọc bằng FlexCel filter nếu chỉ muốn vai có người.
- Ô chữ ký vị trí cố định: dùng key theo mã vai `USERNAMES_EXECUTE_ROLE_{CODE}` + nhãn `EXECUTE_ROLE_NAME_{CODE}` (nhãn cũng lấy từ danh mục).

Danh mục lấy qua `BackendDataWorker.Get<HIS_EXECUTE_ROLE>()`, fallback `api/HisExecuteRole/Get` rồi nạp lại cache (`GetExecuteRolesForMps000324`). Mất danh mục → fallback dùng `EXECUTE_ROLE_NAME` sẵn có trên `V_HIS_EKIP_USER`, ghi `LogSystem.Warn`.

Single key mới:

| Key | Nguồn |
|-----|-------|
| `BARCODE_IN_CODE_STR`, `BARCODE_TREATMENT_CODE_STR` | Ảnh Code128 từ `HIS_TREATMENT.IN_CODE` / `TREATMENT_CODE` |
| `BED_CODE_STR`, `BED_NAME_STR`, `BED_ROOM_NAME_STR`, `BED_ROOM_BED_STR` | `V_HIS_BED_LOG` mới nhất của lần điều trị |
| `START_TIME_SEPARATE_STR`, `FINISH_TIME_SEPARATE_STR` | "08 giờ 00 phút, Ngày 27 tháng 05 năm 2026" |
| `TICKET_NUMBER_STR` | `SERVICE_REQ_CODE + " - " + NUM_ORDER` |
| `PTTT_NOTE_STR` | `V_HIS_SERE_SERV_PTTT.OTHER` |
| `MAIN_SERVICE_NAME_STR` | `sereServ.TDL_SERVICE_NAME` (tên PT/TT chính) |
| `REAL_PTTT_METHOD_STR` | `REAL_PTTT_METHOD_CODE + " " + REAL_PTTT_METHOD_NAME` |
| `GRAND_TOTAL_AMOUNT` | Tổng chi phí các khoản |
| `USERNAMES_EXECUTE_ROLE_{CODE}`, `LOGIN_NAMES_EXECUTE_ROLE_{CODE}` | Gộp **tất cả** thành viên cùng vai bằng `", "` (khác `USERNAME_EXECUTE_ROLE_{CODE}` cũ chỉ giữ 1 người) |
| `EXECUTE_ROLE_NAME_{CODE}` | Tên vai trò lấy từ `HIS_EXECUTE_ROLE` — nhãn khối ekip không viết cứng trên mẫu |

Key đã có sẵn nhưng mẫu cũ chưa dùng — không cần code thêm: `TDL_HEIN_CARD_NUMBER` (thẻ BHYT), `PATIENT_TYPE_NAME` (đối tượng), `SERVICE_REQ_CODE`, `TDL_SERVICE_NAME`, `REAL_PTTT_METHOD_CODE/NAME`, `OTHER`; và các key chung `CURRENT_DATE_SEPARATE_STR`, `CURRENT_TIME_STR`, `CURRENT_LOGINNAME`, `CURRENT_USERNAME`.

**Deploy**: sửa PDO → build `MPS.Processor.Mps000324.PDO` rồi copy DLL vào `lib/MPSv2/MPS.PDO/` (plugin tham chiếu qua HintPath này).

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-12 | sinhnt | Nới điều kiện validate `ValidateVtytTimeWithParentPT`: chỉ INTRUCTION_TIME của VTYT và PT là bắt buộc; START_TIME / FINISH_TIME của cả VTYT và PT chuyển sang conditional — mỗi cross-check (R2–R6) chỉ chạy khi cả hai trường input của rule đều > 0. |
| 16/06/2026 | huyvu20 | **Việc 2.6**: Ẩn chẩn đoán nguyên nhân tử vong (`IS_DEATH_CAUSE_ONLY`) khỏi 3 combo CĐ (chính/trước/sau PT), giữ giá trị đã lưu (trừ YHCT); cảnh báo `IS_NOT_RECOMMEND_MAIN` khi chọn/sửa CĐ chính `cboIcd1`; thêm message `BenhKhongKhuyenKhichDungLamBenhChinh` (vi/en/my). Chẩn đoán phụ qua shared plugin SecondaryIcd chưa sửa. |
| 19/06/2026 | huyvu20 | **Lưu lược đồ vào Mẫu PTTT**: thêm `ImageADO.TextLibId` (runtime only); `SelectListImageTemp` build mapping file đính kèm → ID thư viện và gán `TextLibId` qua `AssignTextLibIdToImageADOs`; `btnSavePtttTemp` cho lưu mẫu khi chỉ có ảnh, truyền danh sách ảnh sang `FormPtttTemp`; `FormPtttTemp.btnSave` build `TEXT_LIB_IDS` (dùng lại `TextLibId` hoặc tạo mới qua `api/HisTextLib/Create`), thất bại thì dừng; thêm message `LuuLuocDoThatBaiKhongTheLuuMau` (vi/en/my). |
| 23/06/2026 | huyvu20 | **Fix lỗi không load được lược đồ khi chọn Mẫu PTTT**: `cboPtttTemp_EditValueChanged` đổi từ `BackendDataWorker.Get<HIS_TEXT_LIB>()` (filter null, thiếu `CAN_VIEW` → `CONTENT` null) sang gọi `api/HisTextLib/Get` với `filter.IDs` + `filter.CAN_VIEW = true` + `LIB_TYPE_ID` để backend trả về `CONTENT` (bytes ảnh). Thêm guard bỏ qua bản ghi `CONTENT` rỗng trong `SelectListImageTemp` (cả 2 nhánh nhóm/đơn) để tránh 1 bản ghi lỗi làm hỏng cả lô. |
| 02/07/2026 | dangth2 | **Việc 2891 - mục 4.1.6 (chạy thận)**: (1) **R18 - Pre-fill Máy thực hiện**: thêm helper `PrefillMachineFromServiceReq()` gợi ý `cboMachine` = `V_HIS_SERVICE_REQ.MACHINE_ID` (Máy chốt ở Chỉ định) khi mức dịch vụ `HIS_SERE_SERV_EXT.MACHINE_ID` chưa có Máy; gọi cuối `LoadSereServExt()` và `FillDataFromSereServLast()`; ĐD/BS được sửa; khi lưu vẫn ghi vào `HIS_SERE_SERV_EXT.MACHINE_ID` (logic sẵn có ở `___Process.cs`) → đồng bộ Máy 2 chiều. (2) **Delta 23169 - nút Tủ trực**: `btnTuTruc_Click` bổ sung truyền `assignPrescription.ExpMestTemplateId = serviceReq.EXP_MEST_TEMPLATE_ID` (Gói vật tư BS chốt) sang `AssignPrescriptionPK` → tự gọi `InitDataByExpMestTemplate()` fill grid kê đơn theo Gói. |

| 20/08/2026 | khainq | **Mps000324 — bổ sung trường cho mẫu Phiếu thanh quyết toán PT/TT**: thêm ADO `Mps000324GroupADO` + `Mps000324ItemADO`; PDO thêm `bedLog`/`Groups`/`Items` và constructor overload 12 tham số (ctor cũ giữ nguyên); Processor thêm `BuildDetailData()` (gom nhóm, STT toàn phiếu + STT trong nhóm, thành tiền, tổng nhóm, tổng cuối), `SetSingleKeyExtend()` (barcode, giường, số phiếu, ghi chú, PP thực tế, ekip gộp nhiều người), hiện thực `SetBarcodeKey()` (Code128) + `barCodeTag.ProcessData`; `ProcessListSereServ` đổi `FirstOrDefault` trong loop → Dictionary; caller thêm `GetLastBedLogForMps000324` + `GetExecuteRolesForMps000324`. Khối kíp mổ sinh từ **danh mục `HIS_EXECUTE_ROLE`** (dataset `EkipRoles` + key `EXECUTE_ROLE_NAME_{CODE}`), mẫu in không hardcode mã vai `_01.._08` nữa. **Thuần bổ sung — key/dataset cũ không đổi nên mẫu `Mps000324.xlsx` hiện tại chạy y nguyên.** |

## 9. Test Cases

### Mps000324 — bổ sung trường (20/08/2026)
- [ ] In Mps000324 với mẫu `Mps000324.xlsx` **hiện tại** → kết quả giống hệt trước khi sửa (nhóm theo loại DV, 4 cột Tên/SL/ĐVT/Đơn giá, khối ekip `USERNAME_EXECUTE_ROLE_xx`).
- [ ] Y lệnh không có dịch vụ con → `Groups`/`Items` rỗng, `GRAND_TOTAL_AMOUNT` = 0, không lỗi.
- [ ] Y lệnh có thuốc + vật tư + dịch vụ khác → `Items` đủ dòng, `NUM_ORDER` chạy liên tục toàn phiếu, `NUM_ORDER_IN_GROUP` reset theo nhóm.
- [ ] Dòng `IS_EXPEND = 1` → `NOTE` = "Hao Phí"; còn lại "Thu Phí".
- [ ] Dòng `PRICE = 0` → `PRICE`/`INTO_MONEY` null → ô trên mẫu để trống; nhóm toàn dòng giá 0 → `TOTAL_AMOUNT` null.
- [ ] `INTO_MONEY` = `AMOUNT × PRICE` với số lượng lẻ (0,02 × 121.800.000 = 2.436.000).
- [ ] `GRAND_TOTAL_AMOUNT` = tổng các `Groups.TOTAL_AMOUNT`.
- [ ] Một vai kíp mổ có 2 người → `USERNAMES_EXECUTE_ROLE_{CODE}` chứa cả 2 (ngăn `", "`), `USERNAME_EXECUTE_ROLE_{CODE}` cũ **không đổi hành vi**.
- [ ] `EkipRoles` liệt kê đúng các vai trong `HIS_EXECUTE_ROLE` (IS_ACTIVE=1, IS_DELETE=0), sắp theo `EXECUTE_ROLE_CODE`.
- [ ] Vai trò không có người trong kíp → vẫn có dòng trong `EkipRoles` với `USER_COUNT = 0`, `USERNAMES` rỗng.
- [ ] Đổi tên vai trong danh mục `HIS_EXECUTE_ROLE` → nhãn trên phiếu đổi theo (không phải sửa mẫu).
- [ ] Xoá cache `HIS_EXECUTE_ROLE` → `GetExecuteRolesForMps000324` gọi API và nạp lại cache, phiếu vẫn đủ vai.
- [ ] Lần điều trị chưa có giường → `BED_*_STR` rỗng, không lỗi.
- [ ] `IN_CODE` rỗng → không sinh barcode, không lỗi.

### Pre-fill Máy thực hiện + Tủ trực theo Gói (2891 - 4.1.6)
- [ ] Mở y lệnh chạy thận có Máy ở Chỉ định, chưa từng lưu Máy mức dịch vụ → `cboMachine` + `txtMachineCode` tự điền Máy của y lệnh.
- [ ] Đã lưu Máy mức dịch vụ (khác Máy Chỉ định) → giữ nguyên Máy đã lưu, KHÔNG ghi đè.
- [ ] Y lệnh không có Máy ở Chỉ định → `cboMachine` trống như cũ.
- [ ] Sửa lại Máy thực hiện rồi Lưu → ghi vào `HIS_SERE_SERV_EXT.MACHINE_ID`.
- [ ] Bấm "Tủ trực", y lệnh có `EXP_MEST_TEMPLATE_ID` → AssignPrescriptionPK tự fill thuốc + vật tư theo Gói.
- [ ] Bấm "Tủ trực", y lệnh KHÔNG có Gói vật tư → mở form kê đơn tủ trực trống như cũ (không lỗi).

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
