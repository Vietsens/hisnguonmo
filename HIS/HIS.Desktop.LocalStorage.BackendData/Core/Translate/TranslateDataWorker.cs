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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Microsoft.Win32;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class TranslateDataWorker
    {
        private static SDA.EFMODEL.DataModels.SDA_LANGUAGE language;
        public static SDA.EFMODEL.DataModels.SDA_LANGUAGE Language
        {
            get
            {
                try
                {
                    if (language == null)
                    {
                        CommonParam param = new CommonParam();
                        SdaLanguageFilter filter = new SdaLanguageFilter();
                        filter.IS_ACTIVE = 1;
                        var languages = new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_LANGUAGE>>(SdaRequestUriStore.SDA_LANGUAGE_GET, ApiConsumers.SdaConsumer, filter, param);
                        language = languages.FirstOrDefault(o => o.LANGUAGE_CODE.ToUpper() == LanguageManager.GetLanguage().ToUpper());
                    }
                    if (language == null) language = new SDA.EFMODEL.DataModels.SDA_LANGUAGE();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                return language;
            }
            set
            {
                language = value;
            }
        }
    }
}
