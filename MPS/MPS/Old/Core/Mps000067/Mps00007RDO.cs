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

namespace MPS.Core.Mps000067
{
    /// <summary>
    /// .
    /// </summary>
    public class Mps000067RDO : RDOBase
    {
        internal PatientADO Patient { get; set; }
        internal MPS.ADO.ExeExpMestMedicineSDO expMestMedicines { get; set; }

        public Mps000067RDO(
            PatientADO patient,
            MPS.ADO.ExeExpMestMedicineSDO expMestMedicines
            )
        {
            try
            {
                this.Patient = patient;
                this.expMestMedicines = expMestMedicines;
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
                //if (expMestMedicines!=null)
                //{
                //    keyValues.Add(new KeyValue(Mps000067ExtendSingleKey.MEDICINE_TYPE_NAME, expMestMedicines.MEDICINE_TYPE_NAME));
                //    keyValues.Add(new KeyValue(Mps000067ExtendSingleKey.AMOUNT, expMestMedicines.AMOUNT));
                //    keyValues.Add(new KeyValue(Mps000067ExtendSingleKey.SERVICE_UNIT_NAME, expMestMedicines.SERVICE_UNIT_NAME));
                //    keyValues.Add(new KeyValue(Mps000067ExtendSingleKey.TUTORIAL, expMestMedicines.TUTORIAL));
                //}
                GlobalQuery.AddObjectKeyIntoListkey<ExeExpMestMedicineSDO>(expMestMedicines, keyValues, false);
                GlobalQuery.AddObjectKeyIntoListkey<PatientADO>(Patient, keyValues);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
