# Chi Tiết Phiếu Lĩnh (AggrExpMestDetail) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.AggrExpMestDetail |
| Loại | Form (frmAggrExpMestDetail : FormBase) |
| Mục đích | Xem chi tiết phiếu lĩnh tổng hợp; Duyệt, Thực xuất, In ấn phiếu lĩnh thuốc/vật tư |
| Subsystem | Frontend |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
- Mở form với 1 phiếu lĩnh tổng hợp (V_HIS_EXP_MEST). Form load các phiếu con + thuốc/vật tư.
- Người dùng có thể: **Duyệt (Ctrl A)**, **Thực xuất (Ctrl E)**, **In ấn** (dropdown nhiều loại phiếu).
- Trạng thái phiếu: Yêu cầu → Duyệt → (Thực xuất) Hoàn thành.

### PTTK_42983 — Tự động in phiếu khi Thực xuất (mặc định TẮT)
- Ô tích **"In Phiếu"** đặt cạnh nút **"In ấn"** (góc dưới phải form). Mặc định bỏ tick → an toàn cho các viện khác.
- Tick ô "In Phiếu" **hoặc** chuột phải vào ô → mở dropdown chọn loại phiếu (cho chọn nhiều), danh sách giống dropdown nút "In ấn".
- Dropdown không còn loại nào được chọn → ô "In Phiếu" tự bỏ tick.
- Mở lại màn hình → khôi phục trạng thái ô tích + các loại phiếu đã chọn (ControlStateWorker, giống checkbox "In:").
- **Thực xuất thành công + ô "In Phiếu" đang tick** → lần lượt mở Xem trước cho từng loại phiếu đã chọn (tái sử dụng plugin in hiện có). Thực xuất thất bại hoặc ô bỏ tick → không tự in.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST | View | Phiếu lĩnh tổng hợp + phiếu con |
| V_HIS_EXP_MEST_MEDICINE / V_HIS_EXP_MEST_MATERIAL | View | Thuốc / vật tư của phiếu |

## 4. UI Layout (vùng nút dưới cùng)

```
[Lưu]      ...      [In: ☐] [Duyệt (Ctrl A)] [Thực xuất (Ctrl E)] [In ấn] [☑ :In phiếu]
```

| Control | Mô tả |
|---------|-------|
| chkPrint ("In:") | Checkbox in khi mở form filter (sẵn có) |
| cboPrint ("In ấn") | Mở dropdown chọn loại phiếu để in |
| **chkInPhieu (":In phiếu")** | **MỚI — bật/tắt tự động in khi Thực xuất; mở dropdown chọn loại phiếu** |

## 5. API Endpoints

| Action | URI | Consumer |
|--------|-----|----------|
| Thực xuất | api/HisExpMest/AggrExport (RequestUriStore.HIS_EXP_MEST_AGGREXPORT) | MosConsumer |
| Thực xuất (THPK) | api/HisExpMest/AggrExamExport | MosConsumer |
| Duyệt | api/HisExpMest/AggrApprove / AggrExamApprove | MosConsumer |

> PTTK_42983 KHÔNG thêm/đổi API — tái sử dụng API thực xuất và plugin in hiện có.

## 6. Dependencies

- Plugin in filter: HIS.Desktop.Plugins.AggrExpMestPrintFilter (ShowFormFilter).
- Library: PrintPrescription, PrintAggrExpMest (in công khai/hủy).

## 7. Print

| Loại phiếu (PrintType) | Hành vi |
|------------------------|---------|
| InTraDoiThuoc / InPhieuTongHop / InPhieuLinhThuoc / InPhieuLinhThuocTheoBenhNhan | ShowFormFilter(1..4) |
| InPhieuCongKhaiThuocBenhNhan | PrintAggrExpMest Mps000262 |
| InPhieuHuyThuocVatTu_434 | PrintAggrExpMest Mps000434 (khi HAS_NOT_PRES=1) |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 02/06/2026 | phuongnm | PTTK_42983: Thêm ô tích "In Phiếu" cạnh nút "In ấn" + dropdown chọn loại phiếu (chọn nhiều, lưu trạng thái); tự động mở Xem trước từng loại phiếu đã chọn khi Thực xuất thành công. Tách `ExecutePrintByType` dùng chung cho menu "In ấn" và tự động in. File mới: `frmAggrExpMestDetail__AutoPrint.cs`. |

## 9. Test Cases

- [ ] Mặc định: ô "In Phiếu" bỏ tick → Thực xuất KHÔNG tự in.
- [ ] Tick "In Phiếu" → hiện dropdown; chọn ≥1 loại → Thực xuất thành công → mở Xem trước đúng từng loại.
- [ ] Chuột phải vào ô "In Phiếu" → hiện dropdown.
- [ ] Bỏ chọn hết loại trong dropdown → ô "In Phiếu" tự bỏ tick.
- [ ] Đóng/mở lại form → khôi phục đúng trạng thái tick + loại phiếu đã chọn.
- [ ] Thực xuất thất bại → KHÔNG tự in.
- [ ] Giao diện vùng nút dưới không vỡ trên 1366x768.
