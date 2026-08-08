# Xuất XML QĐ130 (ExportXmlQD130) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExportXmlQD130 |
| Loại | UC |
| Mục đích | Màn danh sách hồ sơ điều trị và xuất XML BHYT theo QĐ130 (kèm XML12/GDYK, thông tuyến, excel, ký số, đồng bộ 4750). Tài liệu này tạo khi bổ sung chức năng "Kiểm tra hồ sơ" theo PTTK 3142; các luồng xuất XML có sẵn chỉ mô tả tóm tắt. |
| Người tạo | (plugin có sẵn) |
| Ngày sửa | 07/08/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng có sẵn (tóm tắt)
Tìm hồ sơ (api/HisTreatment/GetView1) → chọn hồ sơ trên grid → xuất XML 130/GDYK/thông tuyến/excel qua thư viện `His.Bhyt.ExportXml.XML130.CreateXmlProcessor` (validate + build XML nằm trong DLL), có thể ký số và gửi cổng BHYT.

### Luồng MỚI — menu chuột phải "Kiểm tra hồ sơ" (PTTK 3142 mục 3.3)
1. Chọn 1..n hồ sơ trên grid → chuột phải → **Kiểm tra hồ sơ**.
2. Nạp dữ liệu hồ sơ như luồng xuất (CreateThreadGetData theo batch).
3. **Kiểm tra phạm vi chuyên môn** từng dịch vụ — cách chọn người thực hiện **mô phỏng đúng logic điền trường NGUOI_THUC_HIEN của Xml3Processor (XML3)**:
   - Y lệnh khám (`TDL_HEIN_SERVICE_TYPE_ID = ID__KH`): người thực hiện = `EXECUTE_LOGINNAME` (theo PTTK, không lọc CCHN).
   - Bỏ qua: Giường (GI_* — theo PTTK) và Vật tư (VT_* — XML3 không điền người thực hiện).
   - Y lệnh khác (giống XML3): (1) `HST_BHYT_CODE` của dịch vụ thuộc config `HIS.QD_130_BYT.NGUOI_THUC_HIEN_OPTION` (split `,`) và có kíp → lấy cả kíp; (2) chưa có ai → gom `SAMPLER_LOGINNAME` + `SUBCLINICAL_RESULT_LOGINNAME` (tách `;`) + `EXECUTE_LOGINNAME`; (3) kíp `EKIP_ID` luôn được cộng thêm nếu có; (4) vẫn chưa có ai và `START_TIME` null → `REQUEST_LOGINNAME`. Chỉ tính người **có CCHN** (`HIS_EMPLOYEE.DIPLOMA` khác rỗng) — đúng tập người thực sự xuất hiện trên XML3.
   - Mỗi người thực hiện có `HIS_EMPLOYEE.SPECIALITY_CODES` → tách mã `;` → ID HIS_SPECIALITY → gọi `api/HisServiceSpeciality/Get` (SPECIALITY_IDs) → danh sách A (SERVICE_ID được phép, cache theo login + theo speciality trong phiên kiểm tra). A khác rỗng và dịch vụ ∉ A → lỗi: *"Dịch vụ {mã} {tên} không thuộc phạm vi chuyên môn của người thực hiện ({loginname}). (Mã y lệnh: {TDL_SERVICE_REQ_CODE})"*.
4. **Chạy lại validate xuất XML**: build InputADO y hệt luồng xuất rồi gọi **`CreateXmlProcessor.CheckHoSo()`** — method public MỚI trong thư viện `His.Bhyt.ExportXml.XML130` (source `D:\Common\common\HISUTIL\His.Bhyt`): cùng bộ rule với hàm `Check` khi xuất nhưng gom TẤT CẢ lỗi (không dừng ở lỗi đầu), không build XML, không ghi file, không mutate dữ liệu.
5. Gộp toàn bộ lỗi hiển thị 1 thông báo (`- <lỗi>: <mã hồ sơ>`); không lỗi → **"Hồ sơ hợp lệ"**.

### Điều kiện nghiệp vụ
- Luồng tạo XML thật KHÔNG bị ảnh hưởng (hàm kiểm tra tách riêng; khi xuất XML không check phạm vi chuyên môn).
- Người thực hiện không có SPECIALITY_CODES hoặc chưa map dịch vụ nào → không áp check (theo PTTK "Nếu A khác null thì").
- **Dịch vụ chưa được thiết lập map với bất kỳ phạm vi chuyên môn nào → hợp lệ, bỏ qua không kiểm tra** (tra trước theo SERVICE_IDs, có cache — bổ sung 08/08/2026 theo phản hồi test).
- Bộ lọc dịch vụ đưa vào kiểm tra giống luồng xuất (AMOUNT>0, không IS_EXPEND, giá theo config LAY_CA_DVU_0_DONG).

## 3. EFMODEL Sử Dụng (phần mới)

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_SERE_SERV_2 | View | Dịch vụ hồ sơ: TDL_HEIN_SERVICE_TYPE_ID, EXECUTE/SAMPLER/SUBCLINICAL_RESULT/REQUEST_LOGINNAME, EKIP_ID, START_TIME, SERVICE_ID |
| HIS_EMPLOYEE | Table (cache) | SPECIALITY_CODES (mã PVCM cách `;`), LOGINNAME |
| HIS_SPECIALITY | Table (cache) | Map SPECIALITY_CODE → ID |
| HIS_HEIN_SERVICE_TYPE | Table (cache) | BHYT_CODE đối chiếu config NGUOI_THUC_HIEN_OPTION |
| HIS_EKIP_USER | Table (API) | Thành viên kíp theo EKIP_ID |
| HIS_SERVICE_SPECIALITY | Table (MOS.EFMODEL 06/08/2026) | Map Dịch vụ ↔ Phạm vi chuyên môn |

Filter local: `ADO/HisServiceSpecialityFilter.cs` (MOS.Filter chưa có — thay khi lib bổ sung).

## 4. UI Layout

Grid danh sách hồ sơ (`gridViewTreatment`) — menu chuột phải: 6 item xuất XML có sẵn + item mới **"Kiểm tra hồ sơ"** (cuối menu). Không thay đổi layout khác.

## 5. API Endpoints (phần mới)

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy map DV-PVCM | api/HisServiceSpeciality/Get | MosConsumer | HisServiceSpecialityFilter (SPECIALITY_IDs) — **chờ BE** |

Config mới: `HIS.QD_130_BYT.NGUOI_THUC_HIEN_OPTION` — danh sách BHYT_CODE (HIS_HEIN_SERVICE_TYPE) nhóm y lệnh lấy người thực hiện theo kíp, cách nhau `,` (khai báo trong HIS_CONFIG — **BE/triển khai thêm key**).

## 6. Dependencies

Thư viện `His.Bhyt.ExportXml.XML130` — source tại `D:\Common\common\HISUTIL\His.Bhyt\His.Bhyt.ExportXml.XML130`: InputADO, CreateXmlProcessor (Run = validate + build XML; **CheckHoSo/CheckDetailDataAll = validate gom toàn bộ lỗi, thêm 07/08/2026 theo PTTK 3142** — Check/Run cũ giữ nguyên, tương thích ngược).

## 7. Print

Không.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 07/08/2026 | nampp + Claude | PTTK 3142 mục 3.3: thêm menu chuột phải "Kiểm tra hồ sơ" (file mới `UCExportXml_KiemTraHoSo.cs`): check phạm vi chuyên môn người thực hiện theo loại y lệnh + config NGUOI_THUC_HIEN_OPTION; gọi `CreateXmlProcessor.CheckHoSo()` (method mới trong lib His.Bhyt.ExportXml.XML130, gom toàn bộ lỗi validate, không tạo file); gộp thông báo / "Hồ sơ hợp lệ". Thêm `ADO/HisServiceSpecialityFilter.cs`, key config `HIS.QD_130_BYT.NGUOI_THUC_HIEN_OPTION` vào HisConfigCFG. Xóa entry licenses.licx stale (chặn build). |

## 9. Test Cases

### Kiểm tra hồ sơ — phạm vi chuyên môn
- [ ] Dịch vụ CHƯA thiết lập map với chuyên môn nào → không cảnh báo (hợp lệ)
- [ ] Hồ sơ có y lệnh khám do bác sĩ có PVCM nhưng dịch vụ khám ĐÃ map cho chuyên môn khác → báo lỗi kèm mã y lệnh + loginname
- [ ] Dịch vụ được map đúng PVCM người thực hiện → không báo lỗi phần PVCM
- [ ] Người thực hiện không có SPECIALITY_CODES → bỏ qua check
- [ ] User có PVCM nhưng chưa map dịch vụ nào (A rỗng) → bỏ qua check
- [ ] Y lệnh giường → không check
- [ ] Y lệnh PTTT có BHYT_CODE trong config NGUOI_THUC_HIEN_OPTION → check theo từng thành viên kíp
- [ ] Y lệnh XN không thuộc config → check theo SAMPLER + KTV (tách `;`) + EXECUTE + kíp
- [ ] Y lệnh chưa có START_TIME và không có người thực hiện → check theo REQUEST_LOGINNAME

### Kiểm tra hồ sơ — validate xuất XML
- [ ] Hồ sơ thiếu FEE_LOCK_TIME/OUT_TIME/ICD... → thông báo gộp đúng lỗi như khi xuất XML
- [ ] Hồ sơ đầy đủ → "Hồ sơ hợp lệ"
- [ ] Chọn nhiều hồ sơ lẫn lỗi + hợp lệ → thông báo gộp theo từng lỗi kèm mã hồ sơ
- [ ] Sau khi "Kiểm tra hồ sơ", KHÔNG có file XML nào được tạo trong thư mục lưu
- [ ] Luồng "Xuất XML" bình thường không đổi hành vi (không check PVCM khi xuất)
