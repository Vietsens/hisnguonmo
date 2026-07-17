# HisDeathInfo — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.HisDeathInfo |
| Loại | Form (FormBase) |
| Mục đích | Ghi nhận thông tin tử vong (mặc định) hoặc thông tin người bệnh nặng xin về (theo Phụ lục 1.2 BYT) cho 1 hồ sơ điều trị. Lưu cùng bảng `HIS_SEVERE_ILLNESS_INFO`, không có cờ phân biệt loại phiếu. |
| Người tạo | Inventec |
| Ngày cập nhật | 19/05/2026 |
| Trạng thái | Bảo trì |

## 2. Quy Trình Nghiệp Vụ

### Luồng chính
1. User mở plugin với `treatmentId` → form load → `LoadTreatment()` gọi API `api/HisTreatment/Get` + `api/HisSevereIllnessInfo/Get` + `api/HisEventsCausesDeath/Get`.
2. `ResolveSevereIllnessMode()` quyết định mode hiển thị:
   - `TREATMENT_END_TYPE_ID` của hồ sơ KHÁC `ID__CHET` (tử vong)
   - AND `TREATMENT_END_TYPE_CODE` ∈ list config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES` (phân cách dấu phẩy)
   - → Mode "Nặng xin về" (Phụ lục 1.2 BYT)
   - Ngược lại (kể cả Tử vong) → Mode mặc định "Tử vong"
3. `ApplyFormTitle()` → đổi `this.Text` thành "Thông tin người bệnh nặng xin về" nếu mode = nặng xin về.
4. `InitUCDeath()` (Tab "Thông tin chung") + `InitUCCauseOfDeath()` (Tab "Thông tin chi tiết") → truyền cờ `IsSevereIllnessMode` qua InitADO. UC tự render label/validation theo cờ.
5. User Sửa → Lưu → 2 API songs song:
   - `api/HisTreatment/UpdateDeathInfo` — cập nhật thông tin chung lên `HIS_TREATMENT`
   - `api/HisSevereIllnessInfo/CreateOrUpdate` — cập nhật chi tiết lên `HIS_SEVERE_ILLNESS_INFO` + `HIS_EVENTS_CAUSES_DEATH`

### Sơ đồ trạng thái
```
Mở plugin
   ↓
LoadTreatment → ResolveSevereIllnessMode
   ↓
   ├─ ID__CHET                              → Mode "Tử vong"        (label/validation gốc)
   ├─ != ID__CHET AND END_TYPE_CODE ∈ cfg   → Mode "Nặng xin về"    (label Phụ lục 1.2, validation tối thiểu)
   └─ != ID__CHET AND END_TYPE_CODE ∉ cfg   → Mode "Tử vong"        (label/validation gốc)
   ↓
Render Title + InitUC (truyền cờ IsSevereIllnessMode)
   ↓
Sửa → Lưu vào HIS_SEVERE_ILLNESS_INFO (không cờ phân biệt)
```

### Điều kiện nghiệp vụ
- Lưu dữ liệu vào **cùng bảng** `HIS_SEVERE_ILLNESS_INFO` — không có cờ phân biệt loại phiếu (mode chỉ ảnh hưởng UI).
- Mode "Nặng xin về" auto-check option **"Tiên lượng nặng xin về"** trong nhóm "Tử vong tại" (chkDeathType2).
- Bug BYT chuẩn: label "Trong vòng 43 ngày" sai → **đã sửa thành "42 ngày"** trên Designer (áp dụng cả 2 mode).

## 3. EFMODEL Sử Dụng

| Entity | Loại | Mục đích |
|--------|------|----------|
| HIS_TREATMENT | Table | Hồ sơ điều trị — chứa `TREATMENT_END_TYPE_ID`, `DEATH_*` |
| HIS_TREATMENT_END_TYPE | Table | Danh mục lý do kết thúc điều trị — lookup `TREATMENT_END_TYPE_CODE` để check config |
| HIS_SEVERE_ILLNESS_INFO | Table | Thông tin chi tiết (PT 4 tuần, hình thức, thai nhi/sơ sinh, mang thai...) |
| HIS_EVENTS_CAUSES_DEATH | Table | Chuỗi sự kiện/nguyên nhân (3 bảng: chính, khác, ngoài) |
| HIS_DEATH_CAUSE | Table | Danh mục nguyên nhân (combo) |
| HIS_DEATH_WITHIN | Table | Danh mục "Trong vòng" (combo) |
| V_HIS_DEATH_CERT_BOOK | View | Sổ chứng tử (combo) |
| HIS_PATIENT | Table | Thông tin bệnh nhân |
| HIS_ICD | Table | Danh mục ICD (combo + grid lookup) |

## 4. UI Layout

### Sơ đồ giao diện
```
+---------------------------------------------------------------------+
| Title: "Thông tin tử vong" / "Thông tin người bệnh nặng xin về"     |
+---------------------------------------------------------------------+
| Tab [Thông tin chung]   [Thông tin chi tiết]                        |
+---------------------------------------------------------------------+
| Tab 1 = UCDeath (HIS.UC.Death)                                      |
|   - Mã BN, Mã ĐT, Họ tên, Ngày sinh, Giới tính, Số thẻ BHYT         |
|   - Nguyên nhân, Trong vòng, TG tử vong / TG xin về                 |
|   - Có khám nghiệm, Nơi tử vong, Tình trạng TV                      |
|   - Số chứng từ / Loại / Số / Nơi cấp / Ngày cấp                    |
|   - Sổ báo tử BĐ (số + sổ), Người cấp GBT, Ngày cấp GBT             |
|   - Người thân, Mã định danh                                        |
|   - Nguyên nhân chính (TA) / Nguyên nhân bệnh nặng chính            |
|   - Thông tin giải phẫu (TA)                                        |
+---------------------------------------------------------------------+
| Tab 2 = UCCauseOfDeath (HIS.UC.UCCauseOfDeath)                      |
|   - Header BN, Số ngày vắng mặt, ICU, Tử vong tại (CSKCB/T.lượng    |
|     nặng/Đường)                                                     |
|   - Phần A: chuỗi sự kiện (grid), bệnh lý khác (grid)               |
|   - Phần B / Các thông tin y tế khác: PT 4 tuần, giám định pháp y,  |
|     Hình thức, Ngoài nhân, Người bệnh thai nhi/sơ sinh, Đa thai,    |
|     Sinh non, 24h sống, Cân nặng, Tuổi thai/mẹ, Mang thai...        |
|   - Kết luận nguyên nhân chính                                      |
+---------------------------------------------------------------------+
| [Sửa Ctrl+E] [Lưu Ctrl+S] [Hủy Ctrl+H] [In Ctrl+P]                  |
+---------------------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Death | xtraScrollableControl1 (Tab 1) | Thông tin chung — nhận `DeathInitADO.IsSevereIllnessMode` để đổi label/validation |
| HIS.UC.UCCauseOfDeath | xtraScrollableControl2 (Tab 2) | Thông tin chi tiết — nhận `CauseOfDeathADO.IsSevereIllnessMode` để đổi label + auto-check |

### Label mapping (mode "Nặng xin về" — Phụ lục 1.2 BYT)

**UCDeath (Tab "Thông tin chung"):**
| Field gốc | Mode "Nặng xin về" | Required |
|-----------|--------------------|----|
| TG tử vong | TG xin về | Giữ |
| Trong vòng (combo) | Giữ nguyên | Bỏ |
| Có khám nghiệm | Giữ nguyên | Bỏ |
| Nơi tử vong | Giữ nguyên | Bỏ |
| Tình trạng TV | Giữ nguyên | Bỏ |
| Số chứng từ / Loại / Số / Nơi cấp / Ngày cấp | Giữ nguyên | Bỏ |
| Sổ báo tử BĐ (2 field) | Giữ nguyên | Bỏ |
| Người cấp GBT / Ngày cấp GBT | Giữ nguyên | Bỏ |
| Nguyên nhân chính | Nguyên nhân bệnh nặng chính | Giữ |
| Thông tin giải phẫu | Giữ nguyên | Bỏ |

**UCCauseOfDeath (Tab "Thông tin chi tiết"):**
| Field gốc | Mode "Nặng xin về" |
|-----------|--------------------|
| Nguyên nhân tử vong (chuỗi sự kiện dẫn đến tử vong) | Chuỗi bệnh lý, sự kiện từ khi khởi phát nguyên nhân đến khi nặng xin về |
| Cột "Thời gian" | Thời gian (từ khi khởi phát đến khi xin về) |
| Nguyên nhân tử vong trực tiếp (trước ngừng thở, ngừng tim) | Nguyên nhân nặng xin về trực tiếp |
| (dòng dưới cùng chính là nguyên nhân chính gây tử vong) | (dòng dưới cùng chính là nguyên nhân chính gây bệnh nặng) |
| Bệnh lý, nguy cơ quan trọng khác góp phần gây tử vong | Bệnh lý, nguy cơ quan trọng khác góp phần tăng nặng |
| Phần B: Thông tin tử vong khác | Các thông tin y tế khác |
| Đã sử dụng kết quả giám định để cập nhật chẩn đoán NNTV chưa? (tooltip) | Có trưng cầu giám định pháp y không |
| Hình thức tử vong | Hình thức |
| Tử vong thai nhi hoặc trẻ sơ sinh | Người bệnh là thai nhi hoặc trẻ sơ sinh |
| Nếu tử vong trong vòng 24h, ghi rõ số giờ sống | Nếu xin về trong vòng 24 giờ, ghi rõ giờ sống đến khi xin về |
| Đối với phụ nữ, có phải tử vong khi mang thai? | Đối với phụ nữ, có đang mang thai không |
| Tại thời điểm tử vong | Tại thời điểm xin về |
| Trong vòng 43 ngày trước khi tử vong → **đã fix Designer 43→42 cho cả 2 mode** | Trong vòng 42 ngày trước khi xin về |
| Từ ngày thứ 43 đến 1 năm trước khi tử vong | Trong khoảng 43 ngày đến 1 năm trước khi xin về |
| Việc mang thai có góp phần gây tử vong không | Việc mang thai có góp phần gây bệnh nặng |
| Kết luận Nguyên nhân tử vong chính | Kết luận nguyên nhân chính gây bệnh nặng |
| (Auto-check chkDeathType2) | Option "Tiên lượng nặng xin về" tự check khi load |

## 5. API Endpoints

| Action | URI | Consumer | Filter |
|--------|-----|----------|--------|
| Lấy treatment | `api/HisTreatment/Get` | MosConsumer | HisTreatmentFilter (ID) |
| Lấy SevereIllnessInfo | `api/HisSevereIllnessInfo/Get` | MosConsumer | HisSevereIllnessInfoFilter (TREATMENT_ID) |
| Lấy EventsCausesDeath | `api/HisEventsCausesDeath/Get` | MosConsumer | HisEventsCausesDeathFilter (SEVERE_ILLNESS_INFO_ID) |
| Lấy patient (UCDeath nội bộ) | `api/HisPatient/Get` | MosConsumer | HisPatientViewFilter (ID) |
| Cập nhật thông tin chung | `api/HisTreatment/UpdateDeathInfo` | MosConsumer | HIS_TREATMENT body |
| Lưu chi tiết | `api/HisSevereIllnessInfo/CreateOrUpdate` | MosConsumer | SevereIllnessInfoSDO |

## 6. Dependencies

### Library Plugins
| Library | Mục đích |
|---------|----------|
| HIS.Desktop.Plugins.Library.PrintTreatmentFinish | In "Giấy báo tử" (Mps000268) qua button In |

### UC dùng chung (ảnh hưởng plugin khác khi thay đổi)
| UC | Plugins khác sử dụng |
|----|---------------------|
| HIS.UC.Death | HIS.Desktop.Plugins.TreatmentFinish |
| HIS.UC.UCCauseOfDeath | HIS.Desktop.Plugins.TreatmentFinish, TreatmentList, InformationAllowGoHome |

→ Thay đổi UI/validation phải gắn cờ `IsSevereIllnessMode` (default false) để giữ tương thích.

### Config phụ thuộc
| Config Key | Loại | Mô tả |
|-----------|------|-------|
| MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES | string (CSV) | Danh sách `TREATMENT_END_TYPE_CODE` thuộc nhóm "Nặng xin về" |
| HIS.UC.Death__IS_NOT_REQUIRED_DEATH_CERT_BOOK | string ("1"/"") | Bỏ required Sổ chứng tử trong UCDeath |

## 7. Print

| Loại in | PrintTypeCode | Library | Template |
|---------|--------------|---------|----------|
| Giấy báo tử | Mps000268 | PrintTreatmentFinish | Mps000268.* |

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 19/05/2026 | sinhnt | Thêm mode "Người bệnh nặng xin về" theo Phụ lục 1.2 BYT. Form đọc config `MOS.HIS_SEVERE_ILLNESS_INFO.MUST_INPUT_SEVERE_ILLNESS_HOME_CODES`, lookup `TREATMENT_END_TYPE_CODE` qua BackendDataWorker. Truyền cờ `IsSevereIllnessMode` vào `DeathInitADO` (HIS.UC.Death) và `CauseOfDeathADO` (HIS.UC.UCCauseOfDeath). 2 UC tự render label + giảm validation theo cờ. Auto-check option "Tiên lượng nặng xin về". Fix bug Designer "43 ngày" → "42 ngày" (cho cả 2 mode). Tạo docs đầy đủ. |

## 9. Test Cases

### Mode mặc định "Tử vong"
- [ ] Hồ sơ có `TREATMENT_END_TYPE_ID = ID__CHET` → Title "Thông tin tử vong", label gốc, required đầy đủ.
- [ ] Hồ sơ không thuộc config + KHÔNG `ID__CHET` → Title "Thông tin tử vong", label gốc.
- [ ] Sửa → Lưu thành công vào `HIS_SEVERE_ILLNESS_INFO`.

### Mode "Nặng xin về"
- [ ] Set config `MUST_INPUT_SEVERE_ILLNESS_HOME_CODES = "XV"` (giả sử XV là code XINRAVIEN).
- [ ] Mở hồ sơ KQĐT = XINRAVIEN → Title đổi thành "Thông tin người bệnh nặng xin về".
- [ ] Tab 1: "TG xin về", "Nguyên nhân bệnh nặng chính" hiển thị đúng.
- [ ] Tab 2: option "Tiên lượng nặng xin về" auto-check khi load.
- [ ] Tab 2: label "Trong vòng 42 ngày trước khi xin về" (đã fix 43→42).
- [ ] Validation: chỉ required Nguyên nhân, TG xin về, Người thân, Mã định danh, Nguyên nhân bệnh nặng chính.
- [ ] Lưu → Vào cùng bảng `HIS_SEVERE_ILLNESS_INFO`, không cờ phân biệt.

### Backward compatibility
- [ ] Plugin TreatmentFinish mở UCDeath/UCCauseOfDeath không truyền `IsSevereIllnessMode` → behavior y nguyên (default false).
- [ ] Plugin TreatmentList/InformationAllowGoHome mở UCCauseOfDeath không truyền cờ → behavior y nguyên.

### In ấn
- [ ] Click In → Hiện preview Giấy báo tử (Mps000268).

### Edge case
- [ ] Config rỗng/null → Mặc định mode "Tử vong".
- [ ] Treatment có `TREATMENT_END_TYPE_ID = null` → Mặc định mode "Tử vong".
- [ ] Config có code không khớp với bất kỳ TREATMENT_END_TYPE_CODE nào → Mặc định mode "Tử vong".
