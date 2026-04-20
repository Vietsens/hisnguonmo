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

## 8. Changelog

| Ngay | Nguoi sua | Mo ta thay doi |
|------|-----------|-----------------|
| 16/04/2026 | phuongnm | Fix default filter fallback: khi GP4 loai bo "Tat ca", mac dinh chuyen sang "Toi tao" (ID=0) thay vi "Khoa chi dinh" (ID=2) |
