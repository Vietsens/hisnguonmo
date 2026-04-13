---
description: Kiểm tra plugin có docs đầy đủ chưa — 9 sections theo module_docs.md
argument-hint: <plugin folder path>
---

# Review Docs — Kiểm Tra Tài Liệu Module

Kiểm tra: $ARGUMENTS

## 1. File docs tồn tại?
- Có folder `docs/` trong plugin?
- Có file `docs/{Name}.md`?
- File có nội dung (không rỗng)?

## 2. Tổng quan (Section 1)
- Có Plugin ID?
- Có loại (Form/UC)?
- Có mục đích mô tả?
- Có người tạo + ngày tạo?

## 3. Quy trình nghiệp vụ (Section 2)
- Có mô tả luồng chính?
- Có sơ đồ trạng thái (nếu có trạng thái)?
- Có điều kiện nghiệp vụ?

## 4. EFMODEL (Section 3)
- Có liệt kê entities sử dụng?
- Có mô tả quan hệ (FK, JOIN)?
- Có giải thích TDL_ fields?
- Entities trong docs KHỚP với code?

## 5. UI Layout (Section 4)
- Có sơ đồ giao diện?
- Có liệt kê UC sử dụng?
- UC trong docs KHỚP với code?

## 6. API Endpoints (Section 5)
- Có liệt kê đủ endpoints (GET, CREATE, UPDATE, DELETE)?
- URI trong docs KHỚP với HisRequestUriStore?

## 7. Dependencies (Section 6)
- Có liệt kê Library plugins?
- Có liệt kê inter-plugin connections?
- Dependencies trong docs KHỚP với .csproj references?

## 8. Print (Section 7)
- Nếu plugin có print → docs có section Print?
- PrintTypeCode trong docs KHỚP với code?

## 9. Changelog (Section 8)
- Có ít nhất 1 entry?
- Entry gần nhất có ngày + người + mô tả?
- Changelog phản ánh thay đổi gần nhất trong code?

## 10. Test cases (Section 9)
- Có test cases cho CRUD cơ bản?
- Có test cases cho nghiệp vụ đặc biệt?

## Output
[CRITICAL] Không có docs/ folder — tạo mới
[HIGH] Docs thiếu sections (liệt kê sections thiếu) — bổ sung
[MEDIUM] Docs outdated (code đã sửa nhưng docs chưa cập nhật) — cập nhật
[LOW] Changelog thiếu entry gần nhất — thêm entry
