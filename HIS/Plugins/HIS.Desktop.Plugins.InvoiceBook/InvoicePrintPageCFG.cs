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
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    public class InvoicePrintPageCFG
    {
        private const string CONFIG_KEY__SO_BAN_GHI_TRANG_DAU_TIEN_HIEN_THI_IN_HOA_DON = "CONFIG_KEY__SO_BAN_GHI_TRANG_DAU_TIEN_HIEN_THI_IN_HOA_DON";
        private const string CONFIG_KEY__SO_BAN_GHI_TRANG_TIEP_THEO_HIEN_THI_IN_HOA_DON = "CONFIG_KEY__SO_BAN_GHI_TRANG_TIEP_THEO_HIEN_THI_IN_HOA_DON";

        public static long SoBanGhiTrangDauTien { get; set; }
        public static long SoBanGhiTrangTiepTheo { get; set; }

        public static void LoadConfig()
        {
            try
            {
                SoBanGhiTrangDauTien = ConfigApplicationWorker.Get<long>(CONFIG_KEY__SO_BAN_GHI_TRANG_DAU_TIEN_HIEN_THI_IN_HOA_DON);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                SoBanGhiTrangTiepTheo = ConfigApplicationWorker.Get<long>(CONFIG_KEY__SO_BAN_GHI_TRANG_TIEP_THEO_HIEN_THI_IN_HOA_DON);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
