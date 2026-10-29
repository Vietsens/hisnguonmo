-- =====================================================================
-- De trong khu vuc khi kiem tra the BHYT cua dot dieu tri cu
-- Plugin  : HIS.Desktop.Plugins.RegisterV2
-- Tai lieu: B_KyThuat_RegisterV2_DeTrongKhuVucTheBHYT.md
-- LUU Y   : CONFIG_CODE ben duoi la TAM - phai thay bang ma ke tiep
--           chua duoc su dung trong HIS_CONFIG truoc khi chay that.
--           Mac dinh de TRONG => giu nguyen hanh vi hien tai
--           (lay lai khu vuc cua dot kham chua benh truoc).
-- =====================================================================

INSERT INTO HIS_CONFIG (KEY, DEFAULT_VALUE, DESCRIPTION, CONFIG_CODE, MODULE_LINKS)
SELECT 'HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment', '',
'Để trống khu vực khi kiểm tra thẻ BHYT của đợt điều trị cũ (màn hình Tiếp đón).' || CHR(13) || CHR(10) ||
'- 1: Khi kiểm tra thẻ BHYT trên cổng BHXH, khu vực chỉ lấy theo kết quả trả về của cổng. Thẻ không có mã khu vực thì để trống ô "Khu vực", không lấy khu vực của đợt khám chữa bệnh trước. Các thông tin khác của đợt khám chữa bệnh trước vẫn được lấy lại như hiện tại.' || CHR(13) || CHR(10) ||
'- Khác 1 hoặc để trống: Giữ nguyên hành vi hiện tại (lấy lại khu vực của đợt khám chữa bệnh trước).' || CHR(13) || CHR(10) ||
CHR(13) || CHR(10) ||
'Chỉ áp dụng khi cổng BHXH trả về thông tin thẻ. Cổng lỗi hoặc không kết nối được thì giữ nguyên như hiện tại.',
'01000', 'HIS.Desktop.Plugins.RegisterV2'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM HIS_CONFIG WHERE KEY = 'HIS.Desktop.Plugins.RegisterV2.IsClearLiveAreaWhenCheckHeinCardOldTreatment');

COMMIT;
