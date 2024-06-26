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

using HIS.UC.UCCauseOfDeath.Reload;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.UCCauseOfDeath.SetValue;
using HIS.UC.UCCauseOfDeath.Run;
using HIS.UC.UCCauseOfDeath.GetValue;
using HIS.UC.UCCauseOfDeath.ReadOnlyAll;

namespace HIS.UC.UCCauseOfDeath
{
    public class UCCauseOfDeathProcessor : BussinessBase
    {
        object uc;
        public UCCauseOfDeathProcessor()
            : base()
        {
        }

        public UCCauseOfDeathProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public void Reload(UserControl control)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object Run(CauseOfDeathADO data)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIRun(param, data);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return uc;
        }

        public void SetValue(UserControl control, CauseOfDeathADO data)
        {
            try
            {
                ISetValue behavior = SetValueFactory.MakeISetValue(param, (control == null ? (UserControl)uc : control), data);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetValue(UserControl control)
        {
            object result = null;
            try
            {
                IGetValue behavior = GetValueFactory.MakeIGetValue(param, (control == null ? (UserControl)uc : control));
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void ReadOnlyAll(UserControl control, bool isReadOnly)
        {
            try
            {
                IReadOnlyAll behavior = ReadOnlyAllFactory.MakeIReadOnlyAll(param, (control == null ? (UserControl)uc : control), isReadOnly);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
