-- =====================================================================
-- Ket noi xet nghiem — Cau hinh BAT/TAT Duyet / Huy duyet ket qua theo tung dich vu
-- Plugin  : HIS.Desktop.Plugins.ConnectionTest
-- DB      : HIS (MOS) — bang HIS_CONFIG
-- Tai lieu: A_NghiepVu_ConnectionTest_DuyetKetQua.docx, B_KyThuat_ConnectionTest_DuyetKetQua.docx
-- LUU Y   : CONFIG_CODE ben duoi la TAM - phai thay bang ma ke tiep
--           chua duoc su dung trong HIS_CONFIG truoc khi chay that.
--           Mac dinh de TRONG => tat tinh nang, man hinh giu nguyen nghiep vu cu.
-- Ngay    : 27/10/2026
-- =====================================================================

INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.ConnectionTest.IsApproveResultByService', '',
'Bật chức năng Duyệt / Hủy duyệt kết quả theo từng dịch vụ xét nghiệm tại màn hình Kết nối xét nghiệm.' || CHR(13) || CHR(10) ||
'- 1: Bật. Hiển thị nút "Duyệt KQ / Hủy duyệt KQ" ở khu vực kết quả (áp dụng cho các dịch vụ được tích),' || CHR(13) || CHR(10) ||
'     cho phép bật cột "Người duyệt KQ", "TG duyệt" trên bảng chỉ số, khóa sửa kết quả của dịch vụ đã duyệt.' || CHR(13) || CHR(10) ||
'     Mỗi dịch vụ lưu người duyệt (tài khoản đăng nhập) và thời gian duyệt riêng.' || CHR(13) || CHR(10) ||
'- Khác 1 hoặc để trống: Tắt. Ẩn nút và 2 cột, màn hình giữ nguyên nghiệp vụ cũ.' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Điều kiện khi bật: DB LIS đã có 3 cột LIS_SAMPLE_SERVICE.SERVICE_APPROVER_LOGINNAME / SERVICE_APPROVER_USERNAME / SERVICE_APPROVE_TIME' || CHR(13) || CHR(10) ||
'(script SQL_ConnectionTest_DuyetKetQuaTheoDichVu.sql) và máy chủ LIS có API api/LisSampleService/ApproveResult, UnapproveResult.' || CHR(13) || CHR(10) ||
'Chức năng Duyệt KQ cấp mẫu (cột Duyệt KQ trên danh sách mẫu, Duyệt KQ theo lô) không phụ thuộc cấu hình này.',
'01000', 'HIS.Desktop.Plugins.ConnectionTest'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.ConnectionTest.IsApproveResultByService');

-- Bat tinh nang cho vien (chay khi can):
-- UPDATE HIS_CONFIG SET VALUE = '1' WHERE KEY = 'HIS.Desktop.Plugins.ConnectionTest.IsApproveResultByService';

-- Kiem tra
SELECT KEY, VALUE, DEFAULT_VALUE, IS_ACTIVE, BRANCH_ID, CONFIG_CODE
FROM   HIS_CONFIG
WHERE  KEY = 'HIS.Desktop.Plugins.ConnectionTest.IsApproveResultByService';
