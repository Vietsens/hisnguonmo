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
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace RAE.UCHein.Config
{
    class ConfigUtil
    {
        internal static long GetLongConfig(string code)
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

        internal static decimal GetDecimalConfig(string code)
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

        internal static int GetIntConfig(string code)
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

        internal static string GetStrConfig(string code)
        {
            string result = null;
            try
            {
                SDA_CONFIG config = Loader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                result = !String.IsNullOrEmpty(config.VALUE) ? config.VALUE : (!String.IsNullOrEmpty(config.DEFAULT_VALUE) ? config.DEFAULT_VALUE : "");
                if (String.IsNullOrEmpty(result)) throw new ArgumentNullException(code);

            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
            }
            return result;
        }

        internal static List<string> GetStrConfigs(string code)
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
