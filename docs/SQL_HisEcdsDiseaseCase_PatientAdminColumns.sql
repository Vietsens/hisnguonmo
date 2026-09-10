-- ============================================================================
-- ECDS Báo cáo truyền nhiễm — bổ sung cột LƯU THÔNG TIN HÀNH CHÍNH BỆNH NHÂN
-- vào bảng HIS_ECDS_DISEASE_CASE.
--
-- Lý do: khi mở form Báo cáo (từ màn Đồng bộ -> "Xem"), người dùng điền/sửa các
-- trường hành chính còn thiếu của bệnh nhân (họ tên, ngày sinh, giới tính, dân
-- tộc, nghề nghiệp, CCCD, SĐT, địa chỉ hiện nay/thường trú). Hiện các trường này
-- CHỈ đọc từ V_HIS_PATIENT, KHÔNG có cột trong HIS_ECDS_DISEASE_CASE -> lưu xong
-- mở lại bị mất. Thêm cột để lưu "bản khai báo cho ECDS", đọc lại khi mở form.
--
-- Sau khi chạy SQL: BACKEND regen MOS.EFMODEL (HIS_ECDS_DISEASE_CASE +
-- V_HIS_ECDS_DISEASE_CASE) rồi FRONTEND mới map được (BuildCaseEntity/MapFromSavedCase).
-- ============================================================================

ALTER TABLE HIS_ECDS_DISEASE_CASE ADD (
    PATIENT_FULL_NAME     VARCHAR2(255),   -- HOVATEN (họ và tên khai báo ECDS)
    PATIENT_BIRTH_DATE    NUMBER(14),      -- NGAYSINH (yyyyMMddHHmmss)
    PATIENT_GENDER_CODE   VARCHAR2(10),    -- MAGIOITINH ("M"/"F")
    PATIENT_ETHNIC_CODE   VARCHAR2(20),    -- MADANTOC (mã dân tộc)
    PATIENT_CAREER_CODE   VARCHAR2(20),    -- MANGHENGHIEP (mã nghề nghiệp cổng)
    PATIENT_CCCD          VARCHAR2(20),    -- SOCCCD/CMND
    PATIENT_PHONE         VARCHAR2(20),    -- SODIENTHOAI bệnh nhân
    CUR_PROVINCE_CODE     VARCHAR2(20),    -- Tỉnh HIỆN NAY (mã)
    CUR_COMMUNE_CODE      VARCHAR2(20),    -- Xã HIỆN NAY (mã)
    CUR_ADDRESS           VARCHAR2(500),   -- Địa chỉ chi tiết HIỆN NAY
    RES_PROVINCE_CODE     VARCHAR2(20),    -- Tỉnh THƯỜNG TRÚ (mã)
    RES_COMMUNE_CODE      VARCHAR2(20),    -- Xã THƯỜNG TRÚ (mã)
    RES_ADDRESS           VARCHAR2(500)    -- Địa chỉ chi tiết THƯỜNG TRÚ
);

COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_FULL_NAME   IS 'Họ tên BN khai báo ECDS (HOVATEN)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_BIRTH_DATE  IS 'Ngày sinh BN (NGAYSINH, yyyyMMddHHmmss)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_GENDER_CODE IS 'Mã giới tính ECDS (MAGIOITINH: M/F)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_ETHNIC_CODE IS 'Mã dân tộc (MADANTOC)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_CAREER_CODE IS 'Mã nghề nghiệp cổng (MANGHENGHIEP)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_CCCD        IS 'Số CCCD/CMND (SOCCCD)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.PATIENT_PHONE       IS 'SĐT bệnh nhân (SODIENTHOAI)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.CUR_PROVINCE_CODE   IS 'Tỉnh hiện nay (mã)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.CUR_COMMUNE_CODE    IS 'Xã hiện nay (mã)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.CUR_ADDRESS         IS 'Địa chỉ chi tiết hiện nay';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.RES_PROVINCE_CODE   IS 'Tỉnh thường trú (mã)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.RES_COMMUNE_CODE    IS 'Xã thường trú (mã)';
COMMENT ON COLUMN HIS_ECDS_DISEASE_CASE.RES_ADDRESS         IS 'Địa chỉ chi tiết thường trú';

-- LƯU Ý: cập nhật cả VIEW V_HIS_ECDS_DISEASE_CASE để select các cột mới (frontend
-- đọc lại ca đã lưu qua GetView). Nếu view dùng SELECT * thì regen là đủ.
