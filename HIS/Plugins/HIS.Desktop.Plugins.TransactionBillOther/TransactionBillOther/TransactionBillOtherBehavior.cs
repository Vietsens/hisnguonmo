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

namespace HIS.Desktop.Plugins.TransactionBillOther.TransactionBillOther
{
    class TransactionBillOtherBehavior : Tool<IDesktopToolContext>, ITransactionBillOther
    {
        long? treatmentId = null;
        Inventec.Desktop.Common.Modules.Module Module;
        private HIS_PATIENT hisPatient = null;
        private HIS_PATIENT_PACKAGE patientPackage = null;

        internal TransactionBillOtherBehavior()
            : base()
        {

        }
        internal TransactionBillOtherBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param)
            : base()
        {
            this.Module = module;
        }
        internal TransactionBillOtherBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data)
            : base()
        {
            this.Module = module;
            this.treatmentId = data; 
        }

        internal TransactionBillOtherBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data, HIS_PATIENT _hisPatient, HIS_PATIENT_PACKAGE _patientPackage)
            : base()
        {
            this.Module = module;
            this.treatmentId = data;
            this.hisPatient = _hisPatient;
            this.patientPackage = _patientPackage;
        }

        object ITransactionBillOther.Run() 
        {
            object result = null;
            try
            {
                if (this.hisPatient != null && this.patientPackage != null)
                {
                    result = new frmTransactionBillOther(Module, this.treatmentId.Value, this.hisPatient, this.patientPackage);
                }
                else if (this.treatmentId.HasValue && Module != null)
                {
                    result = new frmTransactionBillOther(Module, this.treatmentId.Value);
                }
                else
                {
                    result = new frmTransactionBillOther(Module);
                }
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData("treatmentId", treatmentId));
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
