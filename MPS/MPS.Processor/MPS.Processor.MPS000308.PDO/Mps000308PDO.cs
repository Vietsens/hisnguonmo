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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.Filter;

namespace MPS.Processor.MPS000308.PDO
{
    public class Mps000308PDO : RDOBase
    {
        public V_HIS_TREATMENT _Treatment { get; set; }
        public HIS_PATIENT _Patient { get; set; }
        public HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }
        public int _CountBaby { get; set; }
        public V_HIS_BABY _CurrentBaby { get; set; }

        public Mps000308PDO(V_HIS_TREATMENT treatment, V_HIS_BABY curentBaby, HIS_PATIENT patient, int countBaby)
        {
              
            this._CurrentBaby = curentBaby;
            this._CountBaby = countBaby;
            this._Treatment = treatment;
            this._Patient = patient;
        }
    }
}
