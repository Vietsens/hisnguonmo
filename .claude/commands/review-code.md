---
description: Review code theo coding_rules — naming, Processor/Factory/Behavior, API calls, exception, logging, constants
argument-hint: <file hoặc folder path>
---

# Review Code — Coding Rules

Review: $ARGUMENTS

## 1. Naming Convention
- Class PascalCase? Method PascalCase? Variable camelCase?
- Control prefix đúng? (btn, txt, cbo, grd, chk, dte, spn, lyt, pnl)
- Interface có prefix I?
- Namespace đúng format CompanyName.Technology.Feature?

## 2. Architecture Processor/Factory/Behavior
- Processor có [ExtensionOf] attribute?
- Processor.Run() parse args đúng pattern (for loop + is type check)?
- Factory có catch NullReferenceException riêng + LogUtil.TraceData?
- Behavior kế thừa BusinessBase?
- Business logic trong Behavior — KHÔNG trong Form?

## 3. API Calls
- Dùng BackendAdapter(param).Post/Get/GetRO?
- URI từ HisRequestUriStore — KHÔNG hardcode?
- Filter có set ORDER_DIRECTION, ORDER_FIELD?
- Sau API: WaitingManager.Hide → MessageManager.Show → SessionManager.ProcessTokenLost?

## 4. Exception Handling
- Mỗi method có try-catch?
- Error level cho Processor/Factory/Save, Warn cho UI events?
- KHÔNG có catch rỗng?
- Debug trace dùng LogUtil.TraceData + LogUtil.GetMemberName?

## 5. DateTime
- Kiểu long yyyyMMddHHmmss?
- Dùng Inventec.Common.DateTime.Convert.*?

## 6. Constants — KHÔNG HARDCODE SỐ
- Có số trực tiếp (== 1, == 2, == 3) trong code?
  → Tìm IMSys.DbConfig.[Schema].[Table].enum tương ứng
  → VD: `== 2` phải là `== IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST`
- Nếu không có trong IMSys.DbConfig → có tạo Enum riêng có XML comment?
- Enum riêng có:
  - XML comment cho class (mô tả mapping bảng/cột)?
  - XML comment cho mỗi value (mô tả rõ nghĩa)?
  - Gán giá trị tường minh (= 0, = 1, = 2)?
  - File riêng Enum{Name}.cs?
- Dùng GlobalVariables.ActionAdd/ActionEdit cho CRUD?

## 7. Config
- HisConfigs.Get cho toàn viện, ConfigApplicationWorker.Get cho per-user?
- Config keys là constants — KHÔNG hardcode string?

## 8. Delegate
- Parse delegate từ args bằng `is` type check — KHÔNG ép kiểu trực tiếp?
- Null check trước khi invoke: `if (delegate != null) delegate(data)`?
- Delegate trong try-catch?

## Output
[CRITICAL] Hardcode số (== 1, == 2), catch rỗng, logic trong Form — file:line — fix
[HIGH] Thiếu IMSys.DbConfig, sai log level, thiếu WaitingManager.Hide — file:line — fix
[MEDIUM] Thiếu Enum XML comment, thiếu TraceData — file:line — fix
[LOW] Naming không chuẩn, thiếu config constant — file:line — fix
