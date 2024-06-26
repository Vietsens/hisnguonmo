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
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using System.Collections.Generic;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class SdaEthnicCFG
    {
        private const string ETHNIC_CODE_BASE = "EXE.ETHNIC_CODE_BASE";

        private static SDA_ETHNIC ethnicBase;
        public static SDA_ETHNIC ETHNIC_BASE
        {
            get
            {
                if (ethnicBase == null)
                {
                    ethnicBase = GetData(ETHNIC_CODE_BASE);
                }
                return ethnicBase;
            }
            set
            {
                ethnicBase = value;
            }
        }

        private static SDA_ETHNIC GetData(string code)
        {
            SDA_ETHNIC result = null;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                SdaEthnicFilter filter = new SdaEthnicFilter();
                filter.KEY_WORD = value;
                var data = BackendDataWorker.Get<SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == value);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                result = data;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
