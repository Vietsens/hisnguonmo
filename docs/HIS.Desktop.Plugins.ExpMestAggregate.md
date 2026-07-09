# Tổng Hợp Phiếu Lĩnh — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExpMestAggregate |
| Loại | UC (UserControlBase) |
| Mục đích | Điều dưỡng/dược tổng hợp nhiều phiếu xuất (đơn thuốc nội trú) thành 1 phiếu lĩnh tổng, gửi đi lĩnh thuốc/vật tư; in phiếu lĩnh tổng hợp (Mps49). |
| Vị trí | Phòng khám - Kế hoạch tổng hợp / Dược |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Lọc danh sách phiếu xuất chưa tổng hợp theo: Kho xuất, Đối tượng bệnh nhân, **Ca chạy thận**, Thời gian chỉ định, Thời gian dự trù, Trạng thái, Buồng, Loại đơn, Giường, Khu vực.
2. Tick chọn phiếu xuất → bấm **Tổng hợp (Ctrl T)** → gọi `api/HisExpMest/AggrCreate` tạo phiếu lĩnh tổng.
3. Chọn phiếu lĩnh tổng → in **Mps49** (phiếu lĩnh tổng hợp) qua plugin `HIS.Desktop.Plugins.AggrExpMestPrintFilter`.

### Nghiệp vụ chạy thận (Tài liệu 2213)
- **Filter Ca chạy thận**: combo `cboKidneyShift` (Ca 1..5) → map vào `HisExpMestViewFilter.KIDNEY_SHIFT`. Lọc các phiếu dự trù thuốc chạy thận theo ca do ĐD tạo.
- **R16**: Đơn chạy thận BS (ngoài kho — nhóm A/B ở kê đơn) KHÔNG sinh phiếu xuất nên KHÔNG lên danh sách tổng hợp.
- **R17**: Phiếu dự trù (do ĐD tạo ở màn "Dự trù thuốc chạy thận") ở trạng thái Yêu cầu, `HAS_AGGR = false` → hiển thị ở tab "Chưa tổng hợp".
- **Key Ca cho biểu in Mps49**: header phiếu lĩnh tổng hiển thị Ca — lấy từ key `KIDNEY_SHIFT` do processor Mps000049 sinh (xem MPS000049).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST | View | Phiếu xuất / phiếu lĩnh tổng (có cột KIDNEY_SHIFT — bổ sung 2213) |
| V_HIS_EXP_MEST_MEDICINE | View | Chi tiết thuốc phiếu xuất |
| V_HIS_BED / V_HIS_MEDI_STOCK / HIS_AREA / HIS_PATIENT_TYPE | View/Table | Dữ liệu combo lọc |

## 4. UI Layout

```
+-- navBarFilterProcess (trái) --------+  +-- Grid phiếu xuất (giữa) ----------+
| Kho xuất                             |  | STT | Mã PX | Kho | BN | ...      |
| Đối tượng bệnh nhân                  |  +-----------------------------------+
| Ca chạy thận      <== MỚI (2213)     |  +-- Chi tiết thuốc/VT (phải) -------+
| Thời gian chỉ định                   |  +-----------------------------------+
| Thời gian dự trù                     |  +-- Grid phiếu lĩnh tổng (dưới) ----+
| Trạng thái / Buồng / Loại đơn        |  +-----------------------------------+
| Giường / Khu vực                     |
+--------------------------------------+
Toolbar: [Tổng hợp (Ctrl T)] [In tra đổi tổng hợp]
```

- Combo **Ca chạy thận**: `cboKidneyShift` (GridLookUpEdit) — nhóm NavBar `navBarGroupKidneyShift`, đặt giữa "Đối tượng bệnh nhân" và "Thời gian chỉ định".

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy phiếu xuất/phiếu lĩnh | HisRequestUriStore.HIS_EXP_MEST_GETVIEW | MosConsumer | HisExpMestViewFilter (có KIDNEY_SHIFT) |
| Tạo phiếu lĩnh tổng | api/HisExpMest/AggrCreate | MosConsumer | HisExpMestAggrSDO |
| Chi tiết thuốc | api/HisExpMestMedicine/getView | MosConsumer | HisExpMestMedicineViewFilter |

## 6. Dependencies

| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AggrExpMestPrintFilter | In phiếu lĩnh tổng / tra đổi | currentAggrExpMest, printType, Module |
| HIS.Desktop.Plugins.Library.PrintAggrExpMest | In phiếu hủy thuốc/VT (Mps000434) | List<V_HIS_EXP_MEST> |

## 7. Print

| Loại in | PrintTypeCode | Nơi thực hiện |
|---------|--------------|---------------|
| Phiếu lĩnh tổng hợp | Mps000049 | AggrExpMestPrintFilter (header có Ca chạy thận) |
| Phiếu tra đổi thuốc | Mps000047 | UCExpMestAggregate__Pluss__Print |
| Phiếu hủy thuốc/VT | Mps000434 | UCExpMestAggregate__Pluss__Print |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 01/07/2026 | phuongnm | 2213: Thêm filter "Ca chạy thận" (cboKidneyShift → HisExpMestViewFilter.KIDNEY_SHIFT). Thêm ADO KidneyShiftADO, đa ngôn ngữ, nhóm NavBar navBarGroupKidneyShift. |

### Phụ thuộc backend (2213 — DEV backend bổ sung)
- `HisExpMestViewFilter.KIDNEY_SHIFT` (MOS.Filter) — filter theo ca (mục 3.1.3).
- `V_HIS_EXP_MEST.KIDNEY_SHIFT` (MOS.EFMODEL + view) — mục 2.7.
- `HIS_EXP_MEST.KIDNEY_SHIFT` (MOS.EFMODEL + bảng) — mục 2.2.

## 9. Test Cases

- [ ] Chọn Ca 1 → Tìm → chỉ hiển thị phiếu dự trù ca 1.
- [ ] Xóa Ca (nút Delete) → Tìm → hiển thị tất cả ca.
- [ ] Làm lại (Ctrl R) → combo Ca reset về rỗng.
- [ ] Tổng hợp phiếu ca 2 → in Mps49 → header hiển thị "Ca 2".
- [ ] Đơn chạy thận BS ngoài kho KHÔNG lên danh sách (R16).
- [ ] Phiếu dự trù ĐD hiển thị ở tab "Chưa tổng hợp" (R17).
