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
using HIS.Desktop.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBill.TransactionBill
{
    class TransactionBillBehavior : Tool<IDesktopToolContext>, ITransactionBill
    {
        V_HIS_TREATMENT_FEE treatment = null;
        Inventec.Desktop.Common.Modules.Module Module;
        List<V_HIS_SERE_SERV_5> ListSereServ = null;
        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter;
        bool? IsDirectlyBilling = null;
        V_HIS_TRANSACTION tran = null;
        internal TransactionBillBehavior()
            : base()
        {

        }
        internal TransactionBillBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, V_HIS_TREATMENT_FEE data, List<V_HIS_SERE_SERV_5> servServ5s, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base()
        {
            this.Module = module;
            this.treatment = data;
            this.ListSereServ = servServ5s;
            this.patientTypeAlter = patientTypeAlter;
            this.IsDirectlyBilling = isDirectlyBilling;
            this.tran = tran;
        }
        internal TransactionBillBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, V_HIS_TREATMENT_FEE data, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base()
        {
            this.Module = module;
            this.treatment = data;
            this.patientTypeAlter = patientTypeAlter;
            this.IsDirectlyBilling = isDirectlyBilling;
            this.tran = tran;
        }
        internal TransactionBillBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base()
        {
            this.Module = module;
            this.IsDirectlyBilling = isDirectlyBilling;
            this.tran = tran;
        }

        object ITransactionBill.Run()
        {
            object result = null;
            try
            {
                if (treatment != null && ListSereServ != null && ListSereServ.Count > 0)
                {
                    result = new frmTransactionBill(Module, treatment, ListSereServ, this.patientTypeAlter, this.IsDirectlyBilling,tran);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else if (treatment != null)
                {
                    result = new frmTransactionBill(Module, treatment, this.patientTypeAlter, this.IsDirectlyBilling,tran);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else
                {
                    result = new frmTransactionBill(Module, this.IsDirectlyBilling,tran);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
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
