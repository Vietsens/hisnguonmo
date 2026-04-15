---
description: Review logging theo logging_guidelines.md — log level, try-catch, TraceData, audit, anti-patterns
argument-hint: <file hoặc folder path>
---

# Review Logging

Review: $ARGUMENTS

## 1. Try-Catch Coverage
- MỌI method có try-catch?
- Có catch rỗng `catch { }` hoặc `catch (Exception ex) { }`?
- Có catch không log `catch { return null; }`?

## 2. Log Level Đúng
- Processor/Factory/Behavior/Save fail → `LogSystem.Error(ex)`?
- UI event handlers (Click, CheckedChanged, KeyDown) → `LogSystem.Warn(ex)`?
- Trace data trước/sau API → `LogSystem.Debug(LogUtil.TraceData(...))`?
- Audit hành động user → `LogAction.Info(...)` (KHÔNG LogSystem.Info)?

## 3. LogUtil.TraceData
- Có dùng `LogUtil.TraceData(LogUtil.GetMemberName(() => var), var)` khi debug?
- Error có kèm context data? VD:
  ```
  LogSystem.Error("SaveProcess fail." + LogUtil.TraceData(GetMemberName(() => dto), dto), ex);
  ```
- Có TraceDbException cho Entity Framework errors?

## 4. WaitingManager + Log
- WaitingManager.Hide() TRƯỚC LogSystem trong catch?
  ```
  catch (Exception ex)
  {
      WaitingManager.Hide();              // 1. Hide trước
      LogSystem.Error(ex);                // 2. Log sau
  }
  ```

## 5. LogAction — Audit Trail
- Hành động quan trọng (save, delete, print, sign) có LogAction.Info?
- Format đúng: AppCode____Version____Seconds____Module____Action____User____IP____Customer?
- Dùng LogUtil.LogActionSuccess/LogActionFail cho CRUD?

## 6. Anti-Patterns
- `Console.WriteLine`? → đổi sang LogSystem
- `Debug.Write`? → đổi sang LogSystem.Debug
- `MessageBox.Show` để debug? → đổi sang LogSystem.Debug
- Log trong vòng lặp lớn (foreach 1000+ items)? → log trước/sau loop
- Log thông tin nhạy cảm (PIN, CMND, password, token, số điện thoại)?
- Dùng `LogSystem.Info` cho audit? → đổi sang `LogAction.Info`

## 7. Pattern Chuẩn
- Try-catch cơ bản: `catch (ex) { LogSystem.Warn(ex); }`
- API call: WaitingManager.Show → try → API → Hide → catch { Hide; Error(ex); }
- Factory: catch NullReferenceException riêng + LogUtil.TraceData
- Debug: LogSystem.Debug(LogUtil.TraceData(GetMemberName(() => filter), filter))

## Output
[CRITICAL] Catch rỗng, thiếu try-catch — file:line — fix
[HIGH] Sai log level (Error cho UI event), thiếu WaitingManager.Hide — file:line — fix
[MEDIUM] Thiếu TraceData, thiếu LogAction audit — file:line — fix
[LOW] Console.Write, log trong loop — file:line — fix
