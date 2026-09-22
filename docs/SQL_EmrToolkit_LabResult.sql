-- =====================================================================
-- Lien thong ket qua xet nghiem qua cong EMRToolkit
-- Phan viec : tien trinh nen day KQXN len cong (MOS.QuartzScheduler)
-- Tai lieu  : docs/PhanTich_EMRToolkit_KQXN_ExamServiceReqExecute.md
-- Source MOS: E:\svn\IMSys\BACKEND\MOS
--
-- Phuong an luu tru da chot: KHONG tao bang moi, chi bo sung cot tren
-- bang co san.
--   PHAN 1 - HIS_SERVICE_REQ : trang thai day cua phieu (tien trinh quet theo cot nay)
--   PHAN 2 - HIS_SERE_SERV   : RECORD_ID cong tra ve cho tung dich vu can lam sang
--   PHAN 2B- HIS_TEST_INDEX : ma dung chung cua chi so (COMMON_CODE) de gui len cong
--   PHAN 3 - HIS_CONFIG      : MOT khoa duy nhat (ket noi + loai CLS + so ngay quet)
--
-- Dat ten cot bam theo loi CSDL4750_* / HOC_CHECKIN_* dang co tren HIS_TREATMENT.
-- Ten cot KHONG chua chu LAB/TEST: dung chung cho moi loai can lam sang (XN lan nay,
-- sau nay them CDHA / PTTT / TDCN khong phai bo sung cot).
-- Them cot NULL tren Oracle 11g+ la thao tac metadata (khong ghi lai toan bang),
-- tuy vay HIS_SERE_SERV rat lon nen van nen chay ngoai gio cao diem.
-- =====================================================================

-- =====================================================================
-- PHAN 1 - Trang thai day tren phieu can lam sang
-- =====================================================================

ALTER TABLE HIS_SERVICE_REQ ADD (
  EMR_TOOLKIT_RESULT NUMBER(1,0),
  EMR_TOOLKIT_DESC   VARCHAR2(1000 CHAR),
  EMR_TOOLKIT_TIME   NUMBER(14,0)
);

COMMENT ON COLUMN HIS_SERVICE_REQ.EMR_TOOLKIT_RESULT IS 'Trang thai day ket qua CLS len cong EMRToolkit: NULL/0=Chua gui, 1=Thanh cong, 2=That bai (tien trinh nen quet lai)';
COMMENT ON COLUMN HIS_SERVICE_REQ.EMR_TOOLKIT_DESC   IS 'Ly do that bai gan nhat khi day len cong EMRToolkit';
COMMENT ON COLUMN HIS_SERVICE_REQ.EMR_TOOLKIT_TIME   IS 'Thoi diem day gan nhat - dang yyyyMMddHHmmss';

-- =====================================================================
-- PHAN 2 - RECORD_ID cong tra ve theo tung dich vu can lam sang
-- Moi lan Submit len cong tuong ung 1 dich vu ky thuat -> 1 RecordId,
-- nen day la hat du lieu dung de luu.
-- =====================================================================

ALTER TABLE HIS_SERE_SERV ADD (
  EMR_TOOLKIT_RECORD_ID   VARCHAR2(50 CHAR),
  EMR_TOOLKIT_VALID_UNTIL NUMBER(14,0),
  EMR_TOOLKIT_TIME        NUMBER(14,0)
);

COMMENT ON COLUMN HIS_SERE_SERV.EMR_TOOLKIT_RECORD_ID   IS 'Ma ban ghi cong EMRToolkit tra ve (Data.RecordId cua api/LabResult/Submit)';
COMMENT ON COLUMN HIS_SERE_SERV.EMR_TOOLKIT_VALID_UNTIL IS 'Het hieu luc lien thong do cong tu tinh - dang yyyyMMddHHmmss. Con hieu luc thi tien trinh khong gui lai';
COMMENT ON COLUMN HIS_SERE_SERV.EMR_TOOLKIT_TIME        IS 'Thoi diem day dich vu nay len cong - dang yyyyMMddHHmmss';

-- =====================================================================
-- PHAN 2B - Ma dung chung cua chi so xet nghiem
-- Cong dinh danh du lieu bang MA DUNG CHUNG:
--   MaDichVu    -> dung HIS_SERVICE.HEIN_SERVICE_BHYT_CODE CO SAN
--                  (ma ky thuat dung chung, chinh ma da khai voi cong BHYT)
--                  -> KHONG them cot cho HIS_SERVICE
--   MaDungChung -> COT MOI HIS_TEST_INDEX.COMMON_CODE,
--                  quan tri khai o HIS.Desktop.Plugins.HisTestIndex
--
-- LUU Y: khac cac cot EMR_TOOLKIT_* o tren, cot COMMON_CODE BAT BUOC duoc
-- bo sung vao MOS.EFMODEL (EDMX + entity + view entity) va phat hanh lai
-- MOS.EFMODEL.dll cho frontend, vi plugin danh muc gan truc tiep vao entity.
-- =====================================================================

ALTER TABLE HIS_TEST_INDEX ADD (
  COMMON_CODE VARCHAR2(50 CHAR)
);

COMMENT ON COLUMN HIS_TEST_INDEX.COMMON_CODE IS 'Ma chi so xet nghiem theo danh muc dung chung - gui truong MaDungChung khi lien thong';

COMMIT;

-- Sau khi them cot: BO SUNG COMMON_CODE vao cau SELECT cua view V_HIS_TEST_INDEX,
-- vi frontend nap danh muc qua BackendDataWorker tu view va luoi danh muc bind theo view.
-- (Lay dinh nghia view hien tai roi CREATE OR REPLACE, khong viet lai tu dau:
--  SELECT TEXT FROM USER_VIEWS WHERE VIEW_NAME = 'V_HIS_TEST_INDEX';)

-- KHONG tao index cho COMMON_CODE: chi doc theo khoa chinh danh muc va de hien thi.

-- KHONG tao index moi tren HIS_SERE_SERV: doc theo ID / SERVICE_REQ_ID da co index.

-- Index cho buoc loc co tren HIS_SERVICE_REQ: TUY CHON.
-- Truy van quet dan huong bang HIS_TREATMENT.OUT_TIME (da co index) roi join
-- HIS_SERVICE_REQ theo TREATMENT_ID (da co index) nen khong can index moi.
-- Chi them khi DBA do thay cham:
-- CREATE INDEX IDX_HIS_SERVICE_REQ_EMR_TK ON HIS_SERVICE_REQ (NVL(EMR_TOOLKIT_RESULT, 0), TREATMENT_ID);

-- =====================================================================
-- PHAN 3 - Cau hinh HIS_CONFIG
-- Thay <BaseUrl>, <TaiKhoan>, <MatKhau> bang gia tri thuc te truoc khi chay.
-- Cot cua HIS_CONFIG co the khac giua cac ban - doi chieu truoc khi chay.
-- =====================================================================

-- CHI MOT KHOA duy nhat, cac tham so la cac vi tri phan tach boi |
-- (cung quy chuan voi MOS.HIS_KSK_SYNC.HSSK_HOC_2062_CONNECTION_INFO).
-- Khoa nay DA TON TAI cho luong import EMR: vi tri 0-3 giu nguyen, chi bo sung
-- vi tri 4-8 nen luong import EMR khong bi anh huong.
--
-- Dinh dang day du:
--   <BaseUrl>|<TaiKhoan>|<MatKhau>|<IDMauPhieu>|<SyncTypes>|<ViewTypes>|<ScanDayNumber>|<MaCskcb>|<TimeoutSecond>
--
--   vi tri 0 BaseUrl       BAT BUOC - dia chi cong, bo dau / cuoi
--   vi tri 1 TaiKhoan      BAT BUOC - dung chung cho gui (BE) va tra cuu (FE)
--   vi tri 2 MatKhau       BAT BUOC
--   vi tri 3 IDMauPhieu    cua luong import EMR san co (lien thong CLS khong dung), trong -> 524
--   vi tri 4 SyncTypes     loai CLS duoc DAY len cong, phan tach ','. Trong -> khong day.
--                          Nhan: XN (xet nghiem), CDHA, TDCN, PTTT. Lan nay: XN
--   vi tri 5 ViewTypes     loai CLS duoc XEM/tra cuu tren Frontend, phan tach ','.
--                          Trong -> an nut EMRToolkit va tab xem. Lan nay: XN
--   vi tri 6 ScanDayNumber so ngay quet theo HIS_TREATMENT.OUT_TIME.
--                          Trong/sai/<=0 -> 3 ngay. Lon hon 90 -> tu kep con 90.
--                          BAT BUOC co moc thoi gian nay de khong quet toan bo CSDL.
--   vi tri 7 MaCskcb       ghi de ma CSKCB khi gui/tra cuu. Trong -> lay theo token
--   vi tri 8 TimeoutSecond timeout HTTP, trong -> 120 giay (tai PDF lau hon goi JSON)
--
-- Thieu cac vi tri phia sau KHONG phai loi -> dung gia tri mac dinh.
-- Nhan loai chua co builder trong ma nguon thi bi bo qua kem log Warn.
-- Bat them loai ve sau: chi sua cau hinh, vi du ...|XN,CDHA|XN,CDHA|3||120

-- Truong hop CHUA co khoa:
-- INSERT INTO HIS_CONFIG (ID, CONFIG_CODE, CONFIG_NAME, VALUE, IS_ACTIVE)
-- VALUES (HIS_CONFIG_SEQ.NEXTVAL, 'HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo',
--         'Cong EMRToolkit: dia chi|tai khoan|mat khau|IDMauPhieu|loai CLS day|loai CLS xem|so ngay quet|ma CSKCB|timeout giay',
--         '<BaseUrl>|<TaiKhoan>|<MatKhau>|524|XN|XN|3||120', 1);

-- Truong hop DA co khoa (dang la url|user|pass|IDMauPhieu) -> chi noi them 5 vi tri:
-- UPDATE HIS_CONFIG
--    SET VALUE = VALUE || '|XN|XN|3||120'
--  WHERE CONFIG_CODE = 'HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo';
-- (Kiem tra truoc: SELECT VALUE FROM HIS_CONFIG WHERE CONFIG_CODE = 'HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo';
--  neu gia tri hien tai chua co vi tri 3 thi them '|524' vao truoc: VALUE || '|524|XN|XN|3||120')

-- =====================================================================
-- PHAN 4 - Cau hinh chu ky tien trinh: MOS.API/Web.config (khong phai DB)
-- =====================================================================
-- <add key="MOS.API.Scheduler.SyncEmrToolkitSubclinical" value="600000" />
-- 600000 ms = 10 phut. Gia tri 0 hoac de trong -> KHONG khoi dong tien trinh.

-- =====================================================================
-- Kiem tra sau khi chay
-- =====================================================================
-- SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE
--   FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'HIS_SERVICE_REQ'
--    AND COLUMN_NAME LIKE 'EMR_TOOLKIT%' ORDER BY COLUMN_ID;
--
-- SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE
--   FROM USER_TAB_COLUMNS WHERE TABLE_NAME = 'HIS_SERE_SERV'
--    AND COLUMN_NAME LIKE 'EMR_TOOLKIT%' ORDER BY COLUMN_ID;
--
-- SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH FROM USER_TAB_COLUMNS
--  WHERE COLUMN_NAME = 'COMMON_CODE'
--    AND TABLE_NAME IN ('HIS_TEST_INDEX','V_HIS_TEST_INDEX');
--
-- Dem danh muc chua khai ma dung chung (se phai dung fallback khi day):
-- SELECT COUNT(*) FROM HIS_TEST_INDEX WHERE COMMON_CODE IS NULL AND NVL(IS_DELETE,0) <> 1;
--
-- Dem so phieu tien trinh se quet o chu ky tiep theo (thay :DAY_NUMBER):
-- SELECT COUNT(*) FROM HIS_SERVICE_REQ REQ
--   JOIN HIS_TREATMENT TREA ON TREA.ID = REQ.TREATMENT_ID
--  WHERE REQ.SERVICE_REQ_TYPE_ID = <ID__TEST>
--    AND REQ.SERVICE_REQ_STT_ID  = <ID__HT>
--    AND (REQ.IS_DELETE IS NULL OR REQ.IS_DELETE <> 1)
--    AND NVL(REQ.EMR_TOOLKIT_RESULT, 0) IN (0, 2)
--    AND TREA.OUT_TIME IS NOT NULL
--    AND TREA.OUT_TIME >= TO_NUMBER(TO_CHAR(SYSDATE - :DAY_NUMBER, 'YYYYMMDD') || '000000');

-- =====================================================================
-- Rollback (chi dung khi can go bo)
-- =====================================================================
-- DROP INDEX IDX_HIS_SERVICE_REQ_EMR_TK;
-- ALTER TABLE HIS_SERVICE_REQ DROP (EMR_TOOLKIT_RESULT, EMR_TOOLKIT_DESC, EMR_TOOLKIT_TIME);
-- ALTER TABLE HIS_SERE_SERV DROP (EMR_TOOLKIT_RECORD_ID, EMR_TOOLKIT_VALID_UNTIL, EMR_TOOLKIT_TIME);
-- ALTER TABLE HIS_TEST_INDEX DROP (COMMON_CODE);
-- (go bo cot COMMON_CODE cung phai bo khoi view V_HIS_TEST_INDEX va khoi MOS.EFMODEL)
