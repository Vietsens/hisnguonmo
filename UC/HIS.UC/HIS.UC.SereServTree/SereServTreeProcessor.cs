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
using HIS.UC.SereServTree.CheckAllNode;
using HIS.UC.SereServTree.GetListCheck;
using HIS.UC.SereServTree.Reload;
using HIS.UC.SereServTree.Run;
using HIS.UC.SereServTree.Search;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.SereServTree
{
    public class SereServTreeProcessor : BussinessBase
    {
        object uc;
        public SereServTreeProcessor()
            : base()
        {
        }

        public SereServTreeProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(SereServTreeADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeISereServTree(param, arg);
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

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs)
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

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> sereServDeposits)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReloadSereServDeposit(param, (control == null ? (UserControl)uc : control), sereServDeposits);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void CheckAllNode(UserControl control)
        {
            try
            {
                ICheckAllNode behavior = CheckAllNodeFactory.MakeICheckAllNode(param, (control == null ? (UserControl)uc : control));
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
    }
}
