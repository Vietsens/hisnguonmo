-- =====================================================================
-- Kết nối xét nghiệm — Duyệt / Hủy duyệt kết quả THEO TỪNG DỊCH VỤ
-- DB: LIS (schema LIS_RS) — Oracle
-- Bảng: LIS_SAMPLE_SERVICE (1 dòng = 1 dịch vụ xét nghiệm của 1 mẫu)
-- Ghi bởi API: api/LisSampleService/ApproveResult, api/LisSampleService/UnapproveResult
-- Không sửa view (frontend/MPS đọc qua api/LisSampleService/Get)
-- Ngày: 22/10/2026
-- =====================================================================

-- 1. Thêm cột
ALTER TABLE LIS_RS.LIS_SAMPLE_SERVICE ADD (
    SERVICE_APPROVER_LOGINNAME  VARCHAR2(50),
    SERVICE_APPROVER_USERNAME   VARCHAR2(100),
    SERVICE_APPROVE_TIME        NUMBER(19)
);

COMMENT ON COLUMN LIS_RS.LIS_SAMPLE_SERVICE.SERVICE_APPROVER_LOGINNAME IS 'Tài khoản người duyệt kết quả dịch vụ';
COMMENT ON COLUMN LIS_RS.LIS_SAMPLE_SERVICE.SERVICE_APPROVER_USERNAME IS 'Họ tên người duyệt kết quả dịch vụ';
COMMENT ON COLUMN LIS_RS.LIS_SAMPLE_SERVICE.SERVICE_APPROVE_TIME IS 'Thời gian duyệt kết quả dịch vụ (yyyyMMddHHmmss)';

-- 2. Kiểm tra sau khi chạy
SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, NULLABLE
FROM   ALL_TAB_COLUMNS
WHERE  OWNER = 'LIS_RS'
AND    TABLE_NAME = 'LIS_SAMPLE_SERVICE'
AND    COLUMN_NAME IN ('SERVICE_APPROVER_LOGINNAME', 'SERVICE_APPROVER_USERNAME', 'SERVICE_APPROVE_TIME');

-- 3. Rollback (chỉ chạy khi cần gỡ tính năng; sẽ mất dữ liệu người duyệt đã lưu)
-- ALTER TABLE LIS_RS.LIS_SAMPLE_SERVICE DROP (SERVICE_APPROVER_LOGINNAME, SERVICE_APPROVER_USERNAME, SERVICE_APPROVE_TIME);
