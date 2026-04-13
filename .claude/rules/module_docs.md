---
description: Tài liệu hóa module — BẮT BUỘC mỗi plugin có docs/ với thiết kế, nghiệp vụ, EFMODEL, UI, API, changelog, test. Áp dụng khi tạo/sửa plugin
paths:
  - "HIS/Plugins/**"
---

# Tài Liệu Hóa Module — Quy Tắc

## BẮT BUỘC

Mỗi plugin PHẢI có tài liệu. Khi **tạo mới** hoặc **sửa** plugin → cập nhật docs.

### Vị trí: Folder chung `hisnguonmo/docs/`

```
hisnguonmo/
└── docs/
    ├── HIS.Desktop.Plugins.HisMachine.md          ← Tên = ModuleLink
    ├── HIS.Desktop.Plugins.TreatmentList.md
    ├── HIS.Desktop.Plugins.ExecuteRoom.md
    ├── EMR.Desktop.Plugins.EmrDocumentList.md
    └── ...
```

**Tên file = ModuleLink** (Plugin ID từ `[ExtensionOf]`). KHÔNG viết tắt, KHÔNG đổi tên.

### Khi sửa plugin đã có docs

Nếu `docs/{ModuleLink}.md` **ĐÃ TỒN TẠI** → KHÔNG tạo mới, chỉ **CẬP NHẬT**:
1. Cập nhật **Section 8 (Changelog)** — thêm entry: ngày, người sửa, mô tả thay đổi
2. Cập nhật **section liên quan** đến nghiệp vụ đã sửa:
   - Sửa logic → cập nhật Section 2 (Quy trình nghiệp vụ)
   - Thêm API → cập nhật Section 5 (API Endpoints)
   - Thêm UC → cập nhật Section 4 (UI Layout)
   - Thêm EFMODEL → cập nhật Section 3 (EFMODEL)
   - Thêm print → cập nhật Section 7 (Print)
   - Thêm inter-plugin → cập nhật Section 6 (Dependencies)
3. KHÔNG xóa nội dung cũ — chỉ thêm/sửa phần thay đổi

### Khi tạo plugin mới

Tạo file `docs/{ModuleLink}.md` với đầy đủ 9 sections theo template bên dưới.

---

## Template Docs (9 Sections)

```markdown
# {Tên Plugin} — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.{Name} |
| Loại | Form / UC |
| Mục đích | {Mô tả chức năng nghiệp vụ — 1-3 câu} |
| Người tạo | {Tên} |
| Ngày tạo | {dd/MM/yyyy} |
| Trạng thái | Đang phát triển / Hoàn thành / Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
{Mô tả quy trình nghiệp vụ từ đầu đến cuối}

### Sơ đồ trạng thái (nếu có)
```
Nháp → Yêu cầu → Duyệt → Hoàn thành
         ↓
       Từ chối
```

### Điều kiện nghiệp vụ
- {Điều kiện 1: VD "Chỉ cho phép xuất khi tồn kho đủ"}
- {Điều kiện 2}

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT | View | Thông tin điều trị chính |
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ |
| V_HIS_SERE_SERV | View | Dịch vụ đã thực hiện |

### Quan hệ chính
- HIS_TREATMENT → HIS_SERVICE_REQ (1-n, qua TREATMENT_ID)
- HIS_SERVICE_REQ → HIS_SERE_SERV (1-n, qua SERVICE_REQ_ID)
- TDL_PATIENT_NAME denormalized từ HIS_PATIENT.VIR_PATIENT_NAME

## 4. UI Layout

### Sơ đồ giao diện
```
+----------------------------------------------------------+
| [Bộ lọc: Từ ngày] [Đến ngày] [Khoa] [Phòng] [Tìm kiếm] |
+----------------------------------------------------------+
| Grid danh sách                                            |
| STT | Mã BN | Họ tên | ... | TG tạo | Người tạo | ...  |
+----------------------------------------------------------+
| [Lưu] [Mới] [Sửa] [Xóa] [In]                            |
+----------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.DateEditor | panelDate | Chọn khoảng ngày |
| HIS.UC.Department | panelDept | Chọn khoa |
| Inventec.UC.Paging | panelPaging | Phân trang |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách | HisRequestUriStore.{URI_GET} | MosConsumer | {FilterClass} |
| Tạo mới | HisRequestUriStore.{URI_CREATE} | MosConsumer | — |
| Cập nhật | HisRequestUriStore.{URI_UPDATE} | MosConsumer | — |
| Xóa | HisRequestUriStore.{URI_DELETE} | MosConsumer | — |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| {Tên Library} | {Mục đích} |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| {Plugin} | {Điều kiện} | {Kiểu args} |

## 7. Print (nếu có)

| Loại in | PrintTypeCode | Library/MPS | Template |
|---------|--------------|-------------|----------|
| {Tên phiếu} | Mps000XXX | {Library} | {Template file} |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| dd/MM/yyyy | {Tên} | Tạo mới plugin |
| dd/MM/yyyy | {Tên} | {Mô tả thay đổi cụ thể} |

## 9. Test Cases

### Tạo mới
- [ ] Nhập đầy đủ thông tin → Lưu thành công
- [ ] Thiếu trường bắt buộc → Hiện validation error

### Sửa
- [ ] Chọn dòng → Load dữ liệu vào form
- [ ] Sửa → Lưu → Grid cập nhật

### Xóa
- [ ] Confirm dialog → Xóa thành công → Grid refresh

### In (nếu có)
- [ ] Chọn dòng → In → Preview hiện đúng data

### Nghiệp vụ đặc biệt
- [ ] {Test case riêng chức năng}
```

---

## Quy Tắc Cập Nhật

| Khi nào | Cập nhật gì |
|---------|-------------|
| **Tạo plugin mới** | Tạo `docs/{Name}.md` với đầy đủ 9 sections |
| **Sửa code** | Cập nhật Changelog (section 8) — ngày, người, mô tả |
| **Thêm API mới** | Cập nhật section 5 (API Endpoints) |
| **Thêm UC mới** | Cập nhật section 4 (UC sử dụng) |
| **Thêm print mới** | Cập nhật section 7 (Print) |
| **Thêm inter-plugin** | Cập nhật section 6 (Dependencies) |
| **Thêm EFMODEL mới** | Cập nhật section 3 (EFMODEL) |
| **Sửa nghiệp vụ** | Cập nhật section 2 (Quy trình) + section 9 (Test) |

## KHÔNG LÀM

- KHÔNG tạo docs rỗng — phải có nội dung thực tế
- KHÔNG bỏ qua Changelog — mỗi lần sửa PHẢI ghi
- KHÔNG để docs outdated — sửa code → sửa docs cùng lúc
- KHÔNG đặt docs ngoài plugin — docs PHẢI trong `{Plugin}/docs/`
