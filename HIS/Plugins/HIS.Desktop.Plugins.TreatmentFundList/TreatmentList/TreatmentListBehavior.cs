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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFundList.TreatmentList
{
    class TreatmentListBehavior : Tool<IDesktopToolContext>, ITreatmentList
    {
        Inventec.Desktop.Common.Modules.Module Module;
        //V_HIS_TREATMENT_FEE_2D treatment = null;
        //V_HIS_ACCOUNT_BOOK accountBook = null;
        long FundId = 0;

        internal TreatmentListBehavior()
            : base()
        {

        }

        internal TreatmentListBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param)
            : base()
        {
            Module = module;
        }

        internal TreatmentListBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data)
            : base()
        {
            Module = module;
            FundId = data;
        }

        object ITreatmentList.Run()
        {
            object result = null;
            try
            {

                if (FundId >0)
                {
                    result = new frmTreatmentList(Module, FundId);
                }
                else
                {
                    result = new frmTreatmentList(Module);
                }
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Module), Module));
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
