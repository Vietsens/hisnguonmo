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
using System.Resources;

namespace HIS.Desktop.Plugins.HisTreatmentType.Resource
{
    /// <summary>
    /// PT-48590: truy xuat tep ngon ngu cua plugin.
    /// LUU Y: plugin nay lech chuan — thu muc tai nguyen ten so it (Resource) va ten goi
    /// (HIS.Desktop.Plugins.TreatmentType) KHAC ten vung ma nguon (HIS.Desktop.Plugins.HisTreatmentType).
    /// Chuoi dinh danh duoi day phai giu nguyen theo TEN GOI, sai se khong loi bien dich
    /// ma chi tra ve chuoi rong.
    /// </summary>
    internal class ResourceLangManager
    {
        private const string RESOURCE_NAME = "HIS.Desktop.Plugins.TreatmentType.Resource.Lang";

        private static ResourceManager languageResource =
            new ResourceManager(RESOURCE_NAME, typeof(ResourceLangManager).Assembly);

        private static string GetValue(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    languageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Thêm (Ctrl N)</summary>
        internal static string BtnAddText
        {
            get { return GetValue("HisTreatmentTypeForm.btnAdd.Text"); }
        }

        /// <summary>Sửa (Ctrl S)</summary>
        internal static string BtnSaveText
        {
            get { return GetValue("HisTreatmentTypeForm.btnSave.Text"); }
        }

        /// <summary>Mã diện điều trị không được vượt quá {0} byte</summary>
        internal static string MaDienDieuTriVuotQuaGioiHan
        {
            get { return GetValue("HisTreatmentTypeForm.MaDienDieuTriVuotQuaGioiHan"); }
        }

        /// <summary>Tên diện điều trị không được vượt quá {0} byte</summary>
        internal static string TenDienDieuTriVuotQuaGioiHan
        {
            get { return GetValue("HisTreatmentTypeForm.TenDienDieuTriVuotQuaGioiHan"); }
        }
    }
}
