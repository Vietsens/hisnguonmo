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
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Get
{
    class HisAgeGetListBehavior : BusinessBase, IGetDataT
    {
        internal HisAgeGetListBehavior(CommonParam param)
            : base(param)
        {
        }

        object IGetDataT.Execute<T>()
        {
            try
            {
                List<AgeADO> ages = new List<AgeADO>();
                AgeADO kh1 = new AgeADO(1, "Tuổi");
                ages.Add(kh1);

                AgeADO kh2 = new AgeADO(2, "Tháng");
                ages.Add(kh2);

                AgeADO kh3 = new AgeADO(3, "Ngày");
                ages.Add(kh3);

                AgeADO kh4 = new AgeADO(4, "Giờ");
                ages.Add(kh4);
                return ages;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
