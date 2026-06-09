# HIS.Desktop.Plugins.TransactionDepositDetail — Tài Liệu Module

## 1. Tổng Quan
| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.TransactionDepositDetail |
| Loại | Form (FormBase) |
| Mục đích | Xem chi tiết phiếu tạm ứng (cây dịch vụ) + (4.2.7) xem/sửa hình thức thanh toán của giao dịch tạm ứng |
| Trạng thái | Đang sử dụng |

## 2. Quy Trình Nghiệp Vụ
- Mở với `billId = DEPOSIT_ID`. Form hiển thị cây dịch vụ trong phiếu tạm ứng (V_HIS_SERE_SERV_DEPOSIT theo DEPOSIT_ID).
- **4.2.7 — Tương tự 4.2.6:** nhúng UC `HIS.UC.TransactionPayformGrid`:
  - Khi mở: GET hình thức đã dùng của giao dịch → điền sẵn vào UC.
  - Bấm **"Lưu hình thức thanh toán"** → POST cập nhật.
- ⚠️ **Lưu ý nguồn ID:** form nhận `billId = DEPOSIT_ID`. Code dùng billId làm `TRANSACTION_ID` cho API payform (giả định DEPOSIT_ID = transaction id của giao dịch tạm ứng). **Cần xác nhận với backend/người giao việc**; nếu khác, đổi nguồn id truyền vào `HisTransactionPayformFilter.TRANSACTION_ID` + `HisTransactionUpdatePayformDetailsSDO.TransactionId`.

## 3. EFMODEL / SDO Sử Dụng
| Type | Loại | Mục đích |
|------|------|----------|
| V_HIS_SERE_SERV_DEPOSIT | View | Cây dịch vụ tạm ứng (đã có) |
| HIS_TRANSACTION_PAYFORM | Table | Hình thức thanh toán của giao dịch |
| MOS.SDO.HisTransactionUpdatePayformDetailsSDO / PayformDetailSDO | SDO | Body lưu hình thức |
| MOS.Filter.HisTransactionPayformFilter | Filter | Lọc theo TRANSACTION_ID |

## 4. UI Layout
Form không có LayoutControl — chỉ `panelControlSereServTree` (Dock=Fill). Nhúng UC bằng **docking** (CODE, file `frmTransactionDepositDetail__Plus__PayformGrid.cs`):
```
[panelControlSereServTree]  ← cây dịch vụ (BringToFront → fill phần trên)
[pnlPayformHost (Dock=Bottom, 150)]
   ├─ ucPayform (Dock=Fill)
   └─ pnlPayformFooter (Dock=Bottom) → btnSavePayform "Lưu hình thức thanh toán" (Dock=Right)
```

## 5. API Endpoints
| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Lấy hình thức đã dùng | api/HisTransactionPayform/Get | MosConsumer | HisTransactionPayformFilter (TRANSACTION_ID = billId) |
| Lưu hình thức | api/HisTransaction/UpdatePayformDetails | MosConsumer | HisTransactionUpdatePayformDetailsSDO |

## 6. Dependencies
| UC | Mục đích |
|----|----------|
| HIS.UC.TransactionPayformGrid | Lưới hình thức thanh toán |
| HIS.UC.SereServTree | Cây dịch vụ (đã có) |

## 8. Changelog
| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 06/06/2026 | anhnh2@vietsens.vn | **4.2.7** (tương tự 4.2.6) — Nhúng UC `HIS.UC.TransactionPayformGrid` + nút "Lưu hình thức thanh toán". File mới `frmTransactionDepositDetail__Plus__PayformGrid.cs`: nhúng bằng docking (form không có LayoutControl), `InitPayformGrid()` (constructor), `LoadPayformData()` (Load — GET `api/HisTransactionPayform/Get` theo TRANSACTION_ID=billId), `btnSavePayform_Click` (POST `api/HisTransaction/UpdatePayformDetails`). csproj: thêm `MOS.SDO` + ref `HIS.UC.TransactionPayformGrid` + include file mới. Caption literal (plugin chưa có Resources). ⚠️ billId = DEPOSIT_ID — cần xác nhận = TRANSACTION_ID. |

## 9. Test Cases
- [ ] Mở chi tiết tạm ứng → lưới hình thức hiện dòng đã dùng.
- [ ] Sửa + "Lưu hình thức thanh toán" → thành công, reload đúng.
- [ ] Xác nhận billId(DEPOSIT_ID) đúng là TRANSACTION_ID khi lưu (không lưu nhầm giao dịch khác).
