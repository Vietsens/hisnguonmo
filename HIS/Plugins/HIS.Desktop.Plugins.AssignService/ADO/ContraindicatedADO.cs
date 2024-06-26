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

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    public class ContraindicatedADO
    {
        public string ICD_NAME { get; set; }
        public string SERVICE_NAME { get; set; }
        public string CONTRAINDICATION_CONTENT { get; set; }
        public ContraindicatedADO()
        {
        }
        public ContraindicatedADO(HIS_ICD_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    this.ICD_NAME = data.ICD_CODE + "-" + data.ICD_NAME;
                    this.CONTRAINDICATION_CONTENT = data.CONTRAINDICATION_CONTENT;
                    this.SERVICE_NAME = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == data.SERVICE_ID).Select(o => o.SERVICE_NAME).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
