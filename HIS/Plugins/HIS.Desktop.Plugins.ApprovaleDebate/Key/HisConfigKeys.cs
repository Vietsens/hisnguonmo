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

namespace HIS.Desktop.Plugins.ApprovaleDebate.Key
{
    internal class HisConfigKeys
    {
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        internal const string HIS_CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal const string HIS_CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";
        //O "Ghi dien bien, PP xu ly vao to dieu tri" khi duyet hoi chan
        //rong/1: hien o, mac dinh tich; 0: hien o, mac dinh khong tich; 2: an o cung 2 o Dien bien, PP xu ly, khong ghi to dieu tri
        internal const string HIS_CONFIG_KEY__WriteTrackingOption = "HIS.Desktop.Plugins.ApprovaleDebate.WriteTrackingOption";
        //1: an nut "Duyet va ky (Ctrl K)", o "In van ban da ky"; nut In phieu duyet hoi chan chi in, khong ky
        internal const string HIS_CONFIG_KEY__IsHideApproveAndSign = "HIS.Desktop.Plugins.ApprovaleDebate.IsHideApproveAndSign";
    }
}
