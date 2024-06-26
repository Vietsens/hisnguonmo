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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHoldReturn
{
    internal class GlobalDatas
    {
        static Dictionary<long, HIS_EXP_MEST_TEMPLATE> expMestTemplates;
        internal static Dictionary<long, HIS_EXP_MEST_TEMPLATE> ExpMestTemplates
        {
            get
            {
                if (expMestTemplates == null)
                {
                    expMestTemplates = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().ToDictionary(o => o.ID, o => o);
                }

                return expMestTemplates;
            }
            set { expMestTemplates = value; }
        }

        //internal static List<HIS_TREATMENT> Treatments;
        //internal async Task GetTreatments()
        //{
        //    if (Treatments == null)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug("GetTreatments. 1");
        //        CommonParam paramCommon = new CommonParam();
        //        dynamic filter = new System.Dynamic.ExpandoObject();
        //        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
        //        Treatments = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, paramCommon);
        //        Inventec.Common.Logging.LogSystem.Debug("GetTreatments. 2");
        //    }
        //}
    }
}
