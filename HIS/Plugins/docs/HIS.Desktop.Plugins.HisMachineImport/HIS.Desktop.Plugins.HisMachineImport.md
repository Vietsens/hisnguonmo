# HIS.Desktop.Plugins.HisMachineImport

## 1. Tong quan

Module import danh sach may CLS (Can Lam Sang) tu file Excel vao he thong HIS. Cho phep nhan vien:

- Tai file mau (template) Excel de nhap du lieu may CLS
- Chon file Excel da nhap lieu de import vao he thong
- Kiem tra (validate) du lieu truoc khi luu: trung lap, do dai truong, trang thai may
- Xem va loc dong loi / dong hop le
- Xoa dong du lieu khong mong muon
- Luu danh sach may CLS hop le vao co so du lieu

**Loai module**: Form popup (hien thi dang cua so rieng)
**Phim tat**: `A`
**MEF Plugin ID**: `HIS.Desktop.Plugins.HisMachineImport`

---

## 2. Kien truc

### Cau truc file

```
HIS.Desktop.Plugins.HisMachineImport/
├── HisMachineImportProcessor.cs              ← MEF registration + Run(args)
├── HisMachineImport/
│   ├── IHisMachineImport.cs                  ← Interface
│   ├── HisMachineImportFactory.cs            ← Factory tao behavior
│   ├── HisMachineImportBehavior.cs           ← Parse args, khoi tao Form
│   ├── frmHisMachineImport.cs                ← Main form - logic chinh (722 LOC)
│   └── frmHisMachineImport.Designer.cs       ← Designer (KHONG sua thu cong)
│
├── ADO/
│   └── HisMachineImportADO.cs                ← MachineImportADO - mo rong HIS_MACHINE
│
└── Message/
    └── MessageImport.cs                       ← Hang so thong bao loi
```

### MEF Registration

```csharp
[ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.HisMachineImport",   // Plugin ID
    "Import",                                  // Ten hien thi
    "Common",                                  // Category
    14,                                        // Thu tu sap xep
    "pivot_32x32.png",                         // Icon
    "A",                                       // Phim tat
    Module.MODULE_TYPE_ID__FORM,               // Loai Form popup
    true, true)]
```

### Processor.Run() → Factory → Behavior → Form

```
HisMachineImportProcessor.Run(args)
  → Parse: Module tu args
  → HisMachineImportFactory.MakeHisMachineImport(param, args)
    → HisMachineImportBehavior.Run()
      → new frmHisMachineImport(moduleData)
```

**Tham so dau vao (args)**:

| Kieu | Mo ta |
|------|-------|
| `Inventec.Desktop.Common.Modules.Module` | Module metadata (text, icon, shortcut) |

---

## 3. Giao dien (UI)

### Layout chinh

frmHisMachineImport la Form popup voi cac thanh phan:

**Cac nut chuc nang** (phia tren):

| Control | Ten | Mo ta |
|---------|-----|-------|
| `btnDownLoadFile` | Tai file mau | Tai template Excel ve may |
| `btnChooseFile` | Chon file | Chon file Excel de import |
| `btnShowLineError` | Dong loi / Dong khong loi | Chuyen doi hien thi dong loi va dong hop le |
| `btnSave` | Luu | Luu du lieu hop le vao DB |

**Grid du lieu** (`gridControlHisMachineImport` + `gridViewHisMachineImport`):

| Cot | Field | Mo ta |
|-----|-------|-------|
| STT | (Unbound) | So thu tu, tu dong tinh |
| Loi | ERROR_ | Nut hien thi chi tiet loi (neu co) |
| Xoa | (Delete button) | Xoa dong khoi danh sach |
| Ma may CLS | MACHINE_CODE | Ma dinh danh may |
| Ten may CLS | MACHINE_NAME | Ten may |
| HD tu ngay | CONTRACT_FROM_DMY | Ngay bat dau hop dong (dd/MM/yyyy) |
| HD den ngay | CONTRACT_TO_DMY | Ngay ket thuc hop dong (dd/MM/yyyy) |
| Tu ngay | FROM_TIME_DMY | Ngay bat dau su dung (dd/MM/yyyy) |
| Den ngay | TO_TIME_DMY | Ngay ket thuc su dung (dd/MM/yyyy) |
| So serial | SERIAL_NUMBER | So serial may |
| Ma nguon kinh phi | SOURCE_CODE | Ma nguon kinh phi (toi da 2 ky tu) |
| Ma phong | ROOM_IDS | Danh sach ID phong |
| Dia chi tich hop | INTEGRATE_ADDRESS | Dia chi tich hop may |
| Ky hieu | SYMBOL | Ky hieu may |
| Ma nhom may | MACHINE_GROUP_CODE | Ma nhom may thuc hien |
| Ten cong ty san xuat | MANUFACTURER_NAME | Nha san xuat |
| Ten nuoc san xuat | NATIONAL_NAME | Nuoc san xuat |
| Nam san xuat | MANUFACTURED_YEAR | Nam san xuat (0-9999) |
| Nam su dung | USED_YEAR | Nam bat dau su dung (0-9999) |
| So luu hanh | CIRCULATION_NUMBER | So luu hanh |
| So DV toi da/ngay | MAX_SERVICE_PER_DAY | So dich vu toi da trong ngay |

**Toolbar**: BarManager voi nut Save (phim tat Ctrl+S)

---

## 4. Luong xu ly chinh

### 4.1 Tai file mau (btnDownLoadFile_Click)

```
btnDownLoadFile_Click()
  → Doc file template: {AppFolder}/Tmp/Imp/IMPORT_MACHINE_CLS.xlsx
  → Mo SaveFileDialog de user chon noi luu
  → Copy file template den vi tri user chon
  → Hoi mo file ngay? → Mo file bang ung dung mac dinh
```

**File template**: `IMPORT_MACHINE_CLS.xlsx` dat tai `Tmp/Imp/` trong thu muc cai dat.

### 4.2 Chon file va doc Excel (btnChooseFile_Click)

```
btnChooseFile_Click()
  → Mo OpenFileDialog (.xlsx)
  → Hien WaitingManager
  → Inventec.Common.ExcelImport.Import.ReadFileExcel(fileName)
  → import.GetWithCheck<MachineImportADO>(0)    // Doc sheet dau tien
  → Loc bo dong rong (tat ca truong deu null/empty)
  → Goi API: api/HisMachine/Get                  // Lay toan bo may trong DB
  → addServiceToProcessList()                     // Validate tung dong
  → SetDataSource()                               // Hien thi len grid
  → CheckErrorLine()                              // Kiem tra co dong loi khong
```

### 4.3 Validate du lieu (addServiceToProcessList)

Duyet tung dong trong file import, kiem tra:

```
addServiceToProcessList(_CurrentAdos, ref _machineAdos)
  → Voi moi dong:
    1. Check trung trong file import (so sanh TAT CA cac truong)
    2. Check trung trong DB (chi so sanh MACHINE_CODE)
    3. Check may bi khoa (IS_ACTIVE != 1)
    4. Check do dai cac truong (maxlength)
    5. Check nam san xuat / nam su dung (0-9999)
    6. Check truong bat buoc (MACHINE_CODE)
  → Ghi nhan loi vao truong ERROR (noi bang " | ")
  → Gan ID thu tu cho moi dong
```

### 4.4 Loc dong loi (btnShowLineError_Click)

```
btnShowLineError_Click()
  → Toggle giua 3 trang thai:
    - "Dong loi": Hien thi chi dong co ERROR != null
    - "Dong khong loi": Hien thi chi dong co ERROR == null
  → Cap nhat grid tuong ung
  → checkButtonErrorLine: 0 = tat ca, 1 = dong loi, 2 = dong hop le
```

### 4.5 Xoa dong (btnDelete_ButtonClick)

```
btnDelete_ButtonClick()
  → Lay dong dang chon tren grid
  → Xoa khoi danh sach _machineAdos
  → Chay lai validate (addServiceToProcessList) tren danh sach con lai
  → Cap nhat grid theo trang thai loc hien tai
```

### 4.6 Luu du lieu (btnSave_Click)

```
btnSave_Click()
  → Kiem tra danh sach khong rong
  → Voi moi MachineImportADO:
    - Tao HIS_MACHINE entity
    - Map tat ca truong tu ADO sang entity
    - Chuyen doi ngay: ConvertDateToHisTime()
      + 8 ky tu (yyyyMMdd) → them "000000" → yyyyMMddHHmmss
      + 14 ky tu → giu nguyen
    - Kiem tra MACHINE_CODE da ton tai trong BackendDataWorker
      + Co: gan ID cu (cap nhat)
      + Chua co: tao moi
  → Goi API: api/HisMachine/CreateList (POST danh sach HIS_MACHINE)
  → Thanh cong:
    - Disable nut Save
    - Goi delegateRefresh() (neu co)
    - Hien thong bao thanh cong
  → That bai:
    - Hien thong bao loi
```

---

## 5. API Calls

| Endpoint | Method | Mo ta |
|----------|--------|-------|
| `api/HisMachine/Get` | GET | Lay toan bo danh sach may CLS tu DB, dung de check trung |
| `api/HisMachine/CreateList` | POST | Tao/cap nhat danh sach may CLS (truyen List\<HIS_MACHINE\>) |

**API Consumer**: `BackendAdapter` voi `ApiConsumers.MosConsumer`

**BackendDataWorker**: Dung `BackendDataWorker.Get<HIS_MACHINE>()` de truy xuat cache local kiem tra trang thai may (IS_ACTIVE) va lay ID may da ton tai.

---

## 6. Validation Rules

### Truong bat buoc

| Truong | Dieu kien |
|--------|-----------|
| MACHINE_CODE | Khong duoc rong |

### Do dai toi da

| Truong | Do dai toi da | Don vi |
|--------|---------------|--------|
| MACHINE_CODE | 100 | ky tu |
| MACHINE_NAME | 200 | ky tu |
| SERIAL_NUMBER | 200 | ky tu |
| SOURCE_CODE | 2 | ky tu |
| INTEGRATE_ADDRESS | 500 | bytes (UTF-8) |
| MACHINE_GROUP_CODE | 10 | ky tu |
| SYMBOL | 500 | bytes (UTF-8) |
| MANUFACTURER_NAME | 500 | bytes (UTF-8) |
| NATIONAL_NAME | 500 | bytes (UTF-8) |
| CIRCULATION_NUMBER | 22 | ky tu |

### Rang buoc khac

| Rule | Mo ta |
|------|-------|
| Trung trong file | So sanh TAT CA truong de phat hien dong trung lap |
| Trung trong DB | So sanh MACHINE_CODE voi DB (chi UK) |
| May bi khoa | MACHINE_CODE da ton tai nhung IS_ACTIVE != 1 → bao "da bi khoa" |
| Nam san xuat | MANUFACTURED_YEAR phai tu 0 den 9999 |
| Nam su dung | USED_YEAR phai tu 0 den 9999 |

### Xu ly loi

- Moi dong co the co NHIEU loi, noi voi nhau bang " | "
- Cot ERROR_ tren grid hien thi nut bam khi co loi
- Click nut → hien XtraMessageBox voi noi dung loi chi tiet
- **KHONG cho luu** khi con bat ky dong loi nao (btnSave.Enabled = false)

---

## 7. ADO Objects

### MachineImportADO

Ke thua `HIS_MACHINE` (MOS.EFMODEL.DataModels), bo sung:

| Property | Kieu | Mo ta |
|----------|------|-------|
| `ROOM_CODE` | string | Ma phong (phu) |
| `ROOM_TYPE_ID` | long | ID loai phong (phu) |
| `ERROR` | string | Chuoi loi validate, nhieu loi noi bang " \| " |
| `CONTRACT_FROM_DMY` | string (readonly) | CONTRACT_FROM dinh dang dd/MM/yyyy |
| `CONTRACT_TO_DMY` | string (readonly) | CONTRACT_TO dinh dang dd/MM/yyyy |
| `FROM_TIME_DMY` | string (readonly) | FROM_TIME dinh dang dd/MM/yyyy |
| `TO_TIME_DMY` | string (readonly) | TO_TIME dinh dang dd/MM/yyyy |

**Chuyen doi ngay**: Ham `ToDdMmYyyy(long? hisTime)` chuyen tu HIS time format (yyyyMMdd hoac yyyyMMddHHmmss) sang chuoi dd/MM/yyyy de hien thi tren grid.

### Cac truong HIS_MACHINE chinh duoc import

| Truong | Kieu | Mo ta |
|--------|------|-------|
| MACHINE_CODE | string | Ma may CLS (bat buoc, unique) |
| MACHINE_NAME | string | Ten may CLS |
| CONTRACT_FROM | long? | Ngay bat dau hop dong |
| CONTRACT_TO | long? | Ngay ket thuc hop dong |
| FROM_TIME | long? | Thoi gian bat dau su dung |
| TO_TIME | long? | Thoi gian ket thuc su dung |
| SERIAL_NUMBER | string | So serial |
| SOURCE_CODE | string | Ma nguon kinh phi |
| ROOM_IDS | string | Danh sach ID phong |
| INTEGRATE_ADDRESS | string | Dia chi tich hop |
| MAX_SERVICE_PER_DAY | long? | So dich vu toi da/ngay |
| MACHINE_GROUP_CODE | string | Ma nhom may |
| SYMBOL | string | Ky hieu |
| MANUFACTURER_NAME | string | Ten nha san xuat |
| NATIONAL_NAME | string | Ten nuoc san xuat |
| MANUFACTURED_YEAR | long? | Nam san xuat |
| USED_YEAR | long? | Nam su dung |
| CIRCULATION_NUMBER | string | So luu hanh |

---

## 8. Message Constants (MessageImport)

| Hang so | Gia tri | Mo ta |
|---------|---------|-------|
| `Maxlength` | "{0} vuot qua {1} ky tu" | Truong vuot do dai cho phep |
| `KhongHopLe` | "{0} khong hop le" | Du lieu khong hop le |
| `ThieuTruongDL` | "Thieu truong {0}" | Thieu truong bat buoc |
| `DaTonTai` | "{0} da ton tai trong danh sach may CLS" | Trung trong danh sach |
| `DaTonTaiLoaiMayCLS` | "{0} da ton tai trong loai may CLS" | Trung loai may |
| `TonTaiTrungNhauTrongFileImport` | "Ton tai may CLS co cung cac thong so trong file import" | Trung dong trong file |
| `CoThiPhaiNhap` | "Co {0} thi phai nhap {1}" | Rang buoc dieu kien |
| `MaMayCLSDaKhoa` | "Ma may CLS da bi khoa" | May da khoa |
| `MaLoaiMayCLSDaKhoa` | "Ma loai may CLS da bi khoa" | Loai may da khoa |
| `DBDaTonTai` | "Ma may CLS \"{0}\" da ton tai trong co so du lieu" | Trung trong DB |

---

## 9. Chuyen doi ngay thang

Ham `ConvertDateToHisTime(long? date)` xu ly chuyen doi ngay truoc khi luu:

| Input | Output | Giai thich |
|-------|--------|------------|
| `null` hoac `<= 0` | `null` | Khong co gia tri |
| `20250315` (8 ky tu) | `20250315000000` | Them "000000" (00:00:00) |
| `20250315143000` (14 ky tu) | `20250315143000` | Giu nguyen |
| Do dai khac | `null` | Khong hop le |

---

## 10. Dependencies

### Core Libraries

| Thu vien | Vai tro |
|----------|---------|
| `HIS.Desktop.Utility` | FormBase |
| `HIS.Desktop.ApiConsumer` | BackendAdapter, ApiConsumers.MosConsumer |
| `HIS.Desktop.Common` | BusinessBase, RefeshReference delegate |
| `HIS.Desktop.LocalStorage.BackendData` | BackendDataWorker (cache local) |
| `Inventec.Common.ExcelImport` | Doc file Excel (.xlsx) |
| `Inventec.Common.Mapper` | Map du lieu giua objects |
| `Inventec.Common.Adapter` | BackendAdapter |
| `Inventec.Common.Logging` | LogSystem.Error(), LogSystem.Warn() |
| `Inventec.Core` | CommonParam |
| `Inventec.Desktop.Common.Message` | MessageManager |
| `Inventec.Desktop.Common.Modules` | Module metadata |

### Data Models (tu lib/)

| Model | Mo ta |
|-------|-------|
| `MOS.EFMODEL.DataModels.HIS_MACHINE` | Entity may CLS |
| `MOS.Filter.HisMachineFilter` | Filter cho API Get |

### Third-party

| Thu vien | Vai tro |
|----------|---------|
| DevExpress | GridControl, LayoutControl, BarManager, XtraMessageBox, WaitingManager |

---

## 11. Luu y

- File template `IMPORT_MACHINE_CLS.xlsx` phai dat tai `{AppFolder}/Tmp/Imp/`
- Khi import, he thong check trung trong DB chi theo `MACHINE_CODE` (UK cua bang HIS_MACHINE)
- Khi luu, neu may da ton tai (cung MACHINE_CODE) se **cap nhat** (gan ID cu), khong tao trung
- Nut Save **chi duoc bat** khi khong con dong loi nao
- Sau khi luu thanh cong, nut Save bi **disable** de tranh luu trung
- `delegateRefresh` duoc goi sau khi luu thanh cong de form cha cap nhat danh sach
