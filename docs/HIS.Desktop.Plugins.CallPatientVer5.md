# Gọi Bệnh Nhân (CallPatientVer5) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.CallPatientVer5 |
| Loại | Form |
| Mục đích | Gọi tên bệnh nhân, hiển thị màn hình chờ (loa + màn hình LED/TV) tại khu vực tiếp đón, phòng khám. Hỗ trợ 2 mẫu màn hình chờ: mặc định và biến thể QY (2 lưới: chờ khám + chờ đọc kết quả). |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Người dùng chọn phòng → mở màn hình chờ (`frmWaitingScreen` / `frmWaitingScreen_QY9` / `frmWaitingScreen_QY_New`).
2. Plugin định kỳ gọi API `HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO` lấy danh sách yêu cầu dịch vụ theo phòng + trong ngày.
3. Phân loại 2 nhóm: danh sách chờ khám (chưa thực hiện) và danh sách chờ đọc kết quả CLS (đã thực hiện, chờ kết quả).
4. Hiển thị lên 2 lưới grid kế bên nhau, mỗi grid lấy `countPatient` bản ghi đầu tiên.
5. Gọi loa đọc tên + phát video quảng cáo từ thư mục cấu hình.

### Điều kiện hiển thị
- Lấy danh sách yêu cầu dịch vụ trong ngày (`INTRUCTION_TIME` between StartDay → EndDay).
- Lọc theo `EXECUTE_ROOM_ID` = phòng đã chọn.
- Danh sách chờ CLS: `DEPENDENCIES_COUNT > 0 && BUSY_COUNT == 0 && SERVICE_REQ_STT_ID == INPROCESS`.
- Danh sách chờ khám: phần còn lại theo các `SERVICE_REQ_STT_ID` đã cấu hình.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_SERVICE_REQ | Table | Yêu cầu dịch vụ |
| V_HIS_SERVICE_REQ | View | Yêu cầu DV kèm thông tin bệnh nhân (TDL_PATIENT_LAST_NAME, TDL_PATIENT_FIRST_NAME, TDL_PATIENT_DOB) |
| HIS_SERVICE_REQ_STT | Table | Trạng thái yêu cầu dịch vụ |
| HIS_PRIORITY_TYPE | Table | Loại ưu tiên (BHYT, người già, ...) |
| V_HIS_ROOM | View | Phòng thực hiện |

### Field quan trọng cho hiển thị tên
- `TDL_PATIENT_LAST_NAME` — họ + tên đệm (VD: "Cao Thị Thanh").
- `TDL_PATIENT_FIRST_NAME` — tên (VD: "Thảo").
- Tên hiển thị = ghép `LAST_NAME + " " + FIRST_NAME`. Khi vượt chiều rộng cột → cắt từ TRÁI bằng `CustomDrawCell` (xem section 4).

## 4. UI Layout

### `frmWaitingScreen_QY9` / `frmWaitingScreen_QY_New` — Mẫu 2 lưới

```
+-------------------------------------------------------------+
| [Logo + Tên BV + Tên phòng + Tên bác sĩ + Thời gian]        |
+--------------------------------+----------------------------+
| Danh sách chờ khám             | Danh sách chờ đọc kết quả  |
| STT | HỌ VÀ TÊN | NS | UT | TT | STT | HỌ VÀ TÊN | NS | UT |
|  8  |... Thị Thanh Thảo | 1999| ƯT| | 39  |... Võ Hoàng    | ...|
+--------------------------------+----------------------------+
| [Video quảng cáo + nhạc nền]                                |
+-------------------------------------------------------------+
```

### Cột PATIENT_FULL_NAME (HỌ VÀ TÊN) — cắt từ TRÁI
- Đăng ký event `gridView.CustomDrawCell` cho cả 2 grid (chờ khám + chờ CLS).
- Mỗi cell đo `TextRenderer.MeasureText(fullName, font)`:
  - Nếu **vừa** chiều rộng cột → để DevExpress vẽ mặc định.
  - Nếu **vượt** → cắt ký tự từ ĐẦU chuỗi, thêm prefix `"..."`, vẽ lại bằng `TextRenderer.DrawText` (center + vertical center).
- VD `"Cao Thị Thanh Thảo"` (cột chỉ chứa được ~14 ký tự) → hiển thị `"... Thị Thanh Thảo"`.
- VD `"Sinh kê đơn thuốc"` → hiển thị `"... kê đơn thuốc"`.
- Mục đích: trên TV 32 inch chia 2 cột, phần TÊN (cuối chuỗi — yếu tố phân biệt bệnh nhân) luôn hiển thị đầy đủ thay vì bị cắt từ phải.

### Implementation chi tiết
- Method `DrawPatientNameLeftTruncated(RowCellCustomDrawEventArgs e)` trong [frmWaitingScreen_QY.cs](../HIS/Plugins/HIS.Desktop.Plugins.CallPatientVer5/frmWaitingScreen_QY.cs) và [frmWaitingScreen_QY_New.cs](../HIS/Plugins/HIS.Desktop.Plugins.CallPatientVer5/frmWaitingScreen_QY_New.cs).
- Filter: `e.Column.FieldName == "PATIENT_FULL_NAME"` — không ảnh hưởng các cột khác.
- Padding: `maxWidth = e.Bounds.Width - 8` để có khoảng trống 2 bên.
- Vòng lặp truncate ký tự từng vị trí đầu cho đến khi `"..." + tail` vừa cột.

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Lấy yêu cầu DV + hospital fee | `HisRequestUriStore.HIS_SERVICE_REQ_GET_VIEW_WITH_HOSPITAL_FEE_INFO` | MosConsumer |

## 6. Dependencies

### Inter-Plugin
- Plugin gốc — không gọi plugin khác trong luồng chính.

## 7. Print

Không có chức năng in trong plugin này.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 15/05/2026 | sinhnt@vietsens.vn | Cột HỌ VÀ TÊN trên màn hình chờ 2 cột (QY9 + QY_New): cắt tên từ TRÁI bằng `CustomDrawCell` thay vì để DevExpress cắt từ phải. Tên dài như "Cao Thị Thanh Thảo" hiển thị thành "... Thị Thanh Thảo" (giữ phần TÊN ở cuối — yếu tố phân biệt bệnh nhân) để phù hợp TV 32 inch hai cột. Null-safety cho `TDL_PATIENT_LAST_NAME`/`TDL_PATIENT_FIRST_NAME` khi ghép. |

## 9. Test Cases

### Hiển thị tên dài
- [ ] "Cao Thị Thanh Thảo" — vượt cột → hiển thị "... Thị Thanh Thảo" (KHÔNG "Cao Thị Thanh...").
- [ ] "Sinh Kê Đơn Thuốc" — vượt cột → hiển thị "... Kê Đơn Thuốc".
- [ ] "LÊ THỊ BÍCH LOAN" — vượt cột → hiển thị "... Thị Bích Loan".
- [ ] "Lê A" (ngắn) — KHÔNG cắt, hiển thị nguyên "Lê A".
- [ ] Tên rỗng cả hai trường — cell trống, không exception.

### Hiển thị 2 lưới
- [ ] BN có yêu cầu CLS đang chờ kết quả → hiện lưới phải (chờ đọc KQ).
- [ ] BN có yêu cầu khám → hiện lưới trái (chờ khám).
- [ ] Số bản ghi mỗi lưới ≤ `CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS`.

### Highlight ưu tiên
- [ ] BN có `PRIORITY > 0` → dòng tô màu theo `_displayConfig.ColorPriority` + cột UT hiển thị "ƯT".
- [ ] BN đăng ký qua App (`IS_REGISTER_BY_APP == 1`) → cột UT hiển thị "HK HH:mm" (đỏ).

### Cột khác không bị ảnh hưởng
- [ ] STT, NS, UT, Trạng thái, Thời gian, Loại — vẽ bình thường, không bị "..." ở đầu.

### Hiển thị 2 lưới
- [ ] BN có yêu cầu CLS đang chờ kết quả → hiện lưới phải (chờ đọc KQ).
- [ ] BN có yêu cầu khám → hiện lưới trái (chờ khám).
- [ ] Số bản ghi mỗi lưới ≤ `CONFIG_KEY__SO_BENH_NHAN_TREN_DANH_SACH_CHO_KHAM_VA_CLS`.

### Highlight ưu tiên
- [ ] BN có `PRIORITY > 0` → dòng tô màu theo `_displayConfig.ColorPriority` + cột UT hiển thị "ƯT".
- [ ] BN đăng ký qua App (`IS_REGISTER_BY_APP == 1`) → cột UT hiển thị "HK HH:mm" (đỏ).
