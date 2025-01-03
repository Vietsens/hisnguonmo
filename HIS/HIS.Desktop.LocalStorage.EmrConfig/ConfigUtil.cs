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
using Inventec.Common.Logging;
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.EmrConfig
{
    public class ConfigUtil
    {
        public static long GetLongConfig(string code)
        {
            long result = 0;
            try
            {
                result = long.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        public static decimal GetDecimalConfig(string code)
        {
            decimal result = 0;
            try
            {
                result = decimal.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        public static int GetIntConfig(string code)
        {
            int result = 0;
            try
            {
                result = int.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        public static string GetStrConfig(string code)
        {
            string result = null;
            try
            {
                EMR_CONFIG config = null;
                object data = null;
                if (!EmrConfigs.dic.TryGetValue(code, out data))
                {
                    LogSystem.Info("ConfigUtil => Khong Get duoc cau hinh theo Key: " + code);
                }
                else
                {
                    config = data as EMR_CONFIG;
                }
                if (config == null) throw new ArgumentNullException(code);
                result = !String.IsNullOrEmpty(config.VALUE) ? config.VALUE : (!String.IsNullOrEmpty(config.DEFAULT_VALUE) ? config.DEFAULT_VALUE : "");
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Loi khi lay Config: " + code, ex);
            }
            return result;
        }

        public static List<string> GetStrConfigs(string code)
        {
            List<string> result = new List<string>();
            try
            {
                string str = GetStrConfig(code);
                string[] arr = str.Split(',');
                if (arr != null)
                {
                    foreach (string s in arr)
                    {
                        result.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
