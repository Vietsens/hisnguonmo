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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000248.PDO
{
    public class Mps000248PDO : RDOBase
    {
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_ADR _ADR { get; set; }
        public List<V_HIS_ADR_MEDICINE_TYPE> _MedicineIsAdrs { get; set; }
        public List<V_HIS_ADR_MEDICINE_TYPE> _Medicines { get; set; }
        public Mps000248PDO(
            HIS_TREATMENT _treatment,
            V_HIS_ADR _adr,
            List<V_HIS_ADR_MEDICINE_TYPE> _medicineIsAdrs,
            List<V_HIS_ADR_MEDICINE_TYPE> _medicines
            )
        {
            try
            {
                this._Treatment = _treatment;
                this._ADR = _adr;
                this._MedicineIsAdrs = _medicineIsAdrs;
                this._Medicines = _medicines;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
