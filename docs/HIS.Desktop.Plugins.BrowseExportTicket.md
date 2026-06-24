# Duyệt Phiếu Xuất — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.BrowseExportTicket |
| Loại | Form (frmBrowseExportTicket : FormBase) |
| Mục đích | Duyệt phiếu xuất kho theo các loại: Thuốc, Vật tư, Máu, Vật tư đích danh - tái sử dụng. Cho phép duyệt, thực xuất và in phiếu. |
| Người tạo | (cũ) |
| Ngày tạo | (cũ) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (tab Máu)
1. Mở phiếu xuất → form load y lệnh máu (`V_HIS_EXP_MEST_BLTY_REQ_1`) vào grid trái, kho máu (`V_HIS_BLOOD`) vào grid dưới.
2. Chọn y lệnh máu (`currentBlty`) → lọc kho máu phù hợp loại/ABO/RH.
3. Thêm túi máu (quét mã vạch hoặc nút `+`) → túi máu vào `dicBloodAdo` (grid phải), mỗi túi gắn `ExpMestBltyId = currentBlty.ID`.
4. (MỚI) Đính kèm dịch vụ xét nghiệm vào túi máu — xem mục dưới.
5. Lưu (Duyệt) → `api/HisExpMest/Approve`; Thực xuất → `api/HisExpMest/Export`.
6. In truyền máu / In phiếu xuất máu (mps107).

### (MỚI) Đính kèm dịch vụ xét nghiệm vào túi máu
- Vùng túi máu đã chọn (grid `gridControlExpMestBlood`) bổ sung:
  - Nút **"Dịch vụ xét nghiệm"** (`btnTestServiceReq`) — áp dụng chung (chọn y lệnh chung).
  - Menu chuột phải **"Đính kèm dịch vụ xét nghiệm"** (`popupMenuBlood` / `bbtnAttachTestService`) — áp dụng cho túi máu đang chọn (chọn từng dịch vụ).
- Mở popup **"Đính kèm dịch vụ xét nghiệm"** (`frmAttachTestService`):
  - Chỉ hiển thị dịch vụ loại Xét nghiệm (`HIS_SERVICE_REQ.SERVICE_REQ_TYPE_ID = HIS_SERVICE_REQ_TYPE.ID__XN = 2`) **chưa có y lệnh cha** (`PARENT_ID = null`) của điều trị.
  - Cột: Chọn, Mã y lệnh (`TDL_SERVICE_REQ_CODE`), Barcode (`HIS_SERVICE_REQ.BARCODE`), Mã dịch vụ (`TDL_SERVICE_CODE`), Tên dịch vụ (`TDL_SERVICE_NAME`), Số lượng (`AMOUNT`).
  - Tìm kiếm: ô Barcode (khớp chính xác), ô từ khóa (mã DV / tên DV / mã y lệnh).
  - Nút **Chọn (Ctrl S)** trả về danh sách tích chọn.
- Gom nhóm theo thiết lập `HIS_SERVICE_FOLLOW` (`SERVICE_ID` túi máu → `FOLLOW_ID` DV đi kèm):
  - **Chọn y lệnh chung** (nút): duyệt từng loại máu; KHÔNG thiết lập → bỏ qua; CÓ thiết lập → chỉ gắn DV đi kèm khớp `FOLLOW_ID`.
  - **Chọn từng dịch vụ** (chuột phải): theo DV túi máu đang chọn; KHÔNG thiết lập → gắn toàn bộ DV đã tích; CÓ thiết lập → chỉ gắn DV đi kèm khớp `FOLLOW_ID`.
  - Ví dụ: loại máu M02 thiết lập đi kèm XN01..XN05; y lệnh có XN03, XN05, XN10, XN15 → chỉ gắn XN03, XN05 vào M02.
- Kết quả lưu tạm vào `VHisBloodADO.AttachSereServIds` (hiển thị tên ở cột "Dịch vụ xét nghiệm" của grid túi máu). Khi Duyệt, gửi qua `ExpBloodSDO.AttackSereServIds`.

### Điều kiện nghiệp vụ
- Nút/menu đính kèm chỉ hiện khi grid kho máu có dữ liệu và phiếu có `TDL_TREATMENT_ID` hợp lệ.
- Phải có ít nhất một túi máu trong danh sách xuất trước khi đính kèm (chế độ chung).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST_BLTY_REQ_1 | View | Y lệnh máu (currentBlty) |
| V_HIS_BLOOD | View | Túi máu trong kho |
| HIS_EXP_MEST_BLOOD | Table | Túi máu đã xuất (BLOOD_ID, SERE_SERV_PARENT_ID, TDL_SERVICE_REQ_ID) |
| HIS_SERVICE_REQ / V_HIS_SERVICE_REQ | Table/View | Y lệnh dịch vụ (BARCODE, SERVICE_REQ_TYPE_ID, PARENT_ID) |
| HIS_SERE_SERV | Table | Dịch vụ thực hiện (TDL_SERVICE_REQ_CODE/CODE/NAME, AMOUNT, SERVICE_REQ_ID, **BLOOD_ID**) |
| HIS_SERVICE_FOLLOW / V_HIS_SERVICE_FOLLOW | Table/View | Thiết lập dịch vụ đi kèm (SERVICE_ID → FOLLOW_ID) |

### Quan hệ chính
- Túi máu (V_HIS_BLOOD.SERVICE_ID) → HIS_SERVICE_FOLLOW.SERVICE_ID → FOLLOW_ID (dịch vụ XN đi kèm).
- Đính kèm: HIS_SERE_SERV.BLOOD_ID = ID túi máu (backend gán khi Duyệt qua `ExpBloodSDO.AttackSereServIds`).

## 4. UI Layout

```
+-------------------------------------------------------------------+
| Tabs: Thuốc | Vật tư | Máu | Vật tư đích danh - tái sử dụng       |
+-------------------------------------------------------------------+
| [Y lệnh máu - grid trái]        | [Túi máu đã chọn - grid phải]   |
|                                 |  ...cột... | Dịch vụ xét nghiệm |
|                                 |  (chuột phải: Đính kèm DV XN)   |
| [Kho máu - grid dưới + lọc]                                       |
+-------------------------------------------------------------------+
| [Dịch vụ xét nghiệm] [Chỉ định DVKT] [Tủ trực] [In truyền máu]   |
| [Thực xuất] ... [Lưu (Ctrl S)] [In ấn]                           |
+-------------------------------------------------------------------+
```

### Form phụ
| Form | Mục đích |
|------|----------|
| frmAttachTestService | Popup đính kèm dịch vụ xét nghiệm vào túi máu (grid check + tìm barcode/từ khóa) |

## 5. API Endpoints

| Action | URI | Consumer | Filter/SDO |
|--------|-----|----------|------------|
| Duyệt phiếu | api/HisExpMest/Approve | MosConsumer | HisExpMestApproveSDO (Bloods → ExpBloodSDO.AttackSereServIds) |
| Thực xuất | api/HisExpMest/Export | MosConsumer | HisExpMestExportSDO |
| Y lệnh DV (popup) | api/HisServiceReq/GetView | MosConsumer | HisServiceReqViewFilter (TREATMENT_ID, SERVICE_REQ_TYPE_IDs) |
| DV thực hiện (popup) | api/HisSereServ/Get | MosConsumer | HisSereServFilter (SERVICE_REQ_IDs / TREATMENT_ID) |
| Thiết lập DV đi kèm | api/HisServiceFollow/GetView | MosConsumer | HisServiceFollowViewFilter (SERVICE_IDs) |
| Túi máu đã xuất (in) | api/HisExpMestBlood/GetView | MosConsumer | HisExpMestBloodViewFilter (EXP_MEST_ID) |

## 6. Dependencies

### Inter-Plugin
| Plugin đích | Khi nào mở | Args truyền |
|-------------|-----------|-------------|
| HIS.Desktop.Plugins.AssignService | Nút "Chỉ định DVKT" | TreatmentID, AssignServiceADO |
| HIS.Desktop.Plugins.AssignPrescriptionPK | Nút "Tủ trực" | AssignPrescriptionADO |
| frmAttachTestService (nội bộ) | Nút / chuột phải "Dịch vụ xét nghiệm" | treatmentId, preCheckedSereServIds |

## 7. Print

| Loại in | PrintTypeCode | MPS | Template |
|---------|--------------|-----|----------|
| Phiếu xuất máu | Mps000107 | MPS.Processor.Mps000107 (FlexCel/Excel) | mps107 .xlsx (SAR) |

> Việc in barcode y lệnh XN lên Phiếu xuất máu (key `<#BARCODE;>`) thuộc **yêu cầu 2.3.3** — KHÔNG thuộc phạm vi công việc 2.3.2 hiện tại, **chưa triển khai** (đã revert). Chức năng 2.3.2 không thay đổi luồng in.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 17/06/2026 | tuanln | 2.3.2 — Bổ sung chức năng "Đính kèm dịch vụ xét nghiệm" vào túi máu (nút + menu chuột phải + popup frmAttachTestService), gom nhóm theo HIS_SERVICE_FOLLOW, lưu qua ExpBloodSDO.AttackSereServIds. |
| 19/06/2026 | tuanln | Thu hẹp về đúng phạm vi 2.3.2: revert toàn bộ phần in mps107 (2.3.3 — Processor/PDO/ExpMestViewDetail) về bản gốc. 2.3.3 KHÔNG thuộc công việc hiện tại. |
| 23/06/2026 | tuanln | Popup frmAttachTestService: thêm **checkbox "chọn tất cả" ở header cột Chọn** (CustomDrawColumnHeader vẽ checkbox + MouseDown hit-test header gcCheck → đảo IsCheck toàn bộ dòng đang hiển thị; CellValueChanged đồng bộ trạng thái header; tắt sort cột Chọn). |

## 9. Test Cases

### Đính kèm DV xét nghiệm
- [ ] Chưa có túi máu → nhấn "Dịch vụ xét nghiệm" → cảnh báo cần chọn túi máu.
- [ ] Popup chỉ hiện DV loại XN (type=2) chưa có y lệnh cha của điều trị.
- [ ] Tìm theo barcode (chính xác) và từ khóa (mã/tên/mã y lệnh) đúng.
- [ ] Nút (chung): loại máu CÓ thiết lập follow → chỉ gắn DV khớp; KHÔNG thiết lập → bỏ qua.
- [ ] Chuột phải (từng): loại máu CÓ thiết lập → chỉ gắn DV khớp; KHÔNG thiết lập → gắn toàn bộ DV đã tích.
- [ ] Cột "Dịch vụ xét nghiệm" hiển thị đúng tên DV đã gắn theo túi máu.
- [ ] Duyệt phiếu → ExpBloodSDO.AttackSereServIds chứa đúng sere_serv id theo từng túi máu.

### In mps107
- [ ] In Phiếu xuất máu → biểu in hiển thị thông tin y lệnh xét nghiệm đã đính kèm (sau khi cập nhật template).
