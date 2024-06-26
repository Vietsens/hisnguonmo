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

namespace MPS.Processor.Mps000370.PDO
{
    public partial class Mps000370PDO : RDOBase
    {
        public const string printTypeCode = "Mps000370";

        public Mps000370PDO(V_HIS_TRANSACTION transaction,
            HIS_PATIENT patient,
            List<HIS_SERE_SERV> listSereServ,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter,
            List<V_HIS_SERE_SERV_DEBT> listSereServDebt,
            List<HIS_SERVICE_TYPE> listServiceType,
            List<HIS_DEBT_GOODS> listDebtGood)
        {
            this._Transaction = transaction;
            this._Patient = patient;
            this._ListSereServ = listSereServ;
            this._DepartmentTran = departmentTran;
            this._PatientTypeAlter = patientTypeAlter;
            this._listSereServDebt = listSereServDebt;
            this._listServiceType = listServiceType;
            this._listDebtGood = listDebtGood;
        }

        public List<HIS_DEBT_GOODS> _listDebtGood { get; set; }
    }
}
