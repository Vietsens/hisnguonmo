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

namespace MPS.Processor.Mps000103.PDO
{
    public partial class Mps000103PDO : RDOBase
    {
        public List<V_HIS_SERE_SERV> _ListSereServ = null;

        public Mps000103PDO() { }

        public Mps000103PDO(V_HIS_PATIENT patient, List<V_HIS_SERE_SERV> sereServs, V_HIS_TRANSACTION transaction)
        {
            this._Patient = patient;
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
        }
        public Mps000103PDO(V_HIS_PATIENT patient, List<V_HIS_SERE_SERV> sereServs)
        {
            this._Patient = patient;
            this._ListSereServ = sereServs;
        }
        public Mps000103PDO(V_HIS_PATIENT patient, V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs)
        {
            this._Patient = patient;
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
        }

        public Mps000103PDO(V_HIS_PATIENT patient, V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> sereServs, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, string ratio_text)
        {
            this._Patient = patient;
            this._Transaction = transaction;
            this._ListSereServ = sereServs;
            this._PatientTypeAlter = patientTypeAlter;
            this.ratioText = ratio_text;
        }
    }
}
