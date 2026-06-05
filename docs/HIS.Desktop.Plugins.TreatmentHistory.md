# Lịch Sử Điều Trị — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TreatmentHistory |
| Loại | Form (FormBase) |
| Mục đích | Tra cứu lịch sử điều trị của bệnh nhân: danh sách đợt (Grid 1), cây yêu cầu dịch vụ theo khoa/ngày (Grid 2), chi tiết dịch vụ + kết quả (Grid 3). Hỗ trợ chế độ "Gộp kết quả KCB theo nhóm dịch vụ" (gated config). |
| Người tạo | (legacy) |
| Cập nhật | tuanln — 05/06/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (chế độ thường)
1. Mở form (từ menu hoặc inter-plugin kèm `TreatmentHistoryADO` có `treatment_code`/`patient_code`).
2. Grid 1 nạp danh sách đợt điều trị (`L_HIS_TREATMENT`) phân trang, lọc theo mã ĐT/mã BN/từ khóa/trạng thái.
3. Click 1 đợt ở Grid 1 → `LoadDataTreeServiceReq2`: dựng Grid 2 cấu trúc **Khoa điều trị → Ngày y lệnh** (từ `V_HIS_DEPARTMENT_TRAN` + `HIS_SERVICE_REQ` theo `TREATMENT_ID`).
4. Click node lá (Ngày) ở Grid 2 → nạp Grid 3:
   - `chkShowTabDetail` TẮT → cây đơn giản `TreeSereServ7` (`LoadDataSereServ7`).
   - `chkShowTabDetail` BẬT → tab chi tiết `UCPanelControlTreeSere7` (`LoadDataSereServByTreatmentId` + `LoadExamInfo`).

### Luồng "Gộp kết quả KCB theo nhóm dịch vụ" (config BẬT)
1. Đọc config `HIS.TREATMENT_HISTORY.MERGE_BY_SERVICE_TYPE`. Nếu khác "1" → ẩn toàn bộ, chạy y luồng thường.
2. Nếu = "1" → hiện CheckEdit **"Gộp kết quả KCB"** + dropdown **"Đợt điều trị cần gộp"** trên thanh lọc.
3. Tích "Gộp kết quả KCB" (phải đang chọn 1 đợt ở Grid 1, nếu không → cảnh báo, tự bỏ tích):
   - Mở popup TreeList 2 cấp **Năm → Đợt** (multi-select, tri-state), tự tích nhánh năm hệ thống hiện tại.
   - Người dùng "Áp dụng" → gọi `HIS_SERVICE_REQ` theo `TREATMENT_IDs` các đợt đã chọn → dựng Grid 2 thành **2 root song song**:
     - **Root A — Theo diện điều trị (4 cấp)**: Diện điều trị → Khoa thực hiện → Loại y lệnh → Ngày y lệnh (mốc `INTRUCTION_TIME`).
     - **Root B — Tổng hợp theo loại y lệnh (2 cấp)**: Loại y lệnh → Ngày y lệnh (gom xuyên Diện + Khoa).
   - Click node lá → nạp Grid 3 (luôn dùng cây đơn giản `TreeSereServ7`; ô tab chi tiết bị vô hiệu khi gộp).
4. Bỏ tích → khôi phục cấu trúc cây cũ + Grid 3 theo đợt đang chọn.

### Điều kiện nghiệp vụ
- Chế độ gộp chỉ hiển thị khi config = "1" (mặc định/khác "1" coi như TẮT — màn hình y nguyên hiện tại).
- "(Không xác định)" áp dụng khi `TREATMENT_TYPE_ID` null hoặc `EXECUTE_DEPARTMENT_ID` = 0.
- Node lá gom theo `INTRUCTION_TIME` (mốc giờ), cột SL: Diện/Khoa = số y lệnh; Loại = số mốc giờ; lá = số y lệnh tại mốc.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| L_HIS_TREATMENT | View | Danh sách đợt điều trị (Grid 1) |
| V_HIS_TREATMENT | View | Danh sách đợt theo BN cho popup gộp (có `LAST_DEPARTMENT_ID`, `TREATMENT_END_TYPE_NAME`) |
| V_HIS_DEPARTMENT_TRAN | View | Chuỗi chuyển khoa (dựng cây chế độ thường) |
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ — nguồn gom 2 root (`TREATMENT_TYPE_ID`, `EXECUTE_DEPARTMENT_ID`, `SERVICE_REQ_TYPE_ID`, `INTRUCTION_TIME`, `SERVICE_REQ_CODE`) |
| DHisSereServ2 (MOS.SDO) | SDO | Chi tiết dịch vụ theo (TREATMENT_ID, INTRUCTION_DATE) cho Grid 3 |
| V_HIS_SERE_SERV_7 | View | Bind Grid 3 (TreeSereServ7) |
| HIS_TREATMENT_TYPE / HIS_DEPARTMENT / HIS_SERVICE_REQ_TYPE / HIS_SERVICE_TYPE | Table | Lookup tên cấp (BackendDataWorker) |

## 4. UI Layout

```
+--------------------------------------------------------------------------------------+
| [Mã ĐT][Mã BN][Từ khóa]  [Trạng thái]  [Tìm]  [✓Gộp kết quả KCB][Đợt cần gộp ▼]      |  ← thanh lọc (2 control cuối: config gated)
+--------------------------------------------------------------------------------------+
| Grid 1 — danh sách đợt (L_HIS_TREATMENT)                                              |
+--------------------------------------------------------------------------------------+
| Phân trang (ucPaging1)                                                                |
+----------------------------------+---------------------------------------------------+
| Grid 2 — tree_HisServiceReq2     | Grid 3 — panelControlTreeSere7 (TreeSereServ7 /  |
|  Thường: Khoa → Ngày             |          UCPanelControlTreeSere7)                 |
|  Gộp: 2 root (Diện..→Ngày /      |                                                   |
|       Loại→Ngày)                 |                                                   |
+----------------------------------+---------------------------------------------------+
| [✓Hiển thị tab chi tiết] [Thu gọn] [Lọc khoa ▼]                                       |
+--------------------------------------------------------------------------------------+
```

### Control chế độ gộp (dựng runtime trong `frmTreatmentHistory__Plus__Merge.cs`)
| Control | Loại | Vai trò |
|---------|------|---------|
| chkMergeByServiceType | CheckEdit | Bật/tắt gộp (lưu ControlState) |
| popupPeriod + popupContainerPeriod | PopupContainerEdit + Control | Dropdown "Đợt điều trị cần gộp" |
| treePeriod | TreeList | Năm → Đợt, tri-state checkbox |
| btnSelectAll/Unselect/Apply/Close | SimpleButton | Thao tác popup |
| lblNoData | LabelControl | Báo "Không có dữ liệu trong khoảng thời gian" |

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.TreeSereServ7 | panelControlTreeSere7 | Cây dịch vụ Grid 3 (chế độ đơn giản) |
| UCPanelControlTreeSere7 (nội bộ plugin) | panelControlTreeSere7 | Grid 3 dạng tab chi tiết |
| Inventec.UC.Paging | ucPaging1 | Phân trang Grid 1 |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Danh sách đợt (Grid 1) | api/HisTreatment/GetLView | MosConsumer | HisTreatmentLViewFilter |
| Đợt theo BN (popup gộp) | api/HisTreatment/GetView | MosConsumer | HisTreatmentViewFilter (PATIENT_CODE__EXACT) |
| Chuyển khoa | HisRequestUriStore.HIS_DEPARTMENT_TRAN_GETVIEW | MosConsumer | HisDepartmentTranViewFilter |
| Yêu cầu DV (thường + gộp) | HisRequestUriStore.HIS_SERVICE_REQ_GET | MosConsumer | HisServiceReqFilter (TREATMENT_ID / **TREATMENT_IDs**) |
| Chi tiết DV (Grid 3) | api/HisSereServ/GetDHisSereServ2 | MosConsumer | DHisSereServ2Filter (TREATMENT_ID + INTRUCTION_DATE) |

> Chế độ gộp **không thêm API mới** — tái dùng toàn bộ endpoint hiện có.

## 6. Dependencies

### Library / Cache
| Thành phần | Mục đích |
|-----------|----------|
| HIS.Desktop.Library.CacheClient (ControlStateWorker) | Lưu trạng thái checkbox (chkShowTabDetail, chkMergeByServiceType) |

### Inter-Plugin (mở từ Grid 3 / Grid 1)
| Plugin đích | Khi nào |
|-------------|---------|
| SereServTein / ExamServiceReqResult / ServiceReqResultView | Click icon kết quả theo loại DV |
| EnterKskInfomantion | Nút KSK |
| AggrHospitalFees | Nút viện phí |
| AssignServiceEdit | Sửa yêu cầu |

## 7. Print
Không có chức năng in trong plugin.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 05/06/2026 | tuanln | Thêm chế độ "Gộp kết quả KCB theo nhóm dịch vụ" (config `HIS.TREATMENT_HISTORY.MERGE_BY_SERVICE_TYPE`): CheckEdit + popup chọn đợt (Năm→Đợt tri-state), Grid 2 dựng 2 root song song (Root A 4 cấp, Root B 2 cấp), Grid 3 lazy-load theo node lá. Thêm `frmTreatmentHistory__Plus__Merge.cs`, `ADO/ServiceReqMergeNodeADO.cs`, `ADO/TreatmentPeriodADO.cs`, `HisConfigCFG.IsMergeByServiceTypeEnabled`, đa ngôn ngữ vi/en. Không đụng Designer, không phá luồng khi config TẮT. |

## 9. Test Cases

### Config TẮT (hoặc khác "1")
- [ ] Không hiển thị checkbox "Gộp kết quả KCB" + dropdown đợt; màn hình hoạt động y nguyên (Khoa → Ngày → Grid 3).

### Config BẬT — bật/tắt gộp
- [ ] Chưa chọn đợt ở Grid 1 mà tích gộp → cảnh báo "Chọn đợt điều trị trước" + tự bỏ tích.
- [ ] Chọn 1 đợt rồi tích gộp → mở popup, năm hiện tại tự tích, "Áp dụng" → Grid 2 hiện 2 root.
- [ ] Bỏ tích → khôi phục cây Khoa → Ngày của đợt đang chọn; ô tab chi tiết hoạt động lại.
- [ ] Đóng/mở lại form (config BẬT, từng tích) → checkbox khôi phục trạng thái đã lưu.

### Cây gộp
- [ ] Root A đủ 4 cấp Diện → Khoa thực hiện → Loại y lệnh → Ngày y lệnh; Root B 2 cấp Loại → Ngày.
- [ ] Y lệnh thiếu diện/khoa → gom dưới node "(Không xác định)".
- [ ] Cột SL: Diện/Khoa = số y lệnh, Loại = số mốc giờ, lá = số y lệnh.
- [ ] 2 root đều rỗng → hiện "Không có dữ liệu trong khoảng thời gian".

### Click lá → Grid 3
- [ ] Click lá (chuột + bàn phím) → Grid 3 nạp đúng dịch vụ của mốc y lệnh đó.
- [ ] Click node cha → Grid 3 trống.

### Popup chọn đợt
- [ ] Tri-state: bỏ tích 1 đợt → năm chuyển indeterminate; tích đủ → năm Checked.
- [ ] "Chọn tất cả"/"Bỏ chọn tất cả" hoạt động; "Đóng" không dựng lại, "Áp dụng" dựng lại.
- [ ] Đổi BN khác ở Grid 1 (đang gộp) → popup nạp lại theo BN mới + dựng lại cây.
