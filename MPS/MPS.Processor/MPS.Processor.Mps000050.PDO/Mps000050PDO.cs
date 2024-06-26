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
using MPS.ProcessorBase.Core;
using MPS.ProcessorBase;
using MPS.Processor.Mps000050.PDO;
using MOS.SDO;

namespace MPS.Processor.Mps000050.PDO
{
    public partial class Mps000050PDO : RDOBase
    {
        public const string PrintTypeCode = "Mps000050";

        public Mps000050PDO() { }

        public Mps000050PDO(
            V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
            HIS_SERVICE_REQ vHisPrescription5,
            List<ExpMestMedicineSDO> expMestMedicines,
            Mps000050ADO mps000050ADO,
            HIS_TREATMENT _treatmeant
            )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000050ADO = mps000050ADO;
                this.hisTreatment = _treatmeant;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000050PDO(
           V_HIS_PATIENT_TYPE_ALTER vHisPatientTypeAlter,
           HIS_SERVICE_REQ vHisPrescription5,
           List<ExpMestMedicineSDO> expMestMedicines,
           Mps000050ADO mps000050ADO,
           HIS_TREATMENT _treatmeant,
               long? _KeyUseForm
           )
        {
            try
            {
                this.vHisPatientTypeAlter = vHisPatientTypeAlter;
                this.vHisPrescription5 = vHisPrescription5;
                this.expMestMedicines = expMestMedicines.ToList();
                this.Mps000050ADO = mps000050ADO;
                this.hisTreatment = _treatmeant;
                this.KeyUseForm = _KeyUseForm;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
