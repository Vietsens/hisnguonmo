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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPaan.AssignPaan
{
    class AssignPaanBehavior : Tool<IDesktopToolContext>, IAssignPaan
    {
        long treatmentId = 0;
        long serviceReqId = 0;
        Inventec.Desktop.Common.Modules.Module Module;
        MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 _sereServ = null;
        HIS.UC.Icd.ADO.IcdInputADO icdAdo = null;
        HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo = null;
        MOS.EFMODEL.DataModels.HIS_TRACKING hisTracking = null;
        HIS.Desktop.Common.RefeshReference delegateActionSave = null;
        internal AssignPaanBehavior()
            : base()
        {

        }

        internal AssignPaanBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, long data, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, long serviceReqId, MOS.EFMODEL.DataModels.HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : base()
        {
            this.Module = module;
            this.treatmentId = data;
            this.icdAdo = icdAdo;
            this.secondIcdAdo = secondIcdAdo;
            this.serviceReqId = serviceReqId;
            this.hisTracking = hisTracking;
            this.delegateActionSave = delegateActionSave;
        }

        internal AssignPaanBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 data, HIS.UC.Icd.ADO.IcdInputADO icdAdo, HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO secondIcdAdo, long serviceReqId, MOS.EFMODEL.DataModels.HIS_TRACKING hisTracking, HIS.Desktop.Common.RefeshReference delegateActionSave)
            : base()
        {
            this.Module = module;
            this._sereServ = data;
            this.icdAdo = icdAdo;
            this.secondIcdAdo = secondIcdAdo;
            this.serviceReqId = serviceReqId;
            this.hisTracking = hisTracking;
            this.delegateActionSave = delegateActionSave;
        }

        object IAssignPaan.Run()
        {
            object result = null;
            try
            {
                if (treatmentId > 0)
                {
                    result = new frmAssignPaan(Module, treatmentId, icdAdo, secondIcdAdo, serviceReqId, hisTracking, delegateActionSave);
                }
                else if (_sereServ != null)
                {
                    result = new frmAssignPaan(Module, _sereServ, icdAdo, secondIcdAdo, serviceReqId, hisTracking, delegateActionSave);
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
