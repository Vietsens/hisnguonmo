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

namespace MPS.Processor.Mps000268.PDO
{
    /// <summary>
    /// In yeu cau kham.
    /// Dau vao:
    /// PatyAlterBhyt: doi tuong the bhyt
    /// ServiceReq: yeu cau dich vu
    /// PatientType: doi tuong benh nhan
    /// </summary>
    /// 
    public partial class Mps000268PDO : RDOBase
    {
        public V_HIS_TREATMENT ViewTreatment { get; set; }
        public const string printTypeCode = "Mps000268";
        public Mps000268PDO() { }
        public Mps000268PDO(V_HIS_PATIENT patient, HIS_TREATMENT treatment, Mps000268ADO ado)
        {
            this.PatientView = patient;
            this.TreatmentView = treatment;
            this.ado = ado;
        }

        public Mps000268PDO(V_HIS_PATIENT patient, V_HIS_TREATMENT treatmentView, Mps000268ADO ado)
        {
            this.PatientView = patient;
            this.ViewTreatment = treatmentView;
            this.ado = ado;
        }
    }
}
