/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

namespace HIS.Desktop
{
    internal class AppConfigKeys
    {
        #region Public key
        public const string CONFIG_KEY__HIS_DESKTOP__AUTO_TOKEN_LOGOUT_WHILE_CLOSE_APPLICATION = "CONFIG_KEY__HIS_DESKTOP__AUTO_TOKEN_LOGOUT_WHILE_CLOSE_APPLICATION";

        //Cấu hình chế độ bặt tắt lưu dữ liệu cache về máy trạm. Đặt 1 là bật, giá trịkhác là tắt.
        public const string CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL = "CONFIG_KEY__HIS_DESKTOP_IS_USE_CACHE_LOCAL";

        //Thời gian tự động gọi đến service đồng bộ tất cả dữ liệu giữa server và DB local
        public const string CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_ALL = "CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_ALL";

        //TThời gian đồng bộ từ DB Sqlite về Ram
        public const string CONFIG_KEY__HIS_DESKTOP__TIME_CACHE_SYNC_TO_RAM = "CONFIG_KEY__HIS_DESKTOP__TIME_CACHE_SYNC_TO_RAM";

        //Thời gian tự động gọi đến service đồng bộ dữ liệu giữa server và DB local theo cấu hình cache monitor
        public const string CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_BY_CACHE_MONITOR = "CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_BY_CACHE_MONITOR";

        //Thời gian tự động gọi thông báo
        public const string CONFIG_KEY__HIS_DESKTOP__AUTO_SHOW_NOTIFY = "CONFIG_KEY__HIS_DESKTOP__AUTO_SHOW_NOTIFY";

        //Option hiển thị thông báo
        public const string CONFIG_KEY__HIS_DESKTOP__OPTION_FORM_NOTIFY = "CONFIG_KEY__HIS_DESKTOP__OPTION_FORM_NOTIFY";

        //Cấu hình có lưu token phiên làm việc vào registry phục vụ cho việc đăng nhập lần đầu tiên, nếu lần sau mở phần mềm token vẫn còn hiệu lực thì sẽ tự động đăng nhập luôn
        //public const string CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN = "CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN";

        public const string CONFIG_KEY__HIS_IS_USE_REDIS_CACHE_SERVER = "CONFIG_KEY__HIS_IS_USE_REDIS_CACHE_SERVER";

        public const string CONFIG_KEY__HIS_DESKTOP_IS_AUTO_SYNC_DATA_IN_DB_CACHE_TO_RAM_AFTER_LOGIN = "CONFIG_KEY__HIS_DESKTOP_IS_AUTO_SYNC_DATA_IN_DB_CACHE_TO_RAM_AFTER_LOGIN";

        public const string CONFIG_KEY__IS_USE_MPS_SERVER = "CONFIG_KEY__HIS_DESKTOP__IS_USE_MPS_SERVER";

        //Thời gian chạy tiến trình LogAction ghi dug lượng ram phần mềm sử dụng
        public const string CONFIG_KEY__HIS_DESKTOP__TIME_LOG_ACTION = "CONFIG_KEY__HIS_DESKTOP__TIME_LOG_ACTION";

        //Thời gian chạy tiến trình LogAction ghi dug lượng ram phần mềm sử dụng
        public const string CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS_CFG = "CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS";
        #endregion
    }
}
