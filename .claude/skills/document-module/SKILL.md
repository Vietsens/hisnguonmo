---
name: document-module
description: Tài liệu hóa module — đọc code plugin, sinh docs đầy đủ 9 sections (tổng quan, nghiệp vụ, EFMODEL, UI, API, dependencies, print, changelog, test)
user-invocable: true
argument-hint: <plugin folder path VD: HIS/Plugins/HIS.Desktop.Plugins.HisMachine>
---

# Document Module — Tài Liệu Hóa Plugin

Target: $ARGUMENTS

## Bước 1: Đọc code plugin

Đọc TẤT CẢ .cs files trong plugin (KHÔNG đọc .Designer.cs trừ khi cần UI layout):

- Processor.cs → Plugin ID, Module Type (Form/UC)
- Behavior.cs → Args nhận, business logic
- Form/UC .cs → Load order, controls, Save/Delete pattern
- HisRequestUriStore.cs → API endpoints
- ADO/ → EFMODEL sử dụng
- Config/ → Config keys
- Resources/ → Ngôn ngữ
- KeyboardWorker.cs → Phím tắt

## Bước 2: Phân tích nghiệp vụ

Từ code, xác định:
- Plugin làm gì? (CRUD danh mục? Nghiệp vụ phức tạp? Báo cáo?)
- Luồng chính? (tạo → duyệt → hoàn thành?)
- Trạng thái? (IMSys.DbConfig constants nào được dùng?)
- Điều kiện nghiệp vụ? (validation, business rules)

## Bước 3: Phân tích EFMODEL

Tìm trong code:
- `BackendAdapter.GetRO<List<{Type}>>` → entity chính
- `BackendDataWorker.Get<{Type}>()` → entity danh mục
- `filter.{FIELD}` → fields lọc
- ADO classes → quan hệ entities
- TDL_ prefix → denormalized fields

## Bước 4: Phân tích UI

Đọc .Designer.cs nếu cần:
- LayoutControl → bố cục
- GridControl → grid columns
- UC sử dụng (search `using HIS.UC.`)
- Buttons chức năng

## Bước 5: Phân tích dependencies

- Library plugins: `using HIS.Desktop.Plugins.Library.`
- Inter-plugin: `PluginInstance.GetPluginInstance`, ModuleLinkString
- Print: PrintTypeCode, Print Library

## Bước 6: Kiểm tra docs đã tồn tại chưa

Xác định ModuleLink từ Processor.cs `[ExtensionOf]` → VD: `HIS.Desktop.Plugins.HisMachine`

Kiểm tra file `hisnguonmo/docs/{ModuleLink}.md`:
- **ĐÃ CÓ** → chỉ CẬP NHẬT sections liên quan + thêm Changelog entry
- **CHƯA CÓ** → tạo mới đầy đủ 9 sections

## Bước 7: Sinh hoặc cập nhật docs

File: `hisnguonmo/docs/{ModuleLink}.md` (VD: `docs/HIS.Desktop.Plugins.HisMachine.md`)

Theo template trong module_docs.md:

1. **Tổng quan** — Plugin ID, loại, mục đích, người tạo
2. **Quy trình nghiệp vụ** — luồng chính, sơ đồ trạng thái, điều kiện
3. **EFMODEL** — bảng entities + quan hệ + TDL_ fields
4. **UI Layout** — sơ đồ ASCII + bảng UC sử dụng
5. **API Endpoints** — bảng URI/consumer/filter cho mỗi action
6. **Dependencies** — Library plugins + inter-plugin
7. **Print** — PrintTypeCode + Library/MPS (nếu có)
8. **Changelog** — entry đầu tiên: ngày tạo docs
9. **Test cases** — checklist CRUD + nghiệp vụ đặc biệt

## Bước 7: Verify docs

- [ ] File `hisnguonmo/docs/{ModuleLink}.md` đã tạo hoặc cập nhật
- [ ] Tổng quan đầy đủ (Plugin ID, loại, mục đích)
- [ ] Nghiệp vụ mô tả đúng flow
- [ ] EFMODEL liệt kê đủ entities + quan hệ
- [ ] UI layout phản ánh giao diện thật
- [ ] API endpoints đủ (GET, CREATE, UPDATE, DELETE)
- [ ] Dependencies đúng (Library, inter-plugin)
- [ ] Print đúng (nếu có)
- [ ] Changelog có entry đầu tiên
- [ ] Test cases cover CRUD + nghiệp vụ

## Output

```
PLUGIN: {path}
DOCS: hisnguonmo/docs/{ModuleLink}.md

SECTIONS:
  ✓ 1. Tổng quan — {Plugin ID}, {loại}
  ✓ 2. Nghiệp vụ — {số flow}, {số trạng thái}
  ✓ 3. EFMODEL — {số entities}, {số quan hệ}
  ✓ 4. UI — {số UC}, {số controls chính}
  ✓ 5. API — {số endpoints}
  ✓ 6. Dependencies — {số Library}, {số inter-plugin}
  ✓ 7. Print — {số PrintTypeCode} (hoặc "Không có")
  ✓ 8. Changelog — Entry tạo docs
  ✓ 9. Test — {số test cases}
```
