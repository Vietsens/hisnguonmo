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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ADO
{
    public class ExpMestBloodADO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_BLOOD
    {
        public string PACKING_TIME_STR { get; set; }

        public ExpMestBloodADO(V_HIS_EXP_MEST_BLOOD data)
        {
            try
            {
                if (data!=null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_EXP_MEST_BLOOD>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(data));
                    }
                    //this.PACKING_TIME_STR=Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.PA
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
