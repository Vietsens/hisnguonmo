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
using Inventec.Core;
using HIS.UC.WorkPlace.Async;
using Inventec.Core;
using HIS.UC.WorkPlace.Async;

namespace HIS.UC.WorkPlace
{
    public partial class WorkPlaceGenerate : BeanObjectBase, IAppDelegacyAsync
    {
        WorkPlaceInitADO data;

        public WorkPlaceGenerate(CommonParam param, WorkPlaceInitADO data)
            : base(param)
        {
            this.data = data;
        }

        async Task<object> IAppDelegacyAsync.Execute()
        {
            object result = null;
            try
            {
                CommonParam param = new CommonParam();
                IWorkPlaceGenerate behavior = WorkPlaceGenerateBehaviorFactory.MakeIServiceRequestRegister(param, this.data);
                result = behavior != null ? await behavior.Run().ConfigureAwait(false) : null;

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
