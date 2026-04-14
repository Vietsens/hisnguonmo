---
description: Kiểm tra thread safety — InvokeRequired, lock, WaitingManager, async/UI thread issues
argument-hint: <file hoặc folder path>
---

# Check Thread Safety

Kiểm tra: $ARGUMENTS

## 1. UI Thread Safety
- Có update UI control từ background thread không?
- Nếu có: đã check InvokeRequired + this.Invoke()?
- Timer callback (System.Threading.Timer) có InvokeRequired?

## 2. WaitingManager
- Show/Hide đi cặp?
- Hide trong catch/finally?
- KHÔNG dùng async/await cho API calls bình thường?

## 3. Lock Pattern
- Shared mutable data có lock?
- Lock object là private readonly object?
- Lock CẢ read và write?
- Copy data trong read path (.ToList())?
- KHÔNG lock trên this, typeof(T), string?

## 4. Timer
- System.Windows.Forms.Timer cho UI updates (Tick trên UI thread)?
- System.Threading.Timer cho background (cần InvokeRequired)?
- Timer có Dispose khi form close?

## 5. CancellationToken
- Long-running operation có hỗ trợ cancel?
- User thay đổi selection → cancel operation cũ?

## 6. Patterns sai thường gặp
- Thread.Sleep() trên UI thread?
- Task.Run/async await cho API bình thường?
- BackgroundWorker (legacy, không dùng)?
- Trực tiếp set .Text/.DataSource từ background?

## Output
[CRITICAL/HIGH/MEDIUM] file:line — mô tả — fix
