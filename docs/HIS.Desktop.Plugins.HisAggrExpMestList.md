# Danh sách phiếu lĩnh (kho nội trú) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisAggrExpMestList |
| Loại | UC (UserControlBase) |
| Mục đích | Hiển thị danh sách phiếu lĩnh tổng hợp (kho nội trú): tìm kiếm, duyệt/bỏ duyệt, thực xuất/hủy thực xuất, in các loại phiếu, xuất danh sách mã phiếu ra Excel. |
| Người tạo | (kế thừa) |
| Ngày tạo | (kế thừa) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
- Lọc theo: mã bệnh nhân / mã điều trị / từ khóa, ngày tạo, ngày thực xuất, trạng thái phiếu, chứa thuốc xuất hủy.
- Trên mỗi dòng grid (1 phiếu lĩnh): Xem chi tiết, Sửa, Hủy, Duyệt, Không duyệt, **Thực xuất**, Hủy thực xuất, Hủy duyệt — tùy trạng thái + kho làm việc.
- "In tra đổi tổng hợp": chọn nhiều dòng → mở `AggrExpMestPrintFilter` (printType 5).
- Chuột phải dòng: In phiếu lĩnh tổng hợp / In tra đổi thuốc tổng hợp / Phiếu công khai theo BN.
- "Xuất mã phiếu": xuất danh sách mã phiếu đã thực xuất ra Excel (FlexCel).

### Sơ đồ trạng thái phiếu (HIS_EXP_MEST_STT)
```
Nháp(DRAFT) → Yêu cầu(REQUEST) → Duyệt(EXECUTE) → Thực xuất(DONE)
                    ↓
              Từ chối(REJECT)
```

### Tính năng "In Phiếu" (tự động in khi thực xuất — mục 4.1.2)
- Ô tích **'In phiếu'** cạnh nút "In tra đổi tổng hợp", phía trên grid. Mặc định bỏ tick.
- Bấm vào ô 'In phiếu' (hoặc chuột phải) → **mở dropdown ô tích vuông** để tự chọn loại (KHÔNG ép chọn mặc định).
- Tích/bỏ tích 1 loại trong dropdown = **1 click** (CheckOnClick).
- Dropdown gồm 5 loại (giống dropdown 'In ẩn' ở màn Chi tiết phiếu lĩnh):
  - Phiếu tra đổi thuốc, Phiếu tổng hợp, Phiếu lĩnh thuốc/vật tư, Phiếu lĩnh theo bệnh nhân, Phiếu công khai theo bệnh nhân.
- Ô 'In phiếu' là **chỉ báo**: tick KHI VÀ CHỈ KHI có ≥1 loại được chọn. Bấm vào ô KHÔNG bật/tắt trực tiếp mà mở dropdown để chọn loại.
  - Tích 1 loại trong dropdown → ô 'In phiếu' tự bật.
  - Bỏ tích hết loại → ô 'In phiếu' tự tắt.
  - Không bao giờ có cảnh "tick mà rỗng" hay "không tick mà có loại".
- Mở lại màn hình → khôi phục các loại phiếu đã chọn; trạng thái tick suy ra theo số loại (ControlState).
- **Thực xuất 1 phiếu trên grid thành công** + ô 'In phiếu' đang tick → mở màn hình **Xem trước** cho từng loại phiếu đã chọn, ứng với phiếu vừa thực xuất.
- Thực xuất thất bại hoặc ô 'In phiếu' bỏ tick → không tự in.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST_3 | View | Dữ liệu hiển thị grid danh sách phiếu lĩnh |
| V_HIS_EXP_MEST | View | Phiếu lĩnh (con) phục vụ duyệt/in |
| V_HIS_EXP_MEST_MEDICINE / _MATERIAL | View | Thuốc / vật tư của phiếu |
| V_HIS_MEDI_STOCK / V_HIS_ROOM / HIS_DEPARTMENT | View/Table | Kho, phòng, khoa làm việc |
| HisExpMestSDO | SDO | DTO cho duyệt/thực xuất/hủy (ExpMestId, ReqRoomId) |

## 4. UI Layout

```
+--------------------------------------------------------------------------+
| [In tra đổi tổng hợp] [☐ In phiếu ▾]                                      |
| [Bộ lọc trái: BN/ĐT/từ khóa, ngày tạo, ngày thực xuất, trạng thái]        |
| Grid: STT | (nút) | Mã xuất | Kho xuất | Khoa yêu cầu | ... | 4 cột audit  |
| [Phân trang]                                       [Xuất mã phiếu]        |
+--------------------------------------------------------------------------+
```

| Control | Mục đích |
|---------|----------|
| chkInPhieu (CheckEdit) | Bật/tắt tự động in khi thực xuất; mở dropdown loại phiếu |
| popupContainerInPhieu (PopupControlContainer + CheckedListBoxControl) | Dropdown ô tích vuông, tích chọn nhiều loại phiếu (ở mở khi chọn nhiều) |
| Inventec.UC.Paging | Phân trang server-side |

## 5. API Endpoints

| Action | URI | Consumer | Filter/DTO |
|--------|-----|----------|------------|
| Lấy danh sách | api/HisExpMest/GetView3 | MosConsumer | HisExpMestView3Filter |
| Duyệt | api/HisExpMest/AggrApprove | MosConsumer | HisExpMestSDO |
| Bỏ duyệt | api/HisExpMest/AggrUnapprove | MosConsumer | HisExpMestSDO |
| Không duyệt | api/HisExpMest/Decline | MosConsumer | HisExpMestSDO |
| **Thực xuất** | api/HisExpMest/AggrExport | MosConsumer | HisExpMestSDO |
| Hủy thực xuất | api/HisExpMest/AggrUnexport | MosConsumer | HisExpMestSDO |
| Xóa/hủy | api/HisExpMest/AggrDelete | MosConsumer | long (id) |
| Lấy phiếu con | api/HisExpMest/GetView | MosConsumer | HisExpMestViewFilter |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintAggrExpMest | In phiếu công khai theo BN (Mps000262) |
| HIS.Desktop.Library.CacheClient | ControlStateWorker — lưu trạng thái ô 'In phiếu' + loại phiếu đã chọn |

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AggrExpMestPrintFilter | In tra đổi tổng hợp; tự in khi thực xuất (loại 1-4) | V_HIS_EXP_MEST hoặc List<V_HIS_EXP_MEST>, long printType, Module |
| HIS.Desktop.Plugins.AggrExpMestDetail | Xem chi tiết phiếu (qua CallModule) | V_HIS_EXP_MEST, DelegateSelectData |

### Ánh xạ loại phiếu → printType (AggrExpMestPrintFilter)
| Loại phiếu | printType | Kết quả |
|------------|-----------|---------|
| Phiếu tra đổi thuốc | 1 | Mps000047 (form/xem trước) |
| Phiếu tổng hợp | 2 | Mps000046 (xem trước) |
| Phiếu lĩnh thuốc, vật tư | 3 | Mps000049 (xem trước) |
| Phiếu lĩnh theo bệnh nhân | 4 | Mps000235 (xem trước) |
| Phiếu công khai theo BN | — | PrintAggrExpMestProcessor → Mps000262 (xem trước) |

## 7. Print

| Loại in | PrintTypeCode | Library/MPS |
|---------|--------------|-------------|
| Phiếu công khai theo BN | Mps000262 | Library.PrintAggrExpMest |
| Tra đổi / Tổng hợp / Lĩnh thuốc-VT / Theo BN | Mps000047/046/049/235 | AggrExpMestPrintFilter (RichEditor/PrintNow) |
| Xuất danh sách mã phiếu | (Excel template DanhSachCacMaPhieuLinh.xlsx) | Inventec.Common.FlexCellExport |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 04/06/2026 | tuanln | Mục 4.1.2: Thêm ô tích 'In phiếu' + dropdown chọn loại phiếu (5 loại) cạnh nút 'In tra đổi tổng hợp'; bổ sung cơ chế lưu trạng thái control (ControlStateWorker); gắn tự động mở xem trước từng loại phiếu đã chọn khi Thực xuất từng phiếu thành công. File mới: `EnumInPhieuPrintType.cs`, `UCHisAggrExpMestList___InPhieu.cs`. |
| 05/06/2026 | tuanln | Tách trạng thái loại phiếu ra nguồn-chuẩn (HashSet) độc lập với dropdown; tự bỏ tick 'In phiếu' khi không còn loại nào (cả lúc toggle lẫn khi load); defer auto-print bằng BeginInvoke. Đổi dropdown từ PopupMenu+BarCheckItem sang **CheckedListBoxControl** (ô tích vuông) trong PopupControlContainer — đúng giao diện ô tích, ở mở khi chọn nhiều loại. |
| 05/06/2026 | tuanln | **Đồng bộ 2 chiều** ô 'In phiếu' ⇔ danh sách loại (tick ⇔ có ≥1 loại): tick khi rỗng tự chọn mặc định; bỏ tích ô → bỏ hết loại; tích 1 loại → ô tự bật; bỏ hết loại → ô tự tắt. Có guard chống đệ quy. Tự sửa trạng thái lưu bị lệch khi load. |
| 05/06/2026 | tuanln | Bật `CheckedListBoxControl.CheckOnClick` → tích/bỏ tích loại bằng 1 click (trước đây phải 2 click do select-rồi-mới-check). Bỏ ép chọn mặc định khi bấm ô 'In phiếu'; ô 'In phiếu' thành CHỈ BÁO (tick ⟺ có ≥1 loại), bấm vào ô = mở dropdown để tự chọn. Không còn cảnh "tick mà rỗng / không tick mà có loại". |

## 9. Test Cases

### Ô 'In phiếu' + dropdown
- [ ] Mặc định ô 'In phiếu' bỏ tick.
- [ ] Tick ô 'In phiếu' → hiện dropdown 5 loại phiếu.
- [ ] Chuột phải vào ô 'In phiếu' → hiện dropdown.
- [ ] Tích/bỏ tích loại phiếu → ghi nhớ trạng thái.
- [ ] Bỏ tích hết các loại trong dropdown → ô 'In phiếu' tự bỏ tick.
- [ ] Đóng/mở lại màn hình → khôi phục đúng trạng thái tick + loại phiếu đã chọn.

### Tự động in khi thực xuất
- [ ] Ô 'In phiếu' tick + chọn ≥1 loại → Thực xuất 1 phiếu thành công → mở xem trước cho từng loại đã chọn, đúng phiếu vừa thực xuất.
- [ ] Ô 'In phiếu' bỏ tick → Thực xuất → KHÔNG tự in.
- [ ] Thực xuất thất bại → KHÔNG tự in.
