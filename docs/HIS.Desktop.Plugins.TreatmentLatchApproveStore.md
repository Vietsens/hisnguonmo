# Chốt Duyệt Hồ Sơ Bệnh Án — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentLatchApproveStore |
| Loại | Form (FormBase) |
| Mục đích | Màn "Chốt duyệt hồ sơ bệnh án" — chốt/hủy chốt (ApprovalStore/UnapprovalStore) + Duyệt/Hủy Duyệt hồ sơ theo quyền & cấu hình. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Cột "Chốt / Hủy chốt" (có sẵn — APPROVAL_STORE_STT_ID_STR)
- `APPROVAL_STORE_STT_ID == 1` → nút Hủy chốt → `UnapprovalStore`.
- khác 1 → nút Chốt → `ApprovalStore`.

### Cột MỚI "Duyệt / Hủy duyệt" (gcDuyet — theo quyền + config)
Config toàn viện `MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE`; quyền `HIS000054` (Duyệt), `HIS000055` (Hủy Duyệt).

| Dòng có `APPROVAL_STORE_STT_ID` | Nút hiển thị | Enable khi |
|---|---|---|
| = 3 | **Duyệt** | `HIS000054` VÀ config `!= 1` |
| != null và != 3 | **Hủy Duyệt** | (config `!= 1` VÀ `HIS000055`) HOẶC config `= 1` |
| = null | nút mờ (disable) | — |

- **Ẩn/hiện cột:** hiện khi user dùng được ít nhất 1 nút: `(HIS000054 && config!=1)` HOẶC `((config!=1 && HIS000055) || config==1)`.
- **Click Duyệt/Hủy Duyệt:** cả 2 gọi `api/HisTreatment/ApprovalStore` (theo yêu cầu 51062) → refresh grid.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| L_HIS_TREATMENT_3 | View (GetLView3) | Dòng hồ sơ trên grid. `APPROVAL_STORE_STT_ID` = trạng thái (null/1/2/3). |
| ACS.SDO.AcsAuthorizeSDO | SDO | Kiểm tra quyền (`ControlInRoles`, `IsFull`). |
| HIS_TREATMENT | Table | Kết quả ApprovalStore/UnapprovalStore. |

## 4. UI Layout
Grid có cột nút: "Xem chi tiết", "Chốt/Hủy chốt", **"Duyệt/Hủy duyệt" (mới, ẩn/hiện theo quyền+config)**, "Trạng thái", cột bảng kiểm (EDIT)... Mỗi dòng hiển thị đúng 1 nút theo trạng thái (pattern CustomRowCellEdit).

## 5. API Endpoints

| Action | URI | Consumer | Body |
|--------|-----|----------|------|
| Lấy danh sách | HisRequestUriStore.HIS_TREATMENT_GETVIEW (`/api/HisTreatment/GetLView3`) | MosConsumer | HisTreatmentLView3Filter |
| Chốt / Duyệt / Hủy Duyệt | HisRequestUriStore.HIS_TREATMENT_APPROVALSTORE (`/api/HisTreatment/ApprovalStore`) | MosConsumer | `List<long>` |
| Hủy chốt | HisRequestUriStore.HIS_TREATMENT_UNAPPROVALSTORE (`/api/HisTreatment/UnapprovalStore`) | MosConsumer | `List<long>` |

## 6. Phân Quyền / Config
| Mã | Ý nghĩa |
|----|---------|
| Config `MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE` | `= 1`: tự động; `!= 1`: duyệt thủ công qua nút Duyệt |
| Control `HIS000054` | Quyền Duyệt |
| Control `HIS000055` | Quyền Hủy Duyệt |

Cơ chế check quyền: `GlobalVariables.AcsAuthorizeSDO.IsFull || ControlInRoles.Any(o => o.CONTROL_CODE == "...")`.

## 7. Print
Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 15/07/2026 | huannh | Việc 51062: thêm cột "Duyệt/Hủy duyệt" trên grid theo quyền HIS000054/HIS000055 + config `IS_AUTO_APPROVAL_STORE`; enable theo trạng thái (Duyệt=3, Hủy Duyệt != null && != 3); cả 2 nút gọi ApprovalStore. |

## 9. Test Cases
- [ ] config != 1 + có HIS000054: dòng STT=3 → nút Duyệt enable → bấm → ApprovalStore → refresh.
- [ ] config != 1 + có HIS000055: dòng STT != null && != 3 → nút Hủy Duyệt enable → bấm → ApprovalStore.
- [ ] config = 1: nút Hủy Duyệt enable (không cần quyền) với STT != null && != 3.
- [ ] Không đủ quyền/không thuộc điều kiện → nút mờ, bấm không tác dụng.
- [ ] Không quyền cả 2 và config != 1 → ẩn cột.
- [ ] Cột "Chốt/Hủy chốt" cũ giữ nguyên: STT=1 → UnapprovalStore, khác → ApprovalStore.
