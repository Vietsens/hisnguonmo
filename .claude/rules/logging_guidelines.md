# Logging Guidelines - WinForms C# (Inventec HIS)

## 1. Mục tiêu

Logging nhằm:

- Theo dõi luồng xử lý
- Phát hiện lỗi nhanh
- Debug dễ dàng
- Audit nghiệp vụ.

Logging phải:

- Rõ ràng
- Có cấu trúc
- Không dư thừa
- Không thiếu thông tin quan trọng
- Log không nằm trong vòng lặp
- Bắt buộc sử dụng log trong Inventec.Common.Logging

## 2. Nguyên tắc chung

- Không dùng Console.WriteLine
- Không bỏ trống catch
- Luôn log khi có exception
- Không log dữ liệu nhạy cảm (password, token)

- Log phải có context:
    + User
    + Action
    + Data chính

---

---
description: Quy tắc logging — Inventec.Common.Logging (log4net). LogSystem, LogAction, LogUtil. Áp dụng khi viết try-catch, debug, trace data, audit action
paths:
  - "HIS/Plugins/**"
  - "UC/**"
  - "MPS/**"
  - "Common/**"
---

# Logging Rules — Inventec.Common.Logging

Framework: **log4net 1.2.10**, lazy init từ App.config.
Source: `Common/Inventec.Common/Inventec.Common.Logging/`

---

## 5 Logger Classes

| Class | File output | Mục đích |
|-------|-------------|----------|
| `LogSystem` | `Logs/LogSystem.txt` | Log kỹ thuật: exception, debug, trace |
| `LogAction` | `Logs/LogAction.txt` | Log hành động user: mở module, save, thời gian xử lý |
| `LogSession` | `Logs/LogSession.txt` | Log phiên làm việc |
| `LogFilter` | `Logs/LogFilter.txt` | Log filter/query |
| `LogTime` | `Logs/LogTime.txt` | Log performance/timing |

Mỗi class có **cùng method signatures**, chỉ khác logger instance và output file.

---

## Methods

```csharp
// 5 levels — mỗi level có 3 overloads
LogSystem.Debug(string message)
LogSystem.Debug(Exception ex)
LogSystem.Debug(string message, Exception ex)

LogSystem.Info(string message)

LogSystem.Warn(string message)
LogSystem.Warn(Exception ex)
LogSystem.Warn(string message, Exception ex)

LogSystem.Error(string message)
LogSystem.Error(Exception ex)
LogSystem.Error(string message, Exception ex)

LogSystem.Fatal(string message)
LogSystem.Fatal(Exception ex)
LogSystem.Fatal(string message, Exception ex)

// Check level enabled
LogSystem.IsDebugEnabled()
LogSystem.IsInfoEnabled()
```

---

## Log Format (output)

```
%level %date [%thread] - %message
%exception
```

Ví dụ thực tế:
```
ERROR 2025-01-18 10:23:45,123 [3] - SaveProcess failed___dto:{"ID":123,"NAME":"Test"}___
System.NullReferenceException: Object reference not set...
   at HIS.Desktop.Plugins.HisMachine.SaveProcess()
```

Config: rolling file, max 5MB/file, giữ 30 backups, UTF-8.

---

## LogUtil — Utilities

### TraceData: Serialize object sang JSON

```csharp
LogUtil.TraceData(string name, object data)
// Output: ___name:{"prop1":"val1","prop2":123}___
```

### GetMemberName: Lấy tên biến từ expression

```csharp
LogUtil.GetMemberName(() => myVariable)
// Output: "myVariable"
```

### Kết hợp (pattern chuẩn):

```csharp
LogSystem.Debug(
    LogUtil.TraceData(LogUtil.GetMemberName(() => dto), dto)
);
// Output: ___dto:{"ID":1,"CODE":"M001","NAME":"Máy XN"}___
```

### TraceDbException: Format lỗi Entity Framework

```csharp
LogUtil.TraceDbException(dbEntityValidationException)
// Output: ___Lỗi tương tác CSDL (DbEntityValidationException){MACHINE_CODE:Required; MACHINE_NAME:Max length 200}___
```

### LogActionSuccess / LogActionFail

```csharp
LogUtil.LogActionSuccess("HisMachineBehavior", "Create", "admin")
// -> LogSystem.Info("HisMachineBehavior.Create.Username=admin.Xử lý thành công.")

LogUtil.LogActionFail("HisMachineBehavior", "Create", "admin")
// -> LogSystem.Info("HisMachineBehavior.Create.Username=admin.Xử lý thất bại.")
```

---

## Quy Tắc Sử Dụng — Theo Level

### ERROR — Lỗi cần xử lý, ảnh hưởng chức năng

```csharp
// API call thất bại
catch (Exception ex)
{
    WaitingManager.Hide();
    Inventec.Common.Logging.LogSystem.Error(ex);
}

// Error với context data
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Error(
        "SaveProcess thất bại."
        + Inventec.Common.Logging.LogUtil.TraceData(
            Inventec.Common.Logging.LogUtil.GetMemberName(() => dto), dto),
        ex);
}
```

**Dùng khi:** Save/Create/Update/Delete fail, API response null, logic nghiệp vụ thất bại.

### WARN — Lỗi nhẹ, app vẫn chạy được

```csharp
// Init combo thất bại — form vẫn hoạt động
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Warn(ex);
}

// UI event lỗi — không ảnh hưởng chức năng chính
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Warn(ex);
}
```

**Dùng khi:** Init combo fail, UI event error, validation logic error, load config fail nhưng có default.

### DEBUG — Trace data để debug

```csharp
// Trace input trước khi gọi API
Inventec.Common.Logging.LogSystem.Debug(
    "INPUT____"
    + Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter)
);

// Trace nhiều biến cùng lúc
Inventec.Common.Logging.LogSystem.Debug(
    Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId)
    + Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqIds), serviceReqIds)
);
```

**Dùng khi:** Trace input/output API, kiểm tra giá trị biến tại thời điểm lỗi, debug logic phức tạp.

### INFO — Thông tin quan trọng (ít dùng trong plugin)

```csharp
// Thông báo action thành công/thất bại
Inventec.Common.Logging.LogUtil.LogActionSuccess("HisMachineBehavior", "Create", loginName);
```

### FATAL — Lỗi nghiêm trọng, app không thể tiếp tục

Rất ít dùng. Chỉ cho lỗi startup, mất kết nối DB hoàn toàn.

---

## LogAction — Audit Trail

Dùng `LogAction` (KHÔNG phải LogSystem) để ghi nhận hành động user:

```csharp
// Mở module
Inventec.Common.Logging.LogAction.Info(
    String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}",
        APPLICATION_CODE,    // Mã ứng dụng
        VersionApp,          // Phiên bản
        elapsedSeconds,      // Thời gian xử lý (giây)
        ModuleLink,          // Module ID
        "OpenModule",        // Hành động
        LoginName,           // Tài khoản
        IpLocal,             // IP máy
        CustomerCode         // Mã khách hàng/bệnh viện
    )
);
```

**Format LogAction:** `AppCode____Version____Seconds____Module____Action____User____IP____Customer`

Dùng khi: Mở module, save thành công, in, ký, xuất báo cáo — bất kỳ hành động cần audit.

---

## BẮT BUỘC

| Quy tắc | Chi tiết |
|---------|----------|
| Mỗi method PHẢI có try-catch | `catch (Exception ex) { LogSystem.Error(ex); }` hoặc `Warn(ex)` |
| KHÔNG dùng `Console.Write` | Dùng `LogSystem` thay thế |
| KHÔNG dùng `Debug.Write` | Dùng `LogSystem.Debug()` thay thế |
| KHÔNG dùng `MessageBox` để log | MessageBox chỉ cho user, log dùng LogSystem |
| KHÔNG log thông tin nhạy cảm | PIN, password, private key, CMND, số điện thoại bệnh nhân |
| KHÔNG log quá nhiều trong vòng lặp | Log trước/sau vòng lặp, KHÔNG log mỗi iteration |
| Error level cho API fail | `LogSystem.Error(ex)` khi BackendAdapter trả null/exception |
| Warn level cho UI event | `LogSystem.Warn(ex)` trong event handlers (CheckedChanged, Click, KeyDown) |
| Debug với TraceData | `LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => var), var))` |
| LogAction cho audit | Hành động user (mở module, save, print) dùng `LogAction.Info()` |
| WaitingManager.Hide trong catch | Nếu đã Show thì PHẢI Hide trước khi log |

---

## Anti-Patterns (KHÔNG LÀM)

```csharp
// SAI: Catch rỗng — nuốt exception
catch (Exception ex) { }

// SAI: Catch generic rồi không log
catch { return null; }

// SAI: Console.Write
catch (Exception ex) { Console.WriteLine(ex.Message); }

// SAI: Log trong vòng lặp lớn
foreach (var item in list10000Items)
{
    LogSystem.Debug(LogUtil.TraceData("item", item)); // QUÁ NHIỀU LOG
}

// SAI: Log thông tin nhạy cảm
LogSystem.Debug("PIN=" + pinCode);
LogSystem.Debug("Patient CMND=" + patient.IDENTIFICATION_NUMBER);

// SAI: Dùng LogSystem cho audit action
LogSystem.Info("User admin saved record");  // Nên dùng LogAction
```

---

## Patterns Chuẩn (COPY-PASTE)

### Try-catch cơ bản (mỗi method)
```csharp
try
{
    // logic
}
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Warn(ex);
}
```

### Try-catch API call
```csharp
CommonParam param = new CommonParam();
try
{
    WaitingManager.Show();
    var result = new BackendAdapter(param).Post<HIS_ENTITY>(uri, consumer, dto, param);
    WaitingManager.Hide();
    MessageManager.Show(this, param, result != null);
    SessionManager.ProcessTokenLost(param);
}
catch (Exception ex)
{
    WaitingManager.Hide();
    Inventec.Common.Logging.LogSystem.Error(ex);
}
```

### Debug trace input trước API
```csharp
Inventec.Common.Logging.LogSystem.Debug(
    Inventec.Common.Logging.LogUtil.TraceData(
        Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
var result = new BackendAdapter(param).Post<bool>(uri, consumer, filter, param);
```

### Error với context data
```csharp
catch (Exception ex)
{
    Inventec.Common.Logging.LogSystem.Error(
        "Factory không khởi tạo được đối tượng. Type="
        + data.GetType().ToString()
        + Inventec.Common.Logging.LogUtil.TraceData(
            Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
        ex);
}
```

### Audit action (LogAction)
```csharp
Inventec.Common.Logging.LogUtil.LogActionSuccess(
    "HisMachineBehavior", "Create", loginName);
```
