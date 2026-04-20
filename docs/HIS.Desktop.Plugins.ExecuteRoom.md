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

### ADO
- ServiceReqADO (extends L_HIS_SERVICE_REQ): Them cac truong hien thi (DOB_DISPLAY, status, DISPLAY_COLOR...)

## 4. UI Layout

### Grid chinh: gridViewServiceReq
- Hien thi danh sach yeu cau dich vu
- Cac cot: Trang thai, Goi BN, Gui, Sua, Ho ten, Ma BN, Ma dieu tri, Ngay sinh, Gioi tinh...
- Cot gcTreatmentHistory: Icon mat xem lich su dieu tri (mac dinh an)

### Grid phu: gridViewSereServServiceReq
- Hien thi chi tiet dich vu cua yeu cau dang chon

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
| 17/04/2026 | phuongnm | Them icon mat xem lich su dieu tri (gcTreatmentHistory) - goi HIS.Desktop.Plugins.TreatmentHistory voi patient_code, patient_id. Mac dinh an, bac si tu dua ra ngoai giao dien. |
| 18/04/2026 | phuongnm | Fix HisConfigCFG.isRestoreLayout split bang ca ',' va ';' (config tester dung ';' phan tach). Note: source code ExecuteRoom da co san goi InitRestoreLayoutGridViewFromXml(gridViewServiceReq) tai dau FillDataToGridServiceReq va InitRestoreLayoutGridViewFromXml(gridViewSereServServiceReq) tai dau LoadSereServServiceReq → trung pattern voi DLL histest, base class UserControlBase tu xu ly check config + hook events + restore/save. |

## 9. Test Cases

### Xem lich su dieu tri
- [ ] Hien cot gcTreatmentHistory (keo cot tu Column Chooser)
- [ ] Click icon mat -> Mo man hinh TreatmentHistory voi dung ma benh nhan
- [ ] Neu khong co yeu cau nao duoc chon -> Khong lam gi
- [ ] Grid luu trang thai cot khi cau hinh RestoreLayout
