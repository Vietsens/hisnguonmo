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

using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000200.ADO
{
    /// <summary>
    /// Catalog lookups built once per export to avoid per-row cache scans.
    /// </summary>
    public class MedicineTypeLookup
    {
        public Dictionary<long, V_HIS_MEDICINE_TYPE> ParentById { get; private set; }
        public ILookup<long, V_HIS_MEDICINE_TYPE_ACIN> AcinByMedicineTypeId { get; private set; }
        public Dictionary<string, HIS_ATC> AtcByCode { get; private set; }
        public Dictionary<long, HIS_SUPPLIER> SupplierById { get; private set; }
        public Dictionary<long, HIS_CONTRAINDICATION> ContraindicationById { get; private set; }

        public MedicineTypeLookup()
        {
            try
            {
                this.ParentById = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>()
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
                this.AcinByMedicineTypeId = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE_ACIN>()
                    .ToLookup(o => o.MEDICINE_TYPE_ID);
                this.AtcByCode = BackendDataWorker.Get<HIS_ATC>()
                    .Where(o => !String.IsNullOrEmpty(o.ATC_CODE))
                    .GroupBy(o => o.ATC_CODE).ToDictionary(g => g.Key, g => g.First());
                this.SupplierById = BackendDataWorker.Get<HIS_SUPPLIER>()
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
                this.ContraindicationById = BackendDataWorker.Get<HIS_CONTRAINDICATION>()
                    .GroupBy(o => o.ID).ToDictionary(g => g.Key, g => g.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
