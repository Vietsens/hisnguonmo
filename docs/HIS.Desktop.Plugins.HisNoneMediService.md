# HIS.Desktop.Plugins.HisNoneMediService — Dịch vụ ngoài khám chữa bệnh

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisNoneMediService |
| Loại | Form (FormBase) |
| Mục đích | Quản lý danh mục dịch vụ ngoài khám chữa bệnh (CRUD + khoá / mở khoá). Dùng cho thanh toán khác — vận chuyển, giường, sổ khám bệnh, dịch vụ người nhà, v.v. |
| Liên quan | Nghiệp vụ #42922 — bổ sung GOODS_TYPE_ID (loại dịch vụ) bắt buộc |
| Trạng thái | Đang bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính

```
Mở module → GetMachine + GetGoodsType (load 1 lần)
  → Load grid + các combobox
    → Người dùng nhập Mã / Tên / Đơn vị / VAT / Loại dịch vụ / Giá / Số thứ tự
      → Validate (Mã 3 ký tự, Tên 1500 ký tự, Loại dịch vụ + Đơn vị bắt buộc)
        → Lưu Create / Update → Backend validate trùng Mã + GOODS_TYPE_ID hợp lệ
          → MessageManager + refresh grid
```

### Quy tắc dữ liệu

- Mã (NONE_MEDI_SERVICE_CODE): bắt buộc, max 3 ký tự, UNIQUE.
- Tên (NONE_MEDI_SERVICE_NAME): max 1500 ký tự (không bắt buộc).
- Đơn vị dịch vụ (SERVICE_UNIT_ID): bắt buộc.
- Loại dịch vụ (GOODS_TYPE_ID): bắt buộc — bổ sung từ #42922.
- Giá / Số thứ tự / VAT: số không âm.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_NONE_MEDI_SERVICE | Table | Dịch vụ ngoài KCB (đối tượng chính) |
| HIS_SERVICE_UNIT | Table | Đơn vị dịch vụ — combobox |
| HIS_GOODS_TYPE | Table | Loại dịch vụ — combobox (#42922) |

### Cột bổ sung (#42922)

| Bảng | Cột | Mô tả |
|------|-----|-------|
| HIS_NONE_MEDI_SERVICE | GOODS_TYPE_ID (NUMBER(19), nullable trong DB nhưng UI bắt buộc) | FK → HIS_GOODS_TYPE.ID |

## 4. UI Layout

### Sơ đồ giao diện

```
+-----------------------------------------------------------------+
| [Tìm (Ctrl F)] [ Từ khoá tìm kiếm.................... ]         |
+-----------------------------------------------+-----------------+
| Grid danh sách                                 | Mã: __________|
| STT|Lock|Edit|Mã|Tên|Loại DV|Giá|VAT           | Tên: _________|
|    |Đơn vị|Số thứ tự|Trạng thái|Tg tạo|...     | Giá: _________|
|                                                | Đơn vị DV: ___|
|                                                | VAT: _________|
| [Phân trang]                                   | Số thứ tự: ___|
|                                                | Loại DV (mới): __ ← Section #42922
|                                                | [Sửa] [Thêm] [Lại]
+-----------------------------------------------+-----------------+
```

### Control mới (#42922)

| Control | Type | Mô tả |
|---------|------|-------|
| cboGoodsType | LookUpEdit | Combobox Loại dịch vụ — Maroon (bắt buộc), load từ HIS_GOODS_TYPE.IS_ACTIVE = 1 |
| lciGoodsType | LayoutControlItem | Caption "Loại dịch vụ:" — Maroon |
| grdColGoodsType | GridColumn | Cột grid hiển thị tên loại dịch vụ (unbound, lookup từ listGoodsType) |

### Phím tắt

| Tổ hợp | Hành động |
|--------|-----------|
| Ctrl + F | Tìm kiếm |
| Ctrl + S | Sửa |
| Ctrl + N | Thêm |
| Ctrl + R | Làm lại |
| F2 | Focus về ô tìm kiếm |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy danh sách dịch vụ ngoài KCB | `api/HisNoneMediService/Get` | MosConsumer |
| Tạo mới | `api/HisNoneMediService/Create` | MosConsumer |
| Cập nhật | `api/HisNoneMediService/Update` | MosConsumer |
| Xoá mềm | `api/HisNoneMediService/Delete` | MosConsumer |
| Khoá / Mở khoá | `api/HisNoneMediService/ChangeLock` | MosConsumer |
| Lấy đơn vị dịch vụ | `api/HisServiceUnit/Get` | MosConsumer |
| Lấy loại dịch vụ (#42922) | `api/HisGoodsType/Get` | MosConsumer |

URI constants ở `HisRequestUriStore.cs`.

## 6. Dependencies

### Library Plugins
- Không dùng library chuyên biệt. Dùng UC chuẩn DevExpress + Inventec.UC.Paging.

### Inter-Plugin
Plugin có thể được mở từ plugin cha với `RefeshReference` delegate để báo cha refresh sau khi sửa danh mục.

## 7. Print
Không in ấn.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 28/04/2026 | anhnh2 | Bổ sung combobox **Loại dịch vụ** (cboGoodsType) — bắt buộc — theo nghiệp vụ #42922. Bind dữ liệu từ HIS_GOODS_TYPE.IS_ACTIVE = 1, sắp xếp theo NUM_ORDER. Thêm cột GOODS_TYPE_NAME vào grid. Validate `cboGoodsType` qua `dxValidationProviderEditorInfo`. Cập nhật UpdateDTOFromDataForm / FillDataToEditorControl / ResetFormData / SetCaptionByLanguageKey / Lang.vi.resx + Lang.en.resx. Bổ sung URI `HIS_GOODS_TYPE_GET` vào `HisRequestUriStore.cs`. Thêm phím Enter từ cboServiceUnit → cboGoodsType → spinVat. |

## 9. Test Cases

### Validation Loại dịch vụ (#42922)
- [ ] Tạo mới — không chọn Loại dịch vụ → Hiện lỗi "Trường dữ liệu bắt buộc" tại cboGoodsType.
- [ ] Tạo mới — chọn Loại dịch vụ → Lưu thành công, grid hiện cột GOODS_TYPE_NAME đúng.
- [ ] Sửa dòng đã có Loại dịch vụ → cboGoodsType binding đúng giá trị.
- [ ] Đổi Loại dịch vụ → Lưu → Grid cập nhật.
- [ ] Loại dịch vụ trong combobox phải sắp xếp theo NUM_ORDER (Vận chuyển → Giường → Sổ khám bệnh → DV người nhà → Khác).

### Validation cũ (smoke)
- [ ] Mã / Đơn vị dịch vụ vẫn bắt buộc.
- [ ] Trùng Mã → Backend trả lỗi.

### Khoá / Mở khoá / Xoá
- [ ] Hoạt động không thay đổi sau khi thêm GOODS_TYPE_ID.

### Đa ngôn ngữ
- [ ] Đổi sang en → label "Goods Type:" + cột "Goods Type" đúng.
