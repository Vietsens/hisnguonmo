# Thiết kế kỹ thuật: Xuất XML theo chuẩn dữ liệu đầu ra sửa đổi QĐ 3176

| Thông tin | Giá trị |
|---|---|
| Chức năng | `HIS.Desktop.Plugins.ExportXmlQD130` + thư viện `common/HISUTIL/His.Bhyt/His.Bhyt.ExportXml.XML130.*`, `His.Bhyt.ExportXml` (Check-in) |
| Tài liệu yêu cầu | `261116_Chuan du lieu dau ra (sua 3176)_Final.xlsx` |
| Tài liệu phân tích | `docs/B_KyThuat_ExportXmlQD130_ChuanXml3176SuaDoi.md` (hiện trạng từng trường, ví dụ số, lỗi sẵn có, 16 câu hỏi Q1–Q16) |
| Ngày lập / cập nhật | 26/11/2026 / 27/11/2026 |
| Phương án | **A: sửa thẳng trong nhánh `IS_3176`, không thêm khóa cấu hình**, theo cách làm của các lần sửa 3176 trước |
| Mốc code | Số dòng tham chiếu theo code Develop kéo về ngày 27/11/2026 (hisnguonmo `4f8fa08b3`, common `95a7846`) |
| Trạng thái | **Bản thiết kế chờ duyệt.** Các điểm phụ thuộc câu hỏi BA chưa chốt được viết theo giả định GĐ1–GĐ9 (mục 3); mỗi giả định gom vào một chỗ trong code để đổi nhanh khi BA chốt khác |

---

## 1. Mục tiêu và phạm vi

### 1.1. Mục tiêu

1. Chế độ **3176 hiện có** (nút Xuất XML 3176, tùy chọn đồng bộ `isXML3176`) sinh XML theo chuẩn sửa đổi. Chuẩn sửa đổi **thay thế** chuẩn 3176 cũ, không giữ song song.
2. File XML của chế độ **QĐ 130** không thay đổi.
3. **Không thêm khóa cấu hình HIS_CONFIG** và không thêm cờ chế độ mới.
4. Gom các logic dùng chung cho nhiều bảng vào một chỗ: mã loại hình KCB, mã đối tượng KCB, mã người ký, tính tiền. Mục đích là để XML1, XML8, XML14 và Check-in không còn lệch nhau.

### 1.2. Cơ sở chọn phương án A

Rà 18 commit liên quan 3176 trong `common/HISUTIL/His.Bhyt` (từ `1e31e6c` đến `e648895`): không commit nào thêm khóa cấu hình mới. Chỉ 2 commit dùng khóa, và cả 2 khóa đều có sẵn (`HIS.QD_130_BYT.XML1.NGAY_VAO_NOI_TRU_OPTION` có từ bản 2.317.0 ngày 29/06/2024; `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK`). Chế độ 3176 được chọn bằng thao tác người dùng, không bằng HIS_CONFIG.

### 1.3. Phạm vi theo đợt

| Đợt | Nội dung | Mức thiết kế trong tài liệu này |
|---|---|---|
| 1 | 3 trường mới, trường null, MA_TTDV fallback CCCD, định dạng/nội dung (XML10, XML11, XML14, XML2), MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã, lỗi sẵn có | **Chi tiết** |
| 2 | Tính tiền XML2/XML3 (MUC_HUONG, T_BNCCT, T_BNTT, T_NGUONKHAC), trần 6 tháng lương cơ sở XML1 | **Chi tiết thuật toán**; tham số còn chờ Q2, Q3, Q7, Q8 |
| 3 | `.C.WWWWW`, `.K` vật tư, `H.XXXXX`, `MA_BAC_SI.TX/.HD`, người thực hành – hướng dẫn, TEN_VAT_TU, nhiều ngày hẹn, MA_GIUONG `.Kxx`, XML4 lọc đơn vị đo | **Chỉ định hướng**: cần thiết kế dữ liệu MOS/DB trước |

### 1.4. Ngoài phạm vi

- Khóa cấu hình mới, script SQL.
- Thay đổi MOS/DB (đợt 3).
- Đồng bộ công thức tiền vào bảng kê MOS (chờ Q3).
- Giao diện mới trong plugin.

---

## 2. Kiến trúc giải pháp

### 2.1. Sơ đồ

```
UCExportXml (plugin) ── chọn chế độ như hiện tại: ado.IS_3176 = isXML3176
      │                 + truyền thêm dữ liệu nguồn: ListBhytParam (đợt 2),
      │                   ListPatientTypeAlter cho Check-in
      ▼
CreateXmlProcessor / HoSoProcessor / HoSoProcessorPlus (XML130)
      │   truyền IS_3176 xuống input1..input15 (bổ sung input12 hiện chưa nhận cờ)
      ▼
XmlNProcessor: if (IS_3176) { ...quy tắc 3176 đã sửa đổi... }
      │ gọi
      ▼
His.Bhyt.ExportXml.XML130.Common (PROJECT MỚI)
  ├─ TreatmentTypeCodeResolver (MA_LOAI_KCB nhiều mã)
  ├─ HeinObjectCodeResolver   (MA_DOITUONG_KCB nhiều mã)
  ├─ SignerCodeResolver       (GPHN → CCCD)
  ├─ Xml3176Rules             (hằng số giả định GĐ2, GĐ3, GĐ9)
  ├─ EfModelReader            (đọc cột mới qua PropertyInfo)
  ├─ LineMoneyCalculator      (đợt 2)
  └─ CoPaymentCapCalculator   (đợt 2, lương cơ sở từ HIS_BHYT_PARAM)
CheckInProcessor (His.Bhyt.ExportXml) ── gọi cùng Common khi IS_3176
```

### 2.2. Quyết định thiết kế

| # | Quyết định | Lý do | Phương án đã loại |
|---|---|---|---|
| D1 | **Sửa thẳng trong nhánh `IS_3176`.** Không thêm cờ `IS_3176_AMENDED` | Đúng cách làm của các lần sửa 3176 trước (mục 1.2); ít thay đổi plugin, ít rủi ro lệch phiên bản | (B) Cờ riêng + 1 khóa ngày hiệu lực; (C) cờ riêng + 5 khóa. Không chọn theo quyết định ngày 27/11/2026 |
| D2 | **Không có ngày hiệu lực.** Chuẩn mới áp dụng từ khi deploy DLL thư viện, cho mọi hồ sơ xuất ở chế độ 3176 | Không thêm khóa cấu hình | Ngày hiệu lực theo `FEE_LOCK_TIME`: cần khóa mới |
| D3 | Tạo project dùng chung **`His.Bhyt.ExportXml.XML130.Common`** (.NET 4.5, AnyCPU, chỉ phụ thuộc MOS.EFMODEL, Inventec.Common.Logging, MOS.LibraryHein.Bhyt) | Logic MA_LOAI_KCB / MA_DOITUONG_KCB / mã người ký đang bị chép ở XML1, XML8, XML14, Check-in và đã lệch nhau | (a) Tính một lần ở HoSoProcessor rồi truyền xuống: Check-in không đi qua HoSoProcessor. (b) Chép hàm vào từng project: lặp lại tình trạng lệch |
| D4 | Mọi thay đổi nghiệp vụ nằm **trong `if (IS_3176)`**. Lỗi sẵn có sửa toàn cục chỉ khi không đổi file hợp lệ (mục 5.10) | Bảo đảm mục tiêu 2 | — |
| D5 | Cột EFMODEL mới (`CO_PAID_ACCUMULATE_AMOUNT`) đọc qua **PropertyInfo** | Viện còn EFMODEL trước 29/10/2026 sẽ lỗi `MissingMethodException` nếu truy cập trực tiếp | Truy cập trực tiếp + bắt buộc nâng EFMODEL |
| D6 | Điểm BA chưa chốt **viết cứng theo giả định**, mỗi điểm là một hằng số hoặc một hàm trong `Xml3176Rules` / resolver | BA chốt khác thì sửa đúng một chỗ, build lại thư viện | Tham số hóa bằng HIS_CONFIG (loại theo D1) |
| D7 | **Lương cơ sở lấy từ bảng `HIS_BHYT_PARAM`** có sẵn (`BASE_SALARY`, `FROM_TIME`, `TO_TIME`), nạp qua `api/HisBhytParam/Get` như `UCHeinInfo__CoPaidMCCT.cs:47-57` | Dữ liệu đã có, MOS đang dùng cho ngưỡng 6 tháng lương cơ sở | Khóa cấu hình lương cơ sở |

**Hệ quả của D1, D2 cần biết trước khi duyệt:**

- Xuất lại hồ sơ cũ ở chế độ 3176 sau khi deploy sẽ ra file theo chuẩn mới.
- Thời điểm áp dụng = thời điểm deploy. Phải deploy đúng mốc BHXH yêu cầu, viện chưa tới mốc thì chưa deploy.
- Quay lui = deploy lại DLL thư viện bản trước. Không tắt được bằng cấu hình.

---

## 3. Giả định thiết kế (chờ BA xác nhận)

| Mã | Câu hỏi | Giả định | Nếu BA chốt khác |
|---|---|---|---|
| GĐ1 | Q1 | Chuẩn sửa đổi thay thế chuẩn 3176 cũ trong chế độ 3176, áp dụng từ khi deploy (D1, D2) | Nếu phải giữ song song 3176 cũ: chuyển sang phương án B (cờ riêng + 1 khóa ngày hiệu lực) |
| GĐ2 | Q2 | MUC_HUONG ghi thang 0–100 như hiện nay | Đổi hằng số `Xml3176Rules.MucHuongScale` (100 → 1) |
| GĐ3 | Q6 | "Để null" = **bỏ thẻ** (gán `null`) | Đổi hàm `Xml3176Rules.NullValue()` trả `""` (thẻ rỗng) |
| GĐ4 | Q4 | MA_LOAI_KCB = các loại hình theo thứ tự `LOG_TIME`, bỏ trùng. SO_NGAY_DTRI tính theo **mã cuối**. NGAY_VAO_NOI_TRU chỉ xóa khi danh sách **không có** mã nội trú/điều trị ban ngày | Đổi `TreatmentTypeCodeResolver` |
| GĐ5 | Q5 | MA_DOITUONG_KCB = mã của các lần đổi diện **có thẻ BHYT**, theo `LOG_TIME`, bỏ trùng. Không có lần đổi nào thì giữ logic một mã hiện tại | Đổi `HeinObjectCodeResolver` |
| GĐ6 | Q7 | T_BNCCT_LK = `CO_PAID_ACCUMULATE_AMOUNT` của PatientTypeAlter BHYT có `LOG_TIME` lớn nhất mà cột có giá trị. Không gọi API Cổng lúc xuất | Bổ sung nguồn API ở đợt sau |
| GĐ7 | Q8 | Lương cơ sở = `HIS_BHYT_PARAM.BASE_SALARY` của bản ghi có `FROM_TIME ≤ ngày ra viện ≤ TO_TIME` (hoặc `TO_TIME` null). Đủ 5 năm = `JOIN_5_YEAR_TIME ≤ ngày ra viện`. Áp trần **chỉ ở XML1**, không phân bổ xuống XML2/XML3 | Thêm bước phân bổ trong `CoPaymentCapCalculator` |
| GĐ8 | Q9 | MA_BENH_KHAC = `HIS_TRACKING.ICD_DIFF_CODE` của các tờ điều trị trong đợt | Bổ sung nguồn trong `Xml1Processor` |
| GĐ9 | Q3 | MUC_HUONG_THE lấy từ thẻ (`BhytHeinProcessor.GetDefaultHeinRatio`). MUC_HUONG_BH tra bảng hằng số `Xml3176Rules.MucHuongBhByObjectCode`; không có trong bảng thì suy từ `HEIN_RATIO` | Sửa bảng hằng số hoặc thay nguồn trong `LineMoneyCalculator` |

---

## 4. Cấu hình và dữ liệu nguồn

**Không thêm khóa HIS_CONFIG.** Chỉ dùng lại khóa và bảng có sẵn:

| Khóa / bảng có sẵn | Cách dùng trong chế độ 3176 sửa đổi |
|---|---|
| `HIS.QD_130_BYT.XML1.NGAY_VAO_NOI_TRU_OPTION` | Áp thêm cho XML7.NGAY_VAO_NOI_TRU (giống XML14) |
| `HIS.QD_130_BYT.XML1.MA_KHOA_OPTION` | Chế độ 3176 luôn chạy nhánh nhiều khoa, không phụ thuộc giá trị khóa |
| `HIS.QD_130_BYT.XML3.KHONG_LAY_DVU_KHONG_THUC_HIEN` | Chế độ 3176 bỏ qua khóa này cho thuốc của DVKT không thực hiện (gắn `_TB`, không bỏ dòng) |
| `HIS.QD_130_BYT.XML2/XML3` các khóa tính lại tiền (`QD_188.XML.BNCCT`, `GAN_LAI_TIEN_BNTT_BNCCT_OPTION`, `TINH_LAI_TIEN_BNTT_BNCCT_NGUON_KHAC_OPTION`, `MOS.BHYT.APPLY_OUT_PATIENT_OVER_ROUTE_PAID_RATIO`) | Đợt 2: chế độ 3176 thay bằng `LineMoneyCalculator`; chế độ 130 giữ nguyên |
| Bảng `HIS_BHYT_PARAM` | Lương cơ sở theo mốc thời gian (D7) |

---

## 5. Thiết kế chi tiết

### 5.1. Cờ chế độ và dữ liệu truyền xuống

Không thêm cờ. Bổ sung:

| Vị trí | Thay đổi |
|---|---|
| `XML12/Base/InputADO.cs` | Thêm `IS_3176` (hiện chưa nhận cờ). `HoSoProcessor` gán ở cả 2 cụm dựng `input12` |
| `XML1/Base/InputADO.cs` | Thêm `List<HIS_TRACKING> ListTracking` (MA_BENH_KHAC) và `List<HIS_BHYT_PARAM> ListBhytParam` (đợt 2). `HoSoProcessor`/`HoSoProcessorPlus` gán từ `data` |
| `XML130/InputADO.cs` | Thêm `List<HIS_BHYT_PARAM> ListBhytParam` (đợt 2) |
| `His.Bhyt.ExportXml/Base/InputADO.cs` (Check-in) | Thêm `List<V_HIS_PATIENT_TYPE_ALTER> ListPatientTypeAlter` |

Vị trí gán `inputN.IS_3176` trong `XML130/HoSoProcessor.cs` (mốc 27/11): 342–343, 421, 507, 558, 579, 603, 629, 651, 672, 694, 717, 743, 765; nhánh thứ hai 1316–1317, 1395, 1481. Không sửa các dòng này, chỉ thêm `input12.IS_3176` và gán các list mới cạnh đó.

### 5.2. Plugin

Thay đổi plugin ở mức tối thiểu, **không** gom 6 chỗ dựng `InputADO` (bỏ việc T3 của bản 26/11 để giảm rủi ro):

| Việc | Vị trí |
|---|---|
| Nạp `HIS_BHYT_PARAM` (IS_ACTIVE) một lần mỗi phiên, cache trong UC | Hàm mới `LoadBhytParam()` trong `UCExportXml.cs`, gọi trong luồng nạp dữ liệu hiện có |
| Gán `ado.ListBhytParam` (đợt 2) | 6 chỗ dựng InputADO: `UCExportXml.cs` `ProcessExportXmlDetail`, `ProcessExportXmlDetailPlus`, `BuildInputAdoForXml`, `MenuItemClick_XuatXmlGiamDinhYKhoa`; `UCExportXml_KiemTraHoSo.cs`; `UCExportXml___Xml48h.cs` |
| Gán `ListPatientTypeAlter` cho input Check-in | Chỗ gọi `CheckInProcessor` |
| Bọc lời gán property mới trong hàm `[MethodImpl(MethodImplOptions.NoInlining)]` | Như tiền lệ `RunXmlInTreatment` (việc 60262): plugin mới + thư viện cũ vẫn xuất được |

Luồng chọn chế độ (`isXML3176`, `configSync.isXML3176`) giữ nguyên.

### 5.3. Project dùng chung `His.Bhyt.ExportXml.XML130.Common`

| Lớp | Phương thức | Dùng ở |
|---|---|---|
| `Xml3176Rules` | Hằng số và hàm giả định: `NullValue()` (GĐ3), `MucHuongScale` (GĐ2), `MucHuongBhByObjectCode` (GĐ9), `NoiTruTreatmentTypeCodes` | Mọi bảng |
| `TreatmentTypeCodeResolver` | `List<string> Resolve(V_HIS_TREATMENT_12, List<V_HIS_PATIENT_TYPE_ALTER>, configs)`; `string ResolveSingle(...)` (logic một mã hiện tại của XML1:550-644, gồm mã 11–16 và "100" KSK) | XML1, XML8, XML14, Check-in |
| `HeinObjectCodeResolver` | `List<string> Resolve(treatment, alters, mediOrgs, configs)`; `string ResolveSingle(alter, …)` (chuyển nguyên logic XML1:186-336) | XML1, XML14, Check-in |
| `SignerCodeResolver` | `GetLicenseOrIdentity(loginName, employees)` = `DIPLOMA`, nếu rỗng thì `IDENTIFICATION_NUMBER`; `FirstNotEmpty(params string[])` thay chuỗi `??` | XML1, XML7–XML11 |
| `EfModelReader` | `decimal? GetDecimal(object entity, string propertyName)` (cache `PropertyInfo` theo kiểu) | XML1 (T_BNCCT_LK) |
| `LineMoneyCalculator` (đợt 2) | `Calculate(LineMoneyInput) → LineMoneyResult` | XML2, XML3 |
| `CoPaymentCapCalculator` (đợt 2) | `GetBaseSalary(List<HIS_BHYT_PARAM>, long outTime)`; `Apply(totalBncct, totalBhtt, bncctLk, baseSalary, isFiveYears)` | XML1 |

Mọi phương thức có try-catch, ghi `LogSystem.Warn` kèm `TraceData` mã điều trị; không ghi CCCD hay số thẻ. Khi lỗi thì trả về kết quả của logic cũ.

### 5.4. MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã

`TreatmentTypeCodeResolver.Resolve`:

```
codes = alters.OrderBy(LOG_TIME)
              .Select(a => ResolveSingle(treatment, a.TREATMENT_TYPE_ID, a.TREATMENT_TYPE_CODE, ...))
              .Where(not empty).Distinct (giữ thứ tự)
nếu codes rỗng → [ResolveSingle(treatment)]           // giữ hành vi hiện tại
áp quy tắc KSK "100" như XML1:620 trên từng mã
```

Các chỗ so sánh phải đổi (chỉ trong `if (IS_3176)`):

| Vị trí | Hiện tại | Sau khi sửa |
|---|---|---|
| X1:646-647 | `MA_LOAI_KCB == "02"/"05"/"08"` → xóa NGAY_VAO_NOI_TRU | `!codes.Intersect(NoiTruTreatmentTypeCodes).Any()` → xóa (GĐ4) |
| X1:653-659 | Nhóm mã → SO_NGAY_DTRI | Dùng `codes.Last()` (GĐ4) |
| HS:844 `CreateXml1InTreatment` | Như trên | Như trên |
| X8:30-97, 260-299 | Tự tính 1 mã | `string.Join(";", Resolve(...))` |
| X14:33-36, 301 | Tự tính | Dùng resolver |
| CI:308-392, 301 | Tự tính 1 mã | Dùng resolver (cần `ListPatientTypeAlter` trong input Check-in) |

Không đổi `XMLTT12.XML01BH` (chuẩn khác), chỉ ghi chú để theo dõi.

### 5.5. XML1

**Thứ tự thẻ.** `XML1Data` từ 66 lên **68** phần tử, khớp STT trong chuẩn:

| Order | Trường | Ghi chú |
|---|---|---|
| 1–28 | MA_LK … MA_BENH_KT | Giữ nguyên |
| **29** | **MA_BENH_KHAC** (string, mới) | |
| 30–50 | MA_BENH_YHCT … T_BNTT | Cũ 29–49, +1 |
| **51** | **T_BNCCT_LK** (string, mới) | |
| 52–68 | T_BNCCT … DU_PHONG | Cũ 50–66, +2 |

Ở chế độ 130, 2 property mới để `null` nên `XmlSerializer` không ghi thẻ, file không đổi. `Xml1ADO`/`HoSoProcessor` (bản hiển thị) bổ sung tương ứng.

**Thay đổi trong `if (data.IS_3176)`** (khối X1:827-861):

| Trường | Thiết kế |
|---|---|
| MA_BENH_KHAC | Tách `ListTracking[].ICD_DIFF_CODE` theo `;`, trim, bỏ trùng, loại mã có trong MA_BENH_CHINH/MA_BENH_KT, nối `;`. Không có thì `""` (chuẩn: "không phát sinh thì để trống") |
| T_BNCCT_LK | `EfModelReader.GetDecimal(alter, "CO_PAID_ACCUMULATE_AMOUNT")` theo GĐ6, định dạng `G27` InvariantCulture. Không có cột hoặc không có giá trị thì `"0"`; BA xác nhận `"0"` hay để trống |
| MAHUYEN_CU_TRU, T_THUOC, T_VTYT | `Xml3176Rules.NullValue()` |
| MA_LOAI_KCB, MA_DOITUONG_KCB | Mục 5.4 |
| MA_KHOA | Luôn dùng nhánh nhiều khoa (logic `MA_KHOA_OPTION=1`, X1:349-395) và bỏ trùng theo **mã BHYT** |
| GIAY_CHUYEN_TUYEN | Bỏ ưu tiên `OUT_CODE`: lấy `TRANSFER_IN_CODE`. Số phiếu hẹn khám lại để đợt 3 (chưa có nguồn) |
| CHAN_DOAN_VAO | Fallback `IN_ICD_NAME` thay cho `ICD_NAME` |
| MA_TTDV | `SignerCodeResolver.GetLicenseOrIdentity(FirstNotEmpty(HOSP_SUBS_DIRECTOR_LOGINNAME, HOSPITAL_DIRECTOR_LOGINNAME), Employees)` |
| SO_CCCD | `FirstNotEmpty(CCCD, CMND, PASSPORT)`: giữ thứ tự hiện tại nhưng bỏ qua chuỗi rỗng. Bỏ CMND hay không chờ BA |
| T_BNCCT, T_BHTT | Đợt 2, mục 5.9 |

### 5.6. XML7

`XML7Data` từ 26 lên **27** phần tử: chèn **`NGAY_VAO_NOI_TRU` Order 6**, các trường từ NGAY_RA trở đi +1. Ở chế độ 130 property để `null`. Bổ sung `XML7ADO.ngayVaoNoiTru` và map trong `MapADOToXml` (X7:52-86).

| Trường | Thiết kế (trong `if (IS_3176)`) |
|---|---|
| NGAY_VAO_NOI_TRU | `CLINICAL_IN_TIME` 12 ký tự (như XML13:117). Áp `NGAY_VAO_NOI_TRU_OPTION` giống XML14:34-39 |
| MA_TTDV | `SignerCodeResolver`, giữ fallback `REPRESENTATIVE_HEIN_CODE` khi không có login giám đốc |
| GHI_CHU | Giữ `ADVISE`, chờ Q13 |

### 5.7. XML2 (đợt 1)

| Trường | Thiết kế (trong `if (IS_3176)`) |
|---|---|
| HAM_LUONG | `""` khi `TDL_HEIN_SERVICE_TYPE_ID` ∈ {máu, chế phẩm máu} (`IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE`). Khí y tế, vị thuốc YHCT để sau khi BA chốt tiêu chí |
| MA_CSKCB_THUOC | Có giá trị và chưa có tiền tố thì thêm `"C."` (như TT12 `Mau04Processor.cs:160`) |
| MA_DICH_VU | DV cha `IS_NO_EXECUTE == 1` thì thêm `"_TB"` và **không bỏ dòng thuốc** (bỏ qua `KHONG_LAY_DVU_KHONG_THUC_HIEN`) |
| SO_DANG_KY | Bỏ khoảng trắng |

### 5.8. XML6, XML8–XML14, Check-in

| Bảng | Trường | Thiết kế (trong `if (IS_3176)`) |
|---|---|---|
| XML6, XML9, XML12 | MAHUYEN_CU_TRU | `Xml3176Rules.NullValue()`. XML6:144 bỏ `?? ""` để không đổi `null` thành `""`. XML12 cần cờ `IS_3176` (mục 5.1) |
| XML8 | MA_LOAI_KCB | Resolver (5.4) |
| XML8, XML9, XML10, XML11 | MA_TTDV | `SignerCodeResolver` |
| XML10 | CHAN_DOAN_RV | `ICD_NAME;ICD_TEXT` (như nhánh 130 X10:39-44). Chưa chứa "dưỡng thai" (không phân biệt hoa thường) thì nối `"; dưỡng thai"`. Thay dòng gán cứng X10:98 |
| XML11 | HO_TEN_CHA, HO_TEN_ME | Chỉ ghi khi tuổi < 7 tại ngày chứng từ (chờ Q14); ngược lại `""` |
| XML13 | MA_BAC_SI | `Xml3176Rules.NullValue()` |
| XML14 | NGAY_HEN_KL | `APPOINTMENT_TIME` **12 ký tự** (thay X14:326 cắt 8) |
| XML14 | MA_DOITUONG_KCB | Resolver (5.4) |
| XML14 | MA_TTDV | `Xml3176Rules.NullValue()` |
| Check-in | MA_LOAI_KCB, MA_DOITUONG_KCB | Resolver; input thêm `ListPatientTypeAlter` (plugin truyền danh sách đã nạp) |

### 5.9. Tính tiền (đợt 2)

**Dòng XML2/XML3: `LineMoneyCalculator`**

```
Input: THANH_TIEN_BV, THANH_TIEN_BH, TYLE_TT_BH, HEIN_RATIO, HEIN_CARD_NUMBER,
       MA_DOITUONG_KCB, NGUON_KHAC
1. MUC_HUONG_THE = GetDefaultHeinRatio(HEIN_CARD_NUMBER) * 100        // 80/95/100
2. MUC_HUONG_BH  = Xml3176Rules.MucHuongBhByObjectCode[MA_DOITUONG_KCB]
                   ?? round(HEIN_RATIO*100 / MUC_HUONG_THE * 100)
   - HEIN_RATIO == 1 (ca được 100%) → MUC_HUONG_BH = 100
3. MUC_HUONG (%) = MUC_HUONG_BH * MUC_HUONG_THE / 100                 // ví dụ 40*80/100 = 32
4. T_BHTT  = round(THANH_TIEN_BH * MUC_HUONG/100, 2)                   // giữ như hiện tại
5. T_BNCCT = max(0, round(THANH_TIEN_BH * MUC_HUONG_BH/100, 2) - T_BHTT)
6. T_BNTT  = max(0, THANH_TIEN_BV - T_BHTT - T_BNCCT)
7. Giảm trừ NGUON_KHAC lần lượt: T_BNTT → T_BNCCT → T_BHTT, không ghi đè T_NGUONKHAC
8. Xuất MUC_HUONG theo Xml3176Rules.MucHuongScale (32 hoặc 0.32)
```

Hiện tại ở XML2 (X2:676-767) và XML3 (X3:739-788, 1664-1792), các nhánh cấu hình tính lại tiền (mục 4) đều tự tính tiền. Trong `if (IS_3176)`, toàn bộ các nhánh này được **thay** bằng một lời gọi `LineMoneyCalculator`. Ở chế độ 130 chúng giữ nguyên.

**XML1: `CoPaymentCapCalculator`** (sau khi đã có tổng)

```
baseSalary = GetBaseSalary(ListBhytParam, OUT_TIME)  // không tìm được → bỏ bước này, log Warn
cap = 6 * baseSalary
fiveYears = JOIN_5_YEAR_TIME != null && JOIN_5_YEAR_TIME <= OUT_TIME
if fiveYears && (T_BNCCT_LK + tongBNCCT) > cap:
    newBncct = max(0, cap - T_BNCCT_LK)
    T_BHTT  = tongBHTT + (tongBNCCT - newBncct)
    T_BNCCT = newBncct
```

Ghi log Debug với mã điều trị, cap, T_BNCCT_LK, tổng trước/sau. Vì XML1 ≠ tổng XML2 + XML3 sau khi áp trần, cần BA/BHXH xác nhận (GĐ7).

### 5.10. Lỗi sẵn có: phạm vi sửa

| Lỗi | Phạm vi sửa | Lý do |
|---|---|---|
| X10:98 CHAN_DOAN_RV cứng | Nhánh 3176 | Nhánh 3176 đang sai rõ |
| X3:532, 557, 1443 ưu tiên toán tử `_GT` | Toàn cục, **chờ duyệt** | Có thể đổi file của viện đang dựa vào hành vi sai |
| `UCExportXml_KiemTraHoSo.cs:475` gán cứng `IS_3176 = false` | Plugin, dùng chế độ đang chọn | Kiểm tra hồ sơ ở chế độ 3176 đang kiểm theo chuẩn 130 |
| XML12 không nhận `IS_3176` | Thư viện (mục 5.1) | Thiếu cờ |
| CI:186 SO_CCCD lặp | Toàn cục | Không đổi file hợp lệ |
| X11:101 vượt chỉ số; X13:49 Parse; X2:549, 681; X3:750; CI:204, 368 null | Toàn cục | Chỉ chặn exception |
| X4:355, X5:361 `\|\|` | Toàn cục | Chỉ chặn exception |
| X2:691, 720 nguồn khác | Nhánh 3176 (đợt 2) | Đổi tiền |
| X3:574 MA_GIUONG `"."` | Nhánh 3176 → `""` | Đổi file |

### 5.11. Viewer XMLViewer130

Không sửa code. Grid tự sinh cột từ `XML1Data`/`XML7Data`, chỉ cần deploy DLL thư viện mới. Kiểm thử mở file cũ và file mới (mục 7).

### 5.12. Logging, đa ngôn ngữ, tài liệu

- **Plugin:** giữ log chế độ hiện có (`UCExportXml.cs:1750-1754`). Log Info một lần khi nạp `HIS_BHYT_PARAM` (số bản ghi, FROM_TIME/TO_TIME/BASE_SALARY). Không có thông báo mới cho người dùng nên không thêm Resources.
- **Thư viện:** `LogSystem.Warn(ex)` trong mọi hàm mới, TraceData mã điều trị. Không log CCCD, số thẻ, `DIPLOMA`.
- **Tài liệu:** cập nhật `docs/HIS.Desktop.Plugins.ExportXmlQD130.md` (mục mới, Changelog, Test Cases).

---

## 6. Build, triển khai, tương thích

| Hạng mục | Nội dung |
|---|---|
| DLL thư viện thay đổi | `His.Bhyt.ExportXml.XML130.Common` (**mới**), `.XML130`, `.XML1`–`.XML14` (trừ XML4, XML5, XML15 nếu đợt 1 không đụng), `His.Bhyt.ExportXml` (Check-in) |
| DLL plugin | `HIS.Desktop.Plugins.ExportXmlQD130` |
| Build | Theo chuẩn hiện tại của team (.NET Framework 4.5, x64 cho plugin). Project Common thêm vào solution His.Bhyt và vào ReferencedAssemblies của bản deploy |
| EFMODEL | Không bắt buộc nâng. Viện có EFMODEL trước 29/10/2026 thì T_BNCCT_LK = `"0"` và ghi log Warn một lần mỗi phiên |
| Cấu hình | **Không có script SQL** |
| Áp dụng | Ngay khi deploy DLL thư viện, cho mọi viện đang xuất/gửi ở chế độ 3176. Thống nhất thời điểm deploy với mốc BHXH và báo trước các viện dùng 3176 |
| Quay lui | Deploy lại DLL thư viện bản trước. **Lưu bản DLL hiện tại trước khi deploy** |
| Tương thích máy trạm | Plugin mới + thư viện cũ: thiếu property mới (`ListBhytParam`, `ListPatientTypeAlter`) sẽ gây `MissingMethodException`. Bọc lời gán trong hàm `NoInlining` (mục 5.2). Thư viện mới + plugin cũ: list mới để null → bỏ bước trần lương cơ sở, Check-in dùng logic một mã |

---

## 7. Kiểm thử

### 7.1. So sánh file (bắt buộc cho mỗi commit)

1. Chọn bộ khoảng 50 hồ sơ mẫu: khám, ngoại trú, nội trú, chuyển viện, sinh, nghỉ dưỡng thai, nghỉ BHXH, GĐYK, lao, HIV, có nguồn khác, đi KCB không đúng quy định, đổi diện, đổi đối tượng.
2. Xuất chế độ **130** trước và sau khi sửa: file phải **giống hệt từng byte**, trừ các lỗi sẵn có đã duyệt sửa toàn cục (mục 5.10).
3. Xuất chế độ **3176** trước và sau khi sửa: diff chỉ được khác ở các trường liệt kê trong mục 5. Lưu bộ file "trước" làm mốc đối chiếu.

### 7.2. Test case nghiệp vụ

- [ ] Chế độ 130: mọi luồng (Xuất, Gửi tay/tự động, 48 giờ, Kiểm tra hồ sơ, Gộp, GĐYK, Excel, Tiền giám định) như hiện tại.
- [ ] Chế độ 3176 (nút Xuất XML 3176 và đồng bộ tự động `isXML3176`): ra chuẩn sửa đổi.
- [ ] Kiểm tra hồ sơ ở chế độ 3176 kiểm theo chuẩn 3176 (sau khi bỏ gán cứng `IS_3176 = false`).
- [ ] XML1 có MA_BENH_KHAC (tờ điều trị có chẩn đoán phân biệt, không trùng bệnh chính/kèm), T_BNCCT_LK; thứ tự thẻ đúng STT 29, 51; tổng 68 thẻ.
- [ ] XML7 có NGAY_VAO_NOI_TRU ở vị trí 6.
- [ ] MAHUYEN_CU_TRU (XML1, 6, 9, 12), T_THUOC, T_VTYT, XML13 MA_BAC_SI, XML14 MA_TTDV không có thẻ (GĐ3).
- [ ] Người ký không có GPHN: MA_TTDV = CCCD (XML1, 7–11); có GPHN: như cũ.
- [ ] Khám rồi nhập viện nội trú: MA_LOAI_KCB `01;03` ở XML1, XML8, Check-in; SO_NGAY_DTRI tính theo mã cuối; NGAY_VAO_NOI_TRU giữ nguyên.
- [ ] Đổi đối tượng giữa đợt: MA_DOITUONG_KCB nhiều mã ở XML1, XML14, Check-in và giống nhau giữa các bảng.
- [ ] XML10 chẩn đoán đầy đủ kèm "dưỡng thai"; XML11 cha/mẹ chỉ khi trẻ dưới 7 tuổi; XML14 NGAY_HEN_KL 12 ký tự.
- [ ] XML2: máu/CPM không có HAM_LUONG; thuốc của DVKT không thực hiện có `_TB` và không bị bỏ dòng; MA_CSKCB_THUOC có `C.`.
- [ ] Máy trạm EFMODEL cũ: xuất không lỗi, T_BNCCT_LK = `"0"`.
- [ ] Máy trạm thư viện cũ + plugin mới: chế độ 130 và 3176 vẫn xuất được.
- [ ] Viewer XMLViewer130 mở file mới (có cột mới) và file cũ.
- [ ] **Đợt 2:** ví dụ A, B (tài liệu phân tích mục 4.1.2) ra đúng số; ca đúng quy định không đổi; ca vượt trần 6 tháng lương cơ sở bị chặn trần ở XML1; hồ sơ ra viện ở 2 mốc lương cơ sở khác nhau lấy đúng `HIS_BHYT_PARAM`.

---

## 8. Rủi ro

| Rủi ro | Mức | Giảm thiểu |
|---|---|---|
| Không có ngày hiệu lực: deploy sớm hơn mốc BHXH thì gửi chuẩn mới quá sớm; xuất lại hồ sơ cũ ra chuẩn mới | **Cao** | Deploy đúng mốc; báo trước các viện dùng 3176; lưu DLL cũ để quay lui |
| Công thức tiền mới lệch bảng kê MOS, BHXH từ chối | Cao | Đợt 2 chỉ làm sau khi chốt Q2, Q3, Q8; phát hành riêng với đợt 1 |
| Không tắt được bằng cấu hình khi có sự cố | Trung bình | Quay lui bằng DLL cũ; so sánh file trước khi phát hành (7.1) |
| Bảng tra MUC_HUONG_BH không đầy đủ → fallback suy ngược sai ở ca được 100% | Trung bình | Log Warn khi phải dùng fallback; báo cáo danh sách mã đối tượng thiếu |
| Lệch phiên bản thư viện giữa các máy trạm | Trung bình | NoInlining (mục 5.2); deploy đồng bộ toàn bộ DLL His.Bhyt |
| Danh mục ĐVHC chưa theo QĐ 19/2025 | Trung bình | Kiểm dữ liệu từng viện trước khi deploy (Q16) |
| Cách hiểu "để null" sai với cổng | Thấp | Đổi `Xml3176Rules.NullValue()`, phát hành bản vá |

---

## 9. Đầu việc

| # | Việc | Phụ thuộc | Độ lớn |
|---|---|---|---|
| T1 | Tạo project Common + `Xml3176Rules`, `SignerCodeResolver`, `EfModelReader` | — | Nhỏ |
| T2 | XML12 nhận `IS_3176`; thêm `ListTracking`, `ListBhytParam` (XML1, XML130), `ListPatientTypeAlter` (Check-in); gán ở HoSoProcessor/Plus | T1 | Nhỏ |
| T3 | Plugin: nạp `HIS_BHYT_PARAM`, gán list mới ở 6 chỗ dựng InputADO + Check-in, NoInlining; bỏ gán cứng `IS_3176 = false` ở Kiểm tra hồ sơ | T2 | Nhỏ |
| T4 | XML1: 2 trường mới, Order, trường null, MA_KHOA, GIAY_CHUYEN_TUYEN, CHAN_DOAN_VAO, MA_TTDV, SO_CCCD | T1, T2 | Vừa |
| T5 | XML7: NGAY_VAO_NOI_TRU, MA_TTDV | T1 | Nhỏ |
| T6 | `TreatmentTypeCodeResolver`, `HeinObjectCodeResolver`; áp vào XML1, 8, 14, Check-in | T1, T2 | Vừa |
| T7 | XML2 đợt 1; XML6, 9–14 (mục 5.8) | T1, T2 | Vừa |
| T8 | Lỗi sẵn có (mục 5.10) | — | Nhỏ |
| T9 | Cập nhật docs plugin | — | Nhỏ |
| T10 | Đợt 2: `LineMoneyCalculator`, `CoPaymentCapCalculator`; thay các nhánh tiền XML2/XML3 trong `if (IS_3176)`; Tiền giám định | Q2, Q3, Q7, Q8 | **Lớn** |
| T11 | Kiểm thử so sánh file + test case mục 7 | T1–T10 | Vừa |

---

## 10. Cần duyệt

1. Hệ quả của D1, D2 (mục 2.2): chuẩn sửa đổi thay thế 3176 cũ, áp dụng từ khi deploy, quay lui bằng DLL cũ.
2. D3 (project dùng chung mới) và D7 (lương cơ sở từ `HIS_BHYT_PARAM`).
3. Các giả định GĐ1–GĐ9 (mục 3), hoặc câu trả lời chính thức cho Q1–Q9.
4. Danh sách lỗi sẵn có được sửa toàn cục (mục 5.10), đặc biệt `_GT` ở XML3.

---

## 11. Lịch sử thay đổi tài liệu

| Ngày | Nội dung |
|---|---|
| 26/11/2026 | Bản đầu: cờ `IS_3176_AMENDED`, ngày hiệu lực, 5 khóa cấu hình mới, gom 6 chỗ dựng InputADO |
| 27/11/2026 (chiều) | **Đã code đợt 1** (T1, T2, T4–T9). Lệch so với thiết kế: (1) plugin không sửa: `CheckHoSo` của thư viện không dùng cờ `IS_3176`, nên dòng gán cứng `IS_3176 = false` ở Kiểm tra hồ sơ không ảnh hưởng kết quả; `ListBhytParam` để đợt 2. (2) Check-in chưa áp MA_LOAI_KCB / MA_DOITUONG_KCB nhiều mã: input Check-in chưa có cờ `IS_3176` và dùng `V_HIS_TREATMENT_3`; chỉ sửa lỗi `SO_CCCD` và null. (3) Lỗi XML4/XML5 `\|\|` sửa thành `&&` (kết quả như cũ vì danh sách rỗng không sinh dòng) |
| 27/11/2026 | Chuyển sang **phương án A**: sửa thẳng nhánh `IS_3176`, bỏ cờ mới và cả 5 khóa cấu hình; giả định viết cứng trong `Xml3176Rules`; lương cơ sở lấy từ `HIS_BHYT_PARAM`; bỏ việc gom InputADO; thêm sửa lỗi Kiểm tra hồ sơ gán cứng `IS_3176 = false`; cập nhật số dòng theo code ngày 27/11 |
