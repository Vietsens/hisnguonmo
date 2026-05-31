# Bug Report: `HisSereServ/UpdatePayslipInfo` không persist `PATIENT_PACKAGE_ID` xuống DB

**Mức độ nghiêm trọng**: 🔴 **CRITICAL** — Toàn bộ luồng nghiệp vụ PTTK 2663 mục 6.2 (gán gói BN trong Bảng kê) **không hoạt động** vì column key chưa được lưu.

**Module ảnh hưởng**: Backend MOS — service xử lý `api/HisSereServ/UpdatePayslipInfo`
**Plugin frontend bị block**: `HIS.Desktop.Plugins.Bordereau` (Bảng kê thanh toán)
**PTTK liên quan**: 2663 mục 3.2, 4.2, 6.2

---

## 1. Mô tả hiện tượng

API `api/HisSereServ/UpdatePayslipInfo` khi nhận `Field = UpdateField.PATIENT_PACKAGE_ID` (= 16):
- ✅ Trả `Success = true` (báo xử lý thành công)
- ❌ **Không** UPDATE giá trị xuống `HIS_SERE_SERV.PATIENT_PACKAGE_ID` trong DB
- ❌ Không tính lại giá theo logic PTTK 4.2

Hệ quả: User thấy UI hiển thị đúng (do FE tự cập nhật ADO), nhưng đóng form / refresh / re-query → giá trị mất sạch.

---

## 2. Các bước tái hiện

| # | Thao tác | Kết quả thực tế | Kết quả mong đợi (per PTTK) |
|---|----------|-----------------|-----------------------------|
| 1 | Mở Bảng kê thanh toán cho 1 BN có gói đã đăng ký | Form load OK | OK |
| 2 | Chọn 1 gói (VD: "Gói 11") cho 1 dòng dịch vụ | Toast "Xử lý thành công", cell hiển thị "Gói 11" | OK |
| 3 | **Query DB ngay sau bước 2** | `HIS_SERE_SERV.PATIENT_PACKAGE_ID = NULL` ❌ | `= 58` (ID của Gói 11) ✅ |
| 4 | Đóng form → mở lại Bảng kê | Cell "Gói bệnh nhân" **rỗng** ❌ | Hiển thị "Gói 11" ✅ |
| 5 | Verify giá DV trong DB | `HIS_SERE_SERV.PRICE` không đổi (vẫn theo CSG) ❌ | `PRICE = HIS_PATIENT_PACKAGE_DT.UNIT_PRICE` (PTTK 4.2) ✅ |

---

## 3. Bằng chứng — Log gọi API

### Request từ FE (đã verify qua `LogSystem.txt`)

```
WARN 2026-05-31 16:46:33,559 [1] - Call API "http://192.168.1.201:8660//api/HisSereServ/UpdatePayslipInfo":
____TraceInfo: [Class: HIS.Desktop.Plugins.Bordereau.frmBordereau; MethodName: UpdatePayslipInfoProcess]
____InputData: ___filterOrInputData:{
  "TreatmentId": 163203,
  "SereServs": [{
    "ID": 1690363,
    "PATIENT_PACKAGE_ID": 44,        ← FE GỬI ĐÚNG GIÁ TRỊ
    "SERVICE_ID": 5208,
    ... (các field khác đầy đủ) ...
  }],
  "Field": 16                          ← UpdateField.PATIENT_PACKAGE_ID
}___
```

→ Frontend đã gửi **đúng** `Field = 16` (`PATIENT_PACKAGE_ID` enum) + `SereServs[0].PATIENT_PACKAGE_ID = 44`.

### Response từ Backend

Backend trả về **Success = true** (cho case DV nằm trong gói — tình huống chính), tức backend **chấp nhận** xử lý field này.

### SQL verify sau request

```sql
SELECT ID, PATIENT_PACKAGE_ID, PRICE, PRIMARY_PRICE, ORIGINAL_PRICE
FROM HIS_SERE_SERV
WHERE ID = 1690363;
```

**Kết quả thực tế:**
| ID | PATIENT_PACKAGE_ID | PRICE | PRIMARY_PRICE | ORIGINAL_PRICE |
|----|--------------------|-------|---------------|----------------|
| 1690363 | **NULL** ❌ | (giá CSG cũ) | (giá CSG cũ) | (giá CSG cũ) |

**Kết quả mong đợi (per PTTK 2663 mục 4.2):**
| ID | PATIENT_PACKAGE_ID | PRICE | PRIMARY_PRICE | ORIGINAL_PRICE |
|----|--------------------|-------|---------------|----------------|
| 1690363 | **44** ✅ | (UNIT_PRICE từ HIS_PATIENT_PACKAGE_DT) | (như PRICE) | (như PRICE) |

---

## 4. Trích PTTK 2663 — Yêu cầu backend phải làm

### Mục 3.2 — Schema HIS_SERE_SERV bổ sung cột

> **HIS_SERE_SERV — 2 cột song song với cơ chế Nguồn khác**
>
> | Cột mới (Gói) | Kiểu | Mô tả |
> |---------------|------|-------|
> | `IS_PATIENT_PACKAGE_PAID` | NUMBER(1) | = 1: dòng DV này đã được gói chi trả |
> | **`PATIENT_PACKAGE_ID`** | **NUMBER(19)** | **FK → HIS_PATIENT_PACKAGE — đánh dấu DV thuộc gói nào** |

→ Column `PATIENT_PACKAGE_ID` **phải tồn tại** trong bảng `HIS_SERE_SERV` và **phải được UPDATE** khi API ghi nhận.

### Mục 4.2 — Logic backend tính giá

> **Xử lý xác định giá**
> - Trường hợp dịch vụ (HIS_SERE_SERV) có thông tin gói bệnh nhân (PATIENT_PACKAGE_ID khác null) thực hiện tính giá riêng không tính theo chính sách giá (HIS_SERVICE_PATY) và thời gian y lệnh
> - Thông tin giá (PRICE, PRIMARY_PRICE, ORIGINAL_PRICE) đều bằng UNIT_PRICE trong HIS_PATIENT_PACKAGE_DT có PATIENT_PACKAGE_ID và SERVICE_ID trong HIS_SERE_SERV.

→ Điều kiện *"PATIENT_PACKAGE_ID khác null"* **chỉ check được nếu column đã được persist**. Nếu không lưu, logic 4.2 **vĩnh viễn không bao giờ trigger** — toàn bộ tính năng gói BN vô tác dụng.

### Mục 6.2 — Yêu cầu trực tiếp cho API

> Thực hiện gọi api cập nhật giá như hiện tại (HisSereServ/UpdatePayslipInfo) **truyền vào thông tin cập nhật là PATIENT_PACKAGE_ID và giá trị tương ứng**.
>
> Lưu ý: **toàn bộ việc tính toán tiền sẽ do API xử lý**.

→ PTTK **nêu đích danh API** này phải xử lý `PATIENT_PACKAGE_ID` (gồm: lưu + tính lại giá).

---

## 5. Backend dev cần kiểm tra

### 5.1 Switch/if-else xử lý `UpdateField` enum

Trong service handler của `HisSereServ/UpdatePayslipInfo`, tìm chỗ switch theo `dto.Field`:

```csharp
// Backend pseudo-code
switch (dto.Field)
{
    case UpdateField.PATIENT_TYPE_ID: ... break;
    case UpdateField.OTHER_PAY_SOURCE_ID: ... break;
    case UpdateField.IS_FUND_ACCEPTED: ... break;
    // ...
    case UpdateField.PATIENT_PACKAGE_ID:
        // ← BLOCK NÀY CÓ EXIST KHÔNG? CÓ UPDATE DB KHÔNG?
        // Phải có:
        //   1. UPDATE HIS_SERE_SERV SET PATIENT_PACKAGE_ID = dto.SereServs[i].PATIENT_PACKAGE_ID WHERE ID = ...
        //   2. Tính lại PRICE, PRIMARY_PRICE, ORIGINAL_PRICE theo HIS_PATIENT_PACKAGE_DT
        //   3. SaveChanges()
        break;
}
```

### 5.2 EF Entity mapping

Kiểm tra `HIS_SERE_SERV` entity (`MOS.EFMODEL`):
- Property `PATIENT_PACKAGE_ID` đã được khai báo chưa?
- Đã được `[Column("PATIENT_PACKAGE_ID")]` map đúng tên column DB chưa?
- Migration đã chạy chưa? (`ALTER TABLE HIS_SERE_SERV ADD PATIENT_PACKAGE_ID NUMBER(19)`)

### 5.3 SaveChanges / Commit

- Đảm bảo `dbContext.SaveChanges()` (hoặc transaction commit) được gọi sau UPDATE.
- Không bị catch-and-swallow exception giấu lỗi DB.

### 5.4 Trigger tính giá

- Theo PTTK 4.2, khi `PATIENT_PACKAGE_ID != null` → ghi `PRICE = HIS_PATIENT_PACKAGE_DT.UNIT_PRICE`.
- Verify trigger này đã được implement và chạy trong cùng transaction.

---

## 6. Test case cho backend tự verify

```sql
-- 1. Pick 1 sere_serv_id và 1 patient_package_id để test
DECLARE
    v_ss_id NUMBER := <SERE_SERV_ID>;
    v_pkg_id NUMBER := <PATIENT_PACKAGE_ID có DV này trong DT>;

-- 2. Trước khi gọi API
SELECT PATIENT_PACKAGE_ID, PRICE FROM HIS_SERE_SERV WHERE ID = v_ss_id;
-- Expected: NULL, <giá CSG>

-- 3. Gọi API HisSereServ/UpdatePayslipInfo (qua Postman/curl):
-- POST /api/HisSereServ/UpdatePayslipInfo
-- {
--   "TreatmentId": <treatment_id>,
--   "Field": 16,
--   "SereServs": [{ "ID": <v_ss_id>, "PATIENT_PACKAGE_ID": <v_pkg_id>, ... }]
-- }
-- Response expected: { "Success": true, "Data": [...] }

-- 4. Sau khi gọi
SELECT PATIENT_PACKAGE_ID, PRICE FROM HIS_SERE_SERV WHERE ID = v_ss_id;
-- Expected per PTTK 4.2:
--   PATIENT_PACKAGE_ID = v_pkg_id
--   PRICE = (SELECT UNIT_PRICE FROM HIS_PATIENT_PACKAGE_DT
--           WHERE PATIENT_PACKAGE_ID = v_pkg_id
--             AND SERVICE_ID = (SELECT SERVICE_ID FROM HIS_SERE_SERV WHERE ID = v_ss_id))
--
-- Actual hiện tại: PATIENT_PACKAGE_ID vẫn NULL, PRICE vẫn giá cũ → BUG.
```

---

## 7. Kết luận

| Tiêu chí | Status |
|----------|--------|
| Frontend gửi đúng `Field` + `PATIENT_PACKAGE_ID` | ✅ Verify qua log |
| Backend trả `Success = true` | ✅ Verify qua log |
| DB `HIS_SERE_SERV.PATIENT_PACKAGE_ID` được persist | ❌ **NULL sau khi save** |
| Giá `PRICE/PRIMARY_PRICE/ORIGINAL_PRICE` tính lại theo gói | ❌ **Không thay đổi** |
| Reload form thấy giá trị | ❌ **Mất** |

→ Backend **chưa implement** (hoặc implement thiếu) nhánh `UpdateField.PATIENT_PACKAGE_ID` trong service `UpdatePayslipInfo`. Đây là vi phạm trực tiếp PTTK 2663 mục 6.2 + 4.2.

## 8. Việc cần backend dev làm

| # | Việc | Tham chiếu |
|---|------|-----------|
| 1 | Thêm case `UpdateField.PATIENT_PACKAGE_ID` trong switch xử lý của `UpdatePayslipInfo` service | PTTK 6.2 |
| 2 | UPDATE `HIS_SERE_SERV.PATIENT_PACKAGE_ID` theo input | PTTK 3.2 |
| 3 | Sau khi UPDATE → tính lại `PRICE`, `PRIMARY_PRICE`, `ORIGINAL_PRICE` theo `HIS_PATIENT_PACKAGE_DT.UNIT_PRICE` (join qua PATIENT_PACKAGE_ID + SERVICE_ID) | PTTK 4.2 |
| 4 | Commit transaction | Best practice |
| 5 | Trả về `HIS_SERE_SERV` list đã update (gồm cả `PATIENT_PACKAGE_ID` và giá mới) trong response để FE refresh đúng | API contract |
| 6 | Test case: chạy SQL section 6 sau API call, verify column được lưu | Verification |

---

**Người báo cáo**: sinhnt (frontend) — `HIS.Desktop.Plugins.Bordereau`
**Ngày phát hiện**: 2026-05-31
**Backend dev liên quan**: NAMPP (theo task vcong 45615)
