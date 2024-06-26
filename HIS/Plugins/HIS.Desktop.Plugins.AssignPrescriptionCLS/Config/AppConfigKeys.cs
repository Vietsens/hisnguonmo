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

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS
{
    internal class AppConfigKeys
    {

        //Cấu hình có lưu token phiên làm việc vào registry phục vụ cho việc đăng nhập lần đầu tiên, nếu lần sau mở phần mềm token vẫn còn hiệu lực thì sẽ tự động đăng nhập luôn
        public const string CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN = "CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN";
        //Cấu hình chế độ bặt tắt lưu dữ liệu cache về máy trạm. Đặt 1 là bật, giá trịkhác là tắt.
        public const string CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL = "CONFIG_KEY__HIS_DESKTOP_IS_USE_CACHE_LOCAL";


        #region Public key
        internal const string CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI = "CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_CP_NGOAI_GOI";
        internal const string CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI = "CONFIG_KEY__MODULE_CHI_DINH_DICH_VU__AN_HIEN_HAO_PHI";
        internal const string CONFIG_KEY__CHI_DINH_NHANH_THUOC_VAT_TU = "CONFIG_KEY__CHI_DINH_NHANH_THUOC_VAT_TU";

        /// <summary>
        /// - Cấu hình chế độ mặc định chọn kho trong chức năng Kê thuốc
        ///- Đặt là 1: là chọn tất cả các kho
        ///- Mặc định: không chọn kho
        /// </summary>
        internal const string CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO = "CONFIG_KEY__CHE_DO_KE_DON_THUOC__MOT_HOAC_NHIEU_KHO";
        internal const string CONFIG_KEY__CHE_DO_PHIEU_LINH_THUOC_GAY_NGHIEN_HUONG_TAM_THAN = "CONFIG_KEY__CHE_DO_PHIEU_LINH_THUOC_GAY_NGHIEN_HUONG_TAM_THAN";
        internal const string CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT = "CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__ISVISIBLE_REMEDY_COUNT";
        internal const string CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW = "CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW";
        internal const string CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__FOCUS_MEDICINE_DEFAULT = "CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__FOCUS_MEDICINE_DEFAULT";

        internal const string CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT = "CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT";

        internal const string CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY = "CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__DEFAULT_NUM_OF_DAY";

        #endregion
    }
}
