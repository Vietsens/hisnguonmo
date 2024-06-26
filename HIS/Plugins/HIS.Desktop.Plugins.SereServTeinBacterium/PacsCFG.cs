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

namespace HIS.Desktop.Plugins.SereServTeinBacterium
{
    public class PacsAddress
    {
        public string RoomCode { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
    }

    class PacsCFG
    {
        private const string PACS_ADDRESS_CFG = "MOS.PACS.ADDRESS";

        internal static List<PacsAddress> PACS_ADDRESS
        {
            get
            {
                return GetAddress(PACS_ADDRESS_CFG); ;
            }
        }

        private static List<PacsAddress> GetAddress(string code)
        {
            List<PacsAddress> result = new List<PacsAddress>();
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(code);
                }
                List<PacsAddress> adds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PacsAddress>>(value);
                if (adds == null || adds.Count == 0)
                {
                    throw new AggregateException(code);
                }
                result.AddRange(adds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<PacsAddress>();
            }
            return result;
        }
    }
}
