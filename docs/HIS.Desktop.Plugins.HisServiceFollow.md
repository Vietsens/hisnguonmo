# Dịch Vụ Đi Kèm (HisServiceFollow) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisServiceFollow |
| Loại | Form (`frmHisServiceFollow` kế thừa `FormBase`) |
| Mục đích | Quản lý danh mục "Dịch vụ đi kèm": cấu hình 1 dịch vụ chính sẽ tự động kéo theo 1 dịch vụ đi kèm khi chỉ định (số lượng, hao phí, diện điều trị, điều kiện áp dụng). |
| Nhóm menu | Danh mục / Bussiness |
| Người tạo | INVENTEC |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
- Người dùng chọn **Loại dịch vụ** → **Dịch vụ** (dịch vụ chính).
- Chọn **Loại DV đi kèm** → **DV đi kèm** (dịch vụ sẽ được tự động chỉ định kèm).
- Nhập **SL dịch vụ đi kèm**, **Số lượng dịch vụ** (điều kiện áp dụng theo SL của DV chính), **Diện điều trị**.
- Tùy chọn các cờ: **Hao phí**, **Chỉ đính kèm nếu trong cùng lượt chỉ định chưa có dịch vụ này**, **Kiểm tra dịch vụ đi kèm khi xuất viện/chuyển khoa**, **Không tự động chỉ định**.
- Lưu (Thêm/Sửa) → ghi xuống `HIS_SERVICE_FOLLOW`. Khóa/Mở khóa và Xóa mềm trên grid.

### Điều kiện nghiệp vụ
- **Chỉ đính kèm** chỉ bật khi DV chính thuộc loại Máu (`HIS_SERVICE_TYPE.ID__MAU`).
- **Kiểm tra khi xuất viện/chuyển khoa** chỉ bật cho các loại PT/TT/Giường/Nội soi/Siêu âm/TDCN/CĐHA.
- **Không tự động chỉ định**: khi bật, dịch vụ đi kèm KHÔNG được phần mềm tự động chỉ định cùng lượt với dịch vụ chính (chỉ lưu cấu hình quan hệ đi kèm).
- Soft delete qua khóa (`IS_ACTIVE`), không xóa vật lý.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERVICE_FOLLOW | View | Hiển thị danh sách + load chi tiết bản ghi cấu hình đi kèm |
| HIS_SERVICE_FOLLOW | Table | Bản ghi gốc (reset cache sau khi lưu) |
| V_HIS_SERVICE | View | Nguồn combo Dịch vụ / DV đi kèm |
| HIS_SERVICE_TYPE | Table | Nguồn combo Loại dịch vụ |
| HIS_TREATMENT_TYPE | Table | Nguồn multi-select Diện điều trị (`TREATMENT_TYPE_IDS`) |

### Trường chính trên V_HIS_SERVICE_FOLLOW
`SERVICE_TYPE_ID`, `SERVICE_ID`, `FOLLOW_TYPE_ID`, `FOLLOW_ID`, `AMOUNT`, `CONDITIONED_AMOUNT`, `TREATMENT_TYPE_IDS`, `IS_EXPEND`, `ADD_IF_NOT_ASSIGNED`, `CHECK_FOLLOW_WHEN_OUT`, **`IS_NOT_AUTO_ASSIGN`** (short?, 1 = không tự động chỉ định / null = mặc định), `IS_ACTIVE`.

## 4. UI Layout

```
+--- Dịch vụ đi kèm -------------------------------------------------+
| [Từ khóa tìm kiếm] [Tìm (Ctrl F)] |  Loại dịch vụ:  [cboLoaiDV]    |
| Grid danh sách:                   |  Dịch vụ:       [gridLookUp1]  |
|  STT | X | 🔒 | Tên loại DV | ...  |  Số lượng dịch vụ:[spnAmount..]|
|  ... | Hao phí | Kiểm tra |        |  Loại DV đi kèm:[cboLoaiKT]    |
|  Không tự động chỉ định | Người tạo|  DV đi kèm:     [gridLookUp2]  |
|  | Thời gian tạo | Người sửa | ... |  SL dịch vụ đi kèm:[spAmount]  |
|                                    |  Diện điều trị: [cboTreatType] |
|                                    |  [ ] Hao phí                  |
|                                    |  [ ] Chỉ đính kèm...          |
|                                    |  [ ] Kiểm tra DV đi kèm...    |
|                                    |  [ ] Không tự động chỉ định   |
|                                    |  [Sửa] [Thêm] [Làm lại]       |
+-------------------------------------------------------------------+
```

### Cột grid liên quan checkbox
| FieldName (unbound) | Caption | Repository |
|---------------------|---------|------------|
| `IS_EXPEND_STR` | Hao phí | `CheckGr` |
| `CHECK_FOLLOW_WHEN_OUT_STR` | Kiểm tra | `CheckFollow` |
| `IS_NOT_AUTO_ASSIGN_STR` | Không tự động chỉ định | `CheckNotAutoAssign` |

UC sử dụng: `Inventec.UC.Paging` (phân trang).

## 5. API Endpoints

| Action | URI (HisRequestUriStore) | Consumer | Filter |
|--------|--------------------------|----------|--------|
| Lấy danh sách (paging) | `MOSV_HIS_SERVICE_FOLLOW_GET` | MosConsumer | `HisServiceFollowViewFilter` |
| Tạo mới | `MOSV_HIS_SERVICE_FOLLOW_CREATE` | MosConsumer | DTO `V_HIS_SERVICE_FOLLOW` |
| Cập nhật | `MOSV_HIS_SERVICE_FOLLOW_UPDATE` | MosConsumer | DTO `V_HIS_SERVICE_FOLLOW` |
| Xóa | `MOSV_HIS_SERVICE_FOLLOW_DELETE` | MosConsumer | ID |
| Khóa/Mở khóa | `api/HisServiceFollow/ChangeLock` | MosConsumer | ID |

## 6. Dependencies

Không phụ thuộc Library/Inter-Plugin. Dữ liệu danh mục lấy từ `BackendDataWorker` (cache RAM).

## 7. Print

Không có chức năng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 19/06/2026 | huannh | Bổ sung checkbox "Không tự động chỉ định" trên form và cột tương ứng trên grid; lưu/hiển thị từ trường `IS_NOT_AUTO_ASSIGN` (V_HIS_SERVICE_FOLLOW / HIS_SERVICE_FOLLOW). |

## 9. Test Cases

### Tạo mới / Sửa
- [ ] Tích "Không tự động chỉ định" → Lưu → `IS_NOT_AUTO_ASSIGN = 1` ghi xuống DB.
- [ ] Bỏ tích → Lưu → `IS_NOT_AUTO_ASSIGN = null`.
- [ ] Chọn lại dòng → checkbox "Không tự động chỉ định" hiển thị đúng trạng thái đã lưu.

### Danh sách
- [ ] Cột "Không tự động chỉ định" hiển thị đúng tick cho bản ghi có `IS_NOT_AUTO_ASSIGN = 1`.
- [ ] Các cột audit (Người tạo, Thời gian tạo, Người sửa, Thời gian sửa) vẫn đứng cuối, đúng thứ tự.

### Nghiệp vụ
- [ ] DV đi kèm có cờ "Không tự động chỉ định" → khi chỉ định DV chính, hệ thống KHÔNG tự thêm DV đi kèm này.
