---
description: Kiểm tra inter-plugin communication — PluginInstance, ModuleLink, args đầu vào, null check, delegate callback
argument-hint: <file hoặc folder path>
---

# Check Inter-Plugin Communication

Kiểm tra: $ARGUMENTS

## 1. ModuleLink
- ModuleLink là constant trong ModuleLinkString.cs? KHÔNG hardcode string?
- ModuleLinkString.cs có tồn tại trong plugin?

## 2. Xác Định Đầu Vào Plugin Đích (QUAN TRỌNG NHẤT)
- Đã đọc Behavior.Run() của plugin ĐÍCH để biết args cần truyền?
- Args truyền ĐỦ các tham số BẮT BUỘC?
- Kiểu dữ liệu KHỚP? (long KHÔNG phải int, DelegateSelectData KHÔNG phải Action)
- List<object> KHÔNG phải object[]?

## 3. Null Check
- moduleData != null && IsPlugin trước khi gọi?
- instance != null trước khi ShowDialog/Add?
- delegate != null trước khi invoke callback?

## 4. Args Parse (Plugin con)
- Parse args bằng `is` type check? KHÔNG ép kiểu trực tiếp `(long)args[0]`?
- for loop duyệt tất cả args?
- Có xử lý khi args null/rỗng?

## 5. Room Context
- Dùng GetModuleWithWorkingRoom() khi plugin con cần biết phòng?
- Truyền currentModule.RoomId và RoomTypeId?

## 6. Delegate Callback
- Plugin con lưu delegate field?
- Gọi delegate trong try-catch?
- Null check trước invoke?

## 7. Anti-Patterns
- Hardcode ModuleLink string trực tiếp?
- Ép kiểu args[0] không check `is`?
- Truyền int thay vì long?
- Không null check module/instance?
- Đoán tham số — không đọc Behavior.Run() của plugin đích?
- Try-catch KHÔNG bao quanh toàn bộ quá trình mở plugin?

## Output
[CRITICAL] Hardcode ModuleLink, không null check — file:line — fix
[HIGH] Sai kiểu args (int vs long), thiếu đầu vào BẮT BUỘC — file:line — fix
[MEDIUM] Thiếu room context, thiếu try-catch — file:line — fix
