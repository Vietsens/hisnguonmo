# HIS.Desktop.Plugins.DrugUsageAnalysis — Tài Liệu Module

> Tài liệu này tập trung vào việc **3212 (PT-54721): Xem được thông tin điều trị trong chức năng "Phân tích sử dụng thuốc"**. Các nghiệp vụ khác của plugin (lọc, phân trang, cây y lệnh thuốc, chi tiết phân tích) chỉ nêu ở mức liên quan.

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.DrugUsageAnalysis |
| Loại | UserControl (màn hình tab) |
| Mục đích | Dược sĩ lâm sàng rà soát đơn thuốc của người bệnh nội trú. Feature này bổ sung 3 lối vào tra cứu **Danh sách y lệnh**, **Tờ điều trị**, **Xem kết quả xét nghiệm** ngay trên màn phân tích. |
| Người sửa | phuongnm |
| Ngày | 19/08/2026 |
| Trạng thái | Bảo trì |

**Plugin được gọi (tái sử dụng nguyên vẹn, không sửa):**
- `HIS.Desktop.Plugins.ServiceReqList` — Danh sách y lệnh.
- `HIS.Desktop.Plugins.HisTrackingList` — Tờ điều trị.
- `HIS.Desktop.Plugins.SumaryTestResults` — Kết quả xét nghiệm.

**Màn hình tham chiếu (mẫu sao chép):** `HIS.Desktop.Plugins.ServiceReqPatient` ("Xem y lệnh thuốc vật tư") — đã có sẵn đúng 3 nút này ở góc dưới phải (`ServiceReqPatientForm.cs:1328-1389`).

## 2. Quy Trình Nghiệp Vụ

### Bối cảnh
Màn "Phân tích sử dụng thuốc" chỉ hiển thị danh sách bệnh nhân (trái) và cây y lệnh thuốc theo tờ điều trị (phải). Để kết luận về liều dùng / trùng lặp hoạt chất, dược sĩ phải đối chiếu với diễn biến điều trị và kết quả cận lâm sàng — trước đây phải thoát ra mở chức năng khác rồi quay lại, mất điều kiện lọc và bệnh nhân đang chọn.

### Thay đổi
Bổ sung 3 nút ở góc dưới phải (dưới cây y lệnh), mở 3 màn hình xem sẵn có dạng dialog theo bệnh nhân đang được chọn. Chỉ tra cứu, không nhập liệu.

### Luồng (rút gọn)
```
UCDrugUsageAnalysis  (bấm 1 trong 3 nút)
  → GetSelectedTreatmentBedRoom()          ← gridView1.GetFocusedRow() as L_HIS_TREATMENT_BED_ROOM
      → null  ⇒ MessageBox cảnh báo, dừng
  → PluginInstanceBehavior.ShowModule(moduleLink,
        currentModule.RoomId, currentModule.RoomTypeId, listArgs)
    → plugin đích tự nạp dữ liệu theo TREATMENT_ID và hiển thị dialog
```

### Điều kiện nghiệp vụ
- Lấy bệnh nhân **tại thời điểm bấm nút** (không lưu biến trạng thái) ⇒ luôn bám đúng dòng đang focus, kể cả khi người dùng chuyển dòng bằng phím mũi tên (không phát sinh `RowClick`).
- Chưa chọn bệnh nhân ⇒ cảnh báo, **không** mở màn hình trống.
- Bệnh nhân chưa có kết quả / chưa có tờ điều trị ⇒ màn hình vẫn mở, hiển thị rỗng (do plugin đích xử lý).
- Không thêm key `HIS_CONFIG`: quyền đã kiểm soát ở 2 tầng — quyền vào chức năng "Phân tích sử dụng thuốc" và quyền dữ liệu của chính 3 plugin được gọi.

## 3. EFMODEL Sử Dụng (liên quan feature)

| Đối tượng | Vai trò |
|-----------|---------|
| `L_HIS_TREATMENT_BED_ROOM` | Dòng lưới bệnh nhân; dùng `TREATMENT_ID`, `TREATMENT_CODE` |
| `HIS_TREATMENT` | Tham số truyền sang `ServiceReqList` (chỉ cần `ID` + `TREATMENT_CODE`, form đích tự nạp lại hồ sơ đầy đủ) |

## 4. Cấu Hình Hệ Thống (HIS_CONFIG)

Không thêm key nào. Không có cấu hình bật/tắt cho feature này.

## 5. Files Thay Đổi

| File | Thay đổi |
|------|----------|
| `UCDrugUsageAnalysis.Designer.cs` | Thêm 3 `SimpleButton` (`btnDsYlenh`, `btnToDieuTri`, `btnSumaryTestResults`) + 3 `LayoutControlItem` + 1 `EmptySpaceItem` vào `layoutControlGroup3`; `layoutControlItem14` (cây y lệnh) thu chiều cao 582 → 554 |
| `UCDrugUsageAnalysis_ViewInfo_EventHandler.cs` **(mới)** | `GetSelectedTreatmentBedRoom()` + 3 hàm `*_Click` gọi `ShowModule` |
| `UCDrugUsageAnalysis_LanguageKey.cs` | Gán `Text` cho 3 nút theo resource |
| `Resources\Lang.vi.resx`, `Lang.en.resx`, `Lang.my.resx` | Thêm 4 khoá: `UCDrugUsageAnalysis.btnDsYlenh.Text`, `.btnToDieuTri.Text`, `.btnSumaryTestResults.Text`, `.MsgChuaChonBenhNhan` |
| `HIS.Desktop.Plugins.DrugUsageAnalysis.csproj` | Khai báo `Compile Include` cho file mới |

**Không sửa** `UCDrugUsageAnalysis.cs`, luồng nạp lưới / nạp cây và các file `GridLookUpEdit` — tránh hồi quy cho luồng lọc, phân trang hiện có.

### Tham số truyền sang từng màn hình

```csharp
// Danh sách y lệnh
var treatment = new HIS_TREATMENT { ID = row.TREATMENT_ID, TREATMENT_CODE = row.TREATMENT_CODE };
ShowModule("HIS.Desktop.Plugins.ServiceReqList", RoomId, RoomTypeId, new List<object> { treatment });

// Tờ điều trị
ShowModule("HIS.Desktop.Plugins.HisTrackingList", RoomId, RoomTypeId, new List<object> { row.TREATMENT_ID });

// Kết quả xét nghiệm
ShowModule("HIS.Desktop.Plugins.SumaryTestResults", RoomId, RoomTypeId, new List<object> { row.TREATMENT_ID });
```

## 6. Dependencies

- `HIS.Desktop.ModuleExt` (`PluginInstanceBehavior.ShowModule`) — **đã có sẵn ProjectReference**, không phải bổ sung.
- `HIS.Desktop.LibraryMessage` (`MessageUtil`) — tiêu đề hộp thoại cảnh báo.
- Máy trạm phải có sẵn 3 plugin đích trong `Plugins\Module` và tài khoản được gán quyền các module đó. Thiếu plugin ⇒ `ShowModule` tự hiện thông báo chuẩn, màn phân tích không treo.

## 7. Build & Deploy

```
MSBuild HIS.Desktop.Plugins.DrugUsageAnalysis.csproj
  /p:Configuration=Debug
  "/p:SolutionDir=d:\HISNGUONMO_BACKUP\hisnguonmo\HIS\\" /p:SolutionName=HIS.Desktop
  /p:BuildProjectReferences=false
  "/p:ReferencePath=...\HIS.Desktop\bin\Debug\ReferencedAssemblies"
```

Deploy: `HIS.Desktop.Plugins.DrugUsageAnalysis.dll` → `Plugins\Module\`; **bắt buộc kèm** `vi\` và `en\HIS.Desktop.Plugins.DrugUsageAnalysis.resources.dll` (thiếu ⇒ nhãn nút và câu cảnh báo hiện rỗng).

## 8. Changelog

| Ngày | Người | Nội dung |
|------|-------|----------|
| 19/08/2026 | phuongnm | Việc 3212 (PT-54721): bổ sung 3 nút "Danh sách y lệnh", "Tờ điều trị", "Xem kết quả xét nghiệm" tại màn Phân tích sử dụng thuốc; gọi lại 3 plugin sẵn có qua `ShowModule`; cảnh báo khi chưa chọn bệnh nhân; resource vi/en/my. |

## 9. Test Cases

### Chuẩn bị
- 1 bệnh nhân nội trú **có** kết quả xét nghiệm và **có** tờ điều trị.
- 1 bệnh nhân mới nhập viện **chưa** có kết quả xét nghiệm / chưa có tờ điều trị.
- Tài khoản được gán quyền cả 4 module (DrugUsageAnalysis + 3 module đích).

### Chức năng
- [ ] Góc dưới phải màn Phân tích sử dụng thuốc có đủ 3 nút, đúng nhãn tiếng Việt.
- [ ] Chưa chọn bệnh nhân, bấm từng nút → hiện cảnh báo, **không** mở màn trống.
- [ ] Chọn bệnh nhân → **Xem kết quả xét nghiệm** → đúng mã điều trị / tên bệnh nhân.
- [ ] **Danh sách y lệnh** → đóng → **Tờ điều trị** → đóng → màn phân tích giữ nguyên điều kiện lọc, trang và dòng đang chọn.
- [ ] Chuyển bệnh nhân bằng **phím mũi tên** rồi mở lại → dữ liệu đổi theo bệnh nhân mới.
- [ ] Bệnh nhân chưa có kết quả / chưa có tờ điều trị → màn vẫn mở, hiển thị rỗng, không báo lỗi.

### Hồi quy
- [ ] Tìm kiếm theo mã BN / mã điều trị / khoa / buồng / diện điều trị → như cũ.
- [ ] Phân trang, cây y lệnh thuốc, nút mở chi tiết phân tích trên cây → như cũ.
- [ ] Bố cục panel lọc bên trái và lưới bệnh nhân không xê dịch.

### Đa ngôn ngữ
- [ ] Đổi sang English → nhãn 3 nút và câu cảnh báo hiển thị đúng tiếng Anh.
