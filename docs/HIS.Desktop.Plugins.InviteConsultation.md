# Mời Hội Chẩn (InviteConsultation) — Tài Liệu Module

## 1. Tổng Quan

| Thông tin | Giá trị |
|-----------|---------|
| Plugin ID | HIS.Desktop.Plugins.InviteConsultation |
| Loại | Form (frmInviteConsultation, FormBase) |
| Mục đích | Tạo phiếu mời hội chẩn cho điều trị nội trú/ngoại trú. Hỗ trợ mời nhiều khoa cùng lúc — tạo N bản ghi `HIS_SPECIALIST_EXAM` (`INVITE_TYPE = 2`) riêng biệt cho N khoa, kèm tự sinh tờ điều trị A + Y lệnh tổng hợp ở bản ghi đầu (theo PTTK_38078 B.4.1). |
| Trạng thái | Đang phát triển |

> **Lưu ý quan trọng về model:** Tài liệu thiết kế B.2/B.3.1 nói dùng `HIS_DEBATE`, nhưng codebase hiện tại **`ApprovaleDebateList` query `V_HIS_SPECIALIST_EXAM`** với filter `INVITE_TYPE = 2`. Vì vậy plugin này phải save vào `HIS_SPECIALIST_EXAM` để record xuất hiện đúng trong "Danh sách duyệt hội chẩn". `HIS_DEBATE` được dùng cho luồng "Biên bản hội chẩn" (DebateDiagnostic) — chức năng khác.

## 2. Quy Trình Nghiệp Vụ

### Luồng chính (tạo mới — N khoa)
1. User chọn **Ngày + giờ phút mời** (`dd/MM/yyyy HH:mm`).
2. User chọn **nhiều khoa** mời hội chẩn (multi-select qua `GridCheckMarksSelection`).
3. Sau khi chọn khoa, danh sách bác sĩ tự lọc — hiển thị bác sĩ thuộc các khoa đã chọn, **phân nhóm theo khoa**.
4. User chọn bác sĩ cụ thể (tùy chọn) — bác sĩ được gán cho đúng khoa của mình (vào `EXAM_EXECUTE_LOGINNAME` của bản ghi tương ứng).
5. User nhập ICD, ICD phụ, nội dung mời, khám tại giường.
6. Nhấn **Thêm**: Frontend gọi API `POST /api/HisSpecialistExam/Create` **N lần** — mỗi khoa 1 bản ghi `HIS_SPECIALIST_EXAM` với `INVITE_TYPE = 2`.
   - Bản ghi **đầu tiên** (i = 0):
     - `IsAutoCreateTracking = 1` → backend tự sinh tờ điều trị A và gán cho HIS_TRACKING
     - `MedicalInstruction = "Mời hội chẩn lúc {HH:mm dd/MM/yyyy} Khoa phòng mời hội chẩn: {Khoa A, Khoa B, ...}"` → backend gán vào `HIS_TRACKING.MEDICAL_INSTRUCTION` của tờ điều trị A
   - Bản ghi **còn lại** (i > 0):
     - `IsAutoCreateTracking = 0`
     - `MedicalInstruction = null`

### Luồng phụ (sửa)
- Chỉ sửa được **1 bản ghi `HIS_SPECIALIST_EXAM`** (specialistExam). Khoa mời cố định (disabled), sửa nội dung/ICD/bác sĩ.
- Gọi `POST /api/HisSpecialistExam/Update` 1 lần với `HIS_SPECIALIST_EXAM`.

### Sơ đồ
```
[N khoa được chọn] ──┬─► i=0: SDO{ HisSpecialistExam(dept A, INVITE_TYPE=2), IsAutoCreateTracking=1, MedicalInstruction=tổng hợp } → /api/HisSpecialistExam/Create
                    │
                    ├─► i=1: SDO{ HisSpecialistExam(dept B), IsAutoCreateTracking=0, MedicalInstruction=null }     → /api/HisSpecialistExam/Create
                    │
                    └─► i=N-1: SDO{ HisSpecialistExam(dept N), IsAutoCreateTracking=0, MedicalInstruction=null }  → /api/HisSpecialistExam/Create
```

## 3. EFMODEL / SDO Sử Dụng

| Entity/SDO | Loại | Mục đích |
|------------|------|----------|
| HIS_SPECIALIST_EXAM | Table | Bản ghi phiếu mời hội chẩn (`INVITE_TYPE = 2`). Mỗi khoa được mời = 1 record (`EXAM_EXECUTE_DEPARMENT_ID` khác nhau) |
| V_HIS_SPECIALIST_EXAM | View | View hiển thị cho `ApprovaleDebateList` |
| HisSpecialistExamCreateAutoTrackingFeSDO | SDO (frontend wrapper) | Bọc `HIS_SPECIALIST_EXAM` + `IsAutoCreateTracking` + `MedicalInstruction` cho POST `/api/HisSpecialistExam/Create`. Khi MOS.SDO update có SDO chuẩn, có thể xoá wrapper. |
| L_HIS_TREATMENT_BED_ROOM | View (cache) | Context giường-buồng-phòng khi mời từ nội trú |
| V_HIS_SERVICE_REQ | View | Context yêu cầu dịch vụ khi mời từ ngoại trú |
| HIS_DEPARTMENT | Table | Danh mục khoa cho cboPhongKham, cboDepartment |
| HIS_EMPLOYEE | Table | Danh sách bác sĩ, lọc theo DEPARTMENT_ID + IS_DOCTOR=1 |
| HIS_ICD | Table | Danh mục ICD cho HIS.UC.Icd, HIS.UC.SecondaryIcd |
| HIS_TRACKING | Table | Backend gán `MEDICAL_INSTRUCTION` vào tờ điều trị khi `IsAutoCreateTracking=1` |

## 4. UI Layout

```
+--------------------------------------------------------+
| Ngày mời:           [dd/MM/yyyy HH:mm  ▼]              |
| Khoa phòng điều trị: [Cbo (disabled)]                   |
| Khoa phòng mời:     [Multi-select dept (checkbox) ▼]    | ← N khoa (GridCheckMarksSelection)
| Bác sĩ hội chẩn:    [Multi-select doctor (grouped) ▼]   | ← grouped by khoa (GridCheckMarksSelection)
|   ┌── ICD ──────────────────────────────────┐          |
|   │ [HIS.UC.Icd]                            │          |
|   └────────────────────────────────────────┘          |
|   ┌── ICD phụ ───────────────────────────────┐         |
|   │ [HIS.UC.SecondaryIcd]                    │         |
|   └────────────────────────────────────────┘          |
| Khám tại giường: [☐]                                   |
| Nội dung mời:                                          |
| ┌──────────────────────────────────────────┐           |
| │ [MemoEdit]                               │           |
| └──────────────────────────────────────────┘           |
|                       [Sửa] [Thêm] [Làm lại]           |
+--------------------------------------------------------+
```

### UC sử dụng
| UC | Panel | Mục đích |
|----|-------|----------|
| HIS.UC.Icd | panelIcd | Chẩn đoán chính |
| HIS.UC.SecondaryIcd | panelSubIcd | Chẩn đoán phụ |

### Control quan trọng
| Control | Loại | Mô tả |
|---------|------|-------|
| dteNgayMoi | DateEdit (date + time) | `dd/MM/yyyy HH:mm`, mask + CalendarTimeProperties |
| cboPhongKham | CustomGridLookUpEditWithFilterMultiColumnNoFocus | Multi-select khoa qua GridCheckMarksSelection. **Filter UI bị suppress** trên popup (xem `SuppressFilterUI`) |
| cboBacSiKham | CustomGridLookUpEditWithFilterMultiColumnNoFocus | Multi-select bác sĩ, GroupIndex theo `DEPARTMENT_NAME`. **Filter UI bị suppress** |

## 5. API Endpoints

| Action | URI | Consumer | Payload |
|--------|-----|----------|---------|
| Tạo mới (N khoa) | `api/HisSpecialistExam/Create` | MosConsumer | `HisSpecialistExamCreateAutoTrackingFeSDO` (N lần) |
| Cập nhật | `api/HisSpecialistExam/Update` | MosConsumer | `HIS_SPECIALIST_EXAM` |

### Field mapping (UI → HIS_SPECIALIST_EXAM)

| UI Control | HIS_SPECIALIST_EXAM field | Ghi chú |
|------------|---------------------------|---------|
| `dteNgayMoi` (DateTime) | `INVITE_TIME` (long yyyyMMddHHmmss) | |
| current login | `INVITE_DOCTOR_LOGINNAME`, `INVITE_DOCTOR_USERNAME` | BS yêu cầu hội chẩn |
| `cboDepartment` (treatment dept) | `INVITE_DEPARMENT_ID` | Khoa điều trị (disabled) |
| `cboPhongKham` — mỗi khoa | `EXAM_EXECUTE_DEPARMENT_ID` (per record) | Multi-select → N record |
| `cboBacSiKham` — lọc theo dept | `EXAM_EXECUTE_LOGINNAME`, `EXAM_EXECUTE_USERNAME` (per record, comma-joined) | Chỉ BS thuộc khoa của record |
| `ucIcd` | `ICD_CODE`, `ICD_NAME` | |
| `ucSecondaryIcd` | `ICD_SUB_CODE`, `ICD_TEXT` | |
| `memContent` | `INVITE_CONTENT` | Nội dung mời |
| `chkExamInBed` | `IS__EXAM_BED` (1 / null) | Khám tại giường |
| — | `INVITE_TYPE = 2` | Cố định = hội chẩn (1 = khám chuyên khoa) |
| `bedRoom`/`serviceReq`/`specialistExam` | `TREATMENT_ID`, `TREATMENT_CODE`, `PATIENT_CODE`, `TDL_PATIENT_*` | Context bệnh nhân |

## 6. Dependencies

### UC
- HIS.UC.Icd, HIS.UC.SecondaryIcd

### Library nội bộ
- Không

### Inter-Plugin
- Được mở từ `HIS.Desktop.Plugins.ExecuteRoom` qua right-click "Mời hội chẩn" → truyền `Module + bool + V_HIS_SERVICE_REQ`.

## 7. Print
Không in tại module này.

## 8. Changelog

| Ngày | Người sửa | Mô tả thay đổi |
|------|-----------|-----------------|
| 2026-05-22 | phuongnm | **Frontend tự tạo HIS_TRACKING (tờ điều trị A)** sau khi save Mời hội chẩn thành công, vì backend chưa hỗ trợ auto-create. Sau khi bản ghi đầu (i=0) save xong, gọi `POST api/HisTracking/Create` với: `TREATMENT_ID`, `TRACKING_TIME=INVITE_TIME`, `CONTENT=INVITE_CONTENT` (Diễn biến), `MEDICAL_INSTRUCTION=chuỗi tổng hợp` (Y lệnh), `LOGINNAME/USERNAME=current user`, `DEPARTMENT_ID=INVITE_DEPARMENT_ID`, ICD copy. Sau đó update lại HIS_SPECIALIST_EXAM với `TRACKING_ID` để link 2 record. Method: `CreateTrackingForFirstExam()`. Phương pháp xử lý trong "Tờ điều trị" giờ hiển thị đúng chuỗi sau Mời hội chẩn. |
| 2026-05-21 | phuongnm | Sửa đổi UI + nghiệp vụ theo PTTK_38078 B.4.1: (1) `dteNgayMoi` hỗ trợ `dd/MM/yyyy HH:mm`; (2) `cboPhongKham` chuyển multi-select khoa; (3) `cboBacSiKham` nhóm bác sĩ theo khoa, chỉ hiển thị BS thuộc các khoa đã chọn; (4) Save tạo N bản ghi: i=0 có `IsAutoCreateTracking=1` + `MedicalInstruction` tổng hợp, các bản còn lại `IsAutoCreateTracking=0`. Thêm SDO wrapper `HisSpecialistExamCreateAutoTrackingFeSDO`. |
| 2026-05-21 | phuongnm | **Fix filter glitch** trong popup `cboBacSiKham`/`cboPhongKham`: hook `Popup` event gọi `SuppressFilterUI` để vô hiệu auto-filter row + header filter + filter panel + filter trên cột `Mark` (do `GridCheckMarksSelection` tạo) → tránh bug `[Mark] = 'Checked'/'Unchecked'` làm ẩn rows + mất data khi user click checkbox header. |
| 2026-05-21 | phuongnm | **Revert model** từ `HIS_DEBATE` (theo lý thuyết tài liệu B.4.1) về `HIS_SPECIALIST_EXAM` với `INVITE_TYPE = 2` (thực tế codebase). Lý do: `ApprovaleDebateList` ("Danh sách duyệt hội chẩn") query `V_HIS_SPECIALIST_EXAM` qua `api/HisSpecialistExam/GetView` với filter `INVITE_TYPE = 2` — nếu save HIS_DEBATE thì record không xuất hiện trong danh sách duyệt. Tài liệu B.2/B.3.1 mismatch với cấu trúc hiện hữu. |

## 9. Test Cases

### Tạo mới (1 khoa)
- [ ] Chọn 1 khoa + bác sĩ → Save → 1 bản ghi `HIS_SPECIALIST_EXAM` tạo thành công với `INVITE_TYPE=2`, `IsAutoCreateTracking=1`, `MedicalInstruction` đúng định dạng
- [ ] Record xuất hiện trong "Danh sách duyệt hội chẩn" (`ApprovaleDebateList`)

### Tạo mới (N khoa)
- [ ] Chọn 3 khoa (A, B, C) + chọn bác sĩ ở từng khoa → Save → 3 record `HIS_SPECIALIST_EXAM` tạo thành công
- [ ] Record i=0 (khoa A): `IsAutoCreateTracking=1`, `MedicalInstruction="Mời hội chẩn lúc HH:mm dd/MM/yyyy Khoa phòng mời hội chẩn: A, B, C"`, `HIS_TRACKING.MEDICAL_INSTRUCTION` của tờ A được gán chuỗi này
- [ ] Record i=1, i=2: `IsAutoCreateTracking=0`, `MedicalInstruction=null`
- [ ] Mỗi record `EXAM_EXECUTE_DEPARMENT_ID` đúng khoa; `EXAM_EXECUTE_LOGINNAME` chỉ chứa bác sĩ thuộc khoa đó
- [ ] 3 record cùng `INVITE_TIME` xuất hiện trong "Danh sách duyệt hội chẩn"

### Validation
- [ ] Chưa chọn khoa nào → Save → cảnh báo trường bắt buộc
- [ ] Chọn khoa → cảnh báo biến mất

### UI
- [ ] Ngày mời mặc định DateTime.Now (date + time)
- [ ] Đổi khoa → danh sách bác sĩ refresh, group theo khoa
- [ ] Bỏ chọn tất cả khoa → danh sách bác sĩ rỗng
- [ ] Click vào checkbox header/row trong picker BS → toggle đúng, **không** apply filter `[Mark] = ...`, **không** mất data
- [ ] Click 2 lần checkbox header → vẫn toggle đúng

### Sửa
- [ ] Mở từ specialistExam → khoa hiển thị đúng, disabled không cho đổi
- [ ] Sửa nội dung, bác sĩ → Save → cập nhật thành công
