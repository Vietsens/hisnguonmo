# HIS.Desktop.Plugins.ServiceReqList — Tai Lieu Module

## 1. Tong Quan

| Thong tin | Gia tri |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ServiceReqList |
| Loai | Form |
| Muc dich | Danh sach y lenh cua benh nhan dang dieu tri. Loc theo nguoi tao, phong chi dinh, khoa chi dinh, khoa thuc hien, tat ca. |
| Trang thai | Bao tri |

## 2. Quy Trinh Nghiep Vu

### Luong chinh
1. Mo form tu man hinh dieu tri benh nhan
2. Chon bo loc phan loai (Toi tao / Phong chi dinh / Khoa chi dinh / Khoa thuc hien / Tat ca)
3. Hien thi danh sach y lenh theo bo loc
4. Thao tac: xem, sua, xoa, in y lenh

### Cau hinh anh huong
- **MOS.HIS_TREATMENT.RESTRICT_SEARCH_OTHER_DEPARTMENT (GP4)**: Khi bat (=1) va user khong phai admin (IS_ADMIN != 1), an lua chon "Tat ca" khoi dropdown, mac dinh chuyen sang "Toi tao"
- **Filter_Type_For_Treatment_Patient**: Cau hinh gia tri mac dinh cua dropdown bo loc

## 4. UI Layout

### Grid danh sach y lenh
Cac cot hien thi chinh (VisibleIndex): STT, Ma y lenh, ..., Loai y lenh (18), **Nguoi yeu cau (18)**, **Nguoi thuc hien (19)**, **Thu ky (20)**, Bua an (21), Nam sinh (22), Gui HT cu (23), Thoi gian tao (24), Nguoi tao (25), Thoi gian sua (26), Nguoi sua (27).

Cot **Thu ky** hien thi truc tiep `SECRETARY_USERNAME` tu V_HIS_SERVICE_REQ — Bound column, chi doc, khong cho sua tren luoi, de trong neu y lenh khong co thu ky.

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lay danh sach y lenh | api/HisServiceReq/GetView | MosConsumer | HisServiceReqViewFilter (da co SECRETARY_USERNAME sau khi cap nhat view) |

## 6. Dependencies

### ACS — Phan quyen
| Ma control | Ten hien thi | Muc dich |
|-----------|-------------|----------|
| HIS000053 | Xoa y lenh giuong | Cho phep xoa y lenh giuong cua tai khoan khac khi loai y lenh la Giuong (`SERVICE_REQ_TYPE.ID__G`) |

Plugin reference `ACS.EFMODEL.dll`. Load quyen qua `GlobalVariables.AcsAuthorizeSDO.ControlInRoles` luu vao co `hasDeleteBedPermission`.

### Dieu kien enable nut "Xoa y lenh" tren grid
Voi `SERVICE_REQ_STT_ID == CXL`, enable khi:
- `accountCanDelete`: loginName la nguoi tao / nguoi chi dinh / admin, **HOAC**
- `bedCanDelete`: loai y lenh la **Giuong (G)** VA tai khoan co quyen **HIS000053**, **HOAC**
- Loai la **Kham (KH)** VA cung khoa chi dinh VA cung phong (yeu cau hoac thuc hien)

## 7. Print

### Nut "In KQ tong hop XN" (dropdown btnDropDownPrint)
Menu item thu 4 trong `GeneratePopupMenu()`. Handler `OnClickInKQTongHopXN` (file `frmServiceReqList__Plus__InKQTongHopXN.cs`).

| Loai in | PrintTypeCode | Co che | PDO |
|---------|--------------|--------|-----|
| KQ tong hop xet nghiem | Mps000517 | RichEditorStore.RunPrintTemplate -> DelegateRunPrinterMps000517 -> MpsPrinter.Run | MPS.Processor.Mps000517.PDO.Mps000517PDO(patientTypeAlter, treatment, List\<V_LIS_SAMPLE\>, List\<HIS_SERVICE_REQ\>, List\<V_HIS_TEST_INDEX\>, List\<V_LIS_RESULT\>, List\<V_HIS_TEST_INDEX_RANGE\>, genderId, List\<V_HIS_SERVICE\>, serviceParent) |

Du lieu build PDO (callback `DelegateRunPrinterMps000517`):
- `treatment`: api/HisTreatment/Get theo TREATMENT_ID. `patientTypeAlter`: api/HisPatientTypeAlter/GetLastByTreatmentId.
- `currentServiceReqs`: map tu cac ServiceReqADO da chon (Mapper sang HIS_SERVICE_REQ).
- `currentSamples`: api/LisSample/GetView (LisConsumer) theo tung SERVICE_REQ_CODE__EXACT.
- `lisResults`: api/LisResult/GetView (LisConsumer) theo tung SAMPLE_ID.
- `testIndexs` / `testIndexRanges` / `listService`: BackendDataWorker (cache RAM), loc theo SERVICE_CODE cua ket qua + dich vu cha.
- `genderId`: TDL_PATIENT_GENDER_ID cua y lenh. `serviceParent`: null (in da mau, processor tu gom theo mau).
- PreviewType theo `ConfigApplications.CheDoInChoCacChucNangTrongPhanMem` (==2: PrintNow, khac: Show + EMR InputADO).

Quy trinh kiem tra truoc khi in (4 buoc):
1. Phai co y lenh **xet nghiem** (`HIS_SERVICE_REQ_TYPE.ID__XN`) duoc tich chon — neu khong -> canh bao `ChuaChonYLenhXetNghiem`.
2. Cac y lenh da chon phai cung 1 benh nhan (`TDL_PATIENT_ID` distinct <= 1) — kiem tra tren object trong bo nho, khong goi API. Neu khac -> canh bao `CacYLenhKhongCungBenhNhan`.
3. Goi `api/LisSample/GetView` (LisConsumer) loc theo tung `SERVICE_REQ_CODE__EXACT`; neu con mau co `RESULT_TIME == null` -> canh bao `CoXetNghiemChuaCoKetQua` (liet ke ma y lenh + phong thuc hien).
4. Dat tat ca -> goi bieu in Mps000517.

## 8. Changelog

| Ngay | Nguoi sua | Mo ta thay doi |
|------|-----------|-----------------|
| 11/08/2026 | nampp | **Bo sung config gate**: them key `MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY_COLUMN` vao `HisConfigCFG.cs` (+ `IsEmergencyClassifyColumnEnabled`). `= 1` chay cach hien thi moi (nhan mau tren tieu de nhom, khong to mau chu toan luoi); khac `1`/khong khai bao thi GIU NGUYEN Y HET code cu (mau cap cuu to ForeColor toan luoi va uu tien hon mau cam don thuoc tam, khong doi caption nhom). `ApplyEmergencyClassifyBadge` return ngay truoc moi lenh ghi khi key tat; clamp mau 0-255 chi ap dung khi key bat. |
| 11/08/2026 | nampp | PTTK phan loai cap cuu: GO nhanh to ForeColor TOAN LUOI theo muc phan loai cap cuu trong `gridViewServiceReq_RowCellStyle` -> don thuoc tam (`IS_TEMPORARY_PRES`) hien thi lai mau cam. Muc phan loai chuyen thanh NHAN CO MAU tren tieu de nhom "Thong tin chung" (`ApplyEmergencyClassifyBadge`: AppearanceCaption BackColor/BackColor2 = mau muc, ForeColor tuong phan, in dam; khoi phuc caption goc khi BN khong co muc) — form chi co 1 benh nhan nen KHONG dung cot rieng (moi dong se lap cung gia tri). `InitEmergencyClassifyColor` lay them `PATIENT_CLASSIFY_NAME` + clamp mau 0-255. |
| 16/04/2026 | phuongnm | Fix default filter fallback: khi GP4 loai bo "Tat ca", mac dinh chuyen sang "Toi tao" (ID=0) thay vi "Khoa chi dinh" (ID=2) |
| 22/04/2026 | tuanln | Them cot "Thu ky" (SECRETARY_USERNAME) canh cot "Nguoi thuc hien" trong grid danh sach y lenh — bound column, chi doc, hien thi ten day du thu ky (trong neu khong co). VisibleIndex cac cot phia sau da duoc day len 1. Resources da cap nhat cho 3 ngon ngu vi/en/my. |
| 22/05/2026 | dangth2 | Viec 44693 (Tai lieu 2671): Bo sung dieu kien enable nut "Xoa y lenh" trong `frmServiceReqList.cs:gridViewServiceReq_CustomRowCellEdit` — neu loai y lenh la Giuong VA tai khoan co quyen HIS000053 thi enable. Cac truong hop khac giu nguyen. Them `Base/ControlCode.cs`, field `hasDeleteBedPermission`, method `LoadDeleteBedPermission()`. Reference `ACS.EFMODEL.dll`. |
| 24/06/2026 | huannh | B.4.2: Them nut "In KQ tong hop XN" vao dropdown `btnDropDownPrint` (`GeneratePopupMenu`). Them partial `frmServiceReqList__Plus__InKQTongHopXN.cs` voi 4 buoc kiem tra (chon y lenh XN, cung benh nhan, kiem tra V_LIS_SAMPLE.RESULT_TIME qua `api/LisSample/GetView`) truoc khi goi bieu in Mps000517. Them reference LIS.EFMODEL, LIS.Filter; URI `LIS_SAMPLE_GETVIEW`; 3 message (vi/en/my). |
| 30/06/2026 | huannh | Hoan thien in Mps000517: build day du Mps000517PDO trong `DelegateRunPrinterMps000517` (load treatment, patientTypeAlter, V_LIS_SAMPLE, V_LIS_RESULT, test index/range/service) va goi `MpsPrinter.Run` (PreviewType theo cau hinh). Them reference `MPS.Processor.Mps000517.PDO`. |
| 01/07/2026 | huannh | YC4: Them cot "So Serial" (`gridColumnSerialNumber`, FieldName `SERIAL_NUMBER`) vao cuoi grid chi tiet thuoc/vat tu `grdViewSereServServiceReq`, chi hien thi khi xem y lenh loai Don dieu tri (`ID__DONDT`) hoac Don tu truc (`ID__DONTT`). Them property `SERIAL_NUMBER` vao `ADO/ListMedicineADO.cs`. Bo sung `SERIAL_NUMBER` vao GroupBy khi gom vat tu trong `FillDataGridDetail` (tu `api/HisExpMestMaterial/Get`) → moi dong vat tu ung dung 1 serial, khong gop sai. Toggle `gridColumnSerialNumber.Visible` theo loai y lenh truoc khi bind (an cho loai OT). Resources cap nhat 3 ngon ngu vi/en/my. |
