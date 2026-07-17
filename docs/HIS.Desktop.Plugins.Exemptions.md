# Miễn Giảm Viện Phí (Exemptions) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.Exemptions |
| Loại | Form (frmExemptions, kế thừa FormBase) |
| Mục đích | Miễn giảm / chiết khấu viện phí theo từng dịch vụ (HIS_SERE_SERV) của 1 hồ sơ điều trị. Hỗ trợ tự động miễn giảm theo tỷ lệ cho toàn bộ chỉ định và (tùy cấu hình) nhiều dòng chiết khấu trên 1 dịch vụ. |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. Mở form theo `treatmentId` hoặc theo 1 `HIS_SERE_SERV` cụ thể (3 constructor).
2. Load thông tin bệnh nhân + thẻ BHYT (`V_HIS_PATIENT_TYPE_ALTER`).
3. Load cây dịch vụ `V_HIS_SERE_SERV_5` (group theo Đối tượng → Loại dịch vụ → Dịch vụ).
4. Người dùng nhập tỷ lệ miễn giảm (%) + nhấn "Đồng ý" để áp cho các dịch vụ đang tích chọn; hoặc nhập trực tiếp số tiền chiết khấu vào ô "Chiết khấu".
5. Nhấn **Lưu** → gọi `api/HisSereServ/UpdateDiscountList` với `HisSereServDiscountSDO`.

### Chế độ đa chiết khấu (key `MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT`)
- **TẮT** (mặc định): giữ luồng cũ; bổ sung cột **"Chiết khấu (%)"** chỉ đọc = (Chiết khấu / Bệnh nhân chi trả) × 100.
- **BẬT**: mỗi dịch vụ "mở rộng" ra các dòng chiết khấu (`HIS_SERE_SERV_DISCOUNT`):
  - Ô "Chiết khấu" và "Lý do miễn giảm" ở dòng dịch vụ: không cho sửa / để trống.
  - Nút **"+"** ở ô Chiết khấu của dịch vụ: thêm 1 dòng chiết khấu mới (chỉ thực sự tạo khi nhấn Lưu).
  - Dòng chiết khấu: cho nhập **Chiết khấu**, **Chiết khấu (%)**, **Lý do** (≤ 250 ký tự).
    - Nhập Chiết khấu (%) → Chiết khấu = (% × Bệnh nhân chi trả) / 100.
    - Nhập Chiết khấu → Chiết khấu (%) = (Chiết khấu / Bệnh nhân chi trả) × 100.
  - Nút **"X"** trên dòng chiết khấu: xóa dòng; nếu dòng đã có ID → gọi `api/HisSereServDiscount/Delete` truyền ID.
  - Ô Chiết khấu của dịch vụ = tổng tiền các dòng chiết khấu con.

### Điều kiện nghiệp vụ
- Hồ sơ đã khóa viện phí (`IS_LOCK_FEE = 1`) → không cho lưu.
- Tổng chiết khấu của 1 dịch vụ không được vượt `VIR_TOTAL_PATIENT_PRICE_NO_DC` (bệnh nhân phải trả).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT | Table | Hồ sơ điều trị (IS_AUTO_DISCOUNT, AUTO_DISCOUNT_RATIO, DISCOUNT_REASON, IS_LOCK_FEE) |
| V_HIS_SERE_SERV_5 | View | Dịch vụ đã thực hiện (DISCOUNT, VIR_TOTAL_PATIENT_PRICE_NO_DC, IS_EXPEND...) |
| HIS_SERE_SERV | Table | Cập nhật DISCOUNT, DISCOUNT_TIME |
| HIS_SERE_SERV_DISCOUNT | Table | Bản ghi chiết khấu nhiều dòng/dịch vụ (ID, SERE_SERV_ID, TREATMENT_ID, DISCOUNT, DISCOUNT_RATIO[long], REASON) |
| HIS_SERE_SERV_BILL | Table | Loại trừ dịch vụ đã lên hóa đơn |
| V_HIS_PATIENT_TYPE_ALTER | View | Thông tin thẻ BHYT |

## 4. UI Layout

```
+--------------------------------------------------------------------------+
| [Mã hồ sơ] [Tìm] | Thông tin BN / Thẻ BHYT                               |
+--------------------------------------------------------------------------+
| Tỷ lệ MG (%) [__] [Đồng ý]   Ngày TH [__]   Lý do MG [______]            |
+--------------------------------------------------------------------------+
| TreeList trvService:                                                     |
|  Tên DV | SL | Đơn giá | Thành tiền | Đồng chi trả | BN chi trả |        |
|  Chiết khấu (+) | Chiết khấu (%) | Hao phí | VAT | Mã DV | Mã YC |       |
|  Người TH | Lý do MG | Ngày TH                                           |
|   └ (đa chiết khấu) dòng con: Chiết khấu (X) | Chiết khấu (%) | Lý do     |
+--------------------------------------------------------------------------+
| [☑ Tự động MG các chỉ định] Tỷ lệ [__]  Lý do [____]   [Lưu] [Mới]       |
+--------------------------------------------------------------------------+
```

Cột "Chiết khấu (%)" (VisibleIndex 7) nằm ngay sau cột "Chiết khấu". Repository:
`repoBtnAddDiscount` (nút +), `repoSpinDiscountRow` (nút X), `repoSpinRatioRow`, `repoTextReason` (max 250).

## 5. API Endpoints

| Action | URI | Consumer | Filter/Body |
|--------|-----|----------|-------------|
| Lấy DV | api/HisSereServ/GetView5 | MosConsumer | HisSereServView5Filter |
| Lấy hóa đơn DV | api/HisSereServBill/Get | MosConsumer | HisSereServBillFilter |
| Lấy hồ sơ | api/HisTreatment/Get | MosConsumer | HisTreatmentFilter |
| Lấy thẻ BHYT | (HIS_PATIENT_TYPE_ALTER_GET_APPLIED) | MosConsumer | HisPatientTypeAlterViewAppliedFilter |
| **Lấy chiết khấu** | api/HisSereServDiscount/Get | MosConsumer | HisSereServDiscountFilterADO (+TREATMENT_ID) |
| **Xóa chiết khấu** | api/HisSereServDiscount/Delete | MosConsumer | long ID |
| **Lưu** | api/HisSereServ/UpdateDiscountList | MosConsumer | HisSereServDiscountSDO (HisSereServs + HisSereServDiscounts) |

`RequestUriStore.cs` giữ 3 hằng URI chiết khấu. `HisSereServDiscountFilterADO` kế thừa `MOS.Filter.HisSereServDiscountFilter` + `TREATMENT_ID`/`SERE_SERV_ID` (MOS.Filter chưa khai báo, backend đọc qua JSON).

## 6. Dependencies

Không dùng Library/inter-plugin riêng. Cấu hình toàn viện:
- `MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT` → đối tượng BHYT.
- `MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT` → bật đa chiết khấu (HisConfigCFG.EnableMultiDiscount).

## 7. Print
Không có.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 29/05/2026 | huannh | Bổ sung cột "Chiết khấu (%)" (read-only). Bổ sung chế độ đa chiết khấu theo key `MOS.HIS_TRANSACTION_ENABLE_MULTI_DISCOUNT`: dòng chiết khấu con (HIS_SERE_SERV_DISCOUNT) với nút +/X, tự tính Chiết khấu ↔ %, tổng chiết khấu dịch vụ, truyền `HisSereServDiscounts` qua UpdateDiscountList, xóa qua HisSereServDiscount/Delete. Thêm file RequestUriStore.cs, ADO/HisSereServDiscountFilterADO.cs, frmExemptions__MultiDiscount.cs. |

## 9. Test Cases

### Key TẮT (mặc định)
- [ ] Mở form → cột "Chiết khấu (%)" hiển thị đúng = (Chiết khấu/BN chi trả)×100, không sửa được.
- [ ] Nhập tỷ lệ + Đồng ý → ô Chiết khấu cập nhật; Lưu thành công.

### Key BẬT
- [ ] Mỗi dịch vụ expand ra dòng chiết khấu đã lưu (nếu có).
- [ ] Nút "+" → thêm dòng chiết khấu mới dưới dịch vụ.
- [ ] Nhập Chiết khấu (%) → cột Chiết khấu tự tính (và ngược lại).
- [ ] Ô Chiết khấu dịch vụ = tổng các dòng con.
- [ ] Nút "X" dòng đã lưu → confirm → gọi Delete API → xóa khỏi cây + tính lại tổng.
- [ ] Nút "X" dòng mới (chưa lưu) → xóa khỏi cây, không gọi API.
- [ ] Lý do > 250 ký tự → không cho nhập.
- [ ] Lưu → SDO có HisSereServDiscounts đúng (ID cũ giữ, mới = 0; DISCOUNT, DISCOUNT_RATIO, REASON, TREATMENT_ID, SERE_SERV_ID).
- [ ] Cột Chiết khấu + Lý do ở dòng dịch vụ không sửa được / để trống.
