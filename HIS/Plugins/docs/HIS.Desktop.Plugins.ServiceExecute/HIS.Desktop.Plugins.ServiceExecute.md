# HIS.Desktop.Plugins.ServiceExecute

## 1. Tong quan

Module xu ly thuc hien dich vu can lam sang (CDHA, TDCN, phau thuat thu thuat). Cho phep nhan vien y te:

- Thuc hien va ghi nhan ket qua dich vu kham (X-quang, CT, MRI, sieu am, noi soi...)
- Quan ly ekip thuc hien (bac si, ky thuat vien, dieu duong)
- Chup/gan hinh anh tu camera hoac PACS
- Nhap mo ta ket qua, ket luan, ghi chu
- Gan may thuc hien, so phim
- In phieu ket qua
- Ket thuc yeu cau dich vu

**Loai module**: UserControl (hien thi dang tab trong main window)
**Phim tat**: `E`
**MEF Plugin ID**: `HIS.Desktop.Plugins.ServiceExecute`

---

## 2. Kien truc

### Cau truc file

```
HIS.Desktop.Plugins.ServiceExecute/
├── ServiceExecuteProcessor.cs              ← MEF registration + Run(args)
├── ServiceExecute/
│   ├── IServiceExecute.cs                  ← Interface
│   ├── ServiceExecuteFactory.cs            ← Factory tao behavior
│   └── ServiceExecuteBehavior.cs           ← Parse args, khoi tao UC
│
├── UCServiceExecute.cs                     ← Main UC - logic chinh, load, init
├── UCServiceExecute.Designer.cs            ← Designer (KHONG sua thu cong)
├── UCServiceExecute_PlusData.cs            ← Xu ly data (ekip, PTTT, time)
├── UCServiceExecute_PlusValidation.cs      ← Validation rules
├── UCServiceExecute_PlusDescription.cs     ← Rich text editor
├── UCServiceExecute_PlusPrint.cs           ← In an
├── UCServiceExecute_PlusCamera.cs          ← Camera capture
├── UCServiceExecute_PlusEkip.cs            ← Quan ly ekip
├── UCServiceExecute_Dispose.cs             ← Cleanup resources
│
├── ADO/                                    ← Data objects
│   ├── ServiceADO.cs                       ← Mo rong HIS_SERE_SERV
│   ├── PatientADO.cs                       ← Thong tin benh nhan
│   ├── ImageADO.cs                         ← Metadata hinh anh
│   ├── ResultADO.cs                        ← Ket qua thuc hien
│   ├── InformationADO.cs                   ← Session info + ekip list
│   ├── SereServHistoryADO.cs               ← Lich su dich vu
│   ├── ComboADO.cs                         ← Combo dropdown data
│   ├── IcdADO.cs                           ← Ma ICD
│   ├── IcdSkinPathologyADO.cs              ← ICD da lieu
│   ├── AcsUserADO.cs                       ← Thong tin user
│   ├── EmrColumnMappingADO.cs              ← EMR field mapping
│   ├── ThreadSereServADO.cs                ← Background thread data
│   ├── SereServFileADO.cs                  ← File dinh kem
│   ├── ImageRequestADO.cs                  ← PACS request
│   └── ImageResponseADO.cs                 ← PACS response
│
├── Config/
│   ├── ServiceExecuteCFG.cs                ← Cau hinh module
│   └── AppConfigKeys.cs                    ← Key cau hinh ung dung
│
├── Validation/
│   ├── BeginTimeValidationRule.cs           ← Validate thoi gian bat dau
│   ├── EndTimeValidationRule.cs             ← Validate thoi gian ket thuc
│   ├── StartTimeValidationRule.cs           ← Validate start time
│   ├── FinishTimeValidationRule.cs          ← Validate finish time
│   ├── FilmValidationRule.cs                ← Validate so phim
│   └── ...
│
├── PACS/
│   ├── PacsApiConsumer.cs                   ← PACS API consumer
│   └── ApiConsumerRaw.cs                    ← HTTP raw POST
│
├── ICD/
│   └── frmIcd.cs                            ← Form chon ma ICD
├── ViewImage/
│   └── FormViewImage*.cs                    ← Form xem hinh anh
├── EkipTemp/
│   └── frmEkipTemp.cs                       ← Form ekip tam
├── PtttTemp/
│   └── FormPtttTemp.cs                      ← Form PTTT tam
├── frmCamera.cs                             ← Form camera
├── frmMessage.cs                            ← Form thong bao
├── frmClsInfo.cs                            ← Form thong tin CLS
│
├── RequestUriStore.cs                       ← Danh sach API endpoints
├── SereServTempProcess.cs                   ← Xu ly template dich vu
├── ApplicationCaptureTypeWorker.cs          ← Loai capture camera
├── KeyboardWorker.cs                        ← Xu ly phim tat
├── WordProcess.cs                           ← Xu ly Word document
└── Resources/
    └── ResourceLanguageManager.cs           ← Da ngon ngu
```

### MEF Registration

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.ServiceExecute",   // Plugin ID
    "Xu ly dich vu",                         // Ten hien thi
    "Common",                                // Category
    16,                                      // Thu tu sap xep
    "weightedpies_32x32.png",                // Icon
    "E",                                     // Phim tat
    Module.MODULE_TYPE_ID__UC,               // Loai UserControl
    true, true)]
```

### Processor.Run() → Behavior → UC

```
ServiceExecuteProcessor.Run(args)
  → Parse: Module, V_HIS_SERVICE_REQ, ServiceExecuteADO
  → ServiceExecuteFactory.MakeIServiceExecute(module, serviceReq, ...)
    → ServiceExecuteBehavior.Run()
      → new UCServiceExecute(module, serviceReq, refreshData, isExecuter, isReadResult)
```

**Tham so dau vao (args)**:

| Kieu | Mo ta |
|------|-------|
| `Inventec.Desktop.Common.Modules.Module` | Module metadata |
| `V_HIS_SERVICE_REQ` | Yeu cau dich vu (truyen truc tiep) |
| `ServiceExecuteADO` | ADO chua: ServiceReq, RefreshData, IsExecuter, IsReadResult |

---

## 3. Giao dien (UI)

### Layout chinh

UCServiceExecute la UserControl chia thanh cac vung:

**Vung thong tin benh nhan** (phia tren):
- `LblPatientName` — Ten benh nhan
- `LblPatientDob` — Ngay sinh
- `LblGender` — Gioi tinh
- `LblHeinCardNumber` — So the BHYT
- `LblPatientType` — Doi tuong
- `LblAddress` — Dia chi
- `LblTreatmentType` — Loai dieu tri
- `LblExecuteName`, `LblKtv`, `LblNurse` — Nguoi thuc hien

**Grid danh sach dich vu** (`gridControlSereServ`):
- SoPhieu — Ma phieu
- Ten dich vu, ma dich vu, so luong
- So phim, may thuc hien
- Thoi gian bat dau/ket thuc
- Trang thai (IsProcessed)

**Grid ekip** (`gridControlEkip`):
- Vai tro thuc hien
- Tai khoan/ten nhan vien
- Ky ten dien tu

**Vung nhap ket qua**:
- `txtServiceReqCode` — Ma yeu cau dich vu
- `dtBeginTime`, `dtEndTime` — Thoi gian thuc hien
- Rich text editor — Mo ta ket qua (DevExpress RichEdit hoac Telerik)
- `txtConclude` — Ket luan
- `txtNote` — Ghi chu
- `cboMachine` — Chon may
- TrackBar zoom — Phong to/thu nho editor

**Vung hinh anh** (`lcgImage`):
- Image gallery/viewer
- Camera frame capture
- `cboConnectionType` — Loai ket noi camera (USB/SVideo)
- `chkAutoCapture` — Tu dong chup
- `spnTotalCapture` — So khung hinh
- `spnTotalTimeToCapture` — Thoi gian chup

**Cac checkbox tuy chon**:
- `chkAttachImage` — Gan hinh anh
- `chkSign` — Ky so
- `chkPrint` — In sau khi luu
- `chkAutoFinish` — Tu dong ket thuc
- `chkClose` — Dong sau khi xong
- `chkSaveImageToFile` — Luu anh ra file local
- `chkUpper` — Viet hoa
- `chkKeTieuHao` — Ke tieu hao

**Cac nut chuc nang**:
- `btnSave` — Luu ket qua thuc hien
- `btnFinish` — Ket thuc yeu cau dich vu
- `btnPrint` — In phieu ket qua
- `btnAssignService` — Chi dinh them dich vu
- `btnAssignPrescription` — Lien ket don thuoc
- `btnCamera` — Mo camera chup anh
- `btnLoadImage` — Tai anh tu PACS
- `btnTrackingList` — Duyet template
- `btnSereServTempList` — Danh sach template dich vu

---

## 4. Luong xu ly chinh

### 4.1 Load du lieu

```
UCServiceExecute_Load()
  → GetDataFromRam()           // Load cache: dich vu, phong, vai tro
  → FillDataToGrid()           // Load grid dich vu tu API
    → ReLoadSereServ()         // Goi api/HisSereServ/GetView5
  → FillDataToCombo()          // Load combo options
  → ProcessPatientInfo()       // Hien thi thong tin benh nhan
  → InitControlState()         // Doc trang thai UI tu cache
  → SetDataSourceEkipUser()   // Khoi phuc ekip tu session
```

### 4.2 Chon dich vu

```
gridViewSereServ_FocusedRowChanged()
  → SereServClickRow()
    → Load HIS_SERE_SERV_EXT cho dich vu duoc chon
    → Hien thi mo ta, ket luan, ghi chu
    → Load ekip da gan
    → Load hinh anh dinh kem
    → Cap nhat thoi gian bat dau/ket thuc
```

### 4.3 Luu ket qua (btnSave_Click)

```
btnSave_Click()
  → Validate ekip (theo IsSampleInfoOption)
  → Validate may thuc hien
  → Validate thoi gian (begin >= instruction, end >= begin)
  → Validate vai tro ekip (IS_SINGLE_IN_EKIP)
  → Goi api/HisSereServ/CheckExecuteTimes   // Kiem tra trung thoi gian
  → Tao/cap nhat HIS_SERE_SERV_EXT:
      - BEGIN_TIME, END_TIME
      - DESCRIPTION_SAR_PRINT_ID (mo ta)
      - CONCLUDE, NOTE
      - MACHINE_ID, NUMBER_OF_FILM
      - SUBCLINICAL_PRES_LOGINNAME, SUBCLINICAL_NURSE_LOGINNAME
  → Goi api/HisSereServExt/UpdateSdo        // Luu ket qua
  → Gan hinh anh neu co
  → Refresh grid
```

### 4.4 Ket thuc yeu cau (btnFinish_Click)

```
btnFinish_Click()
  → Validate tat ca dich vu da co mo ta
  → Tinh START_TIME = min(begin times)
  → Tinh FINISH_TIME = max(end times)
  → Goi api/HisServiceReq/FinishWithTime    // Ket thuc yeu cau
  → RefreshData?.Invoke(serviceReqResult)    // Callback ve parent
  → Disable form (khong cho sua tiep)
```

---

## 5. API Calls

### Service Request

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisServiceReq/GetView` | GET | Lay thong tin yeu cau dich vu |
| `api/HisServiceReq/Get` | GET | Lay yeu cau theo ID |
| `api/HisServiceReq/FinishWithTime` | POST | Ket thuc yeu cau voi thoi gian |
| `api/HisServiceReq/Finish` | POST | Ket thuc yeu cau (legacy) |

### Service (Sere Serv)

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisSereServ/Get` | GET | Lay danh sach dich vu trong yeu cau |
| `api/HisSereServ/GetView5` | GET | Lay view mo rong dich vu |
| `api/HisSereServ/UpdateWithFile` | POST | Cap nhat dich vu voi file |
| `api/HisSereServ/CheckExecuteTimes` | POST | Kiem tra trung thoi gian thuc hien |

### Service Extension (Ket qua)

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisSereServExt/Get` | GET | Lay ket qua thuc hien |
| `api/HisSereServExt/CreateWithFile` | POST | Tao ket qua voi file |
| `api/HisSereServExt/UpdateWithFile` | POST | Cap nhat ket qua voi file |
| `api/HisSereServExt/CreateSdo` | POST | Tao qua SDO |
| `api/HisSereServExt/UpdateSdo` | POST | Cap nhat qua SDO |
| `api/HisSereServExt/CheckConflict` | POST | Kiem tra xung dot du lieu |

### File dinh kem

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisSereServFile/Get` | GET | Lay danh sach file |
| `api/HisSereServFile/Create` | POST | Tao file record |
| `api/HisSereServFile/Update` | POST | Cap nhat file |
| `api/HisSereServFile/Delete` | POST | Xoa file |

### Khac

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisTreatment/Get` | GET | Lay thong tin dieu tri |
| `api/HisTreatment/GetTreatmentWithPatientTypeInfoSdo` | GET | Dieu tri + doi tuong |
| `api/HisSereServBill/Get` | GET | Thong tin thanh toan |
| `api/HisSereServDeposit/Get` | GET | Thong tin tam ung |
| `api/HisSeseDepoRepay/Get` | GET | Thong tin hoan ung |
| `api/HisBedLog/GetView` | GET | Lich su giuong |
| `api/HisDhst/Get` | GET | Dau hieu sinh ton |
| `api/HisSuimSetySuin/Get` | GET | Thong so mau |
| `api/HisSuimIndex/GetView` | GET | Chi so mau |
| `api/HisSereServSuin/GetView` | GET | Ket qua mau |
| `api/His/LayThongTinHinhAnh` | POST | Lay anh tu PACS |

**API Consumer**: `BackendAdapter` voi `ApiConsumer.ApiConsumers.MosConsumer`
**PACS Consumer**: `PacsApiConsumer.PacsConsumer` (HTTP POST rieng)

---

## 6. Cau hinh (ServiceExecuteCFG)

| Config key | Mo ta | Gia tri |
|------------|-------|---------|
| `ShowImageCFG` | An/hien vung hinh anh CDHA | bool |
| `OptionPrint` | Hien thi vung chon template in | bool |
| `OptionDescription` | Loai editor (Telerik/DevExpress) | string |
| `SubclinicalProcessingInformationOption` | Yeu cau ekip: "1"=BHYT, "2"=tat ca | string |
| `SubclinicalMachineOption` | Yeu cau may khi chua nhap thong tin may CLS: 1=canh bao, 2=chan, 3=canh bao (BHYT), 4=chan (BHYT), 5=canh bao (DV co cau hinh Dich vu-May), 6=chan (DV co cau hinh Dich vu-May), 7=canh bao (BHYT + co cau hinh Dich vu-May), 8=chan (BHYT + co cau hinh Dich vu-May). Khac 1-8: khong xu ly. Tieu thu boi ServiceExecute (1-4) va TestServiceReqExcute (1-8) | string |
| `ThoiGianKetThuc` | Cau hinh thoi gian ket thuc | string |
| `ServicePTTT` | Tu dong tao ban ghi PTTT | bool |
| `IsAssignServiceSimulTaneityOption` | Kiem tra trung thoi gian thuc hien | bool |
| `MachineShowOption` | Cach hien thi may: "2"=smart selection | string |

---

## 7. ControlState (Luu trang thai local)

Su dung `HIS.Desktop.Library.CacheClient.ControlStateWorker`:

| Control | Mo ta | Kieu luu |
|---------|-------|----------|
| `chkForPreview` | Xem truoc khi in | bool |
| `ChkAutoFinish` | Tu dong ket thuc | bool |
| `trackBarZoom` | Muc zoom editor | long |
| `chkAttachImage` | Gan hinh anh | bool |
| `chkClose` | Dong sau khi xong | bool |
| `chkPrint` | In sau khi luu | bool |
| `chkSign` | Ky so | bool |
| `chkSaveImageToFile` | Luu anh local | "1\|path" hoac "0\|" |
| `chkUpper` | Viet hoa | bool |
| `chkAutoCapture` | Tu dong chup | bool |
| `spnTotalCapture` | So khung hinh chup | int |
| `spnTotalTimeToCapture` | Thoi gian chup | int |
| `chkKeTieuHao` | Ke tieu hao | bool |
| `xtraTabControl1` | Trang thai pin tab | IsPin flag |
| `InformationADO` | Ekip list (session only) | JSON |

---

## 8. ADO Objects

### ServiceADO (Mo rong HIS_SERE_SERV)

| Property | Kieu | Mo ta |
|----------|------|-------|
| `SoPhieu` | string | Ma phieu dinh dang |
| `conclude` | string | Ket luan |
| `note` | string | Ghi chu |
| `description` | string | Mo ta ket qua |
| `MACHINE_ID` | long? | ID may thuc hien |
| `NUMBER_OF_FILM` | long? | So phim |
| `MustHavePressBeforeExecute` | bool | Bat buoc co don thuoc truoc |
| `IsProcessed` | bool | Trang thai da xu ly |
| `lstEkipUser` | List | Danh sach thanh vien ekip |

### Cac ADO khac

| ADO | Mo ta |
|-----|-------|
| `PatientADO` | Thong tin benh nhan |
| `ImageADO` | Metadata hinh anh |
| `ResultADO` | Ket qua thuc hien |
| `InformationADO` | Session info + ekip |
| `SereServHistoryADO` | Lich su dich vu |
| `ComboADO` | Du lieu dropdown |
| `IcdADO` | Ma benh ICD |
| `AcsUserADO` | Thong tin nguoi dung |
| `EmrColumnMappingADO` | EMR mapping |
| `ThreadSereServADO` | Data cho background thread |
| `SereServFileADO` | Metadata file dinh kem |
| `ImageRequestADO` | PACS request |
| `ImageResponseADO` | PACS response |

---

## 9. Validation Rules

| Rule | File | Mo ta |
|------|------|-------|
| `BeginTimeValidationRule` | Validation/ | Begin >= Instruction time |
| `EndTimeValidationRule` | Validation/ | End >= Begin |
| `StartTimeValidationRule` | Validation/ | Start > Instruction (co du sai cho phep) |
| `FinishTimeValidationRule` | Validation/ | Finish > End |
| `FilmValidationRule` | Validation/ | So phim trong khoang hop le |

**Cac rang buoc them**:
- Thoi gian khong vuot qua thoi gian ra vien/chuyen khoa
- Thoi gian khong vuot qua max process time cua dich vu
- Ekip bat buoc theo cau hinh (BHYT hoac tat ca)
- Vai tro IS_SINGLE_IN_EKIP chi duoc 1 nguoi
- Don thuoc bat buoc truoc khi thuc hien (MustHavePressBeforeExecute)
- Mo ta ket qua bat buoc truoc khi ket thuc

---

## 10. Tinh nang dac biet

### 10.1 Camera Capture

- Tich hop camera qua thu vien **AForge** (USB/SVideo)
- Tu dong phat hien camera khi load (`AppConfigKeys.IsInitCameraDefault`)
- Che do tu dong chup: `chkAutoCapture` + `spnTotalCapture` + `spnTotalTimeToCapture`
- Hinh chup luu tam tai `\Img\Temp`, gan vao dich vu khi luu

### 10.2 PACS Integration

- Ket noi PACS server qua `PacsApiConsumer` (HTTP POST/JSON)
- Endpoint: `api/His/LayThongTinHinhAnh`
- Load hinh anh DICOM tu PACS theo yeu cau dich vu
- Cau hinh trong `PacsCFG`

### 10.3 PTTT (Phau thuat Thu thuat)

- Voi dich vu loai PT (Phau thuat) hoac TT (Thu thuat)
- Tu dong tao ban ghi `HIS_SERE_SERV_PTTT` voi ma ICD tu yeu cau
- Cau hinh: `ServiceExecuteCFG.ServicePTTT`
- Form phu: `FormPtttTemp`

### 10.4 Chu ky so

- Tich hop `Inventec.Common.SignLibrary`
- Kich hoat qua `chkSign`
- Ky truoc khi ket thuc yeu cau

### 10.5 Text Library

- Cac doan van ban mau tu `HIS_TEXT_LIB`
- Hot-key de chen nhanh vao editor
- Phan loai theo user/public
- Chen qua SendKeys (auto-type)

### 10.6 Xu ly hang loat (All-in-One)

- Voi yeu cau co nhieu dich vu, co the xu ly tat ca cung luc
- `listServiceADOForAllInOne` theo doi cac dich vu xu ly dong loat
- Ap dung cung mo ta, ket luan, ekip cho nhieu dich vu

### 10.7 EMR Integration

- Lien ket voi benh an dien tu qua `HIS.Desktop.Plugins.Library.EmrGenerate`
- Mapping column EMR qua `EmrColumnMappingADO`

---

## 11. Dependencies

### Core Libraries

| Thu vien | Vai tro |
|----------|---------|
| `HIS.Desktop.Utility` | FormBase, UserControlBase |
| `HIS.Desktop.ApiConsumer` | API consumer (MosConsumer) |
| `HIS.Desktop.Common` | BusinessBase, delegates, interfaces |
| `HIS.Desktop.ADO` | ServiceExecuteADO |
| `HIS.Desktop.LocalStorage.*` | Config, cache |
| `HIS.Desktop.Library.CacheClient` | ControlState luu trang thai |
| `HIS.Desktop.Print` | In an |
| `HIS.Desktop.ModuleExt` | Module extensions |
| `HIS.Desktop.Controls.Session` | Session management |
| `HIS.Desktop.IsAdmin` | Kiem tra quyen admin |

### Plugin Libraries

| Plugin | Vai tro |
|--------|---------|
| `HIS.Desktop.Plugins.Library.FormOtherSereServ` | Form chi dinh dich vu |
| `HIS.Desktop.Plugins.Library.EmrGenerate` | Tao benh an dien tu |
| `HIS.Desktop.Plugins.Library.MediStockExpend` | Ke tieu hao |
| `HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT` | Canh bao vien phi |
| `HIS.UC.Icd` | Chon ma ICD |
| `HIS.UC.SecondaryIcd` | Chon ICD phu |
| `HIS.UC.TreatmentFinish` | Ket thuc dieu tri |

### Data Models (tu lib/)

| Model | Mo ta |
|-------|-------|
| `MOS.EFMODEL.DataModels.*` | Entity MOS (HIS_SERE_SERV, HIS_SERVICE_REQ, ...) |
| `MOS.Filter.*` | Filter cho API GET |
| `MOS.SDO.*` | Service Data Objects |
| `ACS.EFMODEL.DataModels.*` | Entity phan quyen |
| `EMR.EFMODEL`, `EMR.Filter` | Entity benh an dien tu |
| `IMSys.DbConfig.HIS_RS` | Hang so cau hinh DB |

### Third-party

| Thu vien | Vai tro |
|----------|---------|
| DevExpress v15.2 | UI controls (Grid, Layout, RichEdit, Bars) |
| Telerik WinControls | Rich text editor (thay the) |
| AForge.* | Camera capture (Video, DirectShow, Imaging) |
| Aspose.Cells/Words/Pdf | Tao tai lieu |
| Newtonsoft.Json | JSON serialization |
| AutoMapper | Object mapping |
| `Inventec.Common.*` | Adapter, Logging, DateTime, Mapper, TypeConvert |
| `Inventec.Desktop.Common.*` | Message manager, Language manager |
| `Inventec.UC.ImageLib` | Camera device control |
| `Inventec.Common.SignLibrary` | Chu ky so |
