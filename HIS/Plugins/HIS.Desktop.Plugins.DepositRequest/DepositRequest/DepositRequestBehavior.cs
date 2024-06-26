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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.DepositRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.SDO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DepositRequest.DepositRequest
{
    public sealed class DepositRequestBehavior : Tool<IDesktopToolContext>, IDepositRequest
    {
        object[] entity;
       Inventec.Desktop.Common.Modules.Module Module;
       V_HIS_TREATMENT_BED_ROOM treatmentBedRoom;
        //long treatmentID;
       List<V_HIS_DEPOSIT_REQ> treatmentID;

        internal DepositRequestBehavior()
            : base()
        {
        }

        public DepositRequestBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
            //this.Module = module;
            //entity = data;
        }

        object IDepositRequest.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            Module = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        
                        //else if (item is long)
                        //{
                        //    treatmentID = (long)item;
                        //}
                        //if (Module != null && treatmentID > 0)
                        if (Module != null)
                        {
                            result = new UCDepositRequest(Module, treatmentID);
                            break;
                        }
                    }
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
