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
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000284.PDO
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    /// 
    public partial class Mps000284PDO : RDOBase
    {
        public const string printTypeCode = "Mps000284";
        public V_HIS_TREATMENT Treatment { get; set; }

        public Mps000284PDO() { }

        public Mps000284PDO(V_HIS_PATIENT patient, V_HIS_ACCIDENT_HURT accidentHurt, V_HIS_TREATMENT treatment)
        {
            this.PatientView = patient;
            this.accidentHurt = accidentHurt;
            this.Treatment = treatment;
            
        }
    }
}
