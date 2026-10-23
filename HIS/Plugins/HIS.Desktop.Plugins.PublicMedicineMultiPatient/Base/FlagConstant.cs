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
namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base
{
    internal class FlagConstant
    {
        /// <summary>
        /// Value 1 of the yes/no columns: IS_NO_EXECUTE (không thực hiện), IS_EXPEND (hao phí),
        /// IS_CHEMICAL_SUBSTANCE (hóa chất). IMSys.DbConfig only has IS_ACTIVE/IS_DELETE flags.
        /// </summary>
        internal const short IS_TRUE = 1;
    }

    /// <summary>
    /// Values of SingleKeys.IsOderMedicine read by MPS.Processor.Mps000522 (same as Mps000088)
    /// </summary>
    internal class SortOrderConstant
    {
        /// <summary>Theo thiết lập: by type, medicine group, use form, NUM_ORDER, name</summary>
        internal const int BY_SETTING = 1;
        /// <summary>Sắp xếp theo tên</summary>
        internal const int BY_NAME = 2;
    }

    /// <summary>
    /// Values of GlobalVariables.CheDoInChoCacChucNangTrongPhanMem (chế độ in của phần mềm)
    /// </summary>
    internal class PrintModeConstant
    {
        /// <summary>In ngay, không mở xem trước</summary>
        internal const int PRINT_NOW = 2;
    }
}
