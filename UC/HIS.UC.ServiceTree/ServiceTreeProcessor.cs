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
using HIS.UC.ServiceTree.Run;
using HIS.UC.ServiceTree.Search;
using Inventec.Core;
using System;
using System.Windows.Forms;

namespace HIS.UC.ServiceTree
{
    public class ServiceTreeProcessor : BussinessBase
    {
        public ServiceTreeProcessor()
            : base()
        {
        }

        public ServiceTreeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ServiceTreeADO arg)
        {
            object result = null;
            try
            {
                IRun behavior = RunFactory.MakeIServiceTree(param, arg);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void Search(UserControl control)
        {
            try
            {
                ISearch behavior = SearchFactory.MakeISearch(param, control);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
