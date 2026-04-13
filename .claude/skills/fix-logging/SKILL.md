---
name: fix-logging
description: Tìm và fix logging issues — catch rỗng, sai level, thiếu TraceData, thiếu audit LogAction
user-invocable: true
argument-hint: <file hoặc folder path>
---

# Fix Logging — Tự Động Sửa Logging Issues

Target: $ARGUMENTS

## Bước 1: Scan tìm issues

### L1. Catch rỗng [CRITICAL]
```
Pattern: catch (Exception ex) { }
Pattern: catch { return null; }
Fix: Thêm LogSystem.Warn(ex) hoặc LogSystem.Error(ex)
```

### L2. Console/Debug.Write [HIGH]
```
Pattern: Console.WriteLine(...)
Pattern: Debug.Write(...)
Fix: Đổi sang LogSystem.Debug(...)
```

### L3. Sai log level [HIGH]
```
Pattern: LogSystem.Error(ex) trong UI event handler (Click, CheckedChanged)
Fix: Đổi sang LogSystem.Warn(ex) — UI events là lỗi nhẹ

Pattern: LogSystem.Warn(ex) trong Processor/Factory/Save
Fix: Đổi sang LogSystem.Error(ex) — business logic là lỗi nghiêm trọng
```

### L4. Thiếu WaitingManager.Hide trong catch [HIGH]
```
Pattern: WaitingManager.Show() ... catch { LogSystem.Error(ex); }
         (không có WaitingManager.Hide() trong catch)
Fix: Thêm WaitingManager.Hide() TRƯỚC LogSystem trong catch
```

### L5. Thiếu TraceData trong Error [MEDIUM]
```
Pattern: LogSystem.Error(ex) — không có context data
Fix: Thêm LogUtil.TraceData cho biến quan trọng
LogSystem.Error("SaveProcess fail." + LogUtil.TraceData(GetMemberName(() => dto), dto), ex);
```

### L6. Log trong vòng lặp [MEDIUM]
```
Pattern: foreach { LogSystem.Debug(LogUtil.TraceData("item", item)); }
Fix: Log trước/sau loop, hoặc log count/summary
```

### L7. Log thông tin nhạy cảm [CRITICAL]
```
Pattern: LogSystem.Debug("PIN=" + pin)
Pattern: LogSystem.Debug(patient.IDENTIFICATION_NUMBER)
Fix: Xóa hoặc mask data nhạy cảm
```

### L8. LogSystem.Info cho audit [MEDIUM]
```
Pattern: LogSystem.Info("User saved record")
Fix: Đổi sang LogAction.Info(...) hoặc LogUtil.LogActionSuccess(...)
```

## Bước 2: Tự động fix

Với mỗi issue:
1. Xác định context (Processor? Form event? Save method?)
2. Chọn level phù hợp (Error/Warn/Debug)
3. Sinh code fix

### Fix catch rỗng:
```csharp
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Warn(ex);
}
```

### Fix thiếu WaitingManager.Hide:
```csharp
catch (Exception ex)
{
    WaitingManager.Hide();
    Inventec.Common.Logging.LogSystem.Error(ex);
}
```

### Fix thiếu TraceData:
```csharp
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Error(
        Inventec.Common.Logging.LogUtil.TraceData(
            Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto),
        ex);
}
```

## Bước 3: Output

```
FILE: {path}
FIXES: {số lượng}
  [CRITICAL] Line {n}: Catch rỗng → thêm LogSystem.Warn — FIXED
  [CRITICAL] Line {n}: Log PIN/CMND → xóa — FIXED
  [HIGH] Line {n}: Console.Write → LogSystem.Debug — FIXED
  [HIGH] Line {n}: Thiếu WaitingManager.Hide → thêm — FIXED
  [MEDIUM] Line {n}: Thiếu TraceData → thêm context — FIXED
```
