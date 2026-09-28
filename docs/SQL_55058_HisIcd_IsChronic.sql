-- =====================================================================
-- Viec 55058 - Danh dau ma benh ICD la benh man tinh
-- Them cot HIS_ICD.IS_CHRONIC + bo sung vao view V_HIS_ICD
-- Plugin  : HIS.Desktop.Plugins.HisIcd (checkbox "Benh man tinh")
--           HIS.Desktop.Plugins.ExamServiceReqExecute (canh bao khi ket thuc dieu tri
--           o phong kham: ICD chinh man tinh nhung chua tich "Man tinh")
-- Kieu cot theo mau IS_INFECTIOUS / IS_DEATH_CAUSE_ONLY (EDMX: number, Precision=2).
-- 1 = la benh man tinh; null/0 = khong. Khong NOT NULL, khong default.
-- =====================================================================

ALTER TABLE HIS_ICD ADD (IS_CHRONIC NUMBER(2,0));

COMMENT ON COLUMN HIS_ICD.IS_CHRONIC IS 'La benh man tinh (1 = co, null/0 = khong). Canh bao khi BS phong kham ket thuc dieu tri ma ICD chinh man tinh nhung chua tich Man tinh';

-- =====================================================================
-- V_HIS_ICD: bo sung cot IS_CHRONIC ngay sau IS_INFECTIOUS
-- (EFMODEL V_HIS_ICD: ... IS_DEATH_CAUSE_ONLY, IS_INFECTIOUS, IS_CHRONIC, GENDER_CODE, ...)
--
-- B1. Lay DDL view hien tai:
--   SELECT DBMS_METADATA.GET_DDL('VIEW', 'V_HIS_ICD') FROM DUAL;
-- B2. Trong danh sach cot SELECT, sau dong "ICD.IS_INFECTIOUS," them:
--       ICD.IS_CHRONIC,
--   (neu view dung ICD.* thi chi can CREATE OR REPLACE lai de Oracle nhan cot moi)
-- B3. Chay lai CREATE OR REPLACE VIEW V_HIS_ICD ... voi DDL da sua.
-- =====================================================================

COMMIT;

-- =====================================================================
-- Kiem tra sau khi chay
-- =====================================================================
-- SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_PRECISION, NULLABLE
--   FROM USER_TAB_COLUMNS
--  WHERE TABLE_NAME IN ('HIS_ICD', 'V_HIS_ICD')
--    AND COLUMN_NAME = 'IS_CHRONIC';
--
-- SELECT OBJECT_NAME, STATUS FROM USER_OBJECTS WHERE OBJECT_NAME = 'V_HIS_ICD';  -- phai VALID
