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

namespace MPS.Processor.Mps100006.PDO
{
    public partial class Mps100006PDO : RDOBase
    {
        public const string printTypeCode = "Mps100006";


        public Mps100006PDO(V_HIS_TRANSACTION transaction, HIS_PATIENT patient, List<HIS_BILL_FUND> listBillFund, List<HIS_SERE_SERV> listSereServ, V_HIS_DEPARTMENT_TRAN departmentTran, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, long patientTypeId)
        {
            this._Transaction = transaction;
            this._Patient = patient;
            this._ListBillFund = listBillFund;
            this._ListSereServ = listSereServ;
            this._DepartmentTran = departmentTran;
            this._PatientTypeAlter = patientTypeAlter;
            this._PatientTypeId = patientTypeId;
        }
        public Mps100006PDO(V_HIS_TRANSACTION transaction,  HIS_PATIENT patient, List<HIS_BILL_FUND> listBillFund, List<HIS_SERE_SERV> listSereServ, V_HIS_DEPARTMENT_TRAN departmentTran, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, long patientTypeId, string _KeyTP)
        {
            this._Transaction = transaction;
            this._Patient = patient;
            this._ListBillFund = listBillFund;
            this._ListSereServ = listSereServ;
            this._DepartmentTran = departmentTran;
            this._PatientTypeAlter = patientTypeAlter;
            this._PatientTypeId = patientTypeId;
            this.KeyThuPhi = _KeyTP;
        }
    }
}
