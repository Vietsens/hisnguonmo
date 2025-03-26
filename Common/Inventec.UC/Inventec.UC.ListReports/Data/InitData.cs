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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Data
{
    public class InitData
    {
        internal Inventec.Token.ClientSystem.ClientTokenManager clientToken;
        internal Inventec.Common.WebApiClient.ApiConsumer sarconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer sdaconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer acsconsumer;
        internal long numPage;
        internal string nameIcon;
        internal bool isAdmin;
        public CultureInfo Language { get; set; }
        public ProcessCopy ProcessCopy { get; set; }

        public InitData(Inventec.Common.WebApiClient.ApiConsumer sarConsumer, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsComsumer, Inventec.Token.ClientSystem.ClientTokenManager clientTokenManager, long NumPaging, string NameIcon, CultureInfo Language, bool _isAdmin)
        {
            try
            {
                this.sarconsumer = sarConsumer;
                this.sdaconsumer = sdaConsumer;
                this.acsconsumer = acsComsumer;
                this.clientToken = clientTokenManager;
                this.numPage = NumPaging;
                this.nameIcon = NameIcon;
                this.Language = Language;
                this.isAdmin = _isAdmin;
                Inventec.UC.ListReports.Base.LanguageManager.Init(this.Language);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
