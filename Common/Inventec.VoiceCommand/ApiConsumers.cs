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

namespace Inventec.VoiceCommand
{
    public class ApiConsumers
    {
        private static Inventec.Common.WebApiClient.ApiConsumer rikkeiaiConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer RIKKEIAIConsumer
        {
            get
            {
                if (rikkeiaiConsumer == null)
                {
                    rikkeiaiConsumer = new Inventec.Common.WebApiClient.ApiConsumer(CommandCFG.RikkeiAI__URI, "HIS");
                }
                return rikkeiaiConsumer;
            }
            set
            {
                rikkeiaiConsumer = value;
            }
        }

        private static Inventec.Common.WebApiClient.ApiConsumer wwitaiConsumer;
        public static Inventec.Common.WebApiClient.ApiConsumer WITAIConsumer
        {
            get
            {
                if (wwitaiConsumer == null)
                {
                    wwitaiConsumer = new Inventec.Common.WebApiClient.ApiConsumer(CommandCFG.WitAI__URI, "HIS");
                }
                return wwitaiConsumer;
            }
            set
            {
                wwitaiConsumer = value;
            }
        }
       
    }
}
