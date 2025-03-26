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
using Inventec.Common.WordContent.Base;
using Inventec.Common.WordContent.SarPrint.Create;
using Inventec.Core;
using System;

namespace Inventec.Common.WordContent.SarPrint
{
    partial class SarPrintCreate : BeanObjectBase, IDelegacy
    {
        object entity;

        internal SarPrintCreate(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        object IDelegacy.Execute()
        {
            object result = null;
            try
            {

                ISarPrintCreate behavior = SarPrintCreateBehaviorFactory.MakeIAggrExpMestCreate(param, entity);
                result = behavior != null ? behavior.Run() : null;
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
