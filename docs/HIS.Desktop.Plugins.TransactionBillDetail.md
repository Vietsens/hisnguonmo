# HIS.Desktop.Plugins.TransactionBillDetail — Tài Liệu Module

## 1. Tổng Quan
| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionBillDetail |
| Loại | Form (FormBase) |
| Mục đích | Xem chi tiết 1 giao dịch thanh toán (cây dịch vụ) + (4.2.6) xem/sửa danh sách hình thức thanh toán của giao dịch |
| Trạng thái | Đang sử dụng |

## 2. Quy Trình Nghiệp Vụ
- Mở từ danh sách giao dịch với `billId = TRANSACTION_ID`. Form hiển thị cây dịch vụ của giao dịch (HIS_SERE_SERV_BILL theo BILL_ID).
- **4.2.6 — Hình thức thanh toán:** nhúng UC `HIS.UC.TransactionPayformGrid` dưới cây dịch vụ:
  - Khi mở: GET danh sách hình thức đã dùng của giao dịch → điền sẵn vào lưới UC.
  - Sửa hình thức (hình thức/ngân hàng/số tiền/loại tiền/tỉ giá/phụ phí/thành tiền) trực tiếp trên lưới.
  - Bấm nút **"Lưu hình thức thanh toán"** → đọc data từ UC → POST cập nhật (không mở popup).

## 3. EFMODEL / SDO Sử Dụng
| Type | Loại | Mục đích |
|------|------|----------|
| V_HIS_TRANSACTION | View | Giao dịch (đã có) |
| HIS_SERE_SERV_BILL / V_HIS_SERE_SERV_5 | View/Table | Cây dịch vụ (đã có) |
| HIS_TRANSACTION_PAYFORM | Table | Hình thức thanh toán của giao dịch (1-n với HIS_TRANSACTION) |
| MOS.SDO.HisTransactionUpdatePayformDetailsSDO | SDO | Body lưu: `{ long TransactionId; long RequestRoomId; List<PayformDetailSDO> PayformDetails }` |
| MOS.SDO.PayformDetailSDO | SDO | 1 dòng hình thức |
| MOS.Filter.HisTransactionPayformFilter | Filter | Lọc payform theo `TRANSACTION_ID` |

## 4. UI Layout
```
layoutControl1 / layoutControlGroup1
 ├─ layoutControlItem1  → panelControlSereServTree (cây dịch vụ, fill phía trên)
 ├─ lciPayformGrid      → UC HIS.UC.TransactionPayformGrid (cao 110)   ← MỚI
 └─ lciBtnSavePayform   → SimpleButton "Lưu hình thức thanh toán" (căn phải) ← MỚI
```
UC + nút nhúng bằng CODE qua `layoutControlGroup1.AddItem` (file `frmTransactionBillDetail__Plus__PayformGrid.cs`), KHÔNG sửa Designer.cs.

## 5. API Endpoints
| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Lấy hình thức đã dùng | api/HisTransactionPayform/Get | MosConsumer | HisTransactionPayformFilter (TRANSACTION_ID = billId) |
| Lưu hình thức | api/HisTransaction/UpdatePayformDetails | MosConsumer | HisTransactionUpdatePayformDetailsSDO |

## 6. Dependencies
| UC | Mục đích |
|----|----------|
| HIS.UC.TransactionPayformGrid | Lưới hình thức thanh toán (Run/Reload/GetData/SetRequiredAmount) |
| HIS.UC.SereServTree | Cây dịch vụ (đã có) |

## 8. Changelog
| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/06/2026 | anhnh2@vietsens.vn | **4.2.6** — Nhúng UC `HIS.UC.TransactionPayformGrid` + nút "Lưu hình thức thanh toán". File mới `frmTransactionBillDetail__Plus__PayformGrid.cs`: `InitPayformGrid()` (constructor, sau InitSereServTree — Run UC + nút, AddItem vào layoutControlGroup1), `LoadPayformData()` (Load, sau FillData — GET `api/HisTransactionPayform/Get` theo TRANSACTION_ID=billId → map → Reload), `btnSavePayform_Click` (GetData → `HisTransactionUpdatePayformDetailsSDO` → POST `api/HisTransaction/UpdatePayformDetails` → MessageManager.Show → reload). csproj: ref `HIS.UC.TransactionPayformGrid` + include file mới. Resources: thêm key LBL_PAYFORM/BTN_SAVE_PAYFORM (vi/en). |

## 9. Test Cases
- [ ] Mở chi tiết giao dịch → lưới hình thức hiện đúng các dòng đã dùng.
- [ ] Sửa/thêm/xóa dòng → bấm "Lưu hình thức thanh toán" → thông báo thành công, mở lại đúng dữ liệu đã lưu.
- [ ] Giao dịch chưa có hình thức → lưới trống, thêm dòng + lưu OK.
