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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.Filter;

namespace HIS.Desktop.Plugins.ChangeExamRoomProcess.ChangeExamRoomProcess
{
    class ChangeExamRoomProcessBehavior : Tool<IDesktopToolContext>, IChangeExamRoomProcess
    {
        object[] entity;
        //L_HIS_SERVICE_REQ serReq;

        internal ChangeExamRoomProcessBehavior()
            : base()
        {

        }

        internal ChangeExamRoomProcessBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IChangeExamRoomProcess.Run()
        {
            //CommonParam param= new CommonParam();
            //HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
            //filter.TREATMENT_ID = 484;
            
            object result = null;
            //serReq = new BackendAdapter(param).Get<L_HIS_SERVICE_REQ>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, null);
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ servicerReq = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ)
                        {
                            servicerReq = (MOS.EFMODEL.DataModels.L_HIS_SERVICE_REQ)entity[i];
                        }
                        else if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            module = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                    }
                }
                result = new FormChangeExamRoomProcess(module, servicerReq);

                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
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
