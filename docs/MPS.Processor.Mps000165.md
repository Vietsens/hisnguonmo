# MPS000165 — Phiếu xuất khác — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Mã in | MPS000165 |
| Project | `MPS.Processor.Mps000165` + `MPS.Processor.Mps000165.PDO` (bản đang dùng — **không** phải bản cũ `MPS\MPS\Old\Core\Mps000165`) |
| Loại | MPS Processor (template Excel FlexCel) |
| Template mẫu | `Mps000165_PhieuXuatKhac___A4D_001.xlsx` (viện tự dựng thêm mẫu C31-HD "Phiếu xuất kho cho trạm y tế") |
| Mục đích | In phiếu xuất khác (loại xuất `HIS_EXP_MEST_TYPE.ID__KHAC`) cho **thuốc, vật tư và máu** |
| Nơi gọi | `HIS.Desktop.Plugins.ExpMestOtherExport` (màn Xuất khác), `HIS.Desktop.Plugins.ExpMestViewDetail` (form Chi tiết xuất) |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

Plugin dựng `Mps000165PDO(V_HIS_EXP_MEST, List<V_HIS_EXP_MEST_MEDICINE>, List<V_HIS_EXP_MEST_MATERIAL>[, List<V_HIS_EXP_MEST_BLOOD>][, List<HIS_MACHINE>])` → `MpsPrinter.Run` → `Mps000165Processor.ProcessData()`:

```
ReadTemplate → SetSingleKey()
   ├─ key phiếu: CREATE_TIME_STR/DATE_STR/DATE_SEPARATE, RECIPIENT, RECEIVING_PLACE + AddObjectKeyIntoListkey(V_HIS_EXP_MEST)
   ├─ thuốc   → group → Mps000165ADO (TYPE_ID = 1)
   ├─ vật tư  → group → Mps000165ADO (TYPE_ID = 2)
   ├─ máu     → group → Mps000165ADO (TYPE_ID = 3)      ← thêm 26/08/2026
   └─ key tổng: APPROVAL_LOGINNAME, EXP_LOGINNAME, EXP_TIME_STR, SUM_TOTAL_PRICE(_TEXT/_AFTER_DISCOUNT), IMP_PRICE_AFTER_VAT_SUM(_FORMAT/_TEXT/_SEPARATE/_SEPARATE_TEXT), key máy (HIS_MACHINE)
→ singleTag.ProcessData → barCodeTag.ProcessData
→ objectTag.AddObjectData("ListMediMate1/2/3", listAdo) + FuncMergeData11..33 (CalculateMergerData theo TYPE_ID + MEDI_MATE_TYPE_ID)
```

### Điều kiện nghiệp vụ
- Phiếu loại **KHÁC**: thuốc/vật tư group theo `TYPE_ID, IMP_PRICE, IMP_VAT_RATIO, DISCOUNT, PACKAGE_NUMBER, EXPIRED_DATE` (mỗi lô–hạn một dòng); loại khác chỉ group theo `TYPE_ID, IMP_PRICE, IMP_VAT_RATIO, DISCOUNT`.
- **Máu**: group theo `BLOOD_TYPE_ID, BLOOD_ABO_ID, BLOOD_RH_ID, IMP_PRICE, IMP_VAT_RATIO, DISCOUNT, PACKAGE_NUMBER, EXPIRED_DATE` — thêm ABO/Rh để không gộp hai nhóm máu khác nhau vào một dòng. `V_HIS_EXP_MEST_BLOOD` **không có cột AMOUNT** (mỗi bản ghi = 1 đơn vị) ⇒ `AMOUNT = số bản ghi trong nhóm`; tiền = `1 × IMP_PRICE × (1 + IMP_VAT_RATIO)` mỗi đơn vị (giống MPS000198).
- Không lọc `IN_EXECUTE/IN_REQUEST` theo trạng thái phiếu (in toàn bộ dòng truyền vào) — thuốc, vật tư, máu nhất quán.
- Thứ tự dòng: `OrderBy(TYPE_ID).ThenByDescending(NUM_ORDER)` ⇒ thuốc → vật tư → máu.
- List máu null/rỗng ⇒ nhánh máu không chạy, phiếu kho thuốc **in y hệt trước**.

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| V_HIS_EXP_MEST | View | Thông tin phiếu → key đơn (reflection `AddObjectKeyIntoListkey`) |
| V_HIS_EXP_MEST_MEDICINE | View | Dòng thuốc → ADO TYPE_ID = 1 |
| V_HIS_EXP_MEST_MATERIAL | View | Dòng vật tư → ADO TYPE_ID = 2 |
| V_HIS_EXP_MEST_BLOOD | View | Dòng máu (1 bản ghi = 1 đơn vị) → ADO TYPE_ID = 3 |
| HIS_MACHINE | Table | Key máy (tùy chọn, theo `MACHINE_ID` của phiếu) |

## 4. Cột của `Mps000165ADO` (dùng trong `ListMediMate1/2/3`)

Cột dùng chung (có giá trị với cả 3 loại hàng): `TYPE_ID`, `MEDI_MATE_TYPE_ID/CODE/NAME`, `SERVICE_UNIT_CODE/NAME`, `PACKAGE_NUMBER`, `EXPIRED_DATE_STR`, `BID_NUMBER/NAME`, `SUPPLIER_CODE/NAME`, `AMOUNT`, `DESCRIPTION`, `DISCOUNT`, `EXP_MEST_CODE`, `EXP_TIME_STR`, `IMP_TIME_STR`, `IMP_PRICE`, `IMP_VAT_RATIO`, `PRICE`, `VAT_RATIO`, `IMP_PRICE_AFTER_VAT`, `IMP_PRICE_AFTER_VAT_TOTAL`, `NUM_ORDER`.

Chỉ thuốc/vật tư: `REGISTER_NUMBER`, `NATIONAL_NAME`, `MANUFACTURER_NAME`, `ACTIVE_INGR_BHYT_CODE/NAME`, `CONCENTRA`, `STORAGE_CONDITION_CODE/NAME` (trống với máu).

**Chỉ máu** (thêm 26/08/2026 — trống với thuốc/vật tư), tag dạng `<#ListMediMate1.TÊN_CỘT;>`:

| Cột | Ý nghĩa |
|-----|---------|
| `BLOOD_CODE` | Mã đơn vị máu; nhiều đơn vị gộp 1 dòng thì nối `", "` |
| `BLOOD_TYPE_CODE` / `BLOOD_TYPE_NAME` | Mã / tên loại máu (cũng đổ vào `MEDI_MATE_TYPE_CODE/NAME`) |
| `BLOOD_ABO_CODE` | Nhóm máu ABO |
| `BLOOD_RH_CODE` | Rh |
| `VOLUME` | Thể tích (ml) |
| `PREPARATIONS_BLOOD_NAME` | Chế phẩm máu |
| `IMP_SOURCE_NAME` | Nguồn nhập |
| `GIVE_CODE` | Mã người hiến |

## 5. Constructor PDO

| Constructor | Ghi chú |
|-------------|---------|
| `(V_HIS_EXP_MEST, List<Medicine>, List<Material>)` | Cũ — giữ nguyên |
| `(V_HIS_EXP_MEST, List<Medicine>, List<Material>, List<HIS_MACHINE>)` | Cũ — giữ nguyên |
| `(V_HIS_EXP_MEST, List<Medicine>, List<Material>, List<V_HIS_EXP_MEST_BLOOD>)` | Mới 26/08/2026 |
| `(V_HIS_EXP_MEST, List<Medicine>, List<Material>, List<V_HIS_EXP_MEST_BLOOD>, List<HIS_MACHINE>)` | Mới 26/08/2026 |

Lưu ý: truyền literal `null` ở tham số thứ 4 sẽ mơ hồ giữa `List<HIS_MACHINE>` và `List<V_HIS_EXP_MEST_BLOOD>` — luôn truyền biến có kiểu.

## 6. Build trên máy backup

- Build **PDO trước** rồi processor; `/p:BuildProjectReferences=false`, `SolutionDir=...\MPS\\`, `SolutionName=MPS`, `ReferencePath=HIS.Desktop\bin\Debug\ReferencedAssemblies`.
- Processor override `ProcessEmrStockData()` (code sẵn có) ⇒ cần `MPS.ProcessorBase.dll` bản có method này: lấy `lib\MPSv2\MPS.ProcessorBase\MPS.ProcessorBase.dll` (06/08/2026, 91 648 byte) copy vào `MPS\MPS.ProcessorBase\bin\Debug\` (nơi ProjectReference resolve) và `ReferencedAssemblies`.
- Plugin gọi ctor mới ⇒ copy PDO mới vào `ReferencedAssemblies` trước khi build plugin (HintPath `lib\MPSv2\MPS.PDO\` không tồn tại trên máy).
- Deploy: PDO → `x64\ReferencedAssemblies\`, processor → `x64\Plugins\MpsProcessor\`, plugin → `x64\Plugins\Module\`.

## 7. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 26/08/2026 | nampp | Việc 44751 (BV Nguyễn Thị Thập): phiếu xuất khác tại kho máu in bảng trống vì PDO chỉ nhận thuốc/vật tư. Thêm `_Bloods` + 2 ctor overload vào `Mps000165PDO`; `Mps000165ADO` thêm ctor từ `List<V_HIS_EXP_MEST_BLOOD>` (TYPE_ID = 3, map vào cột thuốc/vật tư sẵn có + 9 cột riêng máu); `SetSingleKey()` thêm khối group máu (cộng `totalPrice`/`discount`); 3 helper `GetListString{Approval,Exp,ExpTime}LogFromExpMestBloods` union vào key người duyệt/người xuất/thời gian xuất. Không sửa template, không DB/BE. |

## 8. Test Cases

- [ ] Kho lẻ máu, phiếu xuất khác 1 đơn vị máu, template hiện tại → 1 dòng: tên loại máu, ĐVT, số lô, hạn dùng, SL 1, đơn giá, thành tiền; "Cộng"/bằng chữ đúng.
- [ ] 3 đơn vị cùng loại–lô–hạn–ABO/Rh → 1 dòng SL 3; 2 đơn vị ABO khác → 2 dòng.
- [ ] "Danh sách key": `APPROVAL_LOGINNAME`, `EXP_LOGINNAME`, `EXP_TIME_STR`, `SUM_TOTAL_PRICE`, `IMP_PRICE_AFTER_VAT_SUM` có giá trị.
- [ ] Template đặt thêm `BLOOD_ABO_CODE`, `BLOOD_RH_CODE`, `VOLUME`, `BLOOD_CODE` → cột hiện đúng.
- [ ] Hồi quy kho thuốc/vật tư (template cũ) → phiếu in y hệt trước.
