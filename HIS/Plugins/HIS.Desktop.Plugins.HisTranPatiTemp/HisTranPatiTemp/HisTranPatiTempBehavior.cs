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

namespace HIS.Desktop.Plugins.HisTranPatiTemp
{
    class HisTranPatiTempBehavior : Tool<IDesktopToolContext>, IHisTranPatiTemp
    {
        object[] entity;

        Inventec.Desktop.Common.Modules.Module moduleData = null;
        HIS_TRAN_PATI_TEMP tranPatiTemp = null;
        internal HisTranPatiTempBehavior()
            : base()
        {

        }

        internal HisTranPatiTempBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IHisTranPatiTemp.Run()
        {
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        if (item is HIS_TRAN_PATI_TEMP)
                            tranPatiTemp = (HIS_TRAN_PATI_TEMP)item;
                    }
                }
                if (moduleData != null && tranPatiTemp != null)
                {
                    return new frmHisTranPatiTemp(moduleData, tranPatiTemp);
                }
                else if (moduleData != null)
                {
                    return new frmHisTranPatiTemp(moduleData, null);
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
