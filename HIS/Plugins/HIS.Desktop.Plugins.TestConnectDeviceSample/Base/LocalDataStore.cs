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

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Base
{
    internal class LocalDataStore
    {
        private static string ConfigTimeout = System.Configuration.ConfigurationSettings.AppSettings["HIS.Desktop.Transaction.SelectAccount.Timeout"];
        internal static string MessageFromDevice { get; set; }

        internal const int TIME_OUT_AWAIT_MESSAGE_DEVICE = 10000;
        internal const short IS_TRUE = (short)1;

        private static int? selectAccountTimeout;
        internal static int SelectAccountTimeout
        {
            get
            {
                if (!selectAccountTimeout.HasValue)
                {
                    try
                    {
                        string second = String.IsNullOrWhiteSpace(ConfigTimeout) ? "10000" : ConfigTimeout;
                        selectAccountTimeout = Convert.ToInt32(second);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        selectAccountTimeout = 10000;
                    }
                }
                return selectAccountTimeout.Value;
            }
        }
    }
}
