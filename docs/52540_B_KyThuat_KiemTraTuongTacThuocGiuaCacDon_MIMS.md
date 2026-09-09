# Việc 52540 — TÀI LIỆU KỸ THUẬT

## Kiểm tra tương tác thuốc giữa các đơn khác nhau của hồ sơ bằng MIMS

| Thông tin | Nội dung |
|---|---|
| Mã việc | 52540 |
| Tên việc | Kiểm tra tương tác giữa các thuốc trong các đơn khác nhau của hồ sơ bằng MIMS |
| Phạm vi plugin | `HIS.Desktop.Plugins.AssignPrescriptionPK`, `...CLS`, `...Kidney`, `...YHCT` |
| Thư viện dùng chung | `HIS/HIS.Desktop.MIMS.Integration` (6 plugin đang tham chiếu) |
| Config điều khiển hiện có | `HIS.Desktop.Plugins.AssignPrescription.ConnectDrugInterventionInfo` |
| Hiện trạng | MIMS chỉ kiểm tra tương tác **trong phạm vi đơn đang kê** |
| Mục tiêu | Kiểm tra tương tác giữa thuốc đang kê và thuốc **các đơn khác còn hiệu lực** của hồ sơ |
| Backend | **Không cần endpoint mới** — dùng `api/HisExpMestMedicine/GetView` sẵn có |
| Loại thay đổi | Thư viện MIMS + 4 plugin + 3 config `HIS_CONFIG` |

**Tài liệu này dành cho lập trình viên.** Quy tắc nghiệp vụ tham chiếu bằng mã `QT-xx` (Phần 3), giả định cần chốt bằng mã `GD-xx` (Phần 2.3).

---

# PHẦN 1. HIỆN TRẠNG

## 1.1 Cấu hình `ConnectDrugInterventionInfo`

Khai báo tại `Config/HisConfigCFG.cs` (cả 4 plugin dùng **chung một key**):

```
CONFIG_KEY__CONNECT_DRUG_INTERVENTION_INFO = "HIS.Desktop.Plugins.AssignPrescription.ConnectDrugInterventionInfo"
    → internal static string ConnectDrugInterventionInfo    (nạp trong LoadConfig() bằng GetValue → HisConfigs.Get<string>)
```

| Giá trị | Ý nghĩa | Vị trí xử lý (bản PK) |
|---|---|---|
| `"1"` | Dùng thư viện cũ `HIS.Desktop.Plugins.Library.DrugInterventionInfo`, endpoint lấy từ `MOS.HIS_DRUG_INTERVENTION.CONNECTION_INFO` | `frmAssignPrescription.cs:3800` — `CheckMediMatyType()` |
| `"2"` | Dùng **MIMS** (`HIS.Desktop.MIMS.Integration`) | `frmAssignPrescription.cs:2591` (lưu đơn), `frmAssignPrescription__InitMenuMouseRight.cs:102` (menu chuột phải), `frmAssignPrescription__MimsPatientProfile.cs:36` (prefetch PatientProfile) |
| rỗng / khác | Không kiểm tra tương tác | — |

Các config MIMS liên quan:

| Key | Kiểu | Ý nghĩa |
|---|---|---|
| `HIS.Desktop.Mims.IsCheckPregnancyLactation` | `"1"` | Gửi khối `<PatientProfile>` (PN mang thai / cho con bú) |
| `HIS.Desktop.Plugins.AssignPrescriptionPK.IsCheckPreviousPrescriptionDetail` | `"1"` | Nạp `ListMedicineTypePreviousPrescription` — **chỉ để cảnh báo trùng TÊN thuốc**, không đưa vào MIMS |
| `HIS.Desktop.Plugins.AssignPrescription.BlockingInteractiveGrade` | số | Mức chặn tương tác của luồng ACIN nội bộ (`Worker/ValidAcinInteractiveWorker.cs:176`) — **không áp dụng cho MIMS** |

Cấu hình kết nối MIMS nằm trong `App.config` (đọc qua `Core/MimsConfig.cs`): `MIMS.Username`, `MIMS.Password`, `MIMS.CDS.ApiUrl`, `MIMS.VNContra.ApiUrl`, `MIMS.Resource.BasePath`, `MIMS.StyleSheet.File`.

## 1.2 Luồng MIMS hiện tại khi lưu đơn

```
ProcessSaveForListSelect()                          frmAssignPrescription.cs:2553
 ├── mediMatyTypeADOs = gridViewServiceProcess.DataSource
 ├── if (ConnectDrugInterventionInfo == "2" && !CheckMIMS(mediMatyTypeADOs)) return;      :2591
 │
 └── CheckMIMS(lstMediMatyTypeADOs)                 frmAssignPrescription.cs:14148
      ├── foreach item in lstMediMatyTypeADOs → DrugItem(MEDICINE_TYPE_CODE, null, null, MimsType)
      │        (MimsType truyền vào KHÔNG có tác dụng — MappingMIMS tự tra lại MIMS_TYPE)
      ├── lstICD = txtIcdCode + txtIcdSubCode(;) + txtIcdCodeCause(;)
      ├── mimsInteractionLog = new HIS_MIMS_INTERACTION_LOG()
      ├── mimsProfile = BuildMimsPatientProfile()
      └── DrugHealthService.CheckAndAlert(lstDrugItem, lstICD, mimsInteractionLog, patientProfile: mimsProfile)

DrugHealthService.CheckAndAlert()                   Modules/DrugHealthService.cs
 ├── Check() → MappingMIMS(drugs)                   Core/BaseService.cs:28
 │      MEDICINE_TYPE_CODE → V_HIS_MEDICINE_TYPE.ATC_CODES → HIS_ATC.MIMS_GUID
 │                         → HIS_MEDICINE_TYPE_ACIN → HIS_ACTIVE_INGREDIENT.MIMS_GUID
 │      (1 thuốc HIS có thể giãn thành NHIỀU GUID; trùng GUID bị loại bằng HashSet)
 ├── MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, allergies, icd, true, true, patientProfile)
 │      → <Request><Interaction><Prescribing>...</Prescribing><HealthIssueCodes/>
 │        <Allergies/><References/><DuplicateTherapy/><DuplicateIngredient/></Interaction>
 │        <PatientProfile/></Request>
 ├── MimsClient.PostXml(MimsConfig.CdsApiUrl, xml)  → POST form: prescriptionquery, responsetype=xml
 │      Timeout 15s / ReadWriteTimeout 15s, TLS 1.2, Basic credential
 ├── Parse: DrugHealthAlertDetails, DrugDrugAlertDetails, PregnancyAlertDetails, LactationAlertDetails
 ├── hasCdsAlert? → WebViewHelper.ShowDialog(result.Html)   → "Xác nhận" = true (tiếp tục lưu)
 │                                                          → "Bỏ qua"/đóng = false (HUỶ lưu)
 ├── không có alert CDS → CheckVnContraindication(drugs) (theo ATC) → popup VN nếu có
 └── chỉ khi IsTimeout / IsErrorResponse mới popup thông báo lỗi kết nối
```

Ghi log: `mimsInteractionLog` được gán `TREATMENT_ID / PATIENT_ID / SERVICE_REQ_ID / EXP_MEST_ID` và `POST api/HisMimsInteractionLog/Create` tại `frmAssignPrescription__Save.cs:1260-1276`.

## 1.3 Menu chuột phải "Đánh giá thông tin thuốc"

`frmAssignPrescription__InitMenuMouseRight.cs`

| Điều kiện | Menu | Hành động |
|---|---|---|
| `ConnectDrugInterventionInfo == "2"` và chọn **1** dòng | "Thông tin thuốc" | `DrugInfomationService` |
| `ConnectDrugInterventionInfo == "2"` và chọn **>1** dòng | "Đánh giá thông tin thuốc" | `DrugHealthService.ShowResultAsync(lstDrugItem, lstICD, BuildMimsPatientProfile())` — `:195-232` |

Cũng **chỉ gửi các dòng đang chọn trong grid đơn hiện tại**.

## 1.4 Dữ liệu "đơn khác" đã có sẵn một phần

`GetHisExpMestMedicine()` — `frmAssignPrescription.cs:14114`

```csharp
searchMedicineFilter.USE_TIME_TO_FROM = <ngày chỉ định>000000;
searchMedicineFilter.TDL_PATIENT_ID   = VHistreatment.PATIENT_ID;   // ← theo BỆNH NHÂN, không theo hồ sơ
searchMedicineFilter.IS_INCLUDE_DELETED = false;
searchMedicineFilter.DATA_DOMAIN_FILTER = false;
ListMedicineTypeOld = Get<List<V_HIS_EXP_MEST_MEDICINE>>(HIS_EXP_MEST_MEDICINE_GETVIEW, ...);
// lọc tiếp: USE_TIME_TO >= giờ chỉ định  →  ListMedicineTypePreviousPrescription
```

Gọi tại 3 chỗ, đều bọc `if (HisConfigCFG.CheckPreviousPrescriptionDetail == "1")`: Load form (`:1236`), đổi ngày chỉ định (`:10969`), đổi giờ chỉ định (`:13408`).

Kết quả **chỉ dùng để so trùng tên thuốc** rồi hiện `MessageBox` "Bệnh nhân đã có đơn thuốc cũ còn sử dụng tới ngày ..." (`:3704`). Không liên quan MIMS.

## 1.5 Khả năng của MIMS đã được xác nhận từ SDK

Nguồn: `HIS.Desktop.MIMS.Integration/_Documents/MIMS_Integrate/`

| Phát hiện | Nguồn | Kết luận |
|---|---|---|
| `<Interaction>` là `xsd:all` gồm `Prescribing` (bắt buộc), `Prescribed` (`minOccurs=0`), `Allergies`, `HealthIssues`, `HealthIssueCodes`, `Food`, `References`, `CautionaryLabels`, `DuplicateTherapy`, `DuplicateIngredient` | `API Guide/XML Documentation/Request55.xsd` | Thứ tự thẻ **không quan trọng**; `<Prescribed>` là thẻ hợp lệ |
| `<PatientProfile>` là con trực tiếp của `<Request>`, ngang hàng `<Interaction>` | `Request55.xsd` | Code hiện tại **đúng** |
| Drug-Health Alert và Duplicate Alert: khối `<Prescribing>` được chú thích **"Current and Past Medications"** | `API Guide/MIMSIntegrated_APIGuide_7Modules.pdf` (trang 11, 14) | MIMS khuyến nghị đưa **cả thuốc cũ** vào `<Prescribing>` |
| Active Medication Logic: `End Date > Current Date` = Active Drug; `End Date < Current Date` = Stopped Drug | `API Guide/MIMS Integrated Guide_ActiveMedications and AlertFilter Logic.pdf` | Xác nhận quy tắc lọc `USE_TIME_TO` |
| Tham số form tuỳ chọn `alertfilterbydrug` = `<GUIDS><GUID>{...}</GUID></GUIDS>` — chỉ hiện alert liên quan các GUID chỉ định | cùng PDF trên + `FastTrack API Lib and Sample Code/Sample Source Code/.Net/Sample_ClientCode.xml:69-102` | Cơ chế **chính thống** để loại nhiễu cặp "cũ–cũ" |
| Tham số `alertfilterbyseverity` = `<WARNINGIDS><WARNINGID>D2D:Severe</WARNINGID>...</WARNINGIDS>` | cùng nguồn | Lọc theo mức nghiêm trọng |
| **`alertfilterbytemplate` (mặc định "Yes") KHÔNG dùng đồng thời với `alertfilterbydrug`** | `Sample_ClientCode.xml:104` | Rủi ro cần xử lý — xem `RR-03` |

`MimsClient.PostXml` hiện chỉ gửi 2 tham số form `prescriptionquery` + `responsetype`, **chưa hỗ trợ** tham số lọc.

## 1.6 Khoảng trống so với yêu cầu 52540

| # | Khoảng trống | Mức |
|---|---|---|
| KT-01 | Request MIMS chỉ chứa thuốc đơn đang kê → không thể phát hiện tương tác chéo đơn | CRITICAL |
| KT-02 | Không có nguồn dữ liệu thuốc các đơn khác **theo hồ sơ điều trị** (`TDL_TREATMENT_ID`) | CRITICAL |
| KT-03 | Popup MIMS (XSL mặc định) không phân biệt thuốc đơn hiện tại vs đơn khác → người dùng không biết xử lý đơn nào | HIGH |
| KT-04 | Chưa có cơ chế chống nhiễu cặp tương tác giữa 2 thuốc **đều thuộc đơn cũ** (bác sĩ không sửa được ở form này) | HIGH |
| KT-05 | Menu chuột phải "Đánh giá thông tin thuốc" cũng chỉ trong 1 đơn | MEDIUM |
| KT-06 | Log `HIS_MIMS_INTERACTION_LOG`: `DRUG_COUNT` / `CHECKED_GUIDS` chỉ tính thuốc đơn hiện tại → không audit được phạm vi kiểm tra | MEDIUM |
| KT-07 | `CheckMIMS()` bị nhân bản ở 4 plugin — sửa phải đồng bộ 4 chỗ | MEDIUM |

---

# PHẦN 2. YÊU CẦU & GIẢ ĐỊNH

## 2.1 Yêu cầu chức năng

| Mã | Yêu cầu |
|---|---|
| YC-01 | Khi lưu đơn (config `= "2"`), MIMS phải kiểm tra tương tác giữa thuốc **đang kê** và thuốc **các đơn khác còn hiệu lực trong cùng hồ sơ** |
| YC-02 | Chỉ hiển thị cảnh báo có liên quan tới ≥ 1 thuốc của **đơn đang kê**; không hiển thị cặp tương tác giữa 2 thuốc đều thuộc đơn khác |
| YC-03 | Popup phải chỉ rõ thuốc nào thuộc đơn khác, thuộc đơn nào, dùng đến ngày nào |
| YC-04 | Bật/tắt được theo bệnh viện; mặc định TẮT — giữ nguyên hành vi hiện tại |
| YC-05 | Không thêm request MIMS mới, không tăng thời gian lưu đơn quá 1 lần gọi API nội bộ (đã prefetch) |
| YC-06 | Menu chuột phải "Đánh giá thông tin thuốc" áp dụng cùng phạm vi |
| YC-07 | Ghi audit đầy đủ phạm vi kiểm tra vào `HIS_MIMS_INTERACTION_LOG` |
| YC-08 | Áp dụng cho cả 4 plugin kê đơn |

## 2.2 Ngoài phạm vi

- Không kiểm tra tương tác với thuốc bệnh nhân **tự mua ngoài** không có trong `HIS_EXP_MEST_MEDICINE`.
- Không sửa XSL của MIMS.
- Không thay đổi luồng `ConnectDrugInterventionInfo == "1"` (thư viện cũ).
- Không kiểm tra vật tư (chỉ `DataType` thuốc).

## 2.3 Giả định đang áp dụng — **cần chốt trước khi code**

| Mã | Giả định | Ảnh hưởng nếu sai |
|---|---|---|
| GD-01 | "Hồ sơ" = **1 lần điều trị** → lọc `TDL_TREATMENT_ID = VHistreatment.ID`. Có option mở rộng sang toàn lịch sử bệnh nhân (`InteractionScopeOption = 3`) | Đổi 1 dòng gán filter |
| GD-02 | Tương tác chéo đơn **cảnh báo, cho phép người dùng bấm "Xác nhận" để lưu** — giữ đúng cơ chế `WebViewHelper.ShowDialog` hiện tại, không chặn cứng | Nếu phải chặn cứng: cần thêm nhánh xử lý theo mức nghiêm trọng |
| GD-03 | Khi kê **nhiều bệnh nhân** (`IsSelectMultiPatient`), chỉ kiểm tra theo hồ sơ đang mở (`VHistreatment`) — giữ nguyên hành vi hiện tại của `CheckMIMS` | Nếu phải kiểm tra từng bệnh nhân: `CheckMIMS` phải chạy trong vòng lặp, cần đổi thiết kế phần 5.3 và cân nhắc hiệu năng (n request MIMS) |
| GD-04 | Chỉ tính thuốc đơn khác **còn hiệu lực**: `USE_TIME_TO >= giờ chỉ định` (đúng Active Medication Logic của MIMS). Thuốc `USE_TIME_TO = null` dùng `TDL_INTRUCTION_TIME` trong `PreviousPrescriptionDayRange` ngày | Đổi hàm lọc |
| GD-05 | Làm cho **cả 4** plugin kê đơn | Giảm còn PK nếu chỉ cần PK |

---

# PHẦN 3. QUY TẮC NGHIỆP VỤ

| Mã | Quy tắc |
|---|---|
| QT-01 | Chỉ chạy khi `ConnectDrugInterventionInfo == "2"` **và** `MimsInteractionScopeOption ∈ {2, 3}` |
| QT-02 | Nguồn thuốc đơn khác: `V_HIS_EXP_MEST_MEDICINE` qua `api/HisExpMestMedicine/GetView` |
| QT-03 | Phạm vi: option `2` → `TDL_TREATMENT_ID = VHistreatment.ID`; option `3` → `TDL_PATIENT_ID = VHistreatment.PATIENT_ID` |
| QT-04 | Loại trừ: `IS_DELETE = 1`; `TDL_SERVICE_REQ_ID == oldServiceReq.ID` (đơn đang sửa); `MEDICINE_TYPE_CODE` đã có trong grid đơn hiện tại |
| QT-05 | Còn hiệu lực: `USE_TIME_TO >= giờ chỉ định của đơn đang kê`. Nếu `USE_TIME_TO == null` → `TDL_INTRUCTION_TIME >= giờ chỉ định − PreviousPrescriptionDayRange ngày` |
| QT-06 | `Distinct` theo `MEDICINE_TYPE_CODE`; giữ bản ghi có `USE_TIME_TO` **lớn nhất** để hiển thị |
| QT-07 | Cắt tối đa `PreviousPrescriptionMaxDrug` thuốc, ưu tiên `USE_TIME_TO` giảm dần (thuốc còn dùng lâu nhất). Bị cắt → ghi WARN + đưa số lượng bị cắt vào log |
| QT-08 | Toàn bộ thuốc (đơn hiện tại + đơn khác) đưa vào `<Prescribing>` theo đúng chú thích "Current and Past Medications" của API Guide |
| QT-09 | Gửi kèm `alertfilterbydrug` = danh sách GUID của **thuốc đơn hiện tại** → MIMS chỉ trả alert liên quan thuốc đang kê (thoả YC-02) |
| QT-10 | Nếu `alertfilterbydrug` bị tắt bằng config → lọc phía client: giữ alert có ≥ 1 GUID thuộc đơn hiện tại |
| QT-11 | Chèn khối HTML "Thuốc đang dùng từ đơn khác trong hồ sơ" lên **đầu** HTML MIMS trả về, có tên thuốc / mã đơn / ngày chỉ định / dùng đến ngày |
| QT-12 | Không có thuốc đơn khác nào thoả điều kiện → request và hành vi **giữ nguyên như hiện tại**, không chèn khối HTML |
| QT-13 | Lỗi khi lấy thuốc đơn khác (API lỗi/timeout) → **không chặn lưu đơn**, ghi `LogSystem.Error`, tiếp tục kiểm tra MIMS với phạm vi đơn hiện tại |

---

# PHẦN 4. THIẾT KẾ TỔNG THỂ

## 4.1 Luồng mới

```
Mở form / đổi ngày–giờ chỉ định
 └── PrefetchMimsCrossPrescription()          [Task.Run — KHÔNG chặn UI]
       └── api/HisExpMestMedicine/GetView  (TDL_TREATMENT_ID | TDL_PATIENT_ID)
             → crossPrescriptionMedicines : List<V_HIS_EXP_MEST_MEDICINE>
             → isCrossPrescriptionLoaded = true

Lưu đơn: ProcessSaveForListSelect()
 └── CheckMIMS(mediMatyTypeADOs)
      ├── lstDrugItem      = thuốc đơn hiện tại        (như hiện tại)
      ├── lstPreviousDrug  = BuildCrossPrescriptionDrugItems()   [QT-03..QT-07]
      │      (chưa prefetch xong → lấy đồng bộ 1 lần, giống BuildMimsPatientProfile)
      └── DrugHealthService.CheckAndAlert(
                lstDrugItem, allergies:null, lstICD, mimsInteractionLog,
                patientProfile: mimsProfile,
                previousDrugs: lstPreviousDrug,
                previousDrugNote: <mô tả nguồn đơn để dựng khối HTML>)

DrugHealthService.CheckAndAlert(... previousDrugs ...)
 ├── currentMapped  = MappingMIMS(drugs)              → GUID thuốc đơn hiện tại
 ├── previousMapped = MappingMIMS(previousDrugs)      → GUID thuốc đơn khác
 ├── allMapped      = currentMapped ∪ previousMapped  (Distinct theo MimsGuid)
 ├── BuildDrugHealthAlertRequest(allMapped, ...)      → <Prescribing> gộp   [QT-08]
 ├── MimsClient.PostXml(url, xml, extraFormParams)
 │       alertfilterbydrug = <GUIDS> GUID của currentMapped </GUIDS>        [QT-09]
 ├── Parse + (nếu tắt filter) lọc client theo GUID đơn hiện tại             [QT-10]
 ├── Html = KhốiHtmlThuocDonKhac + Html(MIMS)                               [QT-11]
 └── ShowDialog → true = tiếp tục lưu                                       [GD-02]
```

## 4.2 So sánh phương án gửi thuốc đơn khác

| PA | Cách làm | Ưu | Nhược | Kết luận |
|---|---|---|---|---|
| **PA-A** | Gộp vào `<Prescribing>` + `alertfilterbydrug` | Đúng chú thích "Current and Past Medications" của API Guide cho D2H/Duplicate; MIMS lọc nhiễu phía server; 1 request | Không dùng chung với `alertfilterbytemplate` (`RR-03`) | **CHỌN** |
| PA-B | Đưa vào `<Prescribed>` | Đúng XSD; đã có tiền lệ ở `BuildDrugDrugInteractionRequest` | API Guide 7Modules **không tài liệu hoá** `<Prescribed>` cho Drug-Health / Duplicate → rủi ro MIMS không sinh alert | Dự phòng — bật bằng config `MimsCrossPrescriptionRequestMode = 2`, phải test trên môi trường trial |
| PA-C | Gọi thêm 1 request `DrugDrugInteractionService.Check(current, previous)` | Không sửa builder D2H | 2 request → chậm gấp đôi, 2 popup, mất tab Thai kỳ/Cho con bú và VN Contraindication ở request thứ 2 | Loại |

> Vì PA-B chỉ khác PA-A ở **vị trí thẻ XML**, thiết kế để cả hai dùng chung một tham số `previousDrugs` và chọn bằng config — chi phí thêm rất nhỏ, giảm rủi ro khi MIMS trial cho kết quả khác kỳ vọng.

---

# PHẦN 5. THAY ĐỔI THƯ VIỆN `HIS.Desktop.MIMS.Integration`

> **Nguyên tắc binary-compat (BẮT BUỘC).** Thư viện này đang được 6 plugin tham chiếu (`AssignPrescription` × 4, `ExamServiceReqExecute`, `PatientUpdate`). Mọi method hiện có **giữ nguyên chữ ký**, thêm hành vi bằng **overload mới**, đúng như tiền lệ đã ghi trong `DrugHealthService.cs` (`#region Overload tương thích ngược`). Không được thêm tham số optional vào method cũ.

## 5.1 `Core/MimsClient.cs` — hỗ trợ tham số form phụ

```csharp
/// <summary>
/// Gửi request tới MIMS kèm các tham số form tuỳ chọn của FT Web Service
/// (alertfilterbydrug / alertfilterbyseverity). extraFormParams = null → post y như cũ.
/// </summary>
public static string PostXml(string url, string xml,
    Dictionary<string, string> extraFormParams, out bool isTimeoutOrConnectionError)
{
    // ... giữ nguyên toàn bộ phần dựng request hiện tại ...
    var postData = new StringBuilder();
    postData.Append("prescriptionquery=").Append(HttpUtility.UrlEncode(xml));
    postData.Append("&responsetype=xml");

    if (extraFormParams != null)
    {
        foreach (var kv in extraFormParams)
        {
            if (string.IsNullOrEmpty(kv.Key) || string.IsNullOrEmpty(kv.Value)) continue;
            postData.Append("&").Append(kv.Key).Append("=").Append(HttpUtility.UrlEncode(kv.Value));
        }
    }
    // ... phần còn lại giữ nguyên ...
}
```

Hai overload cũ `PostXml(url, xml)` và `PostXml(url, xml, out bool)` giữ nguyên, chuyển tiếp với `extraFormParams = null`.

## 5.2 `Core/MimsRequestBuilder.cs`

```csharp
/// <summary>
/// Sinh &lt;GUIDS&gt; cho tham số form "alertfilterbydrug" — chỉ hiện alert liên quan
/// các GUID truyền vào (thuốc đơn đang kê). Trả về null khi không có GUID.
/// </summary>
public static string BuildAlertFilterByDrug(List<DrugItem> drugs)

/// <summary>
/// Overload MỚI: previousDrugs = thuốc các đơn khác còn hiệu lực.
/// requestMode = 1 → gộp previousDrugs vào &lt;Prescribing&gt; (mặc định, theo API Guide).
/// requestMode = 2 → sinh khối &lt;Prescribed&gt; riêng (dự phòng, theo Request55.xsd).
/// previousDrugs null/rỗng → XML sinh ra GIỐNG HOÀN TOÀN bản hiện tại.
/// </summary>
public static string BuildDrugHealthAlertRequest(
    List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes,
    bool checkDuplicateDrug, bool checkAllergy, MimsPatientProfile patientProfile,
    List<DrugItem> previousDrugs, int requestMode)
```

Chi tiết sinh XML khi `requestMode = 2` — chèn ngay sau `</Prescribing>` (thứ tự tự do vì `<Interaction>` là `xsd:all`):

```xml
<Prescribed>
  <GGPI reference="{...}" />
  <Product reference="{...}" />
</Prescribed>
```

Cả 2 overload cũ (5 và 6 tham số) giữ nguyên, chuyển tiếp `previousDrugs = null, requestMode = 1`.

## 5.3 `Models/MimsPreviousDrugInfo.cs` (MỚI)

Chỉ phục vụ dựng khối HTML `QT-11`; **không** ảnh hưởng XML request.

```csharp
/// <summary>
/// Thông tin nguồn của 1 thuốc thuộc đơn khác trong hồ sơ — dùng để hiển thị
/// khối "Thuốc đang dùng từ đơn khác" đầu popup cảnh báo.
/// </summary>
public class MimsPreviousDrugInfo
{
    public string HisDrugCode { get; set; }      // MEDICINE_TYPE_CODE
    public string DrugName { get; set; }         // MEDICINE_TYPE_NAME
    public string ServiceReqCode { get; set; }   // mã đơn (nếu lấy được)
    public long? IntructionTime { get; set; }    // TDL_INTRUCTION_TIME
    public long? UseTimeTo { get; set; }         // USE_TIME_TO
}
```

## 5.4 `Modules/DrugHealthService.cs`

```csharp
/// <summary>
/// Overload MỚI — kiểm tra tương tác có tính cả thuốc các đơn khác trong hồ sơ (việc 52540).
/// previousDrugs null/rỗng → hành vi GIỐNG overload hiện tại.
/// </summary>
public MimsResult Check(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes,
    MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
    List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)

public bool CheckAndAlert(List<DrugItem> drugs, List<AllergyItem> allergies, List<string> icd10Codes,
    HIS_MIMS_INTERACTION_LOG interactionLog, long? treatmentId, long? serviceReqId, long? patientId,
    MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
    List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)

public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes,
    MimsPatientProfile patientProfile, List<DrugItem> previousDrugs,
    List<MimsPreviousDrugInfo> previousDrugInfos, MimsCrossPrescriptionOption crossOption)
```

`MimsCrossPrescriptionOption` (class truyền cấu hình từ plugin xuống thư viện — thư viện **không** đọc `HisConfigs` trực tiếp):

```csharp
public class MimsCrossPrescriptionOption
{
    /// <summary>1 = gộp vào Prescribing (mặc định); 2 = dùng khối Prescribed.</summary>
    public int RequestMode { get; set; }
    /// <summary>true = gửi tham số form alertfilterbydrug (lọc phía MIMS).</summary>
    public bool UseAlertFilterByDrug { get; set; }
    /// <summary>Giá trị alertfilterbyseverity; null = không gửi.</summary>
    public string AlertFilterBySeverity { get; set; }
}
```

Các bước bổ sung bên trong `Check`:

1. `currentMapped = MappingMIMS(drugs)`; `previousMapped = MappingMIMS(previousDrugs)`.
2. `previousMapped` loại các `MimsGuid` đã có trong `currentMapped` (tránh trùng thẻ trong `<Prescribing>`).
3. `xmlRequest = BuildDrugHealthAlertRequest(..., previousMapped, crossOption.RequestMode)`.
4. `extraFormParams`: `alertfilterbydrug = BuildAlertFilterByDrug(currentMapped)` khi `UseAlertFilterByDrug && previousMapped.Count > 0`; `alertfilterbyseverity` khi có cấu hình.
5. Sau parse: nếu `previousMapped.Count > 0 && !UseAlertFilterByDrug` → gọi `FilterAlertsByCurrentDrugs(result, currentMapped)` (QT-10).
6. `result.Html = BuildPreviousDrugBanner(previousDrugInfos) + result.Html` khi `previousDrugInfos` có phần tử (QT-11).
7. Gán thêm cho log: `result.PreviousDrugCount`, `result.PreviousDrugGuids` (2 property mới trong `MimsResult`).

### 5.4.1 `FilterAlertsByCurrentDrugs` — lọc client (QT-10)

Chỉ lọc trên **danh sách detail đã parse** (dùng cho `hasCdsAlert` và ghi log). HTML khi ở chế độ này **không** lọc được (do MIMS đã transform) → khối banner phải nêu rõ điều đó. Đây là lý do `UseAlertFilterByDrug = true` là mặc định.

```csharp
// Giữ alert nếu PrimaryDrugReference hoặc InteractingDrugReference thuộc GUID đơn hiện tại
private void FilterAlertsByCurrentDrugs(MimsResult result, List<DrugItem> currentMapped)
```

So sánh GUID phải chuẩn hoá: MIMS trả `reference="{GUID}"` có ngoặc nhọn, `DrugItem.MimsGuid` không có → dùng `Trim('{','}')` + `StringComparer.OrdinalIgnoreCase`.

### 5.4.2 `BuildPreviousDrugBanner` — khối HTML (QT-11)

```html
<div style="font-family:Segoe UI;font-size:13px;border:1px solid #d9822b;
            background:#fff7e6;padding:8px;margin-bottom:8px;">
  <b>Thuốc đang dùng từ đơn khác trong hồ sơ (đưa vào kiểm tra tương tác):</b>
  <ul>
    <li>Paracetamol 500mg — đơn ngày 05/09/2026, dùng đến 12/09/2026</li>
  </ul>
</div>
```

Bắt buộc `SecurityElement.Escape` cho mọi giá trị lấy từ dữ liệu (tên thuốc có thể chứa `&`, `<`).

## 5.5 Ví dụ XML request sau thay đổi (PA-A, `requestMode = 1`)

```xml
<Request>
<Interaction>
<Prescribing>
<GGPI reference="{B3E6B75E-9519-6AE7-E034-080020E1DD8C}" />   <!-- đơn hiện tại -->
<GGPI reference="{BF33752F-6062-0589-E034-0003BA299378}" />   <!-- đơn khác trong hồ sơ -->
</Prescribing>
<HealthIssueCodes>
<HealthIssueCode code="J45" codeType="ICD10" />
</HealthIssueCodes>
<Allergies/>
<References/>
<DuplicateTherapy checkSameDrug="true"/>
<DuplicateIngredient checkSameDrug="true"/>
</Interaction>
<PatientProfile>
<Gender>F</Gender>
<Age><Year>34</Year></Age>
</PatientProfile>
</Request>
```

Form data POST kèm:

```
prescriptionquery=<url-encoded XML>
responsetype=xml
alertfilterbydrug=<GUIDS><GUID>{B3E6B75E-9519-6AE7-E034-080020E1DD8C}</GUID></GUIDS>
```

---

# PHẦN 6. THAY ĐỔI PLUGIN (× 4)

## 6.1 Config mới — `Config/HisConfigCFG.cs`

```csharp
private const string CONFIG_KEY__MIMS_INTERACTION_SCOPE_OPTION       = "HIS.Desktop.Mims.InteractionScopeOption";
private const string CONFIG_KEY__MIMS_PREVIOUS_PRESCRIPTION_DAY_RANGE = "HIS.Desktop.Mims.PreviousPrescriptionDayRange";
private const string CONFIG_KEY__MIMS_CROSS_PRESCRIPTION_REQUEST_MODE = "HIS.Desktop.Mims.CrossPrescriptionRequestMode";

internal const int MIMS_PREVIOUS_PRESCRIPTION_DAY_RANGE_DEFAULT = 30;
internal const int MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG          = 30;

/// <summary>
/// Phạm vi kiểm tra tương tác MIMS (việc 52540).
/// rỗng/"1" = chỉ đơn đang kê (mặc định — hành vi hiện tại)
/// "2"      = + thuốc còn hiệu lực của các đơn khác trong CÙNG hồ sơ điều trị
/// "3"      = + thuốc còn hiệu lực toàn lịch sử bệnh nhân
/// </summary>
internal static string MimsInteractionScopeOption;

/// <summary>Số ngày lùi khi thuốc đơn khác không có USE_TIME_TO. Mặc định 30.</summary>
internal static int MimsPreviousPrescriptionDayRange = MIMS_PREVIOUS_PRESCRIPTION_DAY_RANGE_DEFAULT;

/// <summary>1 = gộp Prescribing (mặc định); 2 = khối Prescribed (dự phòng).</summary>
internal static int MimsCrossPrescriptionRequestMode = 1;
```

Nạp trong `LoadConfig()`, đặt ngay dưới dòng `IsCheckMimsPregnancyLactation`:

```csharp
MimsInteractionScopeOption = GetValue(CONFIG_KEY__MIMS_INTERACTION_SCOPE_OPTION);
int dayRange = Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__MIMS_PREVIOUS_PRESCRIPTION_DAY_RANGE));
MimsPreviousPrescriptionDayRange = dayRange > 0 ? dayRange : MIMS_PREVIOUS_PRESCRIPTION_DAY_RANGE_DEFAULT;
int reqMode = Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__MIMS_CROSS_PRESCRIPTION_REQUEST_MODE));
MimsCrossPrescriptionRequestMode = reqMode == 2 ? 2 : 1;
```

## 6.2 File mới `AssignPrescription/frmAssignPrescription__MimsCrossPrescription.cs`

Partial class riêng (theo `folder_structure.md`: Form dùng `__` double underscore), cùng mẫu với `frmAssignPrescription__MimsPatientProfile.cs` đã có.

| Thành phần | Mô tả |
|---|---|
| `List<V_HIS_EXP_MEST_MEDICINE> crossPrescriptionMedicines` | Dữ liệu thô đơn khác (prefetch) |
| `bool isCrossPrescriptionLoaded` | Đã nạp xong (kể cả rỗng) |
| `long crossPrescriptionLoadedKey` | Khoá cache = giờ chỉ định đã nạp — đổi giờ mới nạp lại |
| `PrefetchMimsCrossPrescription()` | Guard config + `Task.Run` gọi API, không chặn UI |
| `GetCrossPrescriptionMedicines()` | Gọi API đồng bộ (dùng khi prefetch chưa xong) |
| `BuildCrossPrescriptionDrugItems(out List<MimsPreviousDrugInfo>)` | Lọc QT-04..QT-07 → `List<DrugItem>` + info hiển thị |
| `BuildMimsCrossPrescriptionOption()` | Dựng `MimsCrossPrescriptionOption` từ config |

Khung `PrefetchMimsCrossPrescription`:

```csharp
private void PrefetchMimsCrossPrescription()
{
    try
    {
        if (HisConfigCFG.ConnectDrugInterventionInfo != "2"
            || (HisConfigCFG.MimsInteractionScopeOption != "2" && HisConfigCFG.MimsInteractionScopeOption != "3"))
            return;
        if (this.VHistreatment == null) return;

        long instructionTime = this.InstructionTime;
        if (this.isCrossPrescriptionLoaded && this.crossPrescriptionLoadedKey == instructionTime)
            return;

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var data = GetCrossPrescriptionMedicines(instructionTime);
                this.crossPrescriptionMedicines = data;
                this.crossPrescriptionLoadedKey = instructionTime;
                this.isCrossPrescriptionLoaded = true;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        });
    }
    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
}
```

Khung `GetCrossPrescriptionMedicines`:

```csharp
private List<V_HIS_EXP_MEST_MEDICINE> GetCrossPrescriptionMedicines(long instructionTime)
{
    CommonParam param = new CommonParam();
    HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();

    if (HisConfigCFG.MimsInteractionScopeOption == "3")
        filter.TDL_PATIENT_ID = this.VHistreatment.PATIENT_ID;          // QT-03 option 3
    else
        filter.TDL_TREATMENT_ID = this.VHistreatment.ID;                // QT-03 option 2

    filter.IS_INCLUDE_DELETED = false;
    filter.DATA_DOMAIN_FILTER = false;
    // Giới hạn dữ liệu ngay tại API — giảm payload
    filter.TDL_INTRUCTION_TIME_FROM = <instructionTime lùi MimsPreviousPrescriptionDayRange ngày, 000000>;

    Inventec.Common.Logging.LogSystem.Debug(
        Inventec.Common.Logging.LogUtil.TraceData(
            Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));

    return new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(
        HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer,
        filter, ProcessLostToken, param) ?? new List<V_HIS_EXP_MEST_MEDICINE>();
}
```

Lọc trong `BuildCrossPrescriptionDrugItems` (đúng thứ tự QT-04 → QT-07):

```csharp
var currentCodes = new HashSet<string>(
    (this.mediMatyTypeADOs ?? new List<MediMatyTypeADO>())
        .Where(o => !string.IsNullOrWhiteSpace(o.MEDICINE_TYPE_CODE))
        .Select(o => o.MEDICINE_TYPE_CODE), StringComparer.OrdinalIgnoreCase);

long oldServiceReqId = this.oldServiceReq != null ? this.oldServiceReq.ID : 0;
long minIntructionTime = <instructionTime lùi MimsPreviousPrescriptionDayRange ngày>;

var valid = source.Where(o =>
        o.IS_DELETE != 1
        && o.TDL_SERVICE_REQ_ID != oldServiceReqId
        && !string.IsNullOrWhiteSpace(o.MEDICINE_TYPE_CODE)
        && !currentCodes.Contains(o.MEDICINE_TYPE_CODE)
        && (o.USE_TIME_TO.HasValue
                ? o.USE_TIME_TO.Value >= instructionTime                      // QT-05
                : (o.TDL_INTRUCTION_TIME ?? 0) >= minIntructionTime))
    .GroupBy(o => o.MEDICINE_TYPE_CODE, StringComparer.OrdinalIgnoreCase)      // QT-06
    .Select(g => g.OrderByDescending(o => o.USE_TIME_TO ?? 0).First())
    .OrderByDescending(o => o.USE_TIME_TO ?? 0)
    .ToList();

int total = valid.Count;
if (total > HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG)                  // QT-07
{
    Inventec.Common.Logging.LogSystem.Warn(string.Format(
        "MIMS cross-prescription: cắt {0}/{1} thuốc do vượt giới hạn",
        total - HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG, total));
    valid = valid.Take(HisConfigCFG.MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG).ToList();
}
```

> **Hiệu năng.** `HashSet` + `GroupBy` → O(n). Tuyệt đối không dùng `list.Where(...).Count() > 0`, không `FirstOrDefault` trong vòng lặp, không gọi `BackendDataWorker` cho từng dòng (`performance.md`). `MappingMIMS` bên thư viện vẫn `FirstOrDefault` trên cache theo từng thuốc — với giới hạn 30 thuốc là chấp nhận được; nếu sau này nâng giới hạn thì phải tối ưu `MappingMIMS` bằng `Dictionary` (xem `RR-05`).

## 6.3 Sửa `CheckMIMS()`

Chèn ngay trước lời gọi `CheckAndAlert`:

```csharp
List<MimsPreviousDrugInfo> previousDrugInfos = null;
List<HIS.Desktop.MIMS.Integration.Models.DrugItem> previousDrugItems =
    BuildCrossPrescriptionDrugItems(out previousDrugInfos);

check = service.CheckAndAlert(
    lstDrugItem, null, lstICD, mimsInteractionLog,
    this.VHistreatment != null ? (long?)this.VHistreatment.ID : null,
    null,
    this.VHistreatment != null ? (long?)this.VHistreatment.PATIENT_ID : null,
    mimsProfile,
    previousDrugItems,
    previousDrugInfos,
    BuildMimsCrossPrescriptionOption());
```

`previousDrugItems` rỗng → thư viện chạy đúng nhánh hiện tại (QT-12).

## 6.4 Sửa menu chuột phải (YC-06)

Trong `case MOUSE_RIGHT_TYPE.INFORMATION_EVALUATION`, đổi:

```csharp
service.ShowResultAsync(lstDrugItem, lstICD, BuildMimsPatientProfile());
```

thành overload mới có `previousDrugItems` / `previousDrugInfos` / `BuildMimsCrossPrescriptionOption()`.

> Lưu ý: nhánh này lấy thuốc **đang chọn trong grid**, không phải toàn grid → `currentCodes` trong `BuildCrossPrescriptionDrugItems` phải nhận tham số danh sách thuốc hiện tại thay vì đọc cứng `this.mediMatyTypeADOs`. Thiết kế hàm: `BuildCrossPrescriptionDrugItems(List<MediMatyTypeADO> currentDrugs, out List<MimsPreviousDrugInfo> infos)`.

## 6.5 Gọi prefetch

Thêm `PrefetchMimsCrossPrescription()` tại đúng 3 vị trí đang gọi `GetHisExpMestMedicine()` để tái dùng thời điểm nghiệp vụ đã được kiểm chứng:

| Vị trí (bản PK) | Ngữ cảnh |
|---|---|
| `frmAssignPrescription.cs:1236` | Load form |
| `frmAssignPrescription.cs:10969` | Đổi ngày chỉ định |
| `frmAssignPrescription.cs:13408` | Đổi giờ chỉ định |

Không đặt trong `timerInitForm_Tick` để tránh phụ thuộc thứ tự với prefetch PatientProfile.

## 6.6 Bảng vị trí sửa theo từng plugin

| Plugin | `CheckMIMS()` | Chốt gọi khi lưu | Menu chuột phải |
|---|---|---|---|
| `...AssignPrescriptionPK` | `AssignPrescription/frmAssignPrescription.cs:14148` | `frmAssignPrescription.cs:2591` | `__InitMenuMouseRight.cs:195` |
| `...AssignPrescriptionCLS` | `AssignPrescription/frmAssignPrescription__Save.cs:704` | `__Save.cs:301` | `__InitMenuMouseRight.cs:150` |
| `...AssignPrescriptionKidney` | `AssignPrescription/frmAssignPrescription__Save.cs:477` | `__Save.cs:224` | `__InitMenuMouseRight.cs:146` |
| `...AssignPrescriptionYHCT` | `AssignPrescription/frmAssignPrescription__Save.cs:1464` | `__Save.cs:379` | `__InitMenuMouseRight.cs:104` |

Ba plugin CLS / Kidney / YHCT **không có** `GetHisExpMestMedicine()` → cần kiểm tra chúng có `oldServiceReq`, `InstructionTime`, `VHistreatment`, `mediMatyTypeADOs` tương đương trước khi bê nguyên partial class; nếu tên biến khác thì đổi theo từng plugin (đây là điểm chặn `RR-06`).

---

# PHẦN 7. API & DỮ LIỆU

## 7.1 API sử dụng — không cần backend mới

| Thành phần | Giá trị |
|---|---|
| URI | `api/HisExpMestMedicine/GetView` (`HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW`) |
| Consumer | `ApiConsumers.MosConsumer` |
| Filter | `MOS.Filter.HisExpMestMedicineViewFilter` |
| Kết quả | `List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>` |

Trường filter dùng (đã kiểm chứng bằng reflection trên `lib/MOS/MOS.Filter.dll`):

| Trường | Dùng cho |
|---|---|
| `TDL_TREATMENT_ID` | Phạm vi option `2` |
| `TDL_PATIENT_ID` | Phạm vi option `3` |
| `TDL_INTRUCTION_TIME_FROM` | Giới hạn khoảng ngày, giảm payload |
| `IS_INCLUDE_DELETED = false` | Loại bản ghi đã xoá |
| `DATA_DOMAIN_FILTER = false` | Không giới hạn theo miền dữ liệu |

Trường của `V_HIS_EXP_MEST_MEDICINE` dùng: `MEDICINE_TYPE_CODE`, `MEDICINE_TYPE_NAME`, `USE_TIME_TO`, `TDL_INTRUCTION_TIME`, `TDL_SERVICE_REQ_ID`, `TDL_TREATMENT_ID`, `IS_DELETE`.

## 7.2 Bảng cấu hình cần thêm (`HIS_CONFIG`)

| `CONFIG_CODE` | `CONFIG_NAME` | Giá trị | Mặc định |
|---|---|---|---|
| `HIS.Desktop.Mims.InteractionScopeOption` | Phạm vi kiểm tra tương tác thuốc bằng MIMS | `1` = đơn hiện tại; `2` = cùng hồ sơ điều trị; `3` = toàn lịch sử bệnh nhân | rỗng (= `1`) |
| `HIS.Desktop.Mims.PreviousPrescriptionDayRange` | Số ngày lùi lấy thuốc đơn khác khi thuốc không có ngày dùng đến | số nguyên | `30` |
| `HIS.Desktop.Mims.CrossPrescriptionRequestMode` | Cách gửi thuốc đơn khác tới MIMS | `1` = gộp Prescribing; `2` = khối Prescribed | `1` |

## 7.3 Audit `HIS_MIMS_INTERACTION_LOG` (YC-07)

Bảng đã có đủ cột, **không cần thay đổi CSDL**. Gán bổ sung trong `SaveDataInteractionLog`:

| Cột | Giá trị |
|---|---|
| `DRUG_COUNT` | Số GUID **đơn hiện tại** (giữ nguyên nghĩa hiện tại) |
| `CHECKED_GUIDS` | GUID đơn hiện tại `;` + GUID đơn khác (giữ format `;`) |
| `REQUEST_PARAMS` | JSON: `{"scopeOption":2,"requestMode":1,"previousDrugCount":5,"truncated":0,"useAlertFilterByDrug":true}` |
| `NOTE` | `"52540: kiểm tra tương tác chéo đơn — <n> thuốc từ đơn khác"` |
| `REQUEST_XML` | Đã chứa toàn bộ thẻ thuốc → truy vết được đầy đủ |

---

# PHẦN 8. LOGGING

Theo `logging_guidelines.md`:

| Điểm | Level | Nội dung |
|---|---|---|
| `GetCrossPrescriptionMedicines` — trước gọi API | Debug | `LogUtil.TraceData(... => filter, filter)` |
| `GetCrossPrescriptionMedicines` — API lỗi | Error | `LogSystem.Error(ex)` — **không** chặn lưu (QT-13) |
| `PrefetchMimsCrossPrescription` | Warn | Lỗi prefetch, form vẫn dùng được |
| `BuildCrossPrescriptionDrugItems` — sau khi lọc | Debug | Số thuốc trước/sau lọc, số bị cắt (1 dòng, **ngoài** vòng lặp) |
| `BuildCrossPrescriptionDrugItems` — vượt giới hạn | Warn | QT-07 |
| `DrugHealthService.Check` | Debug | `previousDrugs.Count`, `previousMapped.Count`, `requestLength`, `useAlertFilterByDrug` |
| Menu chuột phải / event handler | Warn | Toàn bộ `catch` trong event handler |

Tuyệt đối không log trong vòng lặp thuốc; không log dữ liệu định danh bệnh nhân.

---

# PHẦN 9. ĐA NGÔN NGỮ

Chuỗi mới hiển thị cho người dùng nằm trong khối HTML do thư viện dựng. Thư viện `HIS.Desktop.MIMS.Integration` **không có** `Resources/` đa ngôn ngữ; tiêu đề popup hiện đang hardcode (`NameText = "Kiểm tra tương tác thuốc, bệnh liên quan"`).

Xử lý: chuỗi tiêu đề khối HTML được **truyền từ plugin xuống** qua `MimsCrossPrescriptionOption.BannerTitle`, plugin lấy từ `Message.Lang.resx` của chính nó.

| File | Key mới | vi | en |
|---|---|---|---|
| `Resources/Message.Lang.vi/en.resx` | `ThuocDangDungTuDonKhacTrongHoSo` | `Thuốc đang dùng từ đơn khác trong hồ sơ (đưa vào kiểm tra tương tác):` | `Medications from other prescriptions in this record (included in the interaction check):` |
| `Resources/Message.Lang.vi/en.resx` | `DonNgayDungDenNgay` | `đơn ngày {0}, dùng đến {1}` | `prescribed on {0}, until {1}` |

Thêm property tương ứng trong `Resources/ResourceMessage.cs` của **cả 4 plugin** theo đúng mẫu hiện có (`Inventec.Common.Resource.Get.Value(key, languageMessage, LanguageManager.GetCulture())`, `catch → LogSystem.Warn`, trả `""`).

---

# PHẦN 10. KIỂM THỬ

## 10.1 Test case chức năng

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-01 | `ConnectDrugInterventionInfo` rỗng | Không gọi MIMS (như hiện tại) |
| TC-02 | `= "2"`, `InteractionScopeOption` rỗng | Request MIMS **giống byte-for-byte** bản hiện tại; không có `alertfilterbydrug` |
| TC-03 | `= "2"`, scope `= "2"`, hồ sơ không có đơn khác | Như TC-02, không có khối banner |
| TC-04 | scope `= "2"`, hồ sơ có 1 đơn khác còn hiệu lực, thuốc **không** tương tác | Không popup; lưu thành công |
| TC-05 | scope `= "2"`, thuốc đơn hiện tại tương tác Severe với thuốc đơn khác | Popup có alert + khối banner nêu đúng tên thuốc/ngày; "Xác nhận" → lưu được; "Bỏ qua" → huỷ lưu |
| TC-06 | 2 thuốc **đều thuộc đơn khác** tương tác với nhau, đơn hiện tại không liên quan | **Không** popup (YC-02) |
| TC-07 | Thuốc đơn khác đã `USE_TIME_TO` < giờ chỉ định | Không đưa vào request |
| TC-08 | Thuốc đơn khác trùng `MEDICINE_TYPE_CODE` với đơn hiện tại | Không đưa vào `<Prescribing>` lần 2 |
| TC-09 | Sửa đơn cũ (`oldServiceReq.ID > 0`) | Thuốc của chính đơn đang sửa **không** bị coi là đơn khác |
| TC-10 | Hồ sơ nội trú dài, > 30 thuốc đơn khác | Chỉ gửi 30 thuốc `USE_TIME_TO` xa nhất; có log Warn |
| TC-11 | API `HisExpMestMedicine/GetView` timeout | Vẫn kiểm tra MIMS phạm vi đơn hiện tại; không chặn lưu; có log Error |
| TC-12 | MIMS timeout | Popup "Kiểm tra kết nối MIMS" như hiện tại |
| TC-13 | scope `= "3"` | Lấy thêm thuốc hồ sơ khác của cùng bệnh nhân trong `PreviousPrescriptionDayRange` ngày |
| TC-14 | `CrossPrescriptionRequestMode = 2` | XML có khối `<Prescribed>`; so sánh kết quả alert với mode `1` |
| TC-15 | Menu chuột phải, chọn 2 thuốc | Kết quả có tính thuốc đơn khác + khối banner |
| TC-16 | BN nữ có tick mang thai + scope `= "2"` | Cả `<PatientProfile>` và thuốc đơn khác cùng có trong 1 request |
| TC-17 | Kê nhiều bệnh nhân | Theo GD-03: chỉ kiểm tra hồ sơ đang mở — xác nhận không phát sinh lỗi/treo |
| TC-18 | Bản ghi log | `HIS_MIMS_INTERACTION_LOG.REQUEST_PARAMS` / `NOTE` / `CHECKED_GUIDS` đúng như 7.3 |

## 10.2 Test case tương thích DLL

| Mã | Điều kiện | Kỳ vọng |
|---|---|---|
| TC-19 | Deploy `HIS.Desktop.MIMS.Integration.dll` mới + plugin `PatientUpdate` / `ExamServiceReqExecute` **bản cũ** | Không `MissingMethodException`; luồng cũ chạy bình thường |
| TC-20 | Deploy plugin AssignPrescription mới + thư viện MIMS **bản cũ** | Phải báo lỗi rõ ràng khi mở, không làm treo form kê đơn (kịch bản dán lệch DLL) |

## 10.3 Test hiệu năng

| Mã | Đo | Ngưỡng |
|---|---|---|
| TC-21 | Thời gian mở form kê đơn (scope `= "2"`, hồ sơ 200 dòng thuốc) | Không tăng so với hiện tại (prefetch async) |
| TC-22 | Thời gian từ bấm Lưu → popup MIMS | ≤ hiện tại + 300 ms (khi prefetch đã xong) |
| TC-23 | `requestLength` gửi MIMS | Ghi lại số thực tế; nếu > 60 KB phải hạ `MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG` |

---

# PHẦN 11. TRIỂN KHAI

## 11.1 Thứ tự deploy

1. `HIS_CONFIG`: thêm 3 config ở mục 7.2, để **giá trị mặc định** (scope rỗng) → hệ thống chạy y như trước.
2. Deploy `HIS.Desktop.MIMS.Integration.dll` (tương thích ngược với plugin cũ — TC-19).
3. Deploy 4 DLL plugin `HIS.Desktop.Plugins.AssignPrescription*.dll` **kèm satellite resource** `vi/`, `en/` (nếu thêm key `Message.Lang.resx` mà thiếu satellite thì `ResourceMessage` trả chuỗi rỗng).
4. Bật `HIS.Desktop.Mims.InteractionScopeOption = 2` cho **1 khoa thí điểm**, theo dõi `LogSystem` + bảng `HIS_MIMS_INTERACTION_LOG` trước khi bật toàn viện.

## 11.2 Kịch bản rollback

Đặt `HIS.Desktop.Mims.InteractionScopeOption` về rỗng → toàn bộ tính năng tắt, không cần rollback DLL.

---

# PHẦN 12. RỦI RO & ĐIỂM CHẶN

| Mã | Rủi ro | Mức | Xử lý |
|---|---|---|---|
| RR-01 | Tăng mạnh tần suất popup, nhất là nội trú → bác sĩ bỏ qua theo phản xạ (alert fatigue) | HIGH | Mặc định TẮT; bật thí điểm 1 khoa; cân nhắc gửi `alertfilterbyseverity` chỉ giữ `D2D:Severe`, `D2H:Contraindicated`, `DI:1`, `DT:1` |
| RR-02 | `<Prescribed>` chưa được API Guide 7Modules tài liệu hoá cho Drug-Health / Duplicate → có thể không sinh alert | HIGH | Mặc định dùng PA-A (`requestMode = 1`); PA-B chỉ để đối chứng; **phải test trên MIMS trial trước khi chốt** |
| RR-03 | `alertfilterbydrug` **không dùng chung** với `alertfilterbytemplate` (mặc định "Yes") → có thể xuất hiện alert mà template bệnh viện đang chặn | HIGH | Đo đối chứng số alert trước/sau; nếu lệch, chuyển sang lọc client (QT-10) bằng config `UseAlertFilterByDrug = false` |
| RR-04 | Lọc client (QT-10) **không** lọc được HTML đã transform → popup vẫn hiện cặp "cũ–cũ" | MEDIUM | Ưu tiên `alertfilterbydrug`; nếu buộc lọc client thì banner phải ghi rõ "cảnh báo giữa các thuốc đơn cũ chỉ mang tính tham khảo" |
| RR-05 | `MappingMIMS` dùng `FirstOrDefault` trên cache cho **từng** thuốc → O(n·m) khi số thuốc tăng | MEDIUM | Giữ giới hạn 30 thuốc; nếu nâng thì tối ưu `MappingMIMS` bằng `Dictionary<string, V_HIS_MEDICINE_TYPE>` |
| RR-06 | 3 plugin CLS/Kidney/YHCT có thể khác tên biến `oldServiceReq` / `InstructionTime` / `VHistreatment` | MEDIUM | Kiểm tra từng plugin trước khi copy partial class; không copy mù |
| RR-07 | Thư viện MIMS dùng chung 6 plugin — sửa sai chữ ký gây `MissingMethodException` khi dán lệch DLL | HIGH | Chỉ thêm overload, giữ nguyên method cũ (Phần 5 mở đầu) |
| RR-08 | `alertfilterbydrug` phải nhận GUID **sau** `MappingMIMS` (1 thuốc HIS → nhiều GUID); truyền thiếu sẽ lọc mất alert đúng | MEDIUM | Lấy GUID từ `currentMapped`, không lấy từ `drugs` gốc |

## Điểm chặn cần chốt trước khi code

| # | Nội dung | Người chốt |
|---|---|---|
| 1 | GD-01 — "hồ sơ" = lần điều trị hay toàn lịch sử bệnh nhân | Nghiệp vụ |
| 2 | GD-02 — tương tác chéo đơn có được chặn lưu hay chỉ cảnh báo | Nghiệp vụ |
| 3 | GD-03 — hành vi khi kê nhiều bệnh nhân | Nghiệp vụ |
| 4 | GD-05 — làm 4 plugin hay chỉ PK | Quản lý dự án |
| 5 | RR-02 / RR-03 — test `<Prescribed>` và `alertfilterbydrug` trên MIMS trial | Kỹ thuật (làm trước khi code phần 5) |
| 6 | RR-01 — bộ mức nghiêm trọng muốn hiển thị (`alertfilterbyseverity`) | Nghiệp vụ / Dược |

---

# PHẦN 13. CHECKLIST CODE

- [ ] Thư viện: chỉ **thêm overload**, không đổi chữ ký method cũ (`MimsClient`, `MimsRequestBuilder`, `DrugHealthService`)
- [ ] `previousDrugs` null/rỗng → XML sinh ra **giống hoàn toàn** bản hiện tại (verify bằng so sánh chuỗi trong unit test tay)
- [ ] Mọi method có `try-catch`; API fail → `LogSystem.Error`; event handler → `LogSystem.Warn`
- [ ] Không `catch` rỗng, không `Console.Write`, không log trong vòng lặp thuốc
- [ ] Không hardcode số — dùng const/config (`MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG`, `..._DAY_RANGE_DEFAULT`)
- [ ] Không hardcode tiếng Việt trong code — chuỗi banner lấy từ `Message.Lang.resx`
- [ ] `Lang.en.resx` / `Message.Lang.en.resx` có **đủ** số entry bằng bản `vi`
- [ ] Prefetch chạy `Task.Run`, **không** thao tác control trong thread (`ui_rules.md` mục 1)
- [ ] Lọc dữ liệu O(n): `HashSet` / `GroupBy` / `Any()`, không `Count() > 0`, không `Get` trong loop
- [ ] `SecurityElement.Escape` mọi giá trị đưa vào HTML banner
- [ ] So sánh GUID: `Trim('{','}')` + `StringComparer.OrdinalIgnoreCase`
- [ ] `partial class` mới đặt tên `frmAssignPrescription__MimsCrossPrescription.cs` (Form → `__`)
- [ ] Thêm file mới vào `.csproj` của cả 4 plugin
- [ ] `ProcessDisposeModuleDataAfterClose()` clear `crossPrescriptionMedicines = null`
- [ ] Cập nhật `docs/HIS.Desktop.Plugins.AssignPrescriptionPK.md` và 3 file docs còn lại (Changelog + mục MIMS)

---

# PHẦN 14. GHI NHẬN TRIỂN KHAI (AS-BUILT) — 09/09/2026

## 14.1 Các điểm đã chốt

| Mã | Chốt |
|---|---|
| GD-01 | Làm cả 3 mức phạm vi bằng config: `1` = đơn hiện tại (mặc định), `2` = cùng hồ sơ điều trị, `3` = toàn lịch sử bệnh nhân |
| GD-02 | Chỉ cảnh báo — giữ nguyên cơ chế `WebViewHelper.ShowDialog` (Xác nhận = lưu, Bỏ qua = không lưu). KHÔNG chặn cứng, KHÔNG bắt nhập lý do ở giai đoạn này |
| GD-03 | Kiểm tra theo hồ sơ đang mở, giữ nguyên hành vi `CheckMIMS` hiện tại |
| GD-05 | Làm cả 4 plugin |
| Phần 12 điểm 5 | Khối HTML hiển thị thêm **mã đơn / khoa / bác sĩ kê** (`EXP_MEST_CODE`, `HIS_DEPARTMENT.DEPARTMENT_NAME` theo `REQ_DEPARTMENT_ID`, `REQ_USERNAME`) |
| Phần 12 điểm 6 | Chưa gửi `alertfilterbyseverity` — `MimsCrossPrescriptionOption.AlertFilterBySeverity` để sẵn, bật khi khách hàng chốt bộ mức |
| Phần 12 điểm 7, 8 | Không làm ở giai đoạn này (đúng khuyến nghị) |

## 14.2 Sai khác so với thiết kế ban đầu — có chủ ý

| # | Nội dung | Lý do |
|---|---|---|
| 1 | **Chống chỉ định VN dùng tập thuốc GỘP** (đơn hiện tại + đơn khác) và **không lọc** cặp "cũ–cũ" | Endpoint VN không có tham số `alertfilterbydrug`, kết quả trả về chỉ có tên hoạt chất (`Drug1`/`Drug2`) nên không đối chiếu được theo GUID. Danh mục VN là tập cặp **chống chỉ định** được kiểm soát, số lượng nhỏ → giữ đủ cảnh báo an toàn hơn là lọc sai. Hàm `MergeDrugsForVnCheck` khử trùng theo `HisDrugCode` |
| 2 | `alertfilterbydrug` **chỉ gửi khi thực sự có thuốc đơn khác** trong request | Khi không có thuốc đơn khác, request phải giống hoàn toàn bản hiện tại (QT-12, TC-02) — tránh xung đột `alertfilterbytemplate` (RR-03) ngoài ý muốn |
| 3 | Lọc phía client (QT-10) chỉ áp lên `DrugDrugAlertDetails` | Chỉ nhóm này có `PrimaryDrugReference` / `InteractingDrugReference` để đối chiếu GUID. Nhánh này chỉ chạy khi `alertfilterbydrug` không gửi được |
| 4 | Dùng `currentTreatmentWithPatientType` (kiểu `HisTreatmentWithPatientTypeInfoSDO`, kế thừa `HIS_TREATMENT`) thay `VHistreatment` | CLS và YHCT **không có** field `VHistreatment`; dùng field chung giúp 4 plugin dùng **cùng một** file partial, chỉ khác namespace |
| 5 | Điểm gọi prefetch ở CLS / Kidney / YHCT đặt cạnh `PrefetchMimsPatientProfile()` | 3 plugin này không có `GetHisExpMestMedicine()`. PK giữ đúng 3 điểm như thiết kế (Load, đổi ngày, đổi giờ chỉ định) |
| 6 | Tiêu đề khối HTML có **giá trị mặc định trong thư viện** (`MimsCrossPrescriptionOption.DEFAULT_BANNER_TITLE`) | Chống trường hợp deploy DLL lẻ thiếu satellite resource `vi/` làm `ResourceMessage` trả chuỗi rỗng |
| 7 | Khối HTML được chèn **ngay sau thẻ `<body>`** của HTML do MIMS transform | Nếu nối chuỗi ở ngoài, khối chữ nằm ngoài `<body>` — một số trình dựng bỏ qua |
| 8 | `MIMS_PREVIOUS_PRESCRIPTION_MAX_DRUG` để **hằng số trong code** (30), không đưa ra `HIS_CONFIG` | Là ngưỡng bảo vệ kỹ thuật, không phải tham số nghiệp vụ. Vượt ngưỡng → ghi `LogSystem.Warn` để CNTT biết |

## 14.3 Trạng thái build

| Thành phần | Kết quả |
|---|---|
| `HIS.Desktop.MIMS.Integration` | **Build sạch** (chỉ còn 1 warning `ConfigurationSettings.AppSettings` có từ trước) |
| `HIS.Desktop.Plugins.AssignPrescriptionPK` | **Build sạch** |
| `HIS.Desktop.Plugins.AssignPrescriptionCLS` | **Build sạch** |
| `HIS.Desktop.Plugins.AssignPrescriptionKidney` | **Chưa build được tại máy dev** — 20 lỗi thiếu assembly, HintPath trỏ ổ `F:` của máy dev khác (`HIS.UC.Icd`, `HIS.UC.DateEditor`, `HIS.UC.PeriousExpMestList`, `Library.*`). Lỗi có từ trước, **không** nằm ở file nào của việc 52540 |
| `HIS.Desktop.Plugins.AssignPrescriptionYHCT` | **Chưa build được tại máy dev** — 36 lỗi thiếu assembly (`HIS.UC.PatientSelect`, `HIS.UC.TreatmentFinish`, `HIS.UC.MenuPrint`, `Library.CheckIcd`, `Library.PrintServiceReq`, `HIS.Desktop.LocalStorage.LocalData`). Lỗi có từ trước, **không** nằm ở file nào của việc 52540 |

> Kidney và YHCT dùng **cùng một** file partial và **cùng một** đoạn sửa `CheckMIMS` / menu chuột phải với PK và CLS (đã build sạch), tên biến đã đối chiếu trùng khớp (`InstructionTime`, `oldServiceReq`, `currentTreatmentWithPatientType`, `mediMatyTypeADOs`, `MediMatyTypeInformationEvluation`, `ProcessLostToken`, `HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW`, namespace `...AssignPrescription{X}.ADO`). Cần build lại 2 plugin này trên máy có đủ bộ DLL tham chiếu.

## 14.4 File đã thay đổi

**Thư viện `HIS/HIS.Desktop.MIMS.Integration`**

| File | Thay đổi |
|---|---|
| `Core/MimsClient.cs` | Overload `PostXml(url, xml, extraFormParams, out isTimeout)`; 2 overload cũ giữ nguyên |
| `Core/MimsRequestBuilder.cs` | Overload `BuildDrugHealthAlertRequest(..., previousDrugs, requestMode)` sinh `<Prescribed>` hoặc gộp `<Prescribing>`; thêm `BuildAlertFilterByDrug` |
| `Models/MimsResult.cs` | Thêm `PreviousDrugCount`, `PreviousDrugGuids`, `IsAlertFilteredByDrug` |
| `Models/MimsPreviousDrugInfo.cs` | **MỚI** |
| `Models/MimsCrossPrescriptionOption.cs` | **MỚI** |
| `Modules/DrugHealthService.cs` | Overload mới `Check` / `CheckAndAlert` / `ShowResultAsync`; thêm `MapPreviousDrugs`, `BuildExtraFormParams`, `FilterAlertsByCurrentDrugs`, `MergeDrugsForVnCheck`, `PrependPreviousDrugBanner`, `ApplyCrossPrescriptionLog` |
| `HIS.Desktop.MIMS.Integration.csproj` | Khai báo 2 file model mới |

**Mỗi plugin (× 4)**

| File | Thay đổi |
|---|---|
| `Config/HisConfigCFG.cs` | 3 const key + 3 field + `IsCheckMimsCrossPrescription` + nạp trong `LoadConfig()` |
| `AssignPrescription/frmAssignPrescription__MimsCrossPrescription.cs` | **MỚI** — prefetch, gọi API, lọc QT-04…QT-07, dựng `MimsCrossPrescriptionOption` |
| `CheckMIMS()` | Truyền `previousDrugItems` / `previousDrugInfos` / option |
| `__InitMenuMouseRight.cs` | `ShowResultAsync` overload mới |
| `frmAssignPrescription.cs` | Gọi `PrefetchMimsCrossPrescription()` |
| `Resources/Message.Lang.*.resx` + `ResourceMessage.cs` | 2 key: `ThuocDangDungTuDonKhacTrongHoSo`, `DonNgayDungDenNgay` |
| `.csproj` | Khai báo file partial mới |

**Cấu hình**: `docs/SQL_52540_Config_KiemTraTuongTacThuocGiuaCacDon_MIMS.sql` — 3 câu `INSERT` theo mẫu chuẩn, có sẵn đoạn kiểm tra, đoạn bật tính năng và đoạn rollback.

## 14.5 Việc còn lại trước khi nghiệm thu

1. Build lại **Kidney** và **YHCT** trên máy có đủ DLL tham chiếu (mục 14.3).
2. Chạy SQL 3 config, để **giá trị trống** — xác nhận hệ thống chạy y như trước (TC-02).
3. Thử nghiệm trên môi trường **MIMS trial**: xác nhận `alertfilterbydrug` lọc đúng (TC-06) và đối chứng `CrossPrescriptionRequestMode` 1 so với 2 (TC-14) — đây là điểm chặn RR-02 / RR-03.
4. Bật `InteractionScopeOption = 2` cho 1 khoa nội trú, theo dõi `HIS_MIMS_INTERACTION_LOG` và số lượng popup.
