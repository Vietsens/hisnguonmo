---
description: Review cấu trúc folder plugin theo folder_structure.md — vai trò folder, partial class, naming, files gốc
argument-hint: <plugin folder path>
---

# Review Folder Structure

Review plugin: $ARGUMENTS

## 1. Cấu Trúc Cơ Bản
- Có folder {Name}/ chứa Interface + Factory + Behavior?
- Processor.cs có [ExtensionOf] tại root?
- Có Properties/AssemblyInfo.cs với [assembly: Plugin]?
- Có Resources/ với Lang.*.resx + Message.Lang.*.resx + ResourceMessage.cs?

## 2. Phân Chia Vai Trò
- Business logic trong Behavior/Worker — KHÔNG trong Form/UC?
- ADO classes trong ADO/ folder — KHÔNG trộn với logic?
- Config classes trong Config/ folder?
- Validation trong Validate/ folder (nếu > 3 rules)?
- Print logic trong Print/ folder (nếu có in)?

## 3. Form/UC Placement
- Form chính trong {Name}/ folder cùng Behavior?
- Form phụ có subfolder riêng?
- Dialog forms trong MessageBoxForm/ hoặc feature folder?

## 4. Partial Classes (nếu form > 500 dòng)
- Form dùng double underscore `__`? (frm{Name}__Load.cs, __Save.cs, __Process.cs)
- UC dùng triple underscore `___`? (UC{Name}___Load.cs, ___Process.cs)
- Mỗi partial file <= 500 dòng?
- Tách đúng vai trò? (Load, Init, Process, Save, Edit, Check, Print, Dispose)

## 5. Files Gốc (Root Level)
- {Name}Processor.cs — entry point?
- HisRequestUriStore.cs — API endpoints (nếu có API)?
- KeyboardWorker.cs — phím tắt (nếu UC)?
- GlobalStore.cs — module cache (nếu cần share state)?
- Enum{Name}.cs — constants (nếu có magic values)?
- Delegate.cs — delegate riêng (nếu có event mới)?

## 6. Resources
- Lang.vi.resx + Lang.en.resx tách riêng?
- Message.Lang.vi.resx + Message.Lang.en.resx tách riêng?
- ResourceLanguageManager.cs + ResourceMessage.cs có đủ?
- Images trong Resources/ hoặc Image/ — KHÔNG tại root?

## 7. Anti-Patterns
- Business logic trong Form/UC? → chuyển sang Behavior/Worker
- ADO chứa logic? → tách logic ra, ADO chỉ properties
- File > 2000 dòng? → tách partial hoặc Worker
- Folder rỗng? → xóa
- Image tại root? → chuyển vào Resources/ hoặc Image/
- Hardcode string? → chuyển vào Resources, Enum, Constants
- Magic numbers? → IMSys.DbConfig hoặc Enum riêng

## Output
[CRITICAL] Thiếu Behavior folder, logic trong Form — fix
[HIGH] Thiếu AssemblyInfo [Plugin], thiếu Resources — fix
[MEDIUM] Thiếu partial class (form > 500 dòng) — fix
[LOW] Naming không chuẩn, thiếu Enum file — fix
