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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.MedicineType.Refresh
{
    public sealed class RefreshBehavior : IRefresh
    {
        UserControl control;
        List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> MedicineTypes;
        public RefreshBehavior()
            : base()
        {
        }

        public RefreshBehavior(CommonParam param, UserControl data, List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> MedicineTypes)
            : base()
        {
            this.control = data;
            this.MedicineTypes = MedicineTypes;
        }

        void IRefresh.Run()
        {
            try
            {
                ((HIS.UC.MedicineType.Run.UCMedicineType)this.control).Refresh(MedicineTypes);                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
