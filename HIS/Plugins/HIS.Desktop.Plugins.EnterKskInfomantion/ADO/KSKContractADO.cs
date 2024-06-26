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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion.ADO
{
    class KSKContractADO : HIS_KSK_CONTRACT
    {
        public string WORK_PLACE_NAME { get; set; }

        public KSKContractADO()
        {
        }

        public KSKContractADO(HIS_KSK_CONTRACT data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(data)));
                    }

                    var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.ID == data.WORK_PLACE_ID);
                    if (workPlace != null)
                    {
                        this.WORK_PLACE_NAME = workPlace.WORK_PLACE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
