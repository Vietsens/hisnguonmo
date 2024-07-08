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
using His.UC.UCHein.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.HisPatient
{
    class HisPatientGet
    {
        /// <summary>
        /// Lấy dữ liệu Bn cũ chính xác theo số thẻ bhyt
        /// </summary>
        /// <returns>List<HisPatientSDO></returns>
        internal static List<HisPatientSDO> GetSDO(string heinCardNumber)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientAdvanceFilter hisPatientFilter = new MOS.Filter.HisPatientAdvanceFilter();
                hisPatientFilter.HEIN_CARD_NUMBER__EXACT = Utils.HeinUtils.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                return new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumerStore.MosConsumer, hisPatientFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
