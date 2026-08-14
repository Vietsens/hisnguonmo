# HIS.Desktop.Plugins.ExecuteRoom

## 1. Tong Quan

| Thong tin | Gia tri |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.ExecuteRoom |
| Loai | UserControl |
| Muc dich | Phong xu ly - Noi bac si xu ly yeu cau kham benh, chi dinh dich vu, thuc hien dich vu CLS |
| Trang thai | Bao tri |

## 2. Quy Trinh Nghiep Vu

### Luong chinh
1. Bac si dang nhap phong xu ly
2. Danh sach yeu cau dich vu (ServiceReq) hien thi tren grid
3. Bac si chon yeu cau -> Thuc hien xu ly (Execute)
4. Sau xu ly -> Cap nhat trang thai yeu cau

### Chuc nang bo sung
- Goi benh nhan (CallPatient)
- Chuyen phong (RoomTran)
- Xem vien phi (Bordereau)
- Xem lich su dieu tri (TreatmentHistory) - thong qua icon mat

## 3. EFMODEL Su Dung

| Entity | Loai | Muc dich |
|--------|------|----------|
| L_HIS_SERVICE_REQ | View | Yeu cau dich vu - data chinh cua grid |
| V_HIS_EXECUTE_ROOM | View | Thong tin phong xu ly |
| V_HIS_SERE_SERV_6 | View | Dich vu da thuc hien |
| V_HIS_PATIENT_TYPE_ALTER | View | Thong tin BHYT |
| V_HIS_ALLERGY_CARD | View | The di ung cua benh nhan (loc theo PATIENT_ID - the di ung di theo benh nhan) |

### ADO
- ServiceReqADO (extends L_HIS_SERVICE_REQ): Them cac truong hien thi (DOB_DISPLAY, status, DISPLAY_COLOR...)

## 4. UI Layout

### Grid chinh: gridViewServiceReq
- Hien thi danh sach yeu cau dich vu
- Cac cot: Trang thai, Goi BN, Gui, Sua, Ho ten, Ma BN, Ma dieu tri, Ngay sinh, Gioi tinh...
- Cot gcTreatmentHistory: Icon mat xem lich su dieu tri (mac dinh an)

### Grid phu: gridViewSereServServiceReq
- Hien thi chi tiet dich vu cua yeu cau dang chon

### Vung "Thong tin benh nhan"
- lblPatientCode (Ma benh nhan): hien icon "vien thuoc" (Resources\thuoc.png) ben phai khi benh nhan co the di ung. Tooltip "Benh nhan co the di ung". The di ung di theo BENH NHAN -> loc HIS_ALLERGY_CARD theo PATIENT_ID (tai lieu 2112).
- Click vao lblPatientCode khi dang hien icon -> mo man The di ung (HIS.Desktop.Plugins.AllergyCard) qua AllergyCardClick(currentHisServiceReq) de xem/sua/xoa. Con tro chuyen Hand khi co icon.

## 5. API Endpoints

Tham khao HisRequestUriStore.cs

## 6. Dependencies

### Inter-Plugin

| Plugin dich | Khi nao mo | Args truyen |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.TreatmentHistory | Click icon mat trong grid | TreatmentHistoryADO (patientId, patient_code) |
| HIS.Desktop.Plugins.AssignPrescriptionCLS | Ke don CLS | AssignPrescriptionADO |
| Cac plugin Execute (Exam, Test, DIIM...) | Double-click yeu cau | ServiceReq data |

## 7. Print

Tham khao UCExecuteRoom___Print.cs

## 8. Changelog

| Ngay | Nguoi sua | Mo ta thay doi |
|------|-----------|-----------------|
| 11/08/2026 | nampp | **Moi hoi chan tai phong kham cap cuu — va loi menu.** (1) `DebateClick` (menu "Bien ban hoi chan") thieu `return` sau khi khong tim thay ModuleLink `HIS.Desktop.Plugins.Debate` -> NullReferenceException ngay dong duoi, exception bi nuot trong catch nen bam menu KHONG phan hoi gi. Them `return` + thong bao nhu man buong benh. (2) `MoiHoiChanClick` truoc day thoat im lang khi thieu module -> them thong bao + tach nhanh `!IsPlugin || ExtensionInfo == null` co ghi log rieng. (3) Chong mat menu chuot phai: tach nhom menu EMR trong `InitMenu` ra ham rieng `InitEmrMenuGroup`, null-check `HIS_TREATMENT` tra ve tu API va `WorkPlaceSDO.FirstOrDefault(...)` (truoc day deref thang, exception bi nuot o catch cuoi ham khien `menu.ShowPopup` khong chay). Them message `Plugin_ExecuteRoom__ChucNangDangPhatTrienLienHeQuanTri` (vi + en) va bo sung ban en con thieu cua `Plugin_ExecuteRoom__DichVuDaDuocKeVoiYLenh`. Build OK va deploy test 11/08 (kem satellite resources vi/en/my). Ghi chu moi truong build: `RowCellStyleEventArgs` cua DevExpress 15.2.9 KHONG co property `HighPriority` (chi co Appearance/Column/RowHandle/CellValue) — tung chan build o viec phan loai cap cuu, da duoc go. |
| 11/08/2026 | nampp | **Bo sung config gate cho viec cot trang thai cap cuu**: them key `MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY_COLUMN` (`Config\HisConfigCFG.cs` const + `IsEmergencyClassifyColumnEnabled` + LoadConfig). `= 1` chay cach hien thi moi; khac `1`/khong khai bao thi GIU NGUYEN Y HET code cu (mau cap cuu van to ForeColor ca dong va uu tien de) — cac vien khac cap nhat ban moi khong bi anh huong. Chi tiet: `RowStyle` khi key bat thi bo qua buoc lay mau cap cuu (bien mau = null) nen chuoi if/else if cu chay nguyen; phan clamp 0-255 + try/catch tung ban ghi trong `LoadEmergencyClassifyColorDict` CHI ap dung khi key bat (key tat dung `Color.FromArgb` tho nhu goc, exception nem ra catch ngoai nhu cu); vong gan 2 property ADO cung chi chay khi key bat. Sua them: chuyen `EnsureEmergencyClassifyColumn()` len ngay sau `InitRestoreLayoutGridViewFromXml` va RA NGOAI `if (rowCount > 0)` (`UCExecuteRoom___Load.cs`) — truoc do khi luoi 0 dong thi cot da restore tu file layout khong ai go, header "Muc CC" con sot du key da tat. |
| 11/08/2026 | nampp | PTTK phan loai cap cuu: TACH mau muc phan loai ra COT TRANG THAI rieng "Muc CC" (FieldName `EMERGENCY_CLASSIFY_NAME`, bound, W=70, dat ngay sau cot "#"). GO nhanh to ForeColor ca dong theo muc cap cuu trong `gridViewServiceReq_RowStyle` -> mau BHYT (Blue) / KSK (Green) / DISPLAY_COLOR hoat dong tro lai, het canh de mau. Them `EnsureEmergencyClassifyColumn()` tao/GO cot luc runtime (BAT BUOC go khi khong phai phong cap cuu vi file layout luoi `ModuleDesign\...\gridViewServiceReq.xml` dung chung cho moi phong), `gridViewServiceReq_RowCellStyle` to nen dung 1 o + `GetContrastForeColor` (chu den/trang theo do sang nen), 2 property pre-compute tren `ServiceReqADO` (`EMERGENCY_CLASSIFY_NAME`, `EMERGENCY_CLASSIFY_COLOR`), them `emergencyClassifyNameDict`. Caption/tooltip set NGAY trong Ensure... (InitLanguage chay truoc khi luoi fill). Fix loi: clamp 0-255 + try/catch tung ban ghi trong `LoadEmergencyClassifyColorDict` — truoc day 1 ban ghi danh muc mau sai lam MAT MAU CA TRANG luoi. Cot hien o MOI PHONG khi bat du 2 key config — **khong** gioi han `IS_EMERGENCY = 1` (chot 12/08): cach to mau cu khong he xet phong, neu gioi han cot theo phong cap cuu thi BN da phan loai chuyen sang phong kham thuong se mat han dau hieu muc uu tien. Resources vi/en/my. |
| 17/04/2026 | phuongnm | Them icon mat xem lich su dieu tri (gcTreatmentHistory) - goi HIS.Desktop.Plugins.TreatmentHistory voi patient_code, patient_id. Mac dinh an, bac si tu dua ra ngoai giao dien. |
| 18/04/2026 | phuongnm | Fix HisConfigCFG.isRestoreLayout split bang ca ',' va ';' (config tester dung ';' phan tach). Note: source code ExecuteRoom da co san goi InitRestoreLayoutGridViewFromXml(gridViewServiceReq) tai dau FillDataToGridServiceReq va InitRestoreLayoutGridViewFromXml(gridViewSereServServiceReq) tai dau LoadSereServServiceReq → trung pattern voi DLL histest, base class UserControlBase tu xu ly check config + hook events + restore/save. |
| 22/04/2026 | phuongnm | GP5 — PTTK_19083: To mau y lenh theo phan loai cap cuu. Config-gated boi MOS.HIS_TREATMENT.EMERGENCY_CLASSIFY. Batch load HIS_TREATMENT theo TREATMENT_IDs, cache Dictionary<long, Color?> emergencyClassifyColorDict. Chi ap dung mau khi EMERGENCY_CLASSIFY_ID_1 != NULL va EMERGENCY_CLASSIFY_ID_2 = NULL. Uu tien hon DISPLAY_COLOR / Blue (BHYT) / Green (KSK) trong gridViewServiceReq_RowStyle. |
| 18/06/2026 | dangth2 | Bo sung icon "vien thuoc" canh lblPatientCode (Ma benh nhan) trong vung Thong tin benh nhan. Chi hien khi benh nhan co the di ung, tooltip "Benh nhan co the di ung". Tham khao pattern tu HIS.Desktop.Plugins.BedRoomPartial. Method SetAllergyCardIcon(long patientId) trong UCExecuteRoom___Load.cs, goi sau khi gan ma BN trong LoadPatientFromServiceReq (truyen 0 de an icon). Icon thuoc.png da dang ky vao Properties\Resources. |
| 18/06/2026 | dangth2 | (Tai lieu 2112) Sua the di ung lay theo BENH NHAN thay vi ho so: SetAllergyCardIcon loc HIS_ALLERGY_CARD theo PATIENT_ID (truoc do TREATMENT_ID nen icon khong len khi the o ho so khac). Bo sung click lblPatientCode -> mo man The di ung (AllergyCardClick) de sua/xoa thong tin di ung da tao. Con tro Hand khi co icon. |
| 08/05/2026 | phuongnm | Them muc "Moi hoi chan" vao menu chuot phai danh sach BN ngoai tru. Truoc khi mo plugin HIS.Desktop.Plugins.InviteConsultation, kiem tra V_HIS_SPECIALIST_EXAM (IS_ACTIVE=1, IS_DELETE!=1, INVITE_TYPE=2) cho TREATMENT_ID hien tai - neu co phieu IS_APPROVAL!=1 thi canh bao "Benh nhan dang co phieu hoi chan chua hoan tat voi khoa XXX. Ban co muon tao them phieu moi khong?" (Yes -> mo, No -> dung). Truyen args: Module, isEdit=false, V_HIS_SERVICE_REQ (lay qua api/HisServiceReq/getView). |
| 26/06/2026 | tuanln | Sua muc huong BHYT: GetDefaultHeinRatio nhan them 3 tham so FACILITY_CLASS, FORMER_LEVEL_CODE, CLASSIFY_POINT lay theo THE BHYT cua benh nhan (V_HIS_PATIENT_TYPE_ALTER) thay vi cau hinh co so (HIS_BRANCH). Sua tai UCExecuteRoom___Load.cs (hien thi muc huong tren lblCardNumber) va UCExecuteRoom___Process.cs (ratio_text khi in). Bo bien bRANCH khong con dung. |
| 21/07/2026 | tuanln | Tai lieu 43719 - Giu ket noi camera khi chuyen benh nhan (config-gated, mac dinh TAT). Doc key HIS.Desktop.Plugins.ServiceExecute.IsKeepCameraConnectionOnSwitchPatient trong HisConfigCFG.LoadConfig. Nhanh mo ServiceExecute (MODULE_LINK__CDHA_TDCN_NS_SA_GPBL) trong LoadModuleExecuteService: khi config bat, goi TryReloadOpenServiceExecute(serviceReqDynamic) - tim tab dang host instance ServiceExecute con song (page.Controls[0] == openServiceExecuteInstance), reflect goi ReloadByServiceReq de nap BN moi vao cung man (giu camera), kich hoat lai tab; neu chua co/tab da dong thi mo tab moi nhu cu va luu openServiceExecuteInstance. Config TAT = giu nguyen hanh vi cu. |

## 9. Test Cases

### Xem lich su dieu tri
- [ ] Hien cot gcTreatmentHistory (keo cot tu Column Chooser)
- [ ] Click icon mat -> Mo man hinh TreatmentHistory voi dung ma benh nhan
- [ ] Neu khong co yeu cau nao duoc chon -> Khong lam gi
- [ ] Grid luu trang thai cot khi cau hinh RestoreLayout
