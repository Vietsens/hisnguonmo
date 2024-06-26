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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisRationSum.ADO
{
    class SereServ15ADO : V_HIS_SERE_SERV_15
    {
        public string CHILD_ID { get; set; }
        public string PARENT_ID_STR { get; set; }
        public decimal AMOUNT_SUM { get; set; }
        public int IS_PARENT { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string TITLE_NAME { get; set; }

        public SereServ15ADO()
        {
        }

        public SereServ15ADO(V_HIS_SERE_SERV_15 sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(sereServ));
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
