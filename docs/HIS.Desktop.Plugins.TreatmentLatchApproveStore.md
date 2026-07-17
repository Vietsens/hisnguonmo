# Chốt Duyệt Hồ Sơ Bệnh Án — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentLatchApproveStore |
| Loại | Form (FormBase) |
| Mục đích | Màn "Chốt duyệt hồ sơ bệnh án" — duyệt đạt điều kiện lưu (ApprovalStore) / hủy chốt cho các hồ sơ đã kết thúc điều trị. Từ đây mở màn Tra soát (HisTreatmentRecordChecking). |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Khoa/phòng vào màn sau khi kết thúc điều trị → lọc danh sách hồ sơ (khoa kết thúc, ngày ra viện, đối tượng, diện điều trị, trạng thái).
2. Chọn hồ sơ → **Duyệt đạt điều kiện** (toolbar Ctrl+S hoặc nút chốt từng dòng) → API ApprovalStore, set `APPROVAL_STORE_STT_ID = 1`.
3. **Hủy chốt** hồ sơ đã chốt → API UnapprovalStore.
4. Nút "con mắt" mỗi dòng → mở màn **Tra soát** (bảng kiểm quy chế hồ sơ bệnh án).

### PTTK 42984 v2.2 — Tự động Duyệt BHYT khi Đạt (luồng 2 bước ↔ 1 bước)
Config toàn viện `MOS.HIS_TREATMENT.IS_AUTO_APPROVE_HEIN_ON_STORE`:

- **= Có (BẬT — luồng 1 bước):** khi Đạt, hệ thống tự động Duyệt BHYT (tạo bản ghi giám định + khóa `IS_LOCK_HEIN=1`).
  - Hiện combobox **"Phòng thu ngân"** (bắt buộc) — giá trị chọn làm `CASHIER_ROOM_ID` cho bản ghi giám định tự tạo.
  - Chưa chọn phòng → cảnh báo, chặn Đạt.
  - Ẩn cột "Đã duyệt BHYT" (vì Đạt đã tự khóa).
- **= Không (TẮT — luồng 2 bước, như cũ):** Đạt chỉ chốt lưu; việc Duyệt/khóa BHYT làm thủ công ở màn "Duyệt hồ sơ BHYT".
  - Hiện cột **"Đã duyệt BHYT"** (icon từ `IS_LOCK_HEIN`).
  - Ẩn combobox "Phòng thu ngân".
- Badge status thể hiện trạng thái config ở thanh lọc.
- **Chặn Hủy chốt** hồ sơ đã Duyệt BHYT (đã khóa `IS_LOCK_HEIN=1`): backend chặn (R7) và yêu cầu Mở khóa BHYT trước; FE hiển thị thông báo lỗi từ backend qua `MessageManager`.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| L_HIS_TREATMENT_3 | View (GetLView3) | Dòng hồ sơ trên grid. **PTTK 42984: chờ BE bổ sung field `IS_LOCK_HEIN`** để render cột "Đã duyệt BHYT" (FE đang đọc qua reflection). |
| V_HIS_CASHIER_ROOM | View | Danh mục phòng thu ngân cho combobox (auto-duyệt=Có). |
| V_HIS_ROOM / HIS_DEPARTMENT | View/Table | Phòng/khoa lọc. |
| HIS_TREATMENT | Table | Kết quả trả về sau ApprovalStore/UnapprovalStore. |

## 4. UI Layout

```
+--------------------------------------------------------------------------------+
| [Khoa kết thúc] [Khoa] [Ngày ra viện từ-đến] [Đối tượng] [Diện điều trị]        |
| [Mã ĐT F2] [Mã BN F3] [Từ khóa] [Trạng thái] [Tìm Ctrl F] [Duyệt ĐĐK Ctrl S]   |
| [Phòng thu ngân ▼] (auto=Có)          [Badge: Tự động Duyệt BHYT khi Đạt: ...]  |
+--------------------------------------------------------------------------------+
| Grid: STT | 👁 | Chốt | TT | Mã ĐT | Mã BN | ... | (Đã duyệt BHYT khi auto=Không)|
+--------------------------------------------------------------------------------+
| ucPaging                                                                        |
+--------------------------------------------------------------------------------+
```

Control mới (PTTK 42984): `cboCashierRoom` (LookUpEdit), `lblAutoApproveBadge` (LabelControl), cột grid `gcIsLockHein` ("Đã duyệt BHYT").

## 5. API Endpoints

| Action | URI | Consumer | Body |
|--------|-----|----------|------|
| Lấy danh sách | HisRequestUriStore.HIS_TREATMENT_GETVIEW (`/api/HisTreatment/GetLView3`) | MosConsumer | HisTreatmentLView3Filter |
| Duyệt đạt điều kiện | HisRequestUriStore.HIS_TREATMENT_APPROVALSTORE (`/api/HisTreatment/ApprovalStore`) | MosConsumer | `List<long>` (mặc định); **auto=Có + có phòng → `MOS.SDO.HisTreatmentApprovalStoreSDO {TreatmentIds, CashierRoomId}`** (additive, backward-compatible) |
| Hủy chốt | HisRequestUriStore.HIS_TREATMENT_UNAPPROVALSTORE (`/api/HisTreatment/UnapprovalStore`) | MosConsumer | `List<long>` (BE chặn nếu đã khóa BHYT — R7) |
| Danh mục phòng thu ngân | HisRequestUriStore.HIS_CASHIER_ROOM_GETVIEW (`api/HisCashierRoom/GetView`) | MosConsumer | HisCashierRoomFilter |

> **Phụ thuộc BE:**
> 1. ✅ SDO `MOS.SDO.HisTreatmentApprovalStoreSDO {TreatmentIds, CashierRoomId}` — BE đã cung cấp trong MOS.SDO.dll (FE đã dùng class thật).
> 2. ⏳ Field `IS_LOCK_HEIN` trên `L_HIS_TREATMENT_3` (GetLView3) — FE đọc qua reflection, tự có giá trị khi BE bổ sung.

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.HisTreatmentRecordChecking | Nút "con mắt" mỗi dòng → màn Tra soát | `long treatmentId`, `Module` (giữ nguyên như cũ — màn Tra soát do nhóm khác xử lý riêng) |
| HIS.Desktop.Plugins.MRSummaryList | Nút bảng kiểm (cột EDIT) | MRSummaryDetailADO, treatmentId, row, Module |

## 7. Print
Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 10/07/2026 | huannh | PTTK 42984 v2.2: thêm combobox "Phòng thu ngân" + cột "Đã duyệt BHYT" ẩn/hiện theo config `IS_AUTO_APPROVE_HEIN_ON_STORE`; validate + truyền `CashierRoomId` vào ApprovalStore qua `MOS.SDO.HisTreatmentApprovalStoreSDO`; badge trạng thái; truyền context sang màn Tra soát. |

## 9. Test Cases

### Config auto-duyệt = Có
- [ ] Hiện combobox "Phòng thu ngân"; ẩn cột "Đã duyệt BHYT"; badge "BẬT".
- [ ] Chưa chọn phòng → bấm Đạt (toolbar/nút dòng) → cảnh báo, chặn, focus combo.
- [ ] Đã chọn phòng → Đạt → POST kèm CashierRoomId → thành công.

### Config auto-duyệt = Không
- [ ] Ẩn combobox; hiện cột "Đã duyệt BHYT" (Đã duyệt / Chưa duyệt / trống); badge "TẮT".
- [ ] Đạt → POST `List<long>` như cũ, không yêu cầu phòng.

### Chung
- [ ] Hủy chốt hồ sơ đã khóa BHYT → hiển thị thông báo lỗi từ backend (R7).
- [ ] Đa ngôn ngữ vi/en cho caption combo, cột, badge, thông báo.
