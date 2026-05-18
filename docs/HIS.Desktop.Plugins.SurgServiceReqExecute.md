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
| 2026-05-12 | sinhnt | Nới điều kiện validate `ValidateVtytTimeWithParentPT`: chỉ INTRUCTION_TIME của VTYT và PT là bắt buộc; START_TIME / FINISH_TIME của cả VTYT và PT chuyển sang conditional — mỗi cross-check (R2–R6) chỉ chạy khi cả hai trường input của rule đều > 0. |

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
