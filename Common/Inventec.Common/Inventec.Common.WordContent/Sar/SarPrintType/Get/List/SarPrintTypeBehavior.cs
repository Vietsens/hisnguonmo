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
using Inventec.Common.Adapter;
using Inventec.Common.WordContent.Base;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Sar.SarPrintType.Get.List
{
    class SarPrintTypeBehavior : BusinessLogicBase, ISarPrintTypeGetList
    {
        SarPrintTypeFilter entity;
        internal SarPrintTypeBehavior(CommonParam param, SarPrintTypeFilter filter)
            : base()
        {
            this.entity = filter;
        }

        List<SAR_PRINT_TYPE> ISarPrintTypeGetList.Run()
        {
            try
            {
                return new BackendAdapter(param).Get<List<SAR_PRINT_TYPE>>(RequestUriStore.SAR_PRINT_TYPE_GET, WordContentStorage.SarConsumer, entity, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }

    }
}
