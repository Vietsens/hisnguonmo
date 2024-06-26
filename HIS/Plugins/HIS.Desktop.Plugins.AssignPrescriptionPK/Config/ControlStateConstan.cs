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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Config
{
    internal class ControlStateConstan
    {
        /// <summary>
        /// In
        /// </summary>
        internal const string chkPrint = "chkPrint";

        /// <summary>
        /// Check trạng thái đơn phòng khám
        /// </summary>
        internal const string chkSign = "chkSign";
        /// <summary>
        /// Check trạng thái đơn tủ trực
        /// </summary>
        internal const string chkSignForDTT = "chkSignForDTT";
        /// <summary>
        /// Check trạng thái đơn điều trị
        /// </summary>   
        internal const string chkSignForDDT = "chkSignForDDT";
        internal const string chkPreviewBeforePrint = "chkPreviewBeforePrint";
        internal const string chkEyeInfo = "chkEyeInfo";
        internal const string AdviseFormData = "AdviseFormData";
        internal const string chkNotShowExpMestTypeDTT = "chkNotShowExpMestTypeDTT";
        internal const string lcgChongChiDinhInfo = "lcgChongChiDinhInfo";
        internal const string lcgDHSTInfo = "lcgDHSTInfo";

        /// <summary>
        /// check trạng thái PDDT
        /// </summary>
        internal const string chkPDDT = "chkPDDT";
    }
}
