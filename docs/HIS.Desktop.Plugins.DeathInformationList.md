# Danh Sách Hồ Sơ Tử Vong — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.DeathInformationList |
| Loại | UC (UserControl) |
| Mục đích | Quản lý danh sách hồ sơ tử vong, đồng bộ giấy báo tử lên cổng Bộ Y tế (BHXH) và đồng bộ ca tử vong lên hệ thống KCB. Sau khi triển khai TLN, mở rộng để xem thêm hồ sơ bệnh nặng xin về (cùng UC HisDeathInfo nhưng phân biệt theo Loại ra viện). |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở Danh sách hồ sơ tử vong → UC load hồ sơ điều trị (V_HIS_TREATMENT_11) có ca tử vong (mặc định) hoặc bệnh nặng xin về (khi tick filter mới).
2. Filter bên trái cho phép lọc theo: mã điều trị, mã/tên BN, ngày tử vong/xin về, ngày cấp GBT, khoa kết thúc, trạng thái đồng bộ, kết quả ĐT, loại ra viện.
3. Người dùng chọn (tick) hồ sơ → bật 2 nút "Đồng bộ giấy báo tử" và "Đồng bộ ca tử vong".
4. "Đồng bộ giấy báo tử": tạo XML giấy báo tử, ký số (USB token / HSM), gửi BHXH.
5. "Đồng bộ ca tử vong": gửi danh sách treatmentId lên backend để sync ca tử vong (BE tự sinh XML phù hợp theo KQĐT của hồ sơ).
6. Click cột TTTV (mở popup) → mở plugin HIS.Desktop.Plugins.HisDeathInfo. UC này tự render title/label theo KQĐT của hồ sơ (tử vong vs nặng xin về).

### Điều kiện nghiệp vụ
- Cấu hình MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES (comma-separated mã KQĐT) quyết định mã KQĐT nào được coi là "bệnh nặng xin về".
- Hồ sơ bệnh nặng xin về KHÔNG có giấy báo tử → filter "Ngày cấp giấy" sẽ bỏ qua nhóm này.
- Cờ phân biệt nằm ở Loại ra viện (TREATMENT_END_TYPE) — không có cờ riêng trong DB.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_TREATMENT_11 | View | Danh sách hồ sơ điều trị tử vong/xin về |
| HIS_TREATMENT_END_TYPE | Table | Mapping mã code → ID loại ra viện (BHYT) |
| HIS_PATIENT | Table | Thông tin bệnh nhân (dùng khi sync XML) |
| HIS_SEVERE_ILLNESS_INFO | Table | Thông tin tử vong / bệnh nặng xin về |
| HIS_BRANCH | Table | Chi nhánh (mã HEIN_MEDI_ORG_CODE để build mã GBT) |
| HIS_DEPARTMENT | Table | Khoa kết thúc điều trị (filter) |

## 4. UI Layout

### Sơ đồ giao diện
```
+----------+------------------------------------------------------+
| Filter:  | Grid V_HIS_TREATMENT_11                              |
| - Mã ĐT  | STT | TT | TTTV | MaĐT | MaBN | TênBN | NS | GT |  |
| - Mã BN  |     Loại ra viện | MãGBT | TGTV/xinvề | NgayGBT | ...|
| - TênBN  +------------------------------------------------------+
| - Ngày   | Bottom buttons:                                      |
|   TV/xinvề| [Đồng bộ giấy báo tử] [Ký số]                       |
| - Ngày   | [Đồng bộ ca tử vong]                                 |
|   GBT    |                                                      |
| - Khoa   |                                                      |
| - Trạng  |                                                      |
|   thái   |                                                      |
|   ĐB     |                                                      |
| - □ Kết  |                                                      |
|   quả TV |                                                      |
| - □ Loại |                                                      |
|   ra viện|                                                      |
|   tử vong|                                                      |
| - □ Kết  |                                                      |
|   quả    |                                                      |
|   nặng   |                                                      |
|   xin về |                                                      |
| - Trạng  |                                                      |
|   thái   |                                                      |
|   ca TV  |                                                      |
| [Tìm]    |                                                      |
| [Làm lại]|                                                      |
+----------+------------------------------------------------------+
```

### UC sử dụng
| UC | Mục đích |
|----|----------|
| Inventec.UC.Paging | Phân trang grid |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy danh sách | api/HisTreatment/GetView11 | MosConsumer | HisTreatmentView11Filter |
| Lấy BN | api/HisPatient/Get | MosConsumer | HisPatientFilter |
| Lấy thông tin tử vong | api/HisSevereIllnessInfo/Get | MosConsumer | HisSevereIllnessInfoFilter |
| Đồng bộ GBT (BHXH) | api/HisTreatment/SyncDeath | MosConsumer | List<DeathSyncSDO> |
| Đồng bộ ca tử vong | api/HisSevereIllnessInfo/DeathCaseSync | MosConsumer | List<long> treatmentIds |
| Ký số XML BHYT | api/EmrSign/SignXmlBhyt | EmrConsumer | SignXmlBhytSDO |

## 6. Dependencies

### Library Plugins / External
| Library | Mục đích |
|---------|----------|
| HIS.Bhyt.Hssk.SyncDataProcess | Build XML GBT để gửi BHXH |
| Inventec.Common.SignFile.CertUtil | Lấy chứng thư số theo serial |
| EMR.SignProcessor.exe | App ký số XML (chạy ngoài) |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.HisDeathInfo | Click cột TTTV — record có TREATMENT_END_TYPE_ID **không** thuộc config nặng xin về | long treatmentId |
| HIS.Desktop.Plugins.InformationAllowGoHome | Click cột TTTV — record có TREATMENT_END_TYPE_ID **thuộc** config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` | long treatmentId |

## 7. Print
Plugin này không có chức năng in trực tiếp — chỉ đồng bộ XML.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/05/2026 | phuongnm | Bổ sung hỗ trợ "Bệnh nặng xin về" (tài liệu 2538): (1) Filter rename "Ngày tử vong" → "Ngày tử vong/xin về"; (2) Grid rename "Thời gian tử vong" → "Thời gian tử vong/xin về", "Trạng thái tử vong" → "Trạng thái đồng bộ ca tử vong"; (3) Thêm cột "Loại ra viện" (TREATMENT_END_TYPE_NAME) sau cột Giới tính; (4) Thêm checkbox "Kết quả nặng xin về" trong group Trạng thái — lọc bản ghi có TREATMENT_END_TYPE_ID thuộc danh sách map từ config MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES; (5) ControlState lưu trạng thái checkbox mới. Phụ thuộc backend: HisTreatmentView11Filter cần có property List<long> TREATMENT_END_TYPE_IDs (spec 2.1). |
| 22/05/2026 | phuongnm | Bổ sung phân nhánh icon TTTV + đồng bộ ngữ nghĩa "tử vong/xin về": (1) `gridView1_RowCellClick` route icon sửa theo `TREATMENT_END_TYPE_ID` — thuộc config MUST_INPUT_SEVERE_ILLNESS_HOME_CODES → mở `HIS.Desktop.Plugins.InformationAllowGoHome` (Phiếu tóm tắt thông tin bệnh nặng xin về), ngược lại giữ luồng cũ mở `HIS.Desktop.Plugins.HisDeathInfo`; tạo `ModuleLinkString.cs` cho 2 plugin đích; (2) Rename cột grid: "Trạng thái đồng bộ ca tử vong" → "Trạng thái đồng bộ ca tử vong/xin về" (gridColumn20), "Thời gian đồng bộ ca tử vong" → "Thời gian đồng bộ ca tử vong/xin về" (gridColumn21), "Lý do đồng bộ ca tử vong" → "Lý do đồng bộ ca tử vong/xin về" (gridColumn22); (3) Rename filter group: navBarGroup5 "Trạng thái" → "Phân loại", navBarGroup6 "Trạng thái ca tử vong" → "Trạng thái ca tử vong/xin về"; (4) Rename button `btnDongBoCTV` "Đồng bộ ca tử vong" → "Đồng bộ ca tử vong/xin về". |

## 9. Test Cases

### Filter
- [ ] Mặc định: hiện tất cả hồ sơ có ca tử vong hoặc bệnh nặng xin về theo ngày tử vong/xin về tháng hiện tại.
- [ ] Tick "Kết quả tử vong" → chỉ hiện hồ sơ có TREATMENT_RESULT_ID = CHET.
- [ ] Tick "Loại ra viện tử vong" → chỉ hiện hồ sơ có TREATMENT_END_TYPE_ID = CHET.
- [ ] Tick "Kết quả nặng xin về" (config MUST_INPUT_SEVERE_ILLNESS_HOME_CODES có khai báo) → chỉ hiện hồ sơ có TREATMENT_END_TYPE_ID nằm trong danh sách map từ config.
- [ ] Tick "Kết quả nặng xin về" khi config rỗng → grid trống (không match record nào).
- [ ] Filter "Ngày cấp giấy" → bỏ qua bản ghi nặng xin về (vì không có DEATH_ISSUED_DATE).

### Grid
- [ ] Cột "Loại ra viện" hiển thị đúng tên TREATMENT_END_TYPE_NAME của hồ sơ.
- [ ] Cột "Thời gian tử vong/xin về" hiển thị DEATH_TIME định dạng yyyyMMddHHmm.
- [ ] Cột "Trạng thái đồng bộ ca tử vong" hiển thị "Thành công"/"Thất bại"/"Null"/trống.

### Đồng bộ
- [ ] Chọn dòng → bật 2 nút đồng bộ.
- [ ] "Đồng bộ ca tử vong" gọi đúng API, BE xử lý XML theo KQĐT của hồ sơ.

### ControlState
- [ ] Tick checkbox bất kỳ → đóng UC → mở lại → vẫn giữ tick.
