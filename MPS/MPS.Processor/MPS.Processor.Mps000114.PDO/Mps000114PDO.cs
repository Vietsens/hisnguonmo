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

namespace MPS.Processor.Mps000114.PDO
{
    public partial class Mps000114PDO : RDOBase
    {
        public const string printTypeCode = "Mps000114";

        public decimal _CanThu_Amount = 0;

        public Mps000114PDO() { }

        public Mps000114PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ctAmount)
        {
            //this._Bill = bill;
            this._Patient = patient;
            this._Transaction = transaction;
            this._CanThu_Amount = ctAmount;
        }

        //public Mps000114PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ctAmount)
        //{
        //    this._Patient = patient;
        //    this._Transaction = transaction;
        //    this._CanThu_Amount = ctAmount;
        //}

        public Mps000114PDO( V_HIS_PATIENT patient, decimal ctAmount)
        {
            //this._Bill = bill;
            this._Patient = patient;
            this._CanThu_Amount = ctAmount;
        }

        public Mps000114PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT patient, decimal ctAmount,V_HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            //this._Bill = bill;
            this._Patient = patient;
            this._Transaction = transaction;
            this._CanThu_Amount = ctAmount;
            this._PatientTypeAlter = patientTypeAlter;
        }
    }
}
