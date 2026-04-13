# Hướng Dẫn Sử Dụng .claude — HIS Desktop

---

## 1. TỔNG QUAN — .claude Là Gì?

Folder `.claude/` là bộ cấu hình Claude Code (AI assistant) cho dự án HIS Desktop. Khi developer dùng Claude Code (CLI, VSCode Extension, Web) với repo `hisnguonmo/` — tất cả rules, commands, skills, agents sẽ **TỰ ĐỘNG khả dụng**.

### Cấu trúc

```
hisnguonmo/
├── CLAUDE.md                  ← Load MỌI session — tổng quan dự án, quy tắc bắt buộc
├── .claude/
│   ├── settings.json          ← Config shared (committed) — permissions, env
│   ├── settings.local.json    ← Config cá nhân (gitignored) — paths máy, tên dev
│   ├── rules/     (11 files)  ← Quy chuẩn code — TỰ ĐỘNG load khi đọc code
│   ├── commands/  (17 files)  ← Lệnh nhanh — gõ /command để dùng
│   ├── skills/    (13 dirs)   ← Workflow phức tạp — gõ /skill để dùng
│   ├── agents/    (8 files)   ← Chuyên gia AI — gọi trong hội thoại
│   └── README.md              ← File này
```

### Cách hoạt động

| File | Khi nào load | Cách dùng |
|------|-------------|-----------|
| `CLAUDE.md` | **Mọi session** bắt đầu | Tự động — Claude đọc trước khi trả lời |
| `settings.json` | Mọi session | Tự động — permissions + env |
| `settings.local.json` | Mọi session | Tự động — config riêng máy |
| `rules/*.md` | **Tự động** khi đọc file matching `paths:` | Tự động — không cần gọi |
| `commands/*.md` | Khi user gõ `/command` | Thủ công — gõ `/tên-command argument` |
| `skills/*/SKILL.md` | Khi user gõ `/skill` | Thủ công — gõ `/tên-skill argument` |
| `agents/*.md` | Khi user yêu cầu trong hội thoại | Thủ công — nói "dùng agent tên-agent" |

---

## 2. BẮT ĐẦU — Developer Mới Tham Gia Dự Án

### Bước 1: Clone 3 repos (BẮT BUỘC cùng cấp)

```bash
cd E:\GitLab                          # hoặc bất kỳ folder nào
git clone https://gitlab.vietsens.vn/ivt-dev/frontend/hisnguonmo.git
git clone https://gitlab.vietsens.vn/ivt-dev/frontend/lib.git
git clone https://gitlab.vietsens.vn/ivt-dev/frontend/common.git
```

Kết quả:
```
E:\GitLab\
├── hisnguonmo\    ← Repo chính
├── lib\           ← Backend models + DLLs (PHẢI cùng cấp)
└── common\        ← Thư viện dùng chung (PHẢI cùng cấp)
```

### Bước 2: Tạo branch cá nhân

```bash
cd hisnguonmo
git checkout Develop
git checkout -b {TenBan}     # VD: git checkout -b Nampp
```

### Bước 3: Tạo settings.local.json

File này CÁ NHÂN (gitignored) — mỗi dev tạo riêng. Tạo file `.claude/settings.local.json`:

```json
{
  "permissions": {
    "allow": [
      "Read(E:/GitLab/lib/**)",
      "Read(E:/GitLab/common/**)",
      "Bash(ls:*)",
      "Bash(find:*)",
      "Bash(mkdir:*)",
      "Bash(git checkout:*)",
      "Bash(git merge:*)",
      "Bash(git push:*)",
      "Bash(git stash:*)"
    ],
    "deny": [
      "Read(E:/GitLab/histest/**)"
    ],
    "additionalDirectories": [
      "E:\\GitLab\\lib",
      "E:\\GitLab\\common"
    ]
  },
  "env": {
    "DEVELOPER_NAME": "{TenBan}",
    "DEVELOPER_BRANCH": "{BranchCaNhan}",
    "VCONG_PREFIX": "[vCong{SoCuaBan}]",
    "LOCAL_REPO_PATH": "E:/GitLab/hisnguonmo",
    "LIB_PATH": "E:/GitLab/lib",
    "COMMON_PATH": "E:/GitLab/common",
    "TEST_API_URL": "http://{test-server}:{port}",
    "MSBUILD_PATH": "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/MSBuild.exe"
  }
}
```

**Thay tất cả `{...}` bằng thông tin của bạn.**

### Bước 4: Mở Claude Code

- **VSCode**: Cài extension Claude Code → mở folder hisnguonmo → Claude tự động load `.claude/`
- **CLI**: `cd hisnguonmo && claude` → session bắt đầu, CLAUDE.md tự động load
- **Web**: claude.ai/code → chọn repo hisnguonmo

---

## 3. RULES — Quy Chuẩn Tự Động (11 files)

**KHÔNG cần làm gì** — rules tự động áp dụng khi Claude đọc code.

### Rules hoạt động như thế nào?

Mỗi rule có `paths:` trong frontmatter:
```yaml
---
paths:
  - "HIS/Plugins/**"
  - "UC/**"
---
```
Khi Claude đọc file matching paths → rule TỰ ĐỘNG load vào context.

### 11 Rules hiện có

| Rule | Tự động khi đọc | Nội dung chính |
|------|----------------|---------------|
| **coding_rules.md** | `HIS/**`, `MPS/**`, `UC/**`, `Common/**` | Architecture Processor/Factory/Behavior, API BackendAdapter, Constants IMSys.DbConfig, Enum XML comment, Delegate, DateTime long, Config HisConfigs |
| **ui_rules.md** | `HIS/Plugins/**`, `UC/**` | DevExpress 15.2.9, LayoutControl, GridControl (4 cột audit), Validation, Phím tắt, ControlState, SetCaptionByLanguageKey BẮT BUỘC, Load order, Tối ưu Grid/Layout/Memory/UX |
| **uc_guide.md** | `HIS/Plugins/**`, `UC/**` | Catalog 134 UC — Processor API (Run/GetValue/SetValue/Reload/Validate), InitADO, code mẫu. Bảng tra cứu: cần gì → dùng UC nào |
| **performance.md** | `HIS/Plugins/**`, `UC/**`, `MPS/**` | Big-O tham chiếu, Dictionary O(1) thay FirstOrDefault O(n), HashSet cho Contains, StringBuilder thay +=, BeginUpdate/EndUpdate, Server-side paging |
| **folder_structure.md** | `HIS/Plugins/**` | 3 cấp độ plugin (Simple/Medium/Complex), vai trò 16 folders, partial class naming (__ form, ___ UC), HintPath .csproj, Resources/, Config/, GlobalStore |
| **logging_guidelines.md** | `HIS/Plugins/**`, `UC/**`, `MPS/**`, `Common/**` | 5 Logger classes (LogSystem/LogAction/LogSession/LogFilter/LogTime), Error vs Warn vs Debug, LogUtil.TraceData, LogAction audit, Anti-patterns |
| **inter_plugin.md** | `HIS/Plugins/**` | PluginInstance.GetPluginInstance, ModuleLinkString constants, XÁC ĐỊNH ĐẦU VÀO plugin đích (đọc Behavior.Run trước), args parse bằng `is`, null check, room context |
| **print_integration.md** | `HIS/Plugins/**`, `HIS/HIS.Desktop.Print/**` | 2 cách in: 12 Print Libraries (ƯU TIÊN) vs MpsPrinter.Run trực tiếp. PrintTypeCode constants, PDO, PreviewType, EMR sign, 7 properties Barcode |
| **library_plugins_guide.md** | `HIS/Plugins/**` | 41 Library plugins: CheckIcd, CheckHeinGOV, EmrGenerate, BankHub, ElectronicBill, 12 Print, 5 Form, 5 nghiệp vụ. CacheClient ControlStateWorker + CacheWorker |
| **message_localization.md** | `HIS/Plugins/**`, `HIS/HIS.Desktop.LibraryMessage/**` | 76 Message.Enum (kết quả, tiêu đề, confirm, validation, hệ thống), MessageUtil API (10 methods), ResourceMessage per-plugin, FontendMessage cache, SetCaptionByLanguageKey |
| **naming_conventions.md** | Tất cả | PascalCase class/method, camelCase var, Control prefix (btn/txt/cbo/grd), Namespace, Menu naming, Enum, Event |

### Kiểm tra rules đang áp dụng

Trong session Claude Code, hỏi:
```
Những rules nào đang áp dụng cho file này?
```
Claude sẽ liệt kê rules đã load dựa trên `paths:` matching.

---

## 4. COMMANDS — Lệnh Nhanh (17 files)

Gõ `/` trong Claude Code → thấy danh sách. Chọn command + nhập argument.

### 4.1 Nhóm REVIEW — Kiểm tra code theo quy chuẩn

| Command | Argument | Làm gì |
|---------|----------|--------|
| `/review-code` | file hoặc folder | Review coding_rules: naming, architecture, API, constants, enum, delegate, exception |
| `/review-ui` | file hoặc folder | Review ui_rules: FormBase, layout, grid (4 cột audit), validation, language, ControlState, tối ưu |
| `/review-performance` | file hoặc folder | Review performance: O(n²), Get trong loop, Count()>0, string+=, grid, API, memory |
| `/review-logging` | file hoặc folder | Review logging: try-catch, log level, TraceData, catch rỗng, Console.Write, LogAction audit |
| `/review-structure` | plugin folder | Review folder: vai trò folder, partial class, AssemblyInfo, Resources |
| `/review-print` | file hoặc folder | Review print: Print Library ưu tiên, PrintTypeCode constant, PDO, Barcode 7 properties |
| `/review-mps` | Mps code hoặc path | Review MPS Processor: AbstractProcessor, ProcessData, SetBarcodeKey, SetSingleKey |

**Ví dụ cụ thể:**
```
/review-code HIS/Plugins/HIS.Desktop.Plugins.HisMachine/HisMachineForm.cs

→ Output:
[CRITICAL] Line 45: Hardcode == 2 → dùng IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
[HIGH] Line 120: Catch rỗng → thêm LogSystem.Warn(ex)
[MEDIUM] Line 200: Count() > 0 → đổi sang Any()
[LOW] Line 15: Variable tên `x` → đổi sang tên rõ nghĩa
```

### 4.2 Nhóm FIND — Tìm kiếm nhanh

| Command | Argument | Trả về |
|---------|----------|--------|
| `/find-uc` | chức năng tiếng Việt | UC phù hợp + Processor API + code mẫu |
| `/find-library` | chức năng | Library plugin + method chính + reference csproj |
| `/find-message` | nội dung thông báo | Message.Enum + MessageUtil code mẫu |
| `/find-delegate` | mục đích | Delegate type + cách dùng 4 bước |
| `/find-ado` | entity name | ADO class + path + properties + plugins dùng |

**Ví dụ cụ thể:**
```
/find-uc chọn ngày giờ

→ Output:
UC: HIS.UC.DateEditor (25 plugins dùng)
Giao diện: DateEdit + TimePicker + checkbox multi-date
Processor API:
  Run(DateInitADO) → UserControl
  GetValue(uc) → List<long> (yyyyMMddHHmm00)
  Reload(uc, DateInputADO)
  ValidationForm(uc) → bool
Code mẫu:
  var proc = new UCDateProcessor();
  var ado = new DateInitADO();
  ado.IsVisibleMultiDate = true;
  UserControl uc = (UserControl)proc.Run(ado);
  panelDate.Controls.Add(uc);
  uc.Dock = DockStyle.Fill;
```

### 4.3 Nhóm TRACE — Theo dõi luồng

| Command | Argument | Trace gì |
|---------|----------|---------|
| `/trace-cache` | EFMODEL type | Cache flow: RAM → SQLite/Redis → API + auto-filter + callers Get/Reset |
| `/trace-data` | field/control/entity | Data flow: UI → ADO → API → EFMODEL → Backend table |

**Ví dụ cụ thể:**
```
/trace-cache V_HIS_SERVICE

→ Output:
Type: MOS.EFMODEL.DataModels.V_HIS_SERVICE
Cache: RAM (ConcurrentDictionary) → API
Backend: MOS via MosGetListBehavior
API URI: HisRequestUriStore.MOSHIS_HIS_SERVICE_GETVIEW
Auto-filter: không
Callers Get: [38 files]
Callers Reset: [5 files]
```

### 4.4 Nhóm CHECK — Kiểm tra cụ thể

| Command | Argument | Kiểm tra |
|---------|----------|---------|
| `/check-thread` | file hoặc folder | Thread safety: InvokeRequired, lock, WaitingManager, timer |
| `/check-inter-plugin` | file hoặc folder | Inter-plugin: ModuleLink, args kiểu, null check, Behavior.Run đích |
| `/check-migration` | mô tả thay đổi | Scan phạm vi: plugins/MPS/UC ảnh hưởng + thứ tự update |

---

## 5. SKILLS — Workflow Sinh Code (13 skills)

Gõ `/` → chọn skill → nhập argument → Claude thực hiện nhiều bước tự động.

### 5.1 Nhóm TẠO MỚI — Sinh code từ đầu

| Skill | Argument | Sinh gì |
|-------|----------|---------|
| `/scaffold-form` | tên form | Processor + Factory + Behavior + Form (FormBase, SetIcon, Load order, CRUD, ControlState, Validation, Language) + AssemblyInfo |
| `/scaffold-uc` | tên UC | Tương tự scaffold-form + KeyboardWorker thay BarManager |
| `/scaffold-mps` | Mps code + mô tả | PDO (RDOBase) + Processor (AbstractProcessor, ProcessData, SetBarcodeKey 7 props, SetSingleKey) + ExtendSingleKey |

**Ví dụ cụ thể:**
```
/scaffold-form frmHisMachine

→ Claude tạo 7 files:
1. HisMachineProcessor.cs — [ExtensionOf], Run(args), IsEnable()
2. IHisMachine.cs — interface
3. HisMachineFactory.cs — MakeIControl, catch NullReferenceException + TraceData
4. HisMachineBehavior.cs — BusinessBase, parse args, tạo Form
5. frmHisMachine.cs — FormBase, SetIcon, Load order 7 bước, SaveProcess, DeleteProcess, ControlState, SetCaptionByLanguageKey
6. AssemblyInfo.cs — [assembly: Plugin]
7. Resources/ — Lang.vi/en.resx, Message.vi/en.resx, ResourceLanguageManager, ResourceMessage
```

### 5.2 Nhóm TÍCH HỢP — Kết nối hệ thống

| Skill | Argument | Làm gì |
|-------|----------|--------|
| `/wire-inter-plugin` | plugin hiện tại + plugin đích | Đọc Behavior.Run đích → xác định args → tạo ModuleLinkString → sinh code PluginInstance + callback + đối chiếu kiểu |
| `/integrate-library` | chức năng | Tìm Library phù hợp → đọc API → sinh code khởi tạo + gọi + reference csproj |
| `/add-print` | plugin + loại in | Chọn Print Library/MpsPrinter → reference → PrintTypeCodeWorker → sinh code button + callback + PDO |
| `/setup-localization` | plugin path | Tạo Resources/ đầy đủ: Lang.vi/en, Message.vi/en, ResourceLanguageManager, ResourceMessage, SetCaptionByLanguageKey |
| `/suggest-uc` | file hoặc mô tả | Phân tích form → gợi ý UC thay thế control tự tạo → sinh code tích hợp |

**Ví dụ cụ thể:**
```
/wire-inter-plugin từ ExecuteRoom mở ContentSubclinical

→ Claude thực hiện:
Bước 1: Đọc ContentSubclinicalBehavior.Run() → tìm args cần truyền
Bước 2: Liệt kê: entity[i] is long (treatmentId), entity[i] is DelegateSelectData
Bước 3: Kiểm tra/tạo ModuleLinkString.cs
Bước 4: Sinh code OpenContentSubclinical() đầy đủ:
  - Tìm module, null check
  - Tạo args ĐỦ kiểu đúng (long, DelegateSelectData)
  - GetModuleWithWorkingRoom
  - ShowDialog
  - Callback method
Bước 5: Bảng đối chiếu kiểu
Bước 6: Checklist verify
```

### 5.3 Nhóm FIX — Tìm và sửa tự động

| Skill | Argument | Làm gì |
|-------|----------|--------|
| `/optimize-code` | file hoặc folder | Tìm 8 loại anti-pattern (O(n²), Get loop, string+=, Count>0...) → sinh code fix |
| `/fix-logging` | file hoặc folder | Tìm 8 loại lỗi logging (catch rỗng, sai level, thiếu TraceData...) → sinh code fix |
| `/migrate-ui` | file form/UC | Checklist 15+ items → fix: SetIcon, Load order, Language, ControlState, 4 cột audit, Layout |

### 5.4 Nhóm TRACE — Phân tích luồng

| Skill | Argument | Trace gì |
|-------|----------|---------|
| `/trace-print` | plugin name hoặc Mps code | End-to-end: button → PrintTypeCode → MPS Processor → Template → output |
| `/debug-cache` | triệu chứng | BackendDataWorker: stale → thiếu Reset? rỗng → sai filter? không refresh → timer? |

---

## 6. AGENTS — Chuyên Gia AI (8 agents)

Agents là chuyên gia — phân tích sâu, kết hợp nhiều rules, CHỜ DUYỆT trước khi sửa code.

### Cách gọi agent

Trong conversation, nói:
```
Hãy dùng agent {tên-agent}: {yêu cầu cụ thể}
```

Hoặc:
```
Gọi agent {tên-agent} để {mô tả việc cần làm}
```

### 8 Agents hiện có

#### `code-reviewer` — Review toàn diện

**Khi dùng:** Review 1 file/folder trước khi commit hoặc merge.
**Làm gì:** Chạy 7 checklist ĐỒNG THỜI: coding + performance + logging + naming + structure + security + UI.
**Output:** Report phân loại CRITICAL/HIGH/MEDIUM/LOW + thứ tự fix + effort.

```
Hãy dùng agent code-reviewer để review folder HIS.Desktop.Plugins.HisMachine

→ Agent đọc TẤT CẢ .cs files → chạy 7 checklist → trả về:
# CODE REVIEW REPORT
Files reviewed: 8
Issues: CRITICAL: 1, HIGH: 3, MEDIUM: 5, LOW: 2

[CRITICAL] HisMachineForm.cs:45 — Hardcode == 2 → IMSys.DbConfig
[HIGH] HisMachineForm.cs:120 — Catch rỗng → LogSystem.Warn
[HIGH] HisMachineForm.cs:200 — Get<T> trong loop → Dictionary
...
Khuyến nghị: Fix CRITICAL trước, HIGH sau. Effort: 2h.
Chờ duyệt — bạn muốn fix issues nào?
```

#### `plugin-builder` — Thiết kế plugin mới

**Khi dùng:** Thiết kế plugin phức tạp (nhiều form, nhiều UC, print, inter-plugin).
**Làm gì:** Hỏi yêu cầu → thiết kế kiến trúc → chọn UC/Library → trình bày → chờ duyệt.
**Output:** Thiết kế đầy đủ: folder structure, UC list, Library list, inter-plugin, print, localization, dependencies.

```
Hãy dùng agent plugin-builder để thiết kế plugin quản lý máy xét nghiệm

→ Agent hỏi: Có những chức năng nào? Có in ấn không? Có liên kết plugin nào?
→ User trả lời
→ Agent trình bày:

# PLUGIN DESIGN: HIS.Desktop.Plugins.HisMachine
Độ phức tạp: Medium
UC sử dụng: HIS.UC.Department, HIS.UC.Room, HIS.UC.DateEditor
Library: CheckIcd (nếu cần)
Print: PrintOtherForm (phiếu kiểm định máy)
Folder structure: {cây thư mục}
Dependencies: {.csproj references}

Chờ duyệt — bạn đồng ý thiết kế này không?
```

#### `bug-fixer` — Fix bug

**Khi dùng:** Có bug cần fix — biết triệu chứng nhưng chưa biết nguyên nhân.
**Làm gì:** Thu thập triệu chứng → phân loại (10 loại bug) → trace 8 bước → tìm root cause → đề xuất fix → kiểm tra side effects → CHỜ DUYỆT.
**KHÔNG TỰ SỬA** — luôn trình bày trước.

```
Hãy dùng agent bug-fixer: grid bệnh nhân hiện dữ liệu cũ sau khi cập nhật tên

→ Agent phân tích:
Triệu chứng: Data cũ (stale) → vùng nghi ngờ: Cache
Trace: BackendDataWorker.Get<V_HIS_PATIENT>()
  → Plugin UpdatePatient gọi BackendAdapter.Post → thành công
  → THIẾU BackendDataWorker.Reset<V_HIS_PATIENT>() sau Post
ROOT CAUSE: Plugin không gọi Reset cache sau update

FIX:
File: frmUpdatePatient.cs:245
Thêm: BackendDataWorker.Reset<V_HIS_PATIENT>();
Sau: var result = new BackendAdapter(param).Post<HIS_PATIENT>(...);

Side effects: Tất cả plugin khác đọc V_HIS_PATIENT sẽ lấy data mới → OK
Chờ duyệt — bạn đồng ý fix này không?
```

#### `integration-expert` — Debug tích hợp

**Khi dùng:** Lỗi giao tiếp giữa plugins, Library, Print, WCF, BHYT, ký số.
**Làm gì:** Trace luồng tích hợp → xác định root cause (config sai, kiểu sai, thiếu tham số, logic sai, dependency thiếu).

```
Hãy dùng agent integration-expert: plugin ExecuteRoom không mở được ContentSubclinical

→ Agent trace:
1. ModuleLink đúng? → OK
2. Args truyền? → THIẾU treatmentId (long)
3. Behavior.Run() cần: long treatmentId (BẮT BUỘC)
4. Plugin cha chỉ truyền: Module, DelegateSelectData → THIẾU long

ROOT CAUSE: Args thiếu treatmentId
FIX: listArgs.Add((long)this.treatmentId); // TRƯỚC delegate
```

#### `ui-optimizer` — Tối ưu giao diện

**Khi dùng:** Form chậm, cần refactor UI, cần chuẩn hóa.
**Làm gì:** Scan 4 khía cạnh: UC replacement, Grid performance, Layout quality, Localization.

```
Hãy dùng agent ui-optimizer để tối ưu form frmAssignPrescription

→ Agent phân tích:
UC Replacement:
  Line 150: 3 ComboBox Tỉnh/Huyện/Xã → HIS.UC.AddressCombo (giảm 150 dòng)
  Line 300: DateEdit tự tạo → HIS.UC.DateEditor (giảm 50 dòng)
Grid Performance:
  Line 500: API trong CustomUnboundColumnData → pre-compute ADO
Localization:
  45 hardcode strings cần thay MessageUtil/ResourceMessage
Chờ duyệt?
```

#### `data-flow-tracer` — Hiểu data

**Khi dùng:** Cần biết "data này từ đâu ra?", "field này map từ bảng nào?".

```
Hãy dùng agent data-flow-tracer: field TDL_PATIENT_NAME từ đâu ra?

→ Agent trace:
UI: gridColumn "PATIENT_NAME" → FieldName: TDL_PATIENT_NAME
Data: List<V_HIS_TREATMENT> → gán tại GridPaging():120
API: HisRequestUriStore.TREATMENT_GETVIEW via MosConsumer
EFMODEL: V_HIS_TREATMENT.TDL_PATIENT_NAME
Backend: HIS_TREATMENT.TDL_PATIENT_NAME (denormalized từ HIS_PATIENT.VIR_PATIENT_NAME)
```

#### `mps-developer` — Tạo/sửa MPS Processor

**Khi dùng:** Tạo processor in mới hoặc sửa processor cũ.

```
Hãy dùng agent mps-developer để tạo Mps000999 in phiếu xuất thuốc

→ Agent thiết kế:
PDO: Mps000999PDO kế thừa RDOBase — properties: ExpMest, SereServs, Patient, Treatment
Processor: override ProcessData() — ReadTemplate → SetBarcodeKey (7 props) → SetSingleKey → objectTag
ExtendSingleKey: BARCODE_PATIENT_CODE, FINISH_TIME_STR...
Template keys: {PATIENT_NAME}, {TREATMENT_CODE}, {BARCODE_PATIENT_CODE}
Chờ duyệt?
```

#### `migration-assistant` — Thay đổi lớn

**Khi dùng:** Backend đổi EFMODEL, API đổi, BHXH ra QĐ mới → cần scan + update nhiều plugins.

```
Hãy dùng agent migration-assistant: backend thêm field IS_EMERGENCY vào HIS_TREATMENT

→ Agent scan:
Phạm vi: 45 plugins + 12 MPS + 3 UC dùng HIS_TREATMENT
Thứ tự update:
  Phase 1: HIS.Desktop.ADO (thêm vào TreatmentADO)
  Phase 2: Plugins cần hiển thị IS_EMERGENCY (15 plugins)
  Phase 3: Plugins chỉ đọc treatment (30 plugins — không ảnh hưởng)
  Phase 4: MPS Processors cần in (12 processors)
Chờ duyệt plan?
```

---

## 7. WORKFLOW PHỔ BIẾN — Từng Bước

### 7.1 Tạo plugin mới từ đầu

```
Bước 1: /scaffold-form frmHisMachine
  → Sinh 7 files: Processor, Factory, Behavior, Form, AssemblyInfo, Resources

Bước 2: /suggest-uc form quản lý máy xét nghiệm
  → Gợi ý: HIS.UC.Department, HIS.UC.Room, HIS.UC.DateEditor

Bước 3: /setup-localization HIS/Plugins/HIS.Desktop.Plugins.HisMachine
  → Sinh Lang.vi/en, Message.vi/en, ResourceLanguageManager, ResourceMessage

Bước 4: /add-print HIS.Desktop.Plugins.HisMachine in phiếu kiểm định
  → Chọn Print Library + sinh code button + callback

Bước 5: /wire-inter-plugin từ HisMachine mở HisService
  → Sinh code PluginInstance + callback

Bước 6: /review-code HIS/Plugins/HIS.Desktop.Plugins.HisMachine/
  → Review toàn bộ trước commit
```

### 7.2 Fix bug

```
Bước 1: Gọi agent bug-fixer: mô tả triệu chứng
  → Agent phân tích root cause → trình bày fix

Bước 2: Duyệt fix → agent thực hiện

Bước 3: /review-code file_da_sua.cs
  → Verify không phát sinh lỗi mới
```

### 7.3 Upgrade code cũ

```
Bước 1: /migrate-ui HIS/Plugins/HIS.Desktop.Plugins.OldPlugin/frmOldForm.cs
  → Checklist + fix: SetIcon, Language, ControlState, 4 cột audit, Layout

Bước 2: /optimize-code HIS/Plugins/HIS.Desktop.Plugins.OldPlugin/
  → Fix O(n²), Get loop, string+=

Bước 3: /fix-logging HIS/Plugins/HIS.Desktop.Plugins.OldPlugin/
  → Fix catch rỗng, sai level, thiếu TraceData
```

### 7.4 Backend thay đổi

```
Bước 1: /check-migration thêm field IS_EMERGENCY vào HIS_TREATMENT
  → Scan phạm vi ảnh hưởng

Bước 2: Gọi agent migration-assistant
  → Plan chi tiết theo phase

Bước 3: Fix theo phase: Infrastructure → Shared → Plugins → MPS → UC
```

### 7.5 Tạo MPS Processor in

```
Bước 1: /scaffold-mps Mps000999 in phiếu xuất thuốc
  → Sinh PDO + Processor + ExtendSingleKey

Bước 2: /review-mps Mps000999
  → Verify: AbstractProcessor, ProcessData, Barcode 7 props
```

---

## 8. LƯU Ý QUAN TRỌNG

### KHÔNG sửa trực tiếp
- **rules/*.md** — chỉ sửa khi TEAM thống nhất đổi quy chuẩn
- **settings.json** — chỉ sửa khi đổi permissions/env CHUNG cho team
- **CLAUDE.md** — chỉ sửa khi đổi tổng quan dự án

### CÁ NHÂN
- **settings.local.json** — MỖI dev tạo riêng, KHÔNG commit

### Rules TỰ ĐỘNG
- KHÔNG cần gõ lệnh để load rules — tự động khi đọc code
- KHÔNG cần nhớ hết rules — Claude đã nhớ và áp dụng

### Agents CHỜ DUYỆT
- Agents KHÔNG tự sửa code — luôn trình bày trước
- Chỉ sửa SAU KHI bạn xác nhận

### Hiệu quả nhất khi
- Mô tả YÊU CẦU rõ ràng (càng cụ thể càng tốt)
- Cung cấp FILE PATH cụ thể (không chung chung)
- Trả lời câu hỏi của Claude/Agent (dùng "không biết" nếu không biết)
- Duyệt từng bước (không "làm hết đi")
