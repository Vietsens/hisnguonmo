# Tài liệu nghiệp vụ: Module Chỉ định dịch vụ kỹ thuật (AssignService)

**Module:** `HIS.Desktop.Plugins.AssignService`
**Tên hiển thị:** Chỉ định dịch vụ kỹ thuật
**Namespace:** `Inventec.Desktop.Plugins.AssignService`
**Loại module:** Form (MODULE_TYPE_ID__FORM)

---

## 1. Tổng quan

Module **AssignService** là chức năng cốt lõi trong hệ thống HIS, cho phép bác sĩ/nhân viên y tế chỉ định (ra y lệnh) các dịch vụ kỹ thuật cho bệnh nhân trong quá trình khám bệnh hoặc điều trị nội trú/ngoại trú. Các dịch vụ kỹ thuật bao gồm: xét nghiệm, chẩn đoán hình ảnh (siêu âm, X-quang, CT, MRI...), nội soi, phẫu thuật/thủ thuật, giường bệnh, và các dịch vụ y tế khác.

Module này đóng vai trò trung tâm trong quy trình khám chữa bệnh, kết nối giữa việc khám bệnh (examination) và thực hiện dịch vụ (service execution), đồng thời quản lý thông tin bảo hiểm y tế (BHYT), bảo lãnh viện phí, và tính chi phí cho bệnh nhân.

---

## 2. Chức năng chính

### 2.1. Chỉ định dịch vụ kỹ thuật
- Chọn và chỉ định nhiều dịch vụ cùng lúc cho một hồ sơ điều trị (treatment)
- Hiển thị danh sách dịch vụ dạng lưới (grid) với checkbox chọn/bỏ chọn
- Lọc dịch vụ theo phòng thực hiện, đối tượng bệnh nhân, khoa phòng
- Hỗ trợ chỉ định dịch vụ theo gói dịch vụ (service package)
- Hỗ trợ dịch vụ đính kèm (attachment service) tự động theo dịch vụ chính

### 2.2. Quản lý mã ICD (chẩn đoán)
- Nhập mã ICD chẩn đoán chính (bắt buộc hoặc tùy cấu hình)
- Nhập mã ICD nguyên nhân (cause ICD)
- Nhập mã ICD bệnh phụ/bệnh kèm theo (secondary/sub ICD) - hỗ trợ nhiều mã, phân tách bằng dấu ";"
- Tự động tải mã ICD từ lần khám trước (nếu có cấu hình)
- Kiểm tra mối quan hệ ICD - Dịch vụ (ICD_SERVICE) với nhiều mức độ kiểm tra

### 2.3. Quản lý thời gian y lệnh
- Chọn thời gian y lệnh (instruction time) - đơn hoặc nhiều thời điểm
- Hỗ trợ chọn nhiều ngày chỉ định cùng lúc qua form `frmMultiIntructonTime`
- Hỗ trợ chức năng dự trù thời gian (DuTru time) qua lịch popup
- Kiểm tra thời gian y lệnh phải lớn hơn thời gian bắt đầu khám (tùy cấu hình)

### 2.4. Quản lý bảo lãnh viện phí / BHYT
- Theo dõi mã bảo lãnh (guarantee code), số tiền đăng ký, đã sử dụng và số dư
- Kiểm tra số dư bảo lãnh trước khi lưu chỉ định
- Tính toán chi phí BHYT, chi phí bệnh nhân phải trả
- Cảnh báo khi vượt trần bảo hiểm (ceiling)
- Hỗ trợ nhiều đối tượng thanh toán: BHYT, viện phí (VP), nguồn chi trả khác (other pay source)

### 2.5. In phiếu chỉ định
- In phiếu yêu cầu dịch vụ (service request) với nhiều mẫu in
- Hỗ trợ lưu và in cùng lúc (Save & Print)
- Hỗ trợ in có chữ ký số (digital signature)
- Hỗ trợ in qua VBA/Office macro
- In hóa đơn (bill)
- Mẫu in phiếu hướng dẫn bệnh nhân: `Mps000276`

### 2.6. Gợi ý dịch vụ bằng AI
- Form `frmSuggestServiceAi` cho phép gợi ý dịch vụ dựa trên:
  - Mã ICD chẩn đoán
  - Giới tính bệnh nhân
  - Tuổi bệnh nhân
  - Số lượng gợi ý (top_n)

### 2.7. Quản lý ekip (nhóm phẫu thuật/thủ thuật)
- Form `FormEkipUser` cho phép chọn thành viên ekip cho dịch vụ phẫu thuật/thủ thuật
- Hiển thị thông tin nhân viên: tên đăng nhập, tên đầy đủ, bằng cấp, khoa phòng

### 2.8. Quản lý giường bệnh
- Form `FormBedInfo` hiển thị thông tin giường cho dịch vụ giường bệnh
- Kiểm tra dịch vụ giường có đầy đủ thông tin giường
- Chỉ cho phép chỉ định giường trong 1 ngày nếu đã có thông tin giường

### 2.9. Tạo nhóm dịch vụ
- Form `FormServiceGroupCreate` cho phép tạo nhóm dịch vụ tùy chỉnh
- Nhóm dịch vụ giúp chỉ định nhanh nhiều dịch vụ thường dùng cùng nhau

### 2.10. Xem lịch sử chỉ định
- Form `frmAssignServiceHistory` hiển thị lịch sử các chỉ định dịch vụ của bệnh nhân
- Thông tin: mã yêu cầu, thời gian y lệnh, phòng yêu cầu, số lượng

### 2.11. Xem chi tiết phiếu chỉ định
- Form `frmDetail` hiển thị danh sách các phiếu yêu cầu dịch vụ đã tạo
- Thông tin: mã phiếu, loại phiếu, thời gian y lệnh, phòng thực hiện, người tạo, tổng tiền

---

## 3. Giao diện người dùng

### 3.1. Bố cục form chính (`frmAssignService`)

Form chính sử dụng `DevExpress LayoutControl` với các vùng chức năng:

#### Thanh công cụ (Toolbar)
| Nút | Phím tắt | Mô tả |
|-----|----------|-------|
| Lưu | `Ctrl+S` | Lưu chỉ định dịch vụ |
| Lưu & In | `Ctrl+Shift+S` | Lưu và in phiếu chỉ định |
| In | `Ctrl+P` | In phiếu chỉ định |
| Mới | - | Tạo chỉ định mới |
| Sửa | `Ctrl+U` | Sửa chỉ định |

#### Vùng thông tin bác sĩ & phòng
- **Bác sĩ chỉ định (cboUser / txtLoginName):** Chọn bác sĩ/người chỉ định
- **Bác sĩ tư vấn (cboConsultantUser):** Chọn bác sĩ tư vấn
- **Phòng chỉ định (cboAssignRoom / beditRoom):** Chọn phòng ra y lệnh, hỗ trợ popup grid chọn phòng
- **Gói dịch vụ (cboPackage):** Chọn gói dịch vụ (nếu có)
- **KSK (cboKsk):** Chọn đợt khám sức khỏe

#### Vùng chẩn đoán (ICD)
- **ICD chính (txtIcdCode / cboIcds):** Nhập hoặc chọn mã bệnh chính
- **ICD nguyên nhân (txtIcdCodeCause / cboIcdsCause):** Nhập mã bệnh nguyên nhân
- **ICD phụ (popup grid):** Chọn nhiều mã bệnh phụ từ popup
- **Chẩn đoán sơ bộ (txtProvisionalDiagnosis):** Nhập mô tả chẩn đoán

#### Vùng lưới dịch vụ (Grid chính)
Lưới hiển thị danh sách dịch vụ có thể chỉ định với các cột:

| Cột | Mô tả |
|-----|-------|
| Chọn (checkbox) | Đánh dấu dịch vụ cần chỉ định |
| Mã dịch vụ | Mã dịch vụ (chỉ đọc) |
| Tên dịch vụ | Tên dịch vụ (chỉ đọc) |
| Nhóm PTTT | Phân loại phẫu thuật thủ thuật |
| Ghi chú y lệnh | Ghi chú hướng dẫn thực hiện |
| Bảo lãnh (checkbox) | Đánh dấu bảo lãnh |
| Đối tượng | Đối tượng thanh toán |
| Phòng thực hiện | Chọn phòng thực hiện dịch vụ |
| Số lượng | Số lượng dịch vụ (format: #,##0.00) |
| Đơn giá | Giá dịch vụ |
| Loại mẫu bệnh phẩm | Loại mẫu xét nghiệm |
| Điều kiện dịch vụ | Điều kiện thực hiện dịch vụ |
| Thời gian dự kiến | Thời gian dự kiến thực hiện |
| Chi phí ngoài gói | Chi phí không thuộc gói dịch vụ |
| Chia sẻ | Số lượng chia sẻ dịch vụ |

#### Vùng thời gian y lệnh
- **Ngày giờ y lệnh (dtInstructionTime):** Chọn thời gian ra y lệnh
- **Nhiều thời điểm (chkMultiIntructionTime):** Bật/tắt chế độ chọn nhiều ngày
- **Dự trù (txtDutruTime):** Chọn thời gian dự trù qua lịch popup

#### Vùng tổng hợp chi phí
| Label | Mô tả |
|-------|-------|
| Tổng DV BHYT | Tổng chi phí dịch vụ BHYT chi trả |
| Tổng DV khác | Tổng chi phí dịch vụ nguồn khác |
| Chênh BHYT | Chênh lệch BHYT |
| Số dư tài khoản | Số dư tài khoản bệnh nhân |
| Đã đóng | Số tiền bệnh nhân đã đóng |
| Còn thừa | Số tiền còn thừa |
| BN phải trả | Số tiền bệnh nhân phải trả |
| Tổng bảo lãnh | Tổng số tiền bảo lãnh |
| BMI / Cân nặng / Chiều cao | Thông tin thể chất bệnh nhân |

#### Các nút chức năng
- **Lưu (btnSave):** Lưu chỉ định
- **Lưu & In (btnSaveAndPrint):** Lưu và in phiếu
- **In (BtnPrint):** In phiếu
- **Tạm ứng DV (btnDepositService):** Tạo phiếu tạm ứng dịch vụ
- **Sửa (btnEdit):** Sửa chỉ định
- **Cấu hình (btnConfiguration):** Cấu hình hiển thị
- **Chỉ định AI (btnAssignAI):** Gợi ý dịch vụ bằng AI
- **In hóa đơn (btnPrintBill):** In hóa đơn
- **QR Pay (btnQRPay):** Thanh toán qua mã QR
- **Tạo nhóm DV (btnCreateServiceGroup):** Tạo nhóm dịch vụ

#### Toggle & Checkbox
- **Mở rộng tất cả (Switch_ExpendAll):** Toggle hiển thị tất cả dịch vụ
- **In (chkPrint):** Tự động in sau khi lưu
- **Ký số (chkSign):** Yêu cầu ký số
- **In văn bản ký (chkPrintDocumentSigned):** In văn bản đã ký
- **Cấp cứu (chkIsEmergency):** Đánh dấu chỉ định cấp cứu
- **Thông báo SMS (chkIsInformResultBySms):** Gửi kết quả qua SMS
- **In VBA (chkPrintVBA):** Sử dụng VBA để in
- **Bỏ qua kiểm tra (chkNotCheckService):** Bỏ qua kiểm tra dịch vụ
- **Tự động check PDDT (chkAutoCheckPDDT):** Tự động chọn phác đồ điều trị

---

## 4. Quy trình nghiệp vụ

### 4.1. Luồng chỉ định dịch vụ (Main Flow)

```
┌─────────────────┐
│  Mở form chỉ    │
│  định dịch vụ   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Tải dữ liệu:   │
│ - Treatment     │
│ - Patient       │
│ - Services      │
│ - ICD mặc định  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Hiển thị lưới   │
│ dịch vụ theo    │
│ phòng/đối tượng │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Người dùng:     │
│ - Chọn dịch vụ  │
│ - Nhập ICD      │
│ - Chọn thời gian│
│ - Chọn phòng TH │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Nhấn Lưu      │
│   (Ctrl+S)      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     Không hợp lệ    ┌──────────────┐
│   Validation    │ ──────────────────► │ Hiển thị lỗi │
│   (15+ bước)    │                      │ / cảnh báo   │
└────────┬────────┘                      └──────────────┘
         │ Hợp lệ
         ▼
┌─────────────────┐
│ Tạo SDO:        │
│ AssignServiceSDO│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Gọi API:        │
│ AssignService    │
│ ByInstruction   │
│ Times           │
└────────┬────────┘
         │
         ▼
┌─────────────────┐     Có    ┌──────────────┐
│ Lưu & In?       │ ────────► │ In phiếu     │
└────────┬────────┘           └──────────────┘
         │ Không
         ▼
┌─────────────────┐
│ Làm mới lưới    │
│ dịch vụ         │
└─────────────────┘
```

### 4.2. Luồng Validation chi tiết (khi lưu)

Quá trình validation được thực hiện tuần tự qua 15+ bước kiểm tra:

1. **ValidForSave()** - Kiểm tra cơ bản form (validation provider)
2. **ValidateGuaranteeBeforeSave()** - Kiểm tra bảo lãnh viện phí
3. **CheckPackage()** - Kiểm tra gói dịch vụ (số lần sử dụng tối đa/ngày)
4. **Kiểm tra loại mẫu bệnh phẩm** - Bắt buộc chọn loại mẫu cho XN (nếu cấu hình)
5. **Valid()** - Kiểm tra dịch vụ đã chọn (validation chung)
6. **CheckIcd()** - Kiểm tra ICD hợp lệ
7. **ValidServiceIcdForServiceSelected()** - Kiểm tra tương thích ICD - Dịch vụ
8. **ValidGenderServiceAllow()** - Kiểm tra dịch vụ phù hợp giới tính
9. **ValidSereServWithMinDuration()** - Kiểm tra thời gian tối thiểu giữa các lần chỉ định
10. **ValidSereServWithCondition()** - Kiểm tra điều kiện dịch vụ
11. **CheckMaxPatientbyDayOption()** - Kiểm tra số BN tối đa/ngày cho dịch vụ
12. **checkContraindicated()** - Kiểm tra chống chỉ định ICD - Dịch vụ
13. **ValidSereServWithOtherPaySource()** - Kiểm tra nguồn chi trả khác
14. **ValidCheckTreatmentTypeBed()** - Kiểm tra loại điều trị cho DV giường
15. **ValidSereServWithBed()** - Kiểm tra thông tin giường
16. **CheckIcdByRoom()** - Kiểm tra ICD theo phòng
17. **ValidFeeForExamTreatment()** - Kiểm tra phí cho hồ sơ khám
18. **CheckMaxAmount()** - Kiểm tra số lượng tối đa dịch vụ
19. **ValidICD()** - Validate ICD cuối cùng
20. **CheckTimeInDepartment()** - Kiểm tra thời gian trong khoa (nếu cấu hình)

---

## 5. Quy tắc nghiệp vụ & Validation

### 5.1. Quy tắc ICD - Dịch vụ

Hệ thống kiểm tra mối quan hệ giữa mã ICD (chẩn đoán) và dịch vụ được chỉ định thông qua bảng `HIS_ICD_SERVICE`. Có nhiều mức độ kiểm tra được cấu hình qua key `HIS.HIS_ICD_SERVICE.HAS_CHECK`:

| Giá trị | Hành vi |
|---------|---------|
| `3` | Kiểm tra dịch vụ có trong danh sách cấu hình ICD hay không. Nếu thiếu ICD, mở form `frmMissingIcd` để bổ sung |
| `4` | Kiểm tra nghiêm ngặt. Nếu dịch vụ không phù hợp ICD → **không cho lưu** |
| `5` | Kiểm tra và cập nhật lại ICD sau khi validate lần đầu |

### 5.2. Quy tắc chống chỉ định (Contraindication)

Cấu hình qua key `HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION`:
- Kiểm tra xem mã ICD có chống chỉ định với dịch vụ được chọn không
- Hiển thị cảnh báo gồm: tên ICD, tên dịch vụ, nội dung chống chỉ định
- Form `frmContraindicated` hiển thị chi tiết các chống chỉ định

### 5.3. Quy tắc giới tính

- Một số dịch vụ chỉ áp dụng cho giới tính nhất định (ví dụ: siêu âm tử cung chỉ cho nữ)
- Hàm `ValidGenderServiceAllow()` kiểm tra giới tính bệnh nhân với dịch vụ được chọn

### 5.4. Quy tắc thời gian tối thiểu

Cấu hình qua key `HIS.Desktop.IsSereServMinDurationAlert`:
- Kiểm tra khoảng cách thời gian tối thiểu giữa 2 lần chỉ định cùng một dịch vụ
- Ngăn chặn chỉ định trùng lặp trong thời gian ngắn

### 5.5. Quy tắc số lượng tối đa (MaxAmount)

- Mỗi dịch vụ có thể có giới hạn số lượng tối đa (`MAX_AMOUNT`) trong một hồ sơ điều trị
- Kiểm tra tổng số lượng đã chỉ định + số lượng mới có vượt quá giới hạn không
- Nếu vượt: hiển thị cảnh báo, người dùng chọn tiếp tục hoặc hủy

### 5.6. Quy tắc số BN tối đa/ngày

Cấu hình qua key `HIS.DESKTOP.ASSIGN_SERVICE.WARNING_MAX_PATIENT_BY_DAY.OPTION`:
- Kiểm tra số lượng bệnh nhân tối đa được chỉ định một dịch vụ trong ngày
- Hữu ích cho các dịch vụ có giới hạn công suất (ví dụ: máy MRI chỉ chụp được X bệnh nhân/ngày)

### 5.7. Quy tắc gói dịch vụ (Package)

Cấu hình qua key `HIS.DESKTOP.HIS_PACKAGE.MAX_PACKAGE_USAGE_PER_DAY.WARNING_OPTION`:
- **Option 1:** Nếu gói đã sử dụng vượt số lần tối đa/ngày → **chặn, không cho lưu**
- **Option 2:** Nếu gói đã sử dụng vượt → **cảnh báo**, người dùng chọn tiếp tục hoặc hủy

### 5.8. Quy tắc dịch vụ giường

- Dịch vụ giường (SERVICE_TYPE_ID = G) có quy tắc riêng:
  - Nếu đã có thông tin giường → chỉ được chỉ định trong 1 ngày (không được chọn nhiều ngày)
  - Nếu hồ sơ trong buồng có dịch vụ giường chưa có thông tin giường → yêu cầu bổ sung
  - Cấu hình `BedServiceType_NotAllow_For_OutPatient`:
    - `1`: Cảnh báo nếu chỉ định giường cho BN không phải nội trú (cho phép tiếp tục)
    - `2`: Chặn nếu chỉ định giường cho BN không phải nội trú

### 5.9. Quy tắc theo dõi (Tracking)

Cấu hình qua key `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED`:
- Nếu bật: bắt buộc chọn phiếu theo dõi (tracking) khi chỉ định DV cho BN nội trú/ngoại trú
- Trường theo dõi được đánh dấu bắt buộc (màu đỏ sẫm)
- Biến thể `IS_TRACKING_REQUIRED_PRESCRIPTION`: bắt buộc tracking cho đơn thuốc BN nội trú hoặc cấp cứu

### 5.10. Quy tắc bắt buộc nhập ICD

Cấu hình qua key `EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD`:
- Giá trị `1`: Bắt buộc nhập mã ICD chẩn đoán khi chỉ định dịch vụ
- Nếu không nhập → không cho lưu

### 5.11. Quy tắc bác sĩ chỉ định

Cấu hình qua key `MOS.HIS_SERVICE_REQ.REQ_USER_MUST_HAVE_DIPLOMA`:
- Nếu bật: bác sĩ chỉ định phải có bằng cấp (diploma)
- Cấu hình `His.Desktop.Plugins.ReqUser.IsShowingInTheSameDepartment`: chỉ hiển thị bác sĩ cùng khoa

### 5.12. Quy tắc đối tượng thanh toán

- Hỗ trợ nhiều đối tượng: BHYT, Viện phí (VP), và nguồn chi trả khác
- Kiểm tra dịch vụ có phù hợp với đối tượng thanh toán của bệnh nhân
- Cấu hình `MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE`: gán phòng theo đối tượng BN
- Cảnh báo khi chi phí BHYT vượt trần
- Cấu hình `HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT`: danh sách dịch vụ có giới hạn thanh toán BHYT

### 5.13. Quy tắc dự trù

- Dịch vụ loại **Khám** (SERVICE_TYPE_ID = 1) và **Khác** (SERVICE_TYPE_ID = 12) **không được phép dự trù**
- Nếu chọn USE_TIME (dự trù) cho các loại DV này → hiển thị thông báo chặn

### 5.14. Quy tắc đồng thời (Simultaneity)

Cấu hình qua key `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION` và `MOS.HIS_SERVICE_REQ.ASSIGN_SIMULTANEITY_OPTION`:
- Quản lý việc chỉ định dịch vụ đồng thời (cùng một lúc nhiều dịch vụ cho nhiều BN)

---

## 6. Cấu hình hệ thống

### 6.1. Bảng tổng hợp cấu hình quan trọng

| Config Key | Mô tả | Giá trị |
|------------|-------|---------|
| `EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD` | Bắt buộc nhập ICD khi chỉ định | `1`: Bắt buộc |
| `HIS.HIS_ICD_SERVICE.HAS_CHECK` | Kiểm tra ICD-Dịch vụ | `3`, `4`, `5` |
| `HIS.HIS_ICD_SERVICE.HAS_REQUIRE_CHECK` | Bắt buộc kiểm tra ICD-Dịch vụ | `true/false` |
| `HIS.ICD_SERVICE.CONTRAINDICATED.WARNING_OPTION` | Cảnh báo chống chỉ định | Số nguyên |
| `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICES.IS_TRACKING_REQUIRED` | Bắt buộc theo dõi | `true/false` |
| `MOS.HIS_SERVICE_REQ.PRESCRIPTION.IS_TRACKING_REQUIRED` | Bắt buộc theo dõi đơn thuốc | `true/false` |
| `MOS.HIS_SERVICE_REQ.REQ_USER_MUST_HAVE_DIPLOMA` | BS phải có bằng cấp | `true/false` |
| `HIS.Desktop.IsSereServMinDurationAlert` | Cảnh báo thời gian tối thiểu | `0`: Tắt, `1`: Bật |
| `HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT` | DS dịch vụ giới hạn BHYT | Chuỗi mã DV phân tách bởi "," |
| `HIS.DESKTOP.ASSIGN_SERVICE.WARNING_MAX_PATIENT_BY_DAY.OPTION` | Cảnh báo số BN tối đa/ngày | Số nguyên |
| `HIS.Desktop.Plugins.AssignService.AutoFilterRow` | Hiển thị dòng lọc tự động | `1`: Hiện |
| `HIS.Desktop.Plugins.AssignService.NoDifference` | Không chênh lệch | Chuỗi cấu hình |
| `MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION` | Chỉ định đồng thời | Chuỗi cấu hình |
| `MOS.HIS_SERVICE_REQ.ASSIGN_ROOM_BY_PATIENT_TYPE` | Gán phòng theo đối tượng | `true/false` |
| `HIS.Desktop.Plugins.AssignService.ShowDefaultExecuteRoom` | Hiển thị phòng TH mặc định | Chuỗi |
| `HIS.Desktop.Plugins.AssignService.BhytServiceColorCode` | Màu dịch vụ BHYT | Mã màu |
| `HIS.Desktop.Plugins.AssignService.IsNotAutoLoadAssignService` | Không tự động tải DV | `true/false` |
| `HIS.Desktop.Plugins.IsloadIcdFromExamServiceExecute` | Tải ICD từ lần khám trước | `true/false` |
| `HIS.Desktop.Plugins.AssignService.IsAllowingChooseServiceWhichInAttachments` | Cho phép chọn DV đính kèm | `true/false` |
| `His.Desktop.Plugins.ReqUser.IsShowingInTheSameDepartment` | BS cùng khoa | `true/false` |
| `HIS.Desktop.Plugins.AssignService.BedServiceType_NotAllow_For_OutPatient` | Giường không cho ngoại trú | `1`: Cảnh báo, `2`: Chặn |
| `HIS.Desktop.Plugins.AssignService.AssignBedServiceWithBedInfo` | DV giường kèm thông tin giường | `true/false` |
| `HIS.Desktop.Plugins.Assign.DefaultPatientTypeOption` | Mặc định đối tượng | `true/false` |
| `MOS.HIS_SERVICE_REQ.ALLOW_ASSIGN_OXYGEN` | Cho phép chỉ định oxy | `true/false` |
| `HIS.Desktop.Plugins.AssignService.IsSingleCheckservice` | Chỉ chọn 1 dịch vụ | `true/false` |
| `HIS.Desktop.Plugins.AssignService.IsSearchAll` | Tìm kiếm tất cả dịch vụ | `true/false` |
| `HIS.Desktop.Plugins.AssignConfig.ShowRequestUser` | Hiển thị người chỉ định | `1`: Hiện |
| `HIS.Desktop.Plugins.AssignServicePrintTEST` | In test | `true/false` |
| `MOS.IS_USING_SERVER_TIME` | Sử dụng giờ server | `true/false` |
| `HIS.Desktop.ShowServerTimeByDefault` | Mặc định hiển thị giờ server | `true/false` |
| `MOS.LIS.INTEGRATION_VERSION` | Phiên bản tích hợp LIS | Chuỗi |
| `MOS.LIS.INTEGRATE_OPTION` | Tùy chọn tích hợp LIS | Chuỗi |
| `HIS.Desktop.AI.SuggestAssignServicesInfo` | Thông tin gợi ý AI | Chuỗi |
| `HIS.Desktop.Plugins.IsCheckDepartmentInTimeWhenPresOrAssign` | Kiểm tra khoa theo thời gian | `true/false` |
| `HIS.Desktop.Plugins.InstructionTimeServiceMustBeGreaterThanStartTimeExam` | Thời gian y lệnh > thời gian khám | Chuỗi |
| `HIS.Desktop.FormClosingOption` | Tùy chọn đóng form | `true/false` |
| `HIS.Desktop.Plugins.CheckIcdWhenSave` | Kiểm tra ICD khi lưu | Chuỗi |
| `HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq` | Tự động xóa EMR khi sửa | Chuỗi |
| `HIS.Desktop.Plugins.AutoCheckIcd` | Tự động kiểm tra ICD | Chuỗi |
| `MOS.BHYT.EXCEED_DAY_ALLOW_FOR_IN_PATIENT` | Số ngày BHYT cho phép vượt | Số nguyên |
| `MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE` | Đặt đối tượng chính | Chuỗi |
| `HIS.Desktop.WarningOverTotalPatientPrice` | Cảnh báo vượt tổng phí BN | Chuỗi |
| `HIS.Desktop.Plugins.IsAllowSignaturePrint.ModuleLinks` | Cho phép in ký số | Chuỗi module links |
| `MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT` | Thanh toán theo phòng TH | `true/false` |
| `HIS.HIS_TRACKING.SERVICE_REQ_ICD_OPTION` | Tùy chọn ICD theo dõi | `true/false` |
| `HIS.Desktop.Plugins.AssignService.SetRequestRoomByBedRoomWhenBeingInSurgery` | Phòng yêu cầu = buồng bệnh khi PT | Chuỗi |
| `His.Desktop.IsUsingWarningHeinFee` | Cảnh báo phí BHYT | Chuỗi |
| `MOS.HIS_TREATMENT.GUARANTEE_CONNECTION_INFO` | Thông tin kết nối bảo lãnh | Chuỗi |

---

## 7. Tích hợp hệ thống

### 7.1. API Endpoints

Module giao tiếp với backend MOS qua các API sau:

| API Endpoint | Mục đích |
|-------------|----------|
| `api/HisServiceReq/AssignServiceByInstructionTimes` | **API chính** - Lưu chỉ định dịch vụ |
| `api/HisServiceReq/GetDynamic` | Lấy danh sách yêu cầu dịch vụ (dynamic) |
| `api/HisServiceReq/Get` | Lấy yêu cầu dịch vụ |
| `api/HisServiceReq/GetView6` | Lấy view yêu cầu dịch vụ |
| `api/HisSereServ/Get` | Lấy dữ liệu dịch vụ thực hiện |
| `api/HisSereServ/GetView1` | Lấy view dịch vụ thực hiện (v1) |
| `api/HisSereServ/GetView7` | Lấy view dịch vụ thực hiện (v7) |
| `api/HisSereServ/GetView8` | Lấy view dịch vụ thực hiện (v8) |
| `api/HisSereServ/GetViewD1` | Lấy view dịch vụ thực hiện (D1) |
| `api/HisTreatment/GetView7` | Lấy thông tin hồ sơ điều trị |
| `api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo` | Lấy hồ sơ kèm thông tin đối tượng |
| `api/HisPatient/GetView` | Lấy thông tin bệnh nhân |
| `api/HisPatient/GetCardBalance` | Lấy số dư thẻ bệnh nhân |
| `api/HisTestServiceReq/GetView` | Lấy view yêu cầu xét nghiệm |
| `api/HisTestServiceReq/Get` | Lấy yêu cầu xét nghiệm |
| `api/HisIcdService/Get` | Lấy cấu hình ICD-Dịch vụ |
| `api/EmrDocument/DeleteByCode` | Xóa tài liệu EMR theo mã |

### 7.2. Dữ liệu đầu vào API chính (AssignServiceSDO)

Khi gọi API `AssignServiceByInstructionTimes`, dữ liệu gửi đi bao gồm:

- **TreatmentId** - ID hồ sơ điều trị
- **ServiceReqDetails** - Danh sách chi tiết dịch vụ cần chỉ định (`List<ServiceReqDetailSDO>`)
- **InstructionTime / InstructionTimes** - Thời gian y lệnh (đơn hoặc nhiều)
- **UseTimes** - Thời gian dự trù sử dụng
- **ICD codes** - Mã ICD chính, phụ, nguyên nhân
- **PackageId** - ID gói dịch vụ (nếu có)
- **SessionCode** - Mã phiên (khi sửa)
- **ParentId** - ID yêu cầu cha
- **EkipId** - ID ekip phẫu thuật

### 7.3. Dữ liệu đầu ra API chính (HisServiceReqListResultSDO)

- **ServiceReqs** - Danh sách yêu cầu dịch vụ đã tạo
- **SereServs** - Danh sách dịch vụ thực hiện đã tạo

### 7.4. Mô hình dữ liệu chính

| Bảng/Entity | Mô tả |
|-------------|-------|
| `HIS_TREATMENT` | Hồ sơ điều trị |
| `HIS_SERVICE_REQ` | Yêu cầu dịch vụ (phiếu chỉ định) |
| `HIS_SERE_SERV` | Dịch vụ thực hiện (chi tiết dịch vụ trong phiếu) |
| `HIS_SERVICE` | Danh mục dịch vụ |
| `HIS_SERVICE_TYPE` | Loại dịch vụ |
| `HIS_ICD` | Danh mục mã bệnh ICD |
| `HIS_ICD_SERVICE` | Cấu hình mối quan hệ ICD - Dịch vụ |
| `HIS_PATIENT` | Thông tin bệnh nhân |
| `HIS_PATIENT_TYPE` | Đối tượng bệnh nhân |
| `V_HIS_PATIENT_TYPE_ALTER` | Thay đổi đối tượng BN |
| `HIS_ROOM` | Phòng ban |
| `HIS_BED` | Giường bệnh |
| `V_HIS_SERVICE_REQ_6` | View yêu cầu dịch vụ (v6) |

### 7.5. Tích hợp với các module khác

- **LIS (Laboratory Information System):** Tích hợp qua cấu hình `MOS.LIS.INTEGRATION_VERSION` để gửi yêu cầu xét nghiệm
- **EMR (Electronic Medical Record):** Tự động xóa tài liệu EMR khi sửa yêu cầu (cấu hình `AutoDeleteEmrDocumentWhenEditReq`)
- **Module In ấn (MPS):** Sử dụng `PrintServiceReqProcessor` để in phiếu chỉ định
- **Module Tạm ứng dịch vụ:** Kết nối qua nút `btnDepositService`
- **Module AssignServiceEdit:** Module sửa chỉ định dịch vụ đã lưu
- **Module Tracking (Theo dõi):** Liên kết phiếu theo dõi với chỉ định dịch vụ
- **Module AI Suggestion:** Gợi ý dịch vụ thông qua AI

---

## 8. Phím tắt

| Phím tắt | Chức năng |
|----------|-----------|
| `Ctrl+S` | Lưu chỉ định |
| `Ctrl+Shift+S` | Lưu và in phiếu chỉ định |
| `Ctrl+P` | In phiếu chỉ định |
| `Ctrl+U` | Sửa chỉ định |
| `Tab` | Di chuyển giữa các trường, mở popup autocomplete cho ICD |
| `Enter` | Xác nhận chọn trong combo/grid |
| `Double-click` | Sửa chi tiết dịch vụ trên lưới |

---

## 9. Cấu trúc mã nguồn

### 9.1. Thư mục và file chính

```
HIS.Desktop.Plugins.AssignService/
├── AssignServiceProcessor.cs          # Entry point, ModuleBase
├── RequestUriStore.cs                 # Định nghĩa API endpoints
├── Delegates.cs                       # Event delegates
├── IcdUtil.cs                         # Tiện ích ICD
├── KeyboardWorker.cs                  # Xử lý phím tắt
│
├── AssignService/                     # Form chính
│   ├── frmAssignService.cs            # Form chính - khai báo biến
│   ├── frmAssignService.Designer.cs   # Thiết kế giao diện
│   ├── frmAssignService__Load.cs      # Logic tải dữ liệu
│   ├── frmAssignService__Save.cs      # Logic lưu dữ liệu
│   ├── frmAssignService__Validate.cs  # Logic validation
│   ├── frmAssignService__BuildTree.cs # Xây dựng cây dịch vụ
│   ├── frmAssignService__InitCombo.cs # Khởi tạo combo box
│   ├── frmAssignService__UcIcd.cs     # Xử lý ICD chính
│   ├── frmAssignService__UcIcdCause.cs# Xử lý ICD nguyên nhân
│   ├── frmAssignService__UcIcdSub.cs  # Xử lý ICD phụ
│   ├── frmAssignService__UcDate.cs    # Xử lý ngày tháng
│   ├── frmAssignService__Print_v2.cs  # In ấn
│   ├── frmAssignService__TabIndex.cs  # Quản lý tab order
│   ├── frmAssignService__Dispose.cs   # Giải phóng tài nguyên
│   ├── frmAssignService__ReloadModuleByInputData.cs  # Tải lại module
│   ├── AssignServiceBehavior.cs       # Behavior pattern
│   ├── AssignServiceFactory.cs        # Factory pattern
│   ├── IAssignService.cs             # Interface
│   ├── frmDetail.cs                   # Form chi tiết phiếu
│   ├── frmAssignServiceHistory.cs     # Form lịch sử chỉ định
│   ├── frmMissingIcd.cs              # Form thiếu ICD
│   ├── frmMultiIntructonTime.cs      # Form nhiều thời gian
│   └── frmWaringConfigIcdService.cs  # Form cảnh báo ICD-DV
│
├── ADO/                               # Data Transfer Objects
│   ├── ChiDinhDichVuADO.cs           # DTO chỉ định dịch vụ
│   ├── GuaranteeInfoADO.cs           # DTO bảo lãnh
│   ├── ServiceGroupADO.cs            # DTO nhóm dịch vụ
│   ├── ContraindicatedADO.cs         # DTO chống chỉ định
│   ├── PreServiceReqsADO.cs          # DTO yêu cầu DV trước đó
│   ├── MissingIcdADO.cs              # DTO ICD thiếu
│   ├── AcsUserADO.cs                 # DTO người dùng
│   ├── HisBedADO.cs                  # DTO giường
│   ├── ICDADO.cs                     # DTO mã ICD
│   ├── LoaiPhieuInADO.cs             # DTO loại phiếu in
│   ├── ShareCountADO.cs              # DTO chia sẻ
│   ├── TrackingAdo.cs                # DTO theo dõi
│   ├── RequestAIADO.cs               # DTO yêu cầu AI
│   └── ResponseAIADO.cs              # DTO phản hồi AI
│
├── Config/                            # Cấu hình
│   ├── HisConfigCFG.cs               # Cấu hình HIS chính
│   ├── AppConfigKeys.cs              # Key cấu hình ứng dụng
│   ├── SereServCFG.cs                # Cấu hình dịch vụ thực hiện
│   └── ControlStateConstan.cs        # Hằng số trạng thái control
│
├── Validation/                        # Validation rules
│   ├── BenhPhuValidationRule.cs      # Validate ICD phụ
│   ├── TrackingValidationRule.cs     # Validate theo dõi
│   ├── IcdValidationRuleControl.cs   # Validate ICD
│   ├── DateValid.cs                  # Validate ngày tháng
│   └── RequestUserValidationRule.cs  # Validate người chỉ định
│
├── Get/                               # Data retrieval
│   └── SereServGet.cs                # Lấy dữ liệu dịch vụ thực hiện
│
├── Ai/                                # AI features
│   └── frmSuggestServiceAi.cs        # Form gợi ý DV bằng AI
│
├── BedInfo/                           # Thông tin giường
│   └── FormBedInfo.cs                # Form thông tin giường
│
├── FormContraindicated/               # Chống chỉ định
│   └── frmContraindicated.cs         # Form cảnh báo chống chỉ định
│
├── ServiceGroup/                      # Nhóm dịch vụ
│   └── FormServiceGroupCreate.cs     # Form tạo nhóm dịch vụ
│
├── FormEkipUser.cs                    # Form chọn ekip
├── frmDetailsSereServ.cs              # Form chi tiết DV thực hiện
├── frmPriceEdit.cs                    # Form sửa giá
├── frmServiceDebateConfirm.cs         # Form xác nhận tranh luận DV
│
├── Print/                             # In ấn
├── Resources/                         # Tài nguyên ngôn ngữ
└── Properties/                        # Thông tin assembly
```

### 9.2. Design Patterns sử dụng

- **Factory Pattern:** `AssignServiceFactory` tạo instance `IAssignService`
- **Behavior Pattern:** `AssignServiceBehavior` thực thi logic form
- **Processor Pattern:** `AssignServiceProcessor` là entry point, implement `IDesktopRoot`
- **Partial Class:** Form chính chia thành nhiều file partial class theo chức năng (Load, Save, Validate, BuildTree, InitCombo, UcIcd, Print...)

---

## 10. Lưu ý triển khai

1. **Hiệu suất:** Module tải nhiều dữ liệu danh mục (dịch vụ, ICD, phòng, đối tượng...) nên cần đảm bảo cache hiệu quả qua `BackendData`
2. **Cấu hình:** Có hơn 40 config key ảnh hưởng đến hành vi module. Cần kiểm tra kỹ các cấu hình khi triển khai tại cơ sở y tế mới
3. **Bảo hiểm BHYT:** Logic tính phí BHYT phức tạp, phụ thuộc vào nhiều yếu tố: đối tượng BN, loại dịch vụ, trần thanh toán, ngày vượt hạn thẻ
4. **Tích hợp LIS:** Cần cấu hình đúng phiên bản tích hợp (`INTEGRATION_VERSION`) để đồng bộ yêu cầu xét nghiệm với hệ thống LIS
5. **EMR:** Khi sửa chỉ định, có thể tự động xóa tài liệu EMR liên quan (cấu hình `AutoDeleteEmrDocumentWhenEditReq`)
