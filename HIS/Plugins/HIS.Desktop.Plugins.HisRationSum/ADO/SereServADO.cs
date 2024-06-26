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

namespace HIS.Desktop.Plugins.HisRationSum.ADO
{
    public class SereServADO : MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_RATION
    {
        public string CHILD_ID { get; set; }
        public string PARENT_ID_STR { get; set; }
        public decimal AMOUNT_SUM { get; set; }
        public int IS_PARENT { get; set; }

        public string TITLE_NAME { get; set; }
        public string SERVICE_REQ_CODE_PATIENT_NAME_GROUP { get; set; }

        public SereServADO()
        {

        }

        public SereServADO(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_RATION sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_RATION>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, item.GetValue(sereServ));
                    }
                    this.SERVICE_REQ_CODE_PATIENT_NAME_GROUP = this.SERVICE_REQ_CODE + " - " + sereServ.TDL_PATIENT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
