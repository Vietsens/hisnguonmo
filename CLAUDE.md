# HIS Desktop — Hospital Information System

## 1. Project

**HIS Desktop** là ứng dụng WinForms quản lý bệnh viện, phục vụ bác sĩ, điều dưỡng, dược sĩ, thu ngân, và nhân viên hành chính.

| Thông tin | Giá trị |
|-----------|---------|
| Platform | Windows Desktop (WinForms) |
| Framework | .NET Framework 4.5 |
| UI Library | DevExpress 15.2.9 |
| Plugin System | MEF (Managed Extensibility Framework) |
| Language | C# |
| IDE | Visual Studio 2019+ |
| VCS | Git (GitLab) |
| Quy mô | 1001 plugins, 802 MPS processors, 134 UCs, 41 Library plugins |

**3 repos** (PHẢI nằm cùng cấp):
- `hisnguonmo/` — HIS Desktop chính + Plugins + MPS + UC
- `lib/` — Backend models (EFMODEL), DevExpress DLLs, Inventec.* pre-built
- `common/` — Thư viện dùng chung (BHYT, Ký số, WCF, Inventec.Common)

---

## 2. Architecture

### Plugin Architecture
```
User click menu
  → Processor.Run(args)                    [ExtensionOf — MEF registration]
    → Factory.MakeIControl(param, args)    [Tạo behavior instance]
      → Behavior.Run()                     [Parse args, tạo Form/UC]
        → Form (FormBase) / UC (UserControlBase)
```

### Folder Structure (mỗi plugin)
```
HIS.Desktop.Plugins.{Name}/
├── {Name}Processor.cs              ← MEF entry point
├── {Name}/
│   ├── I{Name}.cs                  ← Interface
│   ├── {Name}Factory.cs            ← Factory
│   └── {Name}Behavior.cs           ← Business logic (kế thừa BusinessBase)
├── frm{Name}.cs                    ← Form (kế thừa FormBase)
├── Resources/                      ← Đa ngôn ngữ (Lang.vi/en, Message.vi/en)
├── ADO/                            ← Data transfer objects
├── Config/                         ← Cấu hình từ backend
└── Properties/AssemblyInfo.cs      ← [assembly: Plugin] BẮT BUỘC

Tài liệu module (folder CHUNG):
hisnguonmo/docs/{ModuleLink}.md     ← VD: docs/HIS.Desktop.Plugins.HisMachine.md
```

### Data Flow
```
UI Control → ADO/EFMODEL → BackendAdapter → HTTP API → Backend (MOS/ACS/SDA/EMR/LIS)
                         ↕
              BackendDataWorker (RAM cache → SQLite/Redis → API)
```

### Print Flow
```
Plugin button → Print Library (12 libs) / RichEditorStore
  → MPS.MpsPrinter.Run(PrintData)
    → MPS Processor (AbstractProcessor → ProcessData)
      → Template (Excel FlexCel / Word RichEditor)
        → Preview / Print / Export / EMR Sign
```

---

## 3. Coding Standards

### BẮT BUỘC

| Quy tắc | Chi tiết |
|---------|----------|
| Architecture | Processor → Factory → Behavior → Form/UC. KHÔNG logic trong Form |
| Base class | Form: `FormBase`, UC: `UserControlBase`. BẮT BUỘC `SetIcon()` |
| Constants | `IMSys.DbConfig.[Schema].[Table].enum` — TUYỆT ĐỐI KHÔNG hardcode số |
| Enum | Nếu không có IMSys.DbConfig → tạo Enum riêng có XML comment + gán giá trị tường minh |
| Đa ngôn ngữ | Resources/Lang.vi+en.resx BẮT BUỘC. SetCaptionByLanguageKey() trong Load |
| ControlState | Checkbox nhớ trạng thái → ControlStateWorker + flag |
| Grid audit | Grid 1 bảng/view → 4 cột cuối: Thời gian tạo, Người tạo, Thời gian sửa, Người sửa |
| API | BackendAdapter + URI Store. Sau API: WaitingManager.Hide → MessageManager → SessionManager |
| Performance | KHÔNG O(n²), KHÔNG Get trong loop, Any() thay Count()>0, BeginUpdate/EndUpdate |
| Logging | MỌI method try-catch. Error critical, Warn UI. KHÔNG catch rỗng, KHÔNG Console.Write |
| Naming | PascalCase class/method, camelCase var, prefix control (btn, txt, cbo, grd) |
| Partial class | > 500 dòng → tách. Form `__` (double), UC `___` (triple) |
| AssemblyInfo | `[assembly: Inventec.Desktop.Core.Plugin]` BẮT BUỘC |
| Tài liệu hóa | Khi tạo/sửa plugin → cập nhật `hisnguonmo/docs/{ModuleLink}.md` |

### TRƯỚC KHI CODE — Kiểm tra thư viện có sẵn

| Loại | Số lượng | Rule | KHÔNG tự tạo |
|------|---------|------|-------------|
| User Controls | 134 | uc_guide.md | ComboBox, DateEdit, Grid bệnh nhân, Địa chỉ... |
| Library Plugins | 41 | library_plugins_guide.md | CheckIcd, CheckHeinGOV, EmrGenerate... |
| Print Libraries | 12 | print_integration.md | PrintPrescription, PrintBordereau... |

---

## 4. Domain Knowledge — Hệ Thống Bệnh Viện

### Nghiệp vụ chính

| Module | Plugin prefix | Mô tả |
|--------|--------------|-------|
| Khám bệnh | Treatment*, Exam*, Execute* | Tiếp nhận, khám, chỉ định, kết luận |
| Dược | Medicine*, Prescription*, Dispense* | Kê đơn, duyệt đơn, cấp phát thuốc |
| Dịch vụ | Service*, Assign*, Request* | Chỉ định CLS, CĐHA, TDCN, PTTT |
| Kho | ExpMest*, ImpMest*, MediStock* | Xuất, nhập, chuyển kho, tồn kho |
| Viện phí | Cashier*, Transaction*, Bill* | Tạm ứng, thanh toán, hoàn tiền, BHYT |
| Giường | Bed*, Room*, Department* | Phân giường, chuyển giường, chuyển khoa |
| Bệnh án | EMR.Desktop.Plugins.* | Bệnh án điện tử, ký số |
| Xét nghiệm | LIS.Desktop.Plugins.* | Chỉ định XN, kết quả, máy XN |
| Phân quyền | ACS.Desktop.Plugins.* | User, Role, Module, Control |
| In ấn | MPS.Processor.Mps* (802) | 802 mẫu in báo cáo, phiếu, biểu mẫu |

### Khái niệm y tế quan trọng

| Khái niệm | EFMODEL | Mô tả |
|-----------|---------|-------|
| Điều trị | HIS_TREATMENT / V_HIS_TREATMENT | 1 lần điều trị của bệnh nhân (từ tiếp nhận → ra viện) |
| Yêu cầu DV | HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | 1 yêu cầu chỉ định dịch vụ (khám, XN, CĐHA...) |
| DV thực hiện | HIS_SERE_SERV / V_HIS_SERE_SERV | 1 dịch vụ đã thực hiện (có giá, có BHYT) |
| Đơn thuốc | HIS_EXP_MEST | Phiếu xuất thuốc (kê đơn) |
| Nhập kho | HIS_IMP_MEST | Phiếu nhập kho |
| Bệnh nhân | HIS_PATIENT / V_HIS_PATIENT | Thông tin bệnh nhân |
| BHYT | V_HIS_PATIENT_TYPE_ALTER | Thông tin thẻ bảo hiểm y tế |
| ICD | HIS_ICD | Mã bệnh quốc tế |
| Khoa | HIS_DEPARTMENT | Khoa điều trị |
| Phòng | HIS_ROOM / V_HIS_ROOM | Phòng khám, phòng điều trị |
| Giường | HIS_BED | Giường bệnh |

### Quy định đặc biệt

- **Soft delete**: `IS_DELETE = 1` (xóa mềm), `IS_ACTIVE = 0` (khóa). KHÔNG xóa vật lý
- **DateTime**: Kiểu `long`, format `yyyyMMddHHmmss`
- **BHYT**: Mã BHYT theo BHXH, MA_KHOA = `DEPARTMENT_BHYT_CODE` (KHÔNG phải DEPARTMENT_CODE)
- **Token**: SHA256 (64 chars) qua HTTP header `TokenCode` — dùng `TokenCodeStore`
- **Config**: `HisConfigs.Get<T>(key)` toàn viện, `ConfigApplicationWorker.Get<T>(key)` per-user

---

## 5. Cách Trả Lời Mong Muốn

### Ngôn ngữ
- Giao tiếp: **Tiếng Việt CÓ DẤU** (bắt buộc — mọi câu trả lời, câu hỏi lại đều phải có dấu)
- Code + comment trong code: **English**
- UI labels: **Tiếng Việt có dấu** (trong .resx)

### Khi sinh code
- **LUÔN** tuân thủ rules trong `.claude/rules/` — đọc rule TRƯỚC khi sinh code
- **LUÔN** kiểm tra UC/Library có sẵn trước khi tự tạo
- **LUÔN** sinh đầy đủ: try-catch, logging, WaitingManager, MessageManager, SessionManager
- **LUÔN** sinh Resources/ đa ngôn ngữ (vi + en)
- **LUÔN** sinh ControlState cho checkbox/toggle
- **LUÔN** sinh 4 cột audit cho grid load 1 bảng
- **LUÔN** cập nhật `hisnguonmo/docs/{ModuleLink}.md` khi sửa plugin (Changelog + sections liên quan)
- **KHÔNG** sinh code thiếu — phải đầy đủ từ Processor → Form → Save → Validate

### Khi review code
- Phân tích TRƯỚC — KHÔNG vội sửa
- Phân loại severity: CRITICAL → HIGH → MEDIUM → LOW
- Trình bày: vấn đề + tại sao + fix + side effects
- CHỜ user duyệt — KHÔNG tự sửa

### Khi debug
- Thu thập triệu chứng TRƯỚC
- Trace theo flow: Processor → Behavior → Form → API → Cache
- Tìm ROOT CAUSE — KHÔNG fix triệu chứng
- Đề xuất fix + kiểm tra side effects

### Khi thiết kế plugin mới
- Hỏi ĐỦ yêu cầu trước (chức năng, data, print, inter-plugin, localization)
- Thiết kế kiến trúc trước — trình bày → chờ duyệt → mới tạo code
- Chọn UC/Library phù hợp — KHÔNG tự tạo lại

### Format trả lời
- Ngắn gọn, trực tiếp, có code mẫu
- Bảng cho so sánh, danh sách cho checklist
- File:line khi chỉ ra vấn đề
- KHÔNG giải thích dài dòng — đi thẳng vào vấn đề

---

## 6. Tài Liệu Chi Tiết

Auto-load từ `.claude/rules/` khi làm việc với code tương ứng.

| Rule | Nội dung |
|------|----------|
| `coding_rules.md` | Naming, Architecture, API, Constants, Enum, Delegate |
| `ui_rules.md` | DevExpress 15.2, Layout, Grid, Validation, Language, ControlState, 4 cột audit |
| `uc_guide.md` | 134 UC catalog + public API |
| `performance.md` | Big-O, LINQ, Collections, Cache, Grid, Memory |
| `folder_structure.md` | Folder organization, Partial class, HintPath |
| `logging_guidelines.md` | LogSystem/LogAction, Levels, TraceData |
| `inter_plugin.md` | PluginInstance, ModuleLink, Args đầu vào |
| `print_integration.md` | MpsPrinter + 12 Print Libraries |
| `library_plugins_guide.md` | 41 Library plugins (CheckIcd, EmrGenerate...) |
| `message_localization.md` | 76 Message.Enum, MessageUtil, ResourceMessage |
| `module_docs.md` | Tài liệu hóa module — 9 sections, tên file = ModuleLink, folder chung docs/ |
| `naming_conventions.md` | Naming chi tiết |
