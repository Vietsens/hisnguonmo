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
using System;

namespace HIS.Desktop.Plugins.Library.CheckRequireMediMate.Resources
{
    /// <summary>
    /// Thong diep cua thu vien. Chuoi tieng Viet nam trong Message.Lang.resx (resource TRUNG TINH, nhung vao DLL chinh),
    /// tieng Anh nam trong Message.Lang.en.resx (satellite en\...resources.dll).
    /// Nho co resource trung tinh nen thieu satellite van hien duoc tieng Viet, khong bi chuoi rong;
    /// ngoai ra moi chuoi con co gia tri mac dinh trong code de phong loi doc resource.
    /// </summary>
    internal class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.Library.CheckRequireMediMate.Resources.Message.Lang",
            System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>
        /// "Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm:\n{0}\nVui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại."
        /// {0} = danh sach dich vu, moi dong 1 dich vu.
        /// Viec 3353 - chot 22/09/2026: muc xu ly la CHAN (hop thong bao chi co nut OK) nen cau nay KHONG duoc hoi "co muon tiep tuc" -
        /// gia tri mac dinh duoi day phai trung voi Message.Lang.resx, vi no la cau hien ra khi doc resource that bai.
        /// </summary>
        internal static string DichVuChuaCoThuocVatTuDiKem
        {
            get
            {
                return GetValue("Library_CheckRequireMediMate__DichVuChuaCoThuocVatTuDiKem",
                    "Không kết thúc được. Dịch vụ chưa có thuốc, vật tư đi kèm:\n{0}\nVui lòng kê thuốc, vật tư đi kèm cho các dịch vụ trên rồi kết thúc lại.");
            }
        }

        /// <summary>Tieu de hop thoai</summary>
        internal static string ThongBao
        {
            get
            {
                return GetValue("Library_CheckRequireMediMate__ThongBao", "Thông báo");
            }
        }

        private static string GetValue(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(key, languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (!String.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultValue;
        }
    }
}
