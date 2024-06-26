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

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.TransactionBillTwoInOne
{
    class TransactionBillTwoInOneBehavior : Tool<IDesktopToolContext>, ITransactionBillTwoInOne
    {
        V_HIS_TREATMENT_FEE treatment = null;
        List<V_HIS_SERE_SERV_5> listSereServ = null;
        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter;
        Inventec.Desktop.Common.Modules.Module Module;
        bool? IsBill = null;
        internal TransactionBillTwoInOneBehavior()
            : base()
        {

        }

        internal TransactionBillTwoInOneBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, V_HIS_TREATMENT_FEE data, List<V_HIS_SERE_SERV_5> servServ5s, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool? isBill)
            : base()
        {
            this.Module = module;
            this.treatment = data;
            this.listSereServ = servServ5s;
            this.patientTypeAlter = patientTypeAlter;
            this.IsBill = isBill;
        }
        internal TransactionBillTwoInOneBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, bool? isBill)
            : base()
        {
            this.Module = module;
            this.IsBill = isBill;
        }

        object ITransactionBillTwoInOne.Run()
        {
            object result = null;
            try
            {
                if (treatment != null)
                {
                    result = new frmTransactionBillTwoInOne(Module, treatment, this.listSereServ, this.IsBill);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData("treatment", treatment));
                }
                else
                {
                    result = new frmTransactionBillTwoInOne(Module, this.IsBill);
                }
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
