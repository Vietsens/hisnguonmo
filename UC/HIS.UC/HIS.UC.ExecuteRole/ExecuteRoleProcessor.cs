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
using HIS.UC.ExecuteRole.Run;
using HIS.UC.ExecuteRole.Reload;
using HIS.UC.ExecuteRole.ADO;
using HIS.UC.ExecuteRole.GetDataGridView;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ExecuteRole.GetGridControl;

namespace HIS.UC.ExecuteRole
{
    
    public class ExecuteRoleProcessor : BussinessBase
    {
        object uc;
        public ExecuteRoleProcessor()
            : base()
        {
        }

        public ExecuteRoleProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ExecuteRoleInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIRun(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<ExecuteRoleADO> data)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetDataGridView(UserControl control)
        {
            object result = null;
            try
            {
                IGetDataGridView behavior = GetDataGridViewFactory.MakeIGetDataGridView(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public object GetGridControl(UserControl control)
        {
            object result = null;
            try
            {
                IGetGridControl behavior = GetGridControlFactory.MakeIGetGridControl(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}
