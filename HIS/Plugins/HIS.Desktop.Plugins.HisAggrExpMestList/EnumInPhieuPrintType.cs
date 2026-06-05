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
namespace HIS.Desktop.Plugins.HisAggrExpMestList
{
    /// <summary>
    /// Slip types of the 'In Phiếu' auto-print dropdown.
    /// Mirrors the 'In ẩn' dropdown of the Detail screen (AggrExpMestDetail).
    /// Values 1-4 map directly to the printKey consumed by plugin
    /// HIS.Desktop.Plugins.AggrExpMestPrintFilter; value 5 is printed
    /// directly via PrintAggrExpMestProcessor (Mps000262).
    /// </summary>
    internal enum EnumInPhieuPrintType
    {
        /// <summary>Phiếu tra đổi thuốc — AggrExpMestPrintFilter printKey 1 (Mps000047)</summary>
        PhieuTraDoiThuoc = 1,

        /// <summary>Phiếu tổng hợp — AggrExpMestPrintFilter printKey 2 (Mps000046)</summary>
        PhieuTongHop = 2,

        /// <summary>Phiếu lĩnh thuốc, vật tư — AggrExpMestPrintFilter printKey 3 (Mps000049)</summary>
        PhieuLinhThuocVatTu = 3,

        /// <summary>Phiếu lĩnh theo bệnh nhân — AggrExpMestPrintFilter printKey 4 (Mps000235)</summary>
        PhieuLinhTheoBenhNhan = 4,

        /// <summary>Phiếu công khai theo bệnh nhân — PrintAggrExpMestProcessor (Mps000262)</summary>
        PhieuCongKhaiTheoBenhNhan = 5
    }
}
