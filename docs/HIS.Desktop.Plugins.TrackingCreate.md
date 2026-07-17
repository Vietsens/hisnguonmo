# Tạo Tờ Điều Trị (TrackingCreate) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TrackingCreate |
| Loại | Form (`frmTrackingCreateNew` kế thừa FormBase) |
| Mục đích | Màn hình "tờ điều trị" — bác sĩ ghi nội dung theo dõi, y lệnh, chỉ định DV/thuốc, và nhập **Dấu hiệu sinh tồn (DHST)** cho 1 lần điều trị |
| Form chính | `frmTrackingCreateNew.cs` (+ các partial `__Pluss__*.cs`) |
| Trạng thái | Bảo trì |

> Form cũ `frmTrackingCreate.cs` không còn được khởi tạo — Behavior dùng `frmTrackingCreateNew`.

## 2. Quy Trình Nghiệp Vụ (phần DHST)

- Tab **"Dấu hiệu sinh tồn"** nhúng User Control dùng chung **`HIS.UC.DHST`** (qua `InitUCDHST()` → `DHSTProcessor.Run`).
- Khi lưu tờ điều trị: `dhstProcessor.GetValue(uc)` trả về `DHSTADO`; nếu DHST không rỗng (`CheckCtorDhst`/`CheckDhst`) → map sang `HIS_DHST` và gắn vào `trackingSDOs.Dhst` để lưu.
- Khi sửa / chọn tờ cũ: `dhstProcessor.SetValue(uc, HIS_DHST)` nạp lại giá trị lên control.

### Các chỉ số DHST nhập trên màn hình

Mạch, Nhiệt độ, Huyết áp (max/min), Nhịp thở, Cân nặng, Chiều cao, Vòng ngực, Vòng bụng, Nước tiểu, Đường máu mao mạch, SpO2, BMI/Diện tích da (tự tính), Khác (ghi chú), và **5 chỉ số bổ sung** (xem mục 3).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_DHST | Table | Dấu hiệu sinh tồn của lần đo |
| HIS_TRACKING / HIS_TRACKING_TEMP | Table | Nội dung theo dõi + mẫu |
| HIS_TREATMENT | Table | Lần điều trị |

### Các cột HIS_DHST cho 5 chỉ số bổ sung (đặt liền dưới SpO2)

| UI (caption) | Cột HIS_DHST | Kiểu | Control | Ràng buộc |
|--------------|--------------|------|---------|-----------|
| O2 (L/phút) | `O2` | decimal? | SpinEdit | ≥ 0, không bắt buộc |
| FiO2 (%) | `FIO2` | decimal? | SpinEdit | 0–100, không bắt buộc |
| GCS (điểm) | `GCS` | short? | SpinEdit (số nguyên) | 3–15, không bắt buộc |
| Mức độ ý thức | `LOC` | short? | ImageComboBoxEdit | 1=Tỉnh táo … 5=Hôn mê, không bắt buộc |
| AVPU | `AVPU` | short? | ImageComboBoxEdit | 1=A … 4=U, không bắt buộc |

- Hai ô danh sách hiển thị tên tiếng Việt, **lưu giá trị số** (LOC 1–5, AVPU 1–4).
- O2/FiO2/GCS ngoài khoảng hợp lệ bị chặn nhập (Min/MaxValue) và báo lỗi khi lưu (DXValidationProvider).
- Bỏ trống → lưu NULL. Tất cả 5 trường không bắt buộc.

## 4. UI Layout (DHST)

5 control mới là 5 dòng full-width nằm **dưới cùng** `layoutControlGroup1` của UC `HIS.UC.DHST` (sau hàng BMI), thứ tự **O2 → FiO2 → GCS → Mức độ ý thức → AVPU**. Khung DHST giữ nguyên kích thước; LayoutControl tự hiện thanh cuộn dọc khi tràn.

UC được sửa tại `UC/HIS.UC.DHST/`:
- `Run/UCDHST.Designer.cs` — khai báo + layout 5 control (`spinO2`, `spinFIO2`, `spinGCS`, `cboLOC`, `cboAVPU`).
- `Run/UCDHST.cs` — reset control trong Load; nạp caption + đổ items dropdown (`InitComboLocAvpu`) trong `SetCaptionByLanguageKey`.
- `SetValue/UCDHST__SetValue.cs` — nạp `HIS_DHST` → control.
- `GetValue/UCDHST__GetValue.cs` — đọc control → `DHSTADO`.
- `Run/UCDHST__Validate.cs` — validate range O2/FiO2/GCS (luôn áp dụng).
- `Resources/Lang.vi|en|my.resx` — key `UCDHST.lciO2/FIO2/GCS/LOC/AVPU.Text`, `UCDHST.cboLOC.Item1..5`, `UCDHST.cboAVPU.Item1..4`.

## 5. API Endpoints

Không thay đổi. Lưu qua SDO tờ điều trị hiện có (`trackingSDOs`).

## 6. Dependencies

| Thành phần | Vai trò |
|-----------|---------|
| HIS.UC.DHST (UC dùng chung) | Nhập DHST — **đã sửa**: thêm 5 trường O2/FiO2/GCS/LOC/AVPU |

> Lưu ý: `HIS.UC.DHST` được nhiều plugin khác tham chiếu (DLL pre-built). Sau khi build lại UC, cần cập nhật `lib/HIS/HIS.UC.DHST/HIS.UC.DHST.dll` để các plugin khác nhận thay đổi.
>
> `HIS_TRACKING_TEMP` (mẫu DHST) **không có** các cột O2/FIO2/GCS/LOC/AVPU → luồng lưu/đọc mẫu DHST không mang theo 5 trường mới (giới hạn backend).

## 7. Print

Không thay đổi trong phạm vi việc này. (Muốn in 5 chỉ số mới cần cập nhật template/PDO tương ứng — ngoài phạm vi.)

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 19/06/2026 | huannh | Bổ sung 5 chỉ số O2, FiO2, GCS, Mức độ ý thức (LOC), AVPU vào màn hình DHST (UC `HIS.UC.DHST`), đặt liền dưới SpO2; map lưu/đọc `HIS_DHST` trong `frmTrackingCreateNew`; cập nhật `CheckCtorDhst`/`CheckDhst`; thêm validation GCS(3–15)/FiO2(0–100)/O2(≥0); đa ngôn ngữ vi/en/my |

## 9. Test Cases

- [ ] Mở màn hình / load DHST đã lưu → 5 ô hiển thị đúng giá trị; 2 dropdown chọn đúng mục
- [ ] Bỏ trống 5 ô rồi Lưu → lưu thành công, 5 giá trị = NULL
- [ ] Nhập giá trị rồi Lưu → lưu đúng giá trị; load lại đúng
- [ ] Tạo mới / Reset form → 5 ô trở về trống
- [ ] Nhập GCS ngoài 3–15 → bị chặn / báo lỗi
- [ ] Nhập FiO2 ngoài 0–100 → bị chặn / báo lỗi
- [ ] Tờ điều trị chỉ nhập 1 trong 5 trường mới (vd chỉ GCS) → vẫn lưu được DHST
