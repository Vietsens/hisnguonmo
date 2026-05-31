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

## 8. Changelog

| Ngay | Nguoi sua | Mo ta thay doi |
|------|-----------|-----------------|
| 16/04/2026 | phuongnm | Fix default filter fallback: khi GP4 loai bo "Tat ca", mac dinh chuyen sang "Toi tao" (ID=0) thay vi "Khoa chi dinh" (ID=2) |
| 22/04/2026 | tuanln | Them cot "Thu ky" (SECRETARY_USERNAME) canh cot "Nguoi thuc hien" trong grid danh sach y lenh — bound column, chi doc, hien thi ten day du thu ky (trong neu khong co). VisibleIndex cac cot phia sau da duoc day len 1. Resources da cap nhat cho 3 ngon ngu vi/en/my. |
| 22/05/2026 | dangth2 | Viec 44693 (Tai lieu 2671): Bo sung dieu kien enable nut "Xoa y lenh" trong `frmServiceReqList.cs:gridViewServiceReq_CustomRowCellEdit` — neu loai y lenh la Giuong VA tai khoan co quyen HIS000053 thi enable. Cac truong hop khac giu nguyen. Them `Base/ControlCode.cs`, field `hasDeleteBedPermission`, method `LoadDeleteBedPermission()`. Reference `ACS.EFMODEL.dll`. |
