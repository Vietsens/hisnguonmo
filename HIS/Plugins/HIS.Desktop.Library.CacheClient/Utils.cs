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

namespace HIS.Desktop.Library.CacheClient
{
    public class Utils
    {
        public static long? GetModifyTimeMax<T>(List<T> datas)
        {
            long? modifyTimeNew = 0;
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo propertyInfoOrderField = type.GetProperty("MODIFY_TIME");
                if (propertyInfoOrderField != null)
                {
                    var tbl = datas.ListToDataTable<T>();
                    var drSort = tbl.Select("1 = 1", "MODIFY_TIME DESC").FirstOrDefault();
                    modifyTimeNew = (drSort != null ? long.Parse((drSort["MODIFY_TIME"] ?? "").ToString()) : 0);
                    modifyTimeNew = ((modifyTimeNew.HasValue && modifyTimeNew > 0) ? modifyTimeNew : Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return modifyTimeNew;
        }
    }
}
