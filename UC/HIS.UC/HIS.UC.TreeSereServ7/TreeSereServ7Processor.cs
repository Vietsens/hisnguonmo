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
using HIS.UC.TreeSereServ7.DisposeControl;
using HIS.UC.TreeSereServ7.Expand;
using HIS.UC.TreeSereServ7.GetListCheck;
using HIS.UC.TreeSereServ7.GetValueFocus;
using HIS.UC.TreeSereServ7.Reload;
using HIS.UC.TreeSereServ7.ReloadByDepartment;
using HIS.UC.TreeSereServ7.Run;
using HIS.UC.TreeSereServ7.Search;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.TreeSereServ7
{
    public class TreeSereServ7Processor : BussinessBase
    {
        object uc;
        public TreeSereServ7Processor()
            : base()
        {
        }

        public TreeSereServ7Processor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(TreeSereServ7ADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeITreeSereServ7(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Search(UserControl control)
        {
            try
            {
                ISearch behavior = SearchFactory.MakeISearch(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void DisposeControl(UserControl control)
        {
            try
            {
                IDisposeControl behavior = DisposeControlFactory.MakeIDisposeControl(param, (control == null ? (UserControl)uc : control));
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs, bool check = true)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServ7)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<DHisSereServ2> sereServsNew)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServsNew);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, long _department, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
        {
            try
            {
                IReloadByDepartment behavior = ReloadByDepartmentFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), _department, sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, List<SereServADO> sereServs)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Reload(UserControl control, long _department, List<SereServADO> sereServs)
        {
            try
            {
                IReloadByDepartment behavior = ReloadByDepartmentFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), _department, sereServs);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<SereServADO> GetListCheck(UserControl control)
        {
            List<SereServADO> result = null;
            try
            {
                IGetListCheck behavior = GetListCheckFactory.MakeIGetListCheck(control);
                result = (behavior != null) ? behavior.Run() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public void Expand(UserControl control, bool isExpand)
        {
            try
            {
                IExpand behavior = ExpandFactory.MakeIExpand(param, (control == null ? (UserControl)uc : control), isExpand);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public object GetValueFocus(UserControl control)
        {
            object result = null;
            try
            {
                IGetValueFocus behavior = GetValueFocusFactory.MakeIGetValueFocus(param, (control == null ? (UserControl)uc : control));
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
