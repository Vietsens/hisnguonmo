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

namespace HIS.Desktop.LocalStorage.HisConfig
{
    public class WitRecognizeCFG
    {
        private const string CONFIG_KEY__WitAi_AccessToken = "Inventec.Common.WitAI.WitAiAccessToken";
        private const string CONFIG_KEY__WitAI_TimeReplay = "Inventec.Common.WitAI.TimeReplay";
        public static string AccessToken;
        public static int TimeReplay;

        public static void LoadConfig()
        {
            try
            {
                AccessToken = HisConfigs.Get<string>(CONFIG_KEY__WitAi_AccessToken);
                TimeReplay = int.Parse(HisConfigs.Get<string>(CONFIG_KEY__WitAI_TimeReplay) ?? "3000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
