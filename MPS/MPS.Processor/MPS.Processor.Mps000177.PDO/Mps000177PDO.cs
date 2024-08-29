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

namespace MPS.Processor.Mps000177.PDO
{
    public partial class Mps000177PDO : RDOBase
    {
        public const string printTypeCode = "Mps000177";

        public Mps000177PDO(List<PatientADO> currentPatient,
            List<Mps000177DAY> Mps000177DAY,
            List<Mps000177MediMate> Mps000177MediMate,
            string departmentName,
            Dictionary<long, V_HIS_TREATMENT_BED_ROOM> bedRoomName
            )
        {
            try
            {
                this.currentPatient = currentPatient;
                this.Mps000177DAY = Mps000177DAY;
                this.Mps000177MediMate = Mps000177MediMate;
                this.departmentName = departmentName;
                this.bedRoomName = bedRoomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000177PDO(List<PatientADO> currentPatient,
    List<Mps000177DAY> Mps000177DAY,
    List<Mps000177MediMate> Mps000177MediMate,
    string departmentName,
    Dictionary<long, V_HIS_TREATMENT_BED_ROOM> bedRoomName,
    List<V_HIS_EXP_MEST_MEDICINE> emMedicine,
    List<V_HIS_EXP_MEST_MATERIAL> emMaterial,
    List<V_HIS_EXP_MEST_BLOOD> emBlood,
    long DaySize
    )
        {
            try
            {
                this.VExpMestMedicine = emMedicine;
                this.VExpMestMaterial = emMaterial;
                this.VExpMestBlood = emBlood;
                this.DaySize = DaySize;
                this.currentPatient = currentPatient;
                this.Mps000177DAY = Mps000177DAY;
                this.Mps000177MediMate = Mps000177MediMate;
                this.departmentName = departmentName;
                this.bedRoomName = bedRoomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
