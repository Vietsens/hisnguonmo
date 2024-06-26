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

namespace MPS.Processor.Mps000444.PDO
{
    public class Mps000444PDO : RDOBase
    {
        public V_HIS_PATIENT Patient { get; set; }
        public V_HIS_VACCINATION_EXAM VaccineExam { get; set; }
        public V_HIS_VITAMIN_A VitaminA { get; set; }
        public HIS_DHST Dhst { get; set; }

        public Mps000444PDO(V_HIS_PATIENT patient, V_HIS_VACCINATION_EXAM vaccineExam, V_HIS_VITAMIN_A vitaminA, HIS_DHST dhst)
        {
            this.Patient = patient;
            this.VaccineExam = vaccineExam;
            this.VitaminA = vitaminA;
            this.Dhst = dhst;
        }
    }
}
