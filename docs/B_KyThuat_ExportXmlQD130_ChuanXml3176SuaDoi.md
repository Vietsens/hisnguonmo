# Phân tích kỹ thuật: Áp dụng chuẩn dữ liệu đầu ra XML sửa đổi QĐ 3176 vào chức năng Xuất XML QĐ130

| Thông tin | Giá trị |
|---|---|
| Chức năng | `HIS.Desktop.Plugins.ExportXmlQD130` (xuất/gửi XML giám định BHYT) và thư viện sinh XML `common/HISUTIL/His.Bhyt/His.Bhyt.ExportXml.XML130.*` |
| Tài liệu yêu cầu | `261116_Chuan du lieu dau ra (sua 3176)_Final.xlsx` — 16 sheet (Check-in, Bảng 1 → 15). Cột "Sửa đổi, bổ sung" là nội dung mới so với QĐ 3176 |
| Ngày rà | 26/11/2026, trên nhánh `Develop` (hisnguonmo) và source `common` hiện tại. Số dòng code trong tài liệu đúng tại thời điểm này |
| Phạm vi | Chỉ phân tích, chưa sửa code |
| Trạng thái văn bản | Quyết định chưa có số/ngày ("Quyết định số /QĐ-BYT ngày / /2026") |

---

## 1. Tóm tắt

1. **Code hiện tại chưa triển khai dòng nào của phần sửa đổi mới.** Các bảng chạy theo logic QĐ 130 + nhánh `IS_3176`.
2. **97 dòng thay đổi trên 14 bảng** (Bảng 15 Lao không có cột sửa đổi). Phân loại:
   - **Trường mới (3):** XML1 `MA_BENH_KHAC`, XML1 `T_BNCCT_LK`, XML7 `NGAY_VAO_NOI_TRU`.
   - **Sai so với chuẩn mới, sửa được ngay bằng dữ liệu sẵn có:** khoảng 15 trường (trường phải để null, fallback CCCD cho `MA_TTDV`, định dạng `NGAY_HEN_KL`, chẩn đoán XML10, họ tên cha/mẹ XML11, `HAM_LUONG` máu, `_TB` XML2...).
   - **Sai về tiền (rủi ro cao nhất):** `MUC_HUONG`, `T_BNCCT`, `T_BNTT`, `T_NGUONKHAC` ở XML2, XML3 và trần 6 tháng lương cơ sở ở XML1.
   - **Cần dữ liệu nghiệp vụ mới (chưa có trong HIS):** `.C.WWWWW`, `.K` vật tư, `H.XXXXX`, `MA_BAC_SI.TX/.HD`, người thực hành – người hướng dẫn, tên thương mại VTYT, nhiều ngày hẹn khám, mã giường `X.Y.ZZZ(.Kxx)`.
   - **Chỉ đổi văn bản tham chiếu (code không đổi):** khoảng 1/3 số dòng (TT 25/2025, TT 01/2025, QĐ 1804, QĐ 1227...).
3. **Phần lớn việc sửa nằm ở thư viện common**, không nằm ở plugin. Plugin chỉ bổ sung dữ liệu đầu vào, cấu hình và chế độ xuất.
4. **Đổi công thức tiền ảnh hưởng ngoài phạm vi XML**: chức năng Tiền giám định (`BuildHsth01bhChiTietList`) dùng trực tiếp XML2/XML3, và XML1 sẽ không còn bằng tổng XML2 + XML3 khi áp trần lũy kế.

---

## 2. Kiến trúc luồng xuất hiện tại

```
UCExportXml (plugin)
  ├─ Nạp dữ liệu theo lô (thread): Treatment, PatientTypeAlter (GetView), SereServ, Tracking (HIS_TRACKING),
  │   Dhst, BedLog, EkipUser, Baby, Debate, MedicalAssessment, HivTreatment, TuberculosisTreat...
  ├─ Dựng InputADO + ado.IS_3176
  └─ CreateXmlProcessor(ado).Run()   ← thư viện His.Bhyt.ExportXml.XML130
        └─ HoSoProcessor: truyền IS_3176 xuống từng XmlNProcessor → XMLnData (XmlSerializer, [XmlElement(Order)])
```

### 2.1. Các chỗ dựng `InputADO` trong plugin (phải sửa đồng bộ)

| # | Vị trí | Luồng | `IS_3176` |
|---|---|---|---|
| 1 | `UCExportXml.cs:1623` (`ProcessExportXmlDetail`) | Xuất XML, Xem XML, XML 3176, hồ sơ đang điều trị | theo tham số |
| 2 | `UCExportXml.cs:2088` (`ProcessExportXmlDetailPlus`) | Xuất gộp | theo tham số |
| 3 | `UCExportXml.cs:4786` (`BuildInputAdoForXml`, gọi từ `ProcessSyncTreatment`) | Gửi XML 130/3176 tay + tự động | theo `configSync.isXML3176` |
| 4 | `UCExportXml.cs:6404` (`MenuItemClick_XuatXmlGiamDinhYKhoa`) | Xuất XML GĐYK | theo checkbox |
| 5 | `UCExportXml_KiemTraHoSo.cs:394` | Kiểm tra hồ sơ | **cứng `false`** (dòng 475) |
| 6 | `UCExportXml___Xml48h.cs:641` | XML hồ sơ đủ 48 giờ | theo `configSync.isXML3176` |

Ngoài ra có 2 luồng dùng trực tiếp processor XML2/XML3, không qua `InputADO` chung:

- `UCExportXml.cs:5868-5922` (`BuildHsth01bhChiTietList`, Tiền giám định).
- Xuất Excel (`UCExportXml_Excel.cs:1057-1071`, đọc theo kiểu `XMLnData`).

### 2.2. Phạm vi cờ `IS_3176` trong thư viện

| Bảng | Có nhánh 3176? | Nhánh 3176 đang ghi đè gì |
|---|---|---|
| Check-in | **Không** (InputADO của `His.Bhyt.ExportXml` không có cờ) | — |
| XML1 | Có (`Xml1Processor.cs:827-861`) | SO_CCCD, MA_NGHE_NGHIEP, CHAN_DOAN_VAO, MA_BENH_YHCT, MA_TTDV, DU_PHONG |
| XML2 | Có (`Xml2Processor.cs:874-985`) | Chia T_NGUONKHAC theo nguồn, NGAY_TH_YL. Phần tính tiền 3176 đã bị comment, nên tiền tính như QĐ 130 |
| XML3 | Có (`Xml3Processor.cs:1541-1662`) | MA_MAY, NGUOI_THUC_HIEN |
| XML4 | **Không** | — |
| XML5 | Có, một phần (`Xml5Processor.cs:228-239`) | NGUOI_THUC_HIEN |
| XML6–11, 13, 14 | Có | MA_TTDV (DIPLOMA), MA_BS, NGAY_HEN_KL, CHAN_DOAN_RV (XML10)... |
| XML12 | **Không** (`HoSoProcessor.cs:308` không gán `input12.IS_3176`) | — |

### 2.3. Kiểm tra EFMODEL

`lib/MOS/MOS.EFMODEL.dll` (29/10/2026, plugin đang tham chiếu) và bản deploy `histest` (26/11/2026) có:

| Trường | Bảng | Ghi chú |
|---|---|---|
| `CO_PAID_ACCUMULATE_AMOUNT` | `HIS_PATIENT_TYPE_ALTER` **và** `V_HIS_PATIENT_TYPE_ALTER` | Bản EFMODEL 01/2026 **chưa có** → phải đọc qua PropertyInfo để không lỗi `MissingMethodException` ở viện còn bản cũ |
| `ICD_DIFF_CODE`, `ICD_DIFF_TEXT` | `HIS_TRACKING` | Chẩn đoán phân biệt trên tờ điều trị |
| `IDENTIFICATION_NUMBER` | `HIS_EMPLOYEE` | Số CCCD nhân viên, đã được XML3 (`GetCCCD`) và XML5 dùng |
| `JOIN_5_YEAR`, `JOIN_5_YEAR_TIME`, `PAID_6_MONTH`, `FREE_CO_PAID_TIME` | `HIS_PATIENT_TYPE_ALTER` | Điều kiện miễn cùng chi trả |
| `TREATMENT_TYPE_ID`, `LOG_TIME` | `V_HIS_PATIENT_TYPE_ALTER` | Lịch sử diện điều trị → MA_LOAI_KCB nhiều mã |

---

## 3. Phân tích theo bảng

Ký hiệu kết luận: **ĐẠT** · **THIẾU** · **SAI** · **CDL** (cần dữ liệu mới) · **DG** (chỉ đổi diễn giải/văn bản tham chiếu, code không đổi).

Đường dẫn viết tắt (gốc `common/HISUTIL/His.Bhyt/`):

| Viết tắt | Đường dẫn |
|---|---|
| `X1` | `His.Bhyt.ExportXml.XML130.XML1/QD130/Processor/Xml1Processor.cs` |
| `X2` | `His.Bhyt.ExportXml.XML130.XML2/QD130/Processor/Xml2Processor.cs` |
| `X3` | `His.Bhyt.ExportXml.XML130.XML3/Xml3Processor.cs` |
| `X4` | `His.Bhyt.ExportXml.XML130.XML4/QD130/Processor/Xml4Processor.cs` |
| `X5` | `His.Bhyt.ExportXml.XML130.XML5/QD130/Processor/Xml5Processor.cs` |
| `X7` | `His.Bhyt.ExportXml.XML130.XML7/Xml7Processor.cs` |
| `XN` | `His.Bhyt.ExportXml.XML130.XMLN/QD130/Processor/XmlNProcessor.cs` |
| `CI` | `His.Bhyt.ExportXml/CheckIn/Processor/CheckInProcessor.cs` |
| `HS` | `His.Bhyt.ExportXml.XML130/HoSoProcessor.cs` |

### 3.1. Bảng Check-in

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 8, 10, 11 | MA_THE_BHYT, GT_THE_TU/DEN (kích thước n) | CI:192-195, không cắt độ dài | ĐẠT | — |
| 12 | MA_DOITUONG_KCB (50, nhiều mã `;`) | CI:301 lấy 1 mã | THIẾU | Input Check-in chỉ có 1 PatientTypeAlter (`ExportXml/Base/InputADO.cs:124`). Cần thêm danh sách. BA xác nhận lúc check-in có nhiều đối tượng hay không |
| 17 | MA_LOAI_KCB (10, nhiều mã `;`) | CI:308-392 lấy 1 mã | THIẾU | Như trên. CI:368 có nguy cơ lỗi null khi `HeinApproval` null |
| 24 | TEN_VAT_TU (tên thương mại theo trúng thầu) | CI:430-441 lấy `TDL_HEIN_SERVICE_BHYT_NAME` | SAI / CDL | Cần nguồn tên thương mại (xem 3.4 STT 8) |
| 5 | SO_CCCD (lỗi có sẵn) | CI:186 `CCCD ?? CCCD ?? PASSPORT` (lặp CCCD) | Lỗi | Sửa chuỗi fallback |

### 3.2. Bảng 1 – Tổng hợp (XML1)

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 12 | DIA_CHI | X1:54 `TDL_PATIENT_ADDRESS` | DG | Phụ thuộc dữ liệu nhập |
| 13, 15 | MATINH_CU_TRU, MAXA_CU_TRU (QĐ 19/2025/QĐ-TTg) | X1:55-66 lấy thẳng mã lưu trên hồ sơ, không map | CDL | Đúng nếu danh mục ĐVHC trong DB đã chuyển sang mã sau sáp nhập. Nếu chưa: cần bảng map cũ → mới |
| 14 | MAHUYEN_CU_TRU (gạch bỏ, "để null") | X1:59, 65 gán mã huyện | **SAI** | Gán null (xem 4.4) |
| 21 | NGAY_MIEN_CCT | X1:119, 164-166, 204-206 lấy `FREE_CO_PAID_TIME` | DG | BA xác nhận dữ liệu tra từ Cổng BHXH |
| 25 | CHAN_DOAN_VAO | X1:831-836 `PROVISIONAL_DIAGNOSIS`, rỗng thì lấy `ICD_NAME` | ĐẠT (fallback chưa chuẩn) | Fallback nên là `IN_ICD_NAME` (chẩn đoán vào viện), không phải chẩn đoán ra viện |
| 27 | MA_BENH_CHINH | X1:411 `ICD_CODE` | ĐẠT | Danh mục ICD theo TT 06/2026 (dữ liệu) |
| 28 | MA_BENH_KT (tối đa 12 mã) | X1:412-430, đã giới hạn 12 | ĐẠT | Việc loại chẩn đoán nghi ngờ phụ thuộc nhập liệu |
| **29** | **MA_BENH_KHAC (mới)** | Không có trong `XML1Data` và X1 | **THIẾU** | Thêm property Order 29, dời Order phía sau. Nguồn đề xuất: `HIS_TRACKING.ICD_DIFF_CODE` (đã nạp `ListTracking`), gộp, bỏ trùng, loại mã đã có ở MA_BENH_CHINH/MA_BENH_KT. BA chốt nguồn |
| 31 | MA_PTTT_QT (125) | X1:447-467 | ĐẠT | Không kiểm 125 ký tự |
| 32 | MA_DOITUONG_KCB (50, nhiều mã) | X1:404 `HEIN_PATIENT_TYPE_CODE ?? MaDTKCB`, 1 mã | THIẾU | Input có `data.PatientTypeAlter` (List). Tính mã cho từng lần đổi đối tượng, nối `;`. BA chốt: chỉ đối tượng BHYT hay cả viện phí/KSK, có giữ ưu tiên `HEIN_PATIENT_TYPE_CODE` không |
| 39 | GIAY_CHUYEN_TUYEN | X1:474-484: cấu hình `=1` lấy `OUT_CODE ?? TRANSFER_IN_CODE`, mặc định `TRANSFER_IN_CODE` | THIẾU (cấu hình =1 SAI) | Chuẩn: số phiếu chuyển của nơi chuyển đi, hoặc số **phiếu hẹn khám lại** (nếu có). Nhánh `=1` ưu tiên `OUT_CODE` (phiếu do chính viện cấp) là sai. Chưa có liên kết tới phiếu hẹn của lần khám trước → CDL |
| 46, 47 | T_THUOC, T_VTYT ("để null") | X1:515-516 ghi số | **SAI** | Gán null |
| **51** | **T_BNCCT_LK (mới)** | Không có | **THIẾU** | Thêm property. Nguồn: `CO_PAID_ACCUMULATE_AMOUNT` (đọc qua PropertyInfo). BA chốt lấy ở PatientTypeAlter nào khi có nhiều thẻ |
| 52, 53 | T_BNCCT, T_BHTT (trần 6 tháng lương cơ sở) | X1:519-521 = tổng XML2 + XML3 (cộng ở HS:359-373, 438-452, 1356-1359, 1435-1438 và `HoSoProcessorPlus.cs:434-437, 513-516`). Không có trần, không có cấu hình lương cơ sở | **SAI** | Xem 4.1.3 |
| 58 | MA_LOAI_KCB (10, nhiều mã) | X1:550-644, 1 mã | THIẾU | Xem 4.2. Nguồn: `PatientTypeAlter.TREATMENT_TYPE_ID` theo `LOG_TIME` |
| 59 | MA_KHOA (nhiều khoa) | `MA_KHOA_OPTION=1` (X1:349-395) gộp nhiều khoa; mặc định (X1:396-402) 1 mã | ĐẠT khi cấu hình =1, SAI khi mặc định | Ở chế độ chuẩn mới ép dùng nhánh nhiều khoa; bỏ trùng mã BHYT (hiện chỉ bỏ trùng ID khoa) |
| 67 | MA_TTDV (không GPHN thì ghi CCCD) | Nhánh 3176 X1:847 `GetMaBacSi` → `DIPLOMA` hoặc rỗng | THIẾU | Fallback `HIS_EMPLOYEE.IDENTIFICATION_NUMBER` (xem 4.3) |
| 5 | SO_CCCD (lỗi có sẵn) | X1:829 `CCCD ?? CMND ?? PASSPORT`, `??` không bỏ qua chuỗi rỗng | Lỗi | Dùng `IsNullOrWhiteSpace`. BA xác nhận còn dùng CMND không (chuẩn: CCCD/số định danh, người nước ngoài ghi hộ chiếu) |

### 3.3. Bảng 2 – Thuốc (XML2)

`XML2DetailData` có 39 trường, đúng thứ tự chuẩn. Bảng này không có trường mới.

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 3 | MA_THUOC | X2:409-418 `ACTIVE_INGR_BHYT_CODE`. Code không tự sinh "00.0000" | THIẾU + DG | Mã `YYYY.SĐK`, bỏ "00.0000": sửa danh mục. **"Thuốc thành phẩm ngoài phạm vi chi trả thì để trống"**: cần code + tiêu chí phân biệt thuốc thành phẩm với thuốc tự bào chế (CDL) |
| 5 | MA_CSKCB_THUOC | X2:440 `TRANSFER_MEDI_ORG_CODE` nguyên văn | THIẾU / CDL | `C.XXXXX`: tự thêm tiền tố "C." (TT12 `Mau04Processor.cs:160` đã làm). `H.XXXXX` (hỗ trợ từ xa): chưa có dữ liệu |
| 9 | HAM_LUONG | X2:444 luôn `CONCENTRA` | THIẾU | Để trống với máu/CPM (làm được ngay theo `TDL_HEIN_SERVICE_TYPE_ID`). Khí y tế, vị thuốc YHCT: BA chốt tiêu chí nhận diện |
| 12 | LIEU_DUNG | X2:447-509 tự nối " * N ngày [x ĐVT/ngày]" trừ khi `LIEU_DUNG_LAY_NGUYEN_VAN=1` | DG (rủi ro) | Thuốc dùng ngoài, thuốc đổi liều theo ngày: hậu tố tự nối có thể sai mẫu |
| 14 | SO_DANG_KY | X2:518 nguyên văn | DG | `KD.`, `UBND.`, `DL`: dữ liệu. Nên bỏ khoảng trắng |
| 15 | TT_THAU | X2:520-546 | DG | Dạng "Số QĐ;yyyyMMdd": nhập vào `MEDICINE_TT_THAU` |
| 16 | PHAM_VI (thêm mã "3") | X2:610-629: "3" chỉ khi thẻ CA/CY/QN **và** thuốc ngoài danh mục **và** `TyleTTBH>0` | ĐẠT một phần | BA xác nhận điều kiện mã 3 |
| 19 | DON_GIA | X2:566, 582 | DG | Chuẩn ghi "khoản Điều 49" thiếu số khoản |
| 26 | T_NGUONKHAC | X2:601-608, 690-732, 754-767 | **SAI** | Chuẩn: giảm trừ lần lượt T_BNTT → T_BNCCT → T_BHTT. Code trừ vào BNCCT trước, X2:691 không trừ NK ở BNTT, BHTT không bao giờ bị trừ, X2:720 **ghi đè số tiền nguồn khác** khi lệch tổng. Xem ví dụ 4.1.2 |
| 27 | MUC_HUONG (4) | X2:667 `HEIN_RATIO*100` | Cần chốt | Xem 4.1.1 |
| 28, 29 | T_BNTT, T_BNCCT | X2:676-708 | **SAI** khi đi KCB không đúng quy định | Xem 4.1 |
| 30 | T_BHTT | X2:596-599 | ĐẠT | Có thể lệch 0.01 do làm tròn |
| 31 | MA_KHOA (nhiều khoa) | X2:710 `REQUEST_BHYT_CODE` (khoa chỉ định) | Cần BA | Ở mức từng dòng thuốc nghĩa là gì? Xem câu hỏi Q10 |
| 32 | MA_BAC_SI | X2:769-775 `DIPLOMA` | THIẾU / CDL | `.TX.XXXXX` (từ xa), `GPHN.CCCD` (thực hành), `.HD.XXXXX` (huy động): chưa có dữ liệu |
| 33 | MA_DICH_VU (`_TB`) | X2:777-807: DV cha `IS_NO_EXECUTE` thì **bỏ dòng thuốc** (khi cấu hình) hoặc ghi mã cha không kèm `_TB` | THIẾU | Thêm `_TB` như XML3 (X3:678-684), không bỏ dòng. X2:792 fallback sang mã của chính thuốc: đáng ngờ |
| 34 | NGAY_YL | X2:808 | ĐẠT | — |
| 35 | NGAY_TH_YL | X2:810-832, nhánh 3176 X2:900-907 | ĐẠT một phần | BA định nghĩa "dùng tại cơ sở" cho ca khám/ngoại trú |

### 3.4. Bảng 3 – DVKT, VTYT (XML3)

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 3 | MA_DICH_VU | X3:555 mã BHYT. Code chỉ tự ghép `_GT` và `_TB` | THIẾU | `.C.WWWWW` (nhận chuyển giao kỹ thuật, NĐ 188): CDL. **Lỗi sẵn có:** X3:532, 557, 1443 `A \|\| (B) && C` nên `_GT` vẫn được ghép khi cấu hình gây tê ≠ 1 (đã kiểm) |
| 5 | MA_VAT_TU | X3:476 | CDL | `.K` + mã cơ sở cho kít tách máu mua từ đơn vị khác: chưa có dữ liệu |
| 8 | TEN_VAT_TU (tên thương mại theo trúng thầu) | X3:477 `TDL_HEIN_SERVICE_BHYT_NAME` | SAI / CDL | EFMODEL không có trường "tên thương mại". BA chỉ định nguồn (ví dụ `MATERIAL_TYPE_NAME`) |
| 13 | SO_LUONG (ngày giường theo từng ngày) | X3:619 `AMOUNT`, không tách ngày | ĐẠT có điều kiện | Đạt nếu MOS sinh mỗi ngày giường một sere_serv. Nếu không thì phải tách dòng |
| 22 | MUC_HUONG | X3:720 `HEIN_RATIO*100` | Cần chốt | Xem 4.1.1 |
| 28, 29 | T_BNTT, T_BNCCT | X3:754-788, vật tư X3:1664-1792 | **SAI** khi đi KCB không đúng quy định | Xem 4.1. X3:784 điều kiện có trừ NK nhưng giá trị gán thì không |
| 31 | MA_KHOA | X3:519, 554 `REQUEST_BHYT_CODE`, 1 mã | Cần BA | Như XML2 |
| 32 | MA_GIUONG (`X.Y.ZZZ`, `X.Y.ZZZ.Kxx`) | X3:574-594 `BED_CODE`, mặc định `"."` | THIẾU / CDL | Đúng nếu danh mục `BED_CODE` đã khai đúng dạng. Hậu tố khoa khi khoa quá tải: cần dữ liệu khoa sở hữu giường. Giá trị `"."` sẽ bị cổng từ chối |
| 34 | NGUOI_THUC_HIEN (`GPHN_hướng_dẫn.CCCD_thực_hành`) | 3176: `DIPLOMA`, nếu rỗng thì `IDENTIFICATION_NUMBER`, nối `;` (X3:1661, 1897-1916) | THIẾU / CDL | Chưa có mô hình người hướng dẫn – người thực hành. CCCD đứng riêng là sai định dạng mới |
| 36 | MA_BENH_YHCT (150) | X3:1196-1223 | ĐẠT | — |

### 3.5. Bảng 4 – Dịch vụ CLS (XML4, không có cờ 3176)

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 3, 6 | MA_DICH_VU, GIA_TRI (đổi kích thước) | X4:102, 118 | ĐẠT | — |
| 4, 5 | MA_CHI_SO, TEN_CHI_SO (QĐ 1227, chỉ ghi chỉ số có đơn vị đo) | X4:104-116, 221-222, 276-293; **không lọc theo đơn vị đo** | THIẾU | Mã theo QĐ 1227/7603: dữ liệu. Không có mã thì "tên viết liền không dấu". Chỉ số không có đơn vị: BA chốt bỏ dòng hay để trống mã. Thay đổi sẽ áp dụng cho mọi chế độ vì XML4 không có cờ |

### 3.6. Bảng 5 – Diễn biến lâm sàng

| STT | Trường | KL |
|---|---|---|
| 3 | DIEN_BIEN_LS (X5:96-107, 253-353) | DG: thuật ngữ QĐ 2427/2552 chỉ là khuyến khích |

### 3.7. Bảng 6 – HIV/AIDS

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 8 | MAHUYEN_CU_TRU | X6:69, 144 | **SAI** | Gán null trong nhánh 3176 (X6:144 đang đổi null thành `""`) |

### 3.8. Bảng 7 – Giấy ra viện

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| **6** | **NGAY_VAO_NOI_TRU (mới)** | `XML7Data`/`XML7ADO` chưa có | **THIẾU** | Thêm Order 6, dời Order phía sau. Lấy `CLINICAL_IN_TIME` 12 ký tự như XML13:117, XML14:32 |
| 13 | PP_DIEUTRI | X7:153, 248 `TREATMENT_METHOD` | DG | — |
| 14 | GHI_CHU (mẫu số 2 PL II TT 25/2025, chỉ khi cấp để giải quyết chế độ BHXH) | X7:155 `ADVISE` (lời dặn), luôn ghi | Cần BA | Nguồn và điều kiện |
| 15 | MA_TTDV | Nhánh 3176 X7:249 `DIPLOMA`; không có giám đốc thì `""` (mất fallback `REPRESENTATIVE_HEIN_CODE`) | THIẾU | Fallback CCCD |
| 16, 17 | MA_BS, TEN_BS | X7:250-255: trưởng khoa ra viện → người ký thay | ĐẠT, cần BA | Xác nhận đúng người ký giấy ra viện |
| 21 | MA_THE_TAM | X7:187-191 | ĐẠT | — |

### 3.9. Bảng 8 – Tóm tắt HSBA

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 2 | MA_LOAI_KCB (nhiều mã) | X8:30-97, nhánh 3176 X8:260-299, 1 mã. Nhánh 3176 **lệch XML1** (thiếu mã 11-16, XML1 có thêm "100" KSK) | THIẾU | Dùng chung một hàm với XML1 (xem 4.2) |
| 13 | PP_DIEUTRI | X8:161 | DG | — |
| 19 | MA_TTDV | X8:259 `DIPLOMA` | THIẾU | Fallback CCCD |
| 21 | MA_THE_TAM | X8:244-254 | ĐẠT | — |

### 3.10. Bảng 9 – Giấy chứng sinh

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 2, 3, 7, 16 | MA_BHXH_NND, MA_THE_NND, SO_CCCD_NND, MA_THE_TAM | X9:31-52 | ĐẠT | X9:31-32 thẻ có độ dài khác 10/15 ra `""` |
| 10 | NOI_CU_TRU_NND | X9:46 nguyên chuỗi | Cần BA | Chưa có xử lý cho người nước ngoài không cư trú tại VN |
| 12, 14 | MATINH, MAXA | X9:48, 50 | ĐẠT về code | Phụ thuộc danh mục ĐVHC |
| 13 | MAHUYEN_CU_TRU | X9:49 | **SAI** | Gán null |
| 34 | MA_TTDV | X9:131 `DIPLOMA` | THIẾU | Fallback CCCD |

### 3.11. Bảng 10 – Giấy nghỉ dưỡng thai

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 2, 3 | SO_SERI, SO_CT | X10:26-27 cùng lấy `EXTRA_END_CODE` | DG | BA xác nhận hai trường cùng giá trị |
| 5 | DON_VI | X10:30-37 | DG | — |
| 6 | CHAN_DOAN_RV | Nhánh 3176 **X10:98 gán cứng `"dưỡng thai"`**, mất toàn bộ chẩn đoán (đã kiểm) | **SAI** | Ghép chẩn đoán đầy đủ (bệnh chính, bệnh kèm) và thêm cụm "dưỡng thai" |
| 9 | MA_TTDV | X10:96 | THIẾU | Fallback CCCD |

### 3.12. Bảng 11 – Giấy nghỉ hưởng BHXH

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 4 | SO_KCB | X11:28 `TREATMENT_CODE` | DG | — |
| 5 | DON_VI (con ốm thì lấy đơn vị cha/mẹ) | X11:29-36 | Cần BA | Chưa có logic con ốm; `!= null` không bắt chuỗi rỗng |
| 6, 21 | MA_BHXH, MA_THE_TAM | | ĐẠT | — |
| 8 | CHAN_DOAN_RV | X11:64-109 | ĐẠT phần mã/tên | Chưa có "dưỡng thai". **Lỗi sẵn có:** X11:101 `icdText[i]` vượt chỉ số khi số mã phụ nhiều hơn số tên |
| 16, 17 | HO_TEN_CHA, HO_TEN_ME (chỉ ghi khi trẻ dưới 7 tuổi) | X11:127-153, không xét tuổi | **SAI** | Thêm điều kiện tuổi. BA chốt ngày dùng để tính tuổi |
| 18 | MA_TTDV | X11:201 | THIẾU | Fallback CCCD |

### 3.13. Bảng 12 – GĐYK (không có cờ 3176)

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 10, 12 | MATINH, MAXA | X12:37, 39 | ĐẠT về code | — |
| 11 | MAHUYEN_CU_TRU | X12:38 | **SAI** | Thêm `IS_3176`/cờ chuẩn mới vào `XML12/Base/InputADO.cs` và gán ở HS:308, HS:1295 |
| 13, 14 | MA_BHXH, MA_THE_BHYT | X12:40-41 | ĐẠT | `??` không bắt chuỗi rỗng |

### 3.14. Bảng 13 – Giấy chuyển tuyến

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 2, 3, 4 | SO_HOSO, SO_CHUYENTUYEN, GIAY_CHUYEN_TUYEN | X13:38-62 | DG | Chưa cấu hình thì SO_CHUYENTUYEN = `"."`. **Lỗi sẵn có:** X13:49 `Int32.Parse(OUT_CODE.Substring(0,6))` lỗi khi mã ngắn hoặc có chữ |
| 35 | MA_BAC_SI ("để null") | X13:224-245 `DIPLOMA` | **SAI** | Gán null trong chế độ chuẩn mới |

### 3.15. Bảng 14 – Giấy hẹn khám lại

| STT | Trường | Hiện trạng | KL | Việc cần làm |
|---|---|---|---|---|
| 2 | SO_GIAYHEN_KL | X14:24 `END_CODE` | DG | — |
| 13 | NGAY_HEN_KL (12 ký tự, không rõ giờ ghi 0000, nhiều ngày `;`, 200 ký tự) | Nhánh 3176 **X14:326 cắt 8 ký tự** (đã kiểm) | **SAI** + CDL | Đổi thành 12 ký tự (sửa ngay). Nhiều ngày hẹn và cờ "không rõ giờ": chưa có dữ liệu, chỉ có một `APPOINTMENT_TIME` |
| 19 | MA_DOITUONG_KCB | X14:301, 1 mã | THIẾU | Dùng chung hàm với XML1 |
| 21 | MA_TTDV ("để null") | X14:327 `DIPLOMA` | **SAI** | Gán null |

### 3.16. Bảng 15 – Lao

Sheet không có cột "Sửa đổi, bổ sung". `XML15DetailData` có đủ 20 trường, thừa `MA_TTDV` (không gây lỗi). Không cần sửa.

---

## 4. Các chủ đề xuyên bảng

### 4.1. Tiền: MUC_HUONG, T_BNCCT, T_BNTT, T_NGUONKHAC, trần lũy kế

#### 4.1.1. MUC_HUONG

- **Chuẩn mới:** `MUC_HUONG = MUC_HUONG_BH/100 * MUC_HUONG_THE/100`.
  - `MUC_HUONG_THE` là mức hưởng trên thẻ (80/95/100).
  - `MUC_HUONG_BH` tra theo cột MUC_HUONG của Phụ lục 1 QĐ 3276/QĐ-BYT, dựa trên mã đối tượng đến KBCB (100 khi đúng quy định; 40, 50... khi không đúng quy định).
- **Code:** `HEIN_RATIO * 100`. `HEIN_RATIO` là tỉ lệ đã gộp do MOS tính.
- **Mâu thuẫn trong văn bản:** công thức trường MUC_HUONG cho ra 0.32, nhưng công thức T_BHTT lại có "MUC_HUONG/100", tức MUC_HUONG phải là 32. Kích thước 4 thì chứa được cả hai. → **Cần chốt định dạng (Q2).**
- **Nguồn tách riêng:** không có trường lưu sẵn.
  - `MUC_HUONG_THE` có thể lấy từ `MOS.LibraryHein.Bhyt.BhytHeinProcessor.GetDefaultHeinRatio(heinCardNumber)`, theo ký tự quyền lợi trên thẻ. XML1 đã tham chiếu thư viện này, XML2/XML3 chưa.
  - `MUC_HUONG_BH`: tra theo `HEIN_PATIENT_TYPE_CODE` cần bảng tra Phụ lục 1 QĐ 3276. Suy ngược `HEIN_RATIO / tỉ lệ thẻ` thì sai với các ca được hưởng 100% (dưới 15% lương cơ sở, cơ sở cấp ban đầu, miễn cùng chi trả).

#### 4.1.2. Ví dụ số

**Ví dụ A: đi KCB không đúng quy định, không có nguồn khác.**
Thẻ 80%, MUC_HUONG_BH = 40, TYLE_TT_BH = 100, THANH_TIEN_BV = THANH_TIEN_BH = 100.000. Giả định `HEIN_RATIO = 0.32`.

| Trường | Code hiện tại (XML2, XML3) | Chuẩn mới |
|---|---|---|
| MUC_HUONG | 32 | 0.32 hoặc 32 (chờ chốt) |
| T_BHTT | 32.000 | 32.000 |
| T_BNCCT | **68.000** | **8.000** (= 100.000 × 40% − 32.000) |
| T_BNTT | **0** | **60.000** (= 100.000 − 32.000 − 8.000) |

Như vậy 60.000đ đang bị ghi vào cùng chi trả thay vì tự trả. Tổng tiền vẫn khớp nên lỗi khó phát hiện bằng kiểm tra tổng. Ca đi đúng quy định (MUC_HUONG_BH = 100) thì code cho kết quả khớp chuẩn mới.

**Ví dụ B: có nguồn khác.**
Thẻ 80%, đi đúng quy định. THANH_TIEN_BV = 150.000, THANH_TIEN_BH = 100.000, T_BHTT = 80.000. Trước giảm trừ: BNTT = 50.000, BNCCT = 20.000.

| NK | Code hiện tại | Chuẩn mới (trừ BNTT → BNCCT → BHTT) |
|---|---|---|
| 10.000 | BNTT 50.000, BNCCT 10.000, BHTT 80.000 | BNTT 40.000, BNCCT 20.000, BHTT 80.000 |
| 30.000 | BNCCT về 0, BNTT 50.000, tổng 160.000 ≠ 150.000 nên **ghi đè T_NGUONKHAC = 20.000** | BNTT 20.000, BNCCT 20.000, BHTT 80.000, NK 30.000 |

#### 4.1.3. Trần 6 tháng lương cơ sở (XML1)

Quy tắc theo chuẩn mới:

- Nếu `T_BNCCT_LK + (T_BNCCT XML2 + XML3) ≤ 6 × lương cơ sở` thì `T_BNCCT = T_BNCCT XML2 + XML3`.
- Nếu vượt và BN đã tham gia BHYT đủ 5 năm liên tục thì `T_BNCCT = 6 × lương cơ sở − T_BNCCT_LK`, phần dư cộng vào `T_BHTT`.

Những gì còn thiếu để làm được:

- Cấu hình **lương cơ sở** theo thời điểm: chưa có (grep `LUONG_CO_SO|BASE_SALARY|MIN_SALARY` trong His.Bhyt không có kết quả).
- **Cờ 5 năm:** có `JOIN_5_YEAR`, `JOIN_5_YEAR_TIME`; BA chốt so với mốc ngày nào.
- **Phân bổ:** sau khi áp trần, XML1 ≠ tổng XML2 + XML3. Cần BA/BHXH xác nhận có phải phân bổ ngược xuống từng dòng XML2/XML3 không.
- **Vị trí sửa:** đặt logic trong Xml1Processor sau khi đã có tổng, để không phải sửa ở 3 nơi cộng tổng.

#### 4.1.4. Cấu hình tiền hiện có sẽ xung đột với công thức mới

`HIS.QD_188.XML.BNCCT` (QD_188_XML_BNCCT), `APPLY_OUT_PATIENT_OVER_ROUTE_PAID_RATIO`, `GAN_LAI_TIEN_BNTT_BNCCT_OPTION`, `TINH_LAI_TIEN_BNTT_BNCCT_NGUON_KHAC_OPTION`. Ở chế độ chuẩn mới cần bỏ qua hoặc định nghĩa lại các cấu hình này.

#### 4.1.5. Ảnh hưởng ngoài XML

- Tiền giám định (`BuildHsth01bhChiTietList`) gọi trực tiếp XML2/XML3 với `IS_3176 = true` → kết quả đối chiếu tiền sẽ thay đổi.
- Số liệu XML lệch với số liệu MOS (`VIR_TOTAL_PATIENT_PRICE_BHYT`, bảng kê 6556...) cho ca đi không đúng quy định. BA xác nhận có phải đồng bộ MOS không.

### 4.2. MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã

Các chỗ đang so sánh `MA_LOAI_KCB` bằng `==` với một mã (sẽ sai khi giá trị là "01;03"):

| # | Vị trí | Mục đích |
|---|---|---|
| 1 | X1:620 | `== "01"` → đổi sang "100" (KSK) |
| 2 | X1:646-647 | `== "02"/"05"/"08"` → xóa NGAY_VAO_NOI_TRU |
| 3 | X1:653-659 | Nhóm mã → tính SO_NGAY_DTRI |
| 4 | HS:844 (`CreateXml1InTreatment`) | Tính SO_NGAY_DTRI hồ sơ đang điều trị |
| 5 | X1:592, CI:366, X8:72 | So sánh nội bộ `"10"` trước khi gán (không bị ảnh hưởng) |

Các chỗ tự tính MA_LOAI_KCB riêng (sẽ lệch với XML1):

- `XML8/.../Xml8Processor.cs:30-92, 260-299`.
- `XML14/.../Xml14Processor.cs:33-36`.
- `XMLTT12.XML01BH/.../Xml01BHProcessor.cs:118, 284-358`.

**Đề xuất:**

- Tách hàm dùng chung `BuildMaLoaiKcbList(patientTypeAlters, treatment)` trả về danh sách mã theo thứ tự `LOG_TIME`. Chuỗi xuất là `string.Join(";")`.
- Các phép so sánh dùng danh sách (`Contains`) hoặc mã cuối/mã nội trú tùy quy tắc BA chốt (Q4).

### 4.3. MA_TTDV fallback CCCD

| Bảng | Hiện trạng nhánh 3176 | Việc cần làm |
|---|---|---|
| XML1, 7, 8, 9, 10, 11 | `GetMaBacSi` → `DIPLOMA` hoặc `""` | Nếu `DIPLOMA` rỗng thì lấy `HIS_EMPLOYEE.IDENTIFICATION_NUMBER` (dùng lại `GetCCCD` của XML3) |
| XML14 | `DIPLOMA` | Để null |

**Lỗi chung:** `HOSP_SUBS_DIRECTOR_LOGINNAME ?? HOSPITAL_DIRECTOR_LOGINNAME` không bỏ qua chuỗi rỗng. Gặp ở XML7:249-255, XML8:259, XML9:131, XML10:96-97, XML11:201-202, XML13:332, XML14:327.

### 4.4. "Để giá trị null"

Các trường: MAHUYEN_CU_TRU (XML1, 6, 9, 12), T_THUOC, T_VTYT (XML1), MA_BAC_SI (XML13), MA_TTDV (XML14).

`XmlSerializer` xử lý như sau:

- property string = `null` → **không có thẻ**;
- property string = `""` → thẻ rỗng `<MAHUYEN_CU_TRU></MAHUYEN_CU_TRU>`.

Code đã có tiền lệ gán null ở XML13:306-335. → **BA/BHXH chốt cổng muốn bỏ thẻ hay thẻ rỗng (Q6).**

### 4.5. Mã đơn vị hành chính (QĐ 19/2025/QĐ-TTg)

Code lấy thẳng mã tỉnh/xã đang lưu trên hồ sơ (XML1, 6, 9, 12), không có bảng map. Kết quả đúng hay không phụ thuộc danh mục SDA trong DB đã cập nhật mã sau sáp nhập chưa. Cần kiểm dữ liệu từng viện trước khi triển khai.

### 4.6. Độ dài tối đa

Không bảng nào cắt hoặc kiểm tra độ dài, kể cả các trường đổi kích thước: MA_LOAI_KCB 10, MA_DOITUONG_KCB 50, MA_KHOA 50, NGAY_HEN_KL 200, MA_PTTT_QT 125, MUC_HUONG 4. Đã có hàm cắt `XmlProcessorBase.SubString` nhưng không nơi nào gọi.

**Đề xuất:** thêm cảnh báo vượt độ dài vào màn **Kiểm tra hồ sơ**. Không tự cắt, vì cắt mã ICD hoặc mã khoa sẽ làm sai dữ liệu.

---

## 5. Dữ liệu nguồn: sẵn có / cần bổ sung

| Nhu cầu | Trạng thái | Nguồn / ghi chú |
|---|---|---|
| MA_BENH_KHAC | Có thể dùng | `HIS_TRACKING.ICD_DIFF_CODE` (đã nạp). BA xác nhận đủ phạm vi "nghi ngờ, theo dõi, phân biệt đã loại trừ" |
| T_BNCCT_LK | Có cột | `PATIENT_TYPE_ALTER.CO_PAID_ACCUMULATE_AMOUNT`; đọc qua PropertyInfo. BA xác nhận giá trị lấy từ Cổng/API BHXH |
| NGAY_VAO_NOI_TRU (XML7) | Có | `CLINICAL_IN_TIME` |
| MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã | Có (XML1, 8, 14); Check-in không có | `V_HIS_PATIENT_TYPE_ALTER` (TREATMENT_TYPE_ID, LOG_TIME). Check-in phải thêm danh sách vào input |
| MA_TTDV fallback | Có | `HIS_EMPLOYEE.IDENTIFICATION_NUMBER` |
| MUC_HUONG_THE | Suy ra được | Thẻ BHYT + `BhytHeinProcessor.GetDefaultHeinRatio` |
| MUC_HUONG_BH | **Thiếu** | Bảng tra Phụ lục 1 QĐ 3276 theo mã đối tượng (danh mục mới hoặc cấu hình) |
| Lương cơ sở theo thời điểm | **Thiếu** | Khóa cấu hình mới |
| Số phiếu hẹn khám lại cho GIAY_CHUYEN_TUYEN | **Thiếu** | Liên kết tới hồ sơ hẹn lần trước |
| Nhiều ngày hẹn, cờ "không rõ giờ" (XML14) | **Thiếu** | Backend/DB |
| TEN_VAT_TU tên thương mại | **Thiếu** | BA chỉ định trường |
| `.C.WWWWW` chuyển giao kỹ thuật | **Thiếu** | Dữ liệu hợp đồng chuyển giao |
| `.K` vật tư mua từ đơn vị khác | **Thiếu** | Thông tin nguồn nhập |
| `H.XXXXX`, `.TX.`, `.HD.`, người thực hành – hướng dẫn | **Thiếu** | Mô hình dữ liệu KBCB từ xa, huy động, thực hành |
| MA_GIUONG `X.Y.ZZZ(.Kxx)` | Phụ thuộc danh mục | Kiểm `BED_CODE`; khoa sở hữu giường |
| Ngày giường tách theo ngày | Cần xác nhận | MOS có sinh mỗi ngày một sere_serv không |

---

## 6. Lỗi có sẵn phát hiện trong lúc rà

Các lỗi này không thuộc chuẩn mới nhưng nên sửa cùng đợt.

| # | Vị trí | Lỗi | Mức |
|---|---|---|---|
| 1 | X10:98 | Nhánh 3176 gán cứng CHAN_DOAN_RV = "dưỡng thai", mất toàn bộ chẩn đoán | CAO |
| 2 | X2:691, X2:720 | BNTT không trừ nguồn khác; ghi đè số tiền T_NGUONKHAC khi lệch tổng | CAO |
| 3 | X3:532, 557, 1443 | Ưu tiên toán tử `A \|\| B && C`: ghép `_GT` khi cấu hình gây tê ≠ 1 | TRUNG BÌNH |
| 4 | X3:574 | MA_GIUONG mặc định `"."` | TRUNG BÌNH |
| 5 | CI:186 | SO_CCCD `CCCD ?? CCCD ?? PASSPORT` (lặp) | TRUNG BÌNH |
| 6 | X11:101 | `icdText[i]` vượt chỉ số khi số mã phụ nhiều hơn số tên | TRUNG BÌNH |
| 7 | X13:49 | `Int32.Parse(OUT_CODE.Substring(0,6))` lỗi khi mã ngắn hoặc có chữ | TRUNG BÌNH |
| 8 | X2:549, X2:681, X3:750, CI:204, CI:368 | Truy cập thuộc tính không kiểm null (patientType, patientTypeAlter, HeinApproval) | TRUNG BÌNH |
| 9 | X2:792 | MA_DICH_VU fallback sang mã của chính thuốc | THẤP |
| 10 | X2:900 | `HisConfigs.Get` trong vòng lặp | THẤP |
| 11 | X4:355, X5:361 | `listAdo != null \|\| listAdo.Count > 0` | THẤP |
| 12 | `UCExportXml_KiemTraHoSo.cs:475` | Kiểm tra hồ sơ ghi cứng `IS_3176 = false`, khác với chế độ đang xuất | THẤP |

---

## 7. Câu hỏi cần BA / khách hàng chốt

| # | Câu hỏi | Ảnh hưởng |
|---|---|---|
| Q1 | Chế độ áp dụng: khóa cấu hình hoặc checkbox mới có ngày hiệu lực, hay sửa thẳng nhánh 3176? (Quyết định chưa có số). **Đã chốt 27/11/2026: sửa thẳng nhánh 3176 (phương án A)**; BA xác nhận không cần giữ song song chuẩn 3176 cũ | Toàn bộ |
| Q2 | MUC_HUONG ghi `0.32` hay `32`? | XML2, XML3 |
| Q3 | Nguồn MUC_HUONG_BH: bảng tra theo mã đối tượng, MOS tính sẵn, hay suy ngược? Có cần đồng bộ MOS (bảng kê) không? | Tiền |
| Q4 | MA_LOAI_KCB nhiều mã: SO_NGAY_DTRI, NGAY_VAO_NOI_TRU tính theo mã nào? Check-in có phải gửi lại khi đổi diện? | XML1, 8, 14, Check-in |
| Q5 | MA_DOITUONG_KCB nhiều mã: chỉ đối tượng BHYT hay cả viện phí/KSK? Có giữ ưu tiên `HEIN_PATIENT_TYPE_CODE`? | XML1, 14, Check-in |
| Q6 | "Để giá trị null" = bỏ thẻ hay thẻ rỗng? | 8 trường |
| Q7 | T_BNCCT_LK: lấy `CO_PAID_ACCUMULATE_AMOUNT` của thẻ nào? Có phải gọi API Cổng lúc xuất không? | XML1 |
| Q8 | Lương cơ sở cấu hình ở đâu? Mốc tính đủ 5 năm? Có phân bổ trần xuống XML2/XML3? | XML1 |
| Q9 | MA_BENH_KHAC: chỉ lấy `ICD_DIFF_CODE` tờ điều trị hay thêm nguồn khác? | XML1 |
| Q10 | MA_KHOA nhiều khoa ở mức từng dòng XML2/XML3 nghĩa là gì? | XML2, XML3 |
| Q11 | XML4 chỉ số không có đơn vị đo: bỏ dòng hay để trống mã? | XML4 |
| Q12 | TEN_VAT_TU tên thương mại: trường nào? | XML3, Check-in |
| Q13 | XML7 GHI_CHU: nguồn và điều kiện "cấp để giải quyết chế độ BHXH" | XML7 |
| Q14 | XML11 họ tên cha/mẹ: ngưỡng 7 tuổi tính theo ngày nào? | XML11 |
| Q15 | Các mã cần dữ liệu mới (`.C`, `.K`, `H.`, `.TX`, `.HD`, thực hành, nhiều ngày hẹn): làm ở đợt này hay đợt sau khi có thiết kế dữ liệu? | Phạm vi |
| Q16 | Danh mục tỉnh/xã của các viện đã theo QĐ 19/2025 chưa? | XML1, 6, 9, 12 |

---

## 8. Đề xuất triển khai

### Đợt 1: sửa ngay bằng dữ liệu sẵn có (thư viện common + plugin)

1. **Chế độ (đã chốt 27/11/2026, phương án A):** sửa thẳng trong nhánh `IS_3176`, không thêm cờ, không thêm khóa cấu hình. Bổ sung cờ `IS_3176` cho XML12 (hiện chưa nhận cờ).
2. **Trường mới:**
   - XML1 `MA_BENH_KHAC`, `T_BNCCT_LK` (đọc PropertyInfo);
   - XML7 `NGAY_VAO_NOI_TRU`;
   - đánh lại `[XmlElement(Order)]`.
3. **Null:**
   - MAHUYEN_CU_TRU (XML1, 6, 9, 12);
   - T_THUOC, T_VTYT;
   - XML13 MA_BAC_SI;
   - XML14 MA_TTDV.
4. **MA_TTDV fallback CCCD** (XML1, 7–11), sửa luôn lỗi `??` với chuỗi rỗng.
5. **Định dạng và nội dung:**
   - XML14 NGAY_HEN_KL 12 ký tự;
   - XML10 CHAN_DOAN_RV;
   - XML11 HO_TEN_CHA/ME theo tuổi;
   - XML2 HAM_LUONG máu/CPM;
   - XML2 `_TB`;
   - XML2 `C.` cho MA_CSKCB_THUOC;
   - XML1 MA_KHOA ép nhánh nhiều khoa.
6. **MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã:** hàm dùng chung, sửa các phép so sánh ở mục 4.2.
7. **Sửa lỗi sẵn có** mục 6, các dòng 1, 3–8.
8. **Viewer XMLViewer130:** chỉ cần deploy DLL XML1/XML7 mới; grid tự hiện cột mới.

### Đợt 2: tiền (sau khi chốt Q2, Q3, Q7, Q8)

1. Hàm tính tiền thống nhất cho XML2/XML3: `MUC_HUONG`, `T_BHTT`, `T_BNCCT`, `T_BNTT`, phân bổ `T_NGUONKHAC` theo thứ tự chuẩn.
2. Trần 6 tháng lương cơ sở ở XML1, lương cơ sở lấy từ bảng `HIS_BHYT_PARAM` có sẵn (`BASE_SALARY`, `FROM_TIME`, `TO_TIME`).
3. Rà và vô hiệu các cấu hình tiền cũ (mục 4.1.4) trong chế độ 3176.
4. Kiểm tra lại Tiền giám định và xuất Excel.

### Đợt 3: cần thiết kế dữ liệu mới (ngoài phạm vi chức năng xuất)

`.C.WWWWW`, `.K` vật tư, `H.XXXXX`, `MA_BAC_SI.TX/.HD`, người thực hành – hướng dẫn (XML2, XML3), TEN_VAT_TU, nhiều ngày hẹn khám, số phiếu hẹn cho GIAY_CHUYEN_TUYEN, MA_GIUONG `.Kxx`, XML4 lọc theo đơn vị đo.

### Ước lượng tương đối

| Hạng mục | Nơi sửa | Độ lớn |
|---|---|---|
| Đợt 1 mục 1–5 | common XML1–14, Check-in, plugin | Vừa |
| Đợt 1 mục 6 (nhiều mã) | common XML1, 8, 14, Check-in, TT12 | Vừa |
| Đợt 2 | common XML1–3, plugin, cấu hình | **Lớn, rủi ro cao** |
| Đợt 3 | MOS/DB + HIS nhập liệu + common | Lớn, phụ thuộc thiết kế |

---

## 9. Test case chính (cho đợt 1, 2)

- [ ] Chế độ cũ (130 / 3176) xuất ra file **giống hệt** trước khi sửa (so sánh nội dung file trên cùng bộ hồ sơ).
- [ ] Chế độ mới: XML1 có `MA_BENH_KHAC` (hồ sơ có tờ điều trị ghi chẩn đoán phân biệt), có `T_BNCCT_LK` (thẻ có số lũy kế); XML7 có `NGAY_VAO_NOI_TRU` đúng vị trí thẻ.
- [ ] Máy trạm dùng EFMODEL cũ (chưa có `CO_PAID_ACCUMULATE_AMOUNT`): xuất không lỗi, `T_BNCCT_LK` trống.
- [ ] MAHUYEN_CU_TRU, T_THUOC, T_VTYT, XML13 MA_BAC_SI, XML14 MA_TTDV theo quyết định Q6.
- [ ] Người ký không có GPHN: MA_TTDV ra số CCCD (XML1, 7–11).
- [ ] Hồ sơ khám rồi nhập viện nội trú: MA_LOAI_KCB = "01;03"; SO_NGAY_DTRI và NGAY_VAO_NOI_TRU đúng quy tắc Q4.
- [ ] Hồ sơ đổi đối tượng giữa đợt: MA_DOITUONG_KCB nhiều mã (XML1, XML14).
- [ ] XML14 NGAY_HEN_KL 12 ký tự; XML10 chẩn đoán đầy đủ kèm "dưỡng thai"; XML11 họ tên cha/mẹ chỉ khi trẻ dưới 7 tuổi.
- [ ] Đợt 2: ví dụ A, ví dụ B mục 4.1.2 ra đúng số theo chuẩn mới; ca đúng quy định không đổi so với hiện tại.
- [ ] Đợt 2: BN đủ 5 năm, lũy kế vượt 6 tháng lương cơ sở: XML1 T_BNCCT bị chặn trần, phần dư sang T_BHTT.
- [ ] Gửi tự động, XML 48 giờ, Kiểm tra hồ sơ, Xuất gộp, Excel, Tiền giám định: cùng chế độ thì cùng kết quả.
- [ ] Viewer XMLViewer130 mở file chuẩn mới: hiện cột mới; mở file cũ vẫn bình thường.
