# Đối Tượng Điều Trị (CallPatientTypeAlter) — Tài Liệu Module

> Tài liệu được khởi tạo phục vụ thay đổi TT 06/2026 (mục 2.4). Một số phần chỉ ghi nhận phạm vi liên quan đến thay đổi, chưa audit toàn bộ plugin.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CallPatientTypeAlter |
| Loại | Form (đổi đối tượng điều trị/BHYT của hồ sơ) |
| Mục đích | Thay đổi loại đối tượng bệnh nhân (BHYT, thu phí, QN...) và cập nhật thông tin BHYT, thông tin giới thiệu chuyển tuyến |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ (phần liên quan TT 06/2026)

Plugin dùng chung **UC hiển thị thông tin BHYT** `His.UC.UCHein.MainHisHeinBhyt` (template `TEMPLATE__BHYT1`), khởi tạo với cờ `IsInitFromCallPatientTypeAlter = true`. UC chứa ô **chẩn đoán giới thiệu chuyển tuyến từ tuyến dưới** (`cboChanDoanTD` / `txtMaChanDoanTD`).

**Quy tắc ICD theo TT 06/2026 — trường hợp mở từ đối tượng điều trị:**
- **Cảnh báo bệnh chính (IS_NOT_RECOMMEND_MAIN = 1)**: CHỈ cảnh báo khi SỬA chẩn đoán chính (không phải lúc hiển thị thông tin). Nếu chẩn đoán đánh dấu "Không khuyến khích dùng là bệnh chính" → hiện cảnh báo *"Bệnh XXX không khuyến khích dùng làm bệnh chính. Bạn có chắc chắn sử dụng không?"*. Chọn Có → giữ nguyên; chọn Không → xóa thông tin và chọn lại.
- **Nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1)**: Không hiển thị các chẩn đoán đánh dấu nguyên nhân tử vong trong danh sách chọn.
- Không liên quan chẩn đoán YHCT (IS_TRADITIONAL).

Cách truyền tham số khi khởi tạo UC ([frmPatientTypeAlter_PatientTypeAlter.cs](../HIS/Plugins/HIS.Desktop.Plugins.CallPatientTypeAlter/frmPatientTypeAlter_PatientTypeAlter.cs)):
```csharp
dataHein.IsInitFromCallPatientTypeAlter = true;
dataHein.IsHideIcdDeathCauseOnly = true;              // ẩn chẩn đoán nguyên nhân tử vong
dataHein.IsWarningIcdNotRecommendMainWhenEdit = true; // cảnh báo khi sửa "không khuyến khích dùng là bệnh chính"
```

## 3. EFMODEL Sử Dụng (liên quan thay đổi)

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_ICD / V_HIS_ICD | Table/View | Danh mục ICD. Bổ sung `IS_NOT_RECOMMEND_MAIN`, `IS_DEATH_CAUSE_ONLY` (short?) |
| HIS_TREATMENT | Table | Lưu `TRANSFER_IN_ICD_CODE`, `TRANSFER_IN_ICD_NAME` (chẩn đoán chuyển tuyến) |

## 6. Dependencies

| UC dùng chung | Mục đích |
|---------------|----------|
| His.UC.UCHein (MainHisHeinBhyt) | Hiển thị thông tin BHYT + chẩn đoán giới thiệu chuyển tuyến. Logic lọc/cảnh báo ICD nằm trong UC này |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 16/06/2026 | sinhnt | TT 06/2026 (mục 2.4): truyền `IsHideIcdDeathCauseOnly=true`, `IsWarningIcdNotRecommendMainWhenEdit=true` cho UC BHYT — ẩn chẩn đoán nguyên nhân tử vong, cảnh báo khi sửa "không khuyến khích dùng là bệnh chính" |
| 17/06/2026 | sinhnt | Fix bug: sửa mã đối tượng đúng tuyến (RIGHT_ROUTE_TYPE_CODE, vd 3.1→3.6) lưu thành công nhưng mở lại vẫn giá trị cũ. Nguyên nhân: nhánh ActionEdit sau Update không đồng bộ kết quả về object cache `currentTreatmentLogSDO`. Fix: map đầy đủ `resultPatientTypeAlter.PatientTypeAlter` về cache (giống ActionAdd) |
| 29/07/2026 | tuanln | PT-44730: khi chuyển đối tượng thanh toán của hồ sơ, chỉ định thuộc dịch vụ đã khai trong bảng cấu hình `HIS_SERVICE_DEFAULT_PATY` (chờ backend) mà tài khoản không đủ quyền sửa ĐTTT thì **giữ nguyên ĐTTT cũ** của chỉ định đó, các chỉ định còn lại vẫn chuyển bình thường. Thêm partial `frmPatientTypeAlter___ServiceDefaultPaty.cs` (worker nạp cấu hình 1 lần/form + `IsAllowEditPatientTypeByServiceConfig`, người chỉ định lấy theo `TDL_REQUEST_LOGINNAME` của chỉ định). Trong `SwapPatientTypeAlter`, chèn bước hoàn lại `oldPatientTypeId` ngay trước khối xử lý `SERVICE_CONDITION_ID` / `DO_NOT_USE_BHYT`. Quyền theo key `HIS.Desktop.Plugins.Assign.ServiceDefaultPatyEditOption` (`1` = quản trị · `2` = quản trị hoặc người chỉ định · khác = không siết) |

## 9. Test Cases

- [ ] Danh sách chẩn đoán giới thiệu KHÔNG hiển thị ICD có IS_DEATH_CAUSE_ONLY=1
- [ ] Sửa chẩn đoán có IS_NOT_RECOMMEND_MAIN=1 → hiện cảnh báo; chọn Không → xóa & chọn lại; chọn Có → giữ nguyên
- [ ] Hiển thị (load) hồ sơ có sẵn → KHÔNG hiện cảnh báo
- [ ] Chẩn đoán YHCT vẫn hiển thị bình thường
