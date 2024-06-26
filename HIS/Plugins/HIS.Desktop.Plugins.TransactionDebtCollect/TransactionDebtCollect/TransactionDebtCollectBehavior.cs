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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionDebtCollect.TransactionDebtCollect
{
    class TransactionDebtCollectBehavior : Tool<IDesktopToolContext>, ITransactionDebtCollect
    {
        V_HIS_TREATMENT_FEE treatment = null;
        long treatmentId = 0;
        Inventec.Desktop.Common.Modules.Module Module;
        List<long> _ListTransactionId = null;
        RefeshReference refeshReference = null;

        internal TransactionDebtCollectBehavior()
            : base()
        {

        }
        internal TransactionDebtCollectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, V_HIS_TREATMENT_FEE data)
            : base()
        {
            this.Module = module;
            this.treatment = data;
        }
        internal TransactionDebtCollectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, List<long> transactionIdList)
            : base()
        {
            this.Module = module;
            this._ListTransactionId = transactionIdList;
        }
        internal TransactionDebtCollectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, List<long> transactionIdList, long _treatmentId, RefeshReference _refeshReference)
            : base()
        {
            this.Module = module;
            this._ListTransactionId = transactionIdList;
            this.treatmentId = _treatmentId;
            this.refeshReference = _refeshReference;
        }
        internal TransactionDebtCollectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data)
            : base()
        {
            this.Module = module;
            this.treatmentId = data;
        }
        internal TransactionDebtCollectBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param)
            : base()
        {
            this.Module = module;
        }

        object ITransactionDebtCollect.Run()
        {
            object result = null;
            try
            {
                if (treatment != null)
                {
                    result = new frmTransactionDebtCollect(Module, treatment);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else if (_ListTransactionId != null && _ListTransactionId.Count > 0 && treatmentId > 0)
                {
                    result = new frmTransactionDebtCollect(Module, _ListTransactionId, treatmentId, refeshReference);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else if (treatmentId > 0)
                {
                    result = new frmTransactionDebtCollect(Module, treatmentId);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else if (_ListTransactionId != null && _ListTransactionId.Count > 0)
                {
                    result = new frmTransactionDebtCollect(Module, _ListTransactionId);
                    if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment), treatment));
                }
                else
                {
                    result = new frmTransactionDebtCollect(Module);
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
