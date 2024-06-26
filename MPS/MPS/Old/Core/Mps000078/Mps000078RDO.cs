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

using MPS;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPS.ADO;
using MOS.SDO;

namespace MPS.Core.Mps000078
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000078RDO : RDOBase
    {
        internal List<ImpMestMedicinePrintADO> MedicineImpMestTypeADOs { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_AGGR_IMP_MEST AggrImpMest { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_DEPARTMENT Department { get; set; }
        internal List<long> ServiceUnitIds { get; set; }
        internal List<long> UseFormIds { get; set; }
        internal List<long> RoomIds { get; set; }
        internal bool IsMedicine { get; set; }
        internal bool Ismaterial { get; set; }
        internal bool IsChemicalSustance { get; set; }
        internal string keyNameTitles;
        internal long HisImpMestSttId__Imported { get; set; }
        internal long HisImpMestSttId__Approved { get; set; }

        public Mps000078RDO(
            List<ImpMestMedicinePrintADO> medicineImpMestTypeADOs,
            MOS.EFMODEL.DataModels.V_HIS_AGGR_IMP_MEST aggrImpMest,
            MOS.EFMODEL.DataModels.HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            bool isChemicalSustance
            )
        {
            try
            {
                this.MedicineImpMestTypeADOs = medicineImpMestTypeADOs;
                this.AggrImpMest = aggrImpMest;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.IsChemicalSustance = isChemicalSustance;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000078RDO(
            List<ImpMestMedicinePrintADO> medicineImpMestTypeADOs,
            MOS.EFMODEL.DataModels.V_HIS_AGGR_IMP_MEST aggrImpMest,
            MOS.EFMODEL.DataModels.HIS_DEPARTMENT department,
            List<long> serviceUnitIds,
            List<long> useFormIds,
            List<long> roomIds,
            bool isMedicine,
            bool ismaterial,
            bool isChemicalSustance,
            string keyNameTitles,
         long HisImpMestSttId__Imported,
            long HisImpMestSttId__Approved
            )
        {
            try
            {
                this.MedicineImpMestTypeADOs = medicineImpMestTypeADOs;
                this.AggrImpMest = aggrImpMest;
                this.Department = department;
                this.ServiceUnitIds = serviceUnitIds;
                this.UseFormIds = useFormIds;
                this.RoomIds = roomIds;
                this.IsMedicine = isMedicine;
                this.Ismaterial = ismaterial;
                this.IsChemicalSustance = isChemicalSustance;
                this.keyNameTitles = keyNameTitles;
                this.HisImpMestSttId__Imported = HisImpMestSttId__Imported;
                this.HisImpMestSttId__Approved = HisImpMestSttId__Approved;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                keyValues.Add(new KeyValue(Mps000078ExtendSingleKey.KEY_NAME_TITLES, keyNameTitles));
                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.V_HIS_AGGR_IMP_MEST>(AggrImpMest, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(Department, keyValues, false);

                keyValues.Add(new KeyValue(Mps000078ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(AggrImpMest.CREATE_TIME ?? 0)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
