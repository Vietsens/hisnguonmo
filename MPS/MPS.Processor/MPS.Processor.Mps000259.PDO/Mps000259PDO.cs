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

namespace MPS.Processor.Mps000259.PDO
{
    public class Mps000259PDO : RDOBase
    {
        public const string printTypeCode = "Mps000259";

        public decimal _CanThu_Amount = 0;
        public List<V_HIS_SERE_SERV> listSereServ { get; set; }
        public V_HIS_TRANSACTION _Transaction { get; set; }
        public HIS_TREATMENT HisTreatment;
        public decimal depositAmpount = 0;
        public V_HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }
        public V_HIS_PATIENT _patient { get; set; }
        public string RatioText { get; set; }
        public List<V_HIS_SERE_SERV_BILL> listSereServBill { get; set; }
        public List<HIS_DEPARTMENT> ListDepartment { get; set; }
        public Mps000259ADO Mps000259ADO;
        public bool ShowExpend { get; set; }

        public Mps000259PDO() { }

        public Mps000259PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, HIS_TREATMENT _HisTreatment, decimal _depositAmpount, decimal ctAmount, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_PATIENT patient, string ratioText, Mps000259ADO _Mps000259ADO, List<HIS_DEPARTMENT> listDepartment)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
                this.depositAmpount = _depositAmpount;
                this.HisTreatment = _HisTreatment;
                this._patient = patient;
                this.RatioText = ratioText;
                this._PatientTypeAlter = patientTypeAlter;
                this.Mps000259ADO = _Mps000259ADO;
                this.ListDepartment = listDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000259PDO(V_HIS_TRANSACTION transaction, List<V_HIS_SERE_SERV> hisSereServs, List<V_HIS_SERE_SERV_BILL> hisSereServBills, HIS_TREATMENT _HisTreatment, decimal _depositAmpount, decimal ctAmount, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, V_HIS_PATIENT patient, string ratioText, Mps000259ADO _Mps000259ADO, List<HIS_DEPARTMENT> listDepartment)
        {
            try
            {
                this.listSereServ = hisSereServs;
                this._Transaction = transaction;
                this._CanThu_Amount = ctAmount;
                this.depositAmpount = _depositAmpount;
                this.HisTreatment = _HisTreatment;
                this._patient = patient;
                this.RatioText = ratioText;
                this._PatientTypeAlter = patientTypeAlter;
                this.listSereServBill = hisSereServBills;
                this.Mps000259ADO = _Mps000259ADO;
                this.ListDepartment = listDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class Mps000259ADO
    {
        public long PatientTypeBHYT { get; set; }
        public long PatientTypeVP { get; set; }
    }
}
