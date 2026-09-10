# Tạo phiếu chăm sóc — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CareCreate |
| Loại | Form |
| Mục đích | Tạo/sửa phiếu chăm sóc bệnh nhân nội trú (2 phiên bản form theo key `HIS.DESKTOP.HIS_CARE.USING_FORM_VERSION`: frmHisCareCreate cũ và frmCareNew), nhập DHST, theo dõi, in phiếu chăm sóc |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

- Điều dưỡng mở phiếu chăm sóc từ buồng bệnh, nhập thông tin theo dõi (ý thức, da niêm mạc, DHST, y lệnh, chăm sóc...).
- In phiếu chăm sóc: mẫu Mps000069 (phiếu chăm sóc _ theo dõi) và Mps000229 (phiếu chăm sóc _ y lệnh QY7), có gán `InputADO.MergeCode` để in gộp/ký gộp EMR khi bật cấu hình.
- Khi bật in gộp và chọn nhiều bản ghi phiếu chăm sóc → chặn, chỉ cho in 1 bản ghi.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT | Table | Thông tin điều trị |
| HIS_CARE | Table | Phiếu chăm sóc |
| V_HIS_TREATMENT_BED_ROOM | View | Giường/buồng của điều trị |
| HIS_ICD | Table | Chẩn đoán |

## 4. Cấu Hình

| Key | Mô tả |
|-----|-------|
| `HIS.DESKTOP.HIS_CARE.USING_FORM_VERSION` | Chọn phiên bản form phiếu chăm sóc |
| `HIS.Desktop.Plugins.Care.IsPrintMerge` | **(mới — việc 31875)** In gộp phiếu chăm sóc: 1 = gộp; khác = không; rỗng/chưa cấu hình = fallback key cũ bên dưới |
| `HIS.Desktop.Plugins.EmrDocument.IsPrintMerge` | Key cũ dùng chung với tờ điều trị — chỉ còn là fallback cho phiếu chăm sóc |
| `HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking` | Mặc định tracking |
| `HIS.Desktop.Plugins.TrackingCreate.IsMineCheckedByDefault` | Mặc định tick "của tôi" |

Logic đọc key in gộp: `SdaConfigKeys.GetKeyPrintMerge()` (SdaConfigKeys.cs).

## 5. Print

| Loại in | PrintTypeCode | Template |
|---------|--------------|----------|
| Phiếu chăm sóc _ theo dõi | Mps000069 | Excel |
| Phiếu chăm sóc _ y lệnh (QY7) | Mps000229 | Excel |

## 6. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/09/2026 | nampp | Việc 31875 (fix QA): form tạo chăm sóc bản cũ `frmHisCareCreate` (USING_FORM_VERSION=2) in Mps000229/Mps000427 không hề gán MergeCode từ trước → bổ sung `GetKeyPrintMerge()` + gán MergeCode + log debug vào `LoadBieuMauPhieuQy7` và `LoadBieuMauPhieuChamSocCapI` |
| 04/09/2026 | nampp | Việc 31875: tách cấu hình in gộp phiếu chăm sóc khỏi tờ điều trị — key mới `HIS.Desktop.Plugins.Care.IsPrintMerge` (fallback key cũ `HIS.Desktop.Plugins.EmrDocument.IsPrintMerge` khi chưa cấu hình). Sửa `SdaConfigKeys.cs` (thêm key + `GetKeyPrintMerge()`), `FrmCare\frmCareNew__Plus__Print.cs` (đọc qua hàm mới). Tờ điều trị (TrackingCreate) không đổi. Tài liệu: `PTTK\31875 - Thiet ke - Tach cau hinh in gop phieu cham soc truyen dich.md`, script DB: `PTTK\31875_insert_his_config.sql` |

## 7. Test Cases

- [ ] Key mới rỗng, key cũ = 1 → in phiếu chăm sóc vẫn gộp (fallback, tương thích ngược)
- [ ] Key cũ = 1, key `Care.IsPrintMerge` = 0 → phiếu chăm sóc KHÔNG gộp, tờ điều trị VẪN gộp
- [ ] Key cũ = 0/rỗng, key `Care.IsPrintMerge` = 1 → phiếu chăm sóc gộp, tờ điều trị không gộp
- [ ] Bật gộp + chọn nhiều bản ghi → hiện cảnh báo chỉ cho chọn 1 bản ghi như cũ
