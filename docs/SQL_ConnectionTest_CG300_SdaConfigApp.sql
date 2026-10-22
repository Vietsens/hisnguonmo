-- ConnectionTest - In tem barcode qua may CG300: 2 key cau hinh phan mem (schema SDA)
-- Gia tri mac dinh rong => checkbox "In tem CG300" luon an. Dat gia tri theo tai khoan tai "Thiet lap cau hinh phan mem".

INSERT INTO SDA_CONFIG_APP (KEY, DEFAULT_VALUE, VALUE_TYPE, DESCRIPTION, APP_CODE, MODULE_LINKS)
SELECT 'CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER', NULL, 'string', 'Kết nối xét nghiệm - In tem qua máy CG300: đường dẫn thư mục ALL mà phần mềm CG300 đọc file (VD: D:\CG300\ALL). Để trống: không dùng CG300 (mặc định)', 'HIS', 'HIS.Desktop.Plugins.ConnectionTest'
  FROM DUAL
 WHERE NOT EXISTS (SELECT 1 FROM SDA_CONFIG_APP WHERE KEY = 'CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_FOLDER');

INSERT INTO SDA_CONFIG_APP (KEY, DEFAULT_VALUE, VALUE_TYPE, DESCRIPTION, APP_CODE, MODULE_LINKS)
SELECT 'CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME', NULL, 'string', 'Kết nối xét nghiệm - In tem qua máy CG300: tên tiến trình phần mềm điều khiển CG300 (không có .exe). Để trống: không kiểm tra tiến trình (mặc định)', 'HIS', 'HIS.Desktop.Plugins.ConnectionTest'
  FROM DUAL
 WHERE NOT EXISTS (SELECT 1 FROM SDA_CONFIG_APP WHERE KEY = 'CONFIG_KEY__HIS_DESKTOP__PLUGINS__CONNECTIONTEST__CG300_PROCESS_NAME');

COMMIT;
