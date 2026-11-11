# MPS000523 — Giấy Khám Sức Khỏe Định Kỳ Cho Người Cao Tuổi — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000523` (+ `MPS.Processor.Mps000523.PDO`) |
| Mẫu in | "Mẫu 4" của SYT TP.HCM — Giấy khám sức khỏe định kỳ cho người cao tuổi (thân mẫu ghi "Mẫu 03 – Thông tư 25/2026/TT-BYT"), A4 dọc, 9 trang |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Gọi từ | `HIS.Desktop.Plugins.EnterKskInfomantionVer2` — nút In ở tab "Ksk trên 18 tuổi" khi hồ sơ là M4 người cao tuổi |
| Trạng thái | Đang phát triển (việc 60377, BV Xuyên Á Củ Chi) |

Dữ liệu lấy hết từ những gì màn Nhập thông tin KSK đã lưu (HIS_KSK_OVER_EIGHTEEN, HIS_KSK_SYT_HCM, HIS_KSK_GENERAL, HIS_DHST, HIS_PERIOD_DRIVER_DITY). Không thêm bảng/cột, không đổi BE.

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000523PDO(V_HIS_SERVICE_REQ, HIS_KSK_OVER_EIGHTEEN, HIS_KSK_SYT_HCM, HIS_KSK_GENERAL, HIS_DHST, V_HIS_TREATMENT_4, List<HIS_HEALTH_EXAM_RANK>)` và các thuộc tính plugin gán thêm:

| Thuộc tính | Nội dung |
|------------|----------|
| `KskServiceReq`, `KskPatient` | Y lệnh KSK / bệnh nhân (CCCD, hộ chiếu, điện thoại...) |
| `PersonalHistories` | 22 dòng tiền sử bản thân = `HIS_DISEASE_TYPE` (IS_KSK_OVER_EIGHTEEN) theo mã + câu trả lời ĐÃ LƯU (`HIS_PERIOD_DRIVER_DITY.IS_YES_NO` "1"/"0") |
| `InterviewAnswers` | `HIS_KSK_SYT_HCM.INTERVIEW_JSON` (tab "Hỏi bệnh lâm sàng HCM") dạng khóa → chuỗi |
| `ResolvedNames` | Tên hiển thị của mọi giá trị là mã danh mục cổng (tần suất D6/D7, suy yếu D8.3.1, CS KBCB D1, nơi công tác, nguồn chi trả, hình thức chi trả, địa điểm khám, đề nghị, tiền sử gia đình, tình trạng răng) — khóa = tên trường lưu mã |
| `EmployeeNames` | LOGINNAME → họ tên các bác sĩ có trong hồ sơ |
| `ClsValues` | Giá trị XN theo trường của cổng (xnm_*, shm_*, xnnt_*) qua bảng nối CLS của module Đồng bộ KSK trên máy trạm |
| `PatientTypesText` | "5. Người cao tuổi; ..." |

Processor đọc cột của HIS_KSK_SYT_HCM / HIS_KSK_GENERAL **theo tên** (`Mps000523Util.Col/Str/Long`) để MOS.EFMODEL cũ hơn ở viện không làm hỏng cả phiếu.

## 3. Key Đổ Vào Template (506 key)

| Nhóm | Key |
|------|-----|
| Hành chính | `KSK_NUMBER`, `PATIENT_NAME_UPPER`, `GENDER_MALE_X/FEMALE_X`, `DOB_STR`, `ETHNIC_NAME`, `BLOOD_TYPE`, `ID_1..12` (hoặc `ID_NUMBER_LONG` khi > 12 ký tự), `HEIN_1..15` (hoặc `HEIN_CARD_NUMBER_LONG`), `ADDRESS_DETAIL`, `ADDRESS_COMMUNE`, `CAREER_NAME`, `WORK_PLACE`, `WORK_PLACE_COMMUNE`, `PHONE`, `EXAM_REASON` |
| Tiền sử | `FH_NO_X/FH_YES_X/FH_TEXT`, `PH_01..22_CO_X/_KHONG_X/_NAME`, `PH_22_TEXT`, `TREATING_*`, `MATERNITY_*` |
| Đối tượng – chi trả | `SYT_PATIENT_TYPE_TEXT`, `PAY_NSNN_X/CONTRACT_X/NON_BORDER_X/SELF_X/OTHER_X`, `PAY_OTHER_TEXT`, `PLACE_FACILITY_X/MOBILE_X/STATION_X/HOME_X` |
| B, C | `B1_NO_X/B1_YES_X`, `B1_1..7_X`, `HEIGHT_STR`, `WEIGHT_STR`, `WEIGHT_LAST_YEAR_STR`, `WAIST_STR`, `PULSE_STR`, `BP_MAX_STR`, `BP_MIN_STR`, `BREATH_RATE_STR`, `PHYSIQUE_1..5_X` |
| D1–D8 | `D1_NO_X/YES_X`, `D1_n_X/_PLACE`, `D1_OTHER_*`; `D2..D5_n_CO_X/_KHONG_X`; `D6/D7_n_L1..L4_X`; `D8_1_n`, `D8_2_n`, `D8_3_1_A_X/B_X`, `D8_3_2/4`, `D8_4_1/2/4`, `D8_5_1..3` (`_CO_X/_KHONG_X`) |
| Chuyên khoa | `{NOI|NGOAI|DALIEU|SAN|MAT|TMH|RHM}_NORMAL_X/_PRE_X/_PRE_ICD/_FINAL_X/_FINAL_ICD/_RANK_1..5_X/_DOCTOR`, `SAN_REFUSE_X`, `EYE_*`, `ENT_*`, `TOOTH_xx_STR` |
| Cận lâm sàng | `CLS_RBC`... (huyết học), `CLS_GLU_FASTING`, `CLS_GLU_RANDOM`, `CLS_CREATININE`, `CLS_AST`, `CLS_ALT`, `CLS_URINE_*`, `CLS_BLOOD_GENERAL`, `CLS_URINE_GENERAL`, `CLS_DIIM_RESULT`, `CLS_OTHER_RESULT`, `CLS_*_DOCTOR` |
| Kết luận | `HEALTH_RANK_1..5_X`, `CONCL_NORMAL_X/PRE_X/PRE_ICD/FINAL_X/FINAL_ICD`, `SUGGEST_1..5_X`, `SUGGEST_OTHER_TEXT`, `CONCLUSION_DAY/MONTH/YEAR`, `CONCLUDER_USERNAME` |
| Cột gốc | Tiền tố `SREQ_`, `PATIENT_`, `DHST_`, `KSK_`, `HCM_`, `GENERAL_`, `TREATMENT_` + tên cột (viện tự thêm vào mẫu) |

Ô tích là ô có viền chứa `<#..._X;>` = "X" hoặc rỗng (như Mps000516/Mps000520, không dùng ký tự đặc biệt — an toàn khi xuất PDF/ký EMR).

## 4. Quy Tắc In

- **Chưa hỏi bệnh** (JSON chỉ có `_v`, nơi công tác, cân nặng năm trước) → mọi ô Có/Không của B1, D1–D8 để trống, không in "Không".
- **Bỏ qua theo mẫu**: D1.2 Có → bỏ D2; D1.3 → D3; D1.4 → D4; D1.12 → D6; D1.13 → D7; D1.14 → D8.5; D8.5.1 Không → bỏ D8.5.2–3. D5.11 (chảy máu âm đạo) để trống với nam.
- **Nội khoa** gộp 8 phần của tab HCM (tuần hoàn, hô hấp, tiêu hóa, thận-tiết niệu, nội tiết, cơ xương khớp, thần kinh, tâm thần): "Chưa phát hiện bất thường" chỉ khi mọi phần đã khám đều bình thường và không có ICD; ICD sơ bộ/xác định gộp không trùng; phân loại = mức nặng nhất; bác sĩ = các họ tên không trùng.
- **Sản phụ khoa**: nam → trống cả khối; "Từ chối" khi có cờ từ chối và phần không từ chối cũng không có kết quả.
- **Răng**: mã cổng → tên → số 0–9 của mẫu; mã HIS cũ 1–10 của màn hình được đổi tương ứng; "Không ghi nhận" để trống.
- **CLS**: ô của màn KSK trước, rồi bảng nối CLS; không có giá trị nào thì in ô ghi chú tổng hợp (huyết học/nước tiểu) của màn hình. d) = Kết quả CLS khác + Kết quả CLS định kỳ.
- **Tiền sử dòng 22 "Bệnh khác (ghi rõ)"**: chữ ghi rõ (tiền sử bản thân + ICD tiền sử) chỉ in khi dòng 22 = Có — cùng quy tắc Mps000452.
- **Nguồn chi trả**: tên nguồn không nhận ra → tích "Nguồn khác" và in tên sau đó, kể cả khi hình thức chi trả là hợp đồng / phi địa giới (ô của hình thức chi trả tích riêng).
- **Khúc xạ**: giữ vị trí "cầu – trụ – trục", phần trống in "…"; trống cả ba thì để trống.
- **Chỉ đọc lại bản đã lưu** (kể cả kết luận: không phủ ICD đang sửa trên màn hình như Mps000452; DHST gọi lại `api/HisDhst/Get`) — nếu lưu tab HCM thất bại thì phiếu in theo dữ liệu cũ.
- `SetSingleKey` giữ giá trị **ghi đầu tiên**: quyết định để trống phải nằm trong lần ghi đầu (đã gặp lỗi D5.11 với nam khi xóa sau).

## 5. Mẫu

- File: `Mau4_GiayKSKDinhKyNguoiCaoTuoi_A4.xlsx` (345 dòng, 506 key), sinh bằng `PTTK\60377\mau_in\TaoMauMps000523.cs` (`build <out.xlsx>` ghi kèm `<out>.keys.txt`; `preview <mẫu> <thư mục> [KEY<TAB>VALUE]` đổ giá trị ra PNG).
- 40 cột × 4,625 mm, lề 15/10/10/12 mm. Mỗi khối (chuyên khoa + ô chữ ký bác sĩ, từng mục CLS, kết luận...) đánh dấu `KeepRowsTogether` rồi `AutoPageBreaks` → 9 trang, ngắt cố định, không cắt ngang khối; trang 1 chứa trọn bảng 22 tiền sử (bảng chỉ giữ cụm tiêu đề + 2 dòng đầu, phần còn lại ngắt theo dòng như Word).
- Chiều cao dòng cố định (FlexCel không giãn ô gộp): ô chữ dài được cấp sẵn số dòng — địa chỉ / nơi làm việc / lý do khám / đối tượng / nguồn khác 2 dòng, ICD chuyên khoa 3 (Nội khoa 4), ICD kết luận 4, đang điều trị 4, tiền sử gia đình / bệnh khác D1 / đề nghị khác 3, ghi chú huyết học / nước tiểu 3, CĐHA và CLS khác 6, tên bác sĩ ≥ 2 dòng ở đáy ô ký. Chữ ghi rõ dòng 22 in ở hàng rộng hết bảng ngay dưới STT 22 (2 dòng, ~250 ký tự). Cột "CS KBCB đang điều trị" của D1 ghi nhãn một lần ở dòng tiêu đề, mỗi dòng còn 23 cột (~70 ký tự cỡ 10) cho tên cơ sở. Chữ dài hơn vẫn bị cắt.
- **FlexCel 5.7.6 (máy in/PDF của HIS) không vẽ ShrinkToFit** — không dùng "co chữ cho vừa ô", phải cấp dòng/cột.
- Triển khai: `x64\Tmp\Mps\Mps000523\Mau4_GiayKSKDinhKyNguoiCaoTuoi_A4.xlsx`. Viện có thể sửa chữ/bố cục trong Excel, giữ nguyên các thẻ `<#...;>`; thẻ nằm trong vùng gộp ô phải ở ô đầu tiên của vùng (thẻ ở ô khác không hiện).
- Cần dòng SAR_PRINT_TYPE `Mps000523` (`PTTK\60377\trien_khai\60377_SAR_PRINT_TYPE_Mps000523.sql`), đăng nhập lại HIS sau khi thêm.

## 6. Build & Deploy

- Build PDO trước rồi processor (ProjectReference PDO). PDO chưa có trong `lib\MPSv2\MPS.PDO`: máy khác build EnterKskInfomantionVer2 cần PDO được đưa vào lib.
- Deploy: processor → `x64\Plugins\MpsProcessor\MPS.Processor.Mps000523.dll`; PDO → `x64\ReferencedAssemblies\MPS.Processor.Mps000523.PDO.dll`; mẫu như mục 5. Khởi động lại HIS.
- Namespace phải đúng `MPS.Processor.Mps000523` (ProcessorFactory lấy mã in từ namespace).

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 10/11/2026 | dangth | **Việc 60377** — Tạo mới: PDO + processor đổ 506 key theo Mẫu 4 SYT TP.HCM (hành chính, tiền sử, đối tượng – chi trả, B/C/D1–D8, 7 chuyên khoa, CLS, kết luận); mẫu A4 sinh bằng FlexCel, ngắt trang theo khối |
| 10/11/2026 | dangth | **Việc 60377 — sửa theo phản biện độc lập**: nguồn chi trả lạ vẫn tích "Nguồn khác" khi có hình thức chi trả; chữ dòng 22 tiền sử chỉ khi dòng 22 = Có; khúc xạ giữ vị trí 3 phần; mẫu cấp đủ dòng cho các ô chữ dài và tên bác sĩ (9 trang) |
| 11/11/2026 | dangth | **Việc 60377 — sửa theo phản biện vòng 2**: bỏ "co chữ" (FlexCel 5.7.6 không vẽ) — địa chỉ / nơi làm việc / lý do khám / đối tượng / nguồn khác 2 dòng, nhãn "CS KBCB đang điều trị" của D1 ghi một lần để tên cơ sở có 23 cột, chữ ghi rõ dòng 22 sang hàng rộng hết bảng; chú thích HEIN_CARD_NUMBER_LONG |

## 8. Test Cases

Harness `PTTK\60377\harness_test\chay_harness.ps1` (847 ca): chạy plugin + processor + mẫu thật trên 5 hồ sơ M4 của CSDL test và các ca biên tự dựng, xuất ảnh từng trang; kiểm cấu trúc mẫu (thẻ ở ô đầu vùng gộp, số dòng tối thiểu của 42 ô chữ dài, không ô nào dựa vào ShrinkToFit, ô tên cơ sở D1 ≥ 20 cột, số trang dựng = số ngắt cố định + 1).

- [ ] Hồ sơ đủ dữ liệu: đúng ô tích, tên bác sĩ hiện cạnh từng chuyên khoa
- [ ] Hồ sơ chưa hỏi bệnh (`{"_v":1}`): mục B1, D1–D8 để trống
- [ ] D1.2 = Có → D2 trống; D8.5.1 = Không → D8.5.2–3 trống
- [ ] Nam: D5.11, tiền sử thai sản, sản phụ khoa trống
- [ ] CCCD 12 số vào 12 ô; hộ chiếu dài hơn 12 ký tự in liền một dòng
- [ ] Có bảng nối CLS: giá trị XN in đúng dòng; không có: in ghi chú tổng hợp của màn hình
- [ ] Dòng 22 tiền sử = Không mà có ghi tiền sử bản thân → không in chữ ở dòng 22
- [ ] Nội khoa có 2–3 bác sĩ, ICD dài → đủ chữ, tên bác sĩ không bị cắt
- [ ] Địa chỉ đủ số nhà – ấp – xã – huyện – tỉnh, tên cơ sở điều trị D1 dài ("Bệnh viện Đại học Y Dược Thành phố Hồ Chí Minh - Cơ sở 2"), 3 loại đối tượng → đủ chữ
