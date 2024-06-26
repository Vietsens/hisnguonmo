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
using HIS.Desktop.Plugins.BillTransferAccounting;
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

namespace HIS.Desktop.Plugins.BillTransferAccounting.BillTransfer
{   
    public sealed class BillTransferBehavior : Tool<IDesktopToolContext>, IBillTransfer
    {
        object[] entity;
        public BillTransferBehavior()
            : base()
        {

        }

        public BillTransferBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IBillTransfer.Run()
        {
            object result = null;
			Inventec.Desktop.Common.Modules.Module moduleData = null;
			BillTransferADO billTransferADO = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is BillTransferADO)
                        {
							billTransferADO = (BillTransferADO)(entity[i]);                            
                        }
						if (entity[i] is Inventec.Desktop.Common.Modules.Module)
						{
							moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
						}
                    }
					result = new frmBillTransferAccounting(moduleData, billTransferADO);
                }
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
