# HIS.Desktop.Plugins.TreatmentList

**Module ID:** `HIS.Desktop.Plugins.TreatmentList`
**Nhãn hiển thị:** Hồ sơ điều trị
**Ưu tiên (Priority):** 31
**Loại:** UC (User Control) — Plugin dạng thư viện (.dll)
**Framework:** .NET 4.5 · DevExpress v15.2

---

## 1. Tổng quan

Plugin quản lý và hiển thị **danh sách hồ sơ điều trị** (treatment records) của bệnh nhân trong hệ thống HIS. Cho phép tra cứu, lọc, thao tác (in ấn, mở hồ sơ, kiểm tra thẻ BHYT…) trên từng hồ sơ điều trị.

---

## 2. Cấu trúc thư mục

```
HIS.Desktop.Plugins.TreatmentList/
├── TreatmentListProcessor.cs          # Entry point – kế thừa ModuleBase, implement IDesktopRoot
├── TreatmentList/
│   ├── ITreatmentList.cs              # Interface của module (method Run())
│   ├── TreatmentListFactory.cs        # Factory tạo ITreatmentList
│   └── TreatmentListBehavior.cs       # Implement ITreatmentList, khởi tạo UCTreatmentList
├── UCTreatmentList.cs                 # UserControl chính (~49k dòng, partial class)
├── UCTreatmentList.Designer.cs        # Auto-generated UI (DevExpress)
├── UCTreatmentList.resx               # Resources UC chính
├── UCTreatmentList__Dispose.cs        # Giải phóng tài nguyên, hủy đăng ký event
├── UCTreatmentList__InitResource.cs   # Khởi tạo tài nguyên, cấu hình
├── UCTreatmentList__Item_Click.cs     # Xử lý sự kiện click dòng danh sách
├── UCTreatmentList__Print___KSK.cs    # Logic in ấn hợp đồng KSK
├── UCTreatmentList_Right_Click.cs     # Xử lý right-click context menu
├── Print/
│   ├── UCTreatmentList__Print__Init.cs  # Khởi tạo chức năng in
│   └── UCTreatmentList__Print.cs        # Logic in ấn chính
├── ADO/
│   ├── BenhAnCommonADO.cs             # Dữ liệu bệnh án chung (kế thừa BenhAnBase)
│   ├── ChiDinhDichVuADO.cs            # Dữ liệu chỉ định dịch vụ
│   ├── KieuBenhNhanADO.cs             # Kiểu bệnh nhân { ID, KieuBenhNhan }
│   ├── TrangThaiADO.cs                # Trạng thái điều trị { ID, TrangThai }
│   ├── ExcellDataADO.cs               # Dữ liệu xuất Excel KSK (kế thừa V_HIS_TREATMENT_4)
│   ├── TempExcelDataADO.cs            # Dữ liệu Excel tạm { ID_TREATMENT, TDL_SERVICE_NAME, CONCLUDE, VALUE }
│   └── BatchCheckResult.cs            # Kết quả kiểm tra thẻ BHYT theo lô
├── Config/
│   ├── HisConfigCFG.cs                # Cấu hình chính của module
│   └── BHXHLoginCFG.cs                # Cấu hình đăng nhập BHXH (user:pass)
├── Popup/
│   ├── frmCheckBHYT.cs/.Designer.cs   # Form kiểm tra thẻ BHYT
│   └── frmAIViewChatUrlFormat.cs      # Form cấu hình URL AI Chat
├── frmCauseOfDeath.cs/.Designer.cs    # Form nhập nguyên nhân tử vong (1100×768)
├── frmReasonOpenTreatment.cs          # Form nhập lý do mở hồ sơ điều trị
├── frmServiceType.cs/.Designer.cs     # Form chọn loại dịch vụ để in
├── frmTuberculosisTreatment.cs        # Form quản lý điều trị lao
├── frmReqTreatment.cs                 # Form yêu cầu điều trị
├── Base/
│   ├── ControlCode.cs                 # Mã ACS control
│   ├── PrintTypeCodeWorker.cs         # Hằng số mã in MPS
│   └── ResourceLangManager.cs         # Khởi tạo resource đa ngôn ngữ
├── Resources/
│   ├── Lang.vi.resx / Lang.en.resx / Lang.my.resx         # Chuỗi UI
│   └── Message.Lang.vi.resx / .en.resx / .my.resx          # Chuỗi thông báo
├── AgeUtil.cs                         # Tính tuổi từ số (năm/tháng/ngày/giờ)
├── EmrDataStore.cs                    # Lưu trữ treatmentCode dùng chung
├── HisRequestUriStore.cs              # Hằng số API endpoint
├── AllowPrintFinishCFG.cs             # Cấu hình quyền in sau khóa viện phí
├── LaunchChrome.cs                    # Mở trình duyệt Chrome (HSSK, mã thẻ)
├── Print.cs                           # Hàm in dùng MPS.MpsPrinter
├── KeyboardWorker.cs                  # Xử lý phím tắt (Ctrl+F, Ctrl+R)
└── PopupMenuProcessor.cs              # Quản lý context menu right-click
```

---

## 3. Kiến trúc khởi tạo

```
TreatmentListProcessor.Run()
    └─ TreatmentListFactory.MakeITreatmentList(CommonParam, object[])
           └─ TreatmentListBehavior.Run()
                  └─ UCTreatmentList (UserControlBase)
```

| Lớp | Vai trò |
|-----|---------|
| `TreatmentListProcessor` | Entry point, kế thừa `ModuleBase`, implement `IDesktopRoot` |
| `TreatmentListFactory` | Tạo instance `ITreatmentList`, bắt exception và ghi log |
| `TreatmentListBehavior` | Implement `ITreatmentList`, extend `Tool<DesktopToolContext>` |
| `UCTreatmentList` | UserControl chính, xử lý toàn bộ UI và nghiệp vụ |

---

## 4. Tính năng chính

### 4.1 Tìm kiếm & lọc danh sách
- Tìm theo mã bệnh nhân, tên bệnh nhân, số thẻ BHYT
- Lọc theo: loại bệnh nhân, khoa/phòng, trạng thái điều trị, khoảng ngày
- Phím tắt: **Ctrl+F** (tìm kiếm), **Ctrl+R** (làm mới)

### 4.2 Context menu (right-click)
Hơn 20 thao tác trên từng hồ sơ, gồm:
- Mở yêu cầu hồ sơ điều trị (`ReqOpenTreatmentRecord`)
- Chứng nhận điều trị lao (`CertificateOfTBTreatment`)
- Danh sách giường bệnh (`BedRoomList`)
- Nhật ký sự kiện (`EventLog`)
- Timeline bệnh nhân
- **Liên thông EmrToolKit dữ liệu chuyển tuyến** (`GuiGiayChuyenVienEmrToolkit`) — chỉ hiển thị khi đồng thời: (1) key `HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo` có giá trị (`EmrToolkitImportProcessor.IsConfigured()`), (2) hồ sơ có loại ra viện là **chuyển viện** (`TREATMENT_END_TYPE_ID == ID__CHUYEN`). Dựng JSON mẫu Giấy Chuyển Viện rồi gọi thư viện `HIS.Desktop.Plugins.Library.EmrToolkitImport`; hiển thị cửa sổ kết quả (JSON gửi/nhận). Handler ở `UCTreatmentList___EmrToolkit.cs`.
- Và nhiều thao tác khác

### 4.3 In ấn
- Phiếu khám bệnh vào viện
- Giấy ra viện
- Giấy chuyển viện
- Giấy hẹn khám / hẹn mổ
- Thẻ bệnh nhân
- Phiếu yêu cầu bệnh án ngoại trú
- In hợp đồng KSK (sổ, phiếu kết quả, tổng hợp)

### 4.4 Hợp đồng KSK (Khám sức khỏe định kỳ)
- In phiếu KSK định kỳ
- In sổ KSK định kỳ
- Xuất Excel kết quả KSK
- In phiếu kết quả CLSKSK

### 4.5 Form đặc biệt
| Form | Mục đích |
|------|---------|
| `frmCauseOfDeath` | Nhập nguyên nhân tử vong, tích hợp UCCauseOfDeath |
| `frmTuberculosisTreatment` | Quản lý thông tin điều trị lao |
| `frmReasonOpenTreatment` | Bắt lý do khi mở lại hồ sơ (khi config yêu cầu) |
| `frmCheckBHYT` | Kiểm tra thẻ BHYT |
| `frmServiceType` | Chọn loại dịch vụ trước khi in |

### 4.6 Tích hợp AI Chat
- Cấu hình URL AI chat qua `frmAIViewChatUrlFormat`
- Kết nối qua `HisConfigCFG.AIConnectionInfo` và `AIViewChatUrlFormat`

### 4.7 Đa ngôn ngữ
Hỗ trợ ba ngôn ngữ: **Tiếng Việt** (`vi`), **English** (`en`), **Myanmar** (`my`)

---

## 5. Cấu hình (HisConfigCFG)

| Config Key | Field | Mô tả |
|-----------|-------|-------|
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` | `PatientTypeCode__BHYT` | Mã loại bệnh nhân BHYT |
| `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE` | `PatientTypeCode__VP` | Mã loại bệnh nhân viện phí |
| `MOS.OLD_SYSTEM.INTEGRATION_TYPE` | `OldSystemIntegrationType` | Kiểu tích hợp hệ thống cũ |
| `XML.EXPORT.4210.IS_TREATMENT_DAY_COUNT_6556` | `IsTreatmentDayCount6556` | Đếm ngày điều trị theo thông tư 6556 |
| `MOS.HIS_HEIN_APPROVAL.SYNC_XML_FPT_OPTION` | `SYNC_XML_FPT_OPTION` | Tùy chọn đồng bộ XML FPT |
| `HIS.Desktop.EhrViewer.LinkAddress` | `HSSKAddress` | Địa chỉ hệ thống HSSK/EHR |
| `HIS.Desktop.EhrViewer.Base64UrlParamInput` | `HSSKBase64UrlParamInput` | Tham số URL HSSK dạng Base64 |
| `HIS.Desktop.Plugins.ExamServiceReqExecute.IsAllowPrintNoMedicinePrescription` | `IsAllowPrintNoMedicine` | Cho phép in đơn không có thuốc |
| `HIS.Desktop.Plugins.Library.PrintPrescription.Mps` | `MPS_PrintPrescription` | Mã MPS in đơn thuốc |
| `HIS.Desktop.Plugins.TreatmentList.UnlockConditionOption` | `IsUnlockConditionOption` | Tùy chọn điều kiện mở khóa |
| `HIS.Desktop.Plugins.TreatmentList.IsRequiredReasonWhenOpenTreatment` | `IsRequiredReasonWhenOpenTreatment` | Bắt buộc nhập lý do khi mở hồ sơ |
| `HIS.Desktop.Plugins.TreatmentList.SearchPatientsAcrossHospital` | `SearchPatientsAcrossHospital` | Tìm kiếm bệnh nhân toàn viện |
| `HIS.Desktop.AI.ConnectionInfo` | `AIConnectionInfo` | Thông tin kết nối AI |
| `HIS.Desktop.AI.ViewChatUrlFormat` | `AIViewChatUrlFormat` | Định dạng URL AI Chat |
| `MOS.HIS_TREATMENT.ALLOW_FINISH_DIFFERENT_DEPARTMENT` | `isAllowFinishDifferentDepartment` | Cho phép kết thúc điều trị khác khoa |
| `MOS.HIS_TREATMENT.GUARANTEE_CONNECTION_INFO` | `GuaranteeConnection` | Thông tin kết nối bảo lãnh |

### BHXHLoginCFG
| Config Key | Mô tả |
|-----------|-------|
| `...USERNAME` | Tên đăng nhập cổng BHXH |
| `...PASSWORD` | Mật khẩu cổng BHXH (định dạng `user:password`) |
| `...ADDRESS` | Địa chỉ cổng BHXH |
| `...ADDRESS_OPTION` | Tùy chọn địa chỉ cổng BHXH |

### AllowPrintFinishCFG
| Thuộc tính | Mô tả |
|-----------|-------|
| `ALLOW_PRINT_RA_VIEN` | Cho phép in giấy ra viện sau khi khóa viện phí |
| `ALLOW_PRINT_CHUYEN_VIEN` | Cho phép in giấy chuyển viện sau khi khóa viện phí |

---

## 6. API Endpoints (HisRequestUriStore)

| Hằng số | Endpoint | Mô tả |
|---------|----------|-------|
| `HIS_TREATMENT_GETVIEW4` | `api/HisTreatment/GetView4` | Lấy danh sách hồ sơ điều trị (view 4) |
| `HIS_TREATMENT_GET` | `api/HisTreatment/Get` | Lấy chi tiết một hồ sơ điều trị |
| `HIS_CARD_GET` | `api/HisCard/Get` | Lấy thông tin thẻ bệnh nhân |

---

## 7. Mã in MPS (PrintTypeCodeWorker)

| Hằng số | Mã MPS | Tên biểu mẫu |
|---------|--------|-------------|
| `PRINT_TYPE_CODE__IN_GIAY_KHAM_BENH_VAO_VIEN__MPS000007` | `Mps000007` | Phiếu khám bệnh vào viện |
| `PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008` | `Mps000008` | Giấy ra viện |
| `PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010` | `Mps000010` | Giấy hẹn khám |
| `PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011` | `Mps000011` | Giấy chuyển viện |
| `PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000012` | `Mps000012` | Phiếu yêu cầu bệnh án ngoại trú (mẫu 1) |
| `PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174` | `Mps000174` | Phiếu yêu cầu bệnh án ngoại trú (mẫu 2) |
| `PRINT_TYPE_CODE__BIEUMAU__THE_BENH_NHAN__MPS000178` | `Mps000178` | Thẻ bệnh nhân |
| `PRINT_TYPE_CODE__IN_GIAY_HEN_MO__Mps000389` | `Mps000389` | Giấy hẹn mổ |
| `PRINT_TYPE_CODE__MPS000399` | `Mps000399` | (Biểu mẫu bổ sung) |

---

## 8. Phím tắt

| Phím tắt | Chức năng |
|----------|-----------|
| `Ctrl+F` | Tìm kiếm (BtnSearch) |
| `Ctrl+R` | Làm mới danh sách (BtnRefreshs) |

---

## 9. Data Objects (ADO)

| Class | Mô tả |
|-------|-------|
| `KieuBenhNhanADO` | Kiểu bệnh nhân: `{ long ID, string KieuBenhNhan }` |
| `TrangThaiADO` | Trạng thái điều trị: `{ long ID, string TrangThai }` |
| `BenhAnCommonADO` | Bệnh án chung, kế thừa `BenhAnBase` từ EMR_MAIN |
| `ChiDinhDichVuADO` | Chỉ định dịch vụ: loại bệnh nhân, khoa/phòng, tỷ lệ, tiền cọc, hóa đơn |
| `ExcellDataADO` | Dữ liệu xuất Excel KSK (kế thừa `V_HIS_TREATMENT_4`): 40+ trường sinh hiệu (chiều cao, cân nặng, BMI, mạch, huyết áp, nhiệt độ, nhịp thở) |
| `TempExcelDataADO` | Dữ liệu Excel tạm: `{ ID_TREATMENT, TDL_SERVICE_NAME, CONCLUDE, VALUE }` |
| `BatchCheckResult` | Kết quả kiểm tra thẻ BHYT theo lô: `{ ROWNUM, TREATMENT_CODE, TDL_PATIENT_NAME, TDL_PATIENT_DOB, TDL_HEIN_CARD_NUMBER, Message, Note }` |

---

## 10. Phụ thuộc

### Thư viện DevExpress (v15.2)
- `DevExpress.Data`, `Utils`, `XtraBars`, `XtraEditors`
- `XtraGrid`, `XtraLayout`, `XtraNavBar`
- `XtraPrinting`, `XtraScheduler`, `XtraTreeList`, `XtraVerticalGrid`

### Thư viện nội bộ
| Nhóm | Thư viện |
|------|----------|
| Kiểm soát truy cập | `ACS.EFMODEL`, `ACS.Filter`, `ACS.SDO` |
| Dữ liệu HTC | `HTC.EFMODEL`, `HTC.Filter`, `HTC.SDO` |
| Dữ liệu MOS | `MOS.EFMODEL`, `MOS.Filter`, `MOS.SDO` |
| Desktop | `HIS.Desktop.Utility`, `HIS.Desktop.ApiConsumer`, `HIS.Desktop.LocalStorage` |
| Plugin library | `HIS.Desktop.Plugins.Library.*` (Print, Medical Record, EMR) |
| Tích hợp EMRTOOLKIT | `HIS.Desktop.Plugins.Library.EmrToolkitImport` — gửi JSON qua API EMRTOOLKIT (CreateToken → MaHoaJson → Import) |
| EMR | `EMR_MAIN` |
| BHYT | `His.Bhyt.ExportXml`, `His.Bhyt.InsuranceExpertise` |
| In ấn | `MPS.MpsPrinter` |
| UC phụ | `HIS.UC.UCCauseOfDeath` |
| Framework | `Inventec.Desktop.Core`, `Inventec.Common` |

---

## 11. Điểm tích hợp hệ thống

| Hệ thống | Mô tả |
|---------|-------|
| EMR | Hồ sơ bệnh án điện tử |
| BHXH | Kiểm tra, xác thực thẻ bảo hiểm y tế |
| MPS | Hệ thống in ấn mẫu biểu |
| AI Chat | Giao diện hỗ trợ AI theo URL cấu hình |
| Bảo lãnh | Kết nối thông tin bảo lãnh viện phí |
| HSSK/EHR | Mở hồ sơ sức khỏe điện tử qua Chrome |
| EMRTOOLKIT | Gửi dữ liệu hồ sơ (JSON) qua API import của EMRTOOLKIT — xem `docs/HIS.Desktop.Plugins.Library.EmrToolkitImport.md` |

---

## 12. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 30/06/2026 | nampp | Thêm menu chuột phải "Gửi Giấy chuyển viện qua EMRTOOLKIT (Test)" — gọi thư viện mới `HIS.Desktop.Plugins.Library.EmrToolkitImport`. Thêm `UCTreatmentList___EmrToolkit.cs` (handler + dựng JSON mẫu), `ItemType.GuiGiayChuyenVienEmrToolkit` trong `PopupMenuProcessor`, ProjectReference tới thư viện. |
