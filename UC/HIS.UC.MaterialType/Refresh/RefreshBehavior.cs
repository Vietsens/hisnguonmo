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
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.UC.MaterialType.Refresh
{
    public sealed class RefreshBehavior : IRefresh
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> MaterialTypes;
        public RefreshBehavior()
            : base()
        {
        }

        public RefreshBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> MaterialTypes)
            : base()
        {
            this.control = data;
            this.MaterialTypes = MaterialTypes;
        }

        void IRefresh.Run()
        {
            try
            {
                ((HIS.UC.MaterialType.Run.UCMaterialType)this.control).Refresh(MaterialTypes);                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
