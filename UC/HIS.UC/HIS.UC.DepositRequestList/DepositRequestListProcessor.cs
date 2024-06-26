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
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.UC.DepositRequestList.ADO;
using HIS.UC.DepositRequestList.Run;
using System.Windows.Forms;
using HIS.UC.DepositRequestList.Reload;
using HIS.UC.DepositRequestList.GetFocusRow;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.DepositRequestList
{
    public class DepositRequestListProcessor : BussinessBase
    {
        object uc;
        public DepositRequestListProcessor()
            : base()
        {
        }

        public DepositRequestListProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(DepositRequestInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIDepositRequestList(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }

        public void Reload(UserControl control, List<MOS.EFMODEL.DataModels.V_HIS_DEPOSIT_REQ> depositReq)
        {
            try
            {
                IReload behavior = ReloadFactory.MakeIReload(param, (control == null ? (UserControl)uc : control), depositReq);
                if (behavior != null) behavior.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public List<V_HIS_DEPOSIT_REQ> GetFocusRow(UserControl control)
        {
            List<V_HIS_DEPOSIT_REQ> result = null;
            try
            {
                IGetFocusRow behavior = GetFocusRowFactory.MakeIGetFocusRow(control);
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
