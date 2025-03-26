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
using System;
using System.Collections.Generic;

namespace Inventec.Common.WordContent.SarPrint.Create
{
    class SarPrintCreateBehavior : BeanObjectBase, ISarPrintCreate
    {
        SAR_PRINT entity;

        internal SarPrintCreateBehavior(CommonParam param, SAR_PRINT data)
            : base(param)
        {
            entity = data;
        }

        SAR_PRINT ISarPrintCreate.Run()
        {
            SAR_PRINT result = null;
            try
            {
                result = new BackendAdapter(param).Post<SAR_PRINT>(RequestUriStore.SAR_PRINT_CREATE, WordContentStorage.SarConsumer, entity, param);
                //result = new SarPrintDAL(param).PostRequest<SAR_PRINT>(RequestUriStore.SAR_PRINT_CREATE, ApiConsumerStore.SarConsumer, entity, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
