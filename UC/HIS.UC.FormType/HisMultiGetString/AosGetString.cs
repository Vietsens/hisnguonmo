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

namespace HIS.UC.FormType.HisMultiGetString
{
    public class AosGetString
    {
        public static List<DataGet> Get(string value, string key)
        {
            List<DataGet> datasuft = null;
            try
            {
                if (value == null) return new List<DataGet>();

                else if (value == "AOS_ACCOUNT_TYPE") datasuft = Config.AosFormTypeConfig.AosAccountTpye.Select(o => new DataGet { ID = o.ID, CODE = o.ACCOUNT_TYPE_CODE, NAME = o.ACCOUNT_TYPE_NAME }).ToList();
                else if (value == "AOS_BANK_ACCOUNT_TYPE")
                {
                    var lstAos = Config.AosFormTypeConfig.AosAccountTpye.Where(o => !string.IsNullOrWhiteSpace(o.BANK_CODE)).ToList();
                    if (lstAos != null)
                        datasuft = lstAos.Select(o => new DataGet { ID = o.ID, CODE = o.ACCOUNT_TYPE_CODE, NAME = o.ACCOUNT_TYPE_NAME }).ToList();
                }

                datasuft = datasuft.OrderBy(o => o.NAME).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return datasuft;
        }
    }
}
