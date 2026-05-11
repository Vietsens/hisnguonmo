# HIS.Desktop.Plugins.SereServTemplate — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.SereServTemplate |
| Loại | Form (FormSereServTemplate) |
| Mục đích | Quản lý danh mục mẫu mô tả/kết luận dịch vụ (HIS_SERE_SERV_TEMP). Cho phép soạn mẫu RichText (Word) cho từng loại dịch vụ kèm cấu hình hành vi: dịch vụ áp dụng, giới tính, phòng xử lý, loại/nhóm văn bản EMR, nghiệp vụ ký, ánh xạ EMR, ánh xạ key tài khoản → key chữ ký để sinh chữ ký tự động. |
| Người tạo | INVENTEC |
| Ngày tạo | — |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form từ menu hoặc inter-plugin (truyền `service_id`, `serviceTypeIds`).
2. Form load: combobox dịch vụ/loại dịch vụ/giới tính/phòng xử lý/loại + nhóm văn bản/nghiệp vụ ký, sau đó load grid mẫu (paging).
3. Người dùng:
   - Bấm dòng grid → form hiện chi tiết mẫu (ActionEdit).
   - Bấm "Mới (Ctrl N)" → reset form (ActionAdd).
4. Soạn nội dung (RichEdit) + nhập mã/tên + ánh xạ EMR + cấu hình ánh xạ key tài khoản → key chữ ký.
5. Bấm "Lưu (Ctrl S)" → validate (cả validation field cơ bản và validate JSON `GEN_SIGNATURE_BY_KEY_CFG`) → POST API Create/Update → refresh grid.
6. Xuất khẩu / Nhập khẩu hàng loạt qua thư mục .docx/.doc.

### Điều kiện nghiệp vụ
- `SERE_SERV_TEMP_CODE` và `SERE_SERV_TEMP_NAME` bắt buộc.
- Nếu nhập text trong `memoGenSignatureByKeyCFG` (cấu hình sinh chữ ký) → phải là JSON hợp lệ dạng `List<{LoginnameKey, SignatureKey}>`. Sai → hiện error tại memo, KHÔNG gọi API.
- `EMR_COLUMN_MAPPING` lưu JSON ánh xạ tên cột EMR ↔ key data.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERE_SERV_TEMP | Table | Mẫu xử lý dịch vụ (CRUD chính) |
| V_HIS_SERVICE | View | Danh mục dịch vụ (multi-select) |
| HIS_SERVICE_TYPE | Table | Loại dịch vụ |
| HIS_GENDER | Table | Giới tính áp dụng |
| V_HIS_ROOM | View | Phòng xử lý (lọc `ROOM_TYPE_ID = ID__XL`) |
| EMR_DOCUMENT_TYPE | Table | Loại văn bản EMR |
| EMR_DOCUMENT_GROUP | Table | Nhóm văn bản EMR (chỉ leaf node) |
| EMR_BUSINESS | Table | Nghiệp vụ ký EMR |

### Field nghiệp vụ chính của HIS_SERE_SERV_TEMP
- `DESCRIPTION` (byte[] — RTF), `DESCRIPTION_TEXT`, `CONCLUDE`, `NOTE`
- `SERVICE_IDs` (chuỗi ID phân cách dấu phẩy), `SERVICE_TYPE_ID`, `GENDER_ID`, `ROOM_ID`
- `EMR_DOCUMENT_TYPE_CODE`, `EMR_DOCUMENT_GROUP_CODE`, `EMR_BUSINESS_CODES`, `IS_AUTO_CHOOSE_BUSINESS`
- `EMR_COLUMN_MAPPING` (JSON ánh xạ EMR)
- `GEN_SIGNATURE_BY_KEY_CFG` (JSON ánh xạ key tài khoản → key chữ ký) — NEW

## 4. UI Layout

### Sơ đồ giao diện
```
+---------------------------------------------------------------+
| [Tìm kiếm] [Tìm (Ctrl F)] [Xuất khẩu]                         |
+---------+-----------------------------------------------------+
| Grid    | Mã: [..]  Tên: [..]                                 |
| danh    | Dịch vụ: [..multi..]  Loại DV: [..]                 |
| sách    | Phòng xử lý: [..]  Giới tính: [..]                  |
| (paging)| Mô tả: [..]                                         |
|         | Kết luận: [..]                                      |
|         | Ghi chú: [..]                                       |
|         | Loại VB: [..]  Nhóm VB: [..]                        |
|         | Nghiệp vụ ký: [..] [☐ Tự động]  Ánh xạ EMR: [..][⚙] |
|         | ╔═ Cấu hình sinh chữ ký dựa vào key tài khoản ═══╗  |
|         | ║ [memoGenSignatureByKeyCFG (read-only)]      [⚙] ║  |
|         | ╚════════════════════════════════════════════════╝  |
|         | [RichEdit Ribbon]                                   |
|         | [RichEdit txtDescription]                           |
|         | [Danh sách key] [Nhập khẩu] [Lưu] [Mới]            |
+---------+-----------------------------------------------------+
```

### UC sử dụng
| UC | Vai trò | Ghi chú |
|----|---------|---------|
| Inventec.UC.Paging | panelPaging | Phân trang grid danh sách |

(Form không dùng HIS.UC; dùng các DevExpress GridLookUpEdit + RichEdit + popupContainerControl.)

### Popup "Cấu hình ánh xạ"
GridControl 3 cột (LoginnameKey, SignatureKey, action) trong `popupContainerControlGenSignature`:
- Cột 1: `LoginnameKey` (text — edit được)
- Cột 2: `SignatureKey` (text — edit được)
- Cột 3: action — nút `+` (dòng đầu, repAddGenSignature) / `×` (các dòng còn lại, repDeleteGenSignature)
- Button "Đồng ý" (góc dưới phải) → serialize `List<GenSignatureByKeyADO>` → set vào `memoGenSignatureByKeyCFG.Text` → đóng popup. Khi popup `CloseUp` (click ngoài) cũng tự lưu.

## 5. API Endpoints

| Action | URI | Consumer | Filter / Body |
|--------|-----|----------|---------------|
| Lấy danh sách (paging) | api/HisSereServTemp/GetDynamic | MosConsumer | HisSereServTempFilter (KEY_WORD, ColumnParams) |
| Lấy chi tiết (full DESCRIPTION) | api/HisSereServTemp/Get | MosConsumer | HisSereServTempFilter (ID) |
| Lấy danh sách export | api/HisSereServTemp/GetDynamic | MosConsumer | HisSereServTempFilter (IDs) |
| Tạo mới | HisRequestUriStore.HIS_SERE_SERV_TEMP_CREATE | MosConsumer | HIS_SERE_SERV_TEMP DTO |
| Cập nhật | HisRequestUriStore.HIS_SERE_SERV_TEMP_UPDATE | MosConsumer | HIS_SERE_SERV_TEMP DTO |
| Xóa | HisRequestUriStore.HIS_SERE_SERV_TEMP_DELETE | MosConsumer | long ID |
| Loại dịch vụ | HisRequestUriStore.HIS_SERVICE_TYPE_GET | MosConsumer | HisServiceTypeFilter |
| Giới tính | HisRequestUriStore.HIS_GENDER_GET | MosConsumer | HisGenderFilter |
| Nhóm văn bản EMR | api/EmrDocumentGroup/Get | EmrConsumer | EmrDocumentGroupFilter |
| Phòng xử lý | api/HisRoom/GetView | MosConsumer | HisRoomViewFilter (ROOM_TYPE_ID = ID__XL) |

## 6. Dependencies

### BackendDataWorker (cache)
- `V_HIS_SERVICE` — danh mục dịch vụ (lọc theo `SERVICE_TYPE_IDs` đầu vào).
- `EMR_BUSINESS` — nghiệp vụ ký.
- `EMR_DOCUMENT_TYPE` — loại văn bản EMR.

### Inter-Plugin
- Plugin có thể được mở từ plugin khác qua `PluginInstance` với args:
  - `Inventec.Desktop.Common.Modules.Module moduleData` (BẮT BUỘC)
  - `long service_id` (lọc/chọn sẵn dịch vụ)
  - `List<long> serviceTypeIds` (giới hạn loại dịch vụ trong combobox)

### Form phụ
- `TemplateKey.PreviewTemplateKey` — popup hiển thị danh sách key template (mở qua nút "Danh sách key").

## 7. Print

Plugin không có chức năng in trực tiếp. Mẫu RichEdit được sử dụng bởi các plugin/MPS khác để render.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-07 | tuanln | Thêm khu vực **"Cấu hình sinh chữ ký dựa vào key tài khoản"** trên form chỉnh sửa mẫu: <br>• MemoEdit chỉ-đọc `memoGenSignatureByKeyCFG` + Button `btnPopupGenSignatureByKeyCFG` (caption "Cấu hình ánh xạ"). <br>• Popup `popupContainerControlGenSignature` chứa GridControl 3 cột (LoginnameKey, SignatureKey, action +/×) + nút "Lưu". <br>• ADO mới `GenSignatureByKeyADO`. <br>• Sửa `SetDataRow` để parse `HIS_SERE_SERV_TEMP.GEN_SIGNATURE_BY_KEY_CFG` → fill memo + grid khi load mẫu (Edit). <br>• Sửa `SetData` gán field gửi backend khi Create/Update. <br>• Sửa `btnSave_Click` validate JSON: nếu memo khác rỗng → try-parse `List<GenSignatureByKeyADO>` — sai thì hiện validation tại memo, KHÔNG gọi API. <br>• Thêm Resources Lang (caption group/cột grid/button) + Message (`CauHinhSinhChuKyKhongHopLeKiemTraJSON`). |

## 9. Test Cases

### Tạo mới
- [ ] Nhập đầy đủ mã + tên + nội dung → Lưu thành công.
- [ ] Thiếu mã hoặc tên → validation hiện Required.
- [ ] Cấu hình sinh chữ ký để trống → Lưu OK (field gửi NULL).
- [ ] Mở popup → thêm dòng (Add) → nhập LoginnameKey/SignatureKey → Lưu popup → memo hiện JSON.
- [ ] Lưu popup khi tất cả dòng trống → memo về null, grid reset 1 dòng trống.
- [ ] Xoá dòng (Delete) trong popup → còn 0 dòng → tự thêm 1 dòng trống.

### Sửa
- [ ] Chọn dòng đã có `GEN_SIGNATURE_BY_KEY_CFG` → memo hiện JSON, mở popup → grid hiển thị đúng số dòng.
- [ ] Chọn dòng `GEN_SIGNATURE_BY_KEY_CFG = null` → memo trống, mở popup → grid 1 dòng trống.

### Validate
- [ ] Memo có chuỗi không phải JSON (ví dụ `abc`) → bấm Lưu mẫu → hiện thông báo `CauHinhSinhChuKyKhongHopLeKiemTraJSON` tại memo, focus về memo, KHÔNG gọi API.
- [ ] Memo là JSON khác cấu trúc List → vẫn coi là sai → hiện validation.
- [ ] Memo là JSON đúng `[{"LoginnameKey":"x","SignatureKey":"y"}]` → Lưu thành công.

### Xoá
- [ ] Confirm dialog → Xoá thành công → Grid refresh.

### Đa ngôn ngữ
- [ ] Đổi sang English → caption group "Signature generation mapping by account key" + button tooltip + cột grid hiển thị đúng tiếng Anh.
