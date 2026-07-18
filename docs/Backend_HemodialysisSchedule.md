# Backend — HisHemodialysisSchedule (yêu cầu 4.2.1)

> Source backend (MOS server) KHÔNG nằm trong checkout này (chỉ có DLL `MOS.*` + client `HIS.Desktop.ApiConsumer`).
> Tài liệu này là **hợp đồng** để đội backend hiện thực entity + service. Plugin desktop `HIS.Desktop.Plugins.HemodialysisSchedule` đã gọi đúng các endpoint dưới đây.

## 1. Bảng `HIS_HEMODIALYSIS_SCHEDULE` (Oracle)

```sql
CREATE TABLE HIS_HEMODIALYSIS_SCHEDULE (
    ID                     NUMBER(20)   NOT NULL,
    -- Khóa nghiệp vụ
    TREATMENT_ID           NUMBER(20)   NOT NULL,
    PATIENT_ID             NUMBER(20)   NOT NULL,
    ROOM_ID                NUMBER(20)   NOT NULL,
    SCHEDULE_DATE          NUMBER(8)    NOT NULL,   -- yyyyMMdd
    KIDNEY_SHIFT           NUMBER(2)    NOT NULL,   -- Ca 1..5
    -- Thông tin xếp lịch
    EXP_MEST_TEMPLATE_ID   NUMBER(20),              -- Gói vật tư (HIS_EXP_MEST_TEMPLATE.ID)
    NOTE                   NVARCHAR2(2000),
    -- Cột audit chuẩn MOS
    IS_ACTIVE              NUMBER(1)    DEFAULT 1 NOT NULL,
    IS_DELETE              NUMBER(1)    DEFAULT 0 NOT NULL,
    GROUP_CODE             NVARCHAR2(80),
    CREATE_TIME            NUMBER(14),
    MODIFY_TIME            NUMBER(14),
    CREATOR                NVARCHAR2(50),
    MODIFIER               NVARCHAR2(50),
    APP_CREATOR            NVARCHAR2(50),
    APP_MODIFIER           NVARCHAR2(50),
    CONSTRAINT PK_HIS_HEMODIALYSIS_SCHEDULE PRIMARY KEY (ID)
);

-- Unique key chống trùng slot: 1 BN không xếp 2 lần cùng ngày + cùng ca
CREATE UNIQUE INDEX UQ_HEMODIALYSIS_SCHEDULE
    ON HIS_HEMODIALYSIS_SCHEDULE (TREATMENT_ID, SCHEDULE_DATE, KIDNEY_SHIFT);

CREATE INDEX IDX_HEMODIALYSIS_ROOM_DATE
    ON HIS_HEMODIALYSIS_SCHEDULE (ROOM_ID, SCHEDULE_DATE, KIDNEY_SHIFT);

CREATE SEQUENCE HIS_HEMODIALYSIS_SCHEDULE_SEQ START WITH 1 INCREMENT BY 1 NOCACHE;
```

> Cột `SCHEDULE_DATE` lưu số `yyyyMMdd`; `CREATE_TIME/MODIFY_TIME` số `yyyyMMddHHmmss` theo chuẩn MOS. Không có cột MACHINE_ID (theo thiết kế: không quản Máy ở màn xếp lịch). Không sinh `HIS_SERVICE_REQ`.

Sau khi tạo bảng → gen lại entity `HIS_HEMODIALYSIS_SCHEDULE` vào `MOS.EFMODEL`, filter `HisHemodialysisScheduleFilter` vào `MOS.Filter` (nếu backend dùng chung với client, plugin đang dùng DTO cục bộ nên không bắt buộc).

## 2. Endpoints (service `HisHemodialysisSchedule`)

| Action | Method | Request | Response |
|--------|--------|---------|----------|
| `Get` | POST | `HisHemodialysisScheduleFilter` | `List<HemodialysisScheduleADO>` |
| `CreateList` | POST | `List<HemodialysisScheduleADO>` | `List<HemodialysisScheduleADO>` (đã tạo) |
| `Update` | POST | `HemodialysisScheduleADO` | `HemodialysisScheduleADO` |
| `Delete` | POST | `long id` | `bool` |
| `CopySchedule` | POST | `CopyScheduleSDO` | `CopyScheduleResultADO` |

### 2.1 Filter (Get)
```
HisHemodialysisScheduleFilter { long? ROOM_ID; long? SCHEDULE_DATE; short? KIDNEY_SHIFT;
                                string KEY_WORD; string ORDER_FIELD; string ORDER_DIRECTION; }
```
- `Get` trả về đã **JOIN** để đổ các cột hiển thị: `TREATMENT_CODE, TDL_PATIENT_NAME, TDL_PATIENT_CODE, TDL_PATIENT_DOB, TDL_PATIENT_IS_HAS_NOT_DAY_DOB, TDL_PATIENT_GENDER_NAME, IN_TIME, TDL_PATIENT_TYPE_NAME (Đối tượng), EXP_MEST_TEMPLATE_NAME` + 4 cột audit. `KEY_WORD` lọc theo tên/mã BN, mã điều trị.

### 2.2 CreateList (R5)
- Với mỗi phần tử: INSERT slot (TREATMENT_ID, PATIENT_ID, ROOM_ID, SCHEDULE_DATE, KIDNEY_SHIFT, EXP_MEST_TEMPLATE_ID, NOTE).
- Chống trùng theo unique key — nếu trùng thì bỏ qua (không lỗi) hoặc trả lỗi tùy nghiệp vụ; khuyến nghị **bỏ qua trùng** và chỉ trả về các bản ghi tạo mới.
- **KHÔNG** tạo `HIS_SERVICE_REQ` / y lệnh.

### 2.3 Update
- Chỉ cho sửa `EXP_MEST_TEMPLATE_ID` và `NOTE` theo `ID`. Cập nhật `MODIFY_TIME/MODIFIER`.

### 2.4 CopySchedule (R6)
```
CopyScheduleSDO { long ROOM_ID; long SOURCE_DATE; long TARGET_DATE; }   // date = yyyyMMdd
CopyScheduleResultADO { int AddedCount; int SkippedCount;
                        List<CopyScheduleSkippedItem> SkippedItems; }
CopyScheduleSkippedItem { long TREATMENT_ID; string PATIENT_NAME; short KIDNEY_SHIFT; }
```
- Đọc mọi slot của (ROOM_ID, SOURCE_DATE) → INSERT sang TARGET_DATE (giữ nguyên KIDNEY_SHIFT, EXP_MEST_TEMPLATE_ID, NOTE).
- **Skip** slot mà (TREATMENT_ID, TARGET_DATE, KIDNEY_SHIFT) đã tồn tại. **KHÔNG** xóa slot đã có ở ngày đích. **KHÔNG** sinh y lệnh.
- Trả về `AddedCount` + danh sách skip để client hiển thị popup.

## 3. Endpoint phụ trợ (đã có sẵn hoặc cần xác nhận)

| Action | Ghi chú |
|--------|---------|
| `HisTreatment/GetView4` → `List<V_HIS_TREATMENT_4>` | DS BN đang điều trị (vùng dưới). **Xác nhận tên action** — plugin đang gọi `api/HisTreatment/GetView4`. Filter cần hỗ trợ `DEPARTMENT_ID, IN_TIME_FROM, IN_TIME_TO, KEY_WORD, IS_IN_TREATMENT`. |
| `HisExpMestTemplate/Get` → `List<HIS_EXP_MEST_TEMPLATE>` | Filter: `(CREATOR = :loginname OR IS_PUBLIC = 1) AND IS_KIDNEY = 1 AND IS_ACTIVE = 1`. |

## 4. Phân quyền / audit
- Ghi `CREATOR/CREATE_TIME` khi CreateList & CopySchedule; `MODIFIER/MODIFY_TIME` khi Update.
- (Tùy chọn) kiểm tra quyền sửa/xóa slot theo phòng/khoa của người đăng nhập.
