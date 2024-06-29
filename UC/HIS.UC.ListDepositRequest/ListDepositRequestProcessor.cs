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
using HIS.UC.ListDepositRequest.ADO;
using HIS.UC.ListDepositRequest.GetSelectRow;
using HIS.UC.ListDepositRequest.Reload;
using HIS.UC.ListDepositRequest.Run;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.ListDepositRequest
{  
    public class ListDepositRequestProcessor : BussinessBase
    {
        object uc;
        public ListDepositRequestProcessor()
            : base()
        {
        }

        public ListDepositRequestProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(ListDepositRequestInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIDepositReqGrid(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<V_HIS_DEPOSIT_REQ> data)
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

        public object GetSelectRow(UserControl control)
        {
            object result = null;
            try
            {
                IGetSelectRow behavior = GetSelectRowFactory.MakeIGetSelectRow(param, (control == null ? (UserControl)uc : control));
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
