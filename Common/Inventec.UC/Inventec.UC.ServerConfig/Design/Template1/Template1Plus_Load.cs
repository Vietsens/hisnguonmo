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

namespace Inventec.UC.ServerConfig.Design.Template1
{
    internal partial class Template1
    {
        private void Template1_Load(object sender, EventArgs e)
        {
            try
            {
                Data.DataStore.LoadSystemConfigKey();
                SetListDataShow();
                ListElement = Data.DataStore.SystemConfigXLM.GetElements();
                gridControlSystemConfig.DataSource = ListData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetListDataShow()
        {
            try
            {
                Data.DataShow dataacs = new Data.DataShow() { KEYCODE = "Inventec.Token.ClientSystem.Acs.Base.Uri", VALUE = Data.DataAppConfig.ACS_BASE_URI, DEFAULT_VALUE = "", DESCRIPTION = "Đia chỉ cấu hình kết nối đến Resource server xác thực ACS" };
                ListData.Add(dataacs);
                Data.DataShow data__Fss = new Data.DataShow() { KEYCODE = "fss.uri.base", VALUE = Data.DataAppConfig.FSS_BASE_URI, DEFAULT_VALUE = "", DESCRIPTION = "Đia chỉ cấu hình kết nối đến Resource server quản lý file tập trung FSS" };
                ListData.Add(data__Fss);
                foreach (var item in Data.DataStore.SystemConfigXLM.GetElements())
                {
                    //string value = string.IsNullOrEmpty(item.Value.ToString()) ? item.DefaultValue.ToString() : item.Value.ToString();
                    Data.DataShow data = new Data.DataShow() { KEYCODE = item.KeyCode, VALUE = item.Value.ToString(), DEFAULT_VALUE = item.DefaultValue.ToString(), DESCRIPTION = item.Tutorial };
                    ListData.Add(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
