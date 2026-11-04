# PHÂN TÍCH YÊU CẦU

## Chữ ký bệnh nhân / người nhà tuân theo thiết lập "Ký nhiều lần" của loại văn bản

| Thông tin | Nội dung |
|---|---|
| Danh mục liên quan | Loại văn bản (HIS.Desktop.Plugins.EmrDocumentType) — cờ `EMR_DOCUMENT_TYPE.IS_MULTI_SIGN` |
| Chức năng ký | Danh sách văn bản (EMR.Desktop.Plugins.EmrDocumentList) → nút **Bệnh nhân ký**; màn ký của thư viện ký (`Inventec.Common.SignLibrary`, nút **Bệnh nhân ký** / **Người nhà ký**) |
| Thành phần phải sửa | Backend EMR (`E:\svn\IMSys\BACKEND\EMR`) · Thư viện ký `common\SHAREUTIL\Inventec.Common\Inventec.Common.SignLibrary` · Cấu hình hệ thống (SDA) |
| Không phải sửa | Plugin EmrDocumentType (cờ "Ký nhiều lần" đã có, giữ nguyên); EFMODEL (không thêm cột) |
| Trạng thái | **Đã lập trình** (04/11/2026). Đã chốt: **KHÔNG thêm cấu hình** — chữ ký BN tuân theo "Ký nhiều lần" của loại văn bản cho toàn viện. Các mục 3.2, khóa cấu hình ở 3.1/3.3/3.4 và câu C1 **không còn áp dụng**. Tài liệu nghiệp vụ: `A_NghiepVu_EmrSign_BenhNhanKyNhieuLan.docx` |
| Ngày | 04/11/2026 |

> **Cập nhật 04/11/2026 — thiết kế đã thực hiện**
> - Quy tắc: `IS_SIGNING` của dòng BN = `IsSigning` client gửi; không gửi thì theo `EMR_DOCUMENT.IS_MULTI_SIGN` (giống nhân viên). Áp dụng cho cả 3 nhánh A/B/C. Tiện ích chung: `EMR.BusinessManager\DocumentBO\EmrSign\PatientSign\PatientSignUtil.cs`.
> - Phát hiện thêm khi lập trình: chữ ký điện tử của BN (bảng ký, máy tính bảng, vân tay) **không nhúng vào PDF**, chỉ lưu `SIGN_IMAGE` + một toạ độ trên dòng `EMR_SIGN` và vẽ lúc xem → ký lại sẽ ghi đè chữ ký cũ. Giải pháp: trước khi ghi đè, backend **lưu chữ ký cũ thành dòng `EMR_SIGN` lịch sử** (cùng `PATIENT_CODE`, cùng `NUM_ORDER`, `IS_SIGNING = null`); `EmrSignCheckVerifyDuplicate` bỏ qua trùng `NUM_ORDER` khi các dòng cùng một BN; chọn dòng ký BN ưu tiên dòng đang ký.
> - Thư viện ký: thông báo `VanBanDaDuocBenhNhanKy`; lần ký sau chỉ ký vào mốc chưa có chữ ký BN (`GetPatientAutoPositionsToSign`), áp dụng cả "Người nhà ký".
> - Nghi vấn 4.3 được xử lý luôn (backend nhánh A tôn trọng `IsSigning` client gửi cho các mốc trung gian).

---

# PHẦN 1. HIỆN TRẠNG THỰC TẾ TRONG CODE

## 1.1 Cờ "Ký nhiều lần" hoạt động thế nào với bác sĩ, điều dưỡng

1. Khi tạo văn bản, backend **chụp** cờ của loại văn bản vào văn bản: `EMR_DOCUMENT.IS_MULTI_SIGN = documentType.IS_MULTI_SIGN` ([EmrDocumentCreateBehaviorEv.cs:89](E:/svn/IMSys/BACKEND/EMR/EMR.BusinessManager/DocumentBO/EmrDocument/Create/EmrDocumentCreateBehaviorEv.cs#L89)). Đổi cờ trên loại văn bản về sau **không** ảnh hưởng tới văn bản đã tạo.
2. Thư viện ký đọc cờ (`UCViewer.cs:251-256`, `:851`) rồi gửi lên API `IsSigning = isMultiSign`, `IsFinishSign = !isMultiSign` (`SignHSMHandler.cs:194-195, 301-302, 394-395`; `SignUSBHandler.cs:116-117, 181-182`).
3. Backend lưu `EMR_SIGN.IS_SIGNING = 1` → dòng ký **vẫn mở**, người đó ký tiếp được. Khi bấm **Kết thúc ký** (`api/EmrSign/Finish`) → `IS_SIGNING = null`, dòng ký đóng.
4. Chốt chặn ký lần 2 nằm ở [EmrSignCheckVerifyIsUnSigned.cs:18](E:/svn/IMSys/BACKEND/EMR/EMR.CheckBusiness/EmrSign/EmrSignCheckVerifyIsUnSigned.cs#L18): dòng đã có `VERSION_ID` mà `IS_SIGNING ≠ 1` → báo *"Người ký đã ký văn bản"*.

## 1.2 Bệnh nhân / người nhà ký — đi qua 3 API, xử lý KHÔNG đồng nhất

Mỗi văn bản chỉ có **một dòng** `EMR_SIGN` mang `PATIENT_CODE` (frontend chặn thêm dòng thứ 2 — `FormEmrSign.cs:912`, `frmSignerAdd.cs:254/334`). Thư viện ký **đã gửi** `IsSigning = isMultiSign` cho cả bệnh nhân (`UCViewer.cs:1660`, `SignHandle.cs:398, 405`), nhưng backend xử lý mỗi API một kiểu:

| Nhánh | Khi nào đi nhánh này | API → Behavior backend | `IS_SIGNING` của dòng BN sau khi ký | Kết quả |
|---|---|---|---|---|
| **A** | `EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION` = 2/3 và (dùng bảng ký **hoặc** BN không ký bằng chứng thư) — tức là **bảng ký, máy tính bảng, vân tay** | `api/EmrSign/PatientAddAndSign` → `EmrSignByPatientBehaviorSDO` | **Gán cứng `null`** — comment "bệnh nhân ký 1 lần" ([EmrSignByPatientBehaviorSDO.cs:217-218](E:/svn/IMSys/BACKEND/EMR/EMR.BusinessManager/DocumentBO/EmrSign/PatientSign/EmrSignByPatientBehaviorSDO.cs#L217-L218)) | **Luôn ký 1 lần** — đây đúng là tình trạng bệnh viện phản ánh |
| **B** | Các trường hợp còn lại (BN ký bằng thẻ có chứng thư số) | `api/EmrSign/SignPdfHsm` → `EmrSignHsmPdfBehaviorSDO` (dùng chung với bác sĩ) | Lấy theo `IsSigning` client gửi ([EmrSignHsmPdfBehaviorSDO.cs:1148-1158](E:/svn/IMSys/BACKEND/EMR/EMR.BusinessManager/DocumentBO/EmrSign/SignPdf/EmrSignHsmPdfBehaviorSDO.cs#L1148-L1158)) | **Đã ký nhiều lần được** nếu loại văn bản tích "Ký nhiều lần" — không có cấu hình bật/tắt |
| **C** | Client gửi file PDF gốc (chưa thấy nơi gọi trong HIS Desktop, có thể là ứng dụng máy tính bảng/web) | `api/EmrSign/PatientAddAndSignPdf` → `EmrSignByPatientPdfBehaviorSDO` | Lấy theo `IsSigning` client gửi, dù comment ghi "bệnh nhân ký 1 lần" ([EmrSignByPatientPdfBehaviorSDO.cs:1173-1181](E:/svn/IMSys/BACKEND/EMR/EMR.BusinessManager/DocumentBO/EmrSign/PatientSign/EmrSignByPatientPdfBehaviorSDO.cs#L1173-L1181)) | Phụ thuộc client |

Nhánh dự phòng khi client **không** gửi `IsSigning` (USB/HSM/SimCA): backend chủ động loại BN khỏi ký nhiều lần bằng điều kiện `String.IsNullOrWhiteSpace(raw.PATIENT_CODE)` ([EmrSignUsbPdfBehaviorSDO.cs:297-300](E:/svn/IMSys/BACKEND/EMR/EMR.BusinessManager/DocumentBO/EmrSign/SignPdf/EmrSignUsbPdfBehaviorSDO.cs#L297-L300), tương tự `EmrSignHsmPdfBehaviorSDO.cs:1159-1162`, `EmrSignSimCaPdfBehaviorSDO.cs:857-860`).

**Kết luận:** quy tắc "BN ký 1 lần" không phải là quy tắc chung mà là **hành vi ngẫu nhiên theo hình thức ký**. Yêu cầu thực chất là **thống nhất** 3 nhánh về một quy tắc duy nhất, có cấu hình bật/tắt.

## 1.3 Những phần đã sẵn sàng, không phải làm lại

| Thành phần | Đã hỗ trợ dòng BN có `IS_SIGNING = 1` | Vị trí |
|---|---|---|
| Tính người ký kế tiếp | `NEXT_SIGNER` = `#@!@#` + mã BN khi dòng BN còn mở → nút **Bệnh nhân ký** trên Danh sách văn bản vẫn bật | `EmrDocumentUtil.cs:64-74`, `UCEmrDocumentList.cs:858-866` |
| Kiểm tra BN được ký | `EmrSignCheckVerifyPatientAllowSign` cho qua khi `NEXT_SIGNER` là BN | `EmrSignCheckVerifyPatientAllowSign.cs:20-22` |
| Chọn lại dòng ký của BN | `GetSignDocumentFirst` khi ký nhiều lần trả về dòng `IS_SIGNING = 1` | `SignLibrary\Api\EmrSign.cs:155-166` |
| Kết thúc ký | API `Finish` chỉ chặn khi dòng có `LOGINNAME` khác người đăng nhập; dòng BN không có `LOGINNAME` → nhân viên kết thúc thay được | `EmrSignFinishBehaviorId.cs:81`, `EmrSignCheckVerifyLoginname.cs:19` |

## 1.4 Thông báo hiện tại khi BN ký lần 2 (loại văn bản không ký nhiều lần)

- Frontend không tìm thấy dòng ký → báo *"Bắt buộc phải tạo luồng ký cho văn bản đã có trên hệ thống EMR..."* và **mở form thiết lập luồng ký** (`UCViewer.cs:1404-1432`). Thông báo này sai bản chất, người dùng dễ thêm luồng ký mới.
- Nếu lọt xuống backend → *"Người ký đã ký văn bản"* — không nói rõ là bệnh nhân.

→ Mong muốn *"phần mềm thông báo văn bản đã được bệnh nhân ký"* **chưa đáp ứng**, phải bổ sung.

## 1.5 Vị trí chữ ký của bệnh nhân

- Vị trí BN đánh dấu bằng ghi chú PDF có nội dung `#@!@#PATIENT` (`FileUtils.GetPdfPatientSignPosition`, `FileUtils.cs:721-848`); không có thì dùng mốc `$<thứ tự>`; không có mốc thì người dùng kéo khung trên PDF.
- Lần ký **đầu tiên**: ký vào **tất cả** các mốc `#@!@#PATIENT` trong một lần bấm (`UCViewer.cs:3158-3198`).
- Lần ký **thứ 2 trở đi**: điều kiện `signSelected.SIGN_TIME <= 0` tại `UCViewer.cs:3158` sai → **bỏ qua mốc tự động**, người dùng phải tự kéo khung. Nếu sửa điều kiện một cách đơn giản thì vòng lặp sẽ ký **đè lên tất cả các mốc cũ**. → Phải bổ sung logic "chọn mốc chưa ký".

---

# PHẦN 2. NGUYÊN NHÂN GỐC

| # | Nguyên nhân | Vị trí |
|---|---|---|
| 1 | Nhánh bảng ký / máy tính bảng / vân tay gán cứng `IS_SIGNING = null`, bỏ qua loại văn bản | `EmrSignByPatientBehaviorSDO.cs:218` |
| 2 | Các nhánh còn lại tin tuyệt đối vào `IsSigning` client gửi, không có quy tắc phía server cho BN | `EmrSignHsmPdfBehaviorSDO.cs:1148`, `EmrSignUsbPdfBehaviorSDO.cs:286`, `EmrSignSimCaPdfBehaviorSDO.cs`, `EmrSignByPatientPdfBehaviorSDO.cs:1174` |
| 3 | Không có cấu hình để viện chọn áp dụng | `EmrSignCFG` / `EmrConfigKeys.cs` chưa có khóa nào |
| 4 | Thông báo khi BN đã ký không đúng nghiệp vụ | `UCViewer.cs:1404-1432`, `EmrSignCheckVerifyIsUnSigned.cs` |
| 5 | Lần ký sau không tự định vị được mốc chữ ký BN tiếp theo | `UCViewer.cs:3158` |

---

# PHẦN 3. GIẢI PHÁP ĐỀ XUẤT

## 3.1 Quy tắc nghiệp vụ duy nhất (áp dụng cho mọi hình thức ký của BN/người nhà)

```
Cho phép BN ký nhiều lần  =  Cấu hình mới = 1
                           VÀ  EMR_DOCUMENT.IS_MULTI_SIGN = 1   (bản chụp từ loại văn bản lúc tạo)
                           VÀ  client không yêu cầu kết thúc ký (IsFinishSign = false)
```

| Cấu hình | Loại văn bản "Ký nhiều lần" | Hành vi BN/người nhà |
|---|---|---|
| 0 / chưa khai báo (mặc định) | bất kỳ | **Giữ nguyên như hiện nay** — xem điểm chốt C1 về nhánh B |
| 1 | Không tích | Ký 1 lần; lần sau báo **"Văn bản đã được bệnh nhân/người nhà ký"** |
| 1 | Có tích | Ký nhiều lần; mỗi lần ký giữ dòng ký mở (`IS_SIGNING = 1`) cho tới khi **Kết thúc ký** |

Bác sĩ, điều dưỡng: **không đổi** — điều kiện chỉ áp dụng khi `raw.PATIENT_CODE` có giá trị.

## 3.2 Cấu hình mới (SDA)

| Khóa | Giá trị | Mặc định | Đọc ở |
|---|---|---|---|
| `EMR.EMR_SIGN.PATIENT_SIGN.MULTI_SIGN_BY_DOCUMENT_TYPE.OPTION` *(tên chờ chốt)* | 1 = chữ ký BN/người nhà tuân theo "Ký nhiều lần" của loại văn bản; khác 1 = như hiện tại | 0 | Backend `EmrSignCFG`; thư viện ký `EmrConfigKeys` + `GlobalStore` (nạp tại `SignLibraryGUIProcessor.cs:450-529`) |

Backend là nơi **quyết định**; frontend đọc cấu hình chỉ để hiển thị nút **Kết thúc ký**, thông báo và chọn mốc chữ ký.

## 3.3 Backend EMR

| # | File | Sửa |
|---|---|---|
| B1 | `EMR.ConfigManager\...\EmrSignCFG.cs` | Thêm khóa cấu hình ở 3.2 |
| B2 | `EMR.BusinessManager\DocumentBO\EmrSign\` — thêm lớp tiện ích (VD `PatientMultiSignUtil.IsAllow(document, entity)`) | Hiện thực quy tắc 3.1, dùng chung cho mọi nhánh, tránh lặp code |
| B3 | `PatientSign\EmrSignByPatientBehaviorSDO.cs:218` (nhánh A) | Thay `raw.IS_SIGNING = null` bằng `IsAllow(...) ? 1 : null` |
| B4 | `PatientSign\EmrSignByPatientPdfBehaviorSDO.cs:1174-1181` (nhánh C) | Với dòng BN, dùng `IsAllow(...)` thay cho việc tin `IsSigning` |
| B5 | `SignPdf\EmrSignHsmPdfBehaviorSDO.cs:1148`, `EmrSignUsbPdfBehaviorSDO.cs:286`, `EmrSignSimCaPdfBehaviorSDO.cs` (nhánh B) | Khi `raw.PATIENT_CODE` có giá trị **và cấu hình = 1** → dùng `IsAllow(...)`. Cấu hình = 0 → giữ code cũ (xem C1) |
| B6 | `EMR.CheckBusiness\EmrSign\EmrSignCheckVerifyIsUnSigned.cs` | Dòng đã ký có `PATIENT_CODE` → trả thông báo mới `EmrSign__VanBanDaDuocBenhNhanKy` (thêm vào `EMR.LibraryMessage`, vi + en) thay cho "Người ký đã ký văn bản" |

Không đổi: `EmrDocumentUtil` (NEXT_SIGNER/UN_SIGNERS), `EmrSignCheckVerifyPatientAllowSign`, API `Finish`, EFMODEL, URI.

## 3.4 Thư viện ký `Inventec.Common.SignLibrary` (repo common)

| # | File | Sửa |
|---|---|---|
| F1 | `EmrConfigKeys.cs`, `GlobalStore`, `SignLibraryGUIProcessor.cs:450-529` | Nạp cấu hình mới |
| F2 | `Api\EmrSign.cs:155-166` (`GetSignDocumentFirst`, nhánh BN) | Không tìm được dòng ký nhưng **có dòng BN đã ký** → trả về trạng thái "đã ký" để UI báo đúng, thay vì null |
| F3 | `UCViewer.cs:1404-1432` (`VerifyWithExistsDocument`) | Trường hợp F2 → báo **"Văn bản đã được bệnh nhân {tên} ký"** (thêm vào `LibraryMessage` vi + en), **không** mở form thiết lập luồng ký |
| F4 | `UCViewer.cs:3158-3198` (`bbtnPatientSign_ItemClick`) | Lần ký sau (`signSelected.IS_SIGNING = 1`): chỉ ký vào các mốc `#@!@#PATIENT` **chưa có chữ ký** (đối chiếu chữ ký số đã có trên PDF); hết mốc → cho kéo khung tay. Xem C3 |
| F5 | `UCViewer.cs:316, 640, 885, 1811` (nút **Kết thúc ký**) | Bảo đảm hiện/bật khi dòng đang chọn là dòng BN có `IS_SIGNING = 1`; hỏi xác nhận trước khi kết thúc thay BN |
| F6 | `bbtnRelativeHomeSign_ItemClick` (`UCViewer.cs:3266`) | Áp dụng giống F4 cho **Người nhà ký** (cùng dòng `EMR_SIGN` của BN) |

Plugin HIS: kiểm tra lại `UCEmrDocumentList.cs:858-866` — nút **Bệnh nhân ký** chỉ bật khi `NEXT_SIGNER` là BN. Với văn bản **ký song song** (xem 4.1), `NEXT_SIGNER` có thể là bác sĩ dù BN vẫn còn ký được → nên đổi sang xét `UN_SIGNERS` chứa `#@!@#<mã BN>`.

---

# PHẦN 4. ẢNH HƯỞNG VÀ RỦI RO

## 4.1 Văn bản ký tuần tự có cả bác sĩ và BN — nguy cơ "kẹt lượt ký" (QUAN TRỌNG)

`EmrSignCheckVerifyNumOrderSign` ([dòng 24-28](E:/svn/IMSys/BACKEND/EMR/EMR.CheckBusiness/EmrSign/EmrSignCheckVerifyNumOrderSign.cs#L24-L28)) chặn người ký sau khi **bất kỳ** người ký trước còn `IS_SIGNING = 1` → *"Chưa đến lượt ký"*.

Ví dụ phiếu công khai thuốc hàng ngày, thứ tự: Điều dưỡng (1) → BN (2), cả hai ký nhiều lần:
- Ngày 1: Điều dưỡng ký, dòng còn mở → **BN bị chặn** cho tới khi Điều dưỡng **Kết thúc ký** → nhưng kết thúc rồi thì ngày 2 Điều dưỡng không ký tiếp được.
- Nếu BN đứng trước: BN còn mở → điều dưỡng bị chặn.

→ Ký nhiều lần cho **nhiều người trên cùng văn bản** chỉ chạy được khi loại văn bản bật **"Ký song song"** (`IS_SIGN_PARALLEL`, check này bỏ qua khi song song). Đây là giới hạn sẵn có của cơ chế, áp dụng cả cho bác sĩ hiện nay. Đề xuất: khi loại văn bản tích **"Ký nhiều lần"** mà có BN ký (`PATIENT_MUST_SIGN`) nhưng không tích **"Ký song song"** → màn Loại văn bản **cảnh báo** khi lưu (chờ chốt C2).

## 4.2 Ảnh hưởng khác

| Ảnh hưởng | Mức độ | Ghi chú |
|---|---|---|
| Văn bản đã ký trước khi áp dụng | Không ảnh hưởng | Dòng BN cũ đã `IS_SIGNING = null` → vẫn đóng. `IS_MULTI_SIGN` của văn bản là bản chụp lúc tạo |
| Viện không bật cấu hình | Không ảnh hưởng (nhánh A, C) | Nhánh B: xem C1 |
| Trạng thái "đã ký xong" của văn bản, duyệt/lưu trữ hồ sơ | **Có** | Văn bản còn dòng BN mở thì chưa hoàn thành ký → cần **Kết thúc ký** trước khi duyệt hồ sơ, lưu trữ (`EmrTreatmentCheckVerifyApprovalSign`) |
| Thông báo / email | Không | `SendNotification` đã bỏ qua khi `IS_SIGNING = 1` |
| Thao tác bác sĩ, điều dưỡng | Không | Mọi thay đổi có điều kiện `PATIENT_CODE` |
| Triển khai | Phải triển khai đồng thời | Backend EMR + `Inventec.Common.SignLibrary.dll` (nhiều plugin tham chiếu) + khai báo cấu hình. Thư viện mới với backend cũ: chỉ thông báo F3 thay đổi, không lỗi |

## 4.3 Nghi vấn lỗi sẵn có (cần kiểm chứng trước khi làm)

Văn bản **không** ký nhiều lần nhưng có **≥ 2 mốc** `#@!@#PATIENT`, ký qua **nhánh A**: thư viện ký lần lượt từng mốc, gửi `IsSigning = true` cho các mốc trước mốc cuối (`UCViewer.cs:3164-3178`) để giữ dòng mở; nhưng backend nhánh A luôn gán `null` → mốc thứ 2 có thể bị chặn *"Người ký đã ký văn bản"*. Sửa B3 đúng quy tắc 3.1 sẽ **chưa** giải quyết trường hợp này (vì cấu hình/loại văn bản không cho ký nhiều lần) → cần cho phép giữ dòng mở **trong cùng một lượt bấm ký**, xem C4.

---

# PHẦN 5. KỊCH BẢN KIỂM THỬ

| # | Cấu hình | Loại văn bản | Hình thức BN ký | Thao tác | Kết quả mong đợi |
|---|---|---|---|---|---|
| 1 | 0 | Ký nhiều lần | Bảng ký | BN ký 2 lần | Lần 2 bị chặn (như hiện tại) |
| 2 | 1 | Không tích | Bảng ký | BN ký 2 lần | Lần 2 báo "Văn bản đã được bệnh nhân ký", không mở form luồng ký |
| 3 | 1 | Ký nhiều lần + song song | Bảng ký | BN ký ngày 1, ngày 2, ngày 3 | Ký được cả 3 lần; mỗi chữ ký ở đúng mốc tương ứng, không đè mốc cũ |
| 4 | 1 | Ký nhiều lần + song song | Máy tính bảng / vân tay / thẻ có chứng thư | Như #3 | Như #3 (cả nhánh A, B, C) |
| 5 | 1 | Ký nhiều lần + song song | Người nhà ký | Lần 1 BN, lần 2 người nhà | Ký được, ghi đúng tên/quan hệ người nhà |
| 6 | 1 | Ký nhiều lần | — | Nhân viên bấm **Kết thúc ký** dòng BN | Dòng đóng; BN ký tiếp bị chặn với thông báo mới |
| 7 | 1 | Ký nhiều lần, **tuần tự** | — | Điều dưỡng (1) ký, rồi BN (2) | "Chưa đến lượt ký" (giới hạn 4.1); màn Loại văn bản có cảnh báo (nếu chốt C2) |
| 8 | 1 | Ký nhiều lần | — | Văn bản BN đã ký **trước** ngày áp dụng | Vẫn chặn, không mở lại |
| 9 | 1 | Ký nhiều lần | Bác sĩ, điều dưỡng ký | Ký nhiều lần, kết thúc ký | Không đổi so với hiện tại |
| 10 | 1 | Không tích, 2 mốc BN | Bảng ký | BN bấm ký 1 lần | Ký đủ cả 2 mốc (kiểm chứng 4.3) |

---

# PHẦN 6. ĐIỂM CẦN CHỐT

| # | Câu hỏi | Đề xuất |
|---|---|---|
| C1 | Nhánh B (BN ký bằng thẻ có chứng thư) **hiện đã** ký nhiều lần được theo loại văn bản. Khi cấu hình = 0 có ép về 1 lần không? | **Giữ nguyên** hành vi hiện tại khi cấu hình = 0 để không làm thay đổi viện đang dùng; chỉ áp quy tắc mới khi cấu hình = 1 |
| C2 | Loại văn bản tích "Ký nhiều lần" + BN phải ký nhưng không tích "Ký song song": cảnh báo hay chặn lưu? | Cảnh báo, vẫn cho lưu |
| C3 | Lần ký sau chọn mốc thế nào khi PDF có nhiều mốc `#@!@#PATIENT`? | Mốc **đầu tiên chưa có chữ ký** theo thứ tự trang/vị trí; hết mốc → kéo khung tay. Mẫu in (MPS) của phiếu ký theo ngày cần sinh sẵn mỗi ngày một mốc |
| C4 | Sửa luôn nghi vấn 4.3 (nhiều mốc trong một lượt ký, văn bản ký 1 lần) trong phạm vi việc này? | Có — kiểm chứng trên môi trường test trước, nếu lỗi thật thì sửa cùng |
| C5 | Ai được **Kết thúc ký** dòng BN? | Mọi nhân viên có quyền ký văn bản đó; hỏi xác nhận trước khi kết thúc |
| C6 | Ứng dụng ký trên máy tính bảng (nếu là ứng dụng riêng, gọi `PatientAddAndSignPdf`) có thuộc phạm vi? | Backend sửa B4 là đủ cho phía server; giao diện ứng dụng máy tính bảng xác nhận với nhóm phụ trách |
