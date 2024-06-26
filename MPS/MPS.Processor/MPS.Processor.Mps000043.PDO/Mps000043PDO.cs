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

namespace MPS.Processor.Mps000043.PDO
{
    /// <summary>
    /// .
    /// </summary>
    public partial class Mps000043PDO : RDOBase
    {
        public Mps000043PDO(
           string resultCreateMedicine,
           List<MedicineExpmestTypeADO> lstMedicineExpmestTypeADO,
            HisPrescriptionSDO HisPrescriptionSDO,
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
           HIS_DEPARTMENT department,
            HIS_ICD dataIcd,
            HIS_DHST dhst,
            string bebRoomName,
            string mediStockExportName,
            V_HIS_TREATMENT treatment
            )
        {
            try
            {
                this.resultCreateMedicine = resultCreateMedicine;
                this.lstMedicineExpmestTypeADO = lstMedicineExpmestTypeADO;
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.department = department;
                this.bebRoomName = bebRoomName;
                this.HisPrescriptionSDO = HisPrescriptionSDO;
                this.dataIcd = dataIcd;
                this.dhst = dhst;
                this.mediStockExportName = mediStockExportName;
                this.treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
