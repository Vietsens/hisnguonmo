# Danh sách duyệt khám chuyên khoa — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExamSpecialist |
| Loại | Form |
| Mục đích | Hiển thị danh sách yêu cầu khám chuyên khoa, cho phép duyệt / từ chối / xem chi tiết / in phiếu / mở Vỏ bệnh án |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở module → form load danh sách yêu cầu khám chuyên khoa của khoa / phòng / user hiện tại.
2. Lọc theo: mã điều trị, mã bệnh nhân, từ khóa, khoa mời, khoa thực hiện, trạng thái duyệt, khoảng thời gian mời.
3. Trên mỗi dòng yêu cầu:
   - Nút **Duyệt / Từ chối** — nếu user thuộc khoa thực hiện hoặc nằm trong danh sách bác sĩ mời.
   - Nút **Sửa** — nếu user là người tạo / thuộc khoa mời / là bác sĩ mời, và yêu cầu chưa duyệt.
   - Nút **Xóa** — cùng điều kiện sửa.
   - Nút **In** — khi yêu cầu đã duyệt (in MPS000500).
   - Nút **Bỏ duyệt** — khi yêu cầu đã duyệt.
   - Checkbox **Đã mổ** — chỉ cho tích khi yêu cầu đã duyệt (`IS_APPROVAL = 1`), chưa đánh dấu đã mổ (`IS_OPERATED ≠ 1`) và user là người tạo (`CREATOR`) hoặc nằm trong danh sách `EXAM_EXECUTE_LOGINNAME`. Tích thành công → ô khóa cứng (đã tích, không bỏ tích được).
   - **Right-click** → menu Vỏ bệnh án (mở các mẫu bệnh án EMR) cho TREATMENT_ID của dòng đó.

### Hành vi cột "Đã mổ"
| Trạng thái dòng | Quyền | Hành vi |
|-----------------|-------|---------|
| Chưa duyệt / Từ chối | Bất kỳ | Ô khóa, không cho tích |
| Đã duyệt, chưa đánh dấu | Người tạo HOẶC có trong `EXAM_EXECUTE_LOGINNAME` | Ô cho phép tích |
| Đã duyệt, chưa đánh dấu | User khác | Ô khóa, không cho tích |
| Đã duyệt, đã đánh dấu | Bất kỳ | Ô đã tích, bị khóa |

### Sơ đồ trạng thái duyệt
```
Chưa duyệt → Đã duyệt → (Bỏ duyệt) → Chưa duyệt
       ↘ Từ chối ↗
```

### Điều kiện right-click hiện menu
- Phải right-click vào 1 dòng có dữ liệu.
- Dòng phải có `TREATMENT_ID` hợp lệ.
- Không có dòng → KHÔNG hiển thị menu.
- Đóng màn hình con KHÔNG reload grid.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SPECIALIST_EXAM | View | Dữ liệu hiển thị grid yêu cầu khám chuyên khoa (gồm cột `IS_OPERATED` — phục vụ checkbox "Đã mổ") |
| HIS_SPECIALIST_EXAM | Table | Dữ liệu gốc khi sửa / xóa / từ chối / đánh dấu đã mổ (`IS_OPERATED`) |
| HIS_TREATMENT | Table | Load theo `TREATMENT_ID` để build EmrInputADO mở Vỏ bệnh án |
| HIS_DEPARTMENT | Table | Combo lọc khoa mời / khoa thực hiện |
| V_HIS_ROOM | View (cache) | Tra cứu DEPARTMENT_ID của phòng hiện tại |
| HIS_EMR_COVER_CONFIG | Table (cache) | Xác định Vỏ bệnh án mặc định theo phòng / khoa + loại điều trị |
| WorkPlaceSDO | Local cache | Lấy DepartmentId của RoomId hiện tại |

## 4. UI Layout

### Sơ đồ giao diện
```
+----------------------------------------------------------------------------+
| [Mã ĐT] [Mã BN] [Tìm kiếm] [Khoa mời] [Khoa TH] [TT duyệt]                |
| [Từ ngày] [Đến ngày] [Tìm kiếm]                                            |
+----------------------------------------------------------------------------+
| Grid V_HIS_SPECIALIST_EXAM:                                                 |
|   STT | Chi tiết | Xóa | Sửa | Duyệt | Từ chối | Bỏ duyệt | In |          |
|   Trạng thái | Mã BN | Mã ĐT | Họ tên | TG mời | Khoa mời | Khoa TH | ... |
|   Khám tại giường | Khám tiền gây mê | Đã mổ | Bác sĩ khám | TG tạo | ... |
|   ↳ Right-click → menu Vỏ bệnh án (TREATMENT_ID của dòng được click)       |
+----------------------------------------------------------------------------+
| [Phân trang]                                                                |
+----------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang server-side |

### Library sử dụng
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Tạo InputADO ký số khi in MPS000500 |
| HIS.Desktop.Plugins.Library.FormMedicalRecord | Render menu Vỏ bệnh án khi right-click trên grid |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy danh sách yêu cầu | api/HisSpecialistExam/GetView | MosConsumer |
| Xóa | api/HisSpecialistExam/Delete | MosConsumer |
| Bỏ duyệt | api/HisSpecialistExam/UnApproval | MosConsumer |
| Đánh dấu đã mổ | api/HisSpecialistExam/MarkOperated | MosConsumer |
| Lấy danh sách khoa | api/HisDepartment/Get | MosConsumer |
| Lấy điều trị (cho menu Vỏ bệnh án) | api/HisTreatment/Get | MosConsumer |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.EmrGenerate | Ký số EMR khi in |
| HIS.Desktop.Plugins.Library.FormMedicalRecord | Menu Vỏ bệnh án (cover types + forms) |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|------------|-------------|
| HIS.Desktop.Plugins.ApprovalExamSpecialist | Click duyệt / xem chi tiết khi `IS_EXAM_ANESTHESIA ≠ 1` (null hoặc 0) — khám chuyên khoa thông thường | row (V_HIS_SPECIALIST_EXAM) + RefeshReference |
| HIS.Desktop.Plugins.ApprovalExamAnesthesia | Click duyệt / xem chi tiết khi `IS_EXAM_ANESTHESIA = 1` — phiếu khám tiền gây mê | row (V_HIS_SPECIALIST_EXAM) + RefeshReference |
| HIS.Desktop.Plugins.InviteSpecialistExam | Click sửa (khi chưa duyệt) | currentModule, datamapper, RefeshReference |
| HIS.Desktop.Plugins.EmrDocument | Click cột "Chi tiết bệnh án" hoặc menu chuột phải "Chi tiết bệnh án" | TREATMENT_ID (long); permission check qua `GlobalVariables.currentModuleRaws` — không có quyền → ẩn cả nút và menu |

**Logic phân loại plugin duyệt** (tập trung tại helper `OpenApprovalPluginByType(V_HIS_SPECIALIST_EXAM row)` trong `frmExamSpecialist.cs`):
```
row.IS_EXAM_ANESTHESIA == 1 → mở ApprovalExamAnesthesia
ngược lại (null hoặc 0)      → mở ApprovalExamSpecialist
```
Hằng `ModuleLink` định nghĩa trong `ModuleLinkString.cs` (root plugin) — KHÔNG hardcode string trong handler nữa.

## 7. Print

| Loại in | PrintTypeCode | PDO | Mục đích |
|---------|--------------|-----|----------|
| Phiếu khám chuyên khoa | MPS000500 | Mps000500PDO(row, treatment) | In khi yêu cầu đã duyệt |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-04-28 | dangth2 | PTTK_42625 — Thêm right-click menu Vỏ bệnh án trên grid (sử dụng `MediRecordMenuPopupProcessor` từ `HIS.Desktop.Plugins.Library.FormMedicalRecord`); thêm partial class `frmExamSpecialist__RightClick.cs`; reference `HIS.Desktop.Plugins.Library.FormMedicalRecord.dll` |
| 2026-04-28 | dangth2 | PTTK_42625 (bổ sung) — Cấu hình cột `gridColumn_MedicalRecorDetails` ("Chi tiết bệnh án") thành nút icon (RepositoryItemButtonEdit, Glyph + zoom_16x16.png), đặt VisibleIndex=8 ngay trước cột "Trạng thái" (=9). Thêm menu item "Chi tiết bệnh án" vào popup chuột phải. Cả nút và menu đều mở `HIS.Desktop.Plugins.EmrDocument` qua `PluginInstanceBehavior.ShowModule` truyền TREATMENT_ID; nếu user không có quyền truy cập module EmrDocument trong `GlobalVariables.currentModuleRaws` thì ẩn menu / log warn nút. Reference thêm `HIS.Desktop.ModuleExt.dll`. |
| 2026-05-18 | dangth2 | PTTK_42786 — Thêm cột checkbox **"Đã mổ"** (`gridColumn_Is_Operated`, FieldName `IS_OPERATED_STR` Unbound) vào grid, đặt VisibleIndex=19 ngay sau "Khám tiền gây mê" (=18); các cột audit sau đó bump 1 đơn vị. Thêm 2 RepositoryItemCheckEdit: `repositoryItemCheckEditIsOperatedEnabled` (cho phép tích, hook `EditValueChanged`) và `repositoryItemCheckEditIsOperatedDisabled` (ReadOnly=true). `gridView1_CustomRowCellEdit` chọn repo theo trạng thái + quyền; `CustomUnboundColumnData` map từ `IS_OPERATED`. Khi user tích → POST `api/HisSpecialistExam/MarkOperated` (body=row.ID); thành công cập nhật `row.IS_OPERATED=1` + RefreshRow; lỗi giữ nguyên + hiển thị message backend. Lưu ý: phụ thuộc backend gencode lại `V_HIS_SPECIALIST_EXAM` / `HIS_SPECIALIST_EXAM` có thêm cột `IS_OPERATED`. |
| 2026-05-20 | dangth2 | PTTK_42786 (bổ sung) — Cell "Đã mổ" ở trạng thái khoá hiển thị checkbox **disabled thực sự** (glyph xám): `gridView1_CustomDrawCell` cho `IS_OPERATED_STR` không editable → tự vẽ checkbox qua `System.Windows.Forms.ControlPaint.DrawCheckBox` với `ButtonState.Inactive` (cộng `Checked` khi `IS_OPERATED=1`), set `e.Handled=true`. Repo disabled chỉ giữ `ReadOnly=true` + `AllowFocused=false` để chặn input. Tách helper `IsOperatedEditable(data)` dùng chung giữa `CustomRowCellEdit` và `CustomDrawCell`. |
| 2026-05-22 | dangth2 | PTTK_38078 (mục 4.1.3) — Phân loại plugin duyệt theo `IS_EXAM_ANESTHESIA` khi mở từ nút Xem chi tiết / Duyệt trên dòng dữ liệu Grid. Thêm `ModuleLinkString.cs` (root plugin) chứa hằng `ApprovalExamSpecialist`, `ApprovalExamAnesthesia`, `InviteSpecialistExam`. Refactor 2 handler `repositoryItemButtonEditApproval_Click` và `repositoryItemButtonEditDetails_Click` dùng chung helper `OpenApprovalPluginByType(row)`: `IS_EXAM_ANESTHESIA = 1` → mở `HIS.Desktop.Plugins.ApprovalExamAnesthesia`; khác → giữ nguyên `HIS.Desktop.Plugins.ApprovalExamSpecialist`. KHÔNG đổi giao diện / columns / filter danh sách. Nút "Sửa" (khi chưa duyệt) vẫn mở `InviteSpecialistExam` (chỉ thay hardcode string bằng constant). |

## 9. Test Cases

### Cột & menu "Chi tiết bệnh án" (mở plugin EmrDocument)
- [ ] Click nút icon trên cột "Chi tiết bệnh án" của 1 dòng → mở plugin `HIS.Desktop.Plugins.EmrDocument` đúng TREATMENT_ID.
- [ ] Right-click vào dòng → menu popup có mục "Chi tiết bệnh án" → click → mở EmrDocument.
- [ ] Cột "Chi tiết bệnh án" hiển thị NGAY TRƯỚC cột "Trạng thái" (VisibleIndex 8 vs 9).
- [ ] User KHÔNG có quyền truy cập module `HIS.Desktop.Plugins.EmrDocument` (không nằm trong `GlobalVariables.currentModuleRaws`) → menu item "Chi tiết bệnh án" KHÔNG hiển thị; nút trên cột vẫn render nhưng click ra log warn (không mở).
- [ ] Sau khi đóng EmrDocument → grid KHÔNG reload.

### Right-click menu Vỏ bệnh án
- [ ] Right-click vào dòng có TREATMENT_ID → menu Vỏ bệnh án hiển thị tại vị trí chuột.
- [ ] Right-click vào vùng trống grid → KHÔNG hiển thị menu.
- [ ] Dòng có TREATMENT_ID nhưng API `api/HisTreatment/Get` lỗi → ghi log warn, KHÔNG hiển thị menu.
- [ ] HIS_TREATMENT có `EMR_COVER_TYPE_ID` → menu hiển thị đúng cover type đó.
- [ ] HIS_TREATMENT chưa có `EMR_COVER_TYPE_ID` nhưng có config theo phòng → menu lấy config theo phòng.
- [ ] Không có config theo phòng nhưng có theo khoa → menu lấy config theo khoa.
- [ ] User chọn 1 mục con → mở chức năng tương ứng, truyền TREATMENT_ID; đóng màn hình → grid KHÔNG reload.

### Duyệt / Từ chối / Xóa / Sửa / In
- [ ] User thuộc khoa thực hiện → nút Duyệt / Từ chối active.
- [ ] User là người tạo / thuộc khoa mời / là bác sĩ mời → nút Sửa / Xóa active.
- [ ] Yêu cầu đã duyệt → cho phép Bỏ duyệt + In MPS000500.
- [ ] Lọc theo trạng thái duyệt = Tất cả → bỏ filter IS_APPROVAL.

### Phân loại plugin duyệt theo IS_EXAM_ANESTHESIA (PTTK_38078)
- [ ] Click "Duyệt" trên dòng `IS_EXAM_ANESTHESIA = 1` → mở `HIS.Desktop.Plugins.ApprovalExamAnesthesia` (đúng plugin tiền gây mê).
- [ ] Click "Duyệt" trên dòng `IS_EXAM_ANESTHESIA = null` hoặc `= 0` → mở `HIS.Desktop.Plugins.ApprovalExamSpecialist` (giữ nguyên hành vi cũ — CK thông thường).
- [ ] Click "Xem chi tiết" trên dòng `IS_EXAM_ANESTHESIA = 1` → mở `HIS.Desktop.Plugins.ApprovalExamAnesthesia`.
- [ ] Click "Xem chi tiết" trên dòng `IS_EXAM_ANESTHESIA ≠ 1` → mở `HIS.Desktop.Plugins.ApprovalExamSpecialist`.
- [ ] User KHÔNG có quyền truy cập `HIS.Desktop.Plugins.ApprovalExamAnesthesia` (không trong `GlobalVariables.currentModuleRaws`) → ghi log error, KHÔNG mở plugin (không crash).
- [ ] Args truyền vào plugin duyệt (cả 2 loại) giữ nguyên: `row` (V_HIS_SPECIALIST_EXAM) + `RefeshReference` (callback FillDataToGrid).
- [ ] Giao diện grid, columns hiển thị, filter danh sách KHÔNG đổi sau thay đổi.
- [ ] Nút "Sửa" (khi `IS_APPROVAL = null`) vẫn mở `HIS.Desktop.Plugins.InviteSpecialistExam` — KHÔNG bị phân loại theo `IS_EXAM_ANESTHESIA`.

### Cột "Đã mổ"
- [ ] Cột "Đã mổ" nằm ngay sau "Khám tiền gây mê" (VisibleIndex 19); các cột audit (TG tạo / Người tạo / TG sửa / Người sửa) vẫn ở cuối grid.
- [ ] Dòng `IS_APPROVAL` chưa duyệt (null) hoặc từ chối (=2) → ô khóa, không tích được dù là bất kỳ user nào.
- [ ] Dòng `IS_APPROVAL = 1`, `IS_OPERATED ≠ 1`, user là `CREATOR` → tích được.
- [ ] Dòng `IS_APPROVAL = 1`, `IS_OPERATED ≠ 1`, user nằm trong `EXAM_EXECUTE_LOGINNAME` (sau khi split `,` và trim) → tích được.
- [ ] Dòng `IS_APPROVAL = 1`, `IS_OPERATED ≠ 1`, user khác → ô khóa.
- [ ] Dòng `IS_OPERATED = 1` → ô hiển thị đã tích, khóa cứng (không bỏ tích được dù là người tạo).
- [ ] User tích ô → POST `api/HisSpecialistExam/MarkOperated` body=ID; success → ô khóa lại + giữ trạng thái tích trên cùng row hiện hành (không reload toàn grid).
- [ ] API trả lỗi nghiệp vụ (chưa duyệt / đã đánh dấu / không quyền / không tìm thấy) → hiển thị message backend qua `MessageManager.Show`, ô về trạng thái cũ.
