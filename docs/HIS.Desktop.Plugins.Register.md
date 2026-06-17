# Tiếp Đón (Register) — Tài Liệu Module

> Tài liệu được khởi tạo phục vụ thay đổi TT 06/2026 (mục 2.4). Một số phần chỉ ghi nhận phạm vi liên quan đến thay đổi, chưa audit toàn bộ plugin.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Register |
| Loại | UC (tiếp đón/đăng ký bệnh nhân) |
| Mục đích | Tiếp đón, đăng ký khám bệnh, nhập thông tin BHYT và thông tin giới thiệu chuyển tuyến từ tuyến dưới |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ (phần liên quan TT 06/2026)

Plugin dùng chung **UC hiển thị thông tin BHYT** `His.UC.UCHein.MainHisHeinBhyt` (template `TEMPLATE__BHYT1`). UC này chứa ô **chẩn đoán giới thiệu chuyển tuyến từ tuyến dưới** (`cboChanDoanTD` / `txtMaChanDoanTD`).

**Quy tắc ICD theo TT 06/2026 — trường hợp mở từ tiếp đón:**
- **Cảnh báo bệnh chính (IS_NOT_RECOMMEND_MAIN = 1)**: KHÔNG cảnh báo dù nhập hay sửa (thông tin nhập lại của tuyến dưới, không phải quyết định của viện hiện tại).
- **Nguyên nhân tử vong (IS_DEATH_CAUSE_ONLY = 1)**: Không hiển thị các chẩn đoán đánh dấu nguyên nhân tử vong trong danh sách chọn.
- Không liên quan chẩn đoán YHCT (IS_TRADITIONAL).

Cách truyền tham số khi khởi tạo UC ([Run/UCRegister__UCHein.cs](../HIS/Plugins/HIS.Desktop.Plugins.Register/Run/UCRegister__UCHein.cs)):
```csharp
dataHein.IsHideIcdDeathCauseOnly = true;               // ẩn chẩn đoán nguyên nhân tử vong
dataHein.IsWarningIcdNotRecommendMainWhenEdit = false; // không cảnh báo "không khuyến khích dùng là bệnh chính"
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
| 16/06/2026 | sinhnt | TT 06/2026 (mục 2.4): tiếp đón truyền `IsHideIcdDeathCauseOnly=true`, `IsWarningIcdNotRecommendMainWhenEdit=false` cho UC BHYT — ẩn chẩn đoán nguyên nhân tử vong, không cảnh báo "không khuyến khích dùng là bệnh chính" |

## 9. Test Cases

- [ ] Mở tiếp đón → danh sách chẩn đoán giới thiệu KHÔNG hiển thị ICD có IS_DEATH_CAUSE_ONLY=1
- [ ] Chọn/sửa chẩn đoán có IS_NOT_RECOMMEND_MAIN=1 → KHÔNG hiện cảnh báo
- [ ] Chẩn đoán YHCT vẫn hiển thị bình thường
