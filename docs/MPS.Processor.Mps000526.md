# MPS000526 — Giấy Khám Sức Khỏe Và Khám Sức Khỏe Định Kỳ Dùng Cho Người Từ Đủ 18 Tuổi Trở Lên — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Processor | `MPS.Processor.Mps000526` (+ `MPS.Processor.Mps000526.PDO`) |
| Mẫu in | "Mẫu 3" của SYT TP.HCM — Giấy khám sức khỏe và khám sức khỏe định kỳ dùng cho người từ đủ 18 tuổi trở lên (thân mẫu ghi "Mẫu 03 - Thông tư 25/2026/TT-BYT"), A4 dọc, 8 trang |
| Engine | Excel FlexCel (`Inventec.Common.FlexCellExport`) |
| Gọi từ | `HIS.Desktop.Plugins.EnterKskInfomantionVer2` — nút In ở tab "Ksk trên 18 tuổi" khi hồ sơ cổng SYT TP.HCM không phải người cao tuổi |
| Trạng thái | Đang phát triển (việc 60376, BV Xuyên Á Củ Chi) |

Anh em của `Mps000523` (Mẫu 4, người cao tuổi — `docs/MPS.Processor.Mps000523.md`): cùng nguồn dữ liệu đã lưu của màn Nhập thông tin KSK (HIS_KSK_OVER_EIGHTEEN, HIS_KSK_SYT_HCM, HIS_KSK_GENERAL, HIS_DHST, HIS_PERIOD_DRIVER_DITY), cùng cách đọc cột theo tên. Không thêm bảng/cột, không đổi BE.

## 2. Dữ Liệu Đầu Vào (PDO)

`Mps000526PDO(V_HIS_SERVICE_REQ, HIS_KSK_OVER_EIGHTEEN, HIS_KSK_SYT_HCM, HIS_KSK_GENERAL, HIS_DHST, V_HIS_TREATMENT_4, List<HIS_HEALTH_EXAM_RANK>)` và các thuộc tính plugin gán thêm:

| Thuộc tính | Nội dung |
|------------|----------|
| `KskServiceReq`, `KskPatient` | Y lệnh KSK / bệnh nhân (CCCD, hộ chiếu, điện thoại...) |
| `PersonalHistories` | 22 dòng tiền sử bản thân = `HIS_DISEASE_TYPE` (IS_KSK_OVER_EIGHTEEN) theo mã + câu trả lời ĐÃ LƯU (`HIS_PERIOD_DRIVER_DITY.IS_YES_NO` "1"/"0") |
| `ResolvedNames` | Tên hiển thị của các giá trị là mã danh mục cổng (nơi công tác, nguồn chi trả, hình thức chi trả, địa điểm khám, đề nghị, tiền sử gia đình, tình trạng răng) — khóa = tên trường lưu mã |
| `EmployeeNames` | LOGINNAME → họ tên các bác sĩ có trong hồ sơ |
| `ClsValues` | Giá trị cận lâm sàng theo trường của cổng (xnm_*, shm_*, xnnt_*, 4 chỉ tiêu tầm soát nữ) qua bảng nối CLS của module Đồng bộ KSK trên máy trạm: giá trị chỉ số xét nghiệm, hoặc kết quả dịch vụ (kết luận, không có thì mô tả) với chỉ tiêu nối theo dịch vụ |
| `PatientTypesText` | "13. Người lao động; ..." |

Không có `InterviewAnswers` (mẫu này không có phần hỏi bệnh B, D1–D8); nơi công tác vẫn lấy từ `INTERVIEW_JSON` qua `ResolvedNames`.

## 3. Key Đổ Vào Template (389 key trong mẫu)

| Nhóm | Key |
|------|-----|
| Hành chính | `KSK_NUMBER`, `PATIENT_NAME_UPPER`, `GENDER_MALE_X/FEMALE_X`, `DOB_STR`, `ETHNIC_NAME`, `BLOOD_TYPE`, `ID_1..12` (hoặc `ID_NUMBER_LONG` khi > 12 ký tự), `HEIN_1..15` (hoặc `HEIN_CARD_NUMBER_LONG`), `ADDRESS_DETAIL`, `ADDRESS_COMMUNE`, `CAREER_NAME`, `WORK_PLACE`, `WORK_PLACE_COMMUNE`, `PHONE`, `EXAM_REASON` |
| Tiền sử | `FH_NO_X/FH_YES_X/FH_TEXT`, `PH_01..22_CO_X/_KHONG_X/_NAME`, `PH_22_TEXT`, `TREATING_*`, `MATERNITY_*` |
| Đối tượng – chi trả | `SYT_PATIENT_TYPE_TEXT`, `PAY_NSNN_X/CONTRACT_X/NON_BORDER_X/EMPLOYER_X/SELF_X/OTHER_X`, `PAY_OTHER_TEXT`, `PLACE_FACILITY_X/MOBILE_X/STATION_X/SCHOOL_X/HOME_X` |
| Khám thể lực | `HEIGHT_STR`, `WEIGHT_STR`, `BMI_STR`, `PULSE_STR`, `BP_STR` (`BP_MAX_STR`, `BP_MIN_STR`), `BREATH_RATE_STR`, `PHYSIQUE_1..5_X` |
| Nội khoa (8 mục) | `{TUANHOAN|HOHAP|TIEUHOA|THANTN|NOITIET|COXK|THANKINH|TAMTHAN}_NORMAL_X/_PRE_X/_PRE_ICD/_FINAL_X/_FINAL_ICD/_RANK_1..5_X/_DOCTOR` |
| Chuyên khoa 2–7 | `{NGOAI|DALIEU|SAN|MAT|TMH|RHM}_NORMAL_X/_PRE_X/_PRE_ICD/_FINAL_X/_FINAL_ICD/_RANK_1..5_X/_DOCTOR`, `SAN_REFUSE_X`, `EYE_*`, `ENT_*`, `TOOTH_xx_STR` |
| Cận lâm sàng | `CLS_RBC`... (huyết học), `CLS_GLU_FASTING`, `CLS_GLU_RANDOM`, `CLS_UREA`, `CLS_CREATININE`, `CLS_AST`, `CLS_ALT`, `CLS_URINE_*`, `CLS_BLOOD_GENERAL`, `CLS_URINE_GENERAL`, `CLS_DIIM_RESULT`, `CLS_OTHER_NO_X/YES_X`, `CLS_OTHER_RESULT`, `CLS_PERIODIC_RESULT`, `CLS_PAP`, `CLS_HPV`, `CLS_MAMMO`, `CLS_BREAST_US`, `CLS_*_DOCTOR` |
| Kết luận | `CONCL_NORMAL_X/PRE_X/PRE_ICD/FINAL_X/FINAL_ICD`, `HEALTH_RANK_1..5_X`, `SUGGEST_1..5_X`, `SUGGEST_OTHER_TEXT`, `CONCLUSION_DAY/MONTH/YEAR`, `CONCLUDER_USERNAME` |
| Cột gốc | Tiền tố `SREQ_`, `PATIENT_`, `DHST_`, `KSK_`, `HCM_`, `GENERAL_`, `TREATMENT_` + tên cột (viện tự thêm vào mẫu) |

Ô tích là ô có viền chứa `<#..._X;>` = "X" hoặc rỗng (không dùng ký tự đặc biệt — an toàn khi xuất PDF/ký EMR).

## 4. Quy Tắc In

- **Nội khoa in riêng 8 mục** a) Tuần hoàn … h) Tâm thần: mỗi mục tự có "Chưa phát hiện bất thường" (đã khám bình thường và không có ICD), ICD sơ bộ / xác định, phân loại và tên bác sĩ của đúng mục đó (Mps000523 gộp 8 mục làm một).
- **Nguồn chi trả**: có ô riêng "Người sử dụng lao động chi trả" (`PAY_EMPLOYER_X`) — nguồn này không rơi vào "Nguồn khác"; tên nguồn không nhận ra → tích "Nguồn khác" và in tên.
- **Địa điểm khám**: theo tên — lưu động / trạm y tế / trường học / tại nhà; tên khác → CS KBCB.
- **Thể lực**: BMI lấy `HIS_DHST.VIR_BMI`, không có thì cân nặng / (chiều cao m)²; huyết áp một ô "tối đa/tối thiểu", chỉ có một số thì in số đó.
- **Hai khối cận lâm sàng** ("1. Khám phân loại sức khỏe để đi học, đi làm" và "2. Khám sức khỏe định kỳ") dùng CHUNG một bộ key và mẫu lặp thẻ ở cả hai khối: HIS không có cờ phân biệt hồ sơ thuộc khối nào, lúc đẩy cổng cũng điền hai bộ chỉ tiêu giống nhau. Urê chỉ có ở khối 1 (giấy không có dòng Urê ở khối 2).
- **Cận lâm sàng khác** (khối 1): ô "KQ khám CLS khác" có chữ → `CLS_OTHER_YES_X` + chữ; trống → `CLS_OTHER_NO_X`. Mục "KHÁM SỨC KHỎE ĐỊNH KỲ — Kết quả / Người khám" của màn hình → `CLS_PERIODIC_RESULT` / `CLS_PERIODIC_DOCTOR` (mục d khối 2).
- **Tầm soát cho nữ** (`CLS_PAP`, `CLS_HPV`, `CLS_MAMMO`, `CLS_BREAST_US`): lấy theo bảng nối CLS (dòng loại "DV": kết luận của dịch vụ, không có kết luận thì mô tả; dịch vụ đã thực hiện đầu tiên có kết quả), in dạng "tên xét nghiệm: kết quả"; chưa có kết quả hoặc bệnh nhân nam → để trống.
- **Hạn chế đã biết**: màn khai báo bảng nối CLS (module KskSyncList) chỉ cho chọn dịch vụ thuộc loại có tên chứa "siêu âm", "phẫu thuật", "thủ thuật". Dịch vụ X-quang nhũ thuộc loại khác (ví dụ "Chẩn đoán hình ảnh") thì không nối được, dòng X-quang nhũ để trống — cả trên giấy lẫn khi đẩy cổng. Mở rộng danh sách loại là việc của module KskSyncList, cần BA quyết.
- **Giữ nguyên như Mps000523**: sản phụ khoa (nam trống, quy tắc "Từ chối"), răng (mã cổng → số 0–9), khúc xạ, xét nghiệm (ô của màn KSK trước, rồi bảng nối CLS; không có giá trị nào thì in ghi chú tổng hợp), dòng 22 tiền sử, ô CCCD / BHYT, kết luận, chỉ đọc bản đã lưu.
- `SetSingleKey` giữ giá trị **ghi đầu tiên**: key tính toán ghi trước các key cột gốc; hai bảng gốc có cột trùng tên thì key mang giá trị của bảng được ghi trước. Mẫu chỉ nên dùng các key liệt kê ở mục 3.

## 5. Mẫu

- File: `Mau3_GiayKSKNguoiTuDu18Tuoi_A4.xlsx`, sinh bằng `PTTK\60376\mau_in\TaoMauMps000526.cs` (`build <out.xlsx>` ghi kèm `<out>.keys.txt`; `preview <mẫu> <thư mục> [KEY<TAB>VALUE]` đổ giá trị ra PNG).
- Cùng quy ước với mẫu Mps000523: 40 cột, mỗi khối (từng mục nội khoa, chuyên khoa + ô chữ ký bác sĩ, từng mục CLS, kết luận) đánh dấu `KeepRowsTogether` rồi `AutoPageBreaks`; chiều cao dòng cố định nên ô chữ dài được cấp sẵn số dòng; **không dùng "co chữ cho vừa ô"** (FlexCel 5.7.6 không vẽ ShrinkToFit).
- Triển khai: `x64\Tmp\Mps\Mps000526\Mau3_GiayKSKNguoiTuDu18Tuoi_A4.xlsx`. Viện có thể sửa chữ/bố cục trong Excel, giữ nguyên các thẻ `<#...;>`; thẻ nằm trong vùng gộp ô phải ở ô đầu tiên của vùng. Viện muốn kết quả xét nghiệm chỉ in ở một khối thì xóa thẻ ở khối còn lại.
- Cần dòng SAR_PRINT_TYPE `Mps000526` (`PTTK\60376\trien_khai\60376_SAR_PRINT_TYPE_Mps000526.sql`), đăng nhập lại HIS sau khi thêm.

## 6. Build & Deploy

- Build PDO trước rồi processor (ProjectReference PDO). PDO dùng để build plugin nằm ở `lib\MPSv2\MPS.PDO\MPS.Processor.Mps000526.PDO.dll` (repo lib): sửa PDO thì phải đưa bản mới vào lib, nếu không máy khác build EnterKskInfomantionVer2 theo bản cũ.
- Deploy: processor → `x64\Plugins\MpsProcessor\MPS.Processor.Mps000526.dll`; PDO → `x64\ReferencedAssemblies\MPS.Processor.Mps000526.PDO.dll`; mẫu như mục 5. Khởi động lại HIS.
- Namespace phải đúng `MPS.Processor.Mps000526` (ProcessorFactory lấy mã in từ namespace).

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 13/11/2026 | dangth | **Việc 60376** — Tạo mới từ Mps000523: PDO + processor đổ key theo Mẫu 3 SYT TP.HCM (hành chính, tiền sử, đối tượng – chi trả có ô người sử dụng lao động và địa điểm trường học, khám thể lực có BMI, 8 mục nội khoa in riêng, 6 chuyên khoa, hai khối cận lâm sàng có Urê / CLS khác Có-Không / kết quả KSK định kỳ / tầm soát nữ, kết luận); mẫu A4 sinh bằng FlexCel, ngắt trang theo khối |

## 8. Test Cases

Harness `PTTK\60376\harness_test\chay_harness.ps1`: chạy plugin + processor + mẫu thật trên hồ sơ của CSDL test và các ca biên tự dựng, xuất ảnh từng trang; kiểm cấu trúc mẫu (thẻ ở ô đầu vùng gộp, số dòng tối thiểu của ô chữ dài, không ô nào dựa vào ShrinkToFit, hai khối cận lâm sàng cùng thẻ, số trang dựng = số ngắt cố định + 1); phần lấy kết quả dịch vụ theo bảng nối CLS chạy với máy chủ MOS giả trên 127.0.0.1 (kiểm cả bộ lọc gửi lên). Kết quả 14/11/2026 trên bản build 13/11 19:45: 524 đạt / 0 trượt; harness của Mps000523 chạy lại trên cùng plugin: 842 đạt / 0 trượt.

- [ ] Hồ sơ đủ dữ liệu: đúng ô tích, tên bác sĩ hiện cạnh từng mục nội khoa và từng chuyên khoa
- [ ] Nguồn chi trả "Người sử dụng lao động" → ô riêng; địa điểm "Trường học" → ô Trường học
- [ ] BMI khớp màn hình; huyết áp "120/80"; thiếu một số thì không in dấu "/" trơ trọi
- [ ] Nam: tiền sử thai sản, sản phụ khoa, 4 dòng tầm soát nữ trống
- [ ] Có bảng nối CLS: giá trị XN in đúng dòng ở cả hai khối; không có: in ghi chú tổng hợp của màn hình
- [ ] "KQ khám CLS khác" có chữ → Có + chữ; trống → Không; kết quả KSK định kỳ in ở mục d) khối 2
- [ ] CCCD 12 số vào 12 ô; hộ chiếu dài hơn 12 ký tự in liền một dòng
- [ ] ICD dài, tên bác sĩ dài → đủ chữ, không bị cắt
