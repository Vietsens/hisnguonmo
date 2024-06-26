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

using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000200.ADO
{
    public class MedicineTypeAdo : V_HIS_MEDICINE_TYPE
    {
        public string PARENT_NAME { get; set; }

        public MedicineTypeAdo() { }

        public MedicineTypeAdo(V_HIS_MEDICINE_TYPE medicineType)
        {
            try
            {
                if (medicineType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeAdo>(this, medicineType);
                    if (medicineType.PARENT_ID.HasValue)
                    {
                        var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == medicineType.PARENT_ID.Value);
                        if (rs != null)
                        {
                            this.PARENT_NAME = rs.MEDICINE_TYPE_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
