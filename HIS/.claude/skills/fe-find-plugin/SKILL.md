---
name: fe-find-plugin
description: Tìm plugin Frontend trong 992 plugins theo tên chức năng tiếng Việt hoặc tên entity. Sử dụng khi cần tìm plugin UI liên quan đến một chức năng y tế.
user-invocable: true
argument-hint: <từ khóa tìm kiếm - VD: kê đơn, viện phí, HisExpMest>
---

# Tìm Plugin Frontend HIS

## Bước 1: Đọc Plugin Index

Đọc file `FRONTEND/hisnguonmo/HIS/Plugins/CLAUDE.md` — đây là index 992 plugins phân theo 20 nhóm chức năng y tế.

## Bước 2: Tìm theo từ khóa

Từ `$ARGUMENTS`, xác định nhóm chức năng phù hợp:

| Từ khóa | Nhóm | Section |
|---------|------|---------|
| kê đơn, thuốc, dược, medicine, prescription | Dược & Kê đơn | Section 2 |
| khám, điều trị, treatment, exam | Khám bệnh & Điều trị | Section 1 |
| dịch vụ, chỉ định, service, assign | Dịch vụ y tế | Section 3 |
| kho, xuất, nhập, vật tư, ExpMest, ImpMest | Vật tư & Kho | Section 4 |
| viện phí, thanh toán, hóa đơn, transaction, bill | Viện phí & Tài chính | Section 5 |
| bệnh nhân, đăng ký, patient, register | Bệnh nhân & Hồ sơ | Section 6 |
| máu, truyền máu, blood | Máu & Truyền máu | Section 7 |
| giường, phòng, khoa, bed, room, department | Giường & Phòng | Section 8 |
| báo cáo, thống kê, report | Báo cáo | Section 9 |
| BHYT, bảo hiểm, hein, xml | BHYT & Bảo hiểm | Section 10 |
| phẫu thuật, thủ thuật, PTTT, surg | PTTT | Section 11 |
| chuyển viện, chuyển khoa, tranpati | Chuyển viện | Section 12 |
| xét nghiệm, mẫu, test, lis, sample | Xét nghiệm | Section 13 |
| dinh dưỡng, ration, nutrition | Dinh dưỡng | Section 14 |
| import, export, tích hợp | Tích hợp | Section 15 |
| emr, bệnh án, ký số | EMR Plugins | Section 16 |
| acs, phân quyền, role | ACS Plugins | Section 17 |

## Bước 3: Liệt kê kết quả

Trả về:
1. **Nhóm chức năng** phù hợp
2. **Danh sách plugins** liên quan (tên đầy đủ)
3. **Đường dẫn folder**: `FRONTEND/hisnguonmo/HIS/Plugins/{PluginName}/`
4. **Gợi ý plugin chính** cần mở trước

Nếu từ khóa là tên entity (VD: HisExpMest), tìm cả trong danh sách plugin lẫn Plugins/CLAUDE.md.

## Bước 4: Kiểm tra folder tồn tại

Verify rằng folder plugin thực sự tồn tại trên filesystem trước khi gợi ý.

## Input từ user

$ARGUMENTS
