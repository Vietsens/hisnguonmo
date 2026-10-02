-- =====================================================================
-- Viec 57944 (NTP): Tich hop nut Duyet va ky so - Duyet kham chuyen khoa
-- Plugin  : HIS.Desktop.Plugins.ApprovalExamSpecialist
-- Tai lieu: 57944_B_KyThuat_DuyetVaKySo_KhamChuyenKhoa_HoiChan.md
-- LUU Y   : CONFIG_CODE ben duoi la TAM - phai thay bang ma ke tiep
--           chua duoc su dung trong HIS_CONFIG truoc khi chay that.
--           Mac dinh de TRONG => giu nguyen hanh vi hien tai
--           ("Duyet va ky" chi ky to dieu tri Mps000062).
-- =====================================================================

INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption', '',
'Chọn phiếu được ký số khi bấm nút "Duyệt và ký (Ctrl K)" ở chức năng Duyệt khám chuyên khoa.' || CHR(13) || CHR(10) ||
'- Rỗng hoặc 1: Chỉ ký tờ điều trị (Mps000062) - giữ nguyên hành vi hiện tại.' || CHR(13) || CHR(10) ||
'- 2: Chỉ ký phiếu kết quả khám chuyên khoa (Mps000500).' || CHR(13) || CHR(10) ||
'- 3: Ký tờ điều trị (Mps000062), sau đó ký tiếp phiếu kết quả khám chuyên khoa (Mps000500).' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Phiếu chưa duyệt sẽ được duyệt trước; duyệt lỗi thì không mở ký.' || CHR(13) || CHR(10) ||
'Giá trị 3: không xác định được tờ điều trị (hoặc ký tờ điều trị bị hủy) thì vẫn ký phiếu kết quả.' || CHR(13) || CHR(10) ||
'Ký lại phiếu kết quả (Mps000500) không tự xóa văn bản ký cũ trên EMR.',
'01000', 'HIS.Desktop.Plugins.ApprovalExamSpecialist'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption');

COMMIT;
