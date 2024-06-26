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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using AutoMapper;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DebateDiagnostic.DebateDiagnostic
{
    class DebateDiagnosticBehavior : Tool<IDesktopToolContext>, IDebateDiagnostic
    {
        object[] entity;

        internal DebateDiagnosticBehavior()
            : base()
        {

        }

        internal DebateDiagnosticBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IDebateDiagnostic.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            HIS.Desktop.ADO.TreatmentLogADO ado = null;
            MOS.EFMODEL.DataModels.HIS_DEBATE hisDebate = new MOS.EFMODEL.DataModels.HIS_DEBATE();
            MOS.EFMODEL.DataModels.HIS_SERVICE hisService = new MOS.EFMODEL.DataModels.HIS_SERVICE();
            MOS.EFMODEL.DataModels.HIS_SERVICE_REQ examServiceReq = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
            List<MOS.EFMODEL.DataModels.HIS_DEBATE> medicinePrints = null;
            long treatmentId = 0;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        if (item is HIS.Desktop.ADO.TreatmentLogADO)
                            ado = (HIS.Desktop.ADO.TreatmentLogADO)item;
                        if (item is MOS.EFMODEL.DataModels.HIS_DEBATE)
                            hisDebate = (MOS.EFMODEL.DataModels.HIS_DEBATE)item;
                        if (item is MOS.EFMODEL.DataModels.HIS_SERVICE_REQ)
                        {
                            examServiceReq = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ)item;
                        }
                        if (item is MOS.EFMODEL.DataModels.HIS_SERVICE)
                        {
                            hisService = (MOS.EFMODEL.DataModels.HIS_SERVICE)item;
                        }
                        if (item is MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)
                        {
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>();
                            examServiceReq = Mapper.Map<V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>((V_HIS_SERVICE_REQ)item);
                        }
                        if (item is List<MOS.EFMODEL.DataModels.HIS_DEBATE>)
                        {
                            medicinePrints = (List<MOS.EFMODEL.DataModels.HIS_DEBATE>)item;
                        }
                        if (item is long)
                        {
                            treatmentId = (long)item;
                        }
                    }
                }
                if (moduleData != null && ado != null)
                {
                    return new FormDebateDiagnostic(ado, moduleData, medicinePrints);
                }
                else if (moduleData != null && hisService != null && treatmentId != 0)
                {
                    return new FormDebateDiagnostic(hisService, medicinePrints, moduleData, treatmentId);
                }

                else if (moduleData != null && hisDebate != null && hisDebate.ID != 0)
                {

                    return new FormDebateDiagnostic(hisDebate, moduleData, medicinePrints);
                }

                else if (moduleData != null && examServiceReq != null)
                {
                    return new FormDebateDiagnostic(examServiceReq, medicinePrints, moduleData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
