# HIS.Desktop.Plugins.BloodList — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BloodList |
| Loại | UC + Form (UCBloodList + frmBloodUpdate) |
| Mục đích | Hiển thị danh sách túi máu, cho phép tìm kiếm, lọc theo loại máu/ngày nhập, mở form sửa thông tin lô máu (HIS_BLOOD) và người hiến (HIS_BLOOD_GIVER). |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng mở danh sách túi máu (UCBloodList).
2. Lọc theo từ khóa, loại máu, khoảng ngày nhập.
3. Click icon "Sửa" trên grid → mở form `frmBloodUpdate` với lô máu được chọn.
4. Form tự động điền dữ liệu lô máu hiện tại (mã vạch, loại máu, Abo/Rh, giá nhập, ..., **TRANSFER_MEDI_ORG_CODE — CSKCB chuyển**).
5. Người dùng chỉnh sửa thông tin → Lưu → gọi `api/HisBlood/Update`.
6. Nếu lô máu có liên kết người hiến (BLOOD_GIVE_ID > 0) → cập nhật cả `HIS_BLOOD_GIVER` qua `api/HisBloodGiver/Update`.

### Điều kiện nghiệp vụ
- Trường "CSKCB chuyển" (TRANSFER_MEDI_ORG_CODE) tối đa 10 ký tự — validate realtime hiển thị icon cảnh báo, chặn lưu khi vượt.
- Người dùng có thể nhập trực tiếp mã CSKCB hoặc bấm nút "+" để mở form chọn từ danh mục `HIS_MEDI_ORG` (qua `HIS.UC.MediOrgPicker`).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_BLOOD | View | Hiển thị danh sách + load form sửa |
| HIS_BLOOD | Table | Update lô máu (bao gồm TRANSFER_MEDI_ORG_CODE) |
| HIS_BLOOD_GIVER | Table | Update người hiến |
| HIS_BLOOD_TYPE / HIS_BLOOD_ABO / HIS_BLOOD_RH | Lookup | Loại máu, nhóm máu |
| HIS_IMP_SOURCE / HIS_SUPPLIER | Lookup | Nguồn nhập, nhà cung cấp |
| HIS_BLOOD_VOLUME / HIS_GENDER / HIS_CAREER / HIS_WORK_PLACE | Lookup | Thể tích, giới tính, nghề nghiệp, đơn vị công tác |
| SDA_NATIONAL / SDA_PROVINCE / SDA_DISTRICT / SDA_COMMUNE | Lookup | Quốc gia, địa giới |
| HIS_MEDI_ORG (qua MediOrgADO/MediOrgDataWorker) | Lookup | Danh mục cơ sở KCB cho picker CSKCB chuyển |

## 4. UI Layout

### Sơ đồ giao diện frmBloodUpdate (Sửa máu)

```
+--------------------------------------------------------------------------+
| Mã vạch          | Loại máu      | Abo                | Rh               |
| Giá nhập         | VAT nhập (%)  | Giá nội bộ         | Số lô            |
| Nhà cung cấp                                                              |
| Nguồn nhập       | Thời gian đg  | Hạn sử dụng        |                   |
| Mã người cho     | Tên người cho                      | CSKCB chuyển: + |  ← MỚI
| Nhiễm bệnh                                            | [Lưu (Ctrl S)]   |
| (lcgBloodGiver — nhóm thông tin người hiến — nếu có)                     |
+--------------------------------------------------------------------------+
```

### UC sử dụng
| UC | Khi nào | Mục đích |
|----|---------|----------|
| HIS.UC.MediOrgPicker | Click nút "+" cạnh "CSKCB chuyển" | Mở modal chọn cơ sở KCB chuyển tuyến, trả về chuỗi `C.<MEDI_ORG_CODE>` |

## 5. API Endpoints

| Action | URI | Consumer | DTO/Filter |
|--------|-----|----------|------------|
| Get blood detail | api/HisBlood/Get | MosConsumer | HisBloodFilter |
| Update blood | api/HisBlood/Update | MosConsumer | HIS_BLOOD (đã set TRANSFER_MEDI_ORG_CODE) |
| Get blood giver | api/HisBloodGiver/Get | MosConsumer | HisBloodGiverFilter |
| Update blood giver | api/HisBloodGiver/Update | MosConsumer | HIS_BLOOD_GIVER |

## 6. Dependencies

### Library / UC
| Library | Mục đích |
|---------|----------|
| HIS.UC.MediOrgPicker | Form modal "Tìm chọn CSKCB" — `MediOrgPickerProcessor.Pick(initialValue) → "C.<MEDI_ORG_CODE>"` |
| HIS.Desktop.LocalStorage.BackendData | `MediOrgDataWorker.MediOrgADOs` (RAM cache `HIS_MEDI_ORG` lọc IS_ACTIVE=1, IS_DELETE!=1) |

### Inter-Plugin
Plugin này không mở plugin khác. Được mở từ menu chính.

## 7. Print

Không có chức năng in trực tiếp trong plugin.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 09/05/2026 | dangth | **Mục 2.7 PTTK 2562 (việc 43980)** — Thêm trường "CSKCB chuyển" (TRANSFER_MEDI_ORG_CODE) vào form `frmBloodUpdate`. ButtonEdit có nút "+" mở `HIS.UC.MediOrgPicker`. Auto-fill khi load lô máu. Validate realtime + chặn lưu khi vượt 10 ký tự. Lưu vào `HIS_BLOOD.TRANSFER_MEDI_ORG_CODE` qua `api/HisBlood/Update`. Thêm Resources đa ngôn ngữ vi/en. |

## 9. Test Cases

### CSKCB chuyển — load
- [ ] Mở form sửa lô máu có TRANSFER_MEDI_ORG_CODE = "C.01234" → control hiển thị "C.01234"
- [ ] Mở form sửa lô máu có TRANSFER_MEDI_ORG_CODE = null → control trống

### CSKCB chuyển — nhập trực tiếp
- [ ] Gõ "C.99999" → không có icon cảnh báo
- [ ] Gõ "C.999999999999" (vượt 10 ký tự) → hiện icon cảnh báo
- [ ] Vượt 10 ký tự → bấm Lưu → hiện thông báo "Mã CSKCB chuyển tối đa 10 ký tự" → focus về control → KHÔNG gọi API

### CSKCB chuyển — chọn từ picker
- [ ] Click nút "+" → mở form `frmMediOrgPicker`
- [ ] Form hiện danh sách HIS_MEDI_ORG (IS_ACTIVE=1, IS_DELETE!=1)
- [ ] Tìm kiếm theo mã / tên CSKCB
- [ ] Click 1 dòng + bấm "Chọn" → control hiển thị "C.<MEDI_ORG_CODE>"
- [ ] Double-click 1 dòng → control hiển thị "C.<MEDI_ORG_CODE>"
- [ ] Đóng form picker mà không chọn → control giữ nguyên giá trị cũ

### Lưu
- [ ] Sửa CSKCB chuyển → bấm Lưu → API `api/HisBlood/Update` được gọi với HIS_BLOOD.TRANSFER_MEDI_ORG_CODE đúng giá trị
- [ ] Xóa giá trị CSKCB chuyển → Lưu → HIS_BLOOD.TRANSFER_MEDI_ORG_CODE = null

### Đa ngôn ngữ
- [ ] Tiếng Việt: caption "CSKCB chuyển:", tooltip có dấu
- [ ] Tiếng Anh: caption "Transferred FAC:", thông báo "Transferred medical facility code must not exceed 10 characters"
