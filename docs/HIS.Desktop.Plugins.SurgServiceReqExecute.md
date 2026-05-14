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

Quy tắc gốc: `PT.INTRUCTION < VTYT.INTRUCTION < PT.START < VTYT.START < VTYT.FINISH < PT.FINISH`.

Quy tắc nới lỏng (cập nhật 2026-05-12):
- Nếu PT (parent) **không có START_TIME** → BỎ QUA tất cả check liên quan đến thời gian bắt đầu của VTYT con (không bắt VTYT phải có START_TIME, không check R3/R4).
- Nếu PT **không có FINISH_TIME** → BỎ QUA check thời gian kết thúc VTYT (R6) và không bắt VTYT phải có FINISH_TIME.
- Nếu PT thiếu cả hai → KHÔNG validate.
- R2 (PT.INTRUCTION < VTYT.INTRUCTION) vẫn áp dụng độc lập.
- R5 (VTYT.START < VTYT.FINISH) chỉ check khi VTYT có cả hai giá trị.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Yêu cầu dịch vụ PT và VTYT con |
| HIS_SERVICE_REQ_MATY | Table | VTYT tiêu hao khai báo theo service request |
| HIS_EXP_MEST / HIS_EXP_MEST_MATERIAL | Table | VTYT thực xuất kho (đơn trong kho) |
| HIS_MATERIAL_TYPE | Table | Loại vật tư — cờ IS_REQUIRE_TIME_VALIDATE |
| V_HIS_SERE_SERV_5 | View | Dịch vụ đã thực hiện kèm giá |
| HIS_SERE_SERV_PTTT | Table | Thông tin PTTT (stent, phương pháp) |

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
| 2026-05-12 | sinhnt | Nới điều kiện validate `ValidateVtytTimeWithParentPT`: khi PT cha không có START_TIME → bỏ qua các check liên quan đến thời gian bắt đầu của VTYT con (DONDT/DONTT); khi PT không có FINISH_TIME → bỏ qua check thời gian kết thúc. Vẫn giữ R2 (instruction) và R5 khi VTYT có đủ hai mốc. |

## 9. Test Cases

### Validate thời gian PT ↔ VTYT (sau cập nhật)
- [ ] PT có START + FINISH, VTYT trong khoảng → PASS.
- [ ] PT có START + FINISH, VTYT.START < PT.START → FAIL (R4).
- [ ] PT **không có START_TIME**, có FINISH_TIME, VTYT thiếu START → PASS (bỏ R1 start, R3, R4).
- [ ] PT **không có START_TIME**, VTYT.FINISH > PT.FINISH → FAIL (R6 vẫn check).
- [ ] PT **không có FINISH_TIME**, có START_TIME, VTYT thiếu FINISH → PASS (bỏ R1 finish, R6).
- [ ] PT **không có FINISH_TIME**, VTYT.START < PT.START → FAIL (R4 vẫn check).
- [ ] PT thiếu cả START và FINISH → không validate, PASS.
- [ ] VTYT thiếu INTRUCTION_TIME → FAIL (R1 instruction luôn bắt buộc).
- [ ] PT.INTRUCTION > VTYT.INTRUCTION → FAIL (R2 độc lập PT.START/PT.FINISH).
- [ ] VTYT có cả START và FINISH nhưng FINISH < START → FAIL (R5).
