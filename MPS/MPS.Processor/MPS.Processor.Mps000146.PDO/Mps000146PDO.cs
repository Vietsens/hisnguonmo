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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000146.PDO
{
    public partial class Mps000146PDO : RDOBase
    {
        public V_HIS_TREATMENT_BED_ROOM TreatmentBedRoom;
        public List<HIS_MEDICINE> lstHisMedicine;
        public List<V_HIS_MEDICINE_TYPE> lstHisMedicineViewType;
        public List<HIS_MIXED_MEDICINE> lstMixedMedicine;

        public Mps000146PDO() { }
        public Mps000146PDO(
            V_HIS_INFUSION_SUM infusionSum,
            V_HIS_TREATMENT_2 treatment,
            List<V_HIS_INFUSION> listInfusion,
            List<V_HIS_MEDICINE_TYPE> medicineTypes,
        V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom
            )
        {
            this._InfusionSum = infusionSum;
            this._ListInfusion = listInfusion;
            this._Treatment = treatment;
            this._ListMedicineType = medicineTypes;
            this.TreatmentBedRoom = _TreatmentBedRoom;

        }
        public Mps000146PDO(V_HIS_INFUSION_SUM infusionSum, V_HIS_TREATMENT_2 treatment, List<V_HIS_INFUSION> listInfusion, List<V_HIS_MEDICINE_TYPE> medicineTypes,
                            V_HIS_TREATMENT_BED_ROOM _TreatmentBedRoom, List<HIS_MIXED_MEDICINE> _lstMixedMedicine)
        {
            this._InfusionSum = infusionSum;
            this._ListInfusion = listInfusion;
            this._Treatment = treatment;
            this._ListMedicineType = medicineTypes;
            this.TreatmentBedRoom = _TreatmentBedRoom;
            this.lstMixedMedicine = _lstMixedMedicine;
        }
    }
}

