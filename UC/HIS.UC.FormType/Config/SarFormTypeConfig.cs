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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Config
{
    public class SarFormTypeConfig
    {
        private static List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE> sarFormType;
        public static List<SAR.EFMODEL.DataModels.SAR_FORM_TYPE> SarFormType
        {
            get
            {
                if (FormTypeDelegate.ProcessFormType != null) FormTypeDelegate.ProcessFormType(typeof(SAR.EFMODEL.DataModels.SAR_FORM_TYPE));
                if (sarFormType == null || sarFormType.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarFormTypeFilter filter = new SAR.Filter.SarFormTypeFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sarFormType = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_TYPE>(RequestUriStore.SAR_FORM_TYPE_GET, ApiConsumerStore.SarConsumer, filter);
                }
                return sarFormType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            set
            {
                sarFormType = value;
            }
        }

    }
}
