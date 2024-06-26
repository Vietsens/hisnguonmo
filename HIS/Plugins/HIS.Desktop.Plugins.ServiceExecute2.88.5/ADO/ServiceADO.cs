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

namespace HIS.Desktop.Plugins.ServiceExecute.ADO
{
    public class ServiceADO : HIS_SERE_SERV
    {
        public bool isSave { get; set; }
        public string SoPhieu { get; set; }

        public int tableIndex { get; set; }//phục vụ all in one lấy theo index tạo ra
        public string conclude { get; set; }
        public string note { get; set; }
        public string description { get; set; }
        public bool isEdit { get; set; }

        public long? MACHINE_ID { get; set; }
        public long? NUMBER_OF_FILM { get; set; }

        public bool MustHavePressBeforeExecute { get; set; }

        public ServiceADO(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HIS_SERE_SERV>();
                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(data)));
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
