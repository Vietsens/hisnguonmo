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
using HIS.UC.TreeSereServ7V2.Expand;
using HIS.UC.TreeSereServ7V2.GetListCheck;
using HIS.UC.TreeSereServ7V2.GetValueFocus;
using HIS.UC.TreeSereServ7V2.ReloadByServiceReq;
using HIS.UC.TreeSereServ7V2.GetCountTitle;
using HIS.UC.TreeSereServ7V2.Run;
using HIS.UC.TreeSereServ7V2.Search;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.TreeSereServ7V2.GetListAll;

namespace HIS.UC.TreeSereServ7V2
{
    public class TreeSereServ7V2Processor : BussinessBase
    {
        object uc;
        public TreeSereServ7V2Processor()
            : base()
        {
        }

        public TreeSereServ7V2Processor(CommonParam paramBusiness)
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

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs, List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ> currentServiceReq)
        {
            try
            {
                IReloadByServiceReq behavior = ReloadByServiceReqFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs, currentServiceReq);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Reload(UserControl control, List<SereServADO> sereServs, List<MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ> currentServiceReq)
        {
            try
            {
                IReloadByServiceReq behavior = ReloadByServiceReqFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), sereServs, currentServiceReq);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string GetTitleCount(UserControl control)
        {
            string result = "";
            try
            {
                IGetCountTitle behavior = GetCountTitleFactory.MakeIGetCountTitle(param, (control == null ? (UserControl)uc : control));
                if(behavior != null) 
                {
                    result =  behavior.Run();
                }
            }
            catch (Exception ex)
            {
                  Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        public List<SereServADO> GetListAll(UserControl control)
        {
            List<SereServADO> result = null;
            try
            {
                IGetListAll behavior = GetListAllFactory.MakeIGetListAll(control);
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
