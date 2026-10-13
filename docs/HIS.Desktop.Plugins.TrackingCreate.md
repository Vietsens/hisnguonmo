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
| HIS.Desktop.Plugins.HisTrackingList (module) | Tab "DS tờ điều trị" (việc 57960): gọi `PluginInstance.GetPluginInstance` với `listArgs = { treatmentId, true (chỉ xem), module }`, nhúng `UserControl` trả về vào `xtraTabPageTrackingList`. Không có module trong `GlobalVariables.currentModuleRaws` (chưa phân quyền) → hiện "Tài khoản chưa được phân quyền chức năng Danh sách tờ điều trị". **Phải đẩy kèm DLL HisTrackingList mới** (DLL cũ trả về form → tab báo "Không mở được danh sách tờ điều trị"). |

### Tab "DS tờ điều trị" (việc 57960)

- Tab thứ 3 của `xtraTabControl2`, ngay sau "Thực hiện đơn dự trù"; có cả khi tạo mới và khi sửa tờ; tab mặc định vẫn là "Y lệnh".
- Lần đầu chọn tab mới dựng khung xem; đổi tab qua lại không tải lại (giữ vị trí cuộn, mức thu phóng).
- Sau Lưu / Lưu ký thành công (`SetTrackingListNeedReload`): đang ở tab thì tải lại ngay, không thì tải lại ở lần chọn tab kế tiếp.
- Code ở partial `frmTrackingCreate__Pluss__TrackingList.cs`; chữ `frmTrackingCreateNew.xtraTabPageTrackingList.Text`, `frmTrackingCreateNew.lblTrackingListMessage.NoPermission|CannotOpen` (vi/en/my).

> Lưu ý: `HIS.UC.DHST` được nhiều plugin khác tham chiếu (DLL pre-built). Sau khi build lại UC, cần cập nhật `lib/HIS/HIS.UC.DHST/HIS.UC.DHST.dll` để các plugin khác nhận thay đổi.
>
> `HIS_TRACKING_TEMP` (mẫu DHST) **không có** các cột O2/FIO2/GCS/LOC/AVPU → luồng lưu/đọc mẫu DHST không mang theo 5 trường mới (giới hạn backend).

## 7. Print

Không thay đổi trong phạm vi việc này. (Muốn in 5 chỉ số mới cần cập nhật template/PDO tương ứng — ngoài phạm vi.)

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 21/09/2026 | sinhnt | Việc 3352 (PT-56263): y lệnh dịch vụ nay có thể mang giờ phút trong `USE_TIME`. Thêm `IsAnticipateByUseTime` + `FormatUseTimeDisplay` (`frmTrackingCreate__Pluss__Load.cs`): **phân loại** y lệnh dự trù so theo **NGÀY** với y lệnh dịch vụ (đơn thuốc DONDT/DONTT giữ nguyên phép so cũ `USE_TIME > INTRUCTION_DATE`) nên y lệnh dự trù **cùng ngày** chỉ định vẫn là y lệnh thường đúng như trước khi có ô giờ; **nhãn** "Dự trù ngày …" và node ngày ở cây tab 2 hiển thị thêm `HH:mm` khi y lệnh dịch vụ có giờ. `frmTrackingCreate__Pluss__Task.cs` không sửa vì chỉ lọc đơn thuốc DONDT. |
| 19/06/2026 | huannh | Bổ sung 5 chỉ số O2, FiO2, GCS, Mức độ ý thức (LOC), AVPU vào màn hình DHST (UC `HIS.UC.DHST`), đặt liền dưới SpO2; map lưu/đọc `HIS_DHST` trong `frmTrackingCreateNew`; cập nhật `CheckCtorDhst`/`CheckDhst`; thêm validation GCS(3–15)/FiO2(0–100)/O2(≥0); đa ngôn ngữ vi/en/my |
| 09/10/2026 | dangth2 | Việc 59656: tờ điều trị chỉ ghi nhận y lệnh do chính người tạo tờ chỉ định. Bật key: sau khi dựng cây tab 1 `MarkRequestOfOtherUser` đánh dấu `TreeSereServADO.IsRequestOfOtherUser` cho y lệnh CHƯA gắn tờ này và người chỉ định khác người tạo tờ (tờ mới: tài khoản đăng nhập); `treeSereServ_CheckAllNode` / `CheckYLFromBB` không tick sẵn các y lệnh đó (trước đây tick hết y lệnh trong ngày của mọi bác sĩ, kể cả khi sửa tờ cũ chưa ký); key = 2 còn khóa tick tay (`BeforeCheckNode`, `CheckNode`, ô tick vẽ mờ); trước `api/HisTracking/Create|Update` (Lưu, Lưu ký) gọi `CheckTrackingOwnerBeforeSave`: key 1 hỏi, key 2 chặn. Y lệnh đã gắn tờ này giữ nguyên (BE coi danh sách gửi lên là danh sách cuối). Tab 2 'Thực hiện đơn dự trù' không đổi. Partial `frmTrackingCreate__Pluss__TrackingOwner.cs`, 3 message `TrackingOwner__*`. |
| 13/10/2026 | phuongnm | Việc 57960 (NTP): thêm tab "DS tờ điều trị" sau tab "Thực hiện đơn dự trù", nhúng khung xem chỉ xem của HisTrackingList (toàn bộ tờ của hồ sơ, tờ mới nhất ở trên, không in/ký/sao chép); kiểm quyền theo module HisTrackingList; tải lại sau Lưu/Lưu ký. Partial `frmTrackingCreate__Pluss__TrackingList.cs`. |

## 9. Test Cases

- [ ] Mở màn hình / load DHST đã lưu → 5 ô hiển thị đúng giá trị; 2 dropdown chọn đúng mục
- [ ] Bỏ trống 5 ô rồi Lưu → lưu thành công, 5 giá trị = NULL
- [ ] Nhập giá trị rồi Lưu → lưu đúng giá trị; load lại đúng
- [ ] Tạo mới / Reset form → 5 ô trở về trống
- [ ] Nhập GCS ngoài 3–15 → bị chặn / báo lỗi
- [ ] Nhập FiO2 ngoài 0–100 → bị chặn / báo lỗi
- [ ] Tờ điều trị chỉ nhập 1 trong 5 trường mới (vd chỉ GCS) → vẫn lưu được DHST

### Việc 59656 - Y lệnh chỉ vào tờ điều trị của chính người chỉ định
- [ ] Key trống: tạo/sửa tờ vẫn tick sẵn mọi y lệnh trong ngày (như cũ)
- [ ] Key bật, A sửa tờ chưa ký của A sau khi B kê: y lệnh của B không được tick sẵn, Lưu không kéo y lệnh B vào tờ A
- [ ] Key = 1: tick tay y lệnh của B rồi Lưu → hỏi Có/Không
- [ ] Key = 2: không tick được y lệnh của B (ô tick mờ, tick cả nhóm cũng bỏ qua)
- [ ] Y lệnh của B đã gắn tờ A từ trước vẫn giữ nguyên khi Lưu

### Việc 57960 - Tab "DS tờ điều trị"
- [ ] Mở tạo tờ mới: tab mặc định "Y lệnh", tab "DS tờ điều trị" đứng sau "Thực hiện đơn dự trù"
- [ ] Bấm tab: hiện "Đang tải danh sách tờ điều trị...", sau đó các tờ nối tiếp, tờ mới nhất ở trên; vẫn nhập được tờ mới trong lúc tải
- [ ] Hồ sơ chưa có tờ: "Chưa có tờ điều trị trong đợt điều trị này"
- [ ] Tờ đã ký hiện văn bản ký; tờ chưa ký hiện bản dựng Mps000062 giống bản xem 1 tờ ở Danh sách tờ điều trị
- [ ] Không có nút In / Phóng to màn hình; menu chuột phải trống; không chọn/chép được chữ (Ctrl+C, Ctrl+A)
- [ ] Đổi tab rồi quay lại: giữ vị trí cuộn và mức thu phóng; dữ liệu đang nhập không đổi
- [ ] Lưu / Lưu ký thành công rồi mở tab: có tờ vừa lưu ở trên cùng
- [ ] Tài khoản không có chức năng Danh sách tờ điều trị: tab hiện thông báo chưa được phân quyền
- [ ] Mất kết nối khi tải: "Không tải được danh sách tờ điều trị. Vui lòng thử lại." + nút Thử lại
